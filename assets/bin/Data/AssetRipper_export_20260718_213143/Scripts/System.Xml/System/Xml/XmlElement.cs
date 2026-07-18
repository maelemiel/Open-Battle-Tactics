using System.Collections;
using System.Xml.Schema;
using System.Xml.XPath;
using Mono.Xml;

namespace System.Xml
{
	public class XmlElement : XmlLinkedNode, IHasXmlChildNode
	{
		private XmlAttributeCollection attributes;

		private XmlNameEntry name;

		private XmlLinkedNode lastLinkedChild;

		private bool isNotEmpty;

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

		public override XmlAttributeCollection Attributes
		{
			get
			{
				if (attributes == null)
				{
					attributes = new XmlAttributeCollection(this);
				}
				return attributes;
			}
		}

		public virtual bool HasAttributes
		{
			get
			{
				return attributes != null && attributes.Count > 0;
			}
		}

		public override string InnerText
		{
			get
			{
				return base.InnerText;
			}
			set
			{
				if (FirstChild != null && FirstChild.NextSibling == null && FirstChild.NodeType == XmlNodeType.Text)
				{
					FirstChild.Value = value;
					return;
				}
				while (FirstChild != null)
				{
					RemoveChild(FirstChild);
				}
				AppendChild(OwnerDocument.CreateTextNode(value), false);
			}
		}

