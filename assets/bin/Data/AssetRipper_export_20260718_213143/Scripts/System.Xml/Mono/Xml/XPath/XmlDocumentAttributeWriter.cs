using System;
using System.Xml;

namespace Mono.Xml.XPath
{
	internal class XmlDocumentAttributeWriter : XmlWriter
	{
		private XmlElement element;

		private WriteState state;

		private XmlAttribute attribute;

		public override WriteState WriteState
		{
			get
			{
				return state;
			}
		}

		public XmlDocumentAttributeWriter(XmlNode owner)
		{
			element = owner as XmlElement;
			if (element == null)
			{
				throw new ArgumentException("To write attributes, current node must be an element.");
			}
			state = WriteState.Content;
		}

		public override void Close()
		{
		}

		public override void Flush()
		{
		}

		public override string LookupPrefix(string ns)
		{
			return element.GetPrefixOfNamespace(ns);
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
			attribute = element.OwnerDocument.CreateAttribute(prefix, name, ns);
			state = WriteState.Attribute;
		}

		public override void WriteProcessingInstruction(string name, string value)
		{
			throw new NotSupportedException();
		}

		public override void WriteComment(string text)
		{
			throw new NotSupportedException();
		}

		public override void WriteCData(string text)
		{
			throw new NotSupportedException();
		}

		public override void WriteStartElement(string prefix, string name, string ns)
		{
			throw new NotSupportedException();
		}

		public override void WriteEndElement()
		{
			throw new NotSupportedException();
		}

		public override void WriteFullEndElement()
		{
			throw new NotSupportedException();
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
			throw new NotSupportedException();
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
			if (state != WriteState.Attribute)
			{
				throw new InvalidOperationException("Current state is not inside attribute. Cannot write attribute value.");
			}
			attribute.Value += text;
		}

		public override void WriteWhitespace(string text)
		{
			if (state != WriteState.Attribute)
			{
				throw new InvalidOperationException("Current state is not inside attribute. Cannot write attribute value.");
			}
			if (attribute.ChildNodes.Count == 0)
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
			if (state != WriteState.Attribute)
			{
				throw new InvalidOperationException("Current state is not inside attribute. Cannot close attribute.");
			}
			element.SetAttributeNode(attribute);
			attribute = null;
			state = WriteState.Content;
		}
	}
}
