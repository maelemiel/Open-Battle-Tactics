using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class Win32NetworkInterface2 : NetworkInterface
	{
		private System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES addr;

		private System.Net.NetworkInformation.Win32_MIB_IFROW mib4;

		private System.Net.NetworkInformation.Win32_MIB_IFROW mib6;

		private System.Net.NetworkInformation.Win32IPv4InterfaceStatistics ip4stats;

		private IPInterfaceProperties ip_if_props;

		public override string Description
		{
			get
			{
				return addr.Description;
			}
		}

		public override string Id
		{
			get
			{
				return addr.AdapterName;
			}
		}

		public override bool IsReceiveOnly
		{
			get
			{
				return addr.IsReceiveOnly;
			}
		}

		public override string Name
		{
			get
			{
				return addr.FriendlyName;
			}
		}

		public override NetworkInterfaceType NetworkInterfaceType
		{
			get
			{
				return addr.IfType;
			}
		}

		public override OperationalStatus OperationalStatus
		{
			get
			{
				return addr.OperStatus;
			}
		}

		public override long Speed
		{
			get
			{
				return (mib6.Index < 0) ? mib4.Speed : mib6.Speed;
			}
		}

		public override bool SupportsMulticast
		{
			get
			{
				return !addr.NoMulticast;
			}
		}

		private Win32NetworkInterface2(System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES addr)
		{
			this.addr = addr;
			mib4 = default(System.Net.NetworkInformation.Win32_MIB_IFROW);
			mib4.Index = addr.Alignment.IfIndex;
			if (GetIfEntry(ref mib4) != 0)
			{
				mib4.Index = -1;
			}
			mib6 = default(System.Net.NetworkInformation.Win32_MIB_IFROW);
			mib6.Index = addr.Ipv6IfIndex;
			if (GetIfEntry(ref mib6) != 0)
			{
				mib6.Index = -1;
			}
			ip4stats = new System.Net.NetworkInformation.Win32IPv4InterfaceStatistics(mib4);
			ip_if_props = new System.Net.NetworkInformation.Win32IPInterfaceProperties2(addr, mib4, mib6);
		}

		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetAdaptersInfo(byte[] info, ref int size);

		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetAdaptersAddresses(uint family, uint flags, IntPtr reserved, byte[] info, ref int size);

		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetIfEntry(ref System.Net.NetworkInformation.Win32_MIB_IFROW row);

		public static NetworkInterface[] ImplGetAllNetworkInterfaces()
		{
			System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES[] adaptersAddresses = GetAdaptersAddresses();
			NetworkInterface[] array = new NetworkInterface[adaptersAddresses.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new System.Net.NetworkInformation.Win32NetworkInterface2(adaptersAddresses[i]);
			}
			return array;
		}

		public static System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO GetAdapterInfoByIndex(int index)
		{
			System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO[] adaptersInfo = GetAdaptersInfo();
			foreach (System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO win32_IP_ADAPTER_INFO in adaptersInfo)
			{
				if (win32_IP_ADAPTER_INFO.Index == index)
				{
					return win32_IP_ADAPTER_INFO;
				}
			}
			return null;
		}

		private unsafe static System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO[] GetAdaptersInfo()
		{
			//IL_0045->IL004c: Incompatible stack types: I vs Ref
			byte[] info = null;
			int size = 0;
			GetAdaptersInfo(info, ref size);
			info = new byte[size];
			int adaptersInfo = GetAdaptersInfo(info, ref size);
			if (adaptersInfo != 0)
			{
				throw new NetworkInformationException(adaptersInfo);
			}
			List<System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO> list = new List<System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO>();
			fixed (byte* ptr = &(info != null && info.Length != 0 ? ref info[0] : ref *(byte*)null))
			{
				IntPtr intPtr = (IntPtr)ptr;
				while (intPtr != IntPtr.Zero)
				{
					System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO win32_IP_ADAPTER_INFO = new System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO();
					Marshal.PtrToStructure(intPtr, win32_IP_ADAPTER_INFO);
					list.Add(win32_IP_ADAPTER_INFO);
					intPtr = win32_IP_ADAPTER_INFO.Next;
				}
			}
			return list.ToArray();
		}

		private unsafe static System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES[] GetAdaptersAddresses()
		{
			//IL_0053->IL005a: Incompatible stack types: I vs Ref
			byte[] info = null;
			int size = 0;
			GetAdaptersAddresses(0u, 0u, IntPtr.Zero, info, ref size);
			info = new byte[size];
			int adaptersAddresses = GetAdaptersAddresses(0u, 0u, IntPtr.Zero, info, ref size);
			if (adaptersAddresses != 0)
			{
				throw new NetworkInformationException(adaptersAddresses);
			}
			List<System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES> list = new List<System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES>();
			fixed (byte* ptr = &(info != null && info.Length != 0 ? ref info[0] : ref *(byte*)null))
			{
				IntPtr intPtr = (IntPtr)ptr;
				while (intPtr != IntPtr.Zero)
				{
					System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES win32_IP_ADAPTER_ADDRESSES = new System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES();
					Marshal.PtrToStructure(intPtr, win32_IP_ADAPTER_ADDRESSES);
					list.Add(win32_IP_ADAPTER_ADDRESSES);
					intPtr = win32_IP_ADAPTER_ADDRESSES.Next;
				}
			}
			return list.ToArray();
		}

		public override IPInterfaceProperties GetIPProperties()
		{
			return ip_if_props;
		}

		public override IPv4InterfaceStatistics GetIPv4Statistics()
		{
			return ip4stats;
		}

		public override PhysicalAddress GetPhysicalAddress()
		{
			byte[] array = new byte[addr.PhysicalAddressLength];
			Array.Copy(addr.PhysicalAddress, 0, array, 0, array.Length);
			return new PhysicalAddress(array);
		}

		public override bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
		{
			switch (networkInterfaceComponent)
			{
			case NetworkInterfaceComponent.IPv4:
				return mib4.Index >= 0;
			case NetworkInterfaceComponent.IPv6:
				return mib6.Index >= 0;
			default:
				return false;
			}
		}
	}
}
