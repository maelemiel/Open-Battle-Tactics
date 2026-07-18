using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Xml.XPath;

namespace System.Xml
{
	public class XmlDataDocument : XmlDocument
	{
		internal class XmlDataElement : XmlElement
		{
			private DataRow row;

			internal DataRow DataRow
			{
				get
				{
					return row;
				}
			}

			internal XmlDataElement(DataRow row, string prefix, string localName, string ns, XmlDataDocument doc)
				: base(prefix, localName, ns, doc)
			{
				this.row = row;
				if (row != null)
				{
					row.DataElement = this;
					row.XmlRowID = doc.dataRowID;
					doc.dataRowIDList.Add(row.XmlRowID);
					doc.dataRowID++;
				}
			}
		}

		private DataSet dataSet;

		private int dataRowID = 1;

		private ArrayList dataRowIDList = new ArrayList();

		private bool raiseDataSetEvents = true;

		private bool raiseDocumentEvents = true;

		private DataColumnChangeEventHandler columnChanged;

		private DataRowChangeEventHandler rowDeleted;

		private DataRowChangeEventHandler rowChanged;

		private CollectionChangeEventHandler tablesChanged;

		public DataSet DataSet
		{
			get
			{
				return dataSet;
			}
		}

		public XmlDataDocument()
		{
			InitDelegateFields();
			dataSet = new DataSet();
			dataSet._xmlDataDocument = this;
			dataSet.Tables.CollectionChanged += tablesChanged;
			AddXmlDocumentListeners();
			DataSet.EnforceConstraints = false;
		}

		public XmlDataDocument(DataSet dataset)
		{
			if (dataset == null)
			{
				throw new ArgumentException("Parameter dataset cannot be null.");
			}
			if (dataset._xmlDataDocument != null)
			{
				throw new ArgumentException("DataSet cannot be associated with two or more XmlDataDocument.");
			}
			InitDelegateFields();
			dataSet = dataset;
			dataSet._xmlDataDocument = this;
			XmlElement xmlElement = CreateElement(dataSet.Prefix, XmlHelper.Encode(dataSet.DataSetName), dataSet.Namespace);
			foreach (DataTable table in dataSet.Tables)
			{
				if (table.ParentRelations.Count <= 0)
				{
					FillNodeRows(xmlElement, table, table.Rows);
				}
			}
			if (xmlElement.ChildNodes.Count > 0)
			{
				AppendChild(xmlElement);
			}
			foreach (DataTable table2 in dataSet.Tables)
			{
				table2.ColumnChanged += columnChanged;
				table2.RowDeleted += rowDeleted;
				table2.RowChanged += rowChanged;
			}
			AddXmlDocumentListeners();
		}

		private XmlDataDocument(DataSet dataset, bool clone)
		{
			InitDelegateFields();
			dataSet = dataset;
			dataSet._xmlDataDocument = this;
			foreach (DataTable table in DataSet.Tables)
			{
				foreach (DataRow row in table.Rows)
				{
					row.XmlRowID = dataRowID;
					dataRowIDList.Add(dataRowID);
					dataRowID++;
				}
			}
			AddXmlDocumentListeners();
			foreach (DataTable table2 in dataSet.Tables)
			{
				table2.ColumnChanged += columnChanged;
				table2.RowDeleted += rowDeleted;
				table2.RowChanged += rowChanged;
			}
		}

