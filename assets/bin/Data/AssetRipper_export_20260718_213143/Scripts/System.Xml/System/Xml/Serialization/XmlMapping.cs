using System.Collections;

namespace System.Xml.Serialization
{
	public abstract class XmlMapping
	{
		private ObjectMap map;

		private ArrayList relatedMaps;

		private SerializationFormat format;

		private SerializationSource source;

		internal string _elementName;

		internal string _namespace;

		private string key;

		[System.MonoTODO]
		public string XsdElementName
		{
			get
			{
				return _elementName;
			}
		}

		public string ElementName
		{
			get
			{
				return _elementName;
			}
		}

		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}

		internal ObjectMap ObjectMap
		{
			get
			{
				return map;
			}
			set
			{
				map = value;
			}
		}

		internal ArrayList RelatedMaps
		{
			get
			{
				return relatedMaps;
			}
			set
			{
				relatedMaps = value;
			}
		}

		internal SerializationFormat Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
			}
		}

		internal SerializationSource Source
		{
			get
			{
				return source;
			}
			set
			{
				source = value;
			}
		}

		internal XmlMapping()
		{
		}

		internal XmlMapping(string elementName, string ns)
		{
			_elementName = elementName;
			_namespace = ns;
		}

		public void SetKey(string key)
		{
			this.key = key;
		}

		internal string GetKey()
		{
			return key;
		}
	}
}
