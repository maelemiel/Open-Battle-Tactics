using System.Collections;
using System.IO;
using System.Text;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public sealed class XmlSchemaValidator
	{
		private enum Transition
		{
			None = 0,
			Content = 1,
			StartTag = 2,
			Finished = 3
		}

		private static readonly XmlSchemaAttribute[] emptyAttributeArray = new XmlSchemaAttribute[0];

		private object nominalEventSender;

		private IXmlLineInfo lineInfo;

		private IXmlNamespaceResolver nsResolver;

		private Uri sourceUri;

		private XmlNameTable nameTable;

		private XmlSchemaSet schemas;

		private XmlResolver xmlResolver = new XmlUrlResolver();

		private XmlSchemaObject startType;

		private XmlSchemaValidationFlags options;

		private Transition transition;

		private XsdParticleStateManager state;

		private ArrayList occuredAtts = new ArrayList();

		private XmlSchemaAttribute[] defaultAttributes = emptyAttributeArray;

		private ArrayList defaultAttributesCache = new ArrayList();

		private XsdIDManager idManager = new XsdIDManager();

		private ArrayList keyTables = new ArrayList();

		private ArrayList currentKeyFieldConsumers = new ArrayList();

		private ArrayList tmpKeyrefPool;

		private ArrayList elementQNameStack = new ArrayList();

		private StringBuilder storedCharacters = new StringBuilder();

		private bool shouldValidateCharacters;

		private int depth;

		private int xsiNilDepth = -1;

		private int skipValidationDepth = -1;

		internal XmlSchemaDatatype CurrentAttributeType;

		public object ValidationEventSender
		{
			get
			{
				return nominalEventSender;
			}
			set
			{
				nominalEventSender = value;
			}
		}

		public IXmlLineInfo LineInfoProvider
		{
			get
			{
				return lineInfo;
			}
			set
			{
				lineInfo = value;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				xmlResolver = value;
			}
		}

		public Uri SourceUri
		{
			get
			{
				return sourceUri;
			}
			set
			{
				sourceUri = value;
			}
		}

		private string BaseUri
		{
			get
			{
				return (!(sourceUri != null)) ? string.Empty : sourceUri.AbsoluteUri;
			}
		}

		private XsdValidationContext Context
		{
			get
			{
				return state.Context;
			}
		}

		private bool IgnoreWarnings
		{
			get
			{
				return (options & XmlSchemaValidationFlags.ReportValidationWarnings) == 0;
			}
		}

		private bool IgnoreIdentity
		{
			get
			{
				return (options & XmlSchemaValidationFlags.ProcessIdentityConstraints) == 0;
			}
		}

		public event ValidationEventHandler ValidationEventHandler;

		public XmlSchemaValidator(XmlNameTable nameTable, XmlSchemaSet schemas, IXmlNamespaceResolver nsResolver, XmlSchemaValidationFlags options)
		{
			this.nameTable = nameTable;
			this.schemas = schemas;
			this.nsResolver = nsResolver;
			this.options = options;
		}

		public XmlSchemaAttribute[] GetExpectedAttributes()
		{
			XmlSchemaComplexType xmlSchemaComplexType = Context.ActualType as XmlSchemaComplexType;
			if (xmlSchemaComplexType == null)
			{
				return emptyAttributeArray;
			}
			ArrayList arrayList = new ArrayList();
			foreach (DictionaryEntry attributeUse in xmlSchemaComplexType.AttributeUses)
			{
				if (!occuredAtts.Contains((XmlQualifiedName)attributeUse.Key))
				{
					arrayList.Add(attributeUse.Value);
				}
			}
			return (XmlSchemaAttribute[])arrayList.ToArray(typeof(XmlSchemaAttribute));
		}

		private void CollectAtomicParticles(XmlSchemaParticle p, ArrayList al)
		{
			if (p is XmlSchemaGroupBase)
			{
				foreach (XmlSchemaParticle item in ((XmlSchemaGroupBase)p).Items)
				{
					CollectAtomicParticles(item, al);
				}
				return;
			}
			al.Add(p);
		}

		[System.MonoTODO]
		public XmlSchemaParticle[] GetExpectedParticles()
		{
			ArrayList arrayList = new ArrayList();
			Context.State.GetExpectedParticles(arrayList);
			ArrayList arrayList2 = new ArrayList();
			foreach (XmlSchemaParticle item in arrayList)
			{
				CollectAtomicParticles(item, arrayList2);
			}
			return (XmlSchemaParticle[])arrayList2.ToArray(typeof(XmlSchemaParticle));
		}

		public void GetUnspecifiedDefaultAttributes(ArrayList defaultAttributeList)
		{
			if (defaultAttributeList == null)
			{
				throw new ArgumentNullException("defaultAttributeList");
			}
			if (transition != Transition.StartTag)
			{
				throw new InvalidOperationException("Method 'GetUnsoecifiedDefaultAttributes' works only when the validator state is inside a start tag.");
			}
			XmlSchemaAttribute[] expectedAttributes = GetExpectedAttributes();
			foreach (XmlSchemaAttribute xmlSchemaAttribute in expectedAttributes)
			{
				if (xmlSchemaAttribute.ValidatedDefaultValue != null || xmlSchemaAttribute.ValidatedFixedValue != null)
				{
					defaultAttributeList.Add(xmlSchemaAttribute);
				}
			}
			defaultAttributeList.AddRange(defaultAttributes);
		}

		public void AddSchema(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			schemas.Add(schema);
			schemas.Compile();
		}

		public void Initialize()
		{
			transition = Transition.Content;
			state = new XsdParticleStateManager();
			if (!schemas.IsCompiled)
			{
				schemas.Compile();
			}
		}

		public void Initialize(XmlSchemaObject partialValidationType)
		{
			if (partialValidationType == null)
			{
				throw new ArgumentNullException("partialValidationType");
			}
			startType = partialValidationType;
			Initialize();
		}

		public void EndValidation()
		{
			CheckState(Transition.Content);
			transition = Transition.Finished;
			if (schemas.Count != 0)
			{
				if (depth > 0)
				{
					throw new InvalidOperationException(string.Format("There are {0} open element(s). ValidateEndElement() must be called for each open element.", depth));
				}
				if (!IgnoreIdentity && idManager.HasMissingIDReferences())
				{
					HandleError("There are missing ID references: " + idManager.GetMissingIDString());
				}
			}
		}

		[System.MonoTODO]
		public void SkipToEndElement(XmlSchemaInfo info)
		{
			CheckState(Transition.Content);
			if (schemas.Count != 0)
			{
				state.PopContext();
			}
		}

		public object ValidateAttribute(string localName, string ns, string attributeValue, XmlSchemaInfo info)
		{
			if (attributeValue == null)
			{
				throw new ArgumentNullException("attributeValue");
			}
			return ValidateAttribute(localName, ns, () => attributeValue, info);
		}

		public object ValidateAttribute(string localName, string ns, XmlValueGetter attributeValue, XmlSchemaInfo info)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			if (attributeValue == null)
			{
				throw new ArgumentNullException("attributeValue");
			}
			CheckState(Transition.StartTag);
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(localName, ns);
			if (occuredAtts.Contains(xmlQualifiedName))
			{
				throw new InvalidOperationException(string.Format("Attribute '{0}' has already been validated in the same element.", xmlQualifiedName));
			}
			occuredAtts.Add(xmlQualifiedName);
			if (ns == "http://www.w3.org/2000/xmlns/")
			{
				return null;
			}
			if (schemas.Count == 0)
			{
				return null;
			}
			if (Context.Element != null && Context.XsiType == null)
			{
				if (Context.ActualType is XmlSchemaComplexType)
				{
					return AssessAttributeElementLocallyValidType(localName, ns, attributeValue, info);
				}
				HandleError("Current simple type cannot accept attributes other than schema instance namespace.");
			}
			return null;
		}

		public void ValidateElement(string localName, string ns, XmlSchemaInfo info)
		{
			ValidateElement(localName, ns, info, null, null, null, null);
		}

		public void ValidateElement(string localName, string ns, XmlSchemaInfo info, string xsiType, string xsiNil, string schemaLocation, string noNsSchemaLocation)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			CheckState(Transition.Content);
			transition = Transition.StartTag;
			if (schemaLocation != null)
			{
				HandleSchemaLocation(schemaLocation);
			}
			if (noNsSchemaLocation != null)
			{
				HandleNoNSSchemaLocation(noNsSchemaLocation);
			}
			elementQNameStack.Add(new XmlQualifiedName(localName, ns));
			if (schemas.Count == 0)
			{
				return;
			}
			if (!IgnoreIdentity)
			{
				idManager.OnStartElement();
			}
			defaultAttributes = emptyAttributeArray;
			if (skipValidationDepth < 0 || depth <= skipValidationDepth)
			{
				if (shouldValidateCharacters)
				{
					ValidateEndSimpleContent(null);
				}
				AssessOpenStartElementSchemaValidity(localName, ns);
			}
			if (xsiNil != null)
			{
				HandleXsiNil(xsiNil, info);
			}
			if (xsiType != null)
			{
				HandleXsiType(xsiType);
			}
			if (xsiNilDepth < depth)
			{
				shouldValidateCharacters = true;
			}
			if (info != null)
			{
				info.IsNil = xsiNilDepth >= 0;
				info.SchemaElement = Context.Element;
				info.SchemaType = Context.ActualSchemaType;
				info.SchemaAttribute = null;
				info.IsDefault = false;
				info.MemberType = null;
			}
		}

		public object ValidateEndElement(XmlSchemaInfo info)
		{
			return ValidateEndElement(info, null);
		}

		[System.MonoTODO]
		public object ValidateEndElement(XmlSchemaInfo info, object var)
		{
			if (transition == Transition.StartTag)
			{
				ValidateEndOfAttributes(info);
			}
			CheckState(Transition.Content);
			elementQNameStack.RemoveAt(elementQNameStack.Count - 1);
			if (schemas.Count == 0)
			{
				return null;
			}
			if (depth == 0)
			{
				throw new InvalidOperationException("There was no corresponding call to 'ValidateElement' method.");
			}
			depth--;
			object result = null;
			if (depth == skipValidationDepth)
			{
				skipValidationDepth = -1;
			}
			else if (skipValidationDepth < 0 || depth <= skipValidationDepth)
			{
				result = AssessEndElementSchemaValidity(info);
			}
			return result;
		}

		public void ValidateEndOfAttributes(XmlSchemaInfo info)
		{
			try
			{
				CheckState(Transition.StartTag);
				transition = Transition.Content;
				if (schemas.Count != 0)
				{
					if (skipValidationDepth < 0 || depth <= skipValidationDepth)
					{
						AssessCloseStartElementSchemaValidity(info);
					}
					depth++;
				}
			}
			finally
			{
				occuredAtts.Clear();
			}
		}

		public void ValidateText(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			ValidateText(() => value);
		}

		public void ValidateText(XmlValueGetter getter)
		{
			if (getter == null)
			{
				throw new ArgumentNullException("getter");
			}
			CheckState(Transition.Content);
			if (schemas.Count == 0 || (skipValidationDepth >= 0 && depth > skipValidationDepth))
			{
				return;
			}
			XmlSchemaComplexType xmlSchemaComplexType = Context.ActualType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null)
			{
				switch (xmlSchemaComplexType.ContentType)
				{
				case XmlSchemaContentType.Empty:
					HandleError("Not allowed character content was found.");
					break;
				case XmlSchemaContentType.ElementOnly:
				{
					string text = storedCharacters.ToString();
					if (text.Length > 0 && !XmlChar.IsWhitespace(text))
					{
						HandleError("Not allowed character content was found.");
					}
					break;
				}
				}
			}
			ValidateCharacters(getter);
		}

		public void ValidateWhitespace(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			ValidateWhitespace(() => value);
		}

		public void ValidateWhitespace(XmlValueGetter getter)
		{
			ValidateText(getter);
		}

		private void HandleError(string message)
		{
			HandleError(message, null, false);
		}

		private void HandleError(string message, Exception innerException)
		{
			HandleError(message, innerException, false);
		}

		private void HandleError(string message, Exception innerException, bool isWarning)
		{
			if (!isWarning || !IgnoreWarnings)
			{
				XmlSchemaValidationException exception = new XmlSchemaValidationException(message, nominalEventSender, BaseUri, null, innerException);
				HandleError(exception, isWarning);
			}
		}

		private void HandleError(XmlSchemaValidationException exception)
		{
			HandleError(exception, false);
		}

		private void HandleError(XmlSchemaValidationException exception, bool isWarning)
		{
			if (!isWarning || !IgnoreWarnings)
			{
				if (this.ValidationEventHandler == null)
				{
					throw exception;
				}
				ValidationEventArgs e = new ValidationEventArgs(exception, exception.Message, isWarning ? XmlSeverityType.Warning : XmlSeverityType.Error);
				this.ValidationEventHandler(nominalEventSender, e);
			}
		}

		private void CheckState(Transition expected)
		{
			if (transition != expected)
			{
				if (transition == Transition.None)
				{
					throw new InvalidOperationException("Initialize() must be called before processing validation.");
				}
				throw new InvalidOperationException(string.Format("Unexpected attempt to validate state transition from {0} to {1}.", transition, expected));
			}
		}

		private XmlSchemaElement FindElement(string name, string ns)
		{
			return (XmlSchemaElement)schemas.GlobalElements[new XmlQualifiedName(name, ns)];
		}

		private XmlSchemaType FindType(XmlQualifiedName qname)
		{
			return (XmlSchemaType)schemas.GlobalTypes[qname];
		}

		private void ValidateStartElementParticle(string localName, string ns)
		{
			if (Context.State != null)
			{
				Context.XsiType = null;
				state.CurrentElement = null;
				Context.EvaluateStartElement(localName, ns);
				if (Context.IsInvalid)
				{
					HandleError("Invalid start element: " + ns + ":" + localName);
				}
				Context.PushCurrentElement(state.CurrentElement);
			}
		}

		private void AssessOpenStartElementSchemaValidity(string localName, string ns)
		{
			if (xsiNilDepth >= 0 && xsiNilDepth < depth)
			{
				HandleError("Element item appeared, while current element context is nil.");
			}
			ValidateStartElementParticle(localName, ns);
			if (Context.Element == null)
			{
				state.CurrentElement = FindElement(localName, ns);
				Context.PushCurrentElement(state.CurrentElement);
			}
			if (!IgnoreIdentity)
			{
				ValidateKeySelectors();
				ValidateKeyFields(false, xsiNilDepth == depth, Context.ActualType, null, null, null);
			}
		}

		private void AssessCloseStartElementSchemaValidity(XmlSchemaInfo info)
		{
			if (Context.XsiType != null)
			{
				AssessCloseStartElementLocallyValidType(info);
			}
			else if (Context.Element != null)
			{
				AssessElementLocallyValidElement();
				if (Context.Element.ElementType != null)
				{
					AssessCloseStartElementLocallyValidType(info);
				}
			}
			if (Context.Element == null)
			{
				XmlSchemaContentProcessing processContents = state.ProcessContents;
				if (processContents != XmlSchemaContentProcessing.Skip && processContents != XmlSchemaContentProcessing.Lax)
				{
					XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)elementQNameStack[elementQNameStack.Count - 1];
					if (Context.XsiType == null && (schemas.Contains(xmlQualifiedName.Namespace) || !schemas.MissedSubComponents(xmlQualifiedName.Namespace)))
					{
						HandleError(string.Concat("Element declaration for ", xmlQualifiedName, " is missing."));
					}
				}
			}
			state.PushContext();
			XsdValidationState xsdValidationState = null;
			if (state.ProcessContents == XmlSchemaContentProcessing.Skip)
			{
				skipValidationDepth = depth;
			}
			else
			{
				XmlSchemaComplexType xmlSchemaComplexType = Context.ActualType as XmlSchemaComplexType;
				xsdValidationState = ((xmlSchemaComplexType != null) ? state.Create(xmlSchemaComplexType.ValidatableParticle) : ((state.ProcessContents != XmlSchemaContentProcessing.Lax) ? state.Create(XmlSchemaParticle.Empty) : state.Create(XmlSchemaAny.AnyTypeContent)));
			}
			Context.State = xsdValidationState;
		}

		private void AssessElementLocallyValidElement()
		{
			XmlSchemaElement element = Context.Element;
			XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)elementQNameStack[elementQNameStack.Count - 1];
			if (element == null)
			{
				HandleError("Element declaration is required for " + xmlQualifiedName);
			}
			if (element.ActualIsAbstract)
			{
				HandleError("Abstract element declaration was specified for " + xmlQualifiedName);
			}
		}

		private void AssessCloseStartElementLocallyValidType(XmlSchemaInfo info)
		{
			object actualType = Context.ActualType;
			if (actualType == null)
			{
				HandleError("Schema type does not exist.");
				return;
			}
			XmlSchemaComplexType xmlSchemaComplexType = actualType as XmlSchemaComplexType;
			XmlSchemaSimpleType xmlSchemaSimpleType = actualType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType == null && xmlSchemaComplexType != null)
			{
				AssessCloseStartElementLocallyValidComplexType(xmlSchemaComplexType, info);
			}
		}

		private void AssessCloseStartElementLocallyValidComplexType(XmlSchemaComplexType cType, XmlSchemaInfo info)
		{
			if (cType.IsAbstract)
			{
				HandleError("Target complex type is abstract.");
				return;
			}
			XmlSchemaAttribute[] expectedAttributes = GetExpectedAttributes();
			foreach (XmlSchemaAttribute xmlSchemaAttribute in expectedAttributes)
			{
				if (xmlSchemaAttribute.ValidatedUse == XmlSchemaUse.Required && xmlSchemaAttribute.ValidatedFixedValue == null)
				{
					HandleError(string.Concat("Required attribute ", xmlSchemaAttribute.QualifiedName, " was not found."));
				}
				else if (xmlSchemaAttribute.ValidatedDefaultValue != null || xmlSchemaAttribute.ValidatedFixedValue != null)
				{
					defaultAttributesCache.Add(xmlSchemaAttribute);
				}
			}
			if (defaultAttributesCache.Count == 0)
			{
				defaultAttributes = emptyAttributeArray;
			}
			else
			{
				defaultAttributes = (XmlSchemaAttribute[])defaultAttributesCache.ToArray(typeof(XmlSchemaAttribute));
			}
			defaultAttributesCache.Clear();
			if (!IgnoreIdentity)
			{
				XmlSchemaAttribute[] array = defaultAttributes;
				foreach (XmlSchemaAttribute xmlSchemaAttribute2 in array)
				{
					XmlSchemaDatatype dt = (xmlSchemaAttribute2.AttributeType as XmlSchemaDatatype) ?? xmlSchemaAttribute2.AttributeSchemaType.Datatype;
					object parsedValue = xmlSchemaAttribute2.ValidatedFixedValue ?? xmlSchemaAttribute2.ValidatedDefaultValue;
					string text = idManager.AssessEachAttributeIdentityConstraint(dt, parsedValue, ((XmlQualifiedName)elementQNameStack[elementQNameStack.Count - 1]).Name);
					if (text != null)
					{
						HandleError(text);
					}
				}
			}
			if (!IgnoreIdentity)
			{
				XmlSchemaAttribute[] array2 = defaultAttributes;
				foreach (XmlSchemaAttribute xmlSchemaAttribute3 in array2)
				{
					ValidateKeyFieldsAttribute(xmlSchemaAttribute3, xmlSchemaAttribute3.ValidatedFixedValue ?? xmlSchemaAttribute3.ValidatedDefaultValue);
				}
			}
		}

		private object AssessAttributeElementLocallyValidType(string localName, string ns, XmlValueGetter getter, XmlSchemaInfo info)
		{
			XmlSchemaComplexType cType = Context.ActualType as XmlSchemaComplexType;
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(localName, ns);
			XmlSchemaObject xmlSchemaObject = XmlSchemaUtil.FindAttributeDeclaration(ns, schemas, cType, xmlQualifiedName);
			if (xmlSchemaObject == null)
			{
				HandleError("Attribute declaration was not found for " + xmlQualifiedName);
			}
			XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
			if (xmlSchemaAttribute != null)
			{
				AssessAttributeLocallyValidUse(xmlSchemaAttribute);
				return AssessAttributeLocallyValid(xmlSchemaAttribute, info, getter);
			}
			return null;
		}

		private object AssessAttributeLocallyValid(XmlSchemaAttribute attr, XmlSchemaInfo info, XmlValueGetter getter)
		{
			if (attr.AttributeType == null)
			{
				HandleError("Attribute type is missing for " + attr.QualifiedName);
			}
			XmlSchemaDatatype dt = attr.AttributeType as XmlSchemaDatatype;
			if (dt == null)
			{
				dt = ((XmlSchemaSimpleType)attr.AttributeType).Datatype;
			}
			object obj = null;
			if (dt != XmlSchemaSimpleType.AnySimpleType || attr.ValidatedFixedValue != null)
			{
				try
				{
					CurrentAttributeType = dt;
					obj = getter();
				}
				catch (Exception innerException)
				{
					HandleError(string.Format("Attribute value is invalid against its data type {0}", (dt != null) ? dt.TokenizedType : XmlTokenizedType.CDATA), innerException);
				}
				XmlSchemaSimpleType xmlSchemaSimpleType = attr.AttributeType as XmlSchemaSimpleType;
				if (xmlSchemaSimpleType != null)
				{
					ValidateRestrictedSimpleTypeValue(xmlSchemaSimpleType, ref dt, new XmlAtomicValue(obj, attr.AttributeSchemaType).Value);
				}
				if (attr.ValidatedFixedValue != null)
				{
					if (!XmlSchemaUtil.AreSchemaDatatypeEqual(attr.AttributeSchemaType, attr.ValidatedFixedTypedValue, attr.AttributeSchemaType, obj))
					{
						HandleError(string.Format("The value of the attribute {0} does not match with its fixed value '{1}' in the space of type {2}", attr.QualifiedName, attr.ValidatedFixedValue, dt));
					}
					obj = attr.ValidatedFixedTypedValue;
				}
			}
			if (!IgnoreIdentity)
			{
				string text = idManager.AssessEachAttributeIdentityConstraint(dt, obj, ((XmlQualifiedName)elementQNameStack[elementQNameStack.Count - 1]).Name);
				if (text != null)
				{
					HandleError(text);
				}
			}
			if (!IgnoreIdentity)
			{
				ValidateKeyFieldsAttribute(attr, obj);
			}
			return obj;
		}

		private void AssessAttributeLocallyValidUse(XmlSchemaAttribute attr)
		{
			if (attr.ValidatedUse == XmlSchemaUse.Prohibited)
			{
				HandleError(string.Concat("Attribute ", attr.QualifiedName, " is prohibited in this context."));
			}
		}

		private object AssessEndElementSchemaValidity(XmlSchemaInfo info)
		{
			object result = ValidateEndSimpleContent(info);
			ValidateEndElementParticle();
			if (!IgnoreIdentity)
			{
				ValidateEndElementKeyConstraints();
			}
			if (xsiNilDepth == depth)
			{
				xsiNilDepth = -1;
			}
			return result;
		}

		private void ValidateEndElementParticle()
		{
			if (Context.State != null && !Context.EvaluateEndElement())
			{
				HandleError("Invalid end element. There are still required content items.");
			}
			Context.PopCurrentElement();
			state.PopContext();
			Context.XsiType = null;
		}

		private void ValidateCharacters(XmlValueGetter getter)
		{
			if (xsiNilDepth >= 0 && xsiNilDepth < depth)
			{
				HandleError("Element item appeared, while current element context is nil.");
			}
			if (shouldValidateCharacters)
			{
				CurrentAttributeType = null;
				storedCharacters.Append(getter());
			}
		}

		private object ValidateEndSimpleContent(XmlSchemaInfo info)
		{
			object result = null;
			if (shouldValidateCharacters)
			{
				result = ValidateEndSimpleContentCore(info);
			}
			shouldValidateCharacters = false;
			storedCharacters.Length = 0;
			return result;
		}

		private object ValidateEndSimpleContentCore(XmlSchemaInfo info)
		{
			if (Context.ActualType == null)
			{
				return null;
			}
			string text = storedCharacters.ToString();
			object result = null;
			if (text.Length == 0 && Context.Element != null && Context.Element.ValidatedDefaultValue != null)
			{
				text = Context.Element.ValidatedDefaultValue;
			}
			XmlSchemaDatatype xmlSchemaDatatype = Context.ActualType as XmlSchemaDatatype;
			XmlSchemaSimpleType xmlSchemaSimpleType = Context.ActualType as XmlSchemaSimpleType;
			if (xmlSchemaDatatype == null)
			{
				if (xmlSchemaSimpleType != null)
				{
					xmlSchemaDatatype = xmlSchemaSimpleType.Datatype;
				}
				else
				{
					XmlSchemaComplexType xmlSchemaComplexType = Context.ActualType as XmlSchemaComplexType;
					xmlSchemaDatatype = xmlSchemaComplexType.Datatype;
					switch (xmlSchemaComplexType.ContentType)
					{
					case XmlSchemaContentType.ElementOnly:
						if (text.Length > 0 && !XmlChar.IsWhitespace(text))
						{
							HandleError("Character content not allowed in an elementOnly model.");
						}
						break;
					case XmlSchemaContentType.Empty:
						if (text.Length > 0)
						{
							HandleError("Character content not allowed in an empty model.");
						}
						break;
					}
				}
			}
			if (xmlSchemaDatatype != null)
			{
				if (Context.Element != null && Context.Element.ValidatedFixedValue != null && text != Context.Element.ValidatedFixedValue)
				{
					HandleError("Fixed value constraint was not satisfied.");
				}
				result = AssessStringValid(xmlSchemaSimpleType, xmlSchemaDatatype, text);
			}
			if (!IgnoreIdentity)
			{
				ValidateSimpleContentIdentity(xmlSchemaDatatype, text);
			}
			shouldValidateCharacters = false;
			if (info != null)
			{
				info.IsNil = xsiNilDepth >= 0;
				info.SchemaElement = null;
				info.SchemaType = Context.ActualType as XmlSchemaType;
				if (info.SchemaType == null)
				{
					info.SchemaType = XmlSchemaType.GetBuiltInSimpleType(xmlSchemaDatatype.TypeCode);
				}
				info.SchemaAttribute = null;
				info.IsDefault = false;
				info.MemberType = null;
			}
			return result;
		}

		private object AssessStringValid(XmlSchemaSimpleType st, XmlSchemaDatatype dt, string value)
		{
			XmlSchemaDatatype xmlSchemaDatatype = dt;
			object result = null;
			if (st != null)
			{
				string text = xmlSchemaDatatype.Normalize(value);
				switch (st.DerivedBy)
				{
				case XmlSchemaDerivationMethod.List:
				{
					XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = st.Content as XmlSchemaSimpleTypeList;
					string[] array = text.Split(XmlChar.WhitespaceChars);
					object[] array2 = new object[array.Length];
					XmlSchemaDatatype xmlSchemaDatatype2 = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaDatatype;
					XmlSchemaSimpleType xmlSchemaSimpleType2 = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaSimpleType;
					for (int i = 0; i < array.Length; i++)
					{
						string text2 = array[i];
						if (text2 == string.Empty)
						{
							continue;
						}
						if (xmlSchemaDatatype2 != null)
						{
							try
							{
								array2[i] = xmlSchemaDatatype2.ParseValue(text2, nameTable, nsResolver);
							}
							catch (Exception innerException)
							{
								HandleError("List type value contains one or more invalid values.", innerException);
								break;
							}
						}
						else
						{
							AssessStringValid(xmlSchemaSimpleType2, xmlSchemaSimpleType2.Datatype, text2);
						}
					}
					result = array2;
					break;
				}
				case XmlSchemaDerivationMethod.Union:
				{
					XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = st.Content as XmlSchemaSimpleTypeUnion;
					string text3 = text;
					bool flag = false;
					object[] validatedTypes = xmlSchemaSimpleTypeUnion.ValidatedTypes;
					foreach (object obj in validatedTypes)
					{
						XmlSchemaDatatype xmlSchemaDatatype2 = obj as XmlSchemaDatatype;
						XmlSchemaSimpleType xmlSchemaSimpleType2 = obj as XmlSchemaSimpleType;
						if (xmlSchemaDatatype2 != null)
						{
							try
							{
								result = xmlSchemaDatatype2.ParseValue(text3, nameTable, nsResolver);
							}
							catch (Exception)
							{
								continue;
							}
						}
						else
						{
							try
							{
								result = AssessStringValid(xmlSchemaSimpleType2, xmlSchemaSimpleType2.Datatype, text3);
							}
							catch (XmlSchemaValidationException)
							{
								continue;
							}
						}
						flag = true;
						break;
					}
					if (!flag)
					{
						HandleError("Union type value contains one or more invalid values.");
					}
					break;
				}
				case XmlSchemaDerivationMethod.Restriction:
				{
					XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = st.Content as XmlSchemaSimpleTypeRestriction;
					if (xmlSchemaSimpleTypeRestriction != null)
					{
						XmlSchemaSimpleType xmlSchemaSimpleType = st.BaseXmlSchemaType as XmlSchemaSimpleType;
						if (xmlSchemaSimpleType != null)
						{
							result = AssessStringValid(xmlSchemaSimpleType, dt, value);
						}
						if (!xmlSchemaSimpleTypeRestriction.ValidateValueWithFacets(value, nameTable, nsResolver))
						{
							HandleError("Specified value was invalid against the facets.");
							break;
						}
					}
					xmlSchemaDatatype = st.Datatype;
					break;
				}
				}
			}
			if (xmlSchemaDatatype != null)
			{
				try
				{
					result = xmlSchemaDatatype.ParseValue(value, nameTable, nsResolver);
				}
				catch (Exception innerException2)
				{
					HandleError(string.Format("Invalidly typed data was specified."), innerException2);
				}
			}
			return result;
		}

		private void ValidateRestrictedSimpleTypeValue(XmlSchemaSimpleType st, ref XmlSchemaDatatype dt, string normalized)
		{
			switch (st.DerivedBy)
			{
			case XmlSchemaDerivationMethod.List:
			{
				XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = st.Content as XmlSchemaSimpleTypeList;
				string[] array = normalized.Split(XmlChar.WhitespaceChars);
				XmlSchemaDatatype xmlSchemaDatatype = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaDatatype;
				XmlSchemaSimpleType xmlSchemaSimpleType2 = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaSimpleType;
				foreach (string text in array)
				{
					if (text == string.Empty)
					{
						continue;
					}
					if (xmlSchemaDatatype != null)
					{
						try
						{
							xmlSchemaDatatype.ParseValue(text, nameTable, nsResolver);
						}
						catch (Exception innerException)
						{
							HandleError("List type value contains one or more invalid values.", innerException);
							break;
						}
					}
					else
					{
						AssessStringValid(xmlSchemaSimpleType2, xmlSchemaSimpleType2.Datatype, text);
					}
				}
				break;
			}
			case XmlSchemaDerivationMethod.Union:
			{
				XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = st.Content as XmlSchemaSimpleTypeUnion;
				bool flag = false;
				object[] validatedTypes = xmlSchemaSimpleTypeUnion.ValidatedTypes;
				foreach (object obj in validatedTypes)
				{
					XmlSchemaDatatype xmlSchemaDatatype = obj as XmlSchemaDatatype;
					XmlSchemaSimpleType xmlSchemaSimpleType2 = obj as XmlSchemaSimpleType;
					if (xmlSchemaDatatype != null)
					{
						try
						{
							xmlSchemaDatatype.ParseValue(normalized, nameTable, nsResolver);
						}
						catch (Exception)
						{
							continue;
						}
					}
					else
					{
						try
						{
							AssessStringValid(xmlSchemaSimpleType2, xmlSchemaSimpleType2.Datatype, normalized);
						}
						catch (XmlSchemaValidationException)
						{
							continue;
						}
					}
					flag = true;
					break;
				}
				if (!flag)
				{
					HandleError("Union type value contains one or more invalid values.");
				}
				break;
			}
			case XmlSchemaDerivationMethod.Restriction:
			{
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = st.Content as XmlSchemaSimpleTypeRestriction;
				if (xmlSchemaSimpleTypeRestriction != null)
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = st.BaseXmlSchemaType as XmlSchemaSimpleType;
					if (xmlSchemaSimpleType != null)
					{
						AssessStringValid(xmlSchemaSimpleType, dt, normalized);
					}
					if (!xmlSchemaSimpleTypeRestriction.ValidateValueWithFacets(normalized, nameTable, nsResolver))
					{
						HandleError("Specified value was invalid against the facets.");
						break;
					}
				}
				dt = st.Datatype;
				break;
			}
			}
		}

		private XsdKeyTable CreateNewKeyTable(XmlSchemaIdentityConstraint ident)
		{
			XsdKeyTable xsdKeyTable = new XsdKeyTable(ident);
			xsdKeyTable.StartDepth = depth;
			keyTables.Add(xsdKeyTable);
			return xsdKeyTable;
		}

		private void ValidateKeySelectors()
		{
			if (tmpKeyrefPool != null)
			{
				tmpKeyrefPool.Clear();
			}
			if (Context.Element != null && Context.Element.Constraints.Count > 0)
			{
				for (int i = 0; i < Context.Element.Constraints.Count; i++)
				{
					XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = (XmlSchemaIdentityConstraint)Context.Element.Constraints[i];
					XsdKeyTable value = CreateNewKeyTable(xmlSchemaIdentityConstraint);
					if (xmlSchemaIdentityConstraint is XmlSchemaKeyref)
					{
						if (tmpKeyrefPool == null)
						{
							tmpKeyrefPool = new ArrayList();
						}
						tmpKeyrefPool.Add(value);
					}
				}
			}
			for (int j = 0; j < keyTables.Count; j++)
			{
				XsdKeyTable xsdKeyTable = (XsdKeyTable)keyTables[j];
				if (xsdKeyTable.SelectorMatches(elementQNameStack, depth) != null)
				{
					XsdKeyEntry entry = new XsdKeyEntry(xsdKeyTable, depth, lineInfo);
					xsdKeyTable.Entries.Add(entry);
				}
			}
		}

		private void ValidateKeyFieldsAttribute(XmlSchemaAttribute attr, object value)
		{
			ValidateKeyFields(true, false, attr.AttributeType, attr.QualifiedName.Name, attr.QualifiedName.Namespace, value);
		}

		private void ValidateKeyFields(bool isAttr, bool isNil, object schemaType, string attrName, string attrNs, object value)
		{
			for (int i = 0; i < keyTables.Count; i++)
			{
				XsdKeyTable xsdKeyTable = (XsdKeyTable)keyTables[i];
				for (int j = 0; j < xsdKeyTable.Entries.Count; j++)
				{
					CurrentAttributeType = null;
					try
					{
						xsdKeyTable.Entries[j].ProcessMatch(isAttr, elementQNameStack, nominalEventSender, nameTable, BaseUri, schemaType, nsResolver, lineInfo, (!isAttr) ? depth : (depth + 1), attrName, attrNs, value, isNil, currentKeyFieldConsumers);
					}
					catch (XmlSchemaValidationException exception)
					{
						HandleError(exception);
					}
				}
			}
		}

		private void ValidateEndElementKeyConstraints()
		{
			for (int i = 0; i < keyTables.Count; i++)
			{
				XsdKeyTable xsdKeyTable = keyTables[i] as XsdKeyTable;
				if (xsdKeyTable.StartDepth == depth)
				{
					ValidateEndKeyConstraint(xsdKeyTable);
					continue;
				}
				for (int j = 0; j < xsdKeyTable.Entries.Count; j++)
				{
					XsdKeyEntry xsdKeyEntry = xsdKeyTable.Entries[j];
					if (xsdKeyEntry.StartDepth == depth)
					{
						if (xsdKeyEntry.KeyFound)
						{
							xsdKeyTable.FinishedEntries.Add(xsdKeyEntry);
						}
						else if (xsdKeyTable.SourceSchemaIdentity is XmlSchemaKey)
						{
							HandleError("Key sequence is missing.");
						}
						xsdKeyTable.Entries.RemoveAt(j);
						j--;
						continue;
					}
					for (int k = 0; k < xsdKeyEntry.KeyFields.Count; k++)
					{
						XsdKeyEntryField xsdKeyEntryField = xsdKeyEntry.KeyFields[k];
						if (!xsdKeyEntryField.FieldFound && xsdKeyEntryField.FieldFoundDepth == depth)
						{
							xsdKeyEntryField.FieldFoundDepth = 0;
							xsdKeyEntryField.FieldFoundPath = null;
						}
					}
				}
			}
			for (int l = 0; l < keyTables.Count; l++)
			{
				XsdKeyTable xsdKeyTable2 = keyTables[l] as XsdKeyTable;
				if (xsdKeyTable2.StartDepth == depth)
				{
					keyTables.RemoveAt(l);
					l--;
				}
			}
		}

		private void ValidateEndKeyConstraint(XsdKeyTable seq)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < seq.Entries.Count; i++)
			{
				XsdKeyEntry xsdKeyEntry = seq.Entries[i];
				if (!xsdKeyEntry.KeyFound && seq.SourceSchemaIdentity is XmlSchemaKey)
				{
					arrayList.Add("line " + xsdKeyEntry.SelectorLineNumber + "position " + xsdKeyEntry.SelectorLinePosition);
				}
			}
			if (arrayList.Count > 0)
			{
				HandleError("Invalid identity constraints were found. Key was not found. " + string.Join(", ", arrayList.ToArray(typeof(string)) as string[]));
			}
			arrayList.Clear();
			XmlSchemaKeyref xmlSchemaKeyref = seq.SourceSchemaIdentity as XmlSchemaKeyref;
			if (xmlSchemaKeyref == null)
			{
				return;
			}
			for (int num = keyTables.Count - 1; num >= 0; num--)
			{
				XsdKeyTable xsdKeyTable = keyTables[num] as XsdKeyTable;
				if (xsdKeyTable.SourceSchemaIdentity == xmlSchemaKeyref.Target)
				{
					seq.ReferencedKey = xsdKeyTable;
					for (int j = 0; j < seq.FinishedEntries.Count; j++)
					{
						XsdKeyEntry xsdKeyEntry2 = seq.FinishedEntries[j];
						for (int k = 0; k < xsdKeyTable.FinishedEntries.Count; k++)
						{
							XsdKeyEntry other = xsdKeyTable.FinishedEntries[k];
							if (xsdKeyEntry2.CompareIdentity(other))
							{
								xsdKeyEntry2.KeyRefFound = true;
								break;
							}
						}
					}
				}
			}
			if (seq.ReferencedKey == null)
			{
				HandleError("Target key was not found.");
			}
			for (int l = 0; l < seq.FinishedEntries.Count; l++)
			{
				XsdKeyEntry xsdKeyEntry3 = seq.FinishedEntries[l];
				if (!xsdKeyEntry3.KeyRefFound)
				{
					arrayList.Add(" line " + xsdKeyEntry3.SelectorLineNumber + ", position " + xsdKeyEntry3.SelectorLinePosition);
				}
			}
			if (arrayList.Count > 0)
			{
				HandleError("Invalid identity constraints were found. Referenced key was not found: " + string.Join(" / ", arrayList.ToArray(typeof(string)) as string[]));
			}
		}

		private void ValidateSimpleContentIdentity(XmlSchemaDatatype dt, string value)
		{
			if (currentKeyFieldConsumers == null)
			{
				return;
			}
			while (currentKeyFieldConsumers.Count > 0)
			{
				XsdKeyEntryField xsdKeyEntryField = currentKeyFieldConsumers[0] as XsdKeyEntryField;
				if (xsdKeyEntryField.Identity != null)
				{
					HandleError(string.Concat("Two or more identical field was found. Former value is '", xsdKeyEntryField.Identity, "' ."));
				}
				object obj = null;
				if (dt != null)
				{
					try
					{
						obj = dt.ParseValue(value, nameTable, nsResolver);
					}
					catch (Exception innerException)
					{
						HandleError("Identity value is invalid against its data type " + dt.TokenizedType, innerException);
					}
				}
				if (obj == null)
				{
					obj = value;
				}
				if (!xsdKeyEntryField.SetIdentityField(obj, depth == xsiNilDepth, dt as XsdAnySimpleType, depth, lineInfo))
				{
					HandleError("Two or more identical key value was found: '" + value + "' .");
				}
				currentKeyFieldConsumers.RemoveAt(0);
			}
		}

		private object GetXsiType(string name)
		{
			object obj = null;
			XmlQualifiedName xmlQualifiedName = XmlQualifiedName.Parse(name, nsResolver, true);
			if (xmlQualifiedName == XmlSchemaComplexType.AnyTypeName)
			{
				return XmlSchemaComplexType.AnyType;
			}
			if (XmlSchemaUtil.IsBuiltInDatatypeName(xmlQualifiedName))
			{
				return XmlSchemaDatatype.FromName(xmlQualifiedName);
			}
			return FindType(xmlQualifiedName);
		}

		private void HandleXsiType(string typename)
		{
			XmlSchemaElement element = Context.Element;
			object xsiType = GetXsiType(typename);
			if (xsiType == null)
			{
				HandleError("The instance type was not found: " + typename);
				return;
			}
			XmlSchemaType xmlSchemaType = xsiType as XmlSchemaType;
			if (xmlSchemaType != null && Context.Element != null)
			{
				XmlSchemaType xmlSchemaType2 = element.ElementType as XmlSchemaType;
				if (xmlSchemaType2 != null && (xmlSchemaType.DerivedBy & xmlSchemaType2.FinalResolved) != XmlSchemaDerivationMethod.Empty)
				{
					HandleError("The instance type is prohibited by the type of the context element.");
				}
				if (xmlSchemaType2 != xsiType && (xmlSchemaType.DerivedBy & element.BlockResolved) != XmlSchemaDerivationMethod.Empty)
				{
					HandleError("The instance type is prohibited by the context element.");
				}
			}
			XmlSchemaComplexType xmlSchemaComplexType = xsiType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null && xmlSchemaComplexType.IsAbstract)
			{
				HandleError("The instance type is abstract: " + typename);
				return;
			}
			if (element != null)
			{
				AssessLocalTypeDerivationOK(xsiType, element.ElementType, element.BlockResolved);
			}
			Context.XsiType = xsiType;
		}

		private void AssessLocalTypeDerivationOK(object xsiType, object baseType, XmlSchemaDerivationMethod flag)
		{
			XmlSchemaType xmlSchemaType = xsiType as XmlSchemaType;
			XmlSchemaComplexType xmlSchemaComplexType = baseType as XmlSchemaComplexType;
			XmlSchemaComplexType xmlSchemaComplexType2 = xmlSchemaType as XmlSchemaComplexType;
			if (xsiType != baseType)
			{
				if (xmlSchemaComplexType != null)
				{
					flag |= xmlSchemaComplexType.BlockResolved;
				}
				if (flag == XmlSchemaDerivationMethod.All)
				{
					HandleError("Prohibited element type substitution.");
					return;
				}
				if (xmlSchemaType != null && (flag & xmlSchemaType.DerivedBy) != XmlSchemaDerivationMethod.Empty)
				{
					HandleError("Prohibited element type substitution.");
					return;
				}
			}
			if (xmlSchemaComplexType2 != null)
			{
				try
				{
					xmlSchemaComplexType2.ValidateTypeDerivationOK(baseType, null, null);
					return;
				}
				catch (XmlSchemaValidationException exception)
				{
					HandleError(exception);
					return;
				}
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = xsiType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				try
				{
					xmlSchemaSimpleType.ValidateTypeDerivationOK(baseType, null, null, true);
					return;
				}
				catch (XmlSchemaValidationException exception2)
				{
					HandleError(exception2);
					return;
				}
			}
			if (!(xsiType is XmlSchemaDatatype))
			{
				HandleError("Primitive data type cannot be derived type using xsi:type specification.");
			}
		}

		private void HandleXsiNil(string value, XmlSchemaInfo info)
		{
			XmlSchemaElement element = Context.Element;
			if (!element.ActualIsNillable)
			{
				HandleError(string.Format("Current element '{0}' is not nillable and thus does not allow occurence of 'nil' attribute.", Context.Element.QualifiedName));
				return;
			}
			value = value.Trim(XmlChar.WhitespaceChars);
			if (value == "true")
			{
				if (element.ValidatedFixedValue != null)
				{
					HandleError(string.Concat("Schema instance nil was specified, where the element declaration for ", element.QualifiedName, "has fixed value constraints."));
				}
				xsiNilDepth = depth;
				if (info != null)
				{
					info.IsNil = true;
				}
			}
		}

		private XmlSchema ReadExternalSchema(string uri)
		{
			Uri uri2 = new Uri(SourceUri, uri.Trim(XmlChar.WhitespaceChars));
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(uri2.ToString(), (Stream)xmlResolver.GetEntity(uri2, null, typeof(Stream)), nameTable);
				return XmlSchema.Read(xmlTextReader, this.ValidationEventHandler);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		private void HandleSchemaLocation(string schemaLocation)
		{
			if (xmlResolver == null)
			{
				return;
			}
			XmlSchema xmlSchema = null;
			bool flag = false;
			string[] array = null;
			try
			{
				schemaLocation = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Token).Datatype.ParseValue(schemaLocation, null, null) as string;
				array = schemaLocation.Split(XmlChar.WhitespaceChars);
			}
			catch (Exception innerException)
			{
				HandleError("Invalid schemaLocation attribute format.", innerException, true);
				array = new string[0];
			}
			if (array.Length % 2 != 0)
			{
				HandleError("Invalid schemaLocation attribute format.");
			}
			for (int i = 0; i < array.Length; i += 2)
			{
				try
				{
					xmlSchema = ReadExternalSchema(array[i + 1]);
				}
				catch (Exception innerException2)
				{
					HandleError("Could not resolve schema location URI: " + schemaLocation, innerException2, true);
					continue;
				}
				if (xmlSchema.TargetNamespace == null)
				{
					xmlSchema.TargetNamespace = array[i];
				}
				else if (xmlSchema.TargetNamespace != array[i])
				{
					HandleError("Specified schema has different target namespace.");
				}
				if (xmlSchema != null && !schemas.Contains(xmlSchema.TargetNamespace))
				{
					flag = true;
					schemas.Add(xmlSchema);
				}
			}
			if (flag)
			{
				schemas.Compile();
			}
		}

		private void HandleNoNSSchemaLocation(string noNsSchemaLocation)
		{
			if (xmlResolver != null)
			{
				XmlSchema xmlSchema = null;
				bool flag = false;
				try
				{
					xmlSchema = ReadExternalSchema(noNsSchemaLocation);
				}
				catch (Exception innerException)
				{
					HandleError("Could not resolve schema location URI: " + noNsSchemaLocation, innerException, true);
				}
				if (xmlSchema != null && xmlSchema.TargetNamespace != null)
				{
					HandleError("Specified schema has different target namespace.");
				}
				if (xmlSchema != null && !schemas.Contains(xmlSchema.TargetNamespace))
				{
					flag = true;
					schemas.Add(xmlSchema);
				}
				if (flag)
				{
					schemas.Compile();
				}
			}
		}
	}
}
