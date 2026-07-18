using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAppInfo : XmlSchemaObject
	{
		private XmlNode[] markup;

		private string source;

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

		internal static XmlSchemaAppInfo Read(XmlSchemaReader reader, ValidationEventHandler h, out bool skip)
		{
			skip = false;
			XmlSchemaAppInfo xmlSchemaAppInfo = new XmlSchemaAppInfo();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "appinfo")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAppInfo.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaAppInfo.LineNumber = reader.LineNumber;
			xmlSchemaAppInfo.LinePosition = reader.LinePosition;
			xmlSchemaAppInfo.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "source")
				{
					xmlSchemaAppInfo.source = reader.Value;
				}
				else
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for appinfo", null);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				xmlSchemaAppInfo.Markup = new XmlNode[0];
				return xmlSchemaAppInfo;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.ReadNode(reader));
			XmlNode firstChild = xmlDocument.FirstChild;
			if (firstChild != null && firstChild.ChildNodes != null)
			{
				xmlSchemaAppInfo.Markup = new XmlNode[firstChild.ChildNodes.Count];
				for (int i = 0; i < firstChild.ChildNodes.Count; i++)
				{
					xmlSchemaAppInfo.Markup[i] = firstChild.ChildNodes[i];
				}
			}
			if (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.EndElement)
			{
				skip = true;
			}
			return xmlSchemaAppInfo;
		}
	}
}
