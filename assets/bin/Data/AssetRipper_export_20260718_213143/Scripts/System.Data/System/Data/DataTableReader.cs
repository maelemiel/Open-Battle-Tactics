using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace System.Data
{
	public sealed class DataTableReader : DbDataReader
	{
		private bool _closed;

		private DataTable[] _tables;

		private int _current = -1;

		private int _index;

		private DataTable _schemaTable;

		private bool _tableCleared;

		private bool _subscribed;

		private DataRow _rowRef;

		private bool _schemaChanged;

		public override int Depth
		{
			get
			{
				return 0;
			}
		}

		public override int FieldCount
		{
			get
			{
				return CurrentTable.Columns.Count;
			}
		}

		public override bool HasRows
		{
			get
			{
				return CurrentTable.Rows.Count > 0;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return _closed;
			}
		}

		public override object this[int index]
		{
			get
			{
				Validate();
				if (index < 0 || index >= FieldCount)
				{
					throw new ArgumentOutOfRangeException("index " + index + " is not in the range");
				}
				DataRow currentRow = CurrentRow;
				if (currentRow.RowState == DataRowState.Deleted)
				{
					throw new InvalidOperationException("Deleted Row's information cannot be accessed!");
				}
				return currentRow[index];
			}
		}

		private DataTable CurrentTable
		{
			get
			{
				return _tables[_index];
			}
		}

		private DataRow CurrentRow
		{
			get
			{
				return CurrentTable.Rows[_current];
			}
		}

		public override object this[string name]
		{
			get
			{
				Validate();
				DataRow currentRow = CurrentRow;
				if (currentRow.RowState == DataRowState.Deleted)
				{
					throw new InvalidOperationException("Deleted Row's information cannot be accessed!");
				}
				return currentRow[name];
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return 0;
			}
		}

		public DataTableReader(DataTable dt)
			: this(new DataTable[1] { dt })
		{
		}

		public DataTableReader(DataTable[] dataTables)
		{
			if (dataTables == null || dataTables.Length <= 0)
			{
				throw new ArgumentException("Cannot Create DataTable. Argument Empty!");
			}
			_tables = new DataTable[dataTables.Length];
			for (int i = 0; i < dataTables.Length; i++)
			{
				_tables[i] = dataTables[i];
			}
			_closed = false;
			_index = 0;
			_current = -1;
			_rowRef = null;
			_tableCleared = false;
			SubscribeEvents();
		}

		private void SubscribeEvents()
		{
			if (!_subscribed)
			{
				CurrentTable.TableCleared += OnTableCleared;
				CurrentTable.RowChanged += OnRowChanged;
				CurrentTable.Columns.CollectionChanged += OnColumnCollectionChanged;
				for (int i = 0; i < CurrentTable.Columns.Count; i++)
				{
					CurrentTable.Columns[i].PropertyChanged += OnColumnChanged;
				}
				_subscribed = true;
				_schemaChanged = false;
			}
		}

		private void UnsubscribeEvents()
		{
			if (_subscribed)
			{
				CurrentTable.TableCleared -= OnTableCleared;
				CurrentTable.RowChanged -= OnRowChanged;
				CurrentTable.Columns.CollectionChanged -= OnColumnCollectionChanged;
				for (int i = 0; i < CurrentTable.Columns.Count; i++)
				{
					CurrentTable.Columns[i].PropertyChanged -= OnColumnChanged;
				}
				_subscribed = false;
				_schemaChanged = false;
			}
		}

		public override void Close()
		{
			if (!IsClosed)
			{
				UnsubscribeEvents();
				_closed = true;
			}
		}

		public override bool GetBoolean(int i)
		{
			return (bool)GetValue(i);
		}

		public override byte GetByte(int i)
		{
			return (byte)GetValue(i);
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			byte[] array = this[i] as byte[];
			if (array == null)
			{
				ThrowInvalidCastException(this[i].GetType(), typeof(byte[]));
			}
			if (buffer == null)
			{
				return array.Length;
			}
			int num = ((length <= array.Length) ? length : array.Length);
			Array.Copy(array, dataIndex, buffer, bufferIndex, num);
			return num;
		}

		public override char GetChar(int i)
		{
			return (char)GetValue(i);
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			char[] array = this[i] as char[];
			if (array == null)
			{
				ThrowInvalidCastException(this[i].GetType(), typeof(char[]));
			}
			if (buffer == null)
			{
				return array.Length;
			}
			int num = ((length <= array.Length) ? length : array.Length);
			Array.Copy(array, dataIndex, buffer, bufferIndex, num);
			return num;
		}

		public override string GetDataTypeName(int i)
		{
			return GetFieldType(i).ToString();
		}

		public override DateTime GetDateTime(int i)
		{
			return (DateTime)GetValue(i);
		}

		public override decimal GetDecimal(int i)
		{
			return (decimal)GetValue(i);
		}

		public override double GetDouble(int i)
		{
			return (double)GetValue(i);
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this);
		}

		public override Type GetProviderSpecificFieldType(int i)
		{
			return GetFieldType(i);
		}

		public override Type GetFieldType(int i)
		{
			ValidateClosed();
			return CurrentTable.Columns[i].DataType;
		}

		public override float GetFloat(int i)
		{
			return (float)GetValue(i);
		}

		public override Guid GetGuid(int i)
		{
			return (Guid)GetValue(i);
		}

		public override short GetInt16(int i)
		{
			return (short)GetValue(i);
		}

		public override int GetInt32(int i)
		{
			return (int)GetValue(i);
		}

		public override long GetInt64(int i)
		{
			return (long)GetValue(i);
		}

		public override string GetName(int i)
		{
			ValidateClosed();
			return CurrentTable.Columns[i].ColumnName;
		}

		public override int GetOrdinal(string name)
		{
			ValidateClosed();
			int num = CurrentTable.Columns.IndexOf(name);
			if (num == -1)
			{
				throw new ArgumentException(string.Format("Column {0} is not found in the schema", name));
			}
			return num;
		}

		public override object GetProviderSpecificValue(int i)
		{
			return GetValue(i);
		}

		public override int GetProviderSpecificValues(object[] values)
		{
			return GetValues(values);
		}

		public override string GetString(int i)
		{
			return (string)GetValue(i);
		}

		public override object GetValue(int i)
		{
			return this[i];
		}

		public override int GetValues(object[] values)
		{
			Validate();
			if (CurrentRow.RowState == DataRowState.Deleted)
			{
				throw new DeletedRowInaccessibleException(string.Empty);
			}
			int num = ((FieldCount >= values.Length) ? values.Length : FieldCount);
			for (int i = 0; i < num; i++)
			{
				values[i] = CurrentRow[i];
			}
			return num;
		}

		public override bool IsDBNull(int i)
		{
			return GetValue(i) is DBNull;
		}

		public override DataTable GetSchemaTable()
		{
			ValidateClosed();
			ValidateSchemaIntact();
			if (_schemaTable != null)
			{
				return _schemaTable;
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("ColumnOrdinal", typeof(int));
			dataTable.Columns.Add("ColumnSize", typeof(int));
			dataTable.Columns.Add("NumericPrecision", typeof(short));
			dataTable.Columns.Add("NumericScale", typeof(short));
			dataTable.Columns.Add("DataType", typeof(Type));
			dataTable.Columns.Add("ProviderType", typeof(int));
			dataTable.Columns.Add("IsLong", typeof(bool));
			dataTable.Columns.Add("AllowDBNull", typeof(bool));
			dataTable.Columns.Add("IsReadOnly", typeof(bool));
			dataTable.Columns.Add("IsRowVersion", typeof(bool));
			dataTable.Columns.Add("IsUnique", typeof(bool));
			dataTable.Columns.Add("IsKey", typeof(bool));
			dataTable.Columns.Add("IsAutoIncrement", typeof(bool));
			dataTable.Columns.Add("BaseCatalogName", typeof(string));
			dataTable.Columns.Add("BaseSchemaName", typeof(string));
			dataTable.Columns.Add("BaseTableName", typeof(string));
			dataTable.Columns.Add("BaseColumnName", typeof(string));
			dataTable.Columns.Add("AutoIncrementSeed", typeof(long));
			dataTable.Columns.Add("AutoIncrementStep", typeof(long));
			dataTable.Columns.Add("DefaultValue", typeof(object));
			dataTable.Columns.Add("Expression", typeof(string));
			dataTable.Columns.Add("ColumnMapping", typeof(MappingType));
			dataTable.Columns.Add("BaseTableNamespace", typeof(string));
			dataTable.Columns.Add("BaseColumnNamespace", typeof(string));
			for (int i = 0; i < CurrentTable.Columns.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				DataColumn dataColumn = CurrentTable.Columns[i];
				dataRow["ColumnName"] = dataColumn.ColumnName;
				dataRow["BaseColumnName"] = dataColumn.ColumnName;
				dataRow["ColumnOrdinal"] = dataColumn.Ordinal;
				dataRow["ColumnSize"] = dataColumn.MaxLength;
				dataRow["NumericPrecision"] = DBNull.Value;
				dataRow["NumericScale"] = DBNull.Value;
				dataRow["DataType"] = dataColumn.DataType;
				dataRow["ProviderType"] = DBNull.Value;
				dataRow["IsLong"] = false;
				dataRow["AllowDBNull"] = dataColumn.AllowDBNull;
				dataRow["IsReadOnly"] = dataColumn.ReadOnly;
				dataRow["IsRowVersion"] = false;
				dataRow["IsUnique"] = dataColumn.Unique;
				dataRow["IsKey"] = Array.IndexOf(CurrentTable.PrimaryKey, dataColumn) != -1;
				dataRow["IsAutoIncrement"] = dataColumn.AutoIncrement;
				dataRow["AutoIncrementSeed"] = dataColumn.AutoIncrementSeed;
				dataRow["AutoIncrementStep"] = dataColumn.AutoIncrementStep;
				dataRow["BaseCatalogName"] = ((CurrentTable.DataSet == null) ? null : CurrentTable.DataSet.DataSetName);
				dataRow["BaseSchemaName"] = DBNull.Value;
				dataRow["BaseTableName"] = CurrentTable.TableName;
				dataRow["DefaultValue"] = dataColumn.DefaultValue;
				if (dataColumn.Expression == string.Empty)
				{
					dataRow["Expression"] = dataColumn.Expression;
				}
				else
				{
					Regex regex = new Regex("((Parent|Child)( )*[.(])", RegexOptions.IgnoreCase);
					if (regex.IsMatch(dataColumn.Expression, 0))
					{
						dataRow["Expression"] = DBNull.Value;
					}
					else
					{
						dataRow["Expression"] = dataColumn.Expression;
					}
				}
				dataRow["ColumnMapping"] = dataColumn.ColumnMapping;
				dataRow["BaseTableNamespace"] = CurrentTable.Namespace;
				dataRow["BaseColumnNamespace"] = dataColumn.Namespace;
				dataTable.Rows.Add(dataRow);
			}
			return _schemaTable = dataTable;
		}

		private void Validate()
		{
			ValidateClosed();
			if (_index >= _tables.Length)
			{
				throw new InvalidOperationException("Invalid attempt to read when no data is present");
			}
			if (_tableCleared)
			{
				throw new RowNotInTableException("The table is cleared, no rows are accessible");
			}
			if (_current == -1)
			{
				throw new InvalidOperationException("DataReader is invalid for the DataTable");
			}
			ValidateSchemaIntact();
		}

		private void ValidateClosed()
		{
			if (IsClosed)
			{
				throw new InvalidOperationException("Invalid attempt to read when the reader is closed");
			}
		}

		private void ValidateSchemaIntact()
		{
			if (_schemaChanged)
			{
				throw new InvalidOperationException("Schema of current DataTable '" + CurrentTable.TableName + "' in DataTableReader has changed, DataTableReader is invalid.");
			}
		}

		private void ThrowInvalidCastException(Type sourceType, Type destType)
		{
			throw new InvalidCastException(string.Format("Unable to cast object of type '{0}' to type '{1}'.", sourceType, destType));
		}

		private bool MoveNext()
		{
			if (_index >= _tables.Length || _tableCleared)
			{
				return false;
			}
			do
			{
				_current++;
			}
			while (_current < CurrentTable.Rows.Count && CurrentRow.RowState == DataRowState.Deleted);
			_rowRef = ((_current >= CurrentTable.Rows.Count) ? null : CurrentRow);
			return _current < CurrentTable.Rows.Count;
		}

		public override bool NextResult()
		{
			if (_index + 1 >= _tables.Length)
			{
				UnsubscribeEvents();
				_index = _tables.Length;
				return false;
			}
			UnsubscribeEvents();
			_index++;
			_current = -1;
			_rowRef = null;
			_schemaTable = null;
			_tableCleared = false;
			SubscribeEvents();
			return true;
		}

		public override bool Read()
		{
			ValidateClosed();
			return MoveNext();
		}

		private void OnColumnChanged(object sender, PropertyChangedEventArgs args)
		{
			_schemaChanged = true;
		}

		private void OnColumnCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			_schemaChanged = true;
		}

		private void OnRowChanged(object src, DataRowChangeEventArgs args)
		{
			DataRowAction action = args.Action;
			DataRow row = args.Row;
			if (action == DataRowAction.Add)
			{
				if (_tableCleared && _current != -1)
				{
					return;
				}
				if (_current == -1 || (_current >= 0 && row.RowID > CurrentRow.RowID))
				{
					_tableCleared = false;
					return;
				}
				_current++;
				_rowRef = CurrentRow;
			}
			if (action == DataRowAction.Commit && row.RowState == DataRowState.Detached)
			{
				if (_rowRef == row)
				{
					_current--;
					_rowRef = ((_current < 0) ? null : CurrentRow);
				}
				if (_current >= CurrentTable.Rows.Count)
				{
					_current--;
					_rowRef = ((_current < 0) ? null : CurrentRow);
				}
				else if (_current > 0 && _rowRef == CurrentTable.Rows[_current - 1])
				{
					_current--;
					_rowRef = CurrentRow;
				}
			}
		}

		private void OnTableCleared(object src, DataTableClearEventArgs args)
		{
			_tableCleared = true;
		}
	}
}
