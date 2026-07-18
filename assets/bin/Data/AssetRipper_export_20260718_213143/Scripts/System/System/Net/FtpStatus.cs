namespace System.Net
{
	internal class FtpStatus
	{
		private readonly FtpStatusCode statusCode;

		private readonly string statusDescription;

		public FtpStatusCode StatusCode
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
				return statusDescription;
			}
		}

		public FtpStatus(FtpStatusCode statusCode, string statusDescription)
		{
			this.statusCode = statusCode;
			this.statusDescription = statusDescription;
		}
	}
}
