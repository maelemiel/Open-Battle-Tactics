using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Contexts
{
	internal class SynchronizedClientContextSink : IMessageSink
	{
		private IMessageSink _next;

		private SynchronizationAttribute _att;

		public IMessageSink NextSink
		{
			get
			{
				return _next;
			}
		}

		public SynchronizedClientContextSink(IMessageSink next, SynchronizationAttribute att)
		{
			_att = att;
			_next = next;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			if (_att.IsReEntrant)
			{
				_att.ReleaseLock();
				replySink = new SynchronizedContextReplySink(replySink, _att, true);
			}
			return _next.AsyncProcessMessage(msg, replySink);
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			if (_att.IsReEntrant)
			{
				_att.ReleaseLock();
			}
			try
			{
				return _next.SyncProcessMessage(msg);
			}
			finally
			{
				if (_att.IsReEntrant)
				{
					_att.AcquireLock();
				}
			}
		}
	}
}
