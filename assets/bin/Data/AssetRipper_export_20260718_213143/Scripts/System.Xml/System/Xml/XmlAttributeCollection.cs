using System.Collections;
using System.Runtime.CompilerServices;
using Mono.Xml;

namespace System.Xml
{
	public sealed class XmlAttributeCollection : XmlNamedNodeMap, IEnumerable, ICollection
	{
		private XmlElement ownerElement;

		private XmlDocument ownerDocument;

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		private bool IsReadOnly
		{
			get
			{
				return ownerElement.IsReadOnly;
			}
		}

		[IndexerName("ItemOf")]
		public XmlAttribute this[string name]
		{
			get
			{
				return (XmlAttribute)GetNamedItem(name);
			}
		}

		[IndexerName("ItemOf")]
		public XmlAttribute this[int i]
		{
			get
			{
				return (XmlAttribute)base.Nodes[i];
			}
		}

		[IndexerName("ItemOf")]
		public XmlAttribute this[string localName, string namespaceURI]
		{
			get
			{
				return (XmlAttribute)GetNamedItem(localName, namespaceURI);
			}
		}

		internal XmlAttributeCollection(XmlNode parent)
			: base(parent)
		{
			ownerElement = parent as XmlElement;
			ownerDocument = parent.OwnerDocument;
			if (ownerElement == null)
			{
				throw new XmlException("invalid construction for XmlAttributeCollection.");
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			array.CopyTo(base.Nodes.ToArray(typeof(XmlAttribute)), index);
		}

		public XmlAttribute Append(XmlAttribute node)
		{
			SetNamedItem(node);
			return node;
		}

		public void CopyTo(XmlAttribute[] array, int index)
		{
			for (int i = 0; i < Count; i++)
			{
				array[index + i] = base.Nodes[i] as XmlAttribute;
			}
		}

		public XmlAttribute InsertAfter(XmlAttribute newNode, XmlAttribute refNode)
		{
			if (refNode == null)
			{
				if (Count == 0)
				{
					return InsertBefore(newNode, null);
				}
				return InsertBefore(newNode, this[0]);
			}
			for (int i = 0; i < Count; i++)
			{
				if (refNode == base.Nodes[i])
				{
					return InsertBefore(newNode, (Count != i + 1) ? this[i + 1] : null);
				}
			}
			throw new ArgumentException("refNode not found in this collection.");
		}

		public XmlAttribute InsertBefore(XmlAttribute newNode, XmlAttribute refNode)
		{
			if (newNode.OwnerDocument != ownerDocument)
			{
				throw new ArgumentException("different document created this newNode.");
			}
			ownerDocument.onNodeInserting(newNode, null);
			int num = Count;
			if (refNode != null)
			{
				for (int i = 0; i < Count; i++)
				{
					XmlNode xmlNode = base.Nodes[i] as XmlNode;
					if (xmlNode == refNode)
					{
						num = i;
						break;
					}
				}
				if (num == Count)
				{
					throw new ArgumentException("refNode not found in this collection.");
				}
			}
			SetNamedItem(newNode, num, false);
			ownerDocument.onNodeInserted(newNode, null);
			return newNode;
		}

		public XmlAttribute Prepend(XmlAttribute node)
		{
			return InsertAfter(node, null);
		}

		public XmlAttribute Remove(XmlAttribute node)
		{
			if (IsReadOnly)
			{
				throw new ArgumentException("This attribute collection is read-only.");
			}
			if (node == null)
			{
				throw new ArgumentException("Specified node is null.");
			}
			if (node.OwnerDocument != ownerDocument)
			{
				throw new ArgumentException("Specified node is in a different document.");
			}
			if (node.OwnerElement != ownerElement)
			{
				throw new ArgumentException("The specified attribute is not contained in the element.");
			}
			XmlAttribute xmlAttribute = null;
			for (int i = 0; i < Count; i++)
			{
				XmlAttribute xmlAttribute2 = (XmlAttribute)base.Nodes[i];
				if (xmlAttribute2 == node)
				{
					xmlAttribute = xmlAttribute2;
					break;
				}
			}
			if (xmlAttribute != null)
			{
				ownerDocument.onNodeRemoving(node, ownerElement);
				base.RemoveNamedItem(xmlAttribute.LocalName, xmlAttribute.NamespaceURI);
				RemoveIdenticalAttribute(xmlAttribute);
				ownerDocument.onNodeRemoved(node, ownerElement);
			}
			DTDAttributeDefinition attributeDefinition = xmlAttribute.GetAttributeDefinition();
			if (attributeDefinition != null && attributeDefinition.DefaultValue != null)
			{
				XmlAttribute xmlAttribute3 = ownerDocument.CreateAttribute(xmlAttribute.Prefix, xmlAttribute.LocalName, xmlAttribute.NamespaceURI, true, false);
				xmlAttribute3.Value = attributeDefinition.DefaultValue;
				xmlAttribute3.SetDefault();
				SetNamedItem(xmlAttribute3);
			}
			xmlAttribute.AttributeOwnerElement = null;
			return xmlAttribute;
		}

		public void RemoveAll()
		{
			int num = 0;
			while (num < Count)
			{
				XmlAttribute xmlAttribute = this[num];
				if (!xmlAttribute.Specified)
				{
					num++;
				}
				Remove(xmlAttribute);
			}
		}

		public XmlAttribute RemoveAt(int i)
		{
			if (Count <= i)
			{
				return null;
			}
			return Remove((XmlAttribute)base.Nodes[i]);
		}

		public override XmlNode SetNamedItem(XmlNode node)
		{
			if (IsReadOnly)
			{
				throw new ArgumentException("this AttributeCollection is read only.");
			}
			XmlAttribute xmlAttribute = node as XmlAttribute;
			if (xmlAttribute.OwnerElement == ownerElement)
			{
				return node;
			}
			if (xmlAttribute.OwnerElement != null)
			{
				throw new ArgumentException("This attribute is already set to another element.");
			}
			ownerElement.OwnerDocument.onNodeInserting(node, ownerElement);
			xmlAttribute.AttributeOwnerElement = ownerElement;
			XmlNode xmlNode = SetNamedItem(node, -1, false);
			AdjustIdenticalAttributes(node as XmlAttribute, (xmlNode != node) ? xmlNode : null);
			ownerElement.OwnerDocument.onNodeInserted(node, ownerElement);
			return xmlNode as XmlAttribute;
		}

		internal void AddIdenticalAttribute()
		{
			SetIdenticalAttribute(false);
		}

		internal void RemoveIdenticalAttribute()
		{
			SetIdenticalAttribute(true);
		}

		private void SetIdenticalAttribute(bool remove)
		{
			if (ownerElement == null)
			{
				return;
			}
			XmlDocumentType documentType = ownerDocument.DocumentType;
			if (documentType == null || documentType.DTD == null)
			{
				return;
			}
			DTDElementDeclaration dTDElementDeclaration = documentType.DTD.ElementDecls[ownerElement.Name];
			for (int i = 0; i < Count; i++)
			{
				XmlAttribute xmlAttribute = (XmlAttribute)base.Nodes[i];
				DTDAttributeDefinition dTDAttributeDefinition = ((dTDElementDeclaration != null) ? dTDElementDeclaration.Attributes[xmlAttribute.Name] : null);
				if (dTDAttributeDefinition == null || dTDAttributeDefinition.Datatype.TokenizedType != XmlTokenizedType.ID)
				{
					continue;
				}
				if (remove)
				{
					if (ownerDocument.GetIdenticalAttribute(xmlAttribute.Value) != null)
					{
						ownerDocument.RemoveIdenticalAttribute(xmlAttribute.Value);
						break;
					}
					continue;
				}
				if (ownerDocument.GetIdenticalAttribute(xmlAttribute.Value) != null)
				{
					throw new XmlException(string.Format("ID value {0} already exists in this document.", xmlAttribute.Value));
				}
				ownerDocument.AddIdenticalAttribute(xmlAttribute);
				break;
			}
		}

		private void AdjustIdenticalAttributes(XmlAttribute node, XmlNode existing)
		{
			if (ownerElement == null)
			{
				return;
			}
			if (existing != null)
			{
				RemoveIdenticalAttribute(existing);
			}
			XmlDocumentType documentType = node.OwnerDocument.DocumentType;
			if (documentType != null && documentType.DTD != null)
			{
				DTDAttListDeclaration dTDAttListDeclaration = documentType.DTD.AttListDecls[ownerElement.Name];
				DTDAttributeDefinition dTDAttributeDefinition = ((dTDAttListDeclaration != null) ? dTDAttListDeclaration.Get(node.Name) : null);
				if (dTDAttributeDefinition != null && dTDAttributeDefinition.Datatype.TokenizedType == XmlTokenizedType.ID)
				{
					ownerDocument.AddIdenticalAttribute(node);
				}
			}
		}

		private XmlNode RemoveIdenticalAttribute(XmlNode existing)
		{
			if (ownerElement == null)
			{
				return existing;
			}
			if (existing != null && ownerDocument.GetIdenticalAttribute(existing.Value) != null)
			{
				ownerDocument.RemoveIdenticalAttribute(existing.Value);
			}
			return existing;
		}
	}
}
