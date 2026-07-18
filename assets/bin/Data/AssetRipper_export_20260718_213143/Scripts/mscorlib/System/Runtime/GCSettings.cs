using System.Runtime.ConstrainedExecution;

namespace System.Runtime
{
	public static class GCSettings
	{
		[MonoTODO("Always returns false")]
		public static bool IsServerGC
		{
			get
			{
				return false;
			}
		}

		[MonoTODO("Always returns GCLatencyMode.Interactive and ignores set (.NET 2.0 SP1 member)")]
		public static GCLatencyMode LatencyMode
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return GCLatencyMode.Interactive;
			}
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			set
			{
			}
		}
	}
}
