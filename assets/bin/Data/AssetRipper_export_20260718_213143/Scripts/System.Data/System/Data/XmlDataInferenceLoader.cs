using System.Collections;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	internal class XmlDataInferenceLoader
	{
		private DataSet dataset;

		private XmlDocument document;

		private XmlReadMode mode;

		private ArrayList ignoredNamespaces;

		private TableMappingCollection tables = new TableMappingCollection();

		private RelationStructureCollection relations = new RelationStructureCollection();

		private XmlDataInferenceLoader(DataSet ds, XmlDocument doc, XmlReadMode mode, string[] ignoredNamespaces)
		{
			dataset = ds;
			document = doc;
			this.mode = mode;
			this.ignoredNamespaces = ((ignoredNamespaces == null) ? new ArrayList() : new ArrayList(ignoredNamespaces));
			foreach (DataTable table in dataset.Tables)
			{
				tables.Add(new TableMapping(table));
			}
		}

		public static void Infer(DataSet dataset, XmlDocument document, XmlReadMode mode, string[] ignoredNamespaces)
		{
			new XmlDataInferenceLoader(dataset, document, mode, ignoredNamespaces).ReadXml();
		}

		private void ReadXml()
		{
			if (document.DocumentElement == null)
			{
				return;
			}
			dataset.Locale = new CultureInfo("en-US");
			XmlElement documentElement = document.DocumentElement;
			if (documentElement.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
			{
				throw new InvalidOperationException("DataSet is not designed to handle XML Schema as data content. Please use ReadXmlSchema method instead of InferXmlSchema method.");
			}
			if (IsDocumentElementTable())
			{
				InferTopLevelTable(documentElement);
			}
			else
			{
				string dataSetName = XmlHelper.Decode(documentElement.LocalName);
				dataset.DataSetName = dataSetName;
				dataset.Namespace = documentElement.NamespaceURI;
				dataset.Prefix = documentElement.Prefix;
				foreach (XmlNode childNode in documentElement.ChildNodes)
				{
					if (!(childNode.NamespaceURI == "http://www.w3.org/2001/XMLSchema") && childNode.NodeType == XmlNodeType.Element)
					{
						InferTopLevelTable(childNode as XmlElement);
					}
				}
			}
			int num = 0;
			foreach (TableMapping table in tables)
			{
				string text = ((table.PrimaryKey == null) ? (table.Table.TableName + "_Id") : table.PrimaryKey.ColumnName);
				string name = text;
				if (table.ChildTables[table.Table.TableName] != null)
				{
					name = text + '_' + num;
					while (table.GetColumn(name) != null)
					{
						num++;
						name = text + '_' + num;
					}
				}
				foreach (TableMapping childTable in table.ChildTables)
				{
					childTable.ReferenceKey = GetMappedColumn(childTable, name, table.Table.Prefix, table.Table.Namespace, MappingType.Hidden, (table.PrimaryKey == null) ? typeof(int) : table.PrimaryKey.DataType);
				}
			}
			foreach (TableMapping table2 in tables)
			{
				if (table2.ExistsInDataSet)
				{
					continue;
				}
				if (table2.PrimaryKey != null)
				{
					table2.Table.Columns.Add(table2.PrimaryKey);
				}
				foreach (DataColumn element in table2.Elements)
				{
					table2.Table.Columns.Add(element);
				}
				foreach (DataColumn attribute in table2.Attributes)
				{
					table2.Table.Columns.Add(attribute);
				}
				if (table2.SimpleContent != null)
				{
					table2.Table.Columns.Add(table2.SimpleContent);
				}
				if (table2.ReferenceKey != null)
				{
					table2.Table.Columns.Add(table2.ReferenceKey);
				}
				dataset.Tables.Add(table2.Table);
			}
			foreach (RelationStructure relation in relations)
			{
				string relationName = ((relation.ExplicitName == null) ? (relation.ParentTableName + "_" + relation.ChildTableName) : relation.ExplicitName);
				DataTable dataTable = dataset.Tables[relation.ParentTableName];
				DataTable dataTable2 = dataset.Tables[relation.ChildTableName];
				DataColumn dataColumn = dataTable.Columns[relation.ParentColumnName];
				DataColumn dataColumn2 = null;
				if (relation.ParentTableName == relation.ChildTableName)
				{
					dataColumn2 = dataTable2.Columns[relation.ChildColumnName + "_" + num];
				}
				if (dataColumn2 == null)
				{
					dataColumn2 = dataTable2.Columns[relation.ChildColumnName];
				}
				if (dataTable == null)
				{
					throw new DataException("Parent table was not found : " + relation.ParentTableName);
				}
				if (dataTable2 == null)
				{
					throw new DataException("Child table was not found : " + relation.ChildTableName);
				}
				if (dataColumn == null)
				{
					throw new DataException("Parent column was not found :" + relation.ParentColumnName);
				}
				if (dataColumn2 == null)
				{
					throw new DataException("Child column was not found :" + relation.ChildColumnName);
				}
				DataRelation dataRelation = new DataRelation(relationName, dataColumn, dataColumn2, relation.CreateConstraint);
				if (relation.IsNested)
				{
					dataRelation.Nested = true;
					dataRelation.ParentTable.PrimaryKey = dataRelation.ParentColumns;
				}
				dataset.Relations.Add(dataRelation);
			}
		}

		private void InferTopLevelTable(XmlElement el)
		{
			InferTableElement(null, el);
		}

		private void InferColumnElement(TableMapping table, XmlElement el)
		{
			string text = XmlHelper.Decode(el.LocalName);
			DataColumn column = table.GetColumn(text);
			if (column != null)
			{
				if (column.ColumnMapping != MappingType.Element)
				{
					throw new DataException(string.Format("Column {0} is already mapped to {1}.", text, column.ColumnMapping));
				}
				table.lastElementIndex = table.Elements.IndexOf(column);
			}
			else if (table.ChildTables[text] == null)
			{
				column = new DataColumn(text, typeof(string));
				column.Namespace = el.NamespaceURI;
				column.Prefix = el.Prefix;
				table.Elements.Insert(++table.lastElementIndex, column);
			}
		}

		private void CheckExtraneousElementColumn(TableMapping parentTable, XmlElement el)
		{
			if (parentTable != null)
			{
				string name = XmlHelper.Decode(el.LocalName);
				DataColumn column = parentTable.GetColumn(name);
				if (column != null)
				{
					parentTable.RemoveElementColumn(name);
				}
			}
		}

		private void PopulatePrimaryKey(TableMapping table)
		{
			DataColumn dataColumn = new DataColumn(table.Table.TableName + "_Id");
			dataColumn.ColumnMapping = MappingType.Hidden;
			dataColumn.DataType = typeof(int);
			dataColumn.AllowDBNull = false;
			dataColumn.AutoIncrement = true;
			dataColumn.Namespace = table.Table.Namespace;
			dataColumn.Prefix = table.Table.Prefix;
			table.PrimaryKey = dataColumn;
		}

		private void PopulateRelationStructure(string parent, string child, string pkeyColumn)
		{
			if (relations[parent, child] == null)
			{
				RelationStructure relationStructure = new RelationStructure();
				relationStructure.ParentTableName = parent;
				relationStructure.ChildTableName = child;
				relationStructure.ParentColumnName = pkeyColumn;
				relationStructure.ChildColumnName = pkeyColumn;
				relationStructure.CreateConstraint = true;
				relationStructure.IsNested = true;
				relations.Add(relationStructure);
			}
		}

		private void InferRepeatedElement(TableMapping parentTable, XmlElement el)
		{
			string text = XmlHelper.Decode(el.LocalName);
			CheckExtraneousElementColumn(parentTable, el);
			TableMapping mappedTable = GetMappedTable(parentTable, text, el.NamespaceURI);
			if (mappedTable.Elements.Count <= 0 && mappedTable.SimpleContent == null)
			{
				GetMappedColumn(mappedTable, text + "_Column", el.Prefix, el.NamespaceURI, MappingType.SimpleContent, null);
			}
		}

		private void InferTableElement(TableMapping parentTable, XmlElement el)
		{
			CheckExtraneousElementColumn(parentTable, el);
			string tableName = XmlHelper.Decode(el.LocalName);
			TableMapping mappedTable = GetMappedTable(parentTable, tableName, el.NamespaceURI);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			foreach (XmlAttribute attribute in el.Attributes)
			{
				if (!(attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") && !(attribute.NamespaceURI == "http://www.w3.org/XML/1998/namespace") && (ignoredNamespaces == null || !ignoredNamespaces.Contains(attribute.NamespaceURI)))
				{
					flag2 = true;
					DataColumn mappedColumn = GetMappedColumn(mappedTable, XmlHelper.Decode(attribute.LocalName), attribute.Prefix, attribute.NamespaceURI, MappingType.Attribute, null);
				}
			}
			foreach (XmlNode childNode in el.ChildNodes)
			{
				switch (childNode.NodeType)
				{
				default:
					flag3 = true;
					if (GetElementMappingType(el, ignoredNamespaces, null) == ElementMappingType.Repeated)
					{
						flag4 = true;
					}
					break;
				case XmlNodeType.Element:
				{
					flag = true;
					XmlElement xmlElement = childNode as XmlElement;
					string child = XmlHelper.Decode(xmlElement.LocalName);
					switch (GetElementMappingType(xmlElement, ignoredNamespaces, null))
					{
					case ElementMappingType.Simple:
						InferColumnElement(mappedTable, xmlElement);
						break;
					case ElementMappingType.Repeated:
						if (mappedTable.PrimaryKey == null)
						{
							PopulatePrimaryKey(mappedTable);
						}
						PopulateRelationStructure(mappedTable.Table.TableName, child, mappedTable.PrimaryKey.ColumnName);
						InferRepeatedElement(mappedTable, xmlElement);
						break;
					case ElementMappingType.Complex:
						if (mappedTable.PrimaryKey == null)
						{
							PopulatePrimaryKey(mappedTable);
						}
						PopulateRelationStructure(mappedTable.Table.TableName, child, mappedTable.PrimaryKey.ColumnName);
						InferTableElement(mappedTable, xmlElement);
						break;
					}
					break;
				}
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
					break;
				}
			}
			if (mappedTable.SimpleContent == null && !flag && flag3 && (flag2 || flag4))
			{
				GetMappedColumn(mappedTable, mappedTable.Table.TableName + "_Text", string.Empty, string.Empty, MappingType.SimpleContent, null);
			}
		}

		private TableMapping GetMappedTable(TableMapping parent, string tableName, string ns)
		{
			TableMapping tableMapping = tables[tableName];
			if (tableMapping != null)
			{
				if (parent != null && tableMapping.ParentTable != null && tableMapping.ParentTable != parent)
				{
					throw new DataException(string.Format("The table '{0}' is already allocated as a child of another table '{1}'. Cannot set table '{2}' as parent table.", tableName, tableMapping.ParentTable.Table.TableName, parent.Table.TableName));
				}
			}
			else
			{
				tableMapping = new TableMapping(tableName, ns);
				tableMapping.ParentTable = parent;
				tables.Add(tableMapping);
			}
			if (parent != null)
			{
				bool flag = true;
				foreach (TableMapping childTable in parent.ChildTables)
				{
					if (childTable.Table.TableName == tableName)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					parent.ChildTables.Add(tableMapping);
				}
			}
			return tableMapping;
		}

		private DataColumn GetMappedColumn(TableMapping table, string name, string prefix, string ns, MappingType type, Type optColType)
		{
			DataColumn dataColumn = table.GetColumn(name);
			if (dataColumn == null)
			{
				dataColumn = new DataColumn(name);
				dataColumn.Prefix = prefix;
				dataColumn.Namespace = ns;
				dataColumn.ColumnMapping = type;
				switch (type)
				{
				case MappingType.Element:
					table.Elements.Add(dataColumn);
					break;
				case MappingType.Attribute:
					table.Attributes.Add(dataColumn);
					break;
				case MappingType.SimpleContent:
					table.SimpleContent = dataColumn;
					break;
				case MappingType.Hidden:
					dataColumn.DataType = optColType;
					table.ReferenceKey = dataColumn;
					break;
				}
			}
			else if (dataColumn.ColumnMapping != type)
			{
				throw new DataException(string.Format("There are already another column that has different mapping type. Column is {0}, existing mapping type is {1}", dataColumn.ColumnName, dataColumn.ColumnMapping));
			}
			return dataColumn;
		}

		private static void SetAsExistingTable(XmlElement el, Hashtable existingTables)
		{
			if (existingTables != null)
			{
				ArrayList arrayList = existingTables[el.NamespaceURI] as ArrayList;
				if (arrayList == null)
				{
					arrayList = new ArrayList();
					existingTables[el.NamespaceURI] = arrayList;
				}
				if (!arrayList.Contains(el.LocalName))
				{
					arrayList.Add(el.LocalName);
				}
			}
		}

		private static ElementMappingType GetElementMappingType(XmlElement el, ArrayList ignoredNamespaces, Hashtable existingTables)
		{
			if (existingTables != null)
			{
				ArrayList arrayList = existingTables[el.NamespaceURI] as ArrayList;
				if (arrayList != null && arrayList.Contains(el.LocalName))
				{
					return ElementMappingType.Complex;
				}
			}
			foreach (XmlAttribute attribute in el.Attributes)
			{
				if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/" || attribute.NamespaceURI == "http://www.w3.org/XML/1998/namespace" || (ignoredNamespaces != null && ignoredNamespaces.Contains(attribute.NamespaceURI)))
				{
					continue;
				}
				SetAsExistingTable(el, existingTables);
				return ElementMappingType.Complex;
			}
			foreach (XmlNode childNode in el.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Element)
				{
					SetAsExistingTable(el, existingTables);
					return ElementMappingType.Complex;
				}
			}
			for (XmlNode nextSibling = el.NextSibling; nextSibling != null; nextSibling = nextSibling.NextSibling)
			{
				if (nextSibling.NodeType == XmlNodeType.Element && nextSibling.LocalName == el.LocalName)
				{
					SetAsExistingTable(el, existingTables);
					return (GetElementMappingType(nextSibling as XmlElement, ignoredNamespaces, null) != ElementMappingType.Complex) ? ElementMappingType.Repeated : ElementMappingType.Complex;
				}
			}
			return ElementMappingType.Simple;
		}

		private bool IsDocumentElementTable()
		{
			return IsDocumentElementTable(document.DocumentElement, ignoredNamespaces);
		}

		internal static bool IsDocumentElementTable(XmlElement top, ArrayList ignoredNamespaces)
		{
			foreach (XmlAttribute attribute in top.Attributes)
			{
				if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/" || attribute.NamespaceURI == "http://www.w3.org/XML/1998/namespace" || (ignoredNamespaces != null && ignoredNamespaces.Contains(attribute.NamespaceURI)))
				{
					continue;
				}
				return true;
			}
			Hashtable existingTables = new Hashtable();
			foreach (XmlNode childNode in top.ChildNodes)
			{
				XmlElement xmlElement = childNode as XmlElement;
				if (xmlElement == null || GetElementMappingType(xmlElement, ignoredNamespaces, existingTables) != ElementMappingType.Simple)
				{
					continue;
				}
				return true;
			}
			return false;
		}
	}
}
