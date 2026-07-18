using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	[ComVisible(true)]
	public class ThreadStateException : SystemException
	{
		public ThreadStateException()
			: base("Thread State Error")
		{
		}

		public ThreadStateException(string message)
			: base(message)
		{
		}

		protected ThreadStateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public ThreadStateException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
