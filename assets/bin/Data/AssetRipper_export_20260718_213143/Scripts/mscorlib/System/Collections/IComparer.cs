using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface IComparer
	{
		int Compare(object x, object y);
	}
}
