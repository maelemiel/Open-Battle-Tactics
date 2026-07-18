using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security
{
	[Serializable]
	[ComVisible(true)]
	public class VerificationException : SystemException
	{
		public VerificationException()
		{
		}

		public VerificationException(string message)
			: base(message)
		{
		}

		protected VerificationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public VerificationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
