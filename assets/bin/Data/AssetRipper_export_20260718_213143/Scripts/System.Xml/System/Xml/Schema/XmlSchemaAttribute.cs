using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAttribute : XmlSchemaAnnotated
	{
		private const string xmlname = "attribute";

		private object attributeType;

		private XmlSchemaSimpleType attributeSchemaType;

		private string defaultValue;

		private string fixedValue;

		private string validatedDefaultValue;

		private string validatedFixedValue;

		private object validatedFixedTypedValue;

		private XmlSchemaForm form;

		private string name;

		private string targetNamespace;

		private XmlQualifiedName qualifiedName;

		private XmlQualifiedName refName;

		private XmlSchemaSimpleType schemaType;

		private XmlQualifiedName schemaTypeName;

		private XmlSchemaUse use;

		private XmlSchemaUse validatedUse;

		private XmlSchemaAttribute referencedAttribute;

		internal bool ParentIsSchema
		{
			get
			{
				return base.Parent is XmlSchema;
			}
		}

		[DefaultValue(null)]
		[XmlAttribute("default")]
		public string DefaultValue
		{
			get
			{
				return defaultValue;
			}
			set
			{
				fixedValue = null;
				defaultValue = value;
			}
		}

		[XmlAttribute("fixed")]
		[DefaultValue(null)]
		public string FixedValue
		{
			get
			{
				return fixedValue;
			}
			set
			{
				defaultValue = null;
				fixedValue = value;
			}
		}

		[DefaultValue(XmlSchemaForm.None)]
		[XmlAttribute("form")]
		public XmlSchemaForm Form
		{
			get
			{
				return form;
			}
			set
			{
				form = value;
			}
		}

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

		[XmlAttribute("ref")]
		public XmlQualifiedName RefName
		{
			get
			{
				return refName;
			}
			set
			{
				refName = value;
			}
		}

		[XmlAttribute("type")]
		public XmlQualifiedName SchemaTypeName
		{
			get
			{
				return schemaTypeName;
			}
			set
			{
				schemaTypeName = value;
			}
		}

		[XmlElement("simpleType")]
		public XmlSchemaSimpleType SchemaType
		{
			get
			{
				return schemaType;
			}
			set
			{
				schemaType = value;
			}
		}

		[DefaultValue(XmlSchemaUse.None)]
		[XmlAttribute("use")]
		public XmlSchemaUse Use
		{
			get
			{
				return use;
			}
			set
			{
				use = value;
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

		[Obsolete]
		[XmlIgnore]
		public object AttributeType
		{
			get
			{
				if (referencedAttribute != null)
				{
					return referencedAttribute.AttributeType;
				}
				return attributeType;
			}
		}

		[XmlIgnore]
		public XmlSchemaSimpleType AttributeSchemaType
		{
			get
			{
				if (referencedAttribute != null)
				{
					return referencedAttribute.AttributeSchemaType;
				}
				return attributeSchemaType;
			}
		}

		internal string ValidatedDefaultValue
		{
			get
			{
				return validatedDefaultValue;
			}
		}

		internal string ValidatedFixedValue
		{
			get
			{
				return validatedFixedValue;
			}
		}

		internal object ValidatedFixedTypedValue
		{
			get
			{
				return validatedFixedTypedValue;
			}
		}

		internal XmlSchemaUse ValidatedUse
		{
			get
			{
				return validatedUse;
			}
		}

		public XmlSchemaAttribute()
		{
			form = XmlSchemaForm.None;
			use = XmlSchemaUse.None;
			schemaTypeName = XmlQualifiedName.Empty;
			qualifiedName = XmlQualifiedName.Empty;
			refName = XmlQualifiedName.Empty;
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (schemaType != null)
			{
				schemaType.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			errorCount = 0;
			if (ParentIsSchema || isRedefineChild)
			{
				if (RefName != null && !RefName.IsEmpty)
				{
					error(h, "ref must be absent in the top level <attribute>");
				}
				if (Form != XmlSchemaForm.None)
				{
					error(h, "form must be absent in the top level <attribute>");
				}
				if (Use != XmlSchemaUse.None)
				{
					error(h, "use must be absent in the top level <attribute>");
				}
				targetNamespace = base.AncestorSchema.TargetNamespace;
				CompileCommon(h, schema, true);
			}
			else if (RefName == null || RefName.IsEmpty)
			{
				if (form == XmlSchemaForm.Qualified || (form == XmlSchemaForm.None && schema.AttributeFormDefault == XmlSchemaForm.Qualified))
				{
					targetNamespace = base.AncestorSchema.TargetNamespace;
				}
				else
				{
					targetNamespace = string.Empty;
				}
				CompileCommon(h, schema, true);
			}
			else
			{
				if (name != null)
				{
					error(h, "name must be absent if ref is present");
				}
				if (form != XmlSchemaForm.None)
				{
					error(h, "form must be absent if ref is present");
				}
				if (schemaType != null)
				{
					error(h, "simpletype must be absent if ref is present");
				}
				if (schemaTypeName != null && !schemaTypeName.IsEmpty)
				{
					error(h, "type must be absent if ref is present");
				}
				CompileCommon(h, schema, false);
			}
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		private void CompileCommon(ValidationEventHandler h, XmlSchema schema, bool refIsNotPresent)
		{
			if (refIsNotPresent)
			{
				if (Name == null)
				{
					error(h, "Required attribute name must be present");
				}
				else if (!XmlSchemaUtil.CheckNCName(Name))
				{
					error(h, "attribute name must be NCName");
				}
				else if (Name == "xmlns")
				{
					error(h, "attribute name must not be xmlns");
				}
				else
				{
					qualifiedName = new XmlQualifiedName(Name, targetNamespace);
				}
				if (SchemaType != null)
				{
					if (SchemaTypeName != null && !SchemaTypeName.IsEmpty)
					{
						error(h, "attribute can't have both a type and <simpleType> content");
					}
					errorCount += SchemaType.Compile(h, schema);
				}
				if (SchemaTypeName != null && !XmlSchemaUtil.CheckQName(SchemaTypeName))
				{
					error(h, string.Concat(SchemaTypeName, " is not a valid QName"));
				}
			}
			else
			{
				if (RefName == null || RefName.IsEmpty)
				{
					throw new InvalidOperationException("Error: Should Never Happen. refname must be present");
				}
				qualifiedName = RefName;
			}
			if (base.AncestorSchema.TargetNamespace == "http://www.w3.org/2001/XMLSchema-instance" && Name != "nil" && Name != "type" && Name != "schemaLocation" && Name != "noNamespaceSchemaLocation")
			{
				error(h, "targetNamespace can't be http://www.w3.org/2001/XMLSchema-instance");
			}
			if (DefaultValue != null && FixedValue != null)
			{
				error(h, "default and fixed must not both be present in an Attribute");
			}
			if (DefaultValue != null && Use != XmlSchemaUse.None && Use != XmlSchemaUse.Optional)
			{
				error(h, "if default is present, use must be optional");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			if (SchemaType != null)
			{
				SchemaType.Validate(h, schema);
				attributeType = SchemaType;
			}
			else if (SchemaTypeName != null && SchemaTypeName != XmlQualifiedName.Empty)
			{
				XmlSchemaType xmlSchemaType = schema.FindSchemaType(SchemaTypeName);
				if (xmlSchemaType is XmlSchemaComplexType)
				{
					error(h, "An attribute can't have complexType Content");
				}
				else if (xmlSchemaType != null)
				{
					errorCount += xmlSchemaType.Validate(h, schema);
					attributeType = xmlSchemaType;
				}
				else if (SchemaTypeName == XmlSchemaComplexType.AnyTypeName)
				{
					attributeType = XmlSchemaComplexType.AnyType;
				}
				else if (XmlSchemaUtil.IsBuiltInDatatypeName(SchemaTypeName))
				{
					attributeType = XmlSchemaDatatype.FromName(SchemaTypeName);
					if (attributeType == null)
					{
						error(h, "Invalid xml schema namespace datatype was specified.");
					}
				}
				else if (!schema.IsNamespaceAbsent(SchemaTypeName.Namespace))
				{
					error(h, string.Concat("Referenced schema type ", SchemaTypeName, " was not found in the corresponding schema."));
				}
			}
			if (RefName != null && RefName != XmlQualifiedName.Empty)
			{
				referencedAttribute = schema.FindAttribute(RefName);
				if (referencedAttribute != null)
				{
					errorCount += referencedAttribute.Validate(h, schema);
				}
				else if (!schema.IsNamespaceAbsent(RefName.Namespace))
				{
					error(h, string.Concat("Referenced attribute ", RefName, " was not found in the corresponding schema."));
				}
			}
			if (attributeType == null)
			{
				attributeType = XmlSchemaSimpleType.AnySimpleType;
			}
			if (defaultValue != null || fixedValue != null)
			{
				XmlSchemaDatatype xmlSchemaDatatype = attributeType as XmlSchemaDatatype;
				if (xmlSchemaDatatype == null)
				{
					xmlSchemaDatatype = ((XmlSchemaSimpleType)attributeType).Datatype;
				}
				if (xmlSchemaDatatype.TokenizedType == XmlTokenizedType.QName)
				{
					error(h, "By the defection of the W3C XML Schema specification, it is impossible to supply QName default or fixed values.");
				}
				else
				{
					try
					{
						if (defaultValue != null)
						{
							validatedDefaultValue = xmlSchemaDatatype.Normalize(defaultValue);
							xmlSchemaDatatype.ParseValue(validatedDefaultValue, null, null);
						}
					}
					catch (Exception innerException)
					{
						XmlSchemaObject.error(h, "The Attribute's default value is invalid with its type definition.", innerException);
					}
					try
					{
						if (fixedValue != null)
						{
							validatedFixedValue = xmlSchemaDatatype.Normalize(fixedValue);
							validatedFixedTypedValue = xmlSchemaDatatype.ParseValue(validatedFixedValue, null, null);
						}
					}
					catch (Exception innerException2)
					{
						XmlSchemaObject.error(h, "The Attribute's fixed value is invalid with its type definition.", innerException2);
					}
				}
			}
			if (Use == XmlSchemaUse.None)
			{
				validatedUse = XmlSchemaUse.Optional;
			}
			else
			{
				validatedUse = Use;
			}
			if (attributeType != null)
			{
				attributeSchemaType = attributeType as XmlSchemaSimpleType;
				if (attributeType == XmlSchemaSimpleType.AnySimpleType)
				{
					attributeSchemaType = XmlSchemaSimpleType.XsAnySimpleType;
				}
				if (attributeSchemaType == null)
				{
					attributeSchemaType = XmlSchemaType.GetBuiltInSimpleType(SchemaTypeName);
				}
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal bool AttributeEquals(XmlSchemaAttribute other)
		{
			if (base.Id != other.Id || QualifiedName != other.QualifiedName || AttributeType != other.AttributeType || ValidatedUse != other.ValidatedUse || ValidatedDefaultValue != other.ValidatedDefaultValue || ValidatedFixedValue != other.ValidatedFixedValue)
			{
				return false;
			}
			return true;
		}

		internal static XmlSchemaAttribute Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "attribute")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAttribute.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaAttribute.LineNumber = reader.LineNumber;
			xmlSchemaAttribute.LinePosition = reader.LinePosition;
			xmlSchemaAttribute.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "default")
				{
					xmlSchemaAttribute.defaultValue = reader.Value;
				}
				else if (reader.Name == "fixed")
				{
					xmlSchemaAttribute.fixedValue = reader.Value;
				}
				else if (reader.Name == "form")
				{
					Exception innerExcpetion;
					xmlSchemaAttribute.form = XmlSchemaUtil.ReadFormAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for form attribute", innerExcpetion);
					}
				}
				else if (reader.Name == "id")
				{
					xmlSchemaAttribute.Id = reader.Value;
				}
				else if (reader.Name == "name")
				{
					xmlSchemaAttribute.name = reader.Value;
				}
				else if (reader.Name == "ref")
				{
					Exception innerEx;
					xmlSchemaAttribute.refName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for ref attribute", innerEx);
					}
				}
				else if (reader.Name == "type")
				{
					Exception innerEx2;
					xmlSchemaAttribute.schemaTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx2);
					if (innerEx2 != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for type attribute", innerEx2);
					}
				}
				else if (reader.Name == "use")
				{
					Exception innerExcpetion2;
					xmlSchemaAttribute.use = XmlSchemaUtil.ReadUseAttribute(reader, out innerExcpetion2);
					if (innerExcpetion2 != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for use attribute", innerExcpetion2);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for attribute", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaAttribute);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaAttribute;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "attribute")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaAttribute.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaAttribute.Annotation = xmlSchemaAnnotation;
					}
				}
				else if (num <= 2 && reader.LocalName == "simpleType")
				{
					num = 3;
					XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaAttribute.schemaType = xmlSchemaSimpleType;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaAttribute;
		}
	}
}
