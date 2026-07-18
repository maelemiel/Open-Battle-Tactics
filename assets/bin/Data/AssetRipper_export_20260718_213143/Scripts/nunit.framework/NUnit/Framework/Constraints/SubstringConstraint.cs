namespace NUnit.Framework.Constraints
{
	public class SubstringConstraint : StringConstraint
	{
		public SubstringConstraint(string expected)
			: base(expected)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (!(actual is string))
			{
				return false;
			}
			if (caseInsensitive)
			{
				return ((string)actual).ToLower().IndexOf(expected.ToLower()) >= 0;
			}
			return ((string)actual).IndexOf(expected) >= 0;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String containing");
			writer.WriteExpectedValue(expected);
			if (caseInsensitive)
			{
				writer.WriteModifier("ignoring case");
			}
		}
	}
}
