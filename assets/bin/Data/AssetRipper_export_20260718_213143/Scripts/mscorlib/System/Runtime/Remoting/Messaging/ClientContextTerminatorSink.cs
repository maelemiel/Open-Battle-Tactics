using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace System.Runtime.Remoting.Messaging
{
	internal class ClientContextTerminatorSink : IMessageSink
	{
		private Context _context;

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		public ClientContextTerminatorSink(Context ctx)
		{
			_context = ctx;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			IMessage message = null;
			Context.NotifyGlobalDynamicSinks(true, msg, true, false);
			_context.NotifyDynamicSinks(true, msg, true, false);
			if (msg is IConstructionCallMessage)
			{
				message = ActivationServices.RemoteActivate((IConstructionCallMessage)msg);
			}
			else
			{
				Identity messageTargetIdentity = RemotingServices.GetMessageTargetIdentity(msg);
				message = messageTargetIdentity.ChannelSink.SyncProcessMessage(msg);
			}
			Context.NotifyGlobalDynamicSinks(false, msg, true, false);
			_context.NotifyDynamicSinks(false, msg, true, false);
			return message;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			if (_context.HasDynamicSinks || Context.HasGlobalDynamicSinks)
			{
				Context.NotifyGlobalDynamicSinks(true, msg, true, true);
				_context.NotifyDynamicSinks(true, msg, true, true);
				if (replySink != null)
				{
					replySink = new ClientContextReplySink(_context, replySink);
				}
			}
			Identity messageTargetIdentity = RemotingServices.GetMessageTargetIdentity(msg);
			IMessageCtrl result = messageTargetIdentity.ChannelSink.AsyncProcessMessage(msg, replySink);
			if (replySink == null && (_context.HasDynamicSinks || Context.HasGlobalDynamicSinks))
			{
				Context.NotifyGlobalDynamicSinks(false, msg, true, true);
				_context.NotifyDynamicSinks(false, msg, true, true);
			}
			return result;
		}
	}
}
