using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	[DefaultEvent("CollectionChanged")]
	[Editor("Microsoft.VSDesigner.Data.Design.ColumnsCollectionEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class DataColumnCollection : InternalDataCollectionBase
	{
		private Hashtable columnNameCount = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private Hashtable columnFromName = new Hashtable();

		private ArrayList autoIncrement = new ArrayList();

		private int defaultColumnIndex = 1;

		private DataTable parentTable;

		private DataColumn[] _mostRecentColumns;

		private static readonly string ColumnPrefix = "Column";

		private static readonly string[] TenColumns = new string[10] { "Column0", "Column1", "Column2", "Column3", "Column4", "Column5", "Column6", "Column7", "Column8", "Column9" };

		public DataColumn this[int index]
		{
			get
			{
				if (index < 0 || index >= base.List.Count)
				{
					throw new IndexOutOfRangeException("Cannot find column " + index + ".");
				}
				return (DataColumn)base.List[index];
			}
		}

		public DataColumn this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				DataColumn dataColumn = columnFromName[name] as DataColumn;
				if (dataColumn != null)
				{
					return dataColumn;
				}
				int num = IndexOf(name, true);
				return (num != -1) ? ((DataColumn)base.List[num]) : null;
			}
		}

		protected override ArrayList List
		{
			get
			{
				return base.List;
			}
		}

		internal ArrayList AutoIncrmentColumns
		{
			get
			{
				return autoIncrement;
			}
		}

		[ResDescription("Occurs whenever this collection's membership changes.")]
		public event CollectionChangeEventHandler CollectionChanged;

		internal event CollectionChangeEventHandler CollectionMetaDataChanged;

		internal DataColumnCollection(DataTable table)
		{
			parentTable = table;
		}

		public DataColumn Add()
		{
			DataColumn dataColumn = new DataColumn(null);
			Add(dataColumn);
			return dataColumn;
		}

		public void CopyTo(DataColumn[] array, int index)
		{
			CopyTo((Array)array, index);
		}

		internal void RegisterName(string name, DataColumn column)
		{
			try
			{
				columnFromName.Add(name, column);
			}
			catch (ArgumentException)
			{
				throw new DuplicateNameException("A DataColumn named '" + name + "' already belongs to this DataTable.");
			}
			Doublet doublet = (Doublet)columnNameCount[name];
			if (doublet != null)
			{
				doublet.count++;
				doublet.columnNames.Add(name);
			}
			else
			{
				doublet = new Doublet(1, name);
				columnNameCount[name] = doublet;
			}
			if (name.Length > ColumnPrefix.Length && name.StartsWith(ColumnPrefix, StringComparison.Ordinal) && name == MakeName(defaultColumnIndex + 1))
			{
				do
				{
					defaultColumnIndex++;
				}
				while (Contains(MakeName(defaultColumnIndex + 1)));
			}
		}

		internal void UnregisterName(string name)
		{
			if (columnFromName.Contains(name))
			{
				columnFromName.Remove(name);
			}
			Doublet doublet = (Doublet)columnNameCount[name];
			if (doublet != null)
			{
				doublet.count--;
				doublet.columnNames.Remove(name);
				if (doublet.count == 0)
				{
					columnNameCount.Remove(name);
				}
			}
			if (name.StartsWith(ColumnPrefix) && name == MakeName(defaultColumnIndex - 1))
			{
				do
				{
					defaultColumnIndex--;
				}
				while (!Contains(MakeName(defaultColumnIndex - 1)) && defaultColumnIndex > 1);
			}
		}

		private string GetNextDefaultColumnName()
		{
			string text = MakeName(defaultColumnIndex);
			int num = defaultColumnIndex + 1;
			while (Contains(text))
			{
				text = MakeName(num);
				defaultColumnIndex++;
				num++;
			}
			defaultColumnIndex++;
			return text;
		}

		private static string MakeName(int index)
		{
			if (index < 10)
			{
				return TenColumns[index];
			}
			return ColumnPrefix + index;
		}

		public void Add(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column", "'column' argument cannot be null.");
			}
			if (column.ColumnName.Length == 0)
			{
				column.ColumnName = GetNextDefaultColumnName();
			}
			if (column.Table != null)
			{
				throw new ArgumentException("Column '" + column.ColumnName + "' already belongs to this or another DataTable.");
			}
			column.SetTable(parentTable);
			RegisterName(column.ColumnName, column);
			int columnIndex = (column.Ordinal = base.List.Add(column));
			if (column.CompiledExpression != null)
			{
				if (parentTable.Rows.Count == 0)
				{
					column.CompiledExpression.Eval(parentTable.NewRow());
				}
				else
				{
					column.CompiledExpression.Eval(parentTable.Rows[0]);
				}
			}
			if (parentTable.Rows.Count > 0)
			{
				column.DataContainer.Capacity = parentTable.RecordCache.CurrentCapacity;
			}
			if (column.AutoIncrement)
			{
				DataRowCollection rows = column.Table.Rows;
				for (int i = 0; i < rows.Count; i++)
				{
					rows[i][columnIndex] = column.AutoIncrementValue();
				}
			}
			if (column.AutoIncrement)
			{
				autoIncrement.Add(column);
			}
			column.PropertyChanged += ColumnPropertyChanged;
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
		}

		public DataColumn Add(string columnName)
		{
			DataColumn dataColumn = new DataColumn(columnName);
			Add(dataColumn);
			return dataColumn;
		}

		public DataColumn Add(string columnName, Type type)
		{
			if (columnName == null || columnName == string.Empty)
			{
				columnName = GetNextDefaultColumnName();
			}
			DataColumn dataColumn = new DataColumn(columnName, type);
			Add(dataColumn);
			return dataColumn;
		}

		public DataColumn Add(string columnName, Type type, string expression)
		{
			if (columnName == null || columnName == string.Empty)
			{
				columnName = GetNextDefaultColumnName();
			}
			DataColumn dataColumn = new DataColumn(columnName, type, expression);
			Add(dataColumn);
			return dataColumn;
		}

		public void AddRange(DataColumn[] columns)
		{
			if (parentTable.InitInProgress)
			{
				_mostRecentColumns = columns;
			}
			else
			{
				if (columns == null)
				{
					return;
				}
				foreach (DataColumn dataColumn in columns)
				{
					if (dataColumn != null)
					{
						Add(dataColumn);
					}
				}
			}
		}

		private string GetColumnDependency(DataColumn column)
		{
			foreach (DataRelation parentRelation in parentTable.ParentRelations)
			{
				if (Array.IndexOf(parentRelation.ChildColumns, column) != -1)
				{
					return string.Format(" child key for relationship {0}.", parentRelation.RelationName);
				}
			}
			foreach (DataRelation childRelation in parentTable.ChildRelations)
			{
				if (Array.IndexOf(childRelation.ParentColumns, column) != -1)
				{
					return string.Format(" parent key for relationship {0}.", childRelation.RelationName);
				}
			}
			foreach (Constraint constraint3 in parentTable.Constraints)
			{
				if (constraint3.IsColumnContained(column))
				{
					return string.Format(" constraint {0} on the table {1}.", constraint3.ConstraintName, parentTable);
				}
			}
			if (parentTable.DataSet != null)
			{
				foreach (DataTable table in parentTable.DataSet.Tables)
				{
					foreach (Constraint constraint4 in table.Constraints)
					{
						if (constraint4 is ForeignKeyConstraint && constraint4.IsColumnContained(column))
						{
							return string.Format(" constraint {0} on the table {1}.", constraint4.ConstraintName, table.TableName);
						}
					}
				}
			}
			IEnumerator enumerator6 = GetEnumerator();
			try
			{
				while (enumerator6.MoveNext())
				{
					DataColumn dataColumn = (DataColumn)enumerator6.Current;
					if (dataColumn.CompiledExpression != null && dataColumn.CompiledExpression.DependsOn(column))
					{
						return dataColumn.Expression;
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator6 as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return string.Empty;
		}

		public bool CanRemove(DataColumn column)
		{
			if (column == null || column.Table != parentTable || GetColumnDependency(column) != string.Empty)
			{
				return false;
			}
			return true;
		}

		public void Clear()
		{
			CollectionChangeEventArgs ccevent = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this);
			if (parentTable.Constraints.Count != 0 || parentTable.ParentRelations.Count != 0 || parentTable.ChildRelations.Count != 0)
			{
				{
					IEnumerator enumerator = GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							DataColumn column = (DataColumn)enumerator.Current;
							string columnDependency = GetColumnDependency(column);
							if (columnDependency != string.Empty)
							{
								throw new ArgumentException("Cannot remove this column, because it is part of the" + columnDependency);
							}
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
				}
			}
			if (parentTable.DataSet != null)
			{
				foreach (DataTable table in parentTable.DataSet.Tables)
				{
					foreach (Constraint constraint in table.Constraints)
					{
						if (!(constraint is ForeignKeyConstraint) || ((ForeignKeyConstraint)constraint).RelatedTable != parentTable)
						{
							continue;
						}
						throw new ArgumentException(string.Format("Cannot remove this column, because it is part of the constraint {0} on the table {1}", constraint.ConstraintName, table.TableName));
					}
				}
			}
			IEnumerator enumerator4 = GetEnumerator();
			try
			{
				while (enumerator4.MoveNext())
				{
					DataColumn dataColumn = (DataColumn)enumerator4.Current;
					dataColumn.ResetColumnInfo();
				}
			}
			finally
			{
				IDisposable disposable2 = enumerator4 as IDisposable;
				if (disposable2 != null)
				{
					disposable2.Dispose();
				}
			}
			columnFromName.Clear();
			autoIncrement.Clear();
			columnNameCount.Clear();
			base.List.Clear();
			defaultColumnIndex = 1;
			OnCollectionChanged(ccevent);
		}

		public bool Contains(string name)
		{
			if (columnFromName.Contains(name))
			{
				return true;
			}
			return IndexOf(name, false) != -1;
		}

		public int IndexOf(DataColumn column)
		{
			if (column == null)
			{
				return -1;
			}
			return base.List.IndexOf(column);
		}

		public int IndexOf(string columnName)
		{
			if (columnName == null)
			{
				return -1;
			}
			DataColumn dataColumn = columnFromName[columnName] as DataColumn;
			if (dataColumn != null)
			{
				return IndexOf(dataColumn);
			}
			return IndexOf(columnName, false);
		}

		internal void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			parentTable.ResetPropertyDescriptorsCache();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, ccevent);
			}
		}

		internal void OnCollectionChanging(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanged != null)
			{
				throw new NotImplementedException();
			}
		}

		public void Remove(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column", "'column' argument cannot be null.");
			}
			if (!Contains(column.ColumnName))
			{
				throw new ArgumentException("Cannot remove a column that doesn't belong to this table.");
			}
			string columnDependency = GetColumnDependency(column);
			if (columnDependency != string.Empty)
			{
				throw new ArgumentException("Cannot remove this column, because it is part of " + columnDependency);
			}
			CollectionChangeEventArgs ccevent = new CollectionChangeEventArgs(CollectionChangeAction.Remove, column);
			int ordinal = column.Ordinal;
			UnregisterName(column.ColumnName);
			base.List.Remove(column);
			column.ResetColumnInfo();
			for (int i = ordinal; i < Count; i++)
			{
				this[i].Ordinal = i;
			}
			if (parentTable != null)
			{
				parentTable.OnRemoveColumn(column);
			}
			if (column.AutoIncrement)
			{
				autoIncrement.Remove(column);
			}
			column.PropertyChanged -= ColumnPropertyChanged;
			OnCollectionChanged(ccevent);
		}

		public void Remove(string name)
		{
			DataColumn dataColumn = this[name];
			if (dataColumn == null)
			{
				throw new ArgumentException("Column '" + name + "' does not belong to table " + ((parentTable != null) ? parentTable.TableName : string.Empty) + ".");
			}
			Remove(dataColumn);
		}

		public void RemoveAt(int index)
		{
			if (Count <= index)
			{
				throw new IndexOutOfRangeException("Cannot find column " + index + ".");
			}
			DataColumn column = this[index];
			Remove(column);
		}

		internal void PostAddRange()
		{
			if (_mostRecentColumns == null)
			{
				return;
			}
			DataColumn[] mostRecentColumns = _mostRecentColumns;
			foreach (DataColumn dataColumn in mostRecentColumns)
			{
				if (dataColumn != null)
				{
					Add(dataColumn);
				}
			}
			_mostRecentColumns = null;
		}

		internal void UpdateAutoIncrement(DataColumn col, bool isAutoIncrement)
		{
			if (isAutoIncrement)
			{
				if (!autoIncrement.Contains(col))
				{
					autoIncrement.Add(col);
				}
			}
			else if (autoIncrement.Contains(col))
			{
				autoIncrement.Remove(col);
			}
		}

		private int IndexOf(string name, bool error)
		{
			Doublet doublet = (Doublet)columnNameCount[name];
			if (doublet != null)
			{
				if (doublet.count == 1)
				{
					return base.List.IndexOf(columnFromName[doublet.columnNames[0]]);
				}
				if (doublet.count > 1 && error)
				{
					throw new ArgumentException("There is no match for '" + name + "' in the same case and there are multiple matches in different case.");
				}
				return -1;
			}
			return -1;
		}

		private void OnCollectionMetaDataChanged(CollectionChangeEventArgs ccevent)
		{
			parentTable.ResetPropertyDescriptorsCache();
			if (this.CollectionMetaDataChanged != null)
			{
				this.CollectionMetaDataChanged(this, ccevent);
			}
		}

		private void ColumnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			OnCollectionMetaDataChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}

		internal void MoveColumn(int oldOrdinal, int newOrdinal)
		{
			if (newOrdinal == -1 || newOrdinal > Count)
			{
				throw new ArgumentOutOfRangeException("ordinal", "Ordinal '" + newOrdinal + "' exceeds the maximum number.");
			}
			if (oldOrdinal != newOrdinal)
			{
				int num = ((newOrdinal <= oldOrdinal) ? newOrdinal : oldOrdinal);
				int num2 = ((newOrdinal <= oldOrdinal) ? oldOrdinal : newOrdinal);
				int num3 = ((newOrdinal > oldOrdinal) ? 1 : (-1));
				DataColumn dataColumn = this[num];
				for (int i = num; i < num2; i += num3)
				{
					List[i] = List[i + num3];
					((DataColumn)List[i]).Ordinal = i;
				}
				List[num2] = dataColumn;
				dataColumn.Ordinal = num2;
			}
		}
	}
}
