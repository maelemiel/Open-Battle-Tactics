using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleContent : XmlSchemaContentModel
	{
		private const string xmlname = "simpleContent";

		private XmlSchemaContent content;

		[XmlElement("restriction", typeof(XmlSchemaSimpleContentRestriction))]
		[XmlElement("extension", typeof(XmlSchemaSimpleContentExtension))]
		public override XmlSchemaContent Content
		{
			get
			{
				return content;
			}
			set
			{
				content = value;
			}
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (Content != null)
			{
				Content.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			if (Content == null)
			{
				error(h, "Content must be present in a simpleContent");
			}
			else if (Content is XmlSchemaSimpleContentRestriction)
			{
				XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction = (XmlSchemaSimpleContentRestriction)Content;
				errorCount += xmlSchemaSimpleContentRestriction.Compile(h, schema);
			}
			else if (Content is XmlSchemaSimpleContentExtension)
			{
				XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = (XmlSchemaSimpleContentExtension)Content;
				errorCount += xmlSchemaSimpleContentExtension.Compile(h, schema);
			}
			else
			{
				error(h, "simpleContent can't have any value other than restriction or extention");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			errorCount += Content.Validate(h, schema);
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal static XmlSchemaSimpleContent Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleContent xmlSchemaSimpleContent = new XmlSchemaSimpleContent();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "simpleContent")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaComplexContent.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaSimpleContent.LineNumber = reader.LineNumber;
			xmlSchemaSimpleContent.LinePosition = reader.LinePosition;
			xmlSchemaSimpleContent.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaSimpleContent.Id = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for simpleContent", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleContent);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleContent;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "simpleContent")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleContent.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleContent.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "restriction")
					{
						num = 3;
						XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction = XmlSchemaSimpleContentRestriction.Read(reader, h);
						if (xmlSchemaSimpleContentRestriction != null)
						{
							xmlSchemaSimpleContent.content = xmlSchemaSimpleContentRestriction;
						}
						continue;
					}
					if (reader.LocalName == "extension")
					{
						num = 3;
						XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = XmlSchemaSimpleContentExtension.Read(reader, h);
						if (xmlSchemaSimpleContentExtension != null)
						{
							xmlSchemaSimpleContent.content = xmlSchemaSimpleContentExtension;
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
			return xmlSchemaSimpleContent;
		}
	}
}
