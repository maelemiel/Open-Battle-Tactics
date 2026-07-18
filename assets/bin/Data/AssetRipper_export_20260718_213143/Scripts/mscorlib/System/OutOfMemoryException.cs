using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class OutOfMemoryException : SystemException
	{
		private const int Result = -2147024882;

		public OutOfMemoryException()
			: base(Locale.GetText("Out of memory."))
		{
			base.HResult = -2147024882;
		}

		public OutOfMemoryException(string message)
			: base(message)
		{
			base.HResult = -2147024882;
		}

		public OutOfMemoryException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147024882;
		}

		protected OutOfMemoryException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
