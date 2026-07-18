namespace System.Transactions
{
	public sealed class TransactionScope : IDisposable
	{
		private static TransactionOptions defaultOptions = new TransactionOptions(IsolationLevel.Serializable, TransactionManager.DefaultTimeout);

		private Transaction transaction;

		private Transaction oldTransaction;

		private TransactionScope parentScope;

		private int nested;

		private bool disposed;

		private bool completed;

		private bool isRoot;

		internal bool IsComplete
		{
			get
			{
				return completed;
			}
		}

		public TransactionScope()
			: this(TransactionScopeOption.Required, TransactionManager.DefaultTimeout)
		{
		}

		public TransactionScope(Transaction transaction)
			: this(transaction, TransactionManager.DefaultTimeout)
		{
		}

		public TransactionScope(Transaction transaction, TimeSpan timeout)
			: this(transaction, timeout, EnterpriseServicesInteropOption.None)
		{
		}

		[System.MonoTODO("EnterpriseServicesInteropOption not supported.")]
		public TransactionScope(Transaction transaction, TimeSpan timeout, EnterpriseServicesInteropOption opt)
		{
			Initialize(TransactionScopeOption.Required, transaction, defaultOptions, opt, timeout);
		}

		public TransactionScope(TransactionScopeOption option)
			: this(option, TransactionManager.DefaultTimeout)
		{
		}

		[System.MonoTODO("No TimeoutException is thrown")]
		public TransactionScope(TransactionScopeOption option, TimeSpan timeout)
		{
			Initialize(option, null, defaultOptions, EnterpriseServicesInteropOption.None, timeout);
		}

		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions options)
			: this(scopeOption, options, EnterpriseServicesInteropOption.None)
		{
		}

		[System.MonoTODO("EnterpriseServicesInteropOption not supported")]
		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions options, EnterpriseServicesInteropOption opt)
		{
			Initialize(scopeOption, null, options, opt, TransactionManager.DefaultTimeout);
		}

		private void Initialize(TransactionScopeOption scopeOption, Transaction tx, TransactionOptions options, EnterpriseServicesInteropOption interop, TimeSpan timeout)
		{
			completed = false;
			isRoot = false;
			nested = 0;
			oldTransaction = Transaction.CurrentInternal;
			Transaction.CurrentInternal = (transaction = InitTransaction(tx, scopeOption));
			if (transaction != null)
			{
				transaction.InitScope(this);
			}
			if (parentScope != null)
			{
				parentScope.nested++;
			}
		}

		private Transaction InitTransaction(Transaction tx, TransactionScopeOption scopeOption)
		{
			if (tx != null)
			{
				return tx;
			}
			switch (scopeOption)
			{
			case TransactionScopeOption.Suppress:
				if (Transaction.CurrentInternal != null)
				{
					parentScope = Transaction.CurrentInternal.Scope;
				}
				return null;
			case TransactionScopeOption.Required:
				if (Transaction.CurrentInternal == null)
				{
					isRoot = true;
					return new Transaction();
				}
				parentScope = Transaction.CurrentInternal.Scope;
				return Transaction.CurrentInternal;
			default:
				if (Transaction.CurrentInternal != null)
				{
					parentScope = Transaction.CurrentInternal.Scope;
				}
				isRoot = true;
				return new Transaction();
			}
		}

		public void Complete()
		{
			if (completed)
			{
				throw new InvalidOperationException("The current TransactionScope is already complete. You should dispose the TransactionScope.");
			}
			completed = true;
		}

		public void Dispose()
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (parentScope != null)
			{
				parentScope.nested--;
			}
			if (nested > 0)
			{
				transaction.Rollback();
				throw new InvalidOperationException("TransactionScope nested incorrectly");
			}
			if (Transaction.CurrentInternal != transaction)
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}
				if (Transaction.CurrentInternal != null)
				{
					Transaction.CurrentInternal.Rollback();
				}
				throw new InvalidOperationException("Transaction.Current has changed inside of the TransactionScope");
			}
			if (Transaction.CurrentInternal == oldTransaction && oldTransaction != null)
			{
				oldTransaction.Scope = parentScope;
			}
			Transaction.CurrentInternal = oldTransaction;
			if (!(transaction == null))
			{
				transaction.Scope = null;
				if (!IsComplete)
				{
					transaction.Rollback();
				}
				else if (isRoot)
				{
					transaction.CommitInternal();
				}
			}
		}
	}
}
