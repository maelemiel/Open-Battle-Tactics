using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public class ProtocolViolationException : InvalidOperationException, ISerializable
	{
		public ProtocolViolationException()
		{
		}

		public ProtocolViolationException(string message)
			: base(message)
		{
		}

		protected ProtocolViolationException(SerializationInfo info, StreamingContext context)
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
