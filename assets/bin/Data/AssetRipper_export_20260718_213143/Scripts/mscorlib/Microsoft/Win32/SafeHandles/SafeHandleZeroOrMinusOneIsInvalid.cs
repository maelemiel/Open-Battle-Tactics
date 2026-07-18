using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	public abstract class SafeHandleZeroOrMinusOneIsInvalid : SafeHandle, IDisposable
	{
		public override bool IsInvalid
		{
			get
			{
				return handle == (IntPtr)(-1) || handle == (IntPtr)0;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected SafeHandleZeroOrMinusOneIsInvalid(bool ownsHandle)
			: base((IntPtr)0, ownsHandle)
		{
		}
	}
}
