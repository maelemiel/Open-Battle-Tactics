using System.ComponentModel;
using Mono.Data.SqlExpressions;

namespace System.Data.Common
{
	internal class Key
	{
		private DataTable _table;

		private DataColumn[] _columns;

		private ListSortDirection[] _sortDirection;

		private DataViewRowState _rowStateFilter;

		private IExpression _filter;

		private DataRow _tmpRow;

		internal DataColumn[] Columns
		{
			get
			{
				return _columns;
			}
		}

		internal DataTable Table
		{
			get
			{
				return _table;
			}
		}

		private ListSortDirection[] Sort
		{
			get
			{
				return _sortDirection;
			}
		}

		internal DataViewRowState RowStateFilter
		{
			get
			{
				return _rowStateFilter;
			}
			set
			{
				_rowStateFilter = value;
			}
		}

		internal bool HasFilter
		{
			get
			{
				return _filter != null;
			}
		}

		internal Key(DataTable table, DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter)
		{
			_table = table;
			_filter = filter;
			if (_filter != null)
			{
				_tmpRow = _table.NewNotInitializedRow();
			}
			_columns = columns;
			if (sort != null && sort.Length == columns.Length)
			{
				_sortDirection = sort;
			}
			else
			{
				_sortDirection = new ListSortDirection[columns.Length];
				for (int i = 0; i < _sortDirection.Length; i++)
				{
					_sortDirection[i] = ListSortDirection.Ascending;
				}
			}
			if (rowState != DataViewRowState.None)
			{
				_rowStateFilter = rowState;
			}
			else
			{
				_rowStateFilter = DataViewRowState.CurrentRows;
			}
		}

		internal int CompareRecords(int first, int second)
		{
			if (first == second)
			{
				return 0;
			}
			for (int i = 0; i < Columns.Length; i++)
			{
				int num = Columns[i].CompareValues(first, second);
				if (num != 0)
				{
					return (Sort[i] != ListSortDirection.Ascending) ? (-num) : num;
				}
			}
			return 0;
		}

		internal int GetRecord(DataRow row)
		{
			int record = GetRecord(row, _rowStateFilter);
			if (_filter == null)
			{
				return record;
			}
			if (record < 0)
			{
				return record;
			}
			return (!CanContain(record)) ? (-1) : record;
		}

		internal bool CanContain(int index)
		{
			if (_filter == null)
			{
				return true;
			}
			_tmpRow._current = index;
			return _filter.EvalBoolean(_tmpRow);
		}

		internal bool ContainsVersion(DataRowState state, DataRowVersion version)
		{
			switch (state)
			{
			case DataRowState.Unchanged:
				if ((_rowStateFilter & DataViewRowState.Unchanged) != DataViewRowState.None)
				{
					return (version & DataRowVersion.Default) != 0;
				}
				break;
			case DataRowState.Added:
				if ((_rowStateFilter & DataViewRowState.Added) != DataViewRowState.None)
				{
					return (version & DataRowVersion.Default) != 0;
				}
				break;
			case DataRowState.Deleted:
				if ((_rowStateFilter & DataViewRowState.Deleted) != DataViewRowState.None)
				{
					return version == DataRowVersion.Original;
				}
				break;
			default:
				if ((_rowStateFilter & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
				{
					return (version & DataRowVersion.Default) != 0;
				}
				if ((_rowStateFilter & DataViewRowState.ModifiedOriginal) != DataViewRowState.None)
				{
					return version == DataRowVersion.Original;
				}
				break;
			}
			return false;
		}

		internal static int GetRecord(DataRow row, DataViewRowState rowStateFilter)
		{
			switch (row.RowState)
			{
			case DataRowState.Unchanged:
				if ((rowStateFilter & DataViewRowState.Unchanged) != DataViewRowState.None)
				{
					return (row.Proposed < 0) ? row.Current : row.Proposed;
				}
				break;
			case DataRowState.Added:
				if ((rowStateFilter & DataViewRowState.Added) != DataViewRowState.None)
				{
					return (row.Proposed < 0) ? row.Current : row.Proposed;
				}
				break;
			case DataRowState.Deleted:
				if ((rowStateFilter & DataViewRowState.Deleted) != DataViewRowState.None)
				{
					return row.Original;
				}
				break;
			default:
				if ((rowStateFilter & DataViewRowState.ModifiedCurrent) != DataViewRowState.None)
				{
					return (row.Proposed < 0) ? row.Current : row.Proposed;
				}
				if ((rowStateFilter & DataViewRowState.ModifiedOriginal) != DataViewRowState.None)
				{
					return row.Original;
				}
				break;
			}
			return -1;
		}

		internal bool Equals(DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter)
		{
			if (rowState != DataViewRowState.None && RowStateFilter != rowState)
			{
				return false;
			}
			if (_filter != null)
			{
				if (!_filter.Equals(filter))
				{
					return false;
				}
			}
			else if (filter != null)
			{
				return false;
			}
			if (Columns.Length != columns.Length)
			{
				return false;
			}
			if (sort != null && Sort.Length != sort.Length)
			{
				return false;
			}
			if (sort != null)
			{
				for (int i = 0; i < columns.Length; i++)
				{
					if (Sort[i] != sort[i] || Columns[i] != columns[i])
					{
						return false;
					}
				}
			}
			else
			{
				for (int j = 0; j < columns.Length; j++)
				{
					if (Sort[j] != ListSortDirection.Ascending || Columns[j] != columns[j])
					{
						return false;
					}
				}
			}
			return true;
		}

		internal bool DependsOn(DataColumn column)
		{
			if (_filter == null)
			{
				return false;
			}
			return _filter.DependsOn(column);
		}
	}
}
