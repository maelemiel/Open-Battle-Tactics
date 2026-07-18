using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class OverflowException : ArithmeticException
	{
		private const int Result = -2146233066;

		public OverflowException()
			: base(Locale.GetText("Number overflow."))
		{
			base.HResult = -2146233066;
		}

		public OverflowException(string message)
			: base(message)
		{
			base.HResult = -2146233066;
		}

		public OverflowException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233066;
		}

		protected OverflowException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
