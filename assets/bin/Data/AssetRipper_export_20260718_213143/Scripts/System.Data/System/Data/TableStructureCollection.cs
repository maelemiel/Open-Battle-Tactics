using System.Collections;

namespace System.Data
{
	internal class TableStructureCollection : CollectionBase
	{
		public TableStructure this[int i]
		{
			get
			{
				return base.List[i] as TableStructure;
			}
		}

		public TableStructure this[string name]
		{
			get
			{
				foreach (TableStructure item in base.List)
				{
					if (item.Table.TableName == name)
					{
						return item;
					}
				}
				return null;
			}
		}

		public void Add(TableStructure table)
		{
			base.List.Add(table);
		}
	}
}
