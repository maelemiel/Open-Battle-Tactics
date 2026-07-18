using System.Xml.XPath;

namespace System.Xml
{
	public class XmlEntityReference : XmlLinkedNode, IHasXmlChildNode
	{
		private string entityName;

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

		public override string BaseURI
		{
			get
			{
				return base.BaseURI;
			}
		}

		private XmlEntity Entity
		{
			get
			{
				XmlDocumentType documentType = OwnerDocument.DocumentType;
				if (documentType == null)
				{
					return null;
				}
				if (documentType.Entities == null)
				{
					return null;
				}
				return documentType.Entities.GetNamedItem(Name) as XmlEntity;
			}
		}

		internal override string ChildrenBaseURI
		{
			get
			{
				XmlEntity entity = Entity;
				if (entity == null)
				{
					return string.Empty;
				}
				if (entity.SystemId == null || entity.SystemId.Length == 0)
				{
					return entity.BaseURI;
				}
				if (entity.BaseURI == null || entity.BaseURI.Length == 0)
				{
					return entity.SystemId;
				}
				Uri baseUri = null;
				try
				{
					baseUri = new Uri(entity.BaseURI);
				}
				catch (UriFormatException)
				{
				}
				XmlResolver resolver = OwnerDocument.Resolver;
				if (resolver != null)
				{
					return resolver.ResolveUri(baseUri, entity.SystemId).ToString();
				}
				return new Uri(baseUri, entity.SystemId).ToString();
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
				return entityName;
			}
		}

		public override string Name
		{
			get
			{
				return entityName;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.EntityReference;
			}
		}

		public override string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new XmlException("entity reference cannot be set value.");
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Text;
			}
		}

		protected internal XmlEntityReference(string name, XmlDocument doc)
			: base(doc)
		{
			XmlConvert.VerifyName(name);
			entityName = doc.NameTable.Add(name);
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlEntityReference(Name, OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (int i = 0; i < ChildNodes.Count; i++)
			{
				ChildNodes[i].WriteTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteRaw("&");
			w.WriteName(Name);
			w.WriteRaw(";");
		}

		internal void SetReferencedEntityContent()
		{
			if (FirstChild != null || OwnerDocument.DocumentType == null)
			{
				return;
			}
			XmlEntity entity = Entity;
			if (entity == null)
			{
				InsertBefore(OwnerDocument.CreateTextNode(string.Empty), null, false, true);
				return;
			}
			for (int i = 0; i < entity.ChildNodes.Count; i++)
			{
				InsertBefore(entity.ChildNodes[i].CloneNode(true), null, false, true);
			}
		}
	}
}
