using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(false)]
	public class IdentityReferenceCollection : IEnumerable, ICollection<IdentityReference>, IEnumerable<IdentityReference>
	{
		private ArrayList _list;

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IdentityReference this[int index]
		{
			get
			{
				if (index >= _list.Count)
				{
					return null;
				}
				return (IdentityReference)_list[index];
			}
			set
			{
				_list[index] = value;
			}
		}

		public IdentityReferenceCollection()
		{
			_list = new ArrayList();
		}

		public IdentityReferenceCollection(int capacity)
		{
			_list = new ArrayList(capacity);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void Add(IdentityReference identity)
		{
			_list.Add(identity);
		}

		public void Clear()
		{
			_list.Clear();
		}

		public bool Contains(IdentityReference identity)
		{
			foreach (IdentityReference item in _list)
			{
				if (item.Equals(identity))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(IdentityReference[] array, int offset)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<IdentityReference> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool Remove(IdentityReference identity)
		{
			foreach (IdentityReference item in _list)
			{
				if (item.Equals(identity))
				{
					_list.Remove(item);
					return true;
				}
			}
			return false;
		}

		public IdentityReferenceCollection Translate(Type targetType)
		{
			throw new NotImplementedException();
		}

		public IdentityReferenceCollection Translate(Type targetType, bool forceSuccess)
		{
			throw new NotImplementedException();
		}
	}
}
