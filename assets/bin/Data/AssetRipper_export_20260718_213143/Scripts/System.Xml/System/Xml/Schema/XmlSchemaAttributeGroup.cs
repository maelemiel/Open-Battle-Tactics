using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAttributeGroup : XmlSchemaAnnotated
	{
		private const string xmlname = "attributeGroup";

		private XmlSchemaAnyAttribute anyAttribute;

		private XmlSchemaObjectCollection attributes;

		private string name;

		private XmlSchemaAttributeGroup redefined;

		private XmlQualifiedName qualifiedName;

		private XmlSchemaObjectTable attributeUses;

		private XmlSchemaAnyAttribute anyAttributeUse;

		internal bool AttributeGroupRecursionCheck;

		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
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

		internal XmlSchemaObjectTable AttributeUses
		{
			get
			{
				return attributeUses;
			}
		}

		internal XmlSchemaAnyAttribute AnyAttributeUse
		{
			get
			{
				return anyAttributeUse;
			}
		}

		[XmlElement("anyAttribute")]
		public XmlSchemaAnyAttribute AnyAttribute
		{
			get
			{
				return anyAttribute;
			}
			set
			{
				anyAttribute = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaAttributeGroup RedefinedAttributeGroup
		{
			get
			{
				return redefined;
			}
		}

		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return qualifiedName;
			}
		}

		public XmlSchemaAttributeGroup()
		{
			attributes = new XmlSchemaObjectCollection();
			qualifiedName = XmlQualifiedName.Empty;
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
				return errorCount;
			}
			errorCount = 0;
			if (redefinedObject != null)
			{
				errorCount += redefined.Compile(h, schema);
				if (errorCount == 0)
				{
					redefined = (XmlSchemaAttributeGroup)redefinedObject;
				}
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			if (Name == null || Name == string.Empty)
			{
				error(h, "Name is required in top level simpletype");
			}
			else if (!XmlSchemaUtil.CheckNCName(Name))
			{
				error(h, "name attribute of a simpleType must be NCName");
			}
			else
			{
				qualifiedName = new XmlQualifiedName(Name, base.AncestorSchema.TargetNamespace);
			}
			if (AnyAttribute != null)
			{
				errorCount += AnyAttribute.Compile(h, schema);
			}
			foreach (XmlSchemaObject attribute in Attributes)
			{
				if (attribute is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)attribute;
					errorCount += xmlSchemaAttribute.Compile(h, schema);
				}
				else if (attribute is XmlSchemaAttributeGroupRef)
				{
					XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = (XmlSchemaAttributeGroupRef)attribute;
					errorCount += xmlSchemaAttributeGroupRef.Compile(h, schema);
				}
				else
				{
					error(h, "invalid type of object in Attributes property");
				}
			}
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.CompilationId))
			{
				return errorCount;
			}
			if (redefined == null && redefinedObject != null)
			{
				redefinedObject.Compile(h, schema);
				redefined = (XmlSchemaAttributeGroup)redefinedObject;
				redefined.Validate(h, schema);
			}
			XmlSchemaObjectCollection xmlSchemaObjectCollection = null;
			xmlSchemaObjectCollection = Attributes;
			attributeUses = new XmlSchemaObjectTable();
			errorCount += XmlSchemaUtil.ValidateAttributesResolved(attributeUses, h, schema, xmlSchemaObjectCollection, AnyAttribute, ref anyAttributeUse, redefined, false);
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal static XmlSchemaAttributeGroup Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaAttributeGroup xmlSchemaAttributeGroup = new XmlSchemaAttributeGroup();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "attributeGroup")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAttributeGroup.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaAttributeGroup.LineNumber = reader.LineNumber;
			xmlSchemaAttributeGroup.LinePosition = reader.LinePosition;
			xmlSchemaAttributeGroup.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaAttributeGroup.Id = reader.Value;
				}
				else if (reader.Name == "name")
				{
					xmlSchemaAttributeGroup.name = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for attributeGroup in this context", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaAttributeGroup);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaAttributeGroup;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "attributeGroup")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaAttributeGroup.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaAttributeGroup.Annotation = xmlSchemaAnnotation;
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
							xmlSchemaAttributeGroup.Attributes.Add(xmlSchemaAttribute);
						}
						continue;
					}
					if (reader.LocalName == "attributeGroup")
					{
						num = 2;
						XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = XmlSchemaAttributeGroupRef.Read(reader, h);
						if (xmlSchemaAttributeGroupRef != null)
						{
							xmlSchemaAttributeGroup.attributes.Add(xmlSchemaAttributeGroupRef);
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
						xmlSchemaAttributeGroup.AnyAttribute = xmlSchemaAnyAttribute;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaAttributeGroup;
		}
	}
}
