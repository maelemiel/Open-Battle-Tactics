using System.Xml;
using System.Xml.Serialization;

namespace System.Data
{
	internal class XmlDataReader
	{
		private const string xmlnsNS = "http://www.w3.org/2000/xmlns/";

		private DataSet dataset;

		private XmlReader reader;

		private XmlReadMode mode;

		public XmlDataReader(DataSet ds, XmlReader xr, XmlReadMode m)
		{
			dataset = ds;
			reader = xr;
			mode = m;
		}

		public static void ReadXml(DataSet dataset, XmlReader reader, XmlReadMode mode)
		{
			new XmlDataReader(dataset, reader, mode).Process();
		}

		private void Process()
		{
			bool enforceConstraints = dataset.EnforceConstraints;
			try
			{
				dataset.EnforceConstraints = false;
				reader.MoveToContent();
				if (mode == XmlReadMode.Fragment)
				{
					while (reader.NodeType == XmlNodeType.Element && !reader.EOF)
					{
						ReadTopLevelElement();
					}
				}
				else
				{
					ReadTopLevelElement();
				}
			}
			finally
			{
				dataset.EnforceConstraints = enforceConstraints;
			}
		}

		private bool IsTopLevelDataSet()
		{
			string name = XmlHelper.Decode(reader.LocalName);
			DataTable dataTable = dataset.Tables[name];
			if (dataTable == null)
			{
				return true;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = (XmlElement)xmlDocument.ReadNode(reader);
			xmlDocument.AppendChild(xmlElement);
			reader = new XmlNodeReader(xmlElement);
			reader.MoveToContent();
			return !XmlDataInferenceLoader.IsDocumentElementTable(xmlElement, null);
		}

		private void ReadTopLevelElement()
		{
			if (mode == XmlReadMode.Fragment && (XmlHelper.Decode(reader.LocalName) != dataset.DataSetName || reader.NamespaceURI != dataset.Namespace))
			{
				reader.Skip();
			}
			else if (mode == XmlReadMode.Fragment || IsTopLevelDataSet())
			{
				int depth = reader.Depth;
				reader.Read();
				reader.MoveToContent();
				do
				{
					ReadDataSetContent();
				}
				while (reader.Depth > depth && !reader.EOF);
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					reader.ReadEndElement();
				}
				reader.MoveToContent();
			}
			else
			{
				ReadDataSetContent();
			}
		}

		private void ReadDataSetContent()
		{
			DataTable dataTable = dataset.Tables[XmlHelper.Decode(reader.LocalName)];
			if (dataTable == null || dataTable.Namespace != reader.NamespaceURI)
			{
				reader.Skip();
				reader.MoveToContent();
			}
			else if (dataTable.Namespace != reader.NamespaceURI)
			{
				reader.Skip();
				reader.MoveToContent();
			}
			else
			{
				DataRow row = dataTable.NewRow();
				ReadElement(row);
				dataTable.Rows.Add(row);
			}
		}

