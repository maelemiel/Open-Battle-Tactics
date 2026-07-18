using System.Runtime.InteropServices;

namespace System.Runtime.ConstrainedExecution
{
	[ComVisible(true)]
	public abstract class CriticalFinalizerObject
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected CriticalFinalizerObject()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		~CriticalFinalizerObject()
		{
		}
	}
}
