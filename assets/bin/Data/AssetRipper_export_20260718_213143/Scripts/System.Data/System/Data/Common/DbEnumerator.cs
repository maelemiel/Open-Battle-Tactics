using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public class DbEnumerator : IEnumerator
	{
		private readonly IDataReader reader;

		private readonly bool closeReader;

		private readonly SchemaInfo[] schema;

		private readonly object[] values;

		public object Current
		{
			get
			{
				reader.GetValues(values);
				return new DbDataRecordImpl(schema, values);
			}
		}

		public DbEnumerator(IDataReader reader)
			: this(reader, false)
		{
		}

		public DbEnumerator(IDataReader reader, bool closeReader)
		{
			this.reader = reader;
			this.closeReader = closeReader;
			values = new object[reader.FieldCount];
			schema = LoadSchema(reader);
		}

		private static SchemaInfo[] LoadSchema(IDataReader reader)
		{
			int fieldCount = reader.FieldCount;
			SchemaInfo[] array = new SchemaInfo[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				SchemaInfo schemaInfo = new SchemaInfo();
				schemaInfo.ColumnName = reader.GetName(i);
				schemaInfo.ColumnOrdinal = i;
				schemaInfo.DataTypeName = reader.GetDataTypeName(i);
				schemaInfo.FieldType = reader.GetFieldType(i);
				array[i] = schemaInfo;
			}
			return array;
		}

		public bool MoveNext()
		{
			if (reader.Read())
			{
				return true;
			}
			if (closeReader)
			{
				reader.Close();
			}
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset()
		{
			throw new NotSupportedException();
		}
	}
}
