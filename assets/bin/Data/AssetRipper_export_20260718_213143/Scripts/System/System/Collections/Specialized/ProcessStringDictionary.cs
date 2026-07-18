namespace System.Collections.Specialized
{
	internal class ProcessStringDictionary : StringDictionary, IEnumerable
	{
		private Hashtable table;

		public override int Count
		{
			get
			{
				return table.Count;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public override string this[string key]
		{
			get
			{
				return (string)table[key];
			}
			set
			{
				table[key] = value;
			}
		}

		public override ICollection Keys
		{
			get
			{
				return table.Keys;
			}
		}

		public override ICollection Values
		{
			get
			{
				return table.Values;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return table.SyncRoot;
			}
		}

		public ProcessStringDictionary()
		{
			IHashCodeProvider hcp = null;
			IComparer comparer = null;
			int platform = (int)Environment.OSVersion.Platform;
			if (platform != 4 && platform != 128)
			{
				hcp = CaseInsensitiveHashCodeProvider.DefaultInvariant;
				comparer = CaseInsensitiveComparer.DefaultInvariant;
			}
			table = new Hashtable(hcp, comparer);
		}

		public override void Add(string key, string value)
		{
			table.Add(key, value);
		}

		public override void Clear()
		{
			table.Clear();
		}

		public override bool ContainsKey(string key)
		{
			return table.ContainsKey(key);
		}

		public override bool ContainsValue(string value)
		{
			return table.ContainsValue(value);
		}

		public override void CopyTo(Array array, int index)
		{
			table.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return table.GetEnumerator();
		}

		public override void Remove(string key)
		{
			table.Remove(key);
		}
	}
}
