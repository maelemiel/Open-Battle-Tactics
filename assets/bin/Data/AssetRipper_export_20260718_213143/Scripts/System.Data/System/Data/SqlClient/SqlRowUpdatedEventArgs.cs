using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		public new SqlCommand Command
		{
			get
			{
				return (SqlCommand)base.Command;
			}
		}

		public SqlRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(row, command, statementType, tableMapping)
		{
		}
	}
}
