using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;
using Mono.Xml;
using Mono.Xml.XPath;

namespace System.Xml
{
	public class XmlDocument : XmlNode, IHasXmlChildNode
	{
		private static readonly Type[] optimal_create_types = new Type[3]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		};

		private bool optimal_create_element;

		private bool optimal_create_attribute;

		private XmlNameTable nameTable;

		private string baseURI = string.Empty;

		private XmlImplementation implementation;

		private bool preserveWhitespace;

		private XmlResolver resolver;

		private Hashtable idTable = new Hashtable();

		private XmlNameEntryCache nameCache;

		private XmlLinkedNode lastLinkedChild;

		private XmlAttribute nsNodeXml;

		private XmlSchemaSet schemas;

		private IXmlSchemaInfo schemaInfo;

		private bool loadMode;

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

		internal XmlAttribute NsNodeXml
		{
			get
			{
				if (nsNodeXml == null)
				{
					nsNodeXml = CreateAttribute("xmlns", "xml", "http://www.w3.org/2000/xmlns/");
					nsNodeXml.Value = "http://www.w3.org/XML/1998/namespace";
				}
				return nsNodeXml;
			}
		}

		public override string BaseURI
		{
			get
			{
				return baseURI;
			}
		}

		public XmlElement DocumentElement
		{
			get
			{
				XmlNode xmlNode = FirstChild;
				while (xmlNode != null && !(xmlNode is XmlElement))
				{
					xmlNode = xmlNode.NextSibling;
				}
				return (xmlNode == null) ? null : (xmlNode as XmlElement);
			}
		}

