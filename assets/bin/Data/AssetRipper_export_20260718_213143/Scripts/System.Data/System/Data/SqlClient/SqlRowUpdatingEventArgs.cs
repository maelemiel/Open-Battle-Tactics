using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlRowUpdatingEventArgs : RowUpdatingEventArgs
	{
		public new SqlCommand Command
		{
			get
			{
				return (SqlCommand)base.Command;
			}
			set
			{
				base.Command = value;
			}
		}

		protected override IDbCommand BaseCommand
		{
			get
			{
				return base.BaseCommand;
			}
			set
			{
				base.BaseCommand = value as SqlCommand;
			}
		}

		public SqlRowUpdatingEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(row, command, statementType, tableMapping)
		{
		}
	}
}
