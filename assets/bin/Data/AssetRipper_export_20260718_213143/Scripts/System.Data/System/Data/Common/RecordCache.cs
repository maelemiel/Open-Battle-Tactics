using System.Collections;

namespace System.Data.Common
{
	internal class RecordCache
	{
		private const int MIN_CACHE_SIZE = 128;

		private Stack _records = new Stack(16);

		private int _nextFreeIndex;

		private int _currentCapacity;

		private DataTable _table;

		private DataRow[] _rowsToRecords;

		internal int CurrentCapacity
		{
			get
			{
				return _currentCapacity;
			}
		}

		internal DataRow this[int index]
		{
			get
			{
				return _rowsToRecords[index];
			}
			set
			{
				if (index >= 0)
				{
					_rowsToRecords[index] = value;
				}
			}
		}

		internal RecordCache(DataTable table)
		{
			_table = table;
			_rowsToRecords = table.NewRowArray(16);
		}

		internal int NewRecord()
		{
			if (_records.Count > 0)
			{
				return (int)_records.Pop();
			}
			DataColumnCollection columns = _table.Columns;
			if (_nextFreeIndex >= _currentCapacity)
			{
				_currentCapacity *= 2;
				if (_currentCapacity < 128)
				{
					_currentCapacity = 128;
				}
				for (int i = 0; i < columns.Count; i++)
				{
					columns[i].DataContainer.Capacity = _currentCapacity;
				}
				DataRow[] rowsToRecords = _rowsToRecords;
				_rowsToRecords = _table.NewRowArray(_currentCapacity);
				Array.Copy(rowsToRecords, 0, _rowsToRecords, 0, rowsToRecords.Length);
			}
			return _nextFreeIndex++;
		}

		internal void DisposeRecord(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException();
			}
			if (!_records.Contains(index))
			{
				_records.Push(index);
			}
			this[index] = null;
		}

		internal int CopyRecord(DataTable fromTable, int fromRecordIndex, int toRecordIndex)
		{
			int num = toRecordIndex;
			if (toRecordIndex == -1)
			{
				num = NewRecord();
			}
			try
			{
				foreach (DataColumn column in fromTable.Columns)
				{
					DataColumn dataColumn2 = _table.Columns[column.ColumnName];
					if (dataColumn2 != null)
					{
						dataColumn2.DataContainer.CopyValue(column.DataContainer, fromRecordIndex, num);
					}
				}
				return num;
			}
			catch
			{
				if (toRecordIndex == -1)
				{
					DisposeRecord(num);
				}
				throw;
			}
		}

		internal void ReadIDataRecord(int recordIndex, IDataRecord record, int[] mapping, int length)
		{
			if (mapping.Length > _table.Columns.Count)
			{
				throw new ArgumentException();
			}
			int i;
			for (i = 0; i < length; i++)
			{
				DataColumn dataColumn = _table.Columns[mapping[i]];
				dataColumn.DataContainer.SetItemFromDataRecord(recordIndex, record, i);
			}
			for (; i < mapping.Length; i++)
			{
				DataColumn dataColumn2 = _table.Columns[mapping[i]];
				if (dataColumn2.AutoIncrement)
				{
					dataColumn2.DataContainer[recordIndex] = dataColumn2.AutoIncrementValue();
				}
				else
				{
					dataColumn2.DataContainer[recordIndex] = dataColumn2.DefaultValue;
				}
			}
		}
	}
}
