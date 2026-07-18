namespace System.Data
{
	public class DataColumnChangeEventArgs : EventArgs
	{
		private DataColumn _column;

		private DataRow _row;

		private object _proposedValue;

		public DataColumn Column
		{
			get
			{
				return _column;
			}
		}

		public object ProposedValue
		{
			get
			{
				return _proposedValue;
			}
			set
			{
				_proposedValue = value;
			}
		}

		public DataRow Row
		{
			get
			{
				return _row;
			}
		}

		public DataColumnChangeEventArgs(DataRow row, DataColumn column, object value)
		{
			Initialize(row, column, value);
		}

		internal DataColumnChangeEventArgs()
		{
		}

		internal void Initialize(DataRow row, DataColumn column, object value)
		{
			_column = column;
			_row = row;
			_proposedValue = value;
		}
	}
}
