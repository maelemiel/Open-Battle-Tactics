namespace NUnit.Framework.Constraints
{
	public class SamePathConstraint : PathConstraint
	{
		public SamePathConstraint(string expected)
			: base(expected)
		{
		}

		protected override bool IsMatch(string expectedPath, string actualPath)
		{
			return PathConstraint.IsSamePath(PathConstraint.Canonicalize(expectedPath), PathConstraint.Canonicalize(actualPath), caseInsensitive);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("Path matching");
			writer.WriteExpectedValue(expectedPath);
		}
	}
}
