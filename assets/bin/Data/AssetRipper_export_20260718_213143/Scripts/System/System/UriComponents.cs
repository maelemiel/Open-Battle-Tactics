namespace System
{
	[Flags]
	public enum UriComponents
	{
		Scheme = 1,
		UserInfo = 2,
		Host = 4,
		Port = 8,
		Path = 0x10,
		Query = 0x20,
		Fragment = 0x40,
		StrongPort = 0x80,
		KeepDelimiter = 0x40000000,
		HostAndPort = 0x84,
		StrongAuthority = 0x86,
		AbsoluteUri = 0x7F,
		PathAndQuery = 0x30,
		HttpRequestUrl = 0x3D,
		SchemeAndServer = 0xD,
		SerializationInfoString = int.MinValue
	}
}
