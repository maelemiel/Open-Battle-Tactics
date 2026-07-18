using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data
{
	public sealed class DataRowCollection : InternalDataCollectionBase
	{
		private DataTable table;

		public DataRow this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new IndexOutOfRangeException("There is no row at position " + index + ".");
				}
				return (DataRow)List[index];
			}
		}

		public override int Count
		{
			get
			{
				return List.Count;
			}
		}

		internal event ListChangedEventHandler ListChanged;

		internal DataRowCollection(DataTable table)
		{
			this.table = table;
		}

		public void Add(DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row", "'row' argument cannot be null.");
			}
			if (row.Table != table)
			{
				throw new ArgumentException("This row already belongs to another table.");
			}
			if (row.RowID != -1)
			{
				throw new ArgumentException("This row already belongs to this table.");
			}
			row.BeginEdit();
			row.Validate();
			AddInternal(row);
		}

		public int IndexOf(DataRow row)
		{
			if (row == null || row.Table != table)
			{
				return -1;
			}
			int rowID = row.RowID;
			return (rowID < 0 || rowID >= List.Count || row != List[rowID]) ? (-1) : rowID;
		}

		internal void AddInternal(DataRow row)
		{
			AddInternal(row, DataRowAction.Add);
		}

		internal void AddInternal(DataRow row, DataRowAction action)
		{
			row.Table.ChangingDataRow(row, action);
			List.Add(row);
			row.AttachAt(List.Count - 1, action);
			row.Table.ChangedDataRow(row, action);
			if (row._rowChanged)
			{
				row._rowChanged = false;
			}
		}

		public DataRow Add(params object[] values)
		{
			if (values == null)
			{
				throw new NullReferenceException();
			}
			DataRow dataRow = table.NewNotInitializedRow();
			int record = table.CreateRecord(values);
			dataRow.ImportRecord(record);
			dataRow.Validate();
			AddInternal(dataRow);
			return dataRow;
		}

		public void Clear()
		{
			if (table.DataSet != null && table.DataSet.EnforceConstraints)
			{
				foreach (Constraint constraint in table.Constraints)
				{
					UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
					if (uniqueConstraint == null || uniqueConstraint.ChildConstraint == null || uniqueConstraint.ChildConstraint.Table.Rows.Count == 0)
					{
						continue;
					}
					string s = string.Format("Cannot clear table Parent because ForeignKeyConstraint {0} enforces Child.", uniqueConstraint.ConstraintName);
					throw new InvalidConstraintException(s);
				}
			}
			table.DataTableClearing();
			List.Clear();
			table.ResetIndexes();
			table.DataTableCleared();
			OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}

		public bool Contains(object key)
		{
			return Find(key) != null;
		}

		public bool Contains(object[] keys)
		{
			return Find(keys) != null;
		}

		public DataRow Find(object key)
		{
			return Find(new object[1] { key }, DataViewRowState.CurrentRows);
		}

		public DataRow Find(object[] keys)
		{
			return Find(keys, DataViewRowState.CurrentRows);
		}

		internal DataRow Find(object[] keys, DataViewRowState rowStateFilter)
		{
			if (table.PrimaryKey.Length == 0)
			{
				throw new MissingPrimaryKeyException("Table doesn't have a primary key.");
			}
			if (keys == null)
			{
				throw new ArgumentException("Expecting " + table.PrimaryKey.Length + " value(s) for the key being indexed, but received 0 value(s).");
			}
			Index index = table.GetIndex(table.PrimaryKey, null, rowStateFilter, null, false);
			int num = index.Find(keys);
			if (num != -1 || !table._duringDataLoad)
			{
				return (num == -1) ? null : table.RecordCache[num];
			}
			num = table.RecordCache.NewRecord();
			try
			{
				for (int i = 0; i < table.PrimaryKey.Length; i++)
				{
					table.PrimaryKey[i].DataContainer[num] = keys[i];
				}
				IEnumerator enumerator = GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DataRow dataRow = (DataRow)enumerator.Current;
						int record = Key.GetRecord(dataRow, rowStateFilter);
						if (record == -1)
						{
							continue;
						}
						bool flag = true;
						for (int j = 0; j < table.PrimaryKey.Length; j++)
						{
							if (table.PrimaryKey[j].CompareValues(record, num) != 0)
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}
						return dataRow;
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				return null;
			}
			finally
			{
				table.RecordCache.DisposeRecord(num);
			}
		}

		public void InsertAt(DataRow row, int pos)
		{
			if (pos < 0)
			{
				throw new IndexOutOfRangeException("The row insert position " + pos + " is invalid.");
			}
			if (row == null)
			{
				throw new ArgumentNullException("row", "'row' argument cannot be null.");
			}
			if (row.Table != table)
			{
				throw new ArgumentException("This row already belongs to another table.");
			}
			if (row.RowID != -1)
			{
				throw new ArgumentException("This row already belongs to this table.");
			}
			row.Validate();
			row.Table.ChangingDataRow(row, DataRowAction.Add);
			if (pos >= List.Count)
			{
				pos = List.Count;
				List.Add(row);
			}
			else
			{
				List.Insert(pos, row);
				for (int i = pos + 1; i < List.Count; i++)
				{
					((DataRow)List[i]).RowID = i;
				}
			}
			row.AttachAt(pos, DataRowAction.Add);
			row.Table.ChangedDataRow(row, DataRowAction.Add);
		}

		internal void RemoveInternal(DataRow row)
		{
			if (row == null)
			{
				throw new IndexOutOfRangeException("The given datarow is not in the current DataRowCollection.");
			}
			int i = List.IndexOf(row);
			if (i < 0)
			{
				throw new IndexOutOfRangeException("The given datarow is not in the current DataRowCollection.");
			}
			List.RemoveAt(i);
			for (; i < List.Count; i++)
			{
				((DataRow)List[i]).RowID = i;
			}
		}

		public void Remove(DataRow row)
		{
			if (IndexOf(row) < 0)
			{
				throw new IndexOutOfRangeException("The given datarow is not in the current DataRowCollection.");
			}
			DataRowState rowState = row.RowState;
			if (rowState != DataRowState.Deleted && rowState != DataRowState.Detached)
			{
				row.Delete();
				if (row.RowState != DataRowState.Detached)
				{
					row.AcceptChanges();
				}
			}
		}

		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

		internal void OnListChanged(object sender, ListChangedEventArgs args)
		{
			if (this.ListChanged != null)
			{
				this.ListChanged(sender, args);
			}
		}

		public void CopyTo(DataRow[] array, int index)
		{
			CopyTo((Array)array, index);
		}

		public override void CopyTo(Array array, int index)
		{
			base.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return base.GetEnumerator();
		}
	}
}
