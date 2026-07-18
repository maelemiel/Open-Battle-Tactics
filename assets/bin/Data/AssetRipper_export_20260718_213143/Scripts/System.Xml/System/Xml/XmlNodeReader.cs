using System.Collections.Generic;
using System.Xml.Schema;
using Mono.Xml;

namespace System.Xml
{
	public class XmlNodeReader : XmlReader, IHasXmlParserContext, IXmlNamespaceResolver
	{
		private XmlReader entity;

		private XmlNodeReaderImpl source;

		private bool entityInsideAttribute;

		private bool insideAttribute;

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				return ((IHasXmlParserContext)Current).ParserContext;
			}
		}

		private XmlReader Current
		{
			get
			{
				return (entity == null || entity.ReadState == ReadState.Initial) ? source : entity;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return Current.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return Current.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		public override int Depth
		{
			get
			{
				if (entity != null && entity.ReadState == ReadState.Interactive)
				{
					return source.Depth + entity.Depth + 1;
				}
				return source.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return source.EOF;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return Current.HasAttributes;
			}
		}

		public override bool HasValue
		{
			get
			{
				return Current.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return Current.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return Current.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return Current.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return Current.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return Current.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return Current.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (entity != null)
				{
					return (entity.ReadState == ReadState.Initial) ? source.NodeType : ((!entity.EOF) ? entity.NodeType : XmlNodeType.EndEntity);
				}
				return source.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return Current.Prefix;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return (entity != null) ? ReadState.Interactive : source.ReadState;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				IXmlSchemaInfo result;
				if (entity != null)
				{
					IXmlSchemaInfo schemaInfo = entity.SchemaInfo;
					result = schemaInfo;
				}
				else
				{
					result = source.SchemaInfo;
				}
				return result;
			}
		}

		public override string Value
		{
			get
			{
				return Current.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				return Current.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return Current.XmlSpace;
			}
		}

		public XmlNodeReader(XmlNode node)
		{
			source = new XmlNodeReaderImpl(node);
		}

		private XmlNodeReader(XmlNodeReaderImpl entityContainer, bool insideAttribute)
		{
			source = new XmlNodeReaderImpl(entityContainer);
			entityInsideAttribute = insideAttribute;
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return ((IXmlNamespaceResolver)Current).GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			return ((IXmlNamespaceResolver)Current).LookupPrefix(ns);
		}

		public override void Close()
		{
			if (entity != null)
			{
				entity.Close();
			}
			source.Close();
		}

		public override string GetAttribute(int attributeIndex)
		{
			return Current.GetAttribute(attributeIndex);
		}

		public override string GetAttribute(string name)
		{
			return Current.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return Current.GetAttribute(name, namespaceURI);
		}

		public override string LookupNamespace(string prefix)
		{
			return Current.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			if (entity != null && entityInsideAttribute)
			{
				entity.Close();
				entity = null;
			}
			Current.MoveToAttribute(i);
			insideAttribute = true;
		}

		public override bool MoveToAttribute(string name)
		{
			if (entity != null && !entityInsideAttribute)
			{
				return entity.MoveToAttribute(name);
			}
			if (!source.MoveToAttribute(name))
			{
				return false;
			}
			if (entity != null && entityInsideAttribute)
			{
				entity.Close();
				entity = null;
			}
			insideAttribute = true;
			return true;
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			if (entity != null && !entityInsideAttribute)
			{
				return entity.MoveToAttribute(localName, namespaceURI);
			}
			if (!source.MoveToAttribute(localName, namespaceURI))
			{
				return false;
			}
			if (entity != null && entityInsideAttribute)
			{
				entity.Close();
				entity = null;
			}
			insideAttribute = true;
			return true;
		}

		public override bool MoveToElement()
		{
			if (entity != null && entityInsideAttribute)
			{
				entity = null;
			}
			if (!Current.MoveToElement())
			{
				return false;
			}
			insideAttribute = false;
			return true;
		}

		public override bool MoveToFirstAttribute()
		{
			if (entity != null && !entityInsideAttribute)
			{
				return entity.MoveToFirstAttribute();
			}
			if (!source.MoveToFirstAttribute())
			{
				return false;
			}
			if (entity != null && entityInsideAttribute)
			{
				entity.Close();
				entity = null;
			}
			insideAttribute = true;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (entity != null && !entityInsideAttribute)
			{
				return entity.MoveToNextAttribute();
			}
			if (!source.MoveToNextAttribute())
			{
				return false;
			}
			if (entity != null && entityInsideAttribute)
			{
				entity.Close();
				entity = null;
			}
			insideAttribute = true;
			return true;
		}

		public override bool Read()
		{
			insideAttribute = false;
			if (entity != null && (entityInsideAttribute || entity.EOF))
			{
				entity = null;
			}
			if (entity != null)
			{
				entity.Read();
				return true;
			}
			return source.Read();
		}

		public override bool ReadAttributeValue()
		{
			if (entity != null && entityInsideAttribute)
			{
				if (!entity.EOF)
				{
					entity.Read();
					return true;
				}
				entity = null;
			}
			return Current.ReadAttributeValue();
		}

		public override int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadContentAsBase64(buffer, offset, length);
			}
			return source.ReadContentAsBase64(buffer, offset, length);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadContentAsBinHex(buffer, offset, length);
			}
			return source.ReadContentAsBinHex(buffer, offset, length);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadElementContentAsBase64(buffer, offset, length);
			}
			return source.ReadElementContentAsBase64(buffer, offset, length);
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (entity != null)
			{
				return entity.ReadElementContentAsBinHex(buffer, offset, length);
			}
			return source.ReadElementContentAsBinHex(buffer, offset, length);
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public override void ResolveEntity()
		{
			if (entity != null)
			{
				entity.ResolveEntity();
				return;
			}
			if (source.NodeType != XmlNodeType.EntityReference)
			{
				throw new InvalidOperationException("The current node is not an Entity Reference");
			}
			entity = new XmlNodeReader(source, insideAttribute);
		}

		public override void Skip()
		{
			if (entity != null && entityInsideAttribute)
			{
				entity = null;
			}
			Current.Skip();
		}
	}
}
