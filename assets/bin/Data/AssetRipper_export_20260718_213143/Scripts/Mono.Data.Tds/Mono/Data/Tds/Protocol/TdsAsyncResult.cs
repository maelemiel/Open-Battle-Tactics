using System;
using System.Threading;

namespace Mono.Data.Tds.Protocol
{
	internal class TdsAsyncResult : IAsyncResult
	{
		private TdsAsyncState _tdsState;

		private WaitHandle _waitHandle;

		private bool _completed;

		private bool _completedSyncly;

		private AsyncCallback _userCallback;

		private object _retValue;

		private Exception _exception;

		public object AsyncState
		{
			get
			{
				return _tdsState.UserState;
			}
		}

		internal TdsAsyncState TdsAsyncState
		{
			get
			{
				return _tdsState;
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

		public bool IsCompletedWithException
		{
			get
			{
				return _exception != null;
			}
		}

		public Exception Exception
		{
			get
			{
				return _exception;
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

		public TdsAsyncResult(AsyncCallback userCallback, TdsAsyncState tdsState)
		{
			_tdsState = tdsState;
			_userCallback = userCallback;
			_waitHandle = new ManualResetEvent(false);
		}

		public TdsAsyncResult(AsyncCallback userCallback, object state)
		{
			_tdsState = new TdsAsyncState(state);
			_userCallback = userCallback;
			_waitHandle = new ManualResetEvent(false);
		}

		internal void MarkComplete()
		{
			_completed = true;
			_exception = null;
			((ManualResetEvent)_waitHandle).Set();
			if (_userCallback != null)
			{
				_userCallback(this);
			}
		}

		internal void MarkComplete(Exception e)
		{
			_completed = true;
			_exception = e;
			((ManualResetEvent)_waitHandle).Set();
			if (_userCallback != null)
			{
				_userCallback(this);
			}
		}
	}
}
