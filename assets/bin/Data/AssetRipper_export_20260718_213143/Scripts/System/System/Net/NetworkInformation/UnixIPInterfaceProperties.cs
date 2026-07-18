using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace System.Net.NetworkInformation
{
	internal abstract class UnixIPInterfaceProperties : IPInterfaceProperties
	{
		protected IPv4InterfaceProperties ipv4iface_properties;

		protected System.Net.NetworkInformation.UnixNetworkInterface iface;

		private List<IPAddress> addresses;

		private IPAddressCollection dns_servers;

		private IPAddressCollection gateways;

		private string dns_suffix;

		private DateTime last_parse;

		private static Regex ns = new Regex("\\s*nameserver\\s+(?<address>.*)");

		private static Regex search = new Regex("\\s*search\\s+(?<domain>.*)");

		public override IPAddressInformationCollection AnycastAddresses
		{
			get
			{
				List<IPAddress> list = new List<IPAddress>();
				return System.Net.NetworkInformation.IPAddressInformationImplCollection.LinuxFromAnycast(list);
			}
		}

		[System.MonoTODO("Always returns an empty collection.")]
		public override IPAddressCollection DhcpServerAddresses
		{
			get
			{
				IPAddressCollection iPAddressCollection = new IPAddressCollection();
				iPAddressCollection.SetReadOnly();
				return iPAddressCollection;
			}
		}

		public override IPAddressCollection DnsAddresses
		{
			get
			{
				ParseResolvConf();
				return dns_servers;
			}
		}

		public override string DnsSuffix
		{
			get
			{
				ParseResolvConf();
				return dns_suffix;
			}
		}

		public override GatewayIPAddressInformationCollection GatewayAddresses
		{
			get
			{
				ParseRouteInfo(iface.Name.ToString());
				if (gateways.Count > 0)
				{
					return new System.Net.NetworkInformation.LinuxGatewayIPAddressInformationCollection(gateways);
				}
				return System.Net.NetworkInformation.LinuxGatewayIPAddressInformationCollection.Empty;
			}
		}

		[System.MonoTODO("Always returns true")]
		public override bool IsDnsEnabled
		{
			get
			{
				return true;
			}
		}

		[System.MonoTODO("Always returns false")]
		public override bool IsDynamicDnsEnabled
		{
			get
			{
				return false;
			}
		}

		public override MulticastIPAddressInformationCollection MulticastAddresses
		{
			get
			{
				List<IPAddress> list = new List<IPAddress>();
				foreach (IPAddress address in addresses)
				{
					byte[] addressBytes = address.GetAddressBytes();
					if (addressBytes[0] >= 224 && addressBytes[0] <= 239)
					{
						list.Add(address);
					}
				}
				return System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection.LinuxFromList(list);
			}
		}

		public override UnicastIPAddressInformationCollection UnicastAddresses
		{
			get
			{
				List<IPAddress> list = new List<IPAddress>();
				foreach (IPAddress address in addresses)
				{
					switch (address.AddressFamily)
					{
					case AddressFamily.InterNetwork:
					{
						byte b = address.GetAddressBytes()[0];
						if (b < 224 || b > 239)
						{
							list.Add(address);
						}
						break;
					}
					case AddressFamily.InterNetworkV6:
						if (!address.IsIPv6Multicast)
						{
							list.Add(address);
						}
						break;
					}
				}
				return System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection.LinuxFromList(list);
			}
		}

		[System.MonoTODO("Always returns an empty collection.")]
		public override IPAddressCollection WinsServersAddresses
		{
			get
			{
				return new IPAddressCollection();
			}
		}

		public UnixIPInterfaceProperties(System.Net.NetworkInformation.UnixNetworkInterface iface, List<IPAddress> addresses)
		{
			this.iface = iface;
			this.addresses = addresses;
		}

		public override IPv6InterfaceProperties GetIPv6Properties()
		{
			throw new NotImplementedException();
		}

		private void ParseRouteInfo(string iface)
		{
			try
			{
				gateways = new IPAddressCollection();
				using (StreamReader streamReader = new StreamReader("/proc/net/route"))
				{
					streamReader.ReadLine();
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						text = text.Trim();
						if (text.Length == 0)
						{
							continue;
						}
						string[] array = text.Split('\t');
						if (array.Length < 3)
						{
							continue;
						}
						string text2 = array[2].Trim();
						byte[] array2 = new byte[4];
						if (text2.Length != 8 || !iface.Equals(array[0], StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						for (int i = 0; i < 4; i++)
						{
							if (!byte.TryParse(text2.Substring(i * 2, 2), NumberStyles.HexNumber, null, out array2[3 - i]))
							{
							}
						}
						IPAddress iPAddress = new IPAddress(array2);
						if (!iPAddress.Equals(IPAddress.Any))
						{
							gateways.Add(iPAddress);
						}
					}
				}
			}
			catch
			{
			}
		}

		private void ParseResolvConf()
		{
			try
			{
				DateTime lastWriteTime = File.GetLastWriteTime("/etc/resolv.conf");
				if (lastWriteTime <= last_parse)
				{
					return;
				}
				last_parse = lastWriteTime;
				dns_suffix = string.Empty;
				dns_servers = new IPAddressCollection();
				using (StreamReader streamReader = new StreamReader("/etc/resolv.conf"))
				{
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						text = text.Trim();
						if (text.Length == 0 || text[0] == '#')
						{
							continue;
						}
						Match match = ns.Match(text);
						if (match.Success)
						{
							try
							{
								string value = match.Groups["address"].Value;
								value = value.Trim();
								dns_servers.Add(IPAddress.Parse(value));
							}
							catch
							{
							}
							continue;
						}
						match = search.Match(text);
						if (match.Success)
						{
							string value = match.Groups["domain"].Value;
							string[] array = value.Split(',');
							dns_suffix = array[0].Trim();
						}
					}
				}
			}
			catch
			{
			}
			finally
			{
				dns_servers.SetReadOnly();
			}
		}
	}
}
