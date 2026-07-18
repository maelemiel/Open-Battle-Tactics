using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Mono.Xml;

namespace Mono.Xml2
{
	internal class XmlTextReader : XmlReader, IHasXmlParserContext, IXmlLineInfo, IXmlNamespaceResolver
	{
		internal class XmlTokenInfo
		{
			private string valueCache;

			protected XmlTextReader Reader;

			public string Name;

			public string LocalName;

			public string Prefix;

			public string NamespaceURI;

			public bool IsEmptyElement;

			public char QuoteChar;

			public int LineNumber;

			public int LinePosition;

			public int ValueBufferStart;

			public int ValueBufferEnd;

			public XmlNodeType NodeType;

			public virtual string Value
			{
				get
				{
					if (valueCache != null)
					{
						return valueCache;
					}
					if (ValueBufferStart >= 0)
					{
						valueCache = Reader.valueBuffer.ToString(ValueBufferStart, ValueBufferEnd - ValueBufferStart);
						return valueCache;
					}
					switch (NodeType)
					{
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.ProcessingInstruction:
					case XmlNodeType.Comment:
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						valueCache = Reader.CreateValueString();
						return valueCache;
					default:
						return null;
					}
				}
				set
				{
					valueCache = value;
				}
			}

			public XmlTokenInfo(XmlTextReader xtr)
			{
				Reader = xtr;
				Clear();
			}

			public virtual void Clear()
			{
				ValueBufferStart = -1;
				valueCache = null;
				NodeType = XmlNodeType.None;
				Name = (LocalName = (Prefix = (NamespaceURI = string.Empty)));
				IsEmptyElement = false;
				QuoteChar = '"';
				LineNumber = (LinePosition = 0);
			}
		}

		internal class XmlAttributeTokenInfo : XmlTokenInfo
		{
			public int ValueTokenStartIndex;

			public int ValueTokenEndIndex;

			private string valueCache;

			private StringBuilder tmpBuilder = new StringBuilder();

			public override string Value
			{
				get
				{
					if (valueCache != null)
					{
						return valueCache;
					}
					if (ValueTokenStartIndex == ValueTokenEndIndex)
					{
						XmlTokenInfo xmlTokenInfo = Reader.attributeValueTokens[ValueTokenStartIndex];
						if (xmlTokenInfo.NodeType == XmlNodeType.EntityReference)
						{
							valueCache = "&" + xmlTokenInfo.Name + ";";
						}
						else
						{
							valueCache = xmlTokenInfo.Value;
						}
						return valueCache;
					}
					tmpBuilder.Length = 0;
					for (int i = ValueTokenStartIndex; i <= ValueTokenEndIndex; i++)
					{
						XmlTokenInfo xmlTokenInfo2 = Reader.attributeValueTokens[i];
						if (xmlTokenInfo2.NodeType == XmlNodeType.Text)
						{
							tmpBuilder.Append(xmlTokenInfo2.Value);
							continue;
						}
						tmpBuilder.Append('&');
						tmpBuilder.Append(xmlTokenInfo2.Name);
						tmpBuilder.Append(';');
					}
					valueCache = tmpBuilder.ToString(0, tmpBuilder.Length);
					return valueCache;
				}
				set
				{
					valueCache = value;
				}
			}

			public XmlAttributeTokenInfo(XmlTextReader reader)
				: base(reader)
			{
				NodeType = XmlNodeType.Attribute;
			}

			public override void Clear()
			{
				base.Clear();
				valueCache = null;
				NodeType = XmlNodeType.Attribute;
				ValueTokenStartIndex = (ValueTokenEndIndex = 0);
			}

			internal void FillXmlns()
			{
				if (object.ReferenceEquals(Prefix, "xmlns"))
				{
					Reader.nsmgr.AddNamespace(LocalName, Value);
				}
				else if (object.ReferenceEquals(Name, "xmlns"))
				{
					Reader.nsmgr.AddNamespace(string.Empty, Value);
				}
			}

			internal void FillNamespace()
			{
				if (object.ReferenceEquals(Prefix, "xmlns") || object.ReferenceEquals(Name, "xmlns"))
				{
					NamespaceURI = "http://www.w3.org/2000/xmlns/";
				}
				else if (Prefix.Length == 0)
				{
					NamespaceURI = string.Empty;
				}
				else
				{
					NamespaceURI = Reader.LookupNamespace(Prefix, true);
				}
			}
		}

		private struct TagName
		{
			public readonly string Name;

			public readonly string LocalName;

			public readonly string Prefix;

			public TagName(string n, string l, string p)
			{
				Name = n;
				LocalName = l;
				Prefix = p;
			}
		}

		private enum DtdInputState
		{
			Free = 1,
			ElementDecl = 2,
			AttlistDecl = 3,
			EntityDecl = 4,
			NotationDecl = 5,
			PI = 6,
			Comment = 7,
			InsideSingleQuoted = 8,
			InsideDoubleQuoted = 9
		}

		private class DtdInputStateStack
		{
			private Stack intern = new Stack();

			public DtdInputStateStack()
			{
				Push(DtdInputState.Free);
			}

			public DtdInputState Peek()
			{
				return (DtdInputState)(int)intern.Peek();
			}

			public DtdInputState Pop()
			{
				return (DtdInputState)(int)intern.Pop();
			}

			public void Push(DtdInputState val)
			{
				intern.Push(val);
			}
		}

		private const int peekCharCapacity = 1024;

		private XmlTokenInfo cursorToken;

		private XmlTokenInfo currentToken;

		private XmlAttributeTokenInfo currentAttributeToken;

		private XmlTokenInfo currentAttributeValueToken;

		private XmlAttributeTokenInfo[] attributeTokens = new XmlAttributeTokenInfo[10];

		private XmlTokenInfo[] attributeValueTokens = new XmlTokenInfo[10];

		private int currentAttribute;

		private int currentAttributeValue;

		private int attributeCount;

		private XmlParserContext parserContext;

		private XmlNameTable nameTable;

		private XmlNamespaceManager nsmgr;

		private ReadState readState;

		private bool disallowReset;

		private int depth;

		private int elementDepth;

		private bool depthUp;

		private bool popScope;

		private TagName[] elementNames;

		private int elementNameStackPos;

		private bool allowMultipleRoot;

		private bool isStandalone;

		private bool returnEntityReference;

		private string entityReferenceName;

		private StringBuilder valueBuffer;

		private TextReader reader;

		private char[] peekChars;

		private int peekCharsIndex;

		private int peekCharsLength;

		private int curNodePeekIndex;

		private bool preserveCurrentTag;

		private int line;

		private int column;

		private int currentLinkedNodeLineNumber;

		private int currentLinkedNodeLinePosition;

		private bool useProceedingLineInfo;

		private XmlNodeType startNodeType;

		private XmlNodeType currentState;

		private int nestLevel;

		private bool readCharsInProgress;

		private XmlReaderBinarySupport.CharGetter binaryCharGetter;

		private bool namespaces = true;

		private WhitespaceHandling whitespaceHandling;

		private XmlResolver resolver = new XmlUrlResolver();

		private bool normalization;

		private bool checkCharacters;

		private bool prohibitDtd;

		private bool closeInput = true;

		private EntityHandling entityHandling;

		private NameTable whitespacePool;

		private char[] whitespaceCache;

		private DtdInputStateStack stateStack = new DtdInputStateStack();

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				return parserContext;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return attributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return parserContext.BaseURI;
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

		internal bool CharacterChecking
		{
			get
			{
				return checkCharacters;
			}
			set
			{
				checkCharacters = value;
			}
		}

		internal bool CloseInput
		{
			get
			{
				return closeInput;
			}
			set
			{
				closeInput = value;
			}
		}

		public override int Depth
		{
			get
			{
				int num = ((currentToken.NodeType != XmlNodeType.Element) ? (-1) : 0);
				if (currentAttributeValue >= 0)
				{
					return num + elementDepth + 2;
				}
				if (currentAttribute >= 0)
				{
					return num + elementDepth + 1;
				}
				return elementDepth;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return parserContext.Encoding;
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
			}
		}

		public override bool EOF
		{
			get
			{
				return readState == ReadState.EndOfFile;
			}
		}

		public override bool HasValue
		{
			get
			{
				return cursorToken.Value != null;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return false;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return cursorToken.IsEmptyElement;
			}
		}

		public int LineNumber
		{
			get
			{
				if (useProceedingLineInfo)
				{
					return line;
				}
				return cursorToken.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				if (useProceedingLineInfo)
				{
					return column;
				}
				return cursorToken.LinePosition;
			}
		}

		public override string LocalName
		{
			get
			{
				return cursorToken.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return cursorToken.Name;
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
				if (readState != ReadState.Initial)
				{
					throw new InvalidOperationException("Namespaces have to be set before reading.");
				}
				namespaces = value;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return cursorToken.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return nameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return cursorToken.NodeType;
			}
		}

