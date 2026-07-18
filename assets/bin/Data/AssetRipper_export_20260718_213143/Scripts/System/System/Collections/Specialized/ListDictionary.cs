namespace System.Collections.Specialized
{
	[Serializable]
	public class ListDictionary : ICollection, IDictionary, IEnumerable
	{
		[Serializable]
		private class DictionaryNode
		{
			public object key;

			public object value;

			public DictionaryNode next;

			public DictionaryNode(object key, object value, DictionaryNode next)
			{
				this.key = key;
				this.value = value;
				this.next = next;
			}
		}

		private class DictionaryNodeEnumerator : IEnumerator, IDictionaryEnumerator
		{
			private ListDictionary dict;

			private bool isAtStart;

			private DictionaryNode current;

			private int version;

			public object Current
			{
				get
				{
					return Entry;
				}
			}

			private DictionaryNode DictionaryNode
			{
				get
				{
					FailFast();
					if (current == null)
					{
						throw new InvalidOperationException("Enumerator is positioned before the collection's first element or after the last element.");
					}
					return current;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					object key = DictionaryNode.key;
					return new DictionaryEntry(key, current.value);
				}
			}

			public object Key
			{
				get
				{
					return DictionaryNode.key;
				}
			}

			public object Value
			{
				get
				{
					return DictionaryNode.value;
				}
			}

			public DictionaryNodeEnumerator(ListDictionary dict)
			{
				this.dict = dict;
				version = dict.version;
				Reset();
			}

			private void FailFast()
			{
				if (version != dict.version)
				{
					throw new InvalidOperationException("The ListDictionary's contents changed after this enumerator was instantiated.");
				}
			}

			public bool MoveNext()
			{
				FailFast();
				if (current == null && !isAtStart)
				{
					return false;
				}
				current = ((!isAtStart) ? current.next : dict.head);
				isAtStart = false;
				return current != null;
			}

			public void Reset()
			{
				FailFast();
				isAtStart = true;
				current = null;
			}
		}

		private class DictionaryNodeCollection : ICollection, IEnumerable
		{
			private class DictionaryNodeCollectionEnumerator : IEnumerator
			{
				private IDictionaryEnumerator inner;

				private bool isKeyList;

				public object Current
				{
					get
					{
						return (!isKeyList) ? inner.Value : inner.Key;
					}
				}

				public DictionaryNodeCollectionEnumerator(IDictionaryEnumerator inner, bool isKeyList)
				{
					this.inner = inner;
					this.isKeyList = isKeyList;
				}

				public bool MoveNext()
				{
					return inner.MoveNext();
				}

				public void Reset()
				{
					inner.Reset();
				}
			}

			private ListDictionary dict;

			private bool isKeyList;

			public int Count
			{
				get
				{
					return dict.Count;
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
					return dict.SyncRoot;
				}
			}

			public DictionaryNodeCollection(ListDictionary dict, bool isKeyList)
			{
				this.dict = dict;
				this.isKeyList = isKeyList;
			}

			public void CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array", "Array cannot be null.");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", "index is less than 0");
				}
				if (index > array.Length)
				{
					throw new IndexOutOfRangeException("index is too large");
				}
				if (Count > array.Length - index)
				{
					throw new ArgumentException("Not enough room in the array");
				}
				IEnumerator enumerator = GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						array.SetValue(current, index++);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}

			public IEnumerator GetEnumerator()
			{
				return new DictionaryNodeCollectionEnumerator(dict.GetEnumerator(), isKeyList);
			}
		}

		private int count;

		private int version;

		private DictionaryNode head;

		private IComparer comparer;

		public int Count
		{
			get
			{
				return count;
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
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public object this[object key]
		{
			get
			{
				DictionaryNode dictionaryNode = FindEntry(key);
				return (dictionaryNode != null) ? dictionaryNode.value : null;
			}
			set
			{
				DictionaryNode prev;
				DictionaryNode dictionaryNode = FindEntry(key, out prev);
				if (dictionaryNode != null)
				{
					dictionaryNode.value = value;
				}
				else
				{
					AddImpl(key, value, prev);
				}
			}
		}

		public ICollection Keys
		{
			get
			{
				return new DictionaryNodeCollection(this, true);
			}
		}

		public ICollection Values
		{
			get
			{
				return new DictionaryNodeCollection(this, false);
			}
		}

		public ListDictionary()
		{
			count = 0;
			version = 0;
			comparer = null;
			head = null;
		}

		public ListDictionary(IComparer comparer)
			: this()
		{
			this.comparer = comparer;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new DictionaryNodeEnumerator(this);
		}

		private DictionaryNode FindEntry(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Attempted lookup for a null key.");
			}
			DictionaryNode next = head;
			if (comparer == null)
			{
				while (next != null && !key.Equals(next.key))
				{
					next = next.next;
				}
			}
			else
			{
				while (next != null && comparer.Compare(key, next.key) != 0)
				{
					next = next.next;
				}
			}
			return next;
		}

		private DictionaryNode FindEntry(object key, out DictionaryNode prev)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Attempted lookup for a null key.");
			}
			DictionaryNode next = head;
			prev = null;
			if (comparer == null)
			{
				while (next != null && !key.Equals(next.key))
				{
					prev = next;
					next = next.next;
				}
			}
			else
			{
				while (next != null && comparer.Compare(key, next.key) != 0)
				{
					prev = next;
					next = next.next;
				}
			}
			return next;
		}

		private void AddImpl(object key, object value, DictionaryNode prev)
		{
			if (prev == null)
			{
				head = new DictionaryNode(key, value, head);
			}
			else
			{
				prev.next = new DictionaryNode(key, value, prev.next);
			}
			count++;
			version++;
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Array cannot be null.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "index is less than 0");
			}
			if (index > array.Length)
			{
				throw new IndexOutOfRangeException("index is too large");
			}
			if (Count > array.Length - index)
			{
				throw new ArgumentException("Not enough room in the array");
			}
			IDictionaryEnumerator dictionaryEnumerator = GetEnumerator();
			try
			{
				while (dictionaryEnumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)dictionaryEnumerator.Current;
					array.SetValue(dictionaryEntry, index++);
				}
			}
			finally
			{
				IDisposable disposable = dictionaryEnumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void Add(object key, object value)
		{
			DictionaryNode prev;
			DictionaryNode dictionaryNode = FindEntry(key, out prev);
			if (dictionaryNode != null)
			{
				throw new ArgumentException("key", "Duplicate key in add.");
			}
			AddImpl(key, value, prev);
		}

		public void Clear()
		{
			head = null;
			count = 0;
			version++;
		}

		public bool Contains(object key)
		{
			return FindEntry(key) != null;
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new DictionaryNodeEnumerator(this);
		}

		public void Remove(object key)
		{
			DictionaryNode prev;
			DictionaryNode dictionaryNode = FindEntry(key, out prev);
			if (dictionaryNode != null)
			{
				if (prev == null)
				{
					head = dictionaryNode.next;
				}
				else
				{
					prev.next = dictionaryNode.next;
				}
				dictionaryNode.value = null;
				count--;
				version++;
			}
		}
	}
}
