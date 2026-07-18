using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class Win32_IP_ADAPTER_ADDRESSES
	{
		private const int MAX_ADAPTER_ADDRESS_LENGTH = 8;

		private const int IP_ADAPTER_DDNS_ENABLED = 1;

		private const int IP_ADAPTER_RECEIVE_ONLY = 8;

		private const int IP_ADAPTER_NO_MULTICAST = 16;

		public System.Net.NetworkInformation.AlignmentUnion Alignment;

		public IntPtr Next;

		[MarshalAs(UnmanagedType.LPStr)]
		public string AdapterName;

		public IntPtr FirstUnicastAddress;

		public IntPtr FirstAnycastAddress;

		public IntPtr FirstMulticastAddress;

		public IntPtr FirstDnsServerAddress;

		public string DnsSuffix;

		public string Description;

		public string FriendlyName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] PhysicalAddress;

		public uint PhysicalAddressLength;

		public uint Flags;

		public uint Mtu;

		public NetworkInterfaceType IfType;

		public OperationalStatus OperStatus;

		public int Ipv6IfIndex;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public uint[] ZoneIndices;

		public bool DdnsEnabled
		{
			get
			{
				return (Flags & 1) != 0;
			}
		}

		public bool IsReceiveOnly
		{
			get
			{
				return (Flags & 8) != 0;
			}
		}

		public bool NoMulticast
		{
			get
			{
				return (Flags & 0x10) != 0;
			}
		}
	}
}
