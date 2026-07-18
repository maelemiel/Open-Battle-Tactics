using System.ComponentModel;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public class StringAssert
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new AssertionException("Assert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new AssertionException("Assert.ReferenceEquals should not be used for Assertions");
		}

		public static void Contains(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new SubstringConstraint(expected), message, args);
		}

		public static void Contains(string expected, string actual, string message)
		{
			Contains(expected, actual, message, null);
		}

		public static void Contains(string expected, string actual)
		{
			Contains(expected, actual, string.Empty, null);
		}

		public static void DoesNotContain(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new SubstringConstraint(expected)), message, args);
		}

		public static void DoesNotContain(string expected, string actual, string message)
		{
			DoesNotContain(expected, actual, message, null);
		}

		public static void DoesNotContain(string expected, string actual)
		{
			DoesNotContain(expected, actual, string.Empty, null);
		}

		public static void StartsWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new StartsWithConstraint(expected), message, args);
		}

		public static void StartsWith(string expected, string actual, string message)
		{
			StartsWith(expected, actual, message, null);
		}

		public static void StartsWith(string expected, string actual)
		{
			StartsWith(expected, actual, string.Empty, null);
		}

		public static void DoesNotStartWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new StartsWithConstraint(expected)), message, args);
		}

		public static void DoesNotStartWith(string expected, string actual, string message)
		{
			DoesNotStartWith(expected, actual, message, null);
		}

		public static void DoesNotStartWith(string expected, string actual)
		{
			DoesNotStartWith(expected, actual, string.Empty, null);
		}

		public static void EndsWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new EndsWithConstraint(expected), message, args);
		}

		public static void EndsWith(string expected, string actual, string message)
		{
			EndsWith(expected, actual, message, null);
		}

		public static void EndsWith(string expected, string actual)
		{
			EndsWith(expected, actual, string.Empty, null);
		}

		public static void DoesNotEndWith(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new EndsWithConstraint(expected)), message, args);
		}

		public static void DoesNotEndWith(string expected, string actual, string message)
		{
			DoesNotEndWith(expected, actual, message, null);
		}

		public static void DoesNotEndWith(string expected, string actual)
		{
			DoesNotEndWith(expected, actual, string.Empty, null);
		}

		public static void AreEqualIgnoringCase(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new EqualConstraint(expected).IgnoreCase, message, args);
		}

		public static void AreEqualIgnoringCase(string expected, string actual, string message)
		{
			AreEqualIgnoringCase(expected, actual, message, null);
		}

		public static void AreEqualIgnoringCase(string expected, string actual)
		{
			AreEqualIgnoringCase(expected, actual, string.Empty, null);
		}

		public static void AreNotEqualIgnoringCase(string expected, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new EqualConstraint(expected).IgnoreCase), message, args);
		}

		public static void AreNotEqualIgnoringCase(string expected, string actual, string message)
		{
			AreNotEqualIgnoringCase(expected, actual, message, null);
		}

		public static void AreNotEqualIgnoringCase(string expected, string actual)
		{
			AreNotEqualIgnoringCase(expected, actual, string.Empty, null);
		}

		public static void IsMatch(string pattern, string actual, string message, params object[] args)
		{
			Assert.That(actual, new RegexConstraint(pattern), message, args);
		}

		public static void IsMatch(string pattern, string actual, string message)
		{
			IsMatch(pattern, actual, message, null);
		}

		public static void IsMatch(string pattern, string actual)
		{
			IsMatch(pattern, actual, string.Empty, null);
		}

		public static void DoesNotMatch(string pattern, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new RegexConstraint(pattern)), message, args);
		}

		public static void DoesNotMatch(string pattern, string actual, string message)
		{
			DoesNotMatch(pattern, actual, message, null);
		}

		public static void DoesNotMatch(string pattern, string actual)
		{
			DoesNotMatch(pattern, actual, string.Empty, null);
		}
	}
}
