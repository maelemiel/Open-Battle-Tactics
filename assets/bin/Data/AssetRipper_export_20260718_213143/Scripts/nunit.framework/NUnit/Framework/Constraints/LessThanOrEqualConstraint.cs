using System;

namespace NUnit.Framework.Constraints
{
	public class LessThanOrEqualConstraint : ComparisonConstraint
	{
		private object expected;

		public LessThanOrEqualConstraint(object expected)
			: base(expected)
		{
			this.expected = expected;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("less than or equal to");
			writer.WriteExpectedValue(expected);
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (expected == null || actual == null)
			{
				throw new ArgumentException("Cannot compare using a null reference");
			}
			return comparer.Compare(actual, expected) <= 0;
		}
	}
}
