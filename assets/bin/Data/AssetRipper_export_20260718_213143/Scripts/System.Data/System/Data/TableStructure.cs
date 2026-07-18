using System.Collections;

namespace System.Data
{
	internal class TableStructure
	{
		public DataTable Table;

		public Hashtable OrdinalColumns = new Hashtable();

		public ArrayList NonOrdinalColumns = new ArrayList();

		public DataColumn PrimaryKey;

		public TableStructure(DataTable table)
		{
			Table = table;
		}

		public bool ContainsColumn(string name)
		{
			foreach (DataColumn nonOrdinalColumn in NonOrdinalColumns)
			{
				if (nonOrdinalColumn.ColumnName == name)
				{
					return true;
				}
			}
			foreach (DataColumn key in OrdinalColumns.Keys)
			{
				if (key.ColumnName == name)
				{
					return true;
				}
			}
			return false;
		}
	}
}
