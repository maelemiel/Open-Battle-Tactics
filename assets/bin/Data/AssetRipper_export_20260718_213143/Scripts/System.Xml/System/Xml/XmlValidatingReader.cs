using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Schema;
using Mono.Xml;
using Mono.Xml.Schema;

namespace System.Xml
{
	[Obsolete("Use XmlReader created by XmlReader.Create() method using appropriate XmlReaderSettings instead.")]
	public class XmlValidatingReader : XmlReader, IHasXmlParserContext, IXmlLineInfo, IXmlNamespaceResolver
	{
		private EntityHandling entityHandling;

		private XmlReader sourceReader;

		private XmlTextReader xmlTextReader;

		private XmlReader validatingReader;

		private XmlResolver resolver;

		private bool resolverSpecified;

		private ValidationType validationType;

		private XmlSchemaCollection schemas;

		private DTDValidatingReader dtdReader;

		private IHasXmlSchemaInfo schemaInfo;

		private StringBuilder storedCharacters;

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				if (dtdReader != null)
				{
					return dtdReader.ParserContext;
				}
				IHasXmlParserContext hasXmlParserContext = sourceReader as IHasXmlParserContext;
				return (hasXmlParserContext == null) ? null : hasXmlParserContext.ParserContext;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return (validatingReader != null) ? validatingReader.AttributeCount : 0;
			}
		}

		public override string BaseURI
		{
			get
			{
				return (validatingReader != null) ? validatingReader.BaseURI : sourceReader.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
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
				return (validatingReader != null) ? validatingReader.Depth : 0;
			}
		}

		public Encoding Encoding
		{
			get
			{
				if (xmlTextReader != null)
				{
					return xmlTextReader.Encoding;
				}
				throw new NotSupportedException("Encoding is supported only for XmlTextReader.");
			}
		}

		public EntityHandling EntityHandling
		{
			get
			{
				return entityHandling;
			}
			set
			{
				entityHandling = value;
				if (dtdReader != null)
				{
					dtdReader.EntityHandling = value;
				}
			}
		}

		public override bool EOF
		{
			get
			{
				return validatingReader != null && validatingReader.EOF;
			}
		}

		public override bool HasValue
		{
			get
			{
				return validatingReader != null && validatingReader.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return validatingReader != null && validatingReader.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return validatingReader != null && validatingReader.IsEmptyElement;
			}
		}

		public int LineNumber
		{
			get
			{
				if (IsDefault)
				{
					return 0;
				}
				IXmlLineInfo xmlLineInfo = validatingReader as IXmlLineInfo;
				return (xmlLineInfo != null) ? xmlLineInfo.LineNumber : 0;
			}
		}

		public int LinePosition
		{
			get
			{
				if (IsDefault)
				{
					return 0;
				}
				IXmlLineInfo xmlLineInfo = validatingReader as IXmlLineInfo;
				return (xmlLineInfo != null) ? xmlLineInfo.LinePosition : 0;
			}
		}

		public override string LocalName
		{
			get
			{
				if (validatingReader == null)
				{
					return string.Empty;
				}
				if (Namespaces)
				{
					return validatingReader.LocalName;
				}
				return validatingReader.Name;
			}
		}

		public override string Name
		{
			get
			{
				return (validatingReader != null) ? validatingReader.Name : string.Empty;
			}
		}

		public bool Namespaces
		{
			get
			{
				if (xmlTextReader != null)
				{
					return xmlTextReader.Namespaces;
				}
				return true;
			}
			set
			{
				if (ReadState != ReadState.Initial)
				{
					throw new InvalidOperationException("Namespaces have to be set before reading.");
				}
				if (xmlTextReader != null)
				{
					xmlTextReader.Namespaces = value;
					return;
				}
				throw new NotSupportedException("Property 'Namespaces' is supported only for XmlTextReader.");
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (validatingReader == null)
				{
					return string.Empty;
				}
				if (Namespaces)
				{
					return validatingReader.NamespaceURI;
				}
				return string.Empty;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return (validatingReader != null) ? validatingReader.NameTable : sourceReader.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return (validatingReader != null) ? validatingReader.NodeType : XmlNodeType.None;
			}
		}

		public override string Prefix
		{
			get
			{
				return (validatingReader != null) ? validatingReader.Prefix : string.Empty;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return (validatingReader != null) ? validatingReader.QuoteChar : sourceReader.QuoteChar;
			}
		}

		public XmlReader Reader
		{
			get
			{
				return sourceReader;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				if (validatingReader == null)
				{
					return ReadState.Initial;
				}
				return validatingReader.ReadState;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				if (xmlTextReader != null)
				{
					return xmlTextReader.Resolver;
				}
				if (resolverSpecified)
				{
					return resolver;
				}
				return null;
			}
		}

		public XmlSchemaCollection Schemas
		{
			get
			{
				if (schemas == null)
				{
					schemas = new XmlSchemaCollection(NameTable);
				}
				return schemas;
			}
		}

		public object SchemaType
		{
			get
			{
				return schemaInfo.SchemaType;
			}
		}

		[System.MonoTODO]
		public override XmlReaderSettings Settings
		{
			get
			{
				return (validatingReader != null) ? validatingReader.Settings : sourceReader.Settings;
			}
		}

		[System.MonoTODO]
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
					throw new InvalidOperationException("ValidationType cannot be set after the first call to Read method.");
				}
				switch (validationType)
				{
				case ValidationType.None:
				case ValidationType.Auto:
				case ValidationType.DTD:
				case ValidationType.Schema:
					validationType = value;
					break;
				case ValidationType.XDR:
					throw new NotSupportedException();
				}
			}
		}

		public override string Value
		{
			get
			{
				return (validatingReader != null) ? validatingReader.Value : string.Empty;
			}
		}

		public override string XmlLang
		{
			get
			{
				return (validatingReader != null) ? validatingReader.XmlLang : string.Empty;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				resolverSpecified = true;
				resolver = value;
				if (xmlTextReader != null)
				{
					xmlTextReader.XmlResolver = value;
				}
				XsdValidatingReader xsdValidatingReader = validatingReader as XsdValidatingReader;
				if (xsdValidatingReader != null)
				{
					xsdValidatingReader.XmlResolver = value;
				}
				DTDValidatingReader dTDValidatingReader = validatingReader as DTDValidatingReader;
				if (dTDValidatingReader != null)
				{
					dTDValidatingReader.XmlResolver = value;
				}
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return (validatingReader != null) ? validatingReader.XmlSpace : XmlSpace.None;
			}
		}

		public event ValidationEventHandler ValidationEventHandler;

		public XmlValidatingReader(XmlReader reader)
		{
			sourceReader = reader;
			xmlTextReader = reader as XmlTextReader;
			if (xmlTextReader == null)
			{
				resolver = new XmlUrlResolver();
			}
			entityHandling = EntityHandling.ExpandEntities;
			validationType = ValidationType.Auto;
			storedCharacters = new StringBuilder();
		}

		public XmlValidatingReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
			: this(new XmlTextReader(xmlFragment, fragType, context))
		{
		}

		public XmlValidatingReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
			: this(new XmlTextReader(xmlFragment, fragType, context))
		{
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return ((IHasXmlParserContext)this).ParserContext.NamespaceManager.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			IXmlNamespaceResolver xmlNamespaceResolver = null;
			xmlNamespaceResolver = ((validatingReader == null) ? (validatingReader as IXmlNamespaceResolver) : (sourceReader as IXmlNamespaceResolver));
			return (xmlNamespaceResolver == null) ? null : xmlNamespaceResolver.LookupNamespace(ns);
		}

		public override void Close()
		{
			if (validatingReader == null)
			{
				sourceReader.Close();
			}
			else
			{
				validatingReader.Close();
			}
		}

		public override string GetAttribute(int i)
		{
			if (validatingReader == null)
			{
				throw new IndexOutOfRangeException("Reader is not started.");
			}
			return validatingReader[i];
		}

		public override string GetAttribute(string name)
		{
			return (validatingReader != null) ? validatingReader[name] : null;
		}

		public override string GetAttribute(string localName, string namespaceName)
		{
			return (validatingReader != null) ? validatingReader[localName, namespaceName] : null;
		}

		public bool HasLineInfo()
		{
			IXmlLineInfo xmlLineInfo = validatingReader as IXmlLineInfo;
			return xmlLineInfo != null && xmlLineInfo.HasLineInfo();
		}

		public override string LookupNamespace(string prefix)
		{
			if (validatingReader != null)
			{
				return validatingReader.LookupNamespace(prefix);
			}
			return sourceReader.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			if (validatingReader == null)
			{
				throw new IndexOutOfRangeException("Reader is not started.");
			}
			validatingReader.MoveToAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			if (validatingReader == null)
			{
				return false;
			}
			return validatingReader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			if (validatingReader == null)
			{
				return false;
			}
			return validatingReader.MoveToAttribute(localName, namespaceName);
		}

		public override bool MoveToElement()
		{
			if (validatingReader == null)
			{
				return false;
			}
			return validatingReader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			if (validatingReader == null)
			{
				return false;
			}
			return validatingReader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			if (validatingReader == null)
			{
				return false;
			}
			return validatingReader.MoveToNextAttribute();
		}

		[System.MonoTODO]
		public override bool Read()
		{
			if (validatingReader == null)
			{
				switch (ValidationType)
				{
				case ValidationType.DTD:
					validatingReader = (dtdReader = new DTDValidatingReader(sourceReader, this));
					dtdReader.XmlResolver = Resolver;
					break;
				case ValidationType.None:
				case ValidationType.Auto:
				case ValidationType.Schema:
				{
					dtdReader = new DTDValidatingReader(sourceReader, this);
					XsdValidatingReader xsdValidatingReader = new XsdValidatingReader(dtdReader);
					xsdValidatingReader.ValidationEventHandler = (ValidationEventHandler)Delegate.Combine(xsdValidatingReader.ValidationEventHandler, new ValidationEventHandler(OnValidationEvent));
					xsdValidatingReader.ValidationType = ValidationType;
					xsdValidatingReader.Schemas = Schemas.SchemaSet;
					xsdValidatingReader.XmlResolver = Resolver;
					validatingReader = xsdValidatingReader;
					dtdReader.XmlResolver = Resolver;
					break;
				}
				case ValidationType.XDR:
					throw new NotSupportedException();
				}
				schemaInfo = validatingReader as IHasXmlSchemaInfo;
			}
			return validatingReader.Read();
		}

		public override bool ReadAttributeValue()
		{
			if (validatingReader == null)
			{
				return false;
			}
			return validatingReader.ReadAttributeValue();
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public object ReadTypedValue()
		{
			if (dtdReader == null)
			{
				return null;
			}
			XmlSchemaDatatype xmlSchemaDatatype = schemaInfo.SchemaType as XmlSchemaDatatype;
			if (xmlSchemaDatatype == null)
			{
				XmlSchemaType xmlSchemaType = schemaInfo.SchemaType as XmlSchemaType;
				if (xmlSchemaType != null)
				{
					xmlSchemaDatatype = xmlSchemaType.Datatype;
				}
			}
			if (xmlSchemaDatatype == null)
			{
				return null;
			}
			switch (NodeType)
			{
			case XmlNodeType.Element:
			{
				if (IsEmptyElement)
				{
					return null;
				}
				storedCharacters.Length = 0;
				bool flag = true;
				do
				{
					Read();
					switch (NodeType)
					{
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						storedCharacters.Append(Value);
						break;
					default:
						flag = false;
						break;
					case XmlNodeType.Comment:
						break;
					}
				}
				while (flag && !EOF);
				return xmlSchemaDatatype.ParseValue(storedCharacters.ToString(), NameTable, dtdReader.ParserContext.NamespaceManager);
			}
			case XmlNodeType.Attribute:
				return xmlSchemaDatatype.ParseValue(Value, NameTable, dtdReader.ParserContext.NamespaceManager);
			default:
				return null;
			}
		}

		public override void ResolveEntity()
		{
			validatingReader.ResolveEntity();
		}

		internal void OnValidationEvent(object o, ValidationEventArgs e)
		{
			if (this.ValidationEventHandler != null)
			{
				this.ValidationEventHandler(o, e);
			}
			else if (ValidationType != ValidationType.None && e.Severity == XmlSeverityType.Error)
			{
				throw e.Exception;
			}
		}

		[System.MonoTODO]
		public override int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (validatingReader != null)
			{
				return validatingReader.ReadContentAsBase64(buffer, offset, length);
			}
			return sourceReader.ReadContentAsBase64(buffer, offset, length);
		}

		[System.MonoTODO]
		public override int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (validatingReader != null)
			{
				return validatingReader.ReadContentAsBinHex(buffer, offset, length);
			}
			return sourceReader.ReadContentAsBinHex(buffer, offset, length);
		}

		[System.MonoTODO]
		public override int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (validatingReader != null)
			{
				return validatingReader.ReadElementContentAsBase64(buffer, offset, length);
			}
			return sourceReader.ReadElementContentAsBase64(buffer, offset, length);
		}

		[System.MonoTODO]
		public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (validatingReader != null)
			{
				return validatingReader.ReadElementContentAsBinHex(buffer, offset, length);
			}
			return sourceReader.ReadElementContentAsBinHex(buffer, offset, length);
		}
	}
}
