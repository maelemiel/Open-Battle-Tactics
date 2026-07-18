using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal abstract class UnixNetworkInterface : NetworkInterface
	{
		protected IPv4InterfaceStatistics ipv4stats;

		protected IPInterfaceProperties ipproperties;

		private string name;

		private int index;

		protected List<IPAddress> addresses;

		private byte[] macAddress;

		private NetworkInterfaceType type;

		public override string Description
		{
			get
			{
				return name;
			}
		}

		public override string Id
		{
			get
			{
				return name;
			}
		}

		public override bool IsReceiveOnly
		{
			get
			{
				return false;
			}
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		public override NetworkInterfaceType NetworkInterfaceType
		{
			get
			{
				return type;
			}
		}

		[System.MonoTODO("Parse dmesg?")]
		public override long Speed
		{
			get
			{
				return 1000000L;
			}
		}

		internal UnixNetworkInterface(string name)
		{
			this.name = name;
			addresses = new List<IPAddress>();
		}

		[DllImport("libc")]
		private static extern int if_nametoindex(string ifname);

		public static int IfNameToIndex(string ifname)
		{
			return if_nametoindex(ifname);
		}

		internal void AddAddress(IPAddress address)
		{
			addresses.Add(address);
		}

		internal void SetLinkLayerInfo(int index, byte[] macAddress, NetworkInterfaceType type)
		{
			this.index = index;
			this.macAddress = macAddress;
			this.type = type;
		}

		public override PhysicalAddress GetPhysicalAddress()
		{
			if (macAddress != null)
			{
				return new PhysicalAddress(macAddress);
			}
			return PhysicalAddress.None;
		}

		public override bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
		{
			bool flag = networkInterfaceComponent == NetworkInterfaceComponent.IPv4;
			bool flag2 = !flag && networkInterfaceComponent == NetworkInterfaceComponent.IPv6;
			foreach (IPAddress address in addresses)
			{
				if (flag && address.AddressFamily == AddressFamily.InterNetwork)
				{
					return true;
				}
				if (flag2 && address.AddressFamily == AddressFamily.InterNetworkV6)
				{
					return true;
				}
			}
			return false;
		}
	}
}