		private void FillNodeRows(XmlElement parent, DataTable dt, ICollection rows)
		{
			foreach (DataRow row in dt.Rows)
			{
				XmlDataElement dataElement = row.DataElement;
				FillNodeChildrenFromRow(row, dataElement);
				foreach (DataRelation childRelation in dt.ChildRelations)
				{
					FillNodeRows(dataElement, childRelation.ChildTable, row.GetChildRows(childRelation));
				}
				parent.AppendChild(dataElement);
			}
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlDataDocument xmlDataDocument = ((!deep) ? new XmlDataDocument(DataSet.Clone(), true) : new XmlDataDocument(DataSet.Copy(), true));
			xmlDataDocument.RemoveXmlDocumentListeners();
			xmlDataDocument.PreserveWhitespace = base.PreserveWhitespace;
			if (deep)
			{
				foreach (XmlNode childNode in ChildNodes)
				{
					xmlDataDocument.AppendChild(xmlDataDocument.ImportNode(childNode, deep));
				}
			}
			xmlDataDocument.AddXmlDocumentListeners();
			return xmlDataDocument;
		}

		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			DataTable dataTable = DataSet.Tables[XmlHelper.Decode(localName)];
			DataRow dataRow = ((dataTable == null) ? null : dataTable.NewRow());
			if (dataRow != null)
			{
				return GetElementFromRow(dataRow);
			}
			return base.CreateElement(prefix, localName, namespaceURI);
		}

		public override XmlEntityReference CreateEntityReference(string name)
		{
			throw new NotSupportedException();
		}

		public override XmlElement GetElementById(string elemId)
		{
			throw new NotSupportedException();
		}

		public XmlElement GetElementFromRow(DataRow r)
		{
			return r.DataElement;
		}

		public DataRow GetRowFromElement(XmlElement e)
		{
			XmlDataElement xmlDataElement = e as XmlDataElement;
			if (xmlDataElement == null)
			{
				return null;
			}
			return xmlDataElement.DataRow;
		}

		public override void Load(Stream inStream)
		{
			Load(new XmlTextReader(inStream));
		}

		public override void Load(string filename)
		{
			Load(new XmlTextReader(filename));
		}

		public override void Load(TextReader txtReader)
		{
			Load(new XmlTextReader(txtReader));
		}

		public override void Load(XmlReader reader)
		{
			if (base.DocumentElement != null)
			{
				throw new InvalidOperationException("XmlDataDocument does not support multi-time loading. New XmlDadaDocument is always required.");
			}
			bool enforceConstraints = DataSet.EnforceConstraints;
			DataSet.EnforceConstraints = false;
			dataSet.Tables.CollectionChanged -= tablesChanged;
			base.Load(reader);
			DataSet.EnforceConstraints = enforceConstraints;
			dataSet.Tables.CollectionChanged += tablesChanged;
		}

		[System.MonoTODO("Create optimized XPathNavigator")]
		protected override XPathNavigator CreateNavigator(XmlNode node)
		{
			return base.CreateNavigator(node);
		}

		private void OnNodeChanging(object sender, XmlNodeChangedEventArgs args)
		{
			if (!raiseDocumentEvents || !DataSet.EnforceConstraints)
			{
				return;
			}
			throw new InvalidOperationException(global::Locale.GetText("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations."));
		}

		private void OnNodeChanged(object sender, XmlNodeChangedEventArgs args)
		{
			if (!raiseDocumentEvents)
			{
				return;
			}
			bool flag = raiseDataSetEvents;
			raiseDataSetEvents = false;
			try
			{
				if (args.Node != null)
				{
					DataRow rowFromElement = GetRowFromElement((XmlElement)args.Node.ParentNode.ParentNode);
					if (rowFromElement != null && rowFromElement.Table.Columns.Contains(args.Node.ParentNode.Name) && rowFromElement[args.Node.ParentNode.Name].ToString() != args.Node.InnerText)
					{
						DataColumn dataColumn = rowFromElement.Table.Columns[args.Node.ParentNode.Name];
						rowFromElement[dataColumn] = StringToObject(dataColumn.DataType, args.Node.InnerText);
					}
				}
			}
			finally
			{
				raiseDataSetEvents = flag;
			}
		}

		private void OnNodeRemoving(object sender, XmlNodeChangedEventArgs args)
		{
			if (!raiseDocumentEvents || !DataSet.EnforceConstraints)
			{
				return;
			}
			throw new InvalidOperationException(global::Locale.GetText("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations."));
		}

