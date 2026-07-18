using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannelSinkBase
	{
		IDictionary Properties { get; }
	}
}
