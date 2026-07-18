using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbCommand : Component, IDisposable, IDbCommand
	{
		IDbConnection IDbCommand.Connection
		{
			get
			{
				return Connection;
			}
			set
			{
				Connection = (DbConnection)value;
			}
		}

		IDataParameterCollection IDbCommand.Parameters
		{
			get
			{
				return Parameters;
			}
		}

		IDbTransaction IDbCommand.Transaction
		{
			get
			{
				return Transaction;
			}
			set
			{
				Transaction = (DbTransaction)value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		public abstract string CommandText { get; set; }

		public abstract int CommandTimeout { get; set; }

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(CommandType.Text)]
		public abstract CommandType CommandType { get; set; }

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DbConnection Connection
		{
			get
			{
				return DbConnection;
			}
			set
			{
				DbConnection = value;
			}
		}

		protected abstract DbConnection DbConnection { get; set; }

		protected abstract DbParameterCollection DbParameterCollection { get; }

		protected abstract DbTransaction DbTransaction { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignOnly(true)]
		[DefaultValue(true)]
		public abstract bool DesignTimeVisible { get; set; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbParameterCollection Parameters
		{
			get
			{
				return DbParameterCollection;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public DbTransaction Transaction
		{
			get
			{
				return DbTransaction;
			}
			set
			{
				DbTransaction = value;
			}
		}

		[DefaultValue(UpdateRowSource.Both)]
		public abstract UpdateRowSource UpdatedRowSource { get; set; }

		IDbDataParameter IDbCommand.CreateParameter()
		{
			return CreateParameter();
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			return ExecuteReader();
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			return ExecuteReader(behavior);
		}

		public abstract void Cancel();

		protected abstract DbParameter CreateDbParameter();

		public DbParameter CreateParameter()
		{
			return CreateDbParameter();
		}

		protected abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);

		public abstract int ExecuteNonQuery();

		public DbDataReader ExecuteReader()
		{
			return ExecuteDbDataReader(CommandBehavior.Default);
		}

		public DbDataReader ExecuteReader(CommandBehavior behavior)
		{
			return ExecuteDbDataReader(behavior);
		}

		public abstract object ExecuteScalar();

		public abstract void Prepare();
	}
}
