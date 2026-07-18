namespace System.Net
{
	internal class BasicClient : IAuthenticationModule
	{
		public string AuthenticationType
		{
			get
			{
				return "Basic";
			}
		}

		public bool CanPreAuthenticate
		{
			get
			{
				return true;
			}
		}

		public Authorization Authenticate(string challenge, WebRequest webRequest, ICredentials credentials)
		{
			if (credentials == null || challenge == null)
			{
				return null;
			}
			string text = challenge.Trim();
			if (text.ToLower().IndexOf("basic") == -1)
			{
				return null;
			}
			return InternalAuthenticate(webRequest, credentials);
		}

		private static byte[] GetBytes(string str)
		{
			int length = str.Length;
			byte[] array = new byte[length];
			for (length--; length >= 0; length--)
			{
				array[length] = (byte)str[length];
			}
			return array;
		}

		private static Authorization InternalAuthenticate(WebRequest webRequest, ICredentials credentials)
		{
			HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
			if (httpWebRequest == null || credentials == null)
			{
				return null;
			}
			NetworkCredential credential = credentials.GetCredential(httpWebRequest.AuthUri, "basic");
			if (credential == null)
			{
				return null;
			}
			string userName = credential.UserName;
			if (userName == null || userName == string.Empty)
			{
				return null;
			}
			string password = credential.Password;
			string domain = credential.Domain;
			byte[] inArray = ((domain != null && !(domain == string.Empty) && !(domain.Trim() == string.Empty)) ? GetBytes(domain + "\\" + userName + ":" + password) : GetBytes(userName + ":" + password));
			string token = "Basic " + Convert.ToBase64String(inArray);
			return new Authorization(token);
		}

		public Authorization PreAuthenticate(WebRequest webRequest, ICredentials credentials)
		{
			return InternalAuthenticate(webRequest, credentials);
		}
	}
}
