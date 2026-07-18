using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class UniqueItemsConstraint : CollectionItemsEqualConstraint
	{
		protected override bool doMatch(IEnumerable actual)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object item in actual)
			{
				foreach (object item2 in arrayList)
				{
					if (ItemsEqual(item, item2))
					{
						return false;
					}
				}
				arrayList.Add(item);
			}
			return true;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("all items unique");
		}
	}
}
