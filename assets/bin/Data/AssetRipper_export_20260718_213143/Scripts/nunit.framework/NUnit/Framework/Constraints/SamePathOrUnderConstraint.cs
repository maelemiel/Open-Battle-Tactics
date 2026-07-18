namespace NUnit.Framework.Constraints
{
	public class SamePathOrUnderConstraint : PathConstraint
	{
		public SamePathOrUnderConstraint(string expected)
			: base(expected)
		{
		}

		protected override bool IsMatch(string expectedPath, string actualPath)
		{
			string path = PathConstraint.Canonicalize(expectedPath);
			string path2 = PathConstraint.Canonicalize(actualPath);
			return PathConstraint.IsSamePath(path, path2, caseInsensitive) || PathConstraint.IsSubPath(path, path2, caseInsensitive);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("Path under or matching");
			writer.WriteExpectedValue(expectedPath);
		}
	}
}
