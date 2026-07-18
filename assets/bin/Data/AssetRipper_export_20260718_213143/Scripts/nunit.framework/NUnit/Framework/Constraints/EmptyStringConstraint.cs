namespace NUnit.Framework.Constraints
{
	public class EmptyStringConstraint : Constraint
	{
		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (!(actual is string))
			{
				return false;
			}
			return (string)actual == string.Empty;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("<empty>");
		}
	}
}
