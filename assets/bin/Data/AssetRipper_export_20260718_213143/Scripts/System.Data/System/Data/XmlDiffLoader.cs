using System.Collections;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	internal class XmlDiffLoader
	{
		private DataSet DSet;

		private DataTable table;

		private Hashtable DiffGrRows = new Hashtable();

		private Hashtable ErrorRows = new Hashtable();

		public XmlDiffLoader(DataSet DSet)
		{
			this.DSet = DSet;
		}

		public XmlDiffLoader(DataTable table)
		{
			this.table = table;
		}

		public void Load(XmlReader reader)
		{
			bool enforceConstraints = false;
			if (DSet != null)
			{
				enforceConstraints = DSet.EnforceConstraints;
				DSet.EnforceConstraints = false;
			}
			reader.MoveToContent();
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement("diffgram", "urn:schemas-microsoft-com:xml-diffgram-v1");
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.LocalName == "before" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
					{
						LoadBefore(reader);
					}
					else if (reader.LocalName == "errors" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
					{
						LoadErrors(reader);
					}
					else
					{
						LoadCurrent(reader);
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
			if (DSet != null)
			{
				DSet.EnforceConstraints = enforceConstraints;
			}
		}

		private void LoadCurrent(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					DataTable dataTable = GetTable(reader.LocalName);
					if (dataTable != null)
					{
						LoadCurrentTable(dataTable, reader);
					}
					else
					{
						reader.Skip();
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
		}

		private void LoadBefore(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					DataTable dataTable = GetTable(reader.LocalName);
					if (dataTable == null)
					{
						throw new DataException(global::Locale.GetText("Cannot load diffGram. Table '" + reader.LocalName + "' is missing in the destination dataset"));
					}
					LoadBeforeTable(dataTable, reader);
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
		}

		private void LoadErrors(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					DataRow dataRow = null;
					string attribute = reader.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
					if (attribute != null)
					{
						dataRow = (DataRow)ErrorRows[attribute];
					}
					if (reader.IsEmptyElement)
					{
						continue;
					}
					reader.ReadStartElement();
					while (reader.NodeType != XmlNodeType.EndElement)
					{
						if (reader.NodeType == XmlNodeType.Element)
						{
							string attribute2 = reader.GetAttribute("Error", "urn:schemas-microsoft-com:xml-diffgram-v1");
							dataRow.SetColumnError(reader.LocalName, attribute2);
						}
						reader.Read();
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
		}

		private void LoadColumns(DataTable Table, DataRow Row, XmlReader reader, DataRowVersion loadType)
		{
			LoadColumnAttributes(Table, Row, reader, loadType);
			LoadColumnChildren(Table, Row, reader, loadType);
		}

		private void LoadColumnAttributes(DataTable Table, DataRow Row, XmlReader reader, DataRowVersion loadType)
		{
			if (!reader.HasAttributes || !reader.MoveToFirstAttribute())
			{
				return;
			}
			do
			{
				switch (reader.NamespaceURI)
				{
				case "http://www.w3.org/2000/xmlns/":
				case "http://www.w3.org/XML/1998/namespace":
				case "urn:schemas-microsoft-com:xml-diffgram-v1":
				case "urn:schemas-microsoft-com:xml-msdata":
				case "urn:schemas-microsoft-com:xml-msprop":
				case "http://www.w3.org/2001/XMLSchema":
					continue;
				}
				DataColumn dataColumn = Table.Columns[XmlHelper.Decode(reader.LocalName)];
				if (dataColumn != null && dataColumn.ColumnMapping == MappingType.Attribute && ((dataColumn.Namespace == null && reader.NamespaceURI == string.Empty) || dataColumn.Namespace == reader.NamespaceURI))
				{
					object obj = XmlDataLoader.StringToObject(dataColumn.DataType, reader.Value);
					if (loadType == DataRowVersion.Current)
					{
						Row[dataColumn] = obj;
					}
					else
					{
						Row.SetOriginalValue(dataColumn.ColumnName, obj);
					}
				}
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
		}

		private void LoadColumnChildren(DataTable Table, DataRow Row, XmlReader reader, DataRowVersion loadType)
		{
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return;
			}
			reader.ReadStartElement();
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType != XmlNodeType.Element)
				{
					reader.Read();
					continue;
				}
				string text = XmlHelper.Decode(reader.LocalName);
				if (Table.Columns.Contains(text))
				{
					object obj = XmlDataLoader.StringToObject(Table.Columns[text].DataType, reader.ReadString());
					if (loadType == DataRowVersion.Current)
					{
						Row[text] = obj;
					}
					else
					{
						Row.SetOriginalValue(text, obj);
					}
					reader.Read();
					continue;
				}
				DataTable dataTable = GetTable(reader.LocalName);
				if (dataTable != null)
				{
					switch (loadType)
					{
					case DataRowVersion.Original:
						LoadBeforeTable(dataTable, reader);
						break;
					case DataRowVersion.Current:
						LoadCurrentTable(dataTable, reader);
						break;
					}
				}
				else
				{
					reader.Skip();
				}
			}
			reader.ReadEndElement();
		}

		private void LoadBeforeTable(DataTable Table, XmlReader reader)
		{
			string attribute = reader.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
			string attribute2 = reader.GetAttribute("rowOrder", "urn:schemas-microsoft-com:xml-msdata");
			DataRow dataRow = (DataRow)DiffGrRows[attribute];
			if (dataRow == null)
			{
				dataRow = Table.NewRow();
				LoadColumns(Table, dataRow, reader, DataRowVersion.Current);
				Table.Rows.InsertAt(dataRow, int.Parse(attribute2));
				dataRow.AcceptChanges();
				dataRow.Delete();
			}
			else
			{
				LoadColumns(Table, dataRow, reader, DataRowVersion.Original);
			}
		}

		private void LoadCurrentTable(DataTable Table, XmlReader reader)
		{
			DataRow dataRow = Table.NewRow();
			string attribute = reader.GetAttribute("id", "urn:schemas-microsoft-com:xml-diffgram-v1");
			string attribute2 = reader.GetAttribute("hasErrors");
			string attribute3 = reader.GetAttribute("hasChanges", "urn:schemas-microsoft-com:xml-diffgram-v1");
			DataRowState dataRowState;
			if (attribute3 != null)
			{
				if (string.Compare(attribute3, "modified", true, CultureInfo.InvariantCulture) == 0)
				{
					DiffGrRows.Add(attribute, dataRow);
					dataRowState = DataRowState.Modified;
				}
				else
				{
					if (string.Compare(attribute3, "inserted", true, CultureInfo.InvariantCulture) != 0)
					{
						throw new InvalidOperationException("Invalid row change state");
					}
					dataRowState = DataRowState.Added;
				}
			}
			else
			{
				dataRowState = DataRowState.Unchanged;
			}
			if (attribute2 != null && string.Compare(attribute2, "true", true, CultureInfo.InvariantCulture) == 0)
			{
				ErrorRows.Add(attribute, dataRow);
			}
			LoadColumns(Table, dataRow, reader, DataRowVersion.Current);
			Table.Rows.Add(dataRow);
			if (dataRowState != DataRowState.Added)
			{
				dataRow.AcceptChanges();
			}
		}

		private DataTable GetTable(string name)
		{
			if (DSet != null)
			{
				return DSet.Tables[name];
			}
			if (name == table.TableName)
			{
				return table;
			}
			return null;
		}
	}
}
