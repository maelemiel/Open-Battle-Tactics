namespace System.Xml.Schema
{
	public class XmlSchemaTotalDigitsFacet : XmlSchemaNumericFacet
	{
		private const string xmlname = "totalDigits";

		internal override Facet ThisFacet
		{
			get
			{
				return Facet.totalDigits;
			}
		}

		internal static XmlSchemaTotalDigitsFacet Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaTotalDigitsFacet xmlSchemaTotalDigitsFacet = new XmlSchemaTotalDigitsFacet();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "totalDigits")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaTotalDigitsFacet.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaTotalDigitsFacet.LineNumber = reader.LineNumber;
			xmlSchemaTotalDigitsFacet.LinePosition = reader.LinePosition;
			xmlSchemaTotalDigitsFacet.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaTotalDigitsFacet.Id = reader.Value;
				}
				else if (reader.Name == "fixed")
				{
					Exception innerExcpetion;
					xmlSchemaTotalDigitsFacet.IsFixed = XmlSchemaUtil.ReadBoolAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for fixed attribute", innerExcpetion);
					}
				}
				else if (reader.Name == "value")
				{
					xmlSchemaTotalDigitsFacet.Value = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for totalDigits", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaTotalDigitsFacet);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaTotalDigitsFacet;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "totalDigits")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaTotalDigitsFacet.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaTotalDigitsFacet.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaTotalDigitsFacet;
		}
	}
}
