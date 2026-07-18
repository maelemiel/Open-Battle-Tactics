using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class MatchCollection : ICollection, IEnumerable
	{
		private class Enumerator : IEnumerator
		{
			private int index;

			private MatchCollection coll;

			object IEnumerator.Current
			{
				get
				{
					if (index < 0)
					{
						throw new InvalidOperationException("'Current' called before 'MoveNext()'");
					}
					if (index > coll.list.Count)
					{
						throw new SystemException("MatchCollection in invalid state");
					}
					if (index == coll.list.Count && !coll.current.Success)
					{
						throw new InvalidOperationException("'Current' called after 'MoveNext()' returned false");
					}
					return (index >= coll.list.Count) ? coll.current : coll.list[index];
				}
			}

			internal Enumerator(MatchCollection coll)
			{
				this.coll = coll;
				index = -1;
			}

			void IEnumerator.Reset()
			{
				index = -1;
			}

			bool IEnumerator.MoveNext()
			{
				if (index > coll.list.Count)
				{
					throw new SystemException("MatchCollection in invalid state");
				}
				if (index == coll.list.Count && !coll.current.Success)
				{
					return false;
				}
				return coll.TryToGet(++index);
			}
		}

		private Match current;

		private ArrayList list;

		public int Count
		{
			get
			{
				return FullList.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual Match this[int i]
		{
			get
			{
				if (i < 0 || !TryToGet(i))
				{
					throw new ArgumentOutOfRangeException("i");
				}
				return (i >= list.Count) ? current : ((Match)list[i]);
			}
		}

		public object SyncRoot
		{
			get
			{
				return list;
			}
		}

		private ICollection FullList
		{
			get
			{
				if (TryToGet(int.MaxValue))
				{
					throw new SystemException("too many matches");
				}
				return list;
			}
		}

		internal MatchCollection(Match start)
		{
			current = start;
			list = new ArrayList();
		}

		public void CopyTo(Array array, int index)
		{
			FullList.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			IEnumerator result;
			if (current.Success)
			{
				IEnumerator enumerator = new Enumerator(this);
				result = enumerator;
			}
			else
			{
				result = list.GetEnumerator();
			}
			return result;
		}

		private bool TryToGet(int i)
		{
			while (i > list.Count && current.Success)
			{
				list.Add(current);
				current = current.NextMatch();
			}
			return i < list.Count || current.Success;
		}
	}
}
