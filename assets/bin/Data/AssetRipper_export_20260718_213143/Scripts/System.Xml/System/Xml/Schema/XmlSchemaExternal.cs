using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaExternal : XmlSchemaObject
	{
		private string id;

		private XmlSchema schema;

		private string location;

		private XmlAttribute[] unhandledAttributes;

		[XmlAttribute("schemaLocation", DataType = "anyURI")]
		public string SchemaLocation
		{
			get
			{
				return location;
			}
			set
			{
				location = value;
			}
		}

		[XmlIgnore]
		public XmlSchema Schema
		{
			get
			{
				return schema;
			}
			set
			{
				schema = value;
			}
		}

		[XmlAttribute("id", DataType = "ID")]
		public string Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] UnhandledAttributes
		{
			get
			{
				if (unhandledAttributeList != null)
				{
					unhandledAttributes = (XmlAttribute[])unhandledAttributeList.ToArray(typeof(XmlAttribute));
					unhandledAttributeList = null;
				}
				return unhandledAttributes;
			}
			set
			{
				unhandledAttributes = value;
				unhandledAttributeList = null;
			}
		}
	}
}
