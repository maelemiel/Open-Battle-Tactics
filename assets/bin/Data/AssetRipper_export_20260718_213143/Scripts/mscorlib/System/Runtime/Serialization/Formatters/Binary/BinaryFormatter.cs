using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[ComVisible(true)]
	public sealed class BinaryFormatter : IRemotingFormatter, IFormatter
	{
		private FormatterAssemblyStyle assembly_format;

		private SerializationBinder binder;

		private StreamingContext context;

		private ISurrogateSelector surrogate_selector;

		private FormatterTypeStyle type_format = FormatterTypeStyle.TypesAlways;

		private TypeFilterLevel filter_level = TypeFilterLevel.Full;

		public static ISurrogateSelector DefaultSurrogateSelector { get; set; }

		public FormatterAssemblyStyle AssemblyFormat
		{
			get
			{
				return assembly_format;
			}
			set
			{
				assembly_format = value;
			}
		}

		public SerializationBinder Binder
		{
			get
			{
				return binder;
			}
			set
			{
				binder = value;
			}
		}

		public StreamingContext Context
		{
			get
			{
				return context;
			}
			set
			{
				context = value;
			}
		}

		public ISurrogateSelector SurrogateSelector
		{
			get
			{
				return surrogate_selector;
			}
			set
			{
				surrogate_selector = value;
			}
		}

		public FormatterTypeStyle TypeFormat
		{
			get
			{
				return type_format;
			}
			set
			{
				type_format = value;
			}
		}

		public TypeFilterLevel FilterLevel
		{
			get
			{
				return filter_level;
			}
			set
			{
				filter_level = value;
			}
		}

		public BinaryFormatter()
		{
			surrogate_selector = DefaultSurrogateSelector;
			context = new StreamingContext(StreamingContextStates.All);
		}

		public BinaryFormatter(ISurrogateSelector selector, StreamingContext context)
		{
			surrogate_selector = selector;
			this.context = context;
		}

		public object Deserialize(Stream serializationStream)
		{
			return NoCheckDeserialize(serializationStream, null);
		}

		public object Deserialize(Stream serializationStream, HeaderHandler handler)
		{
			return NoCheckDeserialize(serializationStream, handler);
		}

		private object NoCheckDeserialize(Stream serializationStream, HeaderHandler handler)
		{
			if (serializationStream == null)
			{
				throw new ArgumentNullException("serializationStream");
			}
			if (serializationStream.CanSeek && serializationStream.Length == 0L)
			{
				throw new SerializationException("serializationStream supports seeking, but its length is 0");
			}
			BinaryReader binaryReader = new BinaryReader(serializationStream);
			bool hasHeaders;
			ReadBinaryHeader(binaryReader, out hasHeaders);
			BinaryElement binaryElement = (BinaryElement)binaryReader.Read();
			switch (binaryElement)
			{
			case BinaryElement.MethodCall:
				return MessageFormatter.ReadMethodCall(binaryElement, binaryReader, hasHeaders, handler, this);
			case BinaryElement.MethodResponse:
				return MessageFormatter.ReadMethodResponse(binaryElement, binaryReader, hasHeaders, handler, null, this);
			default:
			{
				ObjectReader objectReader = new ObjectReader(this);
				object result;
				Header[] headers;
				objectReader.ReadObjectGraph(binaryElement, binaryReader, hasHeaders, out result, out headers);
				if (handler != null)
				{
					handler(headers);
				}
				return result;
			}
			}
		}

		public object DeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage)
		{
			return NoCheckDeserializeMethodResponse(serializationStream, handler, methodCallMessage);
		}

		private object NoCheckDeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage)
		{
			if (serializationStream == null)
			{
				throw new ArgumentNullException("serializationStream");
			}
			if (serializationStream.CanSeek && serializationStream.Length == 0L)
			{
				throw new SerializationException("serializationStream supports seeking, but its length is 0");
			}
			BinaryReader reader = new BinaryReader(serializationStream);
			bool hasHeaders;
			ReadBinaryHeader(reader, out hasHeaders);
			return MessageFormatter.ReadMethodResponse(reader, hasHeaders, handler, methodCallMessage, this);
		}

		public void Serialize(Stream serializationStream, object graph)
		{
			Serialize(serializationStream, graph, null);
		}

		public void Serialize(Stream serializationStream, object graph, Header[] headers)
		{
			if (serializationStream == null)
			{
				throw new ArgumentNullException("serializationStream");
			}
			BinaryWriter binaryWriter = new BinaryWriter(serializationStream);
			WriteBinaryHeader(binaryWriter, headers != null);
			if (graph is IMethodCallMessage)
			{
				MessageFormatter.WriteMethodCall(binaryWriter, graph, headers, surrogate_selector, context, assembly_format, type_format);
			}
			else if (graph is IMethodReturnMessage)
			{
				MessageFormatter.WriteMethodResponse(binaryWriter, graph, headers, surrogate_selector, context, assembly_format, type_format);
			}
			else
			{
				ObjectWriter objectWriter = new ObjectWriter(surrogate_selector, context, assembly_format, type_format);
				objectWriter.WriteObjectGraph(binaryWriter, graph, headers);
			}
			binaryWriter.Flush();
		}

		[ComVisible(false)]
		public object UnsafeDeserialize(Stream serializationStream, HeaderHandler handler)
		{
			return NoCheckDeserialize(serializationStream, handler);
		}

		[ComVisible(false)]
		public object UnsafeDeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage)
		{
			return NoCheckDeserializeMethodResponse(serializationStream, handler, methodCallMessage);
		}

		private void WriteBinaryHeader(BinaryWriter writer, bool hasHeaders)
		{
			writer.Write((byte)0);
			writer.Write(1);
			if (hasHeaders)
			{
				writer.Write(2);
			}
			else
			{
				writer.Write(-1);
			}
			writer.Write(1);
			writer.Write(0);
		}

		private void ReadBinaryHeader(BinaryReader reader, out bool hasHeaders)
		{
			reader.ReadByte();
			reader.ReadInt32();
			int num = reader.ReadInt32();
			hasHeaders = num == 2;
			reader.ReadInt32();
			reader.ReadInt32();
		}
	}
}
