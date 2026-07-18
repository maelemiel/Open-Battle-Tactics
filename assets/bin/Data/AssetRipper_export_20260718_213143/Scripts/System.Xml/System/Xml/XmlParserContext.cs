using System.Collections;
using System.IO;
using System.Text;
using Mono.Xml;
using Mono.Xml2;

namespace System.Xml
{
	public class XmlParserContext
	{
		private class ContextItem
		{
			public string BaseURI;

			public string XmlLang;

			public XmlSpace XmlSpace;
		}

		private string baseURI = string.Empty;

		private string docTypeName = string.Empty;

		private Encoding encoding;

		private string internalSubset = string.Empty;

		private XmlNamespaceManager namespaceManager;

		private XmlNameTable nameTable;

		private string publicID = string.Empty;

		private string systemID = string.Empty;

		private string xmlLang = string.Empty;

		private XmlSpace xmlSpace;

		private ArrayList contextItems;

		private int contextItemCount;

		private DTDObjectModel dtd;

		public string BaseURI
		{
			get
			{
				return baseURI;
			}
			set
			{
				baseURI = ((value == null) ? string.Empty : value);
			}
		}

		public string DocTypeName
		{
			get
			{
				return (docTypeName != null) ? docTypeName : ((dtd == null) ? null : dtd.Name);
			}
			set
			{
				docTypeName = ((value == null) ? string.Empty : value);
			}
		}

		internal DTDObjectModel Dtd
		{
			get
			{
				return dtd;
			}
			set
			{
				dtd = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				encoding = value;
			}
		}

		public string InternalSubset
		{
			get
			{
				return (internalSubset != null) ? internalSubset : ((dtd == null) ? null : dtd.InternalSubset);
			}
			set
			{
				internalSubset = ((value == null) ? string.Empty : value);
			}
		}

		public XmlNamespaceManager NamespaceManager
		{
			get
			{
				return namespaceManager;
			}
			set
			{
				namespaceManager = value;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return nameTable;
			}
			set
			{
				nameTable = value;
			}
		}

		public string PublicId
		{
			get
			{
				return (publicID != null) ? publicID : ((dtd == null) ? null : dtd.PublicId);
			}
			set
			{
				publicID = ((value == null) ? string.Empty : value);
			}
		}

		public string SystemId
		{
			get
			{
				return (systemID != null) ? systemID : ((dtd == null) ? null : dtd.SystemId);
			}
			set
			{
				systemID = ((value == null) ? string.Empty : value);
			}
		}

		public string XmlLang
		{
			get
			{
				return xmlLang;
			}
			set
			{
				xmlLang = ((value == null) ? string.Empty : value);
			}
		}

		public XmlSpace XmlSpace
		{
			get
			{
				return xmlSpace;
			}
			set
			{
				xmlSpace = value;
			}
		}

		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string xmlLang, XmlSpace xmlSpace)
			: this(nt, nsMgr, null, null, null, null, null, xmlLang, xmlSpace, null)
		{
		}

		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string xmlLang, XmlSpace xmlSpace, Encoding enc)
			: this(nt, nsMgr, null, null, null, null, null, xmlLang, xmlSpace, enc)
		{
		}

		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string docTypeName, string pubId, string sysId, string internalSubset, string baseURI, string xmlLang, XmlSpace xmlSpace)
			: this(nt, nsMgr, docTypeName, pubId, sysId, internalSubset, baseURI, xmlLang, xmlSpace, null)
		{
		}

		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string docTypeName, string pubId, string sysId, string internalSubset, string baseURI, string xmlLang, XmlSpace xmlSpace, Encoding enc)
			: this(nt, nsMgr, (docTypeName == null || !(docTypeName != string.Empty)) ? null : new Mono.Xml2.XmlTextReader(TextReader.Null, nt).GenerateDTDObjectModel(docTypeName, pubId, sysId, internalSubset), baseURI, xmlLang, xmlSpace, enc)
		{
		}

		internal XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, DTDObjectModel dtd, string baseURI, string xmlLang, XmlSpace xmlSpace, Encoding enc)
		{
			namespaceManager = nsMgr;
			nameTable = ((nt != null) ? nt : ((nsMgr == null) ? null : nsMgr.NameTable));
			if (dtd != null)
			{
				DocTypeName = dtd.Name;
				PublicId = dtd.PublicId;
				SystemId = dtd.SystemId;
				InternalSubset = dtd.InternalSubset;
				this.dtd = dtd;
			}
			encoding = enc;
			BaseURI = baseURI;
			XmlLang = xmlLang;
			this.xmlSpace = xmlSpace;
			contextItems = new ArrayList();
		}

		internal void PushScope()
		{
			ContextItem contextItem = null;
			if (contextItems.Count == contextItemCount)
			{
				contextItem = new ContextItem();
				contextItems.Add(contextItem);
			}
			else
			{
				contextItem = (ContextItem)contextItems[contextItemCount];
			}
			contextItem.BaseURI = BaseURI;
			contextItem.XmlLang = XmlLang;
			contextItem.XmlSpace = XmlSpace;
			contextItemCount++;
		}

		internal void PopScope()
		{
			if (contextItemCount == 0)
			{
				throw new XmlException("Unexpected end of element scope.");
			}
			contextItemCount--;
			ContextItem contextItem = (ContextItem)contextItems[contextItemCount];
			baseURI = contextItem.BaseURI;
			xmlLang = contextItem.XmlLang;
			xmlSpace = contextItem.XmlSpace;
		}
	}
}
