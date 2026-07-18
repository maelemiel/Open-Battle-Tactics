using System.Runtime.ConstrainedExecution;

namespace System.Threading
{
	public class HostExecutionContextManager
	{
		[MonoTODO]
		public virtual HostExecutionContext Capture()
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MonoTODO]
		public virtual void Revert(object previousState)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual object SetHostExecutionContext(HostExecutionContext hostExecutionContext)
		{
			throw new NotImplementedException();
		}
	}
}
