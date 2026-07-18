using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public delegate object ServiceCreatorCallback(IServiceContainer container, Type serviceType);
}
