using System.Runtime.InteropServices;

namespace System.Collections
{
	[Serializable]
	[ComVisible(true)]
	public abstract class DictionaryBase : IEnumerable, ICollection, IDictionary
	{
		private Hashtable hashtable;

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
				object obj = hashtable[key];
				OnGet(key, obj);
				return obj;
			}
			set
			{
				OnValidate(key, value);
				object obj = hashtable[key];
				OnSet(key, obj, value);
				hashtable[key] = value;
				try
				{
					OnSetComplete(key, obj, value);
				}
				catch
				{
					hashtable[key] = obj;
					throw;
				}
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return hashtable.Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return hashtable.Values;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return hashtable.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return hashtable.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return hashtable.Count;
			}
		}

		protected IDictionary Dictionary
		{
			get
			{
				return this;
			}
		}

		protected Hashtable InnerHashtable
		{
			get
			{
				return hashtable;
			}
		}

		protected DictionaryBase()
		{
			hashtable = new Hashtable();
		}

		void IDictionary.Add(object key, object value)
		{
			OnValidate(key, value);
			OnInsert(key, value);
			hashtable.Add(key, value);
			try
			{
				OnInsertComplete(key, value);
			}
			catch
			{
				hashtable.Remove(key);
				throw;
			}
		}

		void IDictionary.Remove(object key)
		{
			if (!hashtable.Contains(key))
			{
				return;
			}
			object value = hashtable[key];
			OnValidate(key, value);
			OnRemove(key, value);
			hashtable.Remove(key);
			try
			{
				OnRemoveComplete(key, value);
			}
			catch
			{
				hashtable[key] = value;
				throw;
			}
		}

		bool IDictionary.Contains(object key)
		{
			return hashtable.Contains(key);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return hashtable.GetEnumerator();
		}

		public void Clear()
		{
			OnClear();
			hashtable.Clear();
			OnClearComplete();
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index must be possitive");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException("array is multidimensional");
			}
			int length = array.Length;
			if (index > length)
			{
				throw new ArgumentException("index is larger than array size");
			}
			if (index + Count > length)
			{
				throw new ArgumentException("Copy will overlflow array");
			}
			DoCopy(array, index);
		}

		private void DoCopy(Array array, int index)
		{
			foreach (DictionaryEntry item in hashtable)
			{
				array.SetValue(item, index++);
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return hashtable.GetEnumerator();
		}

		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual object OnGet(object key, object currentValue)
		{
			return currentValue;
		}

		protected virtual void OnInsert(object key, object value)
		{
		}

		protected virtual void OnInsertComplete(object key, object value)
		{
		}

		protected virtual void OnSet(object key, object oldValue, object newValue)
		{
		}

		protected virtual void OnSetComplete(object key, object oldValue, object newValue)
		{
		}

		protected virtual void OnRemove(object key, object value)
		{
		}

		protected virtual void OnRemoveComplete(object key, object value)
		{
		}

		protected virtual void OnValidate(object key, object value)
		{
		}
	}
}
