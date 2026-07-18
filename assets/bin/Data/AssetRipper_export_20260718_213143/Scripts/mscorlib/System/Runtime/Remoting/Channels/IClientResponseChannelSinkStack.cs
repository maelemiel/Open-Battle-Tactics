using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientResponseChannelSinkStack
	{
		void AsyncProcessResponse(ITransportHeaders headers, Stream stream);

		void DispatchException(Exception e);

		void DispatchReplyMessage(IMessage msg);
	}
}
