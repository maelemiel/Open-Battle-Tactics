using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public class CookieException : FormatException, ISerializable
	{
		public CookieException()
		{
		}

		internal CookieException(string msg)
			: base(msg)
		{
		}

		internal CookieException(string msg, Exception e)
			: base(msg, e)
		{
		}

		protected CookieException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData(serializationInfo, streamingContext);
		}
	}
}
