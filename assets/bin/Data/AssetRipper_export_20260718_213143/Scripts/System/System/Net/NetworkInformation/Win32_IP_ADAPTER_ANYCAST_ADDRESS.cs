namespace System.Net.NetworkInformation
{
	internal struct Win32_IP_ADAPTER_ANYCAST_ADDRESS
	{
		public System.Net.NetworkInformation.Win32LengthFlagsUnion LengthFlags;

		public IntPtr Next;

		public System.Net.NetworkInformation.Win32_SOCKET_ADDRESS Address;
	}
}
