using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlTransaction : DbTransaction, IDisposable, IDbTransaction
	{
		private bool disposed;

		private SqlConnection connection;

		private IsolationLevel isolationLevel;

		private bool isOpen;

		public new SqlConnection Connection
		{
			get
			{
				return connection;
			}
		}

		internal bool IsOpen
		{
			get
			{
				return isOpen;
			}
		}

		public override IsolationLevel IsolationLevel
		{
			get
			{
				if (!isOpen)
				{
					throw ExceptionHelper.TransactionNotUsable(GetType());
				}
				return isolationLevel;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return Connection;
			}
		}

		internal SqlTransaction(SqlConnection connection, IsolationLevel isolevel)
		{
			this.connection = connection;
			isolationLevel = isolevel;
			isOpen = true;
		}

		public override void Commit()
		{
			if (!isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(GetType());
			}
			connection.Tds.Execute("COMMIT TRANSACTION");
			connection.Transaction = null;
			connection = null;
			isOpen = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing && isOpen)
				{
					Rollback();
				}
				disposed = true;
			}
		}

		public override void Rollback()
		{
			Rollback(string.Empty);
		}

		public void Rollback(string transactionName)
		{
			if (!disposed)
			{
				if (!isOpen)
				{
					throw ExceptionHelper.TransactionNotUsable(GetType());
				}
				connection.Tds.Execute(string.Format("IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION {0}", transactionName));
				isOpen = false;
				connection.Transaction = null;
				connection = null;
			}
		}

		public void Save(string savePointName)
		{
			if (!isOpen)
			{
				throw ExceptionHelper.TransactionNotUsable(GetType());
			}
			connection.Tds.Execute(string.Format("SAVE TRANSACTION {0}", savePointName));
		}
	}
}
