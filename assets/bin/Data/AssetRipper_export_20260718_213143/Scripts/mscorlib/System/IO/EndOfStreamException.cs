using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public class EndOfStreamException : IOException
	{
		public EndOfStreamException()
			: base(Locale.GetText("Failed to read past end of stream."))
		{
		}

		public EndOfStreamException(string message)
			: base(message)
		{
		}

		protected EndOfStreamException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public EndOfStreamException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
