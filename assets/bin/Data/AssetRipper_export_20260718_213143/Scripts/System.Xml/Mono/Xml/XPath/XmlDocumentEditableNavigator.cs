using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class XmlDocumentEditableNavigator : XPathNavigator, IHasXmlNode
	{
		private static readonly bool isXmlDocumentNavigatorImpl;

		private XPathEditableDocument document;

		private XPathNavigator navigator;

		public override string BaseURI
		{
			get
			{
				return navigator.BaseURI;
			}
		}

		public override bool CanEdit
		{
			get
			{
				return true;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return navigator.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return navigator.LocalName;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return navigator.NameTable;
			}
		}

		public override string Name
		{
			get
			{
				return navigator.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return navigator.NamespaceURI;
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				return navigator.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return navigator.Prefix;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return navigator.SchemaInfo;
			}
		}

		public override object UnderlyingObject
		{
			get
			{
				return navigator.UnderlyingObject;
			}
		}

		public override string Value
		{
			get
			{
				return navigator.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				return navigator.XmlLang;
			}
		}

		public override bool HasChildren
		{
			get
			{
				return navigator.HasChildren;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return navigator.HasAttributes;
			}
		}

		public XmlDocumentEditableNavigator(XPathEditableDocument doc)
		{
			document = doc;
			if (isXmlDocumentNavigatorImpl)
			{
				navigator = new XmlDocumentNavigator(doc.Node);
			}
			else
			{
				navigator = doc.CreateNavigator();
			}
		}

		public XmlDocumentEditableNavigator(XmlDocumentEditableNavigator nav)
		{
			document = nav.document;
			navigator = nav.navigator.Clone();
		}

		static XmlDocumentEditableNavigator()
		{
			isXmlDocumentNavigatorImpl = typeof(XmlDocumentEditableNavigator).Assembly == typeof(XmlDocument).Assembly;
		}

		public override XPathNavigator Clone()
		{
			return new XmlDocumentEditableNavigator(this);
		}

		public override XPathNavigator CreateNavigator()
		{
			return navigator.Clone();
		}

		public XmlNode GetNode()
		{
			return ((IHasXmlNode)navigator).GetNode();
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			XmlDocumentEditableNavigator xmlDocumentEditableNavigator = other as XmlDocumentEditableNavigator;
			if (xmlDocumentEditableNavigator != null)
			{
				return navigator.IsSamePosition(xmlDocumentEditableNavigator.navigator);
			}
			return navigator.IsSamePosition(xmlDocumentEditableNavigator);
		}

		public override bool MoveTo(XPathNavigator other)
		{
			XmlDocumentEditableNavigator xmlDocumentEditableNavigator = other as XmlDocumentEditableNavigator;
			if (xmlDocumentEditableNavigator != null)
			{
				return navigator.MoveTo(xmlDocumentEditableNavigator.navigator);
			}
			return navigator.MoveTo(xmlDocumentEditableNavigator);
		}

		public override bool MoveToFirstAttribute()
		{
			return navigator.MoveToFirstAttribute();
		}

		public override bool MoveToFirstChild()
		{
			return navigator.MoveToFirstChild();
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
		{
			return navigator.MoveToFirstNamespace(scope);
		}

		public override bool MoveToId(string id)
		{
			return navigator.MoveToId(id);
		}

		public override bool MoveToNext()
		{
			return navigator.MoveToNext();
		}

		public override bool MoveToNextAttribute()
		{
			return navigator.MoveToNextAttribute();
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope scope)
		{
			return navigator.MoveToNextNamespace(scope);
		}

		public override bool MoveToParent()
		{
			return navigator.MoveToParent();
		}

		public override bool MoveToPrevious()
		{
			return navigator.MoveToPrevious();
		}

		public override XmlWriter AppendChild()
		{
			XmlNode node = ((IHasXmlNode)navigator).GetNode();
			if (node == null)
			{
				throw new InvalidOperationException("Should not happen.");
			}
			return new XmlDocumentInsertionWriter(node, null);
		}

		public override void DeleteRange(XPathNavigator lastSiblingToDelete)
		{
			if (lastSiblingToDelete == null)
			{
				throw new ArgumentNullException();
			}
			XmlNode node = ((IHasXmlNode)navigator).GetNode();
			XmlNode xmlNode = null;
			if (lastSiblingToDelete is IHasXmlNode)
			{
				xmlNode = ((IHasXmlNode)lastSiblingToDelete).GetNode();
			}
			if (!navigator.MoveToParent())
			{
				throw new InvalidOperationException("There is no parent to remove current node.");
			}
			if (xmlNode == null || node.ParentNode != xmlNode.ParentNode)
			{
				throw new InvalidOperationException("Argument XPathNavigator has different parent node.");
			}
			XmlNode parentNode = node.ParentNode;
			bool flag = true;
			XmlNode xmlNode2 = node;
			while (flag)
			{
				flag = xmlNode2 != xmlNode;
				XmlNode nextSibling = xmlNode2.NextSibling;
				parentNode.RemoveChild(xmlNode2);
				xmlNode2 = nextSibling;
			}
		}

		public override XmlWriter ReplaceRange(XPathNavigator nav)
		{
			if (nav == null)
			{
				throw new ArgumentNullException();
			}
			XmlNode start = ((IHasXmlNode)navigator).GetNode();
			XmlNode end = null;
			if (nav is IHasXmlNode)
			{
				end = ((IHasXmlNode)nav).GetNode();
			}
			if (end == null || start.ParentNode != end.ParentNode)
			{
				throw new InvalidOperationException("Argument XPathNavigator has different parent node.");
			}
			XmlDocumentInsertionWriter xmlDocumentInsertionWriter = (XmlDocumentInsertionWriter)InsertBefore();
			XPathNavigator prev = Clone();
			if (!prev.MoveToPrevious())
			{
				prev = null;
			}
			XPathNavigator parentNav = Clone();
			parentNav.MoveToParent();
			xmlDocumentInsertionWriter.Closed += delegate
			{
				XmlNode parentNode = start.ParentNode;
				bool flag = true;
				XmlNode xmlNode = start;
				while (flag)
				{
					flag = xmlNode != end;
					XmlNode nextSibling = xmlNode.NextSibling;
					parentNode.RemoveChild(xmlNode);
					xmlNode = nextSibling;
				}
				if (prev != null)
				{
					MoveTo(prev);
					MoveToNext();
				}
				else
				{
					MoveTo(parentNav);
					MoveToFirstChild();
				}
			};
			return xmlDocumentInsertionWriter;
		}

		public override XmlWriter InsertBefore()
		{
			XmlNode node = ((IHasXmlNode)navigator).GetNode();
			return new XmlDocumentInsertionWriter(node.ParentNode, node);
		}

		public override XmlWriter CreateAttributes()
		{
			XmlNode node = ((IHasXmlNode)navigator).GetNode();
			return new XmlDocumentAttributeWriter(node);
		}

		public override void DeleteSelf()
		{
			XmlNode node = ((IHasXmlNode)navigator).GetNode();
			XmlAttribute xmlAttribute = node as XmlAttribute;
			if (xmlAttribute != null)
			{
				if (xmlAttribute.OwnerElement == null)
				{
					throw new InvalidOperationException("This attribute node cannot be removed since it has no owner element.");
				}
				navigator.MoveToParent();
				xmlAttribute.OwnerElement.RemoveAttributeNode(xmlAttribute);
			}
			else
			{
				if (node.ParentNode == null)
				{
					throw new InvalidOperationException("This node cannot be removed since it has no parent.");
				}
				navigator.MoveToParent();
				node.ParentNode.RemoveChild(node);
			}
		}

		public override void ReplaceSelf(XmlReader reader)
		{
			XmlNode node = ((IHasXmlNode)navigator).GetNode();
			XmlNode parentNode = node.ParentNode;
			if (parentNode == null)
			{
				throw new InvalidOperationException("This node cannot be removed since it has no parent.");
			}
			bool flag = false;
			if (!MoveToPrevious())
			{
				MoveToParent();
			}
			else
			{
				flag = true;
			}
			XmlDocument xmlDocument = ((parentNode.NodeType != XmlNodeType.Document) ? parentNode.OwnerDocument : (parentNode as XmlDocument));
			bool flag2 = false;
			if (reader.ReadState == ReadState.Initial)
			{
				reader.Read();
				if (reader.EOF)
				{
					flag2 = true;
				}
				else
				{
					while (!reader.EOF)
					{
						parentNode.AppendChild(xmlDocument.ReadNode(reader));
					}
				}
			}
			else if (reader.EOF)
			{
				flag2 = true;
			}
			else
			{
				parentNode.AppendChild(xmlDocument.ReadNode(reader));
			}
			if (flag2)
			{
				throw new InvalidOperationException("Content is required in argument XmlReader to replace current node.");
			}
			parentNode.RemoveChild(node);
			if (flag)
			{
				MoveToNext();
			}
			else
			{
				MoveToFirstChild();
			}
		}

		public override void SetValue(string value)
		{
			XmlNode node = ((IHasXmlNode)navigator).GetNode();
			while (node.FirstChild != null)
			{
				node.RemoveChild(node.FirstChild);
			}
			node.InnerText = value;
		}

		public override void MoveToRoot()
		{
			navigator.MoveToRoot();
		}

		public override bool MoveToNamespace(string name)
		{
			return navigator.MoveToNamespace(name);
		}

		public override bool MoveToFirst()
		{
			return navigator.MoveToFirst();
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			return navigator.MoveToAttribute(localName, namespaceURI);
		}

		public override bool IsDescendant(XPathNavigator nav)
		{
			XmlDocumentEditableNavigator xmlDocumentEditableNavigator = nav as XmlDocumentEditableNavigator;
			if (xmlDocumentEditableNavigator != null)
			{
				return navigator.IsDescendant(xmlDocumentEditableNavigator.navigator);
			}
			return navigator.IsDescendant(nav);
		}

		public override string GetNamespace(string name)
		{
			return navigator.GetNamespace(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return navigator.GetAttribute(localName, namespaceURI);
		}

		public override XmlNodeOrder ComparePosition(XPathNavigator nav)
		{
			XmlDocumentEditableNavigator xmlDocumentEditableNavigator = nav as XmlDocumentEditableNavigator;
			if (xmlDocumentEditableNavigator != null)
			{
				return navigator.ComparePosition(xmlDocumentEditableNavigator.navigator);
			}
			return navigator.ComparePosition(nav);
		}
	}
}
