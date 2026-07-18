using System.Text;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlDocumentFragment : XmlNode, IHasXmlChildNode
	{
		private XmlLinkedNode lastLinkedChild;

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

		public override string InnerXml
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < ChildNodes.Count; i++)
				{
					stringBuilder.Append(ChildNodes[i].OuterXml);
				}
				return stringBuilder.ToString();
			}
			set
			{
				for (int i = 0; i < ChildNodes.Count; i++)
				{
					RemoveChild(ChildNodes[i]);
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

		public override string LocalName
		{
			get
			{
				return "#document-fragment";
			}
		}

		public override string Name
		{
			get
			{
				return "#document-fragment";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.DocumentFragment;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return base.OwnerDocument;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		protected internal XmlDocumentFragment(XmlDocument doc)
			: base(doc)
		{
		}

		public override XmlNode CloneNode(bool deep)
		{
			if (deep)
			{
				XmlNode xmlNode = FirstChild;
				while (xmlNode != null && xmlNode.HasChildNodes)
				{
					AppendChild(xmlNode.NextSibling.CloneNode(false));
					xmlNode = xmlNode.NextSibling;
				}
				return xmlNode;
			}
			return new XmlDocumentFragment(OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (int i = 0; i < ChildNodes.Count; i++)
			{
				ChildNodes[i].WriteContentTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			for (int i = 0; i < ChildNodes.Count; i++)
			{
				ChildNodes[i].WriteTo(w);
			}
		}
	}
}
