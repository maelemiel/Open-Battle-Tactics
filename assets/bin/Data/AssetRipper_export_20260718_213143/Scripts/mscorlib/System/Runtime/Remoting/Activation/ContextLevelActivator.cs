using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Activation
{
	[Serializable]
	internal class ContextLevelActivator : IActivator
	{
		private IActivator m_NextActivator;

		public ActivatorLevel Level
		{
			get
			{
				return ActivatorLevel.Context;
			}
		}

		public IActivator NextActivator
		{
			get
			{
				return m_NextActivator;
			}
			set
			{
				m_NextActivator = value;
			}
		}

		public ContextLevelActivator(IActivator next)
		{
			m_NextActivator = next;
		}

		public IConstructionReturnMessage Activate(IConstructionCallMessage ctorCall)
		{
			ServerIdentity serverIdentity = RemotingServices.CreateContextBoundObjectIdentity(ctorCall.ActivationType);
			RemotingServices.SetMessageTargetIdentity(ctorCall, serverIdentity);
			ConstructionCall constructionCall = ctorCall as ConstructionCall;
			if (constructionCall == null || !constructionCall.IsContextOk)
			{
				serverIdentity.Context = Context.CreateNewContext(ctorCall);
				Context newContext = Context.SwitchToContext(serverIdentity.Context);
				try
				{
					return m_NextActivator.Activate(ctorCall);
				}
				finally
				{
					Context.SwitchToContext(newContext);
				}
			}
			return m_NextActivator.Activate(ctorCall);
		}
	}
}
