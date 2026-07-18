using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientChannelSinkProvider
	{
		IClientChannelSinkProvider Next { get; set; }

		IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData);
	}
}
