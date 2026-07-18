using System.IO;
using System.Text;
using System.Xml.Schema;
using Mono.Xml;
using Mono.Xml.Schema;

namespace System.Xml
{
	public abstract class XmlReader : IDisposable
	{
		private StringBuilder readStringBuffer;

		private XmlReaderBinarySupport binary;

		private XmlReaderSettings settings;

		public abstract int AttributeCount { get; }

		public abstract string BaseURI { get; }

		internal XmlReaderBinarySupport Binary
		{
			get
			{
				return binary;
			}
		}

		internal XmlReaderBinarySupport.CharGetter BinaryCharGetter
		{
			get
			{
				return (binary == null) ? null : binary.Getter;
			}
			set
			{
				if (binary == null)
				{
					binary = new XmlReaderBinarySupport(this);
				}
				binary.Getter = value;
			}
		}

		public virtual bool CanReadBinaryContent
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanReadValueChunk
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanResolveEntity
		{
			get
			{
				return false;
			}
		}

		public abstract int Depth { get; }

		public abstract bool EOF { get; }

		public virtual bool HasAttributes
		{
			get
			{
				return AttributeCount > 0;
			}
		}

		public abstract bool HasValue { get; }

		public abstract bool IsEmptyElement { get; }

		public virtual bool IsDefault
		{
			get
			{
				return false;
			}
		}

		public virtual string this[int i]
		{
			get
			{
				return GetAttribute(i);
			}
		}

		public virtual string this[string name]
		{
			get
			{
				return GetAttribute(name);
			}
		}

		public virtual string this[string name, string namespaceURI]
		{
			get
			{
				return GetAttribute(name, namespaceURI);
			}
		}

		public abstract string LocalName { get; }

		public virtual string Name
		{
			get
			{
				return (Prefix.Length <= 0) ? LocalName : (Prefix + ":" + LocalName);
			}
		}

		public abstract string NamespaceURI { get; }

		public abstract XmlNameTable NameTable { get; }

		public abstract XmlNodeType NodeType { get; }

		public abstract string Prefix { get; }

		public virtual char QuoteChar
		{
			get
			{
				return '"';
			}
		}

