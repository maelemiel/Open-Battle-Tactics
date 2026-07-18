namespace NUnit.Framework.Constraints
{
	public class EndsWithConstraint : StringConstraint
	{
		public EndsWithConstraint(string expected)
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
				return ((string)actual).ToLower().EndsWith(expected.ToLower());
			}
			return ((string)actual).EndsWith(expected);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String ending with");
			writer.WriteExpectedValue(expected);
			if (caseInsensitive)
			{
				writer.WriteModifier("ignoring case");
			}
		}
	}
}
