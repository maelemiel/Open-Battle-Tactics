using System.Collections;

namespace System.Data
{
	internal class TableMapping
	{
		private bool existsInDataSet;

		public DataTable Table;

		public ArrayList Elements = new ArrayList();

		public ArrayList Attributes = new ArrayList();

		public DataColumn SimpleContent;

		public DataColumn PrimaryKey;

		public DataColumn ReferenceKey;

		public int lastElementIndex = -1;

		public TableMapping ParentTable;

		public TableMappingCollection ChildTables = new TableMappingCollection();

		public bool ExistsInDataSet
		{
			get
			{
				return existsInDataSet;
			}
		}

		public TableMapping(string name, string ns)
		{
			Table = new DataTable(name);
			Table.Namespace = ns;
		}

		public TableMapping(DataTable dt)
		{
			existsInDataSet = true;
			Table = dt;
			foreach (DataColumn column in dt.Columns)
			{
				switch (column.ColumnMapping)
				{
				case MappingType.Element:
					Elements.Add(column);
					break;
				case MappingType.Attribute:
					Attributes.Add(column);
					break;
				case MappingType.SimpleContent:
					SimpleContent = column;
					break;
				}
			}
			PrimaryKey = ((dt.PrimaryKey.Length <= 0) ? null : dt.PrimaryKey[0]);
		}

		public bool ContainsColumn(string name)
		{
			return GetColumn(name) != null;
		}

		public DataColumn GetColumn(string name)
		{
			foreach (DataColumn element in Elements)
			{
				if (element.ColumnName == name)
				{
					return element;
				}
			}
			foreach (DataColumn attribute in Attributes)
			{
				if (attribute.ColumnName == name)
				{
					return attribute;
				}
			}
			if (SimpleContent != null && name == SimpleContent.ColumnName)
			{
				return SimpleContent;
			}
			if (PrimaryKey != null && name == PrimaryKey.ColumnName)
			{
				return PrimaryKey;
			}
			return null;
		}

		public void RemoveElementColumn(string name)
		{
			foreach (DataColumn element in Elements)
			{
				if (element.ColumnName == name)
				{
					Elements.Remove(element);
					break;
				}
			}
		}
	}
}
