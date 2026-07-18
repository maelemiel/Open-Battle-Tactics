namespace System.Xml.Schema
{
	public class XmlSchemaMinExclusiveFacet : XmlSchemaFacet
	{
		private const string xmlname = "minExclusive";

		internal override Facet ThisFacet
		{
			get
			{
				return Facet.minExclusive;
			}
		}

		internal static XmlSchemaMinExclusiveFacet Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaMinExclusiveFacet xmlSchemaMinExclusiveFacet = new XmlSchemaMinExclusiveFacet();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "minExclusive")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaMinExclusiveFacet.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaMinExclusiveFacet.LineNumber = reader.LineNumber;
			xmlSchemaMinExclusiveFacet.LinePosition = reader.LinePosition;
			xmlSchemaMinExclusiveFacet.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaMinExclusiveFacet.Id = reader.Value;
				}
				else if (reader.Name == "fixed")
				{
					Exception innerExcpetion;
					xmlSchemaMinExclusiveFacet.IsFixed = XmlSchemaUtil.ReadBoolAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for fixed attribute", innerExcpetion);
					}
				}
				else if (reader.Name == "value")
				{
					xmlSchemaMinExclusiveFacet.Value = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for minExclusive", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaMinExclusiveFacet);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaMinExclusiveFacet;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "minExclusive")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaMinExclusiveFacet.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaMinExclusiveFacet.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaMinExclusiveFacet;
		}
	}
}
