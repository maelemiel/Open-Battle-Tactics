using System.Collections;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	public sealed class SqlBulkCopy : IDisposable
	{
		private int _batchSize;

		private int _notifyAfter;

		private int _bulkCopyTimeout;

		private SqlBulkCopyColumnMappingCollection _columnMappingCollection = new SqlBulkCopyColumnMappingCollection();

		private string _destinationTableName;

		private bool ordinalMapping;

		private bool sqlRowsCopied;

		private bool identityInsert;

		private bool isLocalConnection;

		private SqlConnection connection;

		private SqlBulkCopyOptions copyOptions;

		public int BatchSize
		{
			get
			{
				return _batchSize;
			}
			set
			{
				_batchSize = value;
			}
		}

		public int BulkCopyTimeout
		{
			get
			{
				return _bulkCopyTimeout;
			}
			set
			{
				_bulkCopyTimeout = value;
			}
		}

		public SqlBulkCopyColumnMappingCollection ColumnMappings
		{
			get
			{
				return _columnMappingCollection;
			}
		}

		public string DestinationTableName
		{
			get
			{
				return _destinationTableName;
			}
			set
			{
				_destinationTableName = value;
			}
		}

		public int NotifyAfter
		{
			get
			{
				return _notifyAfter;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("NotifyAfter should be greater than or equal to 0");
				}
				_notifyAfter = value;
			}
		}

		public event SqlRowsCopiedEventHandler SqlRowsCopied;

		public SqlBulkCopy(SqlConnection connection)
		{
			this.connection = connection;
		}

		public SqlBulkCopy(string connectionString)
		{
			connection = new SqlConnection(connectionString);
			isLocalConnection = true;
		}

		[System.MonoTODO]
		public SqlBulkCopy(string connectionString, SqlBulkCopyOptions copyOptions)
		{
			connection = new SqlConnection(connectionString);
			this.copyOptions = copyOptions;
			isLocalConnection = true;
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public SqlBulkCopy(SqlConnection connection, SqlBulkCopyOptions copyOptions, SqlTransaction externalTransaction)
		{
			this.connection = connection;
			this.copyOptions = copyOptions;
			throw new NotImplementedException();
		}

		void IDisposable.Dispose()
		{
			if (isLocalConnection)
			{
				Close();
				connection = null;
			}
		}

		public void Close()
		{
			if (sqlRowsCopied)
			{
				throw new InvalidOperationException("Close should not be called from SqlRowsCopied event");
			}
			if (connection != null && connection.State != ConnectionState.Closed)
			{
				connection.Close();
			}
		}

		private DataTable[] GetColumnMetaData()
		{
			DataTable[] array = new DataTable[2];
			SqlCommand sqlCommand = new SqlCommand("select @@trancount; set fmtonly on select * from " + DestinationTableName + " set fmtonly off;exec sp_tablecollations_90 '" + DestinationTableName + "'", connection);
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			int num = 0;
			do
			{
				switch (num)
				{
				case 1:
					array[num - 1] = sqlDataReader.GetSchemaTable();
					break;
				case 2:
				{
					SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
					sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
					array[num - 1] = new DataTable();
					sqlDataAdapter.FillInternal(array[num - 1], sqlDataReader);
					break;
				}
				}
				num++;
			}
			while (!sqlDataReader.IsClosed && sqlDataReader.NextResult());
			sqlDataReader.Close();
			return array;
		}

		private string GenerateColumnMetaData(SqlCommand tmpCmd, DataTable colMetaData, DataTable tableCollations)
		{
			bool flag = false;
			string text = string.Empty;
			int num = 0;
			foreach (DataRow row in colMetaData.Rows)
			{
				flag = false;
				{
					IEnumerator enumerator2 = colMetaData.Columns.GetEnumerator();
					try
					{
						object value;
						if (enumerator2.MoveNext())
						{
							DataColumn dataColumn = (DataColumn)enumerator2.Current;
							value = null;
							if (_columnMappingCollection.Count <= 0)
							{
								goto IL_0144;
							}
							if (ordinalMapping)
							{
								foreach (SqlBulkCopyColumnMapping item in _columnMappingCollection)
								{
									if (item.DestinationOrdinal == num)
									{
										flag = true;
										break;
									}
								}
							}
							else
							{
								foreach (SqlBulkCopyColumnMapping item2 in _columnMappingCollection)
								{
									if (item2.DestinationColumn == (string)row["ColumnName"])
									{
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								goto IL_0144;
							}
						}
						goto end_IL_0038;
						IL_0176:
						SqlParameter sqlParameter = new SqlParameter((string)row["ColumnName"], (SqlDbType)(int)row["ProviderType"]);
						sqlParameter.Value = value;
						if ((int)row["ColumnSize"] != -1)
						{
							sqlParameter.Size = (int)row["ColumnSize"];
						}
						tmpCmd.Parameters.Add(sqlParameter);
						goto end_IL_0038;
						IL_0144:
						if (!(bool)row["IsReadOnly"])
						{
							goto IL_0176;
						}
						if (ordinalMapping)
						{
							value = false;
							goto IL_0176;
						}
						end_IL_0038:;
					}
					finally
					{
						IDisposable disposable = enumerator2 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				num++;
			}
			flag = false;
			bool flag2 = false;
			foreach (DataRow row2 in colMetaData.Rows)
			{
				if (_columnMappingCollection.Count > 0)
				{
					num = 0;
					flag2 = false;
					foreach (SqlParameter parameter in tmpCmd.Parameters)
					{
						if (ordinalMapping)
						{
							foreach (SqlBulkCopyColumnMapping item3 in _columnMappingCollection)
							{
								if (item3.DestinationOrdinal == num && parameter.Value == null)
								{
									flag2 = true;
								}
							}
						}
						else
						{
							foreach (SqlBulkCopyColumnMapping item4 in _columnMappingCollection)
							{
								if (item4.DestinationColumn == parameter.ParameterName && (string)row2["ColumnName"] == parameter.ParameterName)
								{
									flag2 = true;
									parameter.Value = null;
								}
							}
						}
						num++;
						if (flag2)
						{
							break;
						}
					}
					if (!flag2)
					{
						continue;
					}
				}
				if ((bool)row2["IsReadOnly"])
				{
					continue;
				}
				string empty = string.Empty;
				empty = (((int)row2["ColumnSize"] == -1) ? string.Format("{0}", (SqlDbType)(int)row2["ProviderType"]) : string.Format("{0}({1})", (SqlDbType)(int)row2["ProviderType"], row2["ColumnSize"]));
				if (flag)
				{
					text += ", ";
				}
				string text2 = (string)row2["ColumnName"];
				text += string.Format("[{0}] {1}", text2, empty);
				if (!flag)
				{
					flag = true;
				}
				if (tableCollations == null)
				{
					continue;
				}
				foreach (DataRow row3 in tableCollations.Rows)
				{
					if ((string)row3["name"] == text2)
					{
						text += string.Format(" COLLATE {0}", row3["collation"]);
						break;
					}
				}
			}
			return text;
		}

		private void ValidateColumnMapping(DataTable table, DataTable tableCollations)
		{
			foreach (SqlBulkCopyColumnMapping item in _columnMappingCollection)
			{
				if (!ordinalMapping && (item.DestinationColumn == string.Empty || item.SourceColumn == string.Empty))
				{
					throw new InvalidOperationException("Mappings must be either all null or ordinal");
				}
				if (ordinalMapping && (item.DestinationOrdinal == -1 || item.SourceOrdinal == -1))
				{
					throw new InvalidOperationException("Mappings must be either all null or ordinal");
				}
				bool flag = false;
				if (!ordinalMapping)
				{
					foreach (DataRow row in tableCollations.Rows)
					{
						if ((string)row["name"] == item.DestinationColumn)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new InvalidOperationException("ColumnMapping does not match");
					}
					flag = false;
					foreach (DataColumn column in table.Columns)
					{
						if (column.ColumnName == item.SourceColumn)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new InvalidOperationException("ColumnName " + item.SourceColumn + " does not match");
					}
				}
				else if (item.DestinationOrdinal >= tableCollations.Rows.Count)
				{
					throw new InvalidOperationException("ColumnMapping does not match");
				}
			}
		}

		private void BulkCopyToServer(DataTable table, DataRowState state)
		{
			if (connection == null || connection.State == ConnectionState.Closed)
			{
				throw new InvalidOperationException("This method should not be called on a closed connection");
			}
			if (_destinationTableName == null)
			{
				throw new ArgumentNullException("DestinationTableName");
			}
			if (identityInsert)
			{
				SqlCommand sqlCommand = new SqlCommand("set identity_insert " + table.TableName + " on", connection);
				sqlCommand.ExecuteScalar();
			}
			DataTable[] columnMetaData = GetColumnMetaData();
			DataTable colMetaData = columnMetaData[0];
			DataTable tableCollations = columnMetaData[1];
			if (_columnMappingCollection.Count > 0)
			{
				if (_columnMappingCollection[0].SourceOrdinal != -1)
				{
					ordinalMapping = true;
				}
				ValidateColumnMapping(table, tableCollations);
			}
			SqlCommand sqlCommand2 = new SqlCommand();
			TdsBulkCopy tdsBulkCopy = new TdsBulkCopy(connection.Tds);
			if (connection.Tds.TdsVersion >= TdsVersion.tds70)
			{
				string text = "insert bulk " + DestinationTableName + " (";
				text += GenerateColumnMetaData(sqlCommand2, colMetaData, tableCollations);
				text += ")";
				tdsBulkCopy.SendColumnMetaData(text);
			}
			tdsBulkCopy.BulkCopyStart(sqlCommand2.Parameters.MetaParameters);
			long num = 0L;
			foreach (DataRow row in table.Rows)
			{
				if (row.RowState == DataRowState.Deleted || (state != 0 && row.RowState != state))
				{
					continue;
				}
				bool flag = true;
				int num2 = 0;
				foreach (SqlParameter parameter in sqlCommand2.Parameters)
				{
					int num3 = 0;
					object obj = null;
					if (_columnMappingCollection.Count > 0)
					{
						if (ordinalMapping)
						{
							foreach (SqlBulkCopyColumnMapping item in _columnMappingCollection)
							{
								if (item.DestinationOrdinal != num2 || parameter.Value != null)
								{
									continue;
								}
								obj = row[item.SourceOrdinal];
								SqlParameter sqlParameter2 = new SqlParameter(item.SourceOrdinal.ToString(), obj);
								if (parameter.MetaParameter.TypeName != sqlParameter2.MetaParameter.TypeName)
								{
									sqlParameter2.SqlDbType = parameter.SqlDbType;
									object obj2 = (sqlParameter2.Value = sqlParameter2.ConvertToFrameworkType(obj));
									obj = obj2;
								}
								string text2 = string.Format("{0}", sqlParameter2.MetaParameter.TypeName);
								if (text2 == "nvarchar")
								{
									if (row[num2] != null)
									{
										num3 = ((string)sqlParameter2.Value).Length;
										num3 <<= 1;
									}
								}
								else
								{
									num3 = sqlParameter2.Size;
								}
								break;
							}
						}
						else
						{
							foreach (SqlBulkCopyColumnMapping item2 in _columnMappingCollection)
							{
								if (!(item2.DestinationColumn == parameter.ParameterName))
								{
									continue;
								}
								obj = row[item2.SourceColumn];
								SqlParameter sqlParameter3 = new SqlParameter(item2.SourceColumn, obj);
								if (parameter.MetaParameter.TypeName != sqlParameter3.MetaParameter.TypeName)
								{
									sqlParameter3.SqlDbType = parameter.SqlDbType;
									object obj2 = (sqlParameter3.Value = sqlParameter3.ConvertToFrameworkType(obj));
									obj = obj2;
								}
								string text3 = string.Format("{0}", sqlParameter3.MetaParameter.TypeName);
								if (text3 == "nvarchar")
								{
									if (row[item2.SourceColumn] != null)
									{
										num3 = ((string)obj).Length;
										num3 <<= 1;
									}
								}
								else
								{
									num3 = sqlParameter3.Size;
								}
								break;
							}
						}
						num2++;
					}
					else
					{
						obj = row[parameter.ParameterName];
						string typeName = parameter.MetaParameter.TypeName;
						if (typeName == "nvarchar")
						{
							num3 = ((string)row[parameter.ParameterName]).Length;
							num3 <<= 1;
						}
						else
						{
							num3 = parameter.Size;
						}
					}
					if (obj != null)
					{
						tdsBulkCopy.BulkCopyData(obj, num3, flag);
						if (flag)
						{
							flag = false;
						}
					}
				}
				if (_notifyAfter > 0)
				{
					num++;
					if (num >= _notifyAfter)
					{
						RowsCopied(num);
						num = 0L;
					}
				}
			}
			tdsBulkCopy.BulkCopyEnd();
		}

		public void WriteToServer(DataRow[] rows)
		{
			if (rows == null)
			{
				throw new ArgumentNullException("rows");
			}
			DataTable dataTable = new DataTable(rows[0].Table.TableName);
			foreach (DataColumn column2 in rows[0].Table.Columns)
			{
				DataColumn column = new DataColumn(column2.ColumnName, column2.DataType);
				dataTable.Columns.Add(column);
			}
			foreach (DataRow dataRow in rows)
			{
				DataRow dataRow2 = dataTable.NewRow();
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					dataRow2[j] = dataRow[j];
				}
				dataTable.Rows.Add(dataRow2);
			}
			BulkCopyToServer(dataTable, (DataRowState)0);
		}

		public void WriteToServer(DataTable table)
		{
			BulkCopyToServer(table, (DataRowState)0);
		}

		public void WriteToServer(IDataReader reader)
		{
			DataTable dataTable = new DataTable();
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
			sqlDataAdapter.FillInternal(dataTable, reader);
			BulkCopyToServer(dataTable, (DataRowState)0);
		}

		public void WriteToServer(DataTable table, DataRowState rowState)
		{
			BulkCopyToServer(table, rowState);
		}

		private void RowsCopied(long rowsCopied)
		{
			SqlRowsCopiedEventArgs e = new SqlRowsCopiedEventArgs(rowsCopied);
			if (this.SqlRowsCopied != null)
			{
				this.SqlRowsCopied(this, e);
			}
		}
	}
}
