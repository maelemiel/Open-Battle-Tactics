using System.Collections;
using Mono.Xml;

namespace System.Xml
{
	public class XmlNamedNodeMap : IEnumerable
	{
		private static readonly IEnumerator emptyEnumerator = new XmlNode[0].GetEnumerator();

		private XmlNode parent;

		private ArrayList nodeList;

		private bool readOnly;

		private ArrayList NodeList
		{
			get
			{
				if (nodeList == null)
				{
					nodeList = new ArrayList();
				}
				return nodeList;
			}
		}

		public virtual int Count
		{
			get
			{
				return (nodeList != null) ? nodeList.Count : 0;
			}
		}

		internal ArrayList Nodes
		{
			get
			{
				return NodeList;
			}
		}

		internal XmlNamedNodeMap(XmlNode parent)
		{
			this.parent = parent;
		}

		public virtual IEnumerator GetEnumerator()
		{
			if (nodeList == null)
			{
				return emptyEnumerator;
			}
			return nodeList.GetEnumerator();
		}

		public virtual XmlNode GetNamedItem(string name)
		{
			if (nodeList == null)
			{
				return null;
			}
			for (int i = 0; i < nodeList.Count; i++)
			{
				XmlNode xmlNode = (XmlNode)nodeList[i];
				if (xmlNode.Name == name)
				{
					return xmlNode;
				}
			}
			return null;
		}

		public virtual XmlNode GetNamedItem(string localName, string namespaceURI)
		{
			if (nodeList == null)
			{
				return null;
			}
			for (int i = 0; i < nodeList.Count; i++)
			{
				XmlNode xmlNode = (XmlNode)nodeList[i];
				if (xmlNode.LocalName == localName && xmlNode.NamespaceURI == namespaceURI)
				{
					return xmlNode;
				}
			}
			return null;
		}

		public virtual XmlNode Item(int index)
		{
			if (nodeList == null || index < 0 || index >= nodeList.Count)
			{
				return null;
			}
			return (XmlNode)nodeList[index];
		}

		public virtual XmlNode RemoveNamedItem(string name)
		{
			if (nodeList == null)
			{
				return null;
			}
			for (int i = 0; i < nodeList.Count; i++)
			{
				XmlNode xmlNode = (XmlNode)nodeList[i];
				if (!(xmlNode.Name == name))
				{
					continue;
				}
				if (xmlNode.IsReadOnly)
				{
					throw new InvalidOperationException("Cannot remove. This node is read only: " + name);
				}
				nodeList.Remove(xmlNode);
				XmlAttribute xmlAttribute = xmlNode as XmlAttribute;
				if (xmlAttribute != null)
				{
					DTDAttributeDefinition attributeDefinition = xmlAttribute.GetAttributeDefinition();
					if (attributeDefinition != null && attributeDefinition.DefaultValue != null)
					{
						XmlAttribute xmlAttribute2 = xmlAttribute.OwnerDocument.CreateAttribute(xmlAttribute.Prefix, xmlAttribute.LocalName, xmlAttribute.NamespaceURI, true, false);
						xmlAttribute2.Value = attributeDefinition.DefaultValue;
						xmlAttribute2.SetDefault();
						xmlAttribute.OwnerElement.SetAttributeNode(xmlAttribute2);
					}
				}
				return xmlNode;
			}
			return null;
		}

		public virtual XmlNode RemoveNamedItem(string localName, string namespaceURI)
		{
			if (nodeList == null)
			{
				return null;
			}
			for (int i = 0; i < nodeList.Count; i++)
			{
				XmlNode xmlNode = (XmlNode)nodeList[i];
				if (xmlNode.LocalName == localName && xmlNode.NamespaceURI == namespaceURI)
				{
					nodeList.Remove(xmlNode);
					return xmlNode;
				}
			}
			return null;
		}

		public virtual XmlNode SetNamedItem(XmlNode node)
		{
			return SetNamedItem(node, -1, true);
		}

		internal XmlNode SetNamedItem(XmlNode node, bool raiseEvent)
		{
			return SetNamedItem(node, -1, raiseEvent);
		}

		internal XmlNode SetNamedItem(XmlNode node, int pos, bool raiseEvent)
		{
			if (readOnly || node.OwnerDocument != parent.OwnerDocument)
			{
				throw new ArgumentException("Cannot add to NodeMap.");
			}
			if (raiseEvent)
			{
				parent.OwnerDocument.onNodeInserting(node, parent);
			}
			try
			{
				for (int i = 0; i < NodeList.Count; i++)
				{
					XmlNode xmlNode = (XmlNode)nodeList[i];
					if (xmlNode.LocalName == node.LocalName && xmlNode.NamespaceURI == node.NamespaceURI)
					{
						nodeList.Remove(xmlNode);
						if (pos < 0)
						{
							nodeList.Add(node);
						}
						else
						{
							nodeList.Insert(pos, node);
						}
						return xmlNode;
					}
				}
				if (pos < 0)
				{
					nodeList.Add(node);
				}
				else
				{
					nodeList.Insert(pos, node);
				}
				return node;
			}
			finally
			{
				if (raiseEvent)
				{
					parent.OwnerDocument.onNodeInserted(node, parent);
				}
			}
		}
	}
}
