using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[Serializable]
	[ComVisible(true)]
	public class SerializationException : SystemException
	{
		public SerializationException()
			: base("An error occurred during (de)serialization")
		{
		}

		public SerializationException(string message)
			: base(message)
		{
		}

		public SerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected SerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
