using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class XPathNavigatorReader : XmlReader
	{
		private XPathNavigator root;

		private XPathNavigator current;

		private bool started;

		private bool closed;

		private bool endElement;

		private bool attributeValueConsumed;

		private StringBuilder readStringBuffer = new StringBuilder();

		private StringBuilder innerXmlBuilder = new StringBuilder();

		private int depth;

		private int attributeCount;

		private bool eof;

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

		public override XmlNodeType NodeType
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return XmlNodeType.None;
				}
				if (endElement)
				{
					return XmlNodeType.EndElement;
				}
				if (attributeValueConsumed)
				{
					return XmlNodeType.Text;
				}
				switch (current.NodeType)
				{
				case XPathNodeType.Attribute:
				case XPathNodeType.Namespace:
					return XmlNodeType.Attribute;
				case XPathNodeType.Comment:
					return XmlNodeType.Comment;
				case XPathNodeType.Element:
					return XmlNodeType.Element;
				case XPathNodeType.ProcessingInstruction:
					return XmlNodeType.ProcessingInstruction;
				case XPathNodeType.Root:
					return XmlNodeType.None;
				case XPathNodeType.SignificantWhitespace:
					return XmlNodeType.SignificantWhitespace;
				case XPathNodeType.Text:
					return XmlNodeType.Text;
				case XPathNodeType.Whitespace:
					return XmlNodeType.Whitespace;
				default:
					throw new InvalidOperationException(string.Format("Current XPathNavigator status is {0} which is not acceptable to XmlReader.", current.NodeType));
				}
			}
		}

		public override string Name
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return string.Empty;
				}
				if (attributeValueConsumed)
				{
					return string.Empty;
				}
				if (current.NodeType == XPathNodeType.Namespace)
				{
					return (!(current.Name == string.Empty)) ? ("xmlns:" + current.Name) : "xmlns";
				}
				return current.Name;
			}
		}

		public override string LocalName
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return string.Empty;
				}
				if (attributeValueConsumed)
				{
					return string.Empty;
				}
				if (current.NodeType == XPathNodeType.Namespace && current.LocalName == string.Empty)
				{
					return "xmlns";
				}
				return current.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return string.Empty;
				}
				if (attributeValueConsumed)
				{
					return string.Empty;
				}
				if (current.NodeType == XPathNodeType.Namespace)
				{
					return "http://www.w3.org/2000/xmlns/";
				}
				return current.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return string.Empty;
				}
				if (attributeValueConsumed)
				{
					return string.Empty;
				}
				if (current.NodeType == XPathNodeType.Namespace && current.LocalName != string.Empty)
				{
					return "xmlns";
				}
				return current.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				switch (current.NodeType)
				{
				case XPathNodeType.Attribute:
				case XPathNodeType.Namespace:
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
				case XPathNodeType.ProcessingInstruction:
				case XPathNodeType.Comment:
					return true;
				default:
					return false;
				}
			}
		}

		public override int Depth
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return 0;
				}
				if (NodeType == XmlNodeType.Attribute)
				{
					return depth + 1;
				}
				if (attributeValueConsumed)
				{
					return depth + 2;
				}
				return depth;
			}
		}

		public override string Value
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return string.Empty;
				}
				switch (current.NodeType)
				{
				case XPathNodeType.Attribute:
				case XPathNodeType.Namespace:
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
				case XPathNodeType.ProcessingInstruction:
				case XPathNodeType.Comment:
					return current.Value;
				case XPathNodeType.Root:
				case XPathNodeType.Element:
					return string.Empty;
				default:
					throw new InvalidOperationException("Current XPathNavigator status is {0} which is not acceptable to XmlReader.");
				}
			}
		}

		public override string BaseURI
		{
			get
			{
				return current.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				if (ReadState != ReadState.Interactive)
				{
					return false;
				}
				return current.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				IXmlSchemaInfo xmlSchemaInfo = current as IXmlSchemaInfo;
				return xmlSchemaInfo != null && xmlSchemaInfo.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return '"';
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return current.SchemaInfo;
			}
		}

		public override string XmlLang
		{
			get
			{
				return current.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return attributeCount;
			}
		}

		public override string this[string name]
		{
			get
			{
				string localName;
				string ns;
				SplitName(name, out localName, out ns);
				return this[localName, ns];
			}
		}

		public override string this[string localName, string namespaceURI]
		{
			get
			{
				string attribute = current.GetAttribute(localName, namespaceURI);
				if (attribute != string.Empty)
				{
					return attribute;
				}
				XPathNavigator xPathNavigator = current.Clone();
				return (!xPathNavigator.MoveToAttribute(localName, namespaceURI)) ? null : string.Empty;
			}
		}

		public override bool EOF
		{
			get
			{
				return ReadState == ReadState.EndOfFile;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				if (eof)
				{
					return ReadState.EndOfFile;
				}
				if (closed)
				{
					return ReadState.Closed;
				}
				if (!started)
				{
					return ReadState.Initial;
				}
				return ReadState.Interactive;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return current.NameTable;
			}
		}

		public XPathNavigatorReader(XPathNavigator nav)
		{
			current = nav.Clone();
		}

		private int GetAttributeCount()
		{
			if (ReadState != ReadState.Interactive)
			{
				return 0;
			}
			int num = 0;
			if (current.MoveToFirstAttribute())
			{
				do
				{
					num++;
				}
				while (current.MoveToNextAttribute());
				current.MoveToParent();
			}
			if (current.MoveToFirstNamespace(XPathNamespaceScope.Local))
			{
				do
				{
					num++;
				}
				while (current.MoveToNextNamespace(XPathNamespaceScope.Local));
				current.MoveToParent();
			}
			return num;
		}

		private void SplitName(string name, out string localName, out string ns)
		{
			if (name == "xmlns")
			{
				localName = "xmlns";
				ns = "http://www.w3.org/2000/xmlns/";
				return;
			}
			localName = name;
			ns = string.Empty;
			int num = name.IndexOf(':');
			if (num > 0)
			{
				localName = name.Substring(num + 1, name.Length - num - 1);
				ns = LookupNamespace(name.Substring(0, num));
				if (name.Substring(0, num) == "xmlns")
				{
					ns = "http://www.w3.org/2000/xmlns/";
				}
			}
		}

		public override string GetAttribute(string name)
		{
			string localName;
			string ns;
			SplitName(name, out localName, out ns);
			return this[localName, ns];
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this[localName, namespaceURI];
		}

		public override string GetAttribute(int i)
		{
			return this[i];
		}

		private bool CheckAttributeMove(bool b)
		{
			if (b)
			{
				attributeValueConsumed = false;
			}
			return b;
		}

		public override bool MoveToAttribute(string name)
		{
			string localName;
			string ns;
			SplitName(name, out localName, out ns);
			return MoveToAttribute(localName, ns);
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			bool flag = namespaceURI == "http://www.w3.org/2000/xmlns/";
			XPathNavigator xPathNavigator = null;
			switch (current.NodeType)
			{
			case XPathNodeType.Attribute:
			case XPathNodeType.Namespace:
				xPathNavigator = current.Clone();
				MoveToElement();
				goto case XPathNodeType.Element;
			case XPathNodeType.Element:
				if (!MoveToFirstAttribute())
				{
					break;
				}
				do
				{
					bool flag2 = false;
					if ((!flag) ? (current.LocalName == localName && current.NamespaceURI == namespaceURI) : ((!(localName == "xmlns")) ? (localName == current.Name) : (current.Name == string.Empty)))
					{
						attributeValueConsumed = false;
						return true;
					}
				}
				while (MoveToNextAttribute());
				MoveToElement();
				break;
			}
			if (xPathNavigator != null)
			{
				current = xPathNavigator;
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			switch (current.NodeType)
			{
			case XPathNodeType.Element:
				if (CheckAttributeMove(current.MoveToFirstNamespace(XPathNamespaceScope.Local)))
				{
					return true;
				}
				return CheckAttributeMove(current.MoveToFirstAttribute());
			case XPathNodeType.Attribute:
			case XPathNodeType.Namespace:
				current.MoveToParent();
				goto case XPathNodeType.Element;
			default:
				return false;
			}
		}

		public override bool MoveToNextAttribute()
		{
			switch (current.NodeType)
			{
			case XPathNodeType.Element:
				return MoveToFirstAttribute();
			case XPathNodeType.Namespace:
			{
				if (CheckAttributeMove(current.MoveToNextNamespace(XPathNamespaceScope.Local)))
				{
					return true;
				}
				XPathNavigator other = current.Clone();
				current.MoveToParent();
				if (CheckAttributeMove(current.MoveToFirstAttribute()))
				{
					return true;
				}
				current.MoveTo(other);
				return false;
			}
			case XPathNodeType.Attribute:
				return CheckAttributeMove(current.MoveToNextAttribute());
			default:
				return false;
			}
		}

		public override bool MoveToElement()
		{
			if (current.NodeType == XPathNodeType.Attribute || current.NodeType == XPathNodeType.Namespace)
			{
				attributeValueConsumed = false;
				return current.MoveToParent();
			}
			return false;
		}

		public override void Close()
		{
			closed = true;
			eof = true;
		}

		public override bool Read()
		{
			if (eof)
			{
				return false;
			}
			if (base.Binary != null)
			{
				base.Binary.Reset();
			}
			switch (ReadState)
			{
			case ReadState.Interactive:
				if ((IsEmptyElement || endElement) && root.IsSamePosition(current))
				{
					eof = true;
					return false;
				}
				break;
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return false;
			case ReadState.Initial:
				started = true;
				root = current.Clone();
				if (current.NodeType == XPathNodeType.Root && !current.MoveToFirstChild())
				{
					endElement = false;
					eof = true;
					return false;
				}
				attributeCount = GetAttributeCount();
				return true;
			}
			MoveToElement();
			if (endElement || !current.MoveToFirstChild())
			{
				if (!endElement && !current.IsEmptyElement && current.NodeType == XPathNodeType.Element)
				{
					endElement = true;
				}
				else if (!current.MoveToNext())
				{
					current.MoveToParent();
					if (current.NodeType == XPathNodeType.Root)
					{
						endElement = false;
						eof = true;
						return false;
					}
					endElement = current.NodeType == XPathNodeType.Element;
					if (endElement)
					{
						depth--;
					}
				}
				else
				{
					endElement = false;
				}
			}
			else
			{
				depth++;
			}
			if (!endElement && current.NodeType == XPathNodeType.Element)
			{
				attributeCount = GetAttributeCount();
			}
			else
			{
				attributeCount = 0;
			}
			return true;
		}

		public override string ReadString()
		{
			readStringBuffer.Length = 0;
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

		public override string LookupNamespace(string prefix)
		{
			XPathNavigator xPathNavigator = current.Clone();
			try
			{
				MoveToElement();
				if (current.NodeType != XPathNodeType.Element)
				{
					current.MoveToParent();
				}
				if (current.MoveToFirstNamespace())
				{
					do
					{
						if (current.LocalName == prefix)
						{
							return current.Value;
						}
					}
					while (current.MoveToNextNamespace());
				}
				return null;
			}
			finally
			{
				current = xPathNavigator;
			}
		}

		public override void ResolveEntity()
		{
			throw new InvalidOperationException();
		}

		public override bool ReadAttributeValue()
		{
			if (NodeType != XmlNodeType.Attribute)
			{
				return false;
			}
			if (attributeValueConsumed)
			{
				return false;
			}
			attributeValueConsumed = true;
			return true;
		}
	}
}
