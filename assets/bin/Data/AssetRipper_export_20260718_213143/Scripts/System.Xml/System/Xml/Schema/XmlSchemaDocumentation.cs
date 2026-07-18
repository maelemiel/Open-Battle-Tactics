using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaDocumentation : XmlSchemaObject
	{
		private string language;

		private XmlNode[] markup;

		private string source;

		[XmlAnyElement]
		[XmlText]
		public XmlNode[] Markup
		{
			get
			{
				return markup;
			}
			set
			{
				markup = value;
			}
		}

		[XmlAttribute("source", DataType = "anyURI")]
		public string Source
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

		[XmlAttribute("xml:lang")]
		public string Language
		{
			get
			{
				return language;
			}
			set
			{
				language = value;
			}
		}

		internal static XmlSchemaDocumentation Read(XmlSchemaReader reader, ValidationEventHandler h, out bool skip)
		{
			skip = false;
			XmlSchemaDocumentation xmlSchemaDocumentation = new XmlSchemaDocumentation();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "documentation")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaDocumentation.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaDocumentation.LineNumber = reader.LineNumber;
			xmlSchemaDocumentation.LinePosition = reader.LinePosition;
			xmlSchemaDocumentation.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "source")
				{
					xmlSchemaDocumentation.source = reader.Value;
				}
				else if (reader.Name == "xml:lang")
				{
					xmlSchemaDocumentation.language = reader.Value;
				}
				else
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for documentation", null);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				xmlSchemaDocumentation.Markup = new XmlNode[0];
				return xmlSchemaDocumentation;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.ReadNode(reader));
			XmlNode firstChild = xmlDocument.FirstChild;
			if (firstChild != null && firstChild.ChildNodes != null)
			{
				xmlSchemaDocumentation.Markup = new XmlNode[firstChild.ChildNodes.Count];
				for (int i = 0; i < firstChild.ChildNodes.Count; i++)
				{
					xmlSchemaDocumentation.Markup[i] = firstChild.ChildNodes[i];
				}
			}
			if (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.EndElement)
			{
				skip = true;
			}
			return xmlSchemaDocumentation;
		}
	}
}
