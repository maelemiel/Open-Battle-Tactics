using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace System.Transactions
{
	[Serializable]
	public class Transaction : IDisposable, ISerializable
	{
		private delegate void AsyncCommit();

		[ThreadStatic]
		private static Transaction ambient;

		private IsolationLevel level;

		private TransactionInformation info;

		private ArrayList dependents = new ArrayList();

		private List<IEnlistmentNotification> volatiles = new List<IEnlistmentNotification>();

		private List<ISinglePhaseNotification> durables = new List<ISinglePhaseNotification>();

		private AsyncCommit asyncCommit;

		private bool committing;

		private bool committed;

		private bool aborted;

		private TransactionScope scope;

		private Exception innerException;

		public static Transaction Current
		{
			get
			{
				EnsureIncompleteCurrentScope();
				return CurrentInternal;
			}
			set
			{
				EnsureIncompleteCurrentScope();
				CurrentInternal = value;
			}
		}

		internal static Transaction CurrentInternal
		{
			get
			{
				return ambient;
			}
			set
			{
				ambient = value;
			}
		}

		public IsolationLevel IsolationLevel
		{
			get
			{
				EnsureIncompleteCurrentScope();
				return level;
			}
		}

		public TransactionInformation TransactionInformation
		{
			get
			{
				EnsureIncompleteCurrentScope();
				return info;
			}
		}

		private bool Aborted
		{
			get
			{
				return aborted;
			}
			set
			{
				aborted = value;
				if (aborted)
				{
					info.Status = TransactionStatus.Aborted;
				}
			}
		}

		internal TransactionScope Scope
		{
			get
			{
				return scope;
			}
			set
			{
				scope = value;
			}
		}

		public event TransactionCompletedEventHandler TransactionCompleted;

		internal Transaction()
		{
			info = new TransactionInformation();
			level = IsolationLevel.Serializable;
		}

		internal Transaction(Transaction other)
		{
			level = other.level;
			info = other.info;
			dependents = other.dependents;
		}

		[System.MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public Transaction Clone()
		{
			return new Transaction(this);
		}

		public void Dispose()
		{
			if (TransactionInformation.Status == TransactionStatus.Active)
			{
				Rollback();
			}
		}

		[System.MonoTODO]
		public DependentTransaction DependentClone(DependentCloneOption option)
		{
			DependentTransaction dependentTransaction = new DependentTransaction(this, option);
			dependents.Add(dependentTransaction);
			return dependentTransaction;
		}

		[System.MonoTODO("Only SinglePhase commit supported for durable resource managers.")]
		public Enlistment EnlistDurable(Guid manager, IEnlistmentNotification notification, EnlistmentOptions options)
		{
			throw new NotImplementedException("Only SinglePhase commit supported for durable resource managers.");
		}

		[System.MonoTODO("Only Local Transaction Manager supported. Cannot have more than 1 durable resource per transaction. Only EnlistmentOptions.None supported yet.")]
		public Enlistment EnlistDurable(Guid manager, ISinglePhaseNotification notification, EnlistmentOptions options)
		{
			if (durables.Count == 1)
			{
				throw new NotImplementedException("Only LTM supported. Cannot have more than 1 durable resource per transaction.");
			}
			EnsureIncompleteCurrentScope();
			if (options != EnlistmentOptions.None)
			{
				throw new NotImplementedException("Implement me");
			}
			durables.Add(notification);
			return new Enlistment();
		}

		[System.MonoTODO]
		public bool EnlistPromotableSinglePhase(IPromotableSinglePhaseNotification notification)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO("EnlistmentOptions being ignored")]
		public Enlistment EnlistVolatile(IEnlistmentNotification notification, EnlistmentOptions options)
		{
			return EnlistVolatileInternal(notification, options);
		}

		[System.MonoTODO("EnlistmentOptions being ignored")]
		public Enlistment EnlistVolatile(ISinglePhaseNotification notification, EnlistmentOptions options)
		{
			return EnlistVolatileInternal(notification, options);
		}

		private Enlistment EnlistVolatileInternal(IEnlistmentNotification notification, EnlistmentOptions options)
		{
			EnsureIncompleteCurrentScope();
			volatiles.Add(notification);
			return new Enlistment();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Transaction);
		}

		private bool Equals(Transaction t)
		{
			if (object.ReferenceEquals(t, this))
			{
				return true;
			}
			if (object.ReferenceEquals(t, null))
			{
				return false;
			}
			return level == t.level && info == t.info;
		}

		public override int GetHashCode()
		{
			return (int)((uint)level ^ (uint)info.GetHashCode()) ^ dependents.GetHashCode();
		}

		public void Rollback()
		{
			Rollback(null);
		}

		public void Rollback(Exception ex)
		{
			EnsureIncompleteCurrentScope();
			Rollback(ex, null);
		}

		internal void Rollback(Exception ex, IEnlistmentNotification enlisted)
		{
			if (aborted)
			{
				return;
			}
			if (info.Status == TransactionStatus.Committed)
			{
				throw new TransactionException("Transaction has already been committed. Cannot accept any new work.");
			}
			innerException = ex;
			Enlistment enlistment = new Enlistment();
			foreach (IEnlistmentNotification @volatile in volatiles)
			{
				if (@volatile != enlisted)
				{
					@volatile.Rollback(enlistment);
				}
			}
			if (durables.Count > 0 && durables[0] != enlisted)
			{
				durables[0].Rollback(enlistment);
			}
			Aborted = true;
		}

		protected IAsyncResult BeginCommitInternal(AsyncCallback callback)
		{
			if (committed || committing)
			{
				throw new InvalidOperationException("Commit has already been called for this transaction.");
			}
			committing = true;
			asyncCommit = DoCommit;
			return asyncCommit.BeginInvoke(callback, null);
		}

		protected void EndCommitInternal(IAsyncResult ar)
		{
			asyncCommit.EndInvoke(ar);
		}

		internal void CommitInternal()
		{
			if (committed || committing)
			{
				throw new InvalidOperationException("Commit has already been called for this transaction.");
			}
			committing = true;
			DoCommit();
		}

		private void DoCommit()
		{
			if (Scope != null)
			{
				Rollback(null, null);
				CheckAborted();
			}
			if (volatiles.Count == 1 && durables.Count == 0)
			{
				ISinglePhaseNotification singlePhaseNotification = volatiles[0] as ISinglePhaseNotification;
				if (singlePhaseNotification != null)
				{
					DoSingleCommit(singlePhaseNotification);
					Complete();
					return;
				}
			}
			if (volatiles.Count > 0)
			{
				DoPreparePhase();
			}
			if (durables.Count > 0)
			{
				DoSingleCommit(durables[0]);
			}
			if (volatiles.Count > 0)
			{
				DoCommitPhase();
			}
			Complete();
		}

		private void Complete()
		{
			committing = false;
			committed = true;
			if (!aborted)
			{
				info.Status = TransactionStatus.Committed;
			}
		}

		internal void InitScope(TransactionScope scope)
		{
			CheckAborted();
			if (committed)
			{
				throw new InvalidOperationException("Commit has already been called on this transaction.");
			}
			Scope = scope;
		}

		private void DoPreparePhase()
		{
			foreach (IEnlistmentNotification @volatile in volatiles)
			{
				PreparingEnlistment preparingEnlistment = new PreparingEnlistment(this, @volatile);
				@volatile.Prepare(preparingEnlistment);
				if (!preparingEnlistment.IsPrepared)
				{
					Aborted = true;
					break;
				}
			}
			CheckAborted();
		}

		private void DoCommitPhase()
		{
			foreach (IEnlistmentNotification @volatile in volatiles)
			{
				Enlistment enlistment = new Enlistment();
				@volatile.Commit(enlistment);
			}
		}

		private void DoSingleCommit(ISinglePhaseNotification single)
		{
			if (single != null)
			{
				SinglePhaseEnlistment enlistment = new SinglePhaseEnlistment(this, single);
				single.SinglePhaseCommit(enlistment);
				CheckAborted();
			}
		}

		private void CheckAborted()
		{
			if (aborted)
			{
				throw new TransactionAbortedException("Transaction has aborted", innerException);
			}
		}

		private static void EnsureIncompleteCurrentScope()
		{
			if (CurrentInternal == null || CurrentInternal.Scope == null || !CurrentInternal.Scope.IsComplete)
			{
				return;
			}
			throw new InvalidOperationException("The current TransactionScope is already complete");
		}

		public static bool operator ==(Transaction x, Transaction y)
		{
			if (object.ReferenceEquals(x, null))
			{
				return object.ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}

		public static bool operator !=(Transaction x, Transaction y)
		{
			return !(x == y);
		}
	}
}
