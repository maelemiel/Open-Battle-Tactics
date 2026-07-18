using System;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class DTMXPathDocumentBuilder2
	{
		private XmlReader xmlReader;

		private XmlValidatingReader validatingReader;

		private XmlSpace xmlSpace;

		private XmlNameTable nameTable;

		private IXmlLineInfo lineInfo;

		private int nodeCapacity;

		private int attributeCapacity;

		private int nsCapacity;

		private DTMXPathLinkedNode2[] nodes;

		private DTMXPathAttributeNode2[] attributes;

		private DTMXPathNamespaceNode2[] namespaces;

		private string[] atomicStringPool;

		private int atomicIndex;

		private string[] nonAtomicStringPool;

		private int nonAtomicIndex;

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

		public DTMXPathDocumentBuilder2(string url)
			: this(url, XmlSpace.None, 200)
		{
		}

		public DTMXPathDocumentBuilder2(string url, XmlSpace space)
			: this(url, space, 200)
		{
		}

		public DTMXPathDocumentBuilder2(string url, XmlSpace space, int defaultCapacity)
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

		public DTMXPathDocumentBuilder2(XmlReader reader)
			: this(reader, XmlSpace.None, 200)
		{
		}

		public DTMXPathDocumentBuilder2(XmlReader reader, XmlSpace space)
			: this(reader, space, 200)
		{
		}

		public DTMXPathDocumentBuilder2(XmlReader reader, XmlSpace space, int defaultCapacity)
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
			nodes = new DTMXPathLinkedNode2[nodeCapacity];
			attributes = new DTMXPathAttributeNode2[attributeCapacity];
			namespaces = new DTMXPathNamespaceNode2[nsCapacity];
			atomicStringPool = new string[20];
			nonAtomicStringPool = new string[20];
			Compile();
		}

		public DTMXPathDocument2 CreateDocument()
		{
			return new DTMXPathDocument2(nameTable, nodes, attributes, namespaces, atomicStringPool, nonAtomicStringPool, idTable);
		}

		public void Compile()
		{
			atomicStringPool[0] = (nonAtomicStringPool[0] = string.Empty);
			atomicStringPool[1] = (nonAtomicStringPool[1] = null);
			atomicStringPool[2] = (nonAtomicStringPool[2] = "http://www.w3.org/XML/1998/namespace");
			atomicStringPool[3] = (nonAtomicStringPool[3] = "http://www.w3.org/2000/xmlns/");
			atomicIndex = (nonAtomicIndex = 4);
			AddNode(0, 0, 0, XPathNodeType.All, 0, false, 0, 0, 0, 0, 0, 0, 0, 0);
			nodeIndex++;
			AddAttribute(0, 0, 0, 0, 0, 0, 0);
			AddNsNode(0, 0, 0, 0);
			nsIndex++;
			AddNsNode(1, AtomicIndex("xml"), AtomicIndex("http://www.w3.org/XML/1998/namespace"), 0);
			AddNode(0, 0, 0, XPathNodeType.Root, AtomicIndex(xmlReader.BaseURI), false, 0, 0, 0, 0, 0, 1, 0, 0);
			nodeIndex = 1;
			lastNsInScope = 1;
			parentStack[0] = nodeIndex;
			if (xmlReader.ReadState == ReadState.Initial)
			{
				xmlReader.Read();
			}
			int depth = xmlReader.Depth;
			do
			{
				Read();
			}
			while (skipRead || (xmlReader.Read() && xmlReader.Depth >= depth));
			SetNodeArrayLength(nodeIndex + 1);
			SetAttributeArrayLength(attributeIndex + 1);
			SetNsArrayLength(nsIndex + 1);
			string[] destinationArray = new string[atomicIndex];
			Array.Copy(atomicStringPool, destinationArray, atomicIndex);
			atomicStringPool = destinationArray;
			destinationArray = new string[nonAtomicIndex];
			Array.Copy(nonAtomicStringPool, destinationArray, nonAtomicIndex);
			nonAtomicStringPool = destinationArray;
			xmlReader = null;
		}

		public void Read()
		{
			skipRead = false;
			int num = parentStack[parentStackIndex];
			int num2 = nodeIndex;
			switch (xmlReader.NodeType)
			{
			case XmlNodeType.EndElement:
			{
				int target = parentStack[parentStackIndex];
				AdjustLastNsInScope(target);
				parentStackIndex--;
				break;
			}
			case XmlNodeType.Attribute:
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentType:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
				break;
			case XmlNodeType.Element:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
			{
				string text = null;
				XPathNodeType xPathNodeType = XPathNodeType.Root;
				bool flag = false;
				switch (xmlReader.NodeType)
				{
				default:
					return;
				case XmlNodeType.Element:
					xPathNodeType = XPathNodeType.Element;
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					skipRead = true;
					do
					{
						XmlNodeType nodeType = xmlReader.NodeType;
						if (nodeType != XmlNodeType.Text)
						{
							if (nodeType != XmlNodeType.CDATA)
							{
								switch (nodeType)
								{
								case XmlNodeType.Whitespace:
									if (xPathNodeType == XPathNodeType.Root)
									{
										xPathNodeType = XPathNodeType.Whitespace;
									}
									if (!flag)
									{
										text += xmlReader.Value;
									}
									continue;
								case XmlNodeType.SignificantWhitespace:
									if (xPathNodeType == XPathNodeType.Root || xPathNodeType == XPathNodeType.Whitespace)
									{
										xPathNodeType = XPathNodeType.SignificantWhitespace;
									}
									text += xmlReader.Value;
									continue;
								}
								break;
							}
							if (xPathNodeType != XPathNodeType.Text)
							{
								text = string.Empty;
							}
							flag = true;
						}
						xPathNodeType = XPathNodeType.Text;
						text += xmlReader.Value;
					}
					while (xmlReader.Read());
					break;
				case XmlNodeType.None:
					break;
				case XmlNodeType.Comment:
					text = xmlReader.Value;
					xPathNodeType = XPathNodeType.Comment;
					break;
				case XmlNodeType.ProcessingInstruction:
					text = xmlReader.Value;
					xPathNodeType = XPathNodeType.ProcessingInstruction;
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
				switch (xPathNodeType)
				{
				case XPathNodeType.Whitespace:
					if (xmlSpace != XmlSpace.Preserve)
					{
						return;
					}
					break;
				case XPathNodeType.Root:
					return;
				}
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
				if (xPathNodeType == XPathNodeType.Element)
				{
					ProcessElement(num, num2);
				}
				else
				{
					AddNode(num, 0, num2, xPathNodeType, AtomicIndex(xmlReader.BaseURI), xmlReader.IsEmptyElement, (!skipRead) ? AtomicIndex(xmlReader.LocalName) : 0, (!skipRead) ? AtomicIndex(xmlReader.NamespaceURI) : 0, AtomicIndex(xmlReader.Prefix), (text != null) ? NonAtomicIndex(text) : 0, AtomicIndex(xmlReader.XmlLang), nsIndex, (lineInfo != null) ? lineInfo.LineNumber : 0, (lineInfo != null) ? lineInfo.LinePosition : 0);
				}
				break;
			}
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
			AdjustLastNsInScope(previousSibling);
		}

		private void AdjustLastNsInScope(int target)
		{
			while (namespaces[lastNsInScope].DeclaredElement == target)
			{
				lastNsInScope = namespaces[lastNsInScope].NextNamespace;
			}
		}

		private void WriteStartElement(int parent, int previousSibling)
		{
			PrepareStartElement(previousSibling);
			AddNode(parent, 0, previousSibling, XPathNodeType.Element, AtomicIndex(xmlReader.BaseURI), xmlReader.IsEmptyElement, AtomicIndex(xmlReader.LocalName), AtomicIndex(xmlReader.NamespaceURI), AtomicIndex(xmlReader.Prefix), 0, AtomicIndex(xmlReader.XmlLang), lastNsInScope, (lineInfo != null) ? lineInfo.LineNumber : 0, (lineInfo != null) ? lineInfo.LinePosition : 0);
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
			AddNsNode(nodeIndex, AtomicIndex(prefix), AtomicIndex(ns), nextNs);
			hasLocalNs = true;
		}

		private void ProcessAttribute(string prefix, string localName, string ns, string value)
		{
			attributeIndex++;
			AddAttribute(nodeIndex, AtomicIndex(localName), AtomicIndex(ns), (prefix != null) ? AtomicIndex(prefix) : 0, NonAtomicIndex(value), (lineInfo != null) ? lineInfo.LineNumber : 0, (lineInfo != null) ? lineInfo.LinePosition : 0);
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

		private int AtomicIndex(string s)
		{
			if (s == string.Empty)
			{
				return 0;
			}
			if (s == null)
			{
				return 1;
			}
			for (int i = 2; i < atomicIndex; i++)
			{
				if (object.ReferenceEquals(s, atomicStringPool[i]))
				{
					return i;
				}
			}
			if (atomicIndex == atomicStringPool.Length)
			{
				string[] destinationArray = new string[atomicIndex * 2];
				Array.Copy(atomicStringPool, destinationArray, atomicIndex);
				atomicStringPool = destinationArray;
			}
			atomicStringPool[atomicIndex] = s;
			return atomicIndex++;
		}

		private int NonAtomicIndex(string s)
		{
			if (s == string.Empty)
			{
				return 0;
			}
			if (s == null)
			{
				return 1;
			}
			int i = 2;
			for (int num = ((nonAtomicIndex >= 100) ? 100 : nonAtomicIndex); i < num; i++)
			{
				if (s == nonAtomicStringPool[i])
				{
					return i;
				}
			}
			if (nonAtomicIndex == nonAtomicStringPool.Length)
			{
				string[] destinationArray = new string[nonAtomicIndex * 2];
				Array.Copy(nonAtomicStringPool, destinationArray, nonAtomicIndex);
				nonAtomicStringPool = destinationArray;
			}
			nonAtomicStringPool[nonAtomicIndex] = s;
			return nonAtomicIndex++;
		}

		private void SetNodeArrayLength(int size)
		{
			DTMXPathLinkedNode2[] destinationArray = new DTMXPathLinkedNode2[size];
			Array.Copy(nodes, destinationArray, System.Math.Min(size, nodes.Length));
			nodes = destinationArray;
		}

		private void SetAttributeArrayLength(int size)
		{
			DTMXPathAttributeNode2[] destinationArray = new DTMXPathAttributeNode2[size];
			Array.Copy(attributes, destinationArray, System.Math.Min(size, attributes.Length));
			attributes = destinationArray;
		}

		private void SetNsArrayLength(int size)
		{
			DTMXPathNamespaceNode2[] destinationArray = new DTMXPathNamespaceNode2[size];
			Array.Copy(namespaces, destinationArray, System.Math.Min(size, namespaces.Length));
			namespaces = destinationArray;
		}

		public void AddNode(int parent, int firstAttribute, int previousSibling, XPathNodeType nodeType, int baseUri, bool isEmptyElement, int localName, int ns, int prefix, int value, int xmlLang, int namespaceNode, int lineNumber, int linePosition)
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

		public void AddAttribute(int ownerElement, int localName, int ns, int prefix, int value, int lineNumber, int linePosition)
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

		public void AddNsNode(int declaredElement, int name, int ns, int nextNs)
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
