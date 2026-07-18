using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security.Cryptography
{
	[Serializable]
	[ComVisible(true)]
	public class CryptographicUnexpectedOperationException : CryptographicException
	{
		public CryptographicUnexpectedOperationException()
			: base(Locale.GetText("Unexpected error occured during a cryptographic operation."))
		{
			base.HResult = -2146233295;
		}

		public CryptographicUnexpectedOperationException(string message)
			: base(message)
		{
			base.HResult = -2146233295;
		}

		public CryptographicUnexpectedOperationException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233295;
		}

		public CryptographicUnexpectedOperationException(string format, string insert)
			: base(string.Format(format, insert))
		{
			base.HResult = -2146233295;
		}

		protected CryptographicUnexpectedOperationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
