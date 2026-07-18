using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class KeyContainerPermissionAccessEntryCollection : IEnumerable, ICollection
	{
		private ArrayList _list;

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public KeyContainerPermissionAccessEntry this[int index]
		{
			get
			{
				return (KeyContainerPermissionAccessEntry)_list[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal KeyContainerPermissionAccessEntryCollection()
		{
			_list = new ArrayList();
		}

		internal KeyContainerPermissionAccessEntryCollection(KeyContainerPermissionAccessEntry[] entries)
		{
			if (entries != null)
			{
				foreach (KeyContainerPermissionAccessEntry accessEntry in entries)
				{
					Add(accessEntry);
				}
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			_list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new KeyContainerPermissionAccessEntryEnumerator(_list);
		}

		public int Add(KeyContainerPermissionAccessEntry accessEntry)
		{
			return _list.Add(accessEntry);
		}

		public void Clear()
		{
			_list.Clear();
		}

		public void CopyTo(KeyContainerPermissionAccessEntry[] array, int index)
		{
			_list.CopyTo(array, index);
		}

		public KeyContainerPermissionAccessEntryEnumerator GetEnumerator()
		{
			return new KeyContainerPermissionAccessEntryEnumerator(_list);
		}

		public int IndexOf(KeyContainerPermissionAccessEntry accessEntry)
		{
			if (accessEntry == null)
			{
				throw new ArgumentNullException("accessEntry");
			}
			for (int i = 0; i < _list.Count; i++)
			{
				if (accessEntry.Equals(_list[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public void Remove(KeyContainerPermissionAccessEntry accessEntry)
		{
			if (accessEntry == null)
			{
				throw new ArgumentNullException("accessEntry");
			}
			for (int i = 0; i < _list.Count; i++)
			{
				if (accessEntry.Equals(_list[i]))
				{
					_list.RemoveAt(i);
				}
			}
		}
	}
}
