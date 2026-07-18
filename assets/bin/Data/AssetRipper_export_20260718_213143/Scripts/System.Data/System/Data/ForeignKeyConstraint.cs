using System.ComponentModel;
using System.Text;

namespace System.Data
{
	[DefaultProperty("ConstraintName")]
	[Editor("Microsoft.VSDesigner.Data.Design.ForeignKeyConstraintEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class ForeignKeyConstraint : Constraint
	{
		private UniqueConstraint _parentUniqueConstraint;

		private DataColumn[] _parentColumns;

		private DataColumn[] _childColumns;

		private Rule _deleteRule = Rule.Cascade;

		private Rule _updateRule = Rule.Cascade;

		private AcceptRejectRule _acceptRejectRule;

		private string _parentTableName;

		private string _parentTableNamespace;

		private string _childTableName;

		private string[] _parentColumnNames;

		private string[] _childColumnNames;

		[DefaultValue(AcceptRejectRule.None)]
		[DataCategory("Data")]
		public virtual AcceptRejectRule AcceptRejectRule
		{
			get
			{
				return _acceptRejectRule;
			}
			set
			{
				_acceptRejectRule = value;
			}
		}

		[DataCategory("Data")]
		[ReadOnly(true)]
		public virtual DataColumn[] Columns
		{
			get
			{
				return _childColumns;
			}
		}

		[DefaultValue(Rule.Cascade)]
		[DataCategory("Data")]
		public virtual Rule DeleteRule
		{
			get
			{
				return _deleteRule;
			}
			set
			{
				_deleteRule = value;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(Rule.Cascade)]
		public virtual Rule UpdateRule
		{
			get
			{
				return _updateRule;
			}
			set
			{
				_updateRule = value;
			}
		}

		[DataCategory("Data")]
		[ReadOnly(true)]
		public virtual DataColumn[] RelatedColumns
		{
			get
			{
				return _parentColumns;
			}
		}

		[ReadOnly(true)]
		[DataCategory("Data")]
		public virtual DataTable RelatedTable
		{
			get
			{
				if (_parentColumns != null && _parentColumns.Length > 0)
				{
					return _parentColumns[0].Table;
				}
				throw new InvalidOperationException("Property not accessible because 'Object reference not set to an instance of an object'");
			}
		}

		[ReadOnly(true)]
		[DataCategory("Data")]
		public override DataTable Table
		{
			get
			{
				if (_childColumns != null && _childColumns.Length > 0)
				{
					return _childColumns[0].Table;
				}
				throw new InvalidOperationException("Property not accessible because 'Object reference not set to an instance of an object'");
			}
		}

		internal UniqueConstraint ParentConstraint
		{
			get
			{
				return _parentUniqueConstraint;
			}
		}

		public ForeignKeyConstraint(DataColumn parentColumn, DataColumn childColumn)
		{
			if (parentColumn == null || childColumn == null)
			{
				throw new NullReferenceException("Neither parentColumn or childColumn can be null.");
			}
			_foreignKeyConstraint(null, new DataColumn[1] { parentColumn }, new DataColumn[1] { childColumn });
		}

		public ForeignKeyConstraint(DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			_foreignKeyConstraint(null, parentColumns, childColumns);
		}

		public ForeignKeyConstraint(string constraintName, DataColumn parentColumn, DataColumn childColumn)
		{
			if (parentColumn == null || childColumn == null)
			{
				throw new NullReferenceException("Neither parentColumn or childColumn can be null.");
			}
			_foreignKeyConstraint(constraintName, new DataColumn[1] { parentColumn }, new DataColumn[1] { childColumn });
		}

		public ForeignKeyConstraint(string constraintName, DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			_foreignKeyConstraint(constraintName, parentColumns, childColumns);
		}

		[Browsable(false)]
		public ForeignKeyConstraint(string constraintName, string parentTableName, string[] parentColumnNames, string[] childColumnNames, AcceptRejectRule acceptRejectRule, Rule deleteRule, Rule updateRule)
		{
			InitInProgress = true;
			base.ConstraintName = constraintName;
			_parentTableName = parentTableName;
			_parentColumnNames = parentColumnNames;
			_childColumnNames = childColumnNames;
			_acceptRejectRule = acceptRejectRule;
			_deleteRule = deleteRule;
			_updateRule = updateRule;
		}

		[Browsable(false)]
		public ForeignKeyConstraint(string constraintName, string parentTableName, string parentTableNamespace, string[] parentColumnNames, string[] childColumnNames, AcceptRejectRule acceptRejectRule, Rule deleteRule, Rule updateRule)
		{
			InitInProgress = true;
			base.ConstraintName = constraintName;
			_parentTableName = parentTableName;
			_parentTableNamespace = parentTableNamespace;
			_parentColumnNames = parentColumnNames;
			_childColumnNames = childColumnNames;
			_acceptRejectRule = acceptRejectRule;
			_deleteRule = deleteRule;
			_updateRule = updateRule;
		}

		internal override void FinishInit(DataTable childTable)
		{
			if (childTable.DataSet == null)
			{
				throw new InvalidConstraintException("ChildTable : " + childTable.TableName + " does not belong to any DataSet");
			}
			DataSet dataSet = childTable.DataSet;
			_childTableName = childTable.TableName;
			if (!dataSet.Tables.Contains(_parentTableName))
			{
				throw new InvalidConstraintException("Table : " + _parentTableName + "does not exist in DataSet : " + dataSet);
			}
			DataTable dataTable = dataSet.Tables[_parentTableName];
			int num = 0;
			int num2 = 0;
			if (_parentColumnNames.Length < 0 || _childColumnNames.Length < 0)
			{
				throw new InvalidConstraintException("Neither parent nor child columns can be zero length");
			}
			if (_parentColumnNames.Length != _childColumnNames.Length)
			{
				throw new InvalidConstraintException("Both parent and child columns must be of same length");
			}
			DataColumn[] array = new DataColumn[_parentColumnNames.Length];
			DataColumn[] array2 = new DataColumn[_childColumnNames.Length];
			string[] parentColumnNames = _parentColumnNames;
			foreach (string text in parentColumnNames)
			{
				if (!dataTable.Columns.Contains(text))
				{
					throw new InvalidConstraintException("Table : " + _parentTableName + "does not contain the column :" + text);
				}
				array[num++] = dataTable.Columns[text];
			}
			string[] childColumnNames = _childColumnNames;
			foreach (string text2 in childColumnNames)
			{
				if (!childTable.Columns.Contains(text2))
				{
					throw new InvalidConstraintException("Table : " + _childTableName + "does not contain the column : " + text2);
				}
				array2[num2++] = childTable.Columns[text2];
			}
			_validateColumns(array, array2);
			_parentColumns = array;
			_childColumns = array2;
			dataTable.Namespace = _parentTableNamespace;
			InitInProgress = false;
		}

		private void _foreignKeyConstraint(string constraintName, DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			_validateColumns(parentColumns, childColumns);
			base.ConstraintName = constraintName;
			_parentColumns = parentColumns;
			_childColumns = childColumns;
		}

		private void _validateColumns(DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			if (parentColumns == null || childColumns == null)
			{
				throw new ArgumentNullException();
			}
			if (parentColumns.Length < 1 || childColumns.Length < 1)
			{
				throw new ArgumentException("Neither ParentColumns or ChildColumns can't be zero length.");
			}
			if (parentColumns.Length != childColumns.Length)
			{
				throw new ArgumentException("Parent columns and child columns must be the same length.");
			}
			DataTable table = parentColumns[0].Table;
			DataTable table2 = childColumns[0].Table;
			for (int i = 0; i < parentColumns.Length; i++)
			{
				DataColumn dataColumn = parentColumns[i];
				DataColumn dataColumn2 = childColumns[i];
				if (dataColumn.Table == null)
				{
					throw new ArgumentException("All columns must belong to a table. ColumnName: " + dataColumn.ColumnName + " does not belong to a table.");
				}
				if (table != dataColumn.Table)
				{
					throw new InvalidConstraintException("Parent columns must all belong to the same table.");
				}
				if (dataColumn2.Table == null)
				{
					throw new ArgumentException("All columns must belong to a table. ColumnName: " + dataColumn.ColumnName + " does not belong to a table.");
				}
				if (table2 != dataColumn2.Table)
				{
					throw new InvalidConstraintException("Child columns must all belong to the same table.");
				}
				if (dataColumn.CompiledExpression != null)
				{
					throw new ArgumentException(string.Format("Cannot create a constraint based on Expression column {0}.", dataColumn.ColumnName));
				}
				if (dataColumn2.CompiledExpression != null)
				{
					throw new ArgumentException(string.Format("Cannot create a constraint based on Expression column {0}.", dataColumn2.ColumnName));
				}
			}
			if (table.DataSet != table2.DataSet)
			{
				throw new InvalidOperationException("Parent column and child column must belong to tables that belong to the same DataSet.");
			}
			int num = 0;
			for (int j = 0; j < parentColumns.Length; j++)
			{
				DataColumn dataColumn3 = parentColumns[j];
				DataColumn dataColumn4 = childColumns[j];
				if (dataColumn3 == dataColumn4)
				{
					num++;
				}
				else if (!dataColumn3.DataTypeMatches(dataColumn4))
				{
					throw new InvalidOperationException("Parent column is not type compatible with it's child column.");
				}
			}
			if (num == parentColumns.Length)
			{
				throw new InvalidOperationException("Property not accessible because 'ParentKey and ChildKey are identical.'.");
			}
		}

		private void _ensureUniqueConstraintExists(ConstraintCollection collection, DataColumn[] parentColumns)
		{
			if (parentColumns == null)
			{
				throw new ArgumentNullException("ParentColumns can't be null");
			}
			UniqueConstraint uniqueConstraint = null;
			if (parentColumns[0] != null)
			{
				uniqueConstraint = UniqueConstraint.GetUniqueConstraintForColumnSet(parentColumns[0].Table.Constraints, parentColumns);
			}
			if (uniqueConstraint == null)
			{
				uniqueConstraint = new UniqueConstraint(parentColumns, false);
				parentColumns[0].Table.Constraints.Add(uniqueConstraint);
			}
			_parentUniqueConstraint = uniqueConstraint;
			_parentUniqueConstraint.ChildConstraint = this;
		}

		public override bool Equals(object key)
		{
			ForeignKeyConstraint foreignKeyConstraint = key as ForeignKeyConstraint;
			if (foreignKeyConstraint == null)
			{
				return false;
			}
			if (!DataColumn.AreColumnSetsTheSame(RelatedColumns, foreignKeyConstraint.RelatedColumns))
			{
				return false;
			}
			if (!DataColumn.AreColumnSetsTheSame(Columns, foreignKeyConstraint.Columns))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 32;
			int num2 = 88;
			if (Columns.Length > 0)
			{
				num ^= Columns[0].GetHashCode();
			}
			for (int i = 1; i < Columns.Length; i++)
			{
				num ^= Columns[1].GetHashCode();
			}
			if (RelatedColumns.Length > 0)
			{
				num2 ^= Columns[0].GetHashCode();
			}
			for (int i = 1; i < RelatedColumns.Length; i++)
			{
				num2 ^= RelatedColumns[1].GetHashCode();
			}
			return num ^ num2;
		}

		internal override void AddToConstraintCollectionSetup(ConstraintCollection collection)
		{
			if (collection.Table != Table)
			{
				throw new InvalidConstraintException("This constraint cannot be added since ForeignKey doesn't belong to table " + RelatedTable.TableName + ".");
			}
			_validateColumns(_parentColumns, _childColumns);
			_ensureUniqueConstraintExists(collection, _parentColumns);
			if (((Table.DataSet != null && Table.DataSet.EnforceConstraints) || (Table.DataSet == null && Table.EnforceConstraints)) && IsConstraintViolated())
			{
				throw new ArgumentException("This constraint cannot be enabled as not all values have corresponding parent values.");
			}
		}

		internal override void RemoveFromConstraintCollectionCleanup(ConstraintCollection collection)
		{
			_parentUniqueConstraint.ChildConstraint = null;
			base.Index = null;
		}

		internal override bool IsConstraintViolated()
		{
			if (Table.DataSet == null || RelatedTable.DataSet == null)
			{
				return false;
			}
			bool flag = false;
			foreach (DataRow row in Table.Rows)
			{
				if (!row.IsNullColumns(_childColumns) && !RelatedTable.RowsExist(_parentColumns, _childColumns, row))
				{
					flag = true;
					string[] array = new string[_childColumns.Length];
					for (int i = 0; i < _childColumns.Length; i++)
					{
						DataColumn column = _childColumns[i];
						array[i] = row[column].ToString();
					}
					row.RowError = string.Format("ForeignKeyConstraint {0} requires the child key values ({1}) to exist in the parent table.", ConstraintName, string.Join(",", array));
				}
			}
			if (flag)
			{
				return true;
			}
			return false;
		}

		internal override void AssertConstraint(DataRow row)
		{
			if (row.IsNullColumns(_childColumns) || RelatedTable.RowsExist(_parentColumns, _childColumns, row))
			{
				return;
			}
			throw new InvalidConstraintException(GetErrorMessage(row));
		}

		internal override bool IsColumnContained(DataColumn column)
		{
			for (int i = 0; i < _parentColumns.Length; i++)
			{
				if (column == _parentColumns[i])
				{
					return true;
				}
			}
			for (int j = 0; j < _childColumns.Length; j++)
			{
				if (column == _childColumns[j])
				{
					return true;
				}
			}
			return false;
		}

		internal override bool CanRemoveFromCollection(ConstraintCollection col, bool shouldThrow)
		{
			return true;
		}

		private string GetErrorMessage(DataRow row)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _childColumns.Length; i++)
			{
				stringBuilder.Append(row[_childColumns[0]].ToString());
				if (i != _childColumns.Length - 1)
				{
					stringBuilder.Append(',');
				}
			}
			string text = stringBuilder.ToString();
			return "ForeignKeyConstraint " + ConstraintName + " requires the child key values (" + text + ") to exist in the parent table.";
		}
	}
}
