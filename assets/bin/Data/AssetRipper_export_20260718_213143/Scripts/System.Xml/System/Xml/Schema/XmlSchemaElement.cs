using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaElement : XmlSchemaParticle
	{
		private const string xmlname = "element";

		private XmlSchemaDerivationMethod block;

		private XmlSchemaObjectCollection constraints;

		private string defaultValue;

		private object elementType;

		private XmlSchemaType elementSchemaType;

		private XmlSchemaDerivationMethod final;

		private string fixedValue;

		private XmlSchemaForm form;

		private bool isAbstract;

		private bool isNillable;

		private string name;

		private XmlQualifiedName refName;

		private XmlSchemaType schemaType;

		private XmlQualifiedName schemaTypeName;

		private XmlQualifiedName substitutionGroup;

		private XmlSchema schema;

		internal bool parentIsSchema;

		private XmlQualifiedName qName;

		private XmlSchemaDerivationMethod blockResolved;

		private XmlSchemaDerivationMethod finalResolved;

		private XmlSchemaElement referencedElement;

		private ArrayList substitutingElements = new ArrayList();

		private XmlSchemaElement substitutionGroupElement;

		private bool actualIsAbstract;

		private bool actualIsNillable;

		private string validatedDefaultValue;

		private string validatedFixedValue;

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

		[XmlAttribute("block")]
		[DefaultValue(XmlSchemaDerivationMethod.None)]
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

		[XmlAttribute("default")]
		[DefaultValue(null)]
		public string DefaultValue
		{
			get
			{
				return defaultValue;
			}
			set
			{
				defaultValue = value;
			}
		}

		[XmlAttribute("final")]
		[DefaultValue(XmlSchemaDerivationMethod.None)]
		public XmlSchemaDerivationMethod Final
		{
			get
			{
				return final;
			}
			set
			{
				final = value;
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
				fixedValue = value;
			}
		}

		[XmlAttribute("form")]
		[DefaultValue(XmlSchemaForm.None)]
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
		[DefaultValue("")]
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

		[XmlAttribute("nillable")]
		[DefaultValue(false)]
		public bool IsNillable
		{
			get
			{
				return isNillable;
			}
			set
			{
				isNillable = value;
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

		[XmlAttribute("substitutionGroup")]
		public XmlQualifiedName SubstitutionGroup
		{
			get
			{
				return substitutionGroup;
			}
			set
			{
				substitutionGroup = value;
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

		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		[XmlElement("complexType", typeof(XmlSchemaComplexType))]
		public XmlSchemaType SchemaType
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

		[XmlElement("keyref", typeof(XmlSchemaKeyref))]
		[XmlElement("unique", typeof(XmlSchemaUnique))]
		[XmlElement("key", typeof(XmlSchemaKey))]
		public XmlSchemaObjectCollection Constraints
		{
			get
			{
				return constraints;
			}
		}

		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return qName;
			}
		}

		[XmlIgnore]
		[Obsolete]
		public object ElementType
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.ElementType;
				}
				return elementType;
			}
		}

		[XmlIgnore]
		public XmlSchemaType ElementSchemaType
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.ElementSchemaType;
				}
				return elementSchemaType;
			}
		}

		[XmlIgnore]
		public XmlSchemaDerivationMethod BlockResolved
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.BlockResolved;
				}
				return blockResolved;
			}
		}

		[XmlIgnore]
		public XmlSchemaDerivationMethod FinalResolved
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.FinalResolved;
				}
				return finalResolved;
			}
		}

		internal bool ActualIsNillable
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.ActualIsNillable;
				}
				return actualIsNillable;
			}
		}

		internal bool ActualIsAbstract
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.ActualIsAbstract;
				}
				return actualIsAbstract;
			}
		}

		internal string ValidatedDefaultValue
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.ValidatedDefaultValue;
				}
				return validatedDefaultValue;
			}
		}

		internal string ValidatedFixedValue
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.ValidatedFixedValue;
				}
				return validatedFixedValue;
			}
		}

		internal ArrayList SubstitutingElements
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.SubstitutingElements;
				}
				return substitutingElements;
			}
		}

		internal XmlSchemaElement SubstitutionGroupElement
		{
			get
			{
				if (referencedElement != null)
				{
					return referencedElement.SubstitutionGroupElement;
				}
				return substitutionGroupElement;
			}
		}

		public XmlSchemaElement()
		{
			block = XmlSchemaDerivationMethod.None;
			final = XmlSchemaDerivationMethod.None;
			constraints = new XmlSchemaObjectCollection();
			refName = XmlQualifiedName.Empty;
			schemaTypeName = XmlQualifiedName.Empty;
			substitutionGroup = XmlQualifiedName.Empty;
			InitPostCompileInformations();
		}

		private void InitPostCompileInformations()
		{
			qName = XmlQualifiedName.Empty;
			schema = null;
			blockResolved = XmlSchemaDerivationMethod.None;
			finalResolved = XmlSchemaDerivationMethod.None;
			referencedElement = null;
			substitutingElements.Clear();
			substitutionGroupElement = null;
			actualIsAbstract = false;
			actualIsNillable = false;
			validatedDefaultValue = null;
			validatedFixedValue = null;
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (SchemaType != null)
			{
				SchemaType.SetParent(this);
			}
			foreach (XmlSchemaObject constraint in Constraints)
			{
				constraint.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			InitPostCompileInformations();
			this.schema = schema;
			if (defaultValue != null && fixedValue != null)
			{
				error(h, "both default and fixed can't be present");
			}
			if (parentIsSchema || isRedefineChild)
			{
				if (refName != null && !RefName.IsEmpty)
				{
					error(h, "ref must be absent");
				}
				if (name == null)
				{
					error(h, "Required attribute name must be present");
				}
				else if (!XmlSchemaUtil.CheckNCName(name))
				{
					error(h, "attribute name must be NCName");
				}
				else
				{
					qName = new XmlQualifiedName(name, base.AncestorSchema.TargetNamespace);
				}
				if (form != XmlSchemaForm.None)
				{
					error(h, "form must be absent");
				}
				if (base.MinOccursString != null)
				{
					error(h, "minOccurs must be absent");
				}
				if (base.MaxOccursString != null)
				{
					error(h, "maxOccurs must be absent");
				}
				XmlSchemaDerivationMethod xmlSchemaDerivationMethod = XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction;
				if (final == XmlSchemaDerivationMethod.All)
				{
					finalResolved = xmlSchemaDerivationMethod;
				}
				else if (final == XmlSchemaDerivationMethod.None)
				{
					finalResolved = XmlSchemaDerivationMethod.Empty;
				}
				else
				{
					if ((final | XmlSchemaUtil.FinalAllowed) != XmlSchemaUtil.FinalAllowed)
					{
						error(h, "some values for final are invalid in this context");
					}
					finalResolved = final & xmlSchemaDerivationMethod;
				}
				if (schemaType != null && schemaTypeName != null && !schemaTypeName.IsEmpty)
				{
					error(h, "both schemaType and content can't be present");
				}
				if (schemaType != null)
				{
					if (schemaType is XmlSchemaSimpleType)
					{
						errorCount += ((XmlSchemaSimpleType)schemaType).Compile(h, schema);
					}
					else if (schemaType is XmlSchemaComplexType)
					{
						errorCount += ((XmlSchemaComplexType)schemaType).Compile(h, schema);
					}
					else
					{
						error(h, "only simpletype or complextype is allowed");
					}
				}
				if (schemaTypeName != null && !schemaTypeName.IsEmpty && !XmlSchemaUtil.CheckQName(SchemaTypeName))
				{
					error(h, "SchemaTypeName must be an XmlQualifiedName");
				}
				if (SubstitutionGroup != null && !SubstitutionGroup.IsEmpty && !XmlSchemaUtil.CheckQName(SubstitutionGroup))
				{
					error(h, "SubstitutionGroup must be a valid XmlQualifiedName");
				}
				foreach (XmlSchemaObject constraint in constraints)
				{
					if (constraint is XmlSchemaUnique)
					{
						errorCount += ((XmlSchemaUnique)constraint).Compile(h, schema);
					}
					else if (constraint is XmlSchemaKey)
					{
						errorCount += ((XmlSchemaKey)constraint).Compile(h, schema);
					}
					else if (constraint is XmlSchemaKeyref)
					{
						errorCount += ((XmlSchemaKeyref)constraint).Compile(h, schema);
					}
				}
			}
			else
			{
				if (substitutionGroup != null && !substitutionGroup.IsEmpty)
				{
					error(h, "substitutionGroup must be absent");
				}
				if (final != XmlSchemaDerivationMethod.None)
				{
					error(h, "final must be absent");
				}
				CompileOccurence(h, schema);
				if (refName == null || RefName.IsEmpty)
				{
					string targetNamespace = string.Empty;
					if (form == XmlSchemaForm.Qualified || (form == XmlSchemaForm.None && base.AncestorSchema.ElementFormDefault == XmlSchemaForm.Qualified))
					{
						targetNamespace = base.AncestorSchema.TargetNamespace;
					}
					if (name == null)
					{
						error(h, "Required attribute name must be present");
					}
					else if (!XmlSchemaUtil.CheckNCName(name))
					{
						error(h, "attribute name must be NCName");
					}
					else
					{
						qName = new XmlQualifiedName(name, targetNamespace);
					}
					if (schemaType != null && schemaTypeName != null && !schemaTypeName.IsEmpty)
					{
						error(h, "both schemaType and content can't be present");
					}
					if (schemaType != null)
					{
						if (schemaType is XmlSchemaSimpleType)
						{
							errorCount += ((XmlSchemaSimpleType)schemaType).Compile(h, schema);
						}
						else if (schemaType is XmlSchemaComplexType)
						{
							errorCount += ((XmlSchemaComplexType)schemaType).Compile(h, schema);
						}
						else
						{
							error(h, "only simpletype or complextype is allowed");
						}
					}
					if (schemaTypeName != null && !schemaTypeName.IsEmpty && !XmlSchemaUtil.CheckQName(SchemaTypeName))
					{
						error(h, "SchemaTypeName must be an XmlQualifiedName");
					}
					if (SubstitutionGroup != null && !SubstitutionGroup.IsEmpty && !XmlSchemaUtil.CheckQName(SubstitutionGroup))
					{
						error(h, "SubstitutionGroup must be a valid XmlQualifiedName");
					}
					foreach (XmlSchemaObject constraint2 in constraints)
					{
						if (constraint2 is XmlSchemaUnique)
						{
							errorCount += ((XmlSchemaUnique)constraint2).Compile(h, schema);
						}
						else if (constraint2 is XmlSchemaKey)
						{
							errorCount += ((XmlSchemaKey)constraint2).Compile(h, schema);
						}
						else if (constraint2 is XmlSchemaKeyref)
						{
							errorCount += ((XmlSchemaKeyref)constraint2).Compile(h, schema);
						}
					}
				}
				else
				{
					if (!XmlSchemaUtil.CheckQName(RefName))
					{
						error(h, "RefName must be a XmlQualifiedName");
					}
					if (name != null)
					{
						error(h, "name must not be present when ref is present");
					}
					if (Constraints.Count != 0)
					{
						error(h, "key, keyref and unique must be absent");
					}
					if (isNillable)
					{
						error(h, "nillable must be absent");
					}
					if (defaultValue != null)
					{
						error(h, "default must be absent");
					}
					if (fixedValue != null)
					{
						error(h, "fixed must be null");
					}
					if (form != XmlSchemaForm.None)
					{
						error(h, "form must be absent");
					}
					if (block != XmlSchemaDerivationMethod.None)
					{
						error(h, "block must be absent");
					}
					if (schemaTypeName != null && !schemaTypeName.IsEmpty)
					{
						error(h, "type must be absent");
					}
					if (SchemaType != null)
					{
						error(h, "simpleType or complexType must be absent");
					}
					qName = RefName;
				}
			}
			switch (block)
			{
			case XmlSchemaDerivationMethod.All:
				blockResolved = XmlSchemaDerivationMethod.All;
				break;
			case XmlSchemaDerivationMethod.None:
				blockResolved = XmlSchemaDerivationMethod.Empty;
				break;
			default:
				if ((block | XmlSchemaUtil.ElementBlockAllowed) != XmlSchemaUtil.ElementBlockAllowed)
				{
					error(h, "Some of the values for block are invalid in this context");
				}
				blockResolved = block;
				break;
			}
			if (Constraints != null)
			{
				XmlSchemaObjectTable table = new XmlSchemaObjectTable();
				foreach (XmlSchemaIdentityConstraint constraint3 in Constraints)
				{
					XmlSchemaUtil.AddToTable(table, constraint3, constraint3.QualifiedName, h);
				}
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override XmlSchemaParticle GetOptimizedParticle(bool isTop)
		{
			if (OptimizedParticle != null)
			{
				return OptimizedParticle;
			}
			if (RefName != null && RefName != XmlQualifiedName.Empty)
			{
				referencedElement = schema.FindElement(RefName);
			}
			if (base.ValidatedMaxOccurs == 0m)
			{
				OptimizedParticle = XmlSchemaParticle.Empty;
			}
			else if (SubstitutingElements != null && SubstitutingElements.Count > 0)
			{
				XmlSchemaChoice xmlSchemaChoice = new XmlSchemaChoice();
				xmlSchemaChoice.MinOccurs = base.MinOccurs;
				xmlSchemaChoice.MaxOccurs = base.MaxOccurs;
				xmlSchemaChoice.Compile(null, schema);
				XmlSchemaElement xmlSchemaElement = MemberwiseClone() as XmlSchemaElement;
				xmlSchemaElement.MinOccurs = 1m;
				xmlSchemaElement.MaxOccurs = 1m;
				xmlSchemaElement.substitutionGroupElement = null;
				xmlSchemaElement.substitutingElements = null;
				for (int i = 0; i < SubstitutingElements.Count; i++)
				{
					XmlSchemaElement el = SubstitutingElements[i] as XmlSchemaElement;
					AddSubstElementRecursively(xmlSchemaChoice.Items, el);
					AddSubstElementRecursively(xmlSchemaChoice.CompiledItems, el);
				}
				if (!xmlSchemaChoice.Items.Contains(xmlSchemaElement))
				{
					xmlSchemaChoice.Items.Add(xmlSchemaElement);
					xmlSchemaChoice.CompiledItems.Add(xmlSchemaElement);
				}
				OptimizedParticle = xmlSchemaChoice;
			}
			else
			{
				OptimizedParticle = this;
			}
			return OptimizedParticle;
		}

		private void AddSubstElementRecursively(XmlSchemaObjectCollection col, XmlSchemaElement el)
		{
			if (el.SubstitutingElements != null)
			{
				for (int i = 0; i < el.SubstitutingElements.Count; i++)
				{
					AddSubstElementRecursively(col, el.SubstitutingElements[i] as XmlSchemaElement);
				}
			}
			if (!col.Contains(el))
			{
				col.Add(el);
			}
		}

		internal void FillSubstitutionElementInfo()
		{
			if (substitutionGroupElement == null && SubstitutionGroup != XmlQualifiedName.Empty)
			{
				XmlSchemaElement xmlSchemaElement = (substitutionGroupElement = schema.FindElement(SubstitutionGroup));
				if (xmlSchemaElement != null)
				{
					xmlSchemaElement.substitutingElements.Add(this);
				}
			}
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.CompilationId))
			{
				return errorCount;
			}
			actualIsNillable = IsNillable;
			actualIsAbstract = IsAbstract;
			if (SubstitutionGroup != XmlQualifiedName.Empty)
			{
				XmlSchemaElement xmlSchemaElement = substitutionGroupElement;
				if (xmlSchemaElement != null)
				{
					xmlSchemaElement.Validate(h, schema);
				}
			}
			XmlSchemaDatatype xmlSchemaDatatype = null;
			if (schemaType != null)
			{
				elementType = schemaType;
			}
			else if (SchemaTypeName != XmlQualifiedName.Empty)
			{
				XmlSchemaType xmlSchemaType = schema.FindSchemaType(SchemaTypeName);
				if (xmlSchemaType != null)
				{
					xmlSchemaType.Validate(h, schema);
					elementType = xmlSchemaType;
				}
				else if (SchemaTypeName == XmlSchemaComplexType.AnyTypeName)
				{
					elementType = XmlSchemaComplexType.AnyType;
				}
				else if (XmlSchemaUtil.IsBuiltInDatatypeName(SchemaTypeName))
				{
					xmlSchemaDatatype = XmlSchemaDatatype.FromName(SchemaTypeName);
					if (xmlSchemaDatatype == null)
					{
						error(h, "Invalid schema datatype was specified.");
					}
					else
					{
						elementType = xmlSchemaDatatype;
					}
				}
				else if (!schema.IsNamespaceAbsent(SchemaTypeName.Namespace))
				{
					error(h, string.Concat("Referenced element schema type ", SchemaTypeName, " was not found in the corresponding schema."));
				}
			}
			else if (RefName != XmlQualifiedName.Empty)
			{
				XmlSchemaElement xmlSchemaElement2 = schema.FindElement(RefName);
				if (xmlSchemaElement2 != null)
				{
					referencedElement = xmlSchemaElement2;
					errorCount += xmlSchemaElement2.Validate(h, schema);
				}
				else if (!schema.IsNamespaceAbsent(RefName.Namespace))
				{
					error(h, string.Concat("Referenced element ", RefName, " was not found in the corresponding schema."));
				}
			}
			if (referencedElement == null)
			{
				if (elementType == null && substitutionGroupElement != null)
				{
					elementType = substitutionGroupElement.ElementType;
				}
				if (elementType == null)
				{
					elementType = XmlSchemaComplexType.AnyType;
				}
			}
			XmlSchemaType xmlSchemaType2 = elementType as XmlSchemaType;
			if (xmlSchemaType2 != null)
			{
				errorCount += xmlSchemaType2.Validate(h, schema);
				xmlSchemaDatatype = xmlSchemaType2.Datatype;
			}
			if (SubstitutionGroup != XmlQualifiedName.Empty)
			{
				XmlSchemaElement xmlSchemaElement3 = schema.FindElement(SubstitutionGroup);
				if (xmlSchemaElement3 != null)
				{
					XmlSchemaType xmlSchemaType3 = xmlSchemaElement3.ElementType as XmlSchemaType;
					if (xmlSchemaType3 != null)
					{
						if ((xmlSchemaElement3.FinalResolved & XmlSchemaDerivationMethod.Substitution) != XmlSchemaDerivationMethod.Empty)
						{
							error(h, "Substituted element blocks substitution.");
						}
						if (xmlSchemaType2 != null && (xmlSchemaElement3.FinalResolved & xmlSchemaType2.DerivedBy) != XmlSchemaDerivationMethod.Empty)
						{
							error(h, string.Concat("Invalid derivation was found. Substituted element prohibits this derivation method: ", xmlSchemaType2.DerivedBy, "."));
						}
					}
					XmlSchemaComplexType xmlSchemaComplexType = xmlSchemaType2 as XmlSchemaComplexType;
					if (xmlSchemaComplexType != null)
					{
						xmlSchemaComplexType.ValidateTypeDerivationOK(xmlSchemaElement3.ElementType, h, schema);
					}
					else
					{
						XmlSchemaSimpleType xmlSchemaSimpleType = xmlSchemaType2 as XmlSchemaSimpleType;
						if (xmlSchemaSimpleType != null)
						{
							xmlSchemaSimpleType.ValidateTypeDerivationOK(xmlSchemaElement3.ElementType, h, schema, true);
						}
					}
				}
				else if (!schema.IsNamespaceAbsent(SubstitutionGroup.Namespace))
				{
					error(h, string.Concat("Referenced element type ", SubstitutionGroup, " was not found in the corresponding schema."));
				}
			}
			if (defaultValue != null || fixedValue != null)
			{
				ValidateElementDefaultValidImmediate(h, schema);
				if (xmlSchemaDatatype != null && xmlSchemaDatatype.TokenizedType == XmlTokenizedType.ID)
				{
					error(h, "Element type is ID, which does not allows default or fixed values.");
				}
			}
			foreach (XmlSchemaIdentityConstraint constraint in Constraints)
			{
				constraint.Validate(h, schema);
			}
			if (elementType != null)
			{
				elementSchemaType = elementType as XmlSchemaType;
				if (elementType == XmlSchemaSimpleType.AnySimpleType)
				{
					elementSchemaType = XmlSchemaSimpleType.XsAnySimpleType;
				}
				if (elementSchemaType == null)
				{
					elementSchemaType = XmlSchemaType.GetBuiltInSimpleType(SchemaTypeName);
				}
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal override bool ParticleEquals(XmlSchemaParticle other)
		{
			XmlSchemaElement xmlSchemaElement = other as XmlSchemaElement;
			if (xmlSchemaElement == null)
			{
				return false;
			}
			if (base.ValidatedMaxOccurs != xmlSchemaElement.ValidatedMaxOccurs || base.ValidatedMinOccurs != xmlSchemaElement.ValidatedMinOccurs)
			{
				return false;
			}
			if (QualifiedName != xmlSchemaElement.QualifiedName || ElementType != xmlSchemaElement.ElementType || Constraints.Count != xmlSchemaElement.Constraints.Count)
			{
				return false;
			}
			for (int i = 0; i < Constraints.Count; i++)
			{
				XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = Constraints[i] as XmlSchemaIdentityConstraint;
				XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint2 = xmlSchemaElement.Constraints[i] as XmlSchemaIdentityConstraint;
				if (xmlSchemaIdentityConstraint.QualifiedName != xmlSchemaIdentityConstraint2.QualifiedName || xmlSchemaIdentityConstraint.Selector.XPath != xmlSchemaIdentityConstraint2.Selector.XPath || xmlSchemaIdentityConstraint.Fields.Count != xmlSchemaIdentityConstraint2.Fields.Count)
				{
					return false;
				}
				for (int j = 0; j < xmlSchemaIdentityConstraint.Fields.Count; j++)
				{
					XmlSchemaXPath xmlSchemaXPath = xmlSchemaIdentityConstraint.Fields[j] as XmlSchemaXPath;
					XmlSchemaXPath xmlSchemaXPath2 = xmlSchemaIdentityConstraint2.Fields[j] as XmlSchemaXPath;
					if (xmlSchemaXPath.XPath != xmlSchemaXPath2.XPath)
					{
						return false;
					}
				}
			}
			if (BlockResolved != xmlSchemaElement.BlockResolved || FinalResolved != xmlSchemaElement.FinalResolved || ValidatedDefaultValue != xmlSchemaElement.ValidatedDefaultValue || ValidatedFixedValue != xmlSchemaElement.ValidatedFixedValue)
			{
				return false;
			}
			return true;
		}

		internal override bool ValidateDerivationByRestriction(XmlSchemaParticle baseParticle, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			XmlSchemaElement xmlSchemaElement = baseParticle as XmlSchemaElement;
			if (xmlSchemaElement != null)
			{
				return ValidateDerivationByRestrictionNameAndTypeOK(xmlSchemaElement, h, schema, raiseError);
			}
			XmlSchemaAny xmlSchemaAny = baseParticle as XmlSchemaAny;
			if (xmlSchemaAny != null)
			{
				if (!xmlSchemaAny.ValidateWildcardAllowsNamespaceName(QualifiedName.Namespace, h, schema, raiseError))
				{
					return false;
				}
				return ValidateOccurenceRangeOK(xmlSchemaAny, h, schema, raiseError);
			}
			XmlSchemaGroupBase xmlSchemaGroupBase = null;
			if (baseParticle is XmlSchemaSequence)
			{
				xmlSchemaGroupBase = new XmlSchemaSequence();
			}
			else if (baseParticle is XmlSchemaChoice)
			{
				xmlSchemaGroupBase = new XmlSchemaChoice();
			}
			else if (baseParticle is XmlSchemaAll)
			{
				xmlSchemaGroupBase = new XmlSchemaAll();
			}
			if (xmlSchemaGroupBase != null)
			{
				xmlSchemaGroupBase.Items.Add(this);
				xmlSchemaGroupBase.Compile(h, schema);
				xmlSchemaGroupBase.Validate(h, schema);
				return xmlSchemaGroupBase.ValidateDerivationByRestriction(baseParticle, h, schema, raiseError);
			}
			return true;
		}

		private bool ValidateDerivationByRestrictionNameAndTypeOK(XmlSchemaElement baseElement, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			if (QualifiedName != baseElement.QualifiedName)
			{
				if (raiseError)
				{
					error(h, "Invalid derivation by restriction of particle was found. Both elements must have the same name.");
				}
				return false;
			}
			if (isNillable && !baseElement.isNillable)
			{
				if (raiseError)
				{
					error(h, "Invalid element derivation by restriction of particle was found. Base element is not nillable and derived type is nillable.");
				}
				return false;
			}
			if (!ValidateOccurenceRangeOK(baseElement, h, schema, raiseError))
			{
				return false;
			}
			if (baseElement.ValidatedFixedValue != null && baseElement.ValidatedFixedValue != ValidatedFixedValue)
			{
				if (raiseError)
				{
					error(h, "Invalid element derivation by restriction of particle was found. Both fixed value must be the same.");
				}
				return false;
			}
			if ((baseElement.BlockResolved | BlockResolved) != BlockResolved)
			{
				if (raiseError)
				{
					error(h, "Invalid derivation by restriction of particle was found. Derived element must contain all of the base element's block value.");
				}
				return false;
			}
			if (baseElement.ElementType != null)
			{
				XmlSchemaComplexType xmlSchemaComplexType = ElementType as XmlSchemaComplexType;
				if (xmlSchemaComplexType != null)
				{
					xmlSchemaComplexType.ValidateDerivationValidRestriction(baseElement.ElementType as XmlSchemaComplexType, h, schema);
					xmlSchemaComplexType.ValidateTypeDerivationOK(baseElement.ElementType, h, schema);
				}
				else
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = ElementType as XmlSchemaSimpleType;
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaSimpleType.ValidateTypeDerivationOK(baseElement.ElementType, h, schema, true);
					}
					else if (baseElement.ElementType != XmlSchemaComplexType.AnyType && baseElement.ElementType != ElementType)
					{
						if (raiseError)
						{
							error(h, "Invalid element derivation by restriction of particle was found. Both primitive types differ.");
						}
						return false;
					}
				}
			}
			return true;
		}

		internal override void CheckRecursion(int depth, ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaComplexType xmlSchemaComplexType = ElementType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null && xmlSchemaComplexType.Particle != null)
			{
				xmlSchemaComplexType.Particle.CheckRecursion(depth + 1, h, schema);
			}
		}

		internal override void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			if (qnames.Contains(QualifiedName))
			{
				error(h, "Ambiguous element label was detected: " + QualifiedName);
				return;
			}
			foreach (XmlSchemaAny nsName in nsNames)
			{
				if (nsName.ValidatedMaxOccurs == 0m)
				{
					continue;
				}
				if (nsName.HasValueAny || (nsName.HasValueLocal && QualifiedName.Namespace == string.Empty) || (nsName.HasValueOther && QualifiedName.Namespace != QualifiedName.Namespace) || (nsName.HasValueTargetNamespace && QualifiedName.Namespace == QualifiedName.Namespace))
				{
					error(h, "Ambiguous element label which is contained by -any- particle was detected: " + QualifiedName);
					break;
				}
				if (!nsName.HasValueOther)
				{
					bool flag = false;
					StringEnumerator enumerator2 = nsName.ResolvedNamespaces.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							string current = enumerator2.Current;
							if (current == QualifiedName.Namespace)
							{
								flag = true;
								break;
							}
						}
					}
					finally
					{
						IDisposable disposable = enumerator2 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					if (flag)
					{
						error(h, "Ambiguous element label which is contained by -any- particle was detected: " + QualifiedName);
						break;
					}
				}
				else if (nsName.TargetNamespace != QualifiedName.Namespace)
				{
					error(h, string.Format("Ambiguous element label '{0}' which is contained by -any- particle with ##other value than '{1}' was detected: ", QualifiedName.Namespace, nsName.TargetNamespace));
				}
			}
			qnames.Add(QualifiedName, this);
		}

		internal override void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaElement xmlSchemaElement = labels[QualifiedName] as XmlSchemaElement;
			if (xmlSchemaElement == null)
			{
				labels.Add(QualifiedName, this);
			}
			else if (xmlSchemaElement.ElementType != ElementType)
			{
				error(h, "Different types are specified on the same named elements in the same sequence. Element name is " + QualifiedName);
			}
		}

		private void ValidateElementDefaultValidImmediate(ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaDatatype xmlSchemaDatatype = elementType as XmlSchemaDatatype;
			XmlSchemaSimpleType xmlSchemaSimpleType = elementType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				xmlSchemaDatatype = xmlSchemaSimpleType.Datatype;
			}
			if (xmlSchemaDatatype == null)
			{
				XmlSchemaComplexType xmlSchemaComplexType = elementType as XmlSchemaComplexType;
				XmlSchemaContentType contentType = xmlSchemaComplexType.ContentType;
				if (contentType == XmlSchemaContentType.Empty || contentType == XmlSchemaContentType.ElementOnly)
				{
					error(h, "Element content type must be simple type or mixed.");
				}
				xmlSchemaDatatype = XmlSchemaSimpleType.AnySimpleType;
			}
			XmlNamespaceManager xmlNamespaceManager = null;
			if (xmlSchemaDatatype.TokenizedType == XmlTokenizedType.QName && base.Namespaces != null)
			{
				XmlQualifiedName[] array = base.Namespaces.ToArray();
				foreach (XmlQualifiedName xmlQualifiedName in array)
				{
					xmlNamespaceManager.AddNamespace(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
				}
			}
			try
			{
				if (defaultValue != null)
				{
					validatedDefaultValue = xmlSchemaDatatype.Normalize(defaultValue);
					xmlSchemaDatatype.ParseValue(validatedDefaultValue, null, xmlNamespaceManager);
				}
			}
			catch (Exception innerException)
			{
				XmlSchemaObject.error(h, "The Element's default value is invalid with respect to its type definition.", innerException);
			}
			try
			{
				if (fixedValue != null)
				{
					validatedFixedValue = xmlSchemaDatatype.Normalize(fixedValue);
					xmlSchemaDatatype.ParseValue(validatedFixedValue, null, xmlNamespaceManager);
				}
			}
			catch (Exception innerException2)
			{
				XmlSchemaObject.error(h, "The Element's fixed value is invalid with its type definition.", innerException2);
			}
		}

		internal static XmlSchemaElement Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "element")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaElement.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaElement.LineNumber = reader.LineNumber;
			xmlSchemaElement.LinePosition = reader.LinePosition;
			xmlSchemaElement.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				Exception innerEx;
				if (reader.Name == "abstract")
				{
					xmlSchemaElement.IsAbstract = XmlSchemaUtil.ReadBoolAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is invalid value for abstract", innerEx);
					}
				}
				else if (reader.Name == "block")
				{
					xmlSchemaElement.block = XmlSchemaUtil.ReadDerivationAttribute(reader, out innerEx, "block", XmlSchemaUtil.ElementBlockAllowed);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, "some invalid values for block attribute were found", innerEx);
					}
				}
				else if (reader.Name == "default")
				{
					xmlSchemaElement.defaultValue = reader.Value;
				}
				else if (reader.Name == "final")
				{
					xmlSchemaElement.Final = XmlSchemaUtil.ReadDerivationAttribute(reader, out innerEx, "final", XmlSchemaUtil.FinalAllowed);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, "some invalid values for final attribute were found", innerEx);
					}
				}
				else if (reader.Name == "fixed")
				{
					xmlSchemaElement.fixedValue = reader.Value;
				}
				else if (reader.Name == "form")
				{
					xmlSchemaElement.form = XmlSchemaUtil.ReadFormAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for form attribute", innerEx);
					}
				}
				else if (reader.Name == "id")
				{
					xmlSchemaElement.Id = reader.Value;
				}
				else if (reader.Name == "maxOccurs")
				{
					try
					{
						xmlSchemaElement.MaxOccursString = reader.Value;
					}
					catch (Exception innerException)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for maxOccurs", innerException);
					}
				}
				else if (reader.Name == "minOccurs")
				{
					try
					{
						xmlSchemaElement.MinOccursString = reader.Value;
					}
					catch (Exception innerException2)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for minOccurs", innerException2);
					}
				}
				else if (reader.Name == "name")
				{
					xmlSchemaElement.Name = reader.Value;
				}
				else if (reader.Name == "nillable")
				{
					xmlSchemaElement.IsNillable = XmlSchemaUtil.ReadBoolAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + "is not a valid value for nillable", innerEx);
					}
				}
				else if (reader.Name == "ref")
				{
					xmlSchemaElement.refName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for ref attribute", innerEx);
					}
				}
				else if (reader.Name == "substitutionGroup")
				{
					xmlSchemaElement.substitutionGroup = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for substitutionGroup attribute", innerEx);
					}
				}
				else if (reader.Name == "type")
				{
					xmlSchemaElement.SchemaTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for type attribute", innerEx);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for element", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaElement);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaElement;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "element")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaElement.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaElement.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "simpleType")
					{
						num = 3;
						XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
						if (xmlSchemaSimpleType != null)
						{
							xmlSchemaElement.SchemaType = xmlSchemaSimpleType;
						}
						continue;
					}
					if (reader.LocalName == "complexType")
					{
						num = 3;
						XmlSchemaComplexType xmlSchemaComplexType = XmlSchemaComplexType.Read(reader, h);
						if (xmlSchemaComplexType != null)
						{
							xmlSchemaElement.SchemaType = xmlSchemaComplexType;
						}
						continue;
					}
				}
				if (num <= 3)
				{
					if (reader.LocalName == "unique")
					{
						num = 3;
						XmlSchemaUnique xmlSchemaUnique = XmlSchemaUnique.Read(reader, h);
						if (xmlSchemaUnique != null)
						{
							xmlSchemaElement.constraints.Add(xmlSchemaUnique);
						}
						continue;
					}
					if (reader.LocalName == "key")
					{
						num = 3;
						XmlSchemaKey xmlSchemaKey = XmlSchemaKey.Read(reader, h);
						if (xmlSchemaKey != null)
						{
							xmlSchemaElement.constraints.Add(xmlSchemaKey);
						}
						continue;
					}
					if (reader.LocalName == "keyref")
					{
						num = 3;
						XmlSchemaKeyref xmlSchemaKeyref = XmlSchemaKeyref.Read(reader, h);
						if (xmlSchemaKeyref != null)
						{
							xmlSchemaElement.constraints.Add(xmlSchemaKeyref);
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
			return xmlSchemaElement;
		}
	}
}
