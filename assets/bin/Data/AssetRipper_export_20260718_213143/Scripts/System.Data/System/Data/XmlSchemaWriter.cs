using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	internal class XmlSchemaWriter
	{
		private const string xmlnsxs = "http://www.w3.org/2001/XMLSchema";

		private XmlWriter w;

		private DataTable[] tables;

		private DataRelation[] relations;

		private string mainDataTable;

		private string dataSetName;

		private string dataSetNamespace;

		private PropertyCollection dataSetProperties;

		private CultureInfo dataSetLocale;

		private ArrayList globalTypeTables = new ArrayList();

		private Hashtable additionalNamespaces = new Hashtable();

		private ArrayList annotation = new ArrayList();

		public string ConstraintPrefix
		{
			get
			{
				return (!(dataSetNamespace != string.Empty)) ? string.Empty : ("mstns" + ':');
			}
		}

		public XmlSchemaWriter(DataSet dataset, XmlWriter writer, DataTableCollection tables, DataRelationCollection relations)
		{
			dataSetName = dataset.DataSetName;
			dataSetNamespace = dataset.Namespace;
			dataSetLocale = ((!dataset.LocaleSpecified) ? null : dataset.Locale);
			dataSetProperties = dataset.ExtendedProperties;
			w = writer;
			if (tables != null)
			{
				this.tables = new DataTable[tables.Count];
				for (int i = 0; i < tables.Count; i++)
				{
					this.tables[i] = tables[i];
				}
			}
			if (relations != null)
			{
				this.relations = new DataRelation[relations.Count];
				for (int j = 0; j < relations.Count; j++)
				{
					this.relations[j] = relations[j];
				}
			}
		}

		public XmlSchemaWriter(XmlWriter writer, DataTable[] tables, DataRelation[] relations, string mainDataTable, string dataSetName, CultureInfo locale)
		{
			w = writer;
			this.tables = tables;
			this.relations = relations;
			this.mainDataTable = mainDataTable;
			this.dataSetName = dataSetName;
			dataSetLocale = locale;
			dataSetProperties = new PropertyCollection();
			if (tables[0].DataSet != null)
			{
				dataSetNamespace = tables[0].DataSet.Namespace;
			}
			else
			{
				dataSetNamespace = tables[0].Namespace;
			}
		}

		public static void WriteXmlSchema(DataSet dataset, XmlWriter writer)
		{
			WriteXmlSchema(dataset, writer, dataset.Tables, dataset.Relations);
		}

		public static void WriteXmlSchema(DataSet dataset, XmlWriter writer, DataTableCollection tables, DataRelationCollection relations)
		{
			new XmlSchemaWriter(dataset, writer, tables, relations).WriteSchema();
		}

		internal static void WriteXmlSchema(XmlWriter writer, DataTable[] tables, DataRelation[] relations, string mainDataTable, string dataSetName, CultureInfo locale)
		{
			new XmlSchemaWriter(writer, tables, relations, mainDataTable, dataSetName, locale).WriteSchema();
		}

		public void WriteSchema()
		{
			ListDictionary listDictionary = new ListDictionary();
			ListDictionary listDictionary2 = new ListDictionary();
			DataTable[] array = tables;
			foreach (DataTable dataTable in array)
			{
				foreach (DataColumn column in dataTable.Columns)
				{
					CheckNamespace(column.Prefix, column.Namespace, listDictionary, listDictionary2);
				}
				CheckNamespace(dataTable.Prefix, dataTable.Namespace, listDictionary, listDictionary2);
			}
			w.WriteStartElement("xs", "schema", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("id", XmlHelper.Encode(dataSetName));
			if (dataSetNamespace != string.Empty)
			{
				w.WriteAttributeString("targetNamespace", dataSetNamespace);
				w.WriteAttributeString("xmlns", "mstns", "http://www.w3.org/2000/xmlns/", dataSetNamespace);
			}
			w.WriteAttributeString("xmlns", dataSetNamespace);
			w.WriteAttributeString("xmlns", "xs", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("xmlns", "msdata", "http://www.w3.org/2000/xmlns/", "urn:schemas-microsoft-com:xml-msdata");
			if (CheckExtendedPropertyExists(tables, relations))
			{
				w.WriteAttributeString("xmlns", "msprop", "http://www.w3.org/2000/xmlns/", "urn:schemas-microsoft-com:xml-msprop");
			}
			if (dataSetNamespace != string.Empty)
			{
				w.WriteAttributeString("attributeFormDefault", "qualified");
				w.WriteAttributeString("elementFormDefault", "qualified");
			}
			foreach (string key in listDictionary.Keys)
			{
				w.WriteAttributeString("xmlns", key, "http://www.w3.org/2000/xmlns/", listDictionary[key] as string);
			}
			if (listDictionary2.Count > 0)
			{
				w.WriteComment("ATTENTION: This schema contains references to other imported schemas");
			}
			foreach (string key2 in listDictionary2.Keys)
			{
				w.WriteStartElement("xs", "import", "http://www.w3.org/2001/XMLSchema");
				w.WriteAttributeString("namespace", key2);
				w.WriteAttributeString("schemaLocation", listDictionary2[key2] as string);
				w.WriteEndElement();
			}
			DataTable[] array2 = tables;
			foreach (DataTable dataTable2 in array2)
			{
				bool flag = true;
				foreach (DataRelation parentRelation in dataTable2.ParentRelations)
				{
					if (parentRelation.Nested)
					{
						flag = false;
						break;
					}
				}
				if (!flag && tables.Length < 2)
				{
					WriteTableElement(dataTable2);
				}
			}
			WriteDataSetElement();
			w.WriteEndElement();
			w.Flush();
		}

		private void WriteDataSetElement()
		{
			w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("name", XmlHelper.Encode(dataSetName));
			w.WriteAttributeString("msdata", "IsDataSet", "urn:schemas-microsoft-com:xml-msdata", "true");
			bool flag = dataSetLocale == null;
			if (!flag)
			{
				w.WriteAttributeString("msdata", "Locale", "urn:schemas-microsoft-com:xml-msdata", dataSetLocale.Name);
			}
			if (mainDataTable != null && mainDataTable != string.Empty)
			{
				w.WriteAttributeString("msdata", "MainDataTable", "urn:schemas-microsoft-com:xml-msdata", mainDataTable);
			}
			if (flag)
			{
				w.WriteAttributeString("msdata", "UseCurrentLocale", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			AddExtendedPropertyAttributes(dataSetProperties);
			w.WriteStartElement("xs", "complexType", "http://www.w3.org/2001/XMLSchema");
			w.WriteStartElement("xs", "choice", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("minOccurs", "0");
			w.WriteAttributeString("maxOccurs", "unbounded");
			DataTable[] array = tables;
			foreach (DataTable dataTable in array)
			{
				bool flag2 = true;
				foreach (DataRelation parentRelation in dataTable.ParentRelations)
				{
					if (parentRelation.Nested)
					{
						flag2 = false;
						break;
					}
				}
				bool flag3 = false;
				if (!flag2 && tables.Length < 2)
				{
					flag2 = true;
					flag3 = true;
				}
				if (flag2)
				{
					if (dataSetNamespace != dataTable.Namespace || flag3)
					{
						w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
						w.WriteStartAttribute("ref", string.Empty);
						w.WriteQualifiedName(XmlHelper.Encode(dataTable.TableName), dataTable.Namespace);
						w.WriteEndAttribute();
						w.WriteEndElement();
					}
					else
					{
						WriteTableElement(dataTable);
					}
				}
			}
			w.WriteEndElement();
			w.WriteEndElement();
			WriteConstraints();
			w.WriteEndElement();
			if (annotation.Count <= 0)
			{
				return;
			}
			w.WriteStartElement("xs", "annotation", "http://www.w3.org/2001/XMLSchema");
			w.WriteStartElement("xs", "appinfo", "http://www.w3.org/2001/XMLSchema");
			foreach (object item in annotation)
			{
				if (item is DataRelation)
				{
					WriteDataRelationAnnotation((DataRelation)item);
				}
			}
			w.WriteEndElement();
			w.WriteEndElement();
		}

		private void WriteDataRelationAnnotation(DataRelation rel)
		{
			string empty = string.Empty;
			w.WriteStartElement("msdata", "Relationship", "urn:schemas-microsoft-com:xml-msdata");
			w.WriteAttributeString("name", XmlHelper.Encode(rel.RelationName));
			w.WriteAttributeString("msdata", "parent", "urn:schemas-microsoft-com:xml-msdata", XmlHelper.Encode(rel.ParentTable.TableName));
			w.WriteAttributeString("msdata", "child", "urn:schemas-microsoft-com:xml-msdata", XmlHelper.Encode(rel.ChildTable.TableName));
			empty = string.Empty;
			DataColumn[] parentColumns = rel.ParentColumns;
			foreach (DataColumn dataColumn in parentColumns)
			{
				empty = empty + XmlHelper.Encode(dataColumn.ColumnName) + " ";
			}
			w.WriteAttributeString("msdata", "parentkey", "urn:schemas-microsoft-com:xml-msdata", empty.TrimEnd());
			empty = string.Empty;
			DataColumn[] childColumns = rel.ChildColumns;
			foreach (DataColumn dataColumn2 in childColumns)
			{
				empty = empty + XmlHelper.Encode(dataColumn2.ColumnName) + " ";
			}
			w.WriteAttributeString("msdata", "childkey", "urn:schemas-microsoft-com:xml-msdata", empty.TrimEnd());
			w.WriteEndElement();
		}

		private void WriteConstraints()
		{
			ArrayList names = new ArrayList();
			DataTable[] array = tables;
			foreach (DataTable dataTable in array)
			{
				foreach (Constraint constraint in dataTable.Constraints)
				{
					UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
					if (uniqueConstraint != null)
					{
						AddUniqueConstraints(uniqueConstraint, names);
						continue;
					}
					ForeignKeyConstraint foreignKeyConstraint = constraint as ForeignKeyConstraint;
					bool flag = false;
					if (relations != null)
					{
						DataRelation[] array2 = relations;
						foreach (DataRelation dataRelation in array2)
						{
							if (dataRelation.RelationName == foreignKeyConstraint.ConstraintName)
							{
								flag = true;
							}
						}
					}
					if (tables.Length > 1 && foreignKeyConstraint != null && !flag)
					{
						DataRelation rel = new DataRelation(foreignKeyConstraint.ConstraintName, foreignKeyConstraint.RelatedColumns, foreignKeyConstraint.Columns);
						AddForeignKeys(rel, names, true);
					}
				}
			}
			if (relations == null)
			{
				return;
			}
			DataRelation[] array3 = relations;
			foreach (DataRelation dataRelation2 in array3)
			{
				if (dataRelation2.ParentKeyConstraint == null || dataRelation2.ChildKeyConstraint == null)
				{
					annotation.Add(dataRelation2);
				}
				else
				{
					AddForeignKeys(dataRelation2, names, false);
				}
			}
		}

		private void AddUniqueConstraints(UniqueConstraint uniq, ArrayList names)
		{
			DataColumn[] columns = uniq.Columns;
			foreach (DataColumn dataColumn in columns)
			{
				if (dataColumn.ColumnMapping == MappingType.Hidden)
				{
					return;
				}
			}
			w.WriteStartElement("xs", "unique", "http://www.w3.org/2001/XMLSchema");
			string value;
			if (!names.Contains(uniq.ConstraintName))
			{
				value = XmlHelper.Encode(uniq.ConstraintName);
				w.WriteAttributeString("name", value);
			}
			else
			{
				value = XmlHelper.Encode(uniq.Table.TableName) + "_" + XmlHelper.Encode(uniq.ConstraintName);
				w.WriteAttributeString("name", value);
				w.WriteAttributeString("msdata", "ConstraintName", "urn:schemas-microsoft-com:xml-msdata", XmlHelper.Encode(uniq.ConstraintName));
			}
			names.Add(value);
			if (uniq.IsPrimaryKey)
			{
				w.WriteAttributeString("msdata", "PrimaryKey", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			AddExtendedPropertyAttributes(uniq.ExtendedProperties);
			w.WriteStartElement("xs", "selector", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("xpath", ".//" + ConstraintPrefix + XmlHelper.Encode(uniq.Table.TableName));
			w.WriteEndElement();
			DataColumn[] columns2 = uniq.Columns;
			foreach (DataColumn dataColumn2 in columns2)
			{
				w.WriteStartElement("xs", "field", "http://www.w3.org/2001/XMLSchema");
				w.WriteStartAttribute("xpath", string.Empty);
				if (dataColumn2.ColumnMapping == MappingType.Attribute)
				{
					w.WriteString("@");
				}
				w.WriteString(ConstraintPrefix);
				w.WriteString(XmlHelper.Encode(dataColumn2.ColumnName));
				w.WriteEndAttribute();
				w.WriteEndElement();
			}
			w.WriteEndElement();
		}

		private void AddForeignKeys(DataRelation rel, ArrayList names, bool isConstraintOnly)
		{
			DataColumn[] parentColumns = rel.ParentColumns;
			foreach (DataColumn dataColumn in parentColumns)
			{
				if (dataColumn.ColumnMapping == MappingType.Hidden)
				{
					return;
				}
			}
			DataColumn[] childColumns = rel.ChildColumns;
			foreach (DataColumn dataColumn2 in childColumns)
			{
				if (dataColumn2.ColumnMapping == MappingType.Hidden)
				{
					return;
				}
			}
			w.WriteStartElement("xs", "keyref", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("name", XmlHelper.Encode(rel.RelationName));
			UniqueConstraint uniqueConstraint = null;
			if (isConstraintOnly)
			{
				foreach (Constraint constraint in rel.ParentTable.Constraints)
				{
					uniqueConstraint = constraint as UniqueConstraint;
					if (uniqueConstraint == null || uniqueConstraint.Columns != rel.ParentColumns)
					{
						continue;
					}
					break;
				}
			}
			else
			{
				uniqueConstraint = rel.ParentKeyConstraint;
			}
			string text = XmlHelper.Encode(rel.ParentTable.TableName) + "_" + XmlHelper.Encode(uniqueConstraint.ConstraintName);
			if (names.Contains(text))
			{
				w.WriteStartAttribute("refer", string.Empty);
				w.WriteQualifiedName(text, dataSetNamespace);
				w.WriteEndAttribute();
			}
			else
			{
				w.WriteStartAttribute("refer", string.Empty);
				w.WriteQualifiedName(XmlHelper.Encode(uniqueConstraint.ConstraintName), dataSetNamespace);
				w.WriteEndAttribute();
			}
			if (isConstraintOnly)
			{
				w.WriteAttributeString("msdata", "ConstraintOnly", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			else if (rel.Nested)
			{
				w.WriteAttributeString("msdata", "IsNested", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			AddExtendedPropertyAttributes(uniqueConstraint.ExtendedProperties);
			w.WriteStartElement("xs", "selector", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("xpath", ".//" + ConstraintPrefix + XmlHelper.Encode(rel.ChildTable.TableName));
			w.WriteEndElement();
			DataColumn[] childColumns2 = rel.ChildColumns;
			foreach (DataColumn dataColumn3 in childColumns2)
			{
				w.WriteStartElement("xs", "field", "http://www.w3.org/2001/XMLSchema");
				w.WriteStartAttribute("xpath", string.Empty);
				if (dataColumn3.ColumnMapping == MappingType.Attribute)
				{
					w.WriteString("@");
				}
				w.WriteString(ConstraintPrefix);
				w.WriteString(XmlHelper.Encode(dataColumn3.ColumnName));
				w.WriteEndAttribute();
				w.WriteEndElement();
			}
			w.WriteEndElement();
		}

		private bool CheckExtendedPropertyExists(DataTable[] tables, DataRelation[] relations)
		{
			if (dataSetProperties.Count > 0)
			{
				return true;
			}
			foreach (DataTable dataTable in tables)
			{
				if (dataTable.ExtendedProperties.Count > 0)
				{
					return true;
				}
				foreach (DataColumn column in dataTable.Columns)
				{
					if (column.ExtendedProperties.Count > 0)
					{
						return true;
					}
				}
				foreach (Constraint constraint in dataTable.Constraints)
				{
					if (constraint.ExtendedProperties.Count > 0)
					{
						return true;
					}
				}
			}
			if (relations == null)
			{
				return false;
			}
			foreach (DataRelation dataRelation in relations)
			{
				if (dataRelation.ExtendedProperties.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		private void AddExtendedPropertyAttributes(PropertyCollection props)
		{
			foreach (DictionaryEntry prop in props)
			{
				w.WriteStartAttribute("msprop", XmlConvert.EncodeName(prop.Key.ToString()), "urn:schemas-microsoft-com:xml-msprop");
				if (prop.Value != null)
				{
					w.WriteString(DataSet.WriteObjectXml(prop.Value));
				}
				w.WriteEndAttribute();
			}
		}

		private void WriteTableElement(DataTable table)
		{
			w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("name", XmlHelper.Encode(table.TableName));
			AddExtendedPropertyAttributes(table.ExtendedProperties);
			WriteTableType(table);
			w.WriteEndElement();
		}

		private void WriteTableType(DataTable table)
		{
			ArrayList atts;
			ArrayList elements;
			DataColumn simple;
			DataSet.SplitColumns(table, out atts, out elements, out simple);
			w.WriteStartElement("xs", "complexType", "http://www.w3.org/2001/XMLSchema");
			if (simple != null)
			{
				w.WriteStartElement("xs", "simpleContent", "http://www.w3.org/2001/XMLSchema");
				w.WriteAttributeString("msdata", "ColumnName", "urn:schemas-microsoft-com:xml-msdata", XmlHelper.Encode(simple.ColumnName));
				w.WriteAttributeString("msdata", "Ordinal", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.ToString(simple.Ordinal));
				w.WriteStartElement("xs", "extension", "http://www.w3.org/2001/XMLSchema");
				w.WriteStartAttribute("base", string.Empty);
				WriteQName(MapType(simple.DataType));
				w.WriteEndAttribute();
				WriteTableAttributes(atts);
				w.WriteEndElement();
			}
			else
			{
				WriteTableAttributes(atts);
				bool flag = false;
				foreach (DataRelation parentRelation in table.ParentRelations)
				{
					if (parentRelation.Nested)
					{
						flag = true;
						break;
					}
				}
				if (elements.Count > 0 || (flag && tables.Length < 2))
				{
					w.WriteStartElement("xs", "sequence", "http://www.w3.org/2001/XMLSchema");
					foreach (DataColumn item in elements)
					{
						WriteTableTypeParticles(item);
					}
					foreach (DataRelation childRelation in table.ChildRelations)
					{
						if (childRelation.Nested)
						{
							WriteChildRelations(childRelation);
						}
					}
					w.WriteEndElement();
				}
			}
			w.WriteFullEndElement();
		}

		private void WriteTableTypeParticles(DataColumn col)
		{
			w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("name", XmlHelper.Encode(col.ColumnName));
			if (col.ColumnName != col.Caption && col.Caption != string.Empty)
			{
				w.WriteAttributeString("msdata", "Caption", "urn:schemas-microsoft-com:xml-msdata", col.Caption);
			}
			if (col.AutoIncrement)
			{
				w.WriteAttributeString("msdata", "AutoIncrement", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			if (col.AutoIncrementSeed != 0L)
			{
				w.WriteAttributeString("msdata", "AutoIncrementSeed", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.ToString(col.AutoIncrementSeed));
			}
			if (col.AutoIncrementStep != 1)
			{
				w.WriteAttributeString("msdata", "AutoIncrementStep", "urn:schemas-microsoft-com:xml-msdata", XmlConvert.ToString(col.AutoIncrementStep));
			}
			if (!DataColumn.GetDefaultValueForType(col.DataType).Equals(col.DefaultValue))
			{
				w.WriteAttributeString("default", DataSet.WriteObjectXml(col.DefaultValue));
			}
			if (col.ReadOnly)
			{
				w.WriteAttributeString("msdata", "ReadOnly", "urn:schemas-microsoft-com:xml-msdata", "true");
			}
			XmlQualifiedName xmlQualifiedName = null;
			if (col.MaxLength < 0)
			{
				w.WriteStartAttribute("type", string.Empty);
				xmlQualifiedName = MapType(col.DataType);
				WriteQName(xmlQualifiedName);
				w.WriteEndAttribute();
			}
			if (xmlQualifiedName == XmlConstants.QnString && col.DataType != typeof(string) && col.DataType != typeof(char))
			{
				w.WriteStartAttribute("msdata", "DataType", "urn:schemas-microsoft-com:xml-msdata");
				string assemblyQualifiedName = col.DataType.AssemblyQualifiedName;
				w.WriteString(assemblyQualifiedName);
				w.WriteEndAttribute();
			}
			if (col.AllowDBNull)
			{
				w.WriteAttributeString("minOccurs", "0");
			}
			if (col.MaxLength > -1)
			{
				WriteSimpleType(col);
			}
			AddExtendedPropertyAttributes(col.ExtendedProperties);
			w.WriteEndElement();
		}

		private void WriteChildRelations(DataRelation rel)
		{
			if (rel.ChildTable.Namespace != dataSetNamespace || tables.Length < 2)
			{
				w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
				w.WriteStartAttribute("ref", string.Empty);
				w.WriteQualifiedName(XmlHelper.Encode(rel.ChildTable.TableName), rel.ChildTable.Namespace);
				w.WriteEndAttribute();
			}
			else
			{
				w.WriteStartElement("xs", "element", "http://www.w3.org/2001/XMLSchema");
				w.WriteStartAttribute("name", string.Empty);
				w.WriteQualifiedName(XmlHelper.Encode(rel.ChildTable.TableName), rel.ChildTable.Namespace);
				w.WriteEndAttribute();
				w.WriteAttributeString("minOccurs", "0");
				w.WriteAttributeString("maxOccurs", "unbounded");
				globalTypeTables.Add(rel.ChildTable);
			}
			if (tables.Length > 1)
			{
				WriteTableType(rel.ChildTable);
			}
			w.WriteEndElement();
		}

		private void WriteTableAttributes(ArrayList atts)
		{
			foreach (DataColumn att in atts)
			{
				w.WriteStartElement("xs", "attribute", "http://www.w3.org/2001/XMLSchema");
				string text = XmlHelper.Encode(att.ColumnName);
				if (att.Namespace != string.Empty)
				{
					w.WriteAttributeString("form", "qualified");
					string text2 = ((!(att.Prefix == string.Empty)) ? att.Prefix : ("app" + additionalNamespaces.Count));
					text = text2 + ":" + text;
					additionalNamespaces[text2] = att.Namespace;
				}
				w.WriteAttributeString("name", text);
				AddExtendedPropertyAttributes(att.ExtendedProperties);
				if (att.ReadOnly)
				{
					w.WriteAttributeString("msdata", "ReadOnly", "urn:schemas-microsoft-com:xml-msdata", "true");
				}
				if (att.MaxLength < 0)
				{
					w.WriteStartAttribute("type", string.Empty);
					WriteQName(MapType(att.DataType));
					w.WriteEndAttribute();
				}
				if (!att.AllowDBNull)
				{
					w.WriteAttributeString("use", "required");
				}
				if (att.DefaultValue != DataColumn.GetDefaultValueForType(att.DataType))
				{
					w.WriteAttributeString("default", DataSet.WriteObjectXml(att.DefaultValue));
				}
				if (att.MaxLength > -1)
				{
					WriteSimpleType(att);
				}
				w.WriteEndElement();
			}
		}

		private void WriteSimpleType(DataColumn col)
		{
			w.WriteStartElement("xs", "simpleType", "http://www.w3.org/2001/XMLSchema");
			w.WriteStartElement("xs", "restriction", "http://www.w3.org/2001/XMLSchema");
			w.WriteStartAttribute("base", string.Empty);
			WriteQName(MapType(col.DataType));
			w.WriteEndAttribute();
			w.WriteStartElement("xs", "maxLength", "http://www.w3.org/2001/XMLSchema");
			w.WriteAttributeString("value", XmlConvert.ToString(col.MaxLength));
			w.WriteEndElement();
			w.WriteEndElement();
			w.WriteEndElement();
		}

		private void WriteQName(XmlQualifiedName name)
		{
			w.WriteQualifiedName(name.Name, name.Namespace);
		}

		private void CheckNamespace(string prefix, string ns, ListDictionary names, ListDictionary includes)
		{
			if (ns == string.Empty || !(dataSetNamespace != ns) || !((string)names[prefix] != ns))
			{
				return;
			}
			for (int i = 1; i < int.MaxValue; i++)
			{
				string text = "app" + i;
				if (names[text] == null)
				{
					names.Add(text, ns);
					HandleExternalNamespace(text, ns, includes);
					break;
				}
			}
		}

		private void HandleExternalNamespace(string prefix, string ns, ListDictionary includes)
		{
			if (!includes.Contains(ns))
			{
				includes.Add(ns, "_" + prefix + ".xsd");
			}
		}

		private XmlQualifiedName MapType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.String:
				return XmlConstants.QnString;
			case TypeCode.Int16:
				return XmlConstants.QnShort;
			case TypeCode.Int32:
				return XmlConstants.QnInt;
			case TypeCode.Int64:
				return XmlConstants.QnLong;
			case TypeCode.Boolean:
				return XmlConstants.QnBoolean;
			case TypeCode.Byte:
				return XmlConstants.QnUnsignedByte;
			case TypeCode.DateTime:
				return XmlConstants.QnDateTime;
			case TypeCode.Decimal:
				return XmlConstants.QnDecimal;
			case TypeCode.Double:
				return XmlConstants.QnDouble;
			case TypeCode.SByte:
				return XmlConstants.QnSbyte;
			case TypeCode.Single:
				return XmlConstants.QnFloat;
			case TypeCode.UInt16:
				return XmlConstants.QnUnsignedShort;
			case TypeCode.UInt32:
				return XmlConstants.QnUnsignedInt;
			case TypeCode.UInt64:
				return XmlConstants.QnUnsignedLong;
			default:
				if (typeof(TimeSpan) == type)
				{
					return XmlConstants.QnDuration;
				}
				if (typeof(Uri) == type)
				{
					return XmlConstants.QnUri;
				}
				if (typeof(byte[]) == type)
				{
					return XmlConstants.QnBase64Binary;
				}
				if (typeof(XmlQualifiedName) == type)
				{
					return XmlConstants.QnXmlQualifiedName;
				}
				return XmlConstants.QnString;
			}
		}
	}
}
