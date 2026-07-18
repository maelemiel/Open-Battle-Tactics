namespace System.Runtime.Remoting.Channels
{
	internal class ChanelSinkStackEntry
	{
		public IChannelSinkBase Sink;

		public object State;

		public ChanelSinkStackEntry Next;

		public ChanelSinkStackEntry(IChannelSinkBase sink, object state, ChanelSinkStackEntry next)
		{
			Sink = sink;
			State = state;
			Next = next;
		}
	}
}
