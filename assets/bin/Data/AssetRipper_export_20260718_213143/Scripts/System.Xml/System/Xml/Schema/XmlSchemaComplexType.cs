using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaComplexType : XmlSchemaType
	{
		private const string xmlname = "complexType";

		private XmlSchemaAnyAttribute anyAttribute;

		private XmlSchemaObjectCollection attributes;

		private XmlSchemaObjectTable attributeUses;

		private XmlSchemaAnyAttribute attributeWildcard;

		private XmlSchemaDerivationMethod block;

		private XmlSchemaDerivationMethod blockResolved;

		private XmlSchemaContentModel contentModel;

		private XmlSchemaParticle validatableParticle;

		private XmlSchemaParticle contentTypeParticle;

		private bool isAbstract;

		private bool isMixed;

		private XmlSchemaParticle particle;

		private XmlSchemaContentType resolvedContentType;

		internal bool ValidatedIsAbstract;

		private static XmlSchemaComplexType anyType;

		internal static readonly XmlQualifiedName AnyTypeName = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");

		private Guid CollectProcessId;

		internal bool ParentIsSchema
		{
			get
			{
				return base.Parent is XmlSchema;
			}
		}

		internal static XmlSchemaComplexType AnyType
		{
			get
			{
				if (anyType == null)
				{
					anyType = new XmlSchemaComplexType();
					anyType.Name = "anyType";
					anyType.QNameInternal = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
					if (XmlSchemaUtil.StrictMsCompliant)
					{
						anyType.validatableParticle = XmlSchemaParticle.Empty;
					}
					else
					{
						anyType.validatableParticle = XmlSchemaAny.AnyTypeContent;
					}
					anyType.contentTypeParticle = anyType.validatableParticle;
					anyType.DatatypeInternal = XmlSchemaSimpleType.AnySimpleType;
					anyType.isMixed = true;
					anyType.resolvedContentType = XmlSchemaContentType.Mixed;
				}
				return anyType;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute("abstract")]
		public bool IsAbstract
		{
			get
			{
				return isAbstract;
			}
			set
			{
				isAbstract = value;
			}
		}

		[DefaultValue(XmlSchemaDerivationMethod.None)]
		[XmlAttribute("block")]
		public XmlSchemaDerivationMethod Block
		{
			get
			{
				return block;
			}
			set
			{
				block = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute("mixed")]
		public override bool IsMixed
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

		[XmlElement("complexContent", typeof(XmlSchemaComplexContent))]
		[XmlElement("simpleContent", typeof(XmlSchemaSimpleContent))]
		public XmlSchemaContentModel ContentModel
		{
			get
			{
				return contentModel;
			}
			set
			{
				contentModel = value;
			}
		}

		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("group", typeof(XmlSchemaGroupRef))]
		[XmlElement("sequence", typeof(XmlSchemaSequence))]
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
				return anyAttribute;
			}
			set
			{
				anyAttribute = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaContentType ContentType
		{
			get
			{
				return resolvedContentType;
			}
		}

		[XmlIgnore]
		public XmlSchemaParticle ContentTypeParticle
		{
			get
			{
				return contentTypeParticle;
			}
		}

		[XmlIgnore]
		public XmlSchemaDerivationMethod BlockResolved
		{
			get
			{
				return blockResolved;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable AttributeUses
		{
			get
			{
				return attributeUses;
			}
		}

		[XmlIgnore]
		public XmlSchemaAnyAttribute AttributeWildcard
		{
			get
			{
				return attributeWildcard;
			}
		}

		internal XmlSchemaParticle ValidatableParticle
		{
			get
			{
				return contentTypeParticle;
			}
		}

		public XmlSchemaComplexType()
		{
			attributes = new XmlSchemaObjectCollection();
			block = XmlSchemaDerivationMethod.None;
			attributeUses = new XmlSchemaObjectTable();
			validatableParticle = XmlSchemaParticle.Empty;
			contentTypeParticle = validatableParticle;
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (ContentModel != null)
			{
				ContentModel.SetParent(this);
			}
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
				return errorCount;
			}
			ValidatedIsAbstract = isAbstract;
			attributeUses.Clear();
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
				if (ContentModel != null)
				{
					ContentModel.isRedefinedComponent = true;
				}
				if (Particle != null)
				{
					Particle.isRedefinedComponent = true;
				}
			}
			if (ParentIsSchema || isRedefineChild)
			{
				if (base.Name == null || base.Name == string.Empty)
				{
					error(h, "name must be present in a top level complex type");
				}
				else if (!XmlSchemaUtil.CheckNCName(base.Name))
				{
					error(h, "name must be a NCName");
				}
				else
				{
					QNameInternal = new XmlQualifiedName(base.Name, base.AncestorSchema.TargetNamespace);
				}
				if (Block != XmlSchemaDerivationMethod.None)
				{
					if (Block == XmlSchemaDerivationMethod.All)
					{
						blockResolved = XmlSchemaDerivationMethod.All;
					}
					else
					{
						if ((Block & XmlSchemaUtil.ComplexTypeBlockAllowed) != Block)
						{
							error(h, "Invalid block specification.");
						}
						blockResolved = Block & XmlSchemaUtil.ComplexTypeBlockAllowed;
					}
				}
				else
				{
					switch (schema.BlockDefault)
					{
					case XmlSchemaDerivationMethod.All:
						blockResolved = XmlSchemaDerivationMethod.All;
						break;
					case XmlSchemaDerivationMethod.None:
						blockResolved = XmlSchemaDerivationMethod.Empty;
						break;
					default:
						blockResolved = schema.BlockDefault & XmlSchemaUtil.ComplexTypeBlockAllowed;
						break;
					}
				}
				if (base.Final != XmlSchemaDerivationMethod.None)
				{
					if (base.Final == XmlSchemaDerivationMethod.All)
					{
						finalResolved = XmlSchemaDerivationMethod.All;
					}
					else if ((base.Final & XmlSchemaUtil.FinalAllowed) != base.Final)
					{
						error(h, "Invalid final specification.");
					}
					else
					{
						finalResolved = base.Final;
					}
				}
				else
				{
					switch (schema.FinalDefault)
					{
					case XmlSchemaDerivationMethod.All:
						finalResolved = XmlSchemaDerivationMethod.All;
						break;
					case XmlSchemaDerivationMethod.None:
						finalResolved = XmlSchemaDerivationMethod.Empty;
						break;
					default:
						finalResolved = schema.FinalDefault & XmlSchemaUtil.FinalAllowed;
						break;
					}
				}
			}
			else
			{
				if (isAbstract)
				{
					error(h, "abstract must be false in a local complex type");
				}
				if (base.Name != null)
				{
					error(h, "name must be absent in a local complex type");
				}
				if (base.Final != XmlSchemaDerivationMethod.None)
				{
					error(h, "final must be absent in a local complex type");
				}
				if (block != XmlSchemaDerivationMethod.None)
				{
					error(h, "block must be absent in a local complex type");
				}
			}
			if (contentModel != null)
			{
				if (anyAttribute != null || Attributes.Count != 0 || Particle != null)
				{
					error(h, "attributes, particles or anyattribute is not allowed if ContentModel is present");
				}
				errorCount += contentModel.Compile(h, schema);
				XmlSchemaSimpleContent xmlSchemaSimpleContent = ContentModel as XmlSchemaSimpleContent;
				if (xmlSchemaSimpleContent != null)
				{
					XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = xmlSchemaSimpleContent.Content as XmlSchemaSimpleContentExtension;
					if (xmlSchemaSimpleContentExtension == null)
					{
						XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction = xmlSchemaSimpleContent.Content as XmlSchemaSimpleContentRestriction;
						if (xmlSchemaSimpleContentRestriction != null && xmlSchemaSimpleContentRestriction.BaseType != null)
						{
							xmlSchemaSimpleContentRestriction.BaseType.Compile(h, schema);
							BaseXmlSchemaTypeInternal = xmlSchemaSimpleContentRestriction.BaseType;
						}
					}
				}
			}
			else
			{
				if (Particle != null)
				{
					errorCount += Particle.Compile(h, schema);
				}
				if (anyAttribute != null)
				{
					AnyAttribute.Compile(h, schema);
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
						error(h, string.Concat(attribute2.GetType(), " is not valid in this place::ComplexType"));
					}
				}
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		private void CollectSchemaComponent(ValidationEventHandler h, XmlSchema schema)
		{
			if (CollectProcessId == schema.CompilationId)
			{
				return;
			}
			if (contentModel != null)
			{
				BaseSchemaTypeName = ((contentModel.Content == null) ? XmlQualifiedName.Empty : contentModel.Content.GetBaseTypeName());
				BaseXmlSchemaTypeInternal = schema.FindSchemaType(BaseSchemaTypeName);
			}
			if (isRedefineChild && base.BaseXmlSchemaType != null && base.QualifiedName == BaseSchemaTypeName)
			{
				XmlSchemaType xmlSchemaType = (XmlSchemaType)redefinedObject;
				if (xmlSchemaType == null)
				{
					error(h, "Redefinition base type was not found.");
				}
				else
				{
					BaseXmlSchemaTypeInternal = xmlSchemaType;
				}
			}
			if (contentModel != null && contentModel.Content != null)
			{
				resolvedDerivedBy = ((!contentModel.Content.IsExtension) ? XmlSchemaDerivationMethod.Restriction : XmlSchemaDerivationMethod.Extension);
			}
			else
			{
				resolvedDerivedBy = XmlSchemaDerivationMethod.Empty;
			}
		}

		private void FillContentTypeParticle(ValidationEventHandler h, XmlSchema schema)
		{
			if (ContentModel != null)
			{
				CollectContentTypeFromContentModel(h, schema);
			}
			else
			{
				CollectContentTypeFromImmediateContent();
			}
			contentTypeParticle = validatableParticle.GetOptimizedParticle(true);
			if (contentTypeParticle == XmlSchemaParticle.Empty && resolvedContentType == XmlSchemaContentType.ElementOnly)
			{
				resolvedContentType = XmlSchemaContentType.Empty;
			}
			CollectProcessId = schema.CompilationId;
		}

		private void CollectContentTypeFromImmediateContent()
		{
			if (Particle != null)
			{
				validatableParticle = Particle;
			}
			if (this == AnyType)
			{
				resolvedContentType = XmlSchemaContentType.Mixed;
				return;
			}
			if (validatableParticle == XmlSchemaParticle.Empty)
			{
				if (IsMixed)
				{
					resolvedContentType = XmlSchemaContentType.TextOnly;
				}
				else
				{
					resolvedContentType = XmlSchemaContentType.Empty;
				}
			}
			else if (IsMixed)
			{
				resolvedContentType = XmlSchemaContentType.Mixed;
			}
			else
			{
				resolvedContentType = XmlSchemaContentType.ElementOnly;
			}
			if (this != AnyType)
			{
				BaseXmlSchemaTypeInternal = AnyType;
			}
		}

		private void CollectContentTypeFromContentModel(ValidationEventHandler h, XmlSchema schema)
		{
			if (ContentModel.Content == null)
			{
				validatableParticle = XmlSchemaParticle.Empty;
				resolvedContentType = XmlSchemaContentType.Empty;
				return;
			}
			if (ContentModel.Content is XmlSchemaComplexContentExtension)
			{
				CollectContentTypeFromComplexExtension(h, schema);
			}
			if (ContentModel.Content is XmlSchemaComplexContentRestriction)
			{
				CollectContentTypeFromComplexRestriction();
			}
		}

		private void CollectContentTypeFromComplexExtension(ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = (XmlSchemaComplexContentExtension)ContentModel.Content;
			XmlSchemaComplexType xmlSchemaComplexType = base.BaseXmlSchemaType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null)
			{
				xmlSchemaComplexType.CollectSchemaComponent(h, schema);
			}
			if (BaseSchemaTypeName == AnyTypeName)
			{
				xmlSchemaComplexType = AnyType;
			}
			if (xmlSchemaComplexType == null)
			{
				validatableParticle = XmlSchemaParticle.Empty;
				resolvedContentType = XmlSchemaContentType.Empty;
				return;
			}
			if (xmlSchemaComplexContentExtension.Particle == null || xmlSchemaComplexContentExtension.Particle == XmlSchemaParticle.Empty)
			{
				if (xmlSchemaComplexType == null)
				{
					validatableParticle = XmlSchemaParticle.Empty;
					resolvedContentType = XmlSchemaContentType.Empty;
				}
				else
				{
					validatableParticle = xmlSchemaComplexType.ValidatableParticle;
					resolvedContentType = xmlSchemaComplexType.resolvedContentType;
					if (resolvedContentType == XmlSchemaContentType.Empty)
					{
						resolvedContentType = GetComplexContentType(contentModel);
					}
				}
			}
			else if (xmlSchemaComplexType.validatableParticle == XmlSchemaParticle.Empty || xmlSchemaComplexType == AnyType)
			{
				validatableParticle = xmlSchemaComplexContentExtension.Particle;
				resolvedContentType = GetComplexContentType(contentModel);
			}
			else
			{
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				CopyInfo(xmlSchemaSequence);
				xmlSchemaSequence.Items.Add(xmlSchemaComplexType.validatableParticle);
				xmlSchemaSequence.Items.Add(xmlSchemaComplexContentExtension.Particle);
				xmlSchemaSequence.Compile(h, schema);
				xmlSchemaSequence.Validate(h, schema);
				validatableParticle = xmlSchemaSequence;
				resolvedContentType = GetComplexContentType(contentModel);
			}
			if (validatableParticle == null)
			{
				validatableParticle = XmlSchemaParticle.Empty;
			}
		}

		private void CollectContentTypeFromComplexRestriction()
		{
			XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = (XmlSchemaComplexContentRestriction)ContentModel.Content;
			bool flag = false;
			if (xmlSchemaComplexContentRestriction.Particle == null)
			{
				flag = true;
			}
			else
			{
				XmlSchemaGroupBase xmlSchemaGroupBase = xmlSchemaComplexContentRestriction.Particle as XmlSchemaGroupBase;
				if (xmlSchemaGroupBase != null)
				{
					if (!(xmlSchemaGroupBase is XmlSchemaChoice) && xmlSchemaGroupBase.Items.Count == 0)
					{
						flag = true;
					}
					else if (xmlSchemaGroupBase is XmlSchemaChoice && xmlSchemaGroupBase.Items.Count == 0 && xmlSchemaGroupBase.ValidatedMinOccurs == 0m)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				resolvedContentType = XmlSchemaContentType.Empty;
				validatableParticle = XmlSchemaParticle.Empty;
			}
			else
			{
				resolvedContentType = GetComplexContentType(contentModel);
				validatableParticle = xmlSchemaComplexContentRestriction.Particle;
			}
		}

		private XmlSchemaContentType GetComplexContentType(XmlSchemaContentModel content)
		{
			if (IsMixed || ((XmlSchemaComplexContent)content).IsMixed)
			{
				return XmlSchemaContentType.Mixed;
			}
			return XmlSchemaContentType.ElementOnly;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			ValidationId = schema.ValidationId;
			CollectSchemaComponent(h, schema);
			ValidateContentFirstPass(h, schema);
			FillContentTypeParticle(h, schema);
			if (ContentModel != null)
			{
				ValidateContentModel(h, schema);
			}
			else
			{
				ValidateImmediateAttributes(h, schema);
			}
			if (ContentTypeParticle != null)
			{
				XmlSchemaAll xmlSchemaAll = contentTypeParticle.GetOptimizedParticle(true) as XmlSchemaAll;
				if (xmlSchemaAll != null && (xmlSchemaAll.ValidatedMaxOccurs != 1m || contentTypeParticle.ValidatedMaxOccurs != 1m))
				{
					error(h, "Particle whose term is -all- and consists of complex type content particle must have maxOccurs = 1.");
				}
			}
			if (schema.Schemas.CompilationSettings != null && schema.Schemas.CompilationSettings.EnableUpaCheck)
			{
				contentTypeParticle.ValidateUniqueParticleAttribution(new XmlSchemaObjectTable(), new ArrayList(), h, schema);
			}
			contentTypeParticle.ValidateUniqueTypeAttribution(new XmlSchemaObjectTable(), h, schema);
			XmlSchemaAttribute xmlSchemaAttribute = null;
			foreach (DictionaryEntry attributeUse in attributeUses)
			{
				XmlSchemaAttribute xmlSchemaAttribute2 = (XmlSchemaAttribute)attributeUse.Value;
				XmlSchemaDatatype xmlSchemaDatatype = xmlSchemaAttribute2.AttributeType as XmlSchemaDatatype;
				if (xmlSchemaDatatype != null && xmlSchemaDatatype.TokenizedType != XmlTokenizedType.ID)
				{
					continue;
				}
				if (xmlSchemaDatatype == null)
				{
					xmlSchemaDatatype = ((XmlSchemaSimpleType)xmlSchemaAttribute2.AttributeType).Datatype;
				}
				if (xmlSchemaDatatype != null && xmlSchemaDatatype.TokenizedType == XmlTokenizedType.ID)
				{
					if (xmlSchemaAttribute != null)
					{
						error(h, "Two or more ID typed attribute declarations in a complex type are found.");
					}
					else
					{
						xmlSchemaAttribute = xmlSchemaAttribute2;
					}
				}
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		private void ValidateImmediateAttributes(ValidationEventHandler h, XmlSchema schema)
		{
			attributeUses = new XmlSchemaObjectTable();
			XmlSchemaUtil.ValidateAttributesResolved(attributeUses, h, schema, attributes, anyAttribute, ref attributeWildcard, null, false);
		}

		private void ValidateContentFirstPass(ValidationEventHandler h, XmlSchema schema)
		{
			if (ContentModel != null)
			{
				errorCount += contentModel.Validate(h, schema);
				if (BaseXmlSchemaTypeInternal != null)
				{
					errorCount += BaseXmlSchemaTypeInternal.Validate(h, schema);
				}
			}
			else
			{
				if (Particle == null)
				{
					return;
				}
				errorCount += particle.Validate(h, schema);
				XmlSchemaGroupRef xmlSchemaGroupRef = Particle as XmlSchemaGroupRef;
				if (xmlSchemaGroupRef != null)
				{
					if (xmlSchemaGroupRef.TargetGroup != null)
					{
						errorCount += xmlSchemaGroupRef.TargetGroup.Validate(h, schema);
					}
					else if (!schema.IsNamespaceAbsent(xmlSchemaGroupRef.RefName.Namespace))
					{
						error(h, string.Concat("Referenced group ", xmlSchemaGroupRef.RefName, " was not found in the corresponding schema."));
					}
				}
			}
		}

		private void ValidateContentModel(ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaType baseXmlSchemaTypeInternal = BaseXmlSchemaTypeInternal;
			XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = contentModel.Content as XmlSchemaComplexContentExtension;
			XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = contentModel.Content as XmlSchemaComplexContentRestriction;
			XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = contentModel.Content as XmlSchemaSimpleContentExtension;
			XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction = contentModel.Content as XmlSchemaSimpleContentRestriction;
			XmlSchemaAnyAttribute xmlSchemaAnyAttribute = null;
			XmlSchemaAnyAttribute xmlSchemaAnyAttribute2 = null;
			if (ValidateRecursionCheck())
			{
				error(h, "Circular definition of schema types was found.");
			}
			if (baseXmlSchemaTypeInternal != null)
			{
				DatatypeInternal = baseXmlSchemaTypeInternal.Datatype;
			}
			else if (BaseSchemaTypeName == AnyTypeName)
			{
				DatatypeInternal = XmlSchemaSimpleType.AnySimpleType;
			}
			else if (XmlSchemaUtil.IsBuiltInDatatypeName(BaseSchemaTypeName))
			{
				DatatypeInternal = XmlSchemaDatatype.FromName(BaseSchemaTypeName);
			}
			XmlSchemaComplexType xmlSchemaComplexType = baseXmlSchemaTypeInternal as XmlSchemaComplexType;
			XmlSchemaSimpleType xmlSchemaSimpleType = baseXmlSchemaTypeInternal as XmlSchemaSimpleType;
			if (baseXmlSchemaTypeInternal != null && (baseXmlSchemaTypeInternal.FinalResolved & resolvedDerivedBy) != XmlSchemaDerivationMethod.Empty)
			{
				error(h, "Specified derivation is specified as final by derived schema type.");
			}
			if (xmlSchemaSimpleType != null && resolvedDerivedBy == XmlSchemaDerivationMethod.Restriction)
			{
				error(h, "If the base schema type is a simple type, then this type must be extension.");
			}
			if (xmlSchemaComplexContentExtension != null || xmlSchemaComplexContentRestriction != null)
			{
				if (BaseSchemaTypeName == AnyTypeName)
				{
					xmlSchemaComplexType = AnyType;
				}
				else if (XmlSchemaUtil.IsBuiltInDatatypeName(BaseSchemaTypeName))
				{
					error(h, "Referenced base schema type is XML Schema datatype.");
				}
				else if (xmlSchemaComplexType == null && !schema.IsNamespaceAbsent(BaseSchemaTypeName.Namespace))
				{
					error(h, string.Concat("Referenced base schema type ", BaseSchemaTypeName, " was not complex type or not found in the corresponding schema."));
				}
			}
			else
			{
				resolvedContentType = XmlSchemaContentType.TextOnly;
				if (BaseSchemaTypeName == AnyTypeName)
				{
					xmlSchemaComplexType = AnyType;
				}
				if (xmlSchemaComplexType != null && xmlSchemaComplexType.ContentType != XmlSchemaContentType.TextOnly)
				{
					error(h, "Base schema complex type of a simple content must be simple content type. Base type is " + BaseSchemaTypeName);
				}
				else if (xmlSchemaSimpleContentExtension == null && xmlSchemaSimpleType != null && BaseSchemaTypeName.Namespace != "http://www.w3.org/2001/XMLSchema")
				{
					error(h, "If a simple content is not an extension, base schema type must be complex type. Base type is " + BaseSchemaTypeName);
				}
				else if (!XmlSchemaUtil.IsBuiltInDatatypeName(BaseSchemaTypeName) && baseXmlSchemaTypeInternal == null && !schema.IsNamespaceAbsent(BaseSchemaTypeName.Namespace))
				{
					error(h, string.Concat("Referenced base schema type ", BaseSchemaTypeName, " was not found in the corresponding schema."));
				}
				if (xmlSchemaComplexType != null)
				{
					if (xmlSchemaComplexType.ContentType != XmlSchemaContentType.TextOnly && (xmlSchemaSimpleContentRestriction == null || xmlSchemaComplexType.ContentType != XmlSchemaContentType.Mixed || xmlSchemaComplexType.Particle == null || !xmlSchemaComplexType.Particle.ValidateIsEmptiable() || xmlSchemaSimpleContentRestriction.BaseType == null))
					{
						error(h, "Base complex type of a simple content restriction must be text only.");
					}
				}
				else if (xmlSchemaSimpleContentExtension == null || xmlSchemaComplexType != null)
				{
					error(h, "Not allowed base type of a simple content restriction.");
				}
			}
			if (xmlSchemaComplexContentExtension != null)
			{
				xmlSchemaAnyAttribute = xmlSchemaComplexContentExtension.AnyAttribute;
				if (xmlSchemaComplexType != null)
				{
					foreach (DictionaryEntry attributeUse in xmlSchemaComplexType.AttributeUses)
					{
						XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)attributeUse.Value;
						XmlSchemaUtil.AddToTable(attributeUses, xmlSchemaAttribute, xmlSchemaAttribute.QualifiedName, h);
					}
					xmlSchemaAnyAttribute2 = xmlSchemaComplexType.AttributeWildcard;
				}
				errorCount += XmlSchemaUtil.ValidateAttributesResolved(attributeUses, h, schema, xmlSchemaComplexContentExtension.Attributes, xmlSchemaComplexContentExtension.AnyAttribute, ref attributeWildcard, null, true);
				if (xmlSchemaComplexType != null)
				{
					ValidateComplexBaseDerivationValidExtension(xmlSchemaComplexType, h, schema);
				}
				else if (xmlSchemaSimpleType != null)
				{
					ValidateSimpleBaseDerivationValidExtension(xmlSchemaSimpleType, h, schema);
				}
			}
			if (xmlSchemaComplexContentRestriction != null)
			{
				if (xmlSchemaComplexType == null)
				{
					xmlSchemaComplexType = AnyType;
				}
				xmlSchemaAnyAttribute = (attributeWildcard = xmlSchemaComplexContentRestriction.AnyAttribute);
				if (xmlSchemaComplexType != null)
				{
					xmlSchemaAnyAttribute2 = xmlSchemaComplexType.AttributeWildcard;
				}
				if (xmlSchemaAnyAttribute2 != null && xmlSchemaAnyAttribute != null)
				{
					xmlSchemaAnyAttribute.ValidateWildcardSubset(xmlSchemaAnyAttribute2, h, schema);
				}
				errorCount += XmlSchemaUtil.ValidateAttributesResolved(attributeUses, h, schema, xmlSchemaComplexContentRestriction.Attributes, xmlSchemaComplexContentRestriction.AnyAttribute, ref attributeWildcard, null, false);
				foreach (DictionaryEntry attributeUse2 in xmlSchemaComplexType.AttributeUses)
				{
					XmlSchemaAttribute xmlSchemaAttribute2 = (XmlSchemaAttribute)attributeUse2.Value;
					if (attributeUses[xmlSchemaAttribute2.QualifiedName] == null)
					{
						XmlSchemaUtil.AddToTable(attributeUses, xmlSchemaAttribute2, xmlSchemaAttribute2.QualifiedName, h);
					}
				}
				ValidateDerivationValidRestriction(xmlSchemaComplexType, h, schema);
			}
			if (xmlSchemaSimpleContentExtension != null)
			{
				errorCount += XmlSchemaUtil.ValidateAttributesResolved(attributeUses, h, schema, xmlSchemaSimpleContentExtension.Attributes, xmlSchemaSimpleContentExtension.AnyAttribute, ref attributeWildcard, null, true);
				xmlSchemaAnyAttribute = xmlSchemaSimpleContentExtension.AnyAttribute;
				if (xmlSchemaComplexType != null)
				{
					xmlSchemaAnyAttribute2 = xmlSchemaComplexType.AttributeWildcard;
					foreach (DictionaryEntry attributeUse3 in xmlSchemaComplexType.AttributeUses)
					{
						XmlSchemaAttribute xmlSchemaAttribute3 = (XmlSchemaAttribute)attributeUse3.Value;
						XmlSchemaUtil.AddToTable(attributeUses, xmlSchemaAttribute3, xmlSchemaAttribute3.QualifiedName, h);
					}
				}
				if (xmlSchemaAnyAttribute2 != null && xmlSchemaAnyAttribute != null)
				{
					xmlSchemaAnyAttribute.ValidateWildcardSubset(xmlSchemaAnyAttribute2, h, schema);
				}
			}
			if (xmlSchemaSimpleContentRestriction != null)
			{
				xmlSchemaAnyAttribute2 = ((xmlSchemaComplexType == null) ? null : xmlSchemaComplexType.AttributeWildcard);
				xmlSchemaAnyAttribute = xmlSchemaSimpleContentRestriction.AnyAttribute;
				if (xmlSchemaAnyAttribute != null && xmlSchemaAnyAttribute2 != null)
				{
					xmlSchemaAnyAttribute.ValidateWildcardSubset(xmlSchemaAnyAttribute2, h, schema);
				}
				errorCount += XmlSchemaUtil.ValidateAttributesResolved(attributeUses, h, schema, xmlSchemaSimpleContentRestriction.Attributes, xmlSchemaSimpleContentRestriction.AnyAttribute, ref attributeWildcard, null, false);
			}
			if (xmlSchemaAnyAttribute != null)
			{
				attributeWildcard = xmlSchemaAnyAttribute;
			}
			else
			{
				attributeWildcard = xmlSchemaAnyAttribute2;
			}
		}

		internal void ValidateTypeDerivationOK(object b, ValidationEventHandler h, XmlSchema schema)
		{
			if (this == AnyType && base.BaseXmlSchemaType == this)
			{
				return;
			}
			XmlSchemaType xmlSchemaType = b as XmlSchemaType;
			if (b == this)
			{
				return;
			}
			if (xmlSchemaType != null && (resolvedDerivedBy & xmlSchemaType.FinalResolved) != XmlSchemaDerivationMethod.Empty)
			{
				error(h, string.Concat("Derivation type ", resolvedDerivedBy, " is prohibited by the base type."));
			}
			if (base.BaseSchemaType == b)
			{
				return;
			}
			if (base.BaseSchemaType == null || base.BaseXmlSchemaType == AnyType)
			{
				error(h, "Derived type's base schema type is anyType.");
				return;
			}
			XmlSchemaComplexType xmlSchemaComplexType = base.BaseXmlSchemaType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null)
			{
				xmlSchemaComplexType.ValidateTypeDerivationOK(b, h, schema);
				return;
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = base.BaseXmlSchemaType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				xmlSchemaSimpleType.ValidateTypeDerivationOK(b, h, schema, true);
			}
		}

		internal void ValidateComplexBaseDerivationValidExtension(XmlSchemaComplexType baseComplexType, ValidationEventHandler h, XmlSchema schema)
		{
			if ((baseComplexType.FinalResolved & XmlSchemaDerivationMethod.Extension) != XmlSchemaDerivationMethod.Empty)
			{
				error(h, "Derivation by extension is prohibited.");
			}
			foreach (DictionaryEntry attributeUse in baseComplexType.AttributeUses)
			{
				XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)attributeUse.Value;
				XmlSchemaAttribute xmlSchemaAttribute2 = AttributeUses[xmlSchemaAttribute.QualifiedName] as XmlSchemaAttribute;
				if (xmlSchemaAttribute2 == null)
				{
					error(h, string.Concat("Invalid complex type derivation by extension was found. Missing attribute was found: ", xmlSchemaAttribute.QualifiedName, " ."));
				}
			}
			if (AnyAttribute != null)
			{
				if (baseComplexType.AnyAttribute == null)
				{
					error(h, "Invalid complex type derivation by extension was found. Base complex type does not have an attribute wildcard.");
				}
				else
				{
					baseComplexType.AnyAttribute.ValidateWildcardSubset(AnyAttribute, h, schema);
				}
			}
			if (baseComplexType.ContentType == XmlSchemaContentType.Empty)
			{
				return;
			}
			if (ContentType != baseComplexType.ContentType)
			{
				error(h, string.Concat("Base complex type has different content type ", baseComplexType.ContentType, "."));
			}
			else if (contentTypeParticle == null || !contentTypeParticle.ParticleEquals(baseComplexType.ContentTypeParticle))
			{
				XmlSchemaSequence xmlSchemaSequence = contentTypeParticle as XmlSchemaSequence;
				if (contentTypeParticle != XmlSchemaParticle.Empty && (xmlSchemaSequence == null || contentTypeParticle.ValidatedMinOccurs != 1m || contentTypeParticle.ValidatedMaxOccurs != 1m))
				{
					error(h, "Invalid complex content extension was found.");
				}
			}
		}

		internal void ValidateSimpleBaseDerivationValidExtension(object baseType, ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = baseType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null && (xmlSchemaSimpleType.FinalResolved & XmlSchemaDerivationMethod.Extension) != XmlSchemaDerivationMethod.Empty)
			{
				error(h, "Extension is prohibited by the base type.");
			}
			XmlSchemaDatatype xmlSchemaDatatype = baseType as XmlSchemaDatatype;
			if (xmlSchemaDatatype == null)
			{
				xmlSchemaDatatype = xmlSchemaSimpleType.Datatype;
			}
			if (xmlSchemaDatatype != base.Datatype)
			{
				error(h, "To extend simple type, a complex type must have the same content type as the base type.");
			}
		}

		internal void ValidateDerivationValidRestriction(XmlSchemaComplexType baseType, ValidationEventHandler h, XmlSchema schema)
		{
			if (baseType == null)
			{
				error(h, "Base schema type is not a complex type.");
				return;
			}
			if ((baseType.FinalResolved & XmlSchemaDerivationMethod.Restriction) != XmlSchemaDerivationMethod.Empty)
			{
				error(h, "Prohibited derivation by restriction by base schema type.");
				return;
			}
			foreach (DictionaryEntry attributeUse in AttributeUses)
			{
				XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)attributeUse.Value;
				XmlSchemaAttribute xmlSchemaAttribute2 = baseType.AttributeUses[xmlSchemaAttribute.QualifiedName] as XmlSchemaAttribute;
				if (xmlSchemaAttribute2 != null)
				{
					if (xmlSchemaAttribute2.ValidatedUse != XmlSchemaUse.Optional && xmlSchemaAttribute.ValidatedUse != XmlSchemaUse.Required)
					{
						error(h, string.Concat("Invalid attribute derivation by restriction was found for ", xmlSchemaAttribute.QualifiedName, " ."));
					}
					XmlSchemaSimpleType xmlSchemaSimpleType = xmlSchemaAttribute.AttributeType as XmlSchemaSimpleType;
					XmlSchemaSimpleType xmlSchemaSimpleType2 = xmlSchemaAttribute2.AttributeType as XmlSchemaSimpleType;
					bool flag = false;
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaSimpleType.ValidateDerivationValid(xmlSchemaSimpleType2, null, h, schema);
					}
					else if (xmlSchemaSimpleType == null && xmlSchemaSimpleType2 != null)
					{
						flag = true;
					}
					else
					{
						Type type = xmlSchemaAttribute.AttributeType.GetType();
						Type type2 = xmlSchemaAttribute2.AttributeType.GetType();
						if (type != type2 && type.IsSubclassOf(type2))
						{
							flag = true;
						}
					}
					if (flag)
					{
						error(h, string.Concat("Invalid attribute derivation by restriction because of its type: ", xmlSchemaAttribute.QualifiedName, " ."));
					}
					if (xmlSchemaAttribute2.ValidatedFixedValue != null && xmlSchemaAttribute.ValidatedFixedValue != xmlSchemaAttribute2.ValidatedFixedValue)
					{
						error(h, string.Concat("Invalid attribute derivation by restriction because of its fixed value constraint: ", xmlSchemaAttribute.QualifiedName, " ."));
					}
				}
				else if (baseType.AttributeWildcard != null && !baseType.AttributeWildcard.ValidateWildcardAllowsNamespaceName(xmlSchemaAttribute.QualifiedName.Namespace, schema) && !schema.IsNamespaceAbsent(xmlSchemaAttribute.QualifiedName.Namespace))
				{
					error(h, string.Concat("Invalid attribute derivation by restriction was found for ", xmlSchemaAttribute.QualifiedName, " ."));
				}
			}
			if (AttributeWildcard != null && baseType != AnyType)
			{
				if (baseType.AttributeWildcard == null)
				{
					error(h, "Invalid attribute derivation by restriction because of attribute wildcard.");
				}
				else
				{
					AttributeWildcard.ValidateWildcardSubset(baseType.AttributeWildcard, h, schema);
				}
			}
			if (this == AnyType)
			{
				return;
			}
			if (contentTypeParticle == XmlSchemaParticle.Empty)
			{
				if (ContentType != XmlSchemaContentType.Empty)
				{
					if (baseType.ContentType == XmlSchemaContentType.Mixed && !baseType.ContentTypeParticle.ValidateIsEmptiable())
					{
						error(h, "Invalid content type derivation.");
					}
				}
				else if (baseType.ContentTypeParticle != XmlSchemaParticle.Empty && !baseType.ContentTypeParticle.ValidateIsEmptiable())
				{
					error(h, "Invalid content type derivation.");
				}
			}
			else if (baseType.ContentTypeParticle != null && !contentTypeParticle.ParticleEquals(baseType.ContentTypeParticle))
			{
				contentTypeParticle.ValidateDerivationByRestriction(baseType.ContentTypeParticle, h, schema, true);
			}
		}

		internal static XmlSchemaComplexType Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "complexType")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaComplexType.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaComplexType.LineNumber = reader.LineNumber;
			xmlSchemaComplexType.LinePosition = reader.LinePosition;
			xmlSchemaComplexType.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				Exception innerExcpetion;
				if (reader.Name == "abstract")
				{
					xmlSchemaComplexType.IsAbstract = XmlSchemaUtil.ReadBoolAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is invalid value for abstract", innerExcpetion);
					}
				}
				else if (reader.Name == "block")
				{
					xmlSchemaComplexType.block = XmlSchemaUtil.ReadDerivationAttribute(reader, out innerExcpetion, "block", XmlSchemaUtil.ComplexTypeBlockAllowed);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, "some invalid values for block attribute were found", innerExcpetion);
					}
				}
				else if (reader.Name == "final")
				{
					xmlSchemaComplexType.Final = XmlSchemaUtil.ReadDerivationAttribute(reader, out innerExcpetion, "final", XmlSchemaUtil.FinalAllowed);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, "some invalid values for final attribute were found", innerExcpetion);
					}
				}
				else if (reader.Name == "id")
				{
					xmlSchemaComplexType.Id = reader.Value;
				}
				else if (reader.Name == "mixed")
				{
					xmlSchemaComplexType.isMixed = XmlSchemaUtil.ReadBoolAttribute(reader, out innerExcpetion);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is invalid value for mixed", innerExcpetion);
					}
				}
				else if (reader.Name == "name")
				{
					xmlSchemaComplexType.Name = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for complexType", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaComplexType);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaComplexType;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "complexType")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaComplexType.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaComplexType.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "simpleContent")
					{
						num = 6;
						XmlSchemaSimpleContent xmlSchemaSimpleContent = XmlSchemaSimpleContent.Read(reader, h);
						if (xmlSchemaSimpleContent != null)
						{
							xmlSchemaComplexType.ContentModel = xmlSchemaSimpleContent;
						}
						continue;
					}
					if (reader.LocalName == "complexContent")
					{
						num = 6;
						XmlSchemaComplexContent xmlSchemaComplexContent = XmlSchemaComplexContent.Read(reader, h);
						if (xmlSchemaComplexContent != null)
						{
							xmlSchemaComplexType.contentModel = xmlSchemaComplexContent;
						}
						continue;
					}
				}
				if (num <= 3)
				{
					if (reader.LocalName == "group")
					{
						num = 4;
						XmlSchemaGroupRef xmlSchemaGroupRef = XmlSchemaGroupRef.Read(reader, h);
						if (xmlSchemaGroupRef != null)
						{
							xmlSchemaComplexType.particle = xmlSchemaGroupRef;
						}
						continue;
					}
					if (reader.LocalName == "all")
					{
						num = 4;
						XmlSchemaAll xmlSchemaAll = XmlSchemaAll.Read(reader, h);
						if (xmlSchemaAll != null)
						{
							xmlSchemaComplexType.particle = xmlSchemaAll;
						}
						continue;
					}
					if (reader.LocalName == "choice")
					{
						num = 4;
						XmlSchemaChoice xmlSchemaChoice = XmlSchemaChoice.Read(reader, h);
						if (xmlSchemaChoice != null)
						{
							xmlSchemaComplexType.particle = xmlSchemaChoice;
						}
						continue;
					}
					if (reader.LocalName == "sequence")
					{
						num = 4;
						XmlSchemaSequence xmlSchemaSequence = XmlSchemaSequence.Read(reader, h);
						if (xmlSchemaSequence != null)
						{
							xmlSchemaComplexType.particle = xmlSchemaSequence;
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
							xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
						}
						continue;
					}
					if (reader.LocalName == "attributeGroup")
					{
						num = 4;
						XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = XmlSchemaAttributeGroupRef.Read(reader, h);
						if (xmlSchemaAttributeGroupRef != null)
						{
							xmlSchemaComplexType.attributes.Add(xmlSchemaAttributeGroupRef);
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
						xmlSchemaComplexType.AnyAttribute = xmlSchemaAnyAttribute;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaComplexType;
		}
	}
}
