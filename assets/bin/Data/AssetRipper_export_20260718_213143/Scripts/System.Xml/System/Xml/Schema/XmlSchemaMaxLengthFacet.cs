namespace System.Xml.Schema
{
	public class XmlSchemaMaxLengthFacet : XmlSchemaNumericFacet
	{
		private const string xmlname = "maxLength";

		internal override Facet ThisFacet
		{
			get
			{
				return Facet.maxLength;
			}
		}

		internal static XmlSchemaMaxLengthFacet Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaMaxLengthFacet xmlSchemaMaxLengthFacet = new XmlSchemaMaxLengthFacet();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "maxLength")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaMaxLengthFacet.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaMaxLengthFacet.LineNumber = reader.LineNumber;
			xmlSchemaMaxLengthFacet.LinePosition = reader.LinePosition;
			xmlSchemaMaxLengthFacet.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaMaxLengthFacet.Id = reader.Value;
				}
				else if (reader.Name == "fixed")
				{
					Exception innerExcpetion;
					xmlSchemaMaxLengthFacet.IsFixed = XmlSchemaUtil.ReadBoolAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for fixed attribute", innerExcpetion);
					}
				}
				else if (reader.Name == "value")
				{
					xmlSchemaMaxLengthFacet.Value = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for group", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaMaxLengthFacet);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaMaxLengthFacet;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "maxLength")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaMaxLengthFacet.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaMaxLengthFacet.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaMaxLengthFacet;
		}
	}
}
