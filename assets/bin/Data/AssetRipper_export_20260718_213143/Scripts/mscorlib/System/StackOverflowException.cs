using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class StackOverflowException : SystemException
	{
		public StackOverflowException()
			: base(Locale.GetText("The requested operation caused a stack overflow."))
		{
		}

		public StackOverflowException(string message)
			: base(message)
		{
		}

		public StackOverflowException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal StackOverflowException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
