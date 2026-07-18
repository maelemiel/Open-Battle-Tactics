using System.Collections;

namespace System.Data
{
	internal class RelationStructureCollection : CollectionBase
	{
		public RelationStructure this[int i]
		{
			get
			{
				return base.List[i] as RelationStructure;
			}
		}

		public RelationStructure this[string parent, string child]
		{
			get
			{
				foreach (RelationStructure item in base.List)
				{
					if (item.ParentTableName == parent && item.ChildTableName == child)
					{
						return item;
					}
				}
				return null;
			}
		}

		public void Add(RelationStructure rel)
		{
			base.List.Add(rel);
		}
	}
}
