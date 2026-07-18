using System.IO;

namespace System.Net.NetworkInformation
{
	internal sealed class LinuxIPv4InterfaceProperties : System.Net.NetworkInformation.UnixIPv4InterfaceProperties
	{
		public override bool IsForwardingEnabled
		{
			get
			{
				string path = "/proc/sys/net/ipv4/conf/" + iface.Name + "/forwarding";
				if (File.Exists(path))
				{
					string text = NetworkInterface.ReadLine(path);
					return text != "0";
				}
				return false;
			}
		}

		public override int Mtu
		{
			get
			{
				string path = (iface as System.Net.NetworkInformation.LinuxNetworkInterface).IfacePath + "mtu";
				int result = 0;
				if (File.Exists(path))
				{
					string s = NetworkInterface.ReadLine(path);
					try
					{
						result = int.Parse(s);
					}
					catch
					{
					}
				}
				return result;
			}
		}

		public LinuxIPv4InterfaceProperties(System.Net.NetworkInformation.LinuxNetworkInterface iface)
			: base(iface)
		{
		}
	}
}
