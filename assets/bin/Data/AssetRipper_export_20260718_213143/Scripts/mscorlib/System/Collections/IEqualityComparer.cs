using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface IEqualityComparer
	{
		new bool Equals(object x, object y);

		int GetHashCode(object obj);
	}
}
