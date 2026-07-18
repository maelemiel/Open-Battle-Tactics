using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaComplexContentRestriction : XmlSchemaContent
	{
		private const string xmlname = "restriction";

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

		[XmlElement("group", typeof(XmlSchemaGroupRef))]
		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
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
				return false;
			}
		}

		public XmlSchemaComplexContentRestriction()
		{
			baseTypeName = XmlQualifiedName.Empty;
			attributes = new XmlSchemaObjectCollection();
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
					error(h, string.Concat(attribute2.GetType(), " is not valid in this place::ComplexContentRestriction"));
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
			if (Particle != null)
			{
				Particle.Validate(h, schema);
			}
			return errorCount;
		}

		internal static XmlSchemaComplexContentRestriction Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = new XmlSchemaComplexContentRestriction();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "restriction")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaComplexContentRestriction.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaComplexContentRestriction.LineNumber = reader.LineNumber;
			xmlSchemaComplexContentRestriction.LinePosition = reader.LinePosition;
			xmlSchemaComplexContentRestriction.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "base")
				{
					Exception innerEx;
					xmlSchemaComplexContentRestriction.baseTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for base attribute", innerEx);
					}
				}
				else if (reader.Name == "id")
				{
					xmlSchemaComplexContentRestriction.Id = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for restriction", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaComplexContentRestriction);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaComplexContentRestriction;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "restriction")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaComplexContentRestriction.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaComplexContentRestriction.Annotation = xmlSchemaAnnotation;
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
							xmlSchemaComplexContentRestriction.particle = xmlSchemaGroupRef;
						}
						continue;
					}
					if (reader.LocalName == "all")
					{
						num = 3;
						XmlSchemaAll xmlSchemaAll = XmlSchemaAll.Read(reader, h);
						if (xmlSchemaAll != null)
						{
							xmlSchemaComplexContentRestriction.particle = xmlSchemaAll;
						}
						continue;
					}
					if (reader.LocalName == "choice")
					{
						num = 3;
						XmlSchemaChoice xmlSchemaChoice = XmlSchemaChoice.Read(reader, h);
						if (xmlSchemaChoice != null)
						{
							xmlSchemaComplexContentRestriction.particle = xmlSchemaChoice;
						}
						continue;
					}
					if (reader.LocalName == "sequence")
					{
						num = 3;
						XmlSchemaSequence xmlSchemaSequence = XmlSchemaSequence.Read(reader, h);
						if (xmlSchemaSequence != null)
						{
							xmlSchemaComplexContentRestriction.particle = xmlSchemaSequence;
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
							xmlSchemaComplexContentRestriction.Attributes.Add(xmlSchemaAttribute);
						}
						continue;
					}
					if (reader.LocalName == "attributeGroup")
					{
						num = 3;
						XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = XmlSchemaAttributeGroupRef.Read(reader, h);
						if (xmlSchemaAttributeGroupRef != null)
						{
							xmlSchemaComplexContentRestriction.attributes.Add(xmlSchemaAttributeGroupRef);
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
						xmlSchemaComplexContentRestriction.AnyAttribute = xmlSchemaAnyAttribute;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaComplexContentRestriction;
		}
	}
}
