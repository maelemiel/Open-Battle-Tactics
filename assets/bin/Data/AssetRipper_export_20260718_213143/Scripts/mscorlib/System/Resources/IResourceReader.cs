using System.Collections;
using System.Runtime.InteropServices;

namespace System.Resources
{
	[ComVisible(true)]
	public interface IResourceReader : IEnumerable, IDisposable
	{
		void Close();

		new IDictionaryEnumerator GetEnumerator();
	}
}
