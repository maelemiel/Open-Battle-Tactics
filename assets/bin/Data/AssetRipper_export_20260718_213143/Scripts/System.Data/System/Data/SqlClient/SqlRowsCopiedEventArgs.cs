namespace System.Data.SqlClient
{
	public class SqlRowsCopiedEventArgs : EventArgs
	{
		private long rowsCopied;

		private bool abort;

		public bool Abort
		{
			get
			{
				return abort;
			}
			set
			{
				abort = value;
			}
		}

		public long RowsCopied
		{
			get
			{
				return rowsCopied;
			}
		}

		public SqlRowsCopiedEventArgs(long rowsCopied)
		{
			this.rowsCopied = rowsCopied;
		}
	}
}
