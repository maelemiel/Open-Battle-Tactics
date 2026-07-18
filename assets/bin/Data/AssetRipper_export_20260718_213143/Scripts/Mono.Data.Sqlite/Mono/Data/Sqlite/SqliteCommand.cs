using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace Mono.Data.Sqlite
{
	[Designer("SQLite.Designer.SqliteCommandDesigner, SQLite.Designer, Version=1.0.36.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139")]
	[ToolboxItem(true)]
	public sealed class SqliteCommand : DbCommand, ICloneable
	{
		private string _commandText;

		private SqliteConnection _cnn;

		private long _version;

		private WeakReference _activeReader;

		internal int _commandTimeout;

		private bool _designTimeVisible;

		private UpdateRowSource _updateRowSource;

		private SqliteParameterCollection _parameterCollection;

		internal List<SqliteStatement> _statementList;

		internal string _remainingText;

		private SqliteTransaction _transaction;

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.SQL.Design.SqlCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public override string CommandText
		{
			get
			{
				return _commandText;
			}
			set
			{
				if (!(_commandText == value))
				{
					if (_activeReader != null && _activeReader.IsAlive)
					{
						throw new InvalidOperationException("Cannot set CommandText while a DataReader is active");
					}
					ClearCommands();
					_commandText = value;
					if (_cnn != null)
					{
					}
				}
			}
		}

		[DefaultValue(30)]
		public override int CommandTimeout
		{
			get
			{
				return _commandTimeout;
			}
			set
			{
				_commandTimeout = value;
			}
		}

		[DefaultValue(CommandType.Text)]
		[RefreshProperties(RefreshProperties.All)]
		public override CommandType CommandType
		{
			get
			{
				return CommandType.Text;
			}
			set
			{
				if (value != CommandType.Text)
				{
					throw new NotSupportedException();
				}
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqliteConnection Connection
		{
			get
			{
				return _cnn;
			}
			set
			{
				if (_activeReader != null && _activeReader.IsAlive)
				{
					throw new InvalidOperationException("Cannot set Connection while a DataReader is active");
				}
				if (_cnn != null)
				{
					ClearCommands();
				}
				_cnn = value;
				if (_cnn != null)
				{
					_version = _cnn._version;
				}
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return Connection;
			}
			set
			{
				Connection = (SqliteConnection)value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SqliteParameterCollection Parameters
		{
			get
			{
				return _parameterCollection;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return Parameters;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public new SqliteTransaction Transaction
		{
			get
			{
				return _transaction;
			}
			set
			{
				if (_cnn != null)
				{
					if (_activeReader != null && _activeReader.IsAlive)
					{
						throw new InvalidOperationException("Cannot set Transaction while a DataReader is active");
					}
					if (value != null && value._cnn != _cnn)
					{
						throw new ArgumentException("Transaction is not associated with the command's connection");
					}
					_transaction = value;
				}
				else
				{
					Connection = value.Connection;
					_transaction = value;
				}
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return Transaction;
			}
			set
			{
				Transaction = (SqliteTransaction)value;
			}
		}

		[DefaultValue(UpdateRowSource.None)]
		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				return _updateRowSource;
			}
			set
			{
				_updateRowSource = value;
			}
		}

		[DesignOnly(true)]
		[Browsable(false)]
		[DefaultValue(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool DesignTimeVisible
		{
			get
			{
				return _designTimeVisible;
			}
			set
			{
				_designTimeVisible = value;
				TypeDescriptor.Refresh(this);
			}
		}

		public SqliteCommand()
			: this(null, null)
		{
		}

		public SqliteCommand(string commandText)
			: this(commandText, null, null)
		{
		}

		public SqliteCommand(string commandText, SqliteConnection connection)
			: this(commandText, connection, null)
		{
		}

		public SqliteCommand(SqliteConnection connection)
			: this(null, connection, null)
		{
		}

		private SqliteCommand(SqliteCommand source)
			: this(source.CommandText, source.Connection, source.Transaction)
		{
			CommandTimeout = source.CommandTimeout;
			DesignTimeVisible = source.DesignTimeVisible;
			UpdatedRowSource = source.UpdatedRowSource;
			foreach (SqliteParameter item in source._parameterCollection)
			{
				Parameters.Add(item.Clone());
			}
		}

		public SqliteCommand(string commandText, SqliteConnection connection, SqliteTransaction transaction)
		{
			_statementList = null;
			_activeReader = null;
			_commandTimeout = 30;
			_parameterCollection = new SqliteParameterCollection(this);
			_designTimeVisible = true;
			_updateRowSource = UpdateRowSource.None;
			_transaction = null;
			if (commandText != null)
			{
				CommandText = commandText;
			}
			if (connection != null)
			{
				DbConnection = connection;
				_commandTimeout = connection.DefaultTimeout;
			}
			if (transaction != null)
			{
				Transaction = transaction;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			SqliteDataReader sqliteDataReader = null;
			if (_activeReader != null)
			{
				try
				{
					sqliteDataReader = _activeReader.Target as SqliteDataReader;
				}
				catch
				{
				}
			}
			if (sqliteDataReader != null)
			{
				sqliteDataReader._disposeCommand = true;
				_activeReader = null;
			}
			else
			{
				Connection = null;
				_parameterCollection.Clear();
				_commandText = null;
			}
		}

		internal void ClearCommands()
		{
			if (_activeReader != null)
			{
				SqliteDataReader sqliteDataReader = null;
				try
				{
					sqliteDataReader = _activeReader.Target as SqliteDataReader;
				}
				catch
				{
				}
				if (sqliteDataReader != null)
				{
					sqliteDataReader.Close();
				}
				_activeReader = null;
			}
			if (_statementList != null)
			{
				int count = _statementList.Count;
				for (int i = 0; i < count; i++)
				{
					_statementList[i].Dispose();
				}
				_statementList = null;
				_parameterCollection.Unbind();
			}
		}

		internal SqliteStatement BuildNextCommand()
		{
			SqliteStatement sqliteStatement = null;
			try
			{
				if (_statementList == null)
				{
					_remainingText = _commandText;
				}
				sqliteStatement = _cnn._sql.Prepare(_cnn, _remainingText, (_statementList != null) ? _statementList[_statementList.Count - 1] : null, (uint)(_commandTimeout * 1000), out _remainingText);
				if (sqliteStatement != null)
				{
					sqliteStatement._command = this;
					if (_statementList == null)
					{
						_statementList = new List<SqliteStatement>();
					}
					_statementList.Add(sqliteStatement);
					_parameterCollection.MapParameters(sqliteStatement);
					sqliteStatement.BindParameters();
				}
				return sqliteStatement;
			}
			catch (Exception)
			{
				if (sqliteStatement != null)
				{
					if (_statementList.Contains(sqliteStatement))
					{
						_statementList.Remove(sqliteStatement);
					}
					sqliteStatement.Dispose();
				}
				_remainingText = null;
				throw;
			}
		}

		internal SqliteStatement GetStatement(int index)
		{
			if (_statementList == null)
			{
				return BuildNextCommand();
			}
			if (index == _statementList.Count)
			{
				if (!string.IsNullOrEmpty(_remainingText))
				{
					return BuildNextCommand();
				}
				return null;
			}
			SqliteStatement sqliteStatement = _statementList[index];
			sqliteStatement.BindParameters();
			return sqliteStatement;
		}

		public override void Cancel()
		{
			if (_activeReader != null)
			{
				SqliteDataReader sqliteDataReader = _activeReader.Target as SqliteDataReader;
				if (sqliteDataReader != null)
				{
					sqliteDataReader.Cancel();
				}
			}
		}

		protected override DbParameter CreateDbParameter()
		{
			return CreateParameter();
		}

		public new SqliteParameter CreateParameter()
		{
			return new SqliteParameter();
		}

		private void InitializeForReader()
		{
			if (_activeReader != null && _activeReader.IsAlive)
			{
				throw new InvalidOperationException("DataReader already active on this command");
			}
			if (_cnn == null)
			{
				throw new InvalidOperationException("No connection associated with this command");
			}
			if (_cnn.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("Database is not open");
			}
			if (_cnn._version != _version)
			{
				_version = _cnn._version;
				ClearCommands();
			}
			_parameterCollection.MapParameters(null);
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return ExecuteReader(behavior);
		}

		public new SqliteDataReader ExecuteReader(CommandBehavior behavior)
		{
			InitializeForReader();
			SqliteDataReader sqliteDataReader = new SqliteDataReader(this, behavior);
			_activeReader = new WeakReference(sqliteDataReader, false);
			return sqliteDataReader;
		}

		public new SqliteDataReader ExecuteReader()
		{
			return ExecuteReader(CommandBehavior.Default);
		}

		internal void ClearDataReader()
		{
			_activeReader = null;
		}

		public override int ExecuteNonQuery()
		{
			using (SqliteDataReader sqliteDataReader = ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
			{
				while (sqliteDataReader.NextResult())
				{
				}
				return sqliteDataReader.RecordsAffected;
			}
		}

		public override object ExecuteScalar()
		{
			using (SqliteDataReader sqliteDataReader = ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
			{
				if (sqliteDataReader.Read())
				{
					return sqliteDataReader[0];
				}
			}
			return null;
		}

		public override void Prepare()
		{
		}

		public object Clone()
		{
			return new SqliteCommand(this);
		}
	}
}
