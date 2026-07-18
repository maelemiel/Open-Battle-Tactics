using System.Runtime.InteropServices;

namespace System.Collections
{
	[Serializable]
	[ComVisible(true)]
	public abstract class ReadOnlyCollectionBase : IEnumerable, ICollection
	{
		private ArrayList list;

		object ICollection.SyncRoot
		{
			get
			{
				return InnerList.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return InnerList.IsSynchronized;
			}
		}

		public virtual int Count
		{
			get
			{
				return InnerList.Count;
			}
		}

		protected ArrayList InnerList
		{
			get
			{
				return list;
			}
		}

		protected ReadOnlyCollectionBase()
		{
			list = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			lock (InnerList)
			{
				InnerList.CopyTo(array, index);
			}
		}

		public virtual IEnumerator GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}
	}
}
