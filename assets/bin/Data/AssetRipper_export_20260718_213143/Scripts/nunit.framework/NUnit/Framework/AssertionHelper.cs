using System.Collections;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public class AssertionHelper : ConstraintFactory
	{
		public void Expect(object actual, IResolveConstraint constraint)
		{
			Assert.That(actual, constraint, null, null);
		}

		public void Expect(object actual, IResolveConstraint constraint, string message)
		{
			Assert.That(actual, constraint, message, null);
		}

		public void Expect(object actual, IResolveConstraint constraint, string message, params object[] args)
		{
			Assert.That(actual, constraint, message, args);
		}

		public void Expect(ActualValueDelegate del, IResolveConstraint expr)
		{
			Assert.That(del, expr.Resolve(), null, null);
		}

		public void Expect(ActualValueDelegate del, IResolveConstraint expr, string message)
		{
			Assert.That(del, expr.Resolve(), message, null);
		}

		public void Expect(ActualValueDelegate del, IResolveConstraint expr, string message, params object[] args)
		{
			Assert.That(del, expr, message, args);
		}

		public void Expect<T>(ref T actual, IResolveConstraint constraint)
		{
			Assert.That(ref actual, constraint.Resolve(), null, null);
		}

		public void Expect<T>(ref T actual, IResolveConstraint constraint, string message)
		{
			Assert.That(ref actual, constraint.Resolve(), message, null);
		}

		public void Expect<T>(ref T actual, IResolveConstraint expression, string message, params object[] args)
		{
			Assert.That(ref actual, expression, message, args);
		}

		public void Expect(bool condition, string message, params object[] args)
		{
			Assert.That(condition, Is.True, message, args);
		}

		public void Expect(bool condition, string message)
		{
			Assert.That(condition, Is.True, message, null);
		}

		public void Expect(bool condition)
		{
			Assert.That(condition, Is.True, null, null);
		}

		public void Expect(TestDelegate code, IResolveConstraint constraint)
		{
			Assert.That((object)code, constraint);
		}

		public ListMapper Map(ICollection original)
		{
			return new ListMapper(original);
		}
	}
}
