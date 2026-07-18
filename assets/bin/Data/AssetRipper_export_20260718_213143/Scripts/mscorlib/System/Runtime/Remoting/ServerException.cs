using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting
{
	[Serializable]
	[ComVisible(true)]
	public class ServerException : SystemException
	{
		public ServerException()
		{
		}

		public ServerException(string message)
			: base(message)
		{
		}

		public ServerException(string message, Exception InnerException)
			: base(message, InnerException)
		{
		}

		internal ServerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
