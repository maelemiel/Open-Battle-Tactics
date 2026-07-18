using System.IO;

namespace System.Net
{
	public class FtpWebResponse : WebResponse
	{
		private Stream stream;

		private Uri uri;

		private FtpStatusCode statusCode;

		private DateTime lastModified = DateTime.MinValue;

		private string bannerMessage = string.Empty;

		private string welcomeMessage = string.Empty;

		private string exitMessage = string.Empty;

		private string statusDescription;

		private string method;

		private bool disposed;

		private FtpWebRequest request;

		internal long contentLength = -1L;

		public override long ContentLength
		{
			get
			{
				return contentLength;
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				return new WebHeaderCollection();
			}
		}

		public override Uri ResponseUri
		{
			get
			{
				return uri;
			}
		}

		public DateTime LastModified
		{
			get
			{
				return lastModified;
			}
			internal set
			{
				lastModified = value;
			}
		}

		public string BannerMessage
		{
			get
			{
				return bannerMessage;
			}
			internal set
			{
				bannerMessage = value;
			}
		}

		public string WelcomeMessage
		{
			get
			{
				return welcomeMessage;
			}
			internal set
			{
				welcomeMessage = value;
			}
		}

		public string ExitMessage
		{
			get
			{
				return exitMessage;
			}
			internal set
			{
				exitMessage = value;
			}
		}

		public FtpStatusCode StatusCode
		{
			get
			{
				return statusCode;
			}
			private set
			{
				statusCode = value;
			}
		}

		public string StatusDescription
		{
			get
			{
				return statusDescription;
			}
			private set
			{
				statusDescription = value;
			}
		}

		internal Stream Stream
		{
			get
			{
				return stream;
			}
			set
			{
				stream = value;
			}
		}

		internal FtpWebResponse(FtpWebRequest request, Uri uri, string method, bool keepAlive)
		{
			this.request = request;
			this.uri = uri;
			this.method = method;
		}

		internal FtpWebResponse(FtpWebRequest request, Uri uri, string method, FtpStatusCode statusCode, string statusDescription)
		{
			this.request = request;
			this.uri = uri;
			this.method = method;
			this.statusCode = statusCode;
			this.statusDescription = statusDescription;
		}

		internal FtpWebResponse(FtpWebRequest request, Uri uri, string method, System.Net.FtpStatus status)
			: this(request, uri, method, status.StatusCode, status.StatusDescription)
		{
		}

		public override void Close()
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (stream != null)
			{
				stream.Close();
				if (stream == Stream.Null)
				{
					request.OperationCompleted();
				}
			}
			stream = null;
		}

		public override Stream GetResponseStream()
		{
			if (stream == null)
			{
				return Stream.Null;
			}
			if (method != "RETR" && method != "NLST")
			{
				CheckDisposed();
			}
			return stream;
		}

		internal void UpdateStatus(System.Net.FtpStatus status)
		{
			statusCode = status.StatusCode;
			statusDescription = status.StatusDescription;
		}

		private void CheckDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		internal bool IsFinal()
		{
			return statusCode >= FtpStatusCode.CommandOK;
		}
	}
}
