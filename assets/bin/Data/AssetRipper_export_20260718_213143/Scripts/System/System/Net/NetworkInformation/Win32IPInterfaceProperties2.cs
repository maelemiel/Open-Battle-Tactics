namespace System.Net.NetworkInformation
{
	internal class Win32IPInterfaceProperties2 : IPInterfaceProperties
	{
		private readonly System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES addr;

		private readonly System.Net.NetworkInformation.Win32_MIB_IFROW mib4;

		private readonly System.Net.NetworkInformation.Win32_MIB_IFROW mib6;

		public override IPAddressInformationCollection AnycastAddresses
		{
			get
			{
				return System.Net.NetworkInformation.IPAddressInformationImplCollection.Win32FromAnycast(addr.FirstAnycastAddress);
			}
		}

		public override IPAddressCollection DhcpServerAddresses
		{
			get
			{
				System.Net.NetworkInformation.Win32_MIB_IFROW win32_MIB_IFROW = mib4;
				System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO adapterInfoByIndex = System.Net.NetworkInformation.Win32NetworkInterface2.GetAdapterInfoByIndex(win32_MIB_IFROW.Index);
				return (adapterInfoByIndex == null) ? System.Net.NetworkInformation.Win32IPAddressCollection.Empty : new System.Net.NetworkInformation.Win32IPAddressCollection(adapterInfoByIndex.DhcpServer);
			}
		}

		public override IPAddressCollection DnsAddresses
		{
			get
			{
				return System.Net.NetworkInformation.Win32IPAddressCollection.FromDnsServer(addr.FirstDnsServerAddress);
			}
		}

		public override string DnsSuffix
		{
			get
			{
				return addr.DnsSuffix;
			}
		}

		public override GatewayIPAddressInformationCollection GatewayAddresses
		{
			get
			{
				System.Net.NetworkInformation.Win32_MIB_IFROW win32_MIB_IFROW = mib4;
				System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO adapterInfoByIndex = System.Net.NetworkInformation.Win32NetworkInterface2.GetAdapterInfoByIndex(win32_MIB_IFROW.Index);
				return (adapterInfoByIndex == null) ? System.Net.NetworkInformation.Win32GatewayIPAddressInformationCollection.Empty : new System.Net.NetworkInformation.Win32GatewayIPAddressInformationCollection(adapterInfoByIndex.GatewayList);
			}
		}

		public override bool IsDnsEnabled
		{
			get
			{
				return System.Net.NetworkInformation.Win32_FIXED_INFO.Instance.EnableDns != 0;
			}
		}

		public override bool IsDynamicDnsEnabled
		{
			get
			{
				return addr.DdnsEnabled;
			}
		}

		public override MulticastIPAddressInformationCollection MulticastAddresses
		{
			get
			{
				return System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection.Win32FromMulticast(addr.FirstMulticastAddress);
			}
		}

		public override UnicastIPAddressInformationCollection UnicastAddresses
		{
			get
			{
				System.Net.NetworkInformation.Win32_MIB_IFROW win32_MIB_IFROW = mib4;
				System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO adapterInfoByIndex = System.Net.NetworkInformation.Win32NetworkInterface2.GetAdapterInfoByIndex(win32_MIB_IFROW.Index);
				return (adapterInfoByIndex == null) ? System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection.Empty : System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection.Win32FromUnicast((int)adapterInfoByIndex.Index, addr.FirstUnicastAddress);
			}
		}

		public override IPAddressCollection WinsServersAddresses
		{
			get
			{
				System.Net.NetworkInformation.Win32_MIB_IFROW win32_MIB_IFROW = mib4;
				System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO adapterInfoByIndex = System.Net.NetworkInformation.Win32NetworkInterface2.GetAdapterInfoByIndex(win32_MIB_IFROW.Index);
				return (adapterInfoByIndex == null) ? System.Net.NetworkInformation.Win32IPAddressCollection.Empty : new System.Net.NetworkInformation.Win32IPAddressCollection(adapterInfoByIndex.PrimaryWinsServer, adapterInfoByIndex.SecondaryWinsServer);
			}
		}

		public Win32IPInterfaceProperties2(System.Net.NetworkInformation.Win32_IP_ADAPTER_ADDRESSES addr, System.Net.NetworkInformation.Win32_MIB_IFROW mib4, System.Net.NetworkInformation.Win32_MIB_IFROW mib6)
		{
			this.addr = addr;
			this.mib4 = mib4;
			this.mib6 = mib6;
		}

		public override IPv4InterfaceProperties GetIPv4Properties()
		{
			System.Net.NetworkInformation.Win32_MIB_IFROW win32_MIB_IFROW = mib4;
			System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO adapterInfoByIndex = System.Net.NetworkInformation.Win32NetworkInterface2.GetAdapterInfoByIndex(win32_MIB_IFROW.Index);
			return (adapterInfoByIndex == null) ? null : new System.Net.NetworkInformation.Win32IPv4InterfaceProperties(adapterInfoByIndex, mib4);
		}

		public override IPv6InterfaceProperties GetIPv6Properties()
		{
			System.Net.NetworkInformation.Win32_MIB_IFROW win32_MIB_IFROW = mib6;
			System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO adapterInfoByIndex = System.Net.NetworkInformation.Win32NetworkInterface2.GetAdapterInfoByIndex(win32_MIB_IFROW.Index);
			return (adapterInfoByIndex == null) ? null : new System.Net.NetworkInformation.Win32IPv6InterfaceProperties(mib6);
		}
	}
}
