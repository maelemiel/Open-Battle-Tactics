using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class DTMXPathNavigator2 : XPathNavigator, IXmlLineInfo
	{
		private DTMXPathDocument2 document;

		private bool currentIsNode;

		private bool currentIsAttr;

		private int currentNode;

		private int currentAttr;

		private int currentNs;

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

		private XmlNameTable nameTable
		{
			get
			{
				return document.NameTable;
			}
		}

		private DTMXPathLinkedNode2[] nodes
		{
			get
			{
				return document.Nodes;
			}
		}

		private DTMXPathAttributeNode2[] attributes
		{
			get
			{
				return document.Attributes;
			}
		}

		private DTMXPathNamespaceNode2[] namespaces
		{
			get
			{
				return document.Namespaces;
			}
		}

		private string[] atomicStringPool
		{
			get
			{
				return document.AtomicStringPool;
			}
		}

		private string[] nonAtomicStringPool
		{
			get
			{
				return document.NonAtomicStringPool;
			}
		}

		private Hashtable idTable
		{
			get
			{
				return document.IdTable;
			}
		}

		public override string BaseURI
		{
			get
			{
				return atomicStringPool[nodes[currentNode].BaseURI];
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
					return atomicStringPool[nodes[currentNode].LocalName];
				}
				if (currentIsAttr)
				{
					return atomicStringPool[attributes[currentAttr].LocalName];
				}
				return atomicStringPool[namespaces[currentNs].Name];
			}
		}

		public override string Name
		{
			get
			{
				string text;
				string text2;
				if (currentIsNode)
				{
					text = atomicStringPool[nodes[currentNode].Prefix];
					text2 = atomicStringPool[nodes[currentNode].LocalName];
				}
				else
				{
					if (!currentIsAttr)
					{
						return atomicStringPool[namespaces[currentNs].Name];
					}
					text = atomicStringPool[attributes[currentAttr].Prefix];
					text2 = atomicStringPool[attributes[currentAttr].LocalName];
				}
				if (text != string.Empty)
				{
					return text + ':' + text2;
				}
				return text2;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (currentIsNode)
				{
					return atomicStringPool[nodes[currentNode].NamespaceURI];
				}
				if (currentIsAttr)
				{
					return atomicStringPool[attributes[currentAttr].NamespaceURI];
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
					return atomicStringPool[nodes[currentNode].Prefix];
				}
				if (currentIsAttr)
				{
					return atomicStringPool[attributes[currentAttr].Prefix];
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
					return nonAtomicStringPool[attributes[currentAttr].Value];
				}
				if (!currentIsNode)
				{
					return atomicStringPool[namespaces[currentNs].Namespace];
				}
				switch (nodes[currentNode].NodeType)
				{
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
				case XPathNodeType.ProcessingInstruction:
				case XPathNodeType.Comment:
					return nonAtomicStringPool[nodes[currentNode].Value];
				default:
				{
					int firstChild = nodes[currentNode].FirstChild;
					if (firstChild == 0)
					{
						return string.Empty;
					}
					StringBuilder valueBuilder = null;
					BuildValue(firstChild, ref valueBuilder);
					return (valueBuilder != null) ? valueBuilder.ToString() : string.Empty;
				}
				}
			}
		}

		public override string XmlLang
		{
			get
			{
				return atomicStringPool[nodes[currentNode].XmlLang];
			}
		}

		public DTMXPathNavigator2(DTMXPathDocument2 document)
		{
			MoveToRoot();
			this.document = document;
		}

		public DTMXPathNavigator2(DTMXPathNavigator2 org)
		{
			document = org.document;
			currentIsNode = org.currentIsNode;
			currentIsAttr = org.currentIsAttr;
			currentNode = org.currentNode;
			currentAttr = org.currentAttr;
			currentNs = org.currentNs;
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			return true;
		}

		private void BuildValue(int iter, ref StringBuilder valueBuilder)
		{
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
			while (iter < num)
			{
				switch (nodes[iter].NodeType)
				{
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					if (valueBuilder == null)
					{
						valueBuilder = new StringBuilder();
					}
					valueBuilder.Append(nonAtomicStringPool[nodes[iter].Value]);
					break;
				}
				iter++;
			}
		}

		public override XPathNavigator Clone()
		{
			return new DTMXPathNavigator2(this);
		}

		public override XmlNodeOrder ComparePosition(XPathNavigator nav)
		{
			DTMXPathNavigator2 dTMXPathNavigator = nav as DTMXPathNavigator2;
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
					if (atomicStringPool[attributes[num].LocalName] == localName && atomicStringPool[attributes[num].NamespaceURI] == namespaceURI)
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
			return (num == 0) ? string.Empty : nonAtomicStringPool[attributes[num].Value];
		}

		public override string GetNamespace(string name)
		{
			if (currentIsNode && nodes[currentNode].NodeType == XPathNodeType.Element)
			{
				for (int num = nodes[currentNode].FirstNamespace; num != 0; num = namespaces[num].NextNamespace)
				{
					if (atomicStringPool[namespaces[num].Name] == name)
					{
						return atomicStringPool[namespaces[num].Namespace];
					}
				}
			}
			return string.Empty;
		}

		public override bool IsDescendant(XPathNavigator nav)
		{
			DTMXPathNavigator2 dTMXPathNavigator = nav as DTMXPathNavigator2;
			if (dTMXPathNavigator == null || dTMXPathNavigator.document != document)
			{
				return false;
			}
			if (dTMXPathNavigator.currentNode == currentNode)
			{
				return !dTMXPathNavigator.currentIsNode;
			}
			int parent = nodes[dTMXPathNavigator.currentNode].Parent;
			if (parent < currentNode)
			{
				return false;
			}
			while (parent != 0)
			{
				if (parent == currentNode)
				{
					return true;
				}
				parent = nodes[parent].Parent;
			}
			return false;
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			DTMXPathNavigator2 dTMXPathNavigator = other as DTMXPathNavigator2;
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
			DTMXPathNavigator2 dTMXPathNavigator = other as DTMXPathNavigator2;
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
			int previousSibling = nodes[currentNode].PreviousSibling;
			if (previousSibling == 0)
			{
				return false;
			}
			previousSibling = nodes[previousSibling].Parent;
			previousSibling = nodes[previousSibling].FirstChild;
			currentNode = previousSibling;
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
			if (namespaceScope != XPathNamespaceScope.All && namespaces[cur].Namespace == 2)
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
				if (atomicStringPool[namespaces[num].Name] == name)
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
