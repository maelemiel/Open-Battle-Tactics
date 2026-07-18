using System.Collections;

namespace System.Text.RegularExpressions
{
	internal class FactoryCache
	{
		private class Key
		{
			public string pattern;

			public RegexOptions options;

			public Key(string pattern, RegexOptions options)
			{
				this.pattern = pattern;
				this.options = options;
			}

			public override int GetHashCode()
			{
				return pattern.GetHashCode() ^ (int)options;
			}

			public override bool Equals(object o)
			{
				if (o == null || !(o is Key))
				{
					return false;
				}
				Key key = (Key)o;
				return options == key.options && pattern.Equals(key.pattern);
			}

			public override string ToString()
			{
				return string.Concat("('", pattern, "', [", options, "])");
			}
		}

		private int capacity;

		private Hashtable factories;

		private System.Text.RegularExpressions.MRUList mru_list;

		public int Capacity
		{
			get
			{
				return capacity;
			}
			set
			{
				lock (this)
				{
					capacity = value;
					Cleanup();
				}
			}
		}

		public FactoryCache(int capacity)
		{
			this.capacity = capacity;
			factories = new Hashtable(capacity);
			mru_list = new System.Text.RegularExpressions.MRUList();
		}

		public void Add(string pattern, RegexOptions options, System.Text.RegularExpressions.IMachineFactory factory)
		{
			lock (this)
			{
				Key key = new Key(pattern, options);
				Cleanup();
				factories[key] = factory;
				mru_list.Use(key);
			}
		}

		private void Cleanup()
		{
			while (factories.Count >= capacity && capacity > 0)
			{
				object obj = mru_list.Evict();
				if (obj != null)
				{
					factories.Remove((Key)obj);
				}
			}
		}

		public System.Text.RegularExpressions.IMachineFactory Lookup(string pattern, RegexOptions options)
		{
			lock (this)
			{
				Key key = new Key(pattern, options);
				if (factories.Contains(key))
				{
					mru_list.Use(key);
					return (System.Text.RegularExpressions.IMachineFactory)factories[key];
				}
			}
			return null;
		}
	}
}
