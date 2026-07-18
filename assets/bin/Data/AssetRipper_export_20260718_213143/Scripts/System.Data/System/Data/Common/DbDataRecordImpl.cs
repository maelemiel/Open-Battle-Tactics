namespace System.Data.Common
{
	internal class DbDataRecordImpl : DbDataRecord
	{
		private readonly SchemaInfo[] schema;

		private readonly object[] values;

		private readonly int fieldCount;

		public override int FieldCount
		{
			get
			{
				return fieldCount;
			}
		}

		public override object this[string name]
		{
			get
			{
				return this[GetOrdinal(name)];
			}
		}

		public override object this[int i]
		{
			get
			{
				return GetValue(i);
			}
		}

		internal DbDataRecordImpl(SchemaInfo[] schema, object[] values)
		{
			this.schema = schema;
			this.values = values;
			fieldCount = values.Length;
		}

		public override bool GetBoolean(int i)
		{
			return (bool)GetValue(i);
		}

		public override byte GetByte(int i)
		{
			return (byte)GetValue(i);
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			object value = GetValue(i);
			if (!(value is byte[]))
			{
				throw new InvalidCastException("Type is " + value.GetType().ToString());
			}
			if (buffer == null)
			{
				return ((byte[])value).Length;
			}
			Array.Copy((byte[])value, (int)dataIndex, buffer, bufferIndex, length);
			return ((byte[])value).Length - dataIndex;
		}

		public override char GetChar(int i)
		{
			return (char)GetValue(i);
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			object value = GetValue(i);
			char[] array;
			if (value is char[])
			{
				array = (char[])value;
			}
			else
			{
				if (!(value is string))
				{
					throw new InvalidCastException("Type is " + value.GetType().ToString());
				}
				array = ((string)value).ToCharArray();
			}
			if (buffer == null)
			{
				return array.Length;
			}
			Array.Copy(array, (int)dataIndex, buffer, bufferIndex, length);
			return array.Length - dataIndex;
		}

		public override string GetDataTypeName(int i)
		{
			return schema[i].DataTypeName;
		}

		public override DateTime GetDateTime(int i)
		{
			return (DateTime)GetValue(i);
		}

		[System.MonoTODO]
		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override decimal GetDecimal(int i)
		{
			return (decimal)GetValue(i);
		}

		public override double GetDouble(int i)
		{
			return (double)GetValue(i);
		}

		public override Type GetFieldType(int i)
		{
			return schema[i].FieldType;
		}

		public override float GetFloat(int i)
		{
			return (float)GetValue(i);
		}

		public override Guid GetGuid(int i)
		{
			return (Guid)GetValue(i);
		}

		public override short GetInt16(int i)
		{
			return (short)GetValue(i);
		}

		public override int GetInt32(int i)
		{
			return (int)GetValue(i);
		}

		public override long GetInt64(int i)
		{
			return (long)GetValue(i);
		}

		public override string GetName(int i)
		{
			return schema[i].ColumnName;
		}

		public override int GetOrdinal(string name)
		{
			for (int i = 0; i < FieldCount; i++)
			{
				if (schema[i].ColumnName == name)
				{
					return i;
				}
			}
			return -1;
		}

		public override string GetString(int i)
		{
			return (string)GetValue(i);
		}

		public override object GetValue(int i)
		{
			if (i < 0 || i > fieldCount)
			{
				throw new IndexOutOfRangeException();
			}
			return values[i];
		}

		public override int GetValues(object[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = ((values.Length <= this.values.Length) ? values.Length : this.values.Length);
			for (int i = 0; i < num; i++)
			{
				values[i] = this.values[i];
			}
			return num;
		}

		public override bool IsDBNull(int i)
		{
			return GetValue(i) == DBNull.Value;
		}
	}
}