		public abstract ReadState ReadState { get; }

		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return null;
			}
		}

		public virtual XmlReaderSettings Settings
		{
			get
			{
				return settings;
			}
		}

		public abstract string Value { get; }

		public virtual string XmlLang
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		public virtual Type ValueType
		{
			get
			{
				return typeof(string);
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(false);
		}

		public abstract void Close();

		private static XmlNameTable PopulateNameTable(XmlReaderSettings settings)
		{
			XmlNameTable xmlNameTable = settings.NameTable;
			if (xmlNameTable == null)
			{
				xmlNameTable = new NameTable();
			}
			return xmlNameTable;
		}

		private static XmlParserContext PopulateParserContext(XmlReaderSettings settings, string baseUri)
		{
			XmlNameTable xmlNameTable = PopulateNameTable(settings);
			return new XmlParserContext(xmlNameTable, new XmlNamespaceManager(xmlNameTable), null, null, null, null, baseUri, null, XmlSpace.None, null);
		}

		private static XmlNodeType GetNodeType(XmlReaderSettings settings)
		{
			ConformanceLevel conformanceLevel = ((settings != null) ? settings.ConformanceLevel : ConformanceLevel.Auto);
			return (conformanceLevel == ConformanceLevel.Fragment) ? XmlNodeType.Element : XmlNodeType.Document;
		}

		public static XmlReader Create(Stream stream)
		{
			return Create(stream, null);
		}

		public static XmlReader Create(string url)
		{
			return Create(url, null);
		}

		public static XmlReader Create(TextReader reader)
		{
			return Create(reader, null);
		}

		public static XmlReader Create(string url, XmlReaderSettings settings)
		{
			return Create(url, settings, null);
		}

		public static XmlReader Create(Stream stream, XmlReaderSettings settings)
		{
			return Create(stream, settings, string.Empty);
		}

		public static XmlReader Create(TextReader reader, XmlReaderSettings settings)
		{
			return Create(reader, settings, string.Empty);
		}

		private static XmlReaderSettings PopulateSettings(XmlReaderSettings src)
		{
			if (src == null)
			{
				return new XmlReaderSettings();
			}
			return src.Clone();
		}

		public static XmlReader Create(Stream stream, XmlReaderSettings settings, string baseUri)
		{
			settings = PopulateSettings(settings);
			return Create(stream, settings, PopulateParserContext(settings, baseUri));
		}

		public static XmlReader Create(TextReader reader, XmlReaderSettings settings, string baseUri)
		{
			settings = PopulateSettings(settings);
			return Create(reader, settings, PopulateParserContext(settings, baseUri));
		}

		public static XmlReader Create(XmlReader reader, XmlReaderSettings settings)
		{
			settings = PopulateSettings(settings);
			XmlReader xmlReader = CreateFilteredXmlReader(reader, settings);
			xmlReader.settings = settings;
			return xmlReader;
		}

		public static XmlReader Create(string url, XmlReaderSettings settings, XmlParserContext context)
		{
			settings = PopulateSettings(settings);
			bool closeInput = settings.CloseInput;
			try
			{
				settings.CloseInput = true;
				if (context == null)
				{
					context = PopulateParserContext(settings, url);
				}
				XmlTextReader reader = new XmlTextReader(false, settings.XmlResolver, url, GetNodeType(settings), context);
				return CreateCustomizedTextReader(reader, settings);
			}
			finally
			{
				settings.CloseInput = closeInput;
			}
		}

		public static XmlReader Create(Stream stream, XmlReaderSettings settings, XmlParserContext context)
		{
			settings = PopulateSettings(settings);
			if (context == null)
			{
				context = PopulateParserContext(settings, string.Empty);
			}
			return CreateCustomizedTextReader(new XmlTextReader(stream, GetNodeType(settings), context), settings);
		}

		public static XmlReader Create(TextReader reader, XmlReaderSettings settings, XmlParserContext context)
		{
			settings = PopulateSettings(settings);
			if (context == null)
			{
				context = PopulateParserContext(settings, string.Empty);
			}
			return CreateCustomizedTextReader(new XmlTextReader(context.BaseURI, reader, GetNodeType(settings), context), settings);
		}

		private static XmlReader CreateCustomizedTextReader(XmlTextReader reader, XmlReaderSettings settings)
		{
			reader.XmlResolver = settings.XmlResolver;
			reader.Normalization = true;
			reader.EntityHandling = EntityHandling.ExpandEntities;
			if (settings.ProhibitDtd)
			{
				reader.ProhibitDtd = true;
			}
			if (!settings.CheckCharacters)
			{
				reader.CharacterChecking = false;
			}
			reader.CloseInput = settings.CloseInput;
			reader.Conformance = settings.ConformanceLevel;
			reader.AdjustLineInfoOffset(settings.LineNumberOffset, settings.LinePositionOffset);
			if (settings.NameTable != null)
			{
				reader.SetNameTable(settings.NameTable);
			}
			XmlReader xmlReader = CreateFilteredXmlReader(reader, settings);
			xmlReader.settings = settings;
			return xmlReader;
		}

		private static XmlReader CreateFilteredXmlReader(XmlReader reader, XmlReaderSettings settings)
		{
			ConformanceLevel conformanceLevel = ConformanceLevel.Auto;
			conformanceLevel = ((reader is XmlTextReader) ? ((XmlTextReader)reader).Conformance : ((reader.Settings == null) ? settings.ConformanceLevel : reader.Settings.ConformanceLevel));
			if (settings.ConformanceLevel != ConformanceLevel.Auto && conformanceLevel != settings.ConformanceLevel)
			{
				throw new InvalidOperationException(string.Format("ConformanceLevel cannot be overwritten by a wrapping XmlReader. The source reader has {0}, while {1} is specified.", conformanceLevel, settings.ConformanceLevel));
			}
			settings.ConformanceLevel = conformanceLevel;
			reader = CreateValidatingXmlReader(reader, settings);
			if (settings.IgnoreComments || settings.IgnoreProcessingInstructions || settings.IgnoreWhitespace)
			{
				return new XmlFilterReader(reader, settings);
			}
			reader.settings = settings;
			return reader;
		}

		private static XmlReader CreateValidatingXmlReader(XmlReader reader, XmlReaderSettings settings)
		{
			XmlValidatingReader xmlValidatingReader = null;
			switch (settings.ValidationType)
			{
			default:
				return reader;
			case ValidationType.DTD:
				xmlValidatingReader = new XmlValidatingReader(reader);
				xmlValidatingReader.XmlResolver = settings.XmlResolver;
				xmlValidatingReader.ValidationType = ValidationType.DTD;
				if ((settings.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints) == 0)
				{
					throw new NotImplementedException();
				}
				return (xmlValidatingReader == null) ? reader : xmlValidatingReader;
			case ValidationType.Schema:
				return new XmlSchemaValidatingReader(reader, settings);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (ReadState != ReadState.Closed)
			{
				Close();
			}
		}

		public abstract string GetAttribute(int i);

		public abstract string GetAttribute(string name);

		public abstract string GetAttribute(string localName, string namespaceName);

		public static bool IsName(string s)
		{
			return s != null && XmlChar.IsName(s);
		}

		public static bool IsNameToken(string s)
		{
			return s != null && XmlChar.IsNmToken(s);
		}

		public virtual bool IsStartElement()
		{
			return MoveToContent() == XmlNodeType.Element;
		}

		public virtual bool IsStartElement(string name)
		{
			if (!IsStartElement())
			{
				return false;
			}
			return Name == name;
		}

		public virtual bool IsStartElement(string localName, string namespaceName)
		{
			if (!IsStartElement())
			{
				return false;
			}
			return LocalName == localName && NamespaceURI == namespaceName;
		}

		public abstract string LookupNamespace(string prefix);

		public virtual void MoveToAttribute(int i)
		{
			if (i >= AttributeCount)
			{
				throw new ArgumentOutOfRangeException();
			}
			MoveToFirstAttribute();
			for (int j = 0; j < i; j++)
			{
				MoveToNextAttribute();
			}
		}

		public abstract bool MoveToAttribute(string name);

		public abstract bool MoveToAttribute(string localName, string namespaceName);

		private bool IsContent(XmlNodeType nodeType)
		{
			switch (nodeType)
			{
			case XmlNodeType.Text:
				return true;
			case XmlNodeType.CDATA:
				return true;
			case XmlNodeType.Element:
				return true;
			case XmlNodeType.EndElement:
				return true;
			case XmlNodeType.EntityReference:
				return true;
			case XmlNodeType.EndEntity:
				return true;
			default:
				return false;
			}
		}

		public virtual XmlNodeType MoveToContent()
		{
			ReadState readState = ReadState;
			if (readState != ReadState.Initial && readState != ReadState.Interactive)
			{
				return NodeType;
			}
			if (NodeType == XmlNodeType.Attribute)
			{
				MoveToElement();
			}
			do
			{
				if (IsContent(NodeType))
				{
					return NodeType;
				}
				Read();
			}
			while (!EOF);
			return XmlNodeType.None;
		}

		public abstract bool MoveToElement();

		public abstract bool MoveToFirstAttribute();

		public abstract bool MoveToNextAttribute();

		public abstract bool Read();

		public abstract bool ReadAttributeValue();

		public virtual string ReadElementString()
		{
			if (MoveToContent() != XmlNodeType.Element)
			{
				string message = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
				throw XmlError(message);
			}
			string result = string.Empty;
			if (!IsEmptyElement)
			{
				Read();
				result = ReadString();
				if (NodeType != XmlNodeType.EndElement)
				{
					string message2 = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
					throw XmlError(message2);
				}
			}
			Read();
			return result;
		}

		public virtual string ReadElementString(string name)
		{
			if (MoveToContent() != XmlNodeType.Element)
			{
				string message = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
				throw XmlError(message);
			}
			if (name != Name)
			{
				string message2 = string.Format("The {0} tag from namespace {1} is expected.", Name, NamespaceURI);
				throw XmlError(message2);
			}
			string result = string.Empty;
			if (!IsEmptyElement)
			{
				Read();
				result = ReadString();
				if (NodeType != XmlNodeType.EndElement)
				{
					string message3 = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
					throw XmlError(message3);
				}
			}
			Read();
			return result;
		}

		public virtual string ReadElementString(string localName, string namespaceName)
		{
			if (MoveToContent() != XmlNodeType.Element)
			{
				string message = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
				throw XmlError(message);
			}
			if (localName != LocalName || NamespaceURI != namespaceName)
			{
				string message2 = string.Format("The {0} tag from namespace {1} is expected.", LocalName, NamespaceURI);
				throw XmlError(message2);
			}
			string result = string.Empty;
			if (!IsEmptyElement)
			{
				Read();
				result = ReadString();
				if (NodeType != XmlNodeType.EndElement)
				{
					string message3 = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
					throw XmlError(message3);
				}
			}
			Read();
			return result;
		}

		public virtual void ReadEndElement()
		{
			if (MoveToContent() != XmlNodeType.EndElement)
			{
				string message = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
				throw XmlError(message);
			}
			Read();
		}

		public virtual string ReadInnerXml()
		{
			if (ReadState != ReadState.Interactive || NodeType == XmlNodeType.EndElement)
			{
				return string.Empty;
			}
			if (IsEmptyElement)
			{
				Read();
				return string.Empty;
			}
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			if (NodeType == XmlNodeType.Element)
			{
				int depth = Depth;
				Read();
				while (depth < Depth)
				{
					if (ReadState != ReadState.Interactive)
					{
						throw XmlError("Unexpected end of the XML reader.");
					}
					xmlTextWriter.WriteNode(this, false);
				}
				Read();
			}
			else
			{
				xmlTextWriter.WriteNode(this, false);
			}
			return stringWriter.ToString();
		}

		public virtual string ReadOuterXml()
		{
			if (ReadState != ReadState.Interactive || NodeType == XmlNodeType.EndElement)
			{
				return string.Empty;
			}
			XmlNodeType nodeType = NodeType;
			if (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.Attribute)
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteNode(this, false);
				return stringWriter.ToString();
			}
			Skip();
			return string.Empty;
		}

		public virtual void ReadStartElement()
		{
			if (MoveToContent() != XmlNodeType.Element)
			{
				string message = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
				throw XmlError(message);
			}
			Read();
		}

		public virtual void ReadStartElement(string name)
		{
			if (MoveToContent() != XmlNodeType.Element)
			{
				string message = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
				throw XmlError(message);
			}
			if (name != Name)
			{
				string message2 = string.Format("The {0} tag from namespace {1} is expected.", Name, NamespaceURI);
				throw XmlError(message2);
			}
			Read();
		}

		public virtual void ReadStartElement(string localName, string namespaceName)
		{
			if (MoveToContent() != XmlNodeType.Element)
			{
				string message = string.Format("'{0}' is an invalid node type.", NodeType.ToString());
				throw XmlError(message);
			}
			if (localName != LocalName || NamespaceURI != namespaceName)
			{
				string message2 = string.Format("Expecting {0} tag from namespace {1}, got {2} and {3} instead", localName, namespaceName, LocalName, NamespaceURI);
				throw XmlError(message2);
			}
			Read();
		}

		public virtual string ReadString()
		{
			if (readStringBuffer == null)
			{
				readStringBuffer = new StringBuilder();
			}
			readStringBuffer.Length = 0;
			MoveToElement();
			switch (NodeType)
			{
			default:
				return string.Empty;
			case XmlNodeType.Element:
				if (IsEmptyElement)
				{
					return string.Empty;
				}
				while (true)
				{
					Read();
					XmlNodeType nodeType = NodeType;
					if (nodeType != XmlNodeType.Text && nodeType != XmlNodeType.CDATA && nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.SignificantWhitespace)
					{
						break;
					}
					readStringBuffer.Append(Value);
				}
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				while (true)
				{
					XmlNodeType nodeType = NodeType;
					if (nodeType != XmlNodeType.Text && nodeType != XmlNodeType.CDATA && nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.SignificantWhitespace)
					{
						break;
					}
					readStringBuffer.Append(Value);
					Read();
				}
				break;
			}
			string result = readStringBuffer.ToString();
			readStringBuffer.Length = 0;
			return result;
		}

		public virtual bool ReadToDescendant(string name)
		{
			if (ReadState == ReadState.Initial)
			{
				MoveToContent();
				if (IsStartElement(name))
				{
					return true;
				}
			}
			if (NodeType != XmlNodeType.Element || IsEmptyElement)
			{
				return false;
			}
			int depth = Depth;
			Read();
			while (depth < Depth)
			{
				if (NodeType == XmlNodeType.Element && name == Name)
				{
					return true;
				}
				Read();
			}
			return false;
		}

		public virtual bool ReadToDescendant(string localName, string namespaceURI)
		{
			if (ReadState == ReadState.Initial)
			{
				MoveToContent();
				if (IsStartElement(localName, namespaceURI))
				{
					return true;
				}
			}
			if (NodeType != XmlNodeType.Element || IsEmptyElement)
			{
				return false;
			}
			int depth = Depth;
			Read();
			while (depth < Depth)
			{
				if (NodeType == XmlNodeType.Element && localName == LocalName && namespaceURI == NamespaceURI)
				{
					return true;
				}
				Read();
			}
			return false;
		}

		public virtual bool ReadToFollowing(string name)
		{
			while (Read())
			{
				if (NodeType == XmlNodeType.Element && name == Name)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool ReadToFollowing(string localName, string namespaceURI)
		{
			while (Read())
			{
				if (NodeType == XmlNodeType.Element && localName == LocalName && namespaceURI == NamespaceURI)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool ReadToNextSibling(string name)
		{
			if (ReadState != ReadState.Interactive)
			{
				return false;
			}
			int depth = Depth;
			Skip();
			while (!EOF && depth <= Depth)
			{
				if (NodeType == XmlNodeType.Element && name == Name)
				{
					return true;
				}
				Skip();
			}
			return false;
		}

		public virtual bool ReadToNextSibling(string localName, string namespaceURI)
		{
			if (ReadState != ReadState.Interactive)
			{
				return false;
			}
			int depth = Depth;
			Skip();
			while (!EOF && depth <= Depth)
			{
				if (NodeType == XmlNodeType.Element && localName == LocalName && namespaceURI == NamespaceURI)
				{
					return true;
				}
				Skip();
			}
			return false;
		}

		public virtual XmlReader ReadSubtree()
		{
			if (NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException(string.Format("ReadSubtree() can be invoked only when the reader is positioned on an element. Current node is {0}. {1}", NodeType, GetLocation()));
			}
			return new SubtreeXmlReader(this);
		}

		private string ReadContentString()
		{
			if (NodeType == XmlNodeType.Attribute || (NodeType != XmlNodeType.Element && HasAttributes))
			{
				return Value;
			}
			return ReadContentString(true);
		}

		private string ReadContentString(bool isText)
		{
			if (isText)
			{
				switch (NodeType)
				{
				case XmlNodeType.Element:
					throw new InvalidOperationException(string.Format("Node type {0} is not supported in this operation.{1}", NodeType, GetLocation()));
				default:
					return string.Empty;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					break;
				}
			}
			string text = string.Empty;
			do
			{
				switch (NodeType)
				{
				case XmlNodeType.Element:
					if (isText)
					{
						return text;
					}
					throw XmlError("Child element is not expected in this operation.");
				case XmlNodeType.EndElement:
					return text;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					text += Value;
					break;
				}
			}
			while (Read());
			throw XmlError("Unexpected end of document.");
		}

		private string GetLocation()
		{
			IXmlLineInfo xmlLineInfo = this as IXmlLineInfo;
			return (xmlLineInfo == null || !xmlLineInfo.HasLineInfo()) ? string.Empty : string.Format(" {0} (line {1}, column {2})", BaseURI, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
		}

		[System.MonoTODO]
		public virtual object ReadElementContentAsObject()
		{
			return ReadElementContentAs(ValueType, null);
		}

		[System.MonoTODO]
		public virtual object ReadElementContentAsObject(string localName, string namespaceURI)
		{
			return ReadElementContentAs(ValueType, null, localName, namespaceURI);
		}

		[System.MonoTODO]
		public virtual object ReadContentAsObject()
		{
			return ReadContentAs(ValueType, null);
		}

		public virtual object ReadElementContentAs(Type type, IXmlNamespaceResolver resolver)
		{
			bool isEmptyElement = IsEmptyElement;
			ReadStartElement();
			object result = ValueAs((!isEmptyElement) ? ReadContentString(false) : string.Empty, type, resolver);
			if (!isEmptyElement)
			{
				ReadEndElement();
			}
			return result;
		}

		public virtual object ReadElementContentAs(Type type, IXmlNamespaceResolver resolver, string localName, string namespaceURI)
		{
			ReadStartElement(localName, namespaceURI);
			object result = ReadContentAs(type, resolver);
			ReadEndElement();
			return result;
		}

		public virtual object ReadContentAs(Type type, IXmlNamespaceResolver resolver)
		{
			return ValueAs(ReadContentString(), type, resolver);
		}

		private object ValueAs(string text, Type type, IXmlNamespaceResolver resolver)
		{
			try
			{
				if (type == typeof(object))
				{
					return text;
				}
				if (type == typeof(XmlQualifiedName))
				{
					if (resolver != null)
					{
						return XmlQualifiedName.Parse(text, resolver);
					}
					return XmlQualifiedName.Parse(text, this);
				}
				if (type == typeof(DateTimeOffset))
				{
					return XmlConvert.ToDateTimeOffset(text);
				}
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
					return XQueryConvert.StringToBoolean(text);
				case TypeCode.DateTime:
					return XQueryConvert.StringToDateTime(text);
				case TypeCode.Decimal:
					return XQueryConvert.StringToDecimal(text);
				case TypeCode.Double:
					return XQueryConvert.StringToDouble(text);
				case TypeCode.Int32:
					return XQueryConvert.StringToInt(text);
				case TypeCode.Int64:
					return XQueryConvert.StringToInteger(text);
				case TypeCode.Single:
					return XQueryConvert.StringToFloat(text);
				case TypeCode.String:
					return text;
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case (TypeCode)17:
					break;
				}
			}
			catch (Exception ex)
			{
				throw XmlError(string.Format("Current text value '{0}' is not acceptable for specified type '{1}'. {2}", text, type, (ex == null) ? string.Empty : ex.Message), ex);
			}
			throw new ArgumentException(string.Format("Specified type '{0}' is not supported.", type));
		}

		public virtual bool ReadElementContentAsBoolean()
		{
			try
			{
				return XQueryConvert.StringToBoolean(ReadElementContentAsString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual DateTime ReadElementContentAsDateTime()
		{
			try
			{
				return XQueryConvert.StringToDateTime(ReadElementContentAsString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual decimal ReadElementContentAsDecimal()
		{
			try
			{
				return XQueryConvert.StringToDecimal(ReadElementContentAsString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual double ReadElementContentAsDouble()
		{
			try
			{
				return XQueryConvert.StringToDouble(ReadElementContentAsString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual float ReadElementContentAsFloat()
		{
			try
			{
				return XQueryConvert.StringToFloat(ReadElementContentAsString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual int ReadElementContentAsInt()
		{
			try
			{
				return XQueryConvert.StringToInt(ReadElementContentAsString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual long ReadElementContentAsLong()
		{
			try
			{
				return XQueryConvert.StringToInteger(ReadElementContentAsString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual string ReadElementContentAsString()
		{
			bool isEmptyElement = IsEmptyElement;
			if (NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException(string.Format("'{0}' is an element node.", NodeType));
			}
			ReadStartElement();
			if (isEmptyElement)
			{
				return string.Empty;
			}
			string result = ReadContentString(false);
			ReadEndElement();
			return result;
		}

		public virtual bool ReadElementContentAsBoolean(string localName, string namespaceURI)
		{
			try
			{
				return XQueryConvert.StringToBoolean(ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
		{
			try
			{
				return XQueryConvert.StringToDateTime(ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
		{
			try
			{
				return XQueryConvert.StringToDecimal(ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual double ReadElementContentAsDouble(string localName, string namespaceURI)
		{
			try
			{
				return XQueryConvert.StringToDouble(ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual float ReadElementContentAsFloat(string localName, string namespaceURI)
		{
			try
			{
				return XQueryConvert.StringToFloat(ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual int ReadElementContentAsInt(string localName, string namespaceURI)
		{
			try
			{
				return XQueryConvert.StringToInt(ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual long ReadElementContentAsLong(string localName, string namespaceURI)
		{
			try
			{
				return XQueryConvert.StringToInteger(ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual string ReadElementContentAsString(string localName, string namespaceURI)
		{
			bool isEmptyElement = IsEmptyElement;
			if (NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException(string.Format("'{0}' is an element node.", NodeType));
			}
			ReadStartElement(localName, namespaceURI);
			if (isEmptyElement)
			{
				return string.Empty;
			}
			string result = ReadContentString(false);
			ReadEndElement();
			return result;
		}

		public virtual bool ReadContentAsBoolean()
		{
			try
			{
				return XQueryConvert.StringToBoolean(ReadContentString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual DateTime ReadContentAsDateTime()
		{
			try
			{
				return XQueryConvert.StringToDateTime(ReadContentString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual decimal ReadContentAsDecimal()
		{
			try
			{
				return XQueryConvert.StringToDecimal(ReadContentString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual double ReadContentAsDouble()
		{
			try
			{
				return XQueryConvert.StringToDouble(ReadContentString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual float ReadContentAsFloat()
		{
			try
			{
				return XQueryConvert.StringToFloat(ReadContentString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual int ReadContentAsInt()
		{
			try
			{
				return XQueryConvert.StringToInt(ReadContentString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual long ReadContentAsLong()
		{
			try
			{
				return XQueryConvert.StringToInteger(ReadContentString());
			}
			catch (FormatException innerException)
			{
				throw XmlError("Typed value is invalid.", innerException);
			}
		}

		public virtual string ReadContentAsString()
		{
			return ReadContentString();
		}

		public virtual int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			CheckSupport();
			return binary.ReadContentAsBase64(buffer, offset, length);
		}

		public virtual int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			CheckSupport();
			return binary.ReadContentAsBinHex(buffer, offset, length);
		}

		public virtual int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			CheckSupport();
			return binary.ReadElementContentAsBase64(buffer, offset, length);
		}

		public virtual int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			CheckSupport();
			return binary.ReadElementContentAsBinHex(buffer, offset, length);
		}

		private void CheckSupport()
		{
			if (!CanReadBinaryContent || !CanReadValueChunk)
			{
				throw new NotSupportedException();
			}
			if (binary == null)
			{
				binary = new XmlReaderBinarySupport(this);
			}
		}

		public virtual int ReadValueChunk(char[] buffer, int offset, int length)
		{
			if (!CanReadValueChunk)
			{
				throw new NotSupportedException();
			}
			if (binary == null)
			{
				binary = new XmlReaderBinarySupport(this);
			}
			return binary.ReadValueChunk(buffer, offset, length);
		}

		public abstract void ResolveEntity();

		public virtual void Skip()
		{
			if (ReadState != ReadState.Interactive)
			{
				return;
			}
			MoveToElement();
			if (NodeType != XmlNodeType.Element || IsEmptyElement)
			{
				Read();
				return;
			}
			int depth = Depth;
			while (Read() && depth < Depth)
			{
			}
			if (NodeType == XmlNodeType.EndElement)
			{
				Read();
			}
		}

		private XmlException XmlError(string message)
		{
			return new XmlException(this as IXmlLineInfo, BaseURI, message);
		}

		private XmlException XmlError(string message, Exception innerException)
		{
			return new XmlException(this as IXmlLineInfo, BaseURI, message);
		}
	}
}
