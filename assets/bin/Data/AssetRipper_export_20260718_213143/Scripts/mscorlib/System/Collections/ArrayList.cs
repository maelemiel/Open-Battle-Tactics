using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[Serializable]
	[DebuggerDisplay("Count={Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
	[ComVisible(true)]
	public class ArrayList : IEnumerable, ICloneable, ICollection, IList
	{
		private sealed class ArrayListEnumerator : IEnumerator, ICloneable
		{
			private int m_Pos;

			private int m_Index;

			private int m_Count;

			private object m_Current;

			private ArrayList m_List;

			private int m_ExpectedStateChanges;

			public object Current
			{
				get
				{
					if (m_Pos == m_Index - 1)
					{
						throw new InvalidOperationException("Enumerator unusable (Reset pending, or past end of array.");
					}
					return m_Current;
				}
			}

			public ArrayListEnumerator(ArrayList list)
				: this(list, 0, list.Count)
			{
			}

			public ArrayListEnumerator(ArrayList list, int index, int count)
			{
				m_List = list;
				m_Index = index;
				m_Count = count;
				m_Pos = m_Index - 1;
				m_Current = null;
				m_ExpectedStateChanges = list._version;
			}

			public object Clone()
			{
				return MemberwiseClone();
			}

			public bool MoveNext()
			{
				if (m_List._version != m_ExpectedStateChanges)
				{
					throw new InvalidOperationException("List has changed.");
				}
				m_Pos++;
				if (m_Pos - m_Index < m_Count)
				{
					m_Current = m_List[m_Pos];
					return true;
				}
				return false;
			}

			public void Reset()
			{
				m_Current = null;
				m_Pos = m_Index - 1;
			}
		}

		private sealed class SimpleEnumerator : IEnumerator, ICloneable
		{
			private ArrayList list;

			private int index;

			private int version;

			private object currentElement;

			private static object endFlag = new object();

			public object Current
			{
				get
				{
					if (currentElement == endFlag)
					{
						if (index == -1)
						{
							throw new InvalidOperationException("Enumerator not started");
						}
						throw new InvalidOperationException("Enumerator ended");
					}
					return currentElement;
				}
			}

			public SimpleEnumerator(ArrayList list)
			{
				this.list = list;
				index = -1;
				version = list._version;
				currentElement = endFlag;
			}

			public object Clone()
			{
				return MemberwiseClone();
			}

			public bool MoveNext()
			{
				if (version != list._version)
				{
					throw new InvalidOperationException("List has changed.");
				}
				if (++index < list.Count)
				{
					currentElement = list[index];
					return true;
				}
				currentElement = endFlag;
				return false;
			}

			public void Reset()
			{
				if (version != list._version)
				{
					throw new InvalidOperationException("List has changed.");
				}
				currentElement = endFlag;
				index = -1;
			}
		}

		[Serializable]
		private sealed class ArrayListAdapter : ArrayList
		{
			private sealed class EnumeratorWithRange : IEnumerator, ICloneable
			{
				private int m_StartIndex;

				private int m_Count;

				private int m_MaxCount;

				private IEnumerator m_Enumerator;

				public object Current
				{
					get
					{
						return m_Enumerator.Current;
					}
				}

				public EnumeratorWithRange(IEnumerator enumerator, int index, int count)
				{
					m_Count = 0;
					m_StartIndex = index;
					m_MaxCount = count;
					m_Enumerator = enumerator;
					Reset();
				}

				public object Clone()
				{
					return MemberwiseClone();
				}

				public bool MoveNext()
				{
					if (m_Count >= m_MaxCount)
					{
						return false;
					}
					m_Count++;
					return m_Enumerator.MoveNext();
				}

				public void Reset()
				{
					m_Count = 0;
					m_Enumerator.Reset();
					for (int i = 0; i < m_StartIndex; i++)
					{
						m_Enumerator.MoveNext();
					}
				}
			}

			private IList m_Adaptee;

			public override object this[int index]
			{
				get
				{
					return m_Adaptee[index];
				}
				set
				{
					m_Adaptee[index] = value;
				}
			}

			public override int Count
			{
				get
				{
					return m_Adaptee.Count;
				}
			}

			public override int Capacity
			{
				get
				{
					return m_Adaptee.Count;
				}
				set
				{
					if (value < m_Adaptee.Count)
					{
						throw new ArgumentException("capacity");
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return m_Adaptee.IsFixedSize;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return m_Adaptee.IsReadOnly;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return m_Adaptee.SyncRoot;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return m_Adaptee.IsSynchronized;
				}
			}

			public ArrayListAdapter(IList adaptee)
				: base(0, true)
			{
				m_Adaptee = adaptee;
			}

			public override int Add(object value)
			{
				return m_Adaptee.Add(value);
			}

			public override void Clear()
			{
				m_Adaptee.Clear();
			}

			public override bool Contains(object value)
			{
				return m_Adaptee.Contains(value);
			}

			public override int IndexOf(object value)
			{
				return m_Adaptee.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return IndexOf(value, startIndex, m_Adaptee.Count - startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > m_Adaptee.Count)
				{
					ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "Does not specify valid index.");
				}
				if (count < 0)
				{
					ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
				}
				if (startIndex > m_Adaptee.Count - count)
				{
					throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
				}
				if (value == null)
				{
					for (int i = startIndex; i < startIndex + count; i++)
					{
						if (m_Adaptee[i] == null)
						{
							return i;
						}
					}
				}
				else
				{
					for (int j = startIndex; j < startIndex + count; j++)
					{
						if (value.Equals(m_Adaptee[j]))
						{
							return j;
						}
					}
				}
				return -1;
			}

			public override int LastIndexOf(object value)
			{
				return LastIndexOf(value, m_Adaptee.Count - 1);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return LastIndexOf(value, startIndex, startIndex + 1);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0)
				{
					ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "< 0");
				}
				if (count < 0)
				{
					ThrowNewArgumentOutOfRangeException("count", count, "count is negative.");
				}
				if (startIndex - count + 1 < 0)
				{
					ThrowNewArgumentOutOfRangeException("count", count, "count is too large.");
				}
				if (value == null)
				{
					for (int num = startIndex; num > startIndex - count; num--)
					{
						if (m_Adaptee[num] == null)
						{
							return num;
						}
					}
				}
				else
				{
					for (int num2 = startIndex; num2 > startIndex - count; num2--)
					{
						if (value.Equals(m_Adaptee[num2]))
						{
							return num2;
						}
					}
				}
				return -1;
			}

			public override void Insert(int index, object value)
			{
				m_Adaptee.Insert(index, value);
			}

			public override void InsertRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException("c");
				}
				if (index > m_Adaptee.Count)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				foreach (object item in c)
				{
					m_Adaptee.Insert(index++, item);
				}
			}

			public override void Remove(object value)
			{
				m_Adaptee.Remove(value);
			}

			public override void RemoveAt(int index)
			{
				m_Adaptee.RemoveAt(index);
			}

			public override void RemoveRange(int index, int count)
			{
				CheckRange(index, count, m_Adaptee.Count);
				for (int i = 0; i < count; i++)
				{
					m_Adaptee.RemoveAt(index);
				}
			}

			public override void Reverse()
			{
				Reverse(0, m_Adaptee.Count);
			}

			public override void Reverse(int index, int count)
			{
				CheckRange(index, count, m_Adaptee.Count);
				for (int i = 0; i < count / 2; i++)
				{
					object value = m_Adaptee[i + index];
					m_Adaptee[i + index] = m_Adaptee[index + count - i + index - 1];
					m_Adaptee[index + count - i + index - 1] = value;
				}
			}

			public override void SetRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException("c");
				}
				if (index < 0 || index + c.Count > m_Adaptee.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				int num = index;
				foreach (object item in c)
				{
					m_Adaptee[num++] = item;
				}
			}

			public override void CopyTo(Array array)
			{
				m_Adaptee.CopyTo(array, 0);
			}

			public override void CopyTo(Array array, int index)
			{
				m_Adaptee.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				if (index < 0)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Can't be less than zero.");
				}
				if (arrayIndex < 0)
				{
					ThrowNewArgumentOutOfRangeException("arrayIndex", arrayIndex, "Can't be less than zero.");
				}
				if (count < 0)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Can't be less than zero.");
				}
				if (index >= m_Adaptee.Count)
				{
					throw new ArgumentException("Can't be more or equal to list count.", "index");
				}
				if (array.Rank > 1)
				{
					throw new ArgumentException("Can't copy into multi-dimensional array.");
				}
				if (arrayIndex >= array.Length)
				{
					throw new ArgumentException("arrayIndex can't be greater than array.Length - 1.");
				}
				if (array.Length - arrayIndex + 1 < count)
				{
					throw new ArgumentException("Destination array is too small.");
				}
				if (index > m_Adaptee.Count - count)
				{
					throw new ArgumentException("Index and count do not denote a valid range of elements.", "index");
				}
				for (int i = 0; i < count; i++)
				{
					array.SetValue(m_Adaptee[index + i], arrayIndex + i);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				return m_Adaptee.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				CheckRange(index, count, m_Adaptee.Count);
				return new EnumeratorWithRange(m_Adaptee.GetEnumerator(), index, count);
			}

			public override void AddRange(ICollection c)
			{
				foreach (object item in c)
				{
					m_Adaptee.Add(item);
				}
			}

			public override int BinarySearch(object value)
			{
				return BinarySearch(value, null);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return BinarySearch(0, m_Adaptee.Count, value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				CheckRange(index, count, m_Adaptee.Count);
				if (comparer == null)
				{
					comparer = Comparer.Default;
				}
				int num = index;
				int num2 = index + count - 1;
				while (num <= num2)
				{
					int num3 = num + (num2 - num) / 2;
					int num4 = comparer.Compare(value, m_Adaptee[num3]);
					if (num4 < 0)
					{
						num2 = num3 - 1;
						continue;
					}
					if (num4 > 0)
					{
						num = num3 + 1;
						continue;
					}
					return num3;
				}
				return ~num;
			}

			public override object Clone()
			{
				return new ArrayListAdapter(m_Adaptee);
			}

			public override ArrayList GetRange(int index, int count)
			{
				CheckRange(index, count, m_Adaptee.Count);
				return new RangedArrayList(this, index, count);
			}

			public override void TrimToSize()
			{
			}

			public override void Sort()
			{
				Sort(Comparer.Default);
			}

			public override void Sort(IComparer comparer)
			{
				Sort(0, m_Adaptee.Count, comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				CheckRange(index, count, m_Adaptee.Count);
				if (comparer == null)
				{
					comparer = Comparer.Default;
				}
				QuickSort(m_Adaptee, index, index + count - 1, comparer);
			}

			private static void Swap(IList list, int x, int y)
			{
				object value = list[x];
				list[x] = list[y];
				list[y] = value;
			}

			internal static void QuickSort(IList list, int left, int right, IComparer comparer)
			{
				if (left >= right)
				{
					return;
				}
				int num = left + (right - left) / 2;
				if (comparer.Compare(list[num], list[left]) < 0)
				{
					Swap(list, num, left);
				}
				if (comparer.Compare(list[right], list[left]) < 0)
				{
					Swap(list, right, left);
				}
				if (comparer.Compare(list[right], list[num]) < 0)
				{
					Swap(list, right, num);
				}
				if (right - left + 1 <= 3)
				{
					return;
				}
				Swap(list, right - 1, num);
				object y = list[right - 1];
				int num2 = left;
				int num3 = right - 1;
				while (true)
				{
					if (comparer.Compare(list[++num2], y) >= 0)
					{
						while (comparer.Compare(list[--num3], y) > 0)
						{
						}
						if (num2 >= num3)
						{
							break;
						}
						Swap(list, num2, num3);
					}
				}
				Swap(list, right - 1, num2);
				QuickSort(list, left, num2 - 1, comparer);
				QuickSort(list, num2 + 1, right, comparer);
			}

			public override object[] ToArray()
			{
				object[] array = new object[m_Adaptee.Count];
				m_Adaptee.CopyTo(array, 0);
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				Array array = Array.CreateInstance(elementType, m_Adaptee.Count);
				m_Adaptee.CopyTo(array, 0);
				return array;
			}
		}

		[Serializable]
		private class ArrayListWrapper : ArrayList
		{
			protected ArrayList m_InnerArrayList;

			public override object this[int index]
			{
				get
				{
					return m_InnerArrayList[index];
				}
				set
				{
					m_InnerArrayList[index] = value;
				}
			}

			public override int Count
			{
				get
				{
					return m_InnerArrayList.Count;
				}
			}

			public override int Capacity
			{
				get
				{
					return m_InnerArrayList.Capacity;
				}
				set
				{
					m_InnerArrayList.Capacity = value;
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return m_InnerArrayList.IsFixedSize;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return m_InnerArrayList.IsReadOnly;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return m_InnerArrayList.IsSynchronized;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return m_InnerArrayList.SyncRoot;
				}
			}

			public ArrayListWrapper(ArrayList innerArrayList)
			{
				m_InnerArrayList = innerArrayList;
			}

			public override int Add(object value)
			{
				return m_InnerArrayList.Add(value);
			}

			public override void Clear()
			{
				m_InnerArrayList.Clear();
			}

			public override bool Contains(object value)
			{
				return m_InnerArrayList.Contains(value);
			}

			public override int IndexOf(object value)
			{
				return m_InnerArrayList.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return m_InnerArrayList.IndexOf(value, startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				return m_InnerArrayList.IndexOf(value, startIndex, count);
			}

			public override int LastIndexOf(object value)
			{
				return m_InnerArrayList.LastIndexOf(value);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return m_InnerArrayList.LastIndexOf(value, startIndex);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				return m_InnerArrayList.LastIndexOf(value, startIndex, count);
			}

			public override void Insert(int index, object value)
			{
				m_InnerArrayList.Insert(index, value);
			}

			public override void InsertRange(int index, ICollection c)
			{
				m_InnerArrayList.InsertRange(index, c);
			}

			public override void Remove(object value)
			{
				m_InnerArrayList.Remove(value);
			}

			public override void RemoveAt(int index)
			{
				m_InnerArrayList.RemoveAt(index);
			}

			public override void RemoveRange(int index, int count)
			{
				m_InnerArrayList.RemoveRange(index, count);
			}

			public override void Reverse()
			{
				m_InnerArrayList.Reverse();
			}

			public override void Reverse(int index, int count)
			{
				m_InnerArrayList.Reverse(index, count);
			}

			public override void SetRange(int index, ICollection c)
			{
				m_InnerArrayList.SetRange(index, c);
			}

			public override void CopyTo(Array array)
			{
				m_InnerArrayList.CopyTo(array);
			}

			public override void CopyTo(Array array, int index)
			{
				m_InnerArrayList.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return m_InnerArrayList.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				return m_InnerArrayList.GetEnumerator(index, count);
			}

			public override void AddRange(ICollection c)
			{
				m_InnerArrayList.AddRange(c);
			}

			public override int BinarySearch(object value)
			{
				return m_InnerArrayList.BinarySearch(value);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return m_InnerArrayList.BinarySearch(value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				return m_InnerArrayList.BinarySearch(index, count, value, comparer);
			}

			public override object Clone()
			{
				return m_InnerArrayList.Clone();
			}

			public override ArrayList GetRange(int index, int count)
			{
				return m_InnerArrayList.GetRange(index, count);
			}

			public override void TrimToSize()
			{
				m_InnerArrayList.TrimToSize();
			}

			public override void Sort()
			{
				m_InnerArrayList.Sort();
			}

			public override void Sort(IComparer comparer)
			{
				m_InnerArrayList.Sort(comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				m_InnerArrayList.Sort(index, count, comparer);
			}

			public override object[] ToArray()
			{
				return m_InnerArrayList.ToArray();
			}

			public override Array ToArray(Type elementType)
			{
				return m_InnerArrayList.ToArray(elementType);
			}
		}

		[Serializable]
		private sealed class SynchronizedArrayListWrapper : ArrayListWrapper
		{
			private object m_SyncRoot;

			public override object this[int index]
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerArrayList[index];
					}
				}
				set
				{
					lock (m_SyncRoot)
					{
						m_InnerArrayList[index] = value;
					}
				}
			}

			public override int Count
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerArrayList.Count;
					}
				}
			}

			public override int Capacity
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerArrayList.Capacity;
					}
				}
				set
				{
					lock (m_SyncRoot)
					{
						m_InnerArrayList.Capacity = value;
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerArrayList.IsFixedSize;
					}
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerArrayList.IsReadOnly;
					}
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object SyncRoot
			{
				get
				{
					return m_SyncRoot;
				}
			}

			internal SynchronizedArrayListWrapper(ArrayList innerArrayList)
				: base(innerArrayList)
			{
				m_SyncRoot = innerArrayList.SyncRoot;
			}

			public override int Add(object value)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.Add(value);
				}
			}

			public override void Clear()
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Clear();
				}
			}

			public override bool Contains(object value)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.Contains(value);
				}
			}

			public override int IndexOf(object value)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.IndexOf(value);
				}
			}

			public override int IndexOf(object value, int startIndex)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.IndexOf(value, startIndex);
				}
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.IndexOf(value, startIndex, count);
				}
			}

			public override int LastIndexOf(object value)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.LastIndexOf(value);
				}
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.LastIndexOf(value, startIndex);
				}
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.LastIndexOf(value, startIndex, count);
				}
			}

			public override void Insert(int index, object value)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Insert(index, value);
				}
			}

			public override void InsertRange(int index, ICollection c)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.InsertRange(index, c);
				}
			}

			public override void Remove(object value)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.RemoveAt(index);
				}
			}

			public override void RemoveRange(int index, int count)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.RemoveRange(index, count);
				}
			}

			public override void Reverse()
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Reverse();
				}
			}

			public override void Reverse(int index, int count)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Reverse(index, count);
				}
			}

			public override void CopyTo(Array array)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.CopyTo(array);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.CopyTo(array, index);
				}
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.GetEnumerator();
				}
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.GetEnumerator(index, count);
				}
			}

			public override void AddRange(ICollection c)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.AddRange(c);
				}
			}

			public override int BinarySearch(object value)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.BinarySearch(value);
				}
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.BinarySearch(value, comparer);
				}
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.BinarySearch(index, count, value, comparer);
				}
			}

			public override object Clone()
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.Clone();
				}
			}

			public override ArrayList GetRange(int index, int count)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.GetRange(index, count);
				}
			}

			public override void TrimToSize()
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.TrimToSize();
				}
			}

			public override void Sort()
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Sort();
				}
			}

			public override void Sort(IComparer comparer)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Sort(comparer);
				}
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				lock (m_SyncRoot)
				{
					m_InnerArrayList.Sort(index, count, comparer);
				}
			}

			public override object[] ToArray()
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.ToArray();
				}
			}

			public override Array ToArray(Type elementType)
			{
				lock (m_SyncRoot)
				{
					return m_InnerArrayList.ToArray(elementType);
				}
			}
		}

		[Serializable]
		private class FixedSizeArrayListWrapper : ArrayListWrapper
		{
			protected virtual string ErrorMessage
			{
				get
				{
					return "Can't add or remove from a fixed-size list.";
				}
			}

			public override int Capacity
			{
				get
				{
					return base.Capacity;
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public FixedSizeArrayListWrapper(ArrayList innerList)
				: base(innerList)
			{
			}

			public override int Add(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void AddRange(ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Clear()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Insert(int index, object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void InsertRange(int index, ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class ReadOnlyArrayListWrapper : FixedSizeArrayListWrapper
		{
			protected override string ErrorMessage
			{
				get
				{
					return "Can't modify a readonly list.";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override object this[int index]
			{
				get
				{
					return m_InnerArrayList[index];
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public ReadOnlyArrayListWrapper(ArrayList innerArrayList)
				: base(innerArrayList)
			{
			}

			public override void Reverse()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Reverse(int index, int count)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void SetRange(int index, ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort(IComparer comparer)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class RangedArrayList : ArrayListWrapper
		{
			private int m_InnerIndex;

			private int m_InnerCount;

			private int m_InnerStateChanges;

			public override bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

			public override object this[int index]
			{
				get
				{
					if (index < 0 || index > m_InnerCount)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return m_InnerArrayList[m_InnerIndex + index];
				}
				set
				{
					if (index < 0 || index > m_InnerCount)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					m_InnerArrayList[m_InnerIndex + index] = value;
				}
			}

			public override int Count
			{
				get
				{
					VerifyStateChanges();
					return m_InnerCount;
				}
			}

			public override int Capacity
			{
				get
				{
					return m_InnerArrayList.Capacity;
				}
				set
				{
					if (value < m_InnerCount)
					{
						throw new ArgumentOutOfRangeException();
					}
				}
			}

			public RangedArrayList(ArrayList innerList, int index, int count)
				: base(innerList)
			{
				m_InnerIndex = index;
				m_InnerCount = count;
				m_InnerStateChanges = innerList._version;
			}

			private void VerifyStateChanges()
			{
				if (m_InnerStateChanges != m_InnerArrayList._version)
				{
					throw new InvalidOperationException("ArrayList view is invalid because the underlying ArrayList was modified.");
				}
			}

			public override int Add(object value)
			{
				VerifyStateChanges();
				m_InnerArrayList.Insert(m_InnerIndex + m_InnerCount, value);
				m_InnerStateChanges = m_InnerArrayList._version;
				return ++m_InnerCount;
			}

			public override void Clear()
			{
				VerifyStateChanges();
				m_InnerArrayList.RemoveRange(m_InnerIndex, m_InnerCount);
				m_InnerCount = 0;
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override bool Contains(object value)
			{
				return m_InnerArrayList.Contains(value, m_InnerIndex, m_InnerCount);
			}

			public override int IndexOf(object value)
			{
				return IndexOf(value, 0);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return IndexOf(value, startIndex, m_InnerCount - startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > m_InnerCount)
				{
					ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "Does not specify valid index.");
				}
				if (count < 0)
				{
					ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
				}
				if (startIndex > m_InnerCount - count)
				{
					throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
				}
				int num = m_InnerArrayList.IndexOf(value, m_InnerIndex + startIndex, count);
				if (num == -1)
				{
					return -1;
				}
				return num - m_InnerIndex;
			}

			public override int LastIndexOf(object value)
			{
				return LastIndexOf(value, m_InnerCount - 1);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return LastIndexOf(value, startIndex, startIndex + 1);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0)
				{
					ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "< 0");
				}
				if (count < 0)
				{
					ThrowNewArgumentOutOfRangeException("count", count, "count is negative.");
				}
				int num = m_InnerArrayList.LastIndexOf(value, m_InnerIndex + startIndex, count);
				if (num == -1)
				{
					return -1;
				}
				return num - m_InnerIndex;
			}

			public override void Insert(int index, object value)
			{
				VerifyStateChanges();
				if (index < 0 || index > m_InnerCount)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.Insert(m_InnerIndex + index, value);
				m_InnerCount++;
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override void InsertRange(int index, ICollection c)
			{
				VerifyStateChanges();
				if (index < 0 || index > m_InnerCount)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.InsertRange(m_InnerIndex + index, c);
				m_InnerCount += c.Count;
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override void Remove(object value)
			{
				VerifyStateChanges();
				int num = IndexOf(value);
				if (num > -1)
				{
					RemoveAt(num);
				}
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override void RemoveAt(int index)
			{
				VerifyStateChanges();
				if (index < 0 || index > m_InnerCount)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.RemoveAt(m_InnerIndex + index);
				m_InnerCount--;
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override void RemoveRange(int index, int count)
			{
				VerifyStateChanges();
				CheckRange(index, count, m_InnerCount);
				m_InnerArrayList.RemoveRange(m_InnerIndex + index, count);
				m_InnerCount -= count;
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override void Reverse()
			{
				Reverse(0, m_InnerCount);
			}

			public override void Reverse(int index, int count)
			{
				VerifyStateChanges();
				CheckRange(index, count, m_InnerCount);
				m_InnerArrayList.Reverse(m_InnerIndex + index, count);
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override void SetRange(int index, ICollection c)
			{
				VerifyStateChanges();
				if (index < 0 || index > m_InnerCount)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.SetRange(m_InnerIndex + index, c);
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override void CopyTo(Array array)
			{
				CopyTo(array, 0);
			}

			public override void CopyTo(Array array, int index)
			{
				CopyTo(0, array, index, m_InnerCount);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				CheckRange(index, count, m_InnerCount);
				m_InnerArrayList.CopyTo(m_InnerIndex + index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return GetEnumerator(0, m_InnerCount);
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				CheckRange(index, count, m_InnerCount);
				return m_InnerArrayList.GetEnumerator(m_InnerIndex + index, count);
			}

			public override void AddRange(ICollection c)
			{
				VerifyStateChanges();
				m_InnerArrayList.InsertRange(m_InnerCount, c);
				m_InnerCount += c.Count;
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override int BinarySearch(object value)
			{
				return BinarySearch(0, m_InnerCount, value, Comparer.Default);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return BinarySearch(0, m_InnerCount, value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				CheckRange(index, count, m_InnerCount);
				return m_InnerArrayList.BinarySearch(m_InnerIndex + index, count, value, comparer);
			}

			public override object Clone()
			{
				return new RangedArrayList((ArrayList)m_InnerArrayList.Clone(), m_InnerIndex, m_InnerCount);
			}

			public override ArrayList GetRange(int index, int count)
			{
				CheckRange(index, count, m_InnerCount);
				return new RangedArrayList(this, index, count);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException();
			}

			public override void Sort()
			{
				Sort(Comparer.Default);
			}

			public override void Sort(IComparer comparer)
			{
				Sort(0, m_InnerCount, comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				VerifyStateChanges();
				CheckRange(index, count, m_InnerCount);
				m_InnerArrayList.Sort(m_InnerIndex + index, count, comparer);
				m_InnerStateChanges = m_InnerArrayList._version;
			}

			public override object[] ToArray()
			{
				object[] array = new object[m_InnerCount];
				m_InnerArrayList.CopyTo(m_InnerIndex, array, 0, m_InnerCount);
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				Array array = Array.CreateInstance(elementType, m_InnerCount);
				m_InnerArrayList.CopyTo(m_InnerIndex, array, 0, m_InnerCount);
				return array;
			}
		}

		[Serializable]
		private sealed class SynchronizedListWrapper : ListWrapper
		{
			private object m_SyncRoot;

			public override int Count
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerList.Count;
					}
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object SyncRoot
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerList.SyncRoot;
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerList.IsFixedSize;
					}
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerList.IsReadOnly;
					}
				}
			}

			public override object this[int index]
			{
				get
				{
					lock (m_SyncRoot)
					{
						return m_InnerList[index];
					}
				}
				set
				{
					lock (m_SyncRoot)
					{
						m_InnerList[index] = value;
					}
				}
			}

			public SynchronizedListWrapper(IList innerList)
				: base(innerList)
			{
				m_SyncRoot = innerList.SyncRoot;
			}

			public override int Add(object value)
			{
				lock (m_SyncRoot)
				{
					return m_InnerList.Add(value);
				}
			}

			public override void Clear()
			{
				lock (m_SyncRoot)
				{
					m_InnerList.Clear();
				}
			}

			public override bool Contains(object value)
			{
				lock (m_SyncRoot)
				{
					return m_InnerList.Contains(value);
				}
			}

			public override int IndexOf(object value)
			{
				lock (m_SyncRoot)
				{
					return m_InnerList.IndexOf(value);
				}
			}

			public override void Insert(int index, object value)
			{
				lock (m_SyncRoot)
				{
					m_InnerList.Insert(index, value);
				}
			}

			public override void Remove(object value)
			{
				lock (m_SyncRoot)
				{
					m_InnerList.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (m_SyncRoot)
				{
					m_InnerList.RemoveAt(index);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				lock (m_SyncRoot)
				{
					m_InnerList.CopyTo(array, index);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				lock (m_SyncRoot)
				{
					return m_InnerList.GetEnumerator();
				}
			}
		}

		[Serializable]
		private class FixedSizeListWrapper : ListWrapper
		{
			protected virtual string ErrorMessage
			{
				get
				{
					return "List is fixed-size.";
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public FixedSizeListWrapper(IList innerList)
				: base(innerList)
			{
			}

			public override int Add(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Clear()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Insert(int index, object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class ReadOnlyListWrapper : FixedSizeListWrapper
		{
			protected override string ErrorMessage
			{
				get
				{
					return "List is read-only.";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override object this[int index]
			{
				get
				{
					return m_InnerList[index];
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public ReadOnlyListWrapper(IList innerList)
				: base(innerList)
			{
			}
		}

		[Serializable]
		private class ListWrapper : IEnumerable, ICollection, IList
		{
			protected IList m_InnerList;

			public virtual object this[int index]
			{
				get
				{
					return m_InnerList[index];
				}
				set
				{
					m_InnerList[index] = value;
				}
			}

			public virtual int Count
			{
				get
				{
					return m_InnerList.Count;
				}
			}

			public virtual bool IsSynchronized
			{
				get
				{
					return m_InnerList.IsSynchronized;
				}
			}

			public virtual object SyncRoot
			{
				get
				{
					return m_InnerList.SyncRoot;
				}
			}

			public virtual bool IsFixedSize
			{
				get
				{
					return m_InnerList.IsFixedSize;
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return m_InnerList.IsReadOnly;
				}
			}

			public ListWrapper(IList innerList)
			{
				m_InnerList = innerList;
			}

			public virtual int Add(object value)
			{
				return m_InnerList.Add(value);
			}

			public virtual void Clear()
			{
				m_InnerList.Clear();
			}

			public virtual bool Contains(object value)
			{
				return m_InnerList.Contains(value);
			}

			public virtual int IndexOf(object value)
			{
				return m_InnerList.IndexOf(value);
			}

			public virtual void Insert(int index, object value)
			{
				m_InnerList.Insert(index, value);
			}

			public virtual void Remove(object value)
			{
				m_InnerList.Remove(value);
			}

			public virtual void RemoveAt(int index)
			{
				m_InnerList.RemoveAt(index);
			}

			public virtual void CopyTo(Array array, int index)
			{
				m_InnerList.CopyTo(array, index);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return m_InnerList.GetEnumerator();
			}
		}

		private const int DefaultInitialCapacity = 4;

		private int _size;

		private object[] _items;

		private int _version;

		private static readonly object[] EmptyArray = new object[0];

		public virtual object this[int index]
		{
			get
			{
				if (index < 0 || index >= _size)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Index is less than 0 or more than or equal to the list count.");
				}
				return _items[index];
			}
			set
			{
				if (index < 0 || index >= _size)
				{
					ThrowNewArgumentOutOfRangeException("index", index, "Index is less than 0 or more than or equal to the list count.");
				}
				_items[index] = value;
				_version++;
			}
		}

		public virtual int Count
		{
			get
			{
				return _size;
			}
		}

		public virtual int Capacity
		{
			get
			{
				return _items.Length;
			}
			set
			{
				if (value < _size)
				{
					ThrowNewArgumentOutOfRangeException("Capacity", value, "Must be more than count.");
				}
				object[] array = new object[value];
				Array.Copy(_items, 0, array, 0, _size);
				_items = array;
			}
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public ArrayList()
		{
			_items = EmptyArray;
		}

		public ArrayList(ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			Array array = c as Array;
			if (array != null && array.Rank != 1)
			{
				throw new RankException();
			}
			_items = new object[c.Count];
			AddRange(c);
		}

		public ArrayList(int capacity)
		{
			if (capacity < 0)
			{
				ThrowNewArgumentOutOfRangeException("capacity", capacity, "The initial capacity can't be smaller than zero.");
			}
			if (capacity == 0)
			{
				capacity = 4;
			}
			_items = new object[capacity];
		}

		private ArrayList(int initialCapacity, bool forceZeroSize)
		{
			if (forceZeroSize)
			{
				_items = null;
				return;
			}
			throw new InvalidOperationException("Use ArrayList(int)");
		}

		private ArrayList(object[] array, int index, int count)
		{
			if (count == 0)
			{
				_items = new object[4];
			}
			else
			{
				_items = new object[count];
			}
			Array.Copy(array, index, _items, 0, count);
			_size = count;
		}

		private void EnsureCapacity(int count)
		{
			if (count > _items.Length)
			{
				int num = _items.Length << 1;
				if (num == 0)
				{
					num = 4;
				}
				while (num < count)
				{
					num <<= 1;
				}
				object[] array = new object[num];
				Array.Copy(_items, 0, array, 0, _items.Length);
				_items = array;
			}
		}

		private void Shift(int index, int count)
		{
			if (count > 0)
			{
				if (_size + count > _items.Length)
				{
					int num;
					for (num = ((_items.Length <= 0) ? 1 : (_items.Length << 1)); num < _size + count; num <<= 1)
					{
					}
					object[] array = new object[num];
					Array.Copy(_items, 0, array, 0, index);
					Array.Copy(_items, index, array, index + count, _size - index);
					_items = array;
				}
				else
				{
					Array.Copy(_items, index, _items, index + count, _size - index);
				}
			}
			else if (count < 0)
			{
				int num2 = index - count;
				Array.Copy(_items, num2, _items, index, _size - num2);
				Array.Clear(_items, _size + count, -count);
			}
		}

		public virtual int Add(object value)
		{
			if (_items.Length <= _size)
			{
				EnsureCapacity(_size + 1);
			}
			_items[_size] = value;
			_version++;
			return _size++;
		}

		public virtual void Clear()
		{
			Array.Clear(_items, 0, _size);
			_size = 0;
			_version++;
		}

		public virtual bool Contains(object item)
		{
			return IndexOf(item, 0, _size) > -1;
		}

		internal virtual bool Contains(object value, int startIndex, int count)
		{
			return IndexOf(value, startIndex, count) > -1;
		}

		public virtual int IndexOf(object value)
		{
			return IndexOf(value, 0);
		}

		public virtual int IndexOf(object value, int startIndex)
		{
			return IndexOf(value, startIndex, _size - startIndex);
		}

		public virtual int IndexOf(object value, int startIndex, int count)
		{
			if (startIndex < 0 || startIndex > _size)
			{
				ThrowNewArgumentOutOfRangeException("startIndex", startIndex, "Does not specify valid index.");
			}
			if (count < 0)
			{
				ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
			}
			if (startIndex > _size - count)
			{
				throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
			}
			return Array.IndexOf(_items, value, startIndex, count);
		}

		public virtual int LastIndexOf(object value)
		{
			return LastIndexOf(value, _size - 1);
		}

		public virtual int LastIndexOf(object value, int startIndex)
		{
			return LastIndexOf(value, startIndex, startIndex + 1);
		}

		public virtual int LastIndexOf(object value, int startIndex, int count)
		{
			return Array.LastIndexOf(_items, value, startIndex, count);
		}

		public virtual void Insert(int index, object value)
		{
			if (index < 0 || index > _size)
			{
				ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
			}
			Shift(index, 1);
			_items[index] = value;
			_size++;
			_version++;
		}

		public virtual void InsertRange(int index, ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (index < 0 || index > _size)
			{
				ThrowNewArgumentOutOfRangeException("index", index, "Index must be >= 0 and <= Count.");
			}
			int count = c.Count;
			if (_items.Length < _size + count)
			{
				EnsureCapacity(_size + count);
			}
			if (index < _size)
			{
				Array.Copy(_items, index, _items, index + count, _size - index);
			}
			if (this == c.SyncRoot)
			{
				Array.Copy(_items, 0, _items, index, index);
				Array.Copy(_items, index + count, _items, index << 1, _size - index);
			}
			else
			{
				c.CopyTo(_items, index);
			}
			_size += c.Count;
			_version++;
		}

		public virtual void Remove(object obj)
		{
			int num = IndexOf(obj);
			if (num > -1)
			{
				RemoveAt(num);
			}
			_version++;
		}

		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= _size)
			{
				ThrowNewArgumentOutOfRangeException("index", index, "Less than 0 or more than list count.");
			}
			Shift(index, -1);
			_size--;
			_version++;
		}

		public virtual void RemoveRange(int index, int count)
		{
			CheckRange(index, count, _size);
			Shift(index, -count);
			_size -= count;
			_version++;
		}

		public virtual void Reverse()
		{
			Array.Reverse(_items, 0, _size);
			_version++;
		}

		public virtual void Reverse(int index, int count)
		{
			CheckRange(index, count, _size);
			Array.Reverse(_items, index, count);
			_version++;
		}

		public virtual void CopyTo(Array array)
		{
			Array.Copy(_items, array, _size);
		}

		public virtual void CopyTo(Array array, int arrayIndex)
		{
			CopyTo(0, array, arrayIndex, _size);
		}

		public virtual void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Must have only 1 dimensions.", "array");
			}
			Array.Copy(_items, index, array, arrayIndex, count);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new SimpleEnumerator(this);
		}

		public virtual IEnumerator GetEnumerator(int index, int count)
		{
			CheckRange(index, count, _size);
			return new ArrayListEnumerator(this, index, count);
		}

		public virtual void AddRange(ICollection c)
		{
			InsertRange(_size, c);
		}

		public virtual int BinarySearch(object value)
		{
			try
			{
				return Array.BinarySearch(_items, 0, _size, value);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual int BinarySearch(object value, IComparer comparer)
		{
			try
			{
				return Array.BinarySearch(_items, 0, _size, value, comparer);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual int BinarySearch(int index, int count, object value, IComparer comparer)
		{
			try
			{
				return Array.BinarySearch(_items, index, count, value, comparer);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual ArrayList GetRange(int index, int count)
		{
			CheckRange(index, count, _size);
			if (IsSynchronized)
			{
				return Synchronized(new RangedArrayList(this, index, count));
			}
			return new RangedArrayList(this, index, count);
		}

		public virtual void SetRange(int index, ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (index < 0 || index + c.Count > _size)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			c.CopyTo(_items, index);
			_version++;
		}

		public virtual void TrimToSize()
		{
			if (_items.Length > _size)
			{
				object[] array = ((_size != 0) ? new object[_size] : new object[4]);
				Array.Copy(_items, 0, array, 0, _size);
				_items = array;
			}
		}

		public virtual void Sort()
		{
			Array.Sort(_items, 0, _size);
			_version++;
		}

		public virtual void Sort(IComparer comparer)
		{
			Array.Sort(_items, 0, _size, comparer);
		}

		public virtual void Sort(int index, int count, IComparer comparer)
		{
			CheckRange(index, count, _size);
			Array.Sort(_items, index, count, comparer);
		}

		public virtual object[] ToArray()
		{
			object[] array = new object[_size];
			CopyTo(array);
			return array;
		}

		public virtual Array ToArray(Type type)
		{
			Array array = Array.CreateInstance(type, _size);
			CopyTo(array);
			return array;
		}

		public virtual object Clone()
		{
			return new ArrayList(_items, 0, _size);
		}

		internal static void CheckRange(int index, int count, int listCount)
		{
			if (index < 0)
			{
				ThrowNewArgumentOutOfRangeException("index", index, "Can't be less than 0.");
			}
			if (count < 0)
			{
				ThrowNewArgumentOutOfRangeException("count", count, "Can't be less than 0.");
			}
			if (index > listCount - count)
			{
				throw new ArgumentException("Index and count do not denote a valid range of elements.", "index");
			}
		}

		internal static void ThrowNewArgumentOutOfRangeException(string name, object actual, string message)
		{
			throw new ArgumentOutOfRangeException(name, actual, message);
		}

		public static ArrayList Adapter(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			ArrayList arrayList = list as ArrayList;
			if (arrayList != null)
			{
				return arrayList;
			}
			arrayList = new ArrayListAdapter(list);
			if (list.IsSynchronized)
			{
				return Synchronized(arrayList);
			}
			return arrayList;
		}

		public static ArrayList Synchronized(ArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsSynchronized)
			{
				return list;
			}
			return new SynchronizedArrayListWrapper(list);
		}

		public static IList Synchronized(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsSynchronized)
			{
				return list;
			}
			return new SynchronizedListWrapper(list);
		}

		public static ArrayList ReadOnly(ArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsReadOnly)
			{
				return list;
			}
			return new ReadOnlyArrayListWrapper(list);
		}

		public static IList ReadOnly(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsReadOnly)
			{
				return list;
			}
			return new ReadOnlyListWrapper(list);
		}

		public static ArrayList FixedSize(ArrayList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsFixedSize)
			{
				return list;
			}
			return new FixedSizeArrayListWrapper(list);
		}

		public static IList FixedSize(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsFixedSize)
			{
				return list;
			}
			return new FixedSizeListWrapper(list);
		}

		public static ArrayList Repeat(object value, int count)
		{
			ArrayList arrayList = new ArrayList(count);
			for (int i = 0; i < count; i++)
			{
				arrayList.Add(value);
			}
			return arrayList;
		}
	}
}
