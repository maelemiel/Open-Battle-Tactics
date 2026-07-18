using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Lifetime
{
	internal class LeaseSink : IMessageSink
	{
		private IMessageSink _nextSink;

		public IMessageSink NextSink
		{
			get
			{
				return _nextSink;
			}
		}

		public LeaseSink(IMessageSink nextSink)
		{
			_nextSink = nextSink;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			RenewLease(msg);
			return _nextSink.SyncProcessMessage(msg);
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			RenewLease(msg);
			return _nextSink.AsyncProcessMessage(msg, replySink);
		}

		private void RenewLease(IMessage msg)
		{
			ServerIdentity serverIdentity = (ServerIdentity)RemotingServices.GetMessageTargetIdentity(msg);
			ILease lease = serverIdentity.Lease;
			if (lease != null && lease.CurrentLeaseTime < lease.RenewOnCallTime)
			{
				lease.Renew(lease.RenewOnCallTime);
			}
		}
	}
}
