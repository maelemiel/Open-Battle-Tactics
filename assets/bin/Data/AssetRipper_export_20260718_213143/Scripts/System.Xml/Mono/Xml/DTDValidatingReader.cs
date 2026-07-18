using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Mono.Xml2;

namespace Mono.Xml
{
	internal class DTDValidatingReader : XmlReader, IHasXmlParserContext, IHasXmlSchemaInfo, IXmlLineInfo, IXmlNamespaceResolver
	{
		private class AttributeSlot
		{
			public string Name;

			public string LocalName;

			public string NS;

			public string Prefix;

			public string Value;

			public bool IsDefault;

			public void Clear()
			{
				Prefix = string.Empty;
				LocalName = string.Empty;
				NS = string.Empty;
				Value = string.Empty;
				IsDefault = false;
			}
		}

		private EntityResolvingXmlReader reader;

		private System.Xml.XmlTextReader sourceTextReader;

		private XmlValidatingReader validatingReader;

		private DTDObjectModel dtd;

		private XmlResolver resolver;

		private string currentElement;

		private AttributeSlot[] attributes;

		private int attributeCount;

		private int currentAttribute = -1;

		private bool consumedAttribute;

		private Stack elementStack;

		private Stack automataStack;

		private bool popScope;

		private bool isStandalone;

		private DTDAutomata currentAutomata;

		private DTDAutomata previousAutomata;

		private ArrayList idList;

		private ArrayList missingIDReferences;

		private XmlNamespaceManager nsmgr;

		private string currentTextValue;

		private string constructingTextValue;

		private bool shouldResetCurrentTextValue;

		private bool isSignificantWhitespace;

		private bool isWhitespace;

		private bool isText;

		private Stack attributeValueEntityStack = new Stack();

		private StringBuilder valueBuilder;

		private char[] whitespaceChars = new char[1] { ' ' };

		internal EntityResolvingXmlReader Source
		{
			get
			{
				return reader;
			}
		}

		public DTDObjectModel DTD
		{
			get
			{
				return dtd;
			}
		}

		public EntityHandling EntityHandling
		{
			get
			{
				return reader.EntityHandling;
			}
			set
			{
				reader.EntityHandling = value;
			}
		}

		public override int AttributeCount
		{
			get
			{
				if (currentTextValue != null)
				{
					return 0;
				}
				return attributeCount;
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
				return true;
			}
		}

