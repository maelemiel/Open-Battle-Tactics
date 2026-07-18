using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class MulticastIPAddressInformationCollection : IEnumerable, IEnumerable<MulticastIPAddressInformation>, ICollection<MulticastIPAddressInformation>
	{
		private List<MulticastIPAddressInformation> list = new List<MulticastIPAddressInformation>();

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
				return true;
			}
		}

		public virtual MulticastIPAddressInformation this[int index]
		{
			get
			{
				return list[index];
			}
		}

		protected internal MulticastIPAddressInformationCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public virtual void Add(MulticastIPAddressInformation address)
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

		public virtual bool Contains(MulticastIPAddressInformation address)
		{
			return list.Contains(address);
		}

		public virtual void CopyTo(MulticastIPAddressInformation[] array, int offset)
		{
			list.CopyTo(array, offset);
		}

		public virtual IEnumerator<MulticastIPAddressInformation> GetEnumerator()
		{
			return ((IEnumerable<MulticastIPAddressInformation>)list).GetEnumerator();
		}

		public virtual bool Remove(MulticastIPAddressInformation address)
		{
			if (IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			return list.Remove(address);
		}
	}
}
