namespace System.Net
{
	public class Authorization
	{
		private string token;

		private bool complete;

		private string connectionGroupId;

		private string[] protectionRealm;

		private IAuthenticationModule module;

		public string Message
		{
			get
			{
				return token;
			}
		}

		public bool Complete
		{
			get
			{
				return complete;
			}
		}

		public string ConnectionGroupId
		{
			get
			{
				return connectionGroupId;
			}
		}

		public string[] ProtectionRealm
		{
			get
			{
				return protectionRealm;
			}
			set
			{
				protectionRealm = value;
			}
		}

		internal IAuthenticationModule Module
		{
			get
			{
				return module;
			}
			set
			{
				module = value;
			}
		}

		[System.MonoTODO]
		public bool MutuallyAuthenticated
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

		public Authorization(string token)
			: this(token, true)
		{
		}

		public Authorization(string token, bool complete)
			: this(token, complete, null)
		{
		}

		public Authorization(string token, bool complete, string connectionGroupId)
		{
			this.token = token;
			this.complete = complete;
			this.connectionGroupId = connectionGroupId;
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}
	}
}
