using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	internal class MacOsIPInterfaceProperties : System.Net.NetworkInformation.UnixIPInterfaceProperties
	{
		public MacOsIPInterfaceProperties(System.Net.NetworkInformation.MacOsNetworkInterface iface, List<IPAddress> addresses)
			: base(iface, addresses)
		{
		}

		public override IPv4InterfaceProperties GetIPv4Properties()
		{
			if (ipv4iface_properties == null)
			{
				ipv4iface_properties = new System.Net.NetworkInformation.MacOsIPv4InterfaceProperties(iface as System.Net.NetworkInformation.MacOsNetworkInterface);
			}
			return ipv4iface_properties;
		}
	}
}
