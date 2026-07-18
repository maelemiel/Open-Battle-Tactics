using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public class HttpWebResponse : WebResponse, IDisposable, ISerializable
	{
		private Uri uri;

		private WebHeaderCollection webHeaders;

		private CookieCollection cookieCollection;

		private string method;

		private Version version;

		private HttpStatusCode statusCode;

		private string statusDescription;

		private long contentLength;

		private string contentType;

		private CookieContainer cookie_container;

		private bool disposed;

		private Stream stream;

		private string[] cookieExpiresFormats = new string[3] { "r", "ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", "ddd, dd'-'MMM'-'yy HH':'mm':'ss 'GMT'" };

		public string CharacterSet
		{
			get
			{
				string text = ContentType;
				if (text == null)
				{
					return "ISO-8859-1";
				}
				string text2 = text.ToLower();
				int num = text2.IndexOf("charset=");
				if (num == -1)
				{
					return "ISO-8859-1";
				}
				num += 8;
				int num2 = text2.IndexOf(';', num);
				return (num2 != -1) ? text.Substring(num, num2 - num) : text.Substring(num);
			}
		}

		public string ContentEncoding
		{
			get
			{
				CheckDisposed();
				string text = webHeaders["Content-Encoding"];
				return (text == null) ? string.Empty : text;
			}
		}

		public override long ContentLength
		{
			get
			{
				return contentLength;
			}
		}

		public override string ContentType
		{
			get
			{
				CheckDisposed();
				if (contentType == null)
				{
					contentType = webHeaders["Content-Type"];
				}
				return contentType;
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				CheckDisposed();
				if (cookieCollection == null)
				{
					cookieCollection = new CookieCollection();
				}
				return cookieCollection;
			}
			set
			{
				CheckDisposed();
				cookieCollection = value;
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				return webHeaders;
			}
		}

		[System.MonoTODO]
		public override bool IsMutuallyAuthenticated
		{
			get
			{
				throw GetMustImplement();
			}
		}

		public DateTime LastModified
		{
			get
			{
				CheckDisposed();
				try
				{
					string dateStr = webHeaders["Last-Modified"];
					return System.Net.MonoHttpDate.Parse(dateStr);
				}
				catch (Exception)
				{
					return DateTime.Now;
				}
			}
		}

		public string Method
		{
			get
			{
				CheckDisposed();
				return method;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				CheckDisposed();
				return version;
			}
		}

		public override Uri ResponseUri
		{
			get
			{
				CheckDisposed();
				return uri;
			}
		}

		public string Server
		{
			get
			{
				CheckDisposed();
				return webHeaders["Server"];
			}
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return statusCode;
			}
		}

		public string StatusDescription
		{
			get
			{
				CheckDisposed();
				return statusDescription;
			}
		}

		internal HttpWebResponse(Uri uri, string method, System.Net.WebConnectionData data, CookieContainer container)
		{
			this.uri = uri;
			this.method = method;
			webHeaders = data.Headers;
			version = data.Version;
			statusCode = (HttpStatusCode)data.StatusCode;
			statusDescription = data.StatusDescription;
			stream = data.stream;
			contentLength = -1L;
			try
			{
				string text = webHeaders["Content-Length"];
				if (string.IsNullOrEmpty(text) || !long.TryParse(text, out contentLength))
				{
					contentLength = -1L;
				}
			}
			catch (Exception)
			{
				contentLength = -1L;
			}
			if (container != null)
			{
				cookie_container = container;
				FillCookies();
			}
			string text2 = webHeaders["Content-Encoding"];
			if (text2 == "gzip" && (data.request.AutomaticDecompression & DecompressionMethods.GZip) != DecompressionMethods.None)
			{
				stream = new GZipStream(stream, CompressionMode.Decompress);
			}
			else if (text2 == "deflate" && (data.request.AutomaticDecompression & DecompressionMethods.Deflate) != DecompressionMethods.None)
			{
				stream = new DeflateStream(stream, CompressionMode.Decompress);
			}
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected HttpWebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			uri = (Uri)serializationInfo.GetValue("uri", typeof(Uri));
			contentLength = serializationInfo.GetInt64("contentLength");
			contentType = serializationInfo.GetString("contentType");
			method = serializationInfo.GetString("method");
			statusDescription = serializationInfo.GetString("statusDescription");
			cookieCollection = (CookieCollection)serializationInfo.GetValue("cookieCollection", typeof(CookieCollection));
			version = (Version)serializationInfo.GetValue("version", typeof(Version));
			statusCode = (HttpStatusCode)(int)serializationInfo.GetValue("statusCode", typeof(HttpStatusCode));
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			GetObjectData(serializationInfo, streamingContext);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		public string GetResponseHeader(string headerName)
		{
			CheckDisposed();
			string text = webHeaders[headerName];
			return (text == null) ? string.Empty : text;
		}

		internal void ReadAll()
		{
			System.Net.WebConnectionStream webConnectionStream = stream as System.Net.WebConnectionStream;
			if (webConnectionStream == null)
			{
				return;
			}
			try
			{
				webConnectionStream.ReadAll();
			}
			catch
			{
			}
		}

		public override Stream GetResponseStream()
		{
			CheckDisposed();
			if (stream == null)
			{
				return Stream.Null;
			}
			if (string.Compare(method, "HEAD", true) == 0)
			{
				return Stream.Null;
			}
			return stream;
		}

		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("uri", uri);
			serializationInfo.AddValue("contentLength", contentLength);
			serializationInfo.AddValue("contentType", contentType);
			serializationInfo.AddValue("method", method);
			serializationInfo.AddValue("statusDescription", statusDescription);
			serializationInfo.AddValue("cookieCollection", cookieCollection);
			serializationInfo.AddValue("version", version);
			serializationInfo.AddValue("statusCode", statusCode);
		}

		public override void Close()
		{
			((IDisposable)this).Dispose();
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				if (disposing)
				{
					uri = null;
					cookieCollection = null;
					method = null;
					version = null;
					statusDescription = null;
				}
				Stream stream = this.stream;
				this.stream = null;
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private void CheckDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		private void FillCookies()
		{
			if (webHeaders == null)
			{
				return;
			}
			string[] values = webHeaders.GetValues("Set-Cookie");
			if (values != null)
			{
				string[] array = values;
				foreach (string cookie in array)
				{
					SetCookie(cookie);
				}
			}
			values = webHeaders.GetValues("Set-Cookie2");
			if (values != null)
			{
				string[] array2 = values;
				foreach (string cookie2 in array2)
				{
					SetCookie2(cookie2);
				}
			}
		}

		private void SetCookie(string header)
		{
			Cookie cookie = null;
			System.Net.CookieParser cookieParser = new System.Net.CookieParser(header);
			string name;
			string val;
			while (cookieParser.GetNextNameValue(out name, out val))
			{
				if ((name == null || name == string.Empty) && cookie == null)
				{
					continue;
				}
				if (cookie == null)
				{
					cookie = new Cookie(name, val);
					continue;
				}
				name = name.ToUpper();
				switch (name)
				{
				case "COMMENT":
					if (cookie.Comment == null)
					{
						cookie.Comment = val;
					}
					break;
				case "COMMENTURL":
					if (cookie.CommentUri == null)
					{
						cookie.CommentUri = new Uri(val);
					}
					break;
				case "DISCARD":
					cookie.Discard = true;
					break;
				case "DOMAIN":
					if (cookie.Domain == string.Empty)
					{
						cookie.Domain = val;
					}
					break;
				case "HTTPONLY":
					cookie.HttpOnly = true;
					break;
				case "MAX-AGE":
					if (cookie.Expires == DateTime.MinValue)
					{
						try
						{
							cookie.Expires = cookie.TimeStamp.AddSeconds(uint.Parse(val));
						}
						catch
						{
						}
					}
					break;
				case "EXPIRES":
					if (!(cookie.Expires != DateTime.MinValue))
					{
						cookie.Expires = TryParseCookieExpires(val);
					}
					break;
				case "PATH":
					cookie.Path = val;
					break;
				case "PORT":
					if (cookie.Port == null)
					{
						cookie.Port = val;
					}
					break;
				case "SECURE":
					cookie.Secure = true;
					break;
				case "VERSION":
					try
					{
						cookie.Version = (int)uint.Parse(val);
					}
					catch
					{
					}
					break;
				}
			}
			if (cookie != null)
			{
				if (cookieCollection == null)
				{
					cookieCollection = new CookieCollection();
				}
				if (cookie.Domain == string.Empty)
				{
					cookie.Domain = uri.Host;
				}
				cookieCollection.Add(cookie);
				if (cookie_container != null)
				{
					cookie_container.Add(uri, cookie);
				}
			}
		}

		private void SetCookie2(string cookies_str)
		{
			string[] array = cookies_str.Split(',');
			string[] array2 = array;
			foreach (string cookie in array2)
			{
				SetCookie(cookie);
			}
		}

		private DateTime TryParseCookieExpires(string value)
		{
			if (value == null || value.Length == 0)
			{
				return DateTime.MinValue;
			}
			for (int i = 0; i < cookieExpiresFormats.Length; i++)
			{
				try
				{
					DateTime value2 = DateTime.ParseExact(value, cookieExpiresFormats[i], CultureInfo.InvariantCulture);
					value2 = DateTime.SpecifyKind(value2, DateTimeKind.Utc);
					return TimeZone.CurrentTimeZone.ToLocalTime(value2);
				}
				catch
				{
				}
			}
			return DateTime.MinValue;
		}
	}
}
