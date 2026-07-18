using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	[ComVisible(true)]
	public class SynchronizationLockException : SystemException
	{
		public SynchronizationLockException()
			: base("Synchronization Error")
		{
		}

		public SynchronizationLockException(string message)
			: base(message)
		{
		}

		protected SynchronizationLockException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public SynchronizationLockException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
