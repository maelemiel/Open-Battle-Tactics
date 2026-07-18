using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdValidatingReader : XmlReader, IHasXmlParserContext, IHasXmlSchemaInfo, IXmlLineInfo
	{
		private static readonly XmlSchemaAttribute[] emptyAttributeArray = new XmlSchemaAttribute[0];

		private XmlReader reader;

		private XmlResolver resolver;

		private IHasXmlSchemaInfo sourceReaderSchemaInfo;

		private IXmlLineInfo readerLineInfo;

		private ValidationType validationType;

		private XmlSchemaSet schemas = new XmlSchemaSet();

		private bool namespaces = true;

		private bool validationStarted;

		private bool checkIdentity = true;

		private XsdIDManager idManager = new XsdIDManager();

		private bool checkKeyConstraints = true;

		private ArrayList keyTables = new ArrayList();

		private ArrayList currentKeyFieldConsumers;

		private ArrayList tmpKeyrefPool;

		private ArrayList elementQNameStack = new ArrayList();

		private XsdParticleStateManager state = new XsdParticleStateManager();

		private int skipValidationDepth = -1;

		private int xsiNilDepth = -1;

		private StringBuilder storedCharacters = new StringBuilder();

		private bool shouldValidateCharacters;

		private XmlSchemaAttribute[] defaultAttributes = emptyAttributeArray;

		private int currentDefaultAttribute = -1;

		private ArrayList defaultAttributesCache = new ArrayList();

		private bool defaultAttributeConsumed;

		private object currentAttrType;

		public ValidationEventHandler ValidationEventHandler;

		private XsdValidationContext Context
		{
			get
			{
				return state.Context;
			}
		}

		internal ArrayList CurrentKeyFieldConsumers
		{
			get
			{
				if (currentKeyFieldConsumers == null)
				{
					currentKeyFieldConsumers = new ArrayList();
				}
				return currentKeyFieldConsumers;
			}
		}

		public int XsiNilDepth
		{
			get
			{
				return xsiNilDepth;
			}
		}

		public bool Namespaces
		{
			get
			{
				return namespaces;
			}
			set
			{
				namespaces = value;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				resolver = value;
			}
		}

		public XmlSchemaSet Schemas
		{
			get
			{
				return schemas;
			}
			set
			{
				if (validationStarted)
				{
					throw new InvalidOperationException("Schemas must be set before the first call to Read().");
				}
				schemas = value;
			}
		}

		public object SchemaType
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return null;
				}
				switch (NodeType)
				{
				case XmlNodeType.Element:
					if (Context.ActualType != null)
					{
						return Context.ActualType;
					}
					return SourceReaderSchemaType;
				case XmlNodeType.Attribute:
					if (currentAttrType == null)
					{
						XmlSchemaComplexType xmlSchemaComplexType = Context.ActualType as XmlSchemaComplexType;
						if (xmlSchemaComplexType != null)
						{
							XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaComplexType.AttributeUses[new XmlQualifiedName(LocalName, NamespaceURI)] as XmlSchemaAttribute;
							if (xmlSchemaAttribute != null)
							{
								currentAttrType = xmlSchemaAttribute.AttributeType;
							}
							return currentAttrType;
						}
						currentAttrType = SourceReaderSchemaType;
					}
					return currentAttrType;
				default:
					return SourceReaderSchemaType;
				}
			}
		}

		private object SourceReaderSchemaType
		{
			get
			{
				return (sourceReaderSchemaInfo == null) ? null : sourceReaderSchemaInfo.SchemaType;
			}
		}

		public ValidationType ValidationType
		{
			get
			{
				return validationType;
			}
			set
			{
				if (validationStarted)
				{
					throw new InvalidOperationException("ValidationType must be set before reading.");
				}
				validationType = value;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return reader.AttributeCount + defaultAttributes.Length;
			}
		}

		public override string BaseURI
		{
			get
			{
				return reader.BaseURI;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return reader.CanResolveEntity;
			}
		}

		public override int Depth
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.Depth;
				}
				if (defaultAttributeConsumed)
				{
					return reader.Depth + 2;
				}
				return reader.Depth + 1;
			}
		}

		public override bool EOF
		{
			get
			{
				return reader.EOF;
			}
		}

		public override bool HasValue
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.HasValue;
				}
				return true;
			}
		}

		public override bool IsDefault
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.IsDefault;
				}
				return true;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.IsEmptyElement;
				}
				return false;
			}
		}

		public override string this[int i]
		{
			get
			{
				return GetAttribute(i);
			}
		}

		public override string this[string name]
		{
			get
			{
				return GetAttribute(name);
			}
		}

		public override string this[string localName, string ns]
		{
			get
			{
				return GetAttribute(localName, ns);
			}
		}

		public int LineNumber
		{
			get
			{
				return (readerLineInfo != null) ? readerLineInfo.LineNumber : 0;
			}
		}

		public int LinePosition
		{
			get
			{
				return (readerLineInfo != null) ? readerLineInfo.LinePosition : 0;
			}
		}

		public override string LocalName
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.LocalName;
				}
				if (defaultAttributeConsumed)
				{
					return string.Empty;
				}
				return defaultAttributes[currentDefaultAttribute].QualifiedName.Name;
			}
		}

		public override string Name
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.Name;
				}
				if (defaultAttributeConsumed)
				{
					return string.Empty;
				}
				XmlQualifiedName qualifiedName = defaultAttributes[currentDefaultAttribute].QualifiedName;
				string prefix = Prefix;
				if (prefix == string.Empty)
				{
					return qualifiedName.Name;
				}
				return prefix + ":" + qualifiedName.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.NamespaceURI;
				}
				if (defaultAttributeConsumed)
				{
					return string.Empty;
				}
				return defaultAttributes[currentDefaultAttribute].QualifiedName.Namespace;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return reader.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.NodeType;
				}
				if (defaultAttributeConsumed)
				{
					return XmlNodeType.Text;
				}
				return XmlNodeType.Attribute;
			}
		}

		public XmlParserContext ParserContext
		{
			get
			{
				return XmlSchemaUtil.GetParserContext(reader);
			}
		}

		internal XmlNamespaceManager NamespaceManager
		{
			get
			{
				return (ParserContext == null) ? null : ParserContext.NamespaceManager;
			}
		}

		public override string Prefix
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.Prefix;
				}
				if (defaultAttributeConsumed)
				{
					return string.Empty;
				}
				XmlQualifiedName qualifiedName = defaultAttributes[currentDefaultAttribute].QualifiedName;
				string text = ((NamespaceManager == null) ? null : NamespaceManager.LookupPrefix(qualifiedName.Namespace, false));
				if (text == null)
				{
					return string.Empty;
				}
				return text;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return reader.QuoteChar;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return reader.ReadState;
			}
		}

		public override string Value
		{
			get
			{
				if (currentDefaultAttribute < 0)
				{
					return reader.Value;
				}
				string text = defaultAttributes[currentDefaultAttribute].ValidatedDefaultValue;
				if (text == null)
				{
					text = defaultAttributes[currentDefaultAttribute].ValidatedFixedValue;
				}
				return text;
			}
		}

		public override string XmlLang
		{
			get
			{
				string xmlLang = reader.XmlLang;
				if (xmlLang != null)
				{
					return xmlLang;
				}
				int num = FindDefaultAttribute("lang", "http://www.w3.org/XML/1998/namespace");
				if (num < 0)
				{
					return null;
				}
				xmlLang = defaultAttributes[num].ValidatedDefaultValue;
				if (xmlLang == null)
				{
					xmlLang = defaultAttributes[num].ValidatedFixedValue;
				}
				return xmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				XmlSpace xmlSpace = reader.XmlSpace;
				if (xmlSpace != XmlSpace.None)
				{
					return xmlSpace;
				}
				int num = FindDefaultAttribute("space", "http://www.w3.org/XML/1998/namespace");
				if (num < 0)
				{
					return XmlSpace.None;
				}
				string text = defaultAttributes[num].ValidatedDefaultValue;
				if (text == null)
				{
					text = defaultAttributes[num].ValidatedFixedValue;
				}
				return (XmlSpace)(int)Enum.Parse(typeof(XmlSpace), text, false);
			}
		}

		public XsdValidatingReader(XmlReader reader)
		{
			this.reader = reader;
			readerLineInfo = reader as IXmlLineInfo;
			sourceReaderSchemaInfo = reader as IHasXmlSchemaInfo;
			schemas.ValidationEventHandler += ValidationEventHandler;
		}

		public object ReadTypedValue()
		{
			object result = XmlSchemaUtil.ReadTypedValue(this, SchemaType, NamespaceManager, storedCharacters);
			storedCharacters.Length = 0;
			return result;
		}

		private void HandleError(string error)
		{
			HandleError(error, null);
		}

		private void HandleError(string error, Exception innerException)
		{
			HandleError(error, innerException, false);
		}

		private void HandleError(string error, Exception innerException, bool isWarning)
		{
			if (ValidationType != ValidationType.None)
			{
				XmlSchemaValidationException schemaException = new XmlSchemaValidationException(error, this, BaseURI, null, innerException);
				HandleError(schemaException, isWarning);
			}
		}

		private void HandleError(XmlSchemaValidationException schemaException)
		{
			HandleError(schemaException, false);
		}

		private void HandleError(XmlSchemaValidationException schemaException, bool isWarning)
		{
			if (ValidationType != ValidationType.None)
			{
				ValidationEventArgs e = new ValidationEventArgs(schemaException, schemaException.Message, isWarning ? XmlSeverityType.Warning : XmlSeverityType.Error);
				if (ValidationEventHandler != null)
				{
					ValidationEventHandler(this, e);
				}
				else if (e.Severity == XmlSeverityType.Error)
				{
					throw e.Exception;
				}
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

		private void ValidateStartElementParticle()
		{
			if (Context.State != null)
			{
				Context.XsiType = null;
				state.CurrentElement = null;
				Context.EvaluateStartElement(reader.LocalName, reader.NamespaceURI);
				if (Context.IsInvalid)
				{
					HandleError("Invalid start element: " + reader.NamespaceURI + ":" + reader.LocalName);
				}
				Context.PushCurrentElement(state.CurrentElement);
			}
		}

		private void ValidateEndElementParticle()
		{
			if (Context.State != null && !Context.EvaluateEndElement())
			{
				HandleError("Invalid end element: " + reader.Name);
			}
			Context.PopCurrentElement();
			state.PopContext();
		}

		private void ValidateCharacters()
		{
			if (xsiNilDepth >= 0 && xsiNilDepth < reader.Depth)
			{
				HandleError("Element item appeared, while current element context is nil.");
			}
			if (shouldValidateCharacters)
			{
				storedCharacters.Append(reader.Value);
			}
		}

		private void ValidateEndSimpleContent()
		{
			if (shouldValidateCharacters)
			{
				ValidateEndSimpleContentCore();
			}
			shouldValidateCharacters = false;
			storedCharacters.Length = 0;
		}

		private void ValidateEndSimpleContentCore()
		{
			if (Context.ActualType == null)
			{
				return;
			}
			string text = storedCharacters.ToString();
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
							HandleError("Character content not allowed.");
						}
						break;
					case XmlSchemaContentType.Empty:
						if (text.Length > 0)
						{
							HandleError("Character content not allowed.");
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
				AssessStringValid(xmlSchemaSimpleType, xmlSchemaDatatype, text);
			}
			if (checkKeyConstraints)
			{
				ValidateSimpleContentIdentity(xmlSchemaDatatype, text);
			}
			shouldValidateCharacters = false;
		}

		private void AssessStringValid(XmlSchemaSimpleType st, XmlSchemaDatatype dt, string value)
		{
			XmlSchemaDatatype dt2 = dt;
			if (st != null)
			{
				string normalized = dt2.Normalize(value);
				ValidateRestrictedSimpleTypeValue(st, ref dt2, normalized);
			}
			if (dt2 != null)
			{
				try
				{
					dt2.ParseValue(value, NameTable, NamespaceManager);
				}
				catch (Exception innerException)
				{
					HandleError("Invalidly typed data was specified.", innerException);
				}
			}
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
							xmlSchemaDatatype.ParseValue(text, NameTable, NamespaceManager);
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
							xmlSchemaDatatype.ParseValue(normalized, NameTable, NamespaceManager);
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
					if (!xmlSchemaSimpleTypeRestriction.ValidateValueWithFacets(normalized, NameTable, NamespaceManager))
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

		private object GetXsiType(string name)
		{
			object obj = null;
			XmlQualifiedName xmlQualifiedName = XmlQualifiedName.Parse(name, this);
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
				catch (XmlSchemaValidationException schemaException)
				{
					HandleError(schemaException);
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
				catch (XmlSchemaValidationException schemaException2)
				{
					HandleError(schemaException2);
					return;
				}
			}
			if (!(xsiType is XmlSchemaDatatype))
			{
				HandleError("Primitive data type cannot be derived type using xsi:type specification.");
			}
		}

		private void AssessStartElementSchemaValidity()
		{
			if (xsiNilDepth >= 0 && xsiNilDepth < reader.Depth)
			{
				HandleError("Element item appeared, while current element context is nil.");
			}
			ValidateStartElementParticle();
			string text = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
			if (text != null)
			{
				text = text.Trim(XmlChar.WhitespaceChars);
			}
			if (text == "true" && xsiNilDepth < 0)
			{
				xsiNilDepth = reader.Depth;
			}
			string text2 = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
			if (text2 != null)
			{
				text2 = text2.Trim(XmlChar.WhitespaceChars);
				object xsiType = GetXsiType(text2);
				if (xsiType == null)
				{
					HandleError("The instance type was not found: " + text2 + " .");
				}
				else
				{
					XmlSchemaType xmlSchemaType = xsiType as XmlSchemaType;
					if (xmlSchemaType != null && Context.Element != null)
					{
						XmlSchemaType xmlSchemaType2 = Context.Element.ElementType as XmlSchemaType;
						if (xmlSchemaType2 != null && (xmlSchemaType.DerivedBy & xmlSchemaType2.FinalResolved) != XmlSchemaDerivationMethod.Empty)
						{
							HandleError("The instance type is prohibited by the type of the context element.");
						}
						if (xmlSchemaType2 != xsiType && (xmlSchemaType.DerivedBy & Context.Element.BlockResolved) != XmlSchemaDerivationMethod.Empty)
						{
							HandleError("The instance type is prohibited by the context element.");
						}
					}
					XmlSchemaComplexType xmlSchemaComplexType = xsiType as XmlSchemaComplexType;
					if (xmlSchemaComplexType != null && xmlSchemaComplexType.IsAbstract)
					{
						HandleError("The instance type is abstract: " + text2 + " .");
					}
					else
					{
						if (Context.Element != null)
						{
							AssessLocalTypeDerivationOK(xsiType, Context.Element.ElementType, Context.Element.BlockResolved);
						}
						AssessStartElementLocallyValidType(xsiType);
						Context.XsiType = xsiType;
					}
				}
			}
			if (Context.Element == null)
			{
				state.CurrentElement = FindElement(reader.LocalName, reader.NamespaceURI);
				Context.PushCurrentElement(state.CurrentElement);
			}
			if (Context.Element != null)
			{
				if (Context.XsiType == null)
				{
					AssessElementLocallyValidElement(text);
				}
			}
			else
			{
				XmlSchemaContentProcessing processContents = state.ProcessContents;
				if (processContents != XmlSchemaContentProcessing.Skip && processContents != XmlSchemaContentProcessing.Lax && text2 == null && (schemas.Contains(reader.NamespaceURI) || !schemas.MissedSubComponents(reader.NamespaceURI)))
				{
					HandleError(string.Concat("Element declaration for ", new XmlQualifiedName(reader.LocalName, reader.NamespaceURI), " is missing."));
				}
			}
			state.PushContext();
			XsdValidationState xsdValidationState = null;
			if (state.ProcessContents == XmlSchemaContentProcessing.Skip)
			{
				skipValidationDepth = reader.Depth;
			}
			else
			{
				XmlSchemaComplexType xmlSchemaComplexType2 = SchemaType as XmlSchemaComplexType;
				xsdValidationState = ((xmlSchemaComplexType2 != null) ? state.Create(xmlSchemaComplexType2.ValidatableParticle) : ((state.ProcessContents != XmlSchemaContentProcessing.Lax) ? state.Create(XmlSchemaParticle.Empty) : state.Create(XmlSchemaAny.AnyTypeContent)));
			}
			Context.State = xsdValidationState;
			if (checkKeyConstraints)
			{
				ValidateKeySelectors();
				ValidateKeyFields();
			}
		}

		private void AssessElementLocallyValidElement(string xsiNilValue)
		{
			XmlSchemaElement element = Context.Element;
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(reader.LocalName, reader.NamespaceURI);
			if (element == null)
			{
				HandleError("Element declaration is required for " + xmlQualifiedName);
			}
			if (element.ActualIsAbstract)
			{
				HandleError("Abstract element declaration was specified for " + xmlQualifiedName);
			}
			if (!element.ActualIsNillable && xsiNilValue != null)
			{
				HandleError("This element declaration is not nillable: " + xmlQualifiedName);
			}
			else if (xsiNilValue == "true" && element.ValidatedFixedValue != null)
			{
				HandleError(string.Concat("Schema instance nil was specified, where the element declaration for ", xmlQualifiedName, "has fixed value constraints."));
			}
			string attribute = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute != null)
			{
				Context.XsiType = GetXsiType(attribute);
				AssessLocalTypeDerivationOK(Context.XsiType, element.ElementType, element.BlockResolved);
			}
			else
			{
				Context.XsiType = null;
			}
			if (element.ElementType != null)
			{
				AssessStartElementLocallyValidType(SchemaType);
			}
		}

		private void AssessStartElementLocallyValidType(object schemaType)
		{
			if (schemaType == null)
			{
				HandleError("Schema type does not exist.");
				return;
			}
			XmlSchemaComplexType xmlSchemaComplexType = schemaType as XmlSchemaComplexType;
			XmlSchemaSimpleType xmlSchemaSimpleType = schemaType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				while (reader.MoveToNextAttribute())
				{
					if (!(reader.NamespaceURI == "http://www.w3.org/2000/xmlns/"))
					{
						if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema-instance")
						{
							HandleError("Current simple type cannot accept attributes other than schema instance namespace.");
						}
						switch (reader.LocalName)
						{
						case "type":
						case "nil":
						case "schemaLocation":
						case "noNamespaceSchemaLocation":
							continue;
						}
						HandleError("Unknown schema instance namespace attribute: " + reader.LocalName);
					}
				}
				reader.MoveToElement();
			}
			else if (xmlSchemaComplexType != null)
			{
				if (xmlSchemaComplexType.IsAbstract)
				{
					HandleError("Target complex type is abstract.");
				}
				else
				{
					AssessElementLocallyValidComplexType(xmlSchemaComplexType);
				}
			}
		}

		private void AssessElementLocallyValidComplexType(XmlSchemaComplexType cType)
		{
			if (cType.IsAbstract)
			{
				HandleError("Target complex type is abstract.");
			}
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					switch (reader.NamespaceURI)
					{
					case "http://www.w3.org/2000/xmlns/":
					case "http://www.w3.org/2001/XMLSchema-instance":
						continue;
					}
					XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(reader.LocalName, reader.NamespaceURI);
					XmlSchemaObject xmlSchemaObject = XmlSchemaUtil.FindAttributeDeclaration(reader.NamespaceURI, schemas, cType, xmlQualifiedName);
					if (xmlSchemaObject == null)
					{
						HandleError("Attribute declaration was not found for " + xmlQualifiedName);
					}
					XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
					if (xmlSchemaAttribute != null)
					{
						AssessAttributeLocallyValidUse(xmlSchemaAttribute);
						AssessAttributeLocallyValid(xmlSchemaAttribute);
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			foreach (DictionaryEntry attributeUse in cType.AttributeUses)
			{
				XmlSchemaAttribute xmlSchemaAttribute2 = (XmlSchemaAttribute)attributeUse.Value;
				if (reader[xmlSchemaAttribute2.QualifiedName.Name, xmlSchemaAttribute2.QualifiedName.Namespace] == null)
				{
					if (xmlSchemaAttribute2.ValidatedUse == XmlSchemaUse.Required && xmlSchemaAttribute2.ValidatedFixedValue == null)
					{
						HandleError(string.Concat("Required attribute ", xmlSchemaAttribute2.QualifiedName, " was not found."));
					}
					else if (xmlSchemaAttribute2.ValidatedDefaultValue != null || xmlSchemaAttribute2.ValidatedFixedValue != null)
					{
						defaultAttributesCache.Add(xmlSchemaAttribute2);
					}
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
		}

		private void AssessAttributeLocallyValid(XmlSchemaAttribute attr)
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
			if (dt == XmlSchemaSimpleType.AnySimpleType && attr.ValidatedFixedValue == null)
			{
				return;
			}
			string text = dt.Normalize(reader.Value);
			object parsedValue = null;
			XmlSchemaSimpleType xmlSchemaSimpleType = attr.AttributeType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				ValidateRestrictedSimpleTypeValue(xmlSchemaSimpleType, ref dt, text);
			}
			try
			{
				parsedValue = dt.ParseValue(text, reader.NameTable, NamespaceManager);
			}
			catch (Exception innerException)
			{
				HandleError("Attribute value is invalid against its data type " + dt.TokenizedType, innerException);
			}
			if (attr.ValidatedFixedValue != null && attr.ValidatedFixedValue != text)
			{
				HandleError(string.Concat("The value of the attribute ", attr.QualifiedName, " does not match with its fixed value."));
				parsedValue = dt.ParseValue(attr.ValidatedFixedValue, reader.NameTable, NamespaceManager);
			}
			if (checkIdentity)
			{
				string text2 = idManager.AssessEachAttributeIdentityConstraint(dt, parsedValue, ((XmlQualifiedName)elementQNameStack[elementQNameStack.Count - 1]).Name);
				if (text2 != null)
				{
					HandleError(text2);
				}
			}
		}

		private void AssessAttributeLocallyValidUse(XmlSchemaAttribute attr)
		{
			if (attr.ValidatedUse == XmlSchemaUse.Prohibited)
			{
				HandleError(string.Concat("Attribute ", attr.QualifiedName, " is prohibited in this context."));
			}
		}

		private void AssessEndElementSchemaValidity()
		{
			ValidateEndSimpleContent();
			ValidateEndElementParticle();
			if (checkKeyConstraints)
			{
				ValidateEndElementKeyConstraints();
			}
			if (xsiNilDepth == reader.Depth)
			{
				xsiNilDepth = -1;
			}
		}

		private void ValidateEndElementKeyConstraints()
		{
			for (int i = 0; i < keyTables.Count; i++)
			{
				XsdKeyTable xsdKeyTable = keyTables[i] as XsdKeyTable;
				if (xsdKeyTable.StartDepth == reader.Depth)
				{
					EndIdentityValidation(xsdKeyTable);
					continue;
				}
				for (int j = 0; j < xsdKeyTable.Entries.Count; j++)
				{
					XsdKeyEntry xsdKeyEntry = xsdKeyTable.Entries[j];
					if (xsdKeyEntry.StartDepth == reader.Depth)
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
						if (!xsdKeyEntryField.FieldFound && xsdKeyEntryField.FieldFoundDepth == reader.Depth)
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
				if (xsdKeyTable2.StartDepth == reader.Depth)
				{
					keyTables.RemoveAt(l);
					l--;
				}
			}
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
				if (xsdKeyTable.SelectorMatches(elementQNameStack, reader.Depth) != null)
				{
					XsdKeyEntry entry = new XsdKeyEntry(xsdKeyTable, reader.Depth, readerLineInfo);
					xsdKeyTable.Entries.Add(entry);
				}
			}
		}

		private void ValidateKeyFields()
		{
			for (int i = 0; i < keyTables.Count; i++)
			{
				XsdKeyTable xsdKeyTable = (XsdKeyTable)keyTables[i];
				for (int j = 0; j < xsdKeyTable.Entries.Count; j++)
				{
					try
					{
						ProcessKeyEntry(xsdKeyTable.Entries[j]);
					}
					catch (XmlSchemaValidationException schemaException)
					{
						HandleError(schemaException);
					}
				}
			}
		}

		private void ProcessKeyEntry(XsdKeyEntry entry)
		{
			bool isXsiNil = XsiNilDepth == Depth;
			entry.ProcessMatch(false, elementQNameStack, this, NameTable, BaseURI, SchemaType, NamespaceManager, readerLineInfo, Depth, null, null, null, isXsiNil, CurrentKeyFieldConsumers);
			if (!MoveToFirstAttribute())
			{
				return;
			}
			try
			{
				do
				{
					switch (NamespaceURI)
					{
					case "http://www.w3.org/2000/xmlns/":
					case "http://www.w3.org/2001/XMLSchema-instance":
						continue;
					}
					XmlSchemaDatatype xmlSchemaDatatype = SchemaType as XmlSchemaDatatype;
					XmlSchemaSimpleType xmlSchemaSimpleType = SchemaType as XmlSchemaSimpleType;
					if (xmlSchemaDatatype == null && xmlSchemaSimpleType != null)
					{
						xmlSchemaDatatype = xmlSchemaSimpleType.Datatype;
					}
					object obj = null;
					if (xmlSchemaDatatype != null)
					{
						obj = xmlSchemaDatatype.ParseValue(Value, NameTable, NamespaceManager);
					}
					if (obj == null)
					{
						obj = Value;
					}
					entry.ProcessMatch(true, elementQNameStack, this, NameTable, BaseURI, SchemaType, NamespaceManager, readerLineInfo, Depth, LocalName, NamespaceURI, obj, false, CurrentKeyFieldConsumers);
				}
				while (MoveToNextAttribute());
			}
			finally
			{
				MoveToElement();
			}
		}

		private XsdKeyTable CreateNewKeyTable(XmlSchemaIdentityConstraint ident)
		{
			XsdKeyTable xsdKeyTable = new XsdKeyTable(ident);
			xsdKeyTable.StartDepth = reader.Depth;
			keyTables.Add(xsdKeyTable);
			return xsdKeyTable;
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
						obj = dt.ParseValue(value, NameTable, NamespaceManager);
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
				if (!xsdKeyEntryField.SetIdentityField(obj, reader.Depth == xsiNilDepth, dt as XsdAnySimpleType, Depth, readerLineInfo))
				{
					HandleError("Two or more identical key value was found: '" + value + "' .");
				}
				currentKeyFieldConsumers.RemoveAt(0);
			}
		}

		private void EndIdentityValidation(XsdKeyTable seq)
		{
			ArrayList arrayList = null;
			for (int i = 0; i < seq.Entries.Count; i++)
			{
				XsdKeyEntry xsdKeyEntry = seq.Entries[i];
				if (!xsdKeyEntry.KeyFound && seq.SourceSchemaIdentity is XmlSchemaKey)
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add("line " + xsdKeyEntry.SelectorLineNumber + "position " + xsdKeyEntry.SelectorLinePosition);
				}
			}
			if (arrayList != null)
			{
				HandleError("Invalid identity constraints were found. Key was not found. " + string.Join(", ", arrayList.ToArray(typeof(string)) as string[]));
			}
			XmlSchemaKeyref xmlSchemaKeyref = seq.SourceSchemaIdentity as XmlSchemaKeyref;
			if (xmlSchemaKeyref != null)
			{
				EndKeyrefValidation(seq, xmlSchemaKeyref.Target);
			}
		}

		private void EndKeyrefValidation(XsdKeyTable seq, XmlSchemaIdentityConstraint targetIdent)
		{
			for (int num = keyTables.Count - 1; num >= 0; num--)
			{
				XsdKeyTable xsdKeyTable = keyTables[num] as XsdKeyTable;
				if (xsdKeyTable.SourceSchemaIdentity == targetIdent)
				{
					seq.ReferencedKey = xsdKeyTable;
					for (int i = 0; i < seq.FinishedEntries.Count; i++)
					{
						XsdKeyEntry xsdKeyEntry = seq.FinishedEntries[i];
						for (int j = 0; j < xsdKeyTable.FinishedEntries.Count; j++)
						{
							XsdKeyEntry other = xsdKeyTable.FinishedEntries[j];
							if (xsdKeyEntry.CompareIdentity(other))
							{
								xsdKeyEntry.KeyRefFound = true;
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
			ArrayList arrayList = null;
			for (int k = 0; k < seq.FinishedEntries.Count; k++)
			{
				XsdKeyEntry xsdKeyEntry2 = seq.FinishedEntries[k];
				if (!xsdKeyEntry2.KeyRefFound)
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(" line " + xsdKeyEntry2.SelectorLineNumber + ", position " + xsdKeyEntry2.SelectorLinePosition);
				}
			}
			if (arrayList != null)
			{
				HandleError("Invalid identity constraints were found. Referenced key was not found: " + string.Join(" / ", arrayList.ToArray(typeof(string)) as string[]));
			}
		}

		public override void Close()
		{
			reader.Close();
		}

		public override string GetAttribute(int i)
		{
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType == XmlNodeType.DocumentType || nodeType == XmlNodeType.XmlDeclaration)
			{
				return reader.GetAttribute(i);
			}
			if (reader.AttributeCount > i)
			{
				reader.GetAttribute(i);
			}
			int num = i - reader.AttributeCount;
			if (i < AttributeCount)
			{
				return defaultAttributes[num].DefaultValue;
			}
			throw new ArgumentOutOfRangeException("i", i, "Specified attribute index is out of range.");
		}

		public override string GetAttribute(string name)
		{
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType == XmlNodeType.DocumentType || nodeType == XmlNodeType.XmlDeclaration)
			{
				return reader.GetAttribute(name);
			}
			string attribute = reader.GetAttribute(name);
			if (attribute != null)
			{
				return attribute;
			}
			XmlQualifiedName xmlQualifiedName = SplitQName(name);
			return GetDefaultAttribute(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
		}

		private XmlQualifiedName SplitQName(string name)
		{
			if (!XmlChar.IsName(name))
			{
				throw new ArgumentException("Invalid name was specified.", "name");
			}
			Exception innerEx = null;
			XmlQualifiedName result = XmlSchemaUtil.ToQName(reader, name, out innerEx);
			if (innerEx != null)
			{
				return XmlQualifiedName.Empty;
			}
			return result;
		}

		public override string GetAttribute(string localName, string ns)
		{
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType == XmlNodeType.DocumentType || nodeType == XmlNodeType.XmlDeclaration)
			{
				return reader.GetAttribute(localName, ns);
			}
			string attribute = reader.GetAttribute(localName, ns);
			if (attribute != null)
			{
				return attribute;
			}
			return GetDefaultAttribute(localName, ns);
		}

		private string GetDefaultAttribute(string localName, string ns)
		{
			int num = FindDefaultAttribute(localName, ns);
			if (num < 0)
			{
				return null;
			}
			string text = defaultAttributes[num].ValidatedDefaultValue;
			if (text == null)
			{
				text = defaultAttributes[num].ValidatedFixedValue;
			}
			return text;
		}

		private int FindDefaultAttribute(string localName, string ns)
		{
			for (int i = 0; i < defaultAttributes.Length; i++)
			{
				XmlSchemaAttribute xmlSchemaAttribute = defaultAttributes[i];
				if (xmlSchemaAttribute.QualifiedName.Name == localName && (ns == null || xmlSchemaAttribute.QualifiedName.Namespace == ns))
				{
					return i;
				}
			}
			return -1;
		}

		public bool HasLineInfo()
		{
			return readerLineInfo != null && readerLineInfo.HasLineInfo();
		}

		public override string LookupNamespace(string prefix)
		{
			return reader.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType == XmlNodeType.DocumentType || nodeType == XmlNodeType.XmlDeclaration)
			{
				reader.MoveToAttribute(i);
				return;
			}
			currentAttrType = null;
			if (i < reader.AttributeCount)
			{
				reader.MoveToAttribute(i);
				currentDefaultAttribute = -1;
				defaultAttributeConsumed = false;
			}
			if (i < AttributeCount)
			{
				currentDefaultAttribute = i - reader.AttributeCount;
				defaultAttributeConsumed = false;
				return;
			}
			throw new ArgumentOutOfRangeException("i", i, "Attribute index is out of range.");
		}

		public override bool MoveToAttribute(string name)
		{
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType == XmlNodeType.DocumentType || nodeType == XmlNodeType.XmlDeclaration)
			{
				return reader.MoveToAttribute(name);
			}
			currentAttrType = null;
			if (reader.MoveToAttribute(name))
			{
				currentDefaultAttribute = -1;
				defaultAttributeConsumed = false;
				return true;
			}
			return MoveToDefaultAttribute(name, null);
		}

		public override bool MoveToAttribute(string localName, string ns)
		{
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType == XmlNodeType.DocumentType || nodeType == XmlNodeType.XmlDeclaration)
			{
				return reader.MoveToAttribute(localName, ns);
			}
			currentAttrType = null;
			if (reader.MoveToAttribute(localName, ns))
			{
				currentDefaultAttribute = -1;
				defaultAttributeConsumed = false;
				return true;
			}
			return MoveToDefaultAttribute(localName, ns);
		}

		private bool MoveToDefaultAttribute(string localName, string ns)
		{
			int num = FindDefaultAttribute(localName, ns);
			if (num < 0)
			{
				return false;
			}
			currentDefaultAttribute = num;
			defaultAttributeConsumed = false;
			return true;
		}

		public override bool MoveToElement()
		{
			currentDefaultAttribute = -1;
			defaultAttributeConsumed = false;
			currentAttrType = null;
			return reader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType == XmlNodeType.DocumentType || nodeType == XmlNodeType.XmlDeclaration)
			{
				return reader.MoveToFirstAttribute();
			}
			currentAttrType = null;
			if (reader.AttributeCount > 0)
			{
				bool flag = reader.MoveToFirstAttribute();
				if (flag)
				{
					currentDefaultAttribute = -1;
					defaultAttributeConsumed = false;
				}
				return flag;
			}
			if (defaultAttributes.Length > 0)
			{
				currentDefaultAttribute = 0;
				defaultAttributeConsumed = false;
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType == XmlNodeType.DocumentType || nodeType == XmlNodeType.XmlDeclaration)
			{
				return reader.MoveToNextAttribute();
			}
			currentAttrType = null;
			if (currentDefaultAttribute >= 0)
			{
				if (defaultAttributes.Length == currentDefaultAttribute + 1)
				{
					return false;
				}
				currentDefaultAttribute++;
				defaultAttributeConsumed = false;
				return true;
			}
			if (reader.MoveToNextAttribute())
			{
				currentDefaultAttribute = -1;
				defaultAttributeConsumed = false;
				return true;
			}
			if (defaultAttributes.Length > 0)
			{
				currentDefaultAttribute = 0;
				defaultAttributeConsumed = false;
				return true;
			}
			return false;
		}

		private XmlSchema ReadExternalSchema(string uri)
		{
			Uri uri2 = resolver.ResolveUri((!(BaseURI != string.Empty)) ? null : new Uri(BaseURI), uri);
			string url = ((!(uri2 != null)) ? string.Empty : uri2.ToString());
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(url, (Stream)resolver.GetEntity(uri2, null, typeof(Stream)), NameTable);
				return XmlSchema.Read(xmlTextReader, ValidationEventHandler);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		private void ExamineAdditionalSchema()
		{
			if (resolver == null || ValidationType == ValidationType.None)
			{
				return;
			}
			XmlSchema xmlSchema = null;
			string attribute = reader.GetAttribute("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
			bool flag = false;
			if (attribute != null)
			{
				string[] array = null;
				try
				{
					attribute = XmlSchemaDatatype.FromName("token", "http://www.w3.org/2001/XMLSchema").Normalize(attribute);
					array = attribute.Split(XmlChar.WhitespaceChars);
				}
				catch (Exception innerException)
				{
					if (schemas.Count == 0)
					{
						HandleError("Invalid schemaLocation attribute format.", innerException, true);
					}
					array = new string[0];
				}
				if (array.Length % 2 != 0 && schemas.Count == 0)
				{
					HandleError("Invalid schemaLocation attribute format.");
				}
				int i = 0;
				do
				{
					try
					{
						for (; i < array.Length; i += 2)
						{
							xmlSchema = ReadExternalSchema(array[i + 1]);
							if (xmlSchema.TargetNamespace == null)
							{
								xmlSchema.TargetNamespace = array[i];
							}
							else if (xmlSchema.TargetNamespace != array[i])
							{
								HandleError("Specified schema has different target namespace.");
							}
							if (xmlSchema != null)
							{
								if (!schemas.Contains(xmlSchema.TargetNamespace))
								{
									flag = true;
									schemas.Add(xmlSchema);
								}
								xmlSchema = null;
							}
						}
					}
					catch (Exception)
					{
						if (!schemas.Contains(array[i]))
						{
							HandleError(string.Format("Could not resolve schema location URI: {0}", (i + 1 >= array.Length) ? string.Empty : array[i + 1]), null, true);
						}
						i += 2;
					}
				}
				while (i < array.Length);
			}
			string attribute2 = reader.GetAttribute("noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute2 != null)
			{
				try
				{
					xmlSchema = ReadExternalSchema(attribute2);
				}
				catch (Exception)
				{
					if (schemas.Count != 0)
					{
						HandleError("Could not resolve schema location URI: " + attribute2, null, true);
					}
				}
				if (xmlSchema != null && xmlSchema.TargetNamespace != null)
				{
					HandleError("Specified schema has different target namespace.");
				}
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

		public override bool Read()
		{
			validationStarted = true;
			currentDefaultAttribute = -1;
			defaultAttributeConsumed = false;
			currentAttrType = null;
			defaultAttributes = emptyAttributeArray;
			bool flag = reader.Read();
			if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element)
			{
				DTDValidatingReader dTDValidatingReader = reader as DTDValidatingReader;
				if (dTDValidatingReader != null && dTDValidatingReader.DTD == null)
				{
					reader = dTDValidatingReader.Source;
				}
				ExamineAdditionalSchema();
			}
			if (schemas.Count == 0)
			{
				return flag;
			}
			if (!schemas.IsCompiled)
			{
				schemas.Compile();
			}
			if (checkIdentity)
			{
				idManager.OnStartElement();
			}
			if (!flag && checkIdentity && idManager.HasMissingIDReferences())
			{
				HandleError("There are missing ID references: " + idManager.GetMissingIDString());
			}
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
				if (checkKeyConstraints)
				{
					elementQNameStack.Add(new XmlQualifiedName(reader.LocalName, reader.NamespaceURI));
				}
				if (skipValidationDepth < 0 || reader.Depth <= skipValidationDepth)
				{
					ValidateEndSimpleContent();
					AssessStartElementSchemaValidity();
				}
				if (reader.IsEmptyElement)
				{
					goto case XmlNodeType.EndElement;
				}
				if (xsiNilDepth < reader.Depth)
				{
					shouldValidateCharacters = true;
				}
				break;
			case XmlNodeType.EndElement:
				if (reader.Depth == skipValidationDepth)
				{
					skipValidationDepth = -1;
				}
				else if (skipValidationDepth < 0 || reader.Depth <= skipValidationDepth)
				{
					AssessEndElementSchemaValidity();
				}
				if (checkKeyConstraints)
				{
					elementQNameStack.RemoveAt(elementQNameStack.Count - 1);
				}
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
			{
				if (skipValidationDepth >= 0 && reader.Depth > skipValidationDepth)
				{
					break;
				}
				XmlSchemaComplexType xmlSchemaComplexType = Context.ActualType as XmlSchemaComplexType;
				if (xmlSchemaComplexType != null)
				{
					switch (xmlSchemaComplexType.ContentType)
					{
					case XmlSchemaContentType.ElementOnly:
						if (reader.NodeType != XmlNodeType.Whitespace)
						{
							HandleError(string.Format("Not allowed character content is found (current content model '{0}' is element-only).", xmlSchemaComplexType.QualifiedName));
						}
						break;
					case XmlSchemaContentType.Empty:
						HandleError(string.Format("Not allowed character content is found (current element content model '{0}' is empty).", xmlSchemaComplexType.QualifiedName));
						break;
					}
				}
				ValidateCharacters();
				break;
			}
			}
			return flag;
		}

		public override bool ReadAttributeValue()
		{
			if (currentDefaultAttribute < 0)
			{
				return reader.ReadAttributeValue();
			}
			if (defaultAttributeConsumed)
			{
				return false;
			}
			defaultAttributeConsumed = true;
			return true;
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public override void ResolveEntity()
		{
			reader.ResolveEntity();
		}
	}
}
