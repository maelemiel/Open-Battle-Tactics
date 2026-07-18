using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class DivideByZeroException : ArithmeticException
	{
		private const int Result = -2147352558;

		public DivideByZeroException()
			: base(Locale.GetText("Division by zero"))
		{
			base.HResult = -2147352558;
		}

		public DivideByZeroException(string message)
			: base(message)
		{
			base.HResult = -2147352558;
		}

		public DivideByZeroException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147352558;
		}

		protected DivideByZeroException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
