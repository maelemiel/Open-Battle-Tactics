namespace System.Data
{
	public class MergeFailedEventArgs : EventArgs
	{
		private readonly DataTable data_table;

		private readonly string conflict;

		public DataTable Table
		{
			get
			{
				return data_table;
			}
		}

		public string Conflict
		{
			get
			{
				return conflict;
			}
		}

		public MergeFailedEventArgs(DataTable table, string conflict)
		{
			data_table = table;
			this.conflict = conflict;
		}
	}
}
