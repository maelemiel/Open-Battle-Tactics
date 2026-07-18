using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace System.Data.Common
{
	public abstract class DbCommandBuilder : Component
	{
		private bool _setAllValues;

		private bool _disposed;

		private DataTable _dbSchemaTable;

		private DbDataAdapter _dbDataAdapter;

		private CatalogLocation _catalogLocation = CatalogLocation.Start;

		private ConflictOption _conflictOption = ConflictOption.CompareAllSearchableValues;

		private string _tableName;

		private string _catalogSeparator;

		private string _quotePrefix;

		private string _quoteSuffix;

		private string _schemaSeparator;

		private DbCommand _dbCommand;

		private DbCommand _deleteCommand;

		private DbCommand _insertCommand;

		private DbCommand _updateCommand;

		private static readonly string SEPARATOR_DEFAULT = ".";

		private static readonly string clause1 = "({0} = 1 AND {1} IS NULL)";

		private static readonly string clause2 = "({0} = {1})";

		private string QuotedTableName
		{
			get
			{
				return GetQuotedString(_tableName);
			}
		}

		private bool IsCommandGenerated
		{
			get
			{
				return _insertCommand != null || _updateCommand != null || _deleteCommand != null;
			}
		}

		[DefaultValue(CatalogLocation.Start)]
		public virtual CatalogLocation CatalogLocation
		{
			get
			{
				return _catalogLocation;
			}
			set
			{
				CheckEnumValue(typeof(CatalogLocation), (int)value);
				_catalogLocation = value;
			}
		}

		[DefaultValue(".")]
		public virtual string CatalogSeparator
		{
			get
			{
				if (_catalogSeparator == null || _catalogSeparator.Length == 0)
				{
					return SEPARATOR_DEFAULT;
				}
				return _catalogSeparator;
			}
			set
			{
				_catalogSeparator = value;
			}
		}

		[DefaultValue(ConflictOption.CompareAllSearchableValues)]
		public virtual ConflictOption ConflictOption
		{
			get
			{
				return _conflictOption;
			}
			set
			{
				CheckEnumValue(typeof(ConflictOption), (int)value);
				_conflictOption = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbDataAdapter DataAdapter
		{
			get
			{
				return _dbDataAdapter;
			}
			set
			{
				if (value != null)
				{
					SetRowUpdatingHandler(value);
				}
				_dbDataAdapter = value;
			}
		}

		[DefaultValue("")]
		public virtual string QuotePrefix
		{
			get
			{
				if (_quotePrefix == null)
				{
					return string.Empty;
				}
				return _quotePrefix;
			}
			set
			{
				if (IsCommandGenerated)
				{
					throw new InvalidOperationException("QuotePrefix cannot be set after an Insert, Update or Delete command has been generated.");
				}
				_quotePrefix = value;
			}
		}

		[DefaultValue("")]
		public virtual string QuoteSuffix
		{
			get
			{
				if (_quoteSuffix == null)
				{
					return string.Empty;
				}
				return _quoteSuffix;
			}
			set
			{
				if (IsCommandGenerated)
				{
					throw new InvalidOperationException("QuoteSuffix cannot be set after an Insert, Update or Delete command has been generated.");
				}
				_quoteSuffix = value;
			}
		}

		[DefaultValue(".")]
		public virtual string SchemaSeparator
		{
			get
			{
				if (_schemaSeparator == null || _schemaSeparator.Length == 0)
				{
					return SEPARATOR_DEFAULT;
				}
				return _schemaSeparator;
			}
			set
			{
				_schemaSeparator = value;
			}
		}

		[DefaultValue(false)]
		public bool SetAllValues
		{
			get
			{
				return _setAllValues;
			}
			set
			{
				_setAllValues = value;
			}
		}

		private DbCommand SourceCommand
		{
			get
			{
				if (_dbDataAdapter != null)
				{
					return _dbDataAdapter.SelectCommand;
				}
				return null;
			}
		}

		private void BuildCache(bool closeConnection)
		{
			DbCommand sourceCommand = SourceCommand;
			if (sourceCommand == null)
			{
				throw new InvalidOperationException("The DataAdapter.SelectCommand property needs to be initialized.");
			}
			DbConnection connection = sourceCommand.Connection;
			if (connection == null)
			{
				throw new InvalidOperationException("The DataAdapter.SelectCommand.Connection property needs to be initialized.");
			}
			if (_dbSchemaTable == null)
			{
				if (connection.State == ConnectionState.Open)
				{
					closeConnection = false;
				}
				else
				{
					connection.Open();
				}
				DbDataReader dbDataReader = sourceCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
				_dbSchemaTable = dbDataReader.GetSchemaTable();
				dbDataReader.Close();
				if (closeConnection)
				{
					connection.Close();
				}
				BuildInformation(_dbSchemaTable);
			}
		}

		private string GetQuotedString(string value)
		{
			if (value == string.Empty || value == null)
			{
				return value;
			}
			string quotePrefix = QuotePrefix;
			string quoteSuffix = QuoteSuffix;
			if (quotePrefix.Length == 0 && quoteSuffix.Length == 0)
			{
				return value;
			}
			return string.Format("{0}{1}{2}", quotePrefix, value, quoteSuffix);
		}

		private void BuildInformation(DataTable schemaTable)
		{
			_tableName = string.Empty;
			foreach (DataRow row in schemaTable.Rows)
			{
				if (!row.IsNull("BaseTableName") && !((string)row["BaseTableName"] == string.Empty))
				{
					if (_tableName == string.Empty)
					{
						_tableName = (string)row["BaseTableName"];
					}
					else if (_tableName != (string)row["BaseTableName"])
					{
						throw new InvalidOperationException("Dynamic SQL generation is not supported against multiple base tables.");
					}
				}
			}
			if (_tableName == string.Empty)
			{
				throw new InvalidOperationException("Dynamic SQL generation is not supported with no base table.");
			}
			_dbSchemaTable = schemaTable;
		}

		private bool IncludedInInsert(DataRow schemaRow)
		{
			if (!schemaRow.IsNull("IsAutoIncrement") && (bool)schemaRow["IsAutoIncrement"])
			{
				return false;
			}
			if (!schemaRow.IsNull("IsExpression") && (bool)schemaRow["IsExpression"])
			{
				return false;
			}
			if (!schemaRow.IsNull("IsRowVersion") && (bool)schemaRow["IsRowVersion"])
			{
				return false;
			}
			if (!schemaRow.IsNull("IsReadOnly") && (bool)schemaRow["IsReadOnly"])
			{
				return false;
			}
			return true;
		}

		private bool IncludedInUpdate(DataRow schemaRow)
		{
			if (!schemaRow.IsNull("IsAutoIncrement") && (bool)schemaRow["IsAutoIncrement"])
			{
				return false;
			}
			if (!schemaRow.IsNull("IsRowVersion") && (bool)schemaRow["IsRowVersion"])
			{
				return false;
			}
			if (!schemaRow.IsNull("IsExpression") && (bool)schemaRow["IsExpression"])
			{
				return false;
			}
			if (!schemaRow.IsNull("IsReadOnly") && (bool)schemaRow["IsReadOnly"])
			{
				return false;
			}
			return true;
		}

		private bool IncludedInWhereClause(DataRow schemaRow)
		{
			if ((bool)schemaRow["IsLong"])
			{
				return false;
			}
			return true;
		}

		private DbCommand CreateDeleteCommand(bool option)
		{
			if (QuotedTableName == string.Empty)
			{
				return null;
			}
			CreateNewCommand(ref _deleteCommand);
			string arg = string.Format("DELETE FROM {0}", QuotedTableName);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int num = 1;
			foreach (DataRow row in _dbSchemaTable.Rows)
			{
				if ((!row.IsNull("IsExpression") && (bool)row["IsExpression"]) || !IncludedInWhereClause(row))
				{
					continue;
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" AND ");
				}
				bool flag2 = (bool)row["IsKey"];
				DbParameter dbParameter = null;
				if (flag2)
				{
					flag = true;
				}
				bool flag3 = (bool)row["AllowDBNull"];
				if (!flag2 && flag3)
				{
					dbParameter = _deleteCommand.CreateParameter();
					if (option)
					{
						dbParameter.ParameterName = string.Format("@IsNull_{0}", row["BaseColumnName"]);
					}
					else
					{
						dbParameter.ParameterName = string.Format("@p{0}", num++);
					}
					dbParameter.Value = 1;
					dbParameter.DbType = DbType.Int32;
					string value = (dbParameter.SourceColumn = (string)row["BaseColumnName"]);
					dbParameter.SourceColumnNullMapping = true;
					dbParameter.SourceVersion = DataRowVersion.Original;
					_deleteCommand.Parameters.Add(dbParameter);
					stringBuilder.Append("(");
					stringBuilder.Append(string.Format(clause1, dbParameter.ParameterName, GetQuotedString(value)));
					stringBuilder.Append(" OR ");
				}
				dbParameter = ((!option) ? CreateParameter(_deleteCommand, num++, row) : CreateParameter(_deleteCommand, row, true));
				dbParameter.SourceVersion = DataRowVersion.Original;
				ApplyParameterInfo(dbParameter, row, StatementType.Delete, true);
				stringBuilder.Append(string.Format(clause2, GetQuotedString(dbParameter.SourceColumn), dbParameter.ParameterName));
				if (!flag2 && flag3)
				{
					stringBuilder.Append(")");
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException("Dynamic SQL generation for the DeleteCommand is not supported against a SelectCommand that does not return any key column information.");
			}
			string commandText = string.Format("{0} WHERE ({1})", arg, stringBuilder.ToString());
			_deleteCommand.CommandText = commandText;
			_dbCommand = _deleteCommand;
			return _deleteCommand;
		}

		private DbCommand CreateInsertCommand(bool option, DataRow row)
		{
			if (QuotedTableName == string.Empty)
			{
				return null;
			}
			CreateNewCommand(ref _insertCommand);
			string arg = string.Format("INSERT INTO {0}", QuotedTableName);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			int num = 1;
			DbParameter dbParameter = null;
			foreach (DataRow row2 in _dbSchemaTable.Rows)
			{
				if (IncludedInInsert(row2))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
						stringBuilder2.Append(", ");
					}
					dbParameter = ((!option) ? CreateParameter(_insertCommand, num++, row2) : CreateParameter(_insertCommand, row2, false));
					dbParameter.SourceVersion = DataRowVersion.Current;
					ApplyParameterInfo(dbParameter, row2, StatementType.Insert, false);
					stringBuilder.Append(GetQuotedString(dbParameter.SourceColumn));
					string columnName = row2["ColumnName"] as string;
					if (!(!row2.IsNull("AllowDBNull") & (bool)row2["AllowDBNull"]) && row != null && (row[columnName] == DBNull.Value || row[columnName] == null))
					{
						stringBuilder2.Append("DEFAULT");
					}
					else
					{
						stringBuilder2.Append(dbParameter.ParameterName);
					}
				}
			}
			string commandText = string.Format("{0} ({1}) VALUES ({2})", arg, stringBuilder.ToString(), stringBuilder2.ToString());
			_insertCommand.CommandText = commandText;
			_dbCommand = _insertCommand;
			return _insertCommand;
		}

		private void CreateNewCommand(ref DbCommand command)
		{
			DbCommand sourceCommand = SourceCommand;
			if (command == null)
			{
				command = sourceCommand.Connection.CreateCommand();
				command.CommandTimeout = sourceCommand.CommandTimeout;
				command.Transaction = sourceCommand.Transaction;
			}
			command.CommandType = CommandType.Text;
			command.UpdatedRowSource = UpdateRowSource.None;
			command.Parameters.Clear();
		}

		private DbCommand CreateUpdateCommand(bool option)
		{
			if (QuotedTableName == string.Empty)
			{
				return null;
			}
			CreateNewCommand(ref _updateCommand);
			string arg = string.Format("UPDATE {0} SET ", QuotedTableName);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			int num = 1;
			bool flag = false;
			DbParameter dbParameter = null;
			foreach (DataRow row in _dbSchemaTable.Rows)
			{
				if (IncludedInUpdate(row))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					dbParameter = ((!option) ? CreateParameter(_updateCommand, num++, row) : CreateParameter(_updateCommand, row, false));
					dbParameter.SourceVersion = DataRowVersion.Current;
					ApplyParameterInfo(dbParameter, row, StatementType.Update, false);
					stringBuilder.Append(string.Format("{0} = {1}", GetQuotedString(dbParameter.SourceColumn), dbParameter.ParameterName));
				}
			}
			foreach (DataRow row2 in _dbSchemaTable.Rows)
			{
				if ((!row2.IsNull("IsExpression") && (bool)row2["IsExpression"]) || !IncludedInWhereClause(row2))
				{
					continue;
				}
				if (stringBuilder2.Length > 0)
				{
					stringBuilder2.Append(" AND ");
				}
				bool flag2 = (bool)row2["IsKey"];
				if (flag2)
				{
					flag = true;
				}
				bool flag3 = (bool)row2["AllowDBNull"];
				if (!flag2 && flag3)
				{
					dbParameter = _updateCommand.CreateParameter();
					if (option)
					{
						dbParameter.ParameterName = string.Format("@IsNull_{0}", row2["BaseColumnName"]);
					}
					else
					{
						dbParameter.ParameterName = string.Format("@p{0}", num++);
					}
					dbParameter.DbType = DbType.Int32;
					dbParameter.Value = 1;
					dbParameter.SourceColumn = (string)row2["BaseColumnName"];
					dbParameter.SourceColumnNullMapping = true;
					dbParameter.SourceVersion = DataRowVersion.Original;
					stringBuilder2.Append("(");
					stringBuilder2.Append(string.Format(clause1, dbParameter.ParameterName, GetQuotedString((string)row2["BaseColumnName"])));
					stringBuilder2.Append(" OR ");
					_updateCommand.Parameters.Add(dbParameter);
				}
				dbParameter = ((!option) ? CreateParameter(_updateCommand, num++, row2) : CreateParameter(_updateCommand, row2, true));
				dbParameter.SourceVersion = DataRowVersion.Original;
				ApplyParameterInfo(dbParameter, row2, StatementType.Update, true);
				stringBuilder2.Append(string.Format(clause2, GetQuotedString(dbParameter.SourceColumn), dbParameter.ParameterName));
				if (!flag2 && flag3)
				{
					stringBuilder2.Append(")");
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException("Dynamic SQL generation for the UpdateCommand is not supported against a SelectCommand that does not return any key column information.");
			}
			string commandText = string.Format("{0}{1} WHERE ({2})", arg, stringBuilder.ToString(), stringBuilder2.ToString());
			_updateCommand.CommandText = commandText;
			_dbCommand = _updateCommand;
			return _updateCommand;
		}

		private DbParameter CreateParameter(DbCommand _dbCommand, DataRow schemaRow, bool whereClause)
		{
			string text = (string)schemaRow["BaseColumnName"];
			DbParameter dbParameter = _dbCommand.CreateParameter();
			if (whereClause)
			{
				dbParameter.ParameterName = GetParameterName("Original_" + text);
			}
			else
			{
				dbParameter.ParameterName = GetParameterName(text);
			}
			dbParameter.SourceColumn = text;
			_dbCommand.Parameters.Add(dbParameter);
			return dbParameter;
		}

		private DbParameter CreateParameter(DbCommand _dbCommand, int paramIndex, DataRow schemaRow)
		{
			string sourceColumn = (string)schemaRow["BaseColumnName"];
			DbParameter dbParameter = _dbCommand.CreateParameter();
			dbParameter.ParameterName = GetParameterName(paramIndex);
			dbParameter.SourceColumn = sourceColumn;
			_dbCommand.Parameters.Add(dbParameter);
			return dbParameter;
		}

		protected abstract void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause);

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}
			if (disposing)
			{
				if (_insertCommand != null)
				{
					_insertCommand.Dispose();
				}
				if (_deleteCommand != null)
				{
					_deleteCommand.Dispose();
				}
				if (_updateCommand != null)
				{
					_updateCommand.Dispose();
				}
				if (_dbSchemaTable != null)
				{
					_dbSchemaTable.Dispose();
				}
			}
			_disposed = true;
		}

		public DbCommand GetDeleteCommand()
		{
			return GetDeleteCommand(false);
		}

		public DbCommand GetDeleteCommand(bool option)
		{
			BuildCache(true);
			if (_deleteCommand == null || option)
			{
				return CreateDeleteCommand(option);
			}
			return _deleteCommand;
		}

		public DbCommand GetInsertCommand()
		{
			return GetInsertCommand(false, null);
		}

		public DbCommand GetInsertCommand(bool option)
		{
			return GetInsertCommand(option, null);
		}

		internal DbCommand GetInsertCommand(bool option, DataRow row)
		{
			BuildCache(true);
			if (_insertCommand == null || option)
			{
				return CreateInsertCommand(option, row);
			}
			return _insertCommand;
		}

		public DbCommand GetUpdateCommand()
		{
			return GetUpdateCommand(false);
		}

		public DbCommand GetUpdateCommand(bool option)
		{
			BuildCache(true);
			if (_updateCommand == null || option)
			{
				return CreateUpdateCommand(option);
			}
			return _updateCommand;
		}

		protected virtual DbCommand InitializeCommand(DbCommand command)
		{
			if (_dbCommand == null)
			{
				_dbCommand = SourceCommand;
			}
			else
			{
				_dbCommand.CommandTimeout = 30;
				_dbCommand.Transaction = null;
				_dbCommand.CommandType = CommandType.Text;
				_dbCommand.UpdatedRowSource = UpdateRowSource.None;
			}
			return _dbCommand;
		}

		public virtual string QuoteIdentifier(string unquotedIdentifier)
		{
			throw new NotSupportedException();
		}

		public virtual string UnquoteIdentifier(string quotedIdentifier)
		{
			if (quotedIdentifier == null)
			{
				throw new ArgumentNullException("Quoted identifier parameter cannot be null");
			}
			string text = quotedIdentifier.Trim();
			if (text.StartsWith(QuotePrefix))
			{
				text = text.Remove(0, 1);
			}
			if (text.EndsWith(QuoteSuffix))
			{
				text = text.Remove(text.Length - 1, 1);
			}
			return text;
		}

		public virtual void RefreshSchema()
		{
			_tableName = string.Empty;
			_dbSchemaTable = null;
			_deleteCommand = null;
			_updateCommand = null;
			_insertCommand = null;
		}

		protected void RowUpdatingHandler(RowUpdatingEventArgs args)
		{
			if (args.Command != null)
			{
				return;
			}
			try
			{
				switch (args.StatementType)
				{
				case StatementType.Insert:
					args.Command = GetInsertCommand(false, args.Row);
					break;
				case StatementType.Update:
					args.Command = GetUpdateCommand();
					break;
				case StatementType.Delete:
					args.Command = GetDeleteCommand();
					break;
				}
			}
			catch (Exception errors)
			{
				args.Errors = errors;
				args.Status = UpdateStatus.ErrorsOccurred;
			}
		}

		protected abstract string GetParameterName(int parameterOrdinal);

		protected abstract string GetParameterName(string parameterName);

		protected abstract string GetParameterPlaceholder(int parameterOrdinal);

		protected abstract void SetRowUpdatingHandler(DbDataAdapter adapter);

		protected virtual DataTable GetSchemaTable(DbCommand cmd)
		{
			using (DbDataReader dbDataReader = cmd.ExecuteReader())
			{
				return dbDataReader.GetSchemaTable();
			}
		}

		private static void CheckEnumValue(Type type, int value)
		{
			if (Enum.IsDefined(type, value))
			{
				return;
			}
			string name = type.Name;
			string message = string.Format(CultureInfo.CurrentCulture, "Value {0} is not valid for {1}.", value, name);
			throw new ArgumentOutOfRangeException(name, message);
		}
	}
}
