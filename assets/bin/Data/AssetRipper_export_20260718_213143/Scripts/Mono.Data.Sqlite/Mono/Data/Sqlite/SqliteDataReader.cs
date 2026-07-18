using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Mono.Data.Sqlite
{
	public sealed class SqliteDataReader : DbDataReader
	{
		private SqliteCommand _command;

		private int _activeStatementIndex;

		private SqliteStatement _activeStatement;

		private int _readingState;

		private int _rowsAffected;

		private int _fieldCount;

		private SQLiteType[] _fieldTypeArray;

		private CommandBehavior _commandBehavior;

		internal bool _disposeCommand;

		private SqliteKeyReader _keyInfo;

		internal long _version;

		public override int Depth
		{
			get
			{
				CheckClosed();
				return 0;
			}
		}

		public override int FieldCount
		{
			get
			{
				CheckClosed();
				if (_keyInfo == null)
				{
					return _fieldCount;
				}
				return _fieldCount + _keyInfo.Count;
			}
		}

		public override int VisibleFieldCount
		{
			get
			{
				CheckClosed();
				return _fieldCount;
			}
		}

		public override bool HasRows
		{
			get
			{
				CheckClosed();
				return _readingState != 1;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return _command == null;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return (_rowsAffected >= 0) ? _rowsAffected : 0;
			}
		}

		public override object this[string name]
		{
			get
			{
				return GetValue(GetOrdinal(name));
			}
		}

		public override object this[int i]
		{
			get
			{
				return GetValue(i);
			}
		}

		internal SqliteDataReader(SqliteCommand cmd, CommandBehavior behave)
		{
			_command = cmd;
			_version = _command.Connection._version;
			_commandBehavior = behave;
			_activeStatementIndex = -1;
			_activeStatement = null;
			_rowsAffected = -1;
			_fieldCount = 0;
			if (_command != null)
			{
				NextResult();
			}
		}

		internal void Cancel()
		{
			_version = 0L;
		}

		public override void Close()
		{
			try
			{
				if (_command != null)
				{
					try
					{
						try
						{
							if (_version != 0L)
							{
								try
								{
									while (NextResult())
									{
									}
								}
								catch
								{
								}
							}
							_command.ClearDataReader();
						}
						finally
						{
							if ((_commandBehavior & CommandBehavior.CloseConnection) != CommandBehavior.Default && _command.Connection != null)
							{
								_command.Connection.Close();
							}
						}
					}
					finally
					{
						if (_disposeCommand)
						{
							_command.Dispose();
						}
					}
				}
				_command = null;
				_activeStatement = null;
				_fieldTypeArray = null;
			}
			finally
			{
				if (_keyInfo != null)
				{
					_keyInfo.Dispose();
					_keyInfo = null;
				}
			}
		}

		private void CheckClosed()
		{
			if (_command == null)
			{
				throw new InvalidOperationException("DataReader has been closed");
			}
			if (_version == 0L)
			{
				throw new SqliteException(4, "Execution was aborted by the user");
			}
			if (_command.Connection.State != ConnectionState.Open || _command.Connection._version != _version)
			{
				throw new InvalidOperationException("Connection was closed, statement was terminated");
			}
		}

		private void CheckValidRow()
		{
			if (_readingState != 0)
			{
				throw new InvalidOperationException("No current row");
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this, (_commandBehavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection);
		}

		private TypeAffinity VerifyType(int i, DbType typ)
		{
			CheckClosed();
			CheckValidRow();
			TypeAffinity affinity = GetSQLiteType(i).Affinity;
			switch (affinity)
			{
			case TypeAffinity.Int64:
				switch (typ)
				{
				case DbType.Int16:
					return affinity;
				case DbType.Int32:
					return affinity;
				case DbType.Int64:
					return affinity;
				case DbType.Boolean:
					return affinity;
				case DbType.Byte:
					return affinity;
				case DbType.DateTime:
					return affinity;
				case DbType.Single:
					return affinity;
				case DbType.Double:
					return affinity;
				case DbType.Decimal:
					return affinity;
				}
				break;
			case TypeAffinity.Double:
				switch (typ)
				{
				case DbType.Single:
					return affinity;
				case DbType.Double:
					return affinity;
				case DbType.Decimal:
					return affinity;
				case DbType.DateTime:
					return affinity;
				}
				break;
			case TypeAffinity.Text:
				switch (typ)
				{
				case DbType.SByte:
					return affinity;
				case DbType.String:
					return affinity;
				}
				switch (typ)
				{
				case DbType.SByte:
					return affinity;
				case DbType.Guid:
					return affinity;
				case DbType.DateTime:
					return affinity;
				case DbType.Decimal:
					return affinity;
				}
				break;
			case TypeAffinity.Blob:
				switch (typ)
				{
				case DbType.Guid:
					return affinity;
				case DbType.String:
					return affinity;
				case DbType.Binary:
					return affinity;
				}
				break;
			}
			throw new InvalidCastException();
		}

		public override bool GetBoolean(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetBoolean(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.Boolean);
			return Convert.ToBoolean(GetValue(i), CultureInfo.CurrentCulture);
		}

		public override byte GetByte(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetByte(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.Byte);
			return Convert.ToByte(_activeStatement._sql.GetInt32(_activeStatement, i));
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetBytes(i - VisibleFieldCount, fieldOffset, buffer, bufferoffset, length);
			}
			VerifyType(i, DbType.Binary);
			return _activeStatement._sql.GetBytes(_activeStatement, i, (int)fieldOffset, buffer, bufferoffset, length);
		}

		public override char GetChar(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetChar(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.SByte);
			return Convert.ToChar(_activeStatement._sql.GetInt32(_activeStatement, i));
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetChars(i - VisibleFieldCount, fieldoffset, buffer, bufferoffset, length);
			}
			VerifyType(i, DbType.String);
			return _activeStatement._sql.GetChars(_activeStatement, i, (int)fieldoffset, buffer, bufferoffset, length);
		}

		public override string GetDataTypeName(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetDataTypeName(i - VisibleFieldCount);
			}
			SQLiteType sQLiteType = GetSQLiteType(i);
			if (sQLiteType.Type == DbType.Object)
			{
				return SqliteConvert.SQLiteTypeToType(sQLiteType).Name;
			}
			return _activeStatement._sql.ColumnType(_activeStatement, i, out sQLiteType.Affinity);
		}

		public override DateTime GetDateTime(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetDateTime(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.DateTime);
			return _activeStatement._sql.GetDateTime(_activeStatement, i);
		}

		public override decimal GetDecimal(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetDecimal(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.Decimal);
			return decimal.Parse(_activeStatement._sql.GetText(_activeStatement, i), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture);
		}

		public override double GetDouble(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetDouble(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.Double);
			return _activeStatement._sql.GetDouble(_activeStatement, i);
		}

		public override Type GetFieldType(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetFieldType(i - VisibleFieldCount);
			}
			return SqliteConvert.SQLiteTypeToType(GetSQLiteType(i));
		}

		public override float GetFloat(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetFloat(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.Single);
			return Convert.ToSingle(_activeStatement._sql.GetDouble(_activeStatement, i));
		}

		public override Guid GetGuid(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetGuid(i - VisibleFieldCount);
			}
			TypeAffinity typeAffinity = VerifyType(i, DbType.Guid);
			if (typeAffinity == TypeAffinity.Blob)
			{
				byte[] array = new byte[16];
				_activeStatement._sql.GetBytes(_activeStatement, i, 0, array, 0, 16);
				return new Guid(array);
			}
			return new Guid(_activeStatement._sql.GetText(_activeStatement, i));
		}

		public override short GetInt16(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetInt16(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.Int16);
			return Convert.ToInt16(_activeStatement._sql.GetInt32(_activeStatement, i));
		}

		public override int GetInt32(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetInt32(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.Int32);
			return _activeStatement._sql.GetInt32(_activeStatement, i);
		}

		public override long GetInt64(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetInt64(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.Int64);
			return _activeStatement._sql.GetInt64(_activeStatement, i);
		}

		public override string GetName(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetName(i - VisibleFieldCount);
			}
			return _activeStatement._sql.ColumnName(_activeStatement, i);
		}

		public override int GetOrdinal(string name)
		{
			CheckClosed();
			int num = _activeStatement._sql.ColumnIndex(_activeStatement, name);
			if (num == -1 && _keyInfo != null)
			{
				num = _keyInfo.GetOrdinal(name);
				if (num > -1)
				{
					num += VisibleFieldCount;
				}
			}
			return num;
		}

		public override DataTable GetSchemaTable()
		{
			return GetSchemaTable(true, false);
		}

		internal DataTable GetSchemaTable(bool wantUniqueInfo, bool wantDefaultValue)
		{
			CheckClosed();
			DataTable dataTable = new DataTable("SchemaTable");
			DataTable dataTable2 = null;
			string text = string.Empty;
			string text2 = string.Empty;
			string empty = string.Empty;
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add(SchemaTableColumn.ColumnName, typeof(string));
			dataTable.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int));
			dataTable.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int));
			dataTable.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(short));
			dataTable.Columns.Add(SchemaTableColumn.NumericScale, typeof(short));
			dataTable.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool));
			dataTable.Columns.Add(SchemaTableColumn.IsKey, typeof(bool));
			dataTable.Columns.Add(SchemaTableOptionalColumn.BaseServerName, typeof(string));
			dataTable.Columns.Add(SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
			dataTable.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string));
			dataTable.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string));
			dataTable.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string));
			dataTable.Columns.Add(SchemaTableColumn.DataType, typeof(Type));
			dataTable.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool));
			dataTable.Columns.Add(SchemaTableColumn.ProviderType, typeof(int));
			dataTable.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool));
			dataTable.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool));
			dataTable.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
			dataTable.Columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool));
			dataTable.Columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool));
			dataTable.Columns.Add(SchemaTableColumn.IsLong, typeof(bool));
			dataTable.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool));
			dataTable.Columns.Add(SchemaTableOptionalColumn.ProviderSpecificDataType, typeof(Type));
			dataTable.Columns.Add(SchemaTableOptionalColumn.DefaultValue, typeof(object));
			dataTable.Columns.Add("DataTypeName", typeof(string));
			dataTable.Columns.Add("CollationType", typeof(string));
			dataTable.BeginLoadData();
			for (int i = 0; i < _fieldCount; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				DbType type = GetSQLiteType(i).Type;
				dataRow[SchemaTableColumn.ColumnName] = GetName(i);
				dataRow[SchemaTableColumn.ColumnOrdinal] = i;
				dataRow[SchemaTableColumn.ColumnSize] = SqliteConvert.DbTypeToColumnSize(type);
				dataRow[SchemaTableColumn.NumericPrecision] = SqliteConvert.DbTypeToNumericPrecision(type);
				dataRow[SchemaTableColumn.NumericScale] = SqliteConvert.DbTypeToNumericScale(type);
				dataRow[SchemaTableColumn.ProviderType] = GetSQLiteType(i).Type;
				dataRow[SchemaTableColumn.IsLong] = false;
				dataRow[SchemaTableColumn.AllowDBNull] = true;
				dataRow[SchemaTableOptionalColumn.IsReadOnly] = false;
				dataRow[SchemaTableOptionalColumn.IsRowVersion] = false;
				dataRow[SchemaTableColumn.IsUnique] = false;
				dataRow[SchemaTableColumn.IsKey] = false;
				dataRow[SchemaTableOptionalColumn.IsAutoIncrement] = false;
				dataRow[SchemaTableColumn.DataType] = GetFieldType(i);
				dataRow[SchemaTableOptionalColumn.IsHidden] = false;
				string dataType = null;
				if (!string.IsNullOrEmpty(empty))
				{
					string collateSequence;
					bool notNull;
					bool primaryKey;
					bool autoIncrement;
					_command.Connection._sql.ColumnMetaData((string)dataRow[SchemaTableOptionalColumn.BaseCatalogName], (string)dataRow[SchemaTableColumn.BaseTableName], empty, out dataType, out collateSequence, out notNull, out primaryKey, out autoIncrement);
					if (notNull || primaryKey)
					{
						dataRow[SchemaTableColumn.AllowDBNull] = false;
					}
					dataRow[SchemaTableColumn.IsKey] = primaryKey;
					dataRow[SchemaTableOptionalColumn.IsAutoIncrement] = autoIncrement;
					dataRow["CollationType"] = collateSequence;
					string[] array = dataType.Split('(');
					if (array.Length > 1)
					{
						dataType = array[0];
						array = array[1].Split(')');
						if (array.Length > 1)
						{
							array = array[0].Split(',', '.');
							if (GetSQLiteType(i).Type == DbType.String || GetSQLiteType(i).Type == DbType.Binary)
							{
								dataRow[SchemaTableColumn.ColumnSize] = Convert.ToInt32(array[0], CultureInfo.InvariantCulture);
							}
							else
							{
								dataRow[SchemaTableColumn.NumericPrecision] = Convert.ToInt32(array[0], CultureInfo.InvariantCulture);
								if (array.Length > 1)
								{
									dataRow[SchemaTableColumn.NumericScale] = Convert.ToInt32(array[1], CultureInfo.InvariantCulture);
								}
							}
						}
					}
					if (wantDefaultValue)
					{
						using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "PRAGMA [{0}].TABLE_INFO([{1}])", dataRow[SchemaTableOptionalColumn.BaseCatalogName], dataRow[SchemaTableColumn.BaseTableName]), _command.Connection))
						{
							using (DbDataReader dbDataReader = sqliteCommand.ExecuteReader())
							{
								while (dbDataReader.Read())
								{
									if (string.Compare((string)dataRow[SchemaTableColumn.BaseColumnName], dbDataReader.GetString(1), true, CultureInfo.InvariantCulture) == 0)
									{
										if (!dbDataReader.IsDBNull(4))
										{
											dataRow[SchemaTableOptionalColumn.DefaultValue] = dbDataReader[4];
										}
										break;
									}
								}
							}
						}
					}
					if (wantUniqueInfo)
					{
						if ((string)dataRow[SchemaTableOptionalColumn.BaseCatalogName] != text || (string)dataRow[SchemaTableColumn.BaseTableName] != text2)
						{
							text = (string)dataRow[SchemaTableOptionalColumn.BaseCatalogName];
							text2 = (string)dataRow[SchemaTableColumn.BaseTableName];
							dataTable2 = _command.Connection.GetSchema("Indexes", new string[4]
							{
								(string)dataRow[SchemaTableOptionalColumn.BaseCatalogName],
								null,
								(string)dataRow[SchemaTableColumn.BaseTableName],
								null
							});
						}
						foreach (DataRow row in dataTable2.Rows)
						{
							DataTable schema = _command.Connection.GetSchema("IndexColumns", new string[5]
							{
								(string)dataRow[SchemaTableOptionalColumn.BaseCatalogName],
								null,
								(string)dataRow[SchemaTableColumn.BaseTableName],
								(string)row["INDEX_NAME"],
								null
							});
							foreach (DataRow row2 in schema.Rows)
							{
								if (string.Compare((string)row2["COLUMN_NAME"], empty, true, CultureInfo.InvariantCulture) == 0)
								{
									if (schema.Rows.Count == 1 && !(bool)dataRow[SchemaTableColumn.AllowDBNull])
									{
										dataRow[SchemaTableColumn.IsUnique] = row["UNIQUE"];
									}
									if (schema.Rows.Count == 1 && (bool)row["PRIMARY_KEY"] && !string.IsNullOrEmpty(dataType) && string.Compare(dataType, "integer", true, CultureInfo.InvariantCulture) != 0)
									{
									}
									break;
								}
							}
						}
					}
					if (string.IsNullOrEmpty(dataType))
					{
						TypeAffinity nAffinity;
						dataType = _activeStatement._sql.ColumnType(_activeStatement, i, out nAffinity);
					}
					if (!string.IsNullOrEmpty(dataType))
					{
						dataRow["DataTypeName"] = dataType;
					}
				}
				dataTable.Rows.Add(dataRow);
			}
			if (_keyInfo != null)
			{
				_keyInfo.AppendSchemaTable(dataTable);
			}
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		public override string GetString(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetString(i - VisibleFieldCount);
			}
			VerifyType(i, DbType.String);
			return _activeStatement._sql.GetText(_activeStatement, i);
		}

		public override object GetValue(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.GetValue(i - VisibleFieldCount);
			}
			SQLiteType sQLiteType = GetSQLiteType(i);
			return _activeStatement._sql.GetValue(_activeStatement, i, sQLiteType);
		}

		public override int GetValues(object[] values)
		{
			int num = FieldCount;
			if (values.Length < num)
			{
				num = values.Length;
			}
			for (int i = 0; i < num; i++)
			{
				values[i] = GetValue(i);
			}
			return num;
		}

		public override bool IsDBNull(int i)
		{
			if (i >= VisibleFieldCount && _keyInfo != null)
			{
				return _keyInfo.IsDBNull(i - VisibleFieldCount);
			}
			return _activeStatement._sql.IsNull(_activeStatement, i);
		}

		public override bool NextResult()
		{
			CheckClosed();
			SqliteStatement sqliteStatement = null;
			int num;
			while (true)
			{
				if (_activeStatement != null && sqliteStatement == null)
				{
					_activeStatement._sql.Reset(_activeStatement);
					if ((_commandBehavior & CommandBehavior.SingleResult) != CommandBehavior.Default)
					{
						while (true)
						{
							sqliteStatement = _command.GetStatement(_activeStatementIndex + 1);
							if (sqliteStatement == null)
							{
								break;
							}
							_activeStatementIndex++;
							sqliteStatement._sql.Step(sqliteStatement);
							if (sqliteStatement._sql.ColumnCount(sqliteStatement) == 0)
							{
								if (_rowsAffected == -1)
								{
									_rowsAffected = 0;
								}
								_rowsAffected += sqliteStatement._sql.Changes;
							}
							sqliteStatement._sql.Reset(sqliteStatement);
						}
						return false;
					}
				}
				sqliteStatement = _command.GetStatement(_activeStatementIndex + 1);
				if (sqliteStatement == null)
				{
					return false;
				}
				if (_readingState < 1)
				{
					_readingState = 1;
				}
				_activeStatementIndex++;
				num = sqliteStatement._sql.ColumnCount(sqliteStatement);
				if ((_commandBehavior & CommandBehavior.SchemaOnly) != CommandBehavior.Default && num != 0)
				{
					break;
				}
				if (sqliteStatement._sql.Step(sqliteStatement))
				{
					_readingState = -1;
					break;
				}
				if (num == 0)
				{
					if (_rowsAffected == -1)
					{
						_rowsAffected = 0;
					}
					_rowsAffected += sqliteStatement._sql.Changes;
					sqliteStatement._sql.Reset(sqliteStatement);
					continue;
				}
				_readingState = 1;
				break;
			}
			_activeStatement = sqliteStatement;
			_fieldCount = num;
			_fieldTypeArray = null;
			if ((_commandBehavior & CommandBehavior.KeyInfo) != CommandBehavior.Default)
			{
				LoadKeyInfo();
			}
			return true;
		}

		private SQLiteType GetSQLiteType(int i)
		{
			if (_fieldTypeArray == null)
			{
				_fieldTypeArray = new SQLiteType[VisibleFieldCount];
			}
			if (_fieldTypeArray[i] == null)
			{
				_fieldTypeArray[i] = new SQLiteType();
			}
			SQLiteType sQLiteType = _fieldTypeArray[i];
			if (sQLiteType.Affinity == TypeAffinity.Uninitialized)
			{
				sQLiteType.Type = SqliteConvert.TypeNameToDbType(_activeStatement._sql.ColumnType(_activeStatement, i, out sQLiteType.Affinity));
			}
			else
			{
				sQLiteType.Affinity = _activeStatement._sql.ColumnAffinity(_activeStatement, i);
			}
			return sQLiteType;
		}

		public override bool Read()
		{
			CheckClosed();
			if (_readingState == -1)
			{
				_readingState = 0;
				return true;
			}
			if (_readingState == 0)
			{
				if ((_commandBehavior & CommandBehavior.SingleRow) == 0 && _activeStatement._sql.Step(_activeStatement))
				{
					if (_keyInfo != null)
					{
						_keyInfo.Reset();
					}
					return true;
				}
				_readingState = 1;
			}
			return false;
		}

		private void LoadKeyInfo()
		{
			if (_keyInfo != null)
			{
				_keyInfo.Dispose();
			}
			_keyInfo = new SqliteKeyReader(_command.Connection, this, _activeStatement);
		}
	}
}
