using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannelReceiver : IChannel
	{
		object ChannelData { get; }

		string[] GetUrlsForUri(string objectURI);

		void StartListening(object data);

		void StopListening(object data);
	}
}
