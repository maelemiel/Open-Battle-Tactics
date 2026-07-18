namespace System.Runtime.Remoting.Messaging
{
	internal class ServerObjectTerminatorSink : IMessageSink
	{
		private IMessageSink _nextSink;

		public IMessageSink NextSink
		{
			get
			{
				return _nextSink;
			}
		}

		public ServerObjectTerminatorSink(IMessageSink nextSink)
		{
			_nextSink = nextSink;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			ServerIdentity serverIdentity = (ServerIdentity)RemotingServices.GetMessageTargetIdentity(msg);
			serverIdentity.NotifyServerDynamicSinks(true, msg, false, false);
			IMessage result = _nextSink.SyncProcessMessage(msg);
			serverIdentity.NotifyServerDynamicSinks(false, msg, false, false);
			return result;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			ServerIdentity serverIdentity = (ServerIdentity)RemotingServices.GetMessageTargetIdentity(msg);
			if (serverIdentity.HasServerDynamicSinks)
			{
				serverIdentity.NotifyServerDynamicSinks(true, msg, false, true);
				if (replySink != null)
				{
					replySink = new ServerObjectReplySink(serverIdentity, replySink);
				}
			}
			IMessageCtrl result = _nextSink.AsyncProcessMessage(msg, replySink);
			if (replySink == null)
			{
				serverIdentity.NotifyServerDynamicSinks(false, msg, true, true);
			}
			return result;
		}
	}
}
