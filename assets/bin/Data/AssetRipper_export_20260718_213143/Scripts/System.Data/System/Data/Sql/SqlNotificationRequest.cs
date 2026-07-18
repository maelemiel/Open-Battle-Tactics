namespace System.Data.Sql
{
	public sealed class SqlNotificationRequest
	{
		private string userData;

		private string options;

		private int timeout;

		public string UserData
		{
			get
			{
				return userData;
			}
			set
			{
				if (value != null && value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("UserData");
				}
				userData = value;
			}
		}

		public string Options
		{
			get
			{
				return options;
			}
			set
			{
				if (value != null && value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("Service");
				}
				options = value;
			}
		}

		public int Timeout
		{
			get
			{
				return timeout;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Timeout");
				}
				timeout = value;
			}
		}

		public SqlNotificationRequest()
		{
		}

		public SqlNotificationRequest(string userData, string options, int timeout)
		{
			UserData = userData;
			Options = options;
			Timeout = timeout;
		}
	}
}
