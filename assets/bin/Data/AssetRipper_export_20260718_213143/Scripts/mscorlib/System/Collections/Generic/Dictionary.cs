using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
	[Serializable]
	[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
	[DebuggerDisplay("Count={Count}")]
	[ComVisible(false)]
	public class Dictionary<TKey, TValue> : IEnumerable, ISerializable, ICollection, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IDictionary, IDeserializationCallback
	{
		[Serializable]
		private class ShimEnumerator : IEnumerator, IDictionaryEnumerator
		{
			private Enumerator host_enumerator;

			public DictionaryEntry Entry
			{
				get
				{
					return ((IDictionaryEnumerator)host_enumerator).Entry;
				}
			}

			public object Key
			{
				get
				{
					return host_enumerator.Current.Key;
				}
			}

			public object Value
			{
				get
				{
					return host_enumerator.Current.Value;
				}
			}

			public object Current
			{
				get
				{
					return Entry;
				}
			}

			public ShimEnumerator(Dictionary<TKey, TValue> host)
			{
				host_enumerator = host.GetEnumerator();
			}

			public void Dispose()
			{
				host_enumerator.Dispose();
			}

			public bool MoveNext()
			{
				return host_enumerator.MoveNext();
			}

			public void Reset()
			{
				host_enumerator.Reset();
			}
		}

		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
		{
			private Dictionary<TKey, TValue> dictionary;

			private int next;

			private int stamp;

			internal KeyValuePair<TKey, TValue> current;

			object IEnumerator.Current
			{
				get
				{
					VerifyCurrent();
					return current;
				}
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					VerifyCurrent();
					return new DictionaryEntry(current.Key, current.Value);
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					return CurrentKey;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					return CurrentValue;
				}
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return current;
				}
			}

			internal TKey CurrentKey
			{
				get
				{
					VerifyCurrent();
					return current.Key;
				}
			}

			internal TValue CurrentValue
			{
				get
				{
					VerifyCurrent();
					return current.Value;
				}
			}

			internal Enumerator(Dictionary<TKey, TValue> dictionary)
			{
				this.dictionary = dictionary;
				stamp = dictionary.generation;
			}

			void IEnumerator.Reset()
			{
				Reset();
			}

			public bool MoveNext()
			{
				VerifyState();
				if (next < 0)
				{
					return false;
				}
				while (next < dictionary.touchedSlots)
				{
					int num = next++;
					if ((dictionary.linkSlots[num].HashCode & int.MinValue) != 0)
					{
						current = new KeyValuePair<TKey, TValue>(dictionary.keySlots[num], dictionary.valueSlots[num]);
						return true;
					}
				}
				next = -1;
				return false;
			}

			internal void Reset()
			{
				VerifyState();
				next = 0;
			}

			private void VerifyState()
			{
				if (dictionary == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (dictionary.generation != stamp)
				{
					throw new InvalidOperationException("out of sync");
				}
			}

			private void VerifyCurrent()
			{
				VerifyState();
				if (next <= 0)
				{
					throw new InvalidOperationException("Current is not valid");
				}
			}

			public void Dispose()
			{
				dictionary = null;
			}
		}

		[Serializable]
		[DebuggerDisplay("Count={Count}")]
		[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
		public sealed class KeyCollection : IEnumerable, ICollection, ICollection<TKey>, IEnumerable<TKey>
		{
			[Serializable]
			public struct Enumerator : IEnumerator, IDisposable, IEnumerator<TKey>
			{
				private Dictionary<TKey, TValue>.Enumerator host_enumerator;

				object IEnumerator.Current
				{
					get
					{
						return host_enumerator.CurrentKey;
					}
				}

				public TKey Current
				{
					get
					{
						return host_enumerator.current.Key;
					}
				}

				internal Enumerator(Dictionary<TKey, TValue> host)
				{
					host_enumerator = host.GetEnumerator();
				}

				void IEnumerator.Reset()
				{
					host_enumerator.Reset();
				}

				public void Dispose()
				{
					host_enumerator.Dispose();
				}

				public bool MoveNext()
				{
					return host_enumerator.MoveNext();
				}
			}

			private Dictionary<TKey, TValue> dictionary;

			bool ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
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
					return ((ICollection)dictionary).SyncRoot;
				}
			}

			public int Count
			{
				get
				{
					return dictionary.Count;
				}
			}

			public KeyCollection(Dictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this.dictionary = dictionary;
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return dictionary.ContainsKey(item);
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				TKey[] array2 = array as TKey[];
				if (array2 != null)
				{
					CopyTo(array2, index);
					return;
				}
				dictionary.CopyToCheck(array, index);
				dictionary.Do_ICollectionCopyTo(array, index, Dictionary<TKey, TValue>.pick_key);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void CopyTo(TKey[] array, int index)
			{
				dictionary.CopyToCheck(array, index);
				dictionary.Do_CopyTo(array, index, Dictionary<TKey, TValue>.pick_key);
			}

			public Enumerator GetEnumerator()
			{
				return new Enumerator(dictionary);
			}
		}

		[Serializable]
		[DebuggerTypeProxy(typeof(CollectionDebuggerView<, >))]
		[DebuggerDisplay("Count={Count}")]
		public sealed class ValueCollection : IEnumerable, ICollection, ICollection<TValue>, IEnumerable<TValue>
		{
			[Serializable]
			public struct Enumerator : IEnumerator, IDisposable, IEnumerator<TValue>
			{
				private Dictionary<TKey, TValue>.Enumerator host_enumerator;

				object IEnumerator.Current
				{
					get
					{
						return host_enumerator.CurrentValue;
					}
				}

				public TValue Current
				{
					get
					{
						return host_enumerator.current.Value;
					}
				}

				internal Enumerator(Dictionary<TKey, TValue> host)
				{
					host_enumerator = host.GetEnumerator();
				}

				void IEnumerator.Reset()
				{
					host_enumerator.Reset();
				}

				public void Dispose()
				{
					host_enumerator.Dispose();
				}

				public bool MoveNext()
				{
					return host_enumerator.MoveNext();
				}
			}

			private Dictionary<TKey, TValue> dictionary;

			bool ICollection<TValue>.IsReadOnly
			{
				get
				{
					return true;
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
					return ((ICollection)dictionary).SyncRoot;
				}
			}

			public int Count
			{
				get
				{
					return dictionary.Count;
				}
			}

			public ValueCollection(Dictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this.dictionary = dictionary;
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return dictionary.ContainsValue(item);
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException("this is a read-only collection");
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				TValue[] array2 = array as TValue[];
				if (array2 != null)
				{
					CopyTo(array2, index);
					return;
				}
				dictionary.CopyToCheck(array, index);
				dictionary.Do_ICollectionCopyTo(array, index, Dictionary<TKey, TValue>.pick_value);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void CopyTo(TValue[] array, int index)
			{
				dictionary.CopyToCheck(array, index);
				dictionary.Do_CopyTo(array, index, Dictionary<TKey, TValue>.pick_value);
			}

			public Enumerator GetEnumerator()
			{
				return new Enumerator(dictionary);
			}
		}

		private delegate TRet Transform<TRet>(TKey key, TValue value);

		private const int INITIAL_SIZE = 10;

		private const float DEFAULT_LOAD_FACTOR = 0.9f;

		private const int NO_SLOT = -1;

		private const int HASH_FLAG = int.MinValue;

		private int[] table;

		private Link[] linkSlots;

		private TKey[] keySlots;

		private TValue[] valueSlots;

		private int touchedSlots;

		private int emptySlot;

		private int count;

		private int threshold;

		private IEqualityComparer<TKey> hcp;

		private SerializationInfo serialization_info;

		private int generation;

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				return Keys;
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				return Values;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return Values;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (key is TKey && ContainsKey((TKey)key))
				{
					return this[ToTKey(key)];
				}
				return null;
			}
			set
			{
				this[ToTKey(key)] = ToTValue(value);
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

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
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

		public TValue this[TKey key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int num = hcp.GetHashCode(key) | int.MinValue;
				for (int num2 = table[(num & 0x7FFFFFFF) % table.Length] - 1; num2 != -1; num2 = linkSlots[num2].Next)
				{
					if (linkSlots[num2].HashCode == num && hcp.Equals(keySlots[num2], key))
					{
						return valueSlots[num2];
					}
				}
				throw new KeyNotFoundException();
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				int num = hcp.GetHashCode(key) | int.MinValue;
				int num2 = (num & 0x7FFFFFFF) % table.Length;
				int num3 = table[num2] - 1;
				int num4 = -1;
				if (num3 != -1)
				{
					while (linkSlots[num3].HashCode != num || !hcp.Equals(keySlots[num3], key))
					{
						num4 = num3;
						num3 = linkSlots[num3].Next;
						if (num3 == -1)
						{
							break;
						}
					}
				}
				if (num3 == -1)
				{
					if (++count > threshold)
					{
						Resize();
						num2 = (num & 0x7FFFFFFF) % table.Length;
					}
					num3 = emptySlot;
					if (num3 == -1)
					{
						num3 = touchedSlots++;
					}
					else
					{
						emptySlot = linkSlots[num3].Next;
					}
					linkSlots[num3].Next = table[num2] - 1;
					table[num2] = num3 + 1;
					linkSlots[num3].HashCode = num;
					keySlots[num3] = key;
				}
				else if (num4 != -1)
				{
					linkSlots[num4].Next = linkSlots[num3].Next;
					linkSlots[num3].Next = table[num2] - 1;
					table[num2] = num3 + 1;
				}
				valueSlots[num3] = value;
				generation++;
			}
		}

		public IEqualityComparer<TKey> Comparer
		{
			get
			{
				return hcp;
			}
		}

		public KeyCollection Keys
		{
			get
			{
				return new KeyCollection(this);
			}
		}

		public ValueCollection Values
		{
			get
			{
				return new ValueCollection(this);
			}
		}

		public Dictionary()
		{
			Init(10, null);
		}

		public Dictionary(IEqualityComparer<TKey> comparer)
		{
			Init(10, comparer);
		}

		public Dictionary(IDictionary<TKey, TValue> dictionary)
			: this(dictionary, (IEqualityComparer<TKey>)null)
		{
		}

		public Dictionary(int capacity)
		{
			Init(capacity, null);
		}

		public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			int capacity = dictionary.Count;
			Init(capacity, comparer);
			foreach (KeyValuePair<TKey, TValue> item in dictionary)
			{
				Add(item.Key, item.Value);
			}
		}

		public Dictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			Init(capacity, comparer);
		}

		protected Dictionary(SerializationInfo info, StreamingContext context)
		{
			serialization_info = info;
		}

		void IDictionary.Add(object key, object value)
		{
			Add(ToTKey(key), ToTValue(value));
		}

		bool IDictionary.Contains(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key is TKey)
			{
				return ContainsKey((TKey)key);
			}
			return false;
		}

		void IDictionary.Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key is TKey)
			{
				Remove((TKey)key);
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			Add(keyValuePair.Key, keyValuePair.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			return ContainsKeyValuePair(keyValuePair);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			CopyTo(array, index);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			if (!ContainsKeyValuePair(keyValuePair))
			{
				return false;
			}
			return Remove(keyValuePair.Key);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			KeyValuePair<TKey, TValue>[] array2 = array as KeyValuePair<TKey, TValue>[];
			if (array2 != null)
			{
				CopyTo(array2, index);
				return;
			}
			CopyToCheck(array, index);
			DictionaryEntry[] array3 = array as DictionaryEntry[];
			if (array3 != null)
			{
				Do_CopyTo(array3, index, (TKey key, TValue value) => new DictionaryEntry(key, value));
			}
			else
			{
				Do_ICollectionCopyTo(array, index, make_pair);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new ShimEnumerator(this);
		}

		private void Init(int capacity, IEqualityComparer<TKey> hcp)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			object equalityComparer;
			if (hcp != null)
			{
				equalityComparer = hcp;
			}
			else
			{
				equalityComparer = EqualityComparer<TKey>.Default;
			}
			this.hcp = (IEqualityComparer<TKey>)equalityComparer;
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
			linkSlots = new Link[size];
			emptySlot = -1;
			keySlots = new TKey[size];
			valueSlots = new TValue[size];
			touchedSlots = 0;
			threshold = (int)((float)table.Length * 0.9f);
			if (threshold == 0 && table.Length > 0)
			{
				threshold = 1;
			}
		}

		private void CopyToCheck(Array array, int index)
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
			if (array.Length - index < Count)
			{
				throw new ArgumentException("Destination array cannot hold the requested elements!");
			}
		}

		private void Do_CopyTo<TRet, TElem>(TElem[] array, int index, Transform<TRet> transform) where TRet : TElem
		{
			for (int i = 0; i < touchedSlots; i++)
			{
				if ((linkSlots[i].HashCode & int.MinValue) != 0)
				{
					array[index++] = (TElem)(object)transform(keySlots[i], valueSlots[i]);
				}
			}
		}

		private static KeyValuePair<TKey, TValue> make_pair(TKey key, TValue value)
		{
			return new KeyValuePair<TKey, TValue>(key, value);
		}

		private static TKey pick_key(TKey key, TValue value)
		{
			return key;
		}

		private static TValue pick_value(TKey key, TValue value)
		{
			return value;
		}

		private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			CopyToCheck(array, index);
			Do_CopyTo(array, index, make_pair);
		}

		private void Do_ICollectionCopyTo<TRet>(Array array, int index, Transform<TRet> transform)
		{
			Type typeFromHandle = typeof(TRet);
			Type elementType = array.GetType().GetElementType();
			try
			{
				if ((typeFromHandle.IsPrimitive || elementType.IsPrimitive) && !elementType.IsAssignableFrom(typeFromHandle))
				{
					throw new Exception();
				}
				Do_CopyTo((object[])array, index, transform);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Cannot copy source collection elements to destination array", "array", innerException);
			}
		}

		private void Resize()
		{
			int num = Hashtable.ToPrime((table.Length << 1) | 1);
			int[] array = new int[num];
			Link[] array2 = new Link[num];
			for (int i = 0; i < table.Length; i++)
			{
				for (int num2 = table[i] - 1; num2 != -1; num2 = linkSlots[num2].Next)
				{
					int num3 = ((array2[num2].HashCode = hcp.GetHashCode(keySlots[num2]) | int.MinValue) & 0x7FFFFFFF) % num;
					array2[num2].Next = array[num3] - 1;
					array[num3] = num2 + 1;
				}
			}
			table = array;
			linkSlots = array2;
			TKey[] destinationArray = new TKey[num];
			TValue[] destinationArray2 = new TValue[num];
			Array.Copy(keySlots, 0, destinationArray, 0, touchedSlots);
			Array.Copy(valueSlots, 0, destinationArray2, 0, touchedSlots);
			keySlots = destinationArray;
			valueSlots = destinationArray2;
			threshold = (int)((float)num * 0.9f);
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = hcp.GetHashCode(key) | int.MinValue;
			int num2 = (num & 0x7FFFFFFF) % table.Length;
			int num3;
			for (num3 = table[num2] - 1; num3 != -1; num3 = linkSlots[num3].Next)
			{
				if (linkSlots[num3].HashCode == num && hcp.Equals(keySlots[num3], key))
				{
					throw new ArgumentException("An element with the same key already exists in the dictionary.");
				}
			}
			if (++count > threshold)
			{
				Resize();
				num2 = (num & 0x7FFFFFFF) % table.Length;
			}
			num3 = emptySlot;
			if (num3 == -1)
			{
				num3 = touchedSlots++;
			}
			else
			{
				emptySlot = linkSlots[num3].Next;
			}
			linkSlots[num3].HashCode = num;
			linkSlots[num3].Next = table[num2] - 1;
			table[num2] = num3 + 1;
			keySlots[num3] = key;
			valueSlots[num3] = value;
			generation++;
		}

		public void Clear()
		{
			count = 0;
			Array.Clear(table, 0, table.Length);
			Array.Clear(keySlots, 0, keySlots.Length);
			Array.Clear(valueSlots, 0, valueSlots.Length);
			Array.Clear(linkSlots, 0, linkSlots.Length);
			emptySlot = -1;
			touchedSlots = 0;
			generation++;
		}

		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = hcp.GetHashCode(key) | int.MinValue;
			for (int num2 = table[(num & 0x7FFFFFFF) % table.Length] - 1; num2 != -1; num2 = linkSlots[num2].Next)
			{
				if (linkSlots[num2].HashCode == num && hcp.Equals(keySlots[num2], key))
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsValue(TValue value)
		{
			IEqualityComparer<TValue> equalityComparer = EqualityComparer<TValue>.Default;
			for (int i = 0; i < table.Length; i++)
			{
				for (int num = table[i] - 1; num != -1; num = linkSlots[num].Next)
				{
					if (equalityComparer.Equals(valueSlots[num], value))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", generation);
			info.AddValue("Comparer", hcp);
			KeyValuePair<TKey, TValue>[] array = null;
			if (count > 0)
			{
				array = new KeyValuePair<TKey, TValue>[count];
				CopyTo(array, 0);
			}
			info.AddValue("HashSize", table.Length);
			info.AddValue("KeyValuePairs", array);
		}

		public virtual void OnDeserialization(object sender)
		{
			if (serialization_info == null)
			{
				return;
			}
			generation = serialization_info.GetInt32("Version");
			hcp = (IEqualityComparer<TKey>)serialization_info.GetValue("Comparer", typeof(IEqualityComparer<TKey>));
			int num = serialization_info.GetInt32("HashSize");
			KeyValuePair<TKey, TValue>[] array = (KeyValuePair<TKey, TValue>[])serialization_info.GetValue("KeyValuePairs", typeof(KeyValuePair<TKey, TValue>[]));
			if (num < 10)
			{
				num = 10;
			}
			InitArrays(num);
			count = 0;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					Add(array[i].Key, array[i].Value);
				}
			}
			generation++;
			serialization_info = null;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = hcp.GetHashCode(key) | int.MinValue;
			int num2 = (num & 0x7FFFFFFF) % table.Length;
			int num3 = table[num2] - 1;
			if (num3 == -1)
			{
				return false;
			}
			int num4 = -1;
			while (linkSlots[num3].HashCode != num || !hcp.Equals(keySlots[num3], key))
			{
				num4 = num3;
				num3 = linkSlots[num3].Next;
				if (num3 == -1)
				{
					break;
				}
			}
			if (num3 == -1)
			{
				return false;
			}
			count--;
			if (num4 == -1)
			{
				table[num2] = linkSlots[num3].Next + 1;
			}
			else
			{
				linkSlots[num4].Next = linkSlots[num3].Next;
			}
			linkSlots[num3].Next = emptySlot;
			emptySlot = num3;
			linkSlots[num3].HashCode = 0;
			keySlots[num3] = default(TKey);
			valueSlots[num3] = default(TValue);
			generation++;
			return true;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = hcp.GetHashCode(key) | int.MinValue;
			for (int num2 = table[(num & 0x7FFFFFFF) % table.Length] - 1; num2 != -1; num2 = linkSlots[num2].Next)
			{
				if (linkSlots[num2].HashCode == num && hcp.Equals(keySlots[num2], key))
				{
					value = valueSlots[num2];
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		private TKey ToTKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!(key is TKey))
			{
				throw new ArgumentException("not of type: " + typeof(TKey).ToString(), "key");
			}
			return (TKey)key;
		}

		private TValue ToTValue(object value)
		{
			if (value == null && !typeof(TValue).IsValueType)
			{
				return default(TValue);
			}
			if (!(value is TValue))
			{
				throw new ArgumentException("not of type: " + typeof(TValue).ToString(), "value");
			}
			return (TValue)value;
		}

		private bool ContainsKeyValuePair(KeyValuePair<TKey, TValue> pair)
		{
			TValue value;
			if (!TryGetValue(pair.Key, out value))
			{
				return false;
			}
			return EqualityComparer<TValue>.Default.Equals(pair.Value, value);
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}
