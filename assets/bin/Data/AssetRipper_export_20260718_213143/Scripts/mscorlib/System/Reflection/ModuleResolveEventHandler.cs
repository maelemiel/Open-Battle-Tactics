using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public delegate Module ModuleResolveEventHandler(object sender, ResolveEventArgs e);
}
