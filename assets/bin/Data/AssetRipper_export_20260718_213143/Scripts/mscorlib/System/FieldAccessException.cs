using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class FieldAccessException : MemberAccessException
	{
		private const int Result = -2146233081;

		public FieldAccessException()
			: base(Locale.GetText("Attempt to access a private/protected field failed."))
		{
			base.HResult = -2146233081;
		}

		public FieldAccessException(string message)
			: base(message)
		{
			base.HResult = -2146233081;
		}

		protected FieldAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public FieldAccessException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233081;
		}
	}
}
