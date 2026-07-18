namespace System.Net.Sockets
{
	public class IPv6MulticastOption
	{
		private IPAddress group;

		private long ifIndex;

		public IPAddress Group
		{
			get
			{
				return group;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				group = value;
			}
		}

		public long InterfaceIndex
		{
			get
			{
				return ifIndex;
			}
			set
			{
				if (value < 0 || value > uint.MaxValue)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				ifIndex = value;
			}
		}

		public IPv6MulticastOption(IPAddress group)
			: this(group, 0L)
		{
		}

		public IPv6MulticastOption(IPAddress group, long ifindex)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (ifindex < 0 || ifindex > uint.MaxValue)
			{
				throw new ArgumentOutOfRangeException("ifindex");
			}
			this.group = group;
			ifIndex = ifindex;
		}
	}
}
