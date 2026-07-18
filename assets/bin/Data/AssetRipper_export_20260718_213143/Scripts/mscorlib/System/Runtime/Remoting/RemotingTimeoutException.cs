using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting
{
	[Serializable]
	[ComVisible(true)]
	public class RemotingTimeoutException : RemotingException
	{
		public RemotingTimeoutException()
		{
		}

		public RemotingTimeoutException(string message)
			: base(message)
		{
		}

		public RemotingTimeoutException(string message, Exception InnerException)
			: base(message, InnerException)
		{
		}

		internal RemotingTimeoutException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
