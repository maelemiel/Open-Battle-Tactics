using System.Collections;
using System.Xml;

namespace System.Data
{
	internal class XmlDataLoader
	{
		private DataSet DSet;

		public XmlDataLoader(DataSet set)
		{
			DSet = set;
		}

		public XmlReadMode LoadData(XmlReader reader, XmlReadMode mode)
		{
			XmlReadMode result = mode;
			switch (mode)
			{
			case XmlReadMode.Auto:
				result = ((DSet.Tables.Count != 0) ? XmlReadMode.IgnoreSchema : XmlReadMode.InferSchema);
				ReadModeSchema(reader, (DSet.Tables.Count != 0) ? XmlReadMode.IgnoreSchema : XmlReadMode.Auto);
				break;
			case XmlReadMode.InferSchema:
				result = XmlReadMode.InferSchema;
				ReadModeSchema(reader, mode);
				break;
			case XmlReadMode.IgnoreSchema:
				result = XmlReadMode.IgnoreSchema;
				ReadModeSchema(reader, mode);
				break;
			default:
				reader.Skip();
				break;
			}
			return result;
		}

		private void ReadModeSchema(XmlReader reader, XmlReadMode mode)
		{
			bool flag = mode == XmlReadMode.InferSchema || mode == XmlReadMode.Auto;
			bool fillRows = mode != XmlReadMode.InferSchema;
			if (reader.LocalName == "schema")
			{
				if (mode != XmlReadMode.Auto)
				{
					reader.Skip();
				}
				else
				{
					DSet.ReadXmlSchema(reader);
				}
				reader.MoveToContent();
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(reader);
			if (xmlDocument.DocumentElement == null)
			{
				return;
			}
			switch (XmlNodeElementsDepth(xmlDocument.DocumentElement))
			{
			case 1:
				if (flag)
				{
					DSet.DataSetName = xmlDocument.DocumentElement.LocalName;
					DSet.Prefix = xmlDocument.DocumentElement.Prefix;
					DSet.Namespace = xmlDocument.DocumentElement.NamespaceURI;
				}
				return;
			case 2:
			{
				XmlDocument xmlDocument2 = new XmlDocument();
				XmlElement xmlElement = xmlDocument2.CreateElement("dummy");
				xmlDocument2.AppendChild(xmlElement);
				XmlNode newChild = xmlDocument2.ImportNode(xmlDocument.DocumentElement, true);
				xmlElement.AppendChild(newChild);
				xmlDocument = xmlDocument2;
				break;
			}
			default:
				if (flag)
				{
					DSet.DataSetName = xmlDocument.DocumentElement.LocalName;
					DSet.Prefix = xmlDocument.DocumentElement.Prefix;
					DSet.Namespace = xmlDocument.DocumentElement.NamespaceURI;
				}
				break;
			}
			bool enforceConstraints = DSet.EnforceConstraints;
			DSet.EnforceConstraints = false;
			XmlNodeList childNodes = xmlDocument.DocumentElement.ChildNodes;
			for (int i = 0; i < childNodes.Count; i++)
			{
				XmlNode xmlNode = childNodes[i];
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					AddRowToTable(xmlNode, null, flag, fillRows);
				}
			}
			DSet.EnforceConstraints = enforceConstraints;
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

		private void AddRowToTable(XmlNode tableNode, DataColumn relationColumn, bool inferSchema, bool fillRows)
		{
			Hashtable hashtable = new Hashtable();
			DataTable dataTable;
			if (DSet.Tables.Contains(tableNode.LocalName))
			{
				dataTable = DSet.Tables[tableNode.LocalName];
			}
			else
			{
				if (!inferSchema)
				{
					return;
				}
				dataTable = new DataTable(tableNode.LocalName);
				DSet.Tables.Add(dataTable);
			}
			if (!HaveChildElements(tableNode) && HaveText(tableNode) && !IsRepeatedHaveChildNodes(tableNode))
			{
				string text = tableNode.Name + "_Text";
				if (!dataTable.Columns.Contains(text))
				{
					dataTable.Columns.Add(text);
				}
				hashtable.Add(text, tableNode.InnerText);
			}
			XmlNodeList childNodes = tableNode.ChildNodes;
			for (int i = 0; i < childNodes.Count; i++)
			{
				XmlNode xmlNode = childNodes[i];
				if (xmlNode.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				if (IsInferredAsTable(xmlNode))
				{
					if (inferSchema)
					{
						string text2 = dataTable.TableName + "_Id";
						if (!dataTable.Columns.Contains(text2))
						{
							DataColumn dataColumn = new DataColumn(text2, typeof(int));
							dataColumn.AllowDBNull = false;
							dataColumn.AutoIncrement = true;
							dataColumn.ColumnMapping = MappingType.Hidden;
							dataTable.Columns.Add(dataColumn);
						}
						AddRowToTable(xmlNode, dataTable.Columns[text2], inferSchema, fillRows);
					}
					else
					{
						AddRowToTable(xmlNode, null, inferSchema, fillRows);
					}
				}
				else
				{
					object obj = null;
					obj = ((xmlNode.FirstChild == null) ? string.Empty : xmlNode.FirstChild.Value);
					if (dataTable.Columns.Contains(xmlNode.LocalName))
					{
						hashtable.Add(xmlNode.LocalName, obj);
					}
					else if (inferSchema)
					{
						dataTable.Columns.Add(xmlNode.LocalName);
						hashtable.Add(xmlNode.LocalName, obj);
					}
				}
			}
			XmlAttributeCollection attributes = tableNode.Attributes;
			for (int j = 0; j < attributes.Count; j++)
			{
				XmlAttribute xmlAttribute = attributes[j];
				if (xmlAttribute.Prefix.Equals("xmlns"))
				{
					dataTable.Namespace = xmlAttribute.Value;
					continue;
				}
				if (!dataTable.Columns.Contains(xmlAttribute.LocalName))
				{
					DataColumn dataColumn2 = dataTable.Columns.Add(xmlAttribute.LocalName);
					dataColumn2.ColumnMapping = MappingType.Attribute;
				}
				dataTable.Columns[xmlAttribute.LocalName].Namespace = dataTable.Namespace;
				hashtable.Add(xmlAttribute.LocalName, xmlAttribute.Value);
			}
			if (relationColumn != null)
			{
				if (!dataTable.Columns.Contains(relationColumn.ColumnName))
				{
					DataColumn dataColumn3 = new DataColumn(relationColumn.ColumnName, typeof(int));
					dataColumn3.ColumnMapping = MappingType.Hidden;
					dataTable.Columns.Add(dataColumn3);
					DataRelation dataRelation = new DataRelation(relationColumn.Table.TableName + "_" + dataColumn3.Table.TableName, relationColumn, dataColumn3);
					dataRelation.Nested = true;
					DSet.Relations.Add(dataRelation);
					UniqueConstraint.SetAsPrimaryKey(dataRelation.ParentTable.Constraints, dataRelation.ParentKeyConstraint);
				}
				hashtable.Add(relationColumn.ColumnName, relationColumn.GetAutoIncrementValue());
			}
			DataRow dataRow = dataTable.NewRow();
			IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				dataRow[enumerator.Key.ToString()] = StringToObject(dataTable.Columns[enumerator.Key.ToString()].DataType, enumerator.Value.ToString());
			}
			if (fillRows)
			{
				dataTable.Rows.Add(dataRow);
			}
		}

		private static int XmlNodeElementsDepth(XmlNode node)
		{
			int num = -1;
			if (node != null)
			{
				if (node.HasChildNodes && node.FirstChild.NodeType == XmlNodeType.Element)
				{
					for (int i = 0; i < node.ChildNodes.Count; i++)
					{
						if (node.ChildNodes[i].NodeType == XmlNodeType.Element)
						{
							int num2 = XmlNodeElementsDepth(node.ChildNodes[i]);
							num = ((num >= num2) ? num : num2);
						}
					}
					return num + 1;
				}
				return 1;
			}
			return -1;
		}

		private bool HaveChildElements(XmlNode node)
		{
			bool result = true;
			if (node.ChildNodes.Count > 0)
			{
				foreach (XmlNode childNode in node.ChildNodes)
				{
					if (childNode.NodeType != XmlNodeType.Element)
					{
						result = false;
						break;
					}
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool HaveText(XmlNode node)
		{
			bool result = true;
			if (node.ChildNodes.Count > 0)
			{
				foreach (XmlNode childNode in node.ChildNodes)
				{
					if (childNode.NodeType != XmlNodeType.Text)
					{
						result = false;
						break;
					}
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool IsRepeat(XmlNode node)
		{
			bool result = false;
			if (node.ParentNode != null)
			{
				foreach (XmlNode childNode in node.ParentNode.ChildNodes)
				{
					if (childNode != node && childNode.Name == node.Name)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private bool HaveAttributes(XmlNode node)
		{
			return node.Attributes != null && node.Attributes.Count > 0;
		}

		private bool IsInferredAsTable(XmlNode node)
		{
			return HaveChildElements(node) || HaveAttributes(node) || IsRepeat(node);
		}

		private bool IsRepeatedHaveChildNodes(XmlNode node)
		{
			bool result = false;
			if (node.ParentNode != null)
			{
				foreach (XmlNode childNode in node.ParentNode.ChildNodes)
				{
					if (childNode != node && childNode.Name == node.Name && HaveChildElements(childNode))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}
}
