namespace Mono.Data.Tds.Protocol
{
	public enum TdsPacketType
	{
		None = 0,
		Query = 1,
		Logon = 2,
		Proc = 3,
		Reply = 4,
		Cancel = 6,
		Bulk = 7,
		Logon70 = 16,
		SspAuth = 17,
		Logoff = 113,
		Normal = 15,
		DBRPC = 230,
		RPC = 3
	}
}
