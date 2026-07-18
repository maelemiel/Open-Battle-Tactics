using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlSchemas : CollectionBase, IEnumerable<XmlSchema>, IEnumerable
	{
		private static string msdataNS = "urn:schemas-microsoft-com:xml-msdata";

		private Hashtable table = new Hashtable();

		public XmlSchema this[int index]
		{
			get
			{
				if (index < 0 || index > Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				return (XmlSchema)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public XmlSchema this[string ns]
		{
			get
			{
				return (XmlSchema)table[(ns == null) ? string.Empty : ns];
			}
		}

		[System.MonoTODO]
		public bool IsCompiled
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		IEnumerator<XmlSchema> IEnumerable<XmlSchema>.GetEnumerator()
		{
			return new XmlSchemaEnumerator(this);
		}

		[System.MonoTODO]
		public void Compile(ValidationEventHandler handler, bool fullCompile)
		{
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlSchema xmlSchema = (XmlSchema)enumerator.Current;
					if (fullCompile || !xmlSchema.IsCompiled)
					{
						xmlSchema.Compile(handler);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public int Add(XmlSchema schema)
		{
			Insert(Count, schema);
			return Count - 1;
		}

		public void Add(XmlSchemas schemas)
		{
			foreach (XmlSchema schema in schemas)
			{
				Add(schema);
			}
		}

		[System.MonoNotSupported("")]
		public int Add(XmlSchema schema, Uri baseUri)
		{
			throw new NotImplementedException();
		}

		[System.MonoNotSupported("")]
		public void AddReference(XmlSchema schema)
		{
			throw new NotImplementedException();
		}

		public bool Contains(XmlSchema schema)
		{
			return base.List.Contains(schema);
		}

		[System.MonoNotSupported("")]
		public bool Contains(string targetNamespace)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(XmlSchema[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public object Find(XmlQualifiedName name, Type type)
		{
			XmlSchema xmlSchema = table[name.Namespace] as XmlSchema;
			if (xmlSchema == null)
			{
				{
					IEnumerator enumerator = GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							XmlSchema schema = (XmlSchema)enumerator.Current;
							object obj = Find(schema, name, type);
							if (obj != null)
							{
								return obj;
							}
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				return null;
			}
			object obj2 = Find(xmlSchema, name, type);
			if (obj2 == null)
			{
				{
					IEnumerator enumerator2 = GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							XmlSchema schema2 = (XmlSchema)enumerator2.Current;
							object obj3 = Find(schema2, name, type);
							if (obj3 != null)
							{
								return obj3;
							}
						}
					}
					finally
					{
						IDisposable disposable2 = enumerator2 as IDisposable;
						if (disposable2 != null)
						{
							disposable2.Dispose();
						}
					}
				}
			}
			return obj2;
		}

		private object Find(XmlSchema schema, XmlQualifiedName name, Type type)
		{
			if (!schema.IsCompiled)
			{
				schema.Compile(null);
			}
			XmlSchemaObjectTable xmlSchemaObjectTable = null;
			if (type == typeof(XmlSchemaSimpleType) || type == typeof(XmlSchemaComplexType))
			{
				xmlSchemaObjectTable = schema.SchemaTypes;
			}
			else if (type == typeof(XmlSchemaAttribute))
			{
				xmlSchemaObjectTable = schema.Attributes;
			}
			else if (type == typeof(XmlSchemaAttributeGroup))
			{
				xmlSchemaObjectTable = schema.AttributeGroups;
			}
			else if (type == typeof(XmlSchemaElement))
			{
				xmlSchemaObjectTable = schema.Elements;
			}
			else if (type == typeof(XmlSchemaGroup))
			{
				xmlSchemaObjectTable = schema.Groups;
			}
			else if (type == typeof(XmlSchemaNotation))
			{
				xmlSchemaObjectTable = schema.Notations;
			}
			object obj = ((xmlSchemaObjectTable == null) ? null : xmlSchemaObjectTable[name]);
			if (obj != null && obj.GetType() != type)
			{
				return null;
			}
			return obj;
		}

		[System.MonoNotSupported("")]
		public IList GetSchemas(string ns)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(XmlSchema schema)
		{
			return base.List.IndexOf(schema);
		}

		public void Insert(int index, XmlSchema schema)
		{
			base.List.Insert(index, schema);
		}

		public static bool IsDataSet(XmlSchema schema)
		{
			XmlSchemaElement xmlSchemaElement = ((schema.Items.Count != 1) ? null : (schema.Items[0] as XmlSchemaElement));
			if (xmlSchemaElement != null && xmlSchemaElement.UnhandledAttributes != null && xmlSchemaElement.UnhandledAttributes.Length > 0)
			{
				for (int i = 0; i < xmlSchemaElement.UnhandledAttributes.Length; i++)
				{
					XmlAttribute xmlAttribute = xmlSchemaElement.UnhandledAttributes[i];
					if (xmlAttribute.NamespaceURI == msdataNS && xmlAttribute.LocalName == "IsDataSet")
					{
						return xmlAttribute.Value.ToLower(CultureInfo.InvariantCulture) == "true";
					}
				}
			}
			return false;
		}

		protected override void OnClear()
		{
			table.Clear();
		}

		protected override void OnInsert(int index, object value)
		{
			string text = ((XmlSchema)value).TargetNamespace;
			if (text == null)
			{
				text = string.Empty;
			}
			table[text] = value;
		}

		protected override void OnRemove(int index, object value)
		{
			table.Remove(value);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			string text = ((XmlSchema)oldValue).TargetNamespace;
			if (text == null)
			{
				text = string.Empty;
			}
			table[text] = newValue;
		}

		public void Remove(XmlSchema schema)
		{
			base.List.Remove(schema);
		}
	}
}
