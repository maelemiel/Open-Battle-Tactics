namespace System.Data
{
	public enum UpdateStatus
	{
		Continue = 0,
		ErrorsOccurred = 1,
		SkipCurrentRow = 2,
		SkipAllRemainingRows = 3
	}
}
