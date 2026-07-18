using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data
{
	[Serializable]
	[XmlRoot("DataSet")]
	[ToolboxItem("Microsoft.VSDesigner.Data.VS.DataSetToolboxItem, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultProperty("DataSetName")]
	[Designer("Microsoft.VSDesigner.Data.VS.DataSetDesigner, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[XmlSchemaProvider("GetDataSetSchema")]
	public class DataSet : MarshalByValueComponent, IXmlSerializable, ISerializable, IListSource, ISupportInitialize, ISupportInitializeNotification
	{
		private string dataSetName;

		private string _namespace = string.Empty;

		private string prefix;

		private bool caseSensitive;

		private bool enforceConstraints = true;

		private DataTableCollection tableCollection;

		private DataRelationCollection relationCollection;

		private PropertyCollection properties;

		private DataViewManager defaultView;

		private CultureInfo locale;

		internal XmlDataDocument _xmlDataDocument;

		internal TableAdapterSchemaInfo tableAdapterSchemaInfo;

		private bool initInProgress;

		private bool dataSetInitialized = true;

		private SerializationFormat remotingFormat;

		bool IListSource.ContainsListCollection
		{
			get
			{
				return true;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(false)]
		public bool CaseSensitive
		{
			get
			{
				return caseSensitive;
			}
			set
			{
				caseSensitive = value;
				if (!caseSensitive)
				{
					foreach (DataTable table in Tables)
					{
						table.ResetCaseSensitiveIndexes();
						foreach (Constraint constraint in table.Constraints)
						{
							constraint.AssertConstraint();
						}
					}
					return;
				}
				foreach (DataTable table2 in Tables)
				{
					table2.ResetCaseSensitiveIndexes();
				}
			}
		}

		[DefaultValue("")]
		[DataCategory("Data")]
		public string DataSetName
		{
			get
			{
				return dataSetName;
			}
			set
			{
				dataSetName = value;
			}
		}

		[Browsable(false)]
		public DataViewManager DefaultViewManager
		{
			get
			{
				if (defaultView == null)
				{
					defaultView = new DataViewManager(this);
				}
				return defaultView;
			}
		}

		[DefaultValue(true)]
		public bool EnforceConstraints
		{
			get
			{
				return enforceConstraints;
			}
			set
			{
				InternalEnforceConstraints(value, true);
			}
		}

		[DataCategory("Data")]
		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				return properties;
			}
		}

		[Browsable(false)]
		public bool HasErrors
		{
			get
			{
				for (int i = 0; i < Tables.Count; i++)
				{
					if (Tables[i].HasErrors)
					{
						return true;
					}
				}
				return false;
			}
		}

		[DataCategory("Data")]
		public CultureInfo Locale
		{
			get
			{
				return (locale == null) ? Thread.CurrentThread.CurrentCulture : locale;
			}
			set
			{
				if (locale == null || !locale.Equals(value))
				{
					locale = value;
				}
			}
		}

		internal bool LocaleSpecified
		{
			get
			{
				return locale != null;
			}
		}

		internal TableAdapterSchemaInfo TableAdapterSchemaData
		{
			get
			{
				return tableAdapterSchemaInfo;
			}
		}

		[DefaultValue("")]
		[DataCategory("Data")]
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (value != _namespace)
				{
					RaisePropertyChanging("Namespace");
				}
				_namespace = value;
			}
		}

		[DefaultValue("")]
		[DataCategory("Data")]
		public string Prefix
		{
			get
			{
				return prefix;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				for (int i = 0; i < value.Length; i++)
				{
					if (!char.IsLetterOrDigit(value[i]) && value[i] != '_' && value[i] != ':')
					{
						throw new DataException("Prefix '" + value + "' is not valid, because it contains special characters.");
					}
				}
				if (value != prefix)
				{
					RaisePropertyChanging("Prefix");
				}
				prefix = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DataCategory("Data")]
		public DataRelationCollection Relations
		{
			get
			{
				return relationCollection;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
			}
		}

		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataTableCollection Tables
		{
			get
			{
				return tableCollection;
			}
		}

		internal bool InitInProgress
		{
			get
			{
				return initInProgress;
			}
			set
			{
				initInProgress = value;
			}
		}

		[DefaultValue(SerializationFormat.Xml)]
		public SerializationFormat RemotingFormat
		{
			get
			{
				return remotingFormat;
			}
			set
			{
				remotingFormat = value;
			}
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return dataSetInitialized;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual SchemaSerializationMode SchemaSerializationMode
		{
			get
			{
				return SchemaSerializationMode.IncludeSchema;
			}
			set
			{
				if (value != SchemaSerializationMode.IncludeSchema)
				{
					throw new InvalidOperationException("Only IncludeSchema Mode can be set for Untyped DataSet");
				}
			}
		}

		[DataCategory("Action")]
		public event MergeFailedEventHandler MergeFailed;

		public event EventHandler Initialized;

		public DataSet()
			: this("NewDataSet")
		{
		}

		public DataSet(string dataSetName)
		{
			this.dataSetName = dataSetName;
			tableCollection = new DataTableCollection(this);
			relationCollection = new DataRelationCollection.DataSetRelationCollection(this);
			properties = new PropertyCollection();
			prefix = string.Empty;
		}

		protected DataSet(SerializationInfo info, StreamingContext context)
			: this()
		{
			if (IsBinarySerialized(info, context))
			{
				BinaryDeserialize(info);
				return;
			}
			string s = info.GetValue("XmlSchema", typeof(string)) as string;
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(s));
			ReadXmlSchema(xmlTextReader);
			xmlTextReader.Close();
			GetSerializationData(info, context);
		}

		protected DataSet(SerializationInfo info, StreamingContext context, bool constructSchema)
			: this()
		{
			if (DetermineSchemaSerializationMode(info, context) == SchemaSerializationMode.ExcludeSchema)
			{
				InitializeDerivedDataSet();
			}
			if (IsBinarySerialized(info, context))
			{
				BinaryDeserialize(info);
			}
			else if (constructSchema)
			{
				string s = info.GetValue("XmlSchema", typeof(string)) as string;
				XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(s));
				ReadXmlSchema(xmlTextReader);
				xmlTextReader.Close();
				GetSerializationData(info, context);
			}
		}

		IList IListSource.GetList()
		{
			return DefaultViewManager;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			ReadXmlSerializable(reader);
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			DoWriteXmlSchema(writer);
			WriteXml(writer, XmlWriteMode.DiffGram);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			if (GetType() == typeof(DataSet))
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(memoryStream, null);
			WriteXmlSchema(writer);
			memoryStream.Position = 0L;
			return XmlSchema.Read(new XmlTextReader(memoryStream), null);
		}

		internal void InternalEnforceConstraints(bool value, bool resetIndexes)
		{
			if (value == enforceConstraints)
			{
				return;
			}
			if (value)
			{
				if (resetIndexes)
				{
					foreach (DataTable table in Tables)
					{
						table.ResetIndexes();
					}
				}
				bool flag = false;
				foreach (DataTable table2 in Tables)
				{
					foreach (Constraint constraint in table2.Constraints)
					{
						constraint.AssertConstraint();
					}
					table2.AssertNotNullConstraints();
					if (!flag && table2.HasErrors)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Constraint.ThrowConstraintException();
				}
			}
			enforceConstraints = value;
		}

		public void Merge(DataRow[] rows)
		{
			Merge(rows, false, MissingSchemaAction.Add);
		}

		public void Merge(DataSet dataSet)
		{
			Merge(dataSet, false, MissingSchemaAction.Add);
		}

		public void Merge(DataTable table)
		{
			Merge(table, false, MissingSchemaAction.Add);
		}

		public void Merge(DataSet dataSet, bool preserveChanges)
		{
			Merge(dataSet, preserveChanges, MissingSchemaAction.Add);
		}

		public void Merge(DataRow[] rows, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (rows == null)
			{
				throw new ArgumentNullException("rows");
			}
			if (!IsLegalSchemaAction(missingSchemaAction))
			{
				throw new ArgumentOutOfRangeException("missingSchemaAction");
			}
			MergeManager.Merge(this, rows, preserveChanges, missingSchemaAction);
		}

		public void Merge(DataSet dataSet, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			if (!IsLegalSchemaAction(missingSchemaAction))
			{
				throw new ArgumentOutOfRangeException("missingSchemaAction");
			}
			MergeManager.Merge(this, dataSet, preserveChanges, missingSchemaAction);
		}

		public void Merge(DataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			if (!IsLegalSchemaAction(missingSchemaAction))
			{
				throw new ArgumentOutOfRangeException("missingSchemaAction");
			}
			MergeManager.Merge(this, table, preserveChanges, missingSchemaAction);
		}

		private static bool IsLegalSchemaAction(MissingSchemaAction missingSchemaAction)
		{
			if (missingSchemaAction == MissingSchemaAction.Add || missingSchemaAction == MissingSchemaAction.AddWithKey || missingSchemaAction == MissingSchemaAction.Error || missingSchemaAction == MissingSchemaAction.Ignore)
			{
				return true;
			}
			return false;
		}

		public void AcceptChanges()
		{
			foreach (DataTable item in tableCollection)
			{
				item.AcceptChanges();
			}
		}

		public void Clear()
		{
			if (_xmlDataDocument != null)
			{
				throw new NotSupportedException("Clear function on dataset and datatable is not supported when XmlDataDocument is bound to the DataSet.");
			}
			bool flag = EnforceConstraints;
			EnforceConstraints = false;
			for (int i = 0; i < tableCollection.Count; i++)
			{
				tableCollection[i].Clear();
			}
			EnforceConstraints = flag;
		}

		public virtual DataSet Clone()
		{
			DataSet dataSet = (DataSet)Activator.CreateInstance(GetType(), true);
			CopyProperties(dataSet);
			foreach (DataTable table in Tables)
			{
				if (!dataSet.Tables.Contains(table.TableName))
				{
					dataSet.Tables.Add(table.Clone());
				}
			}
			CopyRelations(dataSet);
			return dataSet;
		}

		public DataSet Copy()
		{
			DataSet dataSet = (DataSet)Activator.CreateInstance(GetType(), true);
			CopyProperties(dataSet);
			foreach (DataTable table in Tables)
			{
				if (!dataSet.Tables.Contains(table.TableName))
				{
					dataSet.Tables.Add(table.Copy());
					continue;
				}
				foreach (DataRow row in table.Rows)
				{
					dataSet.Tables[table.TableName].ImportRow(row);
				}
			}
			CopyRelations(dataSet);
			return dataSet;
		}

		private void CopyProperties(DataSet Copy)
		{
			Copy.CaseSensitive = CaseSensitive;
			Copy.DataSetName = DataSetName;
			Copy.EnforceConstraints = EnforceConstraints;
			if (ExtendedProperties.Count > 0)
			{
				Array array = Array.CreateInstance(typeof(object), ExtendedProperties.Count);
				ExtendedProperties.Keys.CopyTo(array, 0);
				for (int i = 0; i < ExtendedProperties.Count; i++)
				{
					Copy.ExtendedProperties.Add(array.GetValue(i), ExtendedProperties[array.GetValue(i)]);
				}
			}
			Copy.locale = locale;
			Copy.Namespace = Namespace;
			Copy.Prefix = Prefix;
		}

		private void CopyRelations(DataSet Copy)
		{
			foreach (DataRelation relation2 in Relations)
			{
				if (!Copy.Relations.Contains(relation2.RelationName))
				{
					string tableName = relation2.ParentTable.TableName;
					string tableName2 = relation2.ChildTable.TableName;
					DataColumn[] array = new DataColumn[relation2.ParentColumns.Length];
					DataColumn[] array2 = new DataColumn[relation2.ChildColumns.Length];
					int num = 0;
					DataColumn[] parentColumns = relation2.ParentColumns;
					foreach (DataColumn dataColumn in parentColumns)
					{
						array[num] = Copy.Tables[tableName].Columns[dataColumn.ColumnName];
						num++;
					}
					num = 0;
					DataColumn[] childColumns = relation2.ChildColumns;
					foreach (DataColumn dataColumn2 in childColumns)
					{
						array2[num] = Copy.Tables[tableName2].Columns[dataColumn2.ColumnName];
						num++;
					}
					DataRelation relation = new DataRelation(relation2.RelationName, array, array2, false);
					Copy.Relations.Add(relation);
				}
			}
			foreach (DataTable table in Tables)
			{
				foreach (Constraint constraint in table.Constraints)
				{
					if (constraint is ForeignKeyConstraint && !Copy.Tables[table.TableName].Constraints.Contains(constraint.ConstraintName))
					{
						ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)constraint;
						DataTable dataTable2 = Copy.Tables[foreignKeyConstraint.RelatedTable.TableName];
						DataTable dataTable3 = Copy.Tables[table.TableName];
						DataColumn[] array3 = new DataColumn[foreignKeyConstraint.RelatedColumns.Length];
						DataColumn[] array4 = new DataColumn[foreignKeyConstraint.Columns.Length];
						for (int k = 0; k < array3.Length; k++)
						{
							array3[k] = dataTable2.Columns[foreignKeyConstraint.RelatedColumns[k].ColumnName];
						}
						for (int l = 0; l < array4.Length; l++)
						{
							array4[l] = dataTable3.Columns[foreignKeyConstraint.Columns[l].ColumnName];
						}
						dataTable3.Constraints.Add(foreignKeyConstraint.ConstraintName, array3, array4);
					}
				}
			}
		}

		public DataSet GetChanges()
		{
			return GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
		}

		public DataSet GetChanges(DataRowState rowStates)
		{
			if (!HasChanges(rowStates))
			{
				return null;
			}
			DataSet dataSet = Clone();
			bool flag = dataSet.EnforceConstraints;
			dataSet.EnforceConstraints = false;
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < Tables.Count; i++)
			{
				DataTable dataTable = Tables[i];
				DataTable copyTable = dataSet.Tables[dataTable.TableName];
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					DataRow dataRow = dataTable.Rows[j];
					if (dataRow.IsRowChanged(rowStates) && !hashtable.Contains(dataRow))
					{
						AddChangedRow(hashtable, copyTable, dataRow);
					}
				}
			}
			dataSet.EnforceConstraints = flag;
			return dataSet;
		}

		private void AddChangedRow(Hashtable addedRows, DataTable copyTable, DataRow row)
		{
			if (addedRows.ContainsKey(row))
			{
				return;
			}
			foreach (DataRelation parentRelation in row.Table.ParentRelations)
			{
				DataRow dataRow = ((row.RowState == DataRowState.Deleted) ? row.GetParentRow(parentRelation, DataRowVersion.Original) : row.GetParentRow(parentRelation));
				if (dataRow != null)
				{
					DataTable copyTable2 = copyTable.DataSet.Tables[dataRow.Table.TableName];
					AddChangedRow(addedRows, copyTable2, dataRow);
				}
			}
			DataRow dataRow2 = copyTable.NewNotInitializedRow();
			copyTable.Rows.AddInternal(dataRow2);
			row.CopyValuesToRow(dataRow2);
			dataRow2.XmlRowID = row.XmlRowID;
			addedRows.Add(row, row);
		}

		public string GetXml()
		{
			StringWriter stringWriter = new StringWriter();
			WriteXml(stringWriter, XmlWriteMode.IgnoreSchema);
			return stringWriter.ToString();
		}

		public string GetXmlSchema()
		{
			StringWriter stringWriter = new StringWriter();
			WriteXmlSchema(stringWriter);
			return stringWriter.ToString();
		}

		public bool HasChanges()
		{
			return HasChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
		}

		public bool HasChanges(DataRowState rowStates)
		{
			if (((ulong)rowStates & 0xFFFFFFE0uL) != 0L)
			{
				throw new ArgumentOutOfRangeException("rowStates");
			}
			DataTableCollection tables = Tables;
			for (int i = 0; i < tables.Count; i++)
			{
				DataTable dataTable = tables[i];
				DataRowCollection rows = dataTable.Rows;
				for (int j = 0; j < rows.Count; j++)
				{
					DataRow dataRow = rows[j];
					if ((dataRow.RowState & rowStates) != 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void InferXmlSchema(XmlReader reader, string[] nsArray)
		{
			if (reader != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(reader);
				InferXmlSchema(xmlDocument, nsArray);
			}
		}

		private void InferXmlSchema(XmlDocument doc, string[] nsArray)
		{
			XmlDataInferenceLoader.Infer(this, doc, XmlReadMode.InferSchema, nsArray);
		}

		public void InferXmlSchema(Stream stream, string[] nsArray)
		{
			InferXmlSchema(new XmlTextReader(stream), nsArray);
		}

		public void InferXmlSchema(TextReader reader, string[] nsArray)
		{
			InferXmlSchema(new XmlTextReader(reader), nsArray);
		}

		public void InferXmlSchema(string fileName, string[] nsArray)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(fileName);
			try
			{
				InferXmlSchema(xmlTextReader, nsArray);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		public virtual void RejectChanges()
		{
			bool flag = EnforceConstraints;
			EnforceConstraints = false;
			for (int i = 0; i < Tables.Count; i++)
			{
				Tables[i].RejectChanges();
			}
			EnforceConstraints = flag;
		}

		public virtual void Reset()
		{
			for (int i = 0; i < Tables.Count; i++)
			{
				ConstraintCollection constraints = Tables[i].Constraints;
				for (int j = 0; j < constraints.Count; j++)
				{
					if (constraints[j] is ForeignKeyConstraint)
					{
						constraints.Remove(constraints[j]);
					}
				}
			}
			Clear();
			Relations.Clear();
			Tables.Clear();
		}

		public void WriteXml(Stream stream)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			WriteXml(xmlTextWriter);
		}

		public void WriteXml(string fileName)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartDocument(true);
			try
			{
				WriteXml(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Close();
			}
		}

		public void WriteXml(TextWriter writer)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(writer);
			xmlTextWriter.Formatting = Formatting.Indented;
			WriteXml(xmlTextWriter);
		}

		public void WriteXml(XmlWriter writer)
		{
			WriteXml(writer, XmlWriteMode.IgnoreSchema);
		}

		public void WriteXml(string fileName, XmlWriteMode mode)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartDocument(true);
			try
			{
				WriteXml(xmlTextWriter, mode);
			}
			finally
			{
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Close();
			}
		}

		public void WriteXml(Stream stream, XmlWriteMode mode)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			WriteXml(xmlTextWriter, mode);
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(writer);
			xmlTextWriter.Formatting = Formatting.Indented;
			WriteXml(xmlTextWriter, mode);
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode)
		{
			if (mode == XmlWriteMode.DiffGram)
			{
				SetRowsID();
				WriteDiffGramElement(writer);
			}
			bool flag = mode != XmlWriteMode.DiffGram;
			for (int i = 0; i < tableCollection.Count; i++)
			{
				if (flag)
				{
					break;
				}
				flag = tableCollection[i].Rows.Count > 0;
			}
			if (flag)
			{
				WriteStartElement(writer, mode, Namespace, Prefix, XmlHelper.Encode(DataSetName));
				if (mode == XmlWriteMode.WriteSchema)
				{
					DoWriteXmlSchema(writer);
				}
				WriteTables(writer, mode, Tables, DataRowVersion.Default);
				writer.WriteEndElement();
			}
			if (mode == XmlWriteMode.DiffGram && HasChanges(DataRowState.Deleted | DataRowState.Modified))
			{
				DataSet changes = GetChanges(DataRowState.Deleted | DataRowState.Modified);
				WriteStartElement(writer, XmlWriteMode.DiffGram, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "before");
				WriteTables(writer, mode, changes.Tables, DataRowVersion.Original);
				writer.WriteEndElement();
			}
			if (mode == XmlWriteMode.DiffGram)
			{
				writer.WriteEndElement();
			}
			writer.Flush();
		}

		public void WriteXmlSchema(Stream stream)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			WriteXmlSchema(xmlTextWriter);
		}

		public void WriteXmlSchema(string fileName)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, null);
			try
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartDocument(true);
				WriteXmlSchema(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Close();
			}
		}

		public void WriteXmlSchema(TextWriter writer)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(writer);
			try
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				WriteXmlSchema(xmlTextWriter);
			}
			finally
			{
				xmlTextWriter.Close();
			}
		}

		public void WriteXmlSchema(XmlWriter writer)
		{
			DoWriteXmlSchema(writer);
		}

		public void ReadXmlSchema(Stream stream)
		{
			XmlReader reader = new XmlTextReader(stream, null);
			ReadXmlSchema(reader);
		}

		public void ReadXmlSchema(string fileName)
		{
			XmlReader xmlReader = new XmlTextReader(fileName);
			try
			{
				ReadXmlSchema(xmlReader);
			}
			finally
			{
				xmlReader.Close();
			}
		}

		public void ReadXmlSchema(TextReader reader)
		{
			XmlReader reader2 = new XmlTextReader(reader);
			ReadXmlSchema(reader2);
		}

		public void ReadXmlSchema(XmlReader reader)
		{
			XmlSchemaDataImporter xmlSchemaDataImporter = new XmlSchemaDataImporter(this, reader, true);
			xmlSchemaDataImporter.Process();
			tableAdapterSchemaInfo = xmlSchemaDataImporter.CurrentAdapter;
		}

		public XmlReadMode ReadXml(Stream stream)
		{
			return ReadXml(new XmlTextReader(stream));
		}

		public XmlReadMode ReadXml(string fileName)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(fileName);
			try
			{
				return ReadXml(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		public XmlReadMode ReadXml(TextReader reader)
		{
			return ReadXml(new XmlTextReader(reader));
		}

		public XmlReadMode ReadXml(XmlReader reader)
		{
			return ReadXml(reader, XmlReadMode.Auto);
		}

		public XmlReadMode ReadXml(Stream stream, XmlReadMode mode)
		{
			return ReadXml(new XmlTextReader(stream), mode);
		}

		public XmlReadMode ReadXml(string fileName, XmlReadMode mode)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(fileName);
			try
			{
				return ReadXml(xmlTextReader, mode);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		public XmlReadMode ReadXml(TextReader reader, XmlReadMode mode)
		{
			return ReadXml(new XmlTextReader(reader), mode);
		}

		public XmlReadMode ReadXml(XmlReader reader, XmlReadMode mode)
		{
			if (reader == null)
			{
				return mode;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return mode;
			default:
			{
				reader.MoveToContent();
				if (reader.EOF)
				{
					return mode;
				}
				if (reader is XmlTextReader)
				{
					((XmlTextReader)reader).WhitespaceHandling = WhitespaceHandling.None;
				}
				XmlDiffLoader xmlDiffLoader = null;
				if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
				{
					switch (mode)
					{
					case XmlReadMode.Auto:
					case XmlReadMode.DiffGram:
						if (xmlDiffLoader == null)
						{
							xmlDiffLoader = new XmlDiffLoader(this);
						}
						xmlDiffLoader.Load(reader);
						return XmlReadMode.DiffGram;
					case XmlReadMode.Fragment:
						break;
					default:
						reader.Skip();
						return mode;
					}
					reader.Skip();
				}
				if (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					switch (mode)
					{
					case XmlReadMode.IgnoreSchema:
					case XmlReadMode.InferSchema:
						reader.Skip();
						return mode;
					case XmlReadMode.Fragment:
						break;
					case XmlReadMode.Auto:
						if (Tables.Count == 0)
						{
							ReadXmlSchema(reader);
							return XmlReadMode.ReadSchema;
						}
						reader.Skip();
						return XmlReadMode.IgnoreSchema;
					default:
						ReadXmlSchema(reader);
						return mode;
					}
					ReadXmlSchema(reader);
				}
				if (reader.EOF)
				{
					return mode;
				}
				int num = ((reader.NodeType != XmlNodeType.Element) ? (-1) : reader.Depth);
				XmlDocument xmlDocument = new XmlDocument();
				XmlElement xmlElement = xmlDocument.CreateElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
				if (reader.HasAttributes)
				{
					for (int i = 0; i < reader.AttributeCount; i++)
					{
						reader.MoveToAttribute(i);
						if (reader.NamespaceURI == "http://www.w3.org/2000/xmlns/")
						{
							xmlElement.SetAttribute(reader.Name, reader.GetAttribute(i));
							continue;
						}
						XmlAttribute xmlAttribute = xmlElement.SetAttributeNode(reader.LocalName, reader.NamespaceURI);
						xmlAttribute.Prefix = reader.Prefix;
						xmlAttribute.Value = reader.GetAttribute(i);
					}
				}
				reader.Read();
				XmlReadMode xmlReadMode = mode;
				bool flag = false;
				while (reader.Depth != num && reader.NodeType != XmlNodeType.EndElement)
				{
					if (reader.NodeType != XmlNodeType.Element)
					{
						if (!reader.Read())
						{
							break;
						}
					}
					else if (reader.LocalName == "schema" && reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
					{
						if (mode == XmlReadMode.IgnoreSchema || mode == XmlReadMode.InferSchema)
						{
							reader.Skip();
							continue;
						}
						ReadXmlSchema(reader);
						xmlReadMode = XmlReadMode.ReadSchema;
						flag = true;
					}
					else if (reader.LocalName == "diffgram" && reader.NamespaceURI == "urn:schemas-microsoft-com:xml-diffgram-v1")
					{
						if (mode == XmlReadMode.DiffGram || mode == XmlReadMode.IgnoreSchema || mode == XmlReadMode.Auto)
						{
							if (xmlDiffLoader == null)
							{
								xmlDiffLoader = new XmlDiffLoader(this);
							}
							xmlDiffLoader.Load(reader);
							xmlReadMode = XmlReadMode.DiffGram;
						}
						else
						{
							reader.Skip();
						}
					}
					else
					{
						XmlNode newChild = xmlDocument.ReadNode(reader);
						xmlElement.AppendChild(newChild);
					}
				}
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					reader.Read();
				}
				reader.MoveToContent();
				if (mode == XmlReadMode.DiffGram)
				{
					return xmlReadMode;
				}
				xmlDocument.AppendChild(xmlElement);
				if (!flag && xmlReadMode != XmlReadMode.ReadSchema && mode != XmlReadMode.IgnoreSchema && mode != XmlReadMode.Fragment && (Tables.Count == 0 || mode == XmlReadMode.InferSchema))
				{
					InferXmlSchema(xmlDocument, null);
					if (mode == XmlReadMode.Auto)
					{
						xmlReadMode = XmlReadMode.InferSchema;
					}
				}
				reader = new XmlNodeReader(xmlDocument);
				XmlDataReader.ReadXml(this, reader, mode);
				return (xmlReadMode != XmlReadMode.Auto) ? xmlReadMode : XmlReadMode.IgnoreSchema;
			}
			}
		}

		public void BeginInit()
		{
			InitInProgress = true;
			dataSetInitialized = false;
		}

		public void EndInit()
		{
			Tables.PostAddRange();
			for (int i = 0; i < Tables.Count; i++)
			{
				if (Tables[i].InitInProgress)
				{
					Tables[i].FinishInit();
				}
			}
			Relations.PostAddRange();
			InitInProgress = false;
			dataSetInitialized = true;
			DataSetInitialized();
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (RemotingFormat == SerializationFormat.Xml)
			{
				info.AddValue("SchemaSerializationMode.DataSet", SchemaSerializationMode);
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				DoWriteXmlSchema(xmlTextWriter);
				xmlTextWriter.Flush();
				info.AddValue("XmlSchema", stringWriter.ToString());
				stringWriter = new StringWriter();
				xmlTextWriter = new XmlTextWriter(stringWriter);
				WriteXml(xmlTextWriter, XmlWriteMode.DiffGram);
				xmlTextWriter.Flush();
				info.AddValue("XmlDiffGram", stringWriter.ToString());
			}
			else
			{
				BinarySerialize(info);
			}
		}

		protected void GetSerializationData(SerializationInfo info, StreamingContext context)
		{
			string s = info.GetValue("XmlDiffGram", typeof(string)) as string;
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(s));
			ReadXml(xmlTextReader, XmlReadMode.DiffGram);
			xmlTextReader.Close();
		}

		protected virtual XmlSchema GetSchemaSerializable()
		{
			return null;
		}

		protected virtual void ReadXmlSerializable(XmlReader reader)
		{
			ReadXml(reader, XmlReadMode.DiffGram);
		}

		protected virtual bool ShouldSerializeRelations()
		{
			return true;
		}

		protected virtual bool ShouldSerializeTables()
		{
			return true;
		}

		[System.MonoTODO]
		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected virtual void OnRemoveRelation(DataRelation relation)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected virtual void OnRemoveTable(DataTable table)
		{
			throw new NotImplementedException();
		}

		internal virtual void OnMergeFailed(MergeFailedEventArgs e)
		{
			if (this.MergeFailed != null)
			{
				this.MergeFailed(this, e);
				return;
			}
			throw new DataException(e.Conflict);
		}

		[System.MonoTODO]
		protected internal void RaisePropertyChanging(string name)
		{
		}

		internal static string WriteObjectXml(object o)
		{
			switch (Type.GetTypeCode(o.GetType()))
			{
			case TypeCode.Boolean:
				return XmlConvert.ToString((bool)o);
			case TypeCode.Byte:
				return XmlConvert.ToString((byte)o);
			case TypeCode.Char:
				return XmlConvert.ToString((char)o);
			case TypeCode.DateTime:
				return XmlConvert.ToString((DateTime)o, XmlDateTimeSerializationMode.Unspecified);
			case TypeCode.Decimal:
				return XmlConvert.ToString((decimal)o);
			case TypeCode.Double:
				return XmlConvert.ToString((double)o);
			case TypeCode.Int16:
				return XmlConvert.ToString((short)o);
			case TypeCode.Int32:
				return XmlConvert.ToString((int)o);
			case TypeCode.Int64:
				return XmlConvert.ToString((long)o);
			case TypeCode.SByte:
				return XmlConvert.ToString((sbyte)o);
			case TypeCode.Single:
				return XmlConvert.ToString((float)o);
			case TypeCode.UInt16:
				return XmlConvert.ToString((ushort)o);
			case TypeCode.UInt32:
				return XmlConvert.ToString((uint)o);
			case TypeCode.UInt64:
				return XmlConvert.ToString((ulong)o);
			default:
				if (o is TimeSpan)
				{
					return XmlConvert.ToString((TimeSpan)o);
				}
				if (o is Guid)
				{
					return XmlConvert.ToString((Guid)o);
				}
				if (o is byte[])
				{
					return Convert.ToBase64String((byte[])o);
				}
				return o.ToString();
			}
		}

		private void WriteTables(XmlWriter writer, XmlWriteMode mode, DataTableCollection tableCollection, DataRowVersion version)
		{
			foreach (DataTable item in tableCollection)
			{
				WriteTable(writer, item, mode, version);
			}
		}

		internal static void WriteTable(XmlWriter writer, DataTable table, XmlWriteMode mode, DataRowVersion version)
		{
			DataRow[] array = table.NewRowArray(table.Rows.Count);
			table.Rows.CopyTo(array, 0);
			WriteTable(writer, array, mode, version, true);
		}

		internal static void WriteTable(XmlWriter writer, DataRow[] rows, XmlWriteMode mode, DataRowVersion version, bool skipIfNested)
		{
			if (rows.Length == 0)
			{
				return;
			}
			DataTable table = rows[0].Table;
			if (table.TableName == null || table.TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			DataColumn simple = null;
			ArrayList atts;
			ArrayList elements;
			SplitColumns(table, out atts, out elements, out simple);
			int count = table.ParentRelations.Count;
			foreach (DataRow dataRow in rows)
			{
				if (skipIfNested)
				{
					bool flag = false;
					for (int j = 0; j < table.ParentRelations.Count; j++)
					{
						DataRelation dataRelation = table.ParentRelations[j];
						if (dataRelation.Nested && dataRow.GetParentRow(dataRelation) != null)
						{
							flag = true;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				if (!dataRow.HasVersion(version) || (mode == XmlWriteMode.DiffGram && dataRow.RowState == DataRowState.Unchanged && version == DataRowVersion.Original))
				{
					continue;
				}
				bool flag2 = true;
				foreach (DataColumn column in table.Columns)
				{
					if (dataRow[column.ColumnName, version] != DBNull.Value)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					writer.WriteElementString(XmlHelper.Encode(table.TableName), string.Empty);
					continue;
				}
				WriteTableElement(writer, mode, table, dataRow, version);
				foreach (DataColumn item in atts)
				{
					WriteColumnAsAttribute(writer, mode, item, dataRow, version);
				}
				if (simple != null)
				{
					writer.WriteString(WriteObjectXml(dataRow[simple, version]));
				}
				else
				{
					foreach (DataColumn item2 in elements)
					{
						WriteColumnAsElement(writer, mode, item2, dataRow, version);
					}
				}
				foreach (DataRelation childRelation in table.ChildRelations)
				{
					if (childRelation.Nested)
					{
						WriteTable(writer, dataRow.GetChildRows(childRelation), mode, version, false);
					}
				}
				writer.WriteEndElement();
			}
		}

		internal static void WriteColumnAsElement(XmlWriter writer, XmlWriteMode mode, DataColumn col, DataRow row, DataRowVersion version)
		{
			string nspc = null;
			object obj = row[col, version];
			if (obj == null || obj == DBNull.Value)
			{
				return;
			}
			if (col.Namespace != string.Empty)
			{
				nspc = col.Namespace;
			}
			WriteStartElement(writer, mode, nspc, col.Prefix, XmlHelper.Encode(col.ColumnName));
			if (typeof(IXmlSerializable).IsAssignableFrom(col.DataType) || col.DataType == typeof(object))
			{
				IXmlSerializable xmlSerializable = obj as IXmlSerializable;
				if (xmlSerializable == null)
				{
					throw new InvalidOperationException();
				}
				((IXmlSerializable)obj).WriteXml(writer);
			}
			else
			{
				writer.WriteString(WriteObjectXml(obj));
			}
			writer.WriteEndElement();
		}

		internal static void WriteColumnAsAttribute(XmlWriter writer, XmlWriteMode mode, DataColumn col, DataRow row, DataRowVersion version)
		{
			if (!row.IsNull(col))
			{
				WriteAttributeString(writer, mode, col.Namespace, col.Prefix, XmlHelper.Encode(col.ColumnName), WriteObjectXml(row[col, version]));
			}
		}

		internal static void WriteTableElement(XmlWriter writer, XmlWriteMode mode, DataTable table, DataRow row, DataRowVersion version)
		{
			string nspc = ((table.Namespace.Length <= 0 && table.DataSet != null) ? table.DataSet.Namespace : table.Namespace);
			WriteStartElement(writer, mode, nspc, table.Prefix, XmlHelper.Encode(table.TableName));
			if (mode == XmlWriteMode.DiffGram)
			{
				WriteAttributeString(writer, mode, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "id", table.TableName + (row.XmlRowID + 1));
				WriteAttributeString(writer, mode, "urn:schemas-microsoft-com:xml-msdata", "msdata", "rowOrder", XmlConvert.ToString(row.XmlRowID));
				string text = null;
				if (row.RowState == DataRowState.Modified)
				{
					text = "modified";
				}
				else if (row.RowState == DataRowState.Added)
				{
					text = "inserted";
				}
				if (version != DataRowVersion.Original && text != null)
				{
					WriteAttributeString(writer, mode, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "hasChanges", text);
				}
			}
		}

		internal static void WriteStartElement(XmlWriter writer, XmlWriteMode mode, string nspc, string prefix, string name)
		{
			writer.WriteStartElement(prefix, name, nspc);
		}

		internal static void WriteAttributeString(XmlWriter writer, XmlWriteMode mode, string nspc, string prefix, string name, string stringValue)
		{
			if (mode == XmlWriteMode.DiffGram)
			{
				writer.WriteAttributeString(prefix, name, nspc, stringValue);
			}
			else
			{
				writer.WriteAttributeString(name, stringValue);
			}
		}

		internal void WriteIndividualTableContent(XmlWriter writer, DataTable table, XmlWriteMode mode)
		{
			if (mode == XmlWriteMode.DiffGram)
			{
				table.SetRowsID();
				WriteDiffGramElement(writer);
			}
			WriteStartElement(writer, mode, Namespace, Prefix, XmlHelper.Encode(DataSetName));
			WriteTable(writer, table, mode, DataRowVersion.Default);
			if (mode == XmlWriteMode.DiffGram)
			{
				writer.WriteEndElement();
				if (HasChanges(DataRowState.Deleted | DataRowState.Modified))
				{
					DataSet changes = GetChanges(DataRowState.Deleted | DataRowState.Modified);
					WriteStartElement(writer, XmlWriteMode.DiffGram, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "before");
					WriteTable(writer, changes.Tables[table.TableName], mode, DataRowVersion.Original);
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}

		private void DoWriteXmlSchema(XmlWriter writer)
		{
			if (writer.WriteState == WriteState.Start)
			{
				writer.WriteStartDocument();
			}
			XmlSchemaWriter.WriteXmlSchema(this, writer);
		}

		internal static void SplitColumns(DataTable table, out ArrayList atts, out ArrayList elements, out DataColumn simple)
		{
			atts = new ArrayList();
			elements = new ArrayList();
			simple = null;
			foreach (DataColumn column in table.Columns)
			{
				switch (column.ColumnMapping)
				{
				case MappingType.Attribute:
					atts.Add(column);
					break;
				case MappingType.Element:
					elements.Add(column);
					break;
				case MappingType.SimpleContent:
					if (simple != null)
					{
						throw new InvalidOperationException("There may only be one simple content element");
					}
					simple = column;
					break;
				}
			}
		}

		internal static void WriteDiffGramElement(XmlWriter writer)
		{
			WriteStartElement(writer, XmlWriteMode.DiffGram, "urn:schemas-microsoft-com:xml-diffgram-v1", "diffgr", "diffgram");
			WriteAttributeString(writer, XmlWriteMode.DiffGram, null, "xmlns", "msdata", "urn:schemas-microsoft-com:xml-msdata");
		}

		private void SetRowsID()
		{
			foreach (DataTable table in Tables)
			{
				table.SetRowsID();
			}
		}

		public DataTableReader CreateDataReader(params DataTable[] dataTables)
		{
			return new DataTableReader(dataTables);
		}

		public DataTableReader CreateDataReader()
		{
			return new DataTableReader((DataTable[])Tables.ToArray(typeof(DataTable)));
		}

		public static XmlSchemaComplexType GetDataSetSchema(XmlSchemaSet schemaSet)
		{
			return new XmlSchemaComplexType();
		}

		public void Load(IDataReader reader, LoadOption loadOption, params DataTable[] tables)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			foreach (DataTable dataTable in tables)
			{
				if (dataTable.DataSet == null || dataTable.DataSet != this)
				{
					throw new ArgumentException("Table " + dataTable.TableName + " does not belong to this DataSet.");
				}
				dataTable.Load(reader, loadOption);
				reader.NextResult();
			}
		}

		public void Load(IDataReader reader, LoadOption loadOption, params string[] tables)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			foreach (string text in tables)
			{
				DataTable dataTable = Tables[text];
				if (dataTable == null)
				{
					dataTable = new DataTable(text);
					Tables.Add(dataTable);
				}
				dataTable.Load(reader, loadOption);
				reader.NextResult();
			}
		}

		public virtual void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler, params DataTable[] tables)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			foreach (DataTable dataTable in tables)
			{
				if (dataTable.DataSet == null || dataTable.DataSet != this)
				{
					throw new ArgumentException("Table " + dataTable.TableName + " does not belong to this DataSet.");
				}
				dataTable.Load(reader, loadOption, errorHandler);
				reader.NextResult();
			}
		}

		private void BinarySerialize(SerializationInfo si)
		{
			Version value = new Version(2, 0);
			si.AddValue("DataSet.RemotingVersion", value, typeof(Version));
			si.AddValue("DataSet.RemotingFormat", RemotingFormat, typeof(SerializationFormat));
			si.AddValue("DataSet.DataSetName", DataSetName);
			si.AddValue("DataSet.Namespace", Namespace);
			si.AddValue("DataSet.Prefix", Prefix);
			si.AddValue("DataSet.CaseSensitive", CaseSensitive);
			si.AddValue("DataSet.LocaleLCID", Locale.LCID);
			si.AddValue("DataSet.EnforceConstraints", EnforceConstraints);
			si.AddValue("DataSet.ExtendedProperties", properties, typeof(PropertyCollection));
			Tables.BinarySerialize_Schema(si);
			Tables.BinarySerialize_Data(si);
			Relations.BinarySerialize(si);
		}

		private void BinaryDeserialize(SerializationInfo info)
		{
			ArrayList arrayList = null;
			DataSetName = info.GetString("DataSet.DataSetName");
			Namespace = info.GetString("DataSet.Namespace");
			CaseSensitive = info.GetBoolean("DataSet.CaseSensitive");
			Locale = new CultureInfo(info.GetInt32("DataSet.LocaleLCID"));
			EnforceConstraints = info.GetBoolean("DataSet.EnforceConstraints");
			Prefix = info.GetString("DataSet.Prefix");
			properties = (PropertyCollection)info.GetValue("DataSet.ExtendedProperties", typeof(PropertyCollection));
			int @int = info.GetInt32("DataSet.Tables.Count");
			DataTable dataTable = null;
			for (int i = 0; i < @int; i++)
			{
				byte[] buffer = (byte[])info.GetValue("DataSet.Tables_" + i, typeof(byte[]));
				MemoryStream memoryStream = new MemoryStream(buffer);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				dataTable = (DataTable)binaryFormatter.Deserialize(memoryStream);
				memoryStream.Close();
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					dataTable.Columns[j].Expression = info.GetString("DataTable_" + i + ".DataColumn_" + j + ".Expression");
				}
				ArrayList nullBits = (ArrayList)info.GetValue("DataTable_" + i + ".NullBits", typeof(ArrayList));
				arrayList = (ArrayList)info.GetValue("DataTable_" + i + ".Records", typeof(ArrayList));
				BitArray rowStateBitArray = (BitArray)info.GetValue("DataTable_" + i + ".RowStates", typeof(BitArray));
				dataTable.DeserializeRecords(arrayList, nullBits, rowStateBitArray);
				Tables.Add(dataTable);
			}
			for (int k = 0; k < @int; k++)
			{
				dataTable = Tables[k];
				dataTable.dataSet = this;
				arrayList = (ArrayList)info.GetValue("DataTable_" + k + ".Constraints", typeof(ArrayList));
				if (dataTable.Constraints == null)
				{
					dataTable.Constraints = new ConstraintCollection(dataTable);
				}
				dataTable.DeserializeConstraints(arrayList);
			}
			arrayList = (ArrayList)info.GetValue("DataSet.Relations", typeof(ArrayList));
			bool flag = true;
			for (int l = 0; l < arrayList.Count; l++)
			{
				ArrayList arrayList2 = (ArrayList)arrayList[l];
				ArrayList arrayList3 = new ArrayList();
				ArrayList arrayList4 = new ArrayList();
				for (int m = 0; m < arrayList2.Count; m++)
				{
					if (arrayList2[m] != null && typeof(int) == arrayList2[m].GetType().GetElementType())
					{
						Array array = (Array)arrayList2[m];
						if (flag)
						{
							arrayList4.Add(Tables[(int)array.GetValue(0)].Columns[(int)array.GetValue(1)]);
							flag = false;
						}
						else
						{
							arrayList3.Add(Tables[(int)array.GetValue(0)].Columns[(int)array.GetValue(1)]);
							flag = true;
						}
					}
				}
				Relations.Add((string)arrayList2[0], (DataColumn[])arrayList4.ToArray(typeof(DataColumn)), (DataColumn[])arrayList3.ToArray(typeof(DataColumn)), false);
			}
		}

		private void OnDataSetInitialized(EventArgs e)
		{
			if (this.Initialized != null)
			{
				this.Initialized(this, e);
			}
		}

		private void DataSetInitialized()
		{
			EventArgs e = new EventArgs();
			OnDataSetInitialized(e);
		}

		protected virtual void InitializeDerivedDataSet()
		{
		}

		protected SchemaSerializationMode DetermineSchemaSerializationMode(XmlReader reader)
		{
			return SchemaSerializationMode.IncludeSchema;
		}

		protected SchemaSerializationMode DetermineSchemaSerializationMode(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Name == "SchemaSerializationMode.DataSet")
				{
					return (SchemaSerializationMode)(int)enumerator.Value;
				}
			}
			return SchemaSerializationMode.IncludeSchema;
		}

		protected bool IsBinarySerialized(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.ObjectType == typeof(SerializationFormat))
				{
					return true;
				}
			}
			return false;
		}
	}
}
