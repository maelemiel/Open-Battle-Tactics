using System.Runtime.Serialization;

namespace System.Collections.Specialized
{
	[Serializable]
	public class OrderedDictionary : ICollection, IOrderedDictionary, IDictionary, IDeserializationCallback, IEnumerable, ISerializable
	{
		private class OrderedEntryCollectionEnumerator : IEnumerator, IDictionaryEnumerator
		{
			private IEnumerator listEnumerator;

			public object Current
			{
				get
				{
					return listEnumerator.Current;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					return (DictionaryEntry)listEnumerator.Current;
				}
			}

			public object Key
			{
				get
				{
					return Entry.Key;
				}
			}

			public object Value
			{
				get
				{
					return Entry.Value;
				}
			}

			public OrderedEntryCollectionEnumerator(IEnumerator listEnumerator)
			{
				this.listEnumerator = listEnumerator;
			}

			public bool MoveNext()
			{
				return listEnumerator.MoveNext();
			}

			public void Reset()
			{
				listEnumerator.Reset();
			}
		}

		private class OrderedCollection : ICollection, IEnumerable
		{
			private class OrderedCollectionEnumerator : IEnumerator
			{
				private bool isKeyList;

				private IEnumerator listEnumerator;

				public object Current
				{
					get
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)listEnumerator.Current;
						return (!isKeyList) ? dictionaryEntry.Value : dictionaryEntry.Key;
					}
				}

				public OrderedCollectionEnumerator(IEnumerator listEnumerator, bool isKeyList)
				{
					this.listEnumerator = listEnumerator;
					this.isKeyList = isKeyList;
				}

				public bool MoveNext()
				{
					return listEnumerator.MoveNext();
				}

				public void Reset()
				{
					listEnumerator.Reset();
				}
			}

			private ArrayList list;

			private bool isKeyList;

			public int Count
			{
				get
				{
					return list.Count;
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
					return list.SyncRoot;
				}
			}

			public OrderedCollection(ArrayList list, bool isKeyList)
			{
				this.list = list;
				this.isKeyList = isKeyList;
			}

