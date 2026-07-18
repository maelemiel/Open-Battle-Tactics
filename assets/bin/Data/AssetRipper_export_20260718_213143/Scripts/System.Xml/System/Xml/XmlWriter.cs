using System.Collections;
using System.IO;
using System.Text;
using System.Xml.XPath;

namespace System.Xml
{
	public abstract class XmlWriter : IDisposable
	{
		private XmlWriterSettings settings;

		public virtual XmlWriterSettings Settings
		{
			get
			{
				if (settings == null)
				{
					settings = new XmlWriterSettings();
				}
				return settings;
			}
		}

		public abstract WriteState WriteState { get; }

		public virtual string XmlLang
		{
			get
			{
				return null;
			}
		}

		public virtual XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(false);
		}

		public abstract void Close();

		public static XmlWriter Create(Stream stream)
		{
			return Create(stream, null);
		}

		public static XmlWriter Create(string file)
		{
			return Create(file, null);
		}

		public static XmlWriter Create(TextWriter writer)
		{
			return Create(writer, null);
		}

		public static XmlWriter Create(XmlWriter writer)
		{
			return Create(writer, null);
		}

		public static XmlWriter Create(StringBuilder builder)
		{
			return Create(builder, null);
		}

		public static XmlWriter Create(Stream stream, XmlWriterSettings settings)
		{
			Encoding encoding = ((settings == null) ? Encoding.UTF8 : settings.Encoding);
			return Create(new StreamWriter(stream, encoding), settings);
		}

		public static XmlWriter Create(string file, XmlWriterSettings settings)
		{
			Encoding encoding = ((settings == null) ? Encoding.UTF8 : settings.Encoding);
			return CreateTextWriter(new StreamWriter(file, false, encoding), settings, true);
		}

		public static XmlWriter Create(StringBuilder builder, XmlWriterSettings settings)
		{
			return Create(new StringWriter(builder), settings);
		}

		public static XmlWriter Create(TextWriter writer, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return CreateTextWriter(writer, settings, settings.CloseOutput);
		}

		public static XmlWriter Create(XmlWriter writer, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			writer.settings = settings;
			return writer;
		}

