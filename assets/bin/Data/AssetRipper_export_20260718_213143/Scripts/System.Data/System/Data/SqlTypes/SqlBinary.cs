using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlBinary : IComparable, INullable, IXmlSerializable
	{
		private byte[] value;

		private bool notNull;

		public static readonly SqlBinary Null;

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public byte this[int index]
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException("The property contains Null.");
				}
				if (index >= Length)
				{
					throw new IndexOutOfRangeException("The index parameter indicates a position beyond the length of the byte array.");
				}
				return value[index];
			}
		}

		public int Length
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException("The property contains Null.");
				}
				return value.Length;
			}
		}

		public byte[] Value
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException("The property contains Null.");
				}
				return value;
			}
		}

		public SqlBinary(byte[] value)
		{
			this.value = value;
			notNull = true;
		}

		[System.MonoTODO]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		public static SqlBinary Add(SqlBinary x, SqlBinary y)
		{
			return x + y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlBinary))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlBinary"));
			}
			return CompareTo((SqlBinary)value);
		}

		public int CompareTo(SqlBinary value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return Compare(this, value);
		}

		public static SqlBinary Concat(SqlBinary x, SqlBinary y)
		{
			return x + y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlBinary))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlBinary)value).IsNull;
			}
			if (((SqlBinary)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlBinary)value);
		}

		public static SqlBoolean Equals(SqlBinary x, SqlBinary y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			int num = 10;
			for (int i = 0; i < value.Length; i++)
			{
				num = 91 * num + value[i];
			}
			return num;
		}

		public static SqlBoolean GreaterThan(SqlBinary x, SqlBinary y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlBinary x, SqlBinary y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlBinary x, SqlBinary y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlBinary x, SqlBinary y)
		{
			return x <= y;
		}

		public static SqlBoolean NotEquals(SqlBinary x, SqlBinary y)
		{
			return x != y;
		}

		public SqlGuid ToSqlGuid()
		{
			return (SqlGuid)this;
		}

		public override string ToString()
		{
			if (!notNull)
			{
				return "Null";
			}
			return "SqlBinary(" + value.Length + ")";
		}

		private static int Compare(SqlBinary x, SqlBinary y)
		{
			int num = 0;
			if (x.Value.Length != y.Value.Length)
			{
				num = x.Value.Length - y.Value.Length;
				if (num > 0)
				{
					for (int num2 = x.Value.Length - 1; num2 > x.Value.Length - num; num2--)
					{
						if (x.Value[num2] != 0)
						{
							return 1;
						}
					}
				}
				else
				{
					for (int num3 = y.Value.Length - 1; num3 > y.Value.Length - num; num3--)
					{
						if (y.Value[num3] != 0)
						{
							return -1;
						}
					}
				}
			}
			int num4 = ((num <= 0) ? x.Value.Length : y.Value.Length);
			for (int num5 = num4 - 1; num5 > 0; num5--)
			{
				byte b = x.Value[num5];
				byte b2 = y.Value[num5];
				if (b > b2)
				{
					return 1;
				}
				if (b < b2)
				{
					return -1;
				}
			}
			return 0;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("base64Binary", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlBinary operator +(SqlBinary x, SqlBinary y)
		{
			byte[] array = new byte[x.Value.Length + y.Value.Length];
			int num = 0;
			int i;
			for (i = 0; i < x.Value.Length; i++)
			{
				array[i] = x.Value[i];
			}
			for (; i < x.Value.Length + y.Value.Length; i++)
			{
				array[i] = y.Value[num];
				num++;
			}
			return new SqlBinary(array);
		}

		public static SqlBoolean operator ==(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(Compare(x, y) == 0);
		}

		public static SqlBoolean operator >(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(Compare(x, y) > 0);
		}

		public static SqlBoolean operator >=(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(Compare(x, y) >= 0);
		}

		public static SqlBoolean operator !=(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(Compare(x, y) != 0);
		}

		public static SqlBoolean operator <(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(Compare(x, y) < 0);
		}

		public static SqlBoolean operator <=(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(Compare(x, y) <= 0);
		}

		public static explicit operator byte[](SqlBinary x)
		{
			return x.Value;
		}

		public static explicit operator SqlBinary(SqlGuid x)
		{
			return new SqlBinary(x.ToByteArray());
		}

		public static implicit operator SqlBinary(byte[] x)
		{
			return new SqlBinary(x);
		}
	}
}
