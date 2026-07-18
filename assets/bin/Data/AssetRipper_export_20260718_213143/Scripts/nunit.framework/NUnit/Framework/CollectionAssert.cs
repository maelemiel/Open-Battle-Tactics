using System;
using System.Collections;
using System.ComponentModel;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public class CollectionAssert
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

		public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType)
		{
			AllItemsAreInstancesOfType(collection, expectedType, string.Empty, null);
		}

		public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType, string message)
		{
			AllItemsAreInstancesOfType(collection, expectedType, message, null);
		}

		public static void AllItemsAreInstancesOfType(IEnumerable collection, Type expectedType, string message, params object[] args)
		{
			Assert.That(collection, new AllItemsConstraint(new InstanceOfTypeConstraint(expectedType)), message, args);
		}

		public static void AllItemsAreNotNull(IEnumerable collection)
		{
			AllItemsAreNotNull(collection, string.Empty, null);
		}

		public static void AllItemsAreNotNull(IEnumerable collection, string message)
		{
			AllItemsAreNotNull(collection, message, null);
		}

		public static void AllItemsAreNotNull(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, new AllItemsConstraint(new NotConstraint(new EqualConstraint(null))), message, args);
		}

		public static void AllItemsAreUnique(IEnumerable collection)
		{
			AllItemsAreUnique(collection, string.Empty, null);
		}

		public static void AllItemsAreUnique(IEnumerable collection, string message)
		{
			AllItemsAreUnique(collection, message, null);
		}

		public static void AllItemsAreUnique(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, new UniqueItemsConstraint(), message, args);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual)
		{
			Assert.That(actual, new EqualConstraint(expected));
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual, IComparer comparer)
		{
			AreEqual(expected, actual, comparer, string.Empty, null);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual, string message)
		{
			Assert.That(actual, new EqualConstraint(expected), message);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, string message)
		{
			AreEqual(expected, actual, comparer, message, null);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual, string message, params object[] args)
		{
			Assert.That(actual, new EqualConstraint(expected), message, args);
		}

		public static void AreEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, string message, params object[] args)
		{
			Assert.That(actual, new EqualConstraint(expected).Using(comparer), message, args);
		}

		public static void AreEquivalent(IEnumerable expected, IEnumerable actual)
		{
			AreEquivalent(expected, actual, string.Empty, null);
		}

		public static void AreEquivalent(IEnumerable expected, IEnumerable actual, string message)
		{
			AreEquivalent(expected, actual, message, null);
		}

		public static void AreEquivalent(IEnumerable expected, IEnumerable actual, string message, params object[] args)
		{
			Assert.That(actual, new CollectionEquivalentConstraint(expected), message, args);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual)
		{
			Assert.That(actual, new NotConstraint(new EqualConstraint(expected)));
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer)
		{
			AreNotEqual(expected, actual, comparer, string.Empty, null);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual, string message)
		{
			Assert.That(actual, new NotConstraint(new EqualConstraint(expected)), message);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, string message)
		{
			AreNotEqual(expected, actual, comparer, message, null);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new EqualConstraint(expected)), message, args);
		}

		public static void AreNotEqual(IEnumerable expected, IEnumerable actual, IComparer comparer, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new EqualConstraint(expected).Using(comparer)), message, args);
		}

		public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual)
		{
			AreNotEquivalent(expected, actual, string.Empty, null);
		}

		public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual, string message)
		{
			AreNotEquivalent(expected, actual, message, null);
		}

		public static void AreNotEquivalent(IEnumerable expected, IEnumerable actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new CollectionEquivalentConstraint(expected)), message, args);
		}

		public static void Contains(IEnumerable collection, object actual)
		{
			Contains(collection, actual, string.Empty, null);
		}

		public static void Contains(IEnumerable collection, object actual, string message)
		{
			Contains(collection, actual, message, null);
		}

		public static void Contains(IEnumerable collection, object actual, string message, params object[] args)
		{
			Assert.That(collection, new CollectionContainsConstraint(actual), message, args);
		}

		public static void DoesNotContain(IEnumerable collection, object actual)
		{
			DoesNotContain(collection, actual, string.Empty, null);
		}

		public static void DoesNotContain(IEnumerable collection, object actual, string message)
		{
			DoesNotContain(collection, actual, message, null);
		}

		public static void DoesNotContain(IEnumerable collection, object actual, string message, params object[] args)
		{
			Assert.That(collection, new NotConstraint(new CollectionContainsConstraint(actual)), message, args);
		}

		public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset)
		{
			IsNotSubsetOf(subset, superset, string.Empty, null);
		}

		public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset, string message)
		{
			IsNotSubsetOf(subset, superset, message, null);
		}

		public static void IsNotSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object[] args)
		{
			Assert.That(subset, new NotConstraint(new CollectionSubsetConstraint(superset)), message, args);
		}

		public static void IsSubsetOf(IEnumerable subset, IEnumerable superset)
		{
			IsSubsetOf(subset, superset, string.Empty, null);
		}

		public static void IsSubsetOf(IEnumerable subset, IEnumerable superset, string message)
		{
			IsSubsetOf(subset, superset, message, null);
		}

		public static void IsSubsetOf(IEnumerable subset, IEnumerable superset, string message, params object[] args)
		{
			Assert.That(subset, new CollectionSubsetConstraint(superset), message, args);
		}

		public static void IsEmpty(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, new EmptyConstraint(), message, args);
		}

		public static void IsEmpty(IEnumerable collection, string message)
		{
			IsEmpty(collection, message, null);
		}

		public static void IsEmpty(IEnumerable collection)
		{
			IsEmpty(collection, string.Empty, null);
		}

		public static void IsNotEmpty(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, new NotConstraint(new EmptyConstraint()), message, args);
		}

		public static void IsNotEmpty(IEnumerable collection, string message)
		{
			IsNotEmpty(collection, message, null);
		}

		public static void IsNotEmpty(IEnumerable collection)
		{
			IsNotEmpty(collection, string.Empty, null);
		}

		public static void IsOrdered(IEnumerable collection, string message, params object[] args)
		{
			Assert.That(collection, new CollectionOrderedConstraint(), message, args);
		}

		public static void IsOrdered(IEnumerable collection, string message)
		{
			IsOrdered(collection, message, null);
		}

		public static void IsOrdered(IEnumerable collection)
		{
			IsOrdered(collection, string.Empty, null);
		}

		public static void IsOrdered(IEnumerable collection, IComparer comparer, string message, params object[] args)
		{
			Assert.That(collection, new CollectionOrderedConstraint().Using(comparer), message, args);
		}

		public static void IsOrdered(IEnumerable collection, IComparer comparer, string message)
		{
			IsOrdered(collection, comparer, message, null);
		}

		public static void IsOrdered(IEnumerable collection, IComparer comparer)
		{
			IsOrdered(collection, comparer, string.Empty, null);
		}
	}
}
