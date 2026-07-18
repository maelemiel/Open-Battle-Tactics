using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Net
{
	[Serializable]
	public class FileWebRequest : WebRequest, ISerializable
	{
		internal class FileWebStream : FileStream
		{
			private FileWebRequest webRequest;

			internal FileWebStream(FileWebRequest webRequest, FileMode mode, FileAccess access, FileShare share)
				: base(webRequest.RequestUri.LocalPath, mode, access, share)
			{
				this.webRequest = webRequest;
			}

			public override void Close()
			{
				base.Close();
				FileWebRequest fileWebRequest = webRequest;
				webRequest = null;
				if (fileWebRequest != null)
				{
					fileWebRequest.Close();
				}
			}
		}

		private delegate Stream GetRequestStreamCallback();

		private delegate WebResponse GetResponseCallback();

		private Uri uri;

		private WebHeaderCollection webHeaders;

		private ICredentials credentials;

		private string connectionGroup;

		private long contentLength;

		private FileAccess fileAccess = FileAccess.Read;

		private string method = "GET";

		private IWebProxy proxy;

		private bool preAuthenticate;

		private int timeout = 100000;

		private Stream requestStream;

		private FileWebResponse webResponse;

		private AutoResetEvent requestEndEvent;

		private bool requesting;

		private bool asyncResponding;

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
				if (value < 0)
				{
					throw new ArgumentException("The Content-Length value must be greater than or equal to zero.", "value");
				}
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
				webHeaders["Content-Type"] = value;
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

		public override WebHeaderCollection Headers
		{
			get
			{
				return webHeaders;
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
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException("Cannot set null or blank methods on request.", "value");
				}
				method = value;
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

		public override IWebProxy Proxy
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

		public override Uri RequestUri
		{
			get
			{
				return uri;
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
					throw new ArgumentOutOfRangeException("Timeout can be only set to 'System.Threading.Timeout.Infinite' or a value >= 0.");
				}
				timeout = value;
			}
		}

		public override bool UseDefaultCredentials
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal FileWebRequest(Uri uri)
		{
			this.uri = uri;
			webHeaders = new WebHeaderCollection();
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected FileWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			webHeaders = (WebHeaderCollection)serializationInfo.GetValue("headers", typeof(WebHeaderCollection));
			proxy = (IWebProxy)serializationInfo.GetValue("proxy", typeof(IWebProxy));
			uri = (Uri)serializationInfo.GetValue("uri", typeof(Uri));
			connectionGroup = serializationInfo.GetString("connectionGroupName");
			method = serializationInfo.GetString("method");
			contentLength = serializationInfo.GetInt64("contentLength");
			timeout = serializationInfo.GetInt32("timeout");
			fileAccess = (FileAccess)(int)serializationInfo.GetValue("fileAccess", typeof(FileAccess));
			preAuthenticate = serializationInfo.GetBoolean("preauthenticate");
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			GetObjectData(serializationInfo, streamingContext);
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[System.MonoTODO]
		public override void Abort()
		{
			throw GetMustImplement();
		}

		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			if (string.Compare("GET", method, true) == 0 || string.Compare("HEAD", method, true) == 0 || string.Compare("CONNECT", method, true) == 0)
			{
				throw new ProtocolViolationException("Cannot send a content-body with this verb-type.");
			}
			lock (this)
			{
				if (asyncResponding || webResponse != null)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				if (requesting)
				{
					throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
				}
				requesting = true;
			}
			GetRequestStreamCallback getRequestStreamCallback = GetRequestStreamInternal;
			return getRequestStreamCallback.BeginInvoke(callback, state);
		}

		public override Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			GetRequestStreamCallback getRequestStreamCallback = (GetRequestStreamCallback)asyncResult2.AsyncDelegate;
			return getRequestStreamCallback.EndInvoke(asyncResult);
		}

		public override Stream GetRequestStream()
		{
			IAsyncResult asyncResult = BeginGetRequestStream(null, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(timeout, false))
			{
				throw new WebException("The request timed out", WebExceptionStatus.Timeout);
			}
			return EndGetRequestStream(asyncResult);
		}

		internal Stream GetRequestStreamInternal()
		{
			requestStream = new FileWebStream(this, FileMode.Create, FileAccess.Write, FileShare.Read);
			return requestStream;
		}

		public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			lock (this)
			{
				if (asyncResponding)
				{
					throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
				}
				asyncResponding = true;
			}
			GetResponseCallback getResponseCallback = GetResponseInternal;
			return getResponseCallback.BeginInvoke(callback, state);
		}

		public override WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			GetResponseCallback getResponseCallback = (GetResponseCallback)asyncResult2.AsyncDelegate;
			WebResponse result = getResponseCallback.EndInvoke(asyncResult);
			asyncResponding = false;
			return result;
		}

		public override WebResponse GetResponse()
		{
			IAsyncResult asyncResult = BeginGetResponse(null, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(timeout, false))
			{
				throw new WebException("The request timed out", WebExceptionStatus.Timeout);
			}
			return EndGetResponse(asyncResult);
		}

		private WebResponse GetResponseInternal()
		{
			if (webResponse != null)
			{
				return webResponse;
			}
			lock (this)
			{
				if (requesting)
				{
					requestEndEvent = new AutoResetEvent(false);
				}
			}
			if (requestEndEvent != null)
			{
				requestEndEvent.WaitOne();
			}
			FileStream fileStream = null;
			try
			{
				fileStream = new FileWebStream(this, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception ex)
			{
				throw new WebException(ex.Message, ex);
			}
			webResponse = new FileWebResponse(uri, fileStream);
			return webResponse;
		}

		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("headers", webHeaders, typeof(WebHeaderCollection));
			serializationInfo.AddValue("proxy", proxy, typeof(IWebProxy));
			serializationInfo.AddValue("uri", uri, typeof(Uri));
			serializationInfo.AddValue("connectionGroupName", connectionGroup);
			serializationInfo.AddValue("method", method);
			serializationInfo.AddValue("contentLength", contentLength);
			serializationInfo.AddValue("timeout", timeout);
			serializationInfo.AddValue("fileAccess", fileAccess);
			serializationInfo.AddValue("preauthenticate", false);
		}

		internal void Close()
		{
			lock (this)
			{
				requesting = false;
				if (requestEndEvent != null)
				{
					requestEndEvent.Set();
				}
			}
		}
	}
}
