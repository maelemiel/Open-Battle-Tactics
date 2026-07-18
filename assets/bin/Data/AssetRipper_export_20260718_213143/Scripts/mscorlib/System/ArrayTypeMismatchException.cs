using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class ArrayTypeMismatchException : SystemException
	{
		private const int Result = -2146233085;

		public ArrayTypeMismatchException()
			: base(Locale.GetText("Source array type cannot be assigned to destination array type."))
		{
			base.HResult = -2146233085;
		}

		public ArrayTypeMismatchException(string message)
			: base(message)
		{
			base.HResult = -2146233085;
		}

		public ArrayTypeMismatchException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233085;
		}

		protected ArrayTypeMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
