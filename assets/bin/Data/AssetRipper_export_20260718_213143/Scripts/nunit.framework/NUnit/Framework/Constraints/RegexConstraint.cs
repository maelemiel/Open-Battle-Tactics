using System.Text.RegularExpressions;

namespace NUnit.Framework.Constraints
{
	public class RegexConstraint : StringConstraint
	{
		public RegexConstraint(string pattern)
			: base(pattern)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return actual is string && Regex.IsMatch((string)actual, expected, caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String matching");
			writer.WriteExpectedValue(expected);
			if (caseInsensitive)
			{
				writer.WriteModifier("ignoring case");
			}
		}
	}
}
