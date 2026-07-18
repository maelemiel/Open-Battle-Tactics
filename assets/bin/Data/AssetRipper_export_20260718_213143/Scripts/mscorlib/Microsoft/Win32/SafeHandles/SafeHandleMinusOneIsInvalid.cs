using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	public abstract class SafeHandleMinusOneIsInvalid : SafeHandle, IDisposable
	{
		public override bool IsInvalid
		{
			get
			{
				return handle == (IntPtr)(-1);
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected SafeHandleMinusOneIsInvalid(bool ownsHandle)
			: base((IntPtr)0, ownsHandle)
		{
		}
	}
}
