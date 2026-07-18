using System;
using System.Collections;
using System.IO;
using System.Xml;
using Mono.Xml2;

namespace Mono.Xml
{
	internal class DTDObjectModel
	{
		public const int AllowedExternalEntitiesMax = 256;

		private DTDAutomataFactory factory;

		private DTDElementAutomata rootAutomata;

		private DTDEmptyAutomata emptyAutomata;

		private DTDAnyAutomata anyAutomata;

		private DTDInvalidAutomata invalidAutomata;

		private DTDElementDeclarationCollection elementDecls;

		private DTDAttListDeclarationCollection attListDecls;

		private DTDParameterEntityDeclarationCollection peDecls;

		private DTDEntityDeclarationCollection entityDecls;

		private DTDNotationDeclarationCollection notationDecls;

		private ArrayList validationErrors;

		private XmlResolver resolver;

		private XmlNameTable nameTable;

		private Hashtable externalResources;

		private string baseURI;

		private string name;

		private string publicId;

		private string systemId;

		private string intSubset;

		private bool intSubsetHasPERef;

		private bool isStandalone;

		private int lineNumber;

		private int linePosition;

		public string BaseURI
		{
			get
			{
				return baseURI;
			}
			set
			{
				baseURI = value;
			}
		}

		public bool IsStandalone
		{
			get
			{
				return isStandalone;
			}
			set
			{
				isStandalone = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return nameTable;
			}
		}

		public string PublicId
		{
			get
			{
				return publicId;
			}
			set
			{
				publicId = value;
			}
		}

		public string SystemId
		{
			get
			{
				return systemId;
			}
			set
			{
				systemId = value;
			}
		}

		public string InternalSubset
		{
			get
			{
				return intSubset;
			}
			set
			{
				intSubset = value;
			}
		}

		public bool InternalSubsetHasPEReference
		{
			get
			{
				return intSubsetHasPERef;
			}
			set
			{
				intSubsetHasPERef = value;
			}
		}

		public int LineNumber
		{
			get
			{
				return lineNumber;
			}
			set
			{
				lineNumber = value;
			}
		}

		public int LinePosition
		{
			get
			{
				return linePosition;
			}
			set
			{
				linePosition = value;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return resolver;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				resolver = value;
			}
		}

		internal Hashtable ExternalResources
		{
			get
			{
				return externalResources;
			}
		}

		public DTDAutomataFactory Factory
		{
			get
			{
				return factory;
			}
		}

		public DTDElementDeclaration RootElement
		{
			get
			{
				return ElementDecls[Name];
			}
		}

		public DTDElementDeclarationCollection ElementDecls
		{
			get
			{
				return elementDecls;
			}
		}

		public DTDAttListDeclarationCollection AttListDecls
		{
			get
			{
				return attListDecls;
			}
		}

		public DTDEntityDeclarationCollection EntityDecls
		{
			get
			{
				return entityDecls;
			}
		}

		public DTDParameterEntityDeclarationCollection PEDecls
		{
			get
			{
				return peDecls;
			}
		}

		public DTDNotationDeclarationCollection NotationDecls
		{
			get
			{
				return notationDecls;
			}
		}

		public DTDAutomata RootAutomata
		{
			get
			{
				if (rootAutomata == null)
				{
					rootAutomata = new DTDElementAutomata(this, Name);
				}
				return rootAutomata;
			}
		}

		public DTDEmptyAutomata Empty
		{
			get
			{
				if (emptyAutomata == null)
				{
					emptyAutomata = new DTDEmptyAutomata(this);
				}
				return emptyAutomata;
			}
		}

		public DTDAnyAutomata Any
		{
			get
			{
				if (anyAutomata == null)
				{
					anyAutomata = new DTDAnyAutomata(this);
				}
				return anyAutomata;
			}
		}

		public DTDInvalidAutomata Invalid
		{
			get
			{
				if (invalidAutomata == null)
				{
					invalidAutomata = new DTDInvalidAutomata(this);
				}
				return invalidAutomata;
			}
		}

		public XmlException[] Errors
		{
			get
			{
				return validationErrors.ToArray(typeof(XmlException)) as XmlException[];
			}
		}

		public DTDObjectModel(XmlNameTable nameTable)
		{
			this.nameTable = nameTable;
			elementDecls = new DTDElementDeclarationCollection(this);
			attListDecls = new DTDAttListDeclarationCollection(this);
			entityDecls = new DTDEntityDeclarationCollection(this);
			peDecls = new DTDParameterEntityDeclarationCollection(this);
			notationDecls = new DTDNotationDeclarationCollection(this);
			factory = new DTDAutomataFactory(this);
			validationErrors = new ArrayList();
			externalResources = new Hashtable();
		}

		public string ResolveEntity(string name)
		{
			DTDEntityDeclaration dTDEntityDeclaration = EntityDecls[name];
			if (dTDEntityDeclaration == null)
			{
				AddError(new XmlException(string.Format("Required entity was not found: {0}", name), null, LineNumber, LinePosition));
				return " ";
			}
			return dTDEntityDeclaration.EntityValue;
		}

		public void AddError(XmlException ex)
		{
			validationErrors.Add(ex);
		}

		internal string GenerateEntityAttributeText(string entityName)
		{
			DTDEntityDeclaration dTDEntityDeclaration = EntityDecls[entityName];
			if (dTDEntityDeclaration == null)
			{
				return null;
			}
			return dTDEntityDeclaration.EntityValue;
		}

		internal Mono.Xml2.XmlTextReader GenerateEntityContentReader(string entityName, XmlParserContext context)
		{
			DTDEntityDeclaration dTDEntityDeclaration = EntityDecls[entityName];
			if (dTDEntityDeclaration == null)
			{
				return null;
			}
			if (dTDEntityDeclaration.SystemId != null)
			{
				Uri baseUri = ((!(dTDEntityDeclaration.BaseURI == string.Empty)) ? new Uri(dTDEntityDeclaration.BaseURI) : null);
				Stream xmlFragment = resolver.GetEntity(resolver.ResolveUri(baseUri, dTDEntityDeclaration.SystemId), null, typeof(Stream)) as Stream;
				return new Mono.Xml2.XmlTextReader(xmlFragment, XmlNodeType.Element, context);
			}
			return new Mono.Xml2.XmlTextReader(dTDEntityDeclaration.EntityValue, XmlNodeType.Element, context);
		}
	}
}
