using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net.NetworkInformation
{
	internal class MibIPGlobalProperties : IPGlobalProperties
	{
		public const string ProcDir = "/proc";

		public const string CompatProcDir = "/usr/compat/linux/proc";

		public readonly string StatisticsFile;

		public readonly string StatisticsFileIPv6;

		public readonly string TcpFile;

		public readonly string Tcp6File;

		public readonly string UdpFile;

		public readonly string Udp6File;

		private static readonly char[] wsChars = new char[2] { ' ', '\t' };

		public override string DhcpScopeName
		{
			get
			{
				return string.Empty;
			}
		}

		public override string DomainName
		{
			get
			{
				byte[] array = new byte[256];
				if (getdomainname(array, 256) != 0)
				{
					throw new NetworkInformationException();
				}
				int num = Array.IndexOf(array, (byte)0);
				return Encoding.ASCII.GetString(array, 0, (num >= 0) ? num : 256);
			}
		}

		public override string HostName
		{
			get
			{
				byte[] array = new byte[256];
				if (gethostname(array, 256) != 0)
				{
					throw new NetworkInformationException();
				}
				int num = Array.IndexOf(array, (byte)0);
				return Encoding.ASCII.GetString(array, 0, (num >= 0) ? num : 256);
			}
		}

		public override bool IsWinsProxy
		{
			get
			{
				return false;
			}
		}

		public override NetBiosNodeType NodeType
		{
			get
			{
				return NetBiosNodeType.Unknown;
			}
		}

		public MibIPGlobalProperties(string procDir)
		{
			StatisticsFile = Path.Combine(procDir, "net/snmp");
			StatisticsFileIPv6 = Path.Combine(procDir, "net/snmp6");
			TcpFile = Path.Combine(procDir, "net/tcp");
			Tcp6File = Path.Combine(procDir, "net/tcp6");
			UdpFile = Path.Combine(procDir, "net/udp");
			Udp6File = Path.Combine(procDir, "net/udp6");
		}

		[DllImport("libc")]
		private static extern int gethostname([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 1)] byte[] name, int len);

		[DllImport("libc")]
		private static extern int getdomainname([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 1)] byte[] name, int len);

		private StringDictionary GetProperties4(string item)
		{
			string statisticsFile = StatisticsFile;
			string text = item + ": ";
			using (StreamReader streamReader = new StreamReader(statisticsFile, Encoding.ASCII))
			{
				string[] array = null;
				string[] array2 = null;
				string empty = string.Empty;
				do
				{
					empty = streamReader.ReadLine();
					if (string.IsNullOrEmpty(empty) || empty.Length <= text.Length || string.CompareOrdinal(empty, 0, text, 0, text.Length) != 0)
					{
						continue;
					}
					if (array == null)
					{
						array = empty.Substring(text.Length).Split(' ');
						continue;
					}
					if (array2 != null)
					{
						throw CreateException(statisticsFile, string.Format("Found duplicate line for values for the same item '{0}'", item));
					}
					array2 = empty.Substring(text.Length).Split(' ');
					break;
				}
				while (!streamReader.EndOfStream);
				if (array2 == null)
				{
					throw CreateException(statisticsFile, string.Format("No corresponding line was not found for '{0}'", item));
				}
				if (array.Length != array2.Length)
				{
					throw CreateException(statisticsFile, string.Format("The counts in the header line and the value line do not match for '{0}'", item));
				}
				StringDictionary stringDictionary = new StringDictionary();
				for (int i = 0; i < array.Length; i++)
				{
					stringDictionary[array[i]] = array2[i];
				}
				return stringDictionary;
			}
		}

		private StringDictionary GetProperties6(string item)
		{
			if (!File.Exists(StatisticsFileIPv6))
			{
				throw new NetworkInformationException();
			}
			string statisticsFileIPv = StatisticsFileIPv6;
			using (StreamReader streamReader = new StreamReader(statisticsFileIPv, Encoding.ASCII))
			{
				StringDictionary stringDictionary = new StringDictionary();
				string empty = string.Empty;
				do
				{
					empty = streamReader.ReadLine();
					if (!string.IsNullOrEmpty(empty) && empty.Length > item.Length && string.CompareOrdinal(empty, 0, item, 0, item.Length) == 0)
					{
						int num = empty.IndexOfAny(wsChars, item.Length);
						if (num < 0)
						{
							throw CreateException(statisticsFileIPv, null);
						}
						stringDictionary[empty.Substring(item.Length, num - item.Length)] = empty.Substring(num + 1).Trim(wsChars);
					}
				}
				while (!streamReader.EndOfStream);
				return stringDictionary;
			}
		}

		private Exception CreateException(string file, string msg)
		{
			return new InvalidOperationException(string.Format("Unsupported (unexpected) '{0}' file format. ", file) + msg);
		}

		private IPEndPoint[] GetLocalAddresses(List<string[]> list)
		{
			IPEndPoint[] array = new IPEndPoint[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ToEndpoint(list[i][1]);
			}
			return array;
		}

		private IPEndPoint ToEndpoint(string s)
		{
			int num = s.IndexOf(':');
			int port = int.Parse(s.Substring(num + 1), NumberStyles.HexNumber);
			if (s.Length == 13)
			{
				return new IPEndPoint(long.Parse(s.Substring(0, num), NumberStyles.HexNumber), port);
			}
			byte[] array = new byte[16];
			for (int i = 0; i << 1 < num; i++)
			{
				array[i] = byte.Parse(s.Substring(i << 1, 2), NumberStyles.HexNumber);
			}
			return new IPEndPoint(new IPAddress(array), port);
		}

		private void GetRows(string file, List<string[]> list)
		{
			if (!File.Exists(file))
			{
				return;
			}
			using (StreamReader streamReader = new StreamReader(file, Encoding.ASCII))
			{
				streamReader.ReadLine();
				while (!streamReader.EndOfStream)
				{
					string[] array = streamReader.ReadLine().Split(wsChars, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length < 4)
					{
						throw CreateException(file, null);
					}
					list.Add(array);
				}
			}
		}

		public override TcpConnectionInformation[] GetActiveTcpConnections()
		{
			List<string[]> list = new List<string[]>();
			GetRows(TcpFile, list);
			GetRows(Tcp6File, list);
			TcpConnectionInformation[] array = new TcpConnectionInformation[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				IPEndPoint local = ToEndpoint(list[i][1]);
				IPEndPoint remote = ToEndpoint(list[i][2]);
				TcpState state = (TcpState)int.Parse(list[i][3], NumberStyles.HexNumber);
				array[i] = new System.Net.NetworkInformation.TcpConnectionInformationImpl(local, remote, state);
			}
			return array;
		}

		public override IPEndPoint[] GetActiveTcpListeners()
		{
			List<string[]> list = new List<string[]>();
			GetRows(TcpFile, list);
			GetRows(Tcp6File, list);
			return GetLocalAddresses(list);
		}

		public override IPEndPoint[] GetActiveUdpListeners()
		{
			List<string[]> list = new List<string[]>();
			GetRows(UdpFile, list);
			GetRows(Udp6File, list);
			return GetLocalAddresses(list);
		}

		public override IcmpV4Statistics GetIcmpV4Statistics()
		{
			return new System.Net.NetworkInformation.MibIcmpV4Statistics(GetProperties4("Icmp"));
		}

		public override IcmpV6Statistics GetIcmpV6Statistics()
		{
			return new System.Net.NetworkInformation.MibIcmpV6Statistics(GetProperties6("Icmp6"));
		}

		public override IPGlobalStatistics GetIPv4GlobalStatistics()
		{
			return new System.Net.NetworkInformation.MibIPGlobalStatistics(GetProperties4("Ip"));
		}

		public override IPGlobalStatistics GetIPv6GlobalStatistics()
		{
			return new System.Net.NetworkInformation.MibIPGlobalStatistics(GetProperties6("Ip6"));
		}

		public override TcpStatistics GetTcpIPv4Statistics()
		{
			return new System.Net.NetworkInformation.MibTcpStatistics(GetProperties4("Tcp"));
		}

		public override TcpStatistics GetTcpIPv6Statistics()
		{
			return new System.Net.NetworkInformation.MibTcpStatistics(GetProperties4("Tcp"));
		}

		public override UdpStatistics GetUdpIPv4Statistics()
		{
			return new System.Net.NetworkInformation.MibUdpStatistics(GetProperties4("Udp"));
		}

		public override UdpStatistics GetUdpIPv6Statistics()
		{
			return new System.Net.NetworkInformation.MibUdpStatistics(GetProperties6("Udp6"));
		}
	}
}
