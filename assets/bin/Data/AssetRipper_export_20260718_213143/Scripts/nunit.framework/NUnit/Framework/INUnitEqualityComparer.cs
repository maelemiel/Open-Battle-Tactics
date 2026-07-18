using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	public interface INUnitEqualityComparer
	{
		bool AreEqual(object x, object y, ref Tolerance tolerance);
	}
	public interface INUnitEqualityComparer<T>
	{
		bool AreEqual(T x, T y, ref Tolerance tolerance);
	}
}
