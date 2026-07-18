using Mono.Xml;

namespace System.Xml
{
	public class XmlEntity : XmlNode, IHasXmlChildNode
	{
		private string name;

		private string NDATA;

		private string publicId;

		private string systemId;

		private string baseUri;

		private XmlLinkedNode lastLinkedChild;

		private bool contentAlreadySet;

		XmlLinkedNode IHasXmlChildNode.LastLinkedChild
		{
			get
			{
				if (lastLinkedChild != null)
				{
					return lastLinkedChild;
				}
				if (!contentAlreadySet)
				{
					contentAlreadySet = true;
					SetEntityContent();
				}
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
				return baseUri;
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
				throw new InvalidOperationException("This operation is not supported.");
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
				throw new InvalidOperationException("This operation is not supported.");
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public override string LocalName
		{
			get
			{
				return name;
			}
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Entity;
			}
		}

		public string NotationName
		{
			get
			{
				if (NDATA == null)
				{
					return null;
				}
				return NDATA;
			}
		}

		public override string OuterXml
		{
			get
			{
				return string.Empty;
			}
		}

		public string PublicId
		{
			get
			{
				return publicId;
			}
		}

		public string SystemId
		{
			get
			{
				return systemId;
			}
		}

		internal XmlEntity(string name, string NDATA, string publicId, string systemId, XmlDocument doc)
			: base(doc)
		{
			this.name = doc.NameTable.Add(name);
			this.NDATA = NDATA;
			this.publicId = publicId;
			this.systemId = systemId;
			baseUri = doc.BaseURI;
		}

		public override XmlNode CloneNode(bool deep)
		{
			throw new InvalidOperationException("This operation is not supported.");
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
		}

		private void SetEntityContent()
		{
			if (lastLinkedChild != null)
			{
				return;
			}
			XmlDocumentType documentType = OwnerDocument.DocumentType;
			if (documentType == null)
			{
				return;
			}
			DTDEntityDeclaration dTDEntityDeclaration = documentType.DTD.EntityDecls[name];
			if (dTDEntityDeclaration == null)
			{
				return;
			}
			XmlNamespaceManager nsMgr = ConstructNamespaceManager();
			XmlParserContext context = new XmlParserContext(OwnerDocument.NameTable, nsMgr, (documentType == null) ? null : documentType.DTD, BaseURI, XmlLang, XmlSpace, null);
			XmlTextReader xmlTextReader = new XmlTextReader(dTDEntityDeclaration.EntityValue, XmlNodeType.Element, context);
			xmlTextReader.XmlResolver = OwnerDocument.Resolver;
			while (true)
			{
				XmlNode xmlNode = OwnerDocument.ReadNode(xmlTextReader);
				if (xmlNode == null)
				{
					break;
				}
				InsertBefore(xmlNode, null, false, false);
			}
		}
	}
}