		private void ReadElement(DataRow row)
		{
			if (reader.MoveToFirstAttribute())
			{
				do
				{
					if (!(reader.NamespaceURI == "http://www.w3.org/2000/xmlns/") && !(reader.NamespaceURI == "http://www.w3.org/XML/1998/namespace"))
					{
						ReadElementAttribute(row);
					}
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				reader.MoveToContent();
				return;
			}
			int depth = reader.Depth;
			reader.Read();
			reader.MoveToContent();
			do
			{
				ReadElementContent(row);
			}
			while (reader.Depth > depth && !reader.EOF);
			if (reader.IsEmptyElement)
			{
				reader.Read();
			}
			if (reader.NodeType == XmlNodeType.EndElement)
			{
				reader.ReadEndElement();
			}
			reader.MoveToContent();
		}

		private void ReadElementAttribute(DataRow row)
		{
			DataColumn dataColumn = row.Table.Columns[XmlHelper.Decode(reader.LocalName)];
			if (dataColumn != null && !(dataColumn.Namespace != reader.NamespaceURI))
			{
				row[dataColumn] = StringToObject(dataColumn.DataType, reader.Value);
			}
		}

		private void ReadElementContent(DataRow row)
		{
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
				ReadElementElement(row);
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.SignificantWhitespace:
			{
				DataColumn dataColumn = null;
				DataColumnCollection columns = row.Table.Columns;
				for (int i = 0; i < columns.Count; i++)
				{
					DataColumn dataColumn2 = columns[i];
					if (dataColumn2.ColumnMapping == MappingType.SimpleContent)
					{
						dataColumn = dataColumn2;
						break;
					}
				}
				string text = reader.ReadString();
				reader.MoveToContent();
				if (dataColumn != null)
				{
					DataRow dataRow2;
					DataRow dataRow = (dataRow2 = row);
					DataColumn column2;
					DataColumn column = (column2 = dataColumn);
					object obj = dataRow2[column2];
					dataRow[column] = string.Concat(obj, text);
				}
				break;
			}
			case XmlNodeType.Whitespace:
				reader.ReadString();
				break;
			}
		}

		private void ReadElementElement(DataRow row)
		{
			DataColumn dataColumn = row.Table.Columns[XmlHelper.Decode(reader.LocalName)];
			if (dataColumn == null || dataColumn.Namespace != reader.NamespaceURI)
			{
				dataColumn = null;
			}
			if (dataColumn != null && dataColumn.ColumnMapping == MappingType.Element)
			{
				if (dataColumn.Namespace != reader.NamespaceURI)
				{
					reader.Skip();
					return;
				}
				bool isEmptyElement = reader.IsEmptyElement;
				int depth = reader.Depth;
				if (typeof(IXmlSerializable).IsAssignableFrom(dataColumn.DataType))
				{
					try
					{
						IXmlSerializable xmlSerializable = (IXmlSerializable)Activator.CreateInstance(dataColumn.DataType, new object[0]);
						if (!reader.IsEmptyElement)
						{
							xmlSerializable.ReadXml(reader);
							reader.ReadEndElement();
						}
						else
						{
							reader.Skip();
						}
						row[dataColumn] = xmlSerializable;
					}
					catch (XmlException)
					{
						row[dataColumn] = reader.ReadInnerXml();
					}
					catch (InvalidOperationException)
					{
						row[dataColumn] = reader.ReadInnerXml();
					}
				}
				else
				{
					row[dataColumn] = StringToObject(dataColumn.DataType, reader.ReadElementString());
				}
				if (!isEmptyElement && reader.Depth > depth)
				{
					while (reader.Depth > depth)
					{
						reader.Read();
					}
					reader.Read();
				}
				reader.MoveToContent();
				return;
			}
			if (dataColumn != null)
			{
				reader.Skip();
				reader.MoveToContent();
				return;
			}
			DataRelationCollection childRelations = row.Table.ChildRelations;
			for (int i = 0; i < childRelations.Count; i++)
			{
				DataRelation dataRelation = childRelations[i];
				if (!dataRelation.Nested)
				{
					continue;
				}
				DataTable childTable = dataRelation.ChildTable;
				if (!(childTable.TableName != XmlHelper.Decode(reader.LocalName)) && !(childTable.Namespace != reader.NamespaceURI))
				{
					DataRow dataRow = dataRelation.ChildTable.NewRow();
					ReadElement(dataRow);
					for (int j = 0; j < dataRelation.ChildColumns.Length; j++)
					{
						dataRow[dataRelation.ChildColumns[j]] = row[dataRelation.ParentColumns[j]];
					}
					dataRelation.ChildTable.Rows.Add(dataRow);
					return;
				}
			}
			reader.Skip();
			reader.MoveToContent();
		}

		internal static object StringToObject(Type type, string value)
		{
			if (type == null)
			{
				return value;
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
				if (type == typeof(Type))
				{
					return Type.GetType(value);
				}
				return Convert.ChangeType(value, type);
			}
		}
	}
}
