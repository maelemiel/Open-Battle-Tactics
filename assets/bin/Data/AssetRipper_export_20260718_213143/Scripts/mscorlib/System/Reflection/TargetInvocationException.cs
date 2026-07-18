using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public sealed class TargetInvocationException : Exception
	{
		public TargetInvocationException(Exception inner)
			: base("Exception has been thrown by the target of an invocation.", inner)
		{
		}

		public TargetInvocationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		internal TargetInvocationException(SerializationInfo info, StreamingContext sc)
			: base(info, sc)
		{
		}
	}
}
