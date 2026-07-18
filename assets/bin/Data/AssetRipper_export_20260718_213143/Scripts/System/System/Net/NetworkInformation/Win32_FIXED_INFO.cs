using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	[StructLayout(LayoutKind.Sequential)]
	internal class Win32_FIXED_INFO
	{
		private const int MAX_HOSTNAME_LEN = 128;

		private const int MAX_DOMAIN_NAME_LEN = 128;

		private const int MAX_SCOPE_ID_LEN = 256;

		private static System.Net.NetworkInformation.Win32_FIXED_INFO fixed_info;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
		public string HostName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
		public string DomainName;

		public IntPtr CurrentDnsServer;

		public System.Net.NetworkInformation.Win32_IP_ADDR_STRING DnsServerList;

		public NetBiosNodeType NodeType;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string ScopeId;

		public uint EnableRouting;

		public uint EnableProxy;

		public uint EnableDns;

		public static System.Net.NetworkInformation.Win32_FIXED_INFO Instance
		{
			get
			{
				if (fixed_info == null)
				{
					fixed_info = GetInstance();
				}
				return fixed_info;
			}
		}

		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetNetworkParams(byte[] bytes, ref int size);

		private unsafe static System.Net.NetworkInformation.Win32_FIXED_INFO GetInstance()
		{
			//IL_0038->IL003f: Incompatible stack types: I vs Ref
			int size = 0;
			byte[] array = null;
			GetNetworkParams(null, ref size);
			array = new byte[size];
			GetNetworkParams(array, ref size);
			System.Net.NetworkInformation.Win32_FIXED_INFO win32_FIXED_INFO = new System.Net.NetworkInformation.Win32_FIXED_INFO();
			fixed (byte* ptr = &(array != null && array.Length != 0 ? ref array[0] : ref *(byte*)null))
			{
				Marshal.PtrToStructure((IntPtr)ptr, win32_FIXED_INFO);
			}
			return win32_FIXED_INFO;
		}
	}
}
