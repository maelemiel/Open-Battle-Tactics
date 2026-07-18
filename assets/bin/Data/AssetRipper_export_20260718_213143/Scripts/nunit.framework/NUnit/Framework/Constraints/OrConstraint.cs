namespace NUnit.Framework.Constraints
{
	public class OrConstraint : BinaryConstraint
	{
		public OrConstraint(Constraint left, Constraint right)
			: base(left, right)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return left.Matches(actual) || right.Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			left.WriteDescriptionTo(writer);
			writer.WriteConnector("or");
			right.WriteDescriptionTo(writer);
		}
	}
}
