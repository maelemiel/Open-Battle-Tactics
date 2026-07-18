using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Mono.Security.Protocol.Tls;

namespace System.Net
{
	internal class WebConnection
	{
		private class AbortHelper
		{
			public System.Net.WebConnection Connection;

			public void Abort(object sender, EventArgs args)
			{
				System.Net.WebConnection webConnection = ((HttpWebRequest)sender).WebConnection;
				if (webConnection == null)
				{
					webConnection = Connection;
				}
				webConnection.Abort(sender, args);
			}
		}

		private ServicePoint sPoint;

		private Stream nstream;

		private Socket socket;

		private object socketLock = new object();

		private WebExceptionStatus status;

		private WaitCallback initConn;

		private bool keepAlive;

		private byte[] buffer;

		private static AsyncCallback readDoneDelegate = ReadDone;

		private EventHandler abortHandler;

		private AbortHelper abortHelper;

		private System.Net.ReadState readState;

		internal System.Net.WebConnectionData Data;

		private bool chunkedRead;

		private System.Net.ChunkStream chunkStream;

		private Queue queue;

		private bool reused;

		private int position;

		private bool busy;

		private HttpWebRequest priority_request;

		private NetworkCredential ntlm_credentials;

		private bool ntlm_authenticated;

		private bool unsafe_sharing;

		private bool ssl;

		private bool certsAvailable;

		private Exception connect_exception;

		private static object classLock = new object();

		private static Type sslStream;

		private static PropertyInfo piClient;

		private static PropertyInfo piServer;

		private static PropertyInfo piTrustFailure;

		private static MethodInfo method_GetSecurityPolicyFromNonMainThread;

		internal bool Busy
		{
			get
			{
				lock (this)
				{
					return busy;
				}
			}
		}

		internal bool Connected
		{
			get
			{
				lock (this)
				{
					return socket != null && socket.Connected;
				}
			}
		}

		internal HttpWebRequest PriorityRequest
		{
			set
			{
				priority_request = value;
			}
		}

		internal bool NtlmAuthenticated
		{
			get
			{
				return ntlm_authenticated;
			}
			set
			{
				ntlm_authenticated = value;
			}
		}

		internal NetworkCredential NtlmCredential
		{
			get
			{
				return ntlm_credentials;
			}
			set
			{
				ntlm_credentials = value;
			}
		}