		private void OnNodeRemoved(object sender, XmlNodeChangedEventArgs args)
		{
			if (!raiseDocumentEvents)
			{
				return;
			}
			bool flag = raiseDataSetEvents;
			raiseDataSetEvents = false;
			try
			{
				if (args.OldParent == null)
				{
					return;
				}
				XmlElement xmlElement = args.OldParent as XmlElement;
				if (xmlElement == null)
				{
					return;
				}
				XmlElement xmlElement2 = args.Node as XmlElement;
				if (xmlElement2 != null)
				{
					DataRow rowFromElement = GetRowFromElement(xmlElement2);
					if (rowFromElement != null)
					{
						rowFromElement.Table.Rows.Remove(rowFromElement);
					}
				}
				DataRow rowFromElement2 = GetRowFromElement(xmlElement);
				if (rowFromElement2 != null)
				{
					rowFromElement2[args.Node.Name] = null;
				}
			}
			finally
			{
				raiseDataSetEvents = flag;
			}
		}

		private void OnNodeInserting(object sender, XmlNodeChangedEventArgs args)
		{
			if (!raiseDocumentEvents || !DataSet.EnforceConstraints)
			{
				return;
			}
			throw new InvalidOperationException(global::Locale.GetText("Please set DataSet.EnforceConstraints == false before trying to edit XmlDataDocument using XML operations."));
		}

		private void OnNodeInserted(object sender, XmlNodeChangedEventArgs args)
		{
			if (!raiseDocumentEvents)
			{
				return;
			}
			bool flag = raiseDataSetEvents;
			raiseDataSetEvents = false;
			try
			{
				if (!(args.NewParent is XmlElement))
				{
					foreach (XmlNode childNode in args.Node.ChildNodes)
					{
						CheckDescendantRelationship(childNode);
					}
					return;
				}
				DataRow rowFromElement = GetRowFromElement(args.NewParent as XmlElement);
				if (rowFromElement == null)
				{
					if (args.NewParent == base.DocumentElement)
					{
						CheckDescendantRelationship(args.Node);
					}
					return;
				}
				XmlAttribute xmlAttribute = args.Node as XmlAttribute;
				if (xmlAttribute != null)
				{
					DataColumn dataColumn = rowFromElement.Table.Columns[XmlHelper.Decode(xmlAttribute.LocalName)];
					if (dataColumn != null)
					{
						rowFromElement[dataColumn] = StringToObject(dataColumn.DataType, args.Node.Value);
					}
					return;
				}
				DataRow rowFromElement2 = GetRowFromElement(args.Node as XmlElement);
				if (rowFromElement2 != null)
				{
					if (rowFromElement2.RowState != DataRowState.Detached && rowFromElement.RowState != DataRowState.Detached)
					{
						FillRelationship(rowFromElement, rowFromElement2, args.NewParent, args.Node);
					}
				}
				else if (args.Node.NodeType == XmlNodeType.Element)
				{
					DataColumn dataColumn2 = rowFromElement.Table.Columns[XmlHelper.Decode(args.Node.LocalName)];
					if (dataColumn2 != null)
					{
						rowFromElement[dataColumn2] = StringToObject(dataColumn2.DataType, args.Node.InnerText);
					}
				}
				else
				{
					if (!(args.Node is XmlCharacterData) || args.Node.NodeType == XmlNodeType.Comment)
					{
						return;
					}
					for (int i = 0; i < rowFromElement.Table.Columns.Count; i++)
					{
						DataColumn dataColumn3 = rowFromElement.Table.Columns[i];
						if (dataColumn3.ColumnMapping == MappingType.SimpleContent)
						{
							rowFromElement[dataColumn3] = StringToObject(dataColumn3.DataType, args.Node.Value);
						}
					}
				}
			}
			finally
			{
				raiseDataSetEvents = flag;
			}
		}

		private void CheckDescendantRelationship(XmlNode n)
		{
			XmlElement e = n as XmlElement;
			DataRow rowFromElement = GetRowFromElement(e);
			if (rowFromElement != null)
			{
				rowFromElement.Table.Rows.Add(rowFromElement);
				CheckDescendantRelationship(n, rowFromElement);
			}
		}

