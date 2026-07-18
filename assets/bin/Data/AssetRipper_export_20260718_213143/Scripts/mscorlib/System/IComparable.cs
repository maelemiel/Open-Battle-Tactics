using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public interface IComparable
	{
		int CompareTo(object obj);
	}
	public interface IComparable<T>
	{
		int CompareTo(T other);
	}
}
