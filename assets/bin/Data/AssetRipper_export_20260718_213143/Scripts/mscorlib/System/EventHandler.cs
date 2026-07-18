using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;
	[Serializable]
	[ComVisible(true)]
	public delegate void EventHandler(object sender, EventArgs e);
}
