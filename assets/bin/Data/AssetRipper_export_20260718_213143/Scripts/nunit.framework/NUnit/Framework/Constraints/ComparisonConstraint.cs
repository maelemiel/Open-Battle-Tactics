using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public abstract class ComparisonConstraint : Constraint
	{
		protected ComparisonAdapter comparer = ComparisonAdapter.Default;

		public ComparisonConstraint(object arg)
			: base(arg)
		{
		}

		public ComparisonConstraint(object arg1, object arg2)
			: base(arg1, arg2)
		{
		}

		public ComparisonConstraint Using(IComparer comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public ComparisonConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public ComparisonConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}
	}
}
