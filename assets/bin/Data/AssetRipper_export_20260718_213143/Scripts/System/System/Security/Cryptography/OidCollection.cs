using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class OidCollection : ICollection, IEnumerable
	{
		private ArrayList _list;

		private bool _readOnly;

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
				return _list.IsSynchronized;
			}
		}

		public Oid this[int index]
		{
			get
			{
				return (Oid)_list[index];
			}
		}

		public Oid this[string oid]
		{
			get
			{
				foreach (Oid item in _list)
				{
					if (item.Value == oid)
					{
						return item;
					}
				}
				return null;
			}
		}

		public object SyncRoot
		{
			get
			{
				return _list.SyncRoot;
			}
		}

		internal bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
			}
		}

		public OidCollection()
		{
			_list = new ArrayList();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			_list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new OidEnumerator(this);
		}

		public int Add(Oid oid)
		{
			return (!_readOnly) ? _list.Add(oid) : 0;
		}

		public void CopyTo(Oid[] array, int index)
		{
			_list.CopyTo(array, index);
		}

		public OidEnumerator GetEnumerator()
		{
			return new OidEnumerator(this);
		}

		internal OidCollection ReadOnlyCopy()
		{
			OidCollection oidCollection = new OidCollection();
			foreach (Oid item in _list)
			{
				oidCollection.Add(item);
			}
			oidCollection._readOnly = true;
			return oidCollection;
		}
	}
}
