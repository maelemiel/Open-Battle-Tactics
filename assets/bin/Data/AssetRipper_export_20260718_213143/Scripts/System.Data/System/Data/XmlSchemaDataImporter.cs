using System.Collections;
using System.Data.Common;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace System.Data
{
	internal class XmlSchemaDataImporter
	{
		private static readonly XmlSchemaDatatype schemaIntegerType;

		private static readonly XmlSchemaDatatype schemaDecimalType;

		private static readonly XmlSchemaComplexType schemaAnyType;

		private DataSet dataset;

		private bool forDataSet;

		private XmlSchema schema;

		private ArrayList relations = new ArrayList();

		private Hashtable reservedConstraints = new Hashtable();

		private XmlSchemaElement datasetElement;

		private ArrayList topLevelElements = new ArrayList();

		private ArrayList targetElements = new ArrayList();

		private TableStructure currentTable;

		private TableAdapterSchemaInfo currentAdapter;

		internal TableAdapterSchemaInfo CurrentAdapter
		{
			get
			{
				return currentAdapter;
			}
		}

		public XmlSchemaDataImporter(DataSet dataset, XmlReader reader, bool forDataSet)
		{
			this.dataset = dataset;
			this.forDataSet = forDataSet;
			dataset.DataSetName = "NewDataSet";
			schema = XmlSchema.Read(reader, null);
			if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				reader.ReadEndElement();
			}
			schema.Compile(null);
		}

		static XmlSchemaDataImporter()
		{
			XmlSchema xmlSchema = new XmlSchema();
			XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute
			{
				Name = "foo",
				SchemaTypeName = new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema")
			};
			xmlSchema.Items.Add(xmlSchemaAttribute);
			XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute
			{
				Name = "bar",
				SchemaTypeName = new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema")
			};
			xmlSchema.Items.Add(xmlSchemaAttribute2);
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement
			{
				Name = "bar"
			};
			xmlSchema.Items.Add(xmlSchemaElement);
			xmlSchema.Compile(null);
			schemaIntegerType = xmlSchemaAttribute.AttributeSchemaType.Datatype;
			schemaDecimalType = xmlSchemaAttribute2.AttributeSchemaType.Datatype;
			schemaAnyType = xmlSchemaElement.ElementSchemaType as XmlSchemaComplexType;
		}

		public void Process()
		{
			if (schema.Id != null)
			{
				dataset.DataSetName = schema.Id;
			}
			dataset.Namespace = schema.TargetNamespace;
			foreach (XmlSchemaObject item in schema.Items)
			{
				XmlSchemaElement xmlSchemaElement = item as XmlSchemaElement;
				if (xmlSchemaElement != null)
				{
					if (datasetElement == null && IsDataSetElement(xmlSchemaElement))
					{
						datasetElement = xmlSchemaElement;
					}
					if (xmlSchemaElement.ElementSchemaType is XmlSchemaComplexType && xmlSchemaElement.ElementSchemaType != schemaAnyType)
					{
						targetElements.Add(item);
					}
				}
			}
			if (datasetElement != null)
			{
				foreach (XmlSchemaObject constraint in datasetElement.Constraints)
				{
					if (!(constraint is XmlSchemaKeyref))
					{
						ReserveSelfIdentity((XmlSchemaIdentityConstraint)constraint);
					}
				}
				foreach (XmlSchemaObject constraint2 in datasetElement.Constraints)
				{
					if (constraint2 is XmlSchemaKeyref)
					{
						ReserveRelationIdentity(datasetElement, (XmlSchemaKeyref)constraint2);
					}
				}
			}
			foreach (XmlSchemaObject item2 in schema.Items)
			{
				if (item2 is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement2 = item2 as XmlSchemaElement;
					if (xmlSchemaElement2.ElementSchemaType is XmlSchemaComplexType && xmlSchemaElement2.ElementSchemaType != schemaAnyType)
					{
						targetElements.Add(item2);
					}
				}
			}
			int count = targetElements.Count;
			for (int i = 0; i < count; i++)
			{
				ProcessGlobalElement((XmlSchemaElement)targetElements[i]);
			}
			for (int j = count; j < targetElements.Count; j++)
			{
				ProcessDataTableElement((XmlSchemaElement)targetElements[j]);
			}
			foreach (XmlSchemaObject item3 in schema.Items)
			{
				if (item3 is XmlSchemaAnnotation)
				{
					HandleAnnotations((XmlSchemaAnnotation)item3, false);
				}
			}
			if (datasetElement != null)
			{
				foreach (XmlSchemaObject constraint3 in datasetElement.Constraints)
				{
					if (!(constraint3 is XmlSchemaKeyref))
					{
						ProcessSelfIdentity(reservedConstraints[constraint3] as ConstraintStructure);
					}
				}
				foreach (XmlSchemaObject constraint4 in datasetElement.Constraints)
				{
					if (constraint4 is XmlSchemaKeyref)
					{
						ProcessRelationIdentity(datasetElement, reservedConstraints[constraint4] as ConstraintStructure);
					}
				}
			}
			foreach (RelationStructure relation in relations)
			{
				dataset.Relations.Add(GenerateRelationship(relation));
			}
		}

		private bool IsDataSetElement(XmlSchemaElement el)
		{
			if (el.UnhandledAttributes != null)
			{
				XmlAttribute[] unhandledAttributes = el.UnhandledAttributes;
				foreach (XmlAttribute xmlAttribute in unhandledAttributes)
				{
					if (xmlAttribute.LocalName == "IsDataSet" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						switch (xmlAttribute.Value)
						{
						case "true":
							return true;
						default:
							throw new DataException(string.Format("Value {0} is invalid for attribute 'IsDataSet'.", xmlAttribute.Value));
						case "false":
							break;
						}
					}
				}
			}
			if (schema.Elements.Count != 1)
			{
				return false;
			}
			if (!(el.SchemaType is XmlSchemaComplexType))
			{
				return false;
			}
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)el.SchemaType;
			if (xmlSchemaComplexType.AttributeUses.Count > 0)
			{
				return false;
			}
			XmlSchemaGroupBase xmlSchemaGroupBase = xmlSchemaComplexType.ContentTypeParticle as XmlSchemaGroupBase;
			if (xmlSchemaGroupBase == null || xmlSchemaGroupBase.Items.Count == 0)
			{
				return false;
			}
			foreach (XmlSchemaParticle item in xmlSchemaGroupBase.Items)
			{
				if (ContainsColumn(item))
				{
					return false;
				}
			}
			return true;
		}

		private bool ContainsColumn(XmlSchemaParticle p)
		{
			XmlSchemaElement xmlSchemaElement = p as XmlSchemaElement;
			if (xmlSchemaElement != null)
			{
				XmlSchemaComplexType xmlSchemaComplexType = null;
				xmlSchemaComplexType = xmlSchemaElement.ElementSchemaType as XmlSchemaComplexType;
				if (xmlSchemaComplexType == null || xmlSchemaComplexType == schemaAnyType)
				{
					return true;
				}
				if (xmlSchemaComplexType.AttributeUses.Count > 0)
				{
					return false;
				}
				if (xmlSchemaComplexType.ContentType == XmlSchemaContentType.TextOnly)
				{
					return true;
				}
				return false;
			}
			XmlSchemaGroupBase xmlSchemaGroupBase = p as XmlSchemaGroupBase;
			for (int i = 0; i < xmlSchemaGroupBase.Items.Count; i++)
			{
				if (ContainsColumn((XmlSchemaParticle)xmlSchemaGroupBase.Items[i]))
				{
					return true;
				}
			}
			return false;
		}

		private void ProcessGlobalElement(XmlSchemaElement el)
		{
			if (!dataset.Tables.Contains(el.QualifiedName.Name) && el.ElementSchemaType is XmlSchemaComplexType && el.ElementSchemaType != schemaAnyType)
			{
				if (IsDataSetElement(el))
				{
					ProcessDataSetElement(el);
					return;
				}
				dataset.Locale = CultureInfo.CurrentCulture;
				topLevelElements.Add(el);
				ProcessDataTableElement(el);
			}
		}

		private void ProcessDataSetElement(XmlSchemaElement el)
		{
			dataset.DataSetName = el.Name;
			datasetElement = el;
			bool flag = false;
			if (el.UnhandledAttributes != null)
			{
				XmlAttribute[] unhandledAttributes = el.UnhandledAttributes;
				foreach (XmlAttribute xmlAttribute in unhandledAttributes)
				{
					if (xmlAttribute.LocalName == "UseCurrentLocale" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						flag = true;
					}
					if (xmlAttribute.LocalName == "Locale" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						CultureInfo locale = new CultureInfo(xmlAttribute.Value);
						dataset.Locale = locale;
					}
				}
			}
			if (!flag && !dataset.LocaleSpecified)
			{
				dataset.Locale = CultureInfo.CurrentCulture;
			}
			XmlSchemaComplexType xmlSchemaComplexType = null;
			xmlSchemaComplexType = el.ElementSchemaType as XmlSchemaComplexType;
			XmlSchemaParticle xmlSchemaParticle = ((xmlSchemaComplexType == null) ? null : xmlSchemaComplexType.ContentTypeParticle);
			if (xmlSchemaParticle != null)
			{
				HandleDataSetContentTypeParticle(xmlSchemaParticle);
			}
		}

		private void HandleDataSetContentTypeParticle(XmlSchemaParticle p)
		{
			XmlSchemaElement xmlSchemaElement = p as XmlSchemaElement;
			if (xmlSchemaElement != null)
			{
				if (xmlSchemaElement.ElementSchemaType is XmlSchemaComplexType && xmlSchemaElement.RefName != xmlSchemaElement.QualifiedName)
				{
					ProcessDataTableElement(xmlSchemaElement);
				}
			}
			else
			{
				if (!(p is XmlSchemaGroupBase))
				{
					return;
				}
				foreach (XmlSchemaParticle item in ((XmlSchemaGroupBase)p).Items)
				{
					HandleDataSetContentTypeParticle(item);
				}
			}
		}

		private void ProcessDataTableElement(XmlSchemaElement el)
		{
			string text = XmlHelper.Decode(el.QualifiedName.Name);
			if (dataset.Tables.Contains(text))
			{
				return;
			}
			DataTable dataTable = new DataTable(text);
			dataTable.Namespace = el.QualifiedName.Namespace;
			TableStructure tableStructure = currentTable;
			currentTable = new TableStructure(dataTable);
			dataset.Tables.Add(dataTable);
			if (el.UnhandledAttributes != null)
			{
				XmlAttribute[] unhandledAttributes = el.UnhandledAttributes;
				foreach (XmlAttribute xmlAttribute in unhandledAttributes)
				{
					if (xmlAttribute.LocalName == "Locale" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						dataTable.Locale = new CultureInfo(xmlAttribute.Value);
					}
				}
			}
			XmlSchemaComplexType xmlSchemaComplexType = null;
			xmlSchemaComplexType = (XmlSchemaComplexType)el.ElementSchemaType;
			foreach (DictionaryEntry attributeUse in xmlSchemaComplexType.AttributeUses)
			{
				ImportColumnAttribute((XmlSchemaAttribute)attributeUse.Value);
			}
			if (xmlSchemaComplexType.ContentTypeParticle is XmlSchemaElement)
			{
				ImportColumnElement(el, (XmlSchemaElement)xmlSchemaComplexType.ContentTypeParticle);
			}
			else if (xmlSchemaComplexType.ContentTypeParticle is XmlSchemaGroupBase)
			{
				ImportColumnGroupBase(el, (XmlSchemaGroupBase)xmlSchemaComplexType.ContentTypeParticle);
			}
			if (xmlSchemaComplexType.ContentType == XmlSchemaContentType.TextOnly)
			{
				string columnName = el.QualifiedName.Name + "_text";
				DataColumn dataColumn = new DataColumn(columnName);
				dataColumn.Namespace = el.QualifiedName.Namespace;
				dataColumn.AllowDBNull = el.MinOccurs == 0m;
				dataColumn.ColumnMapping = MappingType.SimpleContent;
				dataColumn.DataType = ConvertDatatype(xmlSchemaComplexType.Datatype);
				currentTable.NonOrdinalColumns.Add(dataColumn);
			}
			SortedList sortedList = new SortedList();
			foreach (DictionaryEntry ordinalColumn in currentTable.OrdinalColumns)
			{
				sortedList.Add(ordinalColumn.Value, ordinalColumn.Key);
			}
			foreach (DictionaryEntry item in sortedList)
			{
				dataTable.Columns.Add((DataColumn)item.Value);
			}
			foreach (DataColumn nonOrdinalColumn in currentTable.NonOrdinalColumns)
			{
				dataTable.Columns.Add(nonOrdinalColumn);
			}
			currentTable = tableStructure;
		}

		private DataRelation GenerateRelationship(RelationStructure rs)
		{
			DataTable dataTable = dataset.Tables[rs.ParentTableName];
			DataTable dataTable2 = dataset.Tables[rs.ChildTableName];
			string relationName = ((rs.ExplicitName == null) ? (XmlHelper.Decode(dataTable.TableName) + '_' + XmlHelper.Decode(dataTable2.TableName)) : rs.ExplicitName);
			DataRelation dataRelation;
			if (datasetElement != null)
			{
				string[] array = rs.ParentColumnName.Split(null);
				string[] array2 = rs.ChildColumnName.Split(null);
				DataColumn[] array3 = new DataColumn[array.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i] = dataTable.Columns[XmlHelper.Decode(array[i])];
				}
				DataColumn[] array4 = new DataColumn[array2.Length];
				for (int j = 0; j < array4.Length; j++)
				{
					array4[j] = dataTable2.Columns[XmlHelper.Decode(array2[j])];
					if (array4[j] == null)
					{
						array4[j] = CreateChildColumn(array3[j], dataTable2);
					}
				}
				dataRelation = new DataRelation(relationName, array3, array4, rs.CreateConstraint);
			}
			else
			{
				DataColumn parentColumn = dataTable.Columns[XmlHelper.Decode(rs.ParentColumnName)];
				DataColumn dataColumn = dataTable2.Columns[XmlHelper.Decode(rs.ChildColumnName)];
				if (dataColumn == null)
				{
					dataColumn = CreateChildColumn(parentColumn, dataTable2);
				}
				dataRelation = new DataRelation(relationName, parentColumn, dataColumn, rs.CreateConstraint);
			}
			dataRelation.Nested = rs.IsNested;
			if (rs.CreateConstraint)
			{
				dataRelation.ParentTable.PrimaryKey = dataRelation.ParentColumns;
			}
			return dataRelation;
		}

		private DataColumn CreateChildColumn(DataColumn parentColumn, DataTable childTable)
		{
			DataColumn dataColumn = childTable.Columns.Add(parentColumn.ColumnName, parentColumn.DataType);
			dataColumn.Namespace = string.Empty;
			dataColumn.ColumnMapping = MappingType.Hidden;
			return dataColumn;
		}

		private void ImportColumnGroupBase(XmlSchemaElement parent, XmlSchemaGroupBase gb)
		{
			foreach (XmlSchemaParticle item in gb.Items)
			{
				XmlSchemaElement xmlSchemaElement = item as XmlSchemaElement;
				if (xmlSchemaElement != null)
				{
					ImportColumnElement(parent, xmlSchemaElement);
				}
				else if (item is XmlSchemaGroupBase)
				{
					ImportColumnGroupBase(parent, (XmlSchemaGroupBase)item);
				}
			}
		}

		private XmlSchemaDatatype GetSchemaPrimitiveType(object type)
		{
			if (type is XmlSchemaComplexType)
			{
				return null;
			}
			XmlSchemaDatatype xmlSchemaDatatype = type as XmlSchemaDatatype;
			if (xmlSchemaDatatype == null && type != null)
			{
				xmlSchemaDatatype = ((XmlSchemaSimpleType)type).Datatype;
			}
			return xmlSchemaDatatype;
		}

		private void ImportColumnAttribute(XmlSchemaAttribute attr)
		{
			DataColumn dataColumn = new DataColumn();
			dataColumn.ColumnName = attr.QualifiedName.Name;
			dataColumn.Namespace = attr.QualifiedName.Namespace;
			XmlSchemaDatatype xmlSchemaDatatype = null;
			xmlSchemaDatatype = GetSchemaPrimitiveType(attr.AttributeSchemaType.Datatype);
			dataColumn.DataType = ConvertDatatype(xmlSchemaDatatype);
			if (dataColumn.DataType == typeof(object))
			{
				dataColumn.DataType = typeof(string);
			}
			if (attr.Use == XmlSchemaUse.Prohibited)
			{
				dataColumn.ColumnMapping = MappingType.Hidden;
			}
			else
			{
				dataColumn.ColumnMapping = MappingType.Attribute;
				dataColumn.DefaultValue = GetAttributeDefaultValue(attr);
			}
			if (attr.Use == XmlSchemaUse.Required)
			{
				dataColumn.AllowDBNull = false;
			}
			FillFacet(dataColumn, attr.AttributeSchemaType);
			ImportColumnMetaInfo(attr, attr.QualifiedName, dataColumn);
			AddColumn(dataColumn);
		}

		private void ImportColumnElement(XmlSchemaElement parent, XmlSchemaElement el)
		{
			DataColumn dataColumn = new DataColumn();
			dataColumn.DefaultValue = GetElementDefaultValue(el);
			dataColumn.AllowDBNull = el.MinOccurs == 0m;
			if (el.ElementSchemaType is XmlSchemaComplexType && el.ElementSchemaType != schemaAnyType)
			{
				FillDataColumnComplexElement(parent, el, dataColumn);
			}
			else if (el.MaxOccurs != 1m)
			{
				FillDataColumnRepeatedSimpleElement(parent, el, dataColumn);
			}
			else
			{
				FillDataColumnSimpleElement(el, dataColumn);
			}
		}

		private void ImportColumnMetaInfo(XmlSchemaAnnotated obj, XmlQualifiedName name, DataColumn col)
		{
			if (obj.UnhandledAttributes == null)
			{
				return;
			}
			XmlAttribute[] unhandledAttributes = obj.UnhandledAttributes;
			foreach (XmlAttribute xmlAttribute in unhandledAttributes)
			{
				if (!(xmlAttribute.NamespaceURI != "urn:schemas-microsoft-com:xml-msdata"))
				{
					switch (xmlAttribute.LocalName)
					{
					case "Caption":
						col.Caption = xmlAttribute.Value;
						break;
					case "DataType":
						col.DataType = Type.GetType(xmlAttribute.Value);
						break;
					case "AutoIncrement":
						col.AutoIncrement = bool.Parse(xmlAttribute.Value);
						break;
					case "AutoIncrementSeed":
						col.AutoIncrementSeed = int.Parse(xmlAttribute.Value);
						break;
					case "AutoIncrementStep":
						col.AutoIncrementStep = int.Parse(xmlAttribute.Value);
						break;
					case "ReadOnly":
						col.ReadOnly = XmlConvert.ToBoolean(xmlAttribute.Value);
						break;
					case "Ordinal":
					{
						int num = int.Parse(xmlAttribute.Value);
						break;
					}
					}
				}
			}
		}

		private void FillDataColumnComplexElement(XmlSchemaElement parent, XmlSchemaElement el, DataColumn col)
		{
			if (!targetElements.Contains(el))
			{
				string text = XmlHelper.Decode(el.QualifiedName.Name);
				if (text == dataset.DataSetName)
				{
					throw new ArgumentException("Nested element must not have the same name as DataSet's name.");
				}
				if (el.Annotation != null)
				{
					HandleAnnotations(el.Annotation, true);
				}
				else if (!DataSetDefinesKey(text))
				{
					AddParentKeyColumn(parent, el, col);
					RelationStructure relationStructure = new RelationStructure();
					relationStructure.ParentTableName = XmlHelper.Decode(parent.QualifiedName.Name);
					relationStructure.ChildTableName = text;
					relationStructure.ParentColumnName = col.ColumnName;
					relationStructure.ChildColumnName = col.ColumnName;
					relationStructure.CreateConstraint = true;
					relationStructure.IsNested = true;
					relations.Add(relationStructure);
				}
				if (el.RefName == XmlQualifiedName.Empty)
				{
					ProcessDataTableElement(el);
				}
			}
		}

		private bool DataSetDefinesKey(string name)
		{
			foreach (ConstraintStructure value in reservedConstraints.Values)
			{
				if (value.TableName == name && (value.IsPrimaryKey || value.IsNested))
				{
					return true;
				}
			}
			return false;
		}

		private void AddParentKeyColumn(XmlSchemaElement parent, XmlSchemaElement el, DataColumn col)
		{
			if (currentTable.Table.PrimaryKey.Length > 0)
			{
				throw new DataException(string.Format("There is already primary key columns in the table \"{0}\".", currentTable.Table.TableName));
			}
			if (currentTable.PrimaryKey != null)
			{
				col.ColumnName = currentTable.PrimaryKey.ColumnName;
				col.ColumnMapping = currentTable.PrimaryKey.ColumnMapping;
				col.Namespace = currentTable.PrimaryKey.Namespace;
				col.DataType = currentTable.PrimaryKey.DataType;
				col.AutoIncrement = currentTable.PrimaryKey.AutoIncrement;
				col.AllowDBNull = currentTable.PrimaryKey.AllowDBNull;
				ImportColumnMetaInfo(el, el.QualifiedName, col);
				return;
			}
			string text = XmlHelper.Decode(parent.QualifiedName.Name) + "_Id";
			int num = 0;
			while (currentTable.ContainsColumn(text))
			{
				text = string.Format("{0}_{1}", text, num++);
			}
			col.ColumnName = text;
			col.ColumnMapping = MappingType.Hidden;
			col.Namespace = parent.QualifiedName.Namespace;
			col.DataType = typeof(int);
			col.AutoIncrement = true;
			col.AllowDBNull = false;
			ImportColumnMetaInfo(el, el.QualifiedName, col);
			AddColumn(col);
			currentTable.PrimaryKey = col;
		}

		private void FillDataColumnRepeatedSimpleElement(XmlSchemaElement parent, XmlSchemaElement el, DataColumn col)
		{
			if (!targetElements.Contains(el))
			{
				AddParentKeyColumn(parent, el, col);
				DataColumn primaryKey = currentTable.PrimaryKey;
				string text = XmlHelper.Decode(el.QualifiedName.Name);
				string text2 = XmlHelper.Decode(parent.QualifiedName.Name);
				DataTable dataTable = new DataTable();
				dataTable.TableName = text;
				dataTable.Namespace = el.QualifiedName.Namespace;
				DataColumn dataColumn = new DataColumn();
				dataColumn.ColumnName = text2 + "_Id";
				dataColumn.Namespace = parent.QualifiedName.Namespace;
				dataColumn.ColumnMapping = MappingType.Hidden;
				dataColumn.DataType = typeof(int);
				DataColumn dataColumn2 = new DataColumn();
				dataColumn2.ColumnName = text + "_Column";
				dataColumn2.Namespace = el.QualifiedName.Namespace;
				dataColumn2.ColumnMapping = MappingType.SimpleContent;
				dataColumn2.AllowDBNull = false;
				dataColumn2.DataType = ConvertDatatype(GetSchemaPrimitiveType(el.ElementSchemaType));
				dataTable.Columns.Add(dataColumn2);
				dataTable.Columns.Add(dataColumn);
				dataset.Tables.Add(dataTable);
				RelationStructure relationStructure = new RelationStructure();
				relationStructure.ParentTableName = text2;
				relationStructure.ChildTableName = dataTable.TableName;
				relationStructure.ParentColumnName = primaryKey.ColumnName;
				relationStructure.ChildColumnName = dataColumn.ColumnName;
				relationStructure.IsNested = true;
				relationStructure.CreateConstraint = true;
				relations.Add(relationStructure);
			}
		}

		private void FillDataColumnSimpleElement(XmlSchemaElement el, DataColumn col)
		{
			col.ColumnName = XmlHelper.Decode(el.QualifiedName.Name);
			col.Namespace = el.QualifiedName.Namespace;
			col.ColumnMapping = MappingType.Element;
			col.DataType = ConvertDatatype(GetSchemaPrimitiveType(el.ElementSchemaType));
			FillFacet(col, el.ElementSchemaType as XmlSchemaSimpleType);
			ImportColumnMetaInfo(el, el.QualifiedName, col);
			AddColumn(col);
		}

		private void AddColumn(DataColumn col)
		{
			if (col.Ordinal < 0)
			{
				currentTable.NonOrdinalColumns.Add(col);
			}
			else
			{
				currentTable.OrdinalColumns.Add(col, col.Ordinal);
			}
		}

		private void FillFacet(DataColumn col, XmlSchemaSimpleType st)
		{
			if (st == null || st.Content == null)
			{
				return;
			}
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = ((st != null) ? (st.Content as XmlSchemaSimpleTypeRestriction) : null);
			if (xmlSchemaSimpleTypeRestriction == null)
			{
				throw new DataException("DataSet does not suport 'list' nor 'union' simple type.");
			}
			foreach (XmlSchemaFacet facet in xmlSchemaSimpleTypeRestriction.Facets)
			{
				if (facet is XmlSchemaMaxLengthFacet)
				{
					col.MaxLength = int.Parse(facet.Value);
				}
			}
		}

		private Type ConvertDatatype(XmlSchemaDatatype dt)
		{
			if (dt == null)
			{
				return typeof(string);
			}
			if (dt.ValueType == typeof(decimal))
			{
				if (dt == schemaDecimalType)
				{
					return typeof(decimal);
				}
				if (dt == schemaIntegerType)
				{
					return typeof(long);
				}
				return typeof(ulong);
			}
			return dt.ValueType;
		}

		private string GetSelectorTarget(string xpath)
		{
			string text = xpath;
			int num = text.LastIndexOf('/');
			if (num > 0)
			{
				text = text.Substring(num + 1);
			}
			num = text.LastIndexOf(':');
			if (num > 0)
			{
				text = text.Substring(num + 1);
			}
			return XmlHelper.Decode(text);
		}

		private void ReserveSelfIdentity(XmlSchemaIdentityConstraint ic)
		{
			string selectorTarget = GetSelectorTarget(ic.Selector.XPath);
			string[] array = new string[ic.Fields.Count];
			bool[] array2 = new bool[array.Length];
			int num = 0;
			foreach (XmlSchemaXPath field in ic.Fields)
			{
				string text = field.XPath;
				bool flag = text.Length > 0 && text[0] == '@';
				int num2 = text.LastIndexOf(':');
				if (num2 > 0)
				{
					text = text.Substring(num2 + 1);
				}
				else if (flag)
				{
					text = text.Substring(1);
				}
				text = XmlHelper.Decode(text);
				array[num] = text;
				array2[num] = flag;
				num++;
			}
			bool isPK = false;
			string cname = ic.Name;
			if (ic.UnhandledAttributes != null)
			{
				XmlAttribute[] unhandledAttributes = ic.UnhandledAttributes;
				foreach (XmlAttribute xmlAttribute in unhandledAttributes)
				{
					if (!(xmlAttribute.NamespaceURI != "urn:schemas-microsoft-com:xml-msdata"))
					{
						switch (xmlAttribute.LocalName)
						{
						case "ConstraintName":
							cname = xmlAttribute.Value;
							break;
						case "PrimaryKey":
							isPK = bool.Parse(xmlAttribute.Value);
							break;
						}
					}
				}
			}
			reservedConstraints.Add(ic, new ConstraintStructure(selectorTarget, array, array2, cname, isPK, null, false, false));
		}

		private void ProcessSelfIdentity(ConstraintStructure c)
		{
			string tableName = c.TableName;
			DataTable dataTable = dataset.Tables[tableName];
			if (dataTable == null)
			{
				if (forDataSet)
				{
					throw new DataException(string.Format("Invalid XPath selection inside selector. Cannot find: {0}", tableName));
				}
				return;
			}
			DataColumn[] array = new DataColumn[c.Columns.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string name = c.Columns[i];
				bool flag = c.IsAttribute[i];
				DataColumn dataColumn = dataTable.Columns[name];
				if (dataColumn == null)
				{
					throw new DataException(string.Format("Invalid XPath selection inside field. Cannot find: {0}", tableName));
				}
				if (flag && dataColumn.ColumnMapping != MappingType.Attribute)
				{
					throw new DataException("The XPath specified attribute field, but mapping type is not attribute.");
				}
				if (!flag && dataColumn.ColumnMapping != MappingType.Element)
				{
					throw new DataException("The XPath specified simple element field, but mapping type is not simple element.");
				}
				array[i] = dataTable.Columns[name];
			}
			bool isPrimaryKey = c.IsPrimaryKey;
			string constraintName = c.ConstraintName;
			dataTable.Constraints.Add(new UniqueConstraint(constraintName, array, isPrimaryKey));
		}

		private void ReserveRelationIdentity(XmlSchemaElement element, XmlSchemaKeyref keyref)
		{
			string selectorTarget = GetSelectorTarget(keyref.Selector.XPath);
			string[] array = new string[keyref.Fields.Count];
			bool[] array2 = new bool[array.Length];
			int num = 0;
			foreach (XmlSchemaXPath field in keyref.Fields)
			{
				string text = field.XPath;
				bool flag = text.Length > 0 && text[0] == '@';
				int num2 = text.LastIndexOf(':');
				if (num2 > 0)
				{
					text = text.Substring(num2 + 1);
				}
				else if (flag)
				{
					text = text.Substring(1);
				}
				text = XmlHelper.Decode(text);
				array[num] = text;
				array2[num] = flag;
				num++;
			}
			string cname = keyref.Name;
			bool isNested = false;
			bool isConstraintOnly = false;
			if (keyref.UnhandledAttributes != null)
			{
				XmlAttribute[] unhandledAttributes = keyref.UnhandledAttributes;
				foreach (XmlAttribute xmlAttribute in unhandledAttributes)
				{
					if (xmlAttribute.NamespaceURI != "urn:schemas-microsoft-com:xml-msdata")
					{
						continue;
					}
					switch (xmlAttribute.LocalName)
					{
					case "ConstraintName":
						cname = xmlAttribute.Value;
						break;
					case "IsNested":
						if (xmlAttribute.Value == "true")
						{
							isNested = true;
						}
						break;
					case "ConstraintOnly":
						if (xmlAttribute.Value == "true")
						{
							isConstraintOnly = true;
						}
						break;
					}
				}
			}
			reservedConstraints.Add(keyref, new ConstraintStructure(selectorTarget, array, array2, cname, false, keyref.Refer.Name, isNested, isConstraintOnly));
		}

		private void ProcessRelationIdentity(XmlSchemaElement element, ConstraintStructure c)
		{
			string tableName = c.TableName;
			DataTable dataTable = dataset.Tables[tableName];
			if (dataTable == null)
			{
				throw new DataException(string.Format("Invalid XPath selection inside selector. Cannot find: {0}", tableName));
			}
			DataColumn[] array = new DataColumn[c.Columns.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string name = c.Columns[i];
				bool flag = c.IsAttribute[i];
				DataColumn dataColumn = dataTable.Columns[name];
				if (flag && dataColumn.ColumnMapping != MappingType.Attribute)
				{
					throw new DataException("The XPath specified attribute field, but mapping type is not attribute.");
				}
				if (!flag && dataColumn.ColumnMapping != MappingType.Element)
				{
					throw new DataException("The XPath specified simple element field, but mapping type is not simple element.");
				}
				array[i] = dataColumn;
			}
			string referName = c.ReferName;
			UniqueConstraint uniqueConstraint = FindConstraint(referName, element);
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(c.ConstraintName, uniqueConstraint.Columns, array);
			dataTable.Constraints.Add(foreignKeyConstraint);
			if (!c.IsConstraintOnly)
			{
				DataRelation dataRelation = new DataRelation(c.ConstraintName, uniqueConstraint.Columns, array, true);
				dataRelation.Nested = c.IsNested;
				dataRelation.SetParentKeyConstraint(uniqueConstraint);
				dataRelation.SetChildKeyConstraint(foreignKeyConstraint);
				dataset.Relations.Add(dataRelation);
			}
		}

		private UniqueConstraint FindConstraint(string name, XmlSchemaElement element)
		{
			foreach (XmlSchemaIdentityConstraint constraint in element.Constraints)
			{
				if (constraint is XmlSchemaKeyref || !(constraint.Name == name))
				{
					continue;
				}
				string selectorTarget = GetSelectorTarget(constraint.Selector.XPath);
				DataTable dataTable = dataset.Tables[selectorTarget];
				string name2 = constraint.Name;
				if (constraint.UnhandledAttributes != null)
				{
					XmlAttribute[] unhandledAttributes = constraint.UnhandledAttributes;
					foreach (XmlAttribute xmlAttribute in unhandledAttributes)
					{
						if (xmlAttribute.LocalName == "ConstraintName" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
						{
							name2 = xmlAttribute.Value;
						}
					}
				}
				return (UniqueConstraint)dataTable.Constraints[name2];
			}
			throw new DataException("Target identity constraint was not found: " + name);
		}

		private void HandleAnnotations(XmlSchemaAnnotation an, bool nested)
		{
			foreach (XmlSchemaObject item in an.Items)
			{
				XmlSchemaAppInfo xmlSchemaAppInfo = item as XmlSchemaAppInfo;
				if (xmlSchemaAppInfo == null)
				{
					continue;
				}
				XmlNode[] markup = xmlSchemaAppInfo.Markup;
				foreach (XmlNode xmlNode in markup)
				{
					XmlElement xmlElement = xmlNode as XmlElement;
					if (xmlElement != null && xmlElement.LocalName == "Relationship" && xmlElement.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata")
					{
						HandleRelationshipAnnotation(xmlElement, nested);
					}
					if (xmlElement != null && xmlElement.LocalName == "DataSource" && xmlElement.NamespaceURI == "urn:schemas-microsoft-com:xml-msdatasource")
					{
						HandleDataSourceAnnotation(xmlElement, nested);
					}
				}
			}
		}

		private void HandleDataSourceAnnotation(XmlElement el, bool nested)
		{
			string text = null;
			string connStr = null;
			DbProviderFactory dbProviderFactory = null;
			XmlElement xmlElement = null;
			foreach (XmlNode childNode in el.ChildNodes)
			{
				XmlElement xmlElement2 = childNode as XmlElement;
				if (xmlElement2 != null && xmlElement2.LocalName == "Tables")
				{
					xmlElement = xmlElement2;
				}
			}
			if (xmlElement == null || dbProviderFactory == null)
			{
				return;
			}
			foreach (XmlNode childNode2 in xmlElement.ChildNodes)
			{
				ProcessTableAdapter(childNode2 as XmlElement, dbProviderFactory, connStr);
			}
		}

		private void ProcessTableAdapter(XmlElement el, DbProviderFactory provider, string connStr)
		{
			string text = null;
			if (el == null)
			{
				return;
			}
			currentAdapter = new TableAdapterSchemaInfo(provider);
			currentAdapter.ConnectionString = connStr;
			currentAdapter.BaseClass = el.GetAttribute("BaseClass");
			text = el.GetAttribute("Name");
			currentAdapter.Name = el.GetAttribute("GeneratorDataComponentClassName");
			if (string.IsNullOrEmpty(currentAdapter.Name))
			{
				currentAdapter.Name = el.GetAttribute("DataAccessorName");
			}
			foreach (XmlNode childNode in el.ChildNodes)
			{
				XmlElement xmlElement = childNode as XmlElement;
				if (xmlElement == null)
				{
					continue;
				}
				switch (xmlElement.LocalName)
				{
				case "MainSource":
				case "Sources":
					foreach (XmlNode childNode2 in xmlElement.ChildNodes)
					{
						ProcessDbSource(childNode2 as XmlElement);
					}
					break;
				case "Mappings":
				{
					DataTableMapping dataTableMapping = new DataTableMapping();
					dataTableMapping.SourceTable = "Table";
					dataTableMapping.DataSetTable = text;
					foreach (XmlNode childNode3 in xmlElement.ChildNodes)
					{
						ProcessColumnMapping(childNode3 as XmlElement, dataTableMapping);
					}
					currentAdapter.Adapter.TableMappings.Add(dataTableMapping);
					break;
				}
				}
			}
		}

		private void ProcessDbSource(XmlElement el)
		{
			string text = null;
			if (el == null)
			{
				return;
			}
			text = el.GetAttribute("GenerateShortCommands");
			if (!string.IsNullOrEmpty(text))
			{
				currentAdapter.ShortCommands = Convert.ToBoolean(text);
			}
			DbCommandInfo dbCommandInfo = new DbCommandInfo();
			text = el.GetAttribute("GenerateMethods");
			if (!string.IsNullOrEmpty(text))
			{
				DbSourceMethodInfo dbSourceMethodInfo = null;
				switch ((GenerateMethodsType)(int)Enum.Parse(typeof(GenerateMethodsType), text))
				{
				case GenerateMethodsType.Get:
					dbSourceMethodInfo = new DbSourceMethodInfo();
					dbSourceMethodInfo.Name = el.GetAttribute("GetMethodName");
					dbSourceMethodInfo.Modifier = el.GetAttribute("GetMethodModifier");
					if (string.IsNullOrEmpty(dbSourceMethodInfo.Modifier))
					{
						dbSourceMethodInfo.Modifier = "Public";
					}
					dbSourceMethodInfo.ScalarCallRetval = el.GetAttribute("ScalarCallRetval");
					dbSourceMethodInfo.QueryType = el.GetAttribute("QueryType");
					dbSourceMethodInfo.MethodType = GenerateMethodsType.Get;
					dbCommandInfo.Methods = new DbSourceMethodInfo[1];
					dbCommandInfo.Methods[0] = dbSourceMethodInfo;
					break;
				case GenerateMethodsType.Fill:
					dbSourceMethodInfo = new DbSourceMethodInfo();
					dbSourceMethodInfo.Name = el.GetAttribute("FillMethodName");
					dbSourceMethodInfo.Modifier = el.GetAttribute("FillMethodModifier");
					if (string.IsNullOrEmpty(dbSourceMethodInfo.Modifier))
					{
						dbSourceMethodInfo.Modifier = "Public";
					}
					dbSourceMethodInfo.ScalarCallRetval = null;
					dbSourceMethodInfo.QueryType = null;
					dbSourceMethodInfo.MethodType = GenerateMethodsType.Fill;
					dbCommandInfo.Methods = new DbSourceMethodInfo[1];
					dbCommandInfo.Methods[0] = dbSourceMethodInfo;
					break;
				case GenerateMethodsType.Both:
					dbSourceMethodInfo = new DbSourceMethodInfo();
					dbSourceMethodInfo.Name = el.GetAttribute("GetMethodName");
					dbSourceMethodInfo.Modifier = el.GetAttribute("GetMethodModifier");
					if (string.IsNullOrEmpty(dbSourceMethodInfo.Modifier))
					{
						dbSourceMethodInfo.Modifier = "Public";
					}
					dbSourceMethodInfo.ScalarCallRetval = el.GetAttribute("ScalarCallRetval");
					dbSourceMethodInfo.QueryType = el.GetAttribute("QueryType");
					dbSourceMethodInfo.MethodType = GenerateMethodsType.Get;
					dbCommandInfo.Methods = new DbSourceMethodInfo[2];
					dbCommandInfo.Methods[0] = dbSourceMethodInfo;
					dbSourceMethodInfo = new DbSourceMethodInfo();
					dbSourceMethodInfo.Name = el.GetAttribute("FillMethodName");
					dbSourceMethodInfo.Modifier = el.GetAttribute("FillMethodModifier");
					if (string.IsNullOrEmpty(dbSourceMethodInfo.Modifier))
					{
						dbSourceMethodInfo.Modifier = "Public";
					}
					dbSourceMethodInfo.ScalarCallRetval = null;
					dbSourceMethodInfo.QueryType = null;
					dbSourceMethodInfo.MethodType = GenerateMethodsType.Fill;
					dbCommandInfo.Methods[1] = dbSourceMethodInfo;
					break;
				}
			}
			else
			{
				DbSourceMethodInfo dbSourceMethodInfo2 = new DbSourceMethodInfo();
				dbSourceMethodInfo2.Name = el.GetAttribute("Name");
				dbSourceMethodInfo2.Modifier = el.GetAttribute("Modifier");
				if (string.IsNullOrEmpty(dbSourceMethodInfo2.Modifier))
				{
					dbSourceMethodInfo2.Modifier = "Public";
				}
				dbSourceMethodInfo2.ScalarCallRetval = el.GetAttribute("ScalarCallRetval");
				dbSourceMethodInfo2.QueryType = el.GetAttribute("QueryType");
				dbSourceMethodInfo2.MethodType = GenerateMethodsType.None;
				dbCommandInfo.Methods = new DbSourceMethodInfo[1];
				dbCommandInfo.Methods[0] = dbSourceMethodInfo2;
			}
			foreach (XmlNode childNode in el.ChildNodes)
			{
				XmlElement xmlElement = childNode as XmlElement;
				if (xmlElement != null)
				{
					switch (xmlElement.LocalName)
					{
					case "SelectCommand":
						dbCommandInfo.Command = ProcessDbCommand(xmlElement.FirstChild as XmlElement);
						currentAdapter.Commands.Add(dbCommandInfo);
						break;
					case "InsertCommand":
						currentAdapter.Adapter.InsertCommand = ProcessDbCommand(xmlElement.FirstChild as XmlElement);
						break;
					case "UpdateCommand":
						currentAdapter.Adapter.UpdateCommand = ProcessDbCommand(xmlElement.FirstChild as XmlElement);
						break;
					case "DeleteCommand":
						currentAdapter.Adapter.DeleteCommand = ProcessDbCommand(xmlElement.FirstChild as XmlElement);
						break;
					}
				}
			}
		}

		private DbCommand ProcessDbCommand(XmlElement el)
		{
			string commandText = null;
			string text = null;
			ArrayList arrayList = null;
			if (el == null)
			{
				return null;
			}
			text = el.GetAttribute("CommandType");
			foreach (XmlNode childNode in el.ChildNodes)
			{
				XmlElement xmlElement = childNode as XmlElement;
				if (xmlElement != null && xmlElement.LocalName == "CommandText")
				{
					commandText = xmlElement.InnerText;
				}
				else if (xmlElement != null && xmlElement.LocalName == "Parameters" && !xmlElement.IsEmpty)
				{
					arrayList = ProcessDbParameters(xmlElement);
				}
			}
			DbCommand dbCommand = currentAdapter.Provider.CreateCommand();
			dbCommand.CommandText = commandText;
			if (text == "StoredProcedure")
			{
				dbCommand.CommandType = CommandType.StoredProcedure;
			}
			else
			{
				dbCommand.CommandType = CommandType.Text;
			}
			if (arrayList != null)
			{
				dbCommand.Parameters.AddRange(arrayList.ToArray());
			}
			return dbCommand;
		}

		private ArrayList ProcessDbParameters(XmlElement el)
		{
			string text = null;
			DbParameter dbParameter = null;
			ArrayList arrayList = new ArrayList();
			if (el == null)
			{
				return arrayList;
			}
			foreach (XmlNode childNode in el.ChildNodes)
			{
				XmlElement xmlElement = childNode as XmlElement;
				if (xmlElement != null)
				{
					dbParameter = currentAdapter.Provider.CreateParameter();
					text = xmlElement.GetAttribute("AllowDbNull");
					if (!string.IsNullOrEmpty(text))
					{
						dbParameter.IsNullable = Convert.ToBoolean(text);
					}
					dbParameter.ParameterName = xmlElement.GetAttribute("ParameterName");
					text = xmlElement.GetAttribute("ProviderType");
					if (!string.IsNullOrEmpty(text))
					{
						text = xmlElement.GetAttribute("DbType");
					}
					dbParameter.FrameworkDbType = text;
					text = xmlElement.GetAttribute("Direction");
					dbParameter.Direction = (ParameterDirection)(int)Enum.Parse(typeof(ParameterDirection), text);
					((IDbDataParameter)dbParameter).Precision = Convert.ToByte(xmlElement.GetAttribute("Precision"));
					((IDbDataParameter)dbParameter).Scale = Convert.ToByte(xmlElement.GetAttribute("Scale"));
					dbParameter.Size = Convert.ToInt32(xmlElement.GetAttribute("Size"));
					dbParameter.SourceColumn = xmlElement.GetAttribute("SourceColumn");
					text = xmlElement.GetAttribute("SourceColumnNullMapping");
					if (!string.IsNullOrEmpty(text))
					{
						dbParameter.SourceColumnNullMapping = Convert.ToBoolean(text);
					}
					text = xmlElement.GetAttribute("SourceVersion");
					dbParameter.SourceVersion = (DataRowVersion)(int)Enum.Parse(typeof(DataRowVersion), text);
					arrayList.Add(dbParameter);
				}
			}
			return arrayList;
		}

		private void ProcessColumnMapping(XmlElement el, DataTableMapping tableMapping)
		{
			if (el != null)
			{
				tableMapping.ColumnMappings.Add(el.GetAttribute("SourceColumn"), el.GetAttribute("DataSetColumn"));
			}
		}

		private void HandleRelationshipAnnotation(XmlElement el, bool nested)
		{
			string attribute = el.GetAttribute("name");
			string attribute2 = el.GetAttribute("parent", "urn:schemas-microsoft-com:xml-msdata");
			string attribute3 = el.GetAttribute("child", "urn:schemas-microsoft-com:xml-msdata");
			string attribute4 = el.GetAttribute("parentkey", "urn:schemas-microsoft-com:xml-msdata");
			string attribute5 = el.GetAttribute("childkey", "urn:schemas-microsoft-com:xml-msdata");
			RelationStructure relationStructure = new RelationStructure();
			relationStructure.ExplicitName = XmlHelper.Decode(attribute);
			relationStructure.ParentTableName = XmlHelper.Decode(attribute2);
			relationStructure.ChildTableName = XmlHelper.Decode(attribute3);
			relationStructure.ParentColumnName = attribute4;
			relationStructure.ChildColumnName = attribute5;
			relationStructure.IsNested = nested;
			relationStructure.CreateConstraint = false;
			relations.Add(relationStructure);
		}

		private object GetElementDefaultValue(XmlSchemaElement elem)
		{
			if (elem.RefName == XmlQualifiedName.Empty)
			{
				return elem.DefaultValue;
			}
			XmlSchemaElement xmlSchemaElement = schema.Elements[elem.RefName] as XmlSchemaElement;
			if (xmlSchemaElement == null)
			{
				return null;
			}
			return xmlSchemaElement.DefaultValue;
		}

		private object GetAttributeDefaultValue(XmlSchemaAttribute attr)
		{
			if (attr.DefaultValue != null)
			{
				return attr.DefaultValue;
			}
			if (attr.FixedValue != null)
			{
				return attr.FixedValue;
			}
			if (attr.RefName == XmlQualifiedName.Empty)
			{
				return null;
			}
			XmlSchemaAttribute xmlSchemaAttribute = schema.Attributes[attr.RefName] as XmlSchemaAttribute;
			if (xmlSchemaAttribute == null)
			{
				return null;
			}
			if (xmlSchemaAttribute.DefaultValue != null)
			{
				return xmlSchemaAttribute.DefaultValue;
			}
			return xmlSchemaAttribute.FixedValue;
		}
	}
}
