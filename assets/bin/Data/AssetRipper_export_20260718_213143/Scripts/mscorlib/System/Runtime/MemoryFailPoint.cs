using System.Runtime.ConstrainedExecution;

namespace System.Runtime
{
	public sealed class MemoryFailPoint : CriticalFinalizerObject, IDisposable
	{
		[MonoTODO]
		public MemoryFailPoint(int sizeInMegabytes)
		{
			throw new NotImplementedException();
		}

		~MemoryFailPoint()
		{
		}

		[MonoTODO]
		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
