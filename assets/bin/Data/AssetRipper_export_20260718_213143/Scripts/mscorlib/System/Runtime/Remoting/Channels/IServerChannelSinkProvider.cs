using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IServerChannelSinkProvider
	{
		IServerChannelSinkProvider Next { get; set; }

		IServerChannelSink CreateSink(IChannelReceiver channel);

		void GetChannelData(IChannelDataStore channelData);
	}
}
