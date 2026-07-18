using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public delegate void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e);
}
