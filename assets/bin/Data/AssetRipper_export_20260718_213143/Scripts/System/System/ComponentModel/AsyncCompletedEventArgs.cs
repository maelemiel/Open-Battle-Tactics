using System.Reflection;

namespace System.ComponentModel
{
	public class AsyncCompletedEventArgs : EventArgs
	{
		private Exception _error;

		private bool _cancelled;

		private object _userState;

		public bool Cancelled
		{
			get
			{
				return _cancelled;
			}
		}

		public Exception Error
		{
			get
			{
				return _error;
			}
		}

		public object UserState
		{
			get
			{
				return _userState;
			}
		}

		public AsyncCompletedEventArgs(Exception error, bool cancelled, object userState)
		{
			_error = error;
			_cancelled = cancelled;
			_userState = userState;
		}

		protected void RaiseExceptionIfNecessary()
		{
			if (_error != null)
			{
				throw new TargetInvocationException(_error);
			}
			if (_cancelled)
			{
				throw new InvalidOperationException("The operation was cancelled");
			}
		}
	}
}
