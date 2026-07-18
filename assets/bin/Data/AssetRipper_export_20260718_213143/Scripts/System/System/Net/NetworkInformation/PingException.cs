using System.Runtime.Serialization;

namespace System.Net.NetworkInformation
{
	[Serializable]
	public class PingException : InvalidOperationException
	{
		public PingException(string message)
			: base(message)
		{
		}

		public PingException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected PingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
