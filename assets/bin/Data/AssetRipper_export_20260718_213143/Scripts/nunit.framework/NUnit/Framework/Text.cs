using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	[Obsolete("Use Is class for string constraints")]
	public class Text
	{
		[Obsolete("Use Is.All")]
		public static ConstraintExpression All
		{
			get
			{
				return new ConstraintExpression().All;
			}
		}

		[Obsolete("Use Is.StringContaining")]
		public static SubstringConstraint Contains(string expected)
		{
			return new SubstringConstraint(expected);
		}

		[Obsolete("Use Is.Not.StringContaining")]
		public static SubstringConstraint DoesNotContain(string expected)
		{
			return new ConstraintExpression().Not.ContainsSubstring(expected);
		}

		[Obsolete("Use Is.StringStarting")]
		public static StartsWithConstraint StartsWith(string expected)
		{
			return new StartsWithConstraint(expected);
		}

		public static StartsWithConstraint DoesNotStartWith(string expected)
		{
			return new ConstraintExpression().Not.StartsWith(expected);
		}

		[Obsolete("Use Is.StringEnding")]
		public static EndsWithConstraint EndsWith(string expected)
		{
			return new EndsWithConstraint(expected);
		}

		public static EndsWithConstraint DoesNotEndWith(string expected)
		{
			return new ConstraintExpression().Not.EndsWith(expected);
		}

		[Obsolete("Use Is.StringMatching")]
		public static RegexConstraint Matches(string pattern)
		{
			return new RegexConstraint(pattern);
		}

		[Obsolete]
		public static RegexConstraint DoesNotMatch(string pattern)
		{
			return new ConstraintExpression().Not.Matches(pattern);
		}
	}
}
