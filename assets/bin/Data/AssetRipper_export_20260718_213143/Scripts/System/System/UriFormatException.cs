using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class UriFormatException : FormatException, ISerializable
	{
		public UriFormatException()
			: base(Locale.GetText("Invalid URI format"))
		{
		}

		public UriFormatException(string message)
			: base(message)
		{
		}

		public UriFormatException(string message, Exception exception)
			: base(message, exception)
		{
		}

		protected UriFormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
