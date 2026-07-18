namespace System.Runtime.Remoting.Messaging
{
	internal class ServerObjectReplySink : IMessageSink
	{
		private IMessageSink _replySink;

		private ServerIdentity _identity;

		public IMessageSink NextSink
		{
			get
			{
				return _replySink;
			}
		}

		public ServerObjectReplySink(ServerIdentity identity, IMessageSink replySink)
		{
			_replySink = replySink;
			_identity = identity;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			_identity.NotifyServerDynamicSinks(false, msg, true, true);
			return _replySink.SyncProcessMessage(msg);
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			throw new NotSupportedException();
		}
	}
}
