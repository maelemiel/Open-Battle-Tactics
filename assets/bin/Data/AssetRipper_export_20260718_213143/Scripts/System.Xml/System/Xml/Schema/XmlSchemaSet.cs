using System.Collections;

namespace System.Xml.Schema
{
	public class XmlSchemaSet
	{
		private XmlNameTable nameTable;

		private XmlResolver xmlResolver = new XmlUrlResolver();

		private ArrayList schemas;

		private XmlSchemaObjectTable attributes;

		private XmlSchemaObjectTable elements;

		private XmlSchemaObjectTable types;

		private Hashtable idCollection;

		private XmlSchemaObjectTable namedIdentities;

		private XmlSchemaCompilationSettings settings = new XmlSchemaCompilationSettings();

		private bool isCompiled;

		internal Guid CompilationId;

		public int Count
		{
			get
			{
				return schemas.Count;
			}
		}

		public XmlSchemaObjectTable GlobalAttributes
		{
			get
			{
				if (attributes == null)
				{
					attributes = new XmlSchemaObjectTable();
				}
				return attributes;
			}
		}

		public XmlSchemaObjectTable GlobalElements
		{
			get
			{
				if (elements == null)
				{
					elements = new XmlSchemaObjectTable();
				}
				return elements;
			}
		}

		public XmlSchemaObjectTable GlobalTypes
		{
			get
			{
				if (types == null)
				{
					types = new XmlSchemaObjectTable();
				}
				return types;
			}
		}

		public bool IsCompiled
		{
			get
			{
				return isCompiled;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return nameTable;
			}
		}

