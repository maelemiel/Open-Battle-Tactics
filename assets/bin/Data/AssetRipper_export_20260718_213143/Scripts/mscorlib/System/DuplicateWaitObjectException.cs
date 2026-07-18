using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class DuplicateWaitObjectException : ArgumentException
	{
		private const int Result = -2146233047;

		public DuplicateWaitObjectException()
			: base(Locale.GetText("Duplicate objects in argument."))
		{
			base.HResult = -2146233047;
		}

		public DuplicateWaitObjectException(string parameterName)
			: base(Locale.GetText("Duplicate objects in argument."), parameterName)
		{
			base.HResult = -2146233047;
		}

		public DuplicateWaitObjectException(string parameterName, string message)
			: base(message, parameterName)
		{
			base.HResult = -2146233047;
		}

		public DuplicateWaitObjectException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233047;
		}

		protected DuplicateWaitObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
