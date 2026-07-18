using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientChannelSink : IChannelSinkBase
	{
		IClientChannelSink NextChannelSink { get; }

		void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream);

		void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream);

		Stream GetRequestStream(IMessage msg, ITransportHeaders headers);

		void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream);
	}
}