		private void CheckDescendantRelationship(XmlNode p, DataRow row)
		{
			foreach (XmlNode childNode in p.ChildNodes)
			{
				XmlElement xmlElement = childNode as XmlElement;
				if (xmlElement != null)
				{
					DataRow rowFromElement = GetRowFromElement(xmlElement);
					if (rowFromElement != null)
					{
						rowFromElement.Table.Rows.Add(rowFromElement);
						FillRelationship(row, rowFromElement, p, xmlElement);
					}
				}
			}
		}

		private void FillRelationship(DataRow row, DataRow childRow, XmlNode parentNode, XmlNode childNode)
		{
			for (int i = 0; i < childRow.Table.ParentRelations.Count; i++)
			{
				DataRelation dataRelation = childRow.Table.ParentRelations[i];
				if (dataRelation.ParentTable == row.Table)
				{
					childRow.SetParentRow(row);
					break;
				}
			}
			CheckDescendantRelationship(childNode, childRow);
		}

		private void OnDataTableChanged(object sender, CollectionChangeEventArgs eventArgs)
		{
			if (!raiseDataSetEvents)
			{
				return;
			}
			bool flag = raiseDocumentEvents;
			raiseDocumentEvents = false;
			try
			{
				DataTable dataTable = (DataTable)eventArgs.Element;
				switch (eventArgs.Action)
				{
				case CollectionChangeAction.Add:
					dataTable.ColumnChanged += columnChanged;
					dataTable.RowDeleted += rowDeleted;
					dataTable.RowChanged += rowChanged;
					break;
				case CollectionChangeAction.Remove:
					dataTable.ColumnChanged -= columnChanged;
					dataTable.RowDeleted -= rowDeleted;
					dataTable.RowChanged -= rowChanged;
					break;
				}
			}
			finally
			{
				raiseDocumentEvents = flag;
			}
		}

		private void OnDataTableColumnChanged(object sender, DataColumnChangeEventArgs eventArgs)
		{
			if (!raiseDataSetEvents)
			{
				return;
			}
			bool flag = raiseDocumentEvents;
			raiseDocumentEvents = false;
			try
			{
				DataRow row = eventArgs.Row;
				XmlElement elementFromRow = GetElementFromRow(row);
				if (elementFromRow == null)
				{
					return;
				}
				DataColumn column = eventArgs.Column;
				string text = ((!row.IsNull(column)) ? row[column].ToString() : string.Empty);
				switch (column.ColumnMapping)
				{
				case MappingType.Attribute:
					elementFromRow.SetAttribute(XmlHelper.Encode(column.ColumnName), column.Namespace, text);
					break;
				case MappingType.SimpleContent:
					elementFromRow.InnerText = text;
					break;
				case MappingType.Element:
				{
					bool flag2 = false;
					for (int i = 0; i < elementFromRow.ChildNodes.Count; i++)
					{
						XmlElement xmlElement = elementFromRow.ChildNodes[i] as XmlElement;
						if (xmlElement != null && xmlElement.LocalName == XmlHelper.Encode(column.ColumnName) && xmlElement.NamespaceURI == column.Namespace)
						{
							flag2 = true;
							xmlElement.InnerText = text;
							break;
						}
					}
					if (!flag2)
					{
						XmlElement xmlElement2 = CreateElement(column.Prefix, XmlHelper.Encode(column.ColumnName), column.Namespace);
						xmlElement2.InnerText = text;
						elementFromRow.AppendChild(xmlElement2);
					}
					break;
				}
				}
			}
			finally
			{
				raiseDocumentEvents = flag;
			}
		}

		private void OnDataTableRowDeleted(object sender, DataRowChangeEventArgs eventArgs)
		{
			if (!raiseDataSetEvents)
			{
				return;
			}
			bool flag = raiseDocumentEvents;
			raiseDocumentEvents = false;
			try
			{
				XmlElement elementFromRow = GetElementFromRow(eventArgs.Row);
				if (elementFromRow != null)
				{
					elementFromRow.ParentNode.RemoveChild(elementFromRow);
				}
			}
			finally
			{
				raiseDocumentEvents = flag;
			}
		}

