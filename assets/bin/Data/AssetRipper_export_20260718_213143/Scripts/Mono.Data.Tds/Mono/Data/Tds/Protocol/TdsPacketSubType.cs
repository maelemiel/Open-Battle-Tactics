namespace Mono.Data.Tds.Protocol
{
	public enum TdsPacketSubType
	{
		Capability = 226,
		Dynamic = 231,
		Dynamic2 = 163,
		EnvironmentChange = 227,
		Error = 170,
		Info = 171,
		EED = 229,
		Param = 172,
		Authentication = 237,
		LoginAck = 173,
		ReturnStatus = 121,
		ProcId = 124,
		Done = 253,
		DoneProc = 254,
		DoneInProc = 255,
		ColumnName = 160,
		ColumnInfo = 161,
		ColumnDetail = 165,
		AltName = 167,
		AltFormat = 168,
		TableName = 164,
		ColumnOrder = 169,
		Control = 174,
		Row = 209,
		ColumnMetadata = 129,
		RowFormat = 238,
		ParamFormat = 236,
		Parameters = 215
	}
}
