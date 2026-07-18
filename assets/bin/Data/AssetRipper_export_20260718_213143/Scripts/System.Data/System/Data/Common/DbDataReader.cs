using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbDataReader : MarshalByRefObject, IEnumerable, IDisposable, IDataReader, IDataRecord
	{
		public abstract int Depth { get; }

		public abstract int FieldCount { get; }

		public abstract bool HasRows { get; }

		public abstract bool IsClosed { get; }

		public abstract object this[int index] { get; }

		public abstract object this[string name] { get; }

		public abstract int RecordsAffected { get; }

		public virtual int VisibleFieldCount
		{
			get
			{
				return FieldCount;
			}
		}

		IDataReader IDataRecord.GetData(int i)
		{
			return ((IDataRecord)this).GetData(i);
		}

		public abstract void Close();

		public abstract bool GetBoolean(int i);

		public abstract byte GetByte(int i);

		public abstract long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length);

		public abstract char GetChar(int i);

		public abstract long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public DbDataReader GetData(int i)
		{
			return (DbDataReader)this[i];
		}

		public abstract string GetDataTypeName(int i);

		public abstract DateTime GetDateTime(int i);

		public abstract decimal GetDecimal(int i);

		public abstract double GetDouble(int i);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract IEnumerator GetEnumerator();

		public abstract Type GetFieldType(int i);

		public abstract float GetFloat(int i);

		public abstract Guid GetGuid(int i);

		public abstract short GetInt16(int i);

		public abstract int GetInt32(int i);

		public abstract long GetInt64(int i);

		public abstract string GetName(int i);

		public abstract int GetOrdinal(string name);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Type GetProviderSpecificFieldType(int i)
		{
			return GetFieldType(i);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual object GetProviderSpecificValue(int i)
		{
			return GetValue(i);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int GetProviderSpecificValues(object[] values)
		{
			return GetValues(values);
		}

		protected virtual DbDataReader GetDbDataReader(int ordinal)
		{
			return (DbDataReader)this[ordinal];
		}

		public abstract DataTable GetSchemaTable();

		public abstract string GetString(int i);

		public abstract object GetValue(int i);

		public abstract int GetValues(object[] values);

		public abstract bool IsDBNull(int i);

		public abstract bool NextResult();

		public abstract bool Read();

		internal static DataTable GetSchemaTableTemplate()
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
			return dataTable;
		}
	}
}