		private static XmlWriter CreateTextWriter(TextWriter writer, XmlWriterSettings settings, bool closeOutput)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			XmlTextWriter writer2 = new XmlTextWriter(writer, settings, closeOutput);
			return Create(writer2, settings);
		}

		protected virtual void Dispose(bool disposing)
		{
			Close();
		}

		public abstract void Flush();

		public abstract string LookupPrefix(string ns);

		private void WriteAttribute(XmlReader reader, bool defattr)
		{
			if (!defattr && reader.IsDefault)
			{
				return;
			}
			WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
			while (reader.ReadAttributeValue())
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Text:
					WriteString(reader.Value);
					break;
				case XmlNodeType.EntityReference:
					WriteEntityRef(reader.Name);
					break;
				}
			}
			WriteEndAttribute();
		}

		public virtual void WriteAttributes(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentException("null XmlReader specified.", "reader");
			}
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				switch (nodeType)
				{
				case XmlNodeType.XmlDeclaration:
					WriteAttributeString("version", reader["version"]);
					if (reader["encoding"] != null)
					{
						WriteAttributeString("encoding", reader["encoding"]);
					}
					if (reader["standalone"] != null)
					{
						WriteAttributeString("standalone", reader["standalone"]);
					}
					return;
				case XmlNodeType.Attribute:
					break;
				default:
					throw new XmlException("NodeType is not one of Element, Attribute, nor XmlDeclaration.");
				}
			}
			else if (!reader.MoveToFirstAttribute())
			{
				return;
			}
			do
			{
				WriteAttribute(reader, defattr);
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
		}

		public void WriteAttributeString(string localName, string value)
		{
			WriteAttributeString(string.Empty, localName, null, value);
		}

		public void WriteAttributeString(string localName, string ns, string value)
		{
			WriteAttributeString(string.Empty, localName, ns, value);
		}

		public void WriteAttributeString(string prefix, string localName, string ns, string value)
		{
			WriteStartAttribute(prefix, localName, ns);
			if (value != null && value.Length > 0)
			{
				WriteString(value);
			}
			WriteEndAttribute();
		}

		public abstract void WriteBase64(byte[] buffer, int index, int count);

		public virtual void WriteBinHex(byte[] buffer, int index, int count)
		{
			StringWriter stringWriter = new StringWriter();
			XmlConvert.WriteBinHex(buffer, index, count, stringWriter);
			WriteString(stringWriter.ToString());
		}

		public abstract void WriteCData(string text);

		public abstract void WriteCharEntity(char ch);

		public abstract void WriteChars(char[] buffer, int index, int count);

		public abstract void WriteComment(string text);

		public abstract void WriteDocType(string name, string pubid, string sysid, string subset);

		public void WriteElementString(string localName, string value)
		{
			WriteStartElement(localName);
			if (value != null && value.Length > 0)
			{
				WriteString(value);
			}
			WriteEndElement();
		}

		public void WriteElementString(string localName, string ns, string value)
		{
			WriteStartElement(localName, ns);
			if (value != null && value.Length > 0)
			{
				WriteString(value);
			}
			WriteEndElement();
		}

		public void WriteElementString(string prefix, string localName, string ns, string value)
		{
			WriteStartElement(prefix, localName, ns);
			if (value != null && value.Length > 0)
			{
				WriteString(value);
			}
			WriteEndElement();
		}

		public abstract void WriteEndAttribute();

		public abstract void WriteEndDocument();

		public abstract void WriteEndElement();

		public abstract void WriteEntityRef(string name);

		public abstract void WriteFullEndElement();

		public virtual void WriteName(string name)
		{
			WriteNameInternal(name);
		}

		public virtual void WriteNmToken(string name)
		{
			WriteNmTokenInternal(name);
		}

		public virtual void WriteQualifiedName(string localName, string ns)
		{
			WriteQualifiedNameInternal(localName, ns);
		}

		internal void WriteNameInternal(string name)
		{
			ConformanceLevel conformanceLevel = Settings.ConformanceLevel;
			if (conformanceLevel == ConformanceLevel.Fragment || conformanceLevel == ConformanceLevel.Document)
			{
				XmlConvert.VerifyName(name);
			}
			WriteString(name);
		}

		internal virtual void WriteNmTokenInternal(string name)
		{
			bool flag = true;
			ConformanceLevel conformanceLevel = Settings.ConformanceLevel;
			if (conformanceLevel == ConformanceLevel.Fragment || conformanceLevel == ConformanceLevel.Document)
			{
				flag = XmlChar.IsNmToken(name);
			}
			if (!flag)
			{
				throw new ArgumentException("Argument name is not a valid NMTOKEN.");
			}
			WriteString(name);
		}

		internal void WriteQualifiedNameInternal(string localName, string ns)
		{
			if (localName == null || localName == string.Empty)
			{
				throw new ArgumentException();
			}
			if (ns == null)
			{
				ns = string.Empty;
			}
			ConformanceLevel conformanceLevel = Settings.ConformanceLevel;
			if (conformanceLevel == ConformanceLevel.Fragment || conformanceLevel == ConformanceLevel.Document)
			{
				XmlConvert.VerifyNCName(localName);
			}
			string text = ((ns.Length <= 0) ? string.Empty : LookupPrefix(ns));
			if (text == null)
			{
				throw new ArgumentException(string.Format("Namespace '{0}' is not declared.", ns));
			}
			if (text != string.Empty)
			{
				WriteString(text);
				WriteString(":");
				WriteString(localName);
			}
			else
			{
				WriteString(localName);
			}
		}

		public virtual void WriteNode(XPathNavigator navigator, bool defattr)
		{
			if (navigator == null)
			{
				throw new ArgumentNullException("navigator");
			}
			switch (navigator.NodeType)
			{
			case XPathNodeType.Attribute:
				break;
			case XPathNodeType.Namespace:
				break;
			case XPathNodeType.Text:
				WriteString(navigator.Value);
				break;
			case XPathNodeType.SignificantWhitespace:
				WriteWhitespace(navigator.Value);
				break;
			case XPathNodeType.Whitespace:
				WriteWhitespace(navigator.Value);
				break;
			case XPathNodeType.Comment:
				WriteComment(navigator.Value);
				break;
			case XPathNodeType.ProcessingInstruction:
				WriteProcessingInstruction(navigator.Name, navigator.Value);
				break;
			case XPathNodeType.Root:
				if (navigator.MoveToFirstChild())
				{
					do
					{
						WriteNode(navigator, defattr);
					}
					while (navigator.MoveToNext());
					navigator.MoveToParent();
				}
				break;
			case XPathNodeType.Element:
				WriteStartElement(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
				if (navigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
				{
					do
					{
						if (defattr || navigator.SchemaInfo == null || navigator.SchemaInfo.IsDefault)
						{
							WriteAttributeString(navigator.Prefix, (!(navigator.LocalName == string.Empty)) ? navigator.LocalName : "xmlns", "http://www.w3.org/2000/xmlns/", navigator.Value);
						}
					}
					while (navigator.MoveToNextNamespace(XPathNamespaceScope.Local));
					navigator.MoveToParent();
				}
				if (navigator.MoveToFirstAttribute())
				{
					do
					{
						if (defattr || navigator.SchemaInfo == null || navigator.SchemaInfo.IsDefault)
						{
							WriteAttributeString(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI, navigator.Value);
						}
					}
					while (navigator.MoveToNextAttribute());
					navigator.MoveToParent();
				}
				if (navigator.MoveToFirstChild())
				{
					do
					{
						WriteNode(navigator, defattr);
					}
					while (navigator.MoveToNext());
					navigator.MoveToParent();
				}
				if (navigator.IsEmptyElement)
				{
					WriteEndElement();
				}
				else
				{
					WriteFullEndElement();
				}
				break;
			default:
				throw new NotSupportedException();
			}
		}

		public virtual void WriteNode(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentException();
			}
			if (reader.ReadState == ReadState.Initial)
			{
				reader.Read();
				do
				{
					WriteNode(reader, defattr);
				}
				while (!reader.EOF);
				return;
			}
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
			{
				WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
				if (reader.HasAttributes)
				{
					for (int i = 0; i < reader.AttributeCount; i++)
					{
						reader.MoveToAttribute(i);
						WriteAttribute(reader, defattr);
					}
					reader.MoveToElement();
				}
				if (reader.IsEmptyElement)
				{
					WriteEndElement();
					break;
				}
				int depth = reader.Depth;
				reader.Read();
				if (reader.NodeType != XmlNodeType.EndElement)
				{
					do
					{
						WriteNode(reader, defattr);
					}
					while (depth < reader.Depth);
				}
				WriteFullEndElement();
				break;
			}
			case XmlNodeType.Attribute:
				return;
			case XmlNodeType.Text:
				WriteString(reader.Value);
				break;
			case XmlNodeType.CDATA:
				WriteCData(reader.Value);
				break;
			case XmlNodeType.EntityReference:
				WriteEntityRef(reader.Name);
				break;
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.XmlDeclaration:
				WriteProcessingInstruction(reader.Name, reader.Value);
				break;
			case XmlNodeType.Comment:
				WriteComment(reader.Value);
				break;
			case XmlNodeType.DocumentType:
				WriteDocType(reader.Name, reader["PUBLIC"], reader["SYSTEM"], reader.Value);
				break;
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				WriteWhitespace(reader.Value);
				break;
			case XmlNodeType.EndElement:
				WriteFullEndElement();
				break;
			default:
				throw new XmlException("Unexpected node " + reader.Name + " of type " + reader.NodeType);
			case XmlNodeType.None:
			case XmlNodeType.EndEntity:
				break;
			}
			reader.Read();
		}

		public abstract void WriteProcessingInstruction(string name, string text);

		public abstract void WriteRaw(string data);

		public abstract void WriteRaw(char[] buffer, int index, int count);

		public void WriteStartAttribute(string localName)
		{
			WriteStartAttribute(null, localName, null);
		}

		public void WriteStartAttribute(string localName, string ns)
		{
			WriteStartAttribute(null, localName, ns);
		}

		public abstract void WriteStartAttribute(string prefix, string localName, string ns);

		public abstract void WriteStartDocument();

		public abstract void WriteStartDocument(bool standalone);

		public void WriteStartElement(string localName)
		{
			WriteStartElement(null, localName, null);
		}

		public void WriteStartElement(string localName, string ns)
		{
			WriteStartElement(null, localName, ns);
		}

		public abstract void WriteStartElement(string prefix, string localName, string ns);

		public abstract void WriteString(string text);

		public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

		public abstract void WriteWhitespace(string ws);

		public virtual void WriteValue(bool value)
		{
			WriteString(XQueryConvert.BooleanToString(value));
		}

		public virtual void WriteValue(DateTime value)
		{
			WriteString(XmlConvert.ToString(value));
		}

		public virtual void WriteValue(decimal value)
		{
			WriteString(XQueryConvert.DecimalToString(value));
		}

		public virtual void WriteValue(double value)
		{
			WriteString(XQueryConvert.DoubleToString(value));
		}

		public virtual void WriteValue(int value)
		{
			WriteString(XQueryConvert.IntToString(value));
		}

		public virtual void WriteValue(long value)
		{
			WriteString(XQueryConvert.IntegerToString(value));
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value is string)
			{
				WriteString((string)value);
				return;
			}
			if (value is bool)
			{
				WriteValue((bool)value);
				return;
			}
			if (value is byte)
			{
				WriteValue((int)value);
				return;
			}
			if (value is byte[])
			{
				WriteBase64((byte[])value, 0, ((byte[])value).Length);
				return;
			}
			if (value is char[])
			{
				WriteChars((char[])value, 0, ((char[])value).Length);
				return;
			}
			if (value is DateTime)
			{
				WriteValue((DateTime)value);
				return;
			}
			if (value is decimal)
			{
				WriteValue((decimal)value);
				return;
			}
			if (value is double)
			{
				WriteValue((double)value);
				return;
			}
			if (value is short)
			{
				WriteValue((int)value);
				return;
			}
			if (value is int)
			{
				WriteValue((int)value);
				return;
			}
			if (value is long)
			{
				WriteValue((long)value);
				return;
			}
			if (value is float)
			{
				WriteValue((float)value);
				return;
			}
			if (value is TimeSpan)
			{
				WriteString(XmlConvert.ToString((TimeSpan)value));
				return;
			}
			if (value is XmlQualifiedName)
			{
				XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)value;
				if (!xmlQualifiedName.Equals(XmlQualifiedName.Empty))
				{
					if (xmlQualifiedName.Namespace.Length > 0 && LookupPrefix(xmlQualifiedName.Namespace) == null)
					{
						throw new InvalidCastException(string.Format("The QName '{0}' cannot be written. No corresponding prefix is declared", xmlQualifiedName));
					}
					WriteQualifiedName(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
				}
				else
				{
					WriteString(string.Empty);
				}
				return;
			}
			if (value is IEnumerable)
			{
				bool flag = false;
				{
					foreach (object item in (IEnumerable)value)
					{
						if (flag)
						{
							WriteString(" ");
						}
						else
						{
							flag = true;
						}
						WriteValue(item);
					}
					return;
				}
			}
			throw new InvalidCastException(string.Format("Type '{0}' cannot be cast to string", value.GetType()));
		}

		public virtual void WriteValue(float value)
		{
			WriteString(XQueryConvert.FloatToString(value));
		}

		public virtual void WriteValue(string value)
		{
			WriteString(value);
		}
	}
}
