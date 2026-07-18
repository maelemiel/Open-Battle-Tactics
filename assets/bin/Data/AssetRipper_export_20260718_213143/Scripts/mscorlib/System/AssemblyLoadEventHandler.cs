using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public delegate void AssemblyLoadEventHandler(object sender, AssemblyLoadEventArgs args);
}
