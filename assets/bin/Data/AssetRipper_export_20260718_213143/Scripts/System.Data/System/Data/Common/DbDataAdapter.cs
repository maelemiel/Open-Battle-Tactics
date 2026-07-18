using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Data.Common
{
	public abstract class DbDataAdapter : DataAdapter, ICloneable, IDataAdapter, IDbDataAdapter
	{
		public const string DefaultSourceTableName = "Table";

		private const string DefaultSourceColumnName = "Column";

		private CommandBehavior _behavior;

		private IDbCommand _selectCommand;

		private IDbCommand _updateCommand;

		private IDbCommand _deleteCommand;

		private IDbCommand _insertCommand;

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				return SelectCommand;
			}
			set
			{
				SelectCommand = (DbCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				return UpdateCommand;
			}
			set
			{
				UpdateCommand = (DbCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return DeleteCommand;
			}
			set
			{
				DeleteCommand = (DbCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				return InsertCommand;
			}
			set
			{
				InsertCommand = (DbCommand)value;
			}
		}

		protected internal CommandBehavior FillCommandBehavior
		{
			get
			{
				return _behavior;
			}
			set
			{
				_behavior = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand SelectCommand
		{
			get
			{
				return (DbCommand)_selectCommand;
			}
			set
			{
				if (_selectCommand != value)
				{
					_selectCommand = value;
					((IDbDataAdapter)this).SelectCommand = value;
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DbCommand DeleteCommand
		{
			get
			{
				return (DbCommand)_deleteCommand;
			}
			set
			{
				if (_deleteCommand != value)
				{
					_deleteCommand = value;
					((IDbDataAdapter)this).DeleteCommand = value;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand InsertCommand
		{
			get
			{
				return (DbCommand)_insertCommand;
			}
			set
			{
				if (_insertCommand != value)
				{
					_insertCommand = value;
					((IDbDataAdapter)this).InsertCommand = value;
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DbCommand UpdateCommand
		{
			get
			{
				return (DbCommand)_updateCommand;
			}
			set
			{
				if (_updateCommand != value)
				{
					_updateCommand = value;
					((IDbDataAdapter)this).UpdateCommand = value;
				}
			}
		}

		[DefaultValue(1)]
		public virtual int UpdateBatchSize
		{
			get
			{
				return 1;
			}
			set
			{
				if (value != 1)
				{
					throw new NotSupportedException();
				}
			}
		}

		protected DbDataAdapter()
		{
		}

		protected DbDataAdapter(DbDataAdapter adapter)
			: base(adapter)
		{
		}

		[System.MonoTODO]
		[Obsolete("use 'protected DbDataAdapter(DbDataAdapter)' ctor")]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		protected virtual RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new RowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected virtual RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new RowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected virtual void OnRowUpdated(RowUpdatedEventArgs value)
		{
			if ((object)base.Events["RowUpdated"] != null)
			{
				Delegate[] invocationList = base.Events["RowUpdated"].GetInvocationList();
				Delegate[] array = invocationList;
				foreach (Delegate obj in array)
				{
					MethodInfo method = obj.Method;
					method.Invoke(value, null);
				}
			}
		}

		protected virtual void OnRowUpdating(RowUpdatingEventArgs value)
		{
			if ((object)base.Events["RowUpdating"] != null)
			{
				Delegate[] invocationList = base.Events["RowUpdating"].GetInvocationList();
				Delegate[] array = invocationList;
				foreach (Delegate obj in array)
				{
					MethodInfo method = obj.Method;
					method.Invoke(value, null);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (((IDbDataAdapter)this).SelectCommand != null)
				{
					((IDbDataAdapter)this).SelectCommand.Dispose();
					((IDbDataAdapter)this).SelectCommand = null;
				}
				if (((IDbDataAdapter)this).InsertCommand != null)
				{
					((IDbDataAdapter)this).InsertCommand.Dispose();
					((IDbDataAdapter)this).InsertCommand = null;
				}
				if (((IDbDataAdapter)this).UpdateCommand != null)
				{
					((IDbDataAdapter)this).UpdateCommand.Dispose();
					((IDbDataAdapter)this).UpdateCommand = null;
				}
				if (((IDbDataAdapter)this).DeleteCommand != null)
				{
					((IDbDataAdapter)this).DeleteCommand.Dispose();
					((IDbDataAdapter)this).DeleteCommand = null;
				}
			}
		}

		public override int Fill(DataSet dataSet)
		{
			return Fill(dataSet, 0, 0, "Table", ((IDbDataAdapter)this).SelectCommand, _behavior);
		}

		public int Fill(DataTable dataTable)
		{
			if (dataTable == null)
			{
				throw new ArgumentNullException("DataTable");
			}
			return Fill(dataTable, ((IDbDataAdapter)this).SelectCommand, _behavior);
		}

		public int Fill(DataSet dataSet, string srcTable)
		{
			return Fill(dataSet, 0, 0, srcTable, ((IDbDataAdapter)this).SelectCommand, _behavior);
		}

		protected virtual int Fill(DataTable dataTable, IDbCommand command, CommandBehavior behavior)
		{
			CommandBehavior commandBehavior = behavior;
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				commandBehavior |= CommandBehavior.CloseConnection;
			}
			return Fill(dataTable, command.ExecuteReader(commandBehavior));
		}

		public int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable)
		{
			return Fill(dataSet, startRecord, maxRecords, srcTable, ((IDbDataAdapter)this).SelectCommand, _behavior);
		}

		[System.MonoTODO]
		public int Fill(int startRecord, int maxRecords, params DataTable[] dataTables)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected virtual int Fill(DataTable[] dataTables, int startRecord, int maxRecords, IDbCommand command, CommandBehavior behavior)
		{
			throw new NotImplementedException();
		}

		protected virtual int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable, IDbCommand command, CommandBehavior behavior)
		{
			if (command.Connection == null)
			{
				throw new InvalidOperationException("Connection state is closed");
			}
			if (MissingSchemaAction == MissingSchemaAction.AddWithKey)
			{
				behavior |= CommandBehavior.KeyInfo;
			}
			CommandBehavior commandBehavior = behavior;
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				commandBehavior |= CommandBehavior.CloseConnection;
			}
			return Fill(dataSet, srcTable, command.ExecuteReader(commandBehavior), startRecord, maxRecords);
		}

		internal static int FillFromReader(DataTable table, IDataReader reader, int start, int length, int[] mapping, LoadOption loadOption)
		{
			if (reader.FieldCount == 0)
			{
				return 0;
			}
			for (int i = 0; i < start; i++)
			{
				reader.Read();
			}
			int num = 0;
			object[] array = new object[mapping.Length];
			while (reader.Read() && (length == 0 || num < length))
			{
				for (int j = 0; j < mapping.Length; j++)
				{
					array[j] = ((mapping[j] >= 0) ? reader[mapping[j]] : null);
				}
				table.BeginLoadData();
				table.LoadDataRow(array, loadOption);
				table.EndLoadData();
				num++;
			}
			return num;
		}

		internal static int FillFromReader(DataTable table, IDataReader reader, int start, int length, int[] mapping, LoadOption loadOption, FillErrorEventHandler errorHandler)
		{
			if (reader.FieldCount == 0)
			{
				return 0;
			}
			for (int i = 0; i < start; i++)
			{
				reader.Read();
			}
			int num = 0;
			object[] array = new object[mapping.Length];
			while (reader.Read() && (length == 0 || num < length))
			{
				for (int j = 0; j < mapping.Length; j++)
				{
					array[j] = ((mapping[j] >= 0) ? reader[mapping[j]] : null);
				}
				table.BeginLoadData();
				try
				{
					table.LoadDataRow(array, loadOption);
				}
				catch (Exception ex)
				{
					FillErrorEventArgs e = new FillErrorEventArgs(table, array);
					e.Errors = ex;
					e.Continue = false;
					errorHandler(table, e);
					if (!e.Continue)
					{
						throw ex;
					}
				}
				table.EndLoadData();
				num++;
			}
			return num;
		}

		public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
		{
			return FillSchema(dataSet, schemaType, ((IDbDataAdapter)this).SelectCommand, "Table", _behavior);
		}

		public DataTable FillSchema(DataTable dataTable, SchemaType schemaType)
		{
			return FillSchema(dataTable, schemaType, ((IDbDataAdapter)this).SelectCommand, _behavior);
		}

		public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, string srcTable)
		{
			return FillSchema(dataSet, schemaType, ((IDbDataAdapter)this).SelectCommand, srcTable, _behavior);
		}

		protected virtual DataTable FillSchema(DataTable dataTable, SchemaType schemaType, IDbCommand command, CommandBehavior behavior)
		{
			if (dataTable == null)
			{
				throw new ArgumentNullException("DataTable");
			}
			behavior |= CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo;
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				behavior |= CommandBehavior.CloseConnection;
			}
			IDataReader dataReader = command.ExecuteReader(behavior);
			try
			{
				string text = SetupSchema(schemaType, dataTable.TableName);
				if (text != null)
				{
					MissingSchemaAction missingSchemaAction = MissingSchemaAction;
					if (missingSchemaAction != MissingSchemaAction.Ignore && missingSchemaAction != MissingSchemaAction.Error)
					{
						missingSchemaAction = MissingSchemaAction.AddWithKey;
					}
					DataAdapter.BuildSchema(dataReader, dataTable, schemaType, missingSchemaAction, MissingMappingAction, base.TableMappings);
				}
			}
			finally
			{
				dataReader.Close();
			}
			return dataTable;
		}

		protected virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, IDbCommand command, string srcTable, CommandBehavior behavior)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("DataSet");
			}
			behavior |= CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo;
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				behavior |= CommandBehavior.CloseConnection;
			}
			IDataReader dataReader = command.ExecuteReader(behavior);
			ArrayList arrayList = new ArrayList();
			string text = srcTable;
			int num = 0;
			try
			{
				MissingSchemaAction missingSchAction = MissingSchemaAction;
				if (MissingSchemaAction != MissingSchemaAction.Ignore && MissingSchemaAction != MissingSchemaAction.Error)
				{
					missingSchAction = MissingSchemaAction.AddWithKey;
				}
				do
				{
					text = SetupSchema(schemaType, text);
					if (text == null)
					{
						continue;
					}
					DataTable dataTable;
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
					DataAdapter.BuildSchema(dataReader, dataTable, schemaType, missingSchAction, MissingMappingAction, base.TableMappings);
					arrayList.Add(dataTable);
					text = string.Format("{0}{1}", srcTable, ++num);
				}
				while (dataReader.NextResult());
			}
			finally
			{
				dataReader.Close();
			}
			return (DataTable[])arrayList.ToArray(typeof(DataTable));
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override IDataParameter[] GetFillParameters()
		{
			IDbCommand selectCommand = ((IDbDataAdapter)this).SelectCommand;
			IDataParameter[] array = new IDataParameter[selectCommand.Parameters.Count];
			selectCommand.Parameters.CopyTo(array, 0);
			return array;
		}

		public int Update(DataRow[] dataRows)
		{
			if (dataRows == null)
			{
				throw new ArgumentNullException("dataRows");
			}
			if (dataRows.Length == 0)
			{
				return 0;
			}
			if (dataRows[0] == null)
			{
				throw new ArgumentException("dataRows[0].");
			}
			DataTable table = dataRows[0].Table;
			if (table == null)
			{
				throw new ArgumentException("table is null reference.");
			}
			for (int i = 0; i < dataRows.Length; i++)
			{
				if (dataRows[i] == null)
				{
					throw new ArgumentException("dataRows[" + i + "].");
				}
				if (dataRows[i].Table != table)
				{
					throw new ArgumentException(" DataRow[" + i + "] is from a different DataTable than DataRow[0].");
				}
			}
			DataTableMapping dataTableMapping = base.TableMappings.GetByDataSetTable(table.TableName);
			if (dataTableMapping == null)
			{
				dataTableMapping = DataTableMappingCollection.GetTableMappingBySchemaAction(base.TableMappings, table.TableName, table.TableName, MissingMappingAction);
				if (dataTableMapping != null)
				{
					foreach (DataColumn column in table.Columns)
					{
						if (dataTableMapping.ColumnMappings.IndexOf(column.ColumnName) < 0)
						{
							DataColumnMapping dataColumnMapping = DataColumnMappingCollection.GetColumnMappingBySchemaAction(dataTableMapping.ColumnMappings, column.ColumnName, MissingMappingAction);
							if (dataColumnMapping == null)
							{
								dataColumnMapping = new DataColumnMapping(column.ColumnName, column.ColumnName);
							}
							dataTableMapping.ColumnMappings.Add(dataColumnMapping);
						}
					}
				}
				else
				{
					ArrayList arrayList = new ArrayList();
					foreach (DataColumn column2 in table.Columns)
					{
						arrayList.Add(new DataColumnMapping(column2.ColumnName, column2.ColumnName));
					}
					dataTableMapping = new DataTableMapping(table.TableName, table.TableName, arrayList.ToArray(typeof(DataColumnMapping)) as DataColumnMapping[]);
				}
			}
			DataRow[] array = table.NewRowArray(dataRows.Length);
			Array.Copy(dataRows, 0, array, 0, dataRows.Length);
			return Update(array, dataTableMapping);
		}

		public override int Update(DataSet dataSet)
		{
			return Update(dataSet, "Table");
		}

		public int Update(DataTable dataTable)
		{
			DataTableMapping dataTableMapping = base.TableMappings.GetByDataSetTable(dataTable.TableName);
			if (dataTableMapping == null)
			{
				dataTableMapping = DataTableMappingCollection.GetTableMappingBySchemaAction(base.TableMappings, dataTable.TableName, dataTable.TableName, MissingMappingAction);
				if (dataTableMapping != null)
				{
					foreach (DataColumn column in dataTable.Columns)
					{
						if (dataTableMapping.ColumnMappings.IndexOf(column.ColumnName) < 0)
						{
							DataColumnMapping dataColumnMapping = DataColumnMappingCollection.GetColumnMappingBySchemaAction(dataTableMapping.ColumnMappings, column.ColumnName, MissingMappingAction);
							if (dataColumnMapping == null)
							{
								dataColumnMapping = new DataColumnMapping(column.ColumnName, column.ColumnName);
							}
							dataTableMapping.ColumnMappings.Add(dataColumnMapping);
						}
					}
				}
				else
				{
					ArrayList arrayList = new ArrayList();
					foreach (DataColumn column2 in dataTable.Columns)
					{
						arrayList.Add(new DataColumnMapping(column2.ColumnName, column2.ColumnName));
					}
					dataTableMapping = new DataTableMapping(dataTable.TableName, dataTable.TableName, arrayList.ToArray(typeof(DataColumnMapping)) as DataColumnMapping[]);
				}
			}
			return Update(dataTable, dataTableMapping);
		}

		private int Update(DataTable dataTable, DataTableMapping tableMapping)
		{
			DataRow[] array = dataTable.NewRowArray(dataTable.Rows.Count);
			dataTable.Rows.CopyTo(array, 0);
			return Update(array, tableMapping);
		}

		protected virtual int Update(DataRow[] dataRows, DataTableMapping tableMapping)
		{
			int num = 0;
			foreach (DataRow dataRow in dataRows)
			{
				StatementType statementType = StatementType.Update;
				IDbCommand command = null;
				string text = string.Empty;
				switch (dataRow.RowState)
				{
				case DataRowState.Added:
					statementType = StatementType.Insert;
					command = ((IDbDataAdapter)this).InsertCommand;
					text = "Insert";
					break;
				case DataRowState.Deleted:
					statementType = StatementType.Delete;
					command = ((IDbDataAdapter)this).DeleteCommand;
					text = "Delete";
					break;
				case DataRowState.Modified:
					statementType = StatementType.Update;
					command = ((IDbDataAdapter)this).UpdateCommand;
					text = "Update";
					break;
				case DataRowState.Detached:
				case DataRowState.Unchanged:
					continue;
				}
				RowUpdatingEventArgs e = CreateRowUpdatingEvent(dataRow, command, statementType, tableMapping);
				dataRow.RowError = null;
				OnRowUpdating(e);
				switch (e.Status)
				{
				case UpdateStatus.ErrorsOccurred:
					if (e.Errors == null)
					{
						e.Errors = ExceptionHelper.RowUpdatedError();
					}
					dataRow.RowError += e.Errors.Message;
					if (!base.ContinueUpdateOnError)
					{
						throw e.Errors;
					}
					break;
				case UpdateStatus.SkipAllRemainingRows:
					return num;
				case UpdateStatus.SkipCurrentRow:
					num++;
					break;
				default:
					throw ExceptionHelper.InvalidUpdateStatus(e.Status);
				case UpdateStatus.Continue:
				{
					command = e.Command;
					try
					{
						if (command != null)
						{
							DataColumnMappingCollection columnMappings = tableMapping.ColumnMappings;
							foreach (IDataParameter parameter in command.Parameters)
							{
								if ((parameter.Direction & ParameterDirection.Input) == 0)
								{
									continue;
								}
								DataRowVersion version = parameter.SourceVersion;
								if (statementType == StatementType.Delete)
								{
									version = DataRowVersion.Original;
								}
								string sourceColumn = parameter.SourceColumn;
								if (columnMappings.Contains(sourceColumn))
								{
									sourceColumn = columnMappings[sourceColumn].DataSetColumn;
									parameter.Value = dataRow[sourceColumn, version];
								}
								else
								{
									parameter.Value = null;
								}
								DbParameter dbParameter = parameter as DbParameter;
								if (dbParameter != null && dbParameter.SourceColumnNullMapping)
								{
									if (parameter.Value != null && parameter.Value != DBNull.Value)
									{
										dbParameter.Value = 0;
									}
									else
									{
										dbParameter.Value = 1;
									}
									dbParameter = null;
								}
							}
						}
					}
					catch (Exception errors)
					{
						e.Errors = errors;
						e.Status = UpdateStatus.ErrorsOccurred;
					}
					IDataReader dataReader = null;
					try
					{
						if (command == null)
						{
							throw ExceptionHelper.UpdateRequiresCommand(text);
						}
						CommandBehavior commandBehavior = CommandBehavior.Default;
						if (command.Connection.State == ConnectionState.Closed)
						{
							command.Connection.Open();
							commandBehavior |= CommandBehavior.CloseConnection;
						}
						dataReader = command.ExecuteReader(commandBehavior);
						DataColumnMappingCollection columnMappings2 = tableMapping.ColumnMappings;
						if ((command.UpdatedRowSource == UpdateRowSource.Both || command.UpdatedRowSource == UpdateRowSource.FirstReturnedRecord) && dataReader.Read())
						{
							DataTable schemaTable = dataReader.GetSchemaTable();
							foreach (DataRow row in schemaTable.Rows)
							{
								string text2 = row["ColumnName"].ToString();
								string text3 = text2;
								if (columnMappings2 != null && columnMappings2.Contains(text2))
								{
									text3 = columnMappings2[text3].DataSetColumn;
								}
								DataColumn dataColumn = dataRow.Table.Columns[text3];
								if (dataColumn != null && (dataColumn.Expression == null || dataColumn.Expression.Length <= 0))
								{
									bool readOnly = dataColumn.ReadOnly;
									dataColumn.ReadOnly = false;
									try
									{
										dataRow[text3] = dataReader[text2];
									}
									finally
									{
										dataColumn.ReadOnly = readOnly;
									}
								}
							}
						}
						dataReader.Close();
						int recordsAffected = dataReader.RecordsAffected;
						if (recordsAffected == 0)
						{
							throw new DBConcurrencyException("Concurrency violation: the " + text + "Command affected 0 records.", null, new DataRow[1] { dataRow });
						}
						num += recordsAffected;
						if (command.UpdatedRowSource == UpdateRowSource.Both || command.UpdatedRowSource == UpdateRowSource.OutputParameters)
						{
							foreach (IDataParameter parameter2 in command.Parameters)
							{
								if (parameter2.Direction != ParameterDirection.InputOutput && parameter2.Direction != ParameterDirection.Output && parameter2.Direction != ParameterDirection.ReturnValue)
								{
									continue;
								}
								string text4 = parameter2.SourceColumn;
								if (columnMappings2 != null && columnMappings2.Contains(parameter2.SourceColumn))
								{
									text4 = columnMappings2[parameter2.SourceColumn].DataSetColumn;
								}
								DataColumn dataColumn2 = dataRow.Table.Columns[text4];
								if (dataColumn2 != null && (dataColumn2.Expression == null || dataColumn2.Expression.Length <= 0))
								{
									bool readOnly2 = dataColumn2.ReadOnly;
									dataColumn2.ReadOnly = false;
									try
									{
										dataRow[text4] = parameter2.Value;
									}
									finally
									{
										dataColumn2.ReadOnly = readOnly2;
									}
								}
							}
						}
						RowUpdatedEventArgs e2 = CreateRowUpdatedEvent(dataRow, command, statementType, tableMapping);
						OnRowUpdated(e2);
						switch (e2.Status)
						{
						case UpdateStatus.ErrorsOccurred:
							if (e2.Errors == null)
							{
								e2.Errors = ExceptionHelper.RowUpdatedError();
							}
							dataRow.RowError += e2.Errors.Message;
							if (!base.ContinueUpdateOnError)
							{
								throw e2.Errors;
							}
							break;
						case UpdateStatus.SkipCurrentRow:
							goto end_IL_029c;
						case UpdateStatus.SkipAllRemainingRows:
							return num;
						}
						if (base.AcceptChangesDuringUpdate)
						{
							dataRow.AcceptChanges();
						}
						end_IL_029c:;
					}
					catch (Exception ex)
					{
						dataRow.RowError = ex.Message;
						if (!base.ContinueUpdateOnError)
						{
							throw ex;
						}
					}
					finally
					{
						if (dataReader != null && !dataReader.IsClosed)
						{
							dataReader.Close();
						}
					}
					break;
				}
				}
			}
			return num;
		}

		public int Update(DataSet dataSet, string srcTable)
		{
			MissingMappingAction missingMappingAction = MissingMappingAction;
			if (missingMappingAction == MissingMappingAction.Ignore)
			{
				missingMappingAction = MissingMappingAction.Error;
			}
			DataTableMapping tableMappingBySchemaAction = DataTableMappingCollection.GetTableMappingBySchemaAction(base.TableMappings, srcTable, srcTable, missingMappingAction);
			DataTable dataTable = dataSet.Tables[tableMappingBySchemaAction.DataSetTable];
			if (dataTable == null)
			{
				throw new ArgumentException(string.Format("Missing table {0}", srcTable));
			}
			return Update(dataTable, tableMappingBySchemaAction);
		}

		protected virtual int AddToBatch(IDbCommand command)
		{
			throw CreateMethodNotSupportedException();
		}

		protected virtual void ClearBatch()
		{
			throw CreateMethodNotSupportedException();
		}

		protected virtual int ExecuteBatch()
		{
			throw CreateMethodNotSupportedException();
		}

		protected virtual IDataParameter GetBatchedParameter(int commandIdentifier, int parameterIndex)
		{
			throw CreateMethodNotSupportedException();
		}

		protected virtual bool GetBatchedRecordsAffected(int commandIdentifier, out int recordsAffected, out Exception error)
		{
			recordsAffected = 1;
			error = null;
			return true;
		}

		protected virtual void InitializeBatching()
		{
			throw CreateMethodNotSupportedException();
		}

		protected virtual void TerminateBatching()
		{
			throw CreateMethodNotSupportedException();
		}

		private Exception CreateMethodNotSupportedException()
		{
			return new NotSupportedException("Method is not supported.");
		}
	}
}
