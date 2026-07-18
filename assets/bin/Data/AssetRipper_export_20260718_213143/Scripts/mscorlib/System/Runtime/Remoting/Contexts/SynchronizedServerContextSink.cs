using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Contexts
{
	internal class SynchronizedServerContextSink : IMessageSink
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

		public SynchronizedServerContextSink(IMessageSink next, SynchronizationAttribute att)
		{
			_att = att;
			_next = next;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			_att.AcquireLock();
			replySink = new SynchronizedContextReplySink(replySink, _att, false);
			return _next.AsyncProcessMessage(msg, replySink);
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			_att.AcquireLock();
			try
			{
				return _next.SyncProcessMessage(msg);
			}
			finally
			{
				_att.ReleaseLock();
			}
		}
	}
}
