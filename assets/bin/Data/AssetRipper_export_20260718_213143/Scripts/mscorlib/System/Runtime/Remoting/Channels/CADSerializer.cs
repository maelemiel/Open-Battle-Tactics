using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Runtime.Remoting.Channels
{
	internal class CADSerializer
	{
		internal static IMessage DeserializeMessage(MemoryStream mem, IMethodCallMessage msg)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.SurrogateSelector = null;
			mem.Position = 0L;
			if (msg == null)
			{
				return (IMessage)binaryFormatter.Deserialize(mem, null);
			}
			return (IMessage)binaryFormatter.DeserializeMethodResponse(mem, null, msg);
		}

		internal static MemoryStream SerializeMessage(IMessage msg)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.SurrogateSelector = new RemotingSurrogateSelector();
			binaryFormatter.Serialize(memoryStream, msg);
			memoryStream.Position = 0L;
			return memoryStream;
		}

		internal static MemoryStream SerializeObject(object obj)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.SurrogateSelector = new RemotingSurrogateSelector();
			binaryFormatter.Serialize(memoryStream, obj);
			memoryStream.Position = 0L;
			return memoryStream;
		}

		internal static object DeserializeObject(MemoryStream mem)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.SurrogateSelector = null;
			mem.Position = 0L;
			return binaryFormatter.Deserialize(mem);
		}
	}
}
