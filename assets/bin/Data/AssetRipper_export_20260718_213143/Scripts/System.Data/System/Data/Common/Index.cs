using System.Collections;

namespace System.Data.Common
{
	internal class Index
	{
		private static readonly int[] empty = new int[0];

		private int[] _array;

		private int _size;

		private Key _key;

		private int _refCount;

		private bool know_have_duplicates;

		private bool know_no_duplicates;

		internal Key Key
		{
			get
			{
				return _key;
			}
		}

		internal int Size
		{
			get
			{
				return _size;
			}
		}

		internal int RefCount
		{
			get
			{
				return _refCount;
			}
		}

		internal bool HasDuplicates
		{
			get
			{
				if (!know_have_duplicates && !know_no_duplicates)
				{
					for (int i = 0; i < _size - 1; i++)
					{
						if (Key.CompareRecords(_array[i], _array[i + 1]) == 0)
						{
							know_have_duplicates = true;
							break;
						}
					}
					know_no_duplicates = !know_have_duplicates;
				}
				return know_have_duplicates;
			}
		}

		internal int[] Duplicates
		{
			get
			{
				if (!HasDuplicates)
				{
					return null;
				}
				ArrayList arrayList = new ArrayList();
				bool flag = false;
				for (int i = 0; i < _size - 1; i++)
				{
					if (Key.CompareRecords(_array[i], _array[i + 1]) == 0)
					{
						if (!flag)
						{
							arrayList.Add(_array[i]);
							flag = true;
						}
						arrayList.Add(_array[i + 1]);
					}
					else
					{
						flag = false;
					}
				}
				return (int[])arrayList.ToArray(typeof(int));
			}
		}

		internal Index(Key key)
		{
			_key = key;
			Reset();
		}

		internal int IndexToRecord(int index)
		{
			return (index >= 0) ? _array[index] : index;
		}

		internal int[] GetAll()
		{
			return _array;
		}

		internal DataRow[] GetAllRows()
		{
			DataRow[] array = new DataRow[_size];
			for (int i = 0; i < _size; i++)
			{
				array[i] = Key.Table.RecordCache[_array[i]];
			}
			return array;
		}

		internal DataRow[] GetDistinctRows()
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add(Key.Table.RecordCache[_array[0]]);
			int first = _array[0];
			for (int i = 1; i < _size; i++)
			{
				if (Key.CompareRecords(first, _array[i]) != 0)
				{
					arrayList.Add(Key.Table.RecordCache[_array[i]]);
					first = _array[i];
				}
			}
			return (DataRow[])arrayList.ToArray(typeof(DataRow));
		}

		internal void Reset()
		{
			_array = empty;
			_size = 0;
			RebuildIndex();
		}

		private void RebuildIndex()
		{
			int currentCapacity = Key.Table.RecordCache.CurrentCapacity;
			if (currentCapacity == 0)
			{
				return;
			}
			_array = new int[currentCapacity];
			_size = 0;
			foreach (DataRow row in Key.Table.Rows)
			{
				int record = Key.GetRecord(row);
				if (record != -1)
				{
					_array[_size++] = record;
				}
			}
			know_have_duplicates = (know_no_duplicates = false);
			Sort();
			know_no_duplicates = !know_have_duplicates;
		}

		private void Sort()
		{
			MergeSort(_array, _size);
		}

		internal int Find(object[] keys)
		{
			int index = FindIndex(keys);
			return IndexToRecord(index);
		}

		internal int FindIndex(object[] keys)
		{
			if (keys == null || keys.Length != Key.Columns.Length)
			{
				throw new ArgumentException("Expecting " + Key.Columns.Length + " value(s) for the key being indexed, but received " + ((keys != null) ? keys.Length : 0) + " value(s).");
			}
			int num = Key.Table.RecordCache.NewRecord();
			try
			{
				for (int i = 0; i < Key.Columns.Length; i++)
				{
					Key.Columns[i].DataContainer[num] = keys[i];
				}
				return FindIndex(num);
			}
			finally
			{
				Key.Table.RecordCache.DisposeRecord(num);
			}
		}

		internal int Find(int record)
		{
			int index = FindIndex(record);
			return IndexToRecord(index);
		}

		internal int[] FindAll(object[] keys)
		{
			int[] array = FindAllIndexes(keys);
			IndexesToRecords(array);
			return array;
		}

