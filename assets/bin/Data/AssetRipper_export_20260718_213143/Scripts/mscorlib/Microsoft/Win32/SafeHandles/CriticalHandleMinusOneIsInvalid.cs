using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	public abstract class CriticalHandleMinusOneIsInvalid : CriticalHandle, IDisposable
	{
		public override bool IsInvalid
		{
			get
			{
				return handle == (IntPtr)(-1);
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected CriticalHandleMinusOneIsInvalid()
			: base((IntPtr)(-1))
		{
		}
	}
}
