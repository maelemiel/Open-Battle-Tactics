namespace System.Data.Common
{
	public class RowUpdatingEventArgs : EventArgs
	{
		private DataRow dataRow;

		private IDbCommand command;

		private StatementType statementType;

		private DataTableMapping tableMapping;

		private UpdateStatus status;

		private Exception errors;

		public IDbCommand Command
		{
			get
			{
				return command;
			}
			set
			{
				command = value;
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

		protected virtual IDbCommand BaseCommand
		{
			get
			{
				return command;
			}
			set
			{
				command = value;
			}
		}

		public RowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			this.dataRow = dataRow;
			this.command = command;
			this.statementType = statementType;
			this.tableMapping = tableMapping;
			status = UpdateStatus.Continue;
			errors = null;
		}
	}
}
