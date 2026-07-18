using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Xml;
using Mono.Xml2;

namespace System.Xml
{
	public class XmlTextReader : XmlReader, IHasXmlParserContext, IXmlLineInfo, IXmlNamespaceResolver
	{
		private XmlTextReader entity;

		private Mono.Xml2.XmlTextReader source;

		private bool entityInsideAttribute;

		private bool insideAttribute;

		private Stack<string> entityNameStack;

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				return ParserContext;
			}
		}

		private XmlReader Current
		{
			get
			{
				return (entity == null || entity.ReadState == ReadState.Initial) ? ((XmlReader)source) : ((XmlReader)entity);
			}
		}

		public override int AttributeCount
		{
			get
			{
				return Current.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return Current.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override bool CanReadValueChunk
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
				if (entity != null && entity.ReadState == ReadState.Interactive)
				{
					return source.Depth + entity.Depth + 1;
				}
				return source.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return source.EOF;
			}
		}

		public override bool HasValue
		{
			get
			{
				return Current.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return Current.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return Current.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return Current.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return Current.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return Current.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return Current.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (entity != null)
				{
					return (entity.ReadState == ReadState.Initial) ? source.NodeType : ((!entity.EOF) ? entity.NodeType : XmlNodeType.EndEntity);
				}
				return source.NodeType;
			}
		}

		internal XmlParserContext ParserContext
		{
			get
			{
				return ((IHasXmlParserContext)Current).ParserContext;
			}
		}

		public override string Prefix
		{
			get
			{
				return Current.Prefix;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return Current.QuoteChar;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return (entity != null) ? ReadState.Interactive : source.ReadState;
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return base.Settings;
			}
		}

		public override string Value
		{
			get
			{
				return Current.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				return Current.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return Current.XmlSpace;
			}
		}

		internal bool CharacterChecking
		{
			get
			{
				if (entity != null)
				{
					return entity.CharacterChecking;
				}
				return source.CharacterChecking;
			}
			set
			{
				if (entity != null)
				{
					entity.CharacterChecking = value;
				}
				source.CharacterChecking = value;
			}
		}

		internal bool CloseInput
		{
			get
			{
				if (entity != null)
				{
					return entity.CloseInput;
				}
				return source.CloseInput;
			}
			set
			{
				if (entity != null)
				{
					entity.CloseInput = value;
				}
				source.CloseInput = value;
			}
		}

		internal ConformanceLevel Conformance
		{
			get
			{
				return source.Conformance;
			}
			set
			{
				if (entity != null)
				{
					entity.Conformance = value;
				}
				source.Conformance = value;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return source.Resolver;
			}
		}

		public Encoding Encoding
		{
			get
			{
				if (entity != null)
				{
					return entity.Encoding;
				}
				return source.Encoding;
			}
		}

		public EntityHandling EntityHandling
		{
			get
			{
				return source.EntityHandling;
			}
			set
			{
				if (entity != null)
				{
					entity.EntityHandling = value;
				}
				source.EntityHandling = value;
			}
		}

		public int LineNumber
		{
			get
			{
				if (entity != null)
				{
					return entity.LineNumber;
				}
				return source.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				if (entity != null)
				{
					return entity.LinePosition;
				}
				return source.LinePosition;
			}
		}

		public bool Namespaces
		{
			get
			{
				return source.Namespaces;
			}
			set
			{
				if (entity != null)
				{
					entity.Namespaces = value;
				}
				source.Namespaces = value;
			}
		}

		public bool Normalization
		{
			get
			{
				return source.Normalization;
			}
			set
			{
				if (entity != null)
				{
					entity.Normalization = value;
				}
				source.Normalization = value;
			}
		}

		public bool ProhibitDtd
		{
			get
			{
				return source.ProhibitDtd;
			}
			set
			{
				if (entity != null)
				{
					entity.ProhibitDtd = value;
				}
				source.ProhibitDtd = value;
			}
		}

		public WhitespaceHandling WhitespaceHandling
		{
			get
			{
				return source.WhitespaceHandling;
			}
			set
			{
				if (entity != null)
				{
					entity.WhitespaceHandling = value;
				}
				source.WhitespaceHandling = value;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				if (entity != null)
				{
					entity.XmlResolver = value;
				}
				source.XmlResolver = value;
			}
		}

		protected XmlTextReader()
		{
		}

		public XmlTextReader(Stream input)
			: this(new XmlStreamReader(input))
		{
		}

		public XmlTextReader(string url)
			: this(url, new NameTable())
		{
		}

		public XmlTextReader(TextReader input)
			: this(input, new NameTable())
		{
		}

		protected XmlTextReader(XmlNameTable nt)
			: this(string.Empty, XmlNodeType.Element, null)
		{
		}

		public XmlTextReader(Stream input, XmlNameTable nt)
			: this(new XmlStreamReader(input), nt)
		{
		}

		public XmlTextReader(string url, Stream input)
			: this(url, new XmlStreamReader(input))
		{
		}

		public XmlTextReader(string url, TextReader input)
			: this(url, input, new NameTable())
		{
		}

		public XmlTextReader(string url, XmlNameTable nt)
		{
			source = new Mono.Xml2.XmlTextReader(url, nt);
		}

		public XmlTextReader(TextReader input, XmlNameTable nt)
			: this(string.Empty, input, nt)
		{
		}

		public XmlTextReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			source = new Mono.Xml2.XmlTextReader(xmlFragment, fragType, context);
		}

		public XmlTextReader(string url, Stream input, XmlNameTable nt)
			: this(url, new XmlStreamReader(input), nt)
		{
		}

		public XmlTextReader(string url, TextReader input, XmlNameTable nt)
		{
			source = new Mono.Xml2.XmlTextReader(url, input, nt);
		}

		public XmlTextReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			source = new Mono.Xml2.XmlTextReader(xmlFragment, fragType, context);
		}

		internal XmlTextReader(string baseURI, TextReader xmlFragment, XmlNodeType fragType)
		{
			source = new Mono.Xml2.XmlTextReader(baseURI, xmlFragment, fragType);
		}

		internal XmlTextReader(string baseURI, TextReader xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			source = new Mono.Xml2.XmlTextReader(baseURI, xmlFragment, fragType, context);
		}

		internal XmlTextReader(bool dummy, XmlResolver resolver, string url, XmlNodeType fragType, XmlParserContext context)
		{
			source = new Mono.Xml2.XmlTextReader(dummy, resolver, url, fragType, context);
		}

		private XmlTextReader(Mono.Xml2.XmlTextReader entityContainer, bool insideAttribute)
		{
			source = entityContainer;
			entityInsideAttribute = insideAttribute;
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			return ((IXmlNamespaceResolver)Current).LookupPrefix(ns);
		}

		private void CopyProperties(XmlTextReader other)
		{
			CharacterChecking = other.CharacterChecking;
			CloseInput = other.CloseInput;
			if (other.Settings != null)
			{
				Conformance = other.Settings.ConformanceLevel;
			}
			XmlResolver = other.Resolver;
		}

		internal void AdjustLineInfoOffset(int lineNumberOffset, int linePositionOffset)
		{
			if (entity != null)
			{
				entity.AdjustLineInfoOffset(lineNumberOffset, linePositionOffset);
			}
			source.AdjustLineInfoOffset(lineNumberOffset, linePositionOffset);
		}

		internal void SetNameTable(XmlNameTable nameTable)
		{
			if (entity != null)
			{
				entity.SetNameTable(nameTable);
			}
			source.SetNameTable(nameTable);
		}

		internal void SkipTextDeclaration()
		{
			if (entity != null)
			{
				entity.SkipTextDeclaration();
			}
			else
			{
				source.SkipTextDeclaration();
			}
		}

		public override void Close()
		{
			if (entity != null)
			{
				entity.Close();
			}
			source.Close();
		}

		public override string GetAttribute(int i)
		{
			return Current.GetAttribute(i);
		}

		public override string GetAttribute(string name)
		{
			return Current.GetAttribute(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return Current.GetAttribute(localName, namespaceURI);
		}

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return ((IXmlNamespaceResolver)Current).GetNamespacesInScope(scope);
		}

		public override string LookupNamespace(string prefix)
		{
			return Current.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			if (entity != null && entityInsideAttribute)
			{
				CloseEntity();
			}
			Current.MoveToAttribute(i);
			insideAttribute = true;
		}

		public override bool MoveToAttribute(string name)
		{
			if (entity != null && !entityInsideAttribute)
			{
				return entity.MoveToAttribute(name);
			}
			if (!source.MoveToAttribute(name))
			{
				return false;
			}
			if (entity != null && entityInsideAttribute)
			{
				CloseEntity();
			}
			insideAttribute = true;
			return true;
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			if (entity != null && !entityInsideAttribute)
			{
				return entity.MoveToAttribute(localName, namespaceName);
			}
			if (!source.MoveToAttribute(localName, namespaceName))
			{
				return false;
			}
			if (entity != null && entityInsideAttribute)
			{
				CloseEntity();
			}
			insideAttribute = true;
			return true;
		}

		public override bool MoveToElement()
		{
			if (entity != null && entityInsideAttribute)
			{
				CloseEntity();
			}
			if (!Current.MoveToElement())
			{
				return false;
			}
			insideAttribute = false;
			return true;
		}

		public override bool MoveToFirstAttribute()
		{
			if (entity != null && !entityInsideAttribute)
			{
				return entity.MoveToFirstAttribute();
			}
			if (!source.MoveToFirstAttribute())
			{
				return false;
			}
			if (entity != null && entityInsideAttribute)
			{
				CloseEntity();
			}
			insideAttribute = true;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (entity != null && !entityInsideAttribute)
			{
				return entity.MoveToNextAttribute();
			}
			if (!source.MoveToNextAttribute())
			{
				return false;
			}
			if (entity != null && entityInsideAttribute)
			{
				CloseEntity();
			}
			insideAttribute = true;
			return true;
		}

		public override bool Read()
		{
			insideAttribute = false;
			if (entity != null && (entityInsideAttribute || entity.EOF))
			{
				CloseEntity();
			}
			if (entity != null)
			{
				if (entity.Read())
				{
					return true;
				}
				if (EntityHandling == EntityHandling.ExpandEntities)
				{
					CloseEntity();
					return Read();
				}
				return true;
			}
			if (!source.Read())
			{
				return false;
			}
			if (EntityHandling == EntityHandling.ExpandEntities && source.NodeType == XmlNodeType.EntityReference)
			{
				ResolveEntity();
				return Read();
			}
			return true;
		}

		public override bool ReadAttributeValue()
		{
			if (entity != null && entityInsideAttribute)
			{
				if (!entity.EOF)
				{
					entity.Read();
					return true;
				}
				CloseEntity();
			}
			return Current.ReadAttributeValue();
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public void ResetState()
		{
			if (entity != null)
			{
				CloseEntity();
			}
			source.ResetState();
		}

		public override void ResolveEntity()
		{
			if (entity != null)
			{
				entity.ResolveEntity();
				return;
			}
			if (source.NodeType != XmlNodeType.EntityReference)
			{
				throw new InvalidOperationException("The current node is not an Entity Reference");
			}
			Mono.Xml2.XmlTextReader xmlTextReader = null;
			if (ParserContext.Dtd != null)
			{
				xmlTextReader = ParserContext.Dtd.GenerateEntityContentReader(source.Name, ParserContext);
			}
			if (xmlTextReader == null)
			{
				throw new XmlException(this, BaseURI, string.Format("Reference to undeclared entity '{0}'.", source.Name));
			}
			if (entityNameStack == null)
			{
				entityNameStack = new Stack<string>();
			}
			else if (entityNameStack.Contains(Name))
			{
				throw new XmlException(string.Format("General entity '{0}' has an invalid recursive reference to itself.", Name));
			}
			entityNameStack.Push(Name);
			entity = new XmlTextReader(xmlTextReader, insideAttribute);
			entity.entityNameStack = entityNameStack;
			entity.CopyProperties(this);
		}

		private void CloseEntity()
		{
			entity.Close();
			entity = null;
			entityNameStack.Pop();
		}

		public override void Skip()
		{
			base.Skip();
		}

		[System.MonoTODO]
		public TextReader GetRemainder()
		{
			if (entity != null)
			{
				entity.Close();
				entity = null;
				entityNameStack.Pop();
			}
			return source.GetRemainder();
		}

		public bool HasLineInfo()
		{
			return true;
		}

		[System.MonoTODO]
		public int ReadBase64(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadBase64(buffer, offset, length);
			}
			return source.ReadBase64(buffer, offset, length);
		}

		[System.MonoTODO]
		public int ReadBinHex(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadBinHex(buffer, offset, length);
			}
			return source.ReadBinHex(buffer, offset, length);
		}

		[System.MonoTODO]
		public int ReadChars(char[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadChars(buffer, offset, length);
			}
			return source.ReadChars(buffer, offset, length);
		}

		[System.MonoTODO]
		public override int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadContentAsBase64(buffer, offset, length);
			}
			return source.ReadContentAsBase64(buffer, offset, length);
		}

		[System.MonoTODO]
		public override int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadContentAsBinHex(buffer, offset, length);
			}
			return source.ReadContentAsBinHex(buffer, offset, length);
		}

		[System.MonoTODO]
		public override int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadElementContentAsBase64(buffer, offset, length);
			}
			return source.ReadElementContentAsBase64(buffer, offset, length);
		}

		[System.MonoTODO]
		public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadElementContentAsBinHex(buffer, offset, length);
			}
			return source.ReadElementContentAsBinHex(buffer, offset, length);
		}
	}
}