		public bool Normalization
		{
			get
			{
				return normalization;
			}
			set
			{
				normalization = value;
			}
		}

		public override string Prefix
		{
			get
			{
				return cursorToken.Prefix;
			}
		}

		public bool ProhibitDtd
		{
			get
			{
				return prohibitDtd;
			}
			set
			{
				prohibitDtd = value;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return cursorToken.QuoteChar;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return readState;
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
				return (cursorToken.Value == null) ? string.Empty : cursorToken.Value;
			}
		}

		public WhitespaceHandling WhitespaceHandling
		{
			get
			{
				return whitespaceHandling;
			}
			set
			{
				whitespaceHandling = value;
			}
		}

		public override string XmlLang
		{
			get
			{
				return parserContext.XmlLang;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				resolver = value;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return parserContext.XmlSpace;
			}
		}

		internal DTDObjectModel DTD
		{
			get
			{
				return parserContext.Dtd;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return resolver;
			}
		}

		internal ConformanceLevel Conformance
		{
			get
			{
				return allowMultipleRoot ? ConformanceLevel.Fragment : ConformanceLevel.Document;
			}
			set
			{
				if (value == ConformanceLevel.Fragment)
				{
					currentState = XmlNodeType.Element;
					allowMultipleRoot = true;
				}
			}
		}

		private DtdInputState State
		{
			get
			{
				return stateStack.Peek();
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
			: this(string.Empty, null, XmlNodeType.None, null)
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
			string absoluteUriString;
			Stream streamFromUrl = GetStreamFromUrl(url, out absoluteUriString);
			XmlParserContext context = new XmlParserContext(nt, new XmlNamespaceManager(nt), string.Empty, XmlSpace.None);
			InitializeContext(absoluteUriString, context, new XmlStreamReader(streamFromUrl), XmlNodeType.Document);
		}

		public XmlTextReader(TextReader input, XmlNameTable nt)
			: this(string.Empty, input, nt)
		{
		}

		internal XmlTextReader(bool dummy, XmlResolver resolver, string url, XmlNodeType fragType, XmlParserContext context)
		{
			if (resolver == null)
			{
				resolver = new XmlUrlResolver();
			}
			XmlResolver = resolver;
			string absoluteUriString;
			Stream streamFromUrl = GetStreamFromUrl(url, out absoluteUriString);
			InitializeContext(absoluteUriString, context, new XmlStreamReader(streamFromUrl), fragType);
		}

		public XmlTextReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
			: this((context == null) ? string.Empty : context.BaseURI, new XmlStreamReader(xmlFragment), fragType, context)
		{
			disallowReset = true;
		}

		internal XmlTextReader(string baseURI, TextReader xmlFragment, XmlNodeType fragType)
			: this(baseURI, xmlFragment, fragType, null)
		{
		}

		public XmlTextReader(string url, Stream input, XmlNameTable nt)
			: this(url, new XmlStreamReader(input), nt)
		{
		}

		public XmlTextReader(string url, TextReader input, XmlNameTable nt)
			: this(url, input, XmlNodeType.Document, null)
		{
		}

		public XmlTextReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
			: this((context == null) ? string.Empty : context.BaseURI, new StringReader(xmlFragment), fragType, context)
		{
			disallowReset = true;
		}

		internal XmlTextReader(string url, TextReader fragment, XmlNodeType fragType, XmlParserContext context)
		{
			InitializeContext(url, context, fragment, fragType);
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			return LookupPrefix(ns, false);
		}

		private Stream GetStreamFromUrl(string url, out string absoluteUriString)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new ArgumentException("url");
			}
			Uri uri = resolver.ResolveUri(null, url);
			absoluteUriString = ((!(uri != null)) ? string.Empty : uri.ToString());
			return resolver.GetEntity(uri, null, typeof(Stream)) as Stream;
		}

		public override void Close()
		{
			readState = ReadState.Closed;
			cursorToken.Clear();
			currentToken.Clear();
			attributeCount = 0;
			if (closeInput && reader != null)
			{
				reader.Close();
			}
		}

		public override string GetAttribute(int i)
		{
			if (i >= attributeCount)
			{
				throw new ArgumentOutOfRangeException("i is smaller than AttributeCount");
			}
			return attributeTokens[i].Value;
		}

		public override string GetAttribute(string name)
		{
			for (int i = 0; i < attributeCount; i++)
			{
				if (attributeTokens[i].Name == name)
				{
					return attributeTokens[i].Value;
				}
			}
			return null;
		}

