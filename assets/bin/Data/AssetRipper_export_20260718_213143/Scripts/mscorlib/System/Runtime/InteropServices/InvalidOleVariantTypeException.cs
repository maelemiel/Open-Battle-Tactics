using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public class InvalidOleVariantTypeException : SystemException
	{
		private const int ErrorCode = -2146233039;

		public InvalidOleVariantTypeException()
			: base(Locale.GetText("Found native variant type cannot be marshalled to managed code"))
		{
			base.HResult = -2146233039;
		}

		public InvalidOleVariantTypeException(string message)
			: base(message)
		{
			base.HResult = -2146233039;
		}

		public InvalidOleVariantTypeException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233039;
		}

		protected InvalidOleVariantTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
