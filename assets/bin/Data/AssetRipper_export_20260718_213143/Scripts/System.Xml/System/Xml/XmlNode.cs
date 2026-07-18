using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	public abstract class XmlNode : IEnumerable, ICloneable, IXPathNavigable
	{
		private class EmptyNodeList : XmlNodeList
		{
			private static IEnumerator emptyEnumerator = new object[0].GetEnumerator();

			public override int Count
			{
				get
				{
					return 0;
				}
			}

			public override IEnumerator GetEnumerator()
			{
				return emptyEnumerator;
			}

			public override XmlNode Item(int index)
			{
				return null;
			}
		}

		private static EmptyNodeList emptyList = new EmptyNodeList();

		private XmlDocument ownerDocument;

		private XmlNode parentNode;

		private XmlNodeListChildren childNodes;

		public virtual XmlAttributeCollection Attributes
		{
			get
			{
				return null;
			}
		}

		public virtual string BaseURI
		{
			get
			{
				return (ParentNode == null) ? string.Empty : ParentNode.ChildrenBaseURI;
			}
		}

		internal virtual string ChildrenBaseURI
		{
			get
			{
				return BaseURI;
			}
		}

		public virtual XmlNodeList ChildNodes
		{
			get
			{
				IHasXmlChildNode hasXmlChildNode = this as IHasXmlChildNode;
				if (hasXmlChildNode == null)
				{
					return emptyList;
				}
				if (childNodes == null)
				{
					childNodes = new XmlNodeListChildren(hasXmlChildNode);
				}
				return childNodes;
			}
		}

		public virtual XmlNode FirstChild
		{
			get
			{
				IHasXmlChildNode hasXmlChildNode = this as IHasXmlChildNode;
				XmlLinkedNode xmlLinkedNode = ((hasXmlChildNode != null) ? hasXmlChildNode.LastLinkedChild : null);
				return (xmlLinkedNode != null) ? xmlLinkedNode.NextLinkedSibling : null;
			}
		}

		public virtual bool HasChildNodes
		{
			get
			{
				return LastChild != null;
			}
		}

		public virtual string InnerText
		{
			get
			{
				XmlNodeType nodeType = NodeType;
				if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
				{
					return Value;
				}
				if (FirstChild == null)
				{
					return string.Empty;
				}
				if (FirstChild == LastChild)
				{
					return (FirstChild.NodeType == XmlNodeType.Comment) ? string.Empty : FirstChild.InnerText;
				}
				StringBuilder builder = null;
				AppendChildValues(ref builder);
				return (builder != null) ? builder.ToString() : string.Empty;
			}
			set
			{
				if (!(this is XmlDocumentFragment))
				{
					throw new InvalidOperationException("This node is read only. Cannot be modified.");
				}
				RemoveAll();
				AppendChild(OwnerDocument.CreateTextNode(value));
			}
		}

		public virtual string InnerXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter w = new XmlTextWriter(stringWriter);
				WriteContentTo(w);
				return stringWriter.GetStringBuilder().ToString();
			}
			set
			{
				throw new InvalidOperationException("This node is readonly or doesn't have any children.");
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				XmlNode xmlNode = this;
				do
				{
					switch (xmlNode.NodeType)
					{
					case XmlNodeType.EntityReference:
					case XmlNodeType.Entity:
						return true;
					case XmlNodeType.Attribute:
						xmlNode = ((XmlAttribute)xmlNode).OwnerElement;
						break;
					default:
						xmlNode = xmlNode.ParentNode;
						break;
					}
				}
				while (xmlNode != null);
				return false;
			}
		}

		public virtual XmlElement this[string name]
		{
			get
			{
				for (int i = 0; i < ChildNodes.Count; i++)
				{
					XmlNode xmlNode = ChildNodes[i];
					if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == name)
					{
						return (XmlElement)xmlNode;
					}
				}
				return null;
			}
		}

		public virtual XmlElement this[string localname, string ns]
		{
			get
			{
				for (int i = 0; i < ChildNodes.Count; i++)
				{
					XmlNode xmlNode = ChildNodes[i];
					if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.LocalName == localname && xmlNode.NamespaceURI == ns)
					{
						return (XmlElement)xmlNode;
					}
				}
				return null;
			}
		}

		public virtual XmlNode LastChild
		{
			get
			{
				IHasXmlChildNode hasXmlChildNode = this as IHasXmlChildNode;
				return (hasXmlChildNode != null) ? hasXmlChildNode.LastLinkedChild : null;
			}
		}

		public abstract string LocalName { get; }

		public abstract string Name { get; }

		public virtual string NamespaceURI
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual XmlNode NextSibling
		{
			get
			{
				return null;
			}
		}

		public abstract XmlNodeType NodeType { get; }

		internal virtual XPathNodeType XPathNodeType
		{
			get
			{
				throw new InvalidOperationException("Can not get XPath node type from " + GetType().ToString());
			}
		}

		public virtual string OuterXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter w = new XmlTextWriter(stringWriter);
				WriteTo(w);
				return stringWriter.ToString();
			}
		}

		public virtual XmlDocument OwnerDocument
		{
			get
			{
				return ownerDocument;
			}
		}

		public virtual XmlNode ParentNode
		{
			get
			{
				return parentNode;
			}
		}

		public virtual string Prefix
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public virtual XmlNode PreviousSibling
		{
			get
			{
				return null;
			}
		}

		public virtual string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException("This node does not have a value");
			}
		}

		internal virtual string XmlLang
		{
			get
			{
				if (Attributes != null)
				{
					for (int i = 0; i < Attributes.Count; i++)
					{
						XmlAttribute xmlAttribute = Attributes[i];
						if (xmlAttribute.Name == "xml:lang")
						{
							return xmlAttribute.Value;
						}
					}
				}
				return (ParentNode == null) ? OwnerDocument.XmlLang : ParentNode.XmlLang;
			}
		}

		internal virtual XmlSpace XmlSpace
		{
			get
			{
				if (Attributes != null)
				{
					for (int i = 0; i < Attributes.Count; i++)
					{
						XmlAttribute xmlAttribute = Attributes[i];
						if (xmlAttribute.Name == "xml:space")
						{
							switch (xmlAttribute.Value)
							{
							case "preserve":
								return XmlSpace.Preserve;
							case "default":
								return XmlSpace.Default;
							}
							break;
						}
					}
				}
				return (ParentNode == null) ? OwnerDocument.XmlSpace : ParentNode.XmlSpace;
			}
		}

		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return null;
			}
			internal set
			{
			}
		}

		internal XmlElement AttributeOwnerElement
		{
			get
			{
				return (XmlElement)parentNode;
			}
			set
			{
				parentNode = value;
			}
		}

		internal XmlNode(XmlDocument ownerDocument)
		{
			this.ownerDocument = ownerDocument;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void AppendChildValues(ref StringBuilder builder)
		{
			for (XmlNode xmlNode = FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
				{
					if (builder == null)
					{
						builder = new StringBuilder();
					}
					builder.Append(xmlNode.Value);
				}
				xmlNode.AppendChildValues(ref builder);
			}
		}

		public virtual XmlNode AppendChild(XmlNode newChild)
		{
			return InsertBefore(newChild, null);
		}

		internal XmlNode AppendChild(XmlNode newChild, bool checkNodeType)
		{
			return InsertBefore(newChild, null, checkNodeType, true);
		}

		public virtual XmlNode Clone()
		{
			return CloneNode(true);
		}

		public abstract XmlNode CloneNode(bool deep);

		public virtual XPathNavigator CreateNavigator()
		{
			return OwnerDocument.CreateNavigator(this);
		}

		public IEnumerator GetEnumerator()
		{
			return ChildNodes.GetEnumerator();
		}

		public virtual string GetNamespaceOfPrefix(string prefix)
		{
			switch (prefix)
			{
			case null:
				throw new ArgumentNullException("prefix");
			case "xml":
				return "http://www.w3.org/XML/1998/namespace";
			case "xmlns":
				return "http://www.w3.org/2000/xmlns/";
			default:
			{
				XmlNode xmlNode;
				switch (NodeType)
				{
				case XmlNodeType.Attribute:
					xmlNode = ((XmlAttribute)this).OwnerElement;
					if (xmlNode == null)
					{
						return string.Empty;
					}
					break;
				case XmlNodeType.Element:
					xmlNode = this;
					break;
				default:
					xmlNode = ParentNode;
					break;
				}
				while (xmlNode != null)
				{
					if (xmlNode.Prefix == prefix)
					{
						return xmlNode.NamespaceURI;
					}
					if (xmlNode.NodeType == XmlNodeType.Element && ((XmlElement)xmlNode).HasAttributes)
					{
						int count = xmlNode.Attributes.Count;
						for (int i = 0; i < count; i++)
						{
							XmlAttribute xmlAttribute = xmlNode.Attributes[i];
							if ((prefix == xmlAttribute.LocalName && xmlAttribute.Prefix == "xmlns") || (xmlAttribute.Name == "xmlns" && prefix == string.Empty))
							{
								return xmlAttribute.Value;
							}
						}
					}
					xmlNode = xmlNode.ParentNode;
				}
				return string.Empty;
			}
			}
		}

		public virtual string GetPrefixOfNamespace(string namespaceURI)
		{
			switch (namespaceURI)
			{
			case "http://www.w3.org/XML/1998/namespace":
				return "xml";
			case "http://www.w3.org/2000/xmlns/":
				return "xmlns";
			default:
			{
				XmlNode xmlNode;
				switch (NodeType)
				{
				case XmlNodeType.Attribute:
					xmlNode = ((XmlAttribute)this).OwnerElement;
					break;
				case XmlNodeType.Element:
					xmlNode = this;
					break;
				default:
					xmlNode = ParentNode;
					break;
				}
				while (xmlNode != null)
				{
					if (xmlNode.NodeType == XmlNodeType.Element && ((XmlElement)xmlNode).HasAttributes)
					{
						for (int i = 0; i < xmlNode.Attributes.Count; i++)
						{
							XmlAttribute xmlAttribute = xmlNode.Attributes[i];
							if (xmlAttribute.Prefix == "xmlns" && xmlAttribute.Value == namespaceURI)
							{
								return xmlAttribute.LocalName;
							}
							if (xmlAttribute.Name == "xmlns" && xmlAttribute.Value == namespaceURI)
							{
								return string.Empty;
							}
						}
					}
					xmlNode = xmlNode.ParentNode;
				}
				return string.Empty;
			}
			}
		}

		public virtual XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
		{
			XmlNode refChild2 = null;
			if (refChild != null)
			{
				refChild2 = refChild.NextSibling;
			}
			else if (FirstChild != null)
			{
				refChild2 = FirstChild;
			}
			return InsertBefore(newChild, refChild2);
		}

		public virtual XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
		{
			return InsertBefore(newChild, refChild, true, true);
		}

		internal bool IsAncestor(XmlNode newChild)
		{
			for (XmlNode xmlNode = ParentNode; xmlNode != null; xmlNode = xmlNode.ParentNode)
			{
				if (xmlNode == newChild)
				{
					return true;
				}
			}
			return false;
		}

		internal XmlNode InsertBefore(XmlNode newChild, XmlNode refChild, bool checkNodeType, bool raiseEvent)
		{
			if (checkNodeType)
			{
				CheckNodeInsertion(newChild, refChild);
			}
			if (newChild == refChild)
			{
				return newChild;
			}
			IHasXmlChildNode hasXmlChildNode = (IHasXmlChildNode)this;
			XmlDocument xmlDocument = ((NodeType != XmlNodeType.Document) ? OwnerDocument : ((XmlDocument)this));
			if (raiseEvent)
			{
				xmlDocument.onNodeInserting(newChild, this);
			}
			if (newChild.ParentNode != null)
			{
				newChild.ParentNode.RemoveChild(newChild, checkNodeType);
			}
			if (newChild.NodeType == XmlNodeType.DocumentFragment)
			{
				while (newChild.FirstChild != null)
				{
					InsertBefore(newChild.FirstChild, refChild);
				}
			}
			else
			{
				XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
				xmlLinkedNode.parentNode = this;
				if (refChild == null)
				{
					if (hasXmlChildNode.LastLinkedChild != null)
					{
						XmlLinkedNode nextLinkedSibling = (XmlLinkedNode)FirstChild;
						hasXmlChildNode.LastLinkedChild.NextLinkedSibling = xmlLinkedNode;
						hasXmlChildNode.LastLinkedChild = xmlLinkedNode;
						xmlLinkedNode.NextLinkedSibling = nextLinkedSibling;
					}
					else
					{
						hasXmlChildNode.LastLinkedChild = xmlLinkedNode;
						hasXmlChildNode.LastLinkedChild.NextLinkedSibling = xmlLinkedNode;
					}
				}
				else
				{
					XmlLinkedNode xmlLinkedNode2 = refChild.PreviousSibling as XmlLinkedNode;
					if (xmlLinkedNode2 == null)
					{
						hasXmlChildNode.LastLinkedChild.NextLinkedSibling = xmlLinkedNode;
					}
					else
					{
						xmlLinkedNode2.NextLinkedSibling = xmlLinkedNode;
					}
					xmlLinkedNode.NextLinkedSibling = refChild as XmlLinkedNode;
				}
				switch (newChild.NodeType)
				{
				case XmlNodeType.EntityReference:
					((XmlEntityReference)newChild).SetReferencedEntityContent();
					break;
				}
				if (raiseEvent)
				{
					xmlDocument.onNodeInserted(newChild, newChild.ParentNode);
				}
			}
			return newChild;
		}

		private void CheckNodeInsertion(XmlNode newChild, XmlNode refChild)
		{
			XmlDocument xmlDocument = ((NodeType != XmlNodeType.Document) ? OwnerDocument : ((XmlDocument)this));
			if (NodeType != XmlNodeType.Element && NodeType != XmlNodeType.Attribute && NodeType != XmlNodeType.Document && NodeType != XmlNodeType.DocumentFragment)
			{
				throw new InvalidOperationException(string.Format("Node cannot be appended to current node {0}.", NodeType));
			}
			switch (NodeType)
			{
			case XmlNodeType.Attribute:
				switch (newChild.NodeType)
				{
				default:
					throw new InvalidOperationException(string.Format("Cannot insert specified type of node {0} as a child of this node {1}.", newChild.NodeType, NodeType));
				case XmlNodeType.Text:
				case XmlNodeType.EntityReference:
					break;
				}
				break;
			case XmlNodeType.Element:
				switch (newChild.NodeType)
				{
				case XmlNodeType.Attribute:
				case XmlNodeType.Entity:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentType:
				case XmlNodeType.Notation:
				case XmlNodeType.XmlDeclaration:
					throw new InvalidOperationException("Cannot insert specified type of node as a child of this node.");
				}
				break;
			}
			if (IsReadOnly)
			{
				throw new InvalidOperationException("The node is readonly.");
			}
			if (newChild.OwnerDocument != xmlDocument)
			{
				throw new ArgumentException("Can't append a node created by another document.");
			}
			if (refChild != null && refChild.ParentNode != this)
			{
				throw new ArgumentException("The reference node is not a child of this node.");
			}
			if (this == xmlDocument && xmlDocument.DocumentElement != null && newChild is XmlElement && newChild != xmlDocument.DocumentElement)
			{
				throw new XmlException("multiple document element not allowed.");
			}
			if (newChild == this || IsAncestor(newChild))
			{
				throw new ArgumentException("Cannot insert a node or any ancestor of that node as a child of itself.");
			}
		}

		public virtual void Normalize()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = ChildNodes.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				XmlNode xmlNode = ChildNodes[i];
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace || nodeType == XmlNodeType.Text)
				{
					stringBuilder.Append(xmlNode.Value);
					continue;
				}
				xmlNode.Normalize();
				NormalizeRange(num, i, stringBuilder);
				num = i + 1;
			}
			if (num < count)
			{
				NormalizeRange(num, count, stringBuilder);
			}
		}

		private void NormalizeRange(int start, int i, StringBuilder tmpBuilder)
		{
			int num = -1;
			for (int j = start; j < i; j++)
			{
				XmlNode xmlNode = ChildNodes[j];
				if (xmlNode.NodeType == XmlNodeType.Text)
				{
					num = j;
					break;
				}
				if (xmlNode.NodeType == XmlNodeType.SignificantWhitespace)
				{
					num = j;
				}
			}
			if (num >= 0)
			{
				for (int k = start; k < num; k++)
				{
					RemoveChild(ChildNodes[start]);
				}
				int num2 = i - num - 1;
				for (int l = 0; l < num2; l++)
				{
					RemoveChild(ChildNodes[start + 1]);
				}
			}
			if (num >= 0)
			{
				ChildNodes[start].Value = tmpBuilder.ToString();
			}
			tmpBuilder.Length = 0;
		}

		public virtual XmlNode PrependChild(XmlNode newChild)
		{
			return InsertAfter(newChild, null);
		}

		public virtual void RemoveAll()
		{
			if (Attributes != null)
			{
				Attributes.RemoveAll();
			}
			XmlNode xmlNode = null;
			for (XmlNode xmlNode2 = FirstChild; xmlNode2 != null; xmlNode2 = xmlNode)
			{
				xmlNode = xmlNode2.NextSibling;
				RemoveChild(xmlNode2);
			}
		}

		public virtual XmlNode RemoveChild(XmlNode oldChild)
		{
			return RemoveChild(oldChild, true);
		}

		private void CheckNodeRemoval()
		{
			if (NodeType != XmlNodeType.Attribute && NodeType != XmlNodeType.Element && NodeType != XmlNodeType.Document && NodeType != XmlNodeType.DocumentFragment)
			{
				throw new ArgumentException(string.Format("This {0} node cannot remove its child.", NodeType));
			}
			if (IsReadOnly)
			{
				throw new ArgumentException(string.Format("This {0} node is read only.", NodeType));
			}
		}

		internal XmlNode RemoveChild(XmlNode oldChild, bool checkNodeType)
		{
			if (oldChild == null)
			{
				throw new NullReferenceException();
			}
			XmlDocument xmlDocument = ((NodeType != XmlNodeType.Document) ? OwnerDocument : ((XmlDocument)this));
			if (oldChild.ParentNode != this)
			{
				throw new ArgumentException("The node to be removed is not a child of this node.");
			}
			if (checkNodeType)
			{
				xmlDocument.onNodeRemoving(oldChild, oldChild.ParentNode);
			}
			if (checkNodeType)
			{
				CheckNodeRemoval();
			}
			IHasXmlChildNode hasXmlChildNode = (IHasXmlChildNode)this;
			if (object.ReferenceEquals(hasXmlChildNode.LastLinkedChild, hasXmlChildNode.LastLinkedChild.NextLinkedSibling) && object.ReferenceEquals(hasXmlChildNode.LastLinkedChild, oldChild))
			{
				hasXmlChildNode.LastLinkedChild = null;
			}
			else
			{
				XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)oldChild;
				XmlLinkedNode xmlLinkedNode2 = hasXmlChildNode.LastLinkedChild;
				XmlLinkedNode xmlLinkedNode3 = (XmlLinkedNode)FirstChild;
				while (!object.ReferenceEquals(xmlLinkedNode2.NextLinkedSibling, hasXmlChildNode.LastLinkedChild) && !object.ReferenceEquals(xmlLinkedNode2.NextLinkedSibling, xmlLinkedNode))
				{
					xmlLinkedNode2 = xmlLinkedNode2.NextLinkedSibling;
				}
				if (!object.ReferenceEquals(xmlLinkedNode2.NextLinkedSibling, xmlLinkedNode))
				{
					throw new ArgumentException();
				}
				xmlLinkedNode2.NextLinkedSibling = xmlLinkedNode.NextLinkedSibling;
				if (xmlLinkedNode.NextLinkedSibling == xmlLinkedNode3)
				{
					hasXmlChildNode.LastLinkedChild = xmlLinkedNode2;
				}
				xmlLinkedNode.NextLinkedSibling = null;
			}
			if (checkNodeType)
			{
				xmlDocument.onNodeRemoved(oldChild, oldChild.ParentNode);
			}
			oldChild.parentNode = null;
			return oldChild;
		}

		public virtual XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
		{
			if (oldChild.ParentNode != this)
			{
				throw new ArgumentException("The node to be removed is not a child of this node.");
			}
			if (newChild == this || IsAncestor(newChild))
			{
				throw new InvalidOperationException("Cannot insert a node or any ancestor of that node as a child of itself.");
			}
			XmlNode nextSibling = oldChild.NextSibling;
			RemoveChild(oldChild);
			InsertBefore(newChild, nextSibling);
			return oldChild;
		}

		internal void SearchDescendantElements(string name, bool matchAll, ArrayList list)
		{
			for (XmlNode xmlNode = FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if (matchAll || xmlNode.Name == name)
					{
						list.Add(xmlNode);
					}
					xmlNode.SearchDescendantElements(name, matchAll, list);
				}
			}
		}

		internal void SearchDescendantElements(string name, bool matchAllName, string ns, bool matchAllNS, ArrayList list)
		{
			for (XmlNode xmlNode = FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if ((matchAllName || xmlNode.LocalName == name) && (matchAllNS || xmlNode.NamespaceURI == ns))
					{
						list.Add(xmlNode);
					}
					xmlNode.SearchDescendantElements(name, matchAllName, ns, matchAllNS, list);
				}
			}
		}

		public XmlNodeList SelectNodes(string xpath)
		{
			return SelectNodes(xpath, null);
		}

		public XmlNodeList SelectNodes(string xpath, XmlNamespaceManager nsmgr)
		{
			XPathNavigator xPathNavigator = CreateNavigator();
			XPathExpression xPathExpression = xPathNavigator.Compile(xpath);
			if (nsmgr != null)
			{
				xPathExpression.SetContext(nsmgr);
			}
			XPathNodeIterator iter = xPathNavigator.Select(xPathExpression);
			return new XmlIteratorNodeList(iter);
		}

		public XmlNode SelectSingleNode(string xpath)
		{
			return SelectSingleNode(xpath, null);
		}

		public XmlNode SelectSingleNode(string xpath, XmlNamespaceManager nsmgr)
		{
			XPathNavigator xPathNavigator = CreateNavigator();
			XPathExpression xPathExpression = xPathNavigator.Compile(xpath);
			if (nsmgr != null)
			{
				xPathExpression.SetContext(nsmgr);
			}
			XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(xPathExpression);
			if (!xPathNodeIterator.MoveNext())
			{
				return null;
			}
			return ((IHasXmlNode)xPathNodeIterator.Current).GetNode();
		}

		public virtual bool Supports(string feature, string version)
		{
			if (string.Compare(feature, "xml", true, CultureInfo.InvariantCulture) == 0 && (string.Compare(version, "1.0", true, CultureInfo.InvariantCulture) == 0 || string.Compare(version, "2.0", true, CultureInfo.InvariantCulture) == 0))
			{
				return true;
			}
			return false;
		}

		public abstract void WriteContentTo(XmlWriter w);

		public abstract void WriteTo(XmlWriter w);

		internal XmlNamespaceManager ConstructNamespaceManager()
		{
			XmlDocument xmlDocument = ((!(this is XmlDocument)) ? OwnerDocument : ((XmlDocument)this));
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
			XmlElement xmlElement = null;
			switch (NodeType)
			{
			case XmlNodeType.Attribute:
				xmlElement = ((XmlAttribute)this).OwnerElement;
				break;
			case XmlNodeType.Element:
				xmlElement = this as XmlElement;
				break;
			default:
				xmlElement = ParentNode as XmlElement;
				break;
			}
			while (xmlElement != null)
			{
				for (int i = 0; i < xmlElement.Attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = xmlElement.Attributes[i];
					if (xmlAttribute.Prefix == "xmlns")
					{
						if (xmlNamespaceManager.LookupNamespace(xmlAttribute.LocalName) != xmlAttribute.Value)
						{
							xmlNamespaceManager.AddNamespace(xmlAttribute.LocalName, xmlAttribute.Value);
						}
					}
					else if (xmlAttribute.Name == "xmlns" && xmlNamespaceManager.LookupNamespace(string.Empty) != xmlAttribute.Value)
					{
						xmlNamespaceManager.AddNamespace(string.Empty, xmlAttribute.Value);
					}
				}
				xmlElement = xmlElement.ParentNode as XmlElement;
			}
			return xmlNamespaceManager;
		}
	}
}
