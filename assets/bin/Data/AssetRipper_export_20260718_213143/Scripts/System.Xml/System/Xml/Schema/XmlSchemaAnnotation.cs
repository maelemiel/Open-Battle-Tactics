using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAnnotation : XmlSchemaObject
	{
		private const string xmlname = "annotation";

		private string id;

		private XmlSchemaObjectCollection items;

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

		[XmlElement("appinfo", typeof(XmlSchemaAppInfo))]
		[XmlElement("documentation", typeof(XmlSchemaDocumentation))]
		public XmlSchemaObjectCollection Items
		{
			get
			{
				return items;
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

		public XmlSchemaAnnotation()
		{
			items = new XmlSchemaObjectCollection();
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			CompilationId = schema.CompilationId;
			return 0;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			return 0;
		}

		internal static XmlSchemaAnnotation Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaAnnotation xmlSchemaAnnotation = new XmlSchemaAnnotation();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "annotation")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAnnotation.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaAnnotation.LineNumber = reader.LineNumber;
			xmlSchemaAnnotation.LinePosition = reader.LinePosition;
			xmlSchemaAnnotation.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaAnnotation.Id = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for annotation", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaAnnotation);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaAnnotation;
			}
			bool skip = false;
			string text = null;
			while (!reader.EOF)
			{
				if (skip)
				{
					skip = false;
				}
				else
				{
					reader.ReadNextElement();
				}
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					bool flag = true;
					string text2 = "annotation";
					if (text != null)
					{
						text2 = text;
						text = null;
						flag = false;
					}
					if (reader.LocalName != text2)
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaAnnotation.Read, name=" + reader.Name + ",expected=" + text2, null);
					}
					if (flag)
					{
						break;
					}
				}
				else if (reader.LocalName == "appinfo")
				{
					XmlSchemaAppInfo xmlSchemaAppInfo = XmlSchemaAppInfo.Read(reader, h, out skip);
					if (xmlSchemaAppInfo != null)
					{
						xmlSchemaAnnotation.items.Add(xmlSchemaAppInfo);
					}
				}
				else if (reader.LocalName == "documentation")
				{
					XmlSchemaDocumentation xmlSchemaDocumentation = XmlSchemaDocumentation.Read(reader, h, out skip);
					if (xmlSchemaDocumentation != null)
					{
						xmlSchemaAnnotation.items.Add(xmlSchemaDocumentation);
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaAnnotation;
		}
	}
}
