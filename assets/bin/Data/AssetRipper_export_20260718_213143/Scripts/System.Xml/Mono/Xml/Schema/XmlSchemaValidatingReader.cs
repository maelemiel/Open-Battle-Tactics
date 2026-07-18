using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XmlSchemaValidatingReader : XmlReader, IHasXmlParserContext, IXmlSchemaInfo, IXmlLineInfo, IXmlNamespaceResolver
	{
		private static readonly XmlSchemaAttribute[] emptyAttributeArray = new XmlSchemaAttribute[0];

		private XmlReader reader;

		private XmlSchemaValidationFlags options;

		private XmlSchemaValidator v;

		private XmlValueGetter getter;

		private XmlSchemaInfo xsinfo;

		private IXmlLineInfo readerLineInfo;

		private ValidationType validationType;

		private IXmlNamespaceResolver nsResolver;

		private XmlSchemaAttribute[] defaultAttributes = emptyAttributeArray;

		private int currentDefaultAttribute = -1;

		private ArrayList defaultAttributesCache = new ArrayList();

		private bool defaultAttributeConsumed;

		private XmlSchemaType currentAttrType;

		private bool validationDone;

		private XmlSchemaElement element;

		int IXmlLineInfo.LineNumber
		{
			get
			{
				return (readerLineInfo != null) ? readerLineInfo.LineNumber : 0;
			}
		}

		int IXmlLineInfo.LinePosition
		{
			get
			{
				return (readerLineInfo != null) ? readerLineInfo.LinePosition : 0;
			}
		}

		public XmlSchemaType ElementSchemaType
		{
			get
			{
				return (element == null) ? null : element.ElementSchemaType;
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

		public XmlSchemaType SchemaType
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
					if (ElementSchemaType != null)
					{
						return ElementSchemaType;
					}
					return null;
				case XmlNodeType.Attribute:
					if (currentAttrType == null)
					{
						XmlSchemaComplexType xmlSchemaComplexType = ElementSchemaType as XmlSchemaComplexType;
						if (xmlSchemaComplexType != null)
						{
							XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaComplexType.AttributeUses[new XmlQualifiedName(LocalName, NamespaceURI)] as XmlSchemaAttribute;
							if (xmlSchemaAttribute != null)
							{
								currentAttrType = xmlSchemaAttribute.AttributeSchemaType;
							}
							return currentAttrType;
						}
					}
					return currentAttrType;
				default:
					return null;
				}
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
				if (ReadState != ReadState.Initial)
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
				string text = nsResolver.LookupPrefix(qualifiedName.Namespace);
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

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this;
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

		public bool IsNil
		{
			get
			{
				return xsinfo.IsNil;
			}
		}

		public XmlSchemaSimpleType MemberType
		{
			get
			{
				return xsinfo.MemberType;
			}
		}

		public XmlSchemaAttribute SchemaAttribute
		{
			get
			{
				return xsinfo.SchemaAttribute;
			}
		}

		public XmlSchemaElement SchemaElement
		{
			get
			{
				return xsinfo.SchemaElement;
			}
		}

		public XmlSchemaValidity Validity
		{
			get
			{
				return xsinfo.Validity;
			}
		}

		public event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				v.ValidationEventHandler += value;
			}
			remove
			{
				v.ValidationEventHandler -= value;
			}
		}

		public XmlSchemaValidatingReader(XmlReader reader, XmlReaderSettings settings)
		{
			XmlSchemaValidatingReader xmlSchemaValidatingReader = this;
			IXmlNamespaceResolver xmlNamespaceResolver = reader as IXmlNamespaceResolver;
			if (xmlNamespaceResolver == null)
			{
				xmlNamespaceResolver = new XmlNamespaceManager(reader.NameTable);
			}
			XmlSchemaSet xmlSchemaSet = settings.Schemas;
			if (xmlSchemaSet == null)
			{
				xmlSchemaSet = new XmlSchemaSet();
			}
			options = settings.ValidationFlags;
			this.reader = reader;
			v = new XmlSchemaValidator(reader.NameTable, xmlSchemaSet, xmlNamespaceResolver, options);
			if (reader.BaseURI != string.Empty)
			{
				v.SourceUri = new Uri(reader.BaseURI);
			}
			readerLineInfo = reader as IXmlLineInfo;
			getter = () => (xmlSchemaValidatingReader.v.CurrentAttributeType != null) ? xmlSchemaValidatingReader.v.CurrentAttributeType.ParseValue(xmlSchemaValidatingReader.Value, xmlSchemaValidatingReader.NameTable, xmlSchemaValidatingReader) : xmlSchemaValidatingReader.Value;
			xsinfo = new XmlSchemaInfo();
			v.LineInfoProvider = this;
			v.ValidationEventSender = reader;
			nsResolver = xmlNamespaceResolver;
			ValidationEventHandler += delegate(object o, ValidationEventArgs e)
			{
				settings.OnValidationError(o, e);
			};
			if (settings != null && settings.Schemas != null)
			{
				v.XmlResolver = settings.Schemas.XmlResolver;
			}
			else
			{
				v.XmlResolver = new XmlUrlResolver();
			}
			v.Initialize();
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			return readerLineInfo != null && readerLineInfo.HasLineInfo();
		}

		private void ResetStateOnRead()
		{
			currentDefaultAttribute = -1;
			defaultAttributeConsumed = false;
			currentAttrType = null;
			defaultAttributes = emptyAttributeArray;
			v.CurrentAttributeType = null;
		}

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			IXmlNamespaceResolver xmlNamespaceResolver = reader as IXmlNamespaceResolver;
			if (xmlNamespaceResolver == null)
			{
				throw new NotSupportedException("The input XmlReader does not implement IXmlNamespaceResolver and thus this validating reader cannot collect in-scope namespaces.");
			}
			return xmlNamespaceResolver.GetNamespacesInScope(scope);
		}

		public string LookupPrefix(string ns)
		{
			return nsResolver.LookupPrefix(ns);
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
			XmlConvert.VerifyName(name);
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

		public override bool Read()
		{
			if (!reader.Read())
			{
				if (!validationDone)
				{
					v.EndValidation();
					validationDone = true;
				}
				return false;
			}
			ResetStateOnRead();
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
			{
				string attribute = reader.GetAttribute("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
				string attribute2 = reader.GetAttribute("noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
				string attribute3 = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
				string attribute4 = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
				v.ValidateElement(reader.LocalName, reader.NamespaceURI, xsinfo, attribute3, attribute4, attribute, attribute2);
				if (reader.MoveToFirstAttribute())
				{
					do
					{
						switch (reader.NamespaceURI)
						{
						default:
						{
							int num;
							if (num != 1)
							{
								break;
							}
							continue;
						}
						case "http://www.w3.org/2001/XMLSchema-instance":
							switch (reader.LocalName)
							{
							case "schemaLocation":
							case "noNamespaceSchemaLocation":
							case "nil":
							case "type":
								continue;
							}
							break;
						}
						v.ValidateAttribute(reader.LocalName, reader.NamespaceURI, getter, xsinfo);
					}
					while (reader.MoveToNextAttribute());
					reader.MoveToElement();
				}
				v.GetUnspecifiedDefaultAttributes(defaultAttributesCache);
				defaultAttributes = (XmlSchemaAttribute[])defaultAttributesCache.ToArray(typeof(XmlSchemaAttribute));
				v.ValidateEndOfAttributes(xsinfo);
				defaultAttributesCache.Clear();
				if (reader.IsEmptyElement)
				{
					goto case XmlNodeType.EndElement;
				}
				break;
			}
			case XmlNodeType.EndElement:
				v.ValidateEndElement(xsinfo);
				break;
			case XmlNodeType.Text:
				v.ValidateText(getter);
				break;
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				v.ValidateWhitespace(getter);
				break;
			}
			return true;
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

		public override void ResolveEntity()
		{
			reader.ResolveEntity();
		}
	}
}
