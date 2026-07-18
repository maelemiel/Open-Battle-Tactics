using System.Transactions;

namespace Mono.Data.Sqlite
{
	internal class SQLiteEnlistment : IEnlistmentNotification
	{
		internal SqliteTransaction _transaction;

		internal Transaction _scope;

		internal bool _disposeConnection;

		internal SQLiteEnlistment(SqliteConnection cnn, Transaction scope)
		{
			_transaction = cnn.BeginTransaction();
			_scope = scope;
			_disposeConnection = false;
			_scope.EnlistVolatile(this, EnlistmentOptions.None);
		}

		private void Cleanup(SqliteConnection cnn)
		{
			if (_disposeConnection)
			{
				cnn.Dispose();
			}
			_transaction = null;
			_scope = null;
		}

		public void Commit(Enlistment enlistment)
		{
			SqliteConnection connection = _transaction.Connection;
			connection._enlistment = null;
			try
			{
				_transaction.IsValid(true);
				_transaction.Connection._transactionLevel = 1;
				_transaction.Commit();
				enlistment.Done();
			}
			finally
			{
				Cleanup(connection);
			}
		}

		public void InDoubt(Enlistment enlistment)
		{
			enlistment.Done();
		}

		public void Prepare(PreparingEnlistment preparingEnlistment)
		{
			if (!_transaction.IsValid(false))
			{
				preparingEnlistment.ForceRollback();
			}
			else
			{
				preparingEnlistment.Prepared();
			}
		}

		public void Rollback(Enlistment enlistment)
		{
			SqliteConnection connection = _transaction.Connection;
			connection._enlistment = null;
			try
			{
				_transaction.Rollback();
				enlistment.Done();
			}
			finally
			{
				Cleanup(connection);
			}
		}
	}
}
