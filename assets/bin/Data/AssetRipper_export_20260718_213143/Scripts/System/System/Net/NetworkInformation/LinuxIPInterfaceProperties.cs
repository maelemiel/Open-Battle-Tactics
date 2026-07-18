using System.Collections.Generic;

namespace System.Net.NetworkInformation
{
	internal class LinuxIPInterfaceProperties : System.Net.NetworkInformation.UnixIPInterfaceProperties
	{
		public LinuxIPInterfaceProperties(System.Net.NetworkInformation.LinuxNetworkInterface iface, List<IPAddress> addresses)
			: base(iface, addresses)
		{
		}

		public override IPv4InterfaceProperties GetIPv4Properties()
		{
			if (ipv4iface_properties == null)
			{
				ipv4iface_properties = new System.Net.NetworkInformation.LinuxIPv4InterfaceProperties(iface as System.Net.NetworkInformation.LinuxNetworkInterface);
			}
			return ipv4iface_properties;
		}
	}
}
