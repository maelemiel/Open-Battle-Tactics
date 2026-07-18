namespace System.Data.Common
{
	public abstract class DbTransaction : MarshalByRefObject, IDisposable, IDbTransaction
	{
		IDbConnection IDbTransaction.Connection
		{
			get
			{
				return Connection;
			}
		}

		public DbConnection Connection
		{
			get
			{
				return DbConnection;
			}
		}

		protected abstract DbConnection DbConnection { get; }

		public abstract IsolationLevel IsolationLevel { get; }

		public abstract void Commit();

		public abstract void Rollback();

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
