using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public class PathTooLongException : IOException
	{
		public PathTooLongException()
			: base(Locale.GetText("Pathname is longer than the maximum length"))
		{
		}

		public PathTooLongException(string message)
			: base(message)
		{
		}

		protected PathTooLongException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public PathTooLongException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
