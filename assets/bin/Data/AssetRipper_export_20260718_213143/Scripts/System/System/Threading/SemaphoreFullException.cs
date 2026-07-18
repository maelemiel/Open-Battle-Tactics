using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	[ComVisible(false)]
	public class SemaphoreFullException : SystemException
	{
		public SemaphoreFullException()
			: base(Locale.GetText("Exceeding maximum."))
		{
		}

		public SemaphoreFullException(string message)
			: base(message)
		{
		}

		public SemaphoreFullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected SemaphoreFullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
