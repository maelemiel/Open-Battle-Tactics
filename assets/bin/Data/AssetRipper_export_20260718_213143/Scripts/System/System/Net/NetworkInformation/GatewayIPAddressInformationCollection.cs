using System.Collections;
using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	public class GatewayIPAddressInformationCollection : IEnumerable, IEnumerable<GatewayIPAddressInformation>, ICollection<GatewayIPAddressInformation>
	{
		private List<GatewayIPAddressInformation> list = new List<GatewayIPAddressInformation>();

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

		public virtual GatewayIPAddressInformation this[int index]
		{
			get
			{
				return list[index];
			}
		}

		protected GatewayIPAddressInformationCollection()
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public virtual void Add(GatewayIPAddressInformation address)
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

		public virtual bool Contains(GatewayIPAddressInformation address)
		{
			return list.Contains(address);
		}

		public virtual void CopyTo(GatewayIPAddressInformation[] array, int offset)
		{
			list.CopyTo(array, offset);
		}

		public virtual IEnumerator<GatewayIPAddressInformation> GetEnumerator()
		{
			return ((IEnumerable<GatewayIPAddressInformation>)list).GetEnumerator();
		}

		public virtual bool Remove(GatewayIPAddressInformation address)
		{
			if (IsReadOnly)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			return list.Remove(address);
		}
	}
}