			public void CopyTo(Array array, int index)
			{
				for (int i = 0; i < list.Count; i++)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)list[i];
					if (isKeyList)
					{
						array.SetValue(dictionaryEntry.Key, index + i);
					}
					else
					{
						array.SetValue(dictionaryEntry.Value, index + i);
					}
				}
			}

			public IEnumerator GetEnumerator()
			{
				return new OrderedCollectionEnumerator(list.GetEnumerator(), isKeyList);
			}
		}

		private ArrayList list;

		private Hashtable hash;

		private bool readOnly;

		private int initialCapacity;

		private SerializationInfo serializationInfo;

		private IEqualityComparer comparer;

		bool ICollection.IsSynchronized
		{
			get
			{
				return list.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return list.SyncRoot;
			}
		}

		bool IDictionary.IsFixedSize
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
				return list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return readOnly;
			}
		}

		public object this[object key]
		{
			get
			{
				return hash[key];
			}
			set
			{
				WriteCheck();
				if (hash.Contains(key))
				{
					int index = FindListEntry(key);
					list[index] = new DictionaryEntry(key, value);
				}
				else
				{
					list.Add(new DictionaryEntry(key, value));
				}
				hash[key] = value;
			}
		}

		public object this[int index]
		{
			get
			{
				return ((DictionaryEntry)list[index]).Value;
			}
			set
			{
				WriteCheck();
				DictionaryEntry dictionaryEntry = (DictionaryEntry)list[index];
				dictionaryEntry.Value = value;
				list[index] = dictionaryEntry;
				hash[dictionaryEntry.Key] = value;
			}
		}

		public ICollection Keys
		{
			get
			{
				return new OrderedCollection(list, true);
			}
		}

		public ICollection Values
		{
			get
			{
				return new OrderedCollection(list, false);
			}
		}

		public OrderedDictionary()
		{
			list = new ArrayList();
			hash = new Hashtable();
		}

		public OrderedDictionary(int capacity)
		{
			initialCapacity = ((capacity >= 0) ? capacity : 0);
			list = new ArrayList(initialCapacity);
			hash = new Hashtable(initialCapacity);
		}

		public OrderedDictionary(IEqualityComparer equalityComparer)
		{
			list = new ArrayList();
			hash = new Hashtable(equalityComparer);
			comparer = equalityComparer;
		}

		public OrderedDictionary(int capacity, IEqualityComparer equalityComparer)
		{
			initialCapacity = ((capacity >= 0) ? capacity : 0);
			list = new ArrayList(initialCapacity);
			hash = new Hashtable(initialCapacity, equalityComparer);
			comparer = equalityComparer;
		}

		protected OrderedDictionary(SerializationInfo info, StreamingContext context)
		{
			serializationInfo = info;
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			if (serializationInfo != null)
			{
				comparer = (IEqualityComparer)serializationInfo.GetValue("KeyComparer", typeof(IEqualityComparer));
				readOnly = serializationInfo.GetBoolean("ReadOnly");
				initialCapacity = serializationInfo.GetInt32("InitialCapacity");
				if (list == null)
				{
					list = new ArrayList();
				}
				else
				{
					list.Clear();
				}
				hash = new Hashtable(comparer);
				object[] array = (object[])serializationInfo.GetValue("ArrayList", typeof(object[]));
				object[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)array2[i];
					hash.Add(dictionaryEntry.Key, dictionaryEntry.Value);
					list.Add(dictionaryEntry);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		protected virtual void OnDeserialization(object sender)
		{
			((IDeserializationCallback)this).OnDeserialization(sender);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("KeyComparer", comparer, typeof(IEqualityComparer));
			info.AddValue("ReadOnly", readOnly);
			info.AddValue("InitialCapacity", initialCapacity);
			object[] array = new object[hash.Count];
			hash.CopyTo(array, 0);
			info.AddValue("ArrayList", array);
		}

		public void CopyTo(Array array, int index)
		{
			list.CopyTo(array, index);
		}

		public void Add(object key, object value)
		{
			WriteCheck();
			hash.Add(key, value);
			list.Add(new DictionaryEntry(key, value));
		}

		public void Clear()
		{
			WriteCheck();
			hash.Clear();
			list.Clear();
		}

		public bool Contains(object key)
		{
			return hash.Contains(key);
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new OrderedEntryCollectionEnumerator(list.GetEnumerator());
		}

		public void Remove(object key)
		{
			WriteCheck();
			if (hash.Contains(key))
			{
				hash.Remove(key);
				int index = FindListEntry(key);
				list.RemoveAt(index);
			}
		}

		private int FindListEntry(object key)
		{
			for (int i = 0; i < list.Count; i++)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)list[i];
				if ((comparer == null) ? dictionaryEntry.Key.Equals(key) : comparer.Equals(dictionaryEntry.Key, key))
				{
					return i;
				}
			}
			return -1;
		}

		private void WriteCheck()
		{
			if (readOnly)
			{
				throw new NotSupportedException("Collection is read only");
			}
		}

		public OrderedDictionary AsReadOnly()
		{
			OrderedDictionary orderedDictionary = new OrderedDictionary();
			orderedDictionary.list = list;
			orderedDictionary.hash = hash;
			orderedDictionary.comparer = comparer;
			orderedDictionary.readOnly = true;
			return orderedDictionary;
		}

		public void Insert(int index, object key, object value)
		{
			WriteCheck();
			hash.Add(key, value);
			list.Insert(index, new DictionaryEntry(key, value));
		}

		public void RemoveAt(int index)
		{
			WriteCheck();
			DictionaryEntry dictionaryEntry = (DictionaryEntry)list[index];
			list.RemoveAt(index);
			hash.Remove(dictionaryEntry.Key);
		}
	}
}
