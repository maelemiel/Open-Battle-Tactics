namespace System.Data
{
	public sealed class StatementCompletedEventArgs : EventArgs
	{
		private int recordCount;

		public int RecordCount
		{
			get
			{
				return recordCount;
			}
		}

		public StatementCompletedEventArgs(int recordCount)
		{
			this.recordCount = recordCount;
		}
	}
}
