using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class GroupCollection : ICollection, IEnumerable
	{
		private Group[] list;

		private int gap;

		public int Count
		{
			get
			{
				return list.Length;
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

		public Group this[int i]
		{
			get
			{
				if (i >= gap)
				{
					Match match = (Match)list[0];
					i = ((match != Match.Empty) ? match.Regex.GetGroupIndex(i) : (-1));
				}
				return (i >= 0) ? list[i] : Group.Fail;
			}
		}

		public Group this[string groupName]
		{
			get
			{
				Match match = (Match)list[0];
				if (match != Match.Empty)
				{
					int num = match.Regex.GroupNumberFromName(groupName);
					if (num != -1)
					{
						return this[num];
					}
				}
				return Group.Fail;
			}
		}

		public object SyncRoot
		{
			get
			{
				return list;
			}
		}

		internal GroupCollection(int n, int gap)
		{
			list = new Group[n];
			this.gap = gap;
		}

		internal void SetValue(Group g, int i)
		{
			list[i] = g;
		}

		public void CopyTo(Array array, int index)
		{
			list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
