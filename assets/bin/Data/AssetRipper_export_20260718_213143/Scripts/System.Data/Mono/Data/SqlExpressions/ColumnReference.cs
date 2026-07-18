using System;
using System.ComponentModel;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class ColumnReference : BaseExpression
	{
		private ReferencedTable refTable;

		private string relationName;

		private string columnName;

		private DataColumn _cachedColumn;

		private DataRelation _cachedRelation;

		public ReferencedTable ReferencedTable
		{
			get
			{
				return refTable;
			}
		}

		public ColumnReference(string columnName)
			: this(ReferencedTable.Self, null, columnName)
		{
		}

		public ColumnReference(ReferencedTable refTable, string relationName, string columnName)
		{
			this.refTable = refTable;
			this.relationName = relationName;
			this.columnName = columnName;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is ColumnReference))
			{
				return false;
			}
			ColumnReference columnReference = (ColumnReference)obj;
			if (columnReference.refTable != refTable)
			{
				return false;
			}
			if (columnReference.columnName != columnName)
			{
				return false;
			}
			if (columnReference.relationName != relationName)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode ^= refTable.GetHashCode();
			hashCode ^= columnName.GetHashCode();
			return hashCode ^ relationName.GetHashCode();
		}

		private DataRelation GetRelation(DataRow row)
		{
			if (_cachedRelation == null)
			{
				DataTable table = row.Table;
				if (relationName != null)
				{
					DataRelationCollection relations = table.DataSet.Relations;
					_cachedRelation = relations[relations.IndexOf(relationName)];
				}
				else
				{
					DataRelationCollection relations = ((refTable != ReferencedTable.Parent) ? table.ChildRelations : table.ParentRelations);
					if (relations.Count > 1)
					{
						throw new EvaluateException(string.Format("The table [{0}] is involved in more than one relation.You must explicitly mention a relation name.", table.TableName));
					}
					_cachedRelation = relations[0];
				}
				_cachedRelation.DataSet.Relations.CollectionChanged += OnRelationRemoved;
			}
			return _cachedRelation;
		}

		private DataColumn GetColumn(DataRow row)
		{
			if (_cachedColumn == null)
			{
				DataTable dataTable = row.Table;
				switch (refTable)
				{
				case ReferencedTable.Parent:
					dataTable = GetRelation(row).ParentTable;
					break;
				case ReferencedTable.Child:
					dataTable = GetRelation(row).ChildTable;
					break;
				}
				_cachedColumn = dataTable.Columns[columnName];
				if (_cachedColumn == null)
				{
					throw new EvaluateException(string.Format("Cannot find column [{0}].", columnName));
				}
				_cachedColumn.PropertyChanged += OnColumnPropertyChanged;
				_cachedColumn.Table.Columns.CollectionChanged += OnColumnRemoved;
			}
			return _cachedColumn;
		}

		public DataRow GetReferencedRow(DataRow row)
		{
			GetColumn(row);
			switch (refTable)
			{
			default:
				return row;
			case ReferencedTable.Parent:
				return row.GetParentRow(GetRelation(row));
			case ReferencedTable.Child:
				return row.GetChildRows(GetRelation(row))[0];
			}
		}

		public DataRow[] GetReferencedRows(DataRow row)
		{
			GetColumn(row);
			switch (refTable)
			{
			default:
			{
				DataRow[] array = row.Table.NewRowArray(row.Table.Rows.Count);
				row.Table.Rows.CopyTo(array, 0);
				return array;
			}
			case ReferencedTable.Parent:
				return row.GetParentRows(GetRelation(row));
			case ReferencedTable.Child:
				return row.GetChildRows(GetRelation(row));
			}
		}

		public object[] GetValues(DataRow[] rows)
		{
			object[] array = new object[rows.Length];
			for (int i = 0; i < rows.Length; i++)
			{
				array[i] = Unify(rows[i][GetColumn(rows[i])]);
			}
			return array;
		}

		private object Unify(object val)
		{
			if (Numeric.IsNumeric(val))
			{
				return Numeric.Unify((IConvertible)val);
			}
			if (val == null || val == DBNull.Value)
			{
				return null;
			}
			if (val is bool || val is string || val is DateTime || val is Guid || val is char)
			{
				return val;
			}
			if (val is Enum)
			{
				return (int)val;
			}
			throw new EvaluateException(string.Format("Cannot handle data type found in column '{0}'.", columnName));
		}

		public override object Eval(DataRow row)
		{
			DataRow referencedRow = GetReferencedRow(row);
			if (referencedRow == null)
			{
				return null;
			}
			object val;
			try
			{
				referencedRow._inExpressionEvaluation = true;
				val = referencedRow[GetColumn(row)];
				referencedRow._inExpressionEvaluation = false;
			}
			catch (IndexOutOfRangeException)
			{
				throw new EvaluateException(string.Format("Cannot find column [{0}].", columnName));
			}
			return Unify(val);
		}

		public override bool EvalBoolean(DataRow row)
		{
			DataColumn column = GetColumn(row);
			if (column.DataType != typeof(bool))
			{
				throw new EvaluateException("Not a Boolean Expression");
			}
			object obj = Eval(row);
			if (obj == null || obj == DBNull.Value)
			{
				return false;
			}
			return (bool)obj;
		}

		public override bool DependsOn(DataColumn other)
		{
			return refTable == ReferencedTable.Self && columnName == other.ColumnName;
		}

		private void DropCached(DataColumnCollection columnCollection, DataRelationCollection relationCollection)
		{
			if (_cachedColumn != null)
			{
				_cachedColumn.PropertyChanged -= OnColumnPropertyChanged;
				if (columnCollection != null)
				{
					columnCollection.CollectionChanged -= OnColumnRemoved;
				}
				else if (_cachedColumn.Table != null)
				{
					_cachedColumn.Table.Columns.CollectionChanged -= OnColumnRemoved;
				}
				_cachedColumn = null;
			}
			if (_cachedRelation != null)
			{
				if (relationCollection != null)
				{
					relationCollection.CollectionChanged -= OnRelationRemoved;
				}
				else if (_cachedRelation.DataSet != null)
				{
					_cachedRelation.DataSet.Relations.CollectionChanged -= OnRelationRemoved;
				}
				_cachedRelation = null;
			}
		}

		private void OnColumnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (sender is DataColumn)
			{
				DataColumn dataColumn = (DataColumn)sender;
				if (dataColumn == _cachedColumn && args.PropertyName == "ColumnName")
				{
					DropCached(null, null);
				}
			}
		}

		private void OnColumnRemoved(object sender, CollectionChangeEventArgs args)
		{
			if (args.Element is DataColumnCollection && args.Action == CollectionChangeAction.Remove)
			{
				DataColumnCollection dataColumnCollection = (DataColumnCollection)args.Element;
				if (_cachedColumn != null && dataColumnCollection != null && dataColumnCollection.IndexOf(_cachedColumn) == -1)
				{
					DropCached(dataColumnCollection, null);
				}
			}
		}

		private void OnRelationRemoved(object sender, CollectionChangeEventArgs args)
		{
			if (args.Element is DataRelationCollection && args.Action == CollectionChangeAction.Remove)
			{
				DataRelationCollection dataRelationCollection = (DataRelationCollection)args.Element;
				if (_cachedRelation != null && dataRelationCollection != null && dataRelationCollection.IndexOf(_cachedRelation) == -1)
				{
					DropCached(null, dataRelationCollection);
				}
			}
		}
	}
}
