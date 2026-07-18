using System;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class DTMXPathDocumentBuilder
	{
		private XmlReader xmlReader;

		private XmlValidatingReader validatingReader;

		private XmlSpace xmlSpace;

		private XmlNameTable nameTable;

		private IXmlLineInfo lineInfo;

		private int nodeCapacity;

		private int attributeCapacity;

		private int nsCapacity;

		private DTMXPathLinkedNode[] nodes;

		private DTMXPathAttributeNode[] attributes;

		private DTMXPathNamespaceNode[] namespaces;

		private Hashtable idTable;

		private int nodeIndex;

		private int attributeIndex;

		private int nsIndex;

		private bool hasAttributes;

		private bool hasLocalNs;

		private int attrIndexAtStart;

		private int nsIndexAtStart;

		private int lastNsInScope;

		private bool skipRead;

		private int[] parentStack = new int[10];

		private int parentStackIndex;

		public DTMXPathDocumentBuilder(string url)
			: this(url, XmlSpace.None, 200)
		{
		}

		public DTMXPathDocumentBuilder(string url, XmlSpace space)
			: this(url, space, 200)
		{
		}

		public DTMXPathDocumentBuilder(string url, XmlSpace space, int defaultCapacity)
		{
			XmlReader xmlReader = null;
			try
			{
				xmlReader = new XmlTextReader(url);
				Init(xmlReader, space, defaultCapacity);
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
		}

		public DTMXPathDocumentBuilder(XmlReader reader)
			: this(reader, XmlSpace.None, 200)
		{
		}

		public DTMXPathDocumentBuilder(XmlReader reader, XmlSpace space)
			: this(reader, space, 200)
		{
		}

		public DTMXPathDocumentBuilder(XmlReader reader, XmlSpace space, int defaultCapacity)
		{
			Init(reader, space, defaultCapacity);
		}

		private void Init(XmlReader reader, XmlSpace space, int defaultCapacity)
		{
			xmlReader = reader;
			validatingReader = reader as XmlValidatingReader;
			lineInfo = reader as IXmlLineInfo;
			xmlSpace = space;
			nameTable = reader.NameTable;
			nodeCapacity = defaultCapacity;
			attributeCapacity = nodeCapacity;
			nsCapacity = 10;
			idTable = new Hashtable();
			nodes = new DTMXPathLinkedNode[nodeCapacity];
			attributes = new DTMXPathAttributeNode[attributeCapacity];
			namespaces = new DTMXPathNamespaceNode[nsCapacity];
			Compile();
		}

		public DTMXPathDocument CreateDocument()
		{
			return new DTMXPathDocument(nameTable, nodes, attributes, namespaces, idTable);
		}

		public void Compile()
		{
			AddNode(0, 0, 0, XPathNodeType.All, string.Empty, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0);
			nodeIndex++;
			AddAttribute(0, null, null, null, null, 0, 0);
			AddNsNode(0, null, null, 0);
			nsIndex++;
			AddNsNode(1, "xml", "http://www.w3.org/XML/1998/namespace", 0);
			AddNode(0, 0, 0, XPathNodeType.Root, xmlReader.BaseURI, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 1, 0, 0);
			nodeIndex = 1;
			lastNsInScope = 1;
			parentStack[0] = nodeIndex;
			while (!xmlReader.EOF)
			{
				Read();
			}
			SetNodeArrayLength(nodeIndex + 1);
			SetAttributeArrayLength(attributeIndex + 1);
			SetNsArrayLength(nsIndex + 1);
			xmlReader = null;
		}

		public void Read()
		{
			if (!skipRead && !xmlReader.Read())
			{
				return;
			}
			skipRead = false;
			int num = parentStack[parentStackIndex];
			int num2 = nodeIndex;
			switch (xmlReader.NodeType)
			{
			case XmlNodeType.Element:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.SignificantWhitespace:
			{
				if (num == nodeIndex)
				{
					num2 = 0;
				}
				else
				{
					while (nodes[num2].Parent != num)
					{
						num2 = nodes[num2].Parent;
					}
				}
				nodeIndex++;
				if (num2 != 0)
				{
					nodes[num2].NextSibling = nodeIndex;
				}
				if (parentStack[parentStackIndex] == nodeIndex - 1)
				{
					nodes[num].FirstChild = nodeIndex;
				}
				string text = null;
				XPathNodeType nodeType = XPathNodeType.Text;
				switch (xmlReader.NodeType)
				{
				default:
					return;
				case XmlNodeType.Element:
					ProcessElement(num, num2);
					return;
				case XmlNodeType.SignificantWhitespace:
					nodeType = XPathNodeType.SignificantWhitespace;
					break;
				case XmlNodeType.Whitespace:
					nodeType = XPathNodeType.Whitespace;
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					break;
				case XmlNodeType.Comment:
					text = xmlReader.Value;
					nodeType = XPathNodeType.Comment;
					break;
				case XmlNodeType.ProcessingInstruction:
					text = xmlReader.Value;
					nodeType = XPathNodeType.ProcessingInstruction;
					break;
				case XmlNodeType.Attribute:
				case XmlNodeType.EntityReference:
				case XmlNodeType.Entity:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentType:
				case XmlNodeType.DocumentFragment:
				case XmlNodeType.Notation:
					return;
				}
				AddNode(num, 0, num2, nodeType, xmlReader.BaseURI, xmlReader.IsEmptyElement, xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.Prefix, text, xmlReader.XmlLang, nsIndex, (lineInfo != null) ? lineInfo.LineNumber : 0, (lineInfo != null) ? lineInfo.LinePosition : 0);
				if (text != null)
				{
					break;
				}
				bool flag = true;
				text = string.Empty;
				XPathNodeType xPathNodeType = XPathNodeType.Whitespace;
				do
				{
					XmlNodeType nodeType2 = xmlReader.NodeType;
					if (nodeType2 != XmlNodeType.Text && nodeType2 != XmlNodeType.CDATA)
					{
						if (nodeType2 != XmlNodeType.Whitespace)
						{
							if (nodeType2 != XmlNodeType.SignificantWhitespace)
							{
								flag = false;
								continue;
							}
							if (xPathNodeType == XPathNodeType.Whitespace)
							{
								xPathNodeType = XPathNodeType.SignificantWhitespace;
							}
						}
					}
					else
					{
						xPathNodeType = XPathNodeType.Text;
					}
					if (xmlReader.NodeType != XmlNodeType.Whitespace || xmlSpace == XmlSpace.Preserve)
					{
						text += xmlReader.Value;
					}
					flag = xmlReader.Read();
					skipRead = true;
				}
				while (flag);
				nodes[nodeIndex].Value = text;
				nodes[nodeIndex].NodeType = xPathNodeType;
				break;
			}
			case XmlNodeType.Whitespace:
				if (xmlSpace == XmlSpace.Preserve)
				{
					goto case XmlNodeType.Element;
				}
				break;
			case XmlNodeType.EndElement:
				parentStackIndex--;
				break;
			case XmlNodeType.Attribute:
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentType:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
				break;
			}
		}

		private void ProcessElement(int parent, int previousSibling)
		{
			WriteStartElement(parent, previousSibling);
			if (xmlReader.MoveToFirstAttribute())
			{
				do
				{
					string prefix = xmlReader.Prefix;
					string namespaceURI = xmlReader.NamespaceURI;
					if (namespaceURI == "http://www.w3.org/2000/xmlns/")
					{
						ProcessNamespace((prefix != null && !(prefix == string.Empty)) ? xmlReader.LocalName : string.Empty, xmlReader.Value);
					}
					else
					{
						ProcessAttribute(prefix, xmlReader.LocalName, namespaceURI, xmlReader.Value);
					}
				}
				while (xmlReader.MoveToNextAttribute());
				xmlReader.MoveToElement();
			}
			CloseStartElement();
		}

		private void PrepareStartElement(int previousSibling)
		{
			hasAttributes = false;
			hasLocalNs = false;
			attrIndexAtStart = attributeIndex;
			nsIndexAtStart = nsIndex;
			while (namespaces[lastNsInScope].DeclaredElement == previousSibling)
			{
				lastNsInScope = namespaces[lastNsInScope].NextNamespace;
			}
		}

		private void WriteStartElement(int parent, int previousSibling)
		{
			PrepareStartElement(previousSibling);
			AddNode(parent, 0, previousSibling, XPathNodeType.Element, xmlReader.BaseURI, xmlReader.IsEmptyElement, xmlReader.LocalName, xmlReader.NamespaceURI, xmlReader.Prefix, string.Empty, xmlReader.XmlLang, lastNsInScope, (lineInfo != null) ? lineInfo.LineNumber : 0, (lineInfo != null) ? lineInfo.LinePosition : 0);
		}

		private void CloseStartElement()
		{
			if (attrIndexAtStart != attributeIndex)
			{
				nodes[nodeIndex].FirstAttribute = attrIndexAtStart + 1;
			}
			if (nsIndexAtStart != nsIndex)
			{
				nodes[nodeIndex].FirstNamespace = nsIndex;
				if (!xmlReader.IsEmptyElement)
				{
					lastNsInScope = nsIndex;
				}
			}
			if (!nodes[nodeIndex].IsEmptyElement)
			{
				parentStackIndex++;
				if (parentStack.Length == parentStackIndex)
				{
					int[] destinationArray = new int[parentStackIndex * 2];
					Array.Copy(parentStack, destinationArray, parentStackIndex);
					parentStack = destinationArray;
				}
				parentStack[parentStackIndex] = nodeIndex;
			}
		}

		private void ProcessNamespace(string prefix, string ns)
		{
			int nextNs = ((!hasLocalNs) ? nodes[nodeIndex].FirstNamespace : nsIndex);
			nsIndex++;
			AddNsNode(nodeIndex, prefix, ns, nextNs);
			hasLocalNs = true;
		}

		private void ProcessAttribute(string prefix, string localName, string ns, string value)
		{
			attributeIndex++;
			AddAttribute(nodeIndex, localName, ns, (prefix == null) ? string.Empty : prefix, value, (lineInfo != null) ? lineInfo.LineNumber : 0, (lineInfo != null) ? lineInfo.LinePosition : 0);
			if (hasAttributes)
			{
				attributes[attributeIndex - 1].NextAttribute = attributeIndex;
			}
			else
			{
				hasAttributes = true;
			}
			if (validatingReader == null)
			{
				return;
			}
			XmlSchemaDatatype xmlSchemaDatatype = validatingReader.SchemaType as XmlSchemaDatatype;
			if (xmlSchemaDatatype == null)
			{
				XmlSchemaType xmlSchemaType = validatingReader.SchemaType as XmlSchemaType;
				if (xmlSchemaType != null)
				{
					xmlSchemaDatatype = xmlSchemaType.Datatype;
				}
			}
			if (xmlSchemaDatatype != null && xmlSchemaDatatype.TokenizedType == XmlTokenizedType.ID)
			{
				idTable.Add(value, nodeIndex);
			}
		}

		private void SetNodeArrayLength(int size)
		{
			DTMXPathLinkedNode[] destinationArray = new DTMXPathLinkedNode[size];
			Array.Copy(nodes, destinationArray, System.Math.Min(size, nodes.Length));
			nodes = destinationArray;
		}

		private void SetAttributeArrayLength(int size)
		{
			DTMXPathAttributeNode[] destinationArray = new DTMXPathAttributeNode[size];
			Array.Copy(attributes, destinationArray, System.Math.Min(size, attributes.Length));
			attributes = destinationArray;
		}

		private void SetNsArrayLength(int size)
		{
			DTMXPathNamespaceNode[] destinationArray = new DTMXPathNamespaceNode[size];
			Array.Copy(namespaces, destinationArray, System.Math.Min(size, namespaces.Length));
			namespaces = destinationArray;
		}

		public void AddNode(int parent, int firstAttribute, int previousSibling, XPathNodeType nodeType, string baseUri, bool isEmptyElement, string localName, string ns, string prefix, string value, string xmlLang, int namespaceNode, int lineNumber, int linePosition)
		{
			if (nodes.Length < nodeIndex + 1)
			{
				nodeCapacity *= 4;
				SetNodeArrayLength(nodeCapacity);
			}
			nodes[nodeIndex].FirstChild = 0;
			nodes[nodeIndex].Parent = parent;
			nodes[nodeIndex].FirstAttribute = firstAttribute;
			nodes[nodeIndex].PreviousSibling = previousSibling;
			nodes[nodeIndex].NextSibling = 0;
			nodes[nodeIndex].NodeType = nodeType;
			nodes[nodeIndex].BaseURI = baseUri;
			nodes[nodeIndex].IsEmptyElement = isEmptyElement;
			nodes[nodeIndex].LocalName = localName;
			nodes[nodeIndex].NamespaceURI = ns;
			nodes[nodeIndex].Prefix = prefix;
			nodes[nodeIndex].Value = value;
			nodes[nodeIndex].XmlLang = xmlLang;
			nodes[nodeIndex].FirstNamespace = namespaceNode;
			nodes[nodeIndex].LineNumber = lineNumber;
			nodes[nodeIndex].LinePosition = linePosition;
		}

		public void AddAttribute(int ownerElement, string localName, string ns, string prefix, string value, int lineNumber, int linePosition)
		{
			if (attributes.Length < attributeIndex + 1)
			{
				attributeCapacity *= 4;
				SetAttributeArrayLength(attributeCapacity);
			}
			attributes[attributeIndex].OwnerElement = ownerElement;
			attributes[attributeIndex].LocalName = localName;
			attributes[attributeIndex].NamespaceURI = ns;
			attributes[attributeIndex].Prefix = prefix;
			attributes[attributeIndex].Value = value;
			attributes[attributeIndex].LineNumber = lineNumber;
			attributes[attributeIndex].LinePosition = linePosition;
		}

		public void AddNsNode(int declaredElement, string name, string ns, int nextNs)
		{
			if (namespaces.Length < nsIndex + 1)
			{
				nsCapacity *= 4;
				SetNsArrayLength(nsCapacity);
			}
			namespaces[nsIndex].DeclaredElement = declaredElement;
			namespaces[nsIndex].Name = name;
			namespaces[nsIndex].Namespace = ns;
			namespaces[nsIndex].NextNamespace = nextNs;
		}
	}
}
