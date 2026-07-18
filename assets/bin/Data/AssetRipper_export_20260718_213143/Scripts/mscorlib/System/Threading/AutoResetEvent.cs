using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public sealed class AutoResetEvent : EventWaitHandle
	{
		public AutoResetEvent(bool initialState)
			: base(initialState, EventResetMode.AutoReset)
		{
		}
	}
}
