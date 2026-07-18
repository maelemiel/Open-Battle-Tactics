namespace System.Net
{
	public class NetworkCredential : ICredentials
	{
		private string userName;

		private string password;

		private string domain;

		public string Domain
		{
			get
			{
				return (domain != null) ? domain : string.Empty;
			}
			set
			{
				domain = value;
			}
		}

		public string UserName
		{
			get
			{
				return (userName != null) ? userName : string.Empty;
			}
			set
			{
				userName = value;
			}
		}

		public string Password
		{
			get
			{
				return (password != null) ? password : string.Empty;
			}
			set
			{
				password = value;
			}
		}

		public NetworkCredential()
		{
		}

		public NetworkCredential(string userName, string password)
		{
			this.userName = userName;
			this.password = password;
		}

		public NetworkCredential(string userName, string password, string domain)
		{
			this.userName = userName;
			this.password = password;
			this.domain = domain;
		}

		public NetworkCredential GetCredential(Uri uri, string authType)
		{
			return this;
		}
	}
}
