using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class TimeoutException : SystemException
	{
		private const int Result = -2146233083;

		public TimeoutException()
			: base(Locale.GetText("The operation has timed-out."))
		{
			base.HResult = -2146233083;
		}

		public TimeoutException(string message)
			: base(message)
		{
			base.HResult = -2146233083;
		}

		public TimeoutException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233083;
		}

		protected TimeoutException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
