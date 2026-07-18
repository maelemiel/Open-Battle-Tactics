namespace System.Data
{
	public class FillErrorEventArgs : EventArgs
	{
		private DataTable data_table;

		private object[] values;

		private Exception errors;

		private bool f_continue;

		public bool Continue
		{
			get
			{
				return f_continue;
			}
			set
			{
				f_continue = value;
			}
		}

		public DataTable DataTable
		{
			get
			{
				return data_table;
			}
		}

		public Exception Errors
		{
			get
			{
				return errors;
			}
			set
			{
				errors = value;
			}
		}

		public object[] Values
		{
			get
			{
				return values;
			}
		}

		public FillErrorEventArgs(DataTable dataTable, object[] values)
		{
			data_table = dataTable;
			this.values = values;
		}
	}
}