		[System.MonoTODO("Need to handle hidden columns? - see comments on each private method")]
		private void OnDataTableRowChanged(object sender, DataRowChangeEventArgs eventArgs)
		{
			if (!raiseDataSetEvents)
			{
				return;
			}
			bool flag = raiseDocumentEvents;
			raiseDocumentEvents = false;
			try
			{
				switch (eventArgs.Action)
				{
				case DataRowAction.Delete:
					OnDataTableRowDeleted(sender, eventArgs);
					break;
				case DataRowAction.Add:
					OnDataTableRowAdded(eventArgs);
					break;
				case DataRowAction.Rollback:
					OnDataTableRowRollback(eventArgs);
					break;
				}
			}
			finally
			{
				raiseDocumentEvents = flag;
			}
		}

		private void OnDataTableRowAdded(DataRowChangeEventArgs args)
		{
			if (!raiseDataSetEvents)
			{
				return;
			}
			bool flag = raiseDocumentEvents;
			raiseDocumentEvents = false;
			try
			{
				DataRow row = args.Row;
				if (base.DocumentElement == null)
				{
					AppendChild(CreateElement(XmlHelper.Encode(DataSet.DataSetName)));
				}
				DataTable table = args.Row.Table;
				XmlElement xmlElement = GetElementFromRow(row);
				if (xmlElement == null)
				{
					xmlElement = CreateElement(table.Prefix, XmlHelper.Encode(table.TableName), table.Namespace);
				}
				if (xmlElement.ChildNodes.Count == 0)
				{
					FillNodeChildrenFromRow(row, xmlElement);
				}
				if (xmlElement.ParentNode != null)
				{
					return;
				}
				XmlElement xmlElement2 = null;
				if (table.ParentRelations.Count > 0)
				{
					for (int i = 0; i < table.ParentRelations.Count; i++)
					{
						DataRelation relation = table.ParentRelations[i];
						DataRow parentRow = row.GetParentRow(relation);
						if (parentRow != null)
						{
							xmlElement2 = GetElementFromRow(parentRow);
						}
					}
				}
				if (xmlElement2 == null)
				{
					xmlElement2 = base.DocumentElement;
				}
				xmlElement2.AppendChild(xmlElement);
			}
			finally
			{
				raiseDocumentEvents = flag;
			}
		}

		private void FillNodeChildrenFromRow(DataRow row, XmlElement element)
		{
			DataTable table = row.Table;
			for (int i = 0; i < table.Columns.Count; i++)
			{
				DataColumn dataColumn = table.Columns[i];
				string text = ((!row.IsNull(dataColumn)) ? row[dataColumn].ToString() : string.Empty);
				switch (dataColumn.ColumnMapping)
				{
				case MappingType.Element:
				{
					XmlElement xmlElement = CreateElement(dataColumn.Prefix, XmlHelper.Encode(dataColumn.ColumnName), dataColumn.Namespace);
					xmlElement.InnerText = text;
					element.AppendChild(xmlElement);
					break;
				}
				case MappingType.Attribute:
				{
					XmlAttribute xmlAttribute = CreateAttribute(dataColumn.Prefix, XmlHelper.Encode(dataColumn.ColumnName), dataColumn.Namespace);
					xmlAttribute.Value = text;
					element.SetAttributeNode(xmlAttribute);
					break;
				}
				case MappingType.SimpleContent:
				{
					XmlText newChild = CreateTextNode(text);
					element.AppendChild(newChild);
					break;
				}
				}
			}
		}

