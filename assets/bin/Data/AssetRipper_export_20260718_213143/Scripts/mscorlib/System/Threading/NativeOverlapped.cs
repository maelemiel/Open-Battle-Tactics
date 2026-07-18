using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public struct NativeOverlapped
	{
		public IntPtr EventHandle;

		public IntPtr InternalHigh;

		public IntPtr InternalLow;

		public int OffsetHigh;

		public int OffsetLow;

		internal int Handle1;

		internal int Handle2;
	}
}
