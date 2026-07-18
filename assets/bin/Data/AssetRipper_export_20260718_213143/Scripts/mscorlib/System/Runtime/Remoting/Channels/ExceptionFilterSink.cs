using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels
{
	internal class ExceptionFilterSink : IMessageSink
	{
		private IMessageSink _next;

		private IMessage _call;

		public IMessageSink NextSink
		{
			get
			{
				return _next;
			}
		}

		public ExceptionFilterSink(IMessage call, IMessageSink next)
		{
			_call = call;
			_next = next;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			return _next.SyncProcessMessage(ChannelServices.CheckReturnMessage(_call, msg));
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			throw new InvalidOperationException();
		}
	}
}
