using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel
{
	[Serializable]
	[ComVisible(false)]
	public abstract class KeyedCollection<TKey, TItem> : Collection<TItem>
	{
		private Dictionary<TKey, TItem> dictionary;

		private IEqualityComparer<TKey> comparer;

		private int dictionaryCreationThreshold;

		public IEqualityComparer<TKey> Comparer
		{
			get
			{
				return comparer;
			}
		}

		public TItem this[TKey key]
		{
			get
			{
				if (dictionary != null)
				{
					return dictionary[key];
				}
				int num = IndexOfKey(key);
				if (num >= 0)
				{
					return base[num];
				}
				throw new KeyNotFoundException();
			}
		}

		protected IDictionary<TKey, TItem> Dictionary
		{
			get
			{
				return dictionary;
			}
		}

		protected KeyedCollection()
			: this((IEqualityComparer<TKey>)null, 0)
		{
		}

		protected KeyedCollection(IEqualityComparer<TKey> comparer)
			: this(comparer, 0)
		{
		}

		protected KeyedCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
		{
			if (comparer != null)
			{
				this.comparer = comparer;
			}
			else
			{
				this.comparer = EqualityComparer<TKey>.Default;
			}
			this.dictionaryCreationThreshold = dictionaryCreationThreshold;
			if (dictionaryCreationThreshold == 0)
			{
				dictionary = new Dictionary<TKey, TItem>(this.comparer);
			}
		}

		public bool Contains(TKey key)
		{
			if (dictionary != null)
			{
				return dictionary.ContainsKey(key);
			}
			return IndexOfKey(key) >= 0;
		}

		private int IndexOfKey(TKey key)
		{
			for (int num = Count - 1; num >= 0; num--)
			{
				TKey keyForItem = GetKeyForItem(this[num]);
				if (comparer.Equals(key, keyForItem))
				{
					return num;
				}
			}
			return -1;
		}

		public bool Remove(TKey key)
		{
			if (dictionary != null)
			{
				TItem value;
				if (dictionary.TryGetValue(key, out value))
				{
					return Remove(value);
				}
				return false;
			}
			int num = IndexOfKey(key);
			if (num == -1)
			{
				return false;
			}
			RemoveAt(num);
			return true;
		}

		protected void ChangeItemKey(TItem item, TKey newKey)
		{
			if (!Contains(item))
			{
				throw new ArgumentException();
			}
			TKey keyForItem = GetKeyForItem(item);
			if (comparer.Equals(keyForItem, newKey))
			{
				return;
			}
			if (Contains(newKey))
			{
				throw new ArgumentException();
			}
			if (dictionary != null)
			{
				if (!dictionary.Remove(keyForItem))
				{
					throw new ArgumentException();
				}
				dictionary.Add(newKey, item);
			}
		}

		protected override void ClearItems()
		{
			if (dictionary != null)
			{
				dictionary.Clear();
			}
			base.ClearItems();
		}

		protected abstract TKey GetKeyForItem(TItem item);

		protected override void InsertItem(int index, TItem item)
		{
			TKey keyForItem = GetKeyForItem(item);
			if (keyForItem == null)
			{
				throw new ArgumentNullException("GetKeyForItem(item)");
			}
			if (dictionary != null && dictionary.ContainsKey(keyForItem))
			{
				throw new ArgumentException("An element with the same key already exists in the dictionary.");
			}
			if (dictionary == null)
			{
				for (int i = 0; i < Count; i++)
				{
					if (comparer.Equals(keyForItem, GetKeyForItem(this[i])))
					{
						throw new ArgumentException("An element with the same key already exists in the dictionary.");
					}
				}
			}
			base.InsertItem(index, item);
			if (dictionary != null)
			{
				dictionary.Add(keyForItem, item);
			}
			else if (dictionaryCreationThreshold != -1 && Count > dictionaryCreationThreshold)
			{
				dictionary = new Dictionary<TKey, TItem>(comparer);
				for (int j = 0; j < Count; j++)
				{
					TItem val = this[j];
					dictionary.Add(GetKeyForItem(val), val);
				}
			}
		}

		protected override void RemoveItem(int index)
		{
			if (dictionary != null)
			{
				TKey keyForItem = GetKeyForItem(this[index]);
				dictionary.Remove(keyForItem);
			}
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, TItem item)
		{
			if (dictionary != null)
			{
				dictionary.Remove(GetKeyForItem(this[index]));
				dictionary.Add(GetKeyForItem(item), item);
			}
			base.SetItem(index, item);
		}
	}
}
