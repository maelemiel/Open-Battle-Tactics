namespace System.Collections.Generic
{
	[Serializable]
	public class SortedDictionary<TKey, TValue> : ICollection, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, IEnumerable, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>
	{
		private class Node : System.Collections.Generic.RBTree.Node
		{
			public TKey key;

			public TValue value;

			public Node(TKey key)
			{
				this.key = key;
			}

			public Node(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
			}

			public override void SwapValue(System.Collections.Generic.RBTree.Node other)
			{
				Node node = (Node)other;
				TKey val = key;
				key = node.key;
				node.key = val;
				TValue val2 = value;
				value = node.value;
				node.value = val2;
			}

			public KeyValuePair<TKey, TValue> AsKV()
			{
				return new KeyValuePair<TKey, TValue>(key, value);
			}

			public DictionaryEntry AsDE()
			{
				return new DictionaryEntry(key, value);
			}
		}

		private class NodeHelper : System.Collections.Generic.RBTree.INodeHelper<TKey>
		{
			public IComparer<TKey> cmp;

			private static NodeHelper Default = new NodeHelper(Comparer<TKey>.Default);

			private NodeHelper(IComparer<TKey> cmp)
			{
				this.cmp = cmp;
			}

			public int Compare(TKey key, System.Collections.Generic.RBTree.Node node)
			{
				return cmp.Compare(key, ((Node)node).key);
			}

			public System.Collections.Generic.RBTree.Node CreateNode(TKey key)
			{
				return new Node(key);
			}

			public static NodeHelper GetHelper(IComparer<TKey> cmp)
			{
				if (cmp == null || cmp == Comparer<TKey>.Default)
				{
					return Default;
				}
				return new NodeHelper(cmp);
			}
		}

		[Serializable]
		public sealed class ValueCollection : ICollection, IEnumerable, ICollection<TValue>, IEnumerable<TValue>
		{
			public struct Enumerator : IEnumerator, IDisposable, IEnumerator<TValue>
			{
				private System.Collections.Generic.RBTree.NodeEnumerator host;

				private TValue current;

				object IEnumerator.Current
				{
					get
					{
						host.check_current();
						return current;
					}
				}

				public TValue Current
				{
					get
					{
						return current;
					}
				}

				internal Enumerator(SortedDictionary<TKey, TValue> dic)
				{
					host = dic.tree.GetEnumerator();
				}

				void IEnumerator.Reset()
				{
					host.Reset();
				}

				public bool MoveNext()
				{
					if (!host.MoveNext())
					{
						return false;
					}
					current = ((Node)host.Current).value;
					return true;
				}

				public void Dispose()
				{
					host.Dispose();
				}
			}

			private SortedDictionary<TKey, TValue> _dic;

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
					return _dic;
				}
			}

			public int Count
			{
				get
				{
					return _dic.Count;
				}
			}

			public ValueCollection(SortedDictionary<TKey, TValue> dic)
			{
				_dic = dic;
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return _dic.ContainsValue(item);
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException();
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return GetEnumerator();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (index < 0 || array.Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (array.Length - index < Count)
				{
					throw new ArgumentException();
				}
				foreach (Node item in _dic.tree)
				{
					array.SetValue(item.value, index++);
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new Enumerator(_dic);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				if (Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (arrayIndex < 0 || array.Length <= arrayIndex)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (array.Length - arrayIndex < Count)
				{
					throw new ArgumentException();
				}
				foreach (Node item in _dic.tree)
				{
					array[arrayIndex++] = item.value;
				}
			}

			public Enumerator GetEnumerator()
			{
				return new Enumerator(_dic);
			}
		}

		[Serializable]
		public sealed class KeyCollection : ICollection, IEnumerable, ICollection<TKey>, IEnumerable<TKey>
		{
			public struct Enumerator : IEnumerator, IDisposable, IEnumerator<TKey>
			{
				private System.Collections.Generic.RBTree.NodeEnumerator host;

				private TKey current;

				object IEnumerator.Current
				{
					get
					{
						host.check_current();
						return current;
					}
				}

				public TKey Current
				{
					get
					{
						return current;
					}
				}

				internal Enumerator(SortedDictionary<TKey, TValue> dic)
				{
					host = dic.tree.GetEnumerator();
				}

				void IEnumerator.Reset()
				{
					host.Reset();
				}

				public bool MoveNext()
				{
					if (!host.MoveNext())
					{
						return false;
					}
					current = ((Node)host.Current).key;
					return true;
				}

				public void Dispose()
				{
					host.Dispose();
				}
			}

			private SortedDictionary<TKey, TValue> _dic;

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
					return _dic;
				}
			}

			public int Count
			{
				get
				{
					return _dic.Count;
				}
			}

			public KeyCollection(SortedDictionary<TKey, TValue> dic)
			{
				_dic = dic;
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return _dic.ContainsKey(item);
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return GetEnumerator();
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException();
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (index < 0 || array.Length <= index)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (array.Length - index < Count)
				{
					throw new ArgumentException();
				}
				foreach (Node item in _dic.tree)
				{
					array.SetValue(item.key, index++);
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new Enumerator(_dic);
			}

			public void CopyTo(TKey[] array, int arrayIndex)
			{
				if (Count == 0)
				{
					return;
				}
				if (array == null)
				{
					throw new ArgumentNullException();
				}
				if (arrayIndex < 0 || array.Length <= arrayIndex)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (array.Length - arrayIndex < Count)
				{
					throw new ArgumentException();
				}
				foreach (Node item in _dic.tree)
				{
					array[arrayIndex++] = item.key;
				}
			}

			public Enumerator GetEnumerator()
			{
				return new Enumerator(_dic);
			}
		}

		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
		{
			private System.Collections.Generic.RBTree.NodeEnumerator host;

			private KeyValuePair<TKey, TValue> current;

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					return CurrentNode.AsDE();
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					return CurrentNode.key;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					return CurrentNode.value;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return CurrentNode.AsDE();
				}
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return current;
				}
			}

			private Node CurrentNode
			{
				get
				{
					host.check_current();
					return (Node)host.Current;
				}
			}

			internal Enumerator(SortedDictionary<TKey, TValue> dic)
			{
				host = dic.tree.GetEnumerator();
			}

			void IEnumerator.Reset()
			{
				host.Reset();
			}

			public bool MoveNext()
			{
				if (!host.MoveNext())
				{
					return false;
				}
				current = ((Node)host.Current).AsKV();
				return true;
			}

			public void Dispose()
			{
				host.Dispose();
			}
		}

		private System.Collections.Generic.RBTree tree;

		private NodeHelper hlp;

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				return new KeyCollection(this);
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				return new ValueCollection(this);
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return false;
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

		ICollection IDictionary.Keys
		{
			get
			{
				return new KeyCollection(this);
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return new ValueCollection(this);
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return this[ToKey(key)];
			}
			set
			{
				this[ToKey(key)] = ToValue(value);
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

		public IComparer<TKey> Comparer
		{
			get
			{
				return hlp.cmp;
			}
		}

		public int Count
		{
			get
			{
				return tree.Count;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				Node node = (Node)tree.Lookup(key);
				if (node == null)
				{
					throw new KeyNotFoundException();
				}
				return node.value;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				Node node = (Node)tree.Intern(key, null);
				node.value = value;
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

		public SortedDictionary()
			: this((IComparer<TKey>)null)
		{
		}

		public SortedDictionary(IComparer<TKey> comparer)
		{
			hlp = NodeHelper.GetHelper(comparer);
			tree = new System.Collections.Generic.RBTree(hlp);
		}

		public SortedDictionary(IDictionary<TKey, TValue> dic)
			: this(dic, (IComparer<TKey>)null)
		{
		}

		public SortedDictionary(IDictionary<TKey, TValue> dic, IComparer<TKey> comparer)
			: this(comparer)
		{
			if (dic == null)
			{
				throw new ArgumentNullException();
			}
			foreach (KeyValuePair<TKey, TValue> item in dic)
			{
				Add(item.Key, item.Value);
			}
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			TValue value;
			return TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(item.Value, value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			TValue value;
			return TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(item.Value, value) && Remove(item.Key);
		}

		void IDictionary.Add(object key, object value)
		{
			Add(ToKey(key), ToValue(value));
		}

		bool IDictionary.Contains(object key)
		{
			return ContainsKey(ToKey(key));
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new Enumerator(this);
		}

		void IDictionary.Remove(object key)
		{
			Remove(ToKey(key));
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (Count == 0)
			{
				return;
			}
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (index < 0 || array.Length <= index)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Length - index < Count)
			{
				throw new ArgumentException();
			}
			foreach (Node item in tree)
			{
				array.SetValue(item.AsDE(), index++);
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

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			System.Collections.Generic.RBTree.Node node = new Node(key, value);
			if (tree.Intern(key, node) != node)
			{
				throw new ArgumentException("key already present in dictionary", "key");
			}
		}

		public void Clear()
		{
			tree.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return tree.Lookup(key) != null;
		}

		public bool ContainsValue(TValue value)
		{
			IEqualityComparer<TValue> equalityComparer = EqualityComparer<TValue>.Default;
			foreach (Node item in tree)
			{
				if (equalityComparer.Equals(value, item.value))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (Count == 0)
			{
				return;
			}
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (arrayIndex < 0 || array.Length <= arrayIndex)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Length - arrayIndex < Count)
			{
				throw new ArgumentException();
			}
			foreach (Node item in tree)
			{
				array[arrayIndex++] = item.AsKV();
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		public bool Remove(TKey key)
		{
			return tree.Remove(key) != null;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			Node node = (Node)tree.Lookup(key);
			value = ((node != null) ? node.value : default(TValue));
			return node != null;
		}

		private TKey ToKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!(key is TKey))
			{
				throw new ArgumentException(string.Format("Key \"{0}\" cannot be converted to the key type {1}.", key, typeof(TKey)));
			}
			return (TKey)key;
		}

		private TValue ToValue(object value)
		{
			if (!(value is TValue) && (value != null || typeof(TValue).IsValueType))
			{
				throw new ArgumentException(string.Format("Value \"{0}\" cannot be converted to the value type {1}.", value, typeof(TValue)));
			}
			return (TValue)value;
		}
	}
}
