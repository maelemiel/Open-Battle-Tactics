using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface ISerializationSurrogate
	{
		void GetObjectData(object obj, SerializationInfo info, StreamingContext context);

		object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector);
	}
}
