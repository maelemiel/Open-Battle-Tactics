using System.Collections.Generic;
using System.Data;
using System.Xml;

internal class XmlTableWriter
{
	internal static void WriteTables(XmlWriter writer, XmlWriteMode mode, List<DataTable> tables, List<DataRelation> relations, string mainDataTable, string dataSetName)
	{
		if (mode == XmlWriteMode.DiffGram)
		{
			foreach (DataTable table in tables)
			{
				table.SetRowsID();
			}
			DataSet.WriteDiffGramElement(writer);
		}
		bool flag = mode != XmlWriteMode.DiffGram;
		for (int i = 0; i < tables.Count; i++)
		{
			if (flag)
			{
				break;
			}
			flag = tables[i].Rows.Count > 0;
		}
		if (flag)
		{
			DataSet.WriteStartElement(writer, mode, tables[0].Namespace, tables[0].Prefix, XmlHelper.Encode(dataSetName));
			if (mode == XmlWriteMode.WriteSchema)
			{
				DataTable[] array = new DataTable[tables.Count];
				tables.CopyTo(array);
				DataRelation[] array2 = new DataRelation[relations.Count];
				relations.CopyTo(array2);
				DataTable dataTable = array[0];
				new XmlSchemaWriter(writer, array, array2, mainDataTable, dataSetName, (!dataTable.LocaleSpecified) ? null : dataTable.Locale).WriteSchema();
			}
			WriteTableList(writer, mode, tables, DataRowVersion.Default);
			writer.WriteEndElement();
		}
		if (mode == XmlWriteMode.DiffGram)
		{
			List<DataTable> list = new List<DataTable>();
			foreach (DataTable table2 in tables)
			{
				DataTable changes = table2.GetChanges(DataRowState.Deleted | DataRowState.Modified);
				if (changes != null && changes.Rows.Count > 0)
				{
					list.Add(changes);
				}
			}
			if (list.Count > 0)
			{
				DataSet.WriteStartElement(writer, XmlWriteMode.DiffGram, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "before");
				WriteTableList(writer, mode, list, DataRowVersion.Original);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		writer.Flush();
	}

	internal static void WriteTableList(XmlWriter writer, XmlWriteMode mode, List<DataTable> tables, DataRowVersion version)
	{
		foreach (DataTable table in tables)
		{
			DataSet.WriteTable(writer, table, mode, version);
		}
	}
}
