using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class DTMXPathNavigator : XPathNavigator, IXmlLineInfo
	{
		private XmlNameTable nameTable;

		private DTMXPathDocument document;

		private DTMXPathLinkedNode[] nodes;

		private DTMXPathAttributeNode[] attributes;

		private DTMXPathNamespaceNode[] namespaces;

		private Hashtable idTable;

		private bool currentIsNode;

		private bool currentIsAttr;

		private int currentNode;

		private int currentAttr;

		private int currentNs;

		private StringBuilder valueBuilder;

		int IXmlLineInfo.LineNumber
		{
			get
			{
				return (!currentIsAttr) ? nodes[currentNode].LineNumber : attributes[currentAttr].LineNumber;
			}
		}

		int IXmlLineInfo.LinePosition
		{
			get
			{
				return (!currentIsAttr) ? nodes[currentNode].LinePosition : attributes[currentAttr].LinePosition;
			}
		}

		public override string BaseURI
		{
			get
			{
				return nodes[currentNode].BaseURI;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return currentIsNode && nodes[currentNode].FirstAttribute != 0;
			}
		}

		public override bool HasChildren
		{
			get
			{
				return currentIsNode && nodes[currentNode].FirstChild != 0;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return currentIsNode && nodes[currentNode].IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				if (currentIsNode)
				{
					return nodes[currentNode].LocalName;
				}
				if (currentIsAttr)
				{
					return attributes[currentAttr].LocalName;
				}
				return namespaces[currentNs].Name;
			}
		}

		public override string Name
		{
			get
			{
				string prefix;
				string localName;
				if (currentIsNode)
				{
					prefix = nodes[currentNode].Prefix;
					localName = nodes[currentNode].LocalName;
				}
				else
				{
					if (!currentIsAttr)
					{
						return namespaces[currentNs].Name;
					}
					prefix = attributes[currentAttr].Prefix;
					localName = attributes[currentAttr].LocalName;
				}
				if (prefix != string.Empty)
				{
					return prefix + ':' + localName;
				}
				return localName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (currentIsNode)
				{
					return nodes[currentNode].NamespaceURI;
				}
				if (currentIsAttr)
				{
					return attributes[currentAttr].NamespaceURI;
				}
				return string.Empty;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return nameTable;
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				if (currentIsNode)
				{
					return nodes[currentNode].NodeType;
				}
				if (currentIsAttr)
				{
					return XPathNodeType.Attribute;
				}
				return XPathNodeType.Namespace;
			}
		}

		public override string Prefix
		{
			get
			{
				if (currentIsNode)
				{
					return nodes[currentNode].Prefix;
				}
				if (currentIsAttr)
				{
					return attributes[currentAttr].Prefix;
				}
				return string.Empty;
			}
		}

		public override string Value
		{
			get
			{
				if (currentIsAttr)
				{
					return attributes[currentAttr].Value;
				}
				if (!currentIsNode)
				{
					return namespaces[currentNs].Namespace;
				}
				switch (nodes[currentNode].NodeType)
				{
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
				case XPathNodeType.ProcessingInstruction:
				case XPathNodeType.Comment:
					return nodes[currentNode].Value;
				default:
				{
					int i = nodes[currentNode].FirstChild;
					if (i == 0)
					{
						return string.Empty;
					}
					if (valueBuilder == null)
					{
						valueBuilder = new StringBuilder();
					}
					else
					{
						valueBuilder.Length = 0;
					}
					int num = nodes[currentNode].NextSibling;
					if (num == 0)
					{
						int parent = currentNode;
						do
						{
							parent = nodes[parent].Parent;
							num = nodes[parent].NextSibling;
						}
						while (num == 0 && parent != 0);
						if (num == 0)
						{
							num = nodes.Length;
						}
					}
					for (; i < num; i++)
					{
						switch (nodes[i].NodeType)
						{
						case XPathNodeType.Text:
						case XPathNodeType.SignificantWhitespace:
						case XPathNodeType.Whitespace:
							valueBuilder.Append(nodes[i].Value);
							break;
						}
					}
					return valueBuilder.ToString();
				}
				}
			}
		}

		public override string XmlLang
		{
			get
			{
				return nodes[currentNode].XmlLang;
			}
		}

		public DTMXPathNavigator(DTMXPathDocument document, XmlNameTable nameTable, DTMXPathLinkedNode[] nodes, DTMXPathAttributeNode[] attributes, DTMXPathNamespaceNode[] namespaces, Hashtable idTable)
		{
			this.nodes = nodes;
			this.attributes = attributes;
			this.namespaces = namespaces;
			this.idTable = idTable;
			this.nameTable = nameTable;
			MoveToRoot();
			this.document = document;
		}

		public DTMXPathNavigator(DTMXPathNavigator org)
			: this(org.document, org.nameTable, org.nodes, org.attributes, org.namespaces, org.idTable)
		{
			currentIsNode = org.currentIsNode;
			currentIsAttr = org.currentIsAttr;
			currentNode = org.currentNode;
			currentAttr = org.currentAttr;
			currentNs = org.currentNs;
		}

		internal DTMXPathNavigator(XmlNameTable nt)
		{
			nameTable = nt;
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			return true;
		}

		public override XPathNavigator Clone()
		{
			return new DTMXPathNavigator(this);
		}

		public override XmlNodeOrder ComparePosition(XPathNavigator nav)
		{
			DTMXPathNavigator dTMXPathNavigator = nav as DTMXPathNavigator;
			if (dTMXPathNavigator == null || dTMXPathNavigator.document != document)
			{
				return XmlNodeOrder.Unknown;
			}
			if (currentNode > dTMXPathNavigator.currentNode)
			{
				return XmlNodeOrder.After;
			}
			if (currentNode < dTMXPathNavigator.currentNode)
			{
				return XmlNodeOrder.Before;
			}
			if (dTMXPathNavigator.currentIsAttr)
			{
				if (currentIsAttr)
				{
					if (currentAttr > dTMXPathNavigator.currentAttr)
					{
						return XmlNodeOrder.After;
					}
					if (currentAttr < dTMXPathNavigator.currentAttr)
					{
						return XmlNodeOrder.Before;
					}
					return XmlNodeOrder.Same;
				}
				return XmlNodeOrder.Before;
			}
			if (!dTMXPathNavigator.currentIsNode)
			{
				if (!currentIsNode)
				{
					if (currentNs > dTMXPathNavigator.currentNs)
					{
						return XmlNodeOrder.After;
					}
					if (currentNs < dTMXPathNavigator.currentNs)
					{
						return XmlNodeOrder.Before;
					}
					return XmlNodeOrder.Same;
				}
				return XmlNodeOrder.Before;
			}
			return dTMXPathNavigator.currentIsNode ? XmlNodeOrder.Same : XmlNodeOrder.Before;
		}

		private int findAttribute(string localName, string namespaceURI)
		{
			if (currentIsNode && nodes[currentNode].NodeType == XPathNodeType.Element)
			{
				for (int num = nodes[currentNode].FirstAttribute; num != 0; num = attributes[num].NextAttribute)
				{
					if (attributes[num].LocalName == localName && attributes[num].NamespaceURI == namespaceURI)
					{
						return num;
					}
				}
			}
			return 0;
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			int num = findAttribute(localName, namespaceURI);
			return (num == 0) ? string.Empty : attributes[num].Value;
		}

		public override string GetNamespace(string name)
		{
			if (currentIsNode && nodes[currentNode].NodeType == XPathNodeType.Element)
			{
				for (int num = nodes[currentNode].FirstNamespace; num != 0; num = namespaces[num].NextNamespace)
				{
					if (namespaces[num].Name == name)
					{
						return namespaces[num].Namespace;
					}
				}
			}
			return string.Empty;
		}

		public override bool IsDescendant(XPathNavigator nav)
		{
			DTMXPathNavigator dTMXPathNavigator = nav as DTMXPathNavigator;
			if (dTMXPathNavigator == null || dTMXPathNavigator.document != document)
			{
				return false;
			}
			if (dTMXPathNavigator.currentNode == currentNode)
			{
				return !dTMXPathNavigator.currentIsNode;
			}
			for (int parent = nodes[dTMXPathNavigator.currentNode].Parent; parent != 0; parent = nodes[parent].Parent)
			{
				if (parent == currentNode)
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			DTMXPathNavigator dTMXPathNavigator = other as DTMXPathNavigator;
			if (dTMXPathNavigator == null || dTMXPathNavigator.document != document)
			{
				return false;
			}
			if (currentNode != dTMXPathNavigator.currentNode || currentIsAttr != dTMXPathNavigator.currentIsAttr || currentIsNode != dTMXPathNavigator.currentIsNode)
			{
				return false;
			}
			if (currentIsAttr)
			{
				return currentAttr == dTMXPathNavigator.currentAttr;
			}
			if (!currentIsNode)
			{
				return currentNs == dTMXPathNavigator.currentNs;
			}
			return true;
		}

		public override bool MoveTo(XPathNavigator other)
		{
			DTMXPathNavigator dTMXPathNavigator = other as DTMXPathNavigator;
			if (dTMXPathNavigator == null || dTMXPathNavigator.document != document)
			{
				return false;
			}
			currentNode = dTMXPathNavigator.currentNode;
			currentAttr = dTMXPathNavigator.currentAttr;
			currentNs = dTMXPathNavigator.currentNs;
			currentIsNode = dTMXPathNavigator.currentIsNode;
			currentIsAttr = dTMXPathNavigator.currentIsAttr;
			return true;
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			int num = findAttribute(localName, namespaceURI);
			if (num == 0)
			{
				return false;
			}
			currentAttr = num;
			currentIsAttr = true;
			currentIsNode = false;
			return true;
		}

		public override bool MoveToFirst()
		{
			if (currentIsAttr)
			{
				return false;
			}
			int num = nodes[currentNode].PreviousSibling;
			if (num == 0)
			{
				return false;
			}
			for (int num2 = num; num2 != 0; num2 = nodes[num].PreviousSibling)
			{
				num = num2;
			}
			currentNode = num;
			currentIsNode = true;
			return true;
		}

		public override bool MoveToFirstAttribute()
		{
			if (!currentIsNode)
			{
				return false;
			}
			int firstAttribute = nodes[currentNode].FirstAttribute;
			if (firstAttribute == 0)
			{
				return false;
			}
			currentAttr = firstAttribute;
			currentIsAttr = true;
			currentIsNode = false;
			return true;
		}

		public override bool MoveToFirstChild()
		{
			if (!currentIsNode)
			{
				return false;
			}
			int firstChild = nodes[currentNode].FirstChild;
			if (firstChild == 0)
			{
				return false;
			}
			currentNode = firstChild;
			return true;
		}

		private bool moveToSpecifiedNamespace(int cur, XPathNamespaceScope namespaceScope)
		{
			if (cur == 0)
			{
				return false;
			}
			if (namespaceScope == XPathNamespaceScope.Local && namespaces[cur].DeclaredElement != currentNode)
			{
				return false;
			}
			if (namespaceScope != XPathNamespaceScope.All && namespaces[cur].Namespace == "http://www.w3.org/XML/1998/namespace")
			{
				return false;
			}
			if (cur != 0)
			{
				moveToNamespace(cur);
				return true;
			}
			return false;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
		{
			if (!currentIsNode)
			{
				return false;
			}
			int firstNamespace = nodes[currentNode].FirstNamespace;
			return moveToSpecifiedNamespace(firstNamespace, namespaceScope);
		}

		public override bool MoveToId(string id)
		{
			if (idTable.ContainsKey(id))
			{
				currentNode = (int)idTable[id];
				currentIsNode = true;
				currentIsAttr = false;
				return true;
			}
			return false;
		}

		private void moveToNamespace(int nsNode)
		{
			currentIsNode = (currentIsAttr = false);
			currentNs = nsNode;
		}

		public override bool MoveToNamespace(string name)
		{
			int num = nodes[currentNode].FirstNamespace;
			if (num == 0)
			{
				return false;
			}
			while (num != 0)
			{
				if (namespaces[num].Name == name)
				{
					moveToNamespace(num);
					return true;
				}
				num = namespaces[num].NextNamespace;
			}
			return false;
		}

		public override bool MoveToNext()
		{
			if (currentIsAttr)
			{
				return false;
			}
			int nextSibling = nodes[currentNode].NextSibling;
			if (nextSibling == 0)
			{
				return false;
			}
			currentNode = nextSibling;
			currentIsNode = true;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (!currentIsAttr)
			{
				return false;
			}
			int nextAttribute = attributes[currentAttr].NextAttribute;
			if (nextAttribute == 0)
			{
				return false;
			}
			currentAttr = nextAttribute;
			return true;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
		{
			if (currentIsAttr || currentIsNode)
			{
				return false;
			}
			int nextNamespace = namespaces[currentNs].NextNamespace;
			return moveToSpecifiedNamespace(nextNamespace, namespaceScope);
		}

		public override bool MoveToParent()
		{
			if (!currentIsNode)
			{
				currentIsNode = true;
				currentIsAttr = false;
				return true;
			}
			int parent = nodes[currentNode].Parent;
			if (parent == 0)
			{
				return false;
			}
			currentNode = parent;
			return true;
		}

		public override bool MoveToPrevious()
		{
			if (currentIsAttr)
			{
				return false;
			}
			int previousSibling = nodes[currentNode].PreviousSibling;
			if (previousSibling == 0)
			{
				return false;
			}
			currentNode = previousSibling;
			currentIsNode = true;
			return true;
		}

		public override void MoveToRoot()
		{
			currentNode = 1;
			currentIsNode = true;
			currentIsAttr = false;
		}
	}
}
