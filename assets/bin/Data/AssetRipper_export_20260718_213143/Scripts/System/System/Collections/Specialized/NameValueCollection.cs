using System.Runtime.Serialization;
using System.Text;

namespace System.Collections.Specialized
{
	[Serializable]
	public class NameValueCollection : NameObjectCollectionBase
	{
		private string[] cachedAllKeys;

		private string[] cachedAll;

		public virtual string[] AllKeys
		{
			get
			{
				if (cachedAllKeys == null)
				{
					cachedAllKeys = BaseGetAllKeys();
				}
				return cachedAllKeys;
			}
		}

		public string this[int index]
		{
			get
			{
				return Get(index);
			}
		}

		public string this[string name]
		{
			get
			{
				return Get(name);
			}
			set
			{
				Set(name, value);
			}
		}

		public NameValueCollection()
		{
		}

		public NameValueCollection(int capacity)
			: base(capacity)
		{
		}

		public NameValueCollection(NameValueCollection col)
			: base((col != null) ? col.EqualityComparer : null, (col != null) ? col.Comparer : null, (col != null) ? col.HashCodeProvider : null)
		{
			if (col == null)
			{
				throw new ArgumentNullException("col");
			}
			Add(col);
		}

		[Obsolete("Use NameValueCollection (IEqualityComparer)")]
		public NameValueCollection(IHashCodeProvider hashProvider, IComparer comparer)
			: base(hashProvider, comparer)
		{
		}

		public NameValueCollection(int capacity, NameValueCollection col)
			: base(capacity, (col != null) ? col.HashCodeProvider : null, (col != null) ? col.Comparer : null)
		{
			Add(col);
		}

		protected NameValueCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		[Obsolete("Use NameValueCollection (IEqualityComparer)")]
		public NameValueCollection(int capacity, IHashCodeProvider hashProvider, IComparer comparer)
			: base(capacity, hashProvider, comparer)
		{
		}

		public NameValueCollection(IEqualityComparer equalityComparer)
			: base(equalityComparer)
		{
		}

		public NameValueCollection(int capacity, IEqualityComparer equalityComparer)
			: base(capacity, equalityComparer)
		{
		}

		public void Add(NameValueCollection c)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			InvalidateCachedArrays();
			int count = c.Count;
			for (int i = 0; i < count; i++)
			{
				string key = c.GetKey(i);
				ArrayList arrayList = (ArrayList)c.BaseGet(i);
				ArrayList arrayList2 = (ArrayList)BaseGet(key);
				if (arrayList2 != null && arrayList != null)
				{
					arrayList2.AddRange(arrayList);
				}
				else if (arrayList != null)
				{
					arrayList2 = new ArrayList(arrayList);
				}
				BaseSet(key, arrayList2);
			}
		}

		public virtual void Add(string name, string val)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			InvalidateCachedArrays();
			ArrayList arrayList = (ArrayList)BaseGet(name);
			if (arrayList == null)
			{
				arrayList = new ArrayList();
				if (val != null)
				{
					arrayList.Add(val);
				}
				BaseAdd(name, arrayList);
			}
			else if (val != null)
			{
				arrayList.Add(val);
			}
		}

		public virtual void Clear()
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			InvalidateCachedArrays();
			BaseClear();
		}

		public void CopyTo(Array dest, int index)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest", "Null argument - dest");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "index is less than 0");
			}
			if (dest.Rank > 1)
			{
				throw new ArgumentException("dest", "multidim");
			}
			if (cachedAll == null)
			{
				RefreshCachedAll();
			}
			try
			{
				cachedAll.CopyTo(dest, index);
			}
			catch (ArrayTypeMismatchException)
			{
				throw new InvalidCastException();
			}
		}

		private void RefreshCachedAll()
		{
			cachedAll = null;
			int count = Count;
			cachedAll = new string[count];
			for (int i = 0; i < count; i++)
			{
				cachedAll[i] = Get(i);
			}
		}

		public virtual string Get(int index)
		{
			ArrayList values = (ArrayList)BaseGet(index);
			return AsSingleString(values);
		}

		public virtual string Get(string name)
		{
			ArrayList values = (ArrayList)BaseGet(name);
			return AsSingleString(values);
		}

		private static string AsSingleString(ArrayList values)
		{
			if (values == null)
			{
				return null;
			}
			int count = values.Count;
			switch (count)
			{
			case 0:
				return null;
			case 1:
				return (string)values[0];
			case 2:
				return (string)values[0] + ',' + (string)values[1];
			default:
			{
				int num = count;
				for (int i = 0; i < count; i++)
				{
					num += ((string)values[i]).Length;
				}
				StringBuilder stringBuilder = new StringBuilder((string)values[0], num);
				for (int j = 1; j < count; j++)
				{
					stringBuilder.Append(',');
					stringBuilder.Append(values[j]);
				}
				return stringBuilder.ToString();
			}
			}
		}

		public virtual string GetKey(int index)
		{
			return BaseGetKey(index);
		}

		public virtual string[] GetValues(int index)
		{
			ArrayList values = (ArrayList)BaseGet(index);
			return AsStringArray(values);
		}

		public virtual string[] GetValues(string name)
		{
			ArrayList values = (ArrayList)BaseGet(name);
			return AsStringArray(values);
		}

		private static string[] AsStringArray(ArrayList values)
		{
			if (values == null)
			{
				return null;
			}
			int count = values.Count;
			if (count == 0)
			{
				return null;
			}
			string[] array = new string[count];
			values.CopyTo(array);
			return array;
		}

		public bool HasKeys()
		{
			return BaseHasKeys();
		}

		public virtual void Remove(string name)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			InvalidateCachedArrays();
			BaseRemove(name);
		}

		public virtual void Set(string name, string value)
		{
			if (base.IsReadOnly)
			{
				throw new NotSupportedException("Collection is read-only");
			}
			InvalidateCachedArrays();
			ArrayList arrayList = new ArrayList();
			if (value != null)
			{
				arrayList.Add(value);
				BaseSet(name, arrayList);
			}
			else
			{
				BaseSet(name, null);
			}
		}

		protected void InvalidateCachedArrays()
		{
			cachedAllKeys = null;
			cachedAll = null;
		}
	}
}
