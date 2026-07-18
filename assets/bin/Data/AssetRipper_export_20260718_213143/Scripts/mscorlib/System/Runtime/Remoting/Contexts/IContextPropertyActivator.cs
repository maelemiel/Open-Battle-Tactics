using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public interface IContextPropertyActivator
	{
		void CollectFromClientContext(IConstructionCallMessage msg);

		void CollectFromServerContext(IConstructionReturnMessage msg);

		bool DeliverClientContextToServerContext(IConstructionCallMessage msg);

		bool DeliverServerContextToClientContext(IConstructionReturnMessage msg);

		bool IsOKToActivate(IConstructionCallMessage msg);
	}
}
