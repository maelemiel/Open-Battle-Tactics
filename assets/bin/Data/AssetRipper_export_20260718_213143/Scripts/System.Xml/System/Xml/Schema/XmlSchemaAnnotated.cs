using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAnnotated : XmlSchemaObject
	{
		private XmlSchemaAnnotation annotation;

		private string id;

		private XmlAttribute[] unhandledAttributes;

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

		[XmlElement("annotation", Type = typeof(XmlSchemaAnnotation))]
		public XmlSchemaAnnotation Annotation
		{
			get
			{
				return annotation;
			}
			set
			{
				annotation = value;
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
