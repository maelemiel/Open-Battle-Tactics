namespace System.Data
{
	public sealed class DataRowBuilder
	{
		private DataTable table;

		internal int _rowId;

		internal DataTable Table
		{
			get
			{
				return table;
			}
		}

		internal DataRowBuilder(DataTable table, int rowID, int y)
		{
			this.table = table;
			_rowId = rowID;
		}
	}
}
