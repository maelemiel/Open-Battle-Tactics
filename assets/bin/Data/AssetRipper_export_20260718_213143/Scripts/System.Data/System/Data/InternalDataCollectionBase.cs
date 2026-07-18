using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	public class InternalDataCollectionBase : ICollection, IEnumerable
	{
		private ArrayList list;

		private bool readOnly;

		private bool synchronized;

		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return list.Count;
			}
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return readOnly;
			}
		}

		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return synchronized;
			}
		}

		protected virtual ArrayList List
		{
			get
			{
				return list;
			}
		}

		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public InternalDataCollectionBase()
		{
			list = new ArrayList();
		}

		public virtual void CopyTo(Array ar, int index)
		{
			list.CopyTo(ar, index);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		internal Array ToArray(Type type)
		{
			return list.ToArray(type);
		}
	}
}
