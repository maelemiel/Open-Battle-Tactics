using System.Collections;
using System.Security.Policy;

namespace System.Xml.Serialization
{
	public class XmlSerializerFactory
	{
		private static Hashtable serializersBySource = new Hashtable();

		public XmlSerializer CreateSerializer(Type type)
		{
			return CreateSerializer(type, null, null, null, null);
		}

		public XmlSerializer CreateSerializer(XmlTypeMapping xmlTypeMapping)
		{
			lock (serializersBySource)
			{
				XmlSerializer xmlSerializer = (XmlSerializer)serializersBySource[xmlTypeMapping.Source];
				if (xmlSerializer == null)
				{
					xmlSerializer = new XmlSerializer(xmlTypeMapping);
					serializersBySource[xmlTypeMapping.Source] = xmlSerializer;
				}
				return xmlSerializer;
			}
		}

		public XmlSerializer CreateSerializer(Type type, string defaultNamespace)
		{
			return CreateSerializer(type, null, null, null, defaultNamespace);
		}

		public XmlSerializer CreateSerializer(Type type, Type[] extraTypes)
		{
			return CreateSerializer(type, null, extraTypes, null, null);
		}

		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides)
		{
			return CreateSerializer(type, overrides, null, null, null);
		}

		public XmlSerializer CreateSerializer(Type type, XmlRootAttribute root)
		{
			return CreateSerializer(type, null, null, root, null);
		}

		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
		{
			XmlTypeSerializationSource key = new XmlTypeSerializationSource(type, root, overrides, defaultNamespace, extraTypes);
			lock (serializersBySource)
			{
				XmlSerializer xmlSerializer = (XmlSerializer)serializersBySource[key];
				if (xmlSerializer == null)
				{
					xmlSerializer = new XmlSerializer(type, overrides, extraTypes, root, defaultNamespace);
					serializersBySource[xmlSerializer.Mapping.Source] = xmlSerializer;
				}
				return xmlSerializer;
			}
		}

		[System.MonoTODO]
		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location, Evidence evidence)
		{
			throw new NotImplementedException();
		}
	}
}
