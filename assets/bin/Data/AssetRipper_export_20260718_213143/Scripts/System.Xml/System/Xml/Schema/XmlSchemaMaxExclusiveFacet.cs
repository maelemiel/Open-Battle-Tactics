namespace System.Xml.Schema
{
	public class XmlSchemaMaxExclusiveFacet : XmlSchemaFacet
	{
		private const string xmlname = "maxExclusive";

		internal override Facet ThisFacet
		{
			get
			{
				return Facet.maxExclusive;
			}
		}

		internal static XmlSchemaMaxExclusiveFacet Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaMaxExclusiveFacet xmlSchemaMaxExclusiveFacet = new XmlSchemaMaxExclusiveFacet();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "maxExclusive")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaMaxExclusiveFacet.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaMaxExclusiveFacet.LineNumber = reader.LineNumber;
			xmlSchemaMaxExclusiveFacet.LinePosition = reader.LinePosition;
			xmlSchemaMaxExclusiveFacet.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaMaxExclusiveFacet.Id = reader.Value;
				}
				else if (reader.Name == "fixed")
				{
					Exception innerExcpetion;
					xmlSchemaMaxExclusiveFacet.IsFixed = XmlSchemaUtil.ReadBoolAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for fixed attribute", innerExcpetion);
					}
				}
				else if (reader.Name == "value")
				{
					xmlSchemaMaxExclusiveFacet.Value = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for maxExclusive", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaMaxExclusiveFacet);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaMaxExclusiveFacet;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "maxExclusive")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaMaxExclusiveFacet.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaMaxExclusiveFacet.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaMaxExclusiveFacet;
		}
	}
}
