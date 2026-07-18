using System.Collections;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace System.Xml.Serialization
{
	public class XmlSerializer
	{
		internal class SerializerData
		{
			public int UsageCount;

			public Type ReaderType;

			public MethodInfo ReaderMethod;

			public Type WriterType;

			public MethodInfo WriterMethod;

			public GenerationBatch Batch;

			public XmlSerializerImplementation Implementation;

			public XmlSerializationReader CreateReader()
			{
				if (ReaderType != null)
				{
					return (XmlSerializationReader)Activator.CreateInstance(ReaderType);
				}
				if (Implementation != null)
				{
					return Implementation.Reader;
				}
				return null;
			}

			public XmlSerializationWriter CreateWriter()
			{
				if (WriterType != null)
				{
					return (XmlSerializationWriter)Activator.CreateInstance(WriterType);
				}
				if (Implementation != null)
				{
					return Implementation.Writer;
				}
				return null;
			}
		}

		internal class GenerationBatch
		{
			public bool Done;

			public XmlMapping[] Maps;

			public SerializerData[] Datas;
		}

		internal const string WsdlNamespace = "http://schemas.xmlsoap.org/wsdl/";

		internal const string EncodingNamespace = "http://schemas.xmlsoap.org/soap/encoding/";

		internal const string WsdlTypesNamespace = "http://microsoft.com/wsdl/types/";

		private static int generationThreshold;

		private static bool backgroundGeneration;

		private static bool deleteTempFiles;

		private static bool generatorFallback;

		private bool customSerializer;

		private XmlMapping typeMapping;

		private SerializerData serializerData;

		private static Hashtable serializerTypes;

		private XmlAttributeEventHandler onUnknownAttribute;

		private XmlElementEventHandler onUnknownElement;

		private XmlNodeEventHandler onUnknownNode;

		private UnreferencedObjectEventHandler onUnreferencedObject;

		internal XmlMapping Mapping
		{
			get
			{
				return typeMapping;
			}
		}

		public event XmlAttributeEventHandler UnknownAttribute
		{
			add
			{
				onUnknownAttribute = (XmlAttributeEventHandler)Delegate.Combine(onUnknownAttribute, value);
			}
			remove
			{
				onUnknownAttribute = (XmlAttributeEventHandler)Delegate.Remove(onUnknownAttribute, value);
			}
		}

		public event XmlElementEventHandler UnknownElement
		{
			add
			{
				onUnknownElement = (XmlElementEventHandler)Delegate.Combine(onUnknownElement, value);
			}
			remove
			{
				onUnknownElement = (XmlElementEventHandler)Delegate.Remove(onUnknownElement, value);
			}
		}

		public event XmlNodeEventHandler UnknownNode
		{
			add
			{
				onUnknownNode = (XmlNodeEventHandler)Delegate.Combine(onUnknownNode, value);
			}
			remove
			{
				onUnknownNode = (XmlNodeEventHandler)Delegate.Remove(onUnknownNode, value);
			}
		}

		public event UnreferencedObjectEventHandler UnreferencedObject
		{
			add
			{
				onUnreferencedObject = (UnreferencedObjectEventHandler)Delegate.Combine(onUnreferencedObject, value);
			}
			remove
			{
				onUnreferencedObject = (UnreferencedObjectEventHandler)Delegate.Remove(onUnreferencedObject, value);
			}
		}

		protected XmlSerializer()
		{
			customSerializer = true;
		}

		public XmlSerializer(Type type)
			: this(type, null, null, null, null)
		{
		}

		public XmlSerializer(XmlTypeMapping xmlTypeMapping)
		{
			typeMapping = xmlTypeMapping;
		}

		internal XmlSerializer(XmlMapping mapping, SerializerData data)
		{
			typeMapping = mapping;
			serializerData = data;
		}

		public XmlSerializer(Type type, string defaultNamespace)
			: this(type, null, null, null, defaultNamespace)
		{
		}

		public XmlSerializer(Type type, Type[] extraTypes)
			: this(type, null, extraTypes, null, null)
		{
		}

		public XmlSerializer(Type type, XmlAttributeOverrides overrides)
			: this(type, overrides, null, null, null)
		{
		}

		public XmlSerializer(Type type, XmlRootAttribute root)
			: this(type, null, null, root, null)
		{
		}

		public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			XmlReflectionImporter xmlReflectionImporter = new XmlReflectionImporter(overrides, defaultNamespace);
			if (extraTypes != null)
			{
				foreach (Type type2 in extraTypes)
				{
					xmlReflectionImporter.IncludeType(type2);
				}
			}
			typeMapping = xmlReflectionImporter.ImportTypeMapping(type, root, defaultNamespace);
		}

		[System.MonoTODO]
		public XmlSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location, Evidence evidence)
		{
		}

		static XmlSerializer()
		{
			backgroundGeneration = true;
			deleteTempFiles = true;
			generatorFallback = true;
			serializerTypes = new Hashtable();
			string text = null;
			string text2 = null;
			generationThreshold = -1;
			backgroundGeneration = false;
			deleteTempFiles = text == null || text == "no";
		}

		internal virtual void OnUnknownAttribute(XmlAttributeEventArgs e)
		{
			if (onUnknownAttribute != null)
			{
				onUnknownAttribute(this, e);
			}
		}

		internal virtual void OnUnknownElement(XmlElementEventArgs e)
		{
			if (onUnknownElement != null)
			{
				onUnknownElement(this, e);
			}
		}

		internal virtual void OnUnknownNode(XmlNodeEventArgs e)
		{
			if (onUnknownNode != null)
			{
				onUnknownNode(this, e);
			}
		}

		internal virtual void OnUnreferencedObject(UnreferencedObjectEventArgs e)
		{
			if (onUnreferencedObject != null)
			{
				onUnreferencedObject(this, e);
			}
		}

		public virtual bool CanDeserialize(XmlReader xmlReader)
		{
			xmlReader.MoveToContent();
			if (typeMapping is XmlMembersMapping)
			{
				return true;
			}
			return ((XmlTypeMapping)typeMapping).ElementName == xmlReader.LocalName;
		}

		protected virtual XmlSerializationReader CreateReader()
		{
			throw new NotImplementedException();
		}

		protected virtual XmlSerializationWriter CreateWriter()
		{
			throw new NotImplementedException();
		}

		public object Deserialize(Stream stream)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(stream);
			xmlTextReader.Normalization = true;
			xmlTextReader.WhitespaceHandling = WhitespaceHandling.Significant;
			return Deserialize(xmlTextReader);
		}

		public object Deserialize(TextReader textReader)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(textReader);
			xmlTextReader.Normalization = true;
			xmlTextReader.WhitespaceHandling = WhitespaceHandling.Significant;
			return Deserialize(xmlTextReader);
		}

		public object Deserialize(XmlReader xmlReader)
		{
			XmlSerializationReader xmlSerializationReader = ((!customSerializer) ? CreateReader(typeMapping) : CreateReader());
			xmlSerializationReader.Initialize(xmlReader, this);
			return Deserialize(xmlSerializationReader);
		}

		protected virtual object Deserialize(XmlSerializationReader reader)
		{
			if (customSerializer)
			{
				throw new NotImplementedException();
			}
			try
			{
				if (reader is XmlSerializationReaderInterpreter)
				{
					return ((XmlSerializationReaderInterpreter)reader).ReadRoot();
				}
				return serializerData.ReaderMethod.Invoke(reader, null);
			}
			catch (Exception ex)
			{
				if (ex is InvalidOperationException || ex is InvalidCastException)
				{
					throw new InvalidOperationException("There is an error in XML document.", ex);
				}
				throw;
			}
		}

		public static XmlSerializer[] FromMappings(XmlMapping[] mappings)
		{
			XmlSerializer[] array = new XmlSerializer[mappings.Length];
			SerializerData[] array2 = new SerializerData[mappings.Length];
			GenerationBatch generationBatch = new GenerationBatch();
			generationBatch.Maps = mappings;
			generationBatch.Datas = array2;
			for (int i = 0; i < mappings.Length; i++)
			{
				if (mappings[i] != null)
				{
					SerializerData serializerData = new SerializerData();
					serializerData.Batch = generationBatch;
					array[i] = new XmlSerializer(mappings[i], serializerData);
					array2[i] = serializerData;
				}
			}
			return array;
		}

		public static XmlSerializer[] FromTypes(Type[] mappings)
		{
			XmlSerializer[] array = new XmlSerializer[mappings.Length];
			for (int i = 0; i < mappings.Length; i++)
			{
				array[i] = new XmlSerializer(mappings[i]);
			}
			return array;
		}

		protected virtual void Serialize(object o, XmlSerializationWriter writer)
		{
			if (customSerializer)
			{
				throw new NotImplementedException();
			}
			if (writer is XmlSerializationWriterInterpreter)
			{
				((XmlSerializationWriterInterpreter)writer).WriteRoot(o);
				return;
			}
			serializerData.WriterMethod.Invoke(writer, new object[1] { o });
		}

		public void Serialize(Stream stream, object o)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.Default);
			xmlTextWriter.Formatting = Formatting.Indented;
			Serialize(xmlTextWriter, o, null);
		}

		public void Serialize(TextWriter textWriter, object o)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(textWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			Serialize(xmlTextWriter, o, null);
		}

		public void Serialize(XmlWriter xmlWriter, object o)
		{
			Serialize(xmlWriter, o, null);
		}

		public void Serialize(Stream stream, object o, XmlSerializerNamespaces namespaces)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.Default);
			xmlTextWriter.Formatting = Formatting.Indented;
			Serialize(xmlTextWriter, o, namespaces);
		}

		public void Serialize(TextWriter textWriter, object o, XmlSerializerNamespaces namespaces)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(textWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			Serialize(xmlTextWriter, o, namespaces);
			xmlTextWriter.Flush();
		}

		public void Serialize(XmlWriter writer, object o, XmlSerializerNamespaces namespaces)
		{
			try
			{
				XmlSerializationWriter xmlSerializationWriter = ((!customSerializer) ? CreateWriter(typeMapping) : CreateWriter());
				if (namespaces == null || namespaces.Count == 0)
				{
					namespaces = new XmlSerializerNamespaces();
					namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
					namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
				}
				xmlSerializationWriter.Initialize(writer, namespaces);
				Serialize(o, xmlSerializationWriter);
				writer.Flush();
			}
			catch (Exception innerException)
			{
				if (innerException is TargetInvocationException)
				{
					innerException = innerException.InnerException;
				}
				if (innerException is InvalidOperationException || innerException is InvalidCastException)
				{
					throw new InvalidOperationException("There was an error generating the XML document.", innerException);
				}
				throw;
			}
		}

		[System.MonoTODO]
		public object Deserialize(XmlReader xmlReader, string encodingStyle, XmlDeserializationEvents events)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public object Deserialize(XmlReader xmlReader, string encodingStyle)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public object Deserialize(XmlReader xmlReader, XmlDeserializationEvents events)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static XmlSerializer[] FromMappings(XmlMapping[] mappings, Evidence evidence)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static XmlSerializer[] FromMappings(XmlMapping[] mappings, Type type)
		{
			throw new NotImplementedException();
		}

		public static string GetXmlSerializerAssemblyName(Type type)
		{
			return type.Assembly.GetName().Name + ".XmlSerializers";
		}

		public static string GetXmlSerializerAssemblyName(Type type, string defaultNamespace)
		{
			return GetXmlSerializerAssemblyName(type) + "." + defaultNamespace.GetHashCode();
		}

		[System.MonoTODO]
		public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle)
		{
			throw new NotImplementedException();
		}

		[System.MonoNotSupported("")]
		public void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
		{
			throw new NotImplementedException();
		}

		private XmlSerializationWriter CreateWriter(XmlMapping typeMapping)
		{
			XmlSerializationWriter xmlSerializationWriter;
			lock (this)
			{
				if (serializerData != null)
				{
					lock (serializerData)
					{
						xmlSerializationWriter = serializerData.CreateWriter();
					}
					if (xmlSerializationWriter != null)
					{
						return xmlSerializationWriter;
					}
				}
			}
			if (!typeMapping.Source.CanBeGenerated || generationThreshold == -1)
			{
				return new XmlSerializationWriterInterpreter(typeMapping);
			}
			CheckGeneratedTypes(typeMapping);
			lock (this)
			{
				lock (serializerData)
				{
					xmlSerializationWriter = serializerData.CreateWriter();
				}
				if (xmlSerializationWriter != null)
				{
					return xmlSerializationWriter;
				}
				if (!generatorFallback)
				{
					throw new InvalidOperationException("Error while generating serializer");
				}
			}
			return new XmlSerializationWriterInterpreter(typeMapping);
		}

		private XmlSerializationReader CreateReader(XmlMapping typeMapping)
		{
			XmlSerializationReader xmlSerializationReader;
			lock (this)
			{
				if (serializerData != null)
				{
					lock (serializerData)
					{
						xmlSerializationReader = serializerData.CreateReader();
					}
					if (xmlSerializationReader != null)
					{
						return xmlSerializationReader;
					}
				}
			}
			if (!typeMapping.Source.CanBeGenerated || generationThreshold == -1)
			{
				return new XmlSerializationReaderInterpreter(typeMapping);
			}
			CheckGeneratedTypes(typeMapping);
			lock (this)
			{
				lock (serializerData)
				{
					xmlSerializationReader = serializerData.CreateReader();
				}
				if (xmlSerializationReader != null)
				{
					return xmlSerializationReader;
				}
				if (!generatorFallback)
				{
					throw new InvalidOperationException("Error while generating serializer");
				}
			}
			return new XmlSerializationReaderInterpreter(typeMapping);
		}

		private void CheckGeneratedTypes(XmlMapping typeMapping)
		{
			throw new NotImplementedException();
		}

		private void GenerateSerializersAsync(GenerationBatch batch)
		{
			throw new NotImplementedException();
		}

		private void RunSerializerGeneration(object obj)
		{
			throw new NotImplementedException();
		}

		private GenerationBatch LoadFromSatelliteAssembly(GenerationBatch batch)
		{
			return batch;
		}
	}
}
