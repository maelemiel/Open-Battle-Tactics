using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class OperationCanceledException : SystemException
	{
		private const int Result = -2146233029;

		public OperationCanceledException()
			: base(Locale.GetText("The operation was canceled."))
		{
			base.HResult = -2146233029;
		}

		public OperationCanceledException(string message)
			: base(message)
		{
			base.HResult = -2146233029;
		}

		public OperationCanceledException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233029;
		}

		protected OperationCanceledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
