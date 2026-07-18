using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaComplexContentExtension : XmlSchemaContent
	{
		private const string xmlname = "extension";

		private XmlSchemaAnyAttribute any;

		private XmlSchemaObjectCollection attributes;

		private XmlQualifiedName baseTypeName;

		private XmlSchemaParticle particle;

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

		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("group", typeof(XmlSchemaGroupRef))]
		[XmlElement("all", typeof(XmlSchemaAll))]
		public XmlSchemaParticle Particle
		{
			get
			{
				return particle;
			}
			set
			{
				particle = value;
			}
		}

		[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
		[XmlElement("attribute", typeof(XmlSchemaAttribute))]
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

		public XmlSchemaComplexContentExtension()
		{
			attributes = new XmlSchemaObjectCollection();
			baseTypeName = XmlQualifiedName.Empty;
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (Particle != null)
			{
				Particle.SetParent(this);
			}
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
				if (Particle != null)
				{
					Particle.isRedefinedComponent = true;
				}
			}
			if (BaseTypeName == null || BaseTypeName.IsEmpty)
			{
				error(h, "base must be present, as a QName");
			}
			else if (!XmlSchemaUtil.CheckQName(BaseTypeName))
			{
				error(h, "BaseTypeName is not a valid XmlQualifiedName");
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
					error(h, string.Concat(attribute2.GetType(), " is not valid in this place::ComplexConetnetExtension"));
				}
			}
			if (Particle != null)
			{
				if (Particle is XmlSchemaGroupRef)
				{
					errorCount += ((XmlSchemaGroupRef)Particle).Compile(h, schema);
				}
				else if (Particle is XmlSchemaAll)
				{
					errorCount += ((XmlSchemaAll)Particle).Compile(h, schema);
				}
				else if (Particle is XmlSchemaChoice)
				{
					errorCount += ((XmlSchemaChoice)Particle).Compile(h, schema);
				}
				else if (Particle is XmlSchemaSequence)
				{
					errorCount += ((XmlSchemaSequence)Particle).Compile(h, schema);
				}
				else
				{
					error(h, "Particle of a restriction is limited only to group, sequence, choice and all.");
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
			return particle;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			if (AnyAttribute != null)
			{
				errorCount += AnyAttribute.Validate(h, schema);
			}
			foreach (XmlSchemaObject attribute in Attributes)
			{
				errorCount += attribute.Validate(h, schema);
			}
			if (Particle != null)
			{
				errorCount += Particle.Validate(h, schema);
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal static XmlSchemaComplexContentExtension Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = new XmlSchemaComplexContentExtension();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "extension")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaComplexContentExtension.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaComplexContentExtension.LineNumber = reader.LineNumber;
			xmlSchemaComplexContentExtension.LinePosition = reader.LinePosition;
			xmlSchemaComplexContentExtension.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "base")
				{
					Exception innerEx;
					xmlSchemaComplexContentExtension.baseTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for base attribute", innerEx);
					}
				}
				else if (reader.Name == "id")
				{
					xmlSchemaComplexContentExtension.Id = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for extension", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaComplexContentExtension);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaComplexContentExtension;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "extension")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaComplexContentExtension.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaComplexContentExtension.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "group")
					{
						num = 3;
						XmlSchemaGroupRef xmlSchemaGroupRef = XmlSchemaGroupRef.Read(reader, h);
						if (xmlSchemaGroupRef != null)
						{
							xmlSchemaComplexContentExtension.particle = xmlSchemaGroupRef;
						}
						continue;
					}
					if (reader.LocalName == "all")
					{
						num = 3;
						XmlSchemaAll xmlSchemaAll = XmlSchemaAll.Read(reader, h);
						if (xmlSchemaAll != null)
						{
							xmlSchemaComplexContentExtension.particle = xmlSchemaAll;
						}
						continue;
					}
					if (reader.LocalName == "choice")
					{
						num = 3;
						XmlSchemaChoice xmlSchemaChoice = XmlSchemaChoice.Read(reader, h);
						if (xmlSchemaChoice != null)
						{
							xmlSchemaComplexContentExtension.particle = xmlSchemaChoice;
						}
						continue;
					}
					if (reader.LocalName == "sequence")
					{
						num = 3;
						XmlSchemaSequence xmlSchemaSequence = XmlSchemaSequence.Read(reader, h);
						if (xmlSchemaSequence != null)
						{
							xmlSchemaComplexContentExtension.particle = xmlSchemaSequence;
						}
						continue;
					}
				}
				if (num <= 3)
				{
					if (reader.LocalName == "attribute")
					{
						num = 3;
						XmlSchemaAttribute xmlSchemaAttribute = XmlSchemaAttribute.Read(reader, h);
						if (xmlSchemaAttribute != null)
						{
							xmlSchemaComplexContentExtension.Attributes.Add(xmlSchemaAttribute);
						}
						continue;
					}
					if (reader.LocalName == "attributeGroup")
					{
						num = 3;
						XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = XmlSchemaAttributeGroupRef.Read(reader, h);
						if (xmlSchemaAttributeGroupRef != null)
						{
							xmlSchemaComplexContentExtension.attributes.Add(xmlSchemaAttributeGroupRef);
						}
						continue;
					}
				}
				if (num <= 4 && reader.LocalName == "anyAttribute")
				{
					num = 5;
					XmlSchemaAnyAttribute xmlSchemaAnyAttribute = XmlSchemaAnyAttribute.Read(reader, h);
					if (xmlSchemaAnyAttribute != null)
					{
						xmlSchemaComplexContentExtension.AnyAttribute = xmlSchemaAnyAttribute;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaComplexContentExtension;
		}
	}
}
