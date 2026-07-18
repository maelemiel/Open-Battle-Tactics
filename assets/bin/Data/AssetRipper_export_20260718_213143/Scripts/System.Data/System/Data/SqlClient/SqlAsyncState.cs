namespace System.Data.SqlClient
{
	internal class SqlAsyncState
	{
		private object _userState;

		public object UserState
		{
			get
			{
				return _userState;
			}
			set
			{
				_userState = value;
			}
		}

		public SqlAsyncState(object userState)
		{
			_userState = userState;
		}
	}
}
