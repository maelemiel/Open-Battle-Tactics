using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class UnicastIPAddressInformationCollection : IEnumerable, IEnumerable<UnicastIPAddressInformation>, ICollection<UnicastIPAddressInformation>
	{
		private List<UnicastIPAddressInformation> list = new List<UnicastIPAddressInformation>();

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

		public virtual UnicastIPAddressInformation this[int index]
		{
			get
			{
				return list[index];
			}
		}

		protected internal UnicastIPAddressInformationCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public virtual void Add(UnicastIPAddressInformation address)
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

		public virtual bool Contains(UnicastIPAddressInformation address)
		{
			return list.Contains(address);
		}

		public virtual void CopyTo(UnicastIPAddressInformation[] array, int offset)
		{
			list.CopyTo(array, offset);
		}

		public virtual IEnumerator<UnicastIPAddressInformation> GetEnumerator()
		{
			return ((IEnumerable<UnicastIPAddressInformation>)list).GetEnumerator();
		}

		public virtual bool Remove(UnicastIPAddressInformation address)
		{
			if (IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			return list.Remove(address);
		}
	}
}
