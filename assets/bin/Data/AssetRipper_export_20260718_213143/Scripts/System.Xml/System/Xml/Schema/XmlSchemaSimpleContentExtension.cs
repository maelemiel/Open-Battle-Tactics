using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleContentExtension : XmlSchemaContent
	{
		private const string xmlname = "extension";

		private XmlSchemaAnyAttribute any;

		private XmlSchemaObjectCollection attributes;

		private XmlQualifiedName baseTypeName;

		[XmlAttribute("base")]
		public XmlQualifiedName BaseTypeName
		{
			get
			{
				return baseTypeName;
			}
			set
			{
				baseTypeName = value;
			}
		}

		[XmlElement("attribute", typeof(XmlSchemaAttribute))]
		[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
		public XmlSchemaObjectCollection Attributes
		{
			get
			{
				return attributes;
			}
		}

		[XmlElement("anyAttribute")]
		public XmlSchemaAnyAttribute AnyAttribute
		{
			get
			{
				return any;
			}
			set
			{
				any = value;
			}
		}

		internal override bool IsExtension
		{
			get
			{
				return true;
			}
		}

		public XmlSchemaSimpleContentExtension()
		{
			baseTypeName = XmlQualifiedName.Empty;
			attributes = new XmlSchemaObjectCollection();
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (AnyAttribute != null)
			{
				AnyAttribute.SetParent(this);
			}
			foreach (XmlSchemaObject attribute in Attributes)
			{
				attribute.SetParent(this);
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
				if (AnyAttribute != null)
				{
					AnyAttribute.isRedefinedComponent = true;
				}
				foreach (XmlSchemaObject attribute in Attributes)
				{
					attribute.isRedefinedComponent = true;
				}
			}
			if (BaseTypeName == null || BaseTypeName.IsEmpty)
			{
				error(h, "base must be present, as a QName");
			}
			else if (!XmlSchemaUtil.CheckQName(BaseTypeName))
			{
				error(h, "BaseTypeName must be a QName");
			}
			if (AnyAttribute != null)
			{
				errorCount += AnyAttribute.Compile(h, schema);
			}
			foreach (XmlSchemaObject attribute2 in Attributes)
			{
				if (attribute2 is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)attribute2;
					errorCount += xmlSchemaAttribute.Compile(h, schema);
				}
				else if (attribute2 is XmlSchemaAttributeGroupRef)
				{
					XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = (XmlSchemaAttributeGroupRef)attribute2;
					errorCount += xmlSchemaAttributeGroupRef.Compile(h, schema);
				}
				else
				{
					error(h, string.Concat(attribute2.GetType(), " is not valid in this place::SimpleConentExtension"));
				}
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override XmlQualifiedName GetBaseTypeName()
		{
			return baseTypeName;
		}

		internal override XmlSchemaParticle GetParticle()
		{
			return null;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			XmlSchemaType xmlSchemaType = schema.FindSchemaType(baseTypeName);
			if (xmlSchemaType != null)
			{
				XmlSchemaComplexType xmlSchemaComplexType = xmlSchemaType as XmlSchemaComplexType;
				if (xmlSchemaComplexType != null && xmlSchemaComplexType.ContentModel is XmlSchemaComplexContent)
				{
					error(h, "Specified type is complex type which contains complex content.");
				}
				xmlSchemaType.Validate(h, schema);
				actualBaseSchemaType = xmlSchemaType;
			}
			else if (baseTypeName == XmlSchemaComplexType.AnyTypeName)
			{
				actualBaseSchemaType = XmlSchemaComplexType.AnyType;
			}
			else if (XmlSchemaUtil.IsBuiltInDatatypeName(baseTypeName))
			{
				actualBaseSchemaType = XmlSchemaDatatype.FromName(baseTypeName);
				if (actualBaseSchemaType == null)
				{
					error(h, "Invalid schema datatype name is specified.");
				}
			}
			else if (!schema.IsNamespaceAbsent(baseTypeName.Namespace))
			{
				error(h, string.Concat("Referenced base schema type ", baseTypeName, " was not found in the corresponding schema."));
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal static XmlSchemaSimpleContentExtension Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = new XmlSchemaSimpleContentExtension();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "extension")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAttributeGroup.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaSimpleContentExtension.LineNumber = reader.LineNumber;
			xmlSchemaSimpleContentExtension.LinePosition = reader.LinePosition;
			xmlSchemaSimpleContentExtension.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "base")
				{
					Exception innerEx;
					xmlSchemaSimpleContentExtension.baseTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for base attribute", innerEx);
					}
				}
				else if (reader.Name == "id")
				{
					xmlSchemaSimpleContentExtension.Id = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for extension in this context", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleContentExtension);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleContentExtension;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "extension")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleContentExtension.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleContentExtension.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "attribute")
					{
						num = 2;
						XmlSchemaAttribute xmlSchemaAttribute = XmlSchemaAttribute.Read(reader, h);
						if (xmlSchemaAttribute != null)
						{
							xmlSchemaSimpleContentExtension.Attributes.Add(xmlSchemaAttribute);
						}
						continue;
					}
					if (reader.LocalName == "attributeGroup")
					{
						num = 2;
						XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = XmlSchemaAttributeGroupRef.Read(reader, h);
						if (xmlSchemaAttributeGroupRef != null)
						{
							xmlSchemaSimpleContentExtension.attributes.Add(xmlSchemaAttributeGroupRef);
						}
						continue;
					}
				}
				if (num <= 3 && reader.LocalName == "anyAttribute")
				{
					num = 4;
					XmlSchemaAnyAttribute xmlSchemaAnyAttribute = XmlSchemaAnyAttribute.Read(reader, h);
					if (xmlSchemaAnyAttribute != null)
					{
						xmlSchemaSimpleContentExtension.AnyAttribute = xmlSchemaAnyAttribute;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaSimpleContentExtension;
		}
	}
}