		public override int Depth
		{
			get
			{
				int num = reader.Depth;
				if (currentTextValue != null && reader.NodeType == XmlNodeType.EndElement)
				{
					num++;
				}
				return (!IsDefault) ? num : (num + 1);
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
				return currentAttribute >= 0 || currentTextValue != null || reader.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				if (currentTextValue != null)
				{
					return false;
				}
				if (currentAttribute == -1)
				{
					return false;
				}
				return attributes[currentAttribute].IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				if (currentTextValue != null)
				{
					return false;
				}
				return reader.IsEmptyElement;
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

		public override string this[string name, string ns]
		{
			get
			{
				return GetAttribute(name, ns);
			}
		}

		public int LineNumber
		{
			get
			{
				IXmlLineInfo xmlLineInfo = reader;
				return (xmlLineInfo != null) ? xmlLineInfo.LineNumber : 0;
			}
		}

		public int LinePosition
		{
			get
			{
				IXmlLineInfo xmlLineInfo = reader;
				return (xmlLineInfo != null) ? xmlLineInfo.LinePosition : 0;
			}
		}

		public override string LocalName
		{
			get
			{
				if (currentTextValue != null || consumedAttribute)
				{
					return string.Empty;
				}
				if (NodeType == XmlNodeType.Attribute)
				{
					return attributes[currentAttribute].LocalName;
				}
				return reader.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				if (currentTextValue != null || consumedAttribute)
				{
					return string.Empty;
				}
				if (NodeType == XmlNodeType.Attribute)
				{
					return attributes[currentAttribute].Name;
				}
				return reader.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (currentTextValue != null || consumedAttribute)
				{
					return string.Empty;
				}
				switch (NodeType)
				{
				case XmlNodeType.Attribute:
					return attributes[currentAttribute].NS;
				case XmlNodeType.Element:
				case XmlNodeType.EndElement:
					return nsmgr.LookupNamespace(Prefix);
				default:
					return string.Empty;
				}
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
				if (currentTextValue != null)
				{
					return isSignificantWhitespace ? XmlNodeType.SignificantWhitespace : ((!isWhitespace) ? XmlNodeType.Text : XmlNodeType.Whitespace);
				}
				return consumedAttribute ? XmlNodeType.Text : ((!IsDefault) ? reader.NodeType : XmlNodeType.Attribute);
			}
		}

		public XmlParserContext ParserContext
		{
			get
			{
				return XmlSchemaUtil.GetParserContext(reader);
			}
		}

		public override string Prefix
		{
			get
			{
				if (currentTextValue != null || consumedAttribute)
				{
					return string.Empty;
				}
				if (NodeType == XmlNodeType.Attribute)
				{
					return attributes[currentAttribute].Prefix;
				}
				return reader.Prefix;
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
				if (reader.ReadState == ReadState.EndOfFile && currentTextValue != null)
				{
					return ReadState.Interactive;
				}
				return reader.ReadState;
			}
		}

		public object SchemaType
		{
			get
			{
				if (DTD == null || currentAttribute == -1 || currentElement == null)
				{
					return null;
				}
				DTDAttListDeclaration dTDAttListDeclaration = DTD.AttListDecls[currentElement];
				DTDAttributeDefinition dTDAttributeDefinition = ((dTDAttListDeclaration == null) ? null : dTDAttListDeclaration[attributes[currentAttribute].Name]);
				return (dTDAttributeDefinition == null) ? null : dTDAttributeDefinition.Datatype;
			}
		}

		public override string Value
		{
			get
			{
				if (currentTextValue != null)
				{
					return currentTextValue;
				}
				if (NodeType == XmlNodeType.Attribute || consumedAttribute)
				{
					return attributes[currentAttribute].Value;
				}
				return reader.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				string text = this["xml:lang"];
				return (text == null) ? reader.XmlLang : text;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return resolver;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				if (dtd != null)
				{
					dtd.XmlResolver = value;
				}
				resolver = value;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				switch (this["xml:space"])
				{
				case "preserve":
					return XmlSpace.Preserve;
				case "default":
					return XmlSpace.Default;
				default:
					return reader.XmlSpace;
				}
			}
		}

		public DTDValidatingReader(XmlReader reader)
			: this(reader, null)
		{
		}

		internal DTDValidatingReader(XmlReader reader, XmlValidatingReader validatingReader)
		{
			this.reader = new EntityResolvingXmlReader(reader);
			sourceTextReader = reader as System.Xml.XmlTextReader;
			elementStack = new Stack();
			automataStack = new Stack();
			attributes = new AttributeSlot[10];
			nsmgr = new XmlNamespaceManager(reader.NameTable);
			this.validatingReader = validatingReader;
			valueBuilder = new StringBuilder();
			idList = new ArrayList();
			missingIDReferences = new ArrayList();
			System.Xml.XmlTextReader xmlTextReader = reader as System.Xml.XmlTextReader;
			if (xmlTextReader != null)
			{
				resolver = xmlTextReader.Resolver;
			}
			else
			{
				resolver = new XmlUrlResolver();
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			IXmlNamespaceResolver xmlNamespaceResolver = reader;
			object result;
			if (xmlNamespaceResolver != null)
			{
				IDictionary<string, string> namespacesInScope = xmlNamespaceResolver.GetNamespacesInScope(scope);
				result = namespacesInScope;
			}
			else
			{
				result = new Dictionary<string, string>();
			}
			return (IDictionary<string, string>)result;
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			IXmlLineInfo xmlLineInfo = reader;
			if (xmlLineInfo != null)
			{
				return xmlLineInfo.HasLineInfo();
			}
			return false;
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			IXmlNamespaceResolver xmlNamespaceResolver = reader;
			return (xmlNamespaceResolver == null) ? null : xmlNamespaceResolver.LookupPrefix(ns);
		}

		public override void Close()
		{
			reader.Close();
		}

		private int GetAttributeIndex(string name)
		{
			for (int i = 0; i < attributeCount; i++)
			{
				if (attributes[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		private int GetAttributeIndex(string localName, string ns)
		{
			for (int i = 0; i < attributeCount; i++)
			{
				if (attributes[i].LocalName == localName && attributes[i].NS == ns)
				{
					return i;
				}
			}
			return -1;
		}

		public override string GetAttribute(int i)
		{
			if (currentTextValue != null)
			{
				throw new IndexOutOfRangeException("Specified index is out of range: " + i);
			}
			if (attributeCount <= i)
			{
				throw new IndexOutOfRangeException("Specified index is out of range: " + i);
			}
			return attributes[i].Value;
		}

		public override string GetAttribute(string name)
		{
			if (currentTextValue != null)
			{
				return null;
			}
			int attributeIndex = GetAttributeIndex(name);
			return (attributeIndex >= 0) ? attributes[attributeIndex].Value : null;
		}

		public override string GetAttribute(string name, string ns)
		{
			if (currentTextValue != null)
			{
				return null;
			}
			int attributeIndex = GetAttributeIndex(name, ns);
			return (attributeIndex >= 0) ? attributes[attributeIndex].Value : null;
		}

		public override string LookupNamespace(string prefix)
		{
			string text = nsmgr.LookupNamespace(NameTable.Get(prefix));
			return (!(text == string.Empty)) ? text : null;
		}

		public override void MoveToAttribute(int i)
		{
			if (currentTextValue != null)
			{
				throw new IndexOutOfRangeException("The index is out of range.");
			}
			if (attributeCount <= i)
			{
				throw new IndexOutOfRangeException("The index is out of range.");
			}
			if (i < reader.AttributeCount)
			{
				reader.MoveToAttribute(i);
			}
			currentAttribute = i;
			consumedAttribute = false;
		}

		public override bool MoveToAttribute(string name)
		{
			if (currentTextValue != null)
			{
				return false;
			}
			int attributeIndex = GetAttributeIndex(name);
			if (attributeIndex < 0)
			{
				return false;
			}
			if (attributeIndex < reader.AttributeCount)
			{
				reader.MoveToAttribute(attributeIndex);
			}
			currentAttribute = attributeIndex;
			consumedAttribute = false;
			return true;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			if (currentTextValue != null)
			{
				return false;
			}
			int attributeIndex = GetAttributeIndex(name, ns);
			if (attributeIndex < 0)
			{
				return false;
			}
			if (attributeIndex < reader.AttributeCount)
			{
				reader.MoveToAttribute(attributeIndex);
			}
			currentAttribute = attributeIndex;
			consumedAttribute = false;
			return true;
		}

		public override bool MoveToElement()
		{
			if (currentTextValue != null)
			{
				return false;
			}
			if (!reader.MoveToElement() && !IsDefault)
			{
				return false;
			}
			currentAttribute = -1;
			consumedAttribute = false;
			return true;
		}

		public override bool MoveToFirstAttribute()
		{
			if (currentTextValue != null)
			{
				return false;
			}
			if (attributeCount == 0)
			{
				return false;
			}
			currentAttribute = 0;
			reader.MoveToFirstAttribute();
			consumedAttribute = false;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (currentTextValue != null)
			{
				return false;
			}
			if (currentAttribute == -1)
			{
				return MoveToFirstAttribute();
			}
			if (++currentAttribute == attributeCount)
			{
				currentAttribute--;
				return false;
			}
			if (currentAttribute < reader.AttributeCount)
			{
				reader.MoveToAttribute(currentAttribute);
			}
			consumedAttribute = false;
			return true;
		}

		public override bool Read()
		{
			if (currentTextValue != null)
			{
				shouldResetCurrentTextValue = true;
			}
			if (currentAttribute >= 0)
			{
				MoveToElement();
			}
			currentElement = null;
			currentAttribute = -1;
			consumedAttribute = false;
			attributeCount = 0;
			isWhitespace = false;
			isSignificantWhitespace = false;
			isText = false;
			bool flag = ReadContent() || currentTextValue != null;
			if (!flag && (Settings == null || (Settings.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints) == 0) && missingIDReferences.Count > 0)
			{
				HandleError("Missing ID reference was found: " + string.Join(",", missingIDReferences.ToArray(typeof(string)) as string[]), XmlSeverityType.Error);
				missingIDReferences.Clear();
			}
			if (validatingReader != null)
			{
				EntityHandling = validatingReader.EntityHandling;
			}
			return flag;
		}

		private bool ReadContent()
		{
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return false;
			default:
			{
				if (popScope)
				{
					nsmgr.PopScope();
					popScope = false;
					if (elementStack.Count == 0)
					{
						currentAutomata = null;
					}
				}
				bool flag = !reader.EOF;
				if (shouldResetCurrentTextValue)
				{
					currentTextValue = null;
					shouldResetCurrentTextValue = false;
				}
				else
				{
					flag = reader.Read();
				}
				if (!flag)
				{
					if (elementStack.Count != 0)
					{
						throw new InvalidOperationException("Unexpected end of XmlReader.");
					}
					return false;
				}
				return ProcessContent();
			}
			}
		}

		private bool ProcessContent()
		{
			switch (reader.NodeType)
			{
			case XmlNodeType.XmlDeclaration:
				FillAttributes();
				if (GetAttribute("standalone") == "yes")
				{
					isStandalone = true;
				}
				break;
			case XmlNodeType.DocumentType:
				ReadDoctype();
				break;
			case XmlNodeType.Element:
				if (constructingTextValue != null)
				{
					currentTextValue = constructingTextValue;
					constructingTextValue = null;
					if (isWhitespace)
					{
						ValidateWhitespaceNode();
					}
					return true;
				}
				ProcessStartElement();
				break;
			case XmlNodeType.EndElement:
				if (constructingTextValue != null)
				{
					currentTextValue = constructingTextValue;
					constructingTextValue = null;
					return true;
				}
				ProcessEndElement();
				break;
			case XmlNodeType.CDATA:
				isSignificantWhitespace = (isWhitespace = false);
				isText = true;
				ValidateText();
				if (currentTextValue != null)
				{
					currentTextValue = constructingTextValue;
					constructingTextValue = null;
					return true;
				}
				break;
			case XmlNodeType.SignificantWhitespace:
				if (!isText)
				{
					isSignificantWhitespace = true;
				}
				isWhitespace = false;
				goto case XmlNodeType.DocumentFragment;
			case XmlNodeType.Text:
				isWhitespace = (isSignificantWhitespace = false);
				isText = true;
				goto case XmlNodeType.DocumentFragment;
			case XmlNodeType.DocumentFragment:
				if (reader.NodeType != XmlNodeType.DocumentFragment)
				{
					ValidateText();
				}
				break;
			case XmlNodeType.Whitespace:
				if (!isText && !isSignificantWhitespace)
				{
					isWhitespace = true;
				}
				goto case XmlNodeType.DocumentFragment;
			}
			if (isWhitespace)
			{
				ValidateWhitespaceNode();
			}
			currentTextValue = constructingTextValue;
			constructingTextValue = null;
			return true;
		}

		private void FillAttributes()
		{
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					AttributeSlot attributeSlot = GetAttributeSlot();
					attributeSlot.Name = reader.Name;
					attributeSlot.LocalName = reader.LocalName;
					attributeSlot.Prefix = reader.Prefix;
					attributeSlot.NS = reader.NamespaceURI;
					attributeSlot.Value = reader.Value;
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
		}

		private void ValidateText()
		{
			if (currentAutomata != null)
			{
				DTDElementDeclaration dTDElementDeclaration = null;
				if (elementStack.Count > 0)
				{
					dTDElementDeclaration = dtd.ElementDecls[elementStack.Peek() as string];
				}
				if (dTDElementDeclaration != null && !dTDElementDeclaration.IsMixedContent && !dTDElementDeclaration.IsAny && !isWhitespace)
				{
					HandleError(string.Format("Current element {0} does not allow character data content.", elementStack.Peek() as string), XmlSeverityType.Error);
					currentAutomata = previousAutomata;
				}
			}
		}

		private void ValidateWhitespaceNode()
		{
			if (isStandalone && DTD != null && elementStack.Count > 0)
			{
				DTDElementDeclaration dTDElementDeclaration = DTD.ElementDecls[elementStack.Peek() as string];
				if (dTDElementDeclaration != null && !dTDElementDeclaration.IsInternalSubset && !dTDElementDeclaration.IsMixedContent && !dTDElementDeclaration.IsAny && !dTDElementDeclaration.IsEmpty)
				{
					HandleError("In a standalone document, whitespace cannot appear in an element which is declared to contain only element children.", XmlSeverityType.Error);
				}
			}
		}

		private void HandleError(string message, XmlSeverityType severity)
		{
			if (validatingReader == null || validatingReader.ValidationType != ValidationType.None)
			{
				bool flag = ((IXmlLineInfo)this).HasLineInfo();
				XmlSchemaException ex = new XmlSchemaException(message, flag ? ((IXmlLineInfo)this).LineNumber : 0, flag ? ((IXmlLineInfo)this).LinePosition : 0, null, BaseURI, null);
				HandleError(ex, severity);
			}
		}

		private void HandleError(XmlSchemaException ex, XmlSeverityType severity)
		{
			if (validatingReader == null || validatingReader.ValidationType != ValidationType.None)
			{
				if (validatingReader != null)
				{
					validatingReader.OnValidationEvent(this, new ValidationEventArgs(ex, ex.Message, severity));
				}
				else if (severity == XmlSeverityType.Error)
				{
					throw ex;
				}
			}
		}

		private void ValidateAttributes(DTDAttListDeclaration decl, bool validate)
		{
			DtdValidateAttributes(decl, validate);
			for (int i = 0; i < attributeCount; i++)
			{
				AttributeSlot attributeSlot = attributes[i];
				if (attributeSlot.Name == "xmlns" || attributeSlot.Prefix == "xmlns")
				{
					nsmgr.AddNamespace((!(attributeSlot.Prefix == "xmlns")) ? string.Empty : attributeSlot.LocalName, attributeSlot.Value);
				}
			}
			for (int j = 0; j < attributeCount; j++)
			{
				AttributeSlot attributeSlot2 = attributes[j];
				if (attributeSlot2.Name == "xmlns")
				{
					attributeSlot2.NS = "http://www.w3.org/2000/xmlns/";
				}
				else if (attributeSlot2.Prefix.Length > 0)
				{
					attributeSlot2.NS = LookupNamespace(attributeSlot2.Prefix);
				}
				else
				{
					attributeSlot2.NS = string.Empty;
				}
			}
		}

		private AttributeSlot GetAttributeSlot()
		{
			if (attributeCount == attributes.Length)
			{
				AttributeSlot[] destinationArray = new AttributeSlot[attributeCount << 1];
				Array.Copy(attributes, destinationArray, attributeCount);
				attributes = destinationArray;
			}
			if (attributes[attributeCount] == null)
			{
				attributes[attributeCount] = new AttributeSlot();
			}
			AttributeSlot attributeSlot = attributes[attributeCount];
			attributeSlot.Clear();
			attributeCount++;
			return attributeSlot;
		}

		private void DtdValidateAttributes(DTDAttListDeclaration decl, bool validate)
		{
			while (reader.MoveToNextAttribute())
			{
				string name = reader.Name;
				AttributeSlot attributeSlot = GetAttributeSlot();
				attributeSlot.Name = reader.Name;
				attributeSlot.LocalName = reader.LocalName;
				attributeSlot.Prefix = reader.Prefix;
				XmlReader xmlReader = reader;
				string text = string.Empty;
				while (attributeValueEntityStack.Count >= 0)
				{
					if (!xmlReader.ReadAttributeValue())
					{
						if (attributeValueEntityStack.Count > 0)
						{
							xmlReader = attributeValueEntityStack.Pop() as XmlReader;
							continue;
						}
						break;
					}
					switch (xmlReader.NodeType)
					{
					case XmlNodeType.EntityReference:
					{
						DTDEntityDeclaration dTDEntityDeclaration = DTD.EntityDecls[xmlReader.Name];
						if (dTDEntityDeclaration == null)
						{
							HandleError(string.Format("Referenced entity {0} is not declared.", xmlReader.Name), XmlSeverityType.Error);
							break;
						}
						System.Xml.XmlTextReader xmlTextReader = new System.Xml.XmlTextReader(dTDEntityDeclaration.EntityValue, XmlNodeType.Attribute, ParserContext);
						attributeValueEntityStack.Push(xmlReader);
						xmlReader = xmlTextReader;
						break;
					}
					default:
						text += xmlReader.Value;
						break;
					case XmlNodeType.EndEntity:
						break;
					}
				}
				reader.MoveToElement();
				reader.MoveToAttribute(name);
				attributeSlot.Value = FilterNormalization(name, text);
				if (!validate)
				{
					continue;
				}
				DTDAttributeDefinition dTDAttributeDefinition = decl[reader.Name];
				if (dTDAttributeDefinition == null)
				{
					HandleError(string.Format("Attribute {0} is not declared.", reader.Name), XmlSeverityType.Error);
					continue;
				}
				if (dTDAttributeDefinition.EnumeratedAttributeDeclaration.Count > 0 && !dTDAttributeDefinition.EnumeratedAttributeDeclaration.Contains(attributeSlot.Value))
				{
					HandleError(string.Format("Attribute enumeration constraint error in attribute {0}, value {1}.", reader.Name, text), XmlSeverityType.Error);
				}
				if (dTDAttributeDefinition.EnumeratedNotations.Count > 0 && !dTDAttributeDefinition.EnumeratedNotations.Contains(attributeSlot.Value))
				{
					HandleError(string.Format("Attribute notation enumeration constraint error in attribute {0}, value {1}.", reader.Name, text), XmlSeverityType.Error);
				}
				string text2 = null;
				text2 = ((dTDAttributeDefinition.Datatype == null) ? text : FilterNormalization(dTDAttributeDefinition.Name, text));
				string[] array = null;
				switch (dTDAttributeDefinition.Datatype.TokenizedType)
				{
				case XmlTokenizedType.IDREFS:
				case XmlTokenizedType.ENTITIES:
				case XmlTokenizedType.NMTOKENS:
					try
					{
						array = dTDAttributeDefinition.Datatype.ParseValue(text2, NameTable, null) as string[];
					}
					catch (Exception)
					{
						HandleError("Attribute value is invalid against its data type.", XmlSeverityType.Error);
						array = new string[0];
					}
					break;
				default:
					try
					{
						dTDAttributeDefinition.Datatype.ParseValue(text2, NameTable, null);
					}
					catch (Exception ex)
					{
						HandleError(string.Format("Attribute value is invalid against its data type '{0}'. {1}", dTDAttributeDefinition.Datatype, ex.Message), XmlSeverityType.Error);
					}
					break;
				}
				switch (dTDAttributeDefinition.Datatype.TokenizedType)
				{
				case XmlTokenizedType.ID:
					if (idList.Contains(text2))
					{
						HandleError(string.Format("Node with ID {0} was already appeared.", text), XmlSeverityType.Error);
						break;
					}
					if (missingIDReferences.Contains(text2))
					{
						missingIDReferences.Remove(text2);
					}
					idList.Add(text2);
					break;
				case XmlTokenizedType.IDREF:
					if (!idList.Contains(text2))
					{
						missingIDReferences.Add(text2);
					}
					break;
				case XmlTokenizedType.IDREFS:
					foreach (string text3 in array)
					{
						if (!idList.Contains(text3))
						{
							missingIDReferences.Add(text3);
						}
					}
					break;
				case XmlTokenizedType.ENTITY:
				{
					DTDEntityDeclaration dTDEntityDeclaration2 = dtd.EntityDecls[text2];
					if (dTDEntityDeclaration2 == null)
					{
						HandleError("Reference to undeclared entity was found in attribute: " + reader.Name + ".", XmlSeverityType.Error);
					}
					else if (dTDEntityDeclaration2.NotationName == null)
					{
						HandleError("The entity specified by entity type value must be an unparsed entity. The entity definition has no NDATA in attribute: " + reader.Name + ".", XmlSeverityType.Error);
					}
					break;
				}
				case XmlTokenizedType.ENTITIES:
					foreach (string rawValue in array)
					{
						DTDEntityDeclaration dTDEntityDeclaration2 = dtd.EntityDecls[FilterNormalization(reader.Name, rawValue)];
						if (dTDEntityDeclaration2 == null)
						{
							HandleError("Reference to undeclared entity was found in attribute: " + reader.Name + ".", XmlSeverityType.Error);
						}
						else if (dTDEntityDeclaration2.NotationName == null)
						{
							HandleError("The entity specified by ENTITIES type value must be an unparsed entity. The entity definition has no NDATA in attribute: " + reader.Name + ".", XmlSeverityType.Error);
						}
					}
					break;
				}
				if (isStandalone && !dTDAttributeDefinition.IsInternalSubset && text != text2)
				{
					HandleError("In standalone document, attribute value characters must not be checked against external definition.", XmlSeverityType.Error);
				}
				if (dTDAttributeDefinition.OccurenceType == DTDAttributeOccurenceType.Fixed && text != dTDAttributeDefinition.DefaultValue)
				{
					HandleError(string.Format("Fixed attribute {0} in element {1} has invalid value {2}.", dTDAttributeDefinition.Name, decl.Name, text), XmlSeverityType.Error);
				}
			}
			if (validate)
			{
				VerifyDeclaredAttributes(decl);
			}
			MoveToElement();
		}

		private void ReadDoctype()
		{
			FillAttributes();
			IHasXmlParserContext hasXmlParserContext = reader;
			if (hasXmlParserContext != null)
			{
				dtd = hasXmlParserContext.ParserContext.Dtd;
			}
			if (dtd == null)
			{
				Mono.Xml2.XmlTextReader xmlTextReader = new Mono.Xml2.XmlTextReader(string.Empty, XmlNodeType.Document, null);
				xmlTextReader.XmlResolver = resolver;
				xmlTextReader.GenerateDTDObjectModel(reader.Name, reader["PUBLIC"], reader["SYSTEM"], reader.Value);
				dtd = xmlTextReader.DTD;
			}
			currentAutomata = dtd.RootAutomata;
			for (int i = 0; i < DTD.Errors.Length; i++)
			{
				HandleError(DTD.Errors[i].Message, XmlSeverityType.Error);
			}
			foreach (DTDEntityDeclaration value in dtd.EntityDecls.Values)
			{
				if (value.NotationName != null && dtd.NotationDecls[value.NotationName] == null)
				{
					HandleError("Target notation was not found for NData in entity declaration " + value.Name + ".", XmlSeverityType.Error);
				}
			}
			foreach (DTDAttListDeclaration value2 in dtd.AttListDecls.Values)
			{
				foreach (DTDAttributeDefinition definition in value2.Definitions)
				{
					if (definition.Datatype.TokenizedType != XmlTokenizedType.NOTATION)
					{
						continue;
					}
					foreach (string enumeratedNotation in definition.EnumeratedNotations)
					{
						if (dtd.NotationDecls[enumeratedNotation] == null)
						{
							HandleError("Target notation was not found for NOTATION typed attribute default " + definition.Name + ".", XmlSeverityType.Error);
						}
					}
				}
			}
		}

		private void ProcessStartElement()
		{
			nsmgr.PushScope();
			popScope = reader.IsEmptyElement;
			elementStack.Push(reader.Name);
			currentElement = Name;
			if (currentAutomata == null)
			{
				ValidateAttributes(null, false);
				if (reader.IsEmptyElement)
				{
					ProcessEndElement();
				}
				return;
			}
			previousAutomata = currentAutomata;
			currentAutomata = currentAutomata.TryStartElement(reader.Name);
			if (currentAutomata == DTD.Invalid)
			{
				HandleError(string.Format("Invalid start element found: {0}", reader.Name), XmlSeverityType.Error);
				currentAutomata = previousAutomata;
			}
			DTDElementDeclaration dTDElementDeclaration = DTD.ElementDecls[reader.Name];
			if (dTDElementDeclaration == null)
			{
				HandleError(string.Format("Element {0} is not declared.", reader.Name), XmlSeverityType.Error);
				currentAutomata = previousAutomata;
			}
			automataStack.Push(currentAutomata);
			if (dTDElementDeclaration != null)
			{
				currentAutomata = dTDElementDeclaration.ContentModel.GetAutomata();
			}
			DTDAttListDeclaration dTDAttListDeclaration = dtd.AttListDecls[currentElement];
			if (dTDAttListDeclaration != null)
			{
				ValidateAttributes(dTDAttListDeclaration, true);
				currentAttribute = -1;
			}
			else
			{
				if (reader.HasAttributes)
				{
					HandleError(string.Format("Attributes are found on element {0} while it has no attribute definitions.", currentElement), XmlSeverityType.Error);
				}
				ValidateAttributes(null, false);
			}
			if (reader.IsEmptyElement)
			{
				ProcessEndElement();
			}
		}

		private void ProcessEndElement()
		{
			popScope = true;
			elementStack.Pop();
			if (currentAutomata != null)
			{
				DTDElementDeclaration dTDElementDeclaration = DTD.ElementDecls[reader.Name];
				if (dTDElementDeclaration == null)
				{
					HandleError(string.Format("Element {0} is not declared.", reader.Name), XmlSeverityType.Error);
				}
				previousAutomata = currentAutomata;
				DTDAutomata dTDAutomata = currentAutomata.TryEndElement();
				if (dTDAutomata == DTD.Invalid)
				{
					HandleError(string.Format("Invalid end element found: {0}", reader.Name), XmlSeverityType.Error);
					currentAutomata = previousAutomata;
				}
				currentAutomata = automataStack.Pop() as DTDAutomata;
			}
		}

		private void VerifyDeclaredAttributes(DTDAttListDeclaration decl)
		{
			for (int i = 0; i < decl.Definitions.Count; i++)
			{
				DTDAttributeDefinition dTDAttributeDefinition = (DTDAttributeDefinition)decl.Definitions[i];
				bool flag = false;
				for (int j = 0; j < attributeCount; j++)
				{
					if (attributes[j].Name == dTDAttributeDefinition.Name)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				if (dTDAttributeDefinition.OccurenceType == DTDAttributeOccurenceType.Required)
				{
					HandleError(string.Format("Required attribute {0} in element {1} not found .", dTDAttributeDefinition.Name, decl.Name), XmlSeverityType.Error);
				}
				else
				{
					if (dTDAttributeDefinition.DefaultValue == null)
					{
						continue;
					}
					if (isStandalone && !dTDAttributeDefinition.IsInternalSubset)
					{
						HandleError("In standalone document, external default value definition must not be applied.", XmlSeverityType.Error);
					}
					switch (validatingReader.ValidationType)
					{
					case ValidationType.Auto:
						if (validatingReader.Schemas.Count == 0)
						{
							break;
						}
						continue;
					case ValidationType.None:
					case ValidationType.DTD:
						break;
					default:
						continue;
					}
					AttributeSlot attributeSlot = GetAttributeSlot();
					attributeSlot.Name = dTDAttributeDefinition.Name;
					int num = dTDAttributeDefinition.Name.IndexOf(':');
					attributeSlot.LocalName = ((num >= 0) ? dTDAttributeDefinition.Name.Substring(num + 1) : dTDAttributeDefinition.Name);
					string prefix = ((num >= 0) ? dTDAttributeDefinition.Name.Substring(0, num) : string.Empty);
					attributeSlot.Prefix = prefix;
					attributeSlot.Value = dTDAttributeDefinition.DefaultValue;
					attributeSlot.IsDefault = true;
				}
			}
		}

		public override bool ReadAttributeValue()
		{
			if (consumedAttribute)
			{
				return false;
			}
			if (NodeType == XmlNodeType.Attribute && EntityHandling == EntityHandling.ExpandEntities)
			{
				consumedAttribute = true;
				return true;
			}
			if (IsDefault)
			{
				consumedAttribute = true;
				return true;
			}
			return reader.ReadAttributeValue();
		}

		public override void ResolveEntity()
		{
			reader.ResolveEntity();
		}

		private string FilterNormalization(string attrName, string rawValue)
		{
			if (DTD == null || sourceTextReader == null || !sourceTextReader.Normalization)
			{
				return rawValue;
			}
			DTDAttributeDefinition dTDAttributeDefinition = dtd.AttListDecls[currentElement].Get(attrName);
			valueBuilder.Append(rawValue);
			valueBuilder.Replace('\r', ' ');
			valueBuilder.Replace('\n', ' ');
			valueBuilder.Replace('\t', ' ');
			try
			{
				if (dTDAttributeDefinition == null || dTDAttributeDefinition.Datatype.TokenizedType == XmlTokenizedType.CDATA)
				{
					return valueBuilder.ToString();
				}
				for (int i = 0; i < valueBuilder.Length; i++)
				{
					if (valueBuilder[i] == ' ')
					{
						while (++i < valueBuilder.Length && valueBuilder[i] == ' ')
						{
							valueBuilder.Remove(i, 1);
						}
					}
				}
				return valueBuilder.ToString().Trim(whitespaceChars);
			}
			finally
			{
				valueBuilder.Length = 0;
			}
		}
	}
}
