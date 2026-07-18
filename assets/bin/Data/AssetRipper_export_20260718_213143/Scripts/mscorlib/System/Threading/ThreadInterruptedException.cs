using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	[ComVisible(true)]
	public class ThreadInterruptedException : SystemException
	{
		public ThreadInterruptedException()
			: base("Thread interrupted")
		{
		}

		public ThreadInterruptedException(string message)
			: base(message)
		{
		}

		protected ThreadInterruptedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ThreadInterruptedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
