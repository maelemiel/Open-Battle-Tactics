using System.IO;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface IFormatter
	{
		SerializationBinder Binder { get; set; }

		StreamingContext Context { get; set; }

		ISurrogateSelector SurrogateSelector { get; set; }

		object Deserialize(Stream serializationStream);

		void Serialize(Stream serializationStream, object graph);
	}
}
