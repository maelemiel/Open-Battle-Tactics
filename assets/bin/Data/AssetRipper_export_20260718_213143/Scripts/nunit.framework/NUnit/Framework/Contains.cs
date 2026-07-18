using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public class Contains
	{
		public static SubstringConstraint Substring(string substring)
		{
			return new SubstringConstraint(substring);
		}

		public static CollectionContainsConstraint Item(object item)
		{
			return new CollectionContainsConstraint(item);
		}
	}
}
