using System.IO;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace System.Net
{
	[Serializable]
	public class HttpWebRequest : WebRequest, ISerializable
	{
		private Uri requestUri;

		private Uri actualUri;

		private bool hostChanged;

		private bool allowAutoRedirect = true;

		private bool allowBuffering = true;

		private X509CertificateCollection certificates;

		private string connectionGroup;

		private long contentLength = -1L;

		private HttpContinueDelegate continueDelegate;

		private CookieContainer cookieContainer;

		private ICredentials credentials;

		private bool haveResponse;

		private bool haveRequest;

		private bool requestSent;

		private WebHeaderCollection webHeaders = new WebHeaderCollection(true);

		private bool keepAlive = true;

		private int maxAutoRedirect = 50;

		private string mediaType = string.Empty;

		private string method = "GET";

		private string initialMethod = "GET";

		private bool pipelined = true;

		private bool preAuthenticate;

		private bool usedPreAuth;

		private Version version = HttpVersion.Version11;

		private Version actualVersion;

		private IWebProxy proxy;

		private bool sendChunked;

		private ServicePoint servicePoint;

		private int timeout = 100000;

		private System.Net.WebConnectionStream writeStream;

		private HttpWebResponse webResponse;

		private System.Net.WebAsyncResult asyncWrite;

		private System.Net.WebAsyncResult asyncRead;

		private EventHandler abortHandler;

		private int aborted;

		private bool gotRequestStream;

		private int redirects;

		private bool expectContinue;

		private bool authCompleted;

		private byte[] bodyBuffer;

		private int bodyBufferLength;

		private bool getResponseCalled;

		private Exception saved_exc;

		private object locker = new object();

		private bool is_ntlm_auth;

		private bool finished_reading;

		internal System.Net.WebConnection WebConnection;

		private DecompressionMethods auto_decomp;

		private int maxResponseHeadersLength;

		private static int defaultMaxResponseHeadersLength;

		private int readWriteTimeout = 300000;

		private bool unsafe_auth_blah;

		internal bool UsesNtlmAuthentication
		{
			get
			{
				return is_ntlm_auth;
			}
		}

		public string Accept
		{
			get
			{
				return webHeaders["Accept"];
			}
			set
			{
				CheckRequestStarted();
				webHeaders.RemoveAndAdd("Accept", value);
			}
		}

		public Uri Address
		{
			get
			{
				return actualUri;
			}
		}

		public bool AllowAutoRedirect
		{
			get
			{
				return allowAutoRedirect;
			}
			set
			{
				allowAutoRedirect = value;
			}
		}

		public bool AllowWriteStreamBuffering
		{
			get
			{
				return allowBuffering;
			}
			set
			{
				allowBuffering = value;
			}
		}

		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				return auto_decomp;
			}
			set
			{
				CheckRequestStarted();
				auto_decomp = value;
			}
		}

		internal bool InternalAllowBuffering
		{
			get
			{
				return allowBuffering && method != "HEAD" && method != "GET" && method != "MKCOL" && method != "CONNECT" && method != "DELETE" && method != "TRACE";
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				if (certificates == null)
				{
					certificates = new X509CertificateCollection();
				}
				return certificates;
			}
			[System.MonoTODO]
			set
			{
				throw GetMustImplement();
			}
		}

		public string Connection
		{
			get
			{
				return webHeaders["Connection"];
			}
			set
			{
				CheckRequestStarted();
				string text = value;
				if (text != null)
				{
					text = text.Trim().ToLower();
				}
				if (text == null || text.Length == 0)
				{
					webHeaders.RemoveInternal("Connection");
					return;
				}
				if (text == "keep-alive" || text == "close")
				{
					throw new ArgumentException("Keep-Alive and Close may not be set with this property");
				}
				if (keepAlive && text.IndexOf("keep-alive") == -1)
				{
					value += ", Keep-Alive";
				}
				webHeaders.RemoveAndAdd("Connection", value);
			}
		}

		public override string ConnectionGroupName
		{
			get
			{
				return connectionGroup;
			}
			set
			{
				connectionGroup = value;
			}
		}

		public override long ContentLength
		{
			get
			{
				return contentLength;
			}
			set
			{
				CheckRequestStarted();
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "Content-Length must be >= 0");
				}
				contentLength = value;
			}
		}

		internal long InternalContentLength
		{
			set
			{
				contentLength = value;
			}
		}

		public override string ContentType
		{
			get
			{
				return webHeaders["Content-Type"];
			}
			set
			{
				if (value == null || value.Trim().Length == 0)
				{
					webHeaders.RemoveInternal("Content-Type");
				}
				else
				{
					webHeaders.RemoveAndAdd("Content-Type", value);
				}
			}
		}

		public HttpContinueDelegate ContinueDelegate
		{
			get
			{
				return continueDelegate;
			}
			set
			{
				continueDelegate = value;
			}
		}

		public CookieContainer CookieContainer
		{
			get
			{
				return cookieContainer;
			}
			set
			{
				cookieContainer = value;
			}
		}

		public override ICredentials Credentials
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

		[System.MonoTODO]
		public new static RequestCachePolicy DefaultCachePolicy
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
		public static int DefaultMaximumErrorResponseLength
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

		public string Expect
		{
			get
			{
				return webHeaders["Expect"];
			}
			set
			{
				CheckRequestStarted();
				string text = value;
				if (text != null)
				{
					text = text.Trim().ToLower();
				}
				if (text == null || text.Length == 0)
				{
					webHeaders.RemoveInternal("Expect");
					return;
				}
				if (text == "100-continue")
				{
					throw new ArgumentException("100-Continue cannot be set with this property.", "value");
				}
				webHeaders.RemoveAndAdd("Expect", value);
			}
		}

		public bool HaveResponse
		{
			get
			{
				return haveResponse;
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				return webHeaders;
			}
			set
			{
				CheckRequestStarted();
				WebHeaderCollection webHeaderCollection = new WebHeaderCollection(true);
				int count = value.Count;
				for (int i = 0; i < count; i++)
				{
					webHeaderCollection.Add(value.GetKey(i), value.Get(i));
				}
				webHeaders = webHeaderCollection;
			}
		}

		public DateTime IfModifiedSince
		{
			get
			{
				string text = webHeaders["If-Modified-Since"];
				if (text == null)
				{
					return DateTime.Now;
				}
				try
				{
					return System.Net.MonoHttpDate.Parse(text);
				}
				catch (Exception)
				{
					return DateTime.Now;
				}
			}
			set
			{
				CheckRequestStarted();
				webHeaders.SetInternal("If-Modified-Since", value.ToUniversalTime().ToString("r", null));
			}
		}

		public bool KeepAlive
		{
			get
			{
				return keepAlive;
			}
			set
			{
				keepAlive = value;
			}
		}

		public int MaximumAutomaticRedirections
		{
			get
			{
				return maxAutoRedirect;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("Must be > 0", "value");
				}
				maxAutoRedirect = value;
			}
		}

		[System.MonoTODO("Use this")]
		public int MaximumResponseHeadersLength
		{
			get
			{
				return maxResponseHeadersLength;
			}
			set
			{
				maxResponseHeadersLength = value;
			}
		}

		[System.MonoTODO("Use this")]
		public static int DefaultMaximumResponseHeadersLength
		{
			get
			{
				return defaultMaxResponseHeadersLength;
			}
			set
			{
				defaultMaxResponseHeadersLength = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return readWriteTimeout;
			}
			set
			{
				if (requestSent)
				{
					throw new InvalidOperationException("The request has already been sent.");
				}
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value", "Must be >= -1");
				}
				readWriteTimeout = value;
			}
		}

		public string MediaType
		{
			get
			{
				return mediaType;
			}
			set
			{
				mediaType = value;
			}
		}

		public override string Method
		{
			get
			{
				return method;
			}
			set
			{
				if (value == null || value.Trim() == string.Empty)
				{
					throw new ArgumentException("not a valid method");
				}
				method = value;
			}
		}

		public bool Pipelined
		{
			get
			{
				return pipelined;
			}
			set
			{
				pipelined = value;
			}
		}

		public override bool PreAuthenticate
		{
			get
			{
				return preAuthenticate;
			}
			set
			{
				preAuthenticate = value;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return version;
			}
			set
			{
				if (value != HttpVersion.Version10 && value != HttpVersion.Version11)
				{
					throw new ArgumentException("value");
				}
				version = value;
			}
		}

		public override IWebProxy Proxy
		{
			get
			{
				return proxy;
			}
			set
			{
				CheckRequestStarted();
				proxy = value;
				servicePoint = null;
			}
		}

		public string Referer
		{
			get
			{
				return webHeaders["Referer"];
			}
			set
			{
				CheckRequestStarted();
				if (value == null || value.Trim().Length == 0)
				{
					webHeaders.RemoveInternal("Referer");
				}
				else
				{
					webHeaders.SetInternal("Referer", value);
				}
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return requestUri;
			}
		}

		public bool SendChunked
		{
			get
			{
				return sendChunked;
			}
			set
			{
				CheckRequestStarted();
				sendChunked = value;
			}
		}

		public ServicePoint ServicePoint
		{
			get
			{
				return GetServicePoint();
			}
		}

		public override int Timeout
		{
			get
			{
				return timeout;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				timeout = value;
			}
		}

		public string TransferEncoding
		{
			get
			{
				return webHeaders["Transfer-Encoding"];
			}
			set
			{
				CheckRequestStarted();
				string text = value;
				if (text != null)
				{
					text = text.Trim().ToLower();
				}
				if (text == null || text.Length == 0)
				{
					webHeaders.RemoveInternal("Transfer-Encoding");
					return;
				}
				if (text == "chunked")
				{
					throw new ArgumentException("Chunked encoding must be set with the SendChunked property");
				}
				if (!sendChunked)
				{
					throw new ArgumentException("SendChunked must be True", "value");
				}
				webHeaders.RemoveAndAdd("Transfer-Encoding", value);
			}
		}

		public override bool UseDefaultCredentials
		{
			get
			{
				return CredentialCache.DefaultCredentials == Credentials;
			}
			set
			{
				object obj;
				if (value)
				{
					ICredentials defaultCredentials = CredentialCache.DefaultCredentials;
					obj = defaultCredentials;
				}
				else
				{
					obj = null;
				}
				Credentials = (ICredentials)obj;
			}
		}

		public string UserAgent
		{
			get
			{
				return webHeaders["User-Agent"];
			}
			set
			{
				webHeaders.SetInternal("User-Agent", value);
			}
		}

		public bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return unsafe_auth_blah;
			}
			set
			{
				unsafe_auth_blah = value;
			}
		}

		internal bool GotRequestStream
		{
			get
			{
				return gotRequestStream;
			}
		}

		internal bool ExpectContinue
		{
			get
			{
				return expectContinue;
			}
			set
			{
				expectContinue = value;
			}
		}

		internal Uri AuthUri
		{
			get
			{
				return actualUri;
			}
		}

		internal bool ProxyQuery
		{
			get
			{
				return servicePoint.UsesProxy && !servicePoint.UseConnect;
			}
		}

		internal bool FinishedReading
		{
			get
			{
				return finished_reading;
			}
			set
			{
				finished_reading = value;
			}
		}

		internal bool Aborted
		{
			get
			{
				return Interlocked.CompareExchange(ref aborted, 0, 0) == 1;
			}
		}

		public HttpWebRequest(Uri uri)
		{
			requestUri = uri;
			actualUri = uri;
			proxy = GlobalProxySelection.Select;
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected HttpWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			requestUri = (Uri)serializationInfo.GetValue("requestUri", typeof(Uri));
			actualUri = (Uri)serializationInfo.GetValue("actualUri", typeof(Uri));
			allowAutoRedirect = serializationInfo.GetBoolean("allowAutoRedirect");
			allowBuffering = serializationInfo.GetBoolean("allowBuffering");
			certificates = (X509CertificateCollection)serializationInfo.GetValue("certificates", typeof(X509CertificateCollection));
			connectionGroup = serializationInfo.GetString("connectionGroup");
			contentLength = serializationInfo.GetInt64("contentLength");
			webHeaders = (WebHeaderCollection)serializationInfo.GetValue("webHeaders", typeof(WebHeaderCollection));
			keepAlive = serializationInfo.GetBoolean("keepAlive");
			maxAutoRedirect = serializationInfo.GetInt32("maxAutoRedirect");
			mediaType = serializationInfo.GetString("mediaType");
			method = serializationInfo.GetString("method");
			initialMethod = serializationInfo.GetString("initialMethod");
			pipelined = serializationInfo.GetBoolean("pipelined");
			version = (Version)serializationInfo.GetValue("version", typeof(Version));
			proxy = (IWebProxy)serializationInfo.GetValue("proxy", typeof(IWebProxy));
			sendChunked = serializationInfo.GetBoolean("sendChunked");
			timeout = serializationInfo.GetInt32("timeout");
			redirects = serializationInfo.GetInt32("redirects");
		}

		static HttpWebRequest()
		{
			defaultMaxResponseHeadersLength = 65536;
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			GetObjectData(serializationInfo, streamingContext);
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		internal ServicePoint GetServicePoint()
		{
			lock (locker)
			{
				if (hostChanged || servicePoint == null)
				{
					servicePoint = ServicePointManager.FindServicePoint(actualUri, proxy);
					hostChanged = false;
				}
			}
			return servicePoint;
		}

		public void AddRange(int range)
		{
			AddRange("bytes", range);
		}

		public void AddRange(int from, int to)
		{
			AddRange("bytes", from, to);
		}

		public void AddRange(string rangeSpecifier, int range)
		{
			if (rangeSpecifier == null)
			{
				throw new ArgumentNullException("rangeSpecifier");
			}
			string text = webHeaders["Range"];
			if (text == null || text.Length == 0)
			{
				text = rangeSpecifier + "=";
			}
			else
			{
				if (!text.ToLower().StartsWith(rangeSpecifier.ToLower() + "="))
				{
					throw new InvalidOperationException("rangeSpecifier");
				}
				text += ",";
			}
			webHeaders.RemoveAndAdd("Range", text + range + "-");
		}

		public void AddRange(string rangeSpecifier, int from, int to)
		{
			if (rangeSpecifier == null)
			{
				throw new ArgumentNullException("rangeSpecifier");
			}
			if (from < 0 || to < 0 || from > to)
			{
				throw new ArgumentOutOfRangeException();
			}
			string text = webHeaders["Range"];
			if (text == null || text.Length == 0)
			{
				text = rangeSpecifier + "=";
			}
			else
			{
				if (!text.ToLower().StartsWith(rangeSpecifier.ToLower() + "="))
				{
					throw new InvalidOperationException("rangeSpecifier");
				}
				text += ",";
			}
			webHeaders.RemoveAndAdd("Range", text + from + "-" + to);
		}

		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			if (Aborted)
			{
				throw new WebException("The request was canceled.", WebExceptionStatus.RequestCanceled);
			}
			bool flag = !(method == "GET") && !(method == "CONNECT") && !(method == "HEAD") && !(method == "TRACE") && !(method == "DELETE");
			if (method == null || !flag)
			{
				throw new ProtocolViolationException("Cannot send data when method is: " + method);
			}
			if (contentLength == -1 && !sendChunked && !allowBuffering && KeepAlive)
			{
				throw new ProtocolViolationException("Content-Length not set");
			}
			string transferEncoding = TransferEncoding;
			if (!sendChunked && transferEncoding != null && transferEncoding.Trim() != string.Empty)
			{
				throw new ProtocolViolationException("SendChunked should be true.");
			}
			lock (locker)
			{
				if (asyncWrite != null)
				{
					throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
				}
				asyncWrite = new System.Net.WebAsyncResult(this, callback, state);
				initialMethod = method;
				if (haveRequest && writeStream != null)
				{
					asyncWrite.SetCompleted(true, writeStream);
					asyncWrite.DoCallback();
					return asyncWrite;
				}
				gotRequestStream = true;
				System.Net.WebAsyncResult result = asyncWrite;
				if (!requestSent)
				{
					requestSent = true;
					redirects = 0;
					servicePoint = GetServicePoint();
					abortHandler = servicePoint.SendRequest(this, connectionGroup);
				}
				return result;
			}
		}

		public override Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			System.Net.WebAsyncResult webAsyncResult = asyncResult as System.Net.WebAsyncResult;
			if (webAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult");
			}
			asyncWrite = webAsyncResult;
			webAsyncResult.WaitUntilComplete();
			Exception exception = webAsyncResult.Exception;
			if (exception != null)
			{
				throw exception;
			}
			return webAsyncResult.WriteStream;
		}

		public override Stream GetRequestStream()
		{
			IAsyncResult asyncResult = asyncWrite;
			if (asyncResult == null)
			{
				asyncResult = BeginGetRequestStream(null, null);
				asyncWrite = (System.Net.WebAsyncResult)asyncResult;
			}
			if (!asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(timeout, false))
			{
				Abort();
				throw new WebException("The request timed out", WebExceptionStatus.Timeout);
			}
			return EndGetRequestStream(asyncResult);
		}

		private void CheckIfForceWrite()
		{
			if (writeStream != null && !writeStream.RequestWritten && contentLength >= 0 && InternalAllowBuffering && writeStream.WriteBufferLength == contentLength)
			{
				writeStream.WriteRequest();
			}
		}

		public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			if (Aborted)
			{
				throw new WebException("The request was canceled.", WebExceptionStatus.RequestCanceled);
			}
			if (method == null)
			{
				throw new ProtocolViolationException("Method is null.");
			}
			string transferEncoding = TransferEncoding;
			if (!sendChunked && transferEncoding != null && transferEncoding.Trim() != string.Empty)
			{
				throw new ProtocolViolationException("SendChunked should be true.");
			}
			Monitor.Enter(locker);
			getResponseCalled = true;
			if (asyncRead != null && !haveResponse)
			{
				Monitor.Exit(locker);
				throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
			}
			CheckIfForceWrite();
			asyncRead = new System.Net.WebAsyncResult(this, callback, state);
			System.Net.WebAsyncResult webAsyncResult = asyncRead;
			initialMethod = method;
			if (haveResponse)
			{
				Exception ex = saved_exc;
				if (webResponse != null)
				{
					Monitor.Exit(locker);
					if (ex == null)
					{
						webAsyncResult.SetCompleted(true, webResponse);
					}
					else
					{
						webAsyncResult.SetCompleted(true, ex);
					}
					webAsyncResult.DoCallback();
					return webAsyncResult;
				}
				if (ex != null)
				{
					Monitor.Exit(locker);
					webAsyncResult.SetCompleted(true, ex);
					webAsyncResult.DoCallback();
					return webAsyncResult;
				}
			}
			if (!requestSent)
			{
				requestSent = true;
				redirects = 0;
				servicePoint = GetServicePoint();
				abortHandler = servicePoint.SendRequest(this, connectionGroup);
			}
			Monitor.Exit(locker);
			return webAsyncResult;
		}

		public override WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			System.Net.WebAsyncResult webAsyncResult = asyncResult as System.Net.WebAsyncResult;
			if (webAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (!webAsyncResult.WaitUntilComplete(timeout, false))
			{
				Abort();
				throw new WebException("The request timed out", WebExceptionStatus.Timeout);
			}
			if (webAsyncResult.GotException)
			{
				throw webAsyncResult.Exception;
			}
			return webAsyncResult.Response;
		}

		public override WebResponse GetResponse()
		{
			System.Net.WebAsyncResult asyncResult = (System.Net.WebAsyncResult)BeginGetResponse(null, null);
			return EndGetResponse(asyncResult);
		}

		public override void Abort()
		{
			if (Interlocked.CompareExchange(ref aborted, 1, 0) == 1 || (haveResponse && finished_reading))
			{
				return;
			}
			haveResponse = true;
			if (abortHandler != null)
			{
				try
				{
					abortHandler(this, EventArgs.Empty);
				}
				catch (Exception)
				{
				}
				abortHandler = null;
			}
			if (asyncWrite != null)
			{
				System.Net.WebAsyncResult webAsyncResult = asyncWrite;
				if (!webAsyncResult.IsCompleted)
				{
					try
					{
						WebException e = new WebException("Aborted.", WebExceptionStatus.RequestCanceled);
						webAsyncResult.SetCompleted(false, e);
						webAsyncResult.DoCallback();
					}
					catch
					{
					}
				}
				asyncWrite = null;
			}
			if (asyncRead != null)
			{
				System.Net.WebAsyncResult webAsyncResult2 = asyncRead;
				if (!webAsyncResult2.IsCompleted)
				{
					try
					{
						WebException e2 = new WebException("Aborted.", WebExceptionStatus.RequestCanceled);
						webAsyncResult2.SetCompleted(false, e2);
						webAsyncResult2.DoCallback();
					}
					catch
					{
					}
				}
				asyncRead = null;
			}
			if (writeStream != null)
			{
				try
				{
					writeStream.Close();
					writeStream = null;
				}
				catch
				{
				}
			}
			if (webResponse == null)
			{
				return;
			}
			try
			{
				webResponse.Close();
				webResponse = null;
			}
			catch
			{
			}
		}

		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("requestUri", requestUri, typeof(Uri));
			serializationInfo.AddValue("actualUri", actualUri, typeof(Uri));
			serializationInfo.AddValue("allowAutoRedirect", allowAutoRedirect);
			serializationInfo.AddValue("allowBuffering", allowBuffering);
			serializationInfo.AddValue("certificates", certificates, typeof(X509CertificateCollection));
			serializationInfo.AddValue("connectionGroup", connectionGroup);
			serializationInfo.AddValue("contentLength", contentLength);
			serializationInfo.AddValue("webHeaders", webHeaders, typeof(WebHeaderCollection));
			serializationInfo.AddValue("keepAlive", keepAlive);
			serializationInfo.AddValue("maxAutoRedirect", maxAutoRedirect);
			serializationInfo.AddValue("mediaType", mediaType);
			serializationInfo.AddValue("method", method);
			serializationInfo.AddValue("initialMethod", initialMethod);
			serializationInfo.AddValue("pipelined", pipelined);
			serializationInfo.AddValue("version", version, typeof(Version));
			serializationInfo.AddValue("proxy", proxy, typeof(IWebProxy));
			serializationInfo.AddValue("sendChunked", sendChunked);
			serializationInfo.AddValue("timeout", timeout);
			serializationInfo.AddValue("redirects", redirects);
		}

		private void CheckRequestStarted()
		{
			if (requestSent)
			{
				throw new InvalidOperationException("request started");
			}
		}

		internal void DoContinueDelegate(int statusCode, WebHeaderCollection headers)
		{
			if (continueDelegate != null)
			{
				continueDelegate(statusCode, headers);
			}
		}

		private bool Redirect(System.Net.WebAsyncResult result, HttpStatusCode code)
		{
			redirects++;
			Exception ex = null;
			string text = null;
			switch (code)
			{
			case HttpStatusCode.MultipleChoices:
				ex = new WebException("Ambiguous redirect.");
				break;
			case HttpStatusCode.MovedPermanently:
			case HttpStatusCode.Found:
			case HttpStatusCode.TemporaryRedirect:
				contentLength = -1L;
				bodyBufferLength = 0;
				bodyBuffer = null;
				method = "GET";
				text = webResponse.Headers["Location"];
				break;
			case HttpStatusCode.SeeOther:
				method = "GET";
				text = webResponse.Headers["Location"];
				break;
			case HttpStatusCode.NotModified:
				return false;
			case HttpStatusCode.UseProxy:
				ex = new NotImplementedException("Proxy support not available.");
				break;
			default:
				ex = new ProtocolViolationException("Invalid status code: " + (int)code);
				break;
			}
			if (ex != null)
			{
				throw ex;
			}
			if (text == null)
			{
				throw new WebException("No Location header found for " + (int)code, WebExceptionStatus.ProtocolError);
			}
			Uri uri = actualUri;
			try
			{
				actualUri = new Uri(actualUri, text);
			}
			catch (Exception)
			{
				throw new WebException(string.Format("Invalid URL ({0}) for {1}", text, (int)code), WebExceptionStatus.ProtocolError);
			}
			hostChanged = actualUri.Scheme != uri.Scheme || actualUri.Host != uri.Host || actualUri.Port != uri.Port;
			return true;
		}

		private string GetHeaders()
		{
			bool flag = false;
			if (sendChunked)
			{
				flag = true;
				webHeaders.RemoveAndAdd("Transfer-Encoding", "chunked");
				webHeaders.RemoveInternal("Content-Length");
			}
			else if (contentLength != -1)
			{
				if (contentLength > 0)
				{
					flag = true;
				}
				webHeaders.SetInternal("Content-Length", contentLength.ToString());
				webHeaders.RemoveInternal("Transfer-Encoding");
			}
			if (actualVersion == HttpVersion.Version11 && flag && servicePoint.SendContinue)
			{
				webHeaders.RemoveAndAdd("Expect", "100-continue");
				expectContinue = true;
			}
			else
			{
				webHeaders.RemoveInternal("Expect");
				expectContinue = false;
			}
			bool proxyQuery = ProxyQuery;
			string name = ((!proxyQuery) ? "Connection" : "Proxy-Connection");
			webHeaders.RemoveInternal(proxyQuery ? "Connection" : "Proxy-Connection");
			Version protocolVersion = servicePoint.ProtocolVersion;
			bool flag2 = protocolVersion == null || protocolVersion == HttpVersion.Version10;
			if (keepAlive && (version == HttpVersion.Version10 || flag2))
			{
				webHeaders.RemoveAndAdd(name, "keep-alive");
			}
			else if (!keepAlive && version == HttpVersion.Version11)
			{
				webHeaders.RemoveAndAdd(name, "close");
			}
			webHeaders.SetInternal("Host", actualUri.Authority);
			if (cookieContainer != null)
			{
				string cookieHeader = cookieContainer.GetCookieHeader(actualUri);
				if (cookieHeader != string.Empty)
				{
					webHeaders.SetInternal("Cookie", cookieHeader);
				}
			}
			string text = null;
			if ((auto_decomp & DecompressionMethods.GZip) != DecompressionMethods.None)
			{
				text = "gzip";
			}
			if ((auto_decomp & DecompressionMethods.Deflate) != DecompressionMethods.None)
			{
				text = ((text == null) ? "deflate" : "gzip, deflate");
			}
			if (text != null)
			{
				webHeaders.RemoveAndAdd("Accept-Encoding", text);
			}
			if (!usedPreAuth && preAuthenticate)
			{
				DoPreAuthenticate();
			}
			return webHeaders.ToString();
		}

		private void DoPreAuthenticate()
		{
			bool flag = proxy != null && !proxy.IsBypassed(actualUri);
			ICredentials obj;
			if (!flag || this.credentials != null)
			{
				ICredentials credentials = this.credentials;
				obj = credentials;
			}
			else
			{
				obj = proxy.Credentials;
			}
			ICredentials credentials2 = obj;
			Authorization authorization = AuthenticationManager.PreAuthenticate(this, credentials2);
			if (authorization != null)
			{
				webHeaders.RemoveInternal("Proxy-Authorization");
				webHeaders.RemoveInternal("Authorization");
				string name = ((!flag || this.credentials != null) ? "Authorization" : "Proxy-Authorization");
				webHeaders[name] = authorization.Message;
				usedPreAuth = true;
			}
		}

		internal void SetWriteStreamError(WebExceptionStatus status, Exception exc)
		{
			if (Aborted)
			{
				return;
			}
			System.Net.WebAsyncResult webAsyncResult = asyncWrite;
			if (webAsyncResult == null)
			{
				webAsyncResult = asyncRead;
			}
			if (webAsyncResult != null)
			{
				WebException e;
				if (exc == null)
				{
					string message = "Error: " + status;
					e = new WebException(message, status);
				}
				else
				{
					string message = string.Format("Error: {0} ({1})", status, exc.Message);
					e = new WebException(message, exc, status);
				}
				webAsyncResult.SetCompleted(false, e);
				webAsyncResult.DoCallback();
			}
		}

		internal void SendRequestHeaders(bool propagate_error)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = ((!ProxyQuery) ? actualUri.PathAndQuery : ((!actualUri.IsDefaultPort) ? string.Format("{0}://{1}:{2}{3}", actualUri.Scheme, actualUri.Host, actualUri.Port, actualUri.PathAndQuery) : string.Format("{0}://{1}{2}", actualUri.Scheme, actualUri.Host, actualUri.PathAndQuery)));
			if (servicePoint.ProtocolVersion != null && servicePoint.ProtocolVersion < version)
			{
				actualVersion = servicePoint.ProtocolVersion;
			}
			else
			{
				actualVersion = version;
			}
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}.{3}\r\n", method, text, actualVersion.Major, actualVersion.Minor);
			stringBuilder.Append(GetHeaders());
			string s = stringBuilder.ToString();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			try
			{
				writeStream.SetHeaders(bytes);
			}
			catch (WebException ex)
			{
				SetWriteStreamError(ex.Status, ex);
				if (propagate_error)
				{
					throw;
				}
			}
			catch (Exception exc)
			{
				SetWriteStreamError(WebExceptionStatus.SendFailure, exc);
				if (propagate_error)
				{
					throw;
				}
			}
		}

		internal void SetWriteStream(System.Net.WebConnectionStream stream)
		{
			if (!Aborted)
			{
				writeStream = stream;
				if (bodyBuffer != null)
				{
					webHeaders.RemoveInternal("Transfer-Encoding");
					contentLength = bodyBufferLength;
					writeStream.SendChunked = false;
				}
				SendRequestHeaders(false);
				haveRequest = true;
				if (bodyBuffer != null)
				{
					writeStream.Write(bodyBuffer, 0, bodyBufferLength);
					bodyBuffer = null;
					writeStream.Close();
				}
				else if (method != "HEAD" && method != "GET" && method != "MKCOL" && method != "CONNECT" && method != "DELETE" && method != "TRACE" && getResponseCalled && !writeStream.RequestWritten)
				{
					writeStream.WriteRequest();
				}
				if (asyncWrite != null)
				{
					asyncWrite.SetCompleted(false, stream);
					asyncWrite.DoCallback();
					asyncWrite = null;
				}
			}
		}

		internal void SetResponseError(WebExceptionStatus status, Exception e, string where)
		{
			if (Aborted)
			{
				return;
			}
			lock (locker)
			{
				string message = string.Format("Error getting response stream ({0}): {1}", where, status);
				System.Net.WebAsyncResult webAsyncResult = asyncRead;
				if (webAsyncResult == null)
				{
					webAsyncResult = asyncWrite;
				}
				WebException e2 = ((!(e is WebException)) ? new WebException(message, e, status, null) : ((WebException)e));
				if (webAsyncResult != null)
				{
					if (!webAsyncResult.IsCompleted)
					{
						webAsyncResult.SetCompleted(false, e2);
						webAsyncResult.DoCallback();
					}
					else if (webAsyncResult == asyncWrite)
					{
						saved_exc = e2;
					}
					haveResponse = true;
					asyncRead = null;
					asyncWrite = null;
				}
				else
				{
					haveResponse = true;
					saved_exc = e2;
				}
			}
		}

		private void CheckSendError(System.Net.WebConnectionData data)
		{
			int statusCode = data.StatusCode;
			if (statusCode >= 400 && statusCode != 401 && statusCode != 407 && writeStream != null && asyncRead == null && !writeStream.CompleteRequestWritten)
			{
				saved_exc = new WebException(data.StatusDescription, null, WebExceptionStatus.ProtocolError, webResponse);
				webResponse.ReadAll();
			}
		}

		private void HandleNtlmAuth(System.Net.WebAsyncResult r)
		{
			System.Net.WebConnectionStream webConnectionStream = webResponse.GetResponseStream() as System.Net.WebConnectionStream;
			if (webConnectionStream != null)
			{
				System.Net.WebConnection connection = webConnectionStream.Connection;
				connection.PriorityRequest = this;
				ICredentials obj;
				if (proxy == null || proxy.IsBypassed(actualUri))
				{
					ICredentials credentials = this.credentials;
					obj = credentials;
				}
				else
				{
					obj = proxy.Credentials;
				}
				ICredentials credentials2 = obj;
				if (credentials2 != null)
				{
					connection.NtlmCredential = credentials2.GetCredential(requestUri, "NTLM");
					connection.UnsafeAuthenticatedConnectionSharing = unsafe_auth_blah;
				}
			}
			r.Reset();
			haveResponse = false;
			webResponse.ReadAll();
			webResponse = null;
		}

		internal void SetResponseData(System.Net.WebConnectionData data)
		{
			lock (locker)
			{
				if (Aborted)
				{
					if (data.stream != null)
					{
						data.stream.Close();
					}
					return;
				}
				WebException ex = null;
				try
				{
					webResponse = new HttpWebResponse(actualUri, method, data, cookieContainer);
				}
				catch (Exception ex2)
				{
					ex = new WebException(ex2.Message, ex2, WebExceptionStatus.ProtocolError, null);
					if (data.stream != null)
					{
						data.stream.Close();
					}
				}
				if (ex == null && (method == "POST" || method == "PUT"))
				{
					lock (locker)
					{
						CheckSendError(data);
						if (saved_exc != null)
						{
							ex = (WebException)saved_exc;
						}
					}
				}
				System.Net.WebAsyncResult webAsyncResult = asyncRead;
				bool flag = false;
				if (webAsyncResult == null && webResponse != null)
				{
					flag = true;
					webAsyncResult = new System.Net.WebAsyncResult(null, null);
					webAsyncResult.SetCompleted(false, webResponse);
				}
				if (webAsyncResult == null)
				{
					return;
				}
				if (ex != null)
				{
					webAsyncResult.SetCompleted(false, ex);
					webAsyncResult.DoCallback();
					return;
				}
				try
				{
					if (!CheckFinalStatus(webAsyncResult))
					{
						if (is_ntlm_auth && authCompleted && webResponse != null && webResponse.StatusCode < HttpStatusCode.BadRequest)
						{
							System.Net.WebConnectionStream webConnectionStream = webResponse.GetResponseStream() as System.Net.WebConnectionStream;
							if (webConnectionStream != null)
							{
								System.Net.WebConnection connection = webConnectionStream.Connection;
								connection.NtlmAuthenticated = true;
							}
						}
						if (writeStream != null)
						{
							writeStream.KillBuffer();
						}
						haveResponse = true;
						webAsyncResult.SetCompleted(false, webResponse);
						webAsyncResult.DoCallback();
						return;
					}
					if (webResponse != null)
					{
						if (is_ntlm_auth)
						{
							HandleNtlmAuth(webAsyncResult);
							return;
						}
						webResponse.Close();
					}
					finished_reading = false;
					haveResponse = false;
					webResponse = null;
					webAsyncResult.Reset();
					servicePoint = GetServicePoint();
					abortHandler = servicePoint.SendRequest(this, connectionGroup);
				}
				catch (WebException e)
				{
					if (flag)
					{
						saved_exc = e;
						haveResponse = true;
					}
					webAsyncResult.SetCompleted(false, e);
					webAsyncResult.DoCallback();
				}
				catch (Exception ex3)
				{
					ex = new WebException(ex3.Message, ex3, WebExceptionStatus.ProtocolError, null);
					if (flag)
					{
						saved_exc = ex;
						haveResponse = true;
					}
					webAsyncResult.SetCompleted(false, ex);
					webAsyncResult.DoCallback();
				}
			}
		}

		private bool CheckAuthorization(WebResponse response, HttpStatusCode code)
		{
			authCompleted = false;
			if (code == HttpStatusCode.Unauthorized && this.credentials == null)
			{
				return false;
			}
			bool flag = code == HttpStatusCode.ProxyAuthenticationRequired;
			if (flag && (proxy == null || proxy.Credentials == null))
			{
				return false;
			}
			string[] values = response.Headers.GetValues((!flag) ? "WWW-Authenticate" : "Proxy-Authenticate");
			if (values == null || values.Length == 0)
			{
				return false;
			}
			ICredentials obj;
			if (!flag)
			{
				ICredentials credentials = this.credentials;
				obj = credentials;
			}
			else
			{
				obj = proxy.Credentials;
			}
			ICredentials credentials2 = obj;
			Authorization authorization = null;
			string[] array = values;
			foreach (string challenge in array)
			{
				authorization = AuthenticationManager.Authenticate(challenge, this, credentials2);
				if (authorization != null)
				{
					break;
				}
			}
			if (authorization == null)
			{
				return false;
			}
			webHeaders[(!flag) ? "Authorization" : "Proxy-Authorization"] = authorization.Message;
			authCompleted = authorization.Complete;
			is_ntlm_auth = authorization.Module.AuthenticationType == "NTLM";
			return true;
		}

		private bool CheckFinalStatus(System.Net.WebAsyncResult result)
		{
			if (result.GotException)
			{
				throw result.Exception;
			}
			Exception ex = result.Exception;
			bodyBuffer = null;
			HttpWebResponse response = result.Response;
			WebExceptionStatus status = WebExceptionStatus.ProtocolError;
			HttpStatusCode httpStatusCode = (HttpStatusCode)0;
			if (ex == null && webResponse != null)
			{
				httpStatusCode = webResponse.StatusCode;
				if (!authCompleted && ((httpStatusCode == HttpStatusCode.Unauthorized && credentials != null) || (ProxyQuery && httpStatusCode == HttpStatusCode.ProxyAuthenticationRequired)) && !usedPreAuth && CheckAuthorization(webResponse, httpStatusCode))
				{
					if (InternalAllowBuffering)
					{
						bodyBuffer = writeStream.WriteBuffer;
						bodyBufferLength = writeStream.WriteBufferLength;
						return true;
					}
					if (method != "PUT" && method != "POST")
					{
						return true;
					}
					writeStream.InternalClose();
					writeStream = null;
					webResponse.Close();
					webResponse = null;
					throw new WebException("This request requires buffering of data for authentication or redirection to be sucessful.");
				}
				if (httpStatusCode >= HttpStatusCode.BadRequest)
				{
					string message = string.Format("The remote server returned an error: ({0}) {1}.", (int)httpStatusCode, webResponse.StatusDescription);
					ex = new WebException(message, null, status, webResponse);
					webResponse.ReadAll();
				}
				else if (httpStatusCode == HttpStatusCode.NotModified && allowAutoRedirect)
				{
					string message2 = string.Format("The remote server returned an error: ({0}) {1}.", (int)httpStatusCode, webResponse.StatusDescription);
					ex = new WebException(message2, null, status, webResponse);
				}
				else if (httpStatusCode >= HttpStatusCode.MultipleChoices && allowAutoRedirect && redirects >= maxAutoRedirect)
				{
					ex = new WebException("Max. redirections exceeded.", null, status, webResponse);
					webResponse.ReadAll();
				}
			}
			if (ex == null)
			{
				bool result2 = false;
				int num = (int)httpStatusCode;
				if (allowAutoRedirect && num >= 300)
				{
					if (InternalAllowBuffering && writeStream.WriteBufferLength > 0)
					{
						bodyBuffer = writeStream.WriteBuffer;
						bodyBufferLength = writeStream.WriteBufferLength;
					}
					result2 = Redirect(result, httpStatusCode);
				}
				if (response != null && num >= 300 && num != 304)
				{
					response.ReadAll();
				}
				return result2;
			}
			if (writeStream != null)
			{
				writeStream.InternalClose();
				writeStream = null;
			}
			webResponse = null;
			throw ex;
		}
	}
}
