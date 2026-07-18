using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using Mono.Xml;

namespace System.Xml
{
	internal class XmlNodeReaderImpl : XmlReader, IHasXmlParserContext, IXmlNamespaceResolver
	{
		private XmlDocument document;

		private XmlNode startNode;

		private XmlNode current;

		private XmlNode ownerLinkedNode;

		private ReadState state;

		private int depth;

		private bool isEndElement;

		private bool ignoreStartNode;

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				return new XmlParserContext(document.NameTable, (current != null) ? current.ConstructNamespaceManager() : new XmlNamespaceManager(document.NameTable), (document.DocumentType == null) ? null : document.DocumentType.DTD, (current != null) ? current.BaseURI : document.BaseURI, XmlLang, XmlSpace, Encoding.Unicode);
			}
		}

		public override int AttributeCount
		{
			get
			{
				if (state != ReadState.Interactive)
				{
					return 0;
				}
				if (isEndElement || current == null)
				{
					return 0;
				}
				XmlNode xmlNode = ownerLinkedNode;
				return (xmlNode.Attributes != null) ? xmlNode.Attributes.Count : 0;
			}
		}

		public override string BaseURI
		{
			get
			{
				if (current == null)
				{
					return startNode.BaseURI;
				}
				return current.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override bool CanReadValueChunk
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
				return false;
			}
		}

		public override int Depth
		{
			get
			{
				return (current != null) ? ((current == ownerLinkedNode) ? depth : ((current.NodeType != XmlNodeType.Attribute) ? (depth + 2) : (depth + 1))) : 0;
			}
		}

		public override bool EOF
		{
			get
			{
				return state == ReadState.EndOfFile || state == ReadState.Error;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				if (isEndElement || current == null)
				{
					return false;
				}
				XmlNode xmlNode = ownerLinkedNode;
				if (xmlNode.Attributes == null || xmlNode.Attributes.Count == 0)
				{
					return false;
				}
				return true;
			}
		}

		public override bool HasValue
		{
			get
			{
				if (current == null)
				{
					return false;
				}
				switch (current.NodeType)
				{
				case XmlNodeType.Element:
				case XmlNodeType.EntityReference:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentFragment:
				case XmlNodeType.Notation:
				case XmlNodeType.EndElement:
				case XmlNodeType.EndEntity:
					return false;
				default:
					return true;
				}
			}
		}

		public override bool IsDefault
		{
			get
			{
				if (current == null)
				{
					return false;
				}
				if (current.NodeType != XmlNodeType.Attribute)
				{
					return false;
				}
				return !((XmlAttribute)current).Specified;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				if (current == null)
				{
					return false;
				}
				if (current.NodeType == XmlNodeType.Element)
				{
					return ((XmlElement)current).IsEmpty;
				}
				return false;
			}
		}

		public override string LocalName
		{
			get
			{
				if (current == null)
				{
					return string.Empty;
				}
				switch (current.NodeType)
				{
				case XmlNodeType.Element:
				case XmlNodeType.Attribute:
				case XmlNodeType.EntityReference:
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.DocumentType:
				case XmlNodeType.XmlDeclaration:
					return current.LocalName;
				default:
					return string.Empty;
				}
			}
		}

		public override string Name
		{
			get
			{
				if (current == null)
				{
					return string.Empty;
				}
				switch (current.NodeType)
				{
				case XmlNodeType.Element:
				case XmlNodeType.Attribute:
				case XmlNodeType.EntityReference:
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.DocumentType:
				case XmlNodeType.XmlDeclaration:
					return current.Name;
				default:
					return string.Empty;
				}
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (current == null)
				{
					return string.Empty;
				}
				return current.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return document.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (current == null)
				{
					return XmlNodeType.None;
				}
				return (!isEndElement) ? current.NodeType : XmlNodeType.EndElement;
			}
		}

		public override string Prefix
		{
			get
			{
				if (current == null)
				{
					return string.Empty;
				}
				return current.Prefix;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return state;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				object result;
				if (current != null)
				{
					IXmlSchemaInfo schemaInfo = current.SchemaInfo;
					result = schemaInfo;
				}
				else
				{
					result = null;
				}
				return (IXmlSchemaInfo)result;
			}
		}

		public override string Value
		{
			get
			{
				if (NodeType == XmlNodeType.DocumentType)
				{
					return ((XmlDocumentType)current).InternalSubset;
				}
				return (!HasValue) ? string.Empty : current.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				if (current == null)
				{
					return startNode.XmlLang;
				}
				return current.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				if (current == null)
				{
					return startNode.XmlSpace;
				}
				return current.XmlSpace;
			}
		}

		internal XmlNodeReaderImpl(XmlNodeReaderImpl entityContainer)
			: this(entityContainer.current)
		{
		}

		public XmlNodeReaderImpl(XmlNode node)
		{
			startNode = node;
			depth = 0;
			document = ((startNode.NodeType != XmlNodeType.Document) ? startNode.OwnerDocument : (startNode as XmlDocument));
			switch (node.NodeType)
			{
			case XmlNodeType.EntityReference:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
				ignoreStartNode = true;
				break;
			}
		}

		public override void Close()
		{
			current = null;
			state = ReadState.Closed;
		}

		public override string GetAttribute(int attributeIndex)
		{
			if (NodeType == XmlNodeType.XmlDeclaration)
			{
				XmlDeclaration xmlDeclaration = current as XmlDeclaration;
				switch (attributeIndex)
				{
				case 0:
					return xmlDeclaration.Version;
				case 1:
					if (xmlDeclaration.Encoding != string.Empty)
					{
						return xmlDeclaration.Encoding;
					}
					if (xmlDeclaration.Standalone != string.Empty)
					{
						return xmlDeclaration.Standalone;
					}
					break;
				case 2:
					if (xmlDeclaration.Encoding != string.Empty && xmlDeclaration.Standalone != null)
					{
						return xmlDeclaration.Standalone;
					}
					break;
				}
				throw new ArgumentOutOfRangeException("Index out of range.");
			}
			if (NodeType == XmlNodeType.DocumentType)
			{
				XmlDocumentType xmlDocumentType = current as XmlDocumentType;
				switch (attributeIndex)
				{
				case 0:
					if (xmlDocumentType.PublicId != string.Empty)
					{
						return xmlDocumentType.PublicId;
					}
					if (xmlDocumentType.SystemId != string.Empty)
					{
						return xmlDocumentType.SystemId;
					}
					break;
				case 1:
					if (xmlDocumentType.PublicId == string.Empty && xmlDocumentType.SystemId != string.Empty)
					{
						return xmlDocumentType.SystemId;
					}
					break;
				}
				throw new ArgumentOutOfRangeException("Index out of range.");
			}
			if (isEndElement || current == null)
			{
				return null;
			}
			if (attributeIndex < 0 || attributeIndex > AttributeCount)
			{
				throw new ArgumentOutOfRangeException("Index out of range.");
			}
			return ownerLinkedNode.Attributes[attributeIndex].Value;
		}

		public override string GetAttribute(string name)
		{
			if (isEndElement || current == null)
			{
				return null;
			}
			if (NodeType == XmlNodeType.XmlDeclaration)
			{
				return GetXmlDeclarationAttribute(name);
			}
			if (NodeType == XmlNodeType.DocumentType)
			{
				return GetDocumentTypeAttribute(name);
			}
			if (ownerLinkedNode.Attributes == null)
			{
				return null;
			}
			XmlAttribute xmlAttribute = ownerLinkedNode.Attributes[name];
			if (xmlAttribute == null)
			{
				return null;
			}
			return xmlAttribute.Value;
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			if (isEndElement || current == null)
			{
				return null;
			}
			if (NodeType == XmlNodeType.XmlDeclaration)
			{
				return GetXmlDeclarationAttribute(name);
			}
			if (NodeType == XmlNodeType.DocumentType)
			{
				return GetDocumentTypeAttribute(name);
			}
			if (ownerLinkedNode.Attributes == null)
			{
				return null;
			}
			XmlAttribute xmlAttribute = ownerLinkedNode.Attributes[name, namespaceURI];
			if (xmlAttribute == null)
			{
				return null;
			}
			return xmlAttribute.Value;
		}

		private string GetXmlDeclarationAttribute(string name)
		{
			XmlDeclaration xmlDeclaration = current as XmlDeclaration;
			switch (name)
			{
			case "version":
				return xmlDeclaration.Version;
			case "encoding":
				return (!(xmlDeclaration.Encoding != string.Empty)) ? null : xmlDeclaration.Encoding;
			case "standalone":
				return xmlDeclaration.Standalone;
			default:
				return null;
			}
		}

		private string GetDocumentTypeAttribute(string name)
		{
			XmlDocumentType xmlDocumentType = current as XmlDocumentType;
			switch (name)
			{
			case "PUBLIC":
				return xmlDocumentType.PublicId;
			case "SYSTEM":
				return xmlDocumentType.SystemId;
			default:
				return null;
			}
		}

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			XmlNode parentNode = current;
			while (parentNode.NodeType != XmlNodeType.Document)
			{
				for (int i = 0; i < current.Attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = current.Attributes[i];
					if (xmlAttribute.NamespaceURI == "http://www.w3.org/2000/xmlns/")
					{
						dictionary.Add((!(xmlAttribute.Prefix == "xmlns")) ? string.Empty : xmlAttribute.LocalName, xmlAttribute.Value);
					}
				}
				if (scope == XmlNamespaceScope.Local)
				{
					return dictionary;
				}
				parentNode = parentNode.ParentNode;
				if (parentNode == null)
				{
					break;
				}
			}
			if (scope == XmlNamespaceScope.All)
			{
				dictionary.Add("xml", "http://www.w3.org/XML/1998/namespace");
			}
			return dictionary;
		}

		private XmlElement GetCurrentElement()
		{
			XmlElement result = null;
			switch (current.NodeType)
			{
			case XmlNodeType.Attribute:
				result = ((XmlAttribute)current).OwnerElement;
				break;
			case XmlNodeType.Element:
				result = (XmlElement)current;
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.EntityReference:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				result = current.ParentNode as XmlElement;
				break;
			}
			return result;
		}

		public override string LookupNamespace(string prefix)
		{
			if (current == null)
			{
				return null;
			}
			for (XmlElement xmlElement = GetCurrentElement(); xmlElement != null; xmlElement = xmlElement.ParentNode as XmlElement)
			{
				for (int i = 0; i < xmlElement.Attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = xmlElement.Attributes[i];
					if (xmlAttribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						continue;
					}
					if (prefix == string.Empty)
					{
						if (xmlAttribute.Prefix == string.Empty)
						{
							return xmlAttribute.Value;
						}
					}
					else if (xmlAttribute.LocalName == prefix)
					{
						return xmlAttribute.Value;
					}
				}
			}
			switch (prefix)
			{
			case "xml":
				return "http://www.w3.org/XML/1998/namespace";
			case "xmlns":
				return "http://www.w3.org/2000/xmlns/";
			default:
				return null;
			}
		}

		public string LookupPrefix(string ns)
		{
			return LookupPrefix(ns, false);
		}

		public string LookupPrefix(string ns, bool atomizedNames)
		{
			if (current == null)
			{
				return null;
			}
			for (XmlElement xmlElement = GetCurrentElement(); xmlElement != null; xmlElement = xmlElement.ParentNode as XmlElement)
			{
				for (int i = 0; i < xmlElement.Attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = xmlElement.Attributes[i];
					if (atomizedNames)
					{
						if (object.ReferenceEquals(xmlAttribute.NamespaceURI, "http://www.w3.org/2000/xmlns/") && object.ReferenceEquals(xmlAttribute.Value, ns))
						{
							return (!(xmlAttribute.Prefix != string.Empty)) ? string.Empty : xmlAttribute.LocalName;
						}
					}
					else if (!(xmlAttribute.NamespaceURI != "http://www.w3.org/2000/xmlns/") && xmlAttribute.Value == ns)
					{
						return (!(xmlAttribute.Prefix != string.Empty)) ? string.Empty : xmlAttribute.LocalName;
					}
				}
			}
			switch (ns)
			{
			case "http://www.w3.org/XML/1998/namespace":
				return "xml";
			case "http://www.w3.org/2000/xmlns/":
				return "xmlns";
			default:
				return null;
			}
		}

		public override void MoveToAttribute(int attributeIndex)
		{
			if (isEndElement || attributeIndex < 0 || attributeIndex > AttributeCount)
			{
				throw new ArgumentOutOfRangeException();
			}
			state = ReadState.Interactive;
			current = ownerLinkedNode.Attributes[attributeIndex];
		}

		public override bool MoveToAttribute(string name)
		{
			if (isEndElement || current == null)
			{
				return false;
			}
			XmlNode xmlNode = current;
			if (current.ParentNode.NodeType == XmlNodeType.Attribute)
			{
				current = current.ParentNode;
			}
			if (ownerLinkedNode.Attributes == null)
			{
				return false;
			}
			XmlAttribute xmlAttribute = ownerLinkedNode.Attributes[name];
			if (xmlAttribute == null)
			{
				current = xmlNode;
				return false;
			}
			current = xmlAttribute;
			return true;
		}

		public override bool MoveToAttribute(string name, string namespaceURI)
		{
			if (isEndElement || current == null)
			{
				return false;
			}
			if (ownerLinkedNode.Attributes == null)
			{
				return false;
			}
			XmlAttribute xmlAttribute = ownerLinkedNode.Attributes[name, namespaceURI];
			if (xmlAttribute == null)
			{
				return false;
			}
			current = xmlAttribute;
			return true;
		}

		public override bool MoveToElement()
		{
			if (current == null)
			{
				return false;
			}
			XmlNode xmlNode = ownerLinkedNode;
			if (current != xmlNode)
			{
				current = xmlNode;
				return true;
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			if (current == null)
			{
				return false;
			}
			if (ownerLinkedNode.Attributes == null)
			{
				return false;
			}
			if (ownerLinkedNode.Attributes.Count > 0)
			{
				current = ownerLinkedNode.Attributes[0];
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			if (current == null)
			{
				return false;
			}
			XmlNode parentNode = current;
			if (current.NodeType != XmlNodeType.Attribute)
			{
				if (current.ParentNode == null || current.ParentNode.NodeType != XmlNodeType.Attribute)
				{
					return MoveToFirstAttribute();
				}
				parentNode = current.ParentNode;
			}
			XmlAttributeCollection attributes = ((XmlAttribute)parentNode).OwnerElement.Attributes;
			for (int i = 0; i < attributes.Count - 1; i++)
			{
				XmlAttribute xmlAttribute = attributes[i];
				if (xmlAttribute == parentNode)
				{
					i++;
					if (i == attributes.Count)
					{
						return false;
					}
					current = attributes[i];
					return true;
				}
			}
			return false;
		}

		public override bool Read()
		{
			switch (state)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return false;
			default:
			{
				if (base.Binary != null)
				{
					base.Binary.Reset();
				}
				bool result = ReadContent();
				ownerLinkedNode = current;
				return result;
			}
			}
		}

		private bool ReadContent()
		{
			if (ReadState == ReadState.Initial)
			{
				current = startNode;
				state = ReadState.Interactive;
				if (ignoreStartNode)
				{
					current = startNode.FirstChild;
				}
				if (current == null)
				{
					state = ReadState.Error;
					return false;
				}
				return true;
			}
			MoveToElement();
			XmlNode xmlNode = ((isEndElement || current.NodeType == XmlNodeType.EntityReference) ? null : current.FirstChild);
			if (xmlNode != null)
			{
				isEndElement = false;
				current = xmlNode;
				depth++;
				return true;
			}
			if (current == startNode)
			{
				if (IsEmptyElement || isEndElement)
				{
					isEndElement = false;
					current = null;
					state = ReadState.EndOfFile;
					return false;
				}
				isEndElement = true;
				return true;
			}
			if (!isEndElement && !IsEmptyElement && current.NodeType == XmlNodeType.Element)
			{
				isEndElement = true;
				return true;
			}
			XmlNode nextSibling = current.NextSibling;
			if (nextSibling != null)
			{
				isEndElement = false;
				current = nextSibling;
				return true;
			}
			XmlNode parentNode = current.ParentNode;
			if (parentNode == null || (parentNode == startNode && ignoreStartNode))
			{
				isEndElement = false;
				current = null;
				state = ReadState.EndOfFile;
				return false;
			}
			current = parentNode;
			depth--;
			isEndElement = true;
			return true;
		}

		public override bool ReadAttributeValue()
		{
			if (current.NodeType == XmlNodeType.Attribute)
			{
				if (current.FirstChild == null)
				{
					return false;
				}
				current = current.FirstChild;
				return true;
			}
			if (current.ParentNode.NodeType == XmlNodeType.Attribute)
			{
				if (current.NextSibling == null)
				{
					return false;
				}
				current = current.NextSibling;
				return true;
			}
			return false;
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public override void ResolveEntity()
		{
			throw new NotSupportedException("Should not happen.");
		}

		public override void Skip()
		{
			base.Skip();
		}
	}
}
