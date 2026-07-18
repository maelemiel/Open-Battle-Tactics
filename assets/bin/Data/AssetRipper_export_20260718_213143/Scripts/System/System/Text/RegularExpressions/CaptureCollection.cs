using System.Collections;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class CaptureCollection : ICollection, IEnumerable
	{
		private Capture[] list;

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

		public Capture this[int i]
		{
			get
			{
				if (i < 0 || i >= Count)
				{
					throw new ArgumentOutOfRangeException("Index is out of range");
				}
				return list[i];
			}
		}

		public object SyncRoot
		{
			get
			{
				return list;
			}
		}

		internal CaptureCollection(int n)
		{
			list = new Capture[n];
		}

		internal void SetValue(Capture cap, int i)
		{
			list[i] = cap;
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
