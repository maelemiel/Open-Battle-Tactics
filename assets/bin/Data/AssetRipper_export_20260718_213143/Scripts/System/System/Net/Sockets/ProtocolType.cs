namespace System.Net.Sockets
{
	public enum ProtocolType
	{
		IP = 0,
		Icmp = 1,
		Igmp = 2,
		Ggp = 3,
		Tcp = 6,
		Pup = 12,
		Udp = 17,
		Idp = 22,
		IPv6 = 41,
		ND = 77,
		Raw = 255,
		Unspecified = 0,
		Ipx = 1000,
		Spx = 1256,
		SpxII = 1257,
		Unknown = -1,
		IPv4 = 4,
		IPv6RoutingHeader = 43,
		IPv6FragmentHeader = 44,
		IPSecEncapsulatingSecurityPayload = 50,
		IPSecAuthenticationHeader = 51,
		IcmpV6 = 58,
		IPv6NoNextHeader = 59,
		IPv6DestinationOptions = 60,
		IPv6HopByHopOptions = 0
	}
}
