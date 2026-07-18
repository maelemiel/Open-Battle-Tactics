namespace System.Data
{
	public sealed class DataTableClearEventArgs : EventArgs
	{
		private readonly DataTable _table;

		public DataTable Table
		{
			get
			{
				return _table;
			}
		}

		public string TableName
		{
			get
			{
				return _table.TableName;
			}
		}

		public string TableNamespace
		{
			get
			{
				return _table.Namespace;
			}
		}

		public DataTableClearEventArgs(DataTable table)
		{
			_table = table;
		}
	}
}
