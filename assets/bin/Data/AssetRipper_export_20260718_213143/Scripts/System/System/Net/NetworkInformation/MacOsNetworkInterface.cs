using System.Collections.Generic;
using System.Net.NetworkInformation.MacOsStructs;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class MacOsNetworkInterface : System.Net.NetworkInformation.UnixNetworkInterface
	{
		private const int AF_INET = 2;

		private const int AF_INET6 = 30;

		private const int AF_LINK = 18;

		public override OperationalStatus OperationalStatus
		{
			get
			{
				return OperationalStatus.Unknown;
			}
		}

		public override bool SupportsMulticast
		{
			get
			{
				return false;
			}
		}

		private MacOsNetworkInterface(string name)
			: base(name)
		{
		}

		[DllImport("libc")]
		private static extern int getifaddrs(out IntPtr ifap);

		[DllImport("libc")]
		private static extern void freeifaddrs(IntPtr ifap);

		public static NetworkInterface[] ImplGetAllNetworkInterfaces()
		{
			Dictionary<string, System.Net.NetworkInformation.MacOsNetworkInterface> dictionary = new Dictionary<string, System.Net.NetworkInformation.MacOsNetworkInterface>();
			IntPtr ifap;
			if (getifaddrs(out ifap) != 0)
			{
				throw new SystemException("getifaddrs() failed");
			}
			try
			{
				IntPtr intPtr = ifap;
				while (intPtr != IntPtr.Zero)
				{
					System.Net.NetworkInformation.MacOsStructs.ifaddrs ifaddrs2 = (System.Net.NetworkInformation.MacOsStructs.ifaddrs)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.MacOsStructs.ifaddrs));
					IPAddress iPAddress = IPAddress.None;
					string ifa_name = ifaddrs2.ifa_name;
					int num = -1;
					byte[] array = null;
					NetworkInterfaceType networkInterfaceType = NetworkInterfaceType.Unknown;
					if (ifaddrs2.ifa_addr != IntPtr.Zero)
					{
						System.Net.NetworkInformation.MacOsStructs.sockaddr sockaddr2 = (System.Net.NetworkInformation.MacOsStructs.sockaddr)Marshal.PtrToStructure(ifaddrs2.ifa_addr, typeof(System.Net.NetworkInformation.MacOsStructs.sockaddr));
						if (sockaddr2.sa_family == 30)
						{
							System.Net.NetworkInformation.MacOsStructs.sockaddr_in6 sockaddr_in7 = (System.Net.NetworkInformation.MacOsStructs.sockaddr_in6)Marshal.PtrToStructure(ifaddrs2.ifa_addr, typeof(System.Net.NetworkInformation.MacOsStructs.sockaddr_in6));
							iPAddress = new IPAddress(sockaddr_in7.sin6_addr.u6_addr8, sockaddr_in7.sin6_scope_id);
						}
						else if (sockaddr2.sa_family == 2)
						{
							iPAddress = new IPAddress(((System.Net.NetworkInformation.MacOsStructs.sockaddr_in)Marshal.PtrToStructure(ifaddrs2.ifa_addr, typeof(System.Net.NetworkInformation.MacOsStructs.sockaddr_in))).sin_addr);
						}
						else if (sockaddr2.sa_family == 18)
						{
							System.Net.NetworkInformation.MacOsStructs.sockaddr_dl sockaddr_dl2 = (System.Net.NetworkInformation.MacOsStructs.sockaddr_dl)Marshal.PtrToStructure(ifaddrs2.ifa_addr, typeof(System.Net.NetworkInformation.MacOsStructs.sockaddr_dl));
							array = new byte[sockaddr_dl2.sdl_alen];
							Array.Copy(sockaddr_dl2.sdl_data, sockaddr_dl2.sdl_nlen, array, 0, Math.Min(array.Length, sockaddr_dl2.sdl_data.Length - sockaddr_dl2.sdl_nlen));
							num = sockaddr_dl2.sdl_index;
							int sdl_type = sockaddr_dl2.sdl_type;
							if (Enum.IsDefined(typeof(System.Net.NetworkInformation.MacOsArpHardware), sdl_type))
							{
								switch ((System.Net.NetworkInformation.MacOsArpHardware)sdl_type)
								{
								case System.Net.NetworkInformation.MacOsArpHardware.ETHER:
									networkInterfaceType = NetworkInterfaceType.Ethernet;
									break;
								case System.Net.NetworkInformation.MacOsArpHardware.ATM:
									networkInterfaceType = NetworkInterfaceType.Atm;
									break;
								case System.Net.NetworkInformation.MacOsArpHardware.SLIP:
									networkInterfaceType = NetworkInterfaceType.Slip;
									break;
								case System.Net.NetworkInformation.MacOsArpHardware.PPP:
									networkInterfaceType = NetworkInterfaceType.Ppp;
									break;
								case System.Net.NetworkInformation.MacOsArpHardware.LOOPBACK:
									networkInterfaceType = NetworkInterfaceType.Loopback;
									array = null;
									break;
								case System.Net.NetworkInformation.MacOsArpHardware.FDDI:
									networkInterfaceType = NetworkInterfaceType.Fddi;
									break;
								}
							}
						}
					}
					System.Net.NetworkInformation.MacOsNetworkInterface value = null;
					if (!dictionary.TryGetValue(ifa_name, out value))
					{
						value = new System.Net.NetworkInformation.MacOsNetworkInterface(ifa_name);
						dictionary.Add(ifa_name, value);
					}
					if (!iPAddress.Equals(IPAddress.None))
					{
						value.AddAddress(iPAddress);
					}
					if (array != null || networkInterfaceType == NetworkInterfaceType.Loopback)
					{
						value.SetLinkLayerInfo(num, array, networkInterfaceType);
					}
					intPtr = ifaddrs2.ifa_next;
				}
			}
			finally
			{
				freeifaddrs(ifap);
			}
			NetworkInterface[] array2 = new NetworkInterface[dictionary.Count];
			int num2 = 0;
			foreach (System.Net.NetworkInformation.MacOsNetworkInterface value2 in dictionary.Values)
			{
				array2[num2] = value2;
				num2++;
			}
			return array2;
		}

		public override IPInterfaceProperties GetIPProperties()
		{
			if (ipproperties == null)
			{
				ipproperties = new System.Net.NetworkInformation.MacOsIPInterfaceProperties(this, addresses);
			}
			return ipproperties;
		}

		public override IPv4InterfaceStatistics GetIPv4Statistics()
		{
			if (ipv4stats == null)
			{
				ipv4stats = new System.Net.NetworkInformation.MacOsIPv4InterfaceStatistics(this);
			}
			return ipv4stats;
		}
	}
}
