using System.Runtime.Remoting.Contexts;

namespace System.Runtime.Remoting.Messaging
{
	internal class ClientContextReplySink : IMessageSink
	{
		private IMessageSink _replySink;

		private Context _context;

		public IMessageSink NextSink
		{
			get
			{
				return _replySink;
			}
		}

		public ClientContextReplySink(Context ctx, IMessageSink replySink)
		{
			_replySink = replySink;
			_context = ctx;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			Context.NotifyGlobalDynamicSinks(false, msg, true, true);
			_context.NotifyDynamicSinks(false, msg, true, true);
			return _replySink.SyncProcessMessage(msg);
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			throw new NotSupportedException();
		}
	}
}
