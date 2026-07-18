namespace System.Transactions
{
	public enum IsolationLevel
	{
		Serializable = 0,
		RepeatableRead = 1,
		ReadCommitted = 2,
		ReadUncommitted = 3,
		Snapshot = 4,
		Chaos = 5,
		Unspecified = 6
	}
}
