namespace System.Net.Sockets
{
	public class MulticastOption
	{
		private IPAddress group;

		private IPAddress local;

		private int iface_index;

		public IPAddress Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public IPAddress LocalAddress
		{
			get
			{
				return local;
			}
			set
			{
				local = value;
				iface_index = 0;
			}
		}

		public int InterfaceIndex
		{
			get
			{
				return iface_index;
			}
			set
			{
				if (value < 0 || value > 16777215)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				iface_index = value;
				local = null;
			}
		}

		public MulticastOption(IPAddress group)
			: this(group, IPAddress.Any)
		{
		}

		public MulticastOption(IPAddress group, int interfaceIndex)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (interfaceIndex < 0 || interfaceIndex > 16777215)
			{
				throw new ArgumentOutOfRangeException("interfaceIndex");
			}
			this.group = group;
			iface_index = interfaceIndex;
		}

		public MulticastOption(IPAddress group, IPAddress mcint)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (mcint == null)
			{
				throw new ArgumentNullException("mcint");
			}
			this.group = group;
			local = mcint;
		}
	}
}
