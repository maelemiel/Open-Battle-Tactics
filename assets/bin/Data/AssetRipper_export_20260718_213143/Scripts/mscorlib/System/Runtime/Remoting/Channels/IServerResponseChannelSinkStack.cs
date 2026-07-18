using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IServerResponseChannelSinkStack
	{
		void AsyncProcessResponse(IMessage msg, ITransportHeaders headers, Stream stream);

		Stream GetResponseStream(IMessage msg, ITransportHeaders headers);
	}
}
