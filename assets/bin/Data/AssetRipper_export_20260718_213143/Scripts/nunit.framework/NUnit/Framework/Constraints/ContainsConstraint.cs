using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public class ContainsConstraint : Constraint
	{
		private object expected;

		private Constraint realConstraint;

		private bool ignoreCase;

		private EqualityAdapter adapter = null;

		private Constraint RealConstraint
		{
			get
			{
				if (realConstraint == null)
				{
					if (actual is string)
					{
						StringConstraint stringConstraint = new SubstringConstraint((string)expected);
						if (ignoreCase)
						{
							stringConstraint = stringConstraint.IgnoreCase;
						}
						realConstraint = stringConstraint;
					}
					else
					{
						CollectionContainsConstraint collectionContainsConstraint = new CollectionContainsConstraint(expected);
						if (adapter != null)
						{
							collectionContainsConstraint.comparer.ExternalComparers.Add(adapter);
						}
						realConstraint = collectionContainsConstraint;
					}
				}
				return realConstraint;
			}
			set
			{
				realConstraint = value;
			}
		}

		public ContainsConstraint IgnoreCase
		{
			get
			{
				ignoreCase = true;
				return this;
			}
		}

		public ContainsConstraint(object expected)
			: base(expected)
		{
			this.expected = expected;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return RealConstraint.Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			RealConstraint.WriteDescriptionTo(writer);
		}

		public ContainsConstraint Using(IComparer comparer)
		{
			adapter = EqualityAdapter.For(comparer);
			return this;
		}

		public ContainsConstraint Using<T>(IComparer<T> comparer)
		{
			adapter = EqualityAdapter.For(comparer);
			return this;
		}

		public ContainsConstraint Using<T>(Comparison<T> comparer)
		{
			adapter = EqualityAdapter.For(comparer);
			return this;
		}

		public ContainsConstraint Using(IEqualityComparer comparer)
		{
			adapter = EqualityAdapter.For(comparer);
			return this;
		}

		public ContainsConstraint Using<T>(IEqualityComparer<T> comparer)
		{
			adapter = EqualityAdapter.For(comparer);
			return this;
		}
	}
}
