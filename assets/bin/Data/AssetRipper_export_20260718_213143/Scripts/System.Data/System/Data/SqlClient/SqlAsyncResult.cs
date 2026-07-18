using System.Threading;

namespace System.Data.SqlClient
{
	internal class SqlAsyncResult : IAsyncResult
	{
		private SqlAsyncState _sqlState;

		private WaitHandle _waitHandle;

		private bool _completed;

		private bool _completedSyncly;

		private bool _ended;

		private AsyncCallback _userCallback;

		private object _retValue;

		private string _endMethod;

		private IAsyncResult _internal;

		public object AsyncState
		{
			get
			{
				return _sqlState.UserState;
			}
		}

		internal SqlAsyncState SqlAsyncState
		{
			get
			{
				return _sqlState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return _waitHandle;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return _completed;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return _completedSyncly;
			}
		}

		internal object ReturnValue
		{
			get
			{
				return _retValue;
			}
			set
			{
				_retValue = value;
			}
		}

		public string EndMethod
		{
			get
			{
				return _endMethod;
			}
			set
			{
				_endMethod = value;
			}
		}

		public bool Ended
		{
			get
			{
				return _ended;
			}
			set
			{
				_ended = value;
			}
		}

		internal IAsyncResult InternalResult
		{
			get
			{
				return _internal;
			}
			set
			{
				_internal = value;
			}
		}

		public AsyncCallback BubbleCallback
		{
			get
			{
				return Bubbleback;
			}
		}

		public SqlAsyncResult(AsyncCallback userCallback, SqlAsyncState sqlState)
		{
			_sqlState = sqlState;
			_userCallback = userCallback;
			_waitHandle = new ManualResetEvent(false);
		}

		public SqlAsyncResult(AsyncCallback userCallback, object state)
		{
			_sqlState = new SqlAsyncState(state);
			_userCallback = userCallback;
			_waitHandle = new ManualResetEvent(false);
		}

		internal void MarkComplete()
		{
			_completed = true;
			((ManualResetEvent)_waitHandle).Set();
			if (_userCallback != null)
			{
				_userCallback(this);
			}
		}

		public void Bubbleback(IAsyncResult ar)
		{
			MarkComplete();
		}
	}
}