		[System.MonoTODO("It does not look complete.")]
		private void OnDataTableRowRollback(DataRowChangeEventArgs args)
		{
			if (!raiseDataSetEvents)
			{
				return;
			}
			bool flag = raiseDocumentEvents;
			raiseDocumentEvents = false;
			try
			{
				DataRow row = args.Row;
				XmlElement elementFromRow = GetElementFromRow(row);
				if (elementFromRow == null)
				{
					return;
				}
				DataTable table = row.Table;
				ArrayList arrayList = new ArrayList();
				foreach (XmlAttribute attribute in elementFromRow.Attributes)
				{
					DataColumn dataColumn = table.Columns[XmlHelper.Decode(attribute.LocalName)];
					if (dataColumn != null)
					{
						if (row.IsNull(dataColumn))
						{
							arrayList.Add(attribute);
						}
						else
						{
							attribute.Value = row[dataColumn].ToString();
						}
					}
				}
				foreach (XmlAttribute item in arrayList)
				{
					elementFromRow.RemoveAttributeNode(item);
				}
				arrayList.Clear();
				foreach (XmlNode childNode in elementFromRow.ChildNodes)
				{
					if (childNode.NodeType != XmlNodeType.Element)
					{
						continue;
					}
					DataColumn dataColumn2 = table.Columns[XmlHelper.Decode(childNode.LocalName)];
					if (dataColumn2 != null)
					{
						if (row.IsNull(dataColumn2))
						{
							arrayList.Add(childNode);
						}
						else
						{
							childNode.InnerText = row[dataColumn2].ToString();
						}
					}
				}
				foreach (XmlNode item2 in arrayList)
				{
					elementFromRow.RemoveChild(item2);
				}
			}
			finally
			{
				raiseDocumentEvents = flag;
			}
		}

		private void InitDelegateFields()
		{
			columnChanged = OnDataTableColumnChanged;
			rowDeleted = OnDataTableRowDeleted;
			rowChanged = OnDataTableRowChanged;
			tablesChanged = OnDataTableChanged;
		}

		private void RemoveXmlDocumentListeners()
		{
			base.NodeInserting -= OnNodeInserting;
			base.NodeInserted -= OnNodeInserted;
			base.NodeChanging -= OnNodeChanging;
			base.NodeChanged -= OnNodeChanged;
			base.NodeRemoving -= OnNodeRemoving;
			base.NodeRemoved -= OnNodeRemoved;
		}

		private void AddXmlDocumentListeners()
		{
			base.NodeInserting += OnNodeInserting;
			base.NodeInserted += OnNodeInserted;
			base.NodeChanging += OnNodeChanging;
			base.NodeChanged += OnNodeChanged;
			base.NodeRemoving += OnNodeRemoving;
			base.NodeRemoved += OnNodeRemoved;
		}

		internal static object StringToObject(Type type, string value)
		{
			if (value == null || value == string.Empty)
			{
				return DBNull.Value;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return XmlConvert.ToBoolean(value);
			case TypeCode.Byte:
				return XmlConvert.ToByte(value);
			case TypeCode.Char:
				return (char)XmlConvert.ToInt32(value);
			case TypeCode.DateTime:
				return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified);
			case TypeCode.Decimal:
				return XmlConvert.ToDecimal(value);
			case TypeCode.Double:
				return XmlConvert.ToDouble(value);
			case TypeCode.Int16:
				return XmlConvert.ToInt16(value);
			case TypeCode.Int32:
				return XmlConvert.ToInt32(value);
			case TypeCode.Int64:
				return XmlConvert.ToInt64(value);
			case TypeCode.SByte:
				return XmlConvert.ToSByte(value);
			case TypeCode.Single:
				return XmlConvert.ToSingle(value);
			case TypeCode.UInt16:
				return XmlConvert.ToUInt16(value);
			case TypeCode.UInt32:
				return XmlConvert.ToUInt32(value);
			case TypeCode.UInt64:
				return XmlConvert.ToUInt64(value);
			default:
				if (type == typeof(TimeSpan))
				{
					return XmlConvert.ToTimeSpan(value);
				}
				if (type == typeof(Guid))
				{
					return XmlConvert.ToGuid(value);
				}
				if (type == typeof(byte[]))
				{
					return Convert.FromBase64String(value);
				}
				return Convert.ChangeType(value, type);
			}
		}
	}
}
