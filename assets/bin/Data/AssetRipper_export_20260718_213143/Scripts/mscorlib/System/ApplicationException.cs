using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class ApplicationException : Exception
	{
		private const int Result = -2146232832;

		public ApplicationException()
			: base(Locale.GetText("An application exception has occurred."))
		{
			base.HResult = -2146232832;
		}

		public ApplicationException(string message)
			: base(message)
		{
			base.HResult = -2146232832;
		}

		public ApplicationException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146232832;
		}

		protected ApplicationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
