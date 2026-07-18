using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Collections
{
	[Serializable]
	[DebuggerDisplay("Count={Count}")]
	[ComVisible(true)]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
	public class Hashtable : IEnumerable, ICloneable, ISerializable, ICollection, IDictionary, IDeserializationCallback
	{
		[Serializable]
		internal struct Slot
		{
			internal object key;

			internal object value;
		}

		[Serializable]
		internal class KeyMarker
		{
			public static readonly KeyMarker Removed = new KeyMarker();
		}

		private enum EnumeratorMode
		{
			KEY_MODE = 0,
			VALUE_MODE = 1,
			ENTRY_MODE = 2
		}

		[Serializable]
		private sealed class Enumerator : IEnumerator, IDictionaryEnumerator
		{
			private Hashtable host;

			private int stamp;

			private int pos;

			private int size;

			private EnumeratorMode mode;

			private object currentKey;

			private object currentValue;

			private static readonly string xstr = "Hashtable.Enumerator: snapshot out of sync.";

			public DictionaryEntry Entry
			{
				get
				{
					if (currentKey == null)
					{
						throw new InvalidOperationException();
					}
					FailFast();
					return new DictionaryEntry(currentKey, currentValue);
				}
			}

			public object Key
			{
				get
				{
					if (currentKey == null)
					{
						throw new InvalidOperationException();
					}
					FailFast();
					return currentKey;
				}
			}

			public object Value
			{
				get
				{
					if (currentKey == null)
					{
						throw new InvalidOperationException();
					}
					FailFast();
					return currentValue;
				}
			}

			public object Current
			{
				get
				{
					if (currentKey == null)
					{
						throw new InvalidOperationException();
					}
					switch (mode)
					{
					case EnumeratorMode.KEY_MODE:
						return currentKey;
					case EnumeratorMode.VALUE_MODE:
						return currentValue;
					case EnumeratorMode.ENTRY_MODE:
						return new DictionaryEntry(currentKey, currentValue);
					default:
						throw new Exception("should never happen");
					}
				}
			}

			public Enumerator(Hashtable host, EnumeratorMode mode)
			{
				this.host = host;
				stamp = host.modificationCount;
				size = host.table.Length;
				this.mode = mode;
				Reset();
			}

			public Enumerator(Hashtable host)
				: this(host, EnumeratorMode.KEY_MODE)
			{
			}

			private void FailFast()
			{
				if (host.modificationCount != stamp)
				{
					throw new InvalidOperationException(xstr);
				}
			}

			public void Reset()
			{
				FailFast();
				pos = -1;
				currentKey = null;
				currentValue = null;
			}

			public bool MoveNext()
			{
				FailFast();
				if (pos < size)
				{
					while (++pos < size)
					{
						Slot slot = host.table[pos];
						if (slot.key != null && slot.key != KeyMarker.Removed)
						{
							currentKey = slot.key;
							currentValue = slot.value;
							return true;
						}
					}
				}
				currentKey = null;
				currentValue = null;
				return false;
			}
		}

		[Serializable]
		[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
		[DebuggerDisplay("Count={Count}")]
		private class HashKeys : IEnumerable, ICollection
		{
			private Hashtable host;

			public virtual int Count
			{
				get
				{
					return host.Count;
				}
			}

			public virtual bool IsSynchronized
			{
				get
				{
					return host.IsSynchronized;
				}
			}

			public virtual object SyncRoot
			{
				get
				{
					return host.SyncRoot;
				}
			}

			public HashKeys(Hashtable host)
			{
				if (host == null)
				{
					throw new ArgumentNullException();
				}
				this.host = host;
			}

			public virtual void CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}
				if (array.Length - arrayIndex < Count)
				{
					throw new ArgumentException("not enough space");
				}
				host.CopyToArray(array, arrayIndex, EnumeratorMode.KEY_MODE);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return new Enumerator(host, EnumeratorMode.KEY_MODE);
			}
		}

		[Serializable]
		[DebuggerTypeProxy(typeof(CollectionDebuggerView))]
		[DebuggerDisplay("Count={Count}")]
		private class HashValues : IEnumerable, ICollection
		{
			private Hashtable host;

			public virtual int Count
			{
				get
				{
					return host.Count;
				}
			}

			public virtual bool IsSynchronized
			{
				get
				{
					return host.IsSynchronized;
				}
			}

			public virtual object SyncRoot
			{
				get
				{
					return host.SyncRoot;
				}
			}

			public HashValues(Hashtable host)
			{
				if (host == null)
				{
					throw new ArgumentNullException();
				}
				this.host = host;
			}

			public virtual void CopyTo(Array array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("array");
				}
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}
				if (array.Length - arrayIndex < Count)
				{
					throw new ArgumentException("not enough space");
				}
				host.CopyToArray(array, arrayIndex, EnumeratorMode.VALUE_MODE);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return new Enumerator(host, EnumeratorMode.VALUE_MODE);
			}
		}

		[Serializable]
		private class SyncHashtable : Hashtable, IEnumerable
		{
			private Hashtable host;

			public override int Count
			{
				get
				{
					return host.Count;
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
					return host.SyncRoot;
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return host.IsFixedSize;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return host.IsReadOnly;
				}
			}

			public override ICollection Keys
			{
				get
				{
					ICollection collection = null;
					lock (host.SyncRoot)
					{
						return host.Keys;
					}
				}
			}

			public override ICollection Values
			{
				get
				{
					ICollection collection = null;
					lock (host.SyncRoot)
					{
						return host.Values;
					}
				}
			}

			public override object this[object key]
			{
				get
				{
					return host[key];
				}
				set
				{
					lock (host.SyncRoot)
					{
						host[key] = value;
					}
				}
			}

			public SyncHashtable(Hashtable host)
			{
				if (host == null)
				{
					throw new ArgumentNullException();
				}
				this.host = host;
			}

			internal SyncHashtable(SerializationInfo info, StreamingContext context)
			{
				host = (Hashtable)info.GetValue("ParentTable", typeof(Hashtable));
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new Enumerator(host, EnumeratorMode.ENTRY_MODE);
			}

			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("ParentTable", host);
			}

			public override void CopyTo(Array array, int arrayIndex)
			{
				host.CopyTo(array, arrayIndex);
			}

			public override void Add(object key, object value)
			{
				lock (host.SyncRoot)
				{
					host.Add(key, value);
				}
			}

			public override void Clear()
			{
				lock (host.SyncRoot)
				{
					host.Clear();
				}
			}

			public override bool Contains(object key)
			{
				return host.Find(key) >= 0;
			}

			public override IDictionaryEnumerator GetEnumerator()
			{
				return new Enumerator(host, EnumeratorMode.ENTRY_MODE);
			}

			public override void Remove(object key)
			{
				lock (host.SyncRoot)
				{
					host.Remove(key);
				}
			}

			public override bool ContainsKey(object key)
			{
				return host.Contains(key);
			}

			public override bool ContainsValue(object value)
			{
				return host.ContainsValue(value);
			}

			public override object Clone()
			{
				lock (host.SyncRoot)
				{
					return new SyncHashtable((Hashtable)host.Clone());
				}
			}
		}

		private const int CHAIN_MARKER = int.MinValue;

		private int inUse;

		private int modificationCount;

		private float loadFactor;

		private Slot[] table;

		private int[] hashes;

		private int threshold;

		private HashKeys hashKeys;

		private HashValues hashValues;

		private IHashCodeProvider hcpRef;

		private IComparer comparerRef;

		private SerializationInfo serializationInfo;

		private IEqualityComparer equalityComparer;

		private static readonly int[] primeTbl = new int[34]
		{
			11, 19, 37, 73, 109, 163, 251, 367, 557, 823,
			1237, 1861, 2777, 4177, 6247, 9371, 14057, 21089, 31627, 47431,
			71143, 106721, 160073, 240101, 360163, 540217, 810343, 1215497, 1823231, 2734867,
			4102283, 6153409, 9230113, 13845163
		};

		[Obsolete("Please use EqualityComparer property.")]
		protected IComparer comparer
		{
			get
			{
				return comparerRef;
			}
			set
			{
				comparerRef = value;
			}
		}

		[Obsolete("Please use EqualityComparer property.")]
		protected IHashCodeProvider hcp
		{
			get
			{
				return hcpRef;
			}
			set
			{
				hcpRef = value;
			}
		}

		protected IEqualityComparer EqualityComparer
		{
			get
			{
				return equalityComparer;
			}
		}

		public virtual int Count
		{
			get
			{
				return inUse;
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

		public virtual ICollection Keys
		{
			get
			{
				if (hashKeys == null)
				{
					hashKeys = new HashKeys(this);
				}
				return hashKeys;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				if (hashValues == null)
				{
					hashValues = new HashValues(this);
				}
				return hashValues;
			}
		}

		public virtual object this[object key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key", "null key");
				}
				Slot[] array = table;
				int[] array2 = hashes;
				uint num = (uint)array.Length;
				int num2 = GetHash(key) & 0x7FFFFFFF;
				uint num3 = (uint)num2;
				uint num4 = (uint)((num2 >> 5) + 1) % (num - 1) + 1;
				for (uint num5 = num; num5 != 0; num5--)
				{
					num3 %= num;
					Slot slot = array[num3];
					int num6 = array2[num3];
					object key2 = slot.key;
					if (key2 == null)
					{
						break;
					}
					if (key2 == key || ((num6 & 0x7FFFFFFF) == num2 && KeyEquals(key, key2)))
					{
						return slot.value;
					}
					if ((num6 & int.MinValue) == 0)
					{
						break;
					}
					num3 += num4;
				}
				return null;
			}
			set
			{
				PutImpl(key, value, true);
			}
		}

		public Hashtable()
			: this(0, 1f)
		{
		}

		[Obsolete("Please use Hashtable(int, float, IEqualityComparer) instead")]
		public Hashtable(int capacity, float loadFactor, IHashCodeProvider hcp, IComparer comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", "negative capacity");
			}
			if (loadFactor < 0.1f || loadFactor > 1f || float.IsNaN(loadFactor))
			{
				throw new ArgumentOutOfRangeException("loadFactor", "load factor");
			}
			if (capacity == 0)
			{
				capacity++;
			}
			this.loadFactor = 0.75f * loadFactor;
			double num = (float)capacity / this.loadFactor;
			if (num > 2147483647.0)
			{
				throw new ArgumentException("Size is too big");
			}
			int x = (int)num;
			x = ToPrime(x);
			SetTable(new Slot[x], new int[x]);
			this.hcp = hcp;
			this.comparer = comparer;
			inUse = 0;
			modificationCount = 0;
		}

		public Hashtable(int capacity, float loadFactor)
			: this(capacity, loadFactor, null, null)
		{
		}

		public Hashtable(int capacity)
			: this(capacity, 1f)
		{
		}

		internal Hashtable(Hashtable source)
		{
			inUse = source.inUse;
			loadFactor = source.loadFactor;
			table = (Slot[])source.table.Clone();
			hashes = (int[])source.hashes.Clone();
			threshold = source.threshold;
			hcpRef = source.hcpRef;
			comparerRef = source.comparerRef;
			equalityComparer = source.equalityComparer;
		}

		[Obsolete("Please use Hashtable(int, IEqualityComparer) instead")]
		public Hashtable(int capacity, IHashCodeProvider hcp, IComparer comparer)
			: this(capacity, 1f, hcp, comparer)
		{
		}

		[Obsolete("Please use Hashtable(IDictionary, float, IEqualityComparer) instead")]
		public Hashtable(IDictionary d, float loadFactor, IHashCodeProvider hcp, IComparer comparer)
			: this((d != null) ? d.Count : 0, loadFactor, hcp, comparer)
		{
			if (d == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			IDictionaryEnumerator enumerator = d.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Add(enumerator.Key, enumerator.Value);
			}
		}

		public Hashtable(IDictionary d, float loadFactor)
			: this(d, loadFactor, null, null)
		{
		}

		public Hashtable(IDictionary d)
			: this(d, 1f)
		{
		}

		[Obsolete("Please use Hashtable(IDictionary, IEqualityComparer) instead")]
		public Hashtable(IDictionary d, IHashCodeProvider hcp, IComparer comparer)
			: this(d, 1f, hcp, comparer)
		{
		}

		[Obsolete("Please use Hashtable(IEqualityComparer) instead")]
		public Hashtable(IHashCodeProvider hcp, IComparer comparer)
			: this(1, 1f, hcp, comparer)
		{
		}

		public Hashtable(SerializationInfo info, StreamingContext context)
		{
			serializationInfo = info;
		}

		public Hashtable(IDictionary d, IEqualityComparer equalityComparer)
			: this(d, 1f, equalityComparer)
		{
		}

		public Hashtable(IDictionary d, float loadFactor, IEqualityComparer equalityComparer)
			: this(d, loadFactor)
		{
			this.equalityComparer = equalityComparer;
		}

		public Hashtable(IEqualityComparer equalityComparer)
			: this(1, 1f, equalityComparer)
		{
		}

		public Hashtable(int capacity, IEqualityComparer equalityComparer)
			: this(capacity, 1f, equalityComparer)
		{
		}

		public Hashtable(int capacity, float loadFactor, IEqualityComparer equalityComparer)
			: this(capacity, loadFactor)
		{
			this.equalityComparer = equalityComparer;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this, EnumeratorMode.ENTRY_MODE);
		}

		public virtual void CopyTo(Array array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("array is multidimensional");
			}
			if (array.Length > 0 && arrayIndex >= array.Length)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than array.Length");
			}
			if (arrayIndex + inUse > array.Length)
			{
				throw new ArgumentException("Not enough room from arrayIndex to end of array for this Hashtable");
			}
			IDictionaryEnumerator enumerator = GetEnumerator();
			int num = arrayIndex;
			while (enumerator.MoveNext())
			{
				array.SetValue(enumerator.Entry, num++);
			}
		}

		public virtual void Add(object key, object value)
		{
			PutImpl(key, value, false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public virtual void Clear()
		{
			for (int i = 0; i < table.Length; i++)
			{
				table[i].key = null;
				table[i].value = null;
				hashes[i] = 0;
			}
			inUse = 0;
			modificationCount++;
		}

		public virtual bool Contains(object key)
		{
			return Find(key) >= 0;
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new Enumerator(this, EnumeratorMode.ENTRY_MODE);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public virtual void Remove(object key)
		{
			int num = Find(key);
			if (num >= 0)
			{
				Slot[] array = table;
				int num2 = hashes[num];
				num2 &= int.MinValue;
				hashes[num] = num2;
				array[num].key = ((num2 == 0) ? null : KeyMarker.Removed);
				array[num].value = null;
				inUse--;
				modificationCount++;
			}
		}

		public virtual bool ContainsKey(object key)
		{
			return Contains(key);
		}

		public virtual bool ContainsValue(object value)
		{
			int num = table.Length;
			Slot[] array = table;
			if (value == null)
			{
				for (int i = 0; i < num; i++)
				{
					Slot slot = array[i];
					if (slot.key != null && slot.key != KeyMarker.Removed && slot.value == null)
					{
						return true;
					}
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					Slot slot2 = array[j];
					if (slot2.key != null && slot2.key != KeyMarker.Removed && value.Equals(slot2.value))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual object Clone()
		{
			return new Hashtable(this);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("LoadFactor", loadFactor);
			info.AddValue("Version", modificationCount);
			if (equalityComparer != null)
			{
				info.AddValue("KeyComparer", equalityComparer);
			}
			else
			{
				info.AddValue("Comparer", comparerRef);
			}
			if (hcpRef != null)
			{
				info.AddValue("HashCodeProvider", hcpRef);
			}
			info.AddValue("HashSize", table.Length);
			object[] array = new object[inUse];
			CopyToArray(array, 0, EnumeratorMode.KEY_MODE);
			object[] array2 = new object[inUse];
			CopyToArray(array2, 0, EnumeratorMode.VALUE_MODE);
			info.AddValue("Keys", array);
			info.AddValue("Values", array2);
			info.AddValue("equalityComparer", equalityComparer);
		}

		[MonoTODO("Serialize equalityComparer")]
		public virtual void OnDeserialization(object sender)
		{
			if (serializationInfo != null)
			{
				loadFactor = (float)serializationInfo.GetValue("LoadFactor", typeof(float));
				modificationCount = (int)serializationInfo.GetValue("Version", typeof(int));
				try
				{
					equalityComparer = (IEqualityComparer)serializationInfo.GetValue("KeyComparer", typeof(object));
				}
				catch
				{
				}
				if (equalityComparer == null)
				{
					comparerRef = (IComparer)serializationInfo.GetValue("Comparer", typeof(object));
				}
				try
				{
					hcpRef = (IHashCodeProvider)serializationInfo.GetValue("HashCodeProvider", typeof(object));
				}
				catch
				{
				}
				int x = (int)serializationInfo.GetValue("HashSize", typeof(int));
				object[] array = (object[])serializationInfo.GetValue("Keys", typeof(object[]));
				object[] array2 = (object[])serializationInfo.GetValue("Values", typeof(object[]));
				if (array.Length != array2.Length)
				{
					throw new SerializationException("Keys and values of uneven size");
				}
				x = ToPrime(x);
				SetTable(new Slot[x], new int[x]);
				for (int i = 0; i < array.Length; i++)
				{
					Add(array[i], array2[i]);
				}
				AdjustThreshold();
				serializationInfo = null;
			}
		}

		public static Hashtable Synchronized(Hashtable table)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			return new SyncHashtable(table);
		}

		protected virtual int GetHash(object key)
		{
			if (equalityComparer != null)
			{
				return equalityComparer.GetHashCode(key);
			}
			if (hcpRef == null)
			{
				return key.GetHashCode();
			}
			return hcpRef.GetHashCode(key);
		}

		protected virtual bool KeyEquals(object item, object key)
		{
			if (key == KeyMarker.Removed)
			{
				return false;
			}
			if (equalityComparer != null)
			{
				return equalityComparer.Equals(item, key);
			}
			if (comparerRef == null)
			{
				return item.Equals(key);
			}
			return comparerRef.Compare(item, key) == 0;
		}

		private void AdjustThreshold()
		{
			int num = table.Length;
			threshold = (int)((float)num * loadFactor);
			if (threshold >= num)
			{
				threshold = num - 1;
			}
		}

		private void SetTable(Slot[] table, int[] hashes)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			this.table = table;
			this.hashes = hashes;
			AdjustThreshold();
		}

		private int Find(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "null key");
			}
			Slot[] array = table;
			int[] array2 = hashes;
			uint num = (uint)array.Length;
			int num2 = GetHash(key) & 0x7FFFFFFF;
			uint num3 = (uint)num2;
			uint num4 = (uint)((num2 >> 5) + 1) % (num - 1) + 1;
			for (uint num5 = num; num5 != 0; num5--)
			{
				num3 %= num;
				Slot slot = array[num3];
				int num6 = array2[num3];
				object key2 = slot.key;
				if (key2 == null)
				{
					break;
				}
				if (key2 == key || ((num6 & 0x7FFFFFFF) == num2 && KeyEquals(key, key2)))
				{
					return (int)num3;
				}
				if ((num6 & int.MinValue) == 0)
				{
					break;
				}
				num3 += num4;
			}
			return -1;
		}

		private void Rehash()
		{
			int num = table.Length;
			uint num2 = (uint)ToPrime((num << 1) | 1);
			Slot[] array = new Slot[num2];
			Slot[] array2 = table;
			int[] array3 = new int[num2];
			int[] array4 = hashes;
			for (int i = 0; i < num; i++)
			{
				Slot slot = array2[i];
				if (slot.key != null)
				{
					int num3 = array4[i] & 0x7FFFFFFF;
					uint num4 = (uint)num3;
					uint num5 = (uint)((num3 >> 5) + 1) % (num2 - 1) + 1;
					uint num6 = num4 % num2;
					while (array[num6].key != null)
					{
						array3[num6] |= int.MinValue;
						num4 += num5;
						num6 = num4 % num2;
					}
					array[num6].key = slot.key;
					array[num6].value = slot.value;
					array3[num6] |= num3;
				}
			}
			modificationCount++;
			SetTable(array, array3);
		}

		private void PutImpl(object key, object value, bool overwrite)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "null key");
			}
			if (inUse >= threshold)
			{
				Rehash();
			}
			uint num = (uint)table.Length;
			int num2 = GetHash(key) & 0x7FFFFFFF;
			uint num3 = (uint)num2;
			uint num4 = ((num3 >> 5) + 1) % (num - 1) + 1;
			Slot[] array = table;
			int[] array2 = hashes;
			int num5 = -1;
			for (int i = 0; i < num; i++)
			{
				int num6 = (int)(num3 % num);
				Slot slot = array[num6];
				int num7 = array2[num6];
				if (num5 == -1 && slot.key == KeyMarker.Removed && (num7 & int.MinValue) != 0)
				{
					num5 = num6;
				}
				if (slot.key == null || (slot.key == KeyMarker.Removed && (num7 & int.MinValue) == 0))
				{
					if (num5 == -1)
					{
						num5 = num6;
					}
					break;
				}
				if ((num7 & 0x7FFFFFFF) == num2 && KeyEquals(key, slot.key))
				{
					if (overwrite)
					{
						array[num6].value = value;
						modificationCount++;
						return;
					}
					throw new ArgumentException("Key duplication when adding: " + key);
				}
				if (num5 == -1)
				{
					array2[num6] |= int.MinValue;
				}
				num3 += num4;
			}
			if (num5 != -1)
			{
				array[num5].key = key;
				array[num5].value = value;
				array2[num5] |= num2;
				inUse++;
				modificationCount++;
			}
		}

		private void CopyToArray(Array arr, int i, EnumeratorMode mode)
		{
			IEnumerator enumerator = new Enumerator(this, mode);
			while (enumerator.MoveNext())
			{
				arr.SetValue(enumerator.Current, i++);
			}
		}

		internal static bool TestPrime(int x)
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

		internal static int CalcPrime(int x)
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

		internal static int ToPrime(int x)
		{
			for (int i = 0; i < primeTbl.Length; i++)
			{
				if (x <= primeTbl[i])
				{
					return primeTbl[i];
				}
			}
			return CalcPrime(x);
		}
	}
}
