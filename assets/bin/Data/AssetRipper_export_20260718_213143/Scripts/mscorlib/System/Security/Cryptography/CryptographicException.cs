using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security.Cryptography
{
	[Serializable]
	[ComVisible(true)]
	public class CryptographicException : SystemException, _Exception
	{
		public CryptographicException()
			: base(Locale.GetText("Error occured during a cryptographic operation."))
		{
			base.HResult = -2146233296;
		}

		public CryptographicException(int hr)
		{
			base.HResult = hr;
		}

		public CryptographicException(string message)
			: base(message)
		{
			base.HResult = -2146233296;
		}

		public CryptographicException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233296;
		}

		public CryptographicException(string format, string insert)
			: base(string.Format(format, insert))
		{
			base.HResult = -2146233296;
		}

		protected CryptographicException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
