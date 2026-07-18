using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	public class DataRow
	{
		private DataTable _table;

		internal int _original = -1;

		internal int _current = -1;

		internal int _proposed = -1;

		private ArrayList _columnErrors;

		private string rowError;

		internal int xmlRowID;

		internal bool _nullConstraintViolation;

		private string _nullConstraintMessage;

		private bool _inChangingEvent;

		private int _rowId;

		internal bool _rowChanged;

		private XmlDataDocument.XmlDataElement mappedElement;

		internal bool _inExpressionEvaluation;

		private ArrayList ColumnErrors
		{
			get
			{
				if (_columnErrors == null)
				{
					_columnErrors = new ArrayList();
				}
				return _columnErrors;
			}
			set
			{
				_columnErrors = value;
			}
		}

		public bool HasErrors
		{
			get
			{
				if (RowError != string.Empty)
				{
					return true;
				}
				foreach (string columnError in ColumnErrors)
				{
					if (columnError != null && columnError != string.Empty)
					{
						return true;
					}
				}
				return false;
			}
		}

		public object this[string columnName]
		{
			get
			{
				return this[columnName, DataRowVersion.Default];
			}
			set
			{
				DataColumn dataColumn = _table.Columns[columnName];
				if (dataColumn == null)
				{
					throw new ArgumentException("The column '" + columnName + "' does not belong to the table : " + _table.TableName);
				}
				this[dataColumn.Ordinal] = value;
			}
		}

		public object this[DataColumn column]
		{
			get
			{
				return this[column, DataRowVersion.Default];
			}
			set
			{
				if (column == null)
				{
					throw new ArgumentNullException("column");
				}
				int num = _table.Columns.IndexOf(column);
				if (num == -1)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The column '{0}' does not belong to the table : {1}.", column.ColumnName, _table.TableName));
				}
				this[num] = value;
			}
		}

		public object this[int columnIndex]
		{
			get
			{
				return this[columnIndex, DataRowVersion.Default];
			}
			set
			{
				if (columnIndex < 0 || columnIndex > _table.Columns.Count)
				{
					throw new IndexOutOfRangeException();
				}
				if (RowState == DataRowState.Deleted)
				{
					throw new DeletedRowInaccessibleException();
				}
				DataColumn dataColumn = _table.Columns[columnIndex];
				_table.ChangingDataColumn(this, dataColumn, value);
				if (value == null && dataColumn.DataType.IsValueType)
				{
					throw new ArgumentException("Canot set column '" + dataColumn.ColumnName + "' to be null. Please use DBNull instead.");
				}
				_rowChanged = true;
				CheckValue(value, dataColumn);
				bool flag = Proposed >= 0;
				if (!flag)
				{
					BeginEdit();
				}
				dataColumn[Proposed] = value;
				_table.ChangedDataColumn(this, dataColumn, value);
				if (!flag)
				{
					EndEdit();
				}
			}
		}

		public object this[string columnName, DataRowVersion version]
		{
			get
			{
				DataColumn dataColumn = _table.Columns[columnName];
				if (dataColumn == null)
				{
					throw new ArgumentException("The column '" + columnName + "' does not belong to the table : " + _table.TableName);
				}
				return this[dataColumn.Ordinal, version];
			}
		}

		public object this[DataColumn column, DataRowVersion version]
		{
			get
			{
				if (column == null)
				{
					throw new ArgumentNullException("column");
				}
				if (column.Table != Table)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The column '{0}' does not belong to the table : {1}.", column.ColumnName, _table.TableName));
				}
				return this[column.Ordinal, version];
			}
		}

		public object this[int columnIndex, DataRowVersion version]
		{
			get
			{
				if (columnIndex < 0 || columnIndex > _table.Columns.Count)
				{
					throw new IndexOutOfRangeException();
				}
				DataColumn dataColumn = _table.Columns[columnIndex];
				int index = IndexFromVersion(version);
				if (dataColumn.Expression != string.Empty && _table.Rows.IndexOf(this) != -1)
				{
					object obj = dataColumn.CompiledExpression.Eval(this);
					if (obj != null && obj != DBNull.Value)
					{
						obj = Convert.ChangeType(obj, dataColumn.DataType);
					}
					dataColumn[index] = obj;
					return dataColumn[index];
				}
				return dataColumn[index];
			}
		}

		public object[] ItemArray
		{
			get
			{
				if (RowState == DataRowState.Deleted)
				{
					throw new DeletedRowInaccessibleException("Deleted row information cannot be accessed through the row.");
				}
				int index = Current;
				if (RowState == DataRowState.Detached)
				{
					if (Proposed < 0)
					{
						throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
					}
					index = Proposed;
				}
				object[] array = new object[_table.Columns.Count];
				foreach (DataColumn column in _table.Columns)
				{
					array[column.Ordinal] = column[index];
				}
				return array;
			}
			set
			{
				if (value.Length > _table.Columns.Count)
				{
					throw new ArgumentException();
				}
				if (RowState == DataRowState.Deleted)
				{
					throw new DeletedRowInaccessibleException();
				}
				BeginEdit();
				DataColumnChangeEventArgs e = new DataColumnChangeEventArgs();
				foreach (DataColumn column in _table.Columns)
				{
					int ordinal = column.Ordinal;
					object obj = ((ordinal >= value.Length) ? null : value[ordinal]);
					if (obj != null)
					{
						e.Initialize(this, column, obj);
						CheckValue(e.ProposedValue, column);
						_table.RaiseOnColumnChanging(e);
						column[Proposed] = e.ProposedValue;
						_table.RaiseOnColumnChanged(e);
					}
				}
				EndEdit();
			}
		}

		public DataRowState RowState
		{
			get
			{
				if (Original == -1 && Current == -1)
				{
					return DataRowState.Detached;
				}
				if (Original == Current)
				{
					return DataRowState.Unchanged;
				}
				if (Original == -1)
				{
					return DataRowState.Added;
				}
				if (Current == -1)
				{
					return DataRowState.Deleted;
				}
				return DataRowState.Modified;
			}
			internal set
			{
				if (value == DataRowState.Detached)
				{
					Original = -1;
					Current = -1;
				}
				if (value == DataRowState.Unchanged)
				{
					Original = Current;
				}
				if (value == DataRowState.Added)
				{
					Original = -1;
				}
				if (value == DataRowState.Deleted)
				{
					Current = -1;
				}
			}
		}

		public DataTable Table
		{
			get
			{
				return _table;
			}
			internal set
			{
				_table = value;
			}
		}

		internal int XmlRowID
		{
			get
			{
				return xmlRowID;
			}
			set
			{
				xmlRowID = value;
			}
		}

		internal int RowID
		{
			get
			{
				return _rowId;
			}
			set
			{
				_rowId = value;
			}
		}

		internal int Original
		{
			get
			{
				return _original;
			}
			set
			{
				if (Table != null)
				{
					Table.RecordCache[value] = this;
				}
				_original = value;
			}
		}

		internal int Current
		{
			get
			{
				return _current;
			}
			set
			{
				if (Table != null)
				{
					Table.RecordCache[value] = this;
				}
				_current = value;
			}
		}

		internal int Proposed
		{
			get
			{
				return _proposed;
			}
			set
			{
				if (Table != null)
				{
					Table.RecordCache[value] = this;
				}
				_proposed = value;
			}
		}

		public string RowError
		{
			get
			{
				return rowError;
			}
			set
			{
				rowError = value;
			}
		}

		internal XmlDataDocument.XmlDataElement DataElement
		{
			get
			{
				if (mappedElement != null || _table.DataSet == null || _table.DataSet._xmlDataDocument == null)
				{
					return mappedElement;
				}
				mappedElement = new XmlDataDocument.XmlDataElement(this, _table.Prefix, XmlHelper.Encode(_table.TableName), _table.Namespace, _table.DataSet._xmlDataDocument);
				return mappedElement;
			}
			set
			{
				mappedElement = value;
			}
		}

		protected internal DataRow(DataRowBuilder builder)
		{
			_table = builder.Table;
			_rowId = builder._rowId;
			rowError = string.Empty;
		}

		internal DataRow(DataTable table, int rowId)
		{
			_table = table;
			_rowId = rowId;
		}

		internal void SetValue(int column, object value, int version)
		{
			DataColumn dataColumn = Table.Columns[column];
			if (value == null && !dataColumn.AutoIncrement)
			{
				value = dataColumn.DefaultValue;
			}
			Table.ChangingDataColumn(this, dataColumn, value);
			CheckValue(value, dataColumn);
			if (!dataColumn.AutoIncrement)
			{
				dataColumn[version] = value;
			}
			else if (_proposed >= 0 && _proposed != version)
			{
				dataColumn[version] = dataColumn[_proposed];
			}
		}

		public void SetAdded()
		{
			if (RowState != DataRowState.Unchanged)
			{
				throw new InvalidOperationException("SetAdded and SetModified can only be called on DataRows with Unchanged DataRowState.");
			}
			Original = -1;
		}

		public void SetModified()
		{
			if (RowState != DataRowState.Unchanged)
			{
				throw new InvalidOperationException("SetAdded and SetModified can only be called on DataRows with Unchanged DataRowState.");
			}
			Current = _table.RecordCache.NewRecord();
			_table.RecordCache.CopyRecord(_table, Original, Current);
		}

		internal void AttachAt(int row_id, DataRowAction action)
		{
			_rowId = row_id;
			if (Proposed != -1)
			{
				if (Current >= 0)
				{
					Table.RecordCache.DisposeRecord(Current);
				}
				Current = Proposed;
				Proposed = -1;
			}
			if ((action & (DataRowAction.ChangeCurrentAndOriginal | DataRowAction.ChangeOriginal)) != DataRowAction.Nothing)
			{
				Original = Current;
			}
		}

		private void Detach()
		{
			Table.DeleteRowFromIndexes(this);
			_table.Rows.RemoveInternal(this);
			if (Proposed >= 0 && Proposed != Current && Proposed != Original)
			{
				_table.RecordCache.DisposeRecord(Proposed);
			}
			Proposed = -1;
			if (Current >= 0 && Current != Original)
			{
				_table.RecordCache.DisposeRecord(Current);
			}
			Current = -1;
			if (Original >= 0)
			{
				_table.RecordCache.DisposeRecord(Original);
			}
			Original = -1;
			_rowId = -1;
		}

		internal void ImportRecord(int record)
		{
			if (HasVersion(DataRowVersion.Proposed))
			{
				Table.RecordCache.DisposeRecord(Proposed);
			}
			Proposed = record;
			foreach (DataColumn autoIncrmentColumn in Table.Columns.AutoIncrmentColumns)
			{
				autoIncrmentColumn.UpdateAutoIncrementValue(autoIncrmentColumn.DataContainer.GetInt64(Proposed));
			}
			foreach (DataColumn column in Table.Columns)
			{
				CheckValue(this[column], column, false);
			}
		}

		private void CheckValue(object v, DataColumn col)
		{
			CheckValue(v, col, true);
		}

		private void CheckValue(object v, DataColumn col, bool doROCheck)
		{
			if (doROCheck && _rowId != -1 && col.ReadOnly)
			{
				throw new ReadOnlyException();
			}
			if ((v == null || v == DBNull.Value) && !col.AllowDBNull && !col.AutoIncrement && col.DefaultValue == DBNull.Value)
			{
				_nullConstraintViolation = true;
				if (Table._duringDataLoad || (Table.DataSet != null && !Table.DataSet.EnforceConstraints))
				{
					Table._nullConstraintViolationDuringDataLoad = true;
				}
				_nullConstraintMessage = "Column '" + col.ColumnName + "' does not allow nulls.";
			}
		}

		internal int IndexFromVersion(DataRowVersion version)
		{
			switch (version)
			{
			case DataRowVersion.Default:
				if (Proposed >= 0)
				{
					return Proposed;
				}
				if (Current >= 0)
				{
					return Current;
				}
				if (Original < 0)
				{
					throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
				}
				throw new DeletedRowInaccessibleException("Deleted row information cannot be accessed through the row.");
			case DataRowVersion.Proposed:
				return AssertValidVersionIndex(version, Proposed);
			case DataRowVersion.Current:
				return AssertValidVersionIndex(version, Current);
			case DataRowVersion.Original:
				return AssertValidVersionIndex(version, Original);
			default:
				throw new DataException("Version must be Original, Current, or Proposed.");
			}
		}

		private int AssertValidVersionIndex(DataRowVersion version, int index)
		{
			if (index >= 0)
			{
				return index;
			}
			throw new VersionNotFoundException(string.Format("There is no {0} data to accces.", version));
		}

		internal DataRowVersion VersionFromIndex(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException("Index must not be negative.");
			}
			if (index == Current)
			{
				return DataRowVersion.Current;
			}
			if (index == Original)
			{
				return DataRowVersion.Original;
			}
			if (index == Proposed)
			{
				return DataRowVersion.Proposed;
			}
			throw new ArgumentException(string.Format("The index {0} does not belong to this row.", index));
		}

		internal void SetOriginalValue(string columnName, object val)
		{
			DataColumn dataColumn = _table.Columns[columnName];
			_table.ChangingDataColumn(this, dataColumn, val);
			if (Original < 0 || Original == Current)
			{
				Original = Table.RecordCache.NewRecord();
			}
			CheckValue(val, dataColumn);
			dataColumn[Original] = val;
		}

		public void AcceptChanges()
		{
			EndEdit();
			_table.ChangingDataRow(this, DataRowAction.Commit);
			CheckChildRows(DataRowAction.Commit);
			switch (RowState)
			{
			case DataRowState.Added:
			case DataRowState.Modified:
				if (Original >= 0)
				{
					Table.RecordCache.DisposeRecord(Original);
				}
				Original = Current;
				break;
			case DataRowState.Deleted:
				Detach();
				break;
			case DataRowState.Detached:
				throw new RowNotInTableException("Cannot perform this operation on a row not in the table.");
			}
			_table.ChangedDataRow(this, DataRowAction.Commit);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void BeginEdit()
		{
			if (_inChangingEvent)
			{
				throw new InRowChangingEventException("Cannot call BeginEdit inside an OnRowChanging event.");
			}
			if (RowState == DataRowState.Deleted)
			{
				throw new DeletedRowInaccessibleException();
			}
			if (!HasVersion(DataRowVersion.Proposed))
			{
				Proposed = Table.RecordCache.NewRecord();
				int from_index = ((!HasVersion(DataRowVersion.Current)) ? Table.DefaultValuesRowIndex : Current);
				for (int i = 0; i < Table.Columns.Count; i++)
				{
					DataColumn dataColumn = Table.Columns[i];
					dataColumn.DataContainer.CopyValue(from_index, Proposed);
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void CancelEdit()
		{
			if (_inChangingEvent)
			{
				throw new InRowChangingEventException("Cannot call CancelEdit inside an OnRowChanging event.");
			}
			if (!HasVersion(DataRowVersion.Proposed))
			{
				return;
			}
			int proposed = Proposed;
			DataRowState rowState = RowState;
			Table.RecordCache.DisposeRecord(Proposed);
			Proposed = -1;
			foreach (Index index in Table.Indexes)
			{
				index.Update(this, proposed, DataRowVersion.Proposed, rowState);
			}
		}

		public void ClearErrors()
		{
			rowError = string.Empty;
			ColumnErrors.Clear();
		}

		public void Delete()
		{
			_table.DeletingDataRow(this, DataRowAction.Delete);
			switch (RowState)
			{
			case DataRowState.Added:
				CheckChildRows(DataRowAction.Delete);
				Detach();
				break;
			default:
				CheckChildRows(DataRowAction.Delete);
				break;
			case DataRowState.Detached:
			case DataRowState.Deleted:
				break;
			}
			if (Current >= 0)
			{
				int current = Current;
				DataRowState rowState = RowState;
				if (Current != Original)
				{
					_table.RecordCache.DisposeRecord(Current);
				}
				Current = -1;
				foreach (Index index in Table.Indexes)
				{
					index.Update(this, current, DataRowVersion.Current, rowState);
				}
			}
			_table.DeletedDataRow(this, DataRowAction.Delete);
		}

		private void CheckChildRows(DataRowAction action)
		{
			DataSet dataSet = _table.DataSet;
			if (dataSet == null || !dataSet.EnforceConstraints || _table.Constraints.Count == 0)
			{
				return;
			}
			foreach (DataTable table in dataSet.Tables)
			{
				foreach (Constraint constraint in table.Constraints)
				{
					ForeignKeyConstraint foreignKeyConstraint = constraint as ForeignKeyConstraint;
					if (foreignKeyConstraint == null || foreignKeyConstraint.RelatedTable != _table)
					{
						continue;
					}
					switch (action)
					{
					case DataRowAction.Delete:
						CheckChildRows(foreignKeyConstraint, action, foreignKeyConstraint.DeleteRule);
						break;
					case DataRowAction.Rollback:
					case DataRowAction.Commit:
						if (foreignKeyConstraint.AcceptRejectRule != AcceptRejectRule.None)
						{
							CheckChildRows(foreignKeyConstraint, action, Rule.Cascade);
						}
						break;
					default:
						CheckChildRows(foreignKeyConstraint, action, foreignKeyConstraint.UpdateRule);
						break;
					}
				}
			}
		}

		private void CheckChildRows(ForeignKeyConstraint fkc, DataRowAction action, Rule rule)
		{
			DataRow[] childRows = GetChildRows(fkc, DataRowVersion.Current);
			if (childRows == null)
			{
				return;
			}
			switch (rule)
			{
			case Rule.Cascade:
				switch (action)
				{
				case DataRowAction.Delete:
				{
					for (int l = 0; l < childRows.Length; l++)
					{
						if (childRows[l].RowState != DataRowState.Deleted)
						{
							childRows[l].Delete();
						}
					}
					break;
				}
				case DataRowAction.Change:
				{
					for (int m = 0; m < childRows.Length; m++)
					{
						for (int n = 0; n < fkc.Columns.Length; n++)
						{
							if (!fkc.RelatedColumns[n].DataContainer[Current].Equals(fkc.RelatedColumns[n].DataContainer[Proposed]))
							{
								childRows[m][fkc.Columns[n]] = this[fkc.RelatedColumns[n], DataRowVersion.Proposed];
							}
						}
					}
					break;
				}
				case DataRowAction.Rollback:
				{
					for (int k = 0; k < childRows.Length; k++)
					{
						if (childRows[k].RowState != DataRowState.Unchanged)
						{
							childRows[k].RejectChanges();
						}
					}
					break;
				}
				case DataRowAction.Change | DataRowAction.Delete:
					break;
				}
				break;
			case Rule.None:
			{
				for (int num3 = 0; num3 < childRows.Length; num3++)
				{
					if (childRows[num3].RowState != DataRowState.Deleted)
					{
						string text = "Cannot change this row because constraints are enforced on relation " + fkc.ConstraintName + ", and changing this row will strand child rows.";
						string text2 = "Cannot delete this row because constraints are enforced on relation " + fkc.ConstraintName + ", and deleting this row will strand child rows.";
						string s = ((action != DataRowAction.Delete) ? text : text2);
						throw new InvalidConstraintException(s);
					}
				}
				break;
			}
			case Rule.SetDefault:
			{
				if (childRows.Length <= 0)
				{
					break;
				}
				int defaultValuesRowIndex = childRows[0].Table.DefaultValuesRowIndex;
				DataRow[] array = childRows;
				foreach (DataRow dataRow2 in array)
				{
					if (dataRow2.RowState != DataRowState.Deleted)
					{
						int to_index = dataRow2.IndexFromVersion(DataRowVersion.Default);
						DataColumn[] columns = fkc.Columns;
						foreach (DataColumn dataColumn in columns)
						{
							dataColumn.DataContainer.CopyValue(defaultValuesRowIndex, to_index);
						}
					}
				}
				break;
			}
			case Rule.SetNull:
			{
				for (int i = 0; i < childRows.Length; i++)
				{
					DataRow dataRow = childRows[i];
					if (childRows[i].RowState != DataRowState.Deleted)
					{
						for (int j = 0; j < fkc.Columns.Length; j++)
						{
							dataRow.SetNull(fkc.Columns[j]);
						}
					}
				}
				break;
			}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void EndEdit()
		{
			if (_inChangingEvent)
			{
				throw new InRowChangingEventException("Cannot call EndEdit inside an OnRowChanging event.");
			}
			if (RowState == DataRowState.Detached || !HasVersion(DataRowVersion.Proposed))
			{
				return;
			}
			CheckReadOnlyStatus();
			_inChangingEvent = true;
			try
			{
				_table.ChangingDataRow(this, DataRowAction.Change);
			}
			finally
			{
				_inChangingEvent = false;
			}
			DataRowState rowState = RowState;
			int current = Current;
			Current = Proposed;
			Proposed = -1;
			foreach (Index index3 in Table.Indexes)
			{
				index3.Update(this, current, DataRowVersion.Current, rowState);
			}
			try
			{
				AssertConstraints();
				Proposed = Current;
				Current = current;
				CheckChildRows(DataRowAction.Change);
				Current = Proposed;
				Proposed = -1;
			}
			catch
			{
				int oldRecord = ((Proposed < 0) ? Current : Proposed);
				Current = current;
				foreach (Index index4 in Table.Indexes)
				{
					index4.Update(this, oldRecord, DataRowVersion.Current, RowState);
				}
				throw;
			}
			if (Original != current)
			{
				Table.RecordCache.DisposeRecord(current);
			}
			if (_rowChanged)
			{
				_table.ChangedDataRow(this, DataRowAction.Change);
				_rowChanged = false;
			}
		}

		public DataRow[] GetChildRows(DataRelation relation)
		{
			return GetChildRows(relation, DataRowVersion.Default);
		}

		public DataRow[] GetChildRows(string relationName)
		{
			return GetChildRows(Table.DataSet.Relations[relationName]);
		}

		public DataRow[] GetChildRows(DataRelation relation, DataRowVersion version)
		{
			if (relation == null)
			{
				return Table.NewRowArray(0);
			}
			if (Table == null)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			if (relation.DataSet != Table.DataSet)
			{
				throw new ArgumentException();
			}
			if (_table != relation.ParentTable)
			{
				throw new InvalidConstraintException(string.Concat("GetChildRow requires a row whose Table is ", relation.ParentTable, ", but the specified row's table is ", _table));
			}
			if (relation.ChildKeyConstraint != null)
			{
				return GetChildRows(relation.ChildKeyConstraint, version);
			}
			ArrayList arrayList = new ArrayList();
			DataColumn[] parentColumns = relation.ParentColumns;
			DataColumn[] childColumns = relation.ChildColumns;
			int num = parentColumns.Length;
			DataRow[] array = null;
			int from_index = IndexFromVersion(version);
			int num2 = relation.ChildTable.RecordCache.NewRecord();
			try
			{
				for (int i = 0; i < num; i++)
				{
					childColumns[i].DataContainer.CopyValue(parentColumns[i].DataContainer, from_index, num2);
				}
				Index index = relation.ChildTable.FindIndex(childColumns);
				if (index != null)
				{
					int[] array2 = index.FindAll(num2);
					array = relation.ChildTable.NewRowArray(array2.Length);
					for (int j = 0; j < array2.Length; j++)
					{
						array[j] = relation.ChildTable.RecordCache[array2[j]];
					}
				}
				else
				{
					foreach (DataRow row in relation.ChildTable.Rows)
					{
						bool flag = false;
						if (row.HasVersion(DataRowVersion.Default))
						{
							flag = true;
							int index2 = row.IndexFromVersion(DataRowVersion.Default);
							for (int k = 0; k < num; k++)
							{
								if (childColumns[k].DataContainer.CompareValues(index2, num2) != 0)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							arrayList.Add(row);
						}
					}
					array = relation.ChildTable.NewRowArray(arrayList.Count);
					arrayList.CopyTo(array, 0);
				}
			}
			finally
			{
				relation.ChildTable.RecordCache.DisposeRecord(num2);
			}
			return array;
		}

		public DataRow[] GetChildRows(string relationName, DataRowVersion version)
		{
			return GetChildRows(Table.DataSet.Relations[relationName], version);
		}

		private DataRow[] GetChildRows(ForeignKeyConstraint fkc, DataRowVersion version)
		{
			ArrayList arrayList = new ArrayList();
			DataColumn[] relatedColumns = fkc.RelatedColumns;
			DataColumn[] columns = fkc.Columns;
			int num = relatedColumns.Length;
			Index index = fkc.Index;
			int from_index = IndexFromVersion(version);
			int num2 = fkc.Table.RecordCache.NewRecord();
			for (int i = 0; i < num; i++)
			{
				columns[i].DataContainer.CopyValue(relatedColumns[i].DataContainer, from_index, num2);
			}
			try
			{
				if (index != null)
				{
					int[] array = index.FindAll(num2);
					for (int j = 0; j < array.Length; j++)
					{
						arrayList.Add(columns[j].Table.RecordCache[array[j]]);
					}
				}
				else
				{
					foreach (DataRow row in fkc.Table.Rows)
					{
						bool flag = false;
						if (row.HasVersion(DataRowVersion.Default))
						{
							flag = true;
							int index2 = row.IndexFromVersion(DataRowVersion.Default);
							for (int k = 0; k < num; k++)
							{
								if (columns[k].DataContainer.CompareValues(index2, num2) != 0)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							arrayList.Add(row);
						}
					}
				}
			}
			finally
			{
				fkc.Table.RecordCache.DisposeRecord(num2);
			}
			DataRow[] array2 = fkc.Table.NewRowArray(arrayList.Count);
			arrayList.CopyTo(array2, 0);
			return array2;
		}

		public string GetColumnError(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			int num = _table.Columns.IndexOf(column);
			if (num < 0)
			{
				throw new ArgumentException(string.Format("Column '{0}' does not belong to table {1}.", column.ColumnName, Table.TableName));
			}
			return GetColumnError(num);
		}

		public string GetColumnError(int columnIndex)
		{
			if (columnIndex < 0 || columnIndex >= Table.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			string text = null;
			if (columnIndex < ColumnErrors.Count)
			{
				text = (string)ColumnErrors[columnIndex];
			}
			return (text == null) ? string.Empty : text;
		}

		public string GetColumnError(string columnName)
		{
			return GetColumnError(_table.Columns.IndexOf(columnName));
		}

		public DataColumn[] GetColumnsInError()
		{
			ArrayList arrayList = new ArrayList();
			int num = 0;
			foreach (string columnError in ColumnErrors)
			{
				if (columnError != null && columnError != string.Empty)
				{
					arrayList.Add(_table.Columns[num]);
				}
				num++;
			}
			return (DataColumn[])arrayList.ToArray(typeof(DataColumn));
		}

		public DataRow GetParentRow(DataRelation relation)
		{
			return GetParentRow(relation, DataRowVersion.Default);
		}

		public DataRow GetParentRow(string relationName)
		{
			return GetParentRow(relationName, DataRowVersion.Default);
		}

		public DataRow GetParentRow(DataRelation relation, DataRowVersion version)
		{
			DataRow[] parentRows = GetParentRows(relation, version);
			if (parentRows.Length == 0)
			{
				return null;
			}
			return parentRows[0];
		}

		public DataRow GetParentRow(string relationName, DataRowVersion version)
		{
			return GetParentRow(Table.DataSet.Relations[relationName], version);
		}

		public DataRow[] GetParentRows(DataRelation relation)
		{
			return GetParentRows(relation, DataRowVersion.Default);
		}

		public DataRow[] GetParentRows(string relationName)
		{
			return GetParentRows(relationName, DataRowVersion.Default);
		}

		public DataRow[] GetParentRows(DataRelation relation, DataRowVersion version)
		{
			if (relation == null)
			{
				return Table.NewRowArray(0);
			}
			if (Table == null)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			if (relation.DataSet != Table.DataSet)
			{
				throw new ArgumentException();
			}
			if (_table != relation.ChildTable)
			{
				throw new InvalidConstraintException(string.Concat("GetParentRows requires a row whose Table is ", relation.ChildTable, ", but the specified row's table is ", _table));
			}
			ArrayList arrayList = new ArrayList();
			DataColumn[] parentColumns = relation.ParentColumns;
			DataColumn[] childColumns = relation.ChildColumns;
			int num = parentColumns.Length;
			int from_index = IndexFromVersion(version);
			int num2 = relation.ParentTable.RecordCache.NewRecord();
			for (int i = 0; i < num; i++)
			{
				parentColumns[i].DataContainer.CopyValue(childColumns[i].DataContainer, from_index, num2);
			}
			try
			{
				Index index = relation.ParentTable.FindIndex(parentColumns);
				if (index != null)
				{
					int[] array = index.FindAll(num2);
					for (int j = 0; j < array.Length; j++)
					{
						arrayList.Add(parentColumns[j].Table.RecordCache[array[j]]);
					}
				}
				else
				{
					foreach (DataRow row in relation.ParentTable.Rows)
					{
						bool flag = false;
						if (row.HasVersion(DataRowVersion.Default))
						{
							flag = true;
							int index2 = row.IndexFromVersion(DataRowVersion.Default);
							for (int k = 0; k < num; k++)
							{
								if (parentColumns[k].DataContainer.CompareValues(index2, num2) != 0)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							arrayList.Add(row);
						}
					}
				}
			}
			finally
			{
				relation.ParentTable.RecordCache.DisposeRecord(num2);
			}
			DataRow[] array2 = relation.ParentTable.NewRowArray(arrayList.Count);
			arrayList.CopyTo(array2, 0);
			return array2;
		}

		public DataRow[] GetParentRows(string relationName, DataRowVersion version)
		{
			return GetParentRows(Table.DataSet.Relations[relationName], version);
		}

		public bool HasVersion(DataRowVersion version)
		{
			switch (version)
			{
			case DataRowVersion.Default:
				return Proposed >= 0 || Current >= 0;
			case DataRowVersion.Proposed:
				return Proposed >= 0;
			case DataRowVersion.Current:
				return Current >= 0;
			case DataRowVersion.Original:
				return Original >= 0;
			default:
				return IndexFromVersion(version) >= 0;
			}
		}

		public bool IsNull(DataColumn column)
		{
			return IsNull(column, DataRowVersion.Default);
		}

		public bool IsNull(int columnIndex)
		{
			return IsNull(Table.Columns[columnIndex]);
		}

		public bool IsNull(string columnName)
		{
			return IsNull(Table.Columns[columnName]);
		}

		public bool IsNull(DataColumn column, DataRowVersion version)
		{
			object obj = this[column, version];
			return column.DataContainer.IsNull(IndexFromVersion(version));
		}

		internal bool IsNullColumns(DataColumn[] columns)
		{
			int i;
			for (i = 0; i < columns.Length && IsNull(columns[i]); i++)
			{
			}
			return i == columns.Length;
		}

		public void RejectChanges()
		{
			if (RowState == DataRowState.Detached)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			Table.ChangingDataRow(this, DataRowAction.Rollback);
			switch (RowState)
			{
			case DataRowState.Added:
				Detach();
				break;
			case DataRowState.Modified:
			{
				int current = Current;
				Table.RecordCache.DisposeRecord(Current);
				CheckChildRows(DataRowAction.Rollback);
				Current = Original;
				foreach (Index index in Table.Indexes)
				{
					index.Update(this, current, DataRowVersion.Current, DataRowState.Modified);
				}
				break;
			}
			case DataRowState.Deleted:
				CheckChildRows(DataRowAction.Rollback);
				Current = Original;
				Validate();
				break;
			}
			Table.ChangedDataRow(this, DataRowAction.Rollback);
		}

		public void SetColumnError(DataColumn column, string error)
		{
			SetColumnError(_table.Columns.IndexOf(column), error);
		}

		public void SetColumnError(int columnIndex, string error)
		{
			if (columnIndex < 0 || columnIndex >= Table.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			while (columnIndex >= ColumnErrors.Count)
			{
				ColumnErrors.Add(null);
			}
			ColumnErrors[columnIndex] = error;
		}

		public void SetColumnError(string columnName, string error)
		{
			SetColumnError(_table.Columns.IndexOf(columnName), error);
		}

		protected void SetNull(DataColumn column)
		{
			this[column] = DBNull.Value;
		}

		public void SetParentRow(DataRow parentRow)
		{
			SetParentRow(parentRow, null);
		}

		public void SetParentRow(DataRow parentRow, DataRelation relation)
		{
			if (_table == null || parentRow.Table == null)
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			if (parentRow != null && _table.DataSet != parentRow.Table.DataSet)
			{
				throw new ArgumentException();
			}
			if (RowState == DataRowState.Detached && !HasVersion(DataRowVersion.Default))
			{
				throw new RowNotInTableException("This row has been removed from a table and does not have any data.  BeginEdit() will allow creation of new data in this row.");
			}
			BeginEdit();
			IEnumerable enumerable = ((relation != null) ? ((ICollection)new DataRelation[1] { relation }) : ((ICollection)_table.ParentRelations));
			foreach (DataRelation item in enumerable)
			{
				DataColumn[] childColumns = item.ChildColumns;
				DataColumn[] parentColumns = item.ParentColumns;
				for (int i = 0; i < parentColumns.Length; i++)
				{
					if (parentRow == null)
					{
						childColumns[i].DataContainer[Proposed] = DBNull.Value;
						continue;
					}
					int from_index = parentRow.IndexFromVersion(DataRowVersion.Default);
					childColumns[i].DataContainer.CopyValue(parentColumns[i].DataContainer, from_index, Proposed);
				}
			}
			EndEdit();
		}

		internal void CopyValuesToRow(DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			if (row == this)
			{
				throw new ArgumentException("'row' is the same as this object");
			}
			if (HasVersion(DataRowVersion.Original))
			{
				if (row.Original < 0)
				{
					row.Original = row.Table.RecordCache.NewRecord();
				}
				else if (row.Original == row.Current)
				{
					row.Original = row.Table.RecordCache.NewRecord();
					row.Table.RecordCache.CopyRecord(row.Table, row.Current, row.Original);
				}
			}
			else if (row.Original > 0)
			{
				if (row.Original != row.Current)
				{
					row.Table.RecordCache.DisposeRecord(row.Original);
				}
				row.Original = -1;
			}
			if (HasVersion(DataRowVersion.Current))
			{
				if (Current == Original)
				{
					if (row.Current >= 0)
					{
						row.Table.RecordCache.DisposeRecord(row.Current);
					}
					row.Current = row.Original;
				}
				else if (row.Current < 0)
				{
					row.Current = row.Table.RecordCache.NewRecord();
				}
			}
			else if (row.Current > 0)
			{
				row.Table.RecordCache.DisposeRecord(row.Current);
				row.Current = -1;
			}
			if (HasVersion(DataRowVersion.Proposed))
			{
				if (row.Proposed < 0)
				{
					row.Proposed = row.Table.RecordCache.NewRecord();
				}
			}
			else if (row.Proposed > 0)
			{
				row.Table.RecordCache.DisposeRecord(row.Proposed);
				row.Proposed = -1;
			}
			foreach (DataColumn column in Table.Columns)
			{
				DataColumn dataColumn2 = row.Table.Columns[column.ColumnName];
				if (dataColumn2 != null)
				{
					if (HasVersion(DataRowVersion.Original))
					{
						object obj = column[Original];
						row.CheckValue(obj, dataColumn2);
						dataColumn2[row.Original] = obj;
					}
					if (HasVersion(DataRowVersion.Current) && Current != Original)
					{
						object obj2 = column[Current];
						row.CheckValue(obj2, dataColumn2);
						dataColumn2[row.Current] = obj2;
					}
					if (HasVersion(DataRowVersion.Proposed))
					{
						object obj3 = column[row.Proposed];
						row.CheckValue(obj3, dataColumn2);
						dataColumn2[row.Proposed] = obj3;
					}
				}
			}
			if (HasErrors)
			{
				CopyErrors(row);
			}
		}

		internal void MergeValuesToRow(DataRow row, bool preserveChanges)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}
			if (row == this)
			{
				throw new ArgumentException("'row' is the same as this object");
			}
			if (HasVersion(DataRowVersion.Original))
			{
				if (row.Original < 0)
				{
					row.Original = row.Table.RecordCache.NewRecord();
				}
				else if (row.Original == row.Current && (Original != Current || preserveChanges))
				{
					row.Original = row.Table.RecordCache.NewRecord();
					row.Table.RecordCache.CopyRecord(row.Table, row.Current, row.Original);
				}
			}
			else if (row.Original == row.Current)
			{
				row.Original = row.Table.RecordCache.NewRecord();
				row.Table.RecordCache.CopyRecord(row.Table, row.Current, row.Original);
			}
			if (HasVersion(DataRowVersion.Current))
			{
				if (!preserveChanges && row.Current < 0)
				{
					row.Current = row.Table.RecordCache.NewRecord();
				}
			}
			else if (row.Current > 0 && !preserveChanges)
			{
				row.Table.RecordCache.DisposeRecord(row.Current);
				row.Current = -1;
			}
			foreach (DataColumn column in Table.Columns)
			{
				DataColumn dataColumn2 = row.Table.Columns[column.ColumnName];
				if (dataColumn2 != null)
				{
					if (HasVersion(DataRowVersion.Original))
					{
						object obj = column[Original];
						row.CheckValue(obj, dataColumn2);
						dataColumn2[row.Original] = obj;
					}
					if (HasVersion(DataRowVersion.Current) && !preserveChanges)
					{
						object obj2 = column[Current];
						row.CheckValue(obj2, dataColumn2);
						dataColumn2[row.Current] = obj2;
					}
				}
			}
			if (HasErrors)
			{
				CopyErrors(row);
			}
		}

		internal void CopyErrors(DataRow row)
		{
			row.RowError = RowError;
			DataColumn[] columnsInError = GetColumnsInError();
			DataColumn[] array = columnsInError;
			foreach (DataColumn dataColumn in array)
			{
				DataColumn column = row.Table.Columns[dataColumn.ColumnName];
				row.SetColumnError(column, GetColumnError(dataColumn));
			}
		}

		internal bool IsRowChanged(DataRowState rowState)
		{
			if ((RowState & rowState) != 0)
			{
				return true;
			}
			DataRowVersion version = ((rowState != DataRowState.Deleted) ? DataRowVersion.Current : DataRowVersion.Original);
			int count = Table.ChildRelations.Count;
			for (int i = 0; i < count; i++)
			{
				DataRelation relation = Table.ChildRelations[i];
				DataRow[] childRows = GetChildRows(relation, version);
				for (int j = 0; j < childRows.Length; j++)
				{
					if (childRows[j].IsRowChanged(rowState))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal void Validate()
		{
			Table.AddRowToIndexes(this);
			AssertConstraints();
		}

		private void AssertConstraints()
		{
			if (Table == null || Table._duringDataLoad || (Table.DataSet != null && !Table.DataSet.EnforceConstraints))
			{
				return;
			}
			for (int i = 0; i < Table.Columns.Count; i++)
			{
				DataColumn dataColumn = Table.Columns[i];
				if (!dataColumn.AllowDBNull && IsNull(dataColumn))
				{
					throw new NoNullAllowedException(_nullConstraintMessage);
				}
			}
			foreach (Constraint constraint in Table.Constraints)
			{
				try
				{
					constraint.AssertConstraint(this);
				}
				catch (Exception ex)
				{
					Table.DeleteRowFromIndexes(this);
					throw ex;
				}
			}
		}

		internal void CheckNullConstraints()
		{
			if (!_nullConstraintViolation)
			{
				return;
			}
			if (HasVersion(DataRowVersion.Proposed))
			{
				foreach (DataColumn column in Table.Columns)
				{
					if (IsNull(column) && !column.AllowDBNull)
					{
						throw new NoNullAllowedException(_nullConstraintMessage);
					}
				}
			}
			_nullConstraintViolation = false;
		}

		internal void CheckReadOnlyStatus()
		{
			int index = IndexFromVersion(DataRowVersion.Default);
			foreach (DataColumn column in Table.Columns)
			{
				if (column.DataContainer.CompareValues(index, Proposed) != 0 && column.ReadOnly)
				{
					throw new ReadOnlyException();
				}
			}
		}

		internal void Load(object[] values, LoadOption loadOption)
		{
			int num = -1;
			if (loadOption == LoadOption.OverwriteChanges || (loadOption == LoadOption.PreserveChanges && RowState == DataRowState.Unchanged))
			{
				Table.ChangingDataRow(this, DataRowAction.ChangeCurrentAndOriginal);
				num = Table.CreateRecord(values);
				Table.DeleteRowFromIndexes(this);
				if (HasVersion(DataRowVersion.Original) && Current != Original)
				{
					Table.RecordCache.DisposeRecord(Original);
				}
				Original = num;
				if (HasVersion(DataRowVersion.Current))
				{
					Table.RecordCache.DisposeRecord(Current);
				}
				Current = num;
				Table.AddRowToIndexes(this);
				Table.ChangedDataRow(this, DataRowAction.ChangeCurrentAndOriginal);
			}
			else if (loadOption == LoadOption.PreserveChanges)
			{
				Table.ChangingDataRow(this, DataRowAction.ChangeOriginal);
				num = Table.CreateRecord(values);
				if (HasVersion(DataRowVersion.Original) && Current != Original)
				{
					Table.RecordCache.DisposeRecord(Original);
				}
				Original = num;
				Table.ChangedDataRow(this, DataRowAction.ChangeOriginal);
			}
			else
			{
				if (RowState == DataRowState.Deleted)
				{
					return;
				}
				int x = ((!HasVersion(DataRowVersion.Proposed)) ? Current : Proposed);
				num = Table.CreateRecord(values);
				if (RowState == DataRowState.Added || Table.CompareRecords(x, num) != 0)
				{
					Table.ChangingDataRow(this, DataRowAction.Change);
					Table.DeleteRowFromIndexes(this);
					if (HasVersion(DataRowVersion.Proposed))
					{
						Table.RecordCache.DisposeRecord(Proposed);
						Proposed = -1;
					}
					if (Original != Current)
					{
						Table.RecordCache.DisposeRecord(Current);
					}
					Current = num;
					Table.AddRowToIndexes(this);
					Table.ChangedDataRow(this, DataRowAction.Change);
				}
				else
				{
					Table.ChangingDataRow(this, DataRowAction.Nothing);
					Table.RecordCache.DisposeRecord(num);
					Table.ChangedDataRow(this, DataRowAction.Nothing);
				}
			}
		}
	}
}
