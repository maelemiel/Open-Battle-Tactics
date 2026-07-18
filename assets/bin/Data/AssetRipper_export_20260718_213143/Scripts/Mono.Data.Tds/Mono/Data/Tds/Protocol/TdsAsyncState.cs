namespace Mono.Data.Tds.Protocol
{
	internal class TdsAsyncState
	{
		private object _userState;

		private bool _wantResults;

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

		public bool WantResults
		{
			get
			{
				return _wantResults;
			}
			set
			{
				_wantResults = value;
			}
		}

		public TdsAsyncState(object userState)
		{
			_userState = userState;
		}
	}
}
