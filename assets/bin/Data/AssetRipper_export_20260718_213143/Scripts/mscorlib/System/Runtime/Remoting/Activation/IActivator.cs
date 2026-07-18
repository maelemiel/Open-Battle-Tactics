using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Activation
{
	[ComVisible(true)]
	public interface IActivator
	{
		ActivatorLevel Level { get; }

		IActivator NextActivator { get; set; }

		IConstructionReturnMessage Activate(IConstructionCallMessage msg);
	}
}
