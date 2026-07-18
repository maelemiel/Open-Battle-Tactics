using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface ITransportHeaders
	{
		object this[object key] { get; set; }

		IEnumerator GetEnumerator();
	}
}
