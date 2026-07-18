using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Guid("496B0ABE-CDEE-11d3-88E8-00902754C43A")]
	public interface IEnumerable
	{
		[DispId(-4)]
		IEnumerator GetEnumerator();
	}
}
