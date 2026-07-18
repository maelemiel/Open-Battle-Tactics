using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaComplexContent : XmlSchemaContentModel
	{
		private const string xmlname = "complexContent";

		private XmlSchemaContent content;

		private bool isMixed;

		[XmlAttribute("mixed")]
		public bool IsMixed
		{
			get
			{
				return isMixed;
			}
			set
			{
				isMixed = value;
			}
		}

		[XmlElement("restriction", typeof(XmlSchemaComplexContentRestriction))]
		[XmlElement("extension", typeof(XmlSchemaComplexContentExtension))]
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
			if (isRedefinedComponent)
			{
				if (base.Annotation != null)
				{
					base.Annotation.isRedefinedComponent = true;
				}
				if (Content != null)
				{
					Content.isRedefinedComponent = true;
				}
			}
			if (Content == null)
			{
				error(h, "Content must be present in a complexContent");
			}
			else if (Content is XmlSchemaComplexContentRestriction)
			{
				XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = (XmlSchemaComplexContentRestriction)Content;
				errorCount += xmlSchemaComplexContentRestriction.Compile(h, schema);
			}
			else if (Content is XmlSchemaComplexContentExtension)
			{
				XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = (XmlSchemaComplexContentExtension)Content;
				errorCount += xmlSchemaComplexContentExtension.Compile(h, schema);
			}
			else
			{
				error(h, "complexContent can't have any value other than restriction or extention");
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

		internal static XmlSchemaComplexContent Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaComplexContent xmlSchemaComplexContent = new XmlSchemaComplexContent();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "complexContent")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaComplexContent.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaComplexContent.LineNumber = reader.LineNumber;
			xmlSchemaComplexContent.LinePosition = reader.LinePosition;
			xmlSchemaComplexContent.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaComplexContent.Id = reader.Value;
				}
				else if (reader.Name == "mixed")
				{
					Exception innerExcpetion;
					xmlSchemaComplexContent.isMixed = XmlSchemaUtil.ReadBoolAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for mixed", innerExcpetion);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for complexContent", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaComplexContent);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaComplexContent;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "complexContent")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaComplexContent.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaComplexContent.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "restriction")
					{
						num = 3;
						XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = XmlSchemaComplexContentRestriction.Read(reader, h);
						if (xmlSchemaComplexContentRestriction != null)
						{
							xmlSchemaComplexContent.content = xmlSchemaComplexContentRestriction;
						}
						continue;
					}
					if (reader.LocalName == "extension")
					{
						num = 3;
						XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = XmlSchemaComplexContentExtension.Read(reader, h);
						if (xmlSchemaComplexContentExtension != null)
						{
							xmlSchemaComplexContent.content = xmlSchemaComplexContentExtension;
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
			return xmlSchemaComplexContent;
		}
	}
}
