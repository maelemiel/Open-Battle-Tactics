namespace System.Net.NetworkInformation
{
	internal struct Win32_IP_ADAPTER_UNICAST_ADDRESS
	{
		public System.Net.NetworkInformation.Win32LengthFlagsUnion LengthFlags;

		public IntPtr Next;

		public System.Net.NetworkInformation.Win32_SOCKET_ADDRESS Address;

		public PrefixOrigin PrefixOrigin;

		public SuffixOrigin SuffixOrigin;

		public DuplicateAddressDetectionState DadState;

		public uint ValidLifetime;

		public uint PreferredLifetime;

		public uint LeaseLifetime;

		public byte OnLinkPrefixLength;
	}
}
