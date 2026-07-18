using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Xml
{
	public class XmlTextWriter : XmlWriter
	{
		private class XmlNodeInfo
		{
			public string Prefix;

			public string LocalName;

			public string NS;

			public bool HasSimple;

			public bool HasElements;

			public string XmlLang;

			public XmlSpace XmlSpace;
		}

		internal class StringUtil
		{
			private static CultureInfo cul = CultureInfo.InvariantCulture;

			private static CompareInfo cmp = CultureInfo.InvariantCulture.CompareInfo;

			public static int IndexOf(string src, string target)
			{
				return cmp.IndexOf(src, target);
			}

			public static int Compare(string s1, string s2)
			{
				return cmp.Compare(s1, s2);
			}

			public static string Format(string format, params object[] args)
			{
				return string.Format(cul, format, args);
			}
		}

		private enum XmlDeclState
		{
			Allow = 0,
			Ignore = 1,
			Auto = 2,
			Prohibit = 3
		}

		private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

		private const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

		private static readonly Encoding unmarked_utf8encoding = new UTF8Encoding(false, false);

		private static char[] escaped_text_chars;

		private static char[] escaped_attr_chars;

		private Stream base_stream;

		private TextWriter source;

		private TextWriter writer;

		private StringWriter preserver;

		private string preserved_name;

		private bool is_preserved_xmlns;

		private bool allow_doc_fragment;

		private bool close_output_stream = true;

		private bool ignore_encoding;

		private bool namespaces = true;

		private XmlDeclState xmldecl_state;

		private bool check_character_validity;

		private NewLineHandling newline_handling = NewLineHandling.None;

		private bool is_document_entity;

		private WriteState state;

		private XmlNodeType node_state;

		private XmlNamespaceManager nsmanager;

		private int open_count;

		private XmlNodeInfo[] elements = new XmlNodeInfo[10];

		private Stack new_local_namespaces = new Stack();

		private ArrayList explicit_nsdecls = new ArrayList();

		private NamespaceHandling namespace_handling;

		private bool indent;

		private int indent_count = 2;

		private char indent_char = ' ';

		private string indent_string = "  ";

		private string newline;

		private bool indent_attributes;

		private char quote_char = '"';

		private bool v2;

		public Formatting Formatting
		{
			get
			{
				return indent ? Formatting.Indented : Formatting.None;
			}
			set
			{
				indent = value == Formatting.Indented;
			}
		}

		public int Indentation
		{
			get
			{
				return indent_count;
			}
			set
			{
				if (value < 0)
				{
					throw ArgumentError("Indentation must be non-negative integer.");
				}
				indent_count = value;
				indent_string = ((value != 0) ? new string(indent_char, indent_count) : string.Empty);
			}
		}

		public char IndentChar
		{
			get
			{
				return indent_char;
			}
			set
			{
				indent_char = value;
				indent_string = new string(indent_char, indent_count);
			}
		}

		public char QuoteChar
		{
			get
			{
				return quote_char;
			}
			set
			{
				if (state == WriteState.Attribute)
				{
					throw InvalidOperation("QuoteChar must not be changed inside attribute value.");
				}
				if (value != '\'' && value != '"')
				{
					throw ArgumentError("Only ' and \" are allowed as an attribute quote character.");
				}
				quote_char = value;
				escaped_attr_chars[0] = quote_char;
			}
		}

		public override string XmlLang
		{
			get
			{
				return (open_count != 0) ? elements[open_count - 1].XmlLang : null;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return (open_count != 0) ? elements[open_count - 1].XmlSpace : XmlSpace.None;
			}
		}

		public override WriteState WriteState
		{
			get
			{
				return state;
			}
		}

		public Stream BaseStream
		{
			get
			{
				return base_stream;
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
				if (state != WriteState.Start)
				{
					throw InvalidOperation("This property must be set before writing output.");
				}
				namespaces = value;
			}
		}

		public XmlTextWriter(string filename, Encoding encoding)
			: this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None), encoding)
		{
		}

		public XmlTextWriter(Stream stream, Encoding encoding)
			: this(new StreamWriter(stream, (encoding != null) ? encoding : unmarked_utf8encoding))
		{
			ignore_encoding = encoding == null;
			Initialize(writer);
			allow_doc_fragment = true;
		}

		public XmlTextWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ignore_encoding = writer.Encoding == null;
			Initialize(writer);
			allow_doc_fragment = true;
		}

		internal XmlTextWriter(TextWriter writer, XmlWriterSettings settings, bool closeOutput)
		{
			v2 = true;
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			Initialize(writer);
			close_output_stream = closeOutput;
			allow_doc_fragment = settings.ConformanceLevel != ConformanceLevel.Document;
			switch (settings.ConformanceLevel)
			{
			case ConformanceLevel.Auto:
				xmldecl_state = (settings.OmitXmlDeclaration ? XmlDeclState.Ignore : XmlDeclState.Allow);
				break;
			case ConformanceLevel.Document:
				xmldecl_state = (settings.OmitXmlDeclaration ? XmlDeclState.Ignore : XmlDeclState.Auto);
				break;
			case ConformanceLevel.Fragment:
				xmldecl_state = XmlDeclState.Prohibit;
				break;
			}
			if (settings.Indent)
			{
				Formatting = Formatting.Indented;
			}
			indent_string = ((settings.IndentChars != null) ? settings.IndentChars : string.Empty);
			if (settings.NewLineChars != null)
			{
				newline = settings.NewLineChars;
			}
			indent_attributes = settings.NewLineOnAttributes;
			check_character_validity = settings.CheckCharacters;
			newline_handling = settings.NewLineHandling;
			namespace_handling = settings.NamespaceHandling;
		}

		private void Initialize(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			XmlNameTable nameTable = new NameTable();
			this.writer = writer;
			if (writer is StreamWriter)
			{
				base_stream = ((StreamWriter)writer).BaseStream;
			}
			source = writer;
			nsmanager = new XmlNamespaceManager(nameTable);
			newline = writer.NewLine;
			escaped_text_chars = ((newline_handling != NewLineHandling.None) ? new char[5] { '&', '<', '>', '\r', '\n' } : new char[3] { '&', '<', '>' });
			escaped_attr_chars = new char[6] { '"', '&', '<', '>', '\r', '\n' };
		}

		public override string LookupPrefix(string namespaceUri)
		{
			if (namespaceUri == null || namespaceUri == string.Empty)
			{
				throw ArgumentError("The Namespace cannot be empty.");
			}
			if (namespaceUri == nsmanager.DefaultNamespace)
			{
				return string.Empty;
			}
			return nsmanager.LookupPrefixExclusive(namespaceUri, false);
		}

		public override void Close()
		{
			if (state != WriteState.Error)
			{
				if (state == WriteState.Attribute)
				{
					WriteEndAttribute();
				}
				while (open_count > 0)
				{
					WriteEndElement();
				}
			}
			if (close_output_stream)
			{
				writer.Close();
			}
			else
			{
				writer.Flush();
			}
			state = WriteState.Closed;
		}

		public override void Flush()
		{
			writer.Flush();
		}

		public override void WriteStartDocument()
		{
			WriteStartDocumentCore(false, false);
			is_document_entity = true;
		}

		public override void WriteStartDocument(bool standalone)
		{
			WriteStartDocumentCore(true, standalone);
			is_document_entity = true;
		}

		private void WriteStartDocumentCore(bool outputStd, bool standalone)
		{
			if (state != WriteState.Start)
			{
				throw StateError("XmlDeclaration");
			}
			switch (xmldecl_state)
			{
			case XmlDeclState.Ignore:
				return;
			case XmlDeclState.Prohibit:
				throw InvalidOperation("WriteStartDocument cannot be called when ConformanceLevel is Fragment.");
			}
			state = WriteState.Prolog;
			writer.Write("<?xml version=");
			writer.Write(quote_char);
			writer.Write("1.0");
			writer.Write(quote_char);
			if (!ignore_encoding)
			{
				writer.Write(" encoding=");
				writer.Write(quote_char);
				writer.Write(writer.Encoding.WebName);
				writer.Write(quote_char);
			}
			if (outputStd)
			{
				writer.Write(" standalone=");
				writer.Write(quote_char);
				writer.Write((!standalone) ? "no" : "yes");
				writer.Write(quote_char);
			}
			writer.Write("?>");
			xmldecl_state = XmlDeclState.Ignore;
		}

		public override void WriteEndDocument()
		{
			WriteState writeState = state;
			if (writeState == WriteState.Closed || writeState == WriteState.Error || writeState == WriteState.Start)
			{
				throw StateError("EndDocument");
			}
			if (state == WriteState.Attribute)
			{
				WriteEndAttribute();
			}
			while (open_count > 0)
			{
				WriteEndElement();
			}
			state = WriteState.Start;
			is_document_entity = false;
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			if (name == null)
			{
				throw ArgumentError("name");
			}
			if (!XmlChar.IsName(name))
			{
				throw ArgumentError("name");
			}
			if (node_state != XmlNodeType.None)
			{
				throw StateError("DocType");
			}
			node_state = XmlNodeType.DocumentType;
			if (xmldecl_state == XmlDeclState.Auto)
			{
				OutputAutoStartDocument();
			}
			WriteIndent();
			writer.Write("<!DOCTYPE ");
			writer.Write(name);
			if (pubid != null)
			{
				writer.Write(" PUBLIC ");
				writer.Write(quote_char);
				writer.Write(pubid);
				writer.Write(quote_char);
				writer.Write(' ');
				writer.Write(quote_char);
				if (sysid != null)
				{
					writer.Write(sysid);
				}
				writer.Write(quote_char);
			}
			else if (sysid != null)
			{
				writer.Write(" SYSTEM ");
				writer.Write(quote_char);
				writer.Write(sysid);
				writer.Write(quote_char);
			}
			if (subset != null)
			{
				writer.Write("[");
				writer.Write(subset);
				writer.Write("]");
			}
			writer.Write('>');
			state = WriteState.Prolog;
		}

		public override void WriteStartElement(string prefix, string localName, string namespaceUri)
		{
			if (state == WriteState.Error || state == WriteState.Closed)
			{
				throw StateError("StartTag");
			}
			node_state = XmlNodeType.Element;
			bool flag = prefix == null;
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			if (!namespaces && namespaceUri != null && namespaceUri.Length > 0)
			{
				throw ArgumentError("Namespace is disabled in this XmlTextWriter.");
			}
			if (!namespaces && prefix.Length > 0)
			{
				throw ArgumentError("Namespace prefix is disabled in this XmlTextWriter.");
			}
			if (prefix.Length > 0 && namespaceUri == null)
			{
				namespaceUri = nsmanager.LookupNamespace(prefix, false);
				if (namespaceUri == null || namespaceUri.Length == 0)
				{
					throw ArgumentError("Namespace URI must not be null when prefix is not an empty string.");
				}
			}
			if (namespaces && prefix != null && prefix.Length == 3 && namespaceUri != "http://www.w3.org/XML/1998/namespace" && (prefix[0] == 'x' || prefix[0] == 'X') && (prefix[1] == 'm' || prefix[1] == 'M') && (prefix[2] == 'l' || prefix[2] == 'L'))
			{
				throw new ArgumentException("A prefix cannot be equivalent to \"xml\" in case-insensitive match.");
			}
			if (xmldecl_state == XmlDeclState.Auto)
			{
				OutputAutoStartDocument();
			}
			if (state == WriteState.Element)
			{
				CloseStartElement();
			}
			if (open_count > 0)
			{
				elements[open_count - 1].HasElements = true;
			}
			nsmanager.PushScope();
			if (namespaces && namespaceUri != null)
			{
				if (flag && namespaceUri.Length > 0)
				{
					prefix = LookupPrefix(namespaceUri);
				}
				if (prefix == null || namespaceUri.Length == 0)
				{
					prefix = string.Empty;
				}
			}
			WriteIndent();
			writer.Write("<");
			if (prefix.Length > 0)
			{
				writer.Write(prefix);
				writer.Write(':');
			}
			writer.Write(localName);
			if (elements.Length == open_count)
			{
				XmlNodeInfo[] destinationArray = new XmlNodeInfo[open_count << 1];
				Array.Copy(elements, destinationArray, open_count);
				elements = destinationArray;
			}
			if (elements[open_count] == null)
			{
				elements[open_count] = new XmlNodeInfo();
			}
			XmlNodeInfo xmlNodeInfo = elements[open_count];
			xmlNodeInfo.Prefix = prefix;
			xmlNodeInfo.LocalName = localName;
			xmlNodeInfo.NS = namespaceUri;
			xmlNodeInfo.HasSimple = false;
			xmlNodeInfo.HasElements = false;
			xmlNodeInfo.XmlLang = XmlLang;
			xmlNodeInfo.XmlSpace = XmlSpace;
			open_count++;
			if (namespaces && namespaceUri != null)
			{
				string text = nsmanager.LookupNamespace(prefix, false);
				if (text != namespaceUri)
				{
					nsmanager.AddNamespace(prefix, namespaceUri);
					new_local_namespaces.Push(prefix);
				}
			}
			state = WriteState.Element;
		}

		private void CloseStartElement()
		{
			CloseStartElementCore();
			if (state == WriteState.Element)
			{
				writer.Write('>');
			}
			state = WriteState.Content;
		}

		private void CloseStartElementCore()
		{
			if (state == WriteState.Attribute)
			{
				WriteEndAttribute();
			}
			if (new_local_namespaces.Count == 0)
			{
				if (explicit_nsdecls.Count > 0)
				{
					explicit_nsdecls.Clear();
				}
				return;
			}
			int count = explicit_nsdecls.Count;
			while (new_local_namespaces.Count > 0)
			{
				string text = (string)new_local_namespaces.Pop();
				bool flag = false;
				for (int i = 0; i < explicit_nsdecls.Count; i++)
				{
					if ((string)explicit_nsdecls[i] == text)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					explicit_nsdecls.Add(text);
				}
			}
			for (int j = count; j < explicit_nsdecls.Count; j++)
			{
				string text2 = (string)explicit_nsdecls[j];
				string text3 = nsmanager.LookupNamespace(text2, false);
				if (text3 != null)
				{
					if (text2.Length > 0)
					{
						writer.Write(" xmlns:");
						writer.Write(text2);
					}
					else
					{
						writer.Write(" xmlns");
					}
					writer.Write('=');
					writer.Write(quote_char);
					WriteEscapedString(text3, true);
					writer.Write(quote_char);
				}
			}
			explicit_nsdecls.Clear();
		}

		public override void WriteEndElement()
		{
			WriteEndElementCore(false);
		}

		public override void WriteFullEndElement()
		{
			WriteEndElementCore(true);
		}

		private void WriteEndElementCore(bool full)
		{
			if (state == WriteState.Error || state == WriteState.Closed)
			{
				throw StateError("EndElement");
			}
			if (open_count == 0)
			{
				throw InvalidOperation("There is no more open element.");
			}
			CloseStartElementCore();
			nsmanager.PopScope();
			if (state == WriteState.Element)
			{
				if (full)
				{
					writer.Write('>');
				}
				else
				{
					writer.Write(" />");
				}
			}
			if (full || state == WriteState.Content)
			{
				WriteIndentEndElement();
			}
			XmlNodeInfo xmlNodeInfo = elements[--open_count];
			if (full || state == WriteState.Content)
			{
				writer.Write("</");
				if (xmlNodeInfo.Prefix.Length > 0)
				{
					writer.Write(xmlNodeInfo.Prefix);
					writer.Write(':');
				}
				writer.Write(xmlNodeInfo.LocalName);
				writer.Write('>');
			}
			state = WriteState.Content;
			if (open_count == 0)
			{
				node_state = XmlNodeType.EndElement;
			}
		}

		public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
		{
			if (state == WriteState.Attribute)
			{
				WriteEndAttribute();
			}
			if (state != WriteState.Element && state != WriteState.Start)
			{
				throw StateError("Attribute");
			}
			if (prefix == null)
			{
				prefix = string.Empty;
			}
			bool flag = false;
			if (namespaceUri == "http://www.w3.org/2000/xmlns/")
			{
				flag = true;
				if (prefix.Length == 0 && localName != "xmlns")
				{
					prefix = "xmlns";
				}
			}
			else
			{
				flag = prefix == "xmlns" || (localName == "xmlns" && prefix.Length == 0);
			}
			if (namespaces)
			{
				if (prefix == "xml")
				{
					namespaceUri = "http://www.w3.org/XML/1998/namespace";
				}
				else if (namespaceUri == null)
				{
					namespaceUri = ((!flag) ? string.Empty : "http://www.w3.org/2000/xmlns/");
				}
				if (flag && namespaceUri != "http://www.w3.org/2000/xmlns/")
				{
					throw ArgumentError(string.Format("The 'xmlns' attribute is bound to the reserved namespace '{0}'", "http://www.w3.org/2000/xmlns/"));
				}
				if (prefix.Length > 0 && namespaceUri.Length == 0)
				{
					namespaceUri = nsmanager.LookupNamespace(prefix, false);
					if (namespaceUri == null || namespaceUri.Length == 0)
					{
						throw ArgumentError("Namespace URI must not be null when prefix is not an empty string.");
					}
				}
				if (!flag && namespaceUri.Length > 0)
				{
					prefix = DetermineAttributePrefix(prefix, localName, namespaceUri);
				}
			}
			if (indent_attributes)
			{
				WriteIndentAttribute();
			}
			else if (state != WriteState.Start)
			{
				writer.Write(' ');
			}
			if (prefix.Length > 0)
			{
				writer.Write(prefix);
				writer.Write(':');
			}
			writer.Write(localName);
			writer.Write('=');
			writer.Write(quote_char);
			if (flag || prefix == "xml")
			{
				if (preserver == null)
				{
					preserver = new StringWriter();
				}
				else
				{
					preserver.GetStringBuilder().Length = 0;
				}
				writer = preserver;
				if (!flag)
				{
					is_preserved_xmlns = false;
					preserved_name = localName;
				}
				else
				{
					is_preserved_xmlns = true;
					preserved_name = ((!(localName == "xmlns")) ? localName : string.Empty);
				}
			}
			state = WriteState.Attribute;
		}

		private string DetermineAttributePrefix(string prefix, string local, string ns)
		{
			bool flag = false;
			if (prefix.Length == 0)
			{
				prefix = LookupPrefix(ns);
				if (prefix != null && prefix.Length > 0)
				{
					return prefix;
				}
				flag = true;
			}
			else
			{
				prefix = nsmanager.NameTable.Add(prefix);
				string text = nsmanager.LookupNamespace(prefix, true);
				if (text == ns)
				{
					return prefix;
				}
				if (text != null)
				{
					nsmanager.RemoveNamespace(prefix, text);
					if (nsmanager.LookupNamespace(prefix, true) != text)
					{
						flag = true;
						nsmanager.AddNamespace(prefix, text);
					}
				}
			}
			if (flag)
			{
				prefix = MockupPrefix(ns, true);
			}
			new_local_namespaces.Push(prefix);
			nsmanager.AddNamespace(prefix, ns);
			return prefix;
		}

		private string MockupPrefix(string ns, bool skipLookup)
		{
			string text = ((!skipLookup) ? LookupPrefix(ns) : null);
			if (text != null && text.Length > 0)
			{
				return text;
			}
			int num = 1;
			while (true)
			{
				text = StringUtil.Format("d{0}p{1}", open_count, num);
				if (!new_local_namespaces.Contains(text) && nsmanager.LookupNamespace(nsmanager.NameTable.Get(text)) == null)
				{
					break;
				}
				num++;
			}
			nsmanager.AddNamespace(text, ns);
			new_local_namespaces.Push(text);
			return text;
		}

		public override void WriteEndAttribute()
		{
			if (state != WriteState.Attribute)
			{
				throw StateError("End of attribute");
			}
			if (writer == preserver)
			{
				writer = source;
				string text = preserver.ToString();
				if (is_preserved_xmlns)
				{
					if (preserved_name.Length > 0 && text.Length == 0)
					{
						throw ArgumentError("Non-empty prefix must be mapped to non-empty namespace URI.");
					}
					string text2 = nsmanager.LookupNamespace(preserved_name, false);
					if ((namespace_handling & NamespaceHandling.OmitDuplicates) == 0 || text2 != text)
					{
						explicit_nsdecls.Add(preserved_name);
					}
					if (open_count > 0)
					{
						if (v2 && elements[open_count - 1].Prefix == preserved_name && elements[open_count - 1].NS != text)
						{
							throw new XmlException(string.Format("Cannot redefine the namespace for prefix '{0}' used at current element", preserved_name));
						}
						if ((!(elements[open_count - 1].NS == string.Empty) || !(elements[open_count - 1].Prefix == preserved_name)) && text2 != text)
						{
							nsmanager.AddNamespace(preserved_name, text);
						}
					}
				}
				else
				{
					switch (preserved_name)
					{
					case "lang":
						if (open_count > 0)
						{
							elements[open_count - 1].XmlLang = text;
						}
						break;
					case "space":
						switch (text)
						{
						default:
						{
							int num;
							if (num == 1)
							{
								if (open_count > 0)
								{
									elements[open_count - 1].XmlSpace = XmlSpace.Preserve;
								}
								break;
							}
							throw ArgumentError("Invalid value for xml:space.");
						}
						case "default":
							if (open_count > 0)
							{
								elements[open_count - 1].XmlSpace = XmlSpace.Default;
							}
							break;
						}
						break;
					}
				}
				writer.Write(text);
			}
			writer.Write(quote_char);
			state = WriteState.Element;
		}

		public override void WriteComment(string text)
		{
			if (text == null)
			{
				throw ArgumentError("text");
			}
			if (text.Length > 0 && text[text.Length - 1] == '-')
			{
				throw ArgumentError("An input string to WriteComment method must not end with '-'. Escape it with '&#2D;'.");
			}
			if (StringUtil.IndexOf(text, "--") > 0)
			{
				throw ArgumentError("An XML comment cannot end with \"-\".");
			}
			if (state == WriteState.Attribute || state == WriteState.Element)
			{
				CloseStartElement();
			}
			WriteIndent();
			ShiftStateTopLevel("Comment", false, false, false);
			writer.Write("<!--");
			writer.Write(text);
			writer.Write("-->");
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (name == null)
			{
				throw ArgumentError("name");
			}
			if (text == null)
			{
				throw ArgumentError("text");
			}
			WriteIndent();
			if (!XmlChar.IsName(name))
			{
				throw ArgumentError("A processing instruction name must be a valid XML name.");
			}
			if (StringUtil.IndexOf(text, "?>") > 0)
			{
				throw ArgumentError("Processing instruction cannot contain \"?>\" as its value.");
			}
			ShiftStateTopLevel("ProcessingInstruction", false, name == "xml", false);
			writer.Write("<?");
			writer.Write(name);
			writer.Write(' ');
			writer.Write(text);
			writer.Write("?>");
			if (state == WriteState.Start)
			{
				state = WriteState.Prolog;
			}
		}

		public override void WriteWhitespace(string text)
		{
			if (text == null)
			{
				throw ArgumentError("text");
			}
			if (text.Length == 0 || XmlChar.IndexOfNonWhitespace(text) >= 0)
			{
				throw ArgumentError("WriteWhitespace method accepts only whitespaces.");
			}
			ShiftStateTopLevel("Whitespace", true, false, true);
			writer.Write(text);
		}

		public override void WriteCData(string text)
		{
			if (text == null)
			{
				text = string.Empty;
			}
			ShiftStateContent("CData", false);
			if (StringUtil.IndexOf(text, "]]>") >= 0)
			{
				throw ArgumentError("CDATA section must not contain ']]>'.");
			}
			writer.Write("<![CDATA[");
			WriteCheckedString(text);
			writer.Write("]]>");
		}

		public override void WriteString(string text)
		{
			if (text != null && (text.Length != 0 || v2))
			{
				ShiftStateContent("Text", true);
				WriteEscapedString(text, state == WriteState.Attribute);
			}
		}

		public override void WriteRaw(string raw)
		{
			if (raw != null)
			{
				ShiftStateTopLevel("Raw string", true, true, true);
				writer.Write(raw);
			}
		}

		public override void WriteCharEntity(char ch)
		{
			WriteCharacterEntity(ch, '\0', false);
		}

		public override void WriteSurrogateCharEntity(char low, char high)
		{
			WriteCharacterEntity(low, high, true);
		}

		private void WriteCharacterEntity(char ch, char high, bool surrogate)
		{
			if (surrogate && ('\ud800' > high || high > '\udc00' || '\udc00' > ch || ch > '\udfff'))
			{
				throw ArgumentError(string.Format("Invalid surrogate pair was found. Low: &#x{0:X}; High: &#x{0:X};", (int)ch, (int)high));
			}
			if (check_character_validity && XmlChar.IsInvalid(ch))
			{
				throw ArgumentError(string.Format("Invalid character &#x{0:X};", (int)ch));
			}
			ShiftStateContent("Character", true);
			int num = ((!surrogate) ? ch : ((high - 55296) * 1024 + ch - 56320 + 65536));
			writer.Write("&#x");
			writer.Write(num.ToString("X", CultureInfo.InvariantCulture));
			writer.Write(';');
		}

		public override void WriteEntityRef(string name)
		{
			if (name == null)
			{
				throw ArgumentError("name");
			}
			if (!XmlChar.IsName(name))
			{
				throw ArgumentError("Argument name must be a valid XML name.");
			}
			ShiftStateContent("Entity reference", true);
			writer.Write('&');
			writer.Write(name);
			writer.Write(';');
		}

		public override void WriteName(string name)
		{
			if (name == null)
			{
				throw ArgumentError("name");
			}
			if (!XmlChar.IsName(name))
			{
				throw ArgumentError("Not a valid name string.");
			}
			WriteString(name);
		}

		public override void WriteNmToken(string nmtoken)
		{
			if (nmtoken == null)
			{
				throw ArgumentError("nmtoken");
			}
			if (!XmlChar.IsNmToken(nmtoken))
			{
				throw ArgumentError("Not a valid NMTOKEN string.");
			}
			WriteString(nmtoken);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			if (localName == null)
			{
				throw ArgumentError("localName");
			}
			if (ns == null)
			{
				ns = string.Empty;
			}
			if (ns == "http://www.w3.org/2000/xmlns/")
			{
				throw ArgumentError("Prefix 'xmlns' is reserved and cannot be overriden.");
			}
			if (!XmlChar.IsNCName(localName))
			{
				throw ArgumentError("localName must be a valid NCName.");
			}
			ShiftStateContent("QName", true);
			string text = ((ns.Length <= 0) ? string.Empty : LookupPrefix(ns));
			if (text == null)
			{
				if (state != WriteState.Attribute)
				{
					throw ArgumentError(string.Format("Namespace '{0}' is not declared.", ns));
				}
				text = MockupPrefix(ns, false);
			}
			if (text != string.Empty)
			{
				writer.Write(text);
				writer.Write(":");
			}
			writer.Write(localName);
		}

		private void CheckChunkRange(Array buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || buffer.Length < index)
			{
				throw ArgumentOutOfRangeError("index");
			}
			if (count < 0 || buffer.Length < index + count)
			{
				throw ArgumentOutOfRangeError("count");
			}
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			CheckChunkRange(buffer, index, count);
			WriteString(Convert.ToBase64String(buffer, index, count));
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			CheckChunkRange(buffer, index, count);
			ShiftStateContent("BinHex", true);
			XmlConvert.WriteBinHex(buffer, index, count, writer);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			CheckChunkRange(buffer, index, count);
			ShiftStateContent("Chars", true);
			WriteEscapedBuffer(buffer, index, count, state == WriteState.Attribute);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			CheckChunkRange(buffer, index, count);
			ShiftStateContent("Raw text", false);
			writer.Write(buffer, index, count);
		}

		private void WriteIndent()
		{
			WriteIndentCore(0, false);
		}

		private void WriteIndentEndElement()
		{
			WriteIndentCore(-1, false);
		}

		private void WriteIndentAttribute()
		{
			if (!WriteIndentCore(0, true))
			{
				writer.Write(' ');
			}
		}

		private bool WriteIndentCore(int nestFix, bool attribute)
		{
			if (!indent)
			{
				return false;
			}
			for (int num = open_count - 1; num >= 0; num--)
			{
				if (!attribute && elements[num].HasSimple)
				{
					return false;
				}
			}
			if (state != WriteState.Start)
			{
				writer.Write(newline);
			}
			for (int i = 0; i < open_count + nestFix; i++)
			{
				writer.Write(indent_string);
			}
			return true;
		}

		private void OutputAutoStartDocument()
		{
			if (state == WriteState.Start)
			{
				WriteStartDocumentCore(false, false);
			}
		}

		private void ShiftStateTopLevel(string occured, bool allowAttribute, bool dontCheckXmlDecl, bool isCharacter)
		{
			switch (state)
			{
			case WriteState.Closed:
			case WriteState.Error:
				throw StateError(occured);
			case WriteState.Start:
				if (isCharacter)
				{
					CheckMixedContentState();
				}
				if (xmldecl_state == XmlDeclState.Auto && !dontCheckXmlDecl)
				{
					OutputAutoStartDocument();
				}
				state = WriteState.Prolog;
				break;
			case WriteState.Attribute:
				if (allowAttribute)
				{
					break;
				}
				goto case WriteState.Closed;
			case WriteState.Element:
				if (isCharacter)
				{
					CheckMixedContentState();
				}
				CloseStartElement();
				break;
			case WriteState.Content:
				if (isCharacter)
				{
					CheckMixedContentState();
				}
				break;
			case WriteState.Prolog:
				break;
			}
		}

		private void CheckMixedContentState()
		{
			if (open_count > 0)
			{
				elements[open_count - 1].HasSimple = true;
			}
		}

		private void ShiftStateContent(string occured, bool allowAttribute)
		{
			switch (state)
			{
			case WriteState.Closed:
			case WriteState.Error:
				throw StateError(occured);
			case WriteState.Start:
			case WriteState.Prolog:
				if (!allow_doc_fragment || is_document_entity)
				{
					goto case WriteState.Closed;
				}
				if (xmldecl_state == XmlDeclState.Auto)
				{
					OutputAutoStartDocument();
				}
				CheckMixedContentState();
				state = WriteState.Content;
				break;
			case WriteState.Attribute:
				if (allowAttribute)
				{
					break;
				}
				goto case WriteState.Closed;
			case WriteState.Element:
				CloseStartElement();
				CheckMixedContentState();
				break;
			case WriteState.Content:
				CheckMixedContentState();
				break;
			}
		}

		private void WriteEscapedString(string text, bool isAttribute)
		{
			char[] anyOf = ((!isAttribute) ? escaped_text_chars : escaped_attr_chars);
			int num = text.IndexOfAny(anyOf);
			if (num >= 0)
			{
				char[] array = text.ToCharArray();
				WriteCheckedBuffer(array, 0, num);
				WriteEscapedBuffer(array, num, array.Length - num, isAttribute);
			}
			else
			{
				WriteCheckedString(text);
			}
		}

		private void WriteCheckedString(string s)
		{
			int num = XmlChar.IndexOfInvalid(s, true);
			if (num >= 0)
			{
				char[] array = s.ToCharArray();
				writer.Write(array, 0, num);
				WriteCheckedBuffer(array, num, array.Length - num);
			}
			else
			{
				writer.Write(s);
			}
		}

		private void WriteCheckedBuffer(char[] text, int idx, int length)
		{
			int num = idx;
			int num2 = idx + length;
			while ((idx = XmlChar.IndexOfInvalid(text, num, length, true)) >= 0)
			{
				if (check_character_validity)
				{
					throw ArgumentError(string.Format("Input contains invalid character at {0} : &#x{1:X};", idx, (int)text[idx]));
				}
				if (num < idx)
				{
					writer.Write(text, num, idx - num);
				}
				writer.Write("&#x");
				TextWriter textWriter = writer;
				int num3 = text[idx];
				textWriter.Write(num3.ToString("X", CultureInfo.InvariantCulture));
				writer.Write(';');
				length -= idx - num + 1;
				num = idx + 1;
			}
			if (num < num2)
			{
				writer.Write(text, num, num2 - num);
			}
		}

		private void WriteEscapedBuffer(char[] text, int index, int length, bool isAttribute)
		{
			int num = index;
			int num2 = index + length;
			for (int i = num; i < num2; i++)
			{
				switch (text[i])
				{
				case '&':
				case '<':
				case '>':
					if (num < i)
					{
						WriteCheckedBuffer(text, num, i - num);
					}
					writer.Write('&');
					switch (text[i])
					{
					case '&':
						writer.Write("amp;");
						break;
					case '<':
						writer.Write("lt;");
						break;
					case '>':
						writer.Write("gt;");
						break;
					case '\'':
						writer.Write("apos;");
						break;
					case '"':
						writer.Write("quot;");
						break;
					}
					break;
				case '"':
				case '\'':
					if (isAttribute && text[i] == quote_char)
					{
						goto case '&';
					}
					continue;
				case '\r':
					if (i + 1 < num2 && text[i] == '\n')
					{
						i++;
					}
					goto case '\n';
				case '\n':
					if (num < i)
					{
						WriteCheckedBuffer(text, num, i - num);
					}
					if (isAttribute)
					{
						writer.Write((text[i] != '\r') ? "&#xA;" : "&#xD;");
						break;
					}
					switch (newline_handling)
					{
					case NewLineHandling.Entitize:
						writer.Write((text[i] != '\r') ? "&#xA;" : "&#xD;");
						break;
					case NewLineHandling.Replace:
						writer.Write(newline);
						break;
					default:
						writer.Write(text[i]);
						break;
					}
					break;
				default:
					continue;
				}
				num = i + 1;
			}
			if (num < num2)
			{
				WriteCheckedBuffer(text, num, num2 - num);
			}
		}

		private Exception ArgumentOutOfRangeError(string name)
		{
			state = WriteState.Error;
			return new ArgumentOutOfRangeException(name);
		}

		private Exception ArgumentError(string msg)
		{
			state = WriteState.Error;
			return new ArgumentException(msg);
		}

		private Exception InvalidOperation(string msg)
		{
			state = WriteState.Error;
			return new InvalidOperationException(msg);
		}

		private Exception StateError(string occured)
		{
			return InvalidOperation(string.Format("This XmlWriter does not accept {0} at this state {1}.", occured, state));
		}
	}
}
