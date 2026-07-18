namespace System.Data
{
	public class DataRowChangeEventArgs : EventArgs
	{
		private DataRow row;

		private DataRowAction action;

		public DataRowAction Action
		{
			get
			{
				return action;
			}
		}

		public DataRow Row
		{
			get
			{
				return row;
			}
		}

		public DataRowChangeEventArgs(DataRow row, DataRowAction action)
		{
			this.row = row;
			this.action = action;
		}
	}
}
