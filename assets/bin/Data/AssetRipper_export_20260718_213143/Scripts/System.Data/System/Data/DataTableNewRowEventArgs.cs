namespace System.Data
{
	public sealed class DataTableNewRowEventArgs : EventArgs
	{
		private readonly DataRow _row;

		public DataRow Row
		{
			get
			{
				return _row;
			}
		}

		public DataTableNewRowEventArgs(DataRow row)
		{
			_row = row;
		}
	}
}
