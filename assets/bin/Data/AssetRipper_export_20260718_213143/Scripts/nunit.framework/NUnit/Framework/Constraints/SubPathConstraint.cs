using System;

namespace NUnit.Framework.Constraints
{
	public class SubPathConstraint : PathConstraint
	{
		public SubPathConstraint(string expected)
			: base(expected)
		{
		}

		protected override bool IsMatch(string expectedPath, string actualPath)
		{
			if (actualPath == null)
			{
				throw new ArgumentException("The actual value may not be null", "actual");
			}
			return PathConstraint.IsSubPath(PathConstraint.Canonicalize(expectedPath), PathConstraint.Canonicalize(actualPath), caseInsensitive);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("Path under");
			writer.WriteExpectedValue(expectedPath);
		}
	}
}
