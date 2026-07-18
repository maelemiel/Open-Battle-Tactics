using System.Globalization;

namespace System.Collections.Specialized
{
	[Serializable]
	public class StringDictionary : IEnumerable
	{
		private Hashtable contents;

		public virtual int Count
		{
			get
			{
				return contents.Count;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual string this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				return (string)contents[key.ToLower(CultureInfo.InvariantCulture)];
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				contents[key.ToLower(CultureInfo.InvariantCulture)] = value;
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				return contents.Keys;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				return contents.Values;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return contents.SyncRoot;
			}
		}

		public StringDictionary()
		{
			contents = new Hashtable();
		}

		public virtual void Add(string key, string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			contents.Add(key.ToLower(CultureInfo.InvariantCulture), value);
		}

		public virtual void Clear()
		{
			contents.Clear();
		}

		public virtual bool ContainsKey(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return contents.ContainsKey(key.ToLower(CultureInfo.InvariantCulture));
		}

		public virtual bool ContainsValue(string value)
		{
			return contents.ContainsValue(value);
		}

		public virtual void CopyTo(Array array, int index)
		{
			contents.CopyTo(array, index);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return contents.GetEnumerator();
		}

		public virtual void Remove(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			contents.Remove(key.ToLower(CultureInfo.InvariantCulture));
		}
	}
}
