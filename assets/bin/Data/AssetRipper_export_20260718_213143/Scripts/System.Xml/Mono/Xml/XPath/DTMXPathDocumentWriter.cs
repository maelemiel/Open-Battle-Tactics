using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class DTMXPathDocumentWriter : XmlWriter
	{
		private XmlNameTable nameTable;

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

		private int[] parentStack = new int[10];

		private int parentStackIndex;

		private bool hasAttributes;

		private bool hasLocalNs;

		private int attrIndexAtStart;

		private int nsIndexAtStart;

		private int lastNsInScope;

		private int prevSibling;

		private WriteState state;

		private bool openNamespace;

		private bool isClosed;

		public override string XmlLang
		{
			get
			{
				return null;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		public override WriteState WriteState
		{
			get
			{
				return state;
			}
		}

		public DTMXPathDocumentWriter(XmlNameTable nt, int defaultCapacity)
		{
			nameTable = ((nt != null) ? nt : new NameTable());
			nodeCapacity = defaultCapacity;
			attributeCapacity = nodeCapacity;
			nsCapacity = 10;
			idTable = new Hashtable();
			nodes = new DTMXPathLinkedNode[nodeCapacity];
			attributes = new DTMXPathAttributeNode[attributeCapacity];
			namespaces = new DTMXPathNamespaceNode[nsCapacity];
			Init();
		}

		public DTMXPathDocument CreateDocument()
		{
			if (!isClosed)
			{
				Close();
			}
			return new DTMXPathDocument(nameTable, nodes, attributes, namespaces, idTable);
		}

		public void Init()
		{
			AddNode(0, 0, 0, XPathNodeType.All, string.Empty, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0);
			nodeIndex++;
			AddAttribute(0, null, null, null, null, 0, 0);
			AddNsNode(0, null, null, 0);
			nsIndex++;
			AddNsNode(1, "xml", "http://www.w3.org/XML/1998/namespace", 0);
			AddNode(0, 0, 0, XPathNodeType.Root, null, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 1, 0, 0);
			nodeIndex = 1;
			lastNsInScope = 1;
			parentStack[0] = nodeIndex;
			state = WriteState.Content;
		}

		private int GetParentIndex()
		{
			return parentStack[parentStackIndex];
		}

		private int GetPreviousSiblingIndex()
		{
			int num = parentStack[parentStackIndex];
			if (num == nodeIndex)
			{
				return 0;
			}
			int parent = nodeIndex;
			while (nodes[parent].Parent != num)
			{
				parent = nodes[parent].Parent;
			}
			return parent;
		}

		private void UpdateTreeForAddition()
		{
			int parentIndex = GetParentIndex();
			prevSibling = GetPreviousSiblingIndex();
			nodeIndex++;
			if (prevSibling != 0)
			{
				nodes[prevSibling].NextSibling = nodeIndex;
			}
			if (parentIndex == nodeIndex - 1)
			{
				nodes[parentIndex].FirstChild = nodeIndex;
			}
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
				lastNsInScope = nsIndex;
			}
			parentStackIndex++;
			if (parentStack.Length == parentStackIndex)
			{
				int[] destinationArray = new int[parentStackIndex * 2];
				Array.Copy(parentStack, destinationArray, parentStackIndex);
				parentStack = destinationArray;
			}
			parentStack[parentStackIndex] = nodeIndex;
			state = WriteState.Content;
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

		public override void Close()
		{
			SetNodeArrayLength(nodeIndex + 1);
			SetAttributeArrayLength(attributeIndex + 1);
			SetNsArrayLength(nsIndex + 1);
			isClosed = true;
		}

		public override void Flush()
		{
		}

		public override string LookupPrefix(string ns)
		{
			for (int nextNamespace = nsIndex; nextNamespace != 0; nextNamespace = namespaces[nextNamespace].NextNamespace)
			{
				if (namespaces[nextNamespace].Namespace == ns)
				{
					return namespaces[nextNamespace].Name;
				}
			}
			return null;
		}

		public override void WriteCData(string data)
		{
			AddTextNode(data);
		}

		private void AddTextNode(string data)
		{
			switch (state)
			{
			case WriteState.Element:
				CloseStartElement();
				break;
			default:
				throw new InvalidOperationException("Invalid document state for CDATA section: " + state);
			case WriteState.Content:
				break;
			}
			if (nodes[nodeIndex].Parent == parentStack[parentStackIndex])
			{
				XPathNodeType nodeType = nodes[nodeIndex].NodeType;
				if (nodeType == XPathNodeType.Text || nodeType == XPathNodeType.SignificantWhitespace)
				{
					string text = nodes[nodeIndex].Value + data;
					nodes[nodeIndex].Value = text;
					if (IsWhitespace(text))
					{
						nodes[nodeIndex].NodeType = XPathNodeType.SignificantWhitespace;
					}
					else
					{
						nodes[nodeIndex].NodeType = XPathNodeType.Text;
					}
					return;
				}
			}
			int parentIndex = GetParentIndex();
			UpdateTreeForAddition();
			AddNode(parentIndex, 0, prevSibling, XPathNodeType.Text, null, false, null, string.Empty, string.Empty, data, null, 0, 0, 0);
		}

		private void CheckTopLevelNode()
		{
			switch (state)
			{
			case WriteState.Element:
				CloseStartElement();
				break;
			case WriteState.Start:
			case WriteState.Prolog:
			case WriteState.Content:
				break;
			default:
				throw new InvalidOperationException("Invalid document state for CDATA section: " + state);
			}
		}

		public override void WriteComment(string data)
		{
			CheckTopLevelNode();
			int parentIndex = GetParentIndex();
			UpdateTreeForAddition();
			AddNode(parentIndex, 0, prevSibling, XPathNodeType.Comment, null, false, null, string.Empty, string.Empty, data, null, 0, 0, 0);
		}

		public override void WriteProcessingInstruction(string name, string data)
		{
			CheckTopLevelNode();
			int parentIndex = GetParentIndex();
			UpdateTreeForAddition();
			AddNode(parentIndex, 0, prevSibling, XPathNodeType.ProcessingInstruction, null, false, name, string.Empty, string.Empty, data, null, 0, 0, 0);
		}

		public override void WriteWhitespace(string data)
		{
			CheckTopLevelNode();
			int parentIndex = GetParentIndex();
			UpdateTreeForAddition();
			AddNode(parentIndex, 0, prevSibling, XPathNodeType.Whitespace, null, false, null, string.Empty, string.Empty, data, null, 0, 0, 0);
		}

		public override void WriteStartDocument()
		{
		}

		public override void WriteStartDocument(bool standalone)
		{
		}

		public override void WriteEndDocument()
		{
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			switch (state)
			{
			case WriteState.Element:
				CloseStartElement();
				break;
			default:
				throw new InvalidOperationException("Invalid document state for writing element: " + state);
			case WriteState.Start:
			case WriteState.Prolog:
			case WriteState.Content:
				break;
			}
			int parentIndex = GetParentIndex();
			UpdateTreeForAddition();
			WriteStartElement(parentIndex, prevSibling, prefix, localName, ns);
			state = WriteState.Element;
		}

		private void WriteStartElement(int parent, int previousSibling, string prefix, string localName, string ns)
		{
			PrepareStartElement(previousSibling);
			AddNode(parent, 0, previousSibling, XPathNodeType.Element, null, false, localName, ns, prefix, string.Empty, null, lastNsInScope, 0, 0);
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

		public override void WriteEndElement()
		{
			WriteEndElement(false);
		}

		public override void WriteFullEndElement()
		{
			WriteEndElement(true);
		}

		private void WriteEndElement(bool full)
		{
			switch (state)
			{
			case WriteState.Element:
				CloseStartElement();
				break;
			default:
				throw new InvalidOperationException("Invalid state for writing EndElement: " + state);
			case WriteState.Content:
				break;
			}
			parentStackIndex--;
			if (nodes[nodeIndex].NodeType == XPathNodeType.Element && !full)
			{
				nodes[nodeIndex].IsEmptyElement = true;
			}
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (state != WriteState.Element)
			{
				throw new InvalidOperationException("Invalid document state for attribute: " + state);
			}
			state = WriteState.Attribute;
			if (ns == "http://www.w3.org/2000/xmlns/")
			{
				ProcessNamespace((prefix != null && !(prefix == string.Empty)) ? localName : string.Empty, string.Empty);
			}
			else
			{
				ProcessAttribute(prefix, localName, ns, string.Empty);
			}
		}

		private void ProcessNamespace(string prefix, string ns)
		{
			int nextNs = ((!hasLocalNs) ? nodes[nodeIndex].FirstNamespace : nsIndex);
			nsIndex++;
			AddNsNode(nodeIndex, prefix, ns, nextNs);
			hasLocalNs = true;
			openNamespace = true;
		}

		private void ProcessAttribute(string prefix, string localName, string ns, string value)
		{
			attributeIndex++;
			AddAttribute(nodeIndex, localName, ns, (prefix == null) ? string.Empty : prefix, value, 0, 0);
			if (hasAttributes)
			{
				attributes[attributeIndex - 1].NextAttribute = attributeIndex;
			}
			else
			{
				hasAttributes = true;
			}
		}

		public override void WriteEndAttribute()
		{
			if (state != WriteState.Attribute)
			{
				throw new InvalidOperationException();
			}
			openNamespace = false;
			state = WriteState.Element;
		}

		public override void WriteString(string text)
		{
			if (WriteState == WriteState.Attribute)
			{
				if (openNamespace)
				{
					namespaces[nsIndex].Namespace += text;
				}
				else
				{
					attributes[attributeIndex].Value += text;
				}
			}
			else
			{
				AddTextNode(text);
			}
		}

		public override void WriteRaw(string data)
		{
			WriteString(data);
		}

		public override void WriteRaw(char[] data, int start, int len)
		{
			WriteString(new string(data, start, len));
		}

		public override void WriteName(string name)
		{
			WriteString(name);
		}

		public override void WriteNmToken(string name)
		{
			WriteString(name);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			throw new NotSupportedException();
		}

		public override void WriteCharEntity(char c)
		{
			throw new NotSupportedException();
		}

		public override void WriteDocType(string name, string pub, string sys, string intSubset)
		{
			throw new NotSupportedException();
		}

		public override void WriteEntityRef(string name)
		{
			throw new NotSupportedException();
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			throw new NotSupportedException();
		}

		public override void WriteSurrogateCharEntity(char high, char low)
		{
			throw new NotSupportedException();
		}

		private bool IsWhitespace(string data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				switch (data[i])
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					continue;
				}
				return false;
			}
			return true;
		}
	}
}
