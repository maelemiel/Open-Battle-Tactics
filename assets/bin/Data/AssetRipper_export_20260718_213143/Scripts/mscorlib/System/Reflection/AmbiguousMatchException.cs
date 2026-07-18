using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public sealed class AmbiguousMatchException : SystemException
	{
		public AmbiguousMatchException()
			: base("Ambiguous matching in method resolution")
		{
		}

		public AmbiguousMatchException(string message)
			: base(message)
		{
		}

		public AmbiguousMatchException(string message, Exception inner)
			: base(message, inner)
		{
		}

		internal AmbiguousMatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
