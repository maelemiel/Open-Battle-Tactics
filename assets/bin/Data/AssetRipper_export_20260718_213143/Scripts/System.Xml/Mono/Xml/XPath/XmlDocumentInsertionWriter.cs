using System;
using System.Xml;

namespace Mono.Xml.XPath
{
	internal class XmlDocumentInsertionWriter : XmlWriter
	{
		private XmlNode parent;

		private XmlNode current;

		private XmlNode nextSibling;

		private WriteState state;

		private XmlAttribute attribute;

		public override WriteState WriteState
		{
			get
			{
				return state;
			}
		}

		internal event XmlWriterClosedEventHandler Closed;

		public XmlDocumentInsertionWriter(XmlNode owner, XmlNode nextSibling)
		{
			parent = owner;
			if (parent == null)
			{
				throw new InvalidOperationException();
			}
			switch (parent.NodeType)
			{
			case XmlNodeType.Document:
				current = ((XmlDocument)parent).CreateDocumentFragment();
				break;
			case XmlNodeType.Element:
			case XmlNodeType.DocumentFragment:
				current = parent.OwnerDocument.CreateDocumentFragment();
				break;
			default:
				throw new InvalidOperationException(string.Format("Insertion into {0} node is not allowed.", parent.NodeType));
			}
			this.nextSibling = nextSibling;
			state = WriteState.Content;
		}

		public override void Close()
		{
			while (current.ParentNode != null)
			{
				current = current.ParentNode;
			}
			parent.InsertBefore((XmlDocumentFragment)current, nextSibling);
			if (this.Closed != null)
			{
				this.Closed(this);
			}
		}

		public override void Flush()
		{
		}

		public override string LookupPrefix(string ns)
		{
			return current.GetPrefixOfNamespace(ns);
		}

		public override void WriteStartAttribute(string prefix, string name, string ns)
		{
			if (state != WriteState.Content)
			{
				throw new InvalidOperationException("Current state is not inside element. Cannot start attribute.");
			}
			if (prefix == null && ns != null && ns.Length > 0)
			{
				prefix = LookupPrefix(ns);
			}
			attribute = current.OwnerDocument.CreateAttribute(prefix, name, ns);
			state = WriteState.Attribute;
		}

		public override void WriteProcessingInstruction(string name, string value)
		{
			XmlProcessingInstruction newChild = current.OwnerDocument.CreateProcessingInstruction(name, value);
			current.AppendChild(newChild);
		}

		public override void WriteComment(string text)
		{
			XmlComment newChild = current.OwnerDocument.CreateComment(text);
			current.AppendChild(newChild);
		}

		public override void WriteCData(string text)
		{
			XmlCDataSection newChild = current.OwnerDocument.CreateCDataSection(text);
			current.AppendChild(newChild);
		}

		public override void WriteStartElement(string prefix, string name, string ns)
		{
			if (prefix == null && ns != null && ns.Length > 0)
			{
				prefix = LookupPrefix(ns);
			}
			XmlElement newChild = current.OwnerDocument.CreateElement(prefix, name, ns);
			current.AppendChild(newChild);
			current = newChild;
		}

		public override void WriteEndElement()
		{
			current = current.ParentNode;
			if (current == null)
			{
				throw new InvalidOperationException("No element is opened.");
			}
		}

		public override void WriteFullEndElement()
		{
			XmlElement xmlElement = current as XmlElement;
			if (xmlElement != null)
			{
				xmlElement.IsEmpty = false;
			}
			WriteEndElement();
		}

		public override void WriteDocType(string name, string pubid, string systemId, string intsubset)
		{
			throw new NotSupportedException();
		}

		public override void WriteStartDocument()
		{
			throw new NotSupportedException();
		}

		public override void WriteStartDocument(bool standalone)
		{
			throw new NotSupportedException();
		}

		public override void WriteEndDocument()
		{
			throw new NotSupportedException();
		}

		public override void WriteBase64(byte[] data, int start, int length)
		{
			WriteString(Convert.ToBase64String(data, start, length));
		}

		public override void WriteRaw(char[] raw, int start, int length)
		{
			throw new NotSupportedException();
		}

		public override void WriteRaw(string raw)
		{
			throw new NotSupportedException();
		}

		public override void WriteSurrogateCharEntity(char msb, char lsb)
		{
			throw new NotSupportedException();
		}

		public override void WriteCharEntity(char c)
		{
			throw new NotSupportedException();
		}

		public override void WriteEntityRef(string entname)
		{
			if (state != WriteState.Attribute)
			{
				throw new InvalidOperationException("Current state is not inside attribute. Cannot write attribute value.");
			}
			attribute.AppendChild(attribute.OwnerDocument.CreateEntityReference(entname));
		}

		public override void WriteChars(char[] data, int start, int length)
		{
			WriteString(new string(data, start, length));
		}

		public override void WriteString(string text)
		{
			if (attribute != null)
			{
				attribute.Value += text;
				return;
			}
			XmlText newChild = current.OwnerDocument.CreateTextNode(text);
			current.AppendChild(newChild);
		}

		public override void WriteWhitespace(string text)
		{
			if (state != WriteState.Attribute)
			{
				current.AppendChild(current.OwnerDocument.CreateTextNode(text));
			}
			else if (attribute.ChildNodes.Count == 0)
			{
				attribute.AppendChild(attribute.OwnerDocument.CreateWhitespace(text));
			}
			else
			{
				attribute.Value += text;
			}
		}

		public override void WriteEndAttribute()
		{
			XmlElement xmlElement = (current as XmlElement) ?? ((nextSibling != null) ? null : (parent as XmlElement));
			if (state != WriteState.Attribute || xmlElement == null)
			{
				throw new InvalidOperationException("Current state is not inside attribute. Cannot close attribute.");
			}
			xmlElement.SetAttributeNode(attribute);
			attribute = null;
			state = WriteState.Content;
		}
	}
}
