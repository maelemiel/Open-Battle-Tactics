using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Mono.Xml.Xsl
{
	internal class GenericOutputter : Outputter
	{
		private Hashtable _outputs;

		private XslOutput _currentOutput;

		private Emitter _emitter;

		private TextWriter pendingTextWriter;

		private StringBuilder pendingFirstSpaces;

		private WriteState _state;

		private Attribute[] pendingAttributes = new Attribute[10];

		private int pendingAttributesPos;

		private XmlNamespaceManager _nsManager;

		private ListDictionary _currentNamespaceDecls;

		private ArrayList newNamespaces = new ArrayList();

		private NameTable _nt;

		private Encoding _encoding;

		private bool _canProcessAttributes;

		private bool _insideCData;

		private bool _omitXmlDeclaration;

		private int _xpCount;

		private Emitter Emitter
		{
			get
			{
				if (_emitter == null)
				{
					DetermineOutputMethod(null, null);
				}
				return _emitter;
			}
		}

		public override bool CanProcessAttributes
		{
			get
			{
				return _canProcessAttributes;
			}
		}

		public override bool InsideCDataSection
		{
			get
			{
				return _insideCData;
			}
			set
			{
				_insideCData = value;
			}
		}

		private GenericOutputter(Hashtable outputs, Encoding encoding)
		{
			_encoding = encoding;
			_outputs = outputs;
			_currentOutput = (XslOutput)outputs[string.Empty];
			_state = WriteState.Prolog;
			_nt = new NameTable();
			_nsManager = new XmlNamespaceManager(_nt);
			_currentNamespaceDecls = new ListDictionary();
			_omitXmlDeclaration = false;
		}

		public GenericOutputter(XmlWriter writer, Hashtable outputs, Encoding encoding)
			: this(writer, outputs, encoding, false)
		{
		}

		internal GenericOutputter(XmlWriter writer, Hashtable outputs, Encoding encoding, bool isVariable)
			: this(outputs, encoding)
		{
			_emitter = new XmlWriterEmitter(writer);
			_state = writer.WriteState;
			_omitXmlDeclaration = true;
		}

		public GenericOutputter(TextWriter writer, Hashtable outputs, Encoding encoding)
			: this(outputs, encoding)
		{
			pendingTextWriter = writer;
		}

		internal GenericOutputter(TextWriter writer, Hashtable outputs)
			: this(writer, outputs, null)
		{
		}

		internal GenericOutputter(XmlWriter writer, Hashtable outputs)
			: this(writer, outputs, null)
		{
		}

		private void DetermineOutputMethod(string localName, string ns)
		{
			XslOutput xslOutput = (XslOutput)_outputs[string.Empty];
			switch (xslOutput.Method)
			{
			default:
				if (localName != null && string.Compare(localName, "html", true, CultureInfo.InvariantCulture) == 0 && ns == string.Empty)
				{
					goto case OutputMethod.HTML;
				}
				goto case OutputMethod.XML;
			case OutputMethod.HTML:
				_emitter = new HtmlEmitter(pendingTextWriter, xslOutput);
				break;
			case OutputMethod.XML:
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(pendingTextWriter);
				if (xslOutput.Indent == "yes")
				{
					xmlTextWriter.Formatting = Formatting.Indented;
				}
				_emitter = new XmlWriterEmitter(xmlTextWriter);
				if (!_omitXmlDeclaration && !xslOutput.OmitXmlDeclaration)
				{
					_emitter.WriteStartDocument((_encoding == null) ? xslOutput.Encoding : _encoding, xslOutput.Standalone);
				}
				break;
			}
			case OutputMethod.Text:
				_emitter = new TextEmitter(pendingTextWriter);
				break;
			}
			pendingTextWriter = null;
		}

		private void CheckState()
		{
			if (_state == WriteState.Element)
			{
				_nsManager.PushScope();
				foreach (string key in _currentNamespaceDecls.Keys)
				{
					string text2 = _currentNamespaceDecls[key] as string;
					if (!(_nsManager.LookupNamespace(key, false) == text2))
					{
						newNamespaces.Add(key);
						_nsManager.AddNamespace(key, text2);
					}
				}
				for (int i = 0; i < pendingAttributesPos; i++)
				{
					Attribute attribute = pendingAttributes[i];
					string text3 = attribute.Prefix;
					if (text3 == "xml" && attribute.Namespace != "http://www.w3.org/XML/1998/namespace")
					{
						text3 = string.Empty;
					}
					string text4 = _nsManager.LookupPrefix(attribute.Namespace, false);
					if (text3.Length == 0 && attribute.Namespace.Length > 0)
					{
						text3 = text4;
					}
					if (attribute.Namespace.Length > 0 && (text3 == null || text3 == string.Empty))
					{
						text3 = "xp_" + _xpCount++;
						while (_nsManager.LookupNamespace(text3) != null)
						{
							text3 = "xp_" + _xpCount++;
						}
						newNamespaces.Add(text3);
						_currentNamespaceDecls.Add(text3, attribute.Namespace);
						_nsManager.AddNamespace(text3, attribute.Namespace);
					}
					Emitter.WriteAttributeString(text3, attribute.LocalName, attribute.Namespace, attribute.Value);
				}
				for (int j = 0; j < newNamespaces.Count; j++)
				{
					string text5 = (string)newNamespaces[j];
					string value = _currentNamespaceDecls[text5] as string;
					if (text5 != string.Empty)
					{
						Emitter.WriteAttributeString("xmlns", text5, "http://www.w3.org/2000/xmlns/", value);
					}
					else
					{
						Emitter.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", value);
					}
				}
				_currentNamespaceDecls.Clear();
				_state = WriteState.Content;
				newNamespaces.Clear();
			}
			_canProcessAttributes = false;
		}

		public override void WriteStartElement(string prefix, string localName, string nsURI)
		{
			if (_emitter == null)
			{
				DetermineOutputMethod(localName, nsURI);
				if (pendingFirstSpaces != null)
				{
					WriteWhitespace(pendingFirstSpaces.ToString());
					pendingFirstSpaces = null;
				}
			}
			if (_state == WriteState.Prolog && (_currentOutput.DoctypePublic != null || _currentOutput.DoctypeSystem != null))
			{
				Emitter.WriteDocType(prefix + ((prefix != null) ? string.Empty : ":") + localName, _currentOutput.DoctypePublic, _currentOutput.DoctypeSystem);
			}
			CheckState();
			if (nsURI == string.Empty)
			{
				prefix = string.Empty;
			}
			Emitter.WriteStartElement(prefix, localName, nsURI);
			_state = WriteState.Element;
			if (_nsManager.LookupNamespace(prefix, false) != nsURI)
			{
				_currentNamespaceDecls[prefix] = nsURI;
			}
			pendingAttributesPos = 0;
			_canProcessAttributes = true;
		}

		public override void WriteEndElement()
		{
			WriteEndElementInternal(false);
		}

		public override void WriteFullEndElement()
		{
			WriteEndElementInternal(true);
		}

		private void WriteEndElementInternal(bool fullEndElement)
		{
			CheckState();
			if (fullEndElement)
			{
				Emitter.WriteFullEndElement();
			}
			else
			{
				Emitter.WriteEndElement();
			}
			_state = WriteState.Content;
			_nsManager.PopScope();
		}

		public override void WriteAttributeString(string prefix, string localName, string nsURI, string value)
		{
			for (int i = 0; i < pendingAttributesPos; i++)
			{
				Attribute attribute = pendingAttributes[i];
				if (attribute.LocalName == localName && attribute.Namespace == nsURI)
				{
					pendingAttributes[i].Value = value;
					pendingAttributes[i].Prefix = prefix;
					return;
				}
			}
			if (pendingAttributesPos == pendingAttributes.Length)
			{
				Attribute[] sourceArray = pendingAttributes;
				pendingAttributes = new Attribute[pendingAttributesPos * 2 + 1];
				if (pendingAttributesPos > 0)
				{
					Array.Copy(sourceArray, 0, pendingAttributes, 0, pendingAttributesPos);
				}
			}
			pendingAttributes[pendingAttributesPos].Prefix = prefix;
			pendingAttributes[pendingAttributesPos].Namespace = nsURI;
			pendingAttributes[pendingAttributesPos].LocalName = localName;
			pendingAttributes[pendingAttributesPos].Value = value;
			pendingAttributesPos++;
		}

		public override void WriteNamespaceDecl(string prefix, string nsUri)
		{
			if (_nsManager.LookupNamespace(prefix, false) == nsUri)
			{
				return;
			}
			for (int i = 0; i < pendingAttributesPos; i++)
			{
				Attribute attribute = pendingAttributes[i];
				if (attribute.Prefix == prefix || attribute.Namespace == nsUri)
				{
					return;
				}
			}
			if (_currentNamespaceDecls[prefix] as string != nsUri)
			{
				_currentNamespaceDecls[prefix] = nsUri;
			}
		}

		public override void WriteComment(string text)
		{
			CheckState();
			Emitter.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			CheckState();
			Emitter.WriteProcessingInstruction(name, text);
		}

		public override void WriteString(string text)
		{
			CheckState();
			if (_insideCData)
			{
				Emitter.WriteCDataSection(text);
			}
			else if (_state != WriteState.Content && text.Length > 0 && XmlChar.IsWhitespace(text))
			{
				Emitter.WriteWhitespace(text);
			}
			else
			{
				Emitter.WriteString(text);
			}
		}

		public override void WriteRaw(string data)
		{
			CheckState();
			Emitter.WriteRaw(data);
		}

		public override void WriteWhitespace(string text)
		{
			if (_emitter == null)
			{
				if (pendingFirstSpaces == null)
				{
					pendingFirstSpaces = new StringBuilder();
				}
				pendingFirstSpaces.Append(text);
				if (_state == WriteState.Start)
				{
					_state = WriteState.Prolog;
				}
			}
			else
			{
				CheckState();
				Emitter.WriteWhitespace(text);
			}
		}

		public override void Done()
		{
			Emitter.Done();
			_state = WriteState.Closed;
		}
	}
}