		public XmlSchemaCompilationSettings CompilationSettings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = value;
			}
		}

		public XmlResolver XmlResolver
		{
			internal get
			{
				return xmlResolver;
			}
			set
			{
				xmlResolver = value;
			}
		}

		internal Hashtable IDCollection
		{
			get
			{
				if (idCollection == null)
				{
					idCollection = new Hashtable();
				}
				return idCollection;
			}
		}

		internal XmlSchemaObjectTable NamedIdentities
		{
			get
			{
				if (namedIdentities == null)
				{
					namedIdentities = new XmlSchemaObjectTable();
				}
				return namedIdentities;
			}
		}

		public event ValidationEventHandler ValidationEventHandler;

		public XmlSchemaSet()
			: this(new NameTable())
		{
		}

		public XmlSchemaSet(XmlNameTable nameTable)
		{
			if (nameTable == null)
			{
				throw new ArgumentNullException("nameTable");
			}
			this.nameTable = nameTable;
			schemas = new ArrayList();
			CompilationId = Guid.NewGuid();
		}

		public XmlSchema Add(string targetNamespace, string url)
		{
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(url, nameTable);
				return Add(targetNamespace, xmlTextReader);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		public XmlSchema Add(string targetNamespace, XmlReader reader)
		{
			XmlSchema xmlSchema = XmlSchema.Read(reader, this.ValidationEventHandler);
			if (xmlSchema.TargetNamespace == null)
			{
				xmlSchema.TargetNamespace = targetNamespace;
			}
			else if (targetNamespace != null && xmlSchema.TargetNamespace != targetNamespace)
			{
				throw new XmlSchemaException("The actual targetNamespace in the schema does not match the parameter.");
			}
			Add(xmlSchema);
			return xmlSchema;
		}

		[System.MonoTODO]
		public void Add(XmlSchemaSet schemaSet)
		{
			ArrayList arrayList = new ArrayList();
			foreach (XmlSchema schema2 in schemaSet.schemas)
			{
				if (!schemas.Contains(schema2))
				{
					arrayList.Add(schema2);
				}
			}
			foreach (XmlSchema item in arrayList)
			{
				Add(item);
			}
		}

		public XmlSchema Add(XmlSchema schema)
		{
			schemas.Add(schema);
			ResetCompile();
			return schema;
		}

		public void Compile()
		{
			ClearGlobalComponents();
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(schemas);
			IDCollection.Clear();
			NamedIdentities.Clear();
			Hashtable handledUris = new Hashtable();
			foreach (XmlSchema item in arrayList)
			{
				if (!item.IsCompiled)
				{
					item.CompileSubset(this.ValidationEventHandler, this, xmlResolver, handledUris);
				}
			}
			foreach (XmlSchema item2 in arrayList)
			{
				foreach (XmlSchemaElement value in item2.Elements.Values)
				{
					value.FillSubstitutionElementInfo();
				}
			}
			foreach (XmlSchema item3 in arrayList)
			{
				item3.Validate(this.ValidationEventHandler);
			}
			foreach (XmlSchema item4 in arrayList)
			{
				AddGlobalComponents(item4);
			}
			isCompiled = true;
		}

		private void ClearGlobalComponents()
		{
			GlobalElements.Clear();
			GlobalAttributes.Clear();
			GlobalTypes.Clear();
		}

		private void AddGlobalComponents(XmlSchema schema)
		{
			foreach (XmlSchemaElement value in schema.Elements.Values)
			{
				GlobalElements.Add(value.QualifiedName, value);
			}
			foreach (XmlSchemaAttribute value2 in schema.Attributes.Values)
			{
				GlobalAttributes.Add(value2.QualifiedName, value2);
			}
			foreach (XmlSchemaType value3 in schema.SchemaTypes.Values)
			{
				GlobalTypes.Add(value3.QualifiedName, value3);
			}
		}

		public bool Contains(string targetNamespace)
		{
			targetNamespace = GetSafeNs(targetNamespace);
			foreach (XmlSchema schema in schemas)
			{
				if (GetSafeNs(schema.TargetNamespace) == targetNamespace)
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(XmlSchema targetNamespace)
		{
			foreach (XmlSchema schema in schemas)
			{
				if (schema == targetNamespace)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(XmlSchema[] array, int index)
		{
			schemas.CopyTo(array, index);
		}

		internal void CopyTo(Array array, int index)
		{
			schemas.CopyTo(array, index);
		}

		private string GetSafeNs(string ns)
		{
			return (ns != null) ? ns : string.Empty;
		}

		[System.MonoTODO]
		public XmlSchema Remove(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(schemas);
			if (!arrayList.Contains(schema))
			{
				return null;
			}
			if (!schema.IsCompiled)
			{
				schema.CompileSubset(this.ValidationEventHandler, this, xmlResolver);
			}
			schemas.Remove(schema);
			ResetCompile();
			return schema;
		}

		private void ResetCompile()
		{
			isCompiled = false;
			ClearGlobalComponents();
		}

		public bool RemoveRecursive(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(schemas);
			if (!arrayList.Contains(schema))
			{
				return false;
			}
			arrayList.Remove(schema);
			schemas.Remove(schema);
			if (!IsCompiled)
			{
				return true;
			}
			ClearGlobalComponents();
			foreach (XmlSchema item in arrayList)
			{
				if (item.IsCompiled)
				{
					AddGlobalComponents(schema);
				}
			}
			return true;
		}

		public XmlSchema Reprocess(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(schemas);
			if (!arrayList.Contains(schema))
			{
				throw new ArgumentException("Target schema is not contained in the schema set.");
			}
			ClearGlobalComponents();
			foreach (XmlSchema item in arrayList)
			{
				if (schema == item)
				{
					schema.CompileSubset(this.ValidationEventHandler, this, xmlResolver);
				}
				if (item.IsCompiled)
				{
					AddGlobalComponents(schema);
				}
			}
			return (!schema.IsCompiled) ? null : schema;
		}

		public ICollection Schemas()
		{
			return schemas;
		}

		public ICollection Schemas(string targetNamespace)
		{
			targetNamespace = GetSafeNs(targetNamespace);
			ArrayList arrayList = new ArrayList();
			foreach (XmlSchema schema in schemas)
			{
				if (GetSafeNs(schema.TargetNamespace) == targetNamespace)
				{
					arrayList.Add(schema);
				}
			}
			return arrayList;
		}

		internal bool MissedSubComponents(string targetNamespace)
		{
			foreach (XmlSchema item in Schemas(targetNamespace))
			{
				if (item.missedSubComponents)
				{
					return true;
				}
			}
			return false;
		}
	}
}
