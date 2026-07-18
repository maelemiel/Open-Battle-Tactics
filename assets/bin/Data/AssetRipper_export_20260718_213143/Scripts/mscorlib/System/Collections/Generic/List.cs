using System.Collections.ObjectModel;
using System.Diagnostics;

namespace System.Collections.Generic
{
	[Serializable]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView<>))]
	[DebuggerDisplay("Count={Count}")]
	public class List<T> : IEnumerable, ICollection, IList, ICollection<T>, IEnumerable<T>, IList<T>
	{
		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
		{
			private List<T> l;

			private int next;

			private int ver;

			private T current;

			object IEnumerator.Current
			{
				get
				{
					VerifyState();
					if (next <= 0)
					{
						throw new InvalidOperationException();
					}
					return current;
				}
			}

			public T Current
			{
				get
				{
					return current;
				}
			}

			internal Enumerator(List<T> l)
			{
				this.l = l;
				ver = l._version;
			}

			void IEnumerator.Reset()
			{
				VerifyState();
				next = 0;
			}

			public void Dispose()
			{
				l = null;
			}

			private void VerifyState()
			{
				if (l == null)
				{
					throw new ObjectDisposedException(GetType().FullName);
				}
				if (ver != l._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
			}

			public bool MoveNext()
			{
				VerifyState();
				if (next < 0)
				{
					return false;
				}
				if (next < l._size)
				{
					current = l._items[next++];
					return true;
				}
				next = -1;
				return false;
			}
		}

		private const int DefaultCapacity = 4;

		private T[] _items;

		private int _size;

		private int _version;

		private static readonly T[] EmptyArray = new T[0];

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				try
				{
					this[index] = (T)value;
					return;
				}
				catch (NullReferenceException)
				{
				}
				catch (InvalidCastException)
				{
				}
				throw new ArgumentException("value");
			}
		}

		public int Capacity
		{
			get
			{
				return _items.Length;
			}
			set
			{
				if ((uint)value < (uint)_size)
				{
					throw new ArgumentOutOfRangeException();
				}
				Array.Resize(ref _items, value);
			}
		}

		public int Count
		{
			get
			{
				return _size;
			}
		}

		public T this[int index]
		{
			get
			{
				if ((uint)index >= (uint)_size)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return _items[index];
			}
			set
			{
				CheckIndex(index);
				if (index == _size)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				_items[index] = value;
			}
		}

		public List()
		{
			_items = EmptyArray;
		}

		public List(IEnumerable<T> collection)
		{
			CheckCollection(collection);
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 == null)
			{
				_items = EmptyArray;
				AddEnumerable(collection);
			}
			else
			{
				_items = new T[collection2.Count];
				AddCollection(collection2);
			}
		}

		public List(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			_items = new T[capacity];
		}

		internal List(T[] data, int size)
		{
			_items = data;
			_size = size;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			Array.Copy(_items, 0, array, arrayIndex, _size);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		int IList.Add(object item)
		{
			try
			{
				Add((T)item);
				return _size - 1;
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			throw new ArgumentException("item");
		}

		bool IList.Contains(object item)
		{
			try
			{
				return Contains((T)item);
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			return false;
		}

		int IList.IndexOf(object item)
		{
			try
			{
				return IndexOf((T)item);
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			return -1;
		}

		void IList.Insert(int index, object item)
		{
			CheckIndex(index);
			try
			{
				Insert(index, (T)item);
				return;
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
			throw new ArgumentException("item");
		}

		void IList.Remove(object item)
		{
			try
			{
				Remove((T)item);
			}
			catch (NullReferenceException)
			{
			}
			catch (InvalidCastException)
			{
			}
		}

		public void Add(T item)
		{
			if (_size == _items.Length)
			{
				GrowIfNeeded(1);
			}
			_items[_size++] = item;
			_version++;
		}

		private void GrowIfNeeded(int newCount)
		{
			int num = _size + newCount;
			if (num > _items.Length)
			{
				Capacity = Math.Max(Math.Max(Capacity * 2, 4), num);
			}
		}

		private void CheckRange(int idx, int count)
		{
			if (idx < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((uint)(idx + count) > (uint)_size)
			{
				throw new ArgumentException("index and count exceed length of list");
			}
		}

		private void AddCollection(ICollection<T> collection)
		{
			int count = collection.Count;
			if (count != 0)
			{
				GrowIfNeeded(count);
				collection.CopyTo(_items, _size);
				_size += count;
			}
		}

		private void AddEnumerable(IEnumerable<T> enumerable)
		{
			foreach (T item in enumerable)
			{
				Add(item);
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			CheckCollection(collection);
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				AddCollection(collection2);
			}
			else
			{
				AddEnumerable(collection);
			}
			_version++;
		}

		public ReadOnlyCollection<T> AsReadOnly()
		{
			return new ReadOnlyCollection<T>(this);
		}

		public int BinarySearch(T item)
		{
			return Array.BinarySearch(_items, 0, _size, item);
		}

		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return Array.BinarySearch(_items, 0, _size, item, comparer);
		}

		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			CheckRange(index, count);
			return Array.BinarySearch(_items, index, count, item, comparer);
		}

		public void Clear()
		{
			Array.Clear(_items, 0, _items.Length);
			_size = 0;
			_version++;
		}

		public bool Contains(T item)
		{
			return Array.IndexOf(_items, item, 0, _size) != -1;
		}

		public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
		{
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			List<TOutput> list = new List<TOutput>(_size);
			for (int i = 0; i < _size; i++)
			{
				list._items[i] = converter(_items[i]);
			}
			list._size = _size;
			return list;
		}

		public void CopyTo(T[] array)
		{
			Array.Copy(_items, 0, array, 0, _size);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(_items, 0, array, arrayIndex, _size);
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			CheckRange(index, count);
			Array.Copy(_items, index, array, arrayIndex, count);
		}

		public bool Exists(Predicate<T> match)
		{
			CheckMatch(match);
			return GetIndex(0, _size, match) != -1;
		}

		public T Find(Predicate<T> match)
		{
			CheckMatch(match);
			int index = GetIndex(0, _size, match);
			return (index == -1) ? default(T) : _items[index];
		}

		private static void CheckMatch(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
		}

		public List<T> FindAll(Predicate<T> match)
		{
			CheckMatch(match);
			if (_size <= 65536)
			{
				return FindAllStackBits(match);
			}
			return FindAllList(match);
		}

		private unsafe List<T> FindAllStackBits(Predicate<T> match)
		{
			uint* ptr = stackalloc uint[_size / 32 + 1];
			uint* ptr2 = ptr;
			int num = 0;
			uint num2 = 2147483648u;
			for (int i = 0; i < _size; i++)
			{
				if (match(_items[i]))
				{
					*ptr2 |= num2;
					num++;
				}
				num2 >>= 1;
				if (num2 == 0)
				{
					ptr2++;
					num2 = 2147483648u;
				}
			}
			T[] array = new T[num];
			num2 = 2147483648u;
			ptr2 = ptr;
			int num3 = 0;
			for (int j = 0; j < _size; j++)
			{
				if (num3 >= num)
				{
					break;
				}
				if ((*ptr2 & num2) == num2)
				{
					array[num3++] = _items[j];
				}
				num2 >>= 1;
				if (num2 == 0)
				{
					ptr2++;
					num2 = 2147483648u;
				}
			}
			return new List<T>(array, num);
		}

		private List<T> FindAllList(Predicate<T> match)
		{
			List<T> list = new List<T>();
			for (int i = 0; i < _size; i++)
			{
				if (match(_items[i]))
				{
					list.Add(_items[i]);
				}
			}
			return list;
		}

		public int FindIndex(Predicate<T> match)
		{
			CheckMatch(match);
			return GetIndex(0, _size, match);
		}

		public int FindIndex(int startIndex, Predicate<T> match)
		{
			CheckMatch(match);
			CheckIndex(startIndex);
			return GetIndex(startIndex, _size - startIndex, match);
		}

		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			CheckMatch(match);
			CheckRange(startIndex, count);
			return GetIndex(startIndex, count, match);
		}

		private int GetIndex(int startIndex, int count, Predicate<T> match)
		{
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (match(_items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public T FindLast(Predicate<T> match)
		{
			CheckMatch(match);
			int lastIndex = GetLastIndex(0, _size, match);
			return (lastIndex != -1) ? this[lastIndex] : default(T);
		}

		public int FindLastIndex(Predicate<T> match)
		{
			CheckMatch(match);
			return GetLastIndex(0, _size, match);
		}

		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			CheckMatch(match);
			CheckIndex(startIndex);
			return GetLastIndex(0, startIndex + 1, match);
		}

		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			CheckMatch(match);
			int num = startIndex - count + 1;
			CheckRange(num, count);
			return GetLastIndex(num, count, match);
		}

		private int GetLastIndex(int startIndex, int count, Predicate<T> match)
		{
			int num = startIndex + count;
			while (num != startIndex)
			{
				if (match(_items[--num]))
				{
					return num;
				}
			}
			return -1;
		}

		public void ForEach(Action<T> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			for (int i = 0; i < _size; i++)
			{
				action(_items[i]);
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		public List<T> GetRange(int index, int count)
		{
			CheckRange(index, count);
			T[] array = new T[count];
			Array.Copy(_items, index, array, 0, count);
			return new List<T>(array, count);
		}

		public int IndexOf(T item)
		{
			return Array.IndexOf(_items, item, 0, _size);
		}

		public int IndexOf(T item, int index)
		{
			CheckIndex(index);
			return Array.IndexOf(_items, item, index, _size - index);
		}

		public int IndexOf(T item, int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((uint)(index + count) > (uint)_size)
			{
				throw new ArgumentOutOfRangeException("index and count exceed length of list");
			}
			return Array.IndexOf(_items, item, index, count);
		}

		private void Shift(int start, int delta)
		{
			if (delta < 0)
			{
				start -= delta;
			}
			if (start < _size)
			{
				Array.Copy(_items, start, _items, start + delta, _size - start);
			}
			_size += delta;
			if (delta < 0)
			{
				Array.Clear(_items, _size, -delta);
			}
		}

		private void CheckIndex(int index)
		{
			if (index < 0 || (uint)index > (uint)_size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public void Insert(int index, T item)
		{
			CheckIndex(index);
			if (_size == _items.Length)
			{
				GrowIfNeeded(1);
			}
			Shift(index, 1);
			_items[index] = item;
			_version++;
		}

		private void CheckCollection(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			CheckCollection(collection);
			CheckIndex(index);
			if (collection == this)
			{
				T[] array = new T[_size];
				CopyTo(array, 0);
				GrowIfNeeded(_size);
				Shift(index, array.Length);
				Array.Copy(array, 0, _items, index, array.Length);
			}
			else
			{
				ICollection<T> collection2 = collection as ICollection<T>;
				if (collection2 != null)
				{
					InsertCollection(index, collection2);
				}
				else
				{
					InsertEnumeration(index, collection);
				}
			}
			_version++;
		}

		private void InsertCollection(int index, ICollection<T> collection)
		{
			int count = collection.Count;
			GrowIfNeeded(count);
			Shift(index, count);
			collection.CopyTo(_items, index);
		}

		private void InsertEnumeration(int index, IEnumerable<T> enumerable)
		{
			foreach (T item in enumerable)
			{
				Insert(index++, item);
			}
		}

		public int LastIndexOf(T item)
		{
			return Array.LastIndexOf(_items, item, _size - 1, _size);
		}

		public int LastIndexOf(T item, int index)
		{
			CheckIndex(index);
			return Array.LastIndexOf(_items, item, index, index + 1);
		}

		public int LastIndexOf(T item, int index, int count)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "index is negative");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "count is negative");
			}
			if (index - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("cound", count, "count is too large");
			}
			return Array.LastIndexOf(_items, item, index, count);
		}

		public bool Remove(T item)
		{
			int num = IndexOf(item);
			if (num != -1)
			{
				RemoveAt(num);
			}
			return num != -1;
		}

		public int RemoveAll(Predicate<T> match)
		{
			CheckMatch(match);
			int num = 0;
			int num2 = 0;
			for (num = 0; num < _size && !match(_items[num]); num++)
			{
			}
			if (num == _size)
			{
				return 0;
			}
			_version++;
			for (num2 = num + 1; num2 < _size; num2++)
			{
				if (!match(_items[num2]))
				{
					_items[num++] = _items[num2];
				}
			}
			if (num2 - num > 0)
			{
				Array.Clear(_items, num, num2 - num);
			}
			_size = num;
			return num2 - num;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || (uint)index >= (uint)_size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			Shift(index, -1);
			Array.Clear(_items, _size, 1);
			_version++;
		}

		public void RemoveRange(int index, int count)
		{
			CheckRange(index, count);
			if (count > 0)
			{
				Shift(index, -count);
				Array.Clear(_items, _size, count);
				_version++;
			}
		}

		public void Reverse()
		{
			Array.Reverse(_items, 0, _size);
			_version++;
		}

		public void Reverse(int index, int count)
		{
			CheckRange(index, count);
			Array.Reverse(_items, index, count);
			_version++;
		}

		public void Sort()
		{
			Array.Sort(_items, 0, _size, Comparer<T>.Default);
			_version++;
		}

		public void Sort(IComparer<T> comparer)
		{
			Array.Sort(_items, 0, _size, comparer);
			_version++;
		}

		public void Sort(Comparison<T> comparison)
		{
			Array.Sort(_items, _size, comparison);
			_version++;
		}

		public void Sort(int index, int count, IComparer<T> comparer)
		{
			CheckRange(index, count);
			Array.Sort(_items, index, count, comparer);
			_version++;
		}

		public T[] ToArray()
		{
			T[] array = new T[_size];
			Array.Copy(_items, array, _size);
			return array;
		}

		public void TrimExcess()
		{
			Capacity = _size;
		}

		public bool TrueForAll(Predicate<T> match)
		{
			CheckMatch(match);
			for (int i = 0; i < _size; i++)
			{
				if (!match(_items[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
