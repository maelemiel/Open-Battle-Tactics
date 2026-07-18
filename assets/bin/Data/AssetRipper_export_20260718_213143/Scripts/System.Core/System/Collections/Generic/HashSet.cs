using System.Linq;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
	[Serializable]
	public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ISerializable, IDeserializationCallback
	{
		private struct Link
		{
			public int HashCode;

			public int Next;
		}

		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
		{
			private HashSet<T> hashset;

			private int next;

			private int stamp;

			private T current;

			object IEnumerator.Current
			{
				get
				{
					CheckState();
					if (next <= 0)
					{
						throw new InvalidOperationException("Current is not valid");
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

			internal Enumerator(HashSet<T> hashset)
			{
				this.hashset = hashset;
				stamp = hashset.generation;
			}

			void IEnumerator.Reset()
			{
				CheckState();
				next = 0;
			}

			public bool MoveNext()
			{
				CheckState();
				if (next < 0)
				{
					return false;
				}
				while (next < hashset.touched)
				{
					int num = next++;
					if (hashset.GetLinkHashCode(num) != 0)
					{
						current = hashset.slots[num];
						return true;
					}
				}
				next = -1;
				return false;
			}

			public void Dispose()
			{
				hashset = null;
			}

			private void CheckState()
			{
				if (hashset == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (hashset.generation != stamp)
				{
					throw new InvalidOperationException("HashSet have been modified while it was iterated over");
				}
			}
		}

		private static class PrimeHelper
		{
			private static readonly int[] primes_table = new int[34]
			{
				11, 19, 37, 73, 109, 163, 251, 367, 557, 823,
				1237, 1861, 2777, 4177, 6247, 9371, 14057, 21089, 31627, 47431,
				71143, 106721, 160073, 240101, 360163, 540217, 810343, 1215497, 1823231, 2734867,
				4102283, 6153409, 9230113, 13845163
			};

			private static bool TestPrime(int x)
			{
				if ((x & 1) != 0)
				{
					int num = (int)Math.Sqrt(x);
					for (int i = 3; i < num; i += 2)
					{
						if (x % i == 0)
						{
							return false;
						}
					}
					return true;
				}
				return x == 2;
			}

			private static int CalcPrime(int x)
			{
				for (int i = (x & -2) - 1; i < int.MaxValue; i += 2)
				{
					if (TestPrime(i))
					{
						return i;
					}
				}
				return x;
			}

			public static int ToPrime(int x)
			{
				for (int i = 0; i < primes_table.Length; i++)
				{
					if (x <= primes_table[i])
					{
						return primes_table[i];
					}
				}
				return CalcPrime(x);
			}
		}

		private const int INITIAL_SIZE = 10;

		private const float DEFAULT_LOAD_FACTOR = 0.9f;

		private const int NO_SLOT = -1;

		private const int HASH_FLAG = int.MinValue;

		private int[] table;

		private Link[] links;

		private T[] slots;

		private int touched;

		private int empty_slot;

		private int count;

		private int threshold;

		private IEqualityComparer<T> comparer;

		private SerializationInfo si;

		private int generation;

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public int Count
		{
			get
			{
				return count;
			}
		}

		public IEqualityComparer<T> Comparer
		{
			get
			{
				return comparer;
			}
		}

		public HashSet()
		{
			Init(10, null);
		}

		public HashSet(IEqualityComparer<T> comparer)
		{
			Init(10, comparer);
		}

		public HashSet(IEnumerable<T> collection)
			: this(collection, (IEqualityComparer<T>)null)
		{
		}

		public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			int capacity = 0;
			ICollection<T> collection2 = collection as ICollection<T>;
			if (collection2 != null)
			{
				capacity = collection2.Count;
			}
			Init(capacity, comparer);
			foreach (T item in collection)
			{
				Add(item);
			}
		}

		protected HashSet(SerializationInfo info, StreamingContext context)
		{
			si = info;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		void ICollection<T>.CopyTo(T[] array, int index)
		{
			CopyTo(array, index);
		}

		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		private void Init(int capacity, IEqualityComparer<T> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.comparer = comparer ?? EqualityComparer<T>.Default;
			if (capacity == 0)
			{
				capacity = 10;
			}
			capacity = (int)((float)capacity / 0.9f) + 1;
			InitArrays(capacity);
			generation = 0;
		}

		private void InitArrays(int size)
		{
			table = new int[size];
			links = new Link[size];
			empty_slot = -1;
			slots = new T[size];
			touched = 0;
			threshold = (int)((float)table.Length * 0.9f);
			if (threshold == 0 && table.Length > 0)
			{
				threshold = 1;
			}
		}

		private bool SlotsContainsAt(int index, int hash, T item)
		{
			Link link;
			for (int num = table[index] - 1; num != -1; num = link.Next)
			{
				link = links[num];
				if (link.HashCode != hash)
				{
					continue;
				}
				bool num2;
				if (hash == int.MinValue && (item == null || slots[num] == null))
				{
					if (item != null)
					{
						continue;
					}
					num2 = null == (object)slots[num];
				}
				else
				{
					num2 = comparer.Equals(item, slots[num]);
				}
				if (num2)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(T[] array)
		{
			CopyTo(array, 0, count);
		}

		public void CopyTo(T[] array, int index)
		{
			CopyTo(array, index, count);
		}

		public void CopyTo(T[] array, int index, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index > array.Length)
			{
				throw new ArgumentException("index larger than largest valid index of array");
			}
			if (array.Length - index < count)
			{
				throw new ArgumentException("Destination array cannot hold the requested elements!");
			}
			int i = 0;
			int num = 0;
			for (; i < touched; i++)
			{
				if (num >= count)
				{
					break;
				}
				if (GetLinkHashCode(i) != 0)
				{
					array[index++] = slots[i];
				}
			}
		}

		private void Resize()
		{
			int num = PrimeHelper.ToPrime((table.Length << 1) | 1);
			int[] array = new int[num];
			Link[] array2 = new Link[num];
			for (int i = 0; i < table.Length; i++)
			{
				for (int num2 = table[i] - 1; num2 != -1; num2 = links[num2].Next)
				{
					int num3 = ((array2[num2].HashCode = GetItemHashCode(slots[num2])) & 0x7FFFFFFF) % num;
					array2[num2].Next = array[num3] - 1;
					array[num3] = num2 + 1;
				}
			}
			table = array;
			links = array2;
			T[] destinationArray = new T[num];
			Array.Copy(slots, 0, destinationArray, 0, touched);
			slots = destinationArray;
			threshold = (int)((float)num * 0.9f);
		}

		private int GetLinkHashCode(int index)
		{
			return links[index].HashCode & int.MinValue;
		}

		private int GetItemHashCode(T item)
		{
			if (item == null)
			{
				return int.MinValue;
			}
			return comparer.GetHashCode(item) | int.MinValue;
		}

		public bool Add(T item)
		{
			int itemHashCode = GetItemHashCode(item);
			int num = (itemHashCode & 0x7FFFFFFF) % table.Length;
			if (SlotsContainsAt(num, itemHashCode, item))
			{
				return false;
			}
			if (++count > threshold)
			{
				Resize();
				num = (itemHashCode & 0x7FFFFFFF) % table.Length;
			}
			int num2 = empty_slot;
			if (num2 == -1)
			{
				num2 = touched++;
			}
			else
			{
				empty_slot = links[num2].Next;
			}
			links[num2].HashCode = itemHashCode;
			links[num2].Next = table[num] - 1;
			table[num] = num2 + 1;
			slots[num2] = item;
			generation++;
			return true;
		}

		public void Clear()
		{
			count = 0;
			Array.Clear(table, 0, table.Length);
			Array.Clear(slots, 0, slots.Length);
			Array.Clear(links, 0, links.Length);
			empty_slot = -1;
			touched = 0;
			generation++;
		}

		public bool Contains(T item)
		{
			int itemHashCode = GetItemHashCode(item);
			int index = (itemHashCode & 0x7FFFFFFF) % table.Length;
			return SlotsContainsAt(index, itemHashCode, item);
		}

		public bool Remove(T item)
		{
			int itemHashCode = GetItemHashCode(item);
			int num = (itemHashCode & 0x7FFFFFFF) % table.Length;
			int num2 = table[num] - 1;
			if (num2 == -1)
			{
				return false;
			}
			int num3 = -1;
			do
			{
				Link link = links[num2];
				if (link.HashCode == itemHashCode)
				{
					bool num4;
					if (itemHashCode == int.MinValue && (item == null || slots[num2] == null))
					{
						if (item != null)
						{
							goto IL_00c4;
						}
						num4 = null == (object)slots[num2];
					}
					else
					{
						num4 = comparer.Equals(slots[num2], item);
					}
					if (num4)
					{
						break;
					}
				}
				goto IL_00c4;
				IL_00c4:
				num3 = num2;
				num2 = link.Next;
			}
			while (num2 != -1);
			if (num2 == -1)
			{
				return false;
			}
			count--;
			if (num3 == -1)
			{
				table[num] = links[num2].Next + 1;
			}
			else
			{
				links[num3].Next = links[num2].Next;
			}
			links[num2].Next = empty_slot;
			empty_slot = num2;
			links[num2].HashCode = 0;
			slots[num2] = default(T);
			generation++;
			return true;
		}

		public int RemoveWhere(Predicate<T> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			int num = 0;
			T[] array = new T[count];
			CopyTo(array, 0);
			T[] array2 = array;
			foreach (T val in array2)
			{
				if (predicate(val))
				{
					Remove(val);
					num++;
				}
			}
			return num;
		}

		public void TrimExcess()
		{
			Resize();
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			T[] array = new T[count];
			CopyTo(array, 0);
			T[] array2 = array;
			foreach (T val in array2)
			{
				if (!other.Contains(val))
				{
					Remove(val);
				}
			}
			foreach (T item in other)
			{
				if (!Contains(item))
				{
					Remove(item);
				}
			}
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T item in other)
			{
				Remove(item);
			}
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T item in other)
			{
				if (Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (count != other.Count())
			{
				return false;
			}
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (!other.Contains(current))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T item in other)
			{
				if (Contains(item))
				{
					Remove(item);
				}
				else
				{
					Add(item);
				}
			}
		}

		public void UnionWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T item in other)
			{
				Add(item);
			}
		}

		private bool CheckIsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (!other.Contains(current))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (count == 0)
			{
				return true;
			}
			if (count > other.Count())
			{
				return false;
			}
			return CheckIsSubsetOf(other);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (count == 0)
			{
				return true;
			}
			if (count >= other.Count())
			{
				return false;
			}
			return CheckIsSubsetOf(other);
		}

		private bool CheckIsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (T item in other)
			{
				if (!Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (count < other.Count())
			{
				return false;
			}
			return CheckIsSupersetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (count <= other.Count())
			{
				return false;
			}
			return CheckIsSupersetOf(other);
		}

		[System.MonoTODO]
		public static IEqualityComparer<HashSet<T>> CreateSetComparer()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public virtual void OnDeserialization(object sender)
		{
			if (si == null)
			{
				return;
			}
			throw new NotImplementedException();
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}
