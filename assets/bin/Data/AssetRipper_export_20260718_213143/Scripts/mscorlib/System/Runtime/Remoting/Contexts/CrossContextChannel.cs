using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Contexts
{
	internal class CrossContextChannel : IMessageSink
	{
		private class ContextRestoreSink : IMessageSink
		{
			private IMessageSink _next;

			private Context _context;

			private IMessage _call;

			public IMessageSink NextSink
			{
				get
				{
					return _next;
				}
			}

			public ContextRestoreSink(IMessageSink next, Context context, IMessage call)
			{
				_next = next;
				_context = context;
				_call = call;
			}

			public IMessage SyncProcessMessage(IMessage msg)
			{
				try
				{
					Context.NotifyGlobalDynamicSinks(false, msg, false, false);
					Thread.CurrentContext.NotifyDynamicSinks(false, msg, false, false);
					return _next.SyncProcessMessage(msg);
				}
				catch (Exception e)
				{
					return new ReturnMessage(e, (IMethodCallMessage)_call);
				}
				finally
				{
					if (_context != null)
					{
						Context.SwitchToContext(_context);
					}
				}
			}

			public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
			{
				throw new NotSupportedException();
			}
		}

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			ServerIdentity serverIdentity = (ServerIdentity)RemotingServices.GetMessageTargetIdentity(msg);
			Context context = null;
			if (Thread.CurrentContext != serverIdentity.Context)
			{
				context = Context.SwitchToContext(serverIdentity.Context);
			}
			IMessage result;
			try
			{
				Context.NotifyGlobalDynamicSinks(true, msg, false, false);
				Thread.CurrentContext.NotifyDynamicSinks(true, msg, false, false);
				result = serverIdentity.Context.GetServerContextSinkChain().SyncProcessMessage(msg);
				Context.NotifyGlobalDynamicSinks(false, msg, false, false);
				Thread.CurrentContext.NotifyDynamicSinks(false, msg, false, false);
			}
			catch (Exception e)
			{
				result = new ReturnMessage(e, (IMethodCallMessage)msg);
			}
			finally
			{
				if (context != null)
				{
					Context.SwitchToContext(context);
				}
			}
			return result;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			ServerIdentity serverIdentity = (ServerIdentity)RemotingServices.GetMessageTargetIdentity(msg);
			Context context = null;
			if (Thread.CurrentContext != serverIdentity.Context)
			{
				context = Context.SwitchToContext(serverIdentity.Context);
			}
			try
			{
				Context.NotifyGlobalDynamicSinks(true, msg, false, true);
				Thread.CurrentContext.NotifyDynamicSinks(true, msg, false, false);
				if (replySink != null)
				{
					replySink = new ContextRestoreSink(replySink, context, msg);
				}
				IMessageCtrl result = serverIdentity.AsyncObjectProcessMessage(msg, replySink);
				if (replySink == null)
				{
					Context.NotifyGlobalDynamicSinks(false, msg, false, false);
					Thread.CurrentContext.NotifyDynamicSinks(false, msg, false, false);
				}
				return result;
			}
			catch (Exception e)
			{
				if (replySink != null)
				{
					replySink.SyncProcessMessage(new ReturnMessage(e, (IMethodCallMessage)msg));
				}
				return null;
			}
			finally
			{
				if (context != null)
				{
					Context.SwitchToContext(context);
				}
			}
		}
	}
}
