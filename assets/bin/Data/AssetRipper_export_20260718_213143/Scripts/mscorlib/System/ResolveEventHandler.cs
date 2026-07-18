using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public delegate Assembly ResolveEventHandler(object sender, ResolveEventArgs args);
}
