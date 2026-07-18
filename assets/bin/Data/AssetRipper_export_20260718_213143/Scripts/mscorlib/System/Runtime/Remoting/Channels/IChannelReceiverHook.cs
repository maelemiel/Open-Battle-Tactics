using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannelReceiverHook
	{
		string ChannelScheme { get; }

		IServerChannelSink ChannelSinkChain { get; }

		bool WantsToListen { get; }

		void AddHookChannelUri(string channelUri);
	}
}
