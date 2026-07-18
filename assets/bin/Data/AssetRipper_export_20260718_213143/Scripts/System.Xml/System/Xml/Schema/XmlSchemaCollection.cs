using System.Collections;

namespace System.Xml.Schema
{
	[Obsolete("Use XmlSchemaSet.")]
	public sealed class XmlSchemaCollection : IEnumerable, ICollection
	{
		private XmlSchemaSet schemaSet;

		int ICollection.Count
		{
			get
			{
				return Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal XmlSchemaSet SchemaSet
		{
			get
			{
				return schemaSet;
			}
		}

		public int Count
		{
			get
			{
				return schemaSet.Count;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return schemaSet.NameTable;
			}
		}

		public XmlSchema this[string ns]
		{
			get
			{
				ICollection collection = schemaSet.Schemas(ns);
				if (collection == null)
				{
					return null;
				}
				IEnumerator enumerator = collection.GetEnumerator();
				if (enumerator.MoveNext())
				{
					return (XmlSchema)enumerator.Current;
				}
				return null;
			}
		}

		public event ValidationEventHandler ValidationEventHandler;

		public XmlSchemaCollection()
			: this(new NameTable())
		{
		}

		public XmlSchemaCollection(XmlNameTable nameTable)
			: this(new XmlSchemaSet(nameTable))
		{
			schemaSet.ValidationEventHandler += OnValidationError;
		}

		internal XmlSchemaCollection(XmlSchemaSet schemaSet)
		{
			this.schemaSet = schemaSet;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			lock (schemaSet)
			{
				schemaSet.CopyTo(array, index);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public XmlSchema Add(string ns, XmlReader reader)
		{
			return Add(ns, reader, new XmlUrlResolver());
		}

		public XmlSchema Add(string ns, XmlReader reader, XmlResolver resolver)
		{
			XmlSchema xmlSchema = XmlSchema.Read(reader, this.ValidationEventHandler);
			if (xmlSchema.TargetNamespace == null)
			{
				xmlSchema.TargetNamespace = ns;
			}
			else if (ns != null && xmlSchema.TargetNamespace != ns)
			{
				throw new XmlSchemaException("The actual targetNamespace in the schema does not match the parameter.");
			}
			return Add(xmlSchema);
		}

		public XmlSchema Add(string ns, string uri)
		{
			XmlReader xmlReader = new XmlTextReader(uri);
			try
			{
				return Add(ns, xmlReader);
			}
			finally
			{
				xmlReader.Close();
			}
		}

		public XmlSchema Add(XmlSchema schema)
		{
			return Add(schema, new XmlUrlResolver());
		}

		public XmlSchema Add(XmlSchema schema, XmlResolver resolver)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet(schemaSet.NameTable);
			xmlSchemaSet.Add(schemaSet);
			xmlSchemaSet.Add(schema);
			xmlSchemaSet.ValidationEventHandler += this.ValidationEventHandler;
			xmlSchemaSet.XmlResolver = resolver;
			xmlSchemaSet.Compile();
			if (!xmlSchemaSet.IsCompiled)
			{
				return null;
			}
			schemaSet = xmlSchemaSet;
			return schema;
		}

		public void Add(XmlSchemaCollection schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet(schemaSet.NameTable);
			xmlSchemaSet.Add(schemaSet);
			xmlSchemaSet.Add(schema.schemaSet);
			xmlSchemaSet.ValidationEventHandler += this.ValidationEventHandler;
			xmlSchemaSet.XmlResolver = schemaSet.XmlResolver;
			xmlSchemaSet.Compile();
			if (xmlSchemaSet.IsCompiled)
			{
				schemaSet = xmlSchemaSet;
			}
		}

		public bool Contains(string ns)
		{
			lock (schemaSet)
			{
				return schemaSet.Contains(ns);
			}
		}

		public bool Contains(XmlSchema schema)
		{
			lock (schemaSet)
			{
				return schemaSet.Contains(schema);
			}
		}

		public void CopyTo(XmlSchema[] array, int index)
		{
			lock (schemaSet)
			{
				schemaSet.CopyTo(array, index);
			}
		}

		public XmlSchemaCollectionEnumerator GetEnumerator()
		{
			return new XmlSchemaCollectionEnumerator(schemaSet.Schemas());
		}

		private void OnValidationError(object o, ValidationEventArgs e)
		{
			if (this.ValidationEventHandler != null)
			{
				this.ValidationEventHandler(o, e);
			}
			else if (e.Severity == XmlSeverityType.Error)
			{
				throw e.Exception;
			}
		}
	}
}
