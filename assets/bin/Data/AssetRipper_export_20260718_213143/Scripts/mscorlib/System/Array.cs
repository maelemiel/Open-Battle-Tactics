using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public abstract class Array : IEnumerable, ICloneable, ICollection, IList
	{
		internal struct InternalEnumerator<T> : IEnumerator, IDisposable, IEnumerator<T>
		{
			private const int NOT_STARTED = -2;

			private const int FINISHED = -1;

			private Array array;

			private int idx;

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public T Current
			{
				get
				{
					if (idx == -2)
					{
						throw new InvalidOperationException("Enumeration has not started. Call MoveNext");
					}
					if (idx == -1)
					{
						throw new InvalidOperationException("Enumeration already finished");
					}
					return array.InternalArray__get_Item<T>(array.Length - 1 - idx);
				}
			}

			internal InternalEnumerator(Array array)
			{
				this.array = array;
				idx = -2;
			}

			void IEnumerator.Reset()
			{
				idx = -2;
			}

			public void Dispose()
			{
				idx = -2;
			}

			public bool MoveNext()
			{
				if (idx == -2)
				{
					idx = array.Length;
				}
				return idx != -1 && --idx != -1;
			}
		}

		internal class SimpleEnumerator : IEnumerator, ICloneable
		{
			private Array enumeratee;

			private int currentpos;

			private int length;

			public object Current
			{
				get
				{
					if (currentpos < 0)
					{
						throw new InvalidOperationException(Locale.GetText("Enumeration has not started."));
					}
					if (currentpos >= length)
					{
						throw new InvalidOperationException(Locale.GetText("Enumeration has already ended"));
					}
					return enumeratee.GetValueImpl(currentpos);
				}
			}

			public SimpleEnumerator(Array arrayToEnumerate)
			{
				enumeratee = arrayToEnumerate;
				currentpos = -1;
				length = arrayToEnumerate.Length;
			}

			public bool MoveNext()
			{
				if (currentpos < length)
				{
					currentpos++;
				}
				if (currentpos < length)
				{
					return true;
				}
				return false;
			}

			public void Reset()
			{
				currentpos = -1;
			}

			public object Clone()
			{
				return MemberwiseClone();
			}
		}

		private class ArrayReadOnlyList<T> : IEnumerable, IList<T>, ICollection<T>, IEnumerable<T>
		{
			private T[] array;

			public T this[int index]
			{
				get
				{
					if ((uint)index >= (uint)array.Length)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return array[index];
				}
				set
				{
					throw ReadOnlyError();
				}
			}

			public int Count
			{
				get
				{
					return array.Length;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public ArrayReadOnlyList(T[] array)
			{
				this.array = array;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Add(T item)
			{
				throw ReadOnlyError();
			}

			public void Clear()
			{
				throw ReadOnlyError();
			}

			public bool Contains(T item)
			{
				return Array.IndexOf(array, item) >= 0;
			}

			public void CopyTo(T[] array, int index)
			{
				this.array.CopyTo(array, index);
			}

			public IEnumerator<T> GetEnumerator()
			{
				for (int i = 0; i < array.Length; i++)
				{
					yield return array[i];
				}
			}

			public int IndexOf(T item)
			{
				return Array.IndexOf(array, item);
			}

			public void Insert(int index, T item)
			{
				throw ReadOnlyError();
			}

			public bool Remove(T item)
			{
				throw ReadOnlyError();
			}

			public void RemoveAt(int index)
			{
				throw ReadOnlyError();
			}

			private static Exception ReadOnlyError()
			{
				return new NotSupportedException("This collection is read-only.");
			}
		}

		private delegate void Swapper(int i, int j);

		object IList.this[int index]
		{
			get
			{
				if ((uint)index >= (uint)Length)
				{
					throw new IndexOutOfRangeException("index");
				}
				if (Rank > 1)
				{
					throw new ArgumentException(Locale.GetText("Only single dimension arrays are supported."));
				}
				return GetValueImpl(index);
			}
			set
			{
				if ((uint)index >= (uint)Length)
				{
					throw new IndexOutOfRangeException("index");
				}
				if (Rank > 1)
				{
					throw new ArgumentException(Locale.GetText("Only single dimension arrays are supported."));
				}
				SetValueImpl(value, index);
			}
		}

		int ICollection.Count
		{
			get
			{
				return Length;
			}
		}

		public int Length
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				int num = GetLength(0);
				for (int i = 1; i < Rank; i++)
				{
					num *= GetLength(i);
				}
				return num;
			}
		}

		[ComVisible(false)]
		public long LongLength
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return Length;
			}
		}

		public int Rank
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return GetRank();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		private Array()
		{
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			Clear(this, GetLowerBound(0), Length);
		}

		bool IList.Contains(object value)
		{
			if (Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int length = Length;
			for (int i = 0; i < length; i++)
			{
				if (object.Equals(GetValueImpl(i), value))
				{
					return true;
				}
			}
			return false;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		int IList.IndexOf(object value)
		{
			if (Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int length = Length;
			for (int i = 0; i < length; i++)
			{
				if (object.Equals(GetValueImpl(i), value))
				{
					return i + GetLowerBound(0);
				}
			}
			return GetLowerBound(0) - 1;
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		internal int InternalArray__ICollection_get_Count()
		{
			return Length;
		}

		internal bool InternalArray__ICollection_get_IsReadOnly()
		{
			return true;
		}

		internal IEnumerator<T> InternalArray__IEnumerable_GetEnumerator<T>()
		{
			return new InternalEnumerator<T>(this);
		}

		internal void InternalArray__ICollection_Clear()
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal void InternalArray__ICollection_Add<T>(T item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal bool InternalArray__ICollection_Remove<T>(T item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal bool InternalArray__ICollection_Contains<T>(T item)
		{
			if (Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int length = Length;
			for (int i = 0; i < length; i++)
			{
				T value;
				GetGenericValueImpl<T>(i, out value);
				if (item == null)
				{
					if (value == null)
					{
						return true;
					}
					return false;
				}
				if (item.Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		internal void InternalArray__ICollection_CopyTo<T>(T[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index + GetLength(0) > array.GetLowerBound(0) + array.GetLength(0))
			{
				throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value has to be >= 0."));
			}
			Copy(this, GetLowerBound(0), array, index, GetLength(0));
		}

		internal void InternalArray__Insert<T>(int index, T item)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal void InternalArray__RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only");
		}

		internal int InternalArray__IndexOf<T>(T item)
		{
			if (Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int length = Length;
			for (int i = 0; i < length; i++)
			{
				T value;
				GetGenericValueImpl<T>(i, out value);
				if (item == null)
				{
					if (value == null)
					{
						return i + GetLowerBound(0);
					}
					return GetLowerBound(0) - 1;
				}
				if (value.Equals(item))
				{
					return i + GetLowerBound(0);
				}
			}
			return GetLowerBound(0) - 1;
		}

		internal T InternalArray__get_Item<T>(int index)
		{
			if ((uint)index >= (uint)Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			T value;
			GetGenericValueImpl<T>(index, out value);
			return value;
		}

		internal void InternalArray__set_Item<T>(int index, T item)
		{
			if ((uint)index >= (uint)Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			object[] array = this as object[];
			if (array != null)
			{
				array[index] = item;
			}
			else
			{
				SetGenericValueImpl(index, ref item);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetGenericValueImpl<T>(int pos, out T value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetGenericValueImpl<T>(int pos, ref T value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetRank();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetLength(int dimension);

		[ComVisible(false)]
		public long GetLongLength(int dimension)
		{
			return GetLength(dimension);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public extern int GetLowerBound(int dimension);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object GetValue(params int[] indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetValue(object value, params int[] indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern object GetValueImpl(int pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetValueImpl(object value, int pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool FastCopy(Array source, int source_idx, Array dest, int dest_idx, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Array CreateInstanceImpl(Type elementType, int[] lengths, int[] bounds);

		public IEnumerator GetEnumerator()
		{
			return new SimpleEnumerator(this);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public int GetUpperBound(int dimension)
		{
			return GetLowerBound(dimension) + GetLength(dimension) - 1;
		}

		public object GetValue(int index)
		{
			if (Rank != 1)
			{
				throw new ArgumentException(Locale.GetText("Array was not a one-dimensional array."));
			}
			if (index < GetLowerBound(0) || index > GetUpperBound(0))
			{
				throw new IndexOutOfRangeException(Locale.GetText("Index has to be between upper and lower bound of the array."));
			}
			return GetValueImpl(index - GetLowerBound(0));
		}

		public object GetValue(int index1, int index2)
		{
			int[] indices = new int[2] { index1, index2 };
			return GetValue(indices);
		}

		public object GetValue(int index1, int index2, int index3)
		{
			int[] indices = new int[3] { index1, index2, index3 };
			return GetValue(indices);
		}

		[ComVisible(false)]
		public object GetValue(long index)
		{
			if (index < 0 || index > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			return GetValue((int)index);
		}

		[ComVisible(false)]
		public object GetValue(long index1, long index2)
		{
			if (index1 < 0 || index1 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index1", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index2 < 0 || index2 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index2", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			return GetValue((int)index1, (int)index2);
		}

		[ComVisible(false)]
		public object GetValue(long index1, long index2, long index3)
		{
			if (index1 < 0 || index1 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index1", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index2 < 0 || index2 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index2", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index3 < 0 || index3 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index3", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			return GetValue((int)index1, (int)index2, (int)index3);
		}

		[ComVisible(false)]
		public void SetValue(object value, long index)
		{
			if (index < 0 || index > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			SetValue(value, (int)index);
		}

		[ComVisible(false)]
		public void SetValue(object value, long index1, long index2)
		{
			if (index1 < 0 || index1 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index1", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index2 < 0 || index2 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index2", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			int[] indices = new int[2]
			{
				(int)index1,
				(int)index2
			};
			SetValue(value, indices);
		}

		[ComVisible(false)]
		public void SetValue(object value, long index1, long index2, long index3)
		{
			if (index1 < 0 || index1 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index1", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index2 < 0 || index2 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index2", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			if (index3 < 0 || index3 > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index3", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			int[] indices = new int[3]
			{
				(int)index1,
				(int)index2,
				(int)index3
			};
			SetValue(value, indices);
		}

		public void SetValue(object value, int index)
		{
			if (Rank != 1)
			{
				throw new ArgumentException(Locale.GetText("Array was not a one-dimensional array."));
			}
			if (index < GetLowerBound(0) || index > GetUpperBound(0))
			{
				throw new IndexOutOfRangeException(Locale.GetText("Index has to be >= lower bound and <= upper bound of the array."));
			}
			SetValueImpl(value, index - GetLowerBound(0));
		}

		public void SetValue(object value, int index1, int index2)
		{
			int[] indices = new int[2] { index1, index2 };
			SetValue(value, indices);
		}

		public void SetValue(object value, int index1, int index2, int index3)
		{
			int[] indices = new int[3] { index1, index2, index3 };
			SetValue(value, indices);
		}

		public static Array CreateInstance(Type elementType, int length)
		{
			int[] lengths = new int[1] { length };
			return CreateInstance(elementType, lengths);
		}

		public static Array CreateInstance(Type elementType, int length1, int length2)
		{
			int[] lengths = new int[2] { length1, length2 };
			return CreateInstance(elementType, lengths);
		}

		public static Array CreateInstance(Type elementType, int length1, int length2, int length3)
		{
			int[] lengths = new int[3] { length1, length2, length3 };
			return CreateInstance(elementType, lengths);
		}

		public static Array CreateInstance(Type elementType, params int[] lengths)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (lengths == null)
			{
				throw new ArgumentNullException("lengths");
			}
			if (lengths.Length > 255)
			{
				throw new TypeLoadException();
			}
			int[] bounds = null;
			elementType = elementType.UnderlyingSystemType;
			if (!elementType.IsSystemType)
			{
				throw new ArgumentException("Type must be a type provided by the runtime.", "elementType");
			}
			if (elementType.Equals(typeof(void)))
			{
				throw new NotSupportedException("Array type can not be void");
			}
			if (elementType.ContainsGenericParameters)
			{
				throw new NotSupportedException("Array type can not be an open generic type");
			}
			return CreateInstanceImpl(elementType, lengths, bounds);
		}

		public static Array CreateInstance(Type elementType, int[] lengths, int[] lowerBounds)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}
			if (lengths == null)
			{
				throw new ArgumentNullException("lengths");
			}
			if (lowerBounds == null)
			{
				throw new ArgumentNullException("lowerBounds");
			}
			elementType = elementType.UnderlyingSystemType;
			if (!elementType.IsSystemType)
			{
				throw new ArgumentException("Type must be a type provided by the runtime.", "elementType");
			}
			if (elementType.Equals(typeof(void)))
			{
				throw new NotSupportedException("Array type can not be void");
			}
			if (elementType.ContainsGenericParameters)
			{
				throw new NotSupportedException("Array type can not be an open generic type");
			}
			if (lengths.Length < 1)
			{
				throw new ArgumentException(Locale.GetText("Arrays must contain >= 1 elements."));
			}
			if (lengths.Length != lowerBounds.Length)
			{
				throw new ArgumentException(Locale.GetText("Arrays must be of same size."));
			}
			for (int i = 0; i < lowerBounds.Length; i++)
			{
				if (lengths[i] < 0)
				{
					throw new ArgumentOutOfRangeException("lengths", Locale.GetText("Each value has to be >= 0."));
				}
				if ((long)lowerBounds[i] + (long)lengths[i] > int.MaxValue)
				{
					throw new ArgumentOutOfRangeException("lengths", Locale.GetText("Length + bound must not exceed Int32.MaxValue."));
				}
			}
			if (lengths.Length > 255)
			{
				throw new TypeLoadException();
			}
			return CreateInstanceImpl(elementType, lengths, lowerBounds);
		}

		private static int[] GetIntArray(long[] values)
		{
			int num = values.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				long num2 = values[i];
				if (num2 < 0 || num2 > int.MaxValue)
				{
					throw new ArgumentOutOfRangeException("values", Locale.GetText("Each value has to be >= 0 and <= Int32.MaxValue."));
				}
				array[i] = (int)num2;
			}
			return array;
		}

		public static Array CreateInstance(Type elementType, params long[] lengths)
		{
			if (lengths == null)
			{
				throw new ArgumentNullException("lengths");
			}
			return CreateInstance(elementType, GetIntArray(lengths));
		}

		[ComVisible(false)]
		public object GetValue(params long[] indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			return GetValue(GetIntArray(indices));
		}

		[ComVisible(false)]
		public void SetValue(object value, params long[] indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			SetValue(value, GetIntArray(indices));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch(Array array, object value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (value == null)
			{
				return -1;
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (array.Length == 0)
			{
				return -1;
			}
			if (!(value is IComparable))
			{
				throw new ArgumentException(Locale.GetText("value does not support IComparable."));
			}
			return DoBinarySearch(array, array.GetLowerBound(0), array.GetLength(0), value, null);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch(Array array, object value, IComparer comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (array.Length == 0)
			{
				return -1;
			}
			if (comparer == null && value != null && !(value is IComparable))
			{
				throw new ArgumentException(Locale.GetText("comparer is null and value does not support IComparable."));
			}
			return DoBinarySearch(array, array.GetLowerBound(0), array.GetLength(0), value, comparer);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch(Array array, int index, int length, object value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < array.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("index is less than the lower bound of array."));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (index > array.GetLowerBound(0) + array.GetLength(0) - length)
			{
				throw new ArgumentException(Locale.GetText("index and length do not specify a valid range in array."));
			}
			if (array.Length == 0)
			{
				return -1;
			}
			if (value != null && !(value is IComparable))
			{
				throw new ArgumentException(Locale.GetText("value does not support IComparable"));
			}
			return DoBinarySearch(array, index, length, value, null);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch(Array array, int index, int length, object value, IComparer comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < array.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("index is less than the lower bound of array."));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (index > array.GetLowerBound(0) + array.GetLength(0) - length)
			{
				throw new ArgumentException(Locale.GetText("index and length do not specify a valid range in array."));
			}
			if (array.Length == 0)
			{
				return -1;
			}
			if (comparer == null && value != null && !(value is IComparable))
			{
				throw new ArgumentException(Locale.GetText("comparer is null and value does not support IComparable."));
			}
			return DoBinarySearch(array, index, length, value, comparer);
		}

		private static int DoBinarySearch(Array array, int index, int length, object value, IComparer comparer)
		{
			if (comparer == null)
			{
				comparer = Comparer.Default;
			}
			int num = index;
			int num2 = index + length - 1;
			int num3 = 0;
			try
			{
				while (num <= num2)
				{
					int num4 = num + (num2 - num) / 2;
					object valueImpl = array.GetValueImpl(num4);
					num3 = comparer.Compare(valueImpl, value);
					if (num3 == 0)
					{
						return num4;
					}
					if (num3 > 0)
					{
						num2 = num4 - 1;
					}
					else
					{
						num = num4 + 1;
					}
				}
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(Locale.GetText("Comparer threw an exception."), innerException);
			}
			return ~num;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void Clear(Array array, int index, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (length < 0)
			{
				throw new IndexOutOfRangeException("length < 0");
			}
			int lowerBound = array.GetLowerBound(0);
			if (index < lowerBound)
			{
				throw new IndexOutOfRangeException("index < lower bound");
			}
			index -= lowerBound;
			if (index > array.Length - length)
			{
				throw new IndexOutOfRangeException("index + length > size");
			}
			ClearInternal(array, index, length);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearInternal(Array a, int index, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object Clone();

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Copy(Array sourceArray, Array destinationArray, int length)
		{
			if (sourceArray == null)
			{
				throw new ArgumentNullException("sourceArray");
			}
			if (destinationArray == null)
			{
				throw new ArgumentNullException("destinationArray");
			}
			Copy(sourceArray, sourceArray.GetLowerBound(0), destinationArray, destinationArray.GetLowerBound(0), length);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
		{
			if (sourceArray == null)
			{
				throw new ArgumentNullException("sourceArray");
			}
			if (destinationArray == null)
			{
				throw new ArgumentNullException("destinationArray");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (sourceIndex < 0)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", Locale.GetText("Value has to be >= 0."));
			}
			if (destinationIndex < 0)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", Locale.GetText("Value has to be >= 0."));
			}
			if (FastCopy(sourceArray, sourceIndex, destinationArray, destinationIndex, length))
			{
				return;
			}
			int num = sourceIndex - sourceArray.GetLowerBound(0);
			int num2 = destinationIndex - destinationArray.GetLowerBound(0);
			if (num > sourceArray.Length - length)
			{
				throw new ArgumentException("length");
			}
			if (num2 > destinationArray.Length - length)
			{
				string message = "Destination array was not long enough. Check destIndex and length, and the array's lower bounds";
				throw new ArgumentException(message, string.Empty);
			}
			if (sourceArray.Rank != destinationArray.Rank)
			{
				throw new RankException(Locale.GetText("Arrays must be of same size."));
			}
			Type elementType = sourceArray.GetType().GetElementType();
			Type elementType2 = destinationArray.GetType().GetElementType();
			if (!object.ReferenceEquals(sourceArray, destinationArray) || num > num2)
			{
				for (int i = 0; i < length; i++)
				{
					object valueImpl = sourceArray.GetValueImpl(num + i);
					try
					{
						destinationArray.SetValueImpl(valueImpl, num2 + i);
					}
					catch
					{
						if (elementType.Equals(typeof(object)))
						{
							throw new InvalidCastException();
						}
						throw new ArrayTypeMismatchException(string.Format(Locale.GetText("(Types: source={0};  target={1})"), elementType.FullName, elementType2.FullName));
					}
				}
				return;
			}
			for (int num3 = length - 1; num3 >= 0; num3--)
			{
				object valueImpl2 = sourceArray.GetValueImpl(num + num3);
				try
				{
					destinationArray.SetValueImpl(valueImpl2, num2 + num3);
				}
				catch
				{
					if (elementType.Equals(typeof(object)))
					{
						throw new InvalidCastException();
					}
					throw new ArrayTypeMismatchException(string.Format(Locale.GetText("(Types: source={0};  target={1})"), elementType.FullName, elementType2.FullName));
				}
			}
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Copy(Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length)
		{
			if (sourceArray == null)
			{
				throw new ArgumentNullException("sourceArray");
			}
			if (destinationArray == null)
			{
				throw new ArgumentNullException("destinationArray");
			}
			if (sourceIndex < int.MinValue || sourceIndex > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", Locale.GetText("Must be in the Int32 range."));
			}
			if (destinationIndex < int.MinValue || destinationIndex > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", Locale.GetText("Must be in the Int32 range."));
			}
			if (length < 0 || length > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			Copy(sourceArray, (int)sourceIndex, destinationArray, (int)destinationIndex, (int)length);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Copy(Array sourceArray, Array destinationArray, long length)
		{
			if (length < 0 || length > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			Copy(sourceArray, destinationArray, (int)length);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int IndexOf(Array array, object value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return IndexOf(array, value, 0, array.Length);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int IndexOf(Array array, object value, int startIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return IndexOf(array, value, startIndex, array.Length - startIndex);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int IndexOf(Array array, object value, int startIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (count < 0 || startIndex < array.GetLowerBound(0) || startIndex - 1 > array.GetUpperBound(0) - count)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (object.Equals(array.GetValueImpl(i), value))
				{
					return i;
				}
			}
			return array.GetLowerBound(0) - 1;
		}

		public void Initialize()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int LastIndexOf(Array array, object value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Length == 0)
			{
				return array.GetLowerBound(0) - 1;
			}
			return LastIndexOf(array, value, array.Length - 1);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int LastIndexOf(Array array, object value, int startIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return LastIndexOf(array, value, startIndex, startIndex - array.GetLowerBound(0) + 1);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int LastIndexOf(Array array, object value, int startIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			int lowerBound = array.GetLowerBound(0);
			if (array.Length == 0)
			{
				return lowerBound - 1;
			}
			if (count < 0 || startIndex < lowerBound || startIndex > array.GetUpperBound(0) || startIndex - count + 1 < lowerBound)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int num = startIndex; num >= startIndex - count + 1; num--)
			{
				if (object.Equals(array.GetValueImpl(num), value))
				{
					return num;
				}
			}
			return lowerBound - 1;
		}

		private static Swapper get_swapper(Array array)
		{
			if (array is int[])
			{
				return array.int_swapper;
			}
			if (array is double[])
			{
				return array.double_swapper;
			}
			if (array is object[])
			{
				return array.obj_swapper;
			}
			return array.slow_swapper;
		}

		private static Swapper get_swapper<T>(T[] array)
		{
			if (array is int[])
			{
				return array.int_swapper;
			}
			if (array is double[])
			{
				return array.double_swapper;
			}
			return array.slow_swapper;
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Reverse(Array array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Reverse(array, array.GetLowerBound(0), array.GetLength(0));
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Reverse(Array array, int index, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < array.GetLowerBound(0) || length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (index > array.GetUpperBound(0) + 1 - length)
			{
				throw new ArgumentException();
			}
			int num = index + length - 1;
			object[] array2 = array as object[];
			if (array2 != null)
			{
				while (index < num)
				{
					object obj = array2[index];
					array2[index] = array2[num];
					array2[num] = obj;
					index++;
					num--;
				}
				return;
			}
			int[] array3 = array as int[];
			if (array3 != null)
			{
				while (index < num)
				{
					int num2 = array3[index];
					array3[index] = array3[num];
					array3[num] = num2;
					index++;
					num--;
				}
				return;
			}
			double[] array4 = array as double[];
			if (array4 != null)
			{
				while (index < num)
				{
					double num3 = array4[index];
					array4[index] = array4[num];
					array4[num] = num3;
					index++;
					num--;
				}
			}
			else
			{
				Swapper swapper = get_swapper(array);
				while (index < num)
				{
					swapper(index, num);
					index++;
					num--;
				}
			}
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Sort(array, null, array.GetLowerBound(0), array.GetLength(0), null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array keys, Array items)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			Sort(keys, items, keys.GetLowerBound(0), keys.GetLength(0), null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array array, IComparer comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Sort(array, null, array.GetLowerBound(0), array.GetLength(0), comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array array, int index, int length)
		{
			Sort(array, null, index, length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array keys, Array items, IComparer comparer)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			Sort(keys, items, keys.GetLowerBound(0), keys.GetLength(0), comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array keys, Array items, int index, int length)
		{
			Sort(keys, items, index, length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array array, int index, int length, IComparer comparer)
		{
			Sort(array, null, index, length, comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort(Array keys, Array items, int index, int length, IComparer comparer)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if (keys.Rank > 1 || (items != null && items.Rank > 1))
			{
				throw new RankException();
			}
			if (items != null && keys.GetLowerBound(0) != items.GetLowerBound(0))
			{
				throw new ArgumentException();
			}
			if (index < keys.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (keys.Length - (index + keys.GetLowerBound(0)) < length || (items != null && index > items.Length - length))
			{
				throw new ArgumentException();
			}
			if (length <= 1)
			{
				return;
			}
			if (comparer == null)
			{
				Swapper swap_items = ((items != null) ? get_swapper(items) : null);
				if (keys is double[])
				{
					combsort(keys as double[], index, length, swap_items);
					return;
				}
				if (keys is int[])
				{
					combsort(keys as int[], index, length, swap_items);
					return;
				}
				if (keys is char[])
				{
					combsort(keys as char[], index, length, swap_items);
					return;
				}
			}
			try
			{
				int high = index + length - 1;
				qsort(keys, items, index, high, comparer);
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(Locale.GetText("The comparer threw an exception."), innerException);
			}
		}

		private void int_swapper(int i, int j)
		{
			int[] array = this as int[];
			int num = array[i];
			array[i] = array[j];
			array[j] = num;
		}

		private void obj_swapper(int i, int j)
		{
			object[] array = this as object[];
			object obj = array[i];
			array[i] = array[j];
			array[j] = obj;
		}

		private void slow_swapper(int i, int j)
		{
			object valueImpl = GetValueImpl(i);
			SetValueImpl(GetValue(j), i);
			SetValueImpl(valueImpl, j);
		}

		private void double_swapper(int i, int j)
		{
			double[] array = this as double[];
			double num = array[i];
			array[i] = array[j];
			array[j] = num;
		}

		private static int new_gap(int gap)
		{
			gap = gap * 10 / 13;
			if (gap == 9 || gap == 10)
			{
				return 11;
			}
			if (gap < 1)
			{
				return 1;
			}
			return gap;
		}

		private static void combsort(double[] array, int start, int size, Swapper swap_items)
		{
			int num = size;
			bool flag;
			do
			{
				num = new_gap(num);
				flag = false;
				int num2 = start + size - num;
				for (int i = start; i < num2; i++)
				{
					int num3 = i + num;
					if (array[i] > array[num3])
					{
						double num4 = array[i];
						array[i] = array[num3];
						array[num3] = num4;
						flag = true;
						if (swap_items != null)
						{
							swap_items(i, num3);
						}
					}
				}
			}
			while (num != 1 || flag);
		}

		private static void combsort(int[] array, int start, int size, Swapper swap_items)
		{
			int num = size;
			bool flag;
			do
			{
				num = new_gap(num);
				flag = false;
				int num2 = start + size - num;
				for (int i = start; i < num2; i++)
				{
					int num3 = i + num;
					if (array[i] > array[num3])
					{
						int num4 = array[i];
						array[i] = array[num3];
						array[num3] = num4;
						flag = true;
						if (swap_items != null)
						{
							swap_items(i, num3);
						}
					}
				}
			}
			while (num != 1 || flag);
		}

		private static void combsort(char[] array, int start, int size, Swapper swap_items)
		{
			int num = size;
			bool flag;
			do
			{
				num = new_gap(num);
				flag = false;
				int num2 = start + size - num;
				for (int i = start; i < num2; i++)
				{
					int num3 = i + num;
					if (array[i] > array[num3])
					{
						char c = array[i];
						array[i] = array[num3];
						array[num3] = c;
						flag = true;
						if (swap_items != null)
						{
							swap_items(i, num3);
						}
					}
				}
			}
			while (num != 1 || flag);
		}

		private static void qsort(Array keys, Array items, int low0, int high0, IComparer comparer)
		{
			if (low0 >= high0)
			{
				return;
			}
			int num = low0;
			int num2 = high0;
			int pos = num + (num2 - num) / 2;
			object valueImpl = keys.GetValueImpl(pos);
			while (true)
			{
				if (num < high0 && compare(keys.GetValueImpl(num), valueImpl, comparer) < 0)
				{
					num++;
					continue;
				}
				while (num2 > low0 && compare(valueImpl, keys.GetValueImpl(num2), comparer) < 0)
				{
					num2--;
				}
				if (num <= num2)
				{
					swap(keys, items, num, num2);
					num++;
					num2--;
					continue;
				}
				break;
			}
			if (low0 < num2)
			{
				qsort(keys, items, low0, num2, comparer);
			}
			if (num < high0)
			{
				qsort(keys, items, num, high0, comparer);
			}
		}

		private static void swap(Array keys, Array items, int i, int j)
		{
			object valueImpl = keys.GetValueImpl(i);
			keys.SetValueImpl(keys.GetValue(j), i);
			keys.SetValueImpl(valueImpl, j);
			if (items != null)
			{
				valueImpl = items.GetValueImpl(i);
				items.SetValueImpl(items.GetValueImpl(j), i);
				items.SetValueImpl(valueImpl, j);
			}
		}

		private static int compare(object value1, object value2, IComparer comparer)
		{
			if (value1 == null)
			{
				return (value2 != null) ? (-1) : 0;
			}
			if (value2 == null)
			{
				return 1;
			}
			if (comparer == null)
			{
				return ((IComparable)value1).CompareTo(value2);
			}
			return comparer.Compare(value1, value2);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<T>(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Sort<T, T>(array, null, 0, array.Length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			Sort(keys, items, 0, keys.Length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<T>(T[] array, IComparer<T> comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Sort<T, T>(array, null, 0, array.Length, comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, IComparer<TKey> comparer)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			Sort(keys, items, 0, keys.Length, comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<T>(T[] array, int index, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Sort<T, T>(array, null, index, length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, int index, int length)
		{
			Sort(keys, items, index, length, null);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<T>(T[] array, int index, int length, IComparer<T> comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Sort<T, T>(array, null, index, length, comparer);
		}

		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		public static void Sort<TKey, TValue>(TKey[] keys, TValue[] items, int index, int length, IComparer<TKey> comparer)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (keys.Length - index < length || (items != null && index > items.Length - length))
			{
				throw new ArgumentException();
			}
			if (length <= 1)
			{
				return;
			}
			if (comparer == null)
			{
				Swapper swap_items = ((items != null) ? get_swapper(items) : null);
				if (keys is double[])
				{
					combsort(keys as double[], index, length, swap_items);
					return;
				}
				if (keys is int[])
				{
					combsort(keys as int[], index, length, swap_items);
					return;
				}
				if (keys is char[])
				{
					combsort(keys as char[], index, length, swap_items);
					return;
				}
			}
			try
			{
				int high = index + length - 1;
				qsort(keys, items, index, high, comparer);
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(Locale.GetText("The comparer threw an exception."), innerException);
			}
		}

		public static void Sort<T>(T[] array, Comparison<T> comparison)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			Sort(array, array.Length, comparison);
		}

		internal static void Sort<T>(T[] array, int length, Comparison<T> comparison)
		{
			if (comparison == null)
			{
				throw new ArgumentNullException("comparison");
			}
			if (length <= 1 || array.Length <= 1)
			{
				return;
			}
			try
			{
				int low = 0;
				int high = length - 1;
				qsort(array, low, high, comparison);
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(Locale.GetText("Comparison threw an exception."), innerException);
			}
		}

		private static void qsort<K, V>(K[] keys, V[] items, int low0, int high0, IComparer<K> comparer)
		{
			if (low0 >= high0)
			{
				return;
			}
			int num = low0;
			int num2 = high0;
			int num3 = num + (num2 - num) / 2;
			K val = keys[num3];
			while (true)
			{
				if (num < high0 && compare(keys[num], val, comparer) < 0)
				{
					num++;
					continue;
				}
				while (num2 > low0 && compare(val, keys[num2], comparer) < 0)
				{
					num2--;
				}
				if (num <= num2)
				{
					swap(keys, items, num, num2);
					num++;
					num2--;
					continue;
				}
				break;
			}
			if (low0 < num2)
			{
				qsort(keys, items, low0, num2, comparer);
			}
			if (num < high0)
			{
				qsort(keys, items, num, high0, comparer);
			}
		}

		private static int compare<T>(T value1, T value2, IComparer<T> comparer)
		{
			if (comparer != null)
			{
				return comparer.Compare(value1, value2);
			}
			if (value1 == null)
			{
				return (value2 != null) ? (-1) : 0;
			}
			if (value2 == null)
			{
				return 1;
			}
			if (value1 is IComparable<T>)
			{
				return ((IComparable<T>)(object)value1).CompareTo(value2);
			}
			if (value1 is IComparable)
			{
				return ((IComparable)(object)value1).CompareTo(value2);
			}
			string text = Locale.GetText("No IComparable or IComparable<{0}> interface found.");
			throw new InvalidOperationException(string.Format(text, typeof(T)));
		}

		private static void qsort<T>(T[] array, int low0, int high0, Comparison<T> comparison)
		{
			if (low0 >= high0)
			{
				return;
			}
			int num = low0;
			int num2 = high0;
			int num3 = num + (num2 - num) / 2;
			T val = array[num3];
			while (true)
			{
				if (num < high0 && comparison(array[num], val) < 0)
				{
					num++;
					continue;
				}
				while (num2 > low0 && comparison(val, array[num2]) < 0)
				{
					num2--;
				}
				if (num <= num2)
				{
					swap(array, num, num2);
					num++;
					num2--;
					continue;
				}
				break;
			}
			if (low0 < num2)
			{
				qsort(array, low0, num2, comparison);
			}
			if (num < high0)
			{
				qsort(array, num, high0, comparison);
			}
		}

		private static void swap<K, V>(K[] keys, V[] items, int i, int j)
		{
			K val = keys[i];
			keys[i] = keys[j];
			keys[j] = val;
			if (items != null)
			{
				V val2 = items[i];
				items[i] = items[j];
				items[j] = val2;
			}
		}

		private static void swap<T>(T[] array, int i, int j)
		{
			T val = array[i];
			array[i] = array[j];
			array[j] = val;
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index + GetLength(0) > array.GetLowerBound(0) + array.GetLength(0))
			{
				throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
			}
			if (array.Rank > 1)
			{
				throw new RankException(Locale.GetText("Only single dimension arrays are supported."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value has to be >= 0."));
			}
			Copy(this, GetLowerBound(0), array, index, GetLength(0));
		}

		[ComVisible(false)]
		public void CopyTo(Array array, long index)
		{
			if (index < 0 || index > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("Value must be >= 0 and <= Int32.MaxValue."));
			}
			CopyTo(array, (int)index);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Resize<T>(ref T[] array, int newSize)
		{
			Resize(ref array, (array != null) ? array.Length : 0, newSize);
		}

		internal static void Resize<T>(ref T[] array, int length, int newSize)
		{
			if (newSize < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array == null)
			{
				array = new T[newSize];
			}
			else if (array.Length != newSize)
			{
				T[] array2 = new T[newSize];
				Copy(array, array2, Math.Min(newSize, length));
				array = array2;
			}
		}

		public static bool TrueForAll<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			foreach (T obj in array)
			{
				if (!match(obj))
				{
					return false;
				}
			}
			return true;
		}

		public static void ForEach<T>(T[] array, Action<T> action)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (T obj in array)
			{
				action(obj);
			}
		}

		public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			TOutput[] array2 = new TOutput[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = converter(array[i]);
			}
			return array2;
		}

		public static int FindLastIndex<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return FindLastIndex(array, 0, array.Length, match);
		}

		public static int FindLastIndex<T>(T[] array, int startIndex, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			return FindLastIndex(array, startIndex, array.Length - startIndex, match);
		}

		public static int FindLastIndex<T>(T[] array, int startIndex, int count, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			if (startIndex > array.Length || startIndex + count > array.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int num = startIndex + count - 1; num >= startIndex; num--)
			{
				if (match(array[num]))
				{
					return num;
				}
			}
			return -1;
		}

		public static int FindIndex<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return FindIndex(array, 0, array.Length, match);
		}

		public static int FindIndex<T>(T[] array, int startIndex, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return FindIndex(array, startIndex, array.Length - startIndex, match);
		}

		public static int FindIndex<T>(T[] array, int startIndex, int count, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			if (startIndex > array.Length || startIndex + count > array.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int i = startIndex; i < startIndex + count; i++)
			{
				if (match(array[i]))
				{
					return i;
				}
			}
			return -1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch<T>(T[] array, T value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return BinarySearch(array, 0, array.Length, value, null);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch<T>(T[] array, T value, IComparer<T> comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return BinarySearch(array, 0, array.Length, value, comparer);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch<T>(T[] array, int index, int length, T value)
		{
			return BinarySearch(array, index, length, value, null);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int BinarySearch<T>(T[] array, int index, int length, T value, IComparer<T> comparer)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Locale.GetText("index is less than the lower bound of array."));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Locale.GetText("Value has to be >= 0."));
			}
			if (index > array.Length - length)
			{
				throw new ArgumentException(Locale.GetText("index and length do not specify a valid range in array."));
			}
			if (comparer == null)
			{
				comparer = Comparer<T>.Default;
			}
			int num = index;
			int num2 = index + length - 1;
			int num3 = 0;
			try
			{
				while (num <= num2)
				{
					int num4 = num + (num2 - num) / 2;
					num3 = comparer.Compare(value, array[num4]);
					if (num3 == 0)
					{
						return num4;
					}
					if (num3 < 0)
					{
						num2 = num4 - 1;
					}
					else
					{
						num = num4 + 1;
					}
				}
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(Locale.GetText("Comparer threw an exception."), innerException);
			}
			return ~num;
		}

		public static int IndexOf<T>(T[] array, T value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return IndexOf(array, value, 0, array.Length);
		}

		public static int IndexOf<T>(T[] array, T value, int startIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return IndexOf(array, value, startIndex, array.Length - startIndex);
		}

		public static int IndexOf<T>(T[] array, T value, int startIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count < 0 || startIndex < array.GetLowerBound(0) || startIndex - 1 > array.GetUpperBound(0) - count)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = startIndex + count;
			EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
			for (int i = startIndex; i < num; i++)
			{
				if (equalityComparer.Equals(array[i], value))
				{
					return i;
				}
			}
			return -1;
		}

		public static int LastIndexOf<T>(T[] array, T value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Length == 0)
			{
				return -1;
			}
			return LastIndexOf(array, value, array.Length - 1);
		}

		public static int LastIndexOf<T>(T[] array, T value, int startIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return LastIndexOf(array, value, startIndex, startIndex + 1);
		}

		public static int LastIndexOf<T>(T[] array, T value, int startIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count < 0 || startIndex < array.GetLowerBound(0) || startIndex > array.GetUpperBound(0) || startIndex - count + 1 < array.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException();
			}
			EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
			for (int num = startIndex; num >= startIndex - count + 1; num--)
			{
				if (equalityComparer.Equals(array[num], value))
				{
					return num;
				}
			}
			return -1;
		}

		public static T[] FindAll<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			int newSize = 0;
			T[] array2 = new T[array.Length];
			foreach (T val in array)
			{
				if (match(val))
				{
					array2[newSize++] = val;
				}
			}
			Resize(ref array2, newSize);
			return array2;
		}

		public static bool Exists<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			foreach (T obj in array)
			{
				if (match(obj))
				{
					return true;
				}
			}
			return false;
		}

		public static ReadOnlyCollection<T> AsReadOnly<T>(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			return new ReadOnlyCollection<T>(new ArrayReadOnlyList<T>(array));
		}

		public static T Find<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			foreach (T val in array)
			{
				if (match(val))
				{
					return val;
				}
			}
			return default(T);
		}

		public static T FindLast<T>(T[] array, Predicate<T> match)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (match(array[num]))
				{
					return array[num];
				}
			}
			return default(T);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void ConstrainedCopy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
		{
			Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
		}
	}
}
