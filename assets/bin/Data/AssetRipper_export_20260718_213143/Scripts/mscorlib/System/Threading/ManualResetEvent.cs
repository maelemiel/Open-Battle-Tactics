using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public sealed class ManualResetEvent : EventWaitHandle
	{
		public ManualResetEvent(bool initialState)
			: base(initialState, EventResetMode.ManualReset)
		{
		}
	}
}
