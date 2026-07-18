namespace Mono.Data.Tds.Protocol
{
	public enum TdsColumnStatus
	{
		IsExpression = 4,
		IsKey = 8,
		Hidden = 0x10,
		Rename = 0x20
	}
}
