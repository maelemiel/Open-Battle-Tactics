using System.IO;
using System.Net.Cache;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace System.Net
{
	public sealed class FtpWebRequest : WebRequest
	{
		private enum RequestState
		{
			Before = 0,
			Scheduled = 1,
			Connecting = 2,
			Authenticating = 3,
			OpeningData = 4,
			TransferInProgress = 5,
			Finished = 6,
			Aborted = 7,
			Error = 8
		}

		private const string ChangeDir = "CWD";

		private const string UserCommand = "USER";

		private const string PasswordCommand = "PASS";

		private const string TypeCommand = "TYPE";

		private const string PassiveCommand = "PASV";

		private const string PortCommand = "PORT";

		private const string AbortCommand = "ABOR";

		private const string AuthCommand = "AUTH";

		private const string RestCommand = "REST";

		private const string RenameFromCommand = "RNFR";

		private const string RenameToCommand = "RNTO";

		private const string QuitCommand = "QUIT";

		private const string EOL = "\r\n";

		private Uri requestUri;

		private string file_name;

		private ServicePoint servicePoint;

		private Stream origDataStream;

		private Stream dataStream;

		private Stream controlStream;

		private StreamReader controlReader;

		private NetworkCredential credentials;

		private IPHostEntry hostEntry;

		private IPEndPoint localEndPoint;

		private IWebProxy proxy;

		private int timeout = 100000;

		private int rwTimeout = 300000;

		private long offset;

		private bool binary = true;

		private bool enableSsl;

		private bool usePassive = true;

		private bool keepAlive;

		private string method = "RETR";

		private string renameTo;

		private object locker = new object();

		private RequestState requestState;

		private System.Net.FtpAsyncResult asyncResult;

		private FtpWebResponse ftpResponse;

		private Stream requestStream;

		private string initial_path;

		private static readonly string[] supportedCommands = new string[13]
		{
			"APPE", "DELE", "LIST", "MDTM", "MKD", "NLST", "PWD", "RENAME", "RETR", "RMD",
			"SIZE", "STOR", "STOU"
		};

		private RemoteCertificateValidationCallback callback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (ServicePointManager.ServerCertificateValidationCallback != null)
			{
				return ServicePointManager.ServerCertificateValidationCallback(sender, certificate, chain, sslPolicyErrors);
			}
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				throw new InvalidOperationException("SSL authentication error: " + sslPolicyErrors);
			}
			return true;
		};

		[System.MonoTODO]
		public X509CertificateCollection ClientCertificates
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
		public override string ConnectionGroupName
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

		public override string ContentType
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

		public override long ContentLength
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long ContentOffset
		{
			get
			{
				return offset;
			}
			set
			{
				CheckRequestStarted();
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				offset = value;
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
				CheckRequestStarted();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (!(value is NetworkCredential))
				{
					throw new ArgumentException();
				}
				credentials = value as NetworkCredential;
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

		public bool EnableSsl
		{
			get
			{
				return enableSsl;
			}
			set
			{
				CheckRequestStarted();
				enableSsl = value;
			}
		}

		[System.MonoTODO]
		public override WebHeaderCollection Headers
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

		[System.MonoTODO("We don't support KeepAlive = true")]
		public bool KeepAlive
		{
			get
			{
				return keepAlive;
			}
			set
			{
				CheckRequestStarted();
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
				CheckRequestStarted();
				if (value == null)
				{
					throw new ArgumentNullException("Method string cannot be null");
				}
				if (value.Length == 0 || Array.BinarySearch(supportedCommands, value) < 0)
				{
					throw new ArgumentException("Method not supported", "value");
				}
				method = value;
			}
		}

		public override bool PreAuthenticate
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

		public override IWebProxy Proxy
		{
			get
			{
				return proxy;
			}
			set
			{
				CheckRequestStarted();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				proxy = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return rwTimeout;
			}
			set
			{
				CheckRequestStarted();
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException();
				}
				rwTimeout = value;
			}
		}

		public string RenameTo
		{
			get
			{
				return renameTo;
			}
			set
			{
				CheckRequestStarted();
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException("RenameTo value can't be null or empty", "RenameTo");
				}
				renameTo = value;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return requestUri;
			}
		}

		public ServicePoint ServicePoint
		{
			get
			{
				return GetServicePoint();
			}
		}

		public bool UsePassive
		{
			get
			{
				return usePassive;
			}
			set
			{
				CheckRequestStarted();
				usePassive = value;
			}
		}

		[System.MonoTODO]
		public override bool UseDefaultCredentials
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

		public bool UseBinary
		{
			get
			{
				return binary;
			}
			set
			{
				CheckRequestStarted();
				binary = value;
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
				CheckRequestStarted();
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException();
				}
				timeout = value;
			}
		}

		private string DataType
		{
			get
			{
				return (!binary) ? "A" : "I";
			}
		}

		private RequestState State
		{
			get
			{
				lock (locker)
				{
					return requestState;
				}
			}
			set
			{
				lock (locker)
				{
					CheckIfAborted();
					CheckFinalState();
					requestState = value;
				}
			}
		}

		internal FtpWebRequest(Uri uri)
		{
			requestUri = uri;
			proxy = GlobalProxySelection.Select;
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		public override void Abort()
		{
			lock (locker)
			{
				if (State == RequestState.TransferInProgress)
				{
					SendCommand(false, "ABOR");
				}
				if (!InFinalState())
				{
					State = RequestState.Aborted;
					ftpResponse = new FtpWebResponse(this, requestUri, method, FtpStatusCode.FileActionAborted, "Aborted by request");
				}
			}
		}

		public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			if (asyncResult != null && !asyncResult.IsCompleted)
			{
				throw new InvalidOperationException("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress");
			}
			CheckIfAborted();
			asyncResult = new System.Net.FtpAsyncResult(callback, state);
			lock (locker)
			{
				if (InFinalState())
				{
					asyncResult.SetCompleted(true, ftpResponse);
				}
				else
				{
					if (State == RequestState.Before)
					{
						State = RequestState.Scheduled;
					}
					Thread thread = new Thread(ProcessRequest);
					thread.Start();
				}
			}
			return asyncResult;
		}

		public override WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("AsyncResult cannot be null!");
			}
			if (!(asyncResult is System.Net.FtpAsyncResult) || asyncResult != this.asyncResult)
			{
				throw new ArgumentException("AsyncResult is from another request!");
			}
			System.Net.FtpAsyncResult ftpAsyncResult = (System.Net.FtpAsyncResult)asyncResult;
			if (!ftpAsyncResult.WaitUntilComplete(timeout, false))
			{
				Abort();
				throw new WebException("Transfer timed out.", WebExceptionStatus.Timeout);
			}
			CheckIfAborted();
			asyncResult = null;
			if (ftpAsyncResult.GotException)
			{
				throw ftpAsyncResult.Exception;
			}
			return ftpAsyncResult.Response;
		}

		public override WebResponse GetResponse()
		{
			IAsyncResult asyncResult = BeginGetResponse(null, null);
			return EndGetResponse(asyncResult);
		}

		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			if (method != "STOR" && method != "STOU" && method != "APPE")
			{
				throw new ProtocolViolationException();
			}
			lock (locker)
			{
				CheckIfAborted();
				if (State != RequestState.Before)
				{
					throw new InvalidOperationException("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress");
				}
				State = RequestState.Scheduled;
			}
			asyncResult = new System.Net.FtpAsyncResult(callback, state);
			Thread thread = new Thread(ProcessRequest);
			thread.Start();
			return asyncResult;
		}

		public override Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!(asyncResult is System.Net.FtpAsyncResult))
			{
				throw new ArgumentException("asyncResult");
			}
			if (State == RequestState.Aborted)
			{
				throw new WebException("Request aborted", WebExceptionStatus.RequestCanceled);
			}
			if (asyncResult != this.asyncResult)
			{
				throw new ArgumentException("AsyncResult is from another request!");
			}
			System.Net.FtpAsyncResult ftpAsyncResult = (System.Net.FtpAsyncResult)asyncResult;
			if (!ftpAsyncResult.WaitUntilComplete(timeout, false))
			{
				Abort();
				throw new WebException("Request timed out");
			}
			if (ftpAsyncResult.GotException)
			{
				throw ftpAsyncResult.Exception;
			}
			return ftpAsyncResult.Stream;
		}

		public override Stream GetRequestStream()
		{
			IAsyncResult asyncResult = BeginGetRequestStream(null, null);
			return EndGetRequestStream(asyncResult);
		}

		private ServicePoint GetServicePoint()
		{
			if (servicePoint == null)
			{
				servicePoint = ServicePointManager.FindServicePoint(requestUri, proxy);
			}
			return servicePoint;
		}

		private void ResolveHost()
		{
			CheckIfAborted();
			hostEntry = GetServicePoint().HostEntry;
			if (hostEntry == null)
			{
				ftpResponse.UpdateStatus(new System.Net.FtpStatus(FtpStatusCode.ActionAbortedLocalProcessingError, "Cannot resolve server name"));
				throw new WebException("The remote server name could not be resolved: " + requestUri, null, WebExceptionStatus.NameResolutionFailure, ftpResponse);
			}
		}

		private void ProcessRequest()
		{
			if (State == RequestState.Scheduled)
			{
				ftpResponse = new FtpWebResponse(this, requestUri, method, keepAlive);
				try
				{
					ProcessMethod();
					asyncResult.SetCompleted(false, ftpResponse);
					return;
				}
				catch (Exception completeWithError)
				{
					State = RequestState.Error;
					SetCompleteWithError(completeWithError);
					return;
				}
			}
			if (InProgress())
			{
				System.Net.FtpStatus responseStatus = GetResponseStatus();
				ftpResponse.UpdateStatus(responseStatus);
				if (ftpResponse.IsFinal())
				{
					State = RequestState.Finished;
				}
			}
			asyncResult.SetCompleted(false, ftpResponse);
		}

		private void SetType()
		{
			if (binary)
			{
				System.Net.FtpStatus ftpStatus = SendCommand("TYPE", DataType);
				if (ftpStatus.StatusCode < FtpStatusCode.CommandOK || ftpStatus.StatusCode >= (FtpStatusCode)300)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
			}
		}

		private string GetRemoteFolderPath(Uri uri)
		{
			string text = Uri.UnescapeDataString(uri.LocalPath);
			string text2;
			if (initial_path == null || initial_path == "/")
			{
				text2 = text;
			}
			else
			{
				if (text[0] == '/')
				{
					text = text.Substring(1);
				}
				Uri baseUri = new Uri("ftp://dummy-host" + initial_path);
				text2 = new Uri(baseUri, text).LocalPath;
			}
			int num = text2.LastIndexOf('/');
			if (num == -1)
			{
				return null;
			}
			return text2.Substring(0, num + 1);
		}

		private void CWDAndSetFileName(Uri uri)
		{
			string remoteFolderPath = GetRemoteFolderPath(uri);
			if (remoteFolderPath != null)
			{
				System.Net.FtpStatus ftpStatus = SendCommand("CWD", remoteFolderPath);
				if (ftpStatus.StatusCode < FtpStatusCode.CommandOK || ftpStatus.StatusCode >= (FtpStatusCode)300)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				int num = uri.LocalPath.LastIndexOf('/');
				if (num >= 0)
				{
					file_name = Uri.UnescapeDataString(uri.LocalPath.Substring(num + 1));
				}
			}
		}

		private void ProcessMethod()
		{
			State = RequestState.Connecting;
			ResolveHost();
			OpenControlConnection();
			CWDAndSetFileName(requestUri);
			SetType();
			switch (method)
			{
			case "RETR":
			case "NLST":
			case "LIST":
				DownloadData();
				break;
			case "APPE":
			case "STOR":
			case "STOU":
				UploadData();
				break;
			case "SIZE":
			case "MDTM":
			case "PWD":
			case "MKD":
			case "RENAME":
			case "DELE":
				ProcessSimpleMethod();
				break;
			default:
				throw new Exception(string.Format("Support for command {0} not implemented yet", method));
			}
			CheckIfAborted();
		}

		private void CloseControlConnection()
		{
			if (controlStream != null)
			{
				SendCommand("QUIT");
				controlStream.Close();
				controlStream = null;
			}
		}

		internal void CloseDataConnection()
		{
			if (origDataStream != null)
			{
				origDataStream.Close();
				origDataStream = null;
			}
		}

		private void CloseConnection()
		{
			CloseControlConnection();
			CloseDataConnection();
		}

		private void ProcessSimpleMethod()
		{
			State = RequestState.TransferInProgress;
			if (method == "PWD")
			{
				method = "PWD";
			}
			if (method == "RENAME")
			{
				method = "RNFR";
			}
			System.Net.FtpStatus ftpStatus = SendCommand(method, file_name);
			ftpResponse.Stream = Stream.Null;
			string statusDescription = ftpStatus.StatusDescription;
			switch (method)
			{
			case "SIZE":
			{
				if (ftpStatus.StatusCode != FtpStatusCode.FileStatus)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				int num = 4;
				int num2 = 0;
				while (num < statusDescription.Length && char.IsDigit(statusDescription[num]))
				{
					num++;
					num2++;
				}
				if (num2 == 0)
				{
					throw new WebException("Bad format for server response in " + method);
				}
				long result;
				if (!long.TryParse(statusDescription.Substring(4, num2), out result))
				{
					throw new WebException("Bad format for server response in " + method);
				}
				ftpResponse.contentLength = result;
				break;
			}
			case "MDTM":
				if (ftpStatus.StatusCode != FtpStatusCode.FileStatus)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				ftpResponse.LastModified = DateTime.ParseExact(statusDescription.Substring(4), "yyyyMMddHHmmss", null);
				break;
			case "MKD":
				if (ftpStatus.StatusCode != FtpStatusCode.PathnameCreated)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				break;
			case "CWD":
				method = "PWD";
				if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = SendCommand(method);
				if (ftpStatus.StatusCode != FtpStatusCode.PathnameCreated)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				break;
			case "RNFR":
				method = "RENAME";
				if (ftpStatus.StatusCode != FtpStatusCode.FileCommandPending)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = SendCommand("RNTO", (renameTo == null) ? string.Empty : renameTo);
				if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				break;
			case "DELE":
				if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				break;
			}
			State = RequestState.Finished;
		}

		private void UploadData()
		{
			State = RequestState.OpeningData;
			OpenDataConnection();
			State = RequestState.TransferInProgress;
			requestStream = new System.Net.FtpDataStream(this, dataStream, false);
			asyncResult.Stream = requestStream;
		}

		private void DownloadData()
		{
			State = RequestState.OpeningData;
			OpenDataConnection();
			State = RequestState.TransferInProgress;
			ftpResponse.Stream = new System.Net.FtpDataStream(this, dataStream, true);
		}

		private void CheckRequestStarted()
		{
			if (State != RequestState.Before)
			{
				throw new InvalidOperationException("There is a request currently in progress");
			}
		}

		private void OpenControlConnection()
		{
			Exception innerException = null;
			Socket socket = null;
			IPAddress[] addressList = hostEntry.AddressList;
			foreach (IPAddress iPAddress in addressList)
			{
				socket = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, requestUri.Port);
				if (!ServicePoint.CallEndPointDelegate(socket, iPEndPoint))
				{
					socket.Close();
					socket = null;
					continue;
				}
				try
				{
					socket.Connect(iPEndPoint);
					localEndPoint = (IPEndPoint)socket.LocalEndPoint;
				}
				catch (SocketException ex)
				{
					innerException = ex;
					socket.Close();
					socket = null;
					continue;
				}
				break;
			}
			if (socket == null)
			{
				throw new WebException("Unable to connect to remote server", innerException, WebExceptionStatus.UnknownError, ftpResponse);
			}
			controlStream = new NetworkStream(socket);
			controlReader = new StreamReader(controlStream, Encoding.ASCII);
			State = RequestState.Authenticating;
			Authenticate();
			System.Net.FtpStatus ftpStatus = SendCommand("OPTS", "utf8", "on");
			ftpStatus = SendCommand("PWD");
			initial_path = GetInitialPath(ftpStatus);
		}

		private static string GetInitialPath(System.Net.FtpStatus status)
		{
			int statusCode = (int)status.StatusCode;
			if (statusCode < 200 || statusCode > 300 || status.StatusDescription.Length <= 4)
			{
				throw new WebException("Error getting current directory: " + status.StatusDescription, null, WebExceptionStatus.UnknownError, null);
			}
			string text = status.StatusDescription.Substring(4);
			if (text[0] == '"')
			{
				int num = text.IndexOf('"', 1);
				if (num == -1)
				{
					throw new WebException("Error getting current directory: PWD -> " + status.StatusDescription, null, WebExceptionStatus.UnknownError, null);
				}
				text = text.Substring(1, num - 1);
			}
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			return text;
		}

		private Socket SetupPassiveConnection(string statusDescription)
		{
			if (statusDescription.Length < 4)
			{
				throw new WebException("Cannot open passive data connection");
			}
			int i;
			for (i = 3; i < statusDescription.Length && !char.IsDigit(statusDescription[i]); i++)
			{
			}
			if (i >= statusDescription.Length)
			{
				throw new WebException("Cannot open passive data connection");
			}
			string[] array = statusDescription.Substring(i).Split(new char[1] { ',' }, 6);
			if (array.Length != 6)
			{
				throw new WebException("Cannot open passive data connection");
			}
			int num = array[5].Length - 1;
			while (num >= 0 && !char.IsDigit(array[5][num]))
			{
				num--;
			}
			if (num < 0)
			{
				throw new WebException("Cannot open passive data connection");
			}
			array[5] = array[5].Substring(0, num + 1);
			IPAddress address;
			try
			{
				address = IPAddress.Parse(string.Join(".", array, 0, 4));
			}
			catch (FormatException)
			{
				throw new WebException("Cannot open passive data connection");
			}
			int result;
			int result2;
			if (!int.TryParse(array[4], out result) || !int.TryParse(array[5], out result2))
			{
				throw new WebException("Cannot open passive data connection");
			}
			int num2 = (result << 8) + result2;
			if (num2 < 0 || num2 > 65535)
			{
				throw new WebException("Cannot open passive data connection");
			}
			IPEndPoint iPEndPoint = new IPEndPoint(address, num2);
			Socket socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				socket.Connect(iPEndPoint);
				return socket;
			}
			catch (SocketException)
			{
				socket.Close();
				throw new WebException("Cannot open passive data connection");
			}
		}

		private Exception CreateExceptionFromResponse(System.Net.FtpStatus status)
		{
			FtpWebResponse response = new FtpWebResponse(this, requestUri, method, status);
			return new WebException("Server returned an error: " + status.StatusDescription, null, WebExceptionStatus.ProtocolError, response);
		}

		internal void SetTransferCompleted()
		{
			if (!InFinalState())
			{
				State = RequestState.Finished;
				System.Net.FtpStatus responseStatus = GetResponseStatus();
				ftpResponse.UpdateStatus(responseStatus);
				if (!keepAlive)
				{
					CloseConnection();
				}
			}
		}

		internal void OperationCompleted()
		{
			if (!keepAlive)
			{
				CloseConnection();
			}
		}

		private void SetCompleteWithError(Exception exc)
		{
			if (asyncResult != null)
			{
				asyncResult.SetCompleted(false, exc);
			}
		}

		private Socket InitDataConnection()
		{
			System.Net.FtpStatus ftpStatus;
			if (usePassive)
			{
				ftpStatus = SendCommand("PASV");
				if (ftpStatus.StatusCode != FtpStatusCode.EnteringPassive)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				return SetupPassiveConnection(ftpStatus.StatusDescription);
			}
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				socket.Bind(new IPEndPoint(localEndPoint.Address, 0));
				socket.Listen(1);
			}
			catch (SocketException innerException)
			{
				socket.Close();
				throw new WebException("Couldn't open listening socket on client", innerException);
			}
			IPEndPoint iPEndPoint = (IPEndPoint)socket.LocalEndPoint;
			string text = iPEndPoint.Address.ToString().Replace('.', ',');
			int num = iPEndPoint.Port >> 8;
			int num2 = iPEndPoint.Port % 256;
			string text2 = text + "," + num + "," + num2;
			ftpStatus = SendCommand("PORT", text2);
			if (ftpStatus.StatusCode != FtpStatusCode.CommandOK)
			{
				socket.Close();
				throw CreateExceptionFromResponse(ftpStatus);
			}
			return socket;
		}

		private void OpenDataConnection()
		{
			Socket socket = InitDataConnection();
			System.Net.FtpStatus ftpStatus;
			if (offset > 0)
			{
				ftpStatus = SendCommand("REST", offset.ToString());
				if (ftpStatus.StatusCode != FtpStatusCode.FileCommandPending)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
			}
			ftpStatus = ((!(method != "NLST") || !(method != "LIST") || !(method != "STOU")) ? SendCommand(method) : SendCommand(method, file_name));
			if (ftpStatus.StatusCode != FtpStatusCode.OpeningData && ftpStatus.StatusCode != FtpStatusCode.DataAlreadyOpen)
			{
				throw CreateExceptionFromResponse(ftpStatus);
			}
			if (usePassive)
			{
				origDataStream = new NetworkStream(socket, true);
				dataStream = origDataStream;
				if (EnableSsl)
				{
					ChangeToSSLSocket(ref dataStream);
				}
			}
			else
			{
				Socket socket2 = null;
				try
				{
					socket2 = socket.Accept();
				}
				catch (SocketException)
				{
					socket.Close();
					if (socket2 != null)
					{
						socket2.Close();
					}
					throw new ProtocolViolationException("Server commited a protocol violation.");
				}
				socket.Close();
				origDataStream = new NetworkStream(socket, true);
				dataStream = origDataStream;
				if (EnableSsl)
				{
					ChangeToSSLSocket(ref dataStream);
				}
			}
			ftpResponse.UpdateStatus(ftpStatus);
		}

		private void Authenticate()
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			if (credentials != null)
			{
				text = credentials.UserName;
				text2 = credentials.Password;
				text3 = credentials.Domain;
			}
			if (text == null)
			{
				text = "anonymous";
			}
			if (text2 == null)
			{
				text2 = "@anonymous";
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text = text3 + '\\' + text;
			}
			System.Net.FtpStatus ftpStatus = GetResponseStatus();
			ftpResponse.BannerMessage = ftpStatus.StatusDescription;
			if (EnableSsl)
			{
				InitiateSecureConnection(ref controlStream);
				controlReader = new StreamReader(controlStream, Encoding.ASCII);
				ftpStatus = SendCommand("PBSZ", "0");
				int statusCode = (int)ftpStatus.StatusCode;
				if (statusCode < 200 || statusCode >= 300)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = SendCommand("PROT", "P");
				statusCode = (int)ftpStatus.StatusCode;
				if (statusCode < 200 || statusCode >= 300)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = new System.Net.FtpStatus(FtpStatusCode.SendUserCommand, string.Empty);
			}
			if (ftpStatus.StatusCode != FtpStatusCode.SendUserCommand)
			{
				throw CreateExceptionFromResponse(ftpStatus);
			}
			ftpStatus = SendCommand("USER", text);
			switch (ftpStatus.StatusCode)
			{
			case FtpStatusCode.SendPasswordCommand:
				ftpStatus = SendCommand("PASS", text2);
				if (ftpStatus.StatusCode != FtpStatusCode.LoggedInProceed)
				{
					throw CreateExceptionFromResponse(ftpStatus);
				}
				break;
			default:
				throw CreateExceptionFromResponse(ftpStatus);
			case FtpStatusCode.LoggedInProceed:
				break;
			}
			ftpResponse.WelcomeMessage = ftpStatus.StatusDescription;
			ftpResponse.UpdateStatus(ftpStatus);
		}

		private System.Net.FtpStatus SendCommand(string command, params string[] parameters)
		{
			return SendCommand(true, command, parameters);
		}

		private System.Net.FtpStatus SendCommand(bool waitResponse, string command, params string[] parameters)
		{
			string text = command;
			if (parameters.Length > 0)
			{
				text = text + " " + string.Join(" ", parameters);
			}
			text += "\r\n";
			byte[] bytes = Encoding.ASCII.GetBytes(text);
			try
			{
				controlStream.Write(bytes, 0, bytes.Length);
			}
			catch (IOException)
			{
				return new System.Net.FtpStatus(FtpStatusCode.ServiceNotAvailable, "Write failed");
			}
			if (!waitResponse)
			{
				return null;
			}
			System.Net.FtpStatus responseStatus = GetResponseStatus();
			if (ftpResponse != null)
			{
				ftpResponse.UpdateStatus(responseStatus);
			}
			return responseStatus;
		}

		internal static System.Net.FtpStatus ServiceNotAvailable()
		{
			return new System.Net.FtpStatus(FtpStatusCode.ServiceNotAvailable, Locale.GetText("Invalid response from server"));
		}

		internal System.Net.FtpStatus GetResponseStatus()
		{
			string text = null;
			try
			{
				text = controlReader.ReadLine();
			}
			catch (IOException)
			{
			}
			if (text == null || text.Length < 3)
			{
				return ServiceNotAvailable();
			}
			int result;
			if (!int.TryParse(text.Substring(0, 3), out result))
			{
				return ServiceNotAvailable();
			}
			if (text.Length > 3 && text[3] == '-')
			{
				string text2 = null;
				string value = result.ToString() + ' ';
				do
				{
					text2 = null;
					try
					{
						text2 = controlReader.ReadLine();
					}
					catch (IOException)
					{
					}
					if (text2 == null)
					{
						return ServiceNotAvailable();
					}
					text = text + Environment.NewLine + text2;
				}
				while (!text2.StartsWith(value, StringComparison.Ordinal));
			}
			return new System.Net.FtpStatus((FtpStatusCode)result, text);
		}

		private void InitiateSecureConnection(ref Stream stream)
		{
			System.Net.FtpStatus ftpStatus = SendCommand("AUTH", "TLS");
			if (ftpStatus.StatusCode != FtpStatusCode.ServerWantsSecureSession)
			{
				throw CreateExceptionFromResponse(ftpStatus);
			}
			ChangeToSSLSocket(ref stream);
		}

		internal bool ChangeToSSLSocket(ref Stream stream)
		{
			SslStream sslStream = new SslStream(stream, true, callback, null);
			sslStream.AuthenticateAsClient(requestUri.Host, null, SslProtocols.Default, false);
			stream = sslStream;
			return true;
		}

		private bool InFinalState()
		{
			return State == RequestState.Aborted || State == RequestState.Error || State == RequestState.Finished;
		}

		private bool InProgress()
		{
			return State != RequestState.Before && !InFinalState();
		}

		internal void CheckIfAborted()
		{
			if (State == RequestState.Aborted)
			{
				throw new WebException("Request aborted", WebExceptionStatus.RequestCanceled);
			}
		}

		private void CheckFinalState()
		{
			if (InFinalState())
			{
				throw new InvalidOperationException("Cannot change final state");
			}
		}
	}
}