		internal int[] FindAllIndexes(object[] keys)
		{
			if (keys == null || keys.Length != Key.Columns.Length)
			{
				throw new ArgumentException("Expecting " + Key.Columns.Length + " value(s) for the key being indexed,but received " + ((keys != null) ? keys.Length : 0) + " value(s).");
			}
			int num = Key.Table.RecordCache.NewRecord();
			try
			{
				for (int i = 0; i < Key.Columns.Length; i++)
				{
					Key.Columns[i].DataContainer[num] = keys[i];
				}
				return FindAllIndexes(num);
			}
			catch (FormatException)
			{
				return empty;
			}
			catch (InvalidCastException)
			{
				return empty;
			}
			finally
			{
				Key.Table.RecordCache.DisposeRecord(num);
			}
		}

		internal int[] FindAll(int record)
		{
			int[] array = FindAllIndexes(record);
			IndexesToRecords(array);
			return array;
		}

		internal int[] FindAllIndexes(int record)
		{
			int num = FindIndex(record);
			if (num == -1)
			{
				return empty;
			}
			int num2 = num++;
			int i = num;
			while (num2 >= 0 && Key.CompareRecords(_array[num2], record) == 0)
			{
				num2--;
			}
			for (; i < _size && Key.CompareRecords(_array[i], record) == 0; i++)
			{
			}
			int num3 = i - num2 - 1;
			int[] array = new int[num3];
			for (int j = 0; j < num3; j++)
			{
				num2 = (array[j] = num2 + 1);
			}
			return array;
		}

		private int FindIndex(int record)
		{
			if (_size == 0)
			{
				return -1;
			}
			return BinarySearch(_array, 0, _size - 1, record);
		}

		private int FindIndexExact(int record)
		{
			int i = 0;
			for (int size = _size; i < size; i++)
			{
				if (_array[i] == record)
				{
					return i;
				}
			}
			return -1;
		}

		private void IndexesToRecords(int[] indexes)
		{
			for (int i = 0; i < indexes.Length; i++)
			{
				indexes[i] = _array[indexes[i]];
			}
		}

		internal void Delete(DataRow row)
		{
			int record = Key.GetRecord(row);
			Delete(record);
		}

		internal void Delete(int oldRecord)
		{
			if (oldRecord == -1)
			{
				return;
			}
			int num = FindIndexExact(oldRecord);
			if (num == -1)
			{
				return;
			}
			if (know_have_duplicates)
			{
				int num2 = 1;
				int num3 = 1;
				if (num > 0)
				{
					num2 = Key.CompareRecords(_array[num - 1], oldRecord);
				}
				if (num < _size - 1)
				{
					num3 = Key.CompareRecords(_array[num + 1], oldRecord);
				}
				if ((num2 == 0) ^ (num3 == 0))
				{
					know_have_duplicates = (know_no_duplicates = false);
				}
			}
			Remove(num);
		}

		private void Remove(int index)
		{
			if (_size > 1)
			{
				Array.Copy(_array, index + 1, _array, index, _size - index - 1);
			}
			_size--;
		}

		internal void Update(DataRow row, int oldRecord, DataRowVersion oldVersion, DataRowState oldState)
		{
			bool flag = Key.ContainsVersion(oldState, oldVersion);
			int record = Key.GetRecord(row);
			if (oldRecord == -1 || _size == 0 || !flag)
			{
				if (record >= 0 && FindIndexExact(record) < 0)
				{
					Add(row, record);
				}
				return;
			}
			if (record < 0 || !Key.CanContain(record))
			{
				Delete(oldRecord);
				return;
			}
			int num = FindIndexExact(oldRecord);
			if (num == -1)
			{
				Add(row, record);
				return;
			}
			int num2 = -1;
			int num3 = Key.CompareRecords(_array[num], record);
			int num4 = 1;
			int num5 = 1;
			if (num3 == 0)
			{
				if (_array[num] == record)
				{
					return;
				}
			}
			else if (know_have_duplicates)
			{
				if (num > 0)
				{
					num4 = Key.CompareRecords(_array[num - 1], record);
				}
				if (num < _size - 1)
				{
					num5 = Key.CompareRecords(_array[num + 1], record);
				}
				if (((num4 == 0) ^ (num5 == 0)) && num3 != 0)
				{
					know_have_duplicates = (know_no_duplicates = false);
				}
			}
			if ((num == 0 && num3 > 0) || (num == _size - 1 && num3 < 0) || num3 == 0)
			{
				num2 = num;
			}
			else
			{
				int p;
				int r;
				if (num3 < 0)
				{
					p = num + 1;
					r = _size - 1;
				}
				else
				{
					p = 0;
					r = num - 1;
				}
				num2 = LazyBinarySearch(_array, p, r, record);
				if (num < num2)
				{
					Array.Copy(_array, num + 1, _array, num, num2 - num);
					if (Key.CompareRecords(_array[num2], record) > 0)
					{
						num2--;
					}
				}
				else if (num > num2)
				{
					Array.Copy(_array, num2, _array, num2 + 1, num - num2);
					if (Key.CompareRecords(_array[num2], record) < 0)
					{
						num2++;
					}
				}
			}
			_array[num2] = record;
			if (num3 != 0 && !know_have_duplicates)
			{
				if (num2 > 0)
				{
					num4 = Key.CompareRecords(_array[num2 - 1], record);
				}
				if (num2 < _size - 1)
				{
					num5 = Key.CompareRecords(_array[num2 + 1], record);
				}
				if (num4 == 0 || num5 == 0)
				{
					know_have_duplicates = true;
				}
			}
		}

