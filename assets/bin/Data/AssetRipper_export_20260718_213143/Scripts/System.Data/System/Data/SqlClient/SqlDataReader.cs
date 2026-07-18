using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	public class SqlDataReader : DbDataReader, IDisposable, IDataReader, IDataRecord
	{
		private const int COLUMN_NAME_IDX = 0;

		private const int COLUMN_ORDINAL_IDX = 1;

		private const int COLUMN_SIZE_IDX = 2;

		private const int NUMERIC_PRECISION_IDX = 3;

		private const int NUMERIC_SCALE_IDX = 4;

		private const int IS_UNIQUE_IDX = 5;

		private const int IS_KEY_IDX = 6;

		private const int BASE_SERVER_NAME_IDX = 7;

		private const int BASE_CATALOG_NAME_IDX = 8;

		private const int BASE_COLUMN_NAME_IDX = 9;

		private const int BASE_SCHEMA_NAME_IDX = 10;

		private const int BASE_TABLE_NAME_IDX = 11;

		private const int DATA_TYPE_IDX = 12;

		private const int ALLOW_DBNULL_IDX = 13;

		private const int PROVIDER_TYPE_IDX = 14;

		private const int IS_ALIASED_IDX = 15;

		private const int IS_EXPRESSION_IDX = 16;

		private const int IS_IDENTITY_IDX = 17;

		private const int IS_AUTO_INCREMENT_IDX = 18;

		private const int IS_ROW_VERSION_IDX = 19;

		private const int IS_HIDDEN_IDX = 20;

		private const int IS_LONG_IDX = 21;

		private const int IS_READ_ONLY_IDX = 22;

		private const int PROVIDER_SPECIFIC_TYPE_IDX = 23;

		private const int DATA_TYPE_NAME_IDX = 24;

		private const int XML_SCHEMA_COLLCTN_DB_IDX = 25;

		private const int XML_SCHEMA_COLLCTN_OWN_SCHEMA_IDX = 26;

		private const int XML_SCHEMA_COLLCTN_NAME_IDX = 27;

		private const int UDT_ASMBLY_QUALIFIED_NAME_IDX = 28;

		private const int NON_VER_PROVIDER_TYPE_IDX = 29;

		private const int IS_COLUMN_SET = 30;

		private SqlCommand command;

		private bool disposed;

		private bool isClosed;

		private bool moreResults;

		private int resultsRead;

		private int rowsRead;

		private DataTable schemaTable;

		private bool haveRead;

		private bool readResult;

		private bool readResultUsed;

		private int visibleFieldCount;

		public override int Depth
		{
			get
			{
				return 0;
			}
		}

		public override int FieldCount
		{
			get
			{
				ValidateState();
				return command.Tds.Columns.Count;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return isClosed;
			}
		}

		public override object this[int i]
		{
			get
			{
				return GetValue(i);
			}
		}

		public override object this[string name]
		{
			get
			{
				return GetValue(GetOrdinal(name));
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return command.Tds.RecordsAffected;
			}
		}

		public override bool HasRows
		{
			get
			{
				ValidateState();
				if (rowsRead > 0)
				{
					return true;
				}
				if (!haveRead)
				{
					readResult = ReadRecord();
				}
				return readResult;
			}
		}

		public override int VisibleFieldCount
		{
			get
			{
				return visibleFieldCount;
			}
		}

		protected SqlConnection Connection
		{
			get
			{
				return command.Connection;
			}
		}

		internal SqlDataReader(SqlCommand command)
		{
			this.command = command;
			command.Tds.RecordsAffected = -1;
			NextResult();
		}

		protected bool IsCommandBehavior(CommandBehavior condition)
		{
			return condition == command.CommandBehavior;
		}

		public override void Close()
		{
			if (!IsClosed)
			{
				while (NextResult())
				{
				}
				isClosed = true;
				command.CloseDataReader();
			}
		}

		private static DataTable ConstructSchemaTable()
		{
			Type typeFromHandle = typeof(bool);
			Type typeFromHandle2 = typeof(string);
			Type typeFromHandle3 = typeof(int);
			Type typeFromHandle4 = typeof(Type);
			Type typeFromHandle5 = typeof(short);
			DataTable dataTable = new DataTable("SchemaTable");
			dataTable.Columns.Add("ColumnName", typeFromHandle2);
			dataTable.Columns.Add("ColumnOrdinal", typeFromHandle3);
			dataTable.Columns.Add("ColumnSize", typeFromHandle3);
			dataTable.Columns.Add("NumericPrecision", typeFromHandle5);
			dataTable.Columns.Add("NumericScale", typeFromHandle5);
			dataTable.Columns.Add("IsUnique", typeFromHandle);
			dataTable.Columns.Add("IsKey", typeFromHandle);
			dataTable.Columns.Add("BaseServerName", typeFromHandle2);
			dataTable.Columns.Add("BaseCatalogName", typeFromHandle2);
			dataTable.Columns.Add("BaseColumnName", typeFromHandle2);
			dataTable.Columns.Add("BaseSchemaName", typeFromHandle2);
			dataTable.Columns.Add("BaseTableName", typeFromHandle2);
			dataTable.Columns.Add("DataType", typeFromHandle4);
			dataTable.Columns.Add("AllowDBNull", typeFromHandle);
			dataTable.Columns.Add("ProviderType", typeFromHandle3);
			dataTable.Columns.Add("IsAliased", typeFromHandle);
			dataTable.Columns.Add("IsExpression", typeFromHandle);
			dataTable.Columns.Add("IsIdentity", typeFromHandle);
			dataTable.Columns.Add("IsAutoIncrement", typeFromHandle);
			dataTable.Columns.Add("IsRowVersion", typeFromHandle);
			dataTable.Columns.Add("IsHidden", typeFromHandle);
			dataTable.Columns.Add("IsLong", typeFromHandle);
			dataTable.Columns.Add("IsReadOnly", typeFromHandle);
			dataTable.Columns.Add("ProviderSpecificDataType", typeFromHandle4);
			dataTable.Columns.Add("DataTypeName", typeFromHandle2);
			dataTable.Columns.Add("XmlSchemaCollectionDatabase", typeFromHandle2);
			dataTable.Columns.Add("XmlSchemaCollectionOwningSchema", typeFromHandle2);
			dataTable.Columns.Add("XmlSchemaCollectionName", typeFromHandle2);
			dataTable.Columns.Add("UdtAssemblyQualifiedName", typeFromHandle2);
			dataTable.Columns.Add("NonVersionedProviderType", typeFromHandle3);
			dataTable.Columns.Add("IsColumnSet", typeFromHandle);
			return dataTable;
		}

		private string GetSchemaRowTypeName(TdsColumnType ctype, int csize, short precision, short scale)
		{
			int dbType;
			Type fieldType;
			bool isLong;
			string typeName;
			GetSchemaRowType(ctype, csize, precision, scale, out dbType, out fieldType, out isLong, out typeName);
			return typeName;
		}

		private Type GetSchemaRowFieldType(TdsColumnType ctype, int csize, short precision, short scale)
		{
			int dbType;
			Type fieldType;
			bool isLong;
			string typeName;
			GetSchemaRowType(ctype, csize, precision, scale, out dbType, out fieldType, out isLong, out typeName);
			return fieldType;
		}

		private SqlDbType GetSchemaRowDbType(int ordinal)
		{
			if (ordinal < 0 || ordinal >= command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			TdsDataColumn tdsDataColumn = command.Tds.Columns[ordinal];
			TdsColumnType value = tdsDataColumn.ColumnType.Value;
			int value2 = tdsDataColumn.ColumnSize.Value;
			short? numericPrecision = tdsDataColumn.NumericPrecision;
			short precision = (short)(numericPrecision.HasValue ? numericPrecision.Value : 0);
			short? numericScale = tdsDataColumn.NumericScale;
			short scale = (short)(numericScale.HasValue ? numericScale.Value : 0);
			return GetSchemaRowDbType(value, value2, precision, scale);
		}

		private SqlDbType GetSchemaRowDbType(TdsColumnType ctype, int csize, short precision, short scale)
		{
			int dbType;
			Type fieldType;
			bool isLong;
			string typeName;
			GetSchemaRowType(ctype, csize, precision, scale, out dbType, out fieldType, out isLong, out typeName);
			return (SqlDbType)dbType;
		}

		private void GetSchemaRowType(TdsColumnType ctype, int csize, short precision, short scale, out int dbType, out Type fieldType, out bool isLong, out string typeName)
		{
			dbType = -1;
			typeName = string.Empty;
			isLong = false;
			fieldType = typeof(Type);
			switch (ctype)
			{
			case TdsColumnType.IntN:
			case TdsColumnType.Int1:
			case TdsColumnType.Int2:
			case TdsColumnType.Int4:
			case TdsColumnType.BigInt:
				switch (csize)
				{
				case 1:
					typeName = "tinyint";
					dbType = 20;
					fieldType = typeof(byte);
					isLong = false;
					break;
				case 2:
					typeName = "smallint";
					dbType = 16;
					fieldType = typeof(short);
					isLong = false;
					break;
				case 4:
					typeName = "int";
					dbType = 8;
					fieldType = typeof(int);
					isLong = false;
					break;
				case 8:
					typeName = "bigint";
					dbType = 0;
					fieldType = typeof(long);
					isLong = false;
					break;
				case 3:
				case 5:
				case 6:
				case 7:
					break;
				}
				break;
			case TdsColumnType.Real:
			case TdsColumnType.Float8:
			case TdsColumnType.FloatN:
				switch (csize)
				{
				case 4:
					typeName = "real";
					dbType = 13;
					fieldType = typeof(float);
					isLong = false;
					break;
				case 8:
					typeName = "float";
					dbType = 6;
					fieldType = typeof(double);
					isLong = false;
					break;
				}
				break;
			case TdsColumnType.Image:
				typeName = "image";
				dbType = 7;
				fieldType = typeof(byte[]);
				isLong = true;
				break;
			case TdsColumnType.Text:
				typeName = "text";
				dbType = 18;
				fieldType = typeof(string);
				isLong = true;
				break;
			case TdsColumnType.UniqueIdentifier:
				typeName = "uniqueidentifier";
				dbType = 14;
				fieldType = typeof(Guid);
				isLong = false;
				break;
			case TdsColumnType.VarBinary:
			case TdsColumnType.BigVarBinary:
				typeName = "varbinary";
				dbType = 21;
				fieldType = typeof(byte[]);
				isLong = false;
				break;
			case TdsColumnType.VarChar:
			case TdsColumnType.BigVarChar:
				typeName = "varchar";
				dbType = 22;
				fieldType = typeof(string);
				isLong = false;
				break;
			case TdsColumnType.Binary:
			case TdsColumnType.BigBinary:
				typeName = "binary";
				dbType = 1;
				fieldType = typeof(byte[]);
				isLong = false;
				break;
			case TdsColumnType.Char:
			case TdsColumnType.BigChar:
				typeName = "char";
				dbType = 3;
				fieldType = typeof(string);
				isLong = false;
				break;
			case TdsColumnType.Bit:
			case TdsColumnType.BitN:
				typeName = "bit";
				dbType = 2;
				fieldType = typeof(bool);
				isLong = false;
				break;
			case TdsColumnType.DateTime4:
			case TdsColumnType.DateTime:
			case TdsColumnType.DateTimeN:
				switch (csize)
				{
				case 4:
					typeName = "smalldatetime";
					dbType = 15;
					fieldType = typeof(DateTime);
					isLong = false;
					break;
				case 8:
					typeName = "datetime";
					dbType = 4;
					fieldType = typeof(DateTime);
					isLong = false;
					break;
				}
				break;
			case TdsColumnType.Money:
			case TdsColumnType.MoneyN:
			case TdsColumnType.Money4:
				switch (csize)
				{
				case 4:
					typeName = "smallmoney";
					dbType = 17;
					fieldType = typeof(decimal);
					isLong = false;
					break;
				case 8:
					typeName = "money";
					dbType = 9;
					fieldType = typeof(decimal);
					isLong = false;
					break;
				}
				break;
			case TdsColumnType.NText:
				typeName = "ntext";
				dbType = 11;
				fieldType = typeof(string);
				isLong = true;
				break;
			case TdsColumnType.NVarChar:
				typeName = "nvarchar";
				dbType = 12;
				fieldType = typeof(string);
				isLong = false;
				break;
			case TdsColumnType.Decimal:
			case TdsColumnType.Numeric:
				if (precision == 19 && scale == 0)
				{
					typeName = "bigint";
					dbType = 0;
					fieldType = typeof(long);
				}
				else
				{
					typeName = "decimal";
					dbType = 5;
					fieldType = typeof(decimal);
				}
				isLong = false;
				break;
			case TdsColumnType.NChar:
				typeName = "nchar";
				dbType = 10;
				fieldType = typeof(string);
				isLong = false;
				break;
			case TdsColumnType.SmallMoney:
				typeName = "smallmoney";
				dbType = 17;
				fieldType = typeof(decimal);
				isLong = false;
				break;
			default:
				typeName = "variant";
				dbType = 23;
				fieldType = typeof(object);
				isLong = false;
				break;
			}
		}

		private new void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			if (disposing)
			{
				if (schemaTable != null)
				{
					schemaTable.Dispose();
				}
				Close();
				command = null;
			}
			disposed = true;
		}

		public override bool GetBoolean(int i)
		{
			object value = GetValue(i);
			if (!(value is bool))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (bool)value;
		}

		public override byte GetByte(int i)
		{
			object value = GetValue(i);
			if (!(value is byte))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (byte)value;
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			if ((command.CommandBehavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default)
			{
				ValidateState();
				EnsureDataAvailable();
				try
				{
					long sequentialColumnValue = command.Tds.GetSequentialColumnValue(i, dataIndex, buffer, bufferIndex, length);
					switch (sequentialColumnValue)
					{
					case -1L:
						throw CreateGetBytesOnInvalidColumnTypeException(i);
					case -2L:
						throw new SqlNullValueException();
					default:
						return sequentialColumnValue;
					}
				}
				catch (TdsInternalException e)
				{
					command.Connection.Close();
					throw SqlException.FromTdsInternalException(e);
				}
			}
			object obj = GetValue(i);
			if (!(obj is byte[]))
			{
				switch (GetSchemaRowDbType(i))
				{
				case SqlDbType.Image:
					if (obj is DBNull)
					{
						throw new SqlNullValueException();
					}
					break;
				case SqlDbType.Text:
				{
					string text2 = obj as string;
					obj = ((text2 == null) ? null : Encoding.Default.GetBytes(text2));
					break;
				}
				case SqlDbType.NText:
				{
					string text = obj as string;
					obj = ((text == null) ? null : Encoding.Unicode.GetBytes(text));
					break;
				}
				default:
					throw CreateGetBytesOnInvalidColumnTypeException(i);
				}
			}
			if (buffer == null)
			{
				return ((byte[])obj).Length;
			}
			int num = (int)(((byte[])obj).Length - dataIndex);
			if (num < length)
			{
				length = num;
			}
			if (dataIndex < 0)
			{
				return 0L;
			}
			Array.Copy((byte[])obj, (int)dataIndex, buffer, bufferIndex, length);
			return length;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override char GetChar(int i)
		{
			throw new NotSupportedException();
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			if ((command.CommandBehavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default)
			{
				ValidateState();
				EnsureDataAvailable();
				if (i < 0 || i >= command.Tds.Columns.Count)
				{
					throw new IndexOutOfRangeException();
				}
				Encoding encoding = null;
				byte b = 1;
				switch ((TdsColumnType)(int)command.Tds.Columns[i]["ColumnType"])
				{
				case TdsColumnType.Text:
				case TdsColumnType.VarChar:
				case TdsColumnType.Char:
				case TdsColumnType.BigVarChar:
					encoding = Encoding.ASCII;
					break;
				case TdsColumnType.NText:
				case TdsColumnType.NVarChar:
				case TdsColumnType.NChar:
					encoding = Encoding.Unicode;
					b = 2;
					break;
				default:
					return -1L;
				}
				long num = 0L;
				if (buffer == null)
				{
					num = GetBytes(i, 0L, null, 0, 0);
					return num / (int)b;
				}
				length *= b;
				byte[] array = new byte[length];
				num = GetBytes(i, dataIndex, array, 0, length);
				if (num == -1)
				{
					throw new InvalidCastException("Specified cast is not valid");
				}
				char[] chars = encoding.GetChars(array, 0, (int)num);
				chars.CopyTo(buffer, bufferIndex);
				return chars.Length;
			}
			object value = GetValue(i);
			char[] array2;
			if (value is char[])
			{
				array2 = (char[])value;
			}
			else
			{
				if (!(value is string))
				{
					if (value is DBNull)
					{
						throw new SqlNullValueException();
					}
					throw new InvalidCastException("Type is " + value.GetType().ToString());
				}
				array2 = ((string)value).ToCharArray();
			}
			if (buffer == null)
			{
				return array2.Length;
			}
			Array.Copy(array2, (int)dataIndex, buffer, bufferIndex, length);
			return array2.Length - dataIndex;
		}

		public override string GetDataTypeName(int i)
		{
			ValidateState();
			if (i < 0 || i >= command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			TdsDataColumn tdsDataColumn = command.Tds.Columns[i];
			TdsColumnType value = tdsDataColumn.ColumnType.Value;
			int value2 = tdsDataColumn.ColumnSize.Value;
			short? numericPrecision = tdsDataColumn.NumericPrecision;
			short precision = (short)(numericPrecision.HasValue ? numericPrecision.Value : 0);
			short? numericScale = tdsDataColumn.NumericScale;
			short scale = (short)(numericScale.HasValue ? numericScale.Value : 0);
			return GetSchemaRowTypeName(value, value2, precision, scale);
		}

		public override DateTime GetDateTime(int i)
		{
			object value = GetValue(i);
			if (!(value is DateTime))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (DateTime)value;
		}

		public override decimal GetDecimal(int i)
		{
			object value = GetValue(i);
			if (!(value is decimal))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (decimal)value;
		}

		public override double GetDouble(int i)
		{
			object value = GetValue(i);
			if (!(value is double))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (double)value;
		}

		public override Type GetFieldType(int i)
		{
			ValidateState();
			if (i < 0 || i >= command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			TdsDataColumn tdsDataColumn = command.Tds.Columns[i];
			TdsColumnType value = tdsDataColumn.ColumnType.Value;
			int value2 = tdsDataColumn.ColumnSize.Value;
			short? numericPrecision = tdsDataColumn.NumericPrecision;
			short precision = (short)(numericPrecision.HasValue ? numericPrecision.Value : 0);
			short? numericScale = tdsDataColumn.NumericScale;
			short scale = (short)(numericScale.HasValue ? numericScale.Value : 0);
			return GetSchemaRowFieldType(value, value2, precision, scale);
		}

		public override float GetFloat(int i)
		{
			object value = GetValue(i);
			if (!(value is float))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (float)value;
		}

		public override Guid GetGuid(int i)
		{
			object value = GetValue(i);
			if (!(value is Guid))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (Guid)value;
		}

		public override short GetInt16(int i)
		{
			object value = GetValue(i);
			if (!(value is short))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (short)value;
		}

		public override int GetInt32(int i)
		{
			object value = GetValue(i);
			if (!(value is int))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (int)value;
		}

		public override long GetInt64(int i)
		{
			object value = GetValue(i);
			if (!(value is long))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (long)value;
		}

		public override string GetName(int i)
		{
			ValidateState();
			if (i < 0 || i >= command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return command.Tds.Columns[i].ColumnName;
		}

		public override int GetOrdinal(string name)
		{
			ValidateState();
			if (name == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			foreach (TdsDataColumn column in command.Tds.Columns)
			{
				string columnName = column.ColumnName;
				if (columnName.Equals(name) || string.Compare(columnName, name, true) == 0)
				{
					return column.ColumnOrdinal.Value;
				}
			}
			throw new IndexOutOfRangeException();
		}

		public override DataTable GetSchemaTable()
		{
			ValidateState();
			if (schemaTable == null)
			{
				schemaTable = ConstructSchemaTable();
			}
			if (schemaTable.Rows != null && schemaTable.Rows.Count > 0)
			{
				return schemaTable;
			}
			if (!moreResults)
			{
				return null;
			}
			foreach (TdsDataColumn column in command.Tds.Columns)
			{
				DataRow dataRow = schemaTable.NewRow();
				dataRow[0] = GetSchemaValue(column.ColumnName);
				dataRow[1] = GetSchemaValue(column.ColumnOrdinal);
				dataRow[5] = GetSchemaValue(column.IsUnique);
				dataRow[18] = GetSchemaValue(column.IsAutoIncrement);
				dataRow[19] = GetSchemaValue(column.IsRowVersion);
				dataRow[20] = GetSchemaValue(column.IsHidden);
				dataRow[17] = GetSchemaValue(column.IsIdentity);
				dataRow[3] = GetSchemaValue(column.NumericPrecision);
				dataRow[6] = GetSchemaValue(column.IsKey);
				dataRow[15] = GetSchemaValue(column.IsAliased);
				dataRow[16] = GetSchemaValue(column.IsExpression);
				dataRow[22] = GetSchemaValue(column.IsReadOnly);
				dataRow[7] = GetSchemaValue(column.BaseServerName);
				dataRow[8] = GetSchemaValue(column.BaseCatalogName);
				dataRow[9] = GetSchemaValue(column.BaseColumnName);
				dataRow[10] = GetSchemaValue(column.BaseSchemaName);
				dataRow[11] = GetSchemaValue(column.BaseTableName);
				dataRow[13] = GetSchemaValue(column.AllowDBNull);
				dataRow[23] = DBNull.Value;
				dataRow[24] = GetSchemaValue(column.DataTypeName);
				dataRow[25] = DBNull.Value;
				dataRow[26] = DBNull.Value;
				dataRow[27] = DBNull.Value;
				dataRow[28] = DBNull.Value;
				dataRow[29] = DBNull.Value;
				dataRow[30] = DBNull.Value;
				if (dataRow[9] == DBNull.Value)
				{
					dataRow[9] = dataRow[0];
				}
				TdsColumnType value = column.ColumnType.Value;
				int value2 = column.ColumnSize.Value;
				short num = (short)GetSchemaValue(column.NumericPrecision);
				short num2 = (short)GetSchemaValue(column.NumericScale);
				int dbType;
				Type fieldType;
				bool isLong;
				string typeName;
				GetSchemaRowType(value, value2, num, num2, out dbType, out fieldType, out isLong, out typeName);
				dataRow[2] = value2;
				dataRow[3] = num;
				dataRow[4] = num2;
				dataRow[14] = dbType;
				dataRow[12] = fieldType;
				dataRow[21] = isLong;
				if (!(bool)dataRow[20])
				{
					visibleFieldCount++;
				}
				schemaTable.Rows.Add(dataRow);
			}
			return schemaTable;
		}

		private static object GetSchemaValue(TdsDataColumn schema, string key)
		{
			object obj = schema[key];
			if (obj != null)
			{
				return obj;
			}
			return DBNull.Value;
		}

		private static object GetSchemaValue(object value)
		{
			if (value == null)
			{
				return DBNull.Value;
			}
			return value;
		}

		public virtual SqlBinary GetSqlBinary(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlBinary))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlBinary)sqlValue;
		}

		public virtual SqlBoolean GetSqlBoolean(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlBoolean))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlBoolean)sqlValue;
		}

		public virtual SqlByte GetSqlByte(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlByte))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlByte)sqlValue;
		}

		public virtual SqlDateTime GetSqlDateTime(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlDateTime))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlDateTime)sqlValue;
		}

		public virtual SqlDecimal GetSqlDecimal(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlDecimal))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlDecimal)sqlValue;
		}

		public virtual SqlDouble GetSqlDouble(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlDouble))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlDouble)sqlValue;
		}

		public virtual SqlGuid GetSqlGuid(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlGuid))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlGuid)sqlValue;
		}

		public virtual SqlInt16 GetSqlInt16(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlInt16))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlInt16)sqlValue;
		}

		public virtual SqlInt32 GetSqlInt32(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlInt32))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlInt32)sqlValue;
		}

		public virtual SqlInt64 GetSqlInt64(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlInt64))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlInt64)sqlValue;
		}

		public virtual SqlMoney GetSqlMoney(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlMoney))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlMoney)sqlValue;
		}

		public virtual SqlSingle GetSqlSingle(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlSingle))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlSingle)sqlValue;
		}

		public virtual SqlString GetSqlString(int i)
		{
			object sqlValue = GetSqlValue(i);
			if (!(sqlValue is SqlString))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlString)sqlValue;
		}

		public virtual SqlXml GetSqlXml(int i)
		{
			object obj = GetSqlValue(i);
			if (!(obj is SqlXml))
			{
				if (obj is DBNull)
				{
					throw new SqlNullValueException();
				}
				if (command.Tds.TdsVersion > TdsVersion.tds80 || !(obj is SqlString))
				{
					throw new InvalidCastException("Type is " + obj.GetType().ToString());
				}
				MemoryStream value = null;
				if (!((SqlString)obj).IsNull)
				{
					value = new MemoryStream(Encoding.Unicode.GetBytes(obj.ToString()));
				}
				obj = new SqlXml(value);
			}
			return (SqlXml)obj;
		}

		public virtual object GetSqlValue(int i)
		{
			object value = GetValue(i);
			switch (GetSchemaRowDbType(i))
			{
			case SqlDbType.BigInt:
				if (value == DBNull.Value)
				{
					return SqlInt64.Null;
				}
				return (SqlInt64)(long)value;
			case SqlDbType.Binary:
			case SqlDbType.Image:
			case SqlDbType.Timestamp:
			case SqlDbType.VarBinary:
				if (value == DBNull.Value)
				{
					return SqlBinary.Null;
				}
				return (SqlBinary)(byte[])value;
			case SqlDbType.Bit:
				if (value == DBNull.Value)
				{
					return SqlBoolean.Null;
				}
				return (SqlBoolean)(bool)value;
			case SqlDbType.Char:
			case SqlDbType.NChar:
			case SqlDbType.NText:
			case SqlDbType.NVarChar:
			case SqlDbType.Text:
			case SqlDbType.VarChar:
				if (value == DBNull.Value)
				{
					return SqlString.Null;
				}
				return (SqlString)(string)value;
			case SqlDbType.DateTime:
			case SqlDbType.SmallDateTime:
				if (value == DBNull.Value)
				{
					return SqlDateTime.Null;
				}
				return (SqlDateTime)(DateTime)value;
			case SqlDbType.Decimal:
				if (value == DBNull.Value)
				{
					return SqlDecimal.Null;
				}
				if (value is TdsBigDecimal)
				{
					return SqlDecimal.FromTdsBigDecimal((TdsBigDecimal)value);
				}
				return (SqlDecimal)(decimal)value;
			case SqlDbType.Float:
				if (value == DBNull.Value)
				{
					return SqlDouble.Null;
				}
				return (SqlDouble)(double)value;
			case SqlDbType.Int:
				if (value == DBNull.Value)
				{
					return SqlInt32.Null;
				}
				return (SqlInt32)(int)value;
			case SqlDbType.Money:
			case SqlDbType.SmallMoney:
				if (value == DBNull.Value)
				{
					return SqlMoney.Null;
				}
				return (SqlMoney)(decimal)value;
			case SqlDbType.Real:
				if (value == DBNull.Value)
				{
					return SqlSingle.Null;
				}
				return (SqlSingle)(float)value;
			case SqlDbType.UniqueIdentifier:
				if (value == DBNull.Value)
				{
					return SqlGuid.Null;
				}
				return (SqlGuid)(Guid)value;
			case SqlDbType.SmallInt:
				if (value == DBNull.Value)
				{
					return SqlInt16.Null;
				}
				return (SqlInt16)(short)value;
			case SqlDbType.TinyInt:
				if (value == DBNull.Value)
				{
					return SqlByte.Null;
				}
				return (SqlByte)(byte)value;
			case SqlDbType.Xml:
				if (value == DBNull.Value)
				{
					return SqlByte.Null;
				}
				return (SqlXml)value;
			default:
				throw new InvalidOperationException("The type of this column is unknown.");
			}
		}

		public virtual int GetSqlValues(object[] values)
		{
			ValidateState();
			EnsureDataAvailable();
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = 0;
			int count = command.Tds.Columns.Count;
			int num2 = values.Length;
			num = ((num2 <= count) ? num2 : count);
			for (int i = 0; i < num; i++)
			{
				values[i] = GetSqlValue(i);
			}
			return num;
		}

		public override string GetString(int i)
		{
			object value = GetValue(i);
			if (!(value is string))
			{
				if (value is DBNull)
				{
					throw new SqlNullValueException();
				}
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			return (string)value;
		}

		public override object GetValue(int i)
		{
			ValidateState();
			EnsureDataAvailable();
			if (i < 0 || i >= command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			try
			{
				if ((command.CommandBehavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default)
				{
					return command.Tds.GetSequentialColumnValue(i);
				}
			}
			catch (TdsInternalException e)
			{
				command.Connection.Close();
				throw SqlException.FromTdsInternalException(e);
			}
			return command.Tds.ColumnValues[i];
		}

		public override int GetValues(object[] values)
		{
			ValidateState();
			EnsureDataAvailable();
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = values.Length;
			int bigDecimalIndex = command.Tds.ColumnValues.BigDecimalIndex;
			if (bigDecimalIndex >= 0 && bigDecimalIndex < num)
			{
				throw new OverflowException();
			}
			try
			{
				command.Tds.ColumnValues.CopyTo(0, values, 0, (num <= command.Tds.ColumnValues.Count) ? num : command.Tds.ColumnValues.Count);
			}
			catch (TdsInternalException e)
			{
				command.Connection.Close();
				throw SqlException.FromTdsInternalException(e);
			}
			return (num >= FieldCount) ? FieldCount : num;
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this);
		}

		public override bool IsDBNull(int i)
		{
			return GetValue(i) == DBNull.Value;
		}

		public override bool NextResult()
		{
			ValidateState();
			if ((command.CommandBehavior & CommandBehavior.SingleResult) != CommandBehavior.Default && resultsRead > 0)
			{
				moreResults = false;
				rowsRead = 0;
				haveRead = false;
				return false;
			}
			try
			{
				moreResults = command.Tds.NextResult();
			}
			catch (TdsInternalException e)
			{
				command.Connection.Close();
				throw SqlException.FromTdsInternalException(e);
			}
			if (!moreResults)
			{
				command.GetOutputParameters();
			}
			else
			{
				schemaTable = null;
			}
			rowsRead = 0;
			haveRead = false;
			resultsRead++;
			return moreResults;
		}

		public override bool Read()
		{
			ValidateState();
			if (!haveRead || readResultUsed)
			{
				readResult = ReadRecord();
			}
			readResultUsed = true;
			return readResult;
		}

		internal bool ReadRecord()
		{
			readResultUsed = false;
			if ((command.CommandBehavior & CommandBehavior.SingleRow) != CommandBehavior.Default && haveRead)
			{
				return false;
			}
			if ((command.CommandBehavior & CommandBehavior.SchemaOnly) != CommandBehavior.Default)
			{
				return false;
			}
			if (!moreResults)
			{
				return false;
			}
			try
			{
				bool flag = command.Tds.NextRow();
				if (flag)
				{
					rowsRead++;
				}
				haveRead = true;
				return flag;
			}
			catch (TdsInternalException e)
			{
				command.Connection.Close();
				throw SqlException.FromTdsInternalException(e);
			}
		}

		private void ValidateState()
		{
			if (IsClosed)
			{
				throw new InvalidOperationException("Invalid attempt to read data when reader is closed");
			}
		}

		private void EnsureDataAvailable()
		{
			if (!readResult || !haveRead || !readResultUsed)
			{
				throw new InvalidOperationException("No data available.");
			}
		}

		private InvalidCastException CreateGetBytesOnInvalidColumnTypeException(int ordinal)
		{
			string message = string.Format(CultureInfo.InvariantCulture, "Invalid attempt to GetBytes on column '{0}'.The GetBytes function can only be used on columns of type Text, NText, or Image.", GetName(ordinal));
			return new InvalidCastException(message);
		}

		public override Type GetProviderSpecificFieldType(int i)
		{
			return GetSqlValue(i).GetType();
		}

		public override object GetProviderSpecificValue(int i)
		{
			return GetSqlValue(i);
		}

		public override int GetProviderSpecificValues(object[] values)
		{
			return GetSqlValues(values);
		}

		public virtual SqlBytes GetSqlBytes(int i)
		{
			byte[] buffer = (byte[])GetValue(i);
			return new SqlBytes(buffer);
		}
	}
}
