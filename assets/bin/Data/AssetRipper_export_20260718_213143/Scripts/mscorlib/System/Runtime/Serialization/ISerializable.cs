using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface ISerializable
	{
		void GetObjectData(SerializationInfo info, StreamingContext context);
	}
}
