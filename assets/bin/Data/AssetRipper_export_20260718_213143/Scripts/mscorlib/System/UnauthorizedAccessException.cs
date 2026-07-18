using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class UnauthorizedAccessException : SystemException
	{
		private const int Result = -2146233088;

		public UnauthorizedAccessException()
			: base(Locale.GetText("Access to the requested resource is not authorized."))
		{
			base.HResult = -2146233088;
		}

		public UnauthorizedAccessException(string message)
			: base(message)
		{
			base.HResult = -2146233088;
		}

		public UnauthorizedAccessException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233088;
		}

		protected UnauthorizedAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
