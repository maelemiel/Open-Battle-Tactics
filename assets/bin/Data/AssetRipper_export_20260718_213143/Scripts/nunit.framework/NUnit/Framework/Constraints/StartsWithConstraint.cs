namespace NUnit.Framework.Constraints
{
	public class StartsWithConstraint : StringConstraint
	{
		public StartsWithConstraint(string expected)
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
				return ((string)actual).ToLower().StartsWith(expected.ToLower());
			}
			return ((string)actual).StartsWith(expected);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String starting with");
			writer.WriteExpectedValue(MsgUtils.ClipString(expected, writer.MaxLineLength - 40, 0));
			if (caseInsensitive)
			{
				writer.WriteModifier("ignoring case");
			}
		}
	}
}