		public override string InnerXml
		{
			get
			{
				return base.InnerXml;
			}
			set
			{
				while (FirstChild != null)
				{
					RemoveChild(FirstChild);
				}
				XmlNamespaceManager nsMgr = ConstructNamespaceManager();
				XmlParserContext context = new XmlParserContext(OwnerDocument.NameTable, nsMgr, (OwnerDocument.DocumentType == null) ? null : OwnerDocument.DocumentType.DTD, BaseURI, XmlLang, XmlSpace, null);
				XmlTextReader xmlTextReader = new XmlTextReader(value, XmlNodeType.Element, context);
				xmlTextReader.XmlResolver = OwnerDocument.Resolver;
				while (true)
				{
					XmlNode xmlNode = OwnerDocument.ReadNode(xmlTextReader);
					if (xmlNode == null)
					{
						break;
					}
					AppendChild(xmlNode);
				}
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !isNotEmpty && FirstChild == null;
			}
			set
			{
				isNotEmpty = !value;
				if (value)
				{
					while (FirstChild != null)
					{
						RemoveChild(FirstChild);
					}
				}
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

		public override XmlNode NextSibling
		{
			get
			{
				return (ParentNode != null && ((IHasXmlChildNode)ParentNode).LastLinkedChild != this) ? base.NextLinkedSibling : null;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Element;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Element;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return base.OwnerDocument;
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
					throw new ArgumentException("This node is readonly.");
				}
				if (value == null)
				{
					value = string.Empty;
				}
				if (!string.Empty.Equals(value) && !XmlChar.IsNCName(value))
				{
					throw new ArgumentException("Specified name is not a valid NCName: " + value);
				}
				value = OwnerDocument.NameTable.Add(value);
				name = OwnerDocument.NameCache.Add(value, name.LocalName, name.NS, true);
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return base.ParentNode;
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

		protected internal XmlElement(string prefix, string localName, string namespaceURI, XmlDocument doc)
			: this(prefix, localName, namespaceURI, doc, false)
		{
		}

		internal XmlElement(string prefix, string localName, string namespaceURI, XmlDocument doc, bool atomizedNames)
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
				XmlConvert.VerifyName(localName);
				prefix = doc.NameTable.Add(prefix);
				localName = doc.NameTable.Add(localName);
				namespaceURI = doc.NameTable.Add(namespaceURI);
			}
			name = doc.NameCache.Add(prefix, localName, namespaceURI, true);
			if (doc.DocumentType == null)
			{
				return;
			}
			DTDAttListDeclaration dTDAttListDeclaration = doc.DocumentType.DTD.AttListDecls[localName];
			if (dTDAttListDeclaration == null)
			{
				return;
			}
			for (int i = 0; i < dTDAttListDeclaration.Definitions.Count; i++)
			{
				DTDAttributeDefinition dTDAttributeDefinition = dTDAttListDeclaration[i];
				if (dTDAttributeDefinition.DefaultValue != null)
				{
					SetAttribute(dTDAttributeDefinition.Name, dTDAttributeDefinition.DefaultValue);
					Attributes[dTDAttributeDefinition.Name].SetDefault();
				}
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlElement xmlElement = OwnerDocument.CreateElement(name.Prefix, name.LocalName, name.NS, true);
			for (int i = 0; i < Attributes.Count; i++)
			{
				xmlElement.SetAttributeNode((XmlAttribute)Attributes[i].CloneNode(true));
			}
			if (deep)
			{
				for (int j = 0; j < ChildNodes.Count; j++)
				{
					xmlElement.AppendChild(ChildNodes[j].CloneNode(true), false);
				}
			}
			return xmlElement;
		}

		public virtual string GetAttribute(string name)
		{
			XmlNode namedItem = Attributes.GetNamedItem(name);
			return (namedItem == null) ? string.Empty : namedItem.Value;
		}

		public virtual string GetAttribute(string localName, string namespaceURI)
		{
			XmlNode namedItem = Attributes.GetNamedItem(localName, namespaceURI);
			return (namedItem == null) ? string.Empty : namedItem.Value;
		}

		public virtual XmlAttribute GetAttributeNode(string name)
		{
			XmlNode namedItem = Attributes.GetNamedItem(name);
			return (namedItem == null) ? null : (namedItem as XmlAttribute);
		}

		public virtual XmlAttribute GetAttributeNode(string localName, string namespaceURI)
		{
			XmlNode namedItem = Attributes.GetNamedItem(localName, namespaceURI);
			return (namedItem == null) ? null : (namedItem as XmlAttribute);
		}

		public virtual XmlNodeList GetElementsByTagName(string name)
		{
			ArrayList arrayList = new ArrayList();
			SearchDescendantElements(name, name == "*", arrayList);
			return new XmlNodeArrayList(arrayList);
		}

		public virtual XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
		{
			ArrayList arrayList = new ArrayList();
			SearchDescendantElements(localName, localName == "*", namespaceURI, namespaceURI == "*", arrayList);
			return new XmlNodeArrayList(arrayList);
		}

		public virtual bool HasAttribute(string name)
		{
			XmlNode namedItem = Attributes.GetNamedItem(name);
			return namedItem != null;
		}

		public virtual bool HasAttribute(string localName, string namespaceURI)
		{
			XmlNode namedItem = Attributes.GetNamedItem(localName, namespaceURI);
			return namedItem != null;
		}

		public override void RemoveAll()
		{
			base.RemoveAll();
		}

		public virtual void RemoveAllAttributes()
		{
			if (attributes != null)
			{
				attributes.RemoveAll();
			}
		}

		public virtual void RemoveAttribute(string name)
		{
			if (attributes != null)
			{
				XmlAttribute xmlAttribute = Attributes.GetNamedItem(name) as XmlAttribute;
				if (xmlAttribute != null)
				{
					Attributes.Remove(xmlAttribute);
				}
			}
		}

		public virtual void RemoveAttribute(string localName, string namespaceURI)
		{
			if (attributes != null)
			{
				XmlAttribute xmlAttribute = attributes.GetNamedItem(localName, namespaceURI) as XmlAttribute;
				if (xmlAttribute != null)
				{
					Attributes.Remove(xmlAttribute);
				}
			}
		}

		public virtual XmlNode RemoveAttributeAt(int i)
		{
			if (attributes == null || attributes.Count <= i)
			{
				return null;
			}
			return Attributes.RemoveAt(i);
		}

		public virtual XmlAttribute RemoveAttributeNode(XmlAttribute oldAttr)
		{
			if (attributes == null)
			{
				return null;
			}
			return Attributes.Remove(oldAttr);
		}

		public virtual XmlAttribute RemoveAttributeNode(string localName, string namespaceURI)
		{
			if (attributes == null)
			{
				return null;
			}
			return Attributes.Remove(attributes[localName, namespaceURI]);
		}

		public virtual void SetAttribute(string name, string value)
		{
			XmlAttribute xmlAttribute = Attributes[name];
			if (xmlAttribute == null)
			{
				xmlAttribute = OwnerDocument.CreateAttribute(name);
				xmlAttribute.Value = value;
				Attributes.SetNamedItem(xmlAttribute);
			}
			else
			{
				xmlAttribute.Value = value;
			}
		}

		public virtual string SetAttribute(string localName, string namespaceURI, string value)
		{
			XmlAttribute xmlAttribute = Attributes[localName, namespaceURI];
			if (xmlAttribute == null)
			{
				xmlAttribute = OwnerDocument.CreateAttribute(localName, namespaceURI);
				xmlAttribute.Value = value;
				Attributes.SetNamedItem(xmlAttribute);
			}
			else
			{
				xmlAttribute.Value = value;
			}
			return xmlAttribute.Value;
		}

		public virtual XmlAttribute SetAttributeNode(XmlAttribute newAttr)
		{
			if (newAttr.OwnerElement != null)
			{
				throw new InvalidOperationException("Specified attribute is already an attribute of another element.");
			}
			XmlAttribute xmlAttribute = Attributes.SetNamedItem(newAttr) as XmlAttribute;
			return (xmlAttribute != newAttr) ? xmlAttribute : null;
		}

		public virtual XmlAttribute SetAttributeNode(string localName, string namespaceURI)
		{
			XmlConvert.VerifyNCName(localName);
			return Attributes.Append(OwnerDocument.CreateAttribute(string.Empty, localName, namespaceURI, false, true));
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (XmlNode firstChild = FirstChild; firstChild != null; firstChild = firstChild.NextSibling)
			{
				firstChild.WriteTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteStartElement((name.NS != null && name.NS.Length != 0) ? name.Prefix : string.Empty, name.LocalName, name.NS);
			if (HasAttributes)
			{
				for (int i = 0; i < Attributes.Count; i++)
				{
					Attributes[i].WriteTo(w);
				}
			}
			WriteContentTo(w);
			if (IsEmpty)
			{
				w.WriteEndElement();
			}
			else
			{
				w.WriteFullEndElement();
			}
		}
	}
}
