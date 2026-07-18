using System.Xml.Schema;
using System.Xml.XPath;
using Mono.Xml;

namespace System.Xml
{
	public class XmlAttribute : XmlNode, IHasXmlChildNode
	{
		private XmlNameEntry name;

		internal bool isDefault;

		private XmlLinkedNode lastLinkedChild;

		private IXmlSchemaInfo schemaInfo;

		XmlLinkedNode IHasXmlChildNode.LastLinkedChild
		{
			get
			{
				return lastLinkedChild;
			}
			set
			{
				lastLinkedChild = value;
			}
		}

		public override string BaseURI
		{
			get
			{
				return (OwnerElement == null) ? string.Empty : OwnerElement.BaseURI;
			}
		}

		public override string InnerText
		{
			set
			{
				Value = value;
			}
		}

		public override string InnerXml
		{
			set
			{
				RemoveAll();
				XmlNamespaceManager nsMgr = ConstructNamespaceManager();
				XmlParserContext context = new XmlParserContext(OwnerDocument.NameTable, nsMgr, (OwnerDocument.DocumentType == null) ? null : OwnerDocument.DocumentType.DTD, BaseURI, XmlLang, XmlSpace, null);
				XmlTextReader xmlTextReader = new XmlTextReader(value, XmlNodeType.Attribute, context);
				xmlTextReader.XmlResolver = OwnerDocument.Resolver;
				xmlTextReader.Read();
				OwnerDocument.ReadAttributeNodeValue(xmlTextReader, this);
			}
		}

		public override string LocalName
		{
			get
			{
				return name.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return name.GetPrefixedName(OwnerDocument.NameCache);
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return name.NS;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Attribute;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Attribute;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return base.OwnerDocument;
			}
		}

		public virtual XmlElement OwnerElement
		{
			get
			{
				return base.AttributeOwnerElement;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		public override string Prefix
		{
			get
			{
				return name.Prefix;
			}
			set
			{
				if (IsReadOnly)
				{
					throw new XmlException("This node is readonly.");
				}
				if (name.Prefix == "xmlns" && value != "xmlns")
				{
					throw new ArgumentException("Cannot bind to the reserved namespace.");
				}
				value = OwnerDocument.NameTable.Add(value);
				name = OwnerDocument.NameCache.Add(value, name.LocalName, name.NS, true);
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return schemaInfo;
			}
			internal set
			{
				schemaInfo = value;
			}
		}

		public virtual bool Specified
		{
			get
			{
				return !isDefault;
			}
		}

		public override string Value
		{
			get
			{
				return InnerText;
			}
			set
			{
				if (IsReadOnly)
				{
					throw new ArgumentException("Attempt to modify a read-only node.");
				}
				OwnerDocument.CheckIdTableUpdate(this, InnerText, value);
				XmlNode xmlNode = FirstChild as XmlCharacterData;
				if (xmlNode == null)
				{
					RemoveAll();
					AppendChild(OwnerDocument.CreateTextNode(value), false);
				}
				else if (FirstChild.NextSibling != null)
				{
					RemoveAll();
					AppendChild(OwnerDocument.CreateTextNode(value), false);
				}
				else
				{
					xmlNode.Value = value;
				}
				isDefault = false;
			}
		}

		internal override string XmlLang
		{
			get
			{
				return (OwnerElement == null) ? string.Empty : OwnerElement.XmlLang;
			}
		}

		internal override XmlSpace XmlSpace
		{
			get
			{
				return (OwnerElement != null) ? OwnerElement.XmlSpace : XmlSpace.None;
			}
		}

		protected internal XmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc)
			: this(prefix, localName, namespaceURI, doc, false, true)
		{
		}

		internal XmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc, bool atomizedNames, bool checkNamespace)
			: base(doc)
		{
			if (!atomizedNames)
			{
				if (prefix == null)
				{
					prefix = string.Empty;
				}
				if (namespaceURI == null)
				{
					namespaceURI = string.Empty;
				}
			}
			if (checkNamespace && (prefix == "xmlns" || (prefix == string.Empty && localName == "xmlns")))
			{
				if (namespaceURI != "http://www.w3.org/2000/xmlns/")
				{
					throw new ArgumentException("Invalid attribute namespace for namespace declaration.");
				}
				if (prefix == "xml" && namespaceURI != "http://www.w3.org/XML/1998/namespace")
				{
					throw new ArgumentException("Invalid attribute namespace for namespace declaration.");
				}
			}
			if (!atomizedNames)
			{
				if (prefix != string.Empty && !XmlChar.IsName(prefix))
				{
					throw new ArgumentException("Invalid attribute prefix.");
				}
				if (!XmlChar.IsName(localName))
				{
					throw new ArgumentException("Invalid attribute local name.");
				}
				prefix = doc.NameTable.Add(prefix);
				localName = doc.NameTable.Add(localName);
				namespaceURI = doc.NameTable.Add(namespaceURI);
			}
			name = doc.NameCache.Add(prefix, localName, namespaceURI, true);
		}

		public override XmlNode AppendChild(XmlNode child)
		{
			return base.AppendChild(child);
		}

		public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
		{
			return base.InsertBefore(newChild, refChild);
		}

		public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
		{
			return base.InsertAfter(newChild, refChild);
		}

		public override XmlNode PrependChild(XmlNode node)
		{
			return base.PrependChild(node);
		}

		public override XmlNode RemoveChild(XmlNode node)
		{
			return base.RemoveChild(node);
		}

		public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
		{
			return base.ReplaceChild(newChild, oldChild);
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlNode xmlNode = OwnerDocument.CreateAttribute(name.Prefix, name.LocalName, name.NS, true, false);
			if (deep)
			{
				for (XmlNode xmlNode2 = FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
				{
					xmlNode.AppendChild(xmlNode2.CloneNode(deep), false);
				}
			}
			return xmlNode;
		}

		internal void SetDefault()
		{
			isDefault = true;
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (XmlNode xmlNode = FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				xmlNode.WriteTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			if (!isDefault)
			{
				w.WriteStartAttribute((name.NS.Length <= 0) ? string.Empty : name.Prefix, name.LocalName, name.NS);
				WriteContentTo(w);
				w.WriteEndAttribute();
			}
		}

		internal DTDAttributeDefinition GetAttributeDefinition()
		{
			if (OwnerElement == null)
			{
				return null;
			}
			DTDAttListDeclaration dTDAttListDeclaration = ((OwnerDocument.DocumentType == null) ? null : OwnerDocument.DocumentType.DTD.AttListDecls[OwnerElement.Name]);
			return (dTDAttListDeclaration == null) ? null : dTDAttListDeclaration[Name];
		}
	}
}