		public virtual XmlDocumentType DocumentType
		{
			get
			{
				for (XmlNode xmlNode = FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.NodeType == XmlNodeType.DocumentType)
					{
						return (XmlDocumentType)xmlNode;
					}
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						return null;
					}
				}
				return null;
			}
		}

		public XmlImplementation Implementation
		{
			get
			{
				return implementation;
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
				LoadXml(value);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal bool IsStandalone
		{
			get
			{
				return FirstChild != null && FirstChild.NodeType == XmlNodeType.XmlDeclaration && ((XmlDeclaration)FirstChild).Standalone == "yes";
			}
		}

		public override string LocalName
		{
			get
			{
				return "#document";
			}
		}

		public override string Name
		{
			get
			{
				return "#document";
			}
		}

		internal XmlNameEntryCache NameCache
		{
			get
			{
				return nameCache;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return nameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Document;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return null;
			}
		}

		public bool PreserveWhitespace
		{
			get
			{
				return preserveWhitespace;
			}
			set
			{
				preserveWhitespace = value;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return resolver;
			}
		}

		internal override string XmlLang
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual XmlResolver XmlResolver
		{
			set
			{
				resolver = value;
			}
		}

		internal override XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		internal Encoding TextEncoding
		{
			get
			{
				XmlDeclaration xmlDeclaration = FirstChild as XmlDeclaration;
				if (xmlDeclaration == null || xmlDeclaration.Encoding == string.Empty)
				{
					return null;
				}
				return Encoding.GetEncoding(xmlDeclaration.Encoding);
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		public XmlSchemaSet Schemas
		{
			get
			{
				if (schemas == null)
				{
					schemas = new XmlSchemaSet();
				}
				return schemas;
			}
			set
			{
				schemas = value;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return schemaInfo;
			}
			internal set
			{
				schemaInfo = value;
			}
		}

		public event XmlNodeChangedEventHandler NodeChanged;

		public event XmlNodeChangedEventHandler NodeChanging;

		public event XmlNodeChangedEventHandler NodeInserted;

		public event XmlNodeChangedEventHandler NodeInserting;

		public event XmlNodeChangedEventHandler NodeRemoved;

		public event XmlNodeChangedEventHandler NodeRemoving;

		public XmlDocument()
			: this(null, null)
		{
		}

		protected internal XmlDocument(XmlImplementation imp)
			: this(imp, null)
		{
		}

		public XmlDocument(XmlNameTable nt)
			: this(null, nt)
		{
		}

		private XmlDocument(XmlImplementation impl, XmlNameTable nt)
			: base(null)
		{
			if (impl == null)
			{
				implementation = new XmlImplementation();
			}
			else
			{
				implementation = impl;
			}
			nameTable = ((nt == null) ? implementation.InternalNameTable : nt);
			nameCache = new XmlNameEntryCache(nameTable);
			AddDefaultNameTableKeys();
			resolver = new XmlUrlResolver();
			Type type = GetType();
			optimal_create_element = type.GetMethod("CreateElement", optimal_create_types).DeclaringType == typeof(XmlDocument);
			optimal_create_attribute = type.GetMethod("CreateAttribute", optimal_create_types).DeclaringType == typeof(XmlDocument);
		}

		internal void AddIdenticalAttribute(XmlAttribute attr)
		{
			idTable[attr.Value] = attr;
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDocument xmlDocument = ((implementation == null) ? new XmlDocument() : implementation.CreateDocument());
			xmlDocument.baseURI = baseURI;
			if (deep)
			{
				for (XmlNode xmlNode = FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					xmlDocument.AppendChild(xmlDocument.ImportNode(xmlNode, deep), false);
				}
			}
			return xmlDocument;
		}

		public XmlAttribute CreateAttribute(string name)
		{
			string namespaceURI = string.Empty;
			string prefix;
			string localName;
			ParseName(name, out prefix, out localName);
			if (prefix == "xmlns" || (prefix == string.Empty && localName == "xmlns"))
			{
				namespaceURI = "http://www.w3.org/2000/xmlns/";
			}
			else if (prefix == "xml")
			{
				namespaceURI = "http://www.w3.org/XML/1998/namespace";
			}
			return CreateAttribute(prefix, localName, namespaceURI);
		}

		public XmlAttribute CreateAttribute(string qualifiedName, string namespaceURI)
		{
			string prefix;
			string localName;
			ParseName(qualifiedName, out prefix, out localName);
			return CreateAttribute(prefix, localName, namespaceURI);
		}

		public virtual XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			if (localName == null || localName == string.Empty)
			{
				throw new ArgumentException("The attribute local name cannot be empty.");
			}
			return new XmlAttribute(prefix, localName, namespaceURI, this, false, true);
		}

		internal XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI, bool atomizedNames, bool checkNamespace)
		{
			if (optimal_create_attribute)
			{
				return new XmlAttribute(prefix, localName, namespaceURI, this, atomizedNames, checkNamespace);
			}
			return CreateAttribute(prefix, localName, namespaceURI);
		}

		public virtual XmlCDataSection CreateCDataSection(string data)
		{
			return new XmlCDataSection(data, this);
		}

		public virtual XmlComment CreateComment(string data)
		{
			return new XmlComment(data, this);
		}

		protected internal virtual XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI)
		{
			XmlAttribute xmlAttribute = CreateAttribute(prefix, localName, namespaceURI);
			xmlAttribute.isDefault = true;
			return xmlAttribute;
		}

		public virtual XmlDocumentFragment CreateDocumentFragment()
		{
			return new XmlDocumentFragment(this);
		}

		public virtual XmlDocumentType CreateDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new XmlDocumentType(name, publicId, systemId, internalSubset, this);
		}

		private XmlDocumentType CreateDocumentType(DTDObjectModel dtd)
		{
			return new XmlDocumentType(dtd, this);
		}

		public XmlElement CreateElement(string name)
		{
			return CreateElement(name, string.Empty);
		}

		public XmlElement CreateElement(string qualifiedName, string namespaceURI)
		{
			string prefix;
			string localName;
			ParseName(qualifiedName, out prefix, out localName);
			return CreateElement(prefix, localName, namespaceURI);
		}

		public virtual XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			return new XmlElement((prefix == null) ? string.Empty : prefix, localName, (namespaceURI == null) ? string.Empty : namespaceURI, this, false);
		}

		internal XmlElement CreateElement(string prefix, string localName, string namespaceURI, bool nameAtomized)
		{
			if (localName == null || localName == string.Empty)
			{
				throw new ArgumentException("The local name for elements or attributes cannot be null or an empty string.");
			}
			if (optimal_create_element)
			{
				return new XmlElement((prefix == null) ? string.Empty : prefix, localName, (namespaceURI == null) ? string.Empty : namespaceURI, this, nameAtomized);
			}
			return CreateElement(prefix, localName, namespaceURI);
		}

		public virtual XmlEntityReference CreateEntityReference(string name)
		{
			return new XmlEntityReference(name, this);
		}

		public override XPathNavigator CreateNavigator()
		{
			return CreateNavigator(this);
		}

		protected internal virtual XPathNavigator CreateNavigator(XmlNode node)
		{
			return new XPathEditableDocument(node).CreateNavigator();
		}

		public virtual XmlNode CreateNode(string nodeTypeString, string name, string namespaceURI)
		{
			return CreateNode(GetNodeTypeFromString(nodeTypeString), name, namespaceURI);
		}

		public virtual XmlNode CreateNode(XmlNodeType type, string name, string namespaceURI)
		{
			string prefix = null;
			string localName = name;
			if (type == XmlNodeType.Attribute || type == XmlNodeType.Element || type == XmlNodeType.EntityReference)
			{
				ParseName(name, out prefix, out localName);
			}
			return CreateNode(type, prefix, localName, namespaceURI);
		}

		public virtual XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceURI)
		{
			switch (type)
			{
			case XmlNodeType.Attribute:
				return CreateAttribute(prefix, name, namespaceURI);
			case XmlNodeType.CDATA:
				return CreateCDataSection(null);
			case XmlNodeType.Comment:
				return CreateComment(null);
			case XmlNodeType.Document:
				return new XmlDocument();
			case XmlNodeType.DocumentFragment:
				return CreateDocumentFragment();
			case XmlNodeType.DocumentType:
				return CreateDocumentType(null, null, null, null);
			case XmlNodeType.Element:
				return CreateElement(prefix, name, namespaceURI);
			case XmlNodeType.EntityReference:
				return CreateEntityReference(null);
			case XmlNodeType.ProcessingInstruction:
				return CreateProcessingInstruction(null, null);
			case XmlNodeType.SignificantWhitespace:
				return CreateSignificantWhitespace(string.Empty);
			case XmlNodeType.Text:
				return CreateTextNode(null);
			case XmlNodeType.Whitespace:
				return CreateWhitespace(string.Empty);
			case XmlNodeType.XmlDeclaration:
				return CreateXmlDeclaration("1.0", null, null);
			default:
				throw new ArgumentException(string.Format("{0}\nParameter name: {1}", "Specified argument was out of the range of valid values", type.ToString()));
			}
		}

		public virtual XmlProcessingInstruction CreateProcessingInstruction(string target, string data)
		{
			return new XmlProcessingInstruction(target, data, this);
		}

		public virtual XmlSignificantWhitespace CreateSignificantWhitespace(string text)
		{
			if (!XmlChar.IsWhitespace(text))
			{
				throw new ArgumentException("Invalid whitespace characters.");
			}
			return new XmlSignificantWhitespace(text, this);
		}

		public virtual XmlText CreateTextNode(string text)
		{
			return new XmlText(text, this);
		}

		public virtual XmlWhitespace CreateWhitespace(string text)
		{
			if (!XmlChar.IsWhitespace(text))
			{
				throw new ArgumentException("Invalid whitespace characters.");
			}
			return new XmlWhitespace(text, this);
		}

		public virtual XmlDeclaration CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			if (version != "1.0")
			{
				throw new ArgumentException("version string is not correct.");
			}
			if (standalone != null && standalone != string.Empty && !(standalone == "yes") && !(standalone == "no"))
			{
				throw new ArgumentException("standalone string is not correct.");
			}
			return new XmlDeclaration(version, encoding, standalone, this);
		}

		public virtual XmlElement GetElementById(string elementId)
		{
			XmlAttribute identicalAttribute = GetIdenticalAttribute(elementId);
			return (identicalAttribute == null) ? null : identicalAttribute.OwnerElement;
		}

		public virtual XmlNodeList GetElementsByTagName(string name)
		{
			ArrayList arrayList = new ArrayList();
			SearchDescendantElements(name, name == "*", arrayList);
			return new XmlNodeArrayList(arrayList);
		}

		public virtual XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
		{
			ArrayList arrayList = new ArrayList();
			SearchDescendantElements(localName, localName == "*", namespaceURI, namespaceURI == "*", arrayList);
			return new XmlNodeArrayList(arrayList);
		}

		private XmlNodeType GetNodeTypeFromString(string nodeTypeString)
		{
			if (nodeTypeString == null)
			{
				throw new ArgumentNullException("nodeTypeString");
			}
			switch (nodeTypeString)
			{
			case "attribute":
				return XmlNodeType.Attribute;
			case "cdatasection":
				return XmlNodeType.CDATA;
			case "comment":
				return XmlNodeType.Comment;
			case "document":
				return XmlNodeType.Document;
			case "documentfragment":
				return XmlNodeType.DocumentFragment;
			case "documenttype":
				return XmlNodeType.DocumentType;
			case "element":
				return XmlNodeType.Element;
			case "entityreference":
				return XmlNodeType.EntityReference;
			case "processinginstruction":
				return XmlNodeType.ProcessingInstruction;
			case "significantwhitespace":
				return XmlNodeType.SignificantWhitespace;
			case "text":
				return XmlNodeType.Text;
			case "whitespace":
				return XmlNodeType.Whitespace;
			default:
				throw new ArgumentException(string.Format("The string doesn't represent any node type : {0}.", nodeTypeString));
			}
		}

		internal XmlAttribute GetIdenticalAttribute(string id)
		{
			XmlAttribute xmlAttribute = idTable[id] as XmlAttribute;
			if (xmlAttribute == null)
			{
				return null;
			}
			if (xmlAttribute.OwnerElement == null || !xmlAttribute.OwnerElement.IsRooted)
			{
				return null;
			}
			return xmlAttribute;
		}

		public virtual XmlNode ImportNode(XmlNode node, bool deep)
		{
			if (node == null)
			{
				throw new NullReferenceException("Null node cannot be imported.");
			}
			switch (node.NodeType)
			{
			case XmlNodeType.Attribute:
			{
				XmlAttribute xmlAttribute2 = node as XmlAttribute;
				XmlAttribute xmlAttribute3 = CreateAttribute(xmlAttribute2.Prefix, xmlAttribute2.LocalName, xmlAttribute2.NamespaceURI);
				for (XmlNode xmlNode3 = xmlAttribute2.FirstChild; xmlNode3 != null; xmlNode3 = xmlNode3.NextSibling)
				{
					xmlAttribute3.AppendChild(ImportNode(xmlNode3, deep));
				}
				return xmlAttribute3;
			}
			case XmlNodeType.CDATA:
				return CreateCDataSection(node.Value);
			case XmlNodeType.Comment:
				return CreateComment(node.Value);
			case XmlNodeType.Document:
				throw new XmlException("Document cannot be imported.");
			case XmlNodeType.DocumentFragment:
			{
				XmlDocumentFragment xmlDocumentFragment = CreateDocumentFragment();
				if (deep)
				{
					for (XmlNode xmlNode2 = node.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
					{
						xmlDocumentFragment.AppendChild(ImportNode(xmlNode2, deep));
					}
				}
				return xmlDocumentFragment;
			}
			case XmlNodeType.DocumentType:
				throw new XmlException("DocumentType cannot be imported.");
			case XmlNodeType.Element:
			{
				XmlElement xmlElement = (XmlElement)node;
				XmlElement xmlElement2 = CreateElement(xmlElement.Prefix, xmlElement.LocalName, xmlElement.NamespaceURI);
				for (int i = 0; i < xmlElement.Attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = xmlElement.Attributes[i];
					if (xmlAttribute.Specified)
					{
						xmlElement2.SetAttributeNode((XmlAttribute)ImportNode(xmlAttribute, deep));
					}
				}
				if (deep)
				{
					for (XmlNode xmlNode = xmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
					{
						xmlElement2.AppendChild(ImportNode(xmlNode, deep));
					}
				}
				return xmlElement2;
			}
			case XmlNodeType.EndElement:
				throw new XmlException("Illegal ImportNode call for NodeType.EndElement");
			case XmlNodeType.EndEntity:
				throw new XmlException("Illegal ImportNode call for NodeType.EndEntity");
			case XmlNodeType.EntityReference:
				return CreateEntityReference(node.Name);
			case XmlNodeType.None:
				throw new XmlException("Illegal ImportNode call for NodeType.None");
			case XmlNodeType.ProcessingInstruction:
			{
				XmlProcessingInstruction xmlProcessingInstruction = node as XmlProcessingInstruction;
				return CreateProcessingInstruction(xmlProcessingInstruction.Target, xmlProcessingInstruction.Data);
			}
			case XmlNodeType.SignificantWhitespace:
				return CreateSignificantWhitespace(node.Value);
			case XmlNodeType.Text:
				return CreateTextNode(node.Value);
			case XmlNodeType.Whitespace:
				return CreateWhitespace(node.Value);
			case XmlNodeType.XmlDeclaration:
			{
				XmlDeclaration xmlDeclaration = node as XmlDeclaration;
				return CreateXmlDeclaration(xmlDeclaration.Version, xmlDeclaration.Encoding, xmlDeclaration.Standalone);
			}
			default:
				throw new InvalidOperationException("Cannot import specified node type: " + node.NodeType);
			}
		}

		public virtual void Load(Stream inStream)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(inStream, NameTable);
			xmlTextReader.XmlResolver = resolver;
			XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
			xmlValidatingReader.EntityHandling = EntityHandling.ExpandCharEntities;
			xmlValidatingReader.ValidationType = ValidationType.None;
			Load(xmlValidatingReader);
		}

		public virtual void Load(string filename)
		{
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(filename, NameTable);
				xmlTextReader.XmlResolver = resolver;
				XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
				xmlValidatingReader.EntityHandling = EntityHandling.ExpandCharEntities;
				xmlValidatingReader.ValidationType = ValidationType.None;
				Load(xmlValidatingReader);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		public virtual void Load(TextReader txtReader)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(txtReader, NameTable);
			XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
			xmlValidatingReader.EntityHandling = EntityHandling.ExpandCharEntities;
			xmlValidatingReader.ValidationType = ValidationType.None;
			xmlTextReader.XmlResolver = resolver;
			Load(xmlValidatingReader);
		}

		public virtual void Load(XmlReader xmlReader)
		{
			RemoveAll();
			baseURI = xmlReader.BaseURI;
			try
			{
				loadMode = true;
				while (true)
				{
					XmlNode xmlNode = ReadNode(xmlReader);
					if (xmlNode == null)
					{
						break;
					}
					if (preserveWhitespace || xmlNode.NodeType != XmlNodeType.Whitespace)
					{
						AppendChild(xmlNode, false);
					}
				}
				if (xmlReader.Settings != null)
				{
					schemas = xmlReader.Settings.Schemas;
				}
			}
			finally
			{
				loadMode = false;
			}
		}

		public virtual void LoadXml(string xml)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(xml, XmlNodeType.Document, new XmlParserContext(NameTable, new XmlNamespaceManager(NameTable), null, XmlSpace.None));
			try
			{
				xmlTextReader.XmlResolver = resolver;
				Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		internal void onNodeChanged(XmlNode node, XmlNode parent, string oldValue, string newValue)
		{
			if (this.NodeChanged != null)
			{
				this.NodeChanged(node, new XmlNodeChangedEventArgs(node, parent, parent, oldValue, newValue, XmlNodeChangedAction.Change));
			}
		}

		internal void onNodeChanging(XmlNode node, XmlNode parent, string oldValue, string newValue)
		{
			if (node.IsReadOnly)
			{
				throw new ArgumentException("Node is read-only.");
			}
			if (this.NodeChanging != null)
			{
				this.NodeChanging(node, new XmlNodeChangedEventArgs(node, parent, parent, oldValue, newValue, XmlNodeChangedAction.Change));
			}
		}

		internal void onNodeInserted(XmlNode node, XmlNode newParent)
		{
			if (this.NodeInserted != null)
			{
				this.NodeInserted(node, new XmlNodeChangedEventArgs(node, null, newParent, null, null, XmlNodeChangedAction.Insert));
			}
		}

		internal void onNodeInserting(XmlNode node, XmlNode newParent)
		{
			if (this.NodeInserting != null)
			{
				this.NodeInserting(node, new XmlNodeChangedEventArgs(node, null, newParent, null, null, XmlNodeChangedAction.Insert));
			}
		}

		internal void onNodeRemoved(XmlNode node, XmlNode oldParent)
		{
			if (this.NodeRemoved != null)
			{
				this.NodeRemoved(node, new XmlNodeChangedEventArgs(node, oldParent, null, null, null, XmlNodeChangedAction.Remove));
			}
		}

		internal void onNodeRemoving(XmlNode node, XmlNode oldParent)
		{
			if (this.NodeRemoving != null)
			{
				this.NodeRemoving(node, new XmlNodeChangedEventArgs(node, oldParent, null, null, null, XmlNodeChangedAction.Remove));
			}
		}

		private void ParseName(string name, out string prefix, out string localName)
		{
			int num = name.IndexOf(':');
			if (num != -1)
			{
				prefix = name.Substring(0, num);
				localName = name.Substring(num + 1);
			}
			else
			{
				prefix = string.Empty;
				localName = name;
			}
		}

		private XmlAttribute ReadAttributeNode(XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				reader.MoveToFirstAttribute();
			}
			else if (reader.NodeType != XmlNodeType.Attribute)
			{
				throw new InvalidOperationException(MakeReaderErrorMessage("bad position to read attribute.", reader));
			}
			XmlAttribute xmlAttribute = CreateAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
			if (reader.SchemaInfo != null)
			{
				SchemaInfo = new XmlSchemaInfo(reader.SchemaInfo);
			}
			bool isDefault = reader.IsDefault;
			ReadAttributeNodeValue(reader, xmlAttribute);
			if (isDefault)
			{
				xmlAttribute.SetDefault();
			}
			return xmlAttribute;
		}

		internal void ReadAttributeNodeValue(XmlReader reader, XmlAttribute attribute)
		{
			while (reader.ReadAttributeValue())
			{
				if (reader.NodeType == XmlNodeType.EntityReference)
				{
					attribute.AppendChild(CreateEntityReference(reader.Name), false);
				}
				else
				{
					attribute.AppendChild(CreateTextNode(reader.Value), false);
				}
			}
		}

		public virtual XmlNode ReadNode(XmlReader reader)
		{
			if (PreserveWhitespace)
			{
				return ReadNodeCore(reader);
			}
			XmlTextReader xmlTextReader = reader as XmlTextReader;
			if (xmlTextReader != null && xmlTextReader.WhitespaceHandling == WhitespaceHandling.All)
			{
				try
				{
					xmlTextReader.WhitespaceHandling = WhitespaceHandling.Significant;
					return ReadNodeCore(reader);
				}
				finally
				{
					xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;
				}
			}
			return ReadNodeCore(reader);
		}

		private XmlNode ReadNodeCore(XmlReader reader)
		{
			switch (reader.ReadState)
			{
			case ReadState.Initial:
				if (reader.SchemaInfo != null)
				{
					SchemaInfo = new XmlSchemaInfo(reader.SchemaInfo);
				}
				reader.Read();
				break;
			default:
				return null;
			case ReadState.Interactive:
				break;
			}
			XmlNode xmlNode;
			switch (reader.NodeType)
			{
			case XmlNodeType.Attribute:
			{
				string localName = reader.LocalName;
				string namespaceURI = reader.NamespaceURI;
				xmlNode = ReadAttributeNode(reader);
				reader.MoveToAttribute(localName, namespaceURI);
				return xmlNode;
			}
			case XmlNodeType.CDATA:
				xmlNode = CreateCDataSection(reader.Value);
				break;
			case XmlNodeType.Comment:
				xmlNode = CreateComment(reader.Value);
				break;
			case XmlNodeType.Element:
			{
				XmlElement xmlElement = CreateElement(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.NameTable == NameTable);
				if (reader.SchemaInfo != null)
				{
					SchemaInfo = new XmlSchemaInfo(reader.SchemaInfo);
				}
				xmlElement.IsEmpty = reader.IsEmptyElement;
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					xmlElement.SetAttributeNode(ReadAttributeNode(reader));
					reader.MoveToElement();
				}
				reader.MoveToElement();
				int depth = reader.Depth;
				if (reader.IsEmptyElement)
				{
					xmlNode = xmlElement;
					break;
				}
				reader.Read();
				while (reader.Depth > depth)
				{
					xmlNode = ReadNodeCore(reader);
					if (preserveWhitespace || xmlNode.NodeType != XmlNodeType.Whitespace)
					{
						xmlElement.AppendChild(xmlNode, false);
					}
				}
				xmlNode = xmlElement;
				break;
			}
			case XmlNodeType.ProcessingInstruction:
				xmlNode = CreateProcessingInstruction(reader.Name, reader.Value);
				break;
			case XmlNodeType.Text:
				xmlNode = CreateTextNode(reader.Value);
				break;
			case XmlNodeType.XmlDeclaration:
				xmlNode = CreateXmlDeclaration("1.0", string.Empty, string.Empty);
				xmlNode.Value = reader.Value;
				break;
			case XmlNodeType.DocumentType:
			{
				DTDObjectModel dTDObjectModel = null;
				IHasXmlParserContext hasXmlParserContext = reader as IHasXmlParserContext;
				if (hasXmlParserContext != null)
				{
					dTDObjectModel = hasXmlParserContext.ParserContext.Dtd;
				}
				xmlNode = ((dTDObjectModel == null) ? CreateDocumentType(reader.Name, reader["PUBLIC"], reader["SYSTEM"], reader.Value) : CreateDocumentType(dTDObjectModel));
				break;
			}
			case XmlNodeType.EntityReference:
				if (loadMode && DocumentType != null && DocumentType.Entities.GetNamedItem(reader.Name) == null)
				{
					throw new XmlException("Reference to undeclared entity was found.");
				}
				xmlNode = CreateEntityReference(reader.Name);
				if (reader.CanResolveEntity)
				{
					reader.ResolveEntity();
					reader.Read();
					XmlNode newChild;
					while (reader.NodeType != XmlNodeType.EndEntity && (newChild = ReadNode(reader)) != null)
					{
						xmlNode.InsertBefore(newChild, null, false, false);
					}
				}
				break;
			case XmlNodeType.SignificantWhitespace:
				xmlNode = CreateSignificantWhitespace(reader.Value);
				break;
			case XmlNodeType.Whitespace:
				xmlNode = CreateWhitespace(reader.Value);
				break;
			case XmlNodeType.None:
				return null;
			default:
				throw new NullReferenceException(string.Concat("Unexpected node type ", reader.NodeType, "."));
			}
			reader.Read();
			return xmlNode;
		}

		private string MakeReaderErrorMessage(string message, XmlReader reader)
		{
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			if (xmlLineInfo != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} Line number = {1}, Inline position = {2}.", message, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
			}
			return message;
		}

		internal void RemoveIdenticalAttribute(string id)
		{
			idTable.Remove(id);
		}

		public virtual void Save(Stream outStream)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(outStream, TextEncoding);
			if (!PreserveWhitespace)
			{
				xmlTextWriter.Formatting = Formatting.Indented;
			}
			WriteContentTo(xmlTextWriter);
			xmlTextWriter.Flush();
		}

		public virtual void Save(string filename)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(filename, TextEncoding);
			try
			{
				if (!PreserveWhitespace)
				{
					xmlTextWriter.Formatting = Formatting.Indented;
				}
				WriteContentTo(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter.Close();
			}
		}

		public virtual void Save(TextWriter writer)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(writer);
			if (!PreserveWhitespace)
			{
				xmlTextWriter.Formatting = Formatting.Indented;
			}
			if (FirstChild != null && FirstChild.NodeType != XmlNodeType.XmlDeclaration)
			{
				xmlTextWriter.WriteStartDocument();
			}
			WriteContentTo(xmlTextWriter);
			xmlTextWriter.WriteEndDocument();
			xmlTextWriter.Flush();
		}

		public virtual void Save(XmlWriter xmlWriter)
		{
			bool flag = FirstChild != null && FirstChild.NodeType != XmlNodeType.XmlDeclaration;
			if (flag)
			{
				xmlWriter.WriteStartDocument();
			}
			WriteContentTo(xmlWriter);
			if (flag)
			{
				xmlWriter.WriteEndDocument();
			}
			xmlWriter.Flush();
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (XmlNode xmlNode = FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				xmlNode.WriteTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			WriteContentTo(w);
		}

		private void AddDefaultNameTableKeys()
		{
			nameTable.Add("#text");
			nameTable.Add("xml");
			nameTable.Add("xmlns");
			nameTable.Add("#entity");
			nameTable.Add("#document-fragment");
			nameTable.Add("#comment");
			nameTable.Add("space");
			nameTable.Add("id");
			nameTable.Add("#whitespace");
			nameTable.Add("http://www.w3.org/2000/xmlns/");
			nameTable.Add("#cdata-section");
			nameTable.Add("lang");
			nameTable.Add("#document");
			nameTable.Add("#significant-whitespace");
		}

		internal void CheckIdTableUpdate(XmlAttribute attr, string oldValue, string newValue)
		{
			if (idTable[oldValue] == attr)
			{
				idTable.Remove(oldValue);
				idTable[newValue] = attr;
			}
		}

		public void Validate(ValidationEventHandler handler)
		{
			Validate(handler, this, XmlSchemaValidationFlags.ProcessIdentityConstraints);
		}

		public void Validate(ValidationEventHandler handler, XmlNode node)
		{
			Validate(handler, node, XmlSchemaValidationFlags.ProcessIdentityConstraints);
		}

		private void Validate(ValidationEventHandler handler, XmlNode node, XmlSchemaValidationFlags flags)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.NameTable = NameTable;
			xmlReaderSettings.Schemas = schemas;
			xmlReaderSettings.Schemas.XmlResolver = resolver;
			xmlReaderSettings.XmlResolver = resolver;
			xmlReaderSettings.ValidationFlags = flags;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			XmlReader xmlReader = XmlReader.Create(new XmlNodeReader(node), xmlReaderSettings);
			while (!xmlReader.EOF)
			{
				xmlReader.Read();
			}
		}
	}
}
