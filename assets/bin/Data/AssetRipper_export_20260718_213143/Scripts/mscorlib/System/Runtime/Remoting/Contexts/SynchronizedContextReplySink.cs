using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Contexts
{
	internal class SynchronizedContextReplySink : IMessageSink
	{
		private IMessageSink _next;

		private bool _newLock;

		private SynchronizationAttribute _att;

		public IMessageSink NextSink
		{
			get
			{
				return _next;
			}
		}

		public SynchronizedContextReplySink(IMessageSink next, SynchronizationAttribute att, bool newLock)
		{
			_newLock = newLock;
			_next = next;
			_att = att;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			throw new NotSupportedException();
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			if (_newLock)
			{
				_att.AcquireLock();
			}
			else
			{
				_att.ReleaseLock();
			}
			try
			{
				return _next.SyncProcessMessage(msg);
			}
			finally
			{
				if (_newLock)
				{
					_att.ReleaseLock();
				}
			}
		}
	}
}
