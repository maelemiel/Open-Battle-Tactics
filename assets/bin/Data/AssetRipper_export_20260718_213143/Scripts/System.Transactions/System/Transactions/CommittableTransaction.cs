using System.Runtime.Serialization;
using System.Threading;

namespace System.Transactions
{
	[Serializable]
	public sealed class CommittableTransaction : Transaction, IDisposable, IAsyncResult, ISerializable
	{
		private TransactionOptions options;

		private AsyncCallback callback;

		private object user_defined_state;

		private IAsyncResult asyncResult;

		object IAsyncResult.AsyncState
		{
			get
			{
				return user_defined_state;
			}
		}

		WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get
			{
				return asyncResult.AsyncWaitHandle;
			}
		}

		bool IAsyncResult.CompletedSynchronously
		{
			get
			{
				return asyncResult.CompletedSynchronously;
			}
		}

		bool IAsyncResult.IsCompleted
		{
			get
			{
				return asyncResult.IsCompleted;
			}
		}

		public CommittableTransaction()
			: this(default(TransactionOptions))
		{
		}

		public CommittableTransaction(TimeSpan timeout)
		{
			options = default(TransactionOptions);
			options.Timeout = timeout;
		}

		public CommittableTransaction(TransactionOptions options)
		{
			this.options = options;
		}

		[System.MonoTODO("Not implemented")]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public IAsyncResult BeginCommit(AsyncCallback callback, object user_defined_state)
		{
			this.callback = callback;
			this.user_defined_state = user_defined_state;
			AsyncCallback asyncCallback = null;
			if (callback != null)
			{
				asyncCallback = CommitCallback;
			}
			asyncResult = BeginCommitInternal(asyncCallback);
			return this;
		}

		public void EndCommit(IAsyncResult ar)
		{
			if (ar != this)
			{
				throw new ArgumentException("The IAsyncResult parameter must be the same parameter as returned by BeginCommit.", "asyncResult");
			}
			EndCommitInternal(asyncResult);
		}

		private void CommitCallback(IAsyncResult ar)
		{
			if (asyncResult == null && ar.CompletedSynchronously)
			{
				asyncResult = ar;
			}
			callback(this);
		}

		public void Commit()
		{
			CommitInternal();
		}
	}
}
