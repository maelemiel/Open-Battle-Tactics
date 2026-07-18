using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleContentRestriction : XmlSchemaContent
	{
		private const string xmlname = "restriction";

		private XmlSchemaAnyAttribute any;

		private XmlSchemaObjectCollection attributes;

		private XmlSchemaSimpleType baseType;

		private XmlQualifiedName baseTypeName;

		private XmlSchemaObjectCollection facets;

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

		[XmlElement("simpleType", Type = typeof(XmlSchemaSimpleType))]
		public XmlSchemaSimpleType BaseType
		{
			get
			{
				return baseType;
			}
			set
			{
				baseType = value;
			}
		}

		[XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet))]
		[XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet))]
		[XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet))]
		[XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet))]
		[XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet))]
		[XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet))]
		[XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet))]
		[XmlElement("length", typeof(XmlSchemaLengthFacet))]
		[XmlElement("minLength", typeof(XmlSchemaMinLengthFacet))]
		[XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet))]
		[XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet))]
		[XmlElement("pattern", typeof(XmlSchemaPatternFacet))]
		public XmlSchemaObjectCollection Facets
		{
			get
			{
				return facets;
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

		public XmlSchemaSimpleContentRestriction()
		{
			baseTypeName = XmlQualifiedName.Empty;
			attributes = new XmlSchemaObjectCollection();
			facets = new XmlSchemaObjectCollection();
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (BaseType != null)
			{
				BaseType.SetParent(this);
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
			}
			if (BaseTypeName == null || BaseTypeName.IsEmpty)
			{
				error(h, "base must be present, as a QName");
			}
			else if (!XmlSchemaUtil.CheckQName(BaseTypeName))
			{
				error(h, "BaseTypeName must be a QName");
			}
			if (BaseType != null)
			{
				errorCount += BaseType.Compile(h, schema);
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
					error(h, string.Concat(attribute2.GetType(), " is not valid in this place::SimpleContentRestriction"));
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
			if (baseType != null)
			{
				baseType.Validate(h, schema);
				actualBaseSchemaType = baseType;
			}
			else if (baseTypeName != XmlQualifiedName.Empty)
			{
				XmlSchemaType xmlSchemaType = schema.FindSchemaType(baseTypeName);
				if (xmlSchemaType != null)
				{
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
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal static XmlSchemaSimpleContentRestriction Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction = new XmlSchemaSimpleContentRestriction();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "restriction")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaComplexContentRestriction.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaSimpleContentRestriction.LineNumber = reader.LineNumber;
			xmlSchemaSimpleContentRestriction.LinePosition = reader.LinePosition;
			xmlSchemaSimpleContentRestriction.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "base")
				{
					Exception innerEx;
					xmlSchemaSimpleContentRestriction.baseTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for base attribute", innerEx);
					}
				}
				else if (reader.Name == "id")
				{
					xmlSchemaSimpleContentRestriction.Id = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for restriction", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleContentRestriction);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleContentRestriction;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "restriction")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleContentRestriction.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleContentRestriction.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2 && reader.LocalName == "simpleType")
				{
					num = 3;
					XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaSimpleContentRestriction.baseType = xmlSchemaSimpleType;
					}
					continue;
				}
				if (num <= 3)
				{
					if (reader.LocalName == "minExclusive")
					{
						num = 3;
						XmlSchemaMinExclusiveFacet xmlSchemaMinExclusiveFacet = XmlSchemaMinExclusiveFacet.Read(reader, h);
						if (xmlSchemaMinExclusiveFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaMinExclusiveFacet);
						}
						continue;
					}
					if (reader.LocalName == "minInclusive")
					{
						num = 3;
						XmlSchemaMinInclusiveFacet xmlSchemaMinInclusiveFacet = XmlSchemaMinInclusiveFacet.Read(reader, h);
						if (xmlSchemaMinInclusiveFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaMinInclusiveFacet);
						}
						continue;
					}
					if (reader.LocalName == "maxExclusive")
					{
						num = 3;
						XmlSchemaMaxExclusiveFacet xmlSchemaMaxExclusiveFacet = XmlSchemaMaxExclusiveFacet.Read(reader, h);
						if (xmlSchemaMaxExclusiveFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaMaxExclusiveFacet);
						}
						continue;
					}
					if (reader.LocalName == "maxInclusive")
					{
						num = 3;
						XmlSchemaMaxInclusiveFacet xmlSchemaMaxInclusiveFacet = XmlSchemaMaxInclusiveFacet.Read(reader, h);
						if (xmlSchemaMaxInclusiveFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaMaxInclusiveFacet);
						}
						continue;
					}
					if (reader.LocalName == "totalDigits")
					{
						num = 3;
						XmlSchemaTotalDigitsFacet xmlSchemaTotalDigitsFacet = XmlSchemaTotalDigitsFacet.Read(reader, h);
						if (xmlSchemaTotalDigitsFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaTotalDigitsFacet);
						}
						continue;
					}
					if (reader.LocalName == "fractionDigits")
					{
						num = 3;
						XmlSchemaFractionDigitsFacet xmlSchemaFractionDigitsFacet = XmlSchemaFractionDigitsFacet.Read(reader, h);
						if (xmlSchemaFractionDigitsFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaFractionDigitsFacet);
						}
						continue;
					}
					if (reader.LocalName == "length")
					{
						num = 3;
						XmlSchemaLengthFacet xmlSchemaLengthFacet = XmlSchemaLengthFacet.Read(reader, h);
						if (xmlSchemaLengthFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaLengthFacet);
						}
						continue;
					}
					if (reader.LocalName == "minLength")
					{
						num = 3;
						XmlSchemaMinLengthFacet xmlSchemaMinLengthFacet = XmlSchemaMinLengthFacet.Read(reader, h);
						if (xmlSchemaMinLengthFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaMinLengthFacet);
						}
						continue;
					}
					if (reader.LocalName == "maxLength")
					{
						num = 3;
						XmlSchemaMaxLengthFacet xmlSchemaMaxLengthFacet = XmlSchemaMaxLengthFacet.Read(reader, h);
						if (xmlSchemaMaxLengthFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaMaxLengthFacet);
						}
						continue;
					}
					if (reader.LocalName == "enumeration")
					{
						num = 3;
						XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = XmlSchemaEnumerationFacet.Read(reader, h);
						if (xmlSchemaEnumerationFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaEnumerationFacet);
						}
						continue;
					}
					if (reader.LocalName == "whiteSpace")
					{
						num = 3;
						XmlSchemaWhiteSpaceFacet xmlSchemaWhiteSpaceFacet = XmlSchemaWhiteSpaceFacet.Read(reader, h);
						if (xmlSchemaWhiteSpaceFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaWhiteSpaceFacet);
						}
						continue;
					}
					if (reader.LocalName == "pattern")
					{
						num = 3;
						XmlSchemaPatternFacet xmlSchemaPatternFacet = XmlSchemaPatternFacet.Read(reader, h);
						if (xmlSchemaPatternFacet != null)
						{
							xmlSchemaSimpleContentRestriction.facets.Add(xmlSchemaPatternFacet);
						}
						continue;
					}
				}
				if (num <= 4)
				{
					if (reader.LocalName == "attribute")
					{
						num = 4;
						XmlSchemaAttribute xmlSchemaAttribute = XmlSchemaAttribute.Read(reader, h);
						if (xmlSchemaAttribute != null)
						{
							xmlSchemaSimpleContentRestriction.Attributes.Add(xmlSchemaAttribute);
						}
						continue;
					}
					if (reader.LocalName == "attributeGroup")
					{
						num = 4;
						XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = XmlSchemaAttributeGroupRef.Read(reader, h);
						if (xmlSchemaAttributeGroupRef != null)
						{
							xmlSchemaSimpleContentRestriction.attributes.Add(xmlSchemaAttributeGroupRef);
						}
						continue;
					}
				}
				if (num <= 5 && reader.LocalName == "anyAttribute")
				{
					num = 6;
					XmlSchemaAnyAttribute xmlSchemaAnyAttribute = XmlSchemaAnyAttribute.Read(reader, h);
					if (xmlSchemaAnyAttribute != null)
					{
						xmlSchemaSimpleContentRestriction.AnyAttribute = xmlSchemaAnyAttribute;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaSimpleContentRestriction;
		}
	}
}
