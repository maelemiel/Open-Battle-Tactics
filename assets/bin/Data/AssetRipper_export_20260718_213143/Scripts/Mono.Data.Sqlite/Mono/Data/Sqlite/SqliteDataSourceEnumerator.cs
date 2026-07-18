using System.Data;
using System.Data.Common;

namespace Mono.Data.Sqlite
{
	public class SqliteDataSourceEnumerator : DbDataSourceEnumerator
	{
		public override DataTable GetDataSources()
		{
			DataTable dataTable = new DataTable();
			DataColumn column = new DataColumn("ServerName", typeof(string));
			dataTable.Columns.Add(column);
			column = new DataColumn("InstanceName", typeof(string));
			dataTable.Columns.Add(column);
			column = new DataColumn("IsClustered", typeof(bool));
			dataTable.Columns.Add(column);
			column = new DataColumn("Version", typeof(string));
			dataTable.Columns.Add(column);
			column = new DataColumn("FactoryName", typeof(string));
			dataTable.Columns.Add(column);
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = "Sqlite Embedded Database";
			dataRow[1] = "Sqlite Default Instance";
			dataRow[2] = false;
			dataRow[3] = "?";
			dataRow[4] = "Mono.Data.Sqlite.SqliteConnectionFactory";
			dataTable.Rows.Add(dataRow);
			return dataTable;
		}
	}
}
