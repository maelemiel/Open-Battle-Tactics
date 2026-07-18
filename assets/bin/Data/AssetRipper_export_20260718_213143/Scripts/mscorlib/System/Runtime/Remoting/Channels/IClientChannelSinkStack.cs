using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientChannelSinkStack : IClientResponseChannelSinkStack
	{
		object Pop(IClientChannelSink sink);

		void Push(IClientChannelSink sink, object state);
	}
}
