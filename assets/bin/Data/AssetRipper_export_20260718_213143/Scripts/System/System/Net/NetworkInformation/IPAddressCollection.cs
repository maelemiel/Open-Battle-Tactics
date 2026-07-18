using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class IPAddressCollection : IEnumerable, ICollection<IPAddress>, IEnumerable<IPAddress>
	{
		private IList<IPAddress> list = new List<IPAddress>();

		public virtual int Count
		{
			get
			{
				return list.Count;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return list.IsReadOnly;
			}
		}

		public virtual IPAddress this[int index]
		{
			get
			{
				return list[index];
			}
		}

		protected internal IPAddressCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		internal void SetReadOnly()
		{
			if (!IsReadOnly)
			{
				list = ((List<IPAddress>)list).AsReadOnly();
			}
		}

		public virtual void Add(IPAddress address)
		{
			if (IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			list.Add(address);
		}

		public virtual void Clear()
		{
			if (IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			list.Clear();
		}

		public virtual bool Contains(IPAddress address)
		{
			return list.Contains(address);
		}

		public virtual void CopyTo(IPAddress[] array, int offset)
		{
			list.CopyTo(array, offset);
		}

		public virtual IEnumerator<IPAddress> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public virtual bool Remove(IPAddress address)
		{
			if (IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			return list.Remove(address);
		}
	}
}
