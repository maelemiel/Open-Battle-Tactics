using System.Collections;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	internal class XmlDocumentNavigator : XPathNavigator, IHasXmlNode
	{
		private const string Xmlns = "http://www.w3.org/2000/xmlns/";

		private const string XmlnsXML = "http://www.w3.org/XML/1998/namespace";

		private XmlNode node;

		private XmlAttribute nsNode;

		private ArrayList iteratedNsNames;

		internal XmlDocument Document
		{
			get
			{
				return (node.NodeType != XmlNodeType.Document) ? node.OwnerDocument : (node as XmlDocument);
			}
		}

		public override string BaseURI
		{
			get
			{
				return node.BaseURI;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				if (NsNode != null)
				{
					return false;
				}
				XmlElement xmlElement = node as XmlElement;
				if (xmlElement == null || !xmlElement.HasAttributes)
				{
					return false;
				}
				for (int i = 0; i < node.Attributes.Count; i++)
				{
					if (node.Attributes[i].NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						return true;
					}
				}
				return false;
			}
		}

		public override bool HasChildren
		{
			get
			{
				if (NsNode != null)
				{
					return false;
				}
				XPathNodeType nodeType = NodeType;
				return (nodeType == XPathNodeType.Root || nodeType == XPathNodeType.Element) && GetFirstChild(node) != null;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				if (NsNode != null)
				{
					return false;
				}
				return node.NodeType == XmlNodeType.Element && ((XmlElement)node).IsEmpty;
			}
		}

		public XmlAttribute NsNode
		{
			get
			{
				return nsNode;
			}
			set
			{
				if (value == null)
				{
					iteratedNsNames = null;
				}
				else
				{
					if (iteratedNsNames == null)
					{
						iteratedNsNames = new ArrayList();
					}
					else if (iteratedNsNames.IsReadOnly)
					{
						iteratedNsNames = new ArrayList(iteratedNsNames);
					}
					iteratedNsNames.Add(value.Name);
				}
				nsNode = value;
			}
		}

		public override string LocalName
		{
			get
			{
				XmlAttribute xmlAttribute = NsNode;
				if (xmlAttribute != null)
				{
					if (xmlAttribute == Document.NsNodeXml)
					{
						return "xml";
					}
					return (!(xmlAttribute.Name == "xmlns")) ? xmlAttribute.LocalName : string.Empty;
				}
				XPathNodeType nodeType = NodeType;
				return (nodeType != XPathNodeType.Element && nodeType != XPathNodeType.Attribute && nodeType != XPathNodeType.ProcessingInstruction && nodeType != XPathNodeType.Namespace) ? string.Empty : node.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				if (NsNode != null)
				{
					return LocalName;
				}
				XPathNodeType nodeType = NodeType;
				return (nodeType != XPathNodeType.Element && nodeType != XPathNodeType.Attribute && nodeType != XPathNodeType.ProcessingInstruction && nodeType != XPathNodeType.Namespace) ? string.Empty : node.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return (NsNode == null) ? node.NamespaceURI : string.Empty;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return Document.NameTable;
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				if (NsNode != null)
				{
					return XPathNodeType.Namespace;
				}
				XmlNode xmlNode = node;
				bool flag = false;
				do
				{
					switch (xmlNode.NodeType)
					{
					case XmlNodeType.SignificantWhitespace:
						flag = true;
						xmlNode = GetNextSibling(xmlNode);
						break;
					case XmlNodeType.Whitespace:
						xmlNode = GetNextSibling(xmlNode);
						break;
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						return XPathNodeType.Text;
					default:
						xmlNode = null;
						break;
					}
				}
				while (xmlNode != null);
				return (!flag) ? node.XPathNodeType : XPathNodeType.SignificantWhitespace;
			}
		}

		public override string Prefix
		{
			get
			{
				return (NsNode == null) ? node.Prefix : string.Empty;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				IXmlSchemaInfo result;
				if (NsNode != null)
				{
					IXmlSchemaInfo xmlSchemaInfo = null;
					result = xmlSchemaInfo;
				}
				else
				{
					result = node.SchemaInfo;
				}
				return result;
			}
		}

		public override object UnderlyingObject
		{
			get
			{
				return node;
			}
		}

		public override string Value
		{
			get
			{
				switch (NodeType)
				{
				case XPathNodeType.Attribute:
				case XPathNodeType.ProcessingInstruction:
				case XPathNodeType.Comment:
					return node.Value;
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
				{
					string text = node.Value;
					for (XmlNode nextSibling = GetNextSibling(node); nextSibling != null; text += nextSibling.Value, nextSibling = GetNextSibling(nextSibling))
					{
						switch (nextSibling.XPathNodeType)
						{
						case XPathNodeType.Text:
						case XPathNodeType.SignificantWhitespace:
						case XPathNodeType.Whitespace:
							continue;
						}
						break;
					}
					return text;
				}
				case XPathNodeType.Root:
				case XPathNodeType.Element:
					return node.InnerText;
				case XPathNodeType.Namespace:
					return (NsNode != Document.NsNodeXml) ? NsNode.Value : "http://www.w3.org/XML/1998/namespace";
				default:
					return string.Empty;
				}
			}
		}

		public override string XmlLang
		{
			get
			{
				return node.XmlLang;
			}
		}

		private XmlNode Node
		{
			get
			{
				return (NsNode == null) ? node : NsNode;
			}
		}

		internal XmlDocumentNavigator(XmlNode node)
		{
			this.node = node;
			if (node.NodeType == XmlNodeType.Attribute && node.NamespaceURI == "http://www.w3.org/2000/xmlns/")
			{
				nsNode = (XmlAttribute)node;
				node = nsNode.OwnerElement;
			}
		}

		XmlNode IHasXmlNode.GetNode()
		{
			return Node;
		}

		private bool CheckNsNameAppearance(string name, string ns)
		{
			if (iteratedNsNames != null && iteratedNsNames.Contains(name))
			{
				return true;
			}
			if (ns == string.Empty)
			{
				if (iteratedNsNames == null)
				{
					iteratedNsNames = new ArrayList();
				}
				else if (iteratedNsNames.IsReadOnly)
				{
					iteratedNsNames = new ArrayList(iteratedNsNames);
				}
				iteratedNsNames.Add("xmlns");
				return true;
			}
			return false;
		}

		public override XPathNavigator Clone()
		{
			XmlDocumentNavigator xmlDocumentNavigator = new XmlDocumentNavigator(node);
			xmlDocumentNavigator.nsNode = nsNode;
			xmlDocumentNavigator.iteratedNsNames = ((iteratedNsNames != null && !iteratedNsNames.IsReadOnly) ? ArrayList.ReadOnly(iteratedNsNames) : iteratedNsNames);
			return xmlDocumentNavigator;
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			if (HasAttributes)
			{
				XmlElement xmlElement = Node as XmlElement;
				return (xmlElement == null) ? string.Empty : xmlElement.GetAttribute(localName, namespaceURI);
			}
			return string.Empty;
		}

		public override string GetNamespace(string name)
		{
			return Node.GetNamespaceOfPrefix(name);
		}

		public override bool IsDescendant(XPathNavigator other)
		{
			if (NsNode != null)
			{
				return false;
			}
			XmlDocumentNavigator xmlDocumentNavigator = other as XmlDocumentNavigator;
			if (xmlDocumentNavigator == null)
			{
				return false;
			}
			for (XmlNode xmlNode = ((xmlDocumentNavigator.node.NodeType != XmlNodeType.Attribute) ? xmlDocumentNavigator.node.ParentNode : ((XmlAttribute)xmlDocumentNavigator.node).OwnerElement); xmlNode != null; xmlNode = xmlNode.ParentNode)
			{
				if (xmlNode == node)
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			XmlDocumentNavigator xmlDocumentNavigator = other as XmlDocumentNavigator;
			if (xmlDocumentNavigator != null)
			{
				return node == xmlDocumentNavigator.node && NsNode == xmlDocumentNavigator.NsNode;
			}
			return false;
		}

		public override bool MoveTo(XPathNavigator other)
		{
			XmlDocumentNavigator xmlDocumentNavigator = other as XmlDocumentNavigator;
			if (xmlDocumentNavigator != null && Document == xmlDocumentNavigator.Document)
			{
				node = xmlDocumentNavigator.node;
				NsNode = xmlDocumentNavigator.NsNode;
				return true;
			}
			return false;
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			if (HasAttributes)
			{
				XmlAttribute xmlAttribute = node.Attributes[localName, namespaceURI];
				if (xmlAttribute != null)
				{
					node = xmlAttribute;
					NsNode = null;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			if (NodeType == XPathNodeType.Element)
			{
				XmlElement xmlElement = node as XmlElement;
				if (!xmlElement.HasAttributes)
				{
					return false;
				}
				for (int i = 0; i < node.Attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = node.Attributes[i];
					if (xmlAttribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						node = xmlAttribute;
						NsNode = null;
						return true;
					}
				}
			}
			return false;
		}

		public override bool MoveToFirstChild()
		{
			if (HasChildren)
			{
				XmlNode firstChild = GetFirstChild(node);
				if (firstChild == null)
				{
					return false;
				}
				node = firstChild;
				return true;
			}
			return false;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
		{
			if (NodeType != XPathNodeType.Element)
			{
				return false;
			}
			XmlElement xmlElement = node as XmlElement;
			do
			{
				if (xmlElement.HasAttributes)
				{
					for (int i = 0; i < xmlElement.Attributes.Count; i++)
					{
						XmlAttribute xmlAttribute = xmlElement.Attributes[i];
						if (xmlAttribute.NamespaceURI == "http://www.w3.org/2000/xmlns/" && !CheckNsNameAppearance(xmlAttribute.Name, xmlAttribute.Value))
						{
							NsNode = xmlAttribute;
							return true;
						}
					}
				}
				if (namespaceScope == XPathNamespaceScope.Local)
				{
					return false;
				}
				xmlElement = GetParentNode(xmlElement) as XmlElement;
			}
			while (xmlElement != null);
			if (namespaceScope == XPathNamespaceScope.All)
			{
				if (CheckNsNameAppearance(Document.NsNodeXml.Name, Document.NsNodeXml.Value))
				{
					return false;
				}
				NsNode = Document.NsNodeXml;
				return true;
			}
			return false;
		}

		public override bool MoveToId(string id)
		{
			XmlElement elementById = Document.GetElementById(id);
			if (elementById == null)
			{
				return false;
			}
			node = elementById;
			return true;
		}

		public override bool MoveToNamespace(string name)
		{
			if (name == "xml")
			{
				NsNode = Document.NsNodeXml;
				return true;
			}
			if (NodeType != XPathNodeType.Element)
			{
				return false;
			}
			XmlElement xmlElement = node as XmlElement;
			do
			{
				if (xmlElement.HasAttributes)
				{
					for (int i = 0; i < xmlElement.Attributes.Count; i++)
					{
						XmlAttribute xmlAttribute = xmlElement.Attributes[i];
						if (xmlAttribute.NamespaceURI == "http://www.w3.org/2000/xmlns/" && xmlAttribute.Name == name)
						{
							NsNode = xmlAttribute;
							return true;
						}
					}
				}
				xmlElement = GetParentNode(node) as XmlElement;
			}
			while (xmlElement != null);
			return false;
		}

		public override bool MoveToNext()
		{
			if (NsNode != null)
			{
				return false;
			}
			XmlNode nextSibling = node;
			if (NodeType == XPathNodeType.Text)
			{
				XmlNodeType nodeType;
				do
				{
					nextSibling = GetNextSibling(nextSibling);
					if (nextSibling == null)
					{
						return false;
					}
					nodeType = nextSibling.NodeType;
				}
				while (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace);
			}
			else
			{
				nextSibling = GetNextSibling(nextSibling);
			}
			if (nextSibling == null)
			{
				return false;
			}
			node = nextSibling;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (node == null)
			{
				return false;
			}
			if (NodeType != XPathNodeType.Attribute)
			{
				return false;
			}
			int i = 0;
			XmlElement ownerElement = ((XmlAttribute)node).OwnerElement;
			if (ownerElement == null)
			{
				return false;
			}
			int count;
			for (count = ownerElement.Attributes.Count; i < count && ownerElement.Attributes[i] != node; i++)
			{
			}
			if (i == count)
			{
				return false;
			}
			for (i++; i < count; i++)
			{
				if (ownerElement.Attributes[i].NamespaceURI != "http://www.w3.org/2000/xmlns/")
				{
					node = ownerElement.Attributes[i];
					NsNode = null;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
		{
			if (NsNode == Document.NsNodeXml)
			{
				return false;
			}
			if (NsNode == null)
			{
				return false;
			}
			int i = 0;
			XmlElement ownerElement = NsNode.OwnerElement;
			if (ownerElement == null)
			{
				return false;
			}
			int count;
			for (count = ownerElement.Attributes.Count; i < count && ownerElement.Attributes[i] != NsNode; i++)
			{
			}
			if (i == count)
			{
				return false;
			}
			for (i++; i < count; i++)
			{
				if (ownerElement.Attributes[i].NamespaceURI == "http://www.w3.org/2000/xmlns/")
				{
					XmlAttribute xmlAttribute = ownerElement.Attributes[i];
					if (!CheckNsNameAppearance(xmlAttribute.Name, xmlAttribute.Value))
					{
						NsNode = xmlAttribute;
						return true;
					}
				}
			}
			if (namespaceScope == XPathNamespaceScope.Local)
			{
				return false;
			}
			for (ownerElement = GetParentNode(ownerElement) as XmlElement; ownerElement != null; ownerElement = GetParentNode(ownerElement) as XmlElement)
			{
				if (ownerElement.HasAttributes)
				{
					for (int j = 0; j < ownerElement.Attributes.Count; j++)
					{
						XmlAttribute xmlAttribute2 = ownerElement.Attributes[j];
						if (xmlAttribute2.NamespaceURI == "http://www.w3.org/2000/xmlns/" && !CheckNsNameAppearance(xmlAttribute2.Name, xmlAttribute2.Value))
						{
							NsNode = xmlAttribute2;
							return true;
						}
					}
				}
			}
			if (namespaceScope == XPathNamespaceScope.All)
			{
				if (CheckNsNameAppearance(Document.NsNodeXml.Name, Document.NsNodeXml.Value))
				{
					return false;
				}
				NsNode = Document.NsNodeXml;
				return true;
			}
			return false;
		}

		public override bool MoveToParent()
		{
			if (NsNode != null)
			{
				NsNode = null;
				return true;
			}
			if (node.NodeType == XmlNodeType.Attribute)
			{
				XmlElement ownerElement = ((XmlAttribute)node).OwnerElement;
				if (ownerElement != null)
				{
					node = ownerElement;
					NsNode = null;
					return true;
				}
				return false;
			}
			XmlNode parentNode = GetParentNode(node);
			if (parentNode == null)
			{
				return false;
			}
			node = parentNode;
			NsNode = null;
			return true;
		}

		public override bool MoveToPrevious()
		{
			if (NsNode != null)
			{
				return false;
			}
			XmlNode previousSibling = GetPreviousSibling(node);
			if (previousSibling == null)
			{
				return false;
			}
			node = previousSibling;
			return true;
		}

		public override void MoveToRoot()
		{
			XmlAttribute xmlAttribute = node as XmlAttribute;
			XmlNode xmlNode = ((xmlAttribute == null) ? node : xmlAttribute.OwnerElement);
			if (xmlNode != null)
			{
				for (XmlNode parentNode = GetParentNode(xmlNode); parentNode != null; parentNode = GetParentNode(parentNode))
				{
					xmlNode = parentNode;
				}
				node = xmlNode;
				NsNode = null;
			}
		}

		private XmlNode GetFirstChild(XmlNode n)
		{
			if (n.FirstChild == null)
			{
				return null;
			}
			switch (n.FirstChild.NodeType)
			{
			case XmlNodeType.DocumentType:
			case XmlNodeType.XmlDeclaration:
				return GetNextSibling(n.FirstChild);
			case XmlNodeType.EntityReference:
				foreach (XmlNode childNode in n.ChildNodes)
				{
					if (childNode.NodeType == XmlNodeType.EntityReference)
					{
						XmlNode firstChild = GetFirstChild(childNode);
						if (firstChild != null)
						{
							return firstChild;
						}
						continue;
					}
					return childNode;
				}
				return null;
			default:
				return n.FirstChild;
			}
		}

		private XmlNode GetLastChild(XmlNode n)
		{
			if (n.LastChild == null)
			{
				return null;
			}
			switch (n.LastChild.NodeType)
			{
			case XmlNodeType.DocumentType:
			case XmlNodeType.XmlDeclaration:
				return GetPreviousSibling(n.LastChild);
			case XmlNodeType.EntityReference:
			{
				XmlNode xmlNode = n.LastChild;
				while (xmlNode != null)
				{
					if (xmlNode.NodeType == XmlNodeType.EntityReference)
					{
						XmlNode lastChild = GetLastChild(xmlNode);
						if (lastChild != null)
						{
							return lastChild;
						}
						xmlNode = xmlNode.PreviousSibling;
						continue;
					}
					return xmlNode;
				}
				return null;
			}
			default:
				return n.LastChild;
			}
		}

		private XmlNode GetPreviousSibling(XmlNode n)
		{
			XmlNode previousSibling = n.PreviousSibling;
			if (previousSibling != null)
			{
				switch (previousSibling.NodeType)
				{
				case XmlNodeType.EntityReference:
				{
					XmlNode lastChild = GetLastChild(previousSibling);
					if (lastChild != null)
					{
						return lastChild;
					}
					return GetPreviousSibling(previousSibling);
				}
				case XmlNodeType.DocumentType:
				case XmlNodeType.XmlDeclaration:
					return GetPreviousSibling(previousSibling);
				default:
					return previousSibling;
				}
			}
			if (n.ParentNode == null || n.ParentNode.NodeType != XmlNodeType.EntityReference)
			{
				return null;
			}
			return GetPreviousSibling(n.ParentNode);
		}

		private XmlNode GetNextSibling(XmlNode n)
		{
			XmlNode nextSibling = n.NextSibling;
			if (nextSibling != null)
			{
				switch (nextSibling.NodeType)
				{
				case XmlNodeType.EntityReference:
				{
					XmlNode firstChild = GetFirstChild(nextSibling);
					if (firstChild != null)
					{
						return firstChild;
					}
					return GetNextSibling(nextSibling);
				}
				case XmlNodeType.DocumentType:
				case XmlNodeType.XmlDeclaration:
					return GetNextSibling(nextSibling);
				default:
					return n.NextSibling;
				}
			}
			if (n.ParentNode == null || n.ParentNode.NodeType != XmlNodeType.EntityReference)
			{
				return null;
			}
			return GetNextSibling(n.ParentNode);
		}

		private XmlNode GetParentNode(XmlNode n)
		{
			if (n.ParentNode == null)
			{
				return null;
			}
			for (XmlNode parentNode = n.ParentNode; parentNode != null; parentNode = parentNode.ParentNode)
			{
				if (parentNode.NodeType != XmlNodeType.EntityReference)
				{
					return parentNode;
				}
			}
			return null;
		}

		public override string LookupNamespace(string prefix)
		{
			return base.LookupNamespace(prefix);
		}

		public override string LookupPrefix(string namespaceUri)
		{
			return base.LookupPrefix(namespaceUri);
		}

		public override bool MoveToChild(XPathNodeType type)
		{
			return base.MoveToChild(type);
		}

		public override bool MoveToChild(string localName, string namespaceURI)
		{
			return base.MoveToChild(localName, namespaceURI);
		}

		public override bool MoveToNext(string localName, string namespaceURI)
		{
			return base.MoveToNext(localName, namespaceURI);
		}

		public override bool MoveToNext(XPathNodeType type)
		{
			return base.MoveToNext(type);
		}

		public override bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
		{
			return base.MoveToFollowing(localName, namespaceURI, end);
		}

		public override bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
		{
			return base.MoveToFollowing(type, end);
		}
	}
}
