using System.Runtime.InteropServices;

namespace System.Collections
{
	[Serializable]
	[ComVisible(true)]
	public abstract class CollectionBase : IEnumerable, ICollection, IList
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

		bool IList.IsFixedSize
		{
			get
			{
				return InnerList.IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return InnerList.IsReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return InnerList[index];
			}
			set
			{
				if (index < 0 || index >= InnerList.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				OnValidate(value);
				object obj = InnerList[index];
				OnSet(index, obj, value);
				InnerList[index] = value;
				try
				{
					OnSetComplete(index, obj, value);
				}
				catch
				{
					InnerList[index] = obj;
					throw;
				}
			}
		}

		public int Count
		{
			get
			{
				return InnerList.Count;
			}
		}

		[ComVisible(false)]
		public int Capacity
		{
			get
			{
				if (list == null)
				{
					list = new ArrayList();
				}
				return list.Capacity;
			}
			set
			{
				if (list == null)
				{
					list = new ArrayList();
				}
				list.Capacity = value;
			}
		}

		protected ArrayList InnerList
		{
			get
			{
				if (list == null)
				{
					list = new ArrayList();
				}
				return list;
			}
		}

		protected IList List
		{
			get
			{
				return this;
			}
		}

		protected CollectionBase()
		{
		}

		protected CollectionBase(int capacity)
		{
			list = new ArrayList(capacity);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			InnerList.CopyTo(array, index);
		}

		int IList.Add(object value)
		{
			OnValidate(value);
			int count = InnerList.Count;
			OnInsert(count, value);
			InnerList.Add(value);
			try
			{
				OnInsertComplete(count, value);
				return count;
			}
			catch
			{
				InnerList.RemoveAt(count);
				throw;
			}
		}

		bool IList.Contains(object value)
		{
			return InnerList.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return InnerList.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			OnValidate(value);
			OnInsert(index, value);
			InnerList.Insert(index, value);
			try
			{
				OnInsertComplete(index, value);
			}
			catch
			{
				InnerList.RemoveAt(index);
				throw;
			}
		}

		void IList.Remove(object value)
		{
			OnValidate(value);
			int num = InnerList.IndexOf(value);
			if (num == -1)
			{
				throw new ArgumentException("The element cannot be found.", "value");
			}
			OnRemove(num, value);
			InnerList.Remove(value);
			OnRemoveComplete(num, value);
		}

		public IEnumerator GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		public void Clear()
		{
			OnClear();
			InnerList.Clear();
			OnClearComplete();
		}

		public void RemoveAt(int index)
		{
			object value = InnerList[index];
			OnValidate(value);
			OnRemove(index, value);
			InnerList.RemoveAt(index);
			OnRemoveComplete(index, value);
		}

		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual void OnInsert(int index, object value)
		{
		}

		protected virtual void OnInsertComplete(int index, object value)
		{
		}

		protected virtual void OnRemove(int index, object value)
		{
		}

		protected virtual void OnRemoveComplete(int index, object value)
		{
		}

		protected virtual void OnSet(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnSetComplete(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnValidate(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("CollectionBase.OnValidate: Invalid parameter value passed to method: null");
			}
		}
	}
}
