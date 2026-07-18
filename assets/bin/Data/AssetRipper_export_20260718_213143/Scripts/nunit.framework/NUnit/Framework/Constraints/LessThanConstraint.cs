using System;

namespace NUnit.Framework.Constraints
{
	public class LessThanConstraint : ComparisonConstraint
	{
		private object expected;

		public LessThanConstraint(object expected)
			: base(expected)
		{
			this.expected = expected;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("less than");
			writer.WriteExpectedValue(expected);
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (expected == null || actual == null)
			{
				throw new ArgumentException("Cannot compare using a null reference");
			}
			return comparer.Compare(actual, expected) < 0;
		}
	}
}