		private int GetIndexOfQualifiedAttribute(string localName, string namespaceURI)
		{
			for (int i = 0; i < attributeCount; i++)
			{
				XmlAttributeTokenInfo xmlAttributeTokenInfo = attributeTokens[i];
				if (xmlAttributeTokenInfo.LocalName == localName && xmlAttributeTokenInfo.NamespaceURI == namespaceURI)
				{
					return i;
				}
			}
			return -1;
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			int indexOfQualifiedAttribute = GetIndexOfQualifiedAttribute(localName, namespaceURI);
			if (indexOfQualifiedAttribute < 0)
			{
				return null;
			}
			return attributeTokens[indexOfQualifiedAttribute].Value;
		}

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return nsmgr.GetNamespacesInScope(scope);
		}

		public TextReader GetRemainder()
		{
			if (peekCharsLength < 0)
			{
				return reader;
			}
			return new StringReader(new string(peekChars, peekCharsIndex, peekCharsLength - peekCharsIndex) + reader.ReadToEnd());
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public override string LookupNamespace(string prefix)
		{
			return LookupNamespace(prefix, false);
		}

		private string LookupNamespace(string prefix, bool atomizedNames)
		{
			string text = nsmgr.LookupNamespace(prefix, atomizedNames);
			return (!(text == string.Empty)) ? text : null;
		}

		public string LookupPrefix(string ns, bool atomizedName)
		{
			return nsmgr.LookupPrefix(ns, atomizedName);
		}

		public override void MoveToAttribute(int i)
		{
			if (i >= attributeCount)
			{
				throw new ArgumentOutOfRangeException("attribute index out of range.");
			}
			currentAttribute = i;
			currentAttributeValue = -1;
			cursorToken = attributeTokens[i];
		}

		public override bool MoveToAttribute(string name)
		{
			for (int i = 0; i < attributeCount; i++)
			{
				XmlAttributeTokenInfo xmlAttributeTokenInfo = attributeTokens[i];
				if (xmlAttributeTokenInfo.Name == name)
				{
					MoveToAttribute(i);
					return true;
				}
			}
			return false;
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			int indexOfQualifiedAttribute = GetIndexOfQualifiedAttribute(localName, namespaceName);
			if (indexOfQualifiedAttribute < 0)
			{
				return false;
			}
			MoveToAttribute(indexOfQualifiedAttribute);
			return true;
		}

		public override bool MoveToElement()
		{
			if (currentToken == null)
			{
				return false;
			}
			if (cursorToken == currentToken)
			{
				return false;
			}
			if (currentAttribute >= 0)
			{
				currentAttribute = -1;
				currentAttributeValue = -1;
				cursorToken = currentToken;
				return true;
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			if (attributeCount == 0)
			{
				return false;
			}
			MoveToElement();
			return MoveToNextAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			if (currentAttribute == 0 && attributeCount == 0)
			{
				return false;
			}
			if (currentAttribute + 1 < attributeCount)
			{
				currentAttribute++;
				currentAttributeValue = -1;
				cursorToken = attributeTokens[currentAttribute];
				return true;
			}
			return false;
		}

		public override bool Read()
		{
			if (readState == ReadState.Closed)
			{
				return false;
			}
			curNodePeekIndex = peekCharsIndex;
			preserveCurrentTag = true;
			nestLevel = 0;
			ClearValueBuffer();
			if (startNodeType == XmlNodeType.Attribute)
			{
				if (currentAttribute == 0)
				{
					return false;
				}
				SkipTextDeclaration();
				ClearAttributes();
				IncrementAttributeToken();
				ReadAttributeValueTokens(34);
				cursorToken = attributeTokens[0];
				currentAttributeValue = -1;
				readState = ReadState.Interactive;
				return true;
			}
			if (readState == ReadState.Initial && currentState == XmlNodeType.Element)
			{
				SkipTextDeclaration();
			}
			if (base.Binary != null)
			{
				base.Binary.Reset();
			}
			bool flag = false;
			readState = ReadState.Interactive;
			currentLinkedNodeLineNumber = line;
			currentLinkedNodeLinePosition = column;
			useProceedingLineInfo = true;
			cursorToken = currentToken;
			attributeCount = 0;
			currentAttribute = (currentAttributeValue = -1);
			currentToken.Clear();
			if (depthUp)
			{
				depth++;
				depthUp = false;
			}
			if (readCharsInProgress)
			{
				readCharsInProgress = false;
				return ReadUntilEndTag();
			}
			flag = ReadContent();
			if (!flag && startNodeType == XmlNodeType.Document && currentState != XmlNodeType.EndElement)
			{
				throw NotWFError("Document element did not appear.");
			}
			useProceedingLineInfo = false;
			return flag;
		}

		public override bool ReadAttributeValue()
		{
			if (readState == ReadState.Initial && startNodeType == XmlNodeType.Attribute)
			{
				Read();
			}
			if (currentAttribute < 0)
			{
				return false;
			}
			XmlAttributeTokenInfo xmlAttributeTokenInfo = attributeTokens[currentAttribute];
			if (currentAttributeValue < 0)
			{
				currentAttributeValue = xmlAttributeTokenInfo.ValueTokenStartIndex - 1;
			}
			if (currentAttributeValue < xmlAttributeTokenInfo.ValueTokenEndIndex)
			{
				currentAttributeValue++;
				cursorToken = attributeValueTokens[currentAttributeValue];
				return true;
			}
			return false;
		}

		public int ReadBase64(byte[] buffer, int offset, int length)
		{
			base.BinaryCharGetter = binaryCharGetter;
			try
			{
				return base.Binary.ReadBase64(buffer, offset, length);
			}
			finally
			{
				base.BinaryCharGetter = null;
			}
		}

		public int ReadBinHex(byte[] buffer, int offset, int length)
		{
			base.BinaryCharGetter = binaryCharGetter;
			try
			{
				return base.Binary.ReadBinHex(buffer, offset, length);
			}
			finally
			{
				base.BinaryCharGetter = null;
			}
		}

		public int ReadChars(char[] buffer, int offset, int length)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("Offset must be non-negative integer.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("Length must be non-negative integer.");
			}
			if (buffer.Length < offset + length)
			{
				throw new ArgumentOutOfRangeException("buffer length is smaller than the sum of offset and length.");
			}
			if (IsEmptyElement)
			{
				Read();
				return 0;
			}
			if (!readCharsInProgress && NodeType != XmlNodeType.Element)
			{
				return 0;
			}
			preserveCurrentTag = false;
			readCharsInProgress = true;
			useProceedingLineInfo = true;
			return ReadCharsInternal(buffer, offset, length);
		}

		public void ResetState()
		{
			if (disallowReset)
			{
				throw new InvalidOperationException("Cannot call ResetState when parsing an XML fragment.");
			}
			Clear();
		}

		public override void ResolveEntity()
		{
			throw new InvalidOperationException("XmlTextReader cannot resolve external entities.");
		}

		[System.MonoTODO]
		public override void Skip()
		{
			base.Skip();
		}

		private XmlException NotWFError(string message)
		{
			return new XmlException(this, BaseURI, message);
		}

		private void Init()
		{
			allowMultipleRoot = false;
			elementNames = new TagName[10];
			valueBuffer = new StringBuilder();
			binaryCharGetter = ReadChars;
			checkCharacters = true;
			if (Settings != null)
			{
				checkCharacters = Settings.CheckCharacters;
			}
			prohibitDtd = false;
			closeInput = true;
			entityHandling = EntityHandling.ExpandCharEntities;
			peekCharsIndex = 0;
			if (peekChars == null)
			{
				peekChars = new char[1024];
			}
			peekCharsLength = -1;
			curNodePeekIndex = -1;
			line = 1;
			column = 1;
			currentLinkedNodeLineNumber = (currentLinkedNodeLinePosition = 0);
			Clear();
		}

		private void Clear()
		{
			currentToken = new XmlTokenInfo(this);
			cursorToken = currentToken;
			currentAttribute = -1;
			currentAttributeValue = -1;
			attributeCount = 0;
			readState = ReadState.Initial;
			depth = 0;
			elementDepth = 0;
			depthUp = false;
			popScope = (allowMultipleRoot = false);
			elementNameStackPos = 0;
			isStandalone = false;
			returnEntityReference = false;
			entityReferenceName = string.Empty;
			useProceedingLineInfo = false;
			currentState = XmlNodeType.None;
			readCharsInProgress = false;
		}

		private void InitializeContext(string url, XmlParserContext context, TextReader fragment, XmlNodeType fragType)
		{
			startNodeType = fragType;
			parserContext = context;
			if (context == null)
			{
				XmlNameTable nt = new NameTable();
				parserContext = new XmlParserContext(nt, new XmlNamespaceManager(nt), string.Empty, XmlSpace.None);
			}
			nameTable = parserContext.NameTable;
			nameTable = ((nameTable == null) ? new NameTable() : nameTable);
			nsmgr = parserContext.NamespaceManager;
			nsmgr = ((nsmgr == null) ? new XmlNamespaceManager(nameTable) : nsmgr);
			if (url != null && url.Length > 0)
			{
				Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
				parserContext.BaseURI = uri.ToString();
			}
			Init();
			reader = fragment;
			switch (fragType)
			{
			case XmlNodeType.Attribute:
				reader = new StringReader(fragment.ReadToEnd().Replace("\"", "&quot;"));
				break;
			case XmlNodeType.Element:
				currentState = XmlNodeType.Element;
				allowMultipleRoot = true;
				break;
			case XmlNodeType.Document:
				break;
			default:
				throw new XmlException(string.Format("NodeType {0} is not allowed to create XmlTextReader.", fragType));
			}
		}

		internal void AdjustLineInfoOffset(int lineNumberOffset, int linePositionOffset)
		{
			line += lineNumberOffset;
			column += linePositionOffset;
		}

		internal void SetNameTable(XmlNameTable nameTable)
		{
			parserContext.NameTable = nameTable;
		}

		private void SetProperties(XmlNodeType nodeType, string name, string prefix, string localName, bool isEmptyElement, string value, bool clearAttributes)
		{
			SetTokenProperties(currentToken, nodeType, name, prefix, localName, isEmptyElement, value, clearAttributes);
			currentToken.LineNumber = currentLinkedNodeLineNumber;
			currentToken.LinePosition = currentLinkedNodeLinePosition;
		}

		private void SetTokenProperties(XmlTokenInfo token, XmlNodeType nodeType, string name, string prefix, string localName, bool isEmptyElement, string value, bool clearAttributes)
		{
			token.NodeType = nodeType;
			token.Name = name;
			token.Prefix = prefix;
			token.LocalName = localName;
			token.IsEmptyElement = isEmptyElement;
			token.Value = value;
			elementDepth = depth;
			if (clearAttributes)
			{
				ClearAttributes();
			}
		}

		private void ClearAttributes()
		{
			attributeCount = 0;
			currentAttribute = -1;
			currentAttributeValue = -1;
		}

		private int PeekSurrogate(int c)
		{
			if (peekCharsLength <= peekCharsIndex + 1 && !ReadTextReader(c))
			{
				return c;
			}
			int num = peekChars[peekCharsIndex];
			int num2 = peekChars[peekCharsIndex + 1];
			if ((num & 0xFC00) != 55296 || (num2 & 0xFC00) != 56320)
			{
				return num;
			}
			return 65536 + (num - 55296) * 1024 + (num2 - 56320);
		}

		private int PeekChar()
		{
			if (peekCharsIndex < peekCharsLength)
			{
				int num = peekChars[peekCharsIndex];
				if (num == 0)
				{
					return -1;
				}
				if (num < 55296 || num >= 57343)
				{
					return num;
				}
				return PeekSurrogate(num);
			}
			if (!ReadTextReader(-1))
			{
				return -1;
			}
			return PeekChar();
		}

		private int ReadChar()
		{
			int num = PeekChar();
			peekCharsIndex++;
			if (num >= 65536)
			{
				peekCharsIndex++;
			}
			switch (num)
			{
			case 10:
				line++;
				column = 1;
				break;
			default:
				column++;
				break;
			case -1:
				break;
			}
			return num;
		}

		private void Advance(int ch)
		{
			peekCharsIndex++;
			if (ch >= 65536)
			{
				peekCharsIndex++;
			}
			switch (ch)
			{
			case 10:
				line++;
				column = 1;
				break;
			default:
				column++;
				break;
			case -1:
				break;
			}
		}

		private bool ReadTextReader(int remained)
		{
			if (peekCharsLength < 0)
			{
				peekCharsLength = reader.Read(peekChars, 0, peekChars.Length);
				return peekCharsLength > 0;
			}
			int num = ((remained >= 0) ? 1 : 0);
			int length = peekCharsLength - curNodePeekIndex;
			if (!preserveCurrentTag)
			{
				curNodePeekIndex = 0;
				peekCharsIndex = 0;
			}
			else if (peekCharsLength >= peekChars.Length)
			{
				if (curNodePeekIndex <= peekCharsLength >> 1)
				{
					char[] destinationArray = new char[peekChars.Length * 2];
					Array.Copy(peekChars, curNodePeekIndex, destinationArray, 0, length);
					peekChars = destinationArray;
					curNodePeekIndex = 0;
					peekCharsIndex = length;
				}
				else
				{
					Array.Copy(peekChars, curNodePeekIndex, peekChars, 0, length);
					curNodePeekIndex = 0;
					peekCharsIndex = length;
				}
			}
			if (remained >= 0)
			{
				peekChars[peekCharsIndex] = (char)remained;
			}
			int num2 = peekChars.Length - peekCharsIndex - num;
			if (num2 > 1024)
			{
				num2 = 1024;
			}
			int num3 = reader.Read(peekChars, peekCharsIndex + num, num2);
			int num4 = num + num3;
			peekCharsLength = peekCharsIndex + num4;
			return num4 != 0;
		}

		private bool ReadContent()
		{
			if (popScope)
			{
				nsmgr.PopScope();
				parserContext.PopScope();
				popScope = false;
			}
			if (returnEntityReference)
			{
				SetEntityReferenceProperties();
			}
			else
			{
				int num = PeekChar();
				if (num == -1)
				{
					readState = ReadState.EndOfFile;
					ClearValueBuffer();
					SetProperties(XmlNodeType.None, string.Empty, string.Empty, string.Empty, false, null, true);
					if (depth > 0)
					{
						throw NotWFError("unexpected end of file. Current depth is " + depth);
					}
					return false;
				}
				switch (num)
				{
				case 60:
					Advance(num);
					switch (PeekChar())
					{
					case 47:
						Advance(47);
						ReadEndTag();
						break;
					case 63:
						Advance(63);
						ReadProcessingInstruction();
						break;
					case 33:
						Advance(33);
						ReadDeclaration();
						break;
					default:
						ReadStartTag();
						break;
					}
					break;
				case 9:
				case 10:
				case 13:
				case 32:
					if (!ReadWhitespace())
					{
						return ReadContent();
					}
					break;
				default:
					ReadText(true);
					break;
				}
			}
			return ReadState != ReadState.EndOfFile;
		}

		private void SetEntityReferenceProperties()
		{
			DTDEntityDeclaration dTDEntityDeclaration = ((DTD == null) ? null : DTD.EntityDecls[entityReferenceName]);
			if (isStandalone && (DTD == null || dTDEntityDeclaration == null || !dTDEntityDeclaration.IsInternalSubset))
			{
				throw NotWFError("Standalone document must not contain any references to an non-internally declared entity.");
			}
			if (dTDEntityDeclaration != null && dTDEntityDeclaration.NotationName != null)
			{
				throw NotWFError("Reference to any unparsed entities is not allowed here.");
			}
			ClearValueBuffer();
			SetProperties(XmlNodeType.EntityReference, entityReferenceName, string.Empty, entityReferenceName, false, null, true);
			returnEntityReference = false;
			entityReferenceName = string.Empty;
		}

		private void ReadStartTag()
		{
			if (currentState == XmlNodeType.EndElement)
			{
				throw NotWFError("Multiple document element was detected.");
			}
			currentState = XmlNodeType.Element;
			nsmgr.PushScope();
			currentLinkedNodeLineNumber = line;
			currentLinkedNodeLinePosition = column;
			string prefix;
			string localName;
			string name = ReadName(out prefix, out localName);
			if (currentState == XmlNodeType.EndElement)
			{
				throw NotWFError("document has terminated, cannot open new element");
			}
			bool isEmptyElement = false;
			ClearAttributes();
			SkipWhitespace();
			if (XmlChar.IsFirstNameChar(PeekChar()))
			{
				ReadAttributes(false);
			}
			cursorToken = currentToken;
			for (int i = 0; i < attributeCount; i++)
			{
				attributeTokens[i].FillXmlns();
			}
			for (int j = 0; j < attributeCount; j++)
			{
				attributeTokens[j].FillNamespace();
			}
			if (namespaces)
			{
				for (int k = 0; k < attributeCount; k++)
				{
					if (attributeTokens[k].Prefix == "xmlns" && attributeTokens[k].Value == string.Empty)
					{
						throw NotWFError("Empty namespace URI cannot be mapped to non-empty prefix.");
					}
				}
			}
			for (int l = 0; l < attributeCount; l++)
			{
				for (int m = l + 1; m < attributeCount; m++)
				{
					if (object.ReferenceEquals(attributeTokens[l].Name, attributeTokens[m].Name) || (object.ReferenceEquals(attributeTokens[l].LocalName, attributeTokens[m].LocalName) && object.ReferenceEquals(attributeTokens[l].NamespaceURI, attributeTokens[m].NamespaceURI)))
					{
						throw NotWFError("Attribute name and qualified name must be identical.");
					}
				}
			}
			if (PeekChar() == 47)
			{
				Advance(47);
				isEmptyElement = true;
				popScope = true;
			}
			else
			{
				depthUp = true;
				PushElementName(name, localName, prefix);
			}
			parserContext.PushScope();
			Expect(62);
			SetProperties(XmlNodeType.Element, name, prefix, localName, isEmptyElement, null, false);
			if (prefix.Length > 0)
			{
				currentToken.NamespaceURI = LookupNamespace(prefix, true);
			}
			else if (namespaces)
			{
				currentToken.NamespaceURI = nsmgr.DefaultNamespace;
			}
			if (namespaces)
			{
				if (NamespaceURI == null)
				{
					throw NotWFError(string.Format("'{0}' is undeclared namespace.", Prefix));
				}
				try
				{
					for (int n = 0; n < attributeCount; n++)
					{
						MoveToAttribute(n);
						if (NamespaceURI == null)
						{
							throw NotWFError(string.Format("'{0}' is undeclared namespace.", Prefix));
						}
					}
				}
				finally
				{
					MoveToElement();
				}
			}
			for (int num = 0; num < attributeCount; num++)
			{
				if (!object.ReferenceEquals(attributeTokens[num].Prefix, "xml"))
				{
					continue;
				}
				string localName2 = attributeTokens[num].LocalName;
				string value = attributeTokens[num].Value;
				switch (localName2)
				{
				case "base":
					if (resolver != null)
					{
						Uri baseUri = ((!(BaseURI != string.Empty)) ? null : new Uri(BaseURI));
						Uri uri = resolver.ResolveUri(baseUri, value);
						parserContext.BaseURI = ((!(uri != null)) ? string.Empty : uri.ToString());
					}
					else
					{
						parserContext.BaseURI = value;
					}
					break;
				case "lang":
					parserContext.XmlLang = value;
					break;
				case "space":
					switch (value)
					{
					case "preserve":
						parserContext.XmlSpace = XmlSpace.Preserve;
						break;
					case "default":
						parserContext.XmlSpace = XmlSpace.Default;
						break;
					default:
						throw NotWFError(string.Format("Invalid xml:space value: {0}", value));
					}
					break;
				}
			}
			if (IsEmptyElement)
			{
				CheckCurrentStateUpdate();
			}
		}

		private void PushElementName(string name, string local, string prefix)
		{
			if (elementNames.Length == elementNameStackPos)
			{
				TagName[] destinationArray = new TagName[elementNames.Length * 2];
				Array.Copy(elementNames, 0, destinationArray, 0, elementNameStackPos);
				elementNames = destinationArray;
			}
			elementNames[elementNameStackPos++] = new TagName(name, local, prefix);
		}

		private void ReadEndTag()
		{
			if (currentState != XmlNodeType.Element)
			{
				throw NotWFError("End tag cannot appear in this state.");
			}
			currentLinkedNodeLineNumber = line;
			currentLinkedNodeLinePosition = column;
			if (elementNameStackPos == 0)
			{
				throw NotWFError("closing element without matching opening element");
			}
			TagName tagName = elementNames[--elementNameStackPos];
			Expect(tagName.Name);
			ExpectAfterWhitespace('>');
			depth--;
			SetProperties(XmlNodeType.EndElement, tagName.Name, tagName.Prefix, tagName.LocalName, false, null, true);
			if (tagName.Prefix.Length > 0)
			{
				currentToken.NamespaceURI = LookupNamespace(tagName.Prefix, true);
			}
			else if (namespaces)
			{
				currentToken.NamespaceURI = nsmgr.DefaultNamespace;
			}
			popScope = true;
			CheckCurrentStateUpdate();
		}

		private void CheckCurrentStateUpdate()
		{
			if (depth == 0 && !allowMultipleRoot && (IsEmptyElement || NodeType == XmlNodeType.EndElement))
			{
				currentState = XmlNodeType.EndElement;
			}
		}

		private void AppendValueChar(int ch)
		{
			if (ch < 65535)
			{
				valueBuffer.Append((char)ch);
			}
			else
			{
				AppendSurrogatePairValueChar(ch);
			}
		}

		private void AppendSurrogatePairValueChar(int ch)
		{
			valueBuffer.Append((char)((ch - 65536) / 1024 + 55296));
			valueBuffer.Append((char)((ch - 65536) % 1024 + 56320));
		}

		private string CreateValueString()
		{
			XmlNodeType nodeType = NodeType;
			if (nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
			{
				int length = valueBuffer.Length;
				if (whitespaceCache == null)
				{
					whitespaceCache = new char[32];
				}
				if (length < whitespaceCache.Length)
				{
					if (whitespacePool == null)
					{
						whitespacePool = new NameTable();
					}
					for (int i = 0; i < length; i++)
					{
						whitespaceCache[i] = valueBuffer[i];
					}
					return whitespacePool.Add(whitespaceCache, 0, valueBuffer.Length);
				}
			}
			return (valueBuffer.Capacity >= 100) ? valueBuffer.ToString() : valueBuffer.ToString(0, valueBuffer.Length);
		}

		private void ClearValueBuffer()
		{
			valueBuffer.Length = 0;
		}

		private void ReadText(bool notWhitespace)
		{
			if (currentState != XmlNodeType.Element)
			{
				throw NotWFError("Text node cannot appear in this state.");
			}
			preserveCurrentTag = false;
			if (notWhitespace)
			{
				ClearValueBuffer();
			}
			int num = PeekChar();
			bool flag = false;
			while (num != 60 && num != -1)
			{
				if (num == 38)
				{
					ReadChar();
					num = ReadReference(false);
					if (returnEntityReference)
					{
						break;
					}
				}
				else
				{
					if (normalization && num == 13)
					{
						ReadChar();
						num = PeekChar();
						if (num != 10)
						{
							AppendValueChar(10);
						}
						continue;
					}
					if (CharacterChecking && XmlChar.IsInvalid(num))
					{
						throw NotWFError("Not allowed character was found.");
					}
					num = ReadChar();
				}
				if (num < 65535)
				{
					valueBuffer.Append((char)num);
				}
				else
				{
					AppendSurrogatePairValueChar(num);
				}
				if (num == 93)
				{
					if (flag && PeekChar() == 62)
					{
						throw NotWFError("Inside text content, character sequence ']]>' is not allowed.");
					}
					flag = true;
				}
				else if (flag)
				{
					flag = false;
				}
				num = PeekChar();
				notWhitespace = true;
			}
			if (returnEntityReference && valueBuffer.Length == 0)
			{
				SetEntityReferenceProperties();
				return;
			}
			XmlNodeType nodeType = (notWhitespace ? XmlNodeType.Text : ((XmlSpace != XmlSpace.Preserve) ? XmlNodeType.Whitespace : XmlNodeType.SignificantWhitespace));
			SetProperties(nodeType, string.Empty, string.Empty, string.Empty, false, null, true);
		}

		private int ReadReference(bool ignoreEntityReferences)
		{
			if (PeekChar() == 35)
			{
				Advance(35);
				return ReadCharacterReference();
			}
			return ReadEntityReference(ignoreEntityReferences);
		}

		private int ReadCharacterReference()
		{
			int num = 0;
			if (PeekChar() == 120)
			{
				Advance(120);
				int num2;
				while ((num2 = PeekChar()) != 59 && num2 != -1)
				{
					Advance(num2);
					if (num2 >= 48 && num2 <= 57)
					{
						num = (num << 4) + num2 - 48;
						continue;
					}
					if (num2 >= 65 && num2 <= 70)
					{
						num = (num << 4) + num2 - 65 + 10;
						continue;
					}
					if (num2 >= 97 && num2 <= 102)
					{
						num = (num << 4) + num2 - 97 + 10;
						continue;
					}
					throw NotWFError(string.Format(CultureInfo.InvariantCulture, "invalid hexadecimal digit: {0} (#x{1:X})", (char)num2, num2));
				}
			}
			else
			{
				int num2;
				while ((num2 = PeekChar()) != 59 && num2 != -1)
				{
					Advance(num2);
					if (num2 >= 48 && num2 <= 57)
					{
						num = num * 10 + num2 - 48;
						continue;
					}
					throw NotWFError(string.Format(CultureInfo.InvariantCulture, "invalid decimal digit: {0} (#x{1:X})", (char)num2, num2));
				}
			}
			ReadChar();
			if (CharacterChecking && Normalization && XmlChar.IsInvalid(num))
			{
				throw NotWFError("Referenced character was not allowed in XML. Normalization is " + normalization + ", checkCharacters = " + checkCharacters);
			}
			return num;
		}

		private int ReadEntityReference(bool ignoreEntityReferences)
		{
			string text = ReadName();
			Expect(59);
			int predefinedEntity = XmlChar.GetPredefinedEntity(text);
			if (predefinedEntity >= 0)
			{
				return predefinedEntity;
			}
			if (ignoreEntityReferences)
			{
				AppendValueChar(38);
				for (int i = 0; i < text.Length; i++)
				{
					AppendValueChar(text[i]);
				}
				AppendValueChar(59);
			}
			else
			{
				returnEntityReference = true;
				entityReferenceName = text;
			}
			return -1;
		}

		private void ReadAttributes(bool isXmlDecl)
		{
			int num = -1;
			bool flag = false;
			currentAttribute = -1;
			currentAttributeValue = -1;
			do
			{
				if (!SkipWhitespace() && flag)
				{
					throw NotWFError("Unexpected token. Name is required here.");
				}
				IncrementAttributeToken();
				currentAttributeToken.LineNumber = line;
				currentAttributeToken.LinePosition = column;
				string prefix;
				string localName;
				currentAttributeToken.Name = ReadName(out prefix, out localName);
				currentAttributeToken.Prefix = prefix;
				currentAttributeToken.LocalName = localName;
				ExpectAfterWhitespace('=');
				SkipWhitespace();
				ReadAttributeValueTokens(-1);
				if (isXmlDecl)
				{
					string value = currentAttributeToken.Value;
				}
				attributeCount++;
				if (!SkipWhitespace())
				{
					flag = true;
				}
				num = PeekChar();
				if (isXmlDecl)
				{
					if (num == 63)
					{
						break;
					}
				}
				else if (num == 47 || num == 62)
				{
					break;
				}
			}
			while (num != -1);
			currentAttribute = -1;
			currentAttributeValue = -1;
		}

		private void AddAttributeWithValue(string name, string value)
		{
			IncrementAttributeToken();
			XmlAttributeTokenInfo xmlAttributeTokenInfo = attributeTokens[currentAttribute];
			xmlAttributeTokenInfo.Name = NameTable.Add(name);
			xmlAttributeTokenInfo.Prefix = string.Empty;
			xmlAttributeTokenInfo.NamespaceURI = string.Empty;
			IncrementAttributeValueToken();
			XmlTokenInfo token = attributeValueTokens[currentAttributeValue];
			SetTokenProperties(token, XmlNodeType.Text, string.Empty, string.Empty, string.Empty, false, value, false);
			xmlAttributeTokenInfo.Value = value;
			attributeCount++;
		}

		private void IncrementAttributeToken()
		{
			currentAttribute++;
			if (attributeTokens.Length == currentAttribute)
			{
				XmlAttributeTokenInfo[] array = new XmlAttributeTokenInfo[attributeTokens.Length * 2];
				attributeTokens.CopyTo(array, 0);
				attributeTokens = array;
			}
			if (attributeTokens[currentAttribute] == null)
			{
				attributeTokens[currentAttribute] = new XmlAttributeTokenInfo(this);
			}
			currentAttributeToken = attributeTokens[currentAttribute];
			currentAttributeToken.Clear();
		}

		private void IncrementAttributeValueToken()
		{
			currentAttributeValue++;
			if (attributeValueTokens.Length == currentAttributeValue)
			{
				XmlTokenInfo[] array = new XmlTokenInfo[attributeValueTokens.Length * 2];
				attributeValueTokens.CopyTo(array, 0);
				attributeValueTokens = array;
			}
			if (attributeValueTokens[currentAttributeValue] == null)
			{
				attributeValueTokens[currentAttributeValue] = new XmlTokenInfo(this);
			}
			currentAttributeValueToken = attributeValueTokens[currentAttributeValue];
			currentAttributeValueToken.Clear();
		}

		private void ReadAttributeValueTokens(int dummyQuoteChar)
		{
			int num = ((dummyQuoteChar >= 0) ? dummyQuoteChar : ReadChar());
			if (num != 39 && num != 34)
			{
				throw NotWFError("an attribute value was not quoted");
			}
			currentAttributeToken.QuoteChar = (char)num;
			IncrementAttributeValueToken();
			currentAttributeToken.ValueTokenStartIndex = currentAttributeValue;
			currentAttributeValueToken.LineNumber = line;
			currentAttributeValueToken.LinePosition = column;
			bool flag = false;
			bool flag2 = true;
			bool flag3 = true;
			int num2 = 0;
			currentAttributeValueToken.ValueBufferStart = valueBuffer.Length;
			while (flag3)
			{
				num2 = ReadChar();
				if (num2 == num)
				{
					break;
				}
				if (flag)
				{
					IncrementAttributeValueToken();
					currentAttributeValueToken.ValueBufferStart = valueBuffer.Length;
					currentAttributeValueToken.LineNumber = line;
					currentAttributeValueToken.LinePosition = column;
					flag = false;
					flag2 = true;
				}
				switch (num2)
				{
				case 60:
					throw NotWFError("attribute values cannot contain '<'");
				case -1:
					if (dummyQuoteChar < 0)
					{
						throw NotWFError("unexpected end of file in an attribute value");
					}
					flag3 = false;
					break;
				case 13:
					if (normalization)
					{
						if (PeekChar() == 10)
						{
							continue;
						}
						if (normalization)
						{
							num2 = 32;
						}
					}
					goto default;
				case 9:
				case 10:
					if (normalization)
					{
						num2 = 32;
					}
					goto default;
				case 38:
				{
					if (PeekChar() == 35)
					{
						Advance(35);
						num2 = ReadCharacterReference();
						AppendValueChar(num2);
						break;
					}
					string text = ReadName();
					Expect(59);
					int predefinedEntity = XmlChar.GetPredefinedEntity(text);
					if (predefinedEntity < 0)
					{
						CheckAttributeEntityReferenceWFC(text);
						if (entityHandling == EntityHandling.ExpandEntities)
						{
							string text2 = DTD.GenerateEntityAttributeText(text);
							foreach (char item in (IEnumerable<char>)text2)
							{
								AppendValueChar(item);
							}
							break;
						}
						currentAttributeValueToken.ValueBufferEnd = valueBuffer.Length;
						currentAttributeValueToken.NodeType = XmlNodeType.Text;
						if (!flag2)
						{
							IncrementAttributeValueToken();
						}
						currentAttributeValueToken.Name = text;
						currentAttributeValueToken.Value = string.Empty;
						currentAttributeValueToken.NodeType = XmlNodeType.EntityReference;
						flag = true;
					}
					else
					{
						AppendValueChar(predefinedEntity);
					}
					break;
				}
				default:
					if (CharacterChecking && XmlChar.IsInvalid(num2))
					{
						throw NotWFError("Invalid character was found.");
					}
					if (num2 < 65535)
					{
						valueBuffer.Append((char)num2);
					}
					else
					{
						AppendSurrogatePairValueChar(num2);
					}
					break;
				}
				flag2 = false;
			}
			if (!flag)
			{
				currentAttributeValueToken.ValueBufferEnd = valueBuffer.Length;
				currentAttributeValueToken.NodeType = XmlNodeType.Text;
			}
			currentAttributeToken.ValueTokenEndIndex = currentAttributeValue;
		}

		private void CheckAttributeEntityReferenceWFC(string entName)
		{
			DTDEntityDeclaration dTDEntityDeclaration = ((DTD != null) ? DTD.EntityDecls[entName] : null);
			if (dTDEntityDeclaration == null)
			{
				if (entityHandling == EntityHandling.ExpandEntities || (DTD != null && resolver != null && dTDEntityDeclaration == null))
				{
					throw NotWFError(string.Format("Referenced entity '{0}' does not exist.", entName));
				}
				return;
			}
			if (dTDEntityDeclaration.HasExternalReference)
			{
				throw NotWFError("Reference to external entities is not allowed in the value of an attribute.");
			}
			if (isStandalone && !dTDEntityDeclaration.IsInternalSubset)
			{
				throw NotWFError("Reference to external entities is not allowed in the internal subset.");
			}
			if (dTDEntityDeclaration.EntityValue.IndexOf('<') < 0)
			{
				return;
			}
			throw NotWFError("Attribute must not contain character '<' either directly or indirectly by way of entity references.");
		}

		private void ReadProcessingInstruction()
		{
			string text = ReadName();
			if (text != "xml" && text.ToLower(CultureInfo.InvariantCulture) == "xml")
			{
				throw NotWFError("Not allowed processing instruction name which starts with 'X', 'M', 'L' was found.");
			}
			if (!SkipWhitespace() && PeekChar() != 63)
			{
				throw NotWFError("Invalid processing instruction name was found.");
			}
			ClearValueBuffer();
			int num;
			while ((num = PeekChar()) != -1)
			{
				Advance(num);
				if (num == 63 && PeekChar() == 62)
				{
					Advance(62);
					break;
				}
				if (CharacterChecking && XmlChar.IsInvalid(num))
				{
					throw NotWFError("Invalid character was found.");
				}
				AppendValueChar(num);
			}
			if (object.ReferenceEquals(text, "xml"))
			{
				VerifyXmlDeclaration();
				return;
			}
			if (currentState == XmlNodeType.None)
			{
				currentState = XmlNodeType.XmlDeclaration;
			}
			SetProperties(XmlNodeType.ProcessingInstruction, text, string.Empty, text, false, null, true);
		}

		private void VerifyXmlDeclaration()
		{
			if (!allowMultipleRoot && currentState != XmlNodeType.None)
			{
				throw NotWFError("XML declaration cannot appear in this state.");
			}
			currentState = XmlNodeType.XmlDeclaration;
			string text = CreateValueString();
			ClearAttributes();
			int idx = 0;
			string text2 = null;
			string text3 = null;
			string value;
			string name;
			ParseAttributeFromString(text, ref idx, out name, out value);
			if (name != "version" || value != "1.0")
			{
				throw NotWFError("'version' is expected.");
			}
			name = string.Empty;
			if (SkipWhitespaceInString(text, ref idx) && idx < text.Length)
			{
				ParseAttributeFromString(text, ref idx, out name, out value);
			}
			if (name == "encoding")
			{
				if (!XmlChar.IsValidIANAEncoding(value))
				{
					throw NotWFError("'encoding' must be a valid IANA encoding name.");
				}
				if (reader is XmlStreamReader)
				{
					parserContext.Encoding = ((XmlStreamReader)reader).Encoding;
				}
				else
				{
					parserContext.Encoding = Encoding.Unicode;
				}
				text2 = value;
				name = string.Empty;
				if (SkipWhitespaceInString(text, ref idx) && idx < text.Length)
				{
					ParseAttributeFromString(text, ref idx, out name, out value);
				}
			}
			if (name == "standalone")
			{
				isStandalone = value == "yes";
				if (value != "yes" && value != "no")
				{
					throw NotWFError("Only 'yes' or 'no' is allow for 'standalone'");
				}
				text3 = value;
				SkipWhitespaceInString(text, ref idx);
			}
			else if (name.Length != 0)
			{
				throw NotWFError(string.Format("Unexpected token: '{0}'", name));
			}
			if (idx < text.Length)
			{
				throw NotWFError("'?' is expected.");
			}
			AddAttributeWithValue("version", "1.0");
			if (text2 != null)
			{
				AddAttributeWithValue("encoding", text2);
			}
			if (text3 != null)
			{
				AddAttributeWithValue("standalone", text3);
			}
			currentAttribute = (currentAttributeValue = -1);
			SetProperties(XmlNodeType.XmlDeclaration, "xml", string.Empty, "xml", false, text, false);
		}

		private bool SkipWhitespaceInString(string text, ref int idx)
		{
			int num = idx;
			while (idx < text.Length && XmlChar.IsWhitespace(text[idx]))
			{
				idx++;
			}
			return idx - num > 0;
		}

		private void ParseAttributeFromString(string src, ref int idx, out string name, out string value)
		{
			while (idx < src.Length && XmlChar.IsWhitespace(src[idx]))
			{
				idx++;
			}
			int num = idx;
			while (idx < src.Length && XmlChar.IsNameChar(src[idx]))
			{
				idx++;
			}
			name = src.Substring(num, idx - num);
			while (idx < src.Length && XmlChar.IsWhitespace(src[idx]))
			{
				idx++;
			}
			if (idx == src.Length || src[idx] != '=')
			{
				throw NotWFError(string.Format("'=' is expected after {0}", name));
			}
			idx++;
			while (idx < src.Length && XmlChar.IsWhitespace(src[idx]))
			{
				idx++;
			}
			if (idx == src.Length || (src[idx] != '"' && src[idx] != '\''))
			{
				throw NotWFError("'\"' or ''' is expected.");
			}
			char c = src[idx];
			idx++;
			num = idx;
			while (idx < src.Length && src[idx] != c)
			{
				idx++;
			}
			idx++;
			value = src.Substring(num, idx - num - 1);
		}

		internal void SkipTextDeclaration()
		{
			if (PeekChar() != 60)
			{
				return;
			}
			ReadChar();
			if (PeekChar() != 63)
			{
				peekCharsIndex = 0;
				return;
			}
			ReadChar();
			while (peekCharsIndex < 6 && PeekChar() >= 0)
			{
				ReadChar();
			}
			if (new string(peekChars, 2, 4) != "xml ")
			{
				if (new string(peekChars, 2, 4).ToLower(CultureInfo.InvariantCulture) == "xml ")
				{
					throw NotWFError("Processing instruction name must not be character sequence 'X' 'M' 'L' with case insensitivity.");
				}
				peekCharsIndex = 0;
				return;
			}
			SkipWhitespace();
			if (PeekChar() == 118)
			{
				Expect("version");
				ExpectAfterWhitespace('=');
				SkipWhitespace();
				int num = ReadChar();
				char[] array = new char[3];
				int num2 = 0;
				int num3 = num;
				if (num3 != 34 && num3 != 39)
				{
					throw NotWFError("Invalid version declaration inside text declaration.");
				}
				while (PeekChar() != num)
				{
					if (PeekChar() == -1)
					{
						throw NotWFError("Invalid version declaration inside text declaration.");
					}
					if (num2 == 3)
					{
						throw NotWFError("Invalid version number inside text declaration.");
					}
					array[num2] = (char)ReadChar();
					num2++;
					if (num2 == 3 && new string(array) != "1.0")
					{
						throw NotWFError("Invalid version number inside text declaration.");
					}
				}
				ReadChar();
				SkipWhitespace();
			}
			if (PeekChar() == 101)
			{
				Expect("encoding");
				ExpectAfterWhitespace('=');
				SkipWhitespace();
				int num4 = ReadChar();
				int num3 = num4;
				if (num3 != 34 && num3 != 39)
				{
					throw NotWFError("Invalid encoding declaration inside text declaration.");
				}
				while (PeekChar() != num4)
				{
					if (ReadChar() == -1)
					{
						throw NotWFError("Invalid encoding declaration inside text declaration.");
					}
				}
				ReadChar();
				SkipWhitespace();
			}
			else if (Conformance == ConformanceLevel.Auto)
			{
				throw NotWFError("Encoding declaration is mandatory in text declaration.");
			}
			Expect("?>");
			curNodePeekIndex = peekCharsIndex;
		}

		private void ReadDeclaration()
		{
			switch (PeekChar())
			{
			case 45:
				Expect("--");
				ReadComment();
				break;
			case 91:
				ReadChar();
				Expect("CDATA[");
				ReadCDATA();
				break;
			case 68:
				Expect("DOCTYPE");
				ReadDoctypeDecl();
				break;
			default:
				throw NotWFError("Unexpected declaration markup was found.");
			}
		}

		private void ReadComment()
		{
			if (currentState == XmlNodeType.None)
			{
				currentState = XmlNodeType.XmlDeclaration;
			}
			preserveCurrentTag = false;
			ClearValueBuffer();
			int num;
			while ((num = PeekChar()) != -1)
			{
				Advance(num);
				if (num == 45 && PeekChar() == 45)
				{
					Advance(45);
					if (PeekChar() != 62)
					{
						throw NotWFError("comments cannot contain '--'");
					}
					Advance(62);
					break;
				}
				if (XmlChar.IsInvalid(num))
				{
					throw NotWFError("Not allowed character was found.");
				}
				AppendValueChar(num);
			}
			SetProperties(XmlNodeType.Comment, string.Empty, string.Empty, string.Empty, false, null, true);
		}

		private void ReadCDATA()
		{
			if (currentState != XmlNodeType.Element)
			{
				throw NotWFError("CDATA section cannot appear in this state.");
			}
			preserveCurrentTag = false;
			ClearValueBuffer();
			bool flag = false;
			int num = 0;
			while (PeekChar() != -1)
			{
				if (!flag)
				{
					num = ReadChar();
				}
				flag = false;
				if (num == 93 && PeekChar() == 93)
				{
					num = ReadChar();
					if (PeekChar() == 62)
					{
						ReadChar();
						break;
					}
					flag = true;
				}
				if (normalization && num == 13)
				{
					num = PeekChar();
					if (num != 10)
					{
						AppendValueChar(10);
					}
					continue;
				}
				if (CharacterChecking && XmlChar.IsInvalid(num))
				{
					throw NotWFError("Invalid character was found.");
				}
				if (num < 65535)
				{
					valueBuffer.Append((char)num);
				}
				else
				{
					AppendSurrogatePairValueChar(num);
				}
			}
			SetProperties(XmlNodeType.CDATA, string.Empty, string.Empty, string.Empty, false, null, true);
		}

		private void ReadDoctypeDecl()
		{
			if (prohibitDtd)
			{
				throw NotWFError("Document Type Declaration (DTD) is prohibited in this XML.");
			}
			XmlNodeType xmlNodeType = currentState;
			if (xmlNodeType == XmlNodeType.Element || xmlNodeType == XmlNodeType.DocumentType || xmlNodeType == XmlNodeType.EndElement)
			{
				throw NotWFError("Document type cannot appear in this state.");
			}
			currentState = XmlNodeType.DocumentType;
			string text = null;
			string text2 = null;
			string text3 = null;
			int intSubsetStartLine = 0;
			int intSubsetStartColumn = 0;
			SkipWhitespace();
			text = ReadName();
			SkipWhitespace();
			switch (PeekChar())
			{
			case 83:
				text3 = ReadSystemLiteral(true);
				break;
			case 80:
				text2 = ReadPubidLiteral();
				if (!SkipWhitespace())
				{
					throw NotWFError("Whitespace is required between PUBLIC id and SYSTEM id.");
				}
				text3 = ReadSystemLiteral(false);
				break;
			}
			SkipWhitespace();
			if (PeekChar() == 91)
			{
				ReadChar();
				intSubsetStartLine = LineNumber;
				intSubsetStartColumn = LinePosition;
				ClearValueBuffer();
				ReadInternalSubset();
				parserContext.InternalSubset = CreateValueString();
			}
			ExpectAfterWhitespace('>');
			GenerateDTDObjectModel(text, text2, text3, parserContext.InternalSubset, intSubsetStartLine, intSubsetStartColumn);
			SetProperties(XmlNodeType.DocumentType, text, string.Empty, text, false, parserContext.InternalSubset, true);
			if (text2 != null)
			{
				AddAttributeWithValue("PUBLIC", text2);
			}
			if (text3 != null)
			{
				AddAttributeWithValue("SYSTEM", text3);
			}
			currentAttribute = (currentAttributeValue = -1);
		}

		internal DTDObjectModel GenerateDTDObjectModel(string name, string publicId, string systemId, string internalSubset)
		{
			return GenerateDTDObjectModel(name, publicId, systemId, internalSubset, 0, 0);
		}

		internal DTDObjectModel GenerateDTDObjectModel(string name, string publicId, string systemId, string internalSubset, int intSubsetStartLine, int intSubsetStartColumn)
		{
			parserContext.Dtd = new DTDObjectModel(NameTable);
			DTD.BaseURI = BaseURI;
			DTD.Name = name;
			DTD.PublicId = publicId;
			DTD.SystemId = systemId;
			DTD.InternalSubset = internalSubset;
			DTD.XmlResolver = resolver;
			DTD.IsStandalone = isStandalone;
			DTD.LineNumber = line;
			DTD.LinePosition = column;
			DTDReader dTDReader = new DTDReader(DTD, intSubsetStartLine, intSubsetStartColumn);
			dTDReader.Normalization = normalization;
			return dTDReader.GenerateDTDObjectModel();
		}

		private int ReadValueChar()
		{
			int num = ReadChar();
			AppendValueChar(num);
			return num;
		}

		private void ExpectAndAppend(string s)
		{
			Expect(s);
			valueBuffer.Append(s);
		}

		private void ReadInternalSubset()
		{
			bool flag = true;
			while (flag)
			{
				switch (ReadValueChar())
				{
				case 93:
					switch (State)
					{
					case DtdInputState.Free:
						valueBuffer.Remove(valueBuffer.Length - 1, 1);
						flag = false;
						break;
					default:
						throw NotWFError("unexpected end of file at DTD.");
					case DtdInputState.Comment:
					case DtdInputState.InsideSingleQuoted:
					case DtdInputState.InsideDoubleQuoted:
						break;
					}
					break;
				case -1:
					throw NotWFError("unexpected end of file at DTD.");
				case 60:
					switch (State)
					{
					default:
					{
						int num = ReadValueChar();
						switch (num)
						{
						case 63:
							stateStack.Push(DtdInputState.PI);
							break;
						case 33:
							switch (ReadValueChar())
							{
							case 69:
								switch (ReadValueChar())
								{
								case 76:
									ExpectAndAppend("EMENT");
									stateStack.Push(DtdInputState.ElementDecl);
									break;
								case 78:
									ExpectAndAppend("TITY");
									stateStack.Push(DtdInputState.EntityDecl);
									break;
								default:
									throw NotWFError("unexpected token '<!E'.");
								}
								break;
							case 65:
								ExpectAndAppend("TTLIST");
								stateStack.Push(DtdInputState.AttlistDecl);
								break;
							case 78:
								ExpectAndAppend("OTATION");
								stateStack.Push(DtdInputState.NotationDecl);
								break;
							case 45:
								ExpectAndAppend("-");
								stateStack.Push(DtdInputState.Comment);
								break;
							}
							break;
						default:
							throw NotWFError(string.Format("unexpected '<{0}'.", (char)num));
						}
						break;
					}
					case DtdInputState.Comment:
					case DtdInputState.InsideSingleQuoted:
					case DtdInputState.InsideDoubleQuoted:
						break;
					}
					break;
				case 39:
					if (State == DtdInputState.InsideSingleQuoted)
					{
						stateStack.Pop();
					}
					else if (State != DtdInputState.InsideDoubleQuoted && State != DtdInputState.Comment)
					{
						stateStack.Push(DtdInputState.InsideSingleQuoted);
					}
					break;
				case 34:
					if (State == DtdInputState.InsideDoubleQuoted)
					{
						stateStack.Pop();
					}
					else if (State != DtdInputState.InsideSingleQuoted && State != DtdInputState.Comment)
					{
						stateStack.Push(DtdInputState.InsideDoubleQuoted);
					}
					break;
				case 62:
					switch (State)
					{
					case DtdInputState.ElementDecl:
					case DtdInputState.AttlistDecl:
					case DtdInputState.EntityDecl:
					case DtdInputState.NotationDecl:
						stateStack.Pop();
						break;
					default:
						throw NotWFError("unexpected token '>'");
					case DtdInputState.Comment:
					case DtdInputState.InsideSingleQuoted:
					case DtdInputState.InsideDoubleQuoted:
						break;
					}
					break;
				case 63:
					if (State == DtdInputState.PI && ReadValueChar() == 62)
					{
						stateStack.Pop();
					}
					break;
				case 45:
					if (State == DtdInputState.Comment && PeekChar() == 45)
					{
						ReadValueChar();
						ExpectAndAppend(">");
						stateStack.Pop();
					}
					break;
				case 37:
					if (State != DtdInputState.Free && State != DtdInputState.EntityDecl && State != DtdInputState.Comment && State != DtdInputState.InsideDoubleQuoted && State != DtdInputState.InsideSingleQuoted)
					{
						throw NotWFError("Parameter Entity Reference cannot appear as a part of markupdecl (see XML spec 2.8).");
					}
					break;
				}
			}
		}

		private string ReadSystemLiteral(bool expectSYSTEM)
		{
			if (expectSYSTEM)
			{
				Expect("SYSTEM");
				if (!SkipWhitespace())
				{
					throw NotWFError("Whitespace is required after 'SYSTEM'.");
				}
			}
			else
			{
				SkipWhitespace();
			}
			int num = ReadChar();
			int num2 = 0;
			ClearValueBuffer();
			while (num2 != num)
			{
				num2 = ReadChar();
				if (num2 < 0)
				{
					throw NotWFError("Unexpected end of stream in ExternalID.");
				}
				if (num2 != num)
				{
					AppendValueChar(num2);
				}
			}
			return CreateValueString();
		}

		private string ReadPubidLiteral()
		{
			Expect("PUBLIC");
			if (!SkipWhitespace())
			{
				throw NotWFError("Whitespace is required after 'PUBLIC'.");
			}
			int num = ReadChar();
			int num2 = 0;
			ClearValueBuffer();
			while (num2 != num)
			{
				num2 = ReadChar();
				if (num2 < 0)
				{
					throw NotWFError("Unexpected end of stream in ExternalID.");
				}
				if (num2 != num && !XmlChar.IsPubidChar(num2))
				{
					throw NotWFError(string.Format("character '{0}' not allowed for PUBLIC ID", (char)num2));
				}
				if (num2 != num)
				{
					AppendValueChar(num2);
				}
			}
			return CreateValueString();
		}

		private string ReadName()
		{
			string prefix;
			string localName;
			return ReadName(out prefix, out localName);
		}

		private string ReadName(out string prefix, out string localName)
		{
			bool flag = preserveCurrentTag;
			preserveCurrentTag = true;
			int num = peekCharsIndex - curNodePeekIndex;
			int num2 = PeekChar();
			if (!XmlChar.IsFirstNameChar(num2))
			{
				throw NotWFError(string.Format(CultureInfo.InvariantCulture, "a name did not start with a legal character {0} ({1})", num2, (char)num2));
			}
			Advance(num2);
			int num3 = 1;
			int num4 = -1;
			while (XmlChar.IsNameChar(num2 = PeekChar()))
			{
				Advance(num2);
				if (num2 == 58 && namespaces && num4 < 0)
				{
					num4 = num3;
				}
				num3++;
			}
			int num5 = curNodePeekIndex + num;
			string text = NameTable.Add(peekChars, num5, num3);
			if (num4 > 0)
			{
				prefix = NameTable.Add(peekChars, num5, num4);
				localName = NameTable.Add(peekChars, num5 + num4 + 1, num3 - num4 - 1);
			}
			else
			{
				prefix = string.Empty;
				localName = text;
			}
			preserveCurrentTag = flag;
			return text;
		}

		private void Expect(int expected)
		{
			int num = ReadChar();
			if (num != expected)
			{
				throw NotWFError(string.Format(CultureInfo.InvariantCulture, "expected '{0}' ({1:X}) but found '{2}' ({3:X})", (char)expected, expected, (num >= 0) ? ((object)(char)num) : "EOF", num));
			}
		}

		private void Expect(string expected)
		{
			for (int i = 0; i < expected.Length; i++)
			{
				if (ReadChar() != expected[i])
				{
					throw NotWFError(string.Format(CultureInfo.InvariantCulture, "'{0}' is expected", expected));
				}
			}
		}

		private void ExpectAfterWhitespace(char c)
		{
			int num;
			do
			{
				num = ReadChar();
			}
			while (num < 33 && XmlChar.IsWhitespace(num));
			if (c != num)
			{
				throw NotWFError(string.Format(CultureInfo.InvariantCulture, "Expected {0}, but found {1} [{2}]", c, (num >= 0) ? ((object)(char)num) : "EOF", num));
			}
		}

		private bool SkipWhitespace()
		{
			int num = PeekChar();
			bool flag = num == 32 || num == 9 || num == 10 || num == 13;
			if (!flag)
			{
				return false;
			}
			Advance(num);
			while ((num = PeekChar()) == 32 || num == 9 || num == 10 || num == 13)
			{
				Advance(num);
			}
			return flag;
		}

		private bool ReadWhitespace()
		{
			if (currentState == XmlNodeType.None)
			{
				currentState = XmlNodeType.XmlDeclaration;
			}
			bool flag = preserveCurrentTag;
			preserveCurrentTag = true;
			int num = peekCharsIndex - curNodePeekIndex;
			int num2 = PeekChar();
			do
			{
				Advance(num2);
				num2 = PeekChar();
			}
			while (num2 == 32 || num2 == 9 || num2 == 10 || num2 == 13);
			bool flag2 = currentState == XmlNodeType.Element && num2 != -1 && num2 != 60;
			if (!flag2 && (whitespaceHandling == WhitespaceHandling.None || (whitespaceHandling == WhitespaceHandling.Significant && XmlSpace != XmlSpace.Preserve)))
			{
				return false;
			}
			ClearValueBuffer();
			valueBuffer.Append(peekChars, curNodePeekIndex, peekCharsIndex - curNodePeekIndex - num);
			preserveCurrentTag = flag;
			if (flag2)
			{
				ReadText(false);
			}
			else
			{
				XmlNodeType nodeType = ((XmlSpace != XmlSpace.Preserve) ? XmlNodeType.Whitespace : XmlNodeType.SignificantWhitespace);
				SetProperties(nodeType, string.Empty, string.Empty, string.Empty, false, null, true);
			}
			return true;
		}

		private int ReadCharsInternal(char[] buffer, int offset, int length)
		{
			int num = offset;
			for (int i = 0; i < length; i++)
			{
				int num2 = PeekChar();
				switch (num2)
				{
				case -1:
					throw NotWFError("Unexpected end of xml.");
				case 60:
					if (i + 1 == length)
					{
						return i;
					}
					Advance(num2);
					if (PeekChar() != 47)
					{
						nestLevel++;
						buffer[num++] = '<';
						break;
					}
					if (nestLevel-- > 0)
					{
						buffer[num++] = '<';
						break;
					}
					Expect(47);
					if (depthUp)
					{
						depth++;
						depthUp = false;
					}
					ReadEndTag();
					readCharsInProgress = false;
					Read();
					return i;
				default:
					Advance(num2);
					if (num2 < 65535)
					{
						buffer[num++] = (char)num2;
						break;
					}
					buffer[num++] = (char)((num2 - 65536) / 1024 + 55296);
					buffer[num++] = (char)((num2 - 65536) % 1024 + 56320);
					break;
				}
			}
			return length;
		}

		private bool ReadUntilEndTag()
		{
			if (Depth == 0)
			{
				currentState = XmlNodeType.EndElement;
			}
			while (true)
			{
				switch (ReadChar())
				{
				case -1:
					throw NotWFError("Unexpected end of xml.");
				case 60:
					if (PeekChar() != 47)
					{
						nestLevel++;
					}
					else if (--nestLevel <= 0)
					{
						ReadChar();
						string text = ReadName();
						if (!(text != elementNames[elementNameStackPos - 1].Name))
						{
							Expect(62);
							depth--;
							return Read();
						}
					}
					break;
				}
			}
		}
	}
}
