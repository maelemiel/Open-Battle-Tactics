namespace System.Collections.Specialized
{
	[Serializable]
	public class HybridDictionary : ICollection, IDictionary, IEnumerable
	{
		private const int switchAfter = 10;

		private bool caseInsensitive;

		private Hashtable hashtable;

		private ListDictionary list;

		private IDictionary inner
		{
			get
			{
				object result;
				if (list == null)
				{
					IDictionary dictionary = hashtable;
					result = dictionary;
				}
				else
				{
					result = list;
				}
				return (IDictionary)result;
			}
		}

		public int Count
		{
			get
			{
				return inner.Count;
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

		public bool IsSynchronized
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
				return inner[key];
			}
			set
			{
				inner[key] = value;
				if (list != null && Count > 10)
				{
					Switch();
				}
			}
		}

		public ICollection Keys
		{
			get
			{
				return inner.Keys;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public ICollection Values
		{
			get
			{
				return inner.Values;
			}
		}

		public HybridDictionary()
			: this(0, false)
		{
		}

		public HybridDictionary(bool caseInsensitive)
			: this(0, caseInsensitive)
		{
		}

		public HybridDictionary(int initialSize)
			: this(initialSize, false)
		{
		}

		public HybridDictionary(int initialSize, bool caseInsensitive)
		{
			this.caseInsensitive = caseInsensitive;
			IComparer comparer = ((!caseInsensitive) ? null : CaseInsensitiveComparer.DefaultInvariant);
			IHashCodeProvider hcp = ((!caseInsensitive) ? null : CaseInsensitiveHashCodeProvider.DefaultInvariant);
			if (initialSize <= 10)
			{
				list = new ListDictionary(comparer);
			}
			else
			{
				hashtable = new Hashtable(initialSize, hcp, comparer);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(object key, object value)
		{
			inner.Add(key, value);
			if (list != null && Count > 10)
			{
				Switch();
			}
		}

		public void Clear()
		{
			inner.Clear();
		}

		public bool Contains(object key)
		{
			return inner.Contains(key);
		}

		public void CopyTo(Array array, int index)
		{
			inner.CopyTo(array, index);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		public void Remove(object key)
		{
			inner.Remove(key);
		}

		private void Switch()
		{
			IComparer comparer = ((!caseInsensitive) ? null : CaseInsensitiveComparer.DefaultInvariant);
			IHashCodeProvider hcp = ((!caseInsensitive) ? null : CaseInsensitiveHashCodeProvider.DefaultInvariant);
			hashtable = new Hashtable(list, hcp, comparer);
			list.Clear();
			list = null;
		}
	}
}
