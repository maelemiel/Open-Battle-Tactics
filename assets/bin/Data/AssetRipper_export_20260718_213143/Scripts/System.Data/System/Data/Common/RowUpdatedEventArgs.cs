namespace System.Data.Common
{
	public class RowUpdatedEventArgs : EventArgs
	{
		private DataRow dataRow;

		private IDbCommand command;

		private StatementType statementType;

		private DataTableMapping tableMapping;

		private Exception errors;

		private UpdateStatus status;

		private int recordsAffected;

		public IDbCommand Command
		{
			get
			{
				return command;
			}
		}

		public Exception Errors
		{
			get
			{
				if (errors == null)
				{
					errors = new DataException("RowUpdatedEvent: No additional information is available!");
				}
				return errors;
			}
			set
			{
				errors = value;
			}
		}

		public int RecordsAffected
		{
			get
			{
				return recordsAffected;
			}
		}

		public DataRow Row
		{
			get
			{
				return dataRow;
			}
		}

		public StatementType StatementType
		{
			get
			{
				return statementType;
			}
		}

		public UpdateStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
			}
		}

		public DataTableMapping TableMapping
		{
			get
			{
				return tableMapping;
			}
		}

		public int RowCount
		{
			get
			{
				return 0;
			}
		}

		public RowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			this.dataRow = dataRow;
			this.command = command;
			this.statementType = statementType;
			this.tableMapping = tableMapping;
			status = UpdateStatus.Continue;
		}

		public void CopyToRows(DataRow[] array)
		{
		}

		public void CopyToRows(DataRow[] array, int arrayIndex)
		{
		}
	}
}
