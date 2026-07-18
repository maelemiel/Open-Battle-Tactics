using System.Collections;

namespace System.Data
{
	internal class TableMappingCollection : CollectionBase
	{
		public TableMapping this[string name]
		{
			get
			{
				foreach (TableMapping item in base.List)
				{
					if (item.Table.TableName == name)
					{
						return item;
					}
				}
				return null;
			}
		}

		public void Add(TableMapping map)
		{
			base.List.Add(map);
		}
	}
}
