using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public class DataAdapter : Component, IDataAdapter
	{
		private const string DefaultSourceTableName = "Table";

		private const string DefaultSourceColumnName = "Column";

		private bool acceptChangesDuringFill;

		private bool continueUpdateOnError;

		private MissingMappingAction missingMappingAction;

		private MissingSchemaAction missingSchemaAction;

		private DataTableMappingCollection tableMappings;

		private bool acceptChangesDuringUpdate;

		private LoadOption fillLoadOption;

		private bool returnProviderSpecificTypes;

		ITableMappingCollection IDataAdapter.TableMappings
		{
			get
			{
				return TableMappings;
			}
		}

		[DataCategory("Fill")]
		[DefaultValue(true)]
		public bool AcceptChangesDuringFill
		{
			get
			{
				return acceptChangesDuringFill;
			}
			set
			{
				acceptChangesDuringFill = value;
			}
		}

		[DefaultValue(true)]
		public bool AcceptChangesDuringUpdate
		{
			get
			{
				return acceptChangesDuringUpdate;
			}
			set
			{
				acceptChangesDuringUpdate = value;
			}
		}

		[DefaultValue(false)]
		[DataCategory("Update")]
		public bool ContinueUpdateOnError
		{
			get
			{
				return continueUpdateOnError;
			}
			set
			{
				continueUpdateOnError = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public LoadOption FillLoadOption
		{
			get
			{
				return fillLoadOption;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(LoadOption), value);
				fillLoadOption = value;
			}
		}

		[DefaultValue(MissingMappingAction.Passthrough)]
		[DataCategory("Mapping")]
		public MissingMappingAction MissingMappingAction
		{
			get
			{
				return missingMappingAction;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(MissingMappingAction), value);
				missingMappingAction = value;
			}
		}

		[DefaultValue(MissingSchemaAction.Add)]
		[DataCategory("Mapping")]
		public MissingSchemaAction MissingSchemaAction
		{
			get
			{
				return missingSchemaAction;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(MissingSchemaAction), value);
				missingSchemaAction = value;
			}
		}

		[DefaultValue(false)]
		public virtual bool ReturnProviderSpecificTypes
		{
			get
			{
				return returnProviderSpecificTypes;
			}
			set
			{
				returnProviderSpecificTypes = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DataCategory("Mapping")]
		public DataTableMappingCollection TableMappings
		{
			get
			{
				return tableMappings;
			}
		}

		public event FillErrorEventHandler FillError;

		protected DataAdapter()
		{
			acceptChangesDuringFill = true;
			continueUpdateOnError = false;
			missingMappingAction = MissingMappingAction.Passthrough;
			missingSchemaAction = MissingSchemaAction.Add;
			tableMappings = new DataTableMappingCollection();
			acceptChangesDuringUpdate = true;
			fillLoadOption = LoadOption.OverwriteChanges;
			returnProviderSpecificTypes = false;
		}

		protected DataAdapter(DataAdapter adapter)
		{
			AcceptChangesDuringFill = adapter.AcceptChangesDuringFill;
			ContinueUpdateOnError = adapter.ContinueUpdateOnError;
			MissingMappingAction = adapter.MissingMappingAction;
			MissingSchemaAction = adapter.MissingSchemaAction;
			if (adapter.tableMappings != null)
			{
				foreach (ICloneable tableMapping in adapter.TableMappings)
				{
					TableMappings.Add(tableMapping.Clone());
				}
			}
			acceptChangesDuringUpdate = adapter.AcceptChangesDuringUpdate;
			fillLoadOption = adapter.FillLoadOption;
			returnProviderSpecificTypes = adapter.ReturnProviderSpecificTypes;
		}

		[Obsolete("Use the protected constructor instead", false)]
		[System.MonoTODO]
		protected virtual DataAdapter CloneInternals()
		{
			throw new NotImplementedException();
		}

		protected virtual DataTableMappingCollection CreateTableMappings()
		{
			return new DataTableMappingCollection();
		}

		[System.MonoTODO]
		protected override void Dispose(bool disposing)
		{
			throw new NotImplementedException();
		}

		protected virtual bool ShouldSerializeTableMappings()
		{
			return true;
		}

		internal int FillInternal(DataTable dataTable, IDataReader dataReader)
		{
			if (dataReader.FieldCount == 0)
			{
				dataReader.Close();
				return 0;
			}
			int counter = 0;
			try
			{
				string text = SetupSchema(SchemaType.Mapped, dataTable.TableName);
				if (text != null)
				{
					dataTable.TableName = text;
					FillTable(dataTable, dataReader, 0, 0, ref counter);
				}
			}
			finally
			{
				dataReader.Close();
			}
			return counter;
		}

		internal int[] BuildSchema(IDataReader reader, DataTable table, SchemaType schemaType)
		{
			return BuildSchema(reader, table, schemaType, MissingSchemaAction, MissingMappingAction, TableMappings);
		}

		internal static int[] BuildSchema(IDataReader reader, DataTable table, SchemaType schemaType, MissingSchemaAction missingSchAction, MissingMappingAction missingMapAction, DataTableMappingCollection dtMapping)
		{
			int num = 0;
			int[] array = new int[table.Columns.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = -1;
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			bool flag = true;
			DataTable schemaTable = reader.GetSchemaTable();
			DataColumn dataColumn = schemaTable.Columns["ColumnName"];
			DataColumn column = schemaTable.Columns["DataType"];
			DataColumn dataColumn2 = schemaTable.Columns["IsAutoIncrement"];
			DataColumn dataColumn3 = schemaTable.Columns["AllowDBNull"];
			DataColumn dataColumn4 = schemaTable.Columns["IsReadOnly"];
			DataColumn dataColumn5 = schemaTable.Columns["IsKey"];
			DataColumn dataColumn6 = schemaTable.Columns["IsUnique"];
			DataColumn dataColumn7 = schemaTable.Columns["ColumnSize"];
			foreach (DataRow row in schemaTable.Rows)
			{
				string text;
				string text2;
				if (dataColumn == null || row.IsNull(dataColumn) || (string)row[dataColumn] == string.Empty)
				{
					text = "Column";
					text2 = "Column1";
				}
				else
				{
					text = (string)row[dataColumn];
					text2 = text;
				}
				int num2 = 1;
				while (arrayList2.Contains(text2))
				{
					text2 = string.Format("{0}{1}", text, num2);
					num2++;
				}
				arrayList2.Add(text2);
				DataTableMapping dataTableMapping = null;
				int num3 = dtMapping.IndexOfDataSetTable(table.TableName);
				string sourceTable = ((num3 == -1) ? table.TableName : dtMapping[num3].SourceTable);
				dataTableMapping = DataTableMappingCollection.GetTableMappingBySchemaAction(dtMapping, sourceTable, table.TableName, missingMapAction);
				if (dataTableMapping == null)
				{
					continue;
				}
				table.TableName = dataTableMapping.DataSetTable;
				DataColumnMapping columnMappingBySchemaAction = DataColumnMappingCollection.GetColumnMappingBySchemaAction(dataTableMapping.ColumnMappings, text2, missingMapAction);
				if (columnMappingBySchemaAction == null)
				{
					continue;
				}
				Type type = row[column] as Type;
				DataColumn dataColumn8 = ((type == null) ? null : columnMappingBySchemaAction.GetDataColumnBySchemaAction(table, type, missingSchAction));
				if (dataColumn8 == null)
				{
					continue;
				}
				if (table.Columns.IndexOf(dataColumn8) == -1)
				{
					if (missingSchAction == MissingSchemaAction.Add || missingSchAction == MissingSchemaAction.AddWithKey)
					{
						table.Columns.Add(dataColumn8);
					}
					int[] array2 = new int[array.Length + 1];
					Array.Copy(array, 0, array2, 0, dataColumn8.Ordinal);
					Array.Copy(array, dataColumn8.Ordinal, array2, dataColumn8.Ordinal + 1, array.Length - dataColumn8.Ordinal);
					array = array2;
				}
				if (missingSchAction == MissingSchemaAction.AddWithKey)
				{
					object obj = ((dataColumn3 == null) ? null : row[dataColumn3]);
					bool flag2 = !(obj is bool) || (bool)obj;
					obj = ((dataColumn5 == null) ? null : row[dataColumn5]);
					bool flag3 = obj is bool && (bool)obj;
					obj = ((dataColumn2 == null) ? null : row[dataColumn2]);
					bool flag4 = obj is bool && (bool)obj;
					obj = ((dataColumn4 == null) ? null : row[dataColumn4]);
					bool flag5 = obj is bool && (bool)obj;
					obj = ((dataColumn6 == null) ? null : row[dataColumn6]);
					bool flag6 = obj is bool && (bool)obj;
					dataColumn8.AllowDBNull = flag2;
					if (flag4 && DataColumn.CanAutoIncrement(type))
					{
						dataColumn8.AutoIncrement = true;
						if (!flag2)
						{
							dataColumn8.AllowDBNull = false;
						}
					}
					if (type == DbTypes.TypeOfString)
					{
						dataColumn8.MaxLength = ((dataColumn7 != null) ? ((int)row[dataColumn7]) : 0);
					}
					if (flag5)
					{
						dataColumn8.ReadOnly = true;
					}
					if (!flag2 && (!flag5 || flag3))
					{
						dataColumn8.AllowDBNull = false;
					}
					if (flag6 && !flag3 && !type.IsArray)
					{
						dataColumn8.Unique = true;
						if (!flag2)
						{
							dataColumn8.AllowDBNull = false;
						}
					}
					bool flag7 = false;
					if (schemaTable.Columns.Contains("IsHidden"))
					{
						obj = row["IsHidden"];
						flag7 = obj is bool && (bool)obj;
					}
					if (flag3 && !flag7)
					{
						arrayList.Add(dataColumn8);
						if (flag2)
						{
							flag = false;
						}
					}
				}
				array[dataColumn8.Ordinal] = num++;
			}
			if (arrayList.Count > 0)
			{
				DataColumn[] array3 = (DataColumn[])arrayList.ToArray(typeof(DataColumn));
				if (flag)
				{
					table.PrimaryKey = array3;
				}
				else
				{
					UniqueConstraint uniqueConstraint = new UniqueConstraint(array3);
					for (int j = 0; j < table.Constraints.Count; j++)
					{
						if (table.Constraints[j].Equals(uniqueConstraint))
						{
							uniqueConstraint = null;
							break;
						}
					}
					if (uniqueConstraint != null)
					{
						table.Constraints.Add(uniqueConstraint);
					}
				}
			}
			return array;
		}

		internal bool FillTable(DataTable dataTable, IDataReader dataReader, int startRecord, int maxRecords, ref int counter)
		{
			if (dataReader.FieldCount == 0)
			{
				return false;
			}
			int num = counter;
			int[] array = BuildSchema(dataReader, dataTable, SchemaType.Mapped);
			int[] array2 = new int[array.Length];
			int num2 = array2.Length;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array[i] >= 0)
				{
					array2[array[i]] = i;
				}
				else
				{
					array2[--num2] = i;
				}
			}
			for (int j = 0; j < startRecord; j++)
			{
				dataReader.Read();
			}
			dataTable.BeginLoadData();
			while (dataReader.Read() && (maxRecords == 0 || counter - num < maxRecords))
			{
				try
				{
					dataTable.LoadDataRow(dataReader, array2, num2, AcceptChangesDuringFill);
					counter++;
				}
				catch (Exception ex)
				{
					object[] array3 = new object[dataReader.FieldCount];
					object[] array4 = new object[array.Length];
					dataReader.GetValues(array3);
					for (int k = 0; k < array.Length; k++)
					{
						if (array[k] >= 0)
						{
							array4[k] = array3[array[k]];
						}
					}
					FillErrorEventArgs e = CreateFillErrorEvent(dataTable, array4, ex);
					OnFillErrorInternal(e);
					if (!e.Continue)
					{
						throw ex;
					}
				}
			}
			dataTable.EndLoadData();
			return true;
		}

		internal virtual void OnFillErrorInternal(FillErrorEventArgs value)
		{
			OnFillError(value);
		}

		internal FillErrorEventArgs CreateFillErrorEvent(DataTable dataTable, object[] values, Exception e)
		{
			FillErrorEventArgs e2 = new FillErrorEventArgs(dataTable, values);
			e2.Errors = e;
			e2.Continue = false;
			return e2;
		}

		internal string SetupSchema(SchemaType schemaType, string sourceTableName)
		{
			DataTableMapping dataTableMapping = null;
			if (schemaType == SchemaType.Mapped)
			{
				dataTableMapping = DataTableMappingCollection.GetTableMappingBySchemaAction(TableMappings, sourceTableName, sourceTableName, MissingMappingAction);
				if (dataTableMapping != null)
				{
					return dataTableMapping.DataSetTable;
				}
				return null;
			}
			return sourceTableName;
		}

		internal int FillInternal(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("DataSet");
			}
			if (startRecord < 0)
			{
				throw new ArgumentException("The startRecord parameter was less than 0.");
			}
			if (maxRecords < 0)
			{
				throw new ArgumentException("The maxRecords parameter was less than 0.");
			}
			DataTable dataTable = null;
			int num = 0;
			int counter = 0;
			try
			{
				string text = srcTable;
				do
				{
					if (dataReader.FieldCount == -1)
					{
						continue;
					}
					text = SetupSchema(SchemaType.Mapped, text);
					if (text == null)
					{
						continue;
					}
					if (dataSet.Tables.Contains(text))
					{
						dataTable = dataSet.Tables[text];
					}
					else
					{
						if (MissingSchemaAction == MissingSchemaAction.Ignore)
						{
							continue;
						}
						dataTable = dataSet.Tables.Add(text);
					}
					if (FillTable(dataTable, dataReader, startRecord, maxRecords, ref counter))
					{
						text = string.Format("{0}{1}", srcTable, ++num);
						startRecord = 0;
						maxRecords = 0;
					}
				}
				while (dataReader.NextResult());
				return counter;
			}
			finally
			{
				dataReader.Close();
			}
		}

		public virtual int Fill(DataSet dataSet)
		{
			throw new NotSupportedException();
		}

		protected virtual int Fill(DataTable dataTable, IDataReader dataReader)
		{
			return FillInternal(dataTable, dataReader);
		}

		protected virtual int Fill(DataTable[] dataTables, IDataReader dataReader, int startRecord, int maxRecords)
		{
			int counter = 0;
			if (dataReader.IsClosed)
			{
				return 0;
			}
			if (startRecord < 0)
			{
				throw new ArgumentException("The startRecord parameter was less than 0.");
			}
			if (maxRecords < 0)
			{
				throw new ArgumentException("The maxRecords parameter was less than 0.");
			}
			try
			{
				foreach (DataTable dataTable in dataTables)
				{
					string text = SetupSchema(SchemaType.Mapped, dataTable.TableName);
					if (text != null)
					{
						dataTable.TableName = text;
						FillTable(dataTable, dataReader, 0, 0, ref counter);
					}
				}
				return counter;
			}
			finally
			{
				dataReader.Close();
			}
		}

		protected virtual int Fill(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
		{
			return FillInternal(dataSet, srcTable, dataReader, startRecord, maxRecords);
		}

		[System.MonoTODO]
		protected virtual DataTable FillSchema(DataTable dataTable, SchemaType schemaType, IDataReader dataReader)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, string srcTable, IDataReader dataReader)
		{
			throw new NotImplementedException();
		}

		public virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
		{
			throw new NotSupportedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[System.MonoTODO]
		public virtual IDataParameter[] GetFillParameters()
		{
			throw new NotImplementedException();
		}

		protected bool HasTableMappings()
		{
			return TableMappings.Count != 0;
		}

		protected virtual void OnFillError(FillErrorEventArgs value)
		{
			if (this.FillError != null)
			{
				this.FillError(this, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetFillLoadOption()
		{
			FillLoadOption = LoadOption.OverwriteChanges;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerializeAcceptChangesDuringFill()
		{
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerializeFillLoadOption()
		{
			return false;
		}

		[System.MonoTODO]
		public virtual int Update(DataSet dataSet)
		{
			throw new NotImplementedException();
		}
	}
}
