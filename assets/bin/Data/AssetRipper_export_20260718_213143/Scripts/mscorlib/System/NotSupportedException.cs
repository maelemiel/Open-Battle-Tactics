using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class NotSupportedException : SystemException
	{
		private const int Result = -2146233067;

		public NotSupportedException()
			: base(Locale.GetText("Operation is not supported."))
		{
			base.HResult = -2146233067;
		}

		public NotSupportedException(string message)
			: base(message)
		{
			base.HResult = -2146233067;
		}

		public NotSupportedException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233067;
		}

		protected NotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
