using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class Win32IPGlobalProperties : IPGlobalProperties
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct Win32_IN6_ADDR
		{
			[FieldOffset(0)]
			[MarshalAs(UnmanagedType.U1)]
			public byte[] Bytes;

			[FieldOffset(0)]
			[MarshalAs(UnmanagedType.U2)]
			public byte[] UInt16Array;

			[FieldOffset(0)]
			[MarshalAs(UnmanagedType.U4)]
			public byte[] UInt32Array;
		}

		[StructLayout(LayoutKind.Sequential)]
		private class Win32_MIB_TCPROW
		{
			public TcpState State;

			public uint LocalAddr;

			public int LocalPort;

			public uint RemoteAddr;

			public int RemotePort;

			public IPEndPoint LocalEndPoint
			{
				get
				{
					return new IPEndPoint(LocalAddr, LocalPort);
				}
			}

			public IPEndPoint RemoteEndPoint
			{
				get
				{
					return new IPEndPoint(RemoteAddr, RemotePort);
				}
			}

			public TcpConnectionInformation TcpInfo
			{
				get
				{
					return new System.Net.NetworkInformation.TcpConnectionInformationImpl(LocalEndPoint, RemoteEndPoint, State);
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private class Win32_MIB_TCP6ROW
		{
			public TcpState State;

			public Win32_IN6_ADDR LocalAddr;

			public uint LocalScopeId;

			public int LocalPort;

			public Win32_IN6_ADDR RemoteAddr;

			public uint RemoteScopeId;

			public int RemotePort;

			public IPEndPoint LocalEndPoint
			{
				get
				{
					return new IPEndPoint(new IPAddress(LocalAddr.Bytes, LocalScopeId), LocalPort);
				}
			}

			public IPEndPoint RemoteEndPoint
			{
				get
				{
					return new IPEndPoint(new IPAddress(RemoteAddr.Bytes, RemoteScopeId), RemotePort);
				}
			}

			public TcpConnectionInformation TcpInfo
			{
				get
				{
					return new System.Net.NetworkInformation.TcpConnectionInformationImpl(LocalEndPoint, RemoteEndPoint, State);
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private class Win32_MIB_UDPROW
		{
			public uint LocalAddr;

			public int LocalPort;

			public IPEndPoint LocalEndPoint
			{
				get
				{
					return new IPEndPoint(LocalAddr, LocalPort);
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		private class Win32_MIB_UDP6ROW
		{
			public Win32_IN6_ADDR LocalAddr;

			public uint LocalScopeId;

			public int LocalPort;

			public IPEndPoint LocalEndPoint
			{
				get
				{
					return new IPEndPoint(new IPAddress(LocalAddr.Bytes, LocalScopeId), LocalPort);
				}
			}
		}

		public const int AF_INET = 2;

		public const int AF_INET6 = 23;

		public override string DhcpScopeName
		{
			get
			{
				return System.Net.NetworkInformation.Win32_FIXED_INFO.Instance.ScopeId;
			}
		}

		public override string DomainName
		{
			get
			{
				return System.Net.NetworkInformation.Win32_FIXED_INFO.Instance.DomainName;
			}
		}

		public override string HostName
		{
			get
			{
				return System.Net.NetworkInformation.Win32_FIXED_INFO.Instance.HostName;
			}
		}

		public override bool IsWinsProxy
		{
			get
			{
				return System.Net.NetworkInformation.Win32_FIXED_INFO.Instance.EnableProxy != 0;
			}
		}

		public override NetBiosNodeType NodeType
		{
			get
			{
				return System.Net.NetworkInformation.Win32_FIXED_INFO.Instance.NodeType;
			}
		}

		private unsafe void FillTcpTable(out List<Win32_MIB_TCPROW> tab4, out List<Win32_MIB_TCP6ROW> tab6)
		{
			//IL_0049->IL0050: Incompatible stack types: I vs Ref
			//IL_0101->IL0109: Incompatible stack types: I vs Ref
			tab4 = new List<Win32_MIB_TCPROW>();
			int pdwSize = 0;
			GetTcpTable(null, ref pdwSize, true);
			byte[] array = new byte[pdwSize];
			GetTcpTable(array, ref pdwSize, true);
			int num = Marshal.SizeOf(typeof(Win32_MIB_TCPROW));
			fixed (byte* ptr = &(array != null && array.Length != 0 ? ref array[0] : ref *(byte*)null))
			{
				int num2 = Marshal.ReadInt32((IntPtr)ptr);
				for (int i = 0; i < num2; i++)
				{
					Win32_MIB_TCPROW win32_MIB_TCPROW = new Win32_MIB_TCPROW();
					Marshal.PtrToStructure((IntPtr)(ptr + i * num + 4), win32_MIB_TCPROW);
					tab4.Add(win32_MIB_TCPROW);
				}
			}
			tab6 = new List<Win32_MIB_TCP6ROW>();
			if (Environment.OSVersion.Version.Major < 6)
			{
				return;
			}
			int SizePointer = 0;
			GetTcp6Table(null, ref SizePointer, true);
			byte[] array2 = new byte[SizePointer];
			GetTcp6Table(array2, ref SizePointer, true);
			int num3 = Marshal.SizeOf(typeof(Win32_MIB_TCP6ROW));
			fixed (byte* ptr2 = &(array2 != null && array2.Length != 0 ? ref array2[0] : ref *(byte*)null))
			{
				int num4 = Marshal.ReadInt32((IntPtr)ptr2);
				for (int j = 0; j < num4; j++)
				{
					Win32_MIB_TCP6ROW win32_MIB_TCP6ROW = new Win32_MIB_TCP6ROW();
					Marshal.PtrToStructure((IntPtr)(ptr2 + j * num3 + 4), win32_MIB_TCP6ROW);
					tab6.Add(win32_MIB_TCP6ROW);
				}
			}
		}

		private bool IsListenerState(TcpState state)
		{
			switch (state)
			{
			case TcpState.Listen:
			case TcpState.SynSent:
			case TcpState.FinWait1:
			case TcpState.FinWait2:
			case TcpState.CloseWait:
				return true;
			default:
				return false;
			}
		}

		public override TcpConnectionInformation[] GetActiveTcpConnections()
		{
			List<Win32_MIB_TCPROW> tab = null;
			List<Win32_MIB_TCP6ROW> tab2 = null;
			FillTcpTable(out tab, out tab2);
			int count = tab.Count;
			TcpConnectionInformation[] array = new TcpConnectionInformation[count + tab2.Count];
			for (int i = 0; i < count; i++)
			{
				array[i] = tab[i].TcpInfo;
			}
			for (int j = 0; j < tab2.Count; j++)
			{
				array[count + j] = tab2[j].TcpInfo;
			}
			return array;
		}

		public override IPEndPoint[] GetActiveTcpListeners()
		{
			List<Win32_MIB_TCPROW> tab = null;
			List<Win32_MIB_TCP6ROW> tab2 = null;
			FillTcpTable(out tab, out tab2);
			List<IPEndPoint> list = new List<IPEndPoint>();
			int i = 0;
			for (int count = tab.Count; i < count; i++)
			{
				if (IsListenerState(tab[i].State))
				{
					list.Add(tab[i].LocalEndPoint);
				}
			}
			int j = 0;
			for (int count2 = tab2.Count; j < count2; j++)
			{
				if (IsListenerState(tab2[j].State))
				{
					list.Add(tab2[j].LocalEndPoint);
				}
			}
			return list.ToArray();
		}

		public unsafe override IPEndPoint[] GetActiveUdpListeners()
		{
			//IL_004a->IL0051: Incompatible stack types: I vs Ref
			//IL_0106->IL010e: Incompatible stack types: I vs Ref
			List<IPEndPoint> list = new List<IPEndPoint>();
			byte[] array = null;
			int pdwSize = 0;
			GetUdpTable(null, ref pdwSize, true);
			array = new byte[pdwSize];
			GetUdpTable(array, ref pdwSize, true);
			int num = Marshal.SizeOf(typeof(Win32_MIB_UDPROW));
			fixed (byte* ptr = &(array != null && array.Length != 0 ? ref array[0] : ref *(byte*)null))
			{
				int num2 = Marshal.ReadInt32((IntPtr)ptr);
				for (int i = 0; i < num2; i++)
				{
					Win32_MIB_UDPROW win32_MIB_UDPROW = new Win32_MIB_UDPROW();
					Marshal.PtrToStructure((IntPtr)(ptr + i * num + 4), win32_MIB_UDPROW);
					list.Add(win32_MIB_UDPROW.LocalEndPoint);
				}
			}
			if (Environment.OSVersion.Version.Major >= 6)
			{
				byte[] array2 = null;
				int SizePointer = 0;
				GetUdp6Table(null, ref SizePointer, true);
				array2 = new byte[SizePointer];
				GetUdp6Table(array2, ref SizePointer, true);
				int num3 = Marshal.SizeOf(typeof(Win32_MIB_UDP6ROW));
				fixed (byte* ptr2 = &(array2 != null && array2.Length != 0 ? ref array2[0] : ref *(byte*)null))
				{
					int num4 = Marshal.ReadInt32((IntPtr)ptr2);
					for (int j = 0; j < num4; j++)
					{
						Win32_MIB_UDP6ROW win32_MIB_UDP6ROW = new Win32_MIB_UDP6ROW();
						Marshal.PtrToStructure((IntPtr)(ptr2 + j * num3 + 4), win32_MIB_UDP6ROW);
						list.Add(win32_MIB_UDP6ROW.LocalEndPoint);
					}
				}
			}
			return list.ToArray();
		}

		public override IcmpV4Statistics GetIcmpV4Statistics()
		{
			if (!Socket.SupportsIPv4)
			{
				throw new NetworkInformationException();
			}
			System.Net.NetworkInformation.Win32_MIBICMPINFO pStats;
			GetIcmpStatistics(out pStats, 2);
			return new System.Net.NetworkInformation.Win32IcmpV4Statistics(pStats);
		}

		public override IcmpV6Statistics GetIcmpV6Statistics()
		{
			if (!Socket.OSSupportsIPv6)
			{
				throw new NetworkInformationException();
			}
			System.Net.NetworkInformation.Win32_MIB_ICMP_EX pStats;
			GetIcmpStatisticsEx(out pStats, 23);
			return new System.Net.NetworkInformation.Win32IcmpV6Statistics(pStats);
		}

		public override IPGlobalStatistics GetIPv4GlobalStatistics()
		{
			if (!Socket.SupportsIPv4)
			{
				throw new NetworkInformationException();
			}
			System.Net.NetworkInformation.Win32_MIB_IPSTATS pStats;
			GetIPStatisticsEx(out pStats, 2);
			return new System.Net.NetworkInformation.Win32IPGlobalStatistics(pStats);
		}

		public override IPGlobalStatistics GetIPv6GlobalStatistics()
		{
			if (!Socket.OSSupportsIPv6)
			{
				throw new NetworkInformationException();
			}
			System.Net.NetworkInformation.Win32_MIB_IPSTATS pStats;
			GetIPStatisticsEx(out pStats, 23);
			return new System.Net.NetworkInformation.Win32IPGlobalStatistics(pStats);
		}

		public override TcpStatistics GetTcpIPv4Statistics()
		{
			if (!Socket.SupportsIPv4)
			{
				throw new NetworkInformationException();
			}
			System.Net.NetworkInformation.Win32_MIB_TCPSTATS pStats;
			GetTcpStatisticsEx(out pStats, 2);
			return new System.Net.NetworkInformation.Win32TcpStatistics(pStats);
		}

		public override TcpStatistics GetTcpIPv6Statistics()
		{
			if (!Socket.OSSupportsIPv6)
			{
				throw new NetworkInformationException();
			}
			System.Net.NetworkInformation.Win32_MIB_TCPSTATS pStats;
			GetTcpStatisticsEx(out pStats, 23);
			return new System.Net.NetworkInformation.Win32TcpStatistics(pStats);
		}

		public override UdpStatistics GetUdpIPv4Statistics()
		{
			if (!Socket.SupportsIPv4)
			{
				throw new NetworkInformationException();
			}
			System.Net.NetworkInformation.Win32_MIB_UDPSTATS pStats;
			GetUdpStatisticsEx(out pStats, 2);
			return new System.Net.NetworkInformation.Win32UdpStatistics(pStats);
		}

		public override UdpStatistics GetUdpIPv6Statistics()
		{
			if (!Socket.OSSupportsIPv6)
			{
				throw new NetworkInformationException();
			}
			System.Net.NetworkInformation.Win32_MIB_UDPSTATS pStats;
			GetUdpStatisticsEx(out pStats, 23);
			return new System.Net.NetworkInformation.Win32UdpStatistics(pStats);
		}

		[DllImport("Iphlpapi.dll")]
		private static extern int GetTcpTable(byte[] pTcpTable, ref int pdwSize, bool bOrder);

		[DllImport("Iphlpapi.dll")]
		private static extern int GetTcp6Table(byte[] TcpTable, ref int SizePointer, bool Order);

		[DllImport("Iphlpapi.dll")]
		private static extern int GetUdpTable(byte[] pUdpTable, ref int pdwSize, bool bOrder);

		[DllImport("Iphlpapi.dll")]
		private static extern int GetUdp6Table(byte[] Udp6Table, ref int SizePointer, bool Order);

		[DllImport("Iphlpapi.dll")]
		private static extern int GetTcpStatisticsEx(out System.Net.NetworkInformation.Win32_MIB_TCPSTATS pStats, int dwFamily);

		[DllImport("Iphlpapi.dll")]
		private static extern int GetUdpStatisticsEx(out System.Net.NetworkInformation.Win32_MIB_UDPSTATS pStats, int dwFamily);

		[DllImport("Iphlpapi.dll")]
		private static extern int GetIcmpStatistics(out System.Net.NetworkInformation.Win32_MIBICMPINFO pStats, int dwFamily);

		[DllImport("Iphlpapi.dll")]
		private static extern int GetIcmpStatisticsEx(out System.Net.NetworkInformation.Win32_MIB_ICMP_EX pStats, int dwFamily);

		[DllImport("Iphlpapi.dll")]
		private static extern int GetIPStatisticsEx(out System.Net.NetworkInformation.Win32_MIB_IPSTATS pStats, int dwFamily);
	}
}
