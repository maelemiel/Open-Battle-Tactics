using System;
using System.Data;
using System.Data.Common;

namespace Mono.Data.Sqlite
{
	public sealed class SqliteTransaction : DbTransaction
	{
		internal SqliteConnection _cnn;

		internal long _version;

		private IsolationLevel _level;

		public new SqliteConnection Connection
		{
			get
			{
				return _cnn;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return Connection;
			}
		}

		public override IsolationLevel IsolationLevel
		{
			get
			{
				return _level;
			}
		}

		internal SqliteTransaction(SqliteConnection connection, bool deferredLock)
		{
			_cnn = connection;
			_version = _cnn._version;
			_level = ((!deferredLock) ? IsolationLevel.Serializable : IsolationLevel.ReadCommitted);
			if (_cnn._transactionLevel++ != 0)
			{
				return;
			}
			try
			{
				using (SqliteCommand sqliteCommand = _cnn.CreateCommand())
				{
					if (!deferredLock)
					{
						sqliteCommand.CommandText = "BEGIN IMMEDIATE";
					}
					else
					{
						sqliteCommand.CommandText = "BEGIN";
					}
					sqliteCommand.ExecuteNonQuery();
				}
			}
			catch (SqliteException)
			{
				_cnn._transactionLevel--;
				_cnn = null;
				throw;
			}
		}

		public override void Commit()
		{
			IsValid(true);
			if (_cnn._transactionLevel - 1 == 0)
			{
				using (SqliteCommand sqliteCommand = _cnn.CreateCommand())
				{
					sqliteCommand.CommandText = "COMMIT";
					sqliteCommand.ExecuteNonQuery();
				}
			}
			_cnn._transactionLevel--;
			_cnn = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (this)
				{
					if (IsValid(false))
					{
						Rollback();
					}
					_cnn = null;
				}
			}
			base.Dispose(disposing);
		}

		public override void Rollback()
		{
			IsValid(true);
			IssueRollback(_cnn);
			_cnn._transactionLevel = 0;
			_cnn = null;
		}

		internal static void IssueRollback(SqliteConnection cnn)
		{
			using (SqliteCommand sqliteCommand = cnn.CreateCommand())
			{
				sqliteCommand.CommandText = "ROLLBACK";
				sqliteCommand.ExecuteNonQuery();
			}
		}

		internal bool IsValid(bool throwError)
		{
			if (_cnn == null)
			{
				if (throwError)
				{
					throw new ArgumentNullException("No connection associated with this transaction");
				}
				return false;
			}
			if (_cnn._transactionLevel == 0)
			{
				if (throwError)
				{
					throw new SqliteException(21, "No transaction is active on this connection");
				}
				return false;
			}
			if (_cnn._version != _version)
			{
				if (throwError)
				{
					throw new SqliteException(21, "The connection was closed and re-opened, changes were rolled back");
				}
				return false;
			}
			if (_cnn.State != ConnectionState.Open)
			{
				if (throwError)
				{
					throw new SqliteException(21, "Connection was closed");
				}
				return false;
			}
			return true;
		}
	}
}
