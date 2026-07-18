using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	internal class Grouping<K, T> : IEnumerable, IEnumerable<T>, IGrouping<K, T>
	{
		private K key;

		private IEnumerable<T> group;

		public K Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public Grouping(K key, IEnumerable<T> group)
		{
			this.group = group;
			this.key = key;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return group.GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return group.GetEnumerator();
		}
	}
}