		internal bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return unsafe_sharing;
			}
			set
			{
				unsafe_sharing = value;
			}
		}

		public WebConnection(System.Net.WebConnectionGroup group, ServicePoint sPoint)
		{
			this.sPoint = sPoint;
			buffer = new byte[4096];
			readState = System.Net.ReadState.None;
			Data = new System.Net.WebConnectionData();
			initConn = InitConnection;
			queue = group.Queue;
			abortHelper = new AbortHelper();
			abortHelper.Connection = this;
			abortHandler = abortHelper.Abort;
		}

		private bool CanReuse()
		{
			return !socket.Poll(0, SelectMode.SelectRead);
		}

		private void LoggedThrow(Exception e)
		{
			Console.WriteLine("Throwing this exception: " + e);
			throw e;
		}

		internal static Stream DownloadPolicy(string url, string proxy)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			if (proxy != null)
			{
				httpWebRequest.Proxy = new WebProxy(proxy);
			}
			return httpWebRequest.GetResponse().GetResponseStream();
		}

		private void CheckUnityWebSecurity(HttpWebRequest request)
		{
			if (!Environment.SocketSecurityEnabled)
			{
				return;
			}
			Console.WriteLine("CheckingSecurityForUrl: " + request.RequestUri.AbsoluteUri);
			Uri requestUri = request.RequestUri;
			string text = string.Empty;
			if (!requestUri.IsDefaultPort)
			{
				text = ":" + requestUri.Port;
			}
			if (requestUri.ToString() == requestUri.Scheme + "://" + requestUri.Host + text + "/crossdomain.xml")
			{
				return;
			}
			try
			{
				if (method_GetSecurityPolicyFromNonMainThread == null)
				{
					Type type = Type.GetType("UnityEngine.UnityCrossDomainHelper, CrossDomainPolicyParser, Version=1.0.0.0, Culture=neutral");
					if (type == null)
					{
						LoggedThrow(new SecurityException("Cant find type UnityCrossDomainHelper"));
					}
					method_GetSecurityPolicyFromNonMainThread = type.GetMethod("GetSecurityPolicyForDotNetWebRequest");
					if (method_GetSecurityPolicyFromNonMainThread == null)
					{
						LoggedThrow(new SecurityException("Cant find GetSecurityPolicyFromNonMainThread"));
					}
				}
				MethodInfo method = typeof(System.Net.WebConnection).GetMethod("DownloadPolicy", BindingFlags.Static | BindingFlags.NonPublic);
				if (method == null)
				{
					LoggedThrow(new SecurityException("Cannot find method DownloadPolicy"));
				}
				if (!(bool)method_GetSecurityPolicyFromNonMainThread.Invoke(null, new object[2]
				{
					request.RequestUri.ToString(),
					method
				}))
				{
					LoggedThrow(new SecurityException("Webrequest was denied"));
				}
			}
			catch (Exception ex)
			{
				LoggedThrow(new SecurityException("Unexpected error while trying to call method_GetSecurityPolicyBlocking : " + ex));
			}
		}

		private void Connect(HttpWebRequest request)
		{
			lock (socketLock)
			{
				if (this.socket != null && this.socket.Connected && status == WebExceptionStatus.Success && CanReuse() && CompleteChunkedRead())
				{
					reused = true;
					return;
				}
				reused = false;
				if (this.socket != null)
				{
					this.socket.Close();
					this.socket = null;
				}
				chunkStream = null;
				IPHostEntry hostEntry = sPoint.HostEntry;
				if (hostEntry == null)
				{
					status = ((!sPoint.UsesProxy) ? WebExceptionStatus.NameResolutionFailure : WebExceptionStatus.ProxyNameResolutionFailure);
					return;
				}
				System.Net.WebConnectionData data = Data;
				IPAddress[] addressList = hostEntry.AddressList;
				foreach (IPAddress iPAddress in addressList)
				{
					this.socket = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, sPoint.Address.Port);
					this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, (!sPoint.UseNagleAlgorithm) ? 1 : 0);
					this.socket.NoDelay = !sPoint.UseNagleAlgorithm;
					if (!sPoint.CallEndPointDelegate(this.socket, iPEndPoint))
					{
						this.socket.Close();
						this.socket = null;
						status = WebExceptionStatus.ConnectFailure;
						continue;
					}
					try
					{
						if (!request.Aborted)
						{
							CheckUnityWebSecurity(request);
							this.socket.Connect(iPEndPoint, false);
							status = WebExceptionStatus.Success;
						}
						break;
					}
					catch (ThreadAbortException)
					{
						Socket socket = this.socket;
						this.socket = null;
						if (socket != null)
						{
							socket.Close();
						}
						break;
					}
					catch (ObjectDisposedException)
					{
						break;
					}
					catch (Exception ex3)
					{
						Socket socket2 = this.socket;
						this.socket = null;
						if (socket2 != null)
						{
							socket2.Close();
						}
						if (!request.Aborted)
						{
							status = WebExceptionStatus.ConnectFailure;
						}
						connect_exception = ex3;
					}
				}
			}
		}

		private static void EnsureSSLStreamAvailable()
		{
			lock (classLock)
			{
				if (sslStream == null)
				{
					sslStream = typeof(HttpsClientStream);
					piClient = sslStream.GetProperty("SelectedClientCertificate");
					piServer = sslStream.GetProperty("ServerCertificate");
					piTrustFailure = sslStream.GetProperty("TrustFailure");
				}
			}
		}

		private bool CreateTunnel(HttpWebRequest request, Stream stream, out byte[] buffer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CONNECT ");
			stringBuilder.Append(request.Address.Host);
			stringBuilder.Append(':');
			stringBuilder.Append(request.Address.Port);
			stringBuilder.Append(" HTTP/");
			if (request.ServicePoint.ProtocolVersion == HttpVersion.Version11)
			{
				stringBuilder.Append("1.1");
			}
			else
			{
				stringBuilder.Append("1.0");
			}
			stringBuilder.Append("\r\nHost: ");
			stringBuilder.Append(request.Address.Authority);
			string challenge = Data.Challenge;
			Data.Challenge = null;
			bool flag = request.Headers["Proxy-Authorization"] != null;
			if (flag)
			{
				stringBuilder.Append("\r\nProxy-Authorization: ");
				stringBuilder.Append(request.Headers["Proxy-Authorization"]);
			}
			else if (challenge != null && Data.StatusCode == 407)
			{
				flag = true;
				ICredentials credentials = request.Proxy.Credentials;
				Authorization authorization = AuthenticationManager.Authenticate(challenge, request, credentials);
				if (authorization != null)
				{
					stringBuilder.Append("\r\nProxy-Authorization: ");
					stringBuilder.Append(authorization.Message);
				}
			}
			stringBuilder.Append("\r\n\r\n");
			Data.StatusCode = 0;
			byte[] bytes = Encoding.Default.GetBytes(stringBuilder.ToString());
			stream.Write(bytes, 0, bytes.Length);
			int num;
			WebHeaderCollection webHeaderCollection = ReadHeaders(request, stream, out buffer, out num);
			if (!flag && webHeaderCollection != null && num == 407)
			{
				Data.StatusCode = num;
				Data.Challenge = webHeaderCollection["Proxy-Authenticate"];
				return false;
			}
			if (num != 200)
			{
				string text = string.Format("The remote server returned a {0} status code.", num);
				HandleError(WebExceptionStatus.SecureChannelFailure, null, text);
				return false;
			}
			return webHeaderCollection != null;
		}

		private WebHeaderCollection ReadHeaders(HttpWebRequest request, Stream stream, out byte[] retBuffer, out int status)
		{
			retBuffer = null;
			status = 200;
			byte[] array = new byte[1024];
			MemoryStream memoryStream = new MemoryStream();
			bool flag = false;
			WebHeaderCollection webHeaderCollection = null;
			while (true)
			{
				int num = stream.Read(array, 0, 1024);
				if (num == 0)
				{
					break;
				}
				memoryStream.Write(array, 0, num);
				int start = 0;
				string output = null;
				webHeaderCollection = new WebHeaderCollection();
				while (ReadLine(memoryStream.GetBuffer(), ref start, (int)memoryStream.Length, ref output))
				{
					if (output == null)
					{
						if (memoryStream.Length - start > 0)
						{
							retBuffer = new byte[memoryStream.Length - start];
							Buffer.BlockCopy(memoryStream.GetBuffer(), start, retBuffer, 0, retBuffer.Length);
						}
						return webHeaderCollection;
					}
					if (flag)
					{
						webHeaderCollection.Add(output);
						continue;
					}
					int num2 = output.IndexOf(' ');
					if (num2 == -1)
					{
						HandleError(WebExceptionStatus.ServerProtocolViolation, null, "ReadHeaders2");
						return null;
					}
					status = (int)uint.Parse(output.Substring(num2 + 1, 3));
					flag = true;
				}
			}
			HandleError(WebExceptionStatus.ServerProtocolViolation, null, "ReadHeaders");
			return null;
		}

		private bool CreateStream(HttpWebRequest request)
		{
			try
			{
				NetworkStream networkStream = new NetworkStream(socket, false);
				if (request.Address.Scheme == Uri.UriSchemeHttps)
				{
					ssl = true;
					EnsureSSLStreamAvailable();
					if (!reused || nstream == null || nstream.GetType() != sslStream)
					{
						byte[] array = null;
						if (sPoint.UseConnect && !CreateTunnel(request, networkStream, out array))
						{
							return false;
						}
						object[] args = new object[4] { networkStream, request.ClientCertificates, request, array };
						nstream = (Stream)Activator.CreateInstance(sslStream, args);
						SslClientStream sslClientStream = (SslClientStream)nstream;
						ServicePointManager.ChainValidationHelper chainValidationHelper = new ServicePointManager.ChainValidationHelper(request);
						sslClientStream.ServerCertValidation2 += chainValidationHelper.ValidateChain;
						certsAvailable = false;
					}
				}
				else
				{
					ssl = false;
					nstream = networkStream;
				}
			}
			catch (Exception)
			{
				if (!request.Aborted)
				{
					status = WebExceptionStatus.ConnectFailure;
				}
				return false;
			}
			return true;
		}

		private void HandleError(WebExceptionStatus st, Exception e, string where)
		{
			status = st;
			lock (this)
			{
				if (st == WebExceptionStatus.RequestCanceled)
				{
					Data = new System.Net.WebConnectionData();
				}
			}
			if (e == null)
			{
				try
				{
					throw new Exception(new StackTrace().ToString());
				}
				catch (Exception ex)
				{
					e = ex;
				}
			}
			HttpWebRequest httpWebRequest = null;
			if (Data != null && Data.request != null)
			{
				httpWebRequest = Data.request;
			}
			Close(true);
			if (httpWebRequest != null)
			{
				httpWebRequest.FinishedReading = true;
				httpWebRequest.SetResponseError(st, e, where);
			}
		}

		private static void ReadDone(IAsyncResult result)
		{
			System.Net.WebConnection webConnection = (System.Net.WebConnection)result.AsyncState;
			System.Net.WebConnectionData data = webConnection.Data;
			Stream stream = webConnection.nstream;
			if (stream == null)
			{
				webConnection.Close(true);
				return;
			}
			int num = -1;
			try
			{
				num = stream.EndRead(result);
			}
			catch (Exception e)
			{
				webConnection.HandleError(WebExceptionStatus.ReceiveFailure, e, "ReadDone1");
				return;
			}
			if (num == 0)
			{
				webConnection.HandleError(WebExceptionStatus.ReceiveFailure, null, "ReadDone2");
				return;
			}
			if (num < 0)
			{
				webConnection.HandleError(WebExceptionStatus.ServerProtocolViolation, null, "ReadDone3");
				return;
			}
			int num2 = -1;
			num += webConnection.position;
			if (webConnection.readState == System.Net.ReadState.None)
			{
				Exception ex = null;
				try
				{
					num2 = webConnection.GetResponse(webConnection.buffer, num);
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				if (ex != null)
				{
					webConnection.HandleError(WebExceptionStatus.ServerProtocolViolation, ex, "ReadDone4");
					return;
				}
			}
			if (webConnection.readState != System.Net.ReadState.Content)
			{
				int num3 = num * 2;
				int num4 = ((num3 >= webConnection.buffer.Length) ? num3 : webConnection.buffer.Length);
				byte[] dst = new byte[num4];
				Buffer.BlockCopy(webConnection.buffer, 0, dst, 0, num);
				webConnection.buffer = dst;
				webConnection.position = num;
				webConnection.readState = System.Net.ReadState.None;
				InitRead(webConnection);
				return;
			}
			webConnection.position = 0;
			System.Net.WebConnectionStream webConnectionStream = new System.Net.WebConnectionStream(webConnection);
			string text = data.Headers["Transfer-Encoding"];
			webConnection.chunkedRead = text != null && text.ToLower().IndexOf("chunked") != -1;
			if (!webConnection.chunkedRead)
			{
				webConnectionStream.ReadBuffer = webConnection.buffer;
				webConnectionStream.ReadBufferOffset = num2;
				webConnectionStream.ReadBufferSize = num;
				webConnectionStream.CheckResponseInBuffer();
			}
			else if (webConnection.chunkStream == null)
			{
				try
				{
					webConnection.chunkStream = new System.Net.ChunkStream(webConnection.buffer, num2, num, data.Headers);
				}
				catch (Exception e2)
				{
					webConnection.HandleError(WebExceptionStatus.ServerProtocolViolation, e2, "ReadDone5");
					return;
				}
			}
			else
			{
				webConnection.chunkStream.ResetBuffer();
				try
				{
					webConnection.chunkStream.Write(webConnection.buffer, num2, num);
				}
				catch (Exception e3)
				{
					webConnection.HandleError(WebExceptionStatus.ServerProtocolViolation, e3, "ReadDone6");
					return;
				}
			}
			data.stream = webConnectionStream;
			if (!ExpectContent(data.StatusCode) || data.request.Method == "HEAD")
			{
				webConnectionStream.ForceCompletion();
			}
			data.request.SetResponseData(data);
		}

		private static bool ExpectContent(int statusCode)
		{
			return statusCode >= 200 && statusCode != 204 && statusCode != 304;
		}

		internal void GetCertificates()
		{
			X509Certificate client = (X509Certificate)piClient.GetValue(nstream, null);
			X509Certificate x509Certificate = (X509Certificate)piServer.GetValue(nstream, null);
			sPoint.SetCertificates(client, x509Certificate);
			certsAvailable = x509Certificate != null;
		}

		internal static void InitRead(object state)
		{
			System.Net.WebConnection webConnection = (System.Net.WebConnection)state;
			Stream stream = webConnection.nstream;
			try
			{
				int count = webConnection.buffer.Length - webConnection.position;
				stream.BeginRead(webConnection.buffer, webConnection.position, count, readDoneDelegate, webConnection);
			}
			catch (Exception e)
			{
				webConnection.HandleError(WebExceptionStatus.ReceiveFailure, e, "InitRead");
			}
		}

		private int GetResponse(byte[] buffer, int max)
		{
			int start = 0;
			string output = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			do
			{
				if (readState == System.Net.ReadState.None)
				{
					if (!ReadLine(buffer, ref start, max, ref output))
					{
						return -1;
					}
					if (output == null)
					{
						flag3 = true;
						continue;
					}
					flag3 = false;
					readState = System.Net.ReadState.Status;
					string[] array = output.Split(' ');
					if (array.Length < 2)
					{
						return -1;
					}
					if (string.Compare(array[0], "HTTP/1.1", true) == 0)
					{
						Data.Version = HttpVersion.Version11;
						sPoint.SetVersion(HttpVersion.Version11);
					}
					else
					{
						Data.Version = HttpVersion.Version10;
						sPoint.SetVersion(HttpVersion.Version10);
					}
					Data.StatusCode = (int)uint.Parse(array[1]);
					if (array.Length >= 3)
					{
						Data.StatusDescription = string.Join(" ", array, 2, array.Length - 2);
					}
					else
					{
						Data.StatusDescription = string.Empty;
					}
					if (start >= max)
					{
						return start;
					}
				}
				flag3 = false;
				if (readState != System.Net.ReadState.Status)
				{
					continue;
				}
				readState = System.Net.ReadState.Headers;
				Data.Headers = new WebHeaderCollection();
				ArrayList arrayList = new ArrayList();
				bool flag4 = false;
				while (!flag4 && ReadLine(buffer, ref start, max, ref output))
				{
					if (output == null)
					{
						flag4 = true;
					}
					else if (output.Length > 0 && (output[0] == ' ' || output[0] == '\t'))
					{
						int num = arrayList.Count - 1;
						if (num < 0)
						{
							break;
						}
						string value = (string)arrayList[num] + output;
						arrayList[num] = value;
					}
					else
					{
						arrayList.Add(output);
					}
				}
				if (!flag4)
				{
					return -1;
				}
				foreach (string item in arrayList)
				{
					Data.Headers.SetInternal(item);
				}
				if (Data.StatusCode == 100)
				{
					sPoint.SendContinue = true;
					if (start >= max)
					{
						return start;
					}
					if (Data.request.ExpectContinue)
					{
						Data.request.DoContinueDelegate(Data.StatusCode, Data.Headers);
						Data.request.ExpectContinue = false;
					}
					readState = System.Net.ReadState.None;
					flag2 = true;
					continue;
				}
				readState = System.Net.ReadState.Content;
				return start;
			}
			while (flag3 || flag2);
			return -1;
		}

		private void InitConnection(object state)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)state;
			httpWebRequest.WebConnection = this;
			if (httpWebRequest.Aborted)
			{
				return;
			}
			keepAlive = httpWebRequest.KeepAlive;
			Data = new System.Net.WebConnectionData();
			Data.request = httpWebRequest;
			while (true)
			{
				Connect(httpWebRequest);
				if (httpWebRequest.Aborted)
				{
					return;
				}
				if (status != WebExceptionStatus.Success)
				{
					if (!httpWebRequest.Aborted)
					{
						httpWebRequest.SetWriteStreamError(status, connect_exception);
						Close(true);
					}
					return;
				}
				if (CreateStream(httpWebRequest))
				{
					break;
				}
				if (httpWebRequest.Aborted)
				{
					return;
				}
				WebExceptionStatus webExceptionStatus = status;
				if (Data.Challenge != null)
				{
					continue;
				}
				Exception exc = connect_exception;
				connect_exception = null;
				httpWebRequest.SetWriteStreamError(webExceptionStatus, exc);
				Close(true);
				return;
			}
			readState = System.Net.ReadState.None;
			httpWebRequest.SetWriteStream(new System.Net.WebConnectionStream(this, httpWebRequest));
		}

		internal EventHandler SendRequest(HttpWebRequest request)
		{
			if (request.Aborted)
			{
				return null;
			}
			lock (this)
			{
				if (!busy)
				{
					busy = true;
					status = WebExceptionStatus.Success;
					ThreadPool.QueueUserWorkItem(initConn, request);
				}
				else
				{
					lock (queue)
					{
						queue.Enqueue(request);
					}
				}
			}
			return abortHandler;
		}

		private void SendNext()
		{
			lock (queue)
			{
				if (queue.Count > 0)
				{
					SendRequest((HttpWebRequest)queue.Dequeue());
				}
			}
		}

		internal void NextRead()
		{
			lock (this)
			{
				Data.request.FinishedReading = true;
				string name = ((!sPoint.UsesProxy) ? "Connection" : "Proxy-Connection");
				string text = ((Data.Headers == null) ? null : Data.Headers[name]);
				bool flag = Data.Version == HttpVersion.Version11 && keepAlive;
				if (text != null)
				{
					text = text.ToLower();
					flag = keepAlive && text.IndexOf("keep-alive") != -1;
				}
				if ((socket != null && !socket.Connected) || !flag || (text != null && text.IndexOf("close") != -1))
				{
					Close(false);
				}
				busy = false;
				if (priority_request != null)
				{
					SendRequest(priority_request);
					priority_request = null;
				}
				else
				{
					SendNext();
				}
			}
		}

		private static bool ReadLine(byte[] buffer, ref int start, int max, ref string output)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (start < max)
			{
				num = buffer[start++];
				if (num == 10)
				{
					if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\r')
					{
						stringBuilder.Length--;
					}
					flag = false;
					break;
				}
				if (flag)
				{
					stringBuilder.Length--;
					break;
				}
				if (num == 13)
				{
					flag = true;
				}
				stringBuilder.Append((char)num);
			}
			if (num != 10 && num != 13)
			{
				return false;
			}
			if (stringBuilder.Length == 0)
			{
				output = null;
				return num == 10 || num == 13;
			}
			if (flag)
			{
				stringBuilder.Length--;
			}
			output = stringBuilder.ToString();
			return true;
		}

		internal IAsyncResult BeginRead(HttpWebRequest request, byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			lock (this)
			{
				if (Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (nstream == null)
				{
					return null;
				}
			}
			IAsyncResult asyncResult = null;
			if (!chunkedRead || chunkStream.WantMore)
			{
				try
				{
					asyncResult = nstream.BeginRead(buffer, offset, size, cb, state);
					cb = null;
				}
				catch (Exception)
				{
					HandleError(WebExceptionStatus.ReceiveFailure, null, "chunked BeginRead");
					throw;
				}
			}
			if (chunkedRead)
			{
				System.Net.WebAsyncResult webAsyncResult = new System.Net.WebAsyncResult(cb, state, buffer, offset, size);
				webAsyncResult.InnerAsyncResult = asyncResult;
				if (asyncResult == null)
				{
					webAsyncResult.SetCompleted(true, (Exception)null);
					webAsyncResult.DoCallback();
				}
				return webAsyncResult;
			}
			return asyncResult;
		}

		internal int EndRead(HttpWebRequest request, IAsyncResult result)
		{
			lock (this)
			{
				if (Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (nstream == null)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
			}
			int read = 0;
			System.Net.WebAsyncResult webAsyncResult = null;
			IAsyncResult innerAsyncResult = ((System.Net.WebAsyncResult)result).InnerAsyncResult;
			if (chunkedRead && innerAsyncResult is System.Net.WebAsyncResult)
			{
				webAsyncResult = (System.Net.WebAsyncResult)innerAsyncResult;
				IAsyncResult innerAsyncResult2 = webAsyncResult.InnerAsyncResult;
				if (innerAsyncResult2 != null && !(innerAsyncResult2 is System.Net.WebAsyncResult))
				{
					read = nstream.EndRead(innerAsyncResult2);
				}
			}
			else if (!(innerAsyncResult is System.Net.WebAsyncResult))
			{
				read = nstream.EndRead(innerAsyncResult);
				webAsyncResult = (System.Net.WebAsyncResult)result;
			}
			if (chunkedRead)
			{
				bool flag = read == 0;
				try
				{
					chunkStream.WriteAndReadBack(webAsyncResult.Buffer, webAsyncResult.Offset, webAsyncResult.Size, ref read);
					if (!flag && read == 0 && chunkStream.WantMore)
					{
						read = EnsureRead(webAsyncResult.Buffer, webAsyncResult.Offset, webAsyncResult.Size);
					}
				}
				catch (Exception ex)
				{
					if (ex is WebException)
					{
						throw ex;
					}
					throw new WebException("Invalid chunked data.", ex, WebExceptionStatus.ServerProtocolViolation, null);
				}
				if ((flag || read == 0) && chunkStream.ChunkLeft != 0)
				{
					HandleError(WebExceptionStatus.ReceiveFailure, null, "chunked EndRead");
					throw new WebException("Read error", null, WebExceptionStatus.ReceiveFailure, null);
				}
			}
			return (read == 0) ? (-1) : read;
		}

		private int EnsureRead(byte[] buffer, int offset, int size)
		{
			byte[] array = null;
			int i;
			for (i = 0; i == 0; i += chunkStream.Read(buffer, offset + i, size - i))
			{
				if (!chunkStream.WantMore)
				{
					break;
				}
				int num = chunkStream.ChunkLeft;
				if (num <= 0)
				{
					num = 1024;
				}
				else if (num > 16384)
				{
					num = 16384;
				}
				if (array == null || array.Length < num)
				{
					array = new byte[num];
				}
				int num2 = nstream.Read(array, 0, num);
				if (num2 <= 0)
				{
					return 0;
				}
				chunkStream.Write(array, 0, num2);
			}
			return i;
		}

		private bool CompleteChunkedRead()
		{
			if (!chunkedRead || chunkStream == null)
			{
				return true;
			}
			while (chunkStream.WantMore)
			{
				int num = nstream.Read(buffer, 0, buffer.Length);
				if (num <= 0)
				{
					return false;
				}
				chunkStream.Write(buffer, 0, num);
			}
			return true;
		}

		internal IAsyncResult BeginWrite(HttpWebRequest request, byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			lock (this)
			{
				if (Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (nstream == null)
				{
					return null;
				}
			}
			IAsyncResult asyncResult = null;
			try
			{
				return nstream.BeginWrite(buffer, offset, size, cb, state);
			}
			catch (Exception)
			{
				status = WebExceptionStatus.SendFailure;
				throw;
			}
		}

		internal void EndWrite2(HttpWebRequest request, IAsyncResult result)
		{
			if (request.FinishedReading)
			{
				return;
			}
			lock (this)
			{
				if (Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (nstream == null)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
			}
			try
			{
				nstream.EndWrite(result);
			}
			catch (Exception ex)
			{
				status = WebExceptionStatus.SendFailure;
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw;
			}
		}

		internal bool EndWrite(HttpWebRequest request, IAsyncResult result)
		{
			if (request.FinishedReading)
			{
				return true;
			}
			lock (this)
			{
				if (Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (nstream == null)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
			}
			try
			{
				nstream.EndWrite(result);
				return true;
			}
			catch
			{
				status = WebExceptionStatus.SendFailure;
				return false;
			}
		}

		internal int Read(HttpWebRequest request, byte[] buffer, int offset, int size)
		{
			lock (this)
			{
				if (Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (nstream == null)
				{
					return 0;
				}
			}
			int read = 0;
			try
			{
				bool flag = false;
				if (!chunkedRead)
				{
					read = nstream.Read(buffer, offset, size);
					flag = read == 0;
				}
				if (chunkedRead)
				{
					try
					{
						chunkStream.WriteAndReadBack(buffer, offset, size, ref read);
						if (!flag && read == 0 && chunkStream.WantMore)
						{
							read = EnsureRead(buffer, offset, size);
						}
					}
					catch (Exception e)
					{
						HandleError(WebExceptionStatus.ReceiveFailure, e, "chunked Read1");
						throw;
					}
					if ((flag || read == 0) && chunkStream.WantMore)
					{
						HandleError(WebExceptionStatus.ReceiveFailure, null, "chunked Read2");
						throw new WebException("Read error", null, WebExceptionStatus.ReceiveFailure, null);
					}
				}
			}
			catch (Exception e2)
			{
				HandleError(WebExceptionStatus.ReceiveFailure, e2, "Read");
			}
			return read;
		}

		internal bool Write(HttpWebRequest request, byte[] buffer, int offset, int size, ref string err_msg)
		{
			err_msg = null;
			lock (this)
			{
				if (Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (nstream == null)
				{
					return false;
				}
			}
			try
			{
				nstream.Write(buffer, offset, size);
				if (ssl && !certsAvailable)
				{
					GetCertificates();
				}
			}
			catch (Exception ex)
			{
				err_msg = ex.Message;
				WebExceptionStatus st = WebExceptionStatus.SendFailure;
				string text = "Write: " + err_msg;
				if (ex is WebException)
				{
					HandleError(st, ex, text);
					return false;
				}
				if (ssl && (bool)piTrustFailure.GetValue(nstream, null))
				{
					st = WebExceptionStatus.TrustFailure;
					text = "Trust failure";
				}
				HandleError(st, ex, text);
				return false;
			}
			return true;
		}

		internal void Close(bool sendNext)
		{
			lock (this)
			{
				if (nstream != null)
				{
					try
					{
						nstream.Close();
					}
					catch
					{
					}
					nstream = null;
				}
				if (socket != null)
				{
					try
					{
						socket.Close();
					}
					catch
					{
					}
					socket = null;
				}
				busy = false;
				Data = new System.Net.WebConnectionData();
				if (sendNext)
				{
					SendNext();
				}
			}
		}

		private void Abort(object sender, EventArgs args)
		{
			lock (this)
			{
				lock (queue)
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)sender;
					if (Data.request == httpWebRequest)
					{
						if (!httpWebRequest.FinishedReading)
						{
							status = WebExceptionStatus.RequestCanceled;
							Close(false);
							if (queue.Count > 0)
							{
								Data.request = (HttpWebRequest)queue.Dequeue();
								SendRequest(Data.request);
							}
						}
						return;
					}
					httpWebRequest.FinishedReading = true;
					httpWebRequest.SetResponseError(WebExceptionStatus.RequestCanceled, null, "User aborted");
					if (queue.Count > 0 && queue.Peek() == sender)
					{
						queue.Dequeue();
					}
					else
					{
						if (queue.Count <= 0)
						{
							return;
						}
						object[] array = queue.ToArray();
						queue.Clear();
						for (int num = array.Length - 1; num >= 0; num--)
						{
							if (array[num] != sender)
							{
								queue.Enqueue(array[num]);
							}
						}
					}
				}
			}
		}

		internal void ResetNtlm()
		{
			ntlm_authenticated = false;
			ntlm_credentials = null;
			unsafe_sharing = false;
		}
	}
}
