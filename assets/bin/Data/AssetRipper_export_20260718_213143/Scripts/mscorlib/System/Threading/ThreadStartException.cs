using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	public sealed class ThreadStartException : SystemException
	{
		internal ThreadStartException()
			: base("Thread Start Error")
		{
		}

		internal ThreadStartException(string message)
			: base(message)
		{
		}

		internal ThreadStartException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal ThreadStartException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