		internal void Add(DataRow row)
		{
			Add(row, Key.GetRecord(row));
		}

		private void Add(DataRow row, int newRecord)
		{
			if (newRecord < 0 || !Key.CanContain(newRecord))
			{
				return;
			}
			int num;
			if (_size == 0)
			{
				num = 0;
			}
			else
			{
				num = LazyBinarySearch(_array, 0, _size - 1, newRecord);
				if (Key.CompareRecords(_array[num], newRecord) < 0)
				{
					num++;
				}
			}
			Insert(num, newRecord);
			int num2 = 1;
			int num3 = 1;
			if (!know_have_duplicates)
			{
				if (num > 0)
				{
					num2 = Key.CompareRecords(_array[num - 1], newRecord);
				}
				if (num < _size - 1)
				{
					num3 = Key.CompareRecords(_array[num + 1], newRecord);
				}
				if (num2 == 0 || num3 == 0)
				{
					know_have_duplicates = true;
				}
			}
		}

		private void Insert(int index, int r)
		{
			if (_array.Length == _size)
			{
				int[] array = ((_size != 0) ? new int[_size << 1] : new int[16]);
				Array.Copy(_array, 0, array, 0, index);
				array[index] = r;
				Array.Copy(_array, index, array, index + 1, _size - index);
				_array = array;
			}
			else
			{
				Array.Copy(_array, index, _array, index + 1, _size - index);
				_array[index] = r;
			}
			_size++;
		}

		private void MergeSort(int[] to, int length)
		{
			int[] array = new int[length];
			Array.Copy(to, 0, array, 0, array.Length);
			MergeSort(array, to, 0, array.Length);
		}

		private void MergeSort(int[] from, int[] to, int p, int r)
		{
			int num = p + r >> 1;
			if (num == p)
			{
				return;
			}
			MergeSort(to, from, p, num);
			MergeSort(to, from, num, r);
			int num2 = num;
			int num3 = p;
			while (true)
			{
				int num4 = Key.CompareRecords(from[p], from[num]);
				if (num4 > 0)
				{
					to[num3++] = from[num++];
					if (num == r)
					{
						while (p < num2)
						{
							to[num3++] = from[p++];
						}
						return;
					}
					continue;
				}
				if (num4 == 0)
				{
					know_have_duplicates = true;
				}
				to[num3++] = from[p++];
				if (p != num2)
				{
					continue;
				}
				break;
			}
			while (num < r)
			{
				to[num3++] = from[num++];
			}
		}

		private void QuickSort(int[] a, int p, int r)
		{
			if (p < r)
			{
				int num = Partition(a, p, r);
				QuickSort(a, p, num);
				QuickSort(a, num + 1, r);
			}
		}

		private int Partition(int[] a, int p, int r)
		{
			int second = a[p];
			int num = p - 1;
			int num2 = r + 1;
			while (true)
			{
				num2--;
				if (Key.CompareRecords(a[num2], second) <= 0)
				{
					do
					{
						num++;
					}
					while (Key.CompareRecords(a[num], second) < 0);
					if (num >= num2)
					{
						break;
					}
					int num3 = a[num2];
					a[num2] = a[num];
					a[num] = num3;
				}
			}
			return num2;
		}

		private int BinarySearch(int[] a, int p, int r, int b)
		{
			int num = LazyBinarySearch(a, p, r, b);
			return (Key.CompareRecords(a[num], b) != 0) ? (-1) : num;
		}

		private int LazyBinarySearch(int[] a, int p, int r, int b)
		{
			if (p == r)
			{
				return p;
			}
			int num = p + r >> 1;
			int num2 = Key.CompareRecords(a[num], b);
			if (num2 < 0)
			{
				return LazyBinarySearch(a, num + 1, r, b);
			}
			if (num2 > 0)
			{
				return LazyBinarySearch(a, p, num, b);
			}
			return num;
		}

		internal void AddRef()
		{
			_refCount++;
		}

		internal void RemoveRef()
		{
			_refCount--;
		}
	}
}
