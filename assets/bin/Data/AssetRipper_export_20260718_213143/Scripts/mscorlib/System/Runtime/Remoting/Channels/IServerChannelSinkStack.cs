using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IServerChannelSinkStack : IServerResponseChannelSinkStack
	{
		object Pop(IServerChannelSink sink);

		void Push(IServerChannelSink sink, object state);

		void ServerCallback(IAsyncResult ar);

		void Store(IServerChannelSink sink, object state);

		void StoreAndDispatch(IServerChannelSink sink, object state);
	}
}
