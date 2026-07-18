namespace System.Data.SqlClient
{
	[Flags]
	public enum SqlBulkCopyOptions
	{
		CheckConstraints = 2,
		Default = 0,
		FireTriggers = 0x10,
		KeepIdentity = 1,
		KeepNulls = 8,
		TableLock = 4,
		UseInternalTransaction = 0x20
	}
}
