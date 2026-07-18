using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Net
{
	[ComVisible(true)]
	public class WebClient : Component
	{
		private static readonly string urlEncodedCType;

		private static byte[] hexBytes;

		private ICredentials credentials;

		private WebHeaderCollection headers;

		private WebHeaderCollection responseHeaders;

		private Uri baseAddress;

		private string baseString;

		private NameValueCollection queryString;

		private bool is_busy;

		private bool async;

		private Thread async_thread;

		private Encoding encoding = Encoding.Default;

		private IWebProxy proxy;

		public string BaseAddress
		{
			get
			{
				if (baseString == null && baseAddress == null)
				{
					return string.Empty;
				}
				baseString = baseAddress.ToString();
				return baseString;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					baseAddress = null;
				}
				else
				{
					baseAddress = new Uri(value);
				}
			}
		}

		[System.MonoTODO]
		public RequestCachePolicy CachePolicy
		{
			get
			{
				throw GetMustImplement();
			}
			set
			{
				throw GetMustImplement();
			}
		}

		[System.MonoTODO]
		public bool UseDefaultCredentials
		{
			get
			{
				throw GetMustImplement();
			}
			set
			{
				throw GetMustImplement();
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return credentials;
			}
			set
			{
				credentials = value;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				if (headers == null)
				{
					headers = new WebHeaderCollection();
				}
				return headers;
			}
			set
			{
				headers = value;
			}
		}

		public NameValueCollection QueryString
		{
			get
			{
				if (queryString == null)
				{
					queryString = new NameValueCollection();
				}
				return queryString;
			}
			set
			{
				queryString = value;
			}
		}

		public WebHeaderCollection ResponseHeaders
		{
			get
			{
				return responseHeaders;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Encoding");
				}
				encoding = value;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				return proxy;
			}
			set
			{
				proxy = value;
			}
		}

		public bool IsBusy
		{
			get
			{
				return is_busy;
			}
		}

		public event DownloadDataCompletedEventHandler DownloadDataCompleted;

		public event AsyncCompletedEventHandler DownloadFileCompleted;

		public event DownloadProgressChangedEventHandler DownloadProgressChanged;

		public event DownloadStringCompletedEventHandler DownloadStringCompleted;

		public event OpenReadCompletedEventHandler OpenReadCompleted;

		public event OpenWriteCompletedEventHandler OpenWriteCompleted;

		public event UploadDataCompletedEventHandler UploadDataCompleted;

		public event UploadFileCompletedEventHandler UploadFileCompleted;

		public event UploadProgressChangedEventHandler UploadProgressChanged;

		public event UploadStringCompletedEventHandler UploadStringCompleted;

		public event UploadValuesCompletedEventHandler UploadValuesCompleted;

		static WebClient()
		{
			urlEncodedCType = "application/x-www-form-urlencoded";
			hexBytes = new byte[16];
			int num = 0;
			int num2 = 48;
			while (num2 <= 57)
			{
				hexBytes[num] = (byte)num2;
				num2++;
				num++;
			}
			int num3 = 97;
			while (num3 <= 102)
			{
				hexBytes[num] = (byte)num3;
				num3++;
				num++;
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		private void CheckBusy()
		{
			if (IsBusy)
			{
				throw new NotSupportedException("WebClient does not support conccurent I/O operations.");
			}
		}

		private void SetBusy()
		{
			lock (this)
			{
				CheckBusy();
				is_busy = true;
			}
		}

		public byte[] DownloadData(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return DownloadData(CreateUri(address));
		}

		public byte[] DownloadData(Uri address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			try
			{
				SetBusy();
				async = false;
				return DownloadDataCore(address, null);
			}
			finally
			{
				is_busy = false;
			}
		}

		private byte[] DownloadDataCore(Uri address, object userToken)
		{
			WebRequest webRequest = null;
			try
			{
				webRequest = SetupRequest(address);
				WebResponse webResponse = GetWebResponse(webRequest);
				Stream responseStream = webResponse.GetResponseStream();
				return ReadAll(responseStream, (int)webResponse.ContentLength, userToken);
			}
			catch (ThreadInterruptedException)
			{
				if (webRequest != null)
				{
					webRequest.Abort();
				}
				throw;
			}
			catch (WebException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new WebException("An error occurred performing a WebClient request.", innerException);
			}
		}

		public void DownloadFile(string address, string fileName)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			DownloadFile(CreateUri(address), fileName);
		}

		public void DownloadFile(Uri address, string fileName)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			try
			{
				SetBusy();
				async = false;
				DownloadFileCore(address, fileName, null);
			}
			catch (WebException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new WebException("An error occurred performing a WebClient request.", innerException);
			}
			finally
			{
				is_busy = false;
			}
		}

		private void DownloadFileCore(Uri address, string fileName, object userToken)
		{
			WebRequest webRequest = null;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
			{
				try
				{
					webRequest = SetupRequest(address);
					WebResponse webResponse = GetWebResponse(webRequest);
					Stream responseStream = webResponse.GetResponseStream();
					int num = (int)webResponse.ContentLength;
					int num2 = ((num > -1 && num <= 32768) ? num : 32768);
					byte[] array = new byte[num2];
					int num3 = 0;
					long num4 = 0L;
					while ((num3 = responseStream.Read(array, 0, num2)) != 0)
					{
						if (async)
						{
							num4 += num3;
							OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(num4, webResponse.ContentLength, userToken));
						}
						fileStream.Write(array, 0, num3);
					}
				}
				catch (ThreadInterruptedException)
				{
					if (webRequest != null)
					{
						webRequest.Abort();
					}
					throw;
				}
			}
		}

		public Stream OpenRead(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return OpenRead(CreateUri(address));
		}

		public Stream OpenRead(Uri address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			WebRequest webRequest = null;
			try
			{
				SetBusy();
				async = false;
				webRequest = SetupRequest(address);
				WebResponse webResponse = GetWebResponse(webRequest);
				return webResponse.GetResponseStream();
			}
			catch (WebException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new WebException("An error occurred performing a WebClient request.", innerException);
			}
			finally
			{
				is_busy = false;
			}
		}

		public Stream OpenWrite(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return OpenWrite(CreateUri(address));
		}

		public Stream OpenWrite(string address, string method)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return OpenWrite(CreateUri(address), method);
		}

		public Stream OpenWrite(Uri address)
		{
			return OpenWrite(address, null);
		}

		public Stream OpenWrite(Uri address, string method)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			try
			{
				SetBusy();
				async = false;
				WebRequest webRequest = SetupRequest(address, method, true);
				return webRequest.GetRequestStream();
			}
			catch (WebException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new WebException("An error occurred performing a WebClient request.", innerException);
			}
			finally
			{
				is_busy = false;
			}
		}

		private string DetermineMethod(Uri address, string method, bool is_upload)
		{
			if (method != null)
			{
				return method;
			}
			if (address.Scheme == Uri.UriSchemeFtp)
			{
				return (!is_upload) ? "RETR" : "STOR";
			}
			return (!is_upload) ? "GET" : "POST";
		}

		public byte[] UploadData(string address, byte[] data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return UploadData(CreateUri(address), data);
		}

		public byte[] UploadData(string address, string method, byte[] data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return UploadData(CreateUri(address), method, data);
		}

		public byte[] UploadData(Uri address, byte[] data)
		{
			return UploadData(address, null, data);
		}

		public byte[] UploadData(Uri address, string method, byte[] data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			try
			{
				SetBusy();
				async = false;
				return UploadDataCore(address, method, data, null);
			}
			catch (WebException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new WebException("An error occurred performing a WebClient request.", innerException);
			}
			finally
			{
				is_busy = false;
			}
		}

		private byte[] UploadDataCore(Uri address, string method, byte[] data, object userToken)
		{
			WebRequest webRequest = SetupRequest(address, method, true);
			try
			{
				int num = data.Length;
				webRequest.ContentLength = num;
				using (Stream stream = webRequest.GetRequestStream())
				{
					stream.Write(data, 0, num);
				}
				WebResponse webResponse = GetWebResponse(webRequest);
				Stream responseStream = webResponse.GetResponseStream();
				return ReadAll(responseStream, (int)webResponse.ContentLength, userToken);
			}
			catch (ThreadInterruptedException)
			{
				if (webRequest != null)
				{
					webRequest.Abort();
				}
				throw;
			}
		}

		public byte[] UploadFile(string address, string fileName)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return UploadFile(CreateUri(address), fileName);
		}

		public byte[] UploadFile(Uri address, string fileName)
		{
			return UploadFile(address, null, fileName);
		}

		public byte[] UploadFile(string address, string method, string fileName)
		{
			return UploadFile(CreateUri(address), method, fileName);
		}

		public byte[] UploadFile(Uri address, string method, string fileName)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			try
			{
				SetBusy();
				async = false;
				return UploadFileCore(address, method, fileName, null);
			}
			catch (WebException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new WebException("An error occurred performing a WebClient request.", innerException);
			}
			finally
			{
				is_busy = false;
			}
		}

		private byte[] UploadFileCore(Uri address, string method, string fileName, object userToken)
		{
			string text = Headers["Content-Type"];
			if (text != null)
			{
				string text2 = text.ToLower();
				if (text2.StartsWith("multipart/"))
				{
					throw new WebException("Content-Type cannot be set to a multipart type for this request.");
				}
			}
			else
			{
				text = "application/octet-stream";
			}
			string text3 = "------------" + DateTime.Now.Ticks.ToString("x");
			Headers["Content-Type"] = string.Format("multipart/form-data; boundary={0}", text3);
			Stream stream = null;
			Stream stream2 = null;
			byte[] array = null;
			fileName = Path.GetFullPath(fileName);
			WebRequest webRequest = null;
			try
			{
				stream2 = File.OpenRead(fileName);
				webRequest = SetupRequest(address, method, true);
				stream = webRequest.GetRequestStream();
				byte[] bytes = Encoding.ASCII.GetBytes("--" + text3 + "\r\n");
				stream.Write(bytes, 0, bytes.Length);
				string s = string.Format("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n", Path.GetFileName(fileName), text);
				byte[] bytes2 = Encoding.UTF8.GetBytes(s);
				stream.Write(bytes2, 0, bytes2.Length);
				byte[] buffer = new byte[4096];
				int count;
				while ((count = stream2.Read(buffer, 0, 4096)) != 0)
				{
					stream.Write(buffer, 0, count);
				}
				stream.WriteByte(13);
				stream.WriteByte(10);
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
				stream = null;
				WebResponse webResponse = GetWebResponse(webRequest);
				Stream responseStream = webResponse.GetResponseStream();
				return ReadAll(responseStream, (int)webResponse.ContentLength, userToken);
			}
			catch (ThreadInterruptedException)
			{
				if (webRequest != null)
				{
					webRequest.Abort();
				}
				throw;
			}
			finally
			{
				if (stream2 != null)
				{
					stream2.Close();
				}
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		public byte[] UploadValues(string address, NameValueCollection data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return UploadValues(CreateUri(address), data);
		}

		public byte[] UploadValues(string address, string method, NameValueCollection data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return UploadValues(CreateUri(address), method, data);
		}

		public byte[] UploadValues(Uri address, NameValueCollection data)
		{
			return UploadValues(address, null, data);
		}

		public byte[] UploadValues(Uri address, string method, NameValueCollection data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			try
			{
				SetBusy();
				async = false;
				return UploadValuesCore(address, method, data, null);
			}
			catch (WebException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new WebException("An error occurred performing a WebClient request.", innerException);
			}
			finally
			{
				is_busy = false;
			}
		}

		private byte[] UploadValuesCore(Uri uri, string method, NameValueCollection data, object userToken)
		{
			string text = Headers["Content-Type"];
			if (text != null && string.Compare(text, urlEncodedCType, true) != 0)
			{
				throw new WebException("Content-Type header cannot be changed from its default value for this request.");
			}
			Headers["Content-Type"] = urlEncodedCType;
			WebRequest webRequest = SetupRequest(uri, method, true);
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				foreach (string datum in data)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(datum);
					UrlEncodeAndWrite(memoryStream, bytes);
					memoryStream.WriteByte(61);
					bytes = Encoding.UTF8.GetBytes(data[datum]);
					UrlEncodeAndWrite(memoryStream, bytes);
					memoryStream.WriteByte(38);
				}
				int num = (int)memoryStream.Length;
				if (num > 0)
				{
					memoryStream.SetLength(--num);
				}
				byte[] buffer = memoryStream.GetBuffer();
				webRequest.ContentLength = num;
				using (Stream stream = webRequest.GetRequestStream())
				{
					stream.Write(buffer, 0, num);
				}
				memoryStream.Close();
				WebResponse webResponse = GetWebResponse(webRequest);
				Stream responseStream = webResponse.GetResponseStream();
				return ReadAll(responseStream, (int)webResponse.ContentLength, userToken);
			}
			catch (ThreadInterruptedException)
			{
				webRequest.Abort();
				throw;
			}
		}

		public string DownloadString(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return encoding.GetString(DownloadData(CreateUri(address)));
		}

		public string DownloadString(Uri address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return encoding.GetString(DownloadData(CreateUri(address)));
		}

		public string UploadString(string address, string data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] bytes = UploadData(address, encoding.GetBytes(data));
			return encoding.GetString(bytes);
		}

		public string UploadString(string address, string method, string data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] bytes = UploadData(address, method, encoding.GetBytes(data));
			return encoding.GetString(bytes);
		}

		public string UploadString(Uri address, string data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] bytes = UploadData(address, encoding.GetBytes(data));
			return encoding.GetString(bytes);
		}

		public string UploadString(Uri address, string method, string data)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] bytes = UploadData(address, method, encoding.GetBytes(data));
			return encoding.GetString(bytes);
		}

		private Uri CreateUri(string address)
		{
			return MakeUri(address);
		}

		private Uri CreateUri(Uri address)
		{
			string query = address.Query;
			if (string.IsNullOrEmpty(query))
			{
				query = GetQueryString(true);
			}
			if (baseAddress == null && query == null)
			{
				return address;
			}
			if (baseAddress == null)
			{
				return new Uri(address.ToString() + query, query != null);
			}
			if (query == null)
			{
				return new Uri(baseAddress, address.ToString());
			}
			return new Uri(baseAddress, address.ToString() + query, query != null);
		}

		private string GetQueryString(bool add_qmark)
		{
			if (queryString == null || queryString.Count == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (add_qmark)
			{
				stringBuilder.Append('?');
			}
			foreach (string item in queryString)
			{
				stringBuilder.AppendFormat("{0}={1}&", item, UrlEncode(queryString[item]));
			}
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Length--;
			}
			if (stringBuilder.Length == 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		private Uri MakeUri(string path)
		{
			string text = GetQueryString(true);
			if (baseAddress == null && text == null)
			{
				try
				{
					return new Uri(path);
				}
				catch (ArgumentNullException)
				{
					if (Environment.UnityWebSecurityEnabled)
					{
						throw;
					}
					path = Path.GetFullPath(path);
					return new Uri("file://" + path);
				}
				catch (UriFormatException)
				{
					if (Environment.UnityWebSecurityEnabled)
					{
						throw;
					}
					path = Path.GetFullPath(path);
					return new Uri("file://" + path);
				}
			}
			if (baseAddress == null)
			{
				return new Uri(path + text, text != null);
			}
			if (text == null)
			{
				return new Uri(baseAddress, path);
			}
			return new Uri(baseAddress, path + text, text != null);
		}

		private WebRequest SetupRequest(Uri uri)
		{
			WebRequest webRequest = GetWebRequest(uri);
			if (Proxy != null)
			{
				webRequest.Proxy = Proxy;
			}
			webRequest.Credentials = credentials;
			if (headers != null && headers.Count != 0 && webRequest is HttpWebRequest)
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)webRequest;
				string text = headers["Expect"];
				string text2 = headers["Content-Type"];
				string text3 = headers["Accept"];
				string text4 = headers["Connection"];
				string text5 = headers["User-Agent"];
				string text6 = headers["Referer"];
				headers.RemoveInternal("Expect");
				headers.RemoveInternal("Content-Type");
				headers.RemoveInternal("Accept");
				headers.RemoveInternal("Connection");
				headers.RemoveInternal("Referer");
				headers.RemoveInternal("User-Agent");
				webRequest.Headers = headers;
				if (text != null && text.Length > 0)
				{
					httpWebRequest.Expect = text;
				}
				if (text3 != null && text3.Length > 0)
				{
					httpWebRequest.Accept = text3;
				}
				if (text2 != null && text2.Length > 0)
				{
					httpWebRequest.ContentType = text2;
				}
				if (text4 != null && text4.Length > 0)
				{
					httpWebRequest.Connection = text4;
				}
				if (text5 != null && text5.Length > 0)
				{
					httpWebRequest.UserAgent = text5;
				}
				if (text6 != null && text6.Length > 0)
				{
					httpWebRequest.Referer = text6;
				}
			}
			responseHeaders = null;
			return webRequest;
		}

		private WebRequest SetupRequest(Uri uri, string method, bool is_upload)
		{
			WebRequest webRequest = SetupRequest(uri);
			webRequest.Method = DetermineMethod(uri, method, is_upload);
			return webRequest;
		}

		private byte[] ReadAll(Stream stream, int length, object userToken)
		{
			MemoryStream memoryStream = null;
			bool flag = length == -1;
			int num = ((!flag) ? length : 8192);
			if (flag)
			{
				memoryStream = new MemoryStream();
			}
			int num2 = 0;
			int num3 = 0;
			byte[] array = new byte[num];
			while ((num2 = stream.Read(array, num3, num)) != 0)
			{
				if (flag)
				{
					memoryStream.Write(array, 0, num2);
				}
				else
				{
					num3 += num2;
					num -= num2;
				}
				if (async)
				{
					OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(num2, length, userToken));
				}
			}
			if (flag)
			{
				return memoryStream.ToArray();
			}
			return array;
		}

		private string UrlEncode(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int length = str.Length;
			for (int i = 0; i < length; i++)
			{
				char c = str[i];
				if (c == ' ')
				{
					stringBuilder.Append('+');
				}
				else if ((c < '0' && c != '-' && c != '.') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a' && c != '_') || c > 'z')
				{
					stringBuilder.Append('%');
					int num = (int)c >> 4;
					stringBuilder.Append((char)hexBytes[num]);
					num = c & 0xF;
					stringBuilder.Append((char)hexBytes[num]);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private static void UrlEncodeAndWrite(Stream stream, byte[] bytes)
		{
			if (bytes == null)
			{
				return;
			}
			int num = bytes.Length;
			if (num == 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				char c = (char)bytes[i];
				if (c == ' ')
				{
					stream.WriteByte(43);
				}
				else if ((c < '0' && c != '-' && c != '.') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a' && c != '_') || c > 'z')
				{
					stream.WriteByte(37);
					int num2 = (int)c >> 4;
					stream.WriteByte(hexBytes[num2]);
					num2 = c & 0xF;
					stream.WriteByte(hexBytes[num2]);
				}
				else
				{
					stream.WriteByte((byte)c);
				}
			}
		}

		public void CancelAsync()
		{
			lock (this)
			{
				if (async_thread != null)
				{
					Thread thread = async_thread;
					CompleteAsync();
					thread.Interrupt();
				}
			}
		}

		private void CompleteAsync()
		{
			lock (this)
			{
				is_busy = false;
				async_thread = null;
			}
		}

		public void DownloadDataAsync(Uri address)
		{
			DownloadDataAsync(address, null);
		}

		public void DownloadDataAsync(Uri address, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			lock (this)
			{
				SetBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					try
					{
						byte[] result = DownloadDataCore((Uri)array[0], array[1]);
						OnDownloadDataCompleted(new DownloadDataCompletedEventArgs(result, null, false, array[1]));
					}
					catch (ThreadInterruptedException)
					{
						OnDownloadDataCompleted(new DownloadDataCompletedEventArgs(null, null, true, array[1]));
						throw;
					}
					catch (Exception error)
					{
						OnDownloadDataCompleted(new DownloadDataCompletedEventArgs(null, error, false, array[1]));
					}
				});
				object[] parameter = new object[2] { address, userToken };
				async_thread.Start(parameter);
			}
		}

		public void DownloadFileAsync(Uri address, string fileName)
		{
			DownloadFileAsync(address, fileName, null);
		}

		public void DownloadFileAsync(Uri address, string fileName, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			lock (this)
			{
				SetBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					try
					{
						DownloadFileCore((Uri)array[0], (string)array[1], array[2]);
						OnDownloadFileCompleted(new AsyncCompletedEventArgs(null, false, array[2]));
					}
					catch (ThreadInterruptedException)
					{
						OnDownloadFileCompleted(new AsyncCompletedEventArgs(null, true, array[2]));
					}
					catch (Exception error)
					{
						OnDownloadFileCompleted(new AsyncCompletedEventArgs(error, false, array[2]));
					}
				});
				object[] parameter = new object[3] { address, fileName, userToken };
				async_thread.Start(parameter);
			}
		}

		public void DownloadStringAsync(Uri address)
		{
			DownloadStringAsync(address, null);
		}

		public void DownloadStringAsync(Uri address, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			lock (this)
			{
				SetBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					try
					{
						string result = encoding.GetString(DownloadDataCore((Uri)array[0], array[1]));
						OnDownloadStringCompleted(new DownloadStringCompletedEventArgs(result, null, false, array[1]));
					}
					catch (ThreadInterruptedException)
					{
						OnDownloadStringCompleted(new DownloadStringCompletedEventArgs(null, null, true, array[1]));
					}
					catch (Exception error)
					{
						OnDownloadStringCompleted(new DownloadStringCompletedEventArgs(null, error, false, array[1]));
					}
				});
				object[] parameter = new object[2] { address, userToken };
				async_thread.Start(parameter);
			}
		}

		public void OpenReadAsync(Uri address)
		{
			OpenReadAsync(address, null);
		}

		public void OpenReadAsync(Uri address, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			lock (this)
			{
				SetBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					WebRequest webRequest = null;
					try
					{
						webRequest = SetupRequest((Uri)array[0]);
						WebResponse webResponse = GetWebResponse(webRequest);
						Stream responseStream = webResponse.GetResponseStream();
						OnOpenReadCompleted(new OpenReadCompletedEventArgs(responseStream, null, false, array[1]));
					}
					catch (ThreadInterruptedException)
					{
						if (webRequest != null)
						{
							webRequest.Abort();
						}
						OnOpenReadCompleted(new OpenReadCompletedEventArgs(null, null, true, array[1]));
					}
					catch (Exception error)
					{
						OnOpenReadCompleted(new OpenReadCompletedEventArgs(null, error, false, array[1]));
					}
				});
				object[] parameter = new object[2] { address, userToken };
				async_thread.Start(parameter);
			}
		}

		public void OpenWriteAsync(Uri address)
		{
			OpenWriteAsync(address, null);
		}

		public void OpenWriteAsync(Uri address, string method)
		{
			OpenWriteAsync(address, method, null);
		}

		public void OpenWriteAsync(Uri address, string method, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			lock (this)
			{
				SetBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					WebRequest webRequest = null;
					try
					{
						webRequest = SetupRequest((Uri)array[0], (string)array[1], true);
						Stream requestStream = webRequest.GetRequestStream();
						OnOpenWriteCompleted(new OpenWriteCompletedEventArgs(requestStream, null, false, array[2]));
					}
					catch (ThreadInterruptedException)
					{
						if (webRequest != null)
						{
							webRequest.Abort();
						}
						OnOpenWriteCompleted(new OpenWriteCompletedEventArgs(null, null, true, array[2]));
					}
					catch (Exception error)
					{
						OnOpenWriteCompleted(new OpenWriteCompletedEventArgs(null, error, false, array[2]));
					}
				});
				object[] parameter = new object[3] { address, method, userToken };
				async_thread.Start(parameter);
			}
		}

		public void UploadDataAsync(Uri address, byte[] data)
		{
			UploadDataAsync(address, null, data);
		}

		public void UploadDataAsync(Uri address, string method, byte[] data)
		{
			UploadDataAsync(address, method, data, null);
		}

		public void UploadDataAsync(Uri address, string method, byte[] data, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			lock (this)
			{
				SetBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					try
					{
						byte[] result = UploadDataCore((Uri)array[0], (string)array[1], (byte[])array[2], array[3]);
						OnUploadDataCompleted(new UploadDataCompletedEventArgs(result, null, false, array[3]));
					}
					catch (ThreadInterruptedException)
					{
						OnUploadDataCompleted(new UploadDataCompletedEventArgs(null, null, true, array[3]));
					}
					catch (Exception error)
					{
						OnUploadDataCompleted(new UploadDataCompletedEventArgs(null, error, false, array[3]));
					}
				});
				object[] parameter = new object[4] { address, method, data, userToken };
				async_thread.Start(parameter);
			}
		}

		public void UploadFileAsync(Uri address, string fileName)
		{
			UploadFileAsync(address, null, fileName);
		}

		public void UploadFileAsync(Uri address, string method, string fileName)
		{
			UploadFileAsync(address, method, fileName, null);
		}

		public void UploadFileAsync(Uri address, string method, string fileName, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			lock (this)
			{
				SetBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					try
					{
						byte[] result = UploadFileCore((Uri)array[0], (string)array[1], (string)array[2], array[3]);
						OnUploadFileCompleted(new UploadFileCompletedEventArgs(result, null, false, array[3]));
					}
					catch (ThreadInterruptedException)
					{
						OnUploadFileCompleted(new UploadFileCompletedEventArgs(null, null, true, array[3]));
					}
					catch (Exception error)
					{
						OnUploadFileCompleted(new UploadFileCompletedEventArgs(null, error, false, array[3]));
					}
				});
				object[] parameter = new object[4] { address, method, fileName, userToken };
				async_thread.Start(parameter);
			}
		}

		public void UploadStringAsync(Uri address, string data)
		{
			UploadStringAsync(address, null, data);
		}

		public void UploadStringAsync(Uri address, string method, string data)
		{
			UploadStringAsync(address, method, data, null);
		}

		public void UploadStringAsync(Uri address, string method, string data, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			lock (this)
			{
				CheckBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					try
					{
						string result = UploadString((Uri)array[0], (string)array[1], (string)array[2]);
						OnUploadStringCompleted(new UploadStringCompletedEventArgs(result, null, false, array[3]));
					}
					catch (ThreadInterruptedException)
					{
						OnUploadStringCompleted(new UploadStringCompletedEventArgs(null, null, true, array[3]));
					}
					catch (Exception error)
					{
						OnUploadStringCompleted(new UploadStringCompletedEventArgs(null, error, false, array[3]));
					}
				});
				object[] parameter = new object[4] { address, method, data, userToken };
				async_thread.Start(parameter);
			}
		}

		public void UploadValuesAsync(Uri address, NameValueCollection values)
		{
			UploadValuesAsync(address, null, values);
		}

		public void UploadValuesAsync(Uri address, string method, NameValueCollection values)
		{
			UploadValuesAsync(address, method, values, null);
		}

		public void UploadValuesAsync(Uri address, string method, NameValueCollection values, object userToken)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			lock (this)
			{
				CheckBusy();
				async = true;
				async_thread = new Thread(delegate(object state)
				{
					object[] array = (object[])state;
					try
					{
						byte[] result = UploadValuesCore((Uri)array[0], (string)array[1], (NameValueCollection)array[2], array[3]);
						OnUploadValuesCompleted(new UploadValuesCompletedEventArgs(result, null, false, array[3]));
					}
					catch (ThreadInterruptedException)
					{
						OnUploadValuesCompleted(new UploadValuesCompletedEventArgs(null, null, true, array[3]));
					}
					catch (Exception error)
					{
						OnUploadValuesCompleted(new UploadValuesCompletedEventArgs(null, error, false, array[3]));
					}
				});
				object[] parameter = new object[4] { address, method, values, userToken };
				async_thread.Start(parameter);
			}
		}

		protected virtual void OnDownloadDataCompleted(DownloadDataCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.DownloadDataCompleted != null)
			{
				this.DownloadDataCompleted(this, args);
			}
		}

		protected virtual void OnDownloadFileCompleted(AsyncCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.DownloadFileCompleted != null)
			{
				this.DownloadFileCompleted(this, args);
			}
		}

		protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
		{
			if (this.DownloadProgressChanged != null)
			{
				this.DownloadProgressChanged(this, e);
			}
		}

		protected virtual void OnDownloadStringCompleted(DownloadStringCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.DownloadStringCompleted != null)
			{
				this.DownloadStringCompleted(this, args);
			}
		}

		protected virtual void OnOpenReadCompleted(OpenReadCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.OpenReadCompleted != null)
			{
				this.OpenReadCompleted(this, args);
			}
		}

		protected virtual void OnOpenWriteCompleted(OpenWriteCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.OpenWriteCompleted != null)
			{
				this.OpenWriteCompleted(this, args);
			}
		}

		protected virtual void OnUploadDataCompleted(UploadDataCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.UploadDataCompleted != null)
			{
				this.UploadDataCompleted(this, args);
			}
		}

		protected virtual void OnUploadFileCompleted(UploadFileCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.UploadFileCompleted != null)
			{
				this.UploadFileCompleted(this, args);
			}
		}

		protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
		{
			if (this.UploadProgressChanged != null)
			{
				this.UploadProgressChanged(this, e);
			}
		}

		protected virtual void OnUploadStringCompleted(UploadStringCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.UploadStringCompleted != null)
			{
				this.UploadStringCompleted(this, args);
			}
		}

		protected virtual void OnUploadValuesCompleted(UploadValuesCompletedEventArgs args)
		{
			CompleteAsync();
			if (this.UploadValuesCompleted != null)
			{
				this.UploadValuesCompleted(this, args);
			}
		}

		protected virtual WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
		{
			WebResponse webResponse = request.EndGetResponse(result);
			responseHeaders = webResponse.Headers;
			return webResponse;
		}

		protected virtual WebRequest GetWebRequest(Uri address)
		{
			return WebRequest.Create(address);
		}

		protected virtual WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse response = request.GetResponse();
			responseHeaders = response.Headers;
			return response;
		}
	}
}
