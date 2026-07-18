using System.IO;

namespace System.Net.NetworkInformation
{
	public abstract class IPGlobalProperties
	{
		public abstract string DhcpScopeName { get; }

		public abstract string DomainName { get; }

		public abstract string HostName { get; }

		public abstract bool IsWinsProxy { get; }

		public abstract NetBiosNodeType NodeType { get; }

		public static IPGlobalProperties GetIPGlobalProperties()
		{
			PlatformID platform = Environment.OSVersion.Platform;
			if (platform == PlatformID.Unix)
			{
				System.Net.NetworkInformation.MibIPGlobalProperties mibIPGlobalProperties = null;
				if (Directory.Exists("/proc"))
				{
					mibIPGlobalProperties = new System.Net.NetworkInformation.MibIPGlobalProperties("/proc");
					if (File.Exists(mibIPGlobalProperties.StatisticsFile))
					{
						return mibIPGlobalProperties;
					}
				}
				if (Directory.Exists("/usr/compat/linux/proc"))
				{
					mibIPGlobalProperties = new System.Net.NetworkInformation.MibIPGlobalProperties("/usr/compat/linux/proc");
					if (File.Exists(mibIPGlobalProperties.StatisticsFile))
					{
						return mibIPGlobalProperties;
					}
				}
				throw new NotSupportedException("This platform is not supported");
			}
			return new System.Net.NetworkInformation.Win32IPGlobalProperties();
		}

		public abstract TcpConnectionInformation[] GetActiveTcpConnections();

		public abstract IPEndPoint[] GetActiveTcpListeners();

		public abstract IPEndPoint[] GetActiveUdpListeners();

		public abstract IcmpV4Statistics GetIcmpV4Statistics();

		public abstract IcmpV6Statistics GetIcmpV6Statistics();

		public abstract IPGlobalStatistics GetIPv4GlobalStatistics();

		public abstract IPGlobalStatistics GetIPv6GlobalStatistics();

		public abstract TcpStatistics GetTcpIPv4Statistics();

		public abstract TcpStatistics GetTcpIPv6Statistics();

		public abstract UdpStatistics GetUdpIPv4Statistics();

		public abstract UdpStatistics GetUdpIPv6Statistics();
	}
}
