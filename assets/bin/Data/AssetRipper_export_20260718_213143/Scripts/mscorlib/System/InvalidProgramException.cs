using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class InvalidProgramException : SystemException
	{
		public InvalidProgramException()
			: base(Locale.GetText("Metadata is invalid."))
		{
		}

		public InvalidProgramException(string message)
			: base(message)
		{
		}

		public InvalidProgramException(string message, Exception inner)
			: base(message, inner)
		{
		}

		internal InvalidProgramException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
