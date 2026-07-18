using System;

namespace NUnit.Framework.Constraints
{
	public class RangeConstraint<T> : ComparisonConstraint where T : IComparable<T>
	{
		private T from;

		private T to;

		public RangeConstraint(T from, T to)
			: base(from, to)
		{
			this.from = from;
			this.to = to;
			comparer = ComparisonAdapter.For(new NUnitComparer<T>());
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (from == null || to == null || actual == null)
			{
				throw new ArgumentException("Cannot compare using a null reference", "expected");
			}
			return comparer.Compare(from, actual) <= 0 && comparer.Compare(to, actual) >= 0;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("in range ({0},{1})", from, to);
		}
	}
}
