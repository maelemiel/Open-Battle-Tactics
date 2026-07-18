namespace System.Security.Authentication
{
	[Flags]
	public enum SslProtocols
	{
		None = 0,
		Ssl2 = 0xC,
		Ssl3 = 0x30,
		Tls = 0xC0,
		Default = 0xF0
	}
}
