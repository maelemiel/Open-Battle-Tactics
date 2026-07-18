using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class IPAddressInformationCollection : IEnumerable, IEnumerable<IPAddressInformation>, ICollection<IPAddressInformation>
	{
		private List<IPAddressInformation> list = new List<IPAddressInformation>();

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

		public virtual IPAddressInformation this[int index]
		{
			get
			{
				return list[index];
			}
		}

		internal IPAddressInformationCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public virtual void Add(IPAddressInformation address)
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

		public virtual bool Contains(IPAddressInformation address)
		{
			return list.Contains(address);
		}

		public virtual void CopyTo(IPAddressInformation[] array, int offset)
		{
			list.CopyTo(array, offset);
		}

		public virtual IEnumerator<IPAddressInformation> GetEnumerator()
		{
			return ((IEnumerable<IPAddressInformation>)list).GetEnumerator();
		}

		public virtual bool Remove(IPAddressInformation address)
		{
			if (IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			return list.Remove(address);
		}
	}
}
