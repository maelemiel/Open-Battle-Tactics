namespace System.Xml.Schema
{
	public class XmlSchemaFractionDigitsFacet : XmlSchemaNumericFacet
	{
		private const string xmlname = "fractionDigits";

		internal override Facet ThisFacet
		{
			get
			{
				return Facet.fractionDigits;
			}
		}

		internal static XmlSchemaFractionDigitsFacet Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaFractionDigitsFacet xmlSchemaFractionDigitsFacet = new XmlSchemaFractionDigitsFacet();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "fractionDigits")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaFractionDigitsFacet.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaFractionDigitsFacet.LineNumber = reader.LineNumber;
			xmlSchemaFractionDigitsFacet.LinePosition = reader.LinePosition;
			xmlSchemaFractionDigitsFacet.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaFractionDigitsFacet.Id = reader.Value;
				}
				else if (reader.Name == "fixed")
				{
					Exception innerExcpetion;
					xmlSchemaFractionDigitsFacet.IsFixed = XmlSchemaUtil.ReadBoolAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for fixed attribute", innerExcpetion);
					}
				}
				else if (reader.Name == "value")
				{
					xmlSchemaFractionDigitsFacet.Value = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for fractionDigits", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaFractionDigitsFacet);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaFractionDigitsFacet;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "fractionDigits")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaFractionDigitsFacet.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaFractionDigitsFacet.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaFractionDigitsFacet;
		}
	}
}
