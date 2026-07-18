using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientFormatterSink : IChannelSinkBase, IClientChannelSink, IMessageSink
	{
	}
}
