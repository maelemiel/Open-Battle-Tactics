using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Text;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.UniqueConstraintEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultProperty("ConstraintName")]
	public class UniqueConstraint : Constraint
	{
		private bool _isPrimaryKey;

		private bool _belongsToCollection;

		private DataTable _dataTable;

		private DataColumn[] _dataColumns;

		private string[] _dataColumnNames;

		private ForeignKeyConstraint _childConstraint;

		internal ForeignKeyConstraint ChildConstraint
		{
			get
			{
				return _childConstraint;
			}
			set
			{
				_childConstraint = value;
			}
		}

		[ReadOnly(true)]
		[DataCategory("Data")]
		public virtual DataColumn[] Columns
		{
			get
			{
				return _dataColumns;
			}
		}

		[DataCategory("Data")]
		public bool IsPrimaryKey
		{
			get
			{
				if (Table == null || !_belongsToCollection)
				{
					return false;
				}
				return _isPrimaryKey;
			}
		}

		[ReadOnly(true)]
		[DataCategory("Data")]
		public override DataTable Table
		{
			get
			{
				return _dataTable;
			}
		}

		public UniqueConstraint(DataColumn column)
		{
			_uniqueConstraint(string.Empty, column, false);
		}

		public UniqueConstraint(DataColumn[] columns)
		{
			_uniqueConstraint(string.Empty, columns, false);
		}

		public UniqueConstraint(DataColumn column, bool isPrimaryKey)
		{
			_uniqueConstraint(string.Empty, column, isPrimaryKey);
		}

		public UniqueConstraint(DataColumn[] columns, bool isPrimaryKey)
		{
			_uniqueConstraint(string.Empty, columns, isPrimaryKey);
		}

		public UniqueConstraint(string name, DataColumn column)
		{
			_uniqueConstraint(name, column, false);
		}

		public UniqueConstraint(string name, DataColumn[] columns)
		{
			_uniqueConstraint(name, columns, false);
		}

		public UniqueConstraint(string name, DataColumn column, bool isPrimaryKey)
		{
			_uniqueConstraint(name, column, isPrimaryKey);
		}

		public UniqueConstraint(string name, DataColumn[] columns, bool isPrimaryKey)
		{
			_uniqueConstraint(name, columns, isPrimaryKey);
		}

		[Browsable(false)]
		public UniqueConstraint(string name, string[] columnNames, bool isPrimaryKey)
		{
			InitInProgress = true;
			_dataColumnNames = columnNames;
			base.ConstraintName = name;
			_isPrimaryKey = isPrimaryKey;
		}

		private void _uniqueConstraint(string name, DataColumn column, bool isPrimaryKey)
		{
			_validateColumn(column);
			base.ConstraintName = name;
			_isPrimaryKey = isPrimaryKey;
			_dataColumns = new DataColumn[1] { column };
			_dataTable = column.Table;
		}

		private void _uniqueConstraint(string name, DataColumn[] columns, bool isPrimaryKey)
		{
			_validateColumns(columns, out _dataTable);
			base.ConstraintName = name;
			_dataColumns = columns;
			_isPrimaryKey = isPrimaryKey;
		}

		private void _validateColumns(DataColumn[] columns)
		{
			DataTable table;
			_validateColumns(columns, out table);
		}

		private void _validateColumns(DataColumn[] columns, out DataTable table)
		{
			table = null;
			if (columns == null)
			{
				throw new ArgumentNullException();
			}
			if (columns.Length < 1)
			{
				throw new InvalidConstraintException("Must be at least one column.");
			}
			DataTable table2 = columns[0].Table;
			foreach (DataColumn dataColumn in columns)
			{
				_validateColumn(dataColumn);
				if (table2 != dataColumn.Table)
				{
					throw new InvalidConstraintException("Columns must be from the same table.");
				}
			}
			table = table2;
		}

		private void _validateColumn(DataColumn column)
		{
			if (column == null)
			{
				throw new NullReferenceException("Object reference not set to an instance of an object.");
			}
			if (column.Table == null)
			{
				throw new ArgumentException("Column must belong to a table.");
			}
		}

		internal static void SetAsPrimaryKey(ConstraintCollection collection, UniqueConstraint newPrimaryKey)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("ConstraintCollection can't be null.");
			}
			if (collection.IndexOf(newPrimaryKey) < 0 && newPrimaryKey != null)
			{
				throw new ArgumentException("newPrimaryKey must belong to collection.");
			}
			UniqueConstraint primaryKeyConstraint = GetPrimaryKeyConstraint(collection);
			if (primaryKeyConstraint != null)
			{
				primaryKeyConstraint._isPrimaryKey = false;
			}
			if (newPrimaryKey != null)
			{
				newPrimaryKey._isPrimaryKey = true;
			}
		}

		internal static UniqueConstraint GetPrimaryKeyConstraint(ConstraintCollection collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("Collection can't be null.");
			}
			IEnumerator enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				UniqueConstraint uniqueConstraint = enumerator.Current as UniqueConstraint;
				if (uniqueConstraint == null || !uniqueConstraint.IsPrimaryKey)
				{
					continue;
				}
				return uniqueConstraint;
			}
			return null;
		}

		internal static UniqueConstraint GetUniqueConstraintForColumnSet(ConstraintCollection collection, DataColumn[] columns)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("Collection can't be null.");
			}
			if (columns == null)
			{
				return null;
			}
			foreach (Constraint item in collection)
			{
				if (item is UniqueConstraint)
				{
					UniqueConstraint uniqueConstraint = item as UniqueConstraint;
					if (DataColumn.AreColumnSetsTheSame(uniqueConstraint.Columns, columns))
					{
						return uniqueConstraint;
					}
				}
			}
			return null;
		}

		internal override void FinishInit(DataTable _setTable)
		{
			_dataTable = _setTable;
			if (_isPrimaryKey && _setTable.PrimaryKey.Length != 0)
			{
				throw new ArgumentException("Cannot add primary key constraint since primary keyis already set for the table");
			}
			DataColumn[] array = new DataColumn[_dataColumnNames.Length];
			int num = 0;
			string[] dataColumnNames = _dataColumnNames;
			foreach (string name in dataColumnNames)
			{
				if (_setTable.Columns.Contains(name))
				{
					array[num] = _setTable.Columns[name];
					num++;
					continue;
				}
				throw new InvalidConstraintException("The named columns must exist in the table");
			}
			_dataColumns = array;
			_validateColumns(array);
			InitInProgress = false;
		}

		internal void SetIsPrimaryKey(bool value)
		{
			_isPrimaryKey = value;
		}

		public override bool Equals(object key2)
		{
			UniqueConstraint uniqueConstraint = key2 as UniqueConstraint;
			if (uniqueConstraint == null)
			{
				return false;
			}
			return DataColumn.AreColumnSetsTheSame(uniqueConstraint.Columns, Columns);
		}

		public override int GetHashCode()
		{
			int num = 42;
			if (Columns.Length > 0)
			{
				num ^= Columns[0].GetHashCode();
			}
			for (int i = 1; i < Columns.Length; i++)
			{
				num ^= Columns[1].GetHashCode();
			}
			return num;
		}

		internal override void AddToConstraintCollectionSetup(ConstraintCollection collection)
		{
			for (int i = 0; i < Columns.Length; i++)
			{
				if (Columns[i].Table != collection.Table)
				{
					throw new ArgumentException("These columns don't point to this table.");
				}
			}
			_validateColumns(_dataColumns);
			UniqueConstraint uniqueConstraintForColumnSet = GetUniqueConstraintForColumnSet(collection, Columns);
			if (uniqueConstraintForColumnSet != null)
			{
				throw new ArgumentException("Unique constraint already exists for these columns. Existing ConstraintName is " + uniqueConstraintForColumnSet.ConstraintName);
			}
			if (IsPrimaryKey)
			{
				uniqueConstraintForColumnSet = GetPrimaryKeyConstraint(collection);
				if (uniqueConstraintForColumnSet != null)
				{
					uniqueConstraintForColumnSet._isPrimaryKey = false;
				}
			}
			if (_dataColumns.Length == 1)
			{
				_dataColumns[0].SetUnique();
			}
			if (IsConstraintViolated())
			{
				throw new ArgumentException("These columns don't currently have unique values.");
			}
			_belongsToCollection = true;
		}

		internal override void RemoveFromConstraintCollectionCleanup(ConstraintCollection collection)
		{
			if (Columns.Length == 1)
			{
				Columns[0].Unique = false;
			}
			_belongsToCollection = false;
			Index index = base.Index;
			base.Index = null;
		}

		internal override bool IsConstraintViolated()
		{
			if (base.Index == null)
			{
				base.Index = Table.GetIndex(Columns, null, DataViewRowState.None, null, false);
			}
			if (base.Index.HasDuplicates)
			{
				int[] duplicates = base.Index.Duplicates;
				for (int i = 0; i < duplicates.Length; i++)
				{
					DataRow dataRow = Table.RecordCache[duplicates[i]];
					ArrayList arrayList = new ArrayList();
					ArrayList arrayList2 = new ArrayList();
					DataColumn[] columns = Columns;
					foreach (DataColumn dataColumn in columns)
					{
						arrayList.Add(dataColumn.ColumnName);
						arrayList2.Add(dataRow[dataColumn].ToString());
					}
					string arg = string.Join(", ", (string[])arrayList.ToArray(typeof(string)));
					string arg2 = string.Join(", ", (string[])arrayList2.ToArray(typeof(string)));
					dataRow.RowError = string.Format("Column '{0}' is constrained to be unique.  Value '{1}' is already present.", arg, arg2);
					for (int k = 0; k < Columns.Length; k++)
					{
						dataRow.SetColumnError(Columns[k], dataRow.RowError);
					}
				}
				return true;
			}
			return false;
		}

		internal override void AssertConstraint(DataRow row)
		{
			if (IsPrimaryKey && row.HasVersion(DataRowVersion.Default))
			{
				for (int i = 0; i < Columns.Length; i++)
				{
					if (row.IsNull(Columns[i]))
					{
						throw new NoNullAllowedException("Column '" + Columns[i].ColumnName + "' does not allow nulls.");
					}
				}
			}
			if (base.Index == null)
			{
				base.Index = Table.GetIndex(Columns, null, DataViewRowState.None, null, false);
			}
			if (base.Index.HasDuplicates)
			{
				throw new ConstraintException(GetErrorMessage(row));
			}
		}

		internal override bool IsColumnContained(DataColumn column)
		{
			for (int i = 0; i < _dataColumns.Length; i++)
			{
				if (column == _dataColumns[i])
				{
					return true;
				}
			}
			return false;
		}

		internal override bool CanRemoveFromCollection(ConstraintCollection col, bool shouldThrow)
		{
			if (IsPrimaryKey)
			{
				if (shouldThrow)
				{
					throw new ArgumentException("Cannot remove unique constraint since it's the primary key of a table.");
				}
				return false;
			}
			if (Table.DataSet == null)
			{
				return true;
			}
			if (ChildConstraint != null)
			{
				if (!shouldThrow)
				{
					return false;
				}
				throw new ArgumentException(string.Format("Cannot remove unique constraint '{0}'.Remove foreign key constraint '{1}' first.", ConstraintName, ChildConstraint.ConstraintName));
			}
			return true;
		}

		private string GetErrorMessage(DataRow row)
		{
			StringBuilder stringBuilder = new StringBuilder(row[_dataColumns[0]].ToString());
			for (int i = 1; i < _dataColumns.Length; i++)
			{
				stringBuilder = stringBuilder.Append(", ").Append(row[_dataColumns[i].ColumnName]);
			}
			string text = stringBuilder.ToString();
			stringBuilder = new StringBuilder(_dataColumns[0].ColumnName);
			for (int i = 1; i < _dataColumns.Length; i++)
			{
				stringBuilder = stringBuilder.Append(", ").Append(_dataColumns[i].ColumnName);
			}
			string text2 = stringBuilder.ToString();
			return "Column '" + text2 + "' is constrained to be unique.  Value '" + text + "' is already present.";
		}
	}
}
