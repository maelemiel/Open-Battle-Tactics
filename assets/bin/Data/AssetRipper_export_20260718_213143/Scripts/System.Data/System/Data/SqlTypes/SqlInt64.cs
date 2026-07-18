using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlInt64 : IComparable, INullable, IXmlSerializable
	{
		private long value;

		private bool notNull;

		public static readonly SqlInt64 MaxValue = new SqlInt64(long.MaxValue);

		public static readonly SqlInt64 MinValue = new SqlInt64(long.MinValue);

		public static readonly SqlInt64 Null;

		public static readonly SqlInt64 Zero = new SqlInt64(0L);

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public long Value
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException();
				}
				return value;
			}
		}

		public SqlInt64(long value)
		{
			this.value = value;
			notNull = true;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				return;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return;
			}
			reader.MoveToContent();
			if (reader.EOF)
			{
				return;
			}
			reader.Read();
			if (reader.NodeType != XmlNodeType.EndElement && reader.Value.Length > 0)
			{
				if (string.Compare("Null", reader.Value) == 0)
				{
					notNull = false;
					return;
				}
				value = long.Parse(reader.Value);
				notNull = true;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString());
		}

		public static SqlInt64 Add(SqlInt64 x, SqlInt64 y)
		{
			return x + y;
		}

		public static SqlInt64 BitwiseAnd(SqlInt64 x, SqlInt64 y)
		{
			return x & y;
		}

		public static SqlInt64 BitwiseOr(SqlInt64 x, SqlInt64 y)
		{
			return x | y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlInt64))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlInt64"));
			}
			return CompareSqlInt64((SqlInt64)value);
		}

		public int CompareTo(SqlInt64 value)
		{
			return CompareSqlInt64(value);
		}

		private int CompareSqlInt64(SqlInt64 value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public static SqlInt64 Divide(SqlInt64 x, SqlInt64 y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlInt64))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlInt64)value).IsNull;
			}
			if (((SqlInt64)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlInt64)value);
		}

		public static SqlBoolean Equals(SqlInt64 x, SqlInt64 y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			return (int)(value & 0xFFFFFFFFu) ^ (int)(value >> 32);
		}

		public static SqlBoolean GreaterThan(SqlInt64 x, SqlInt64 y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlInt64 x, SqlInt64 y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlInt64 x, SqlInt64 y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlInt64 x, SqlInt64 y)
		{
			return x <= y;
		}

		public static SqlInt64 Mod(SqlInt64 x, SqlInt64 y)
		{
			return x % y;
		}

		public static SqlInt64 Modulus(SqlInt64 x, SqlInt64 y)
		{
			return x % y;
		}

		public static SqlInt64 Multiply(SqlInt64 x, SqlInt64 y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlInt64 x, SqlInt64 y)
		{
			return x != y;
		}

		public static SqlInt64 OnesComplement(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return ~x;
		}

		public static SqlInt64 Parse(string s)
		{
			return new SqlInt64(long.Parse(s));
		}

		public static SqlInt64 Subtract(SqlInt64 x, SqlInt64 y)
		{
			return x - y;
		}

		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		public SqlDecimal ToSqlDecimal()
		{
			return this;
		}

		public SqlDouble ToSqlDouble()
		{
			return this;
		}

		public SqlInt16 ToSqlInt16()
		{
			return (SqlInt16)this;
		}

		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		public SqlMoney ToSqlMoney()
		{
			return this;
		}

		public SqlSingle ToSqlSingle()
		{
			return this;
		}

		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		public override string ToString()
		{
			if (IsNull)
			{
				return "Null";
			}
			return value.ToString();
		}

		public static SqlInt64 Xor(SqlInt64 x, SqlInt64 y)
		{
			return x ^ y;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "long";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("long", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlInt64 operator +(SqlInt64 x, SqlInt64 y)
		{
			return new SqlInt64(checked(x.Value + y.Value));
		}

		public static SqlInt64 operator &(SqlInt64 x, SqlInt64 y)
		{
			return new SqlInt64(x.value & y.Value);
		}

		public static SqlInt64 operator |(SqlInt64 x, SqlInt64 y)
		{
			return new SqlInt64(x.value | y.Value);
		}

		public static SqlInt64 operator /(SqlInt64 x, SqlInt64 y)
		{
			return new SqlInt64(x.Value / y.Value);
		}

		public static SqlBoolean operator ==(SqlInt64 x, SqlInt64 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlInt64 operator ^(SqlInt64 x, SqlInt64 y)
		{
			return new SqlInt64(x.Value ^ y.Value);
		}

		public static SqlBoolean operator >(SqlInt64 x, SqlInt64 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlInt64 x, SqlInt64 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlInt64 x, SqlInt64 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value != y.Value);
		}

		public static SqlBoolean operator <(SqlInt64 x, SqlInt64 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlInt64 x, SqlInt64 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlInt64 operator %(SqlInt64 x, SqlInt64 y)
		{
			return new SqlInt64(x.Value % y.Value);
		}

		public static SqlInt64 operator *(SqlInt64 x, SqlInt64 y)
		{
			return new SqlInt64(checked(x.Value * y.Value));
		}

		public static SqlInt64 operator ~(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64(~x.Value);
		}

		public static SqlInt64 operator -(SqlInt64 x, SqlInt64 y)
		{
			return new SqlInt64(checked(x.Value - y.Value));
		}

		public static SqlInt64 operator -(SqlInt64 x)
		{
			return new SqlInt64(-x.Value);
		}

		public static explicit operator SqlInt64(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64((int)x.ByteValue);
		}

		public static explicit operator SqlInt64(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64((long)x.Value);
		}

		public static explicit operator SqlInt64(SqlDouble x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64(checked((long)x.Value));
		}

		public static explicit operator long(SqlInt64 x)
		{
			return x.Value;
		}

		public static explicit operator SqlInt64(SqlMoney x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64((long)Math.Round(x.Value));
		}

		public static explicit operator SqlInt64(SqlSingle x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64(checked((long)x.Value));
		}

		public static explicit operator SqlInt64(SqlString x)
		{
			return Parse(x.Value);
		}

		public static implicit operator SqlInt64(long x)
		{
			return new SqlInt64(x);
		}

		public static implicit operator SqlInt64(SqlByte x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64((int)x.Value);
		}

		public static implicit operator SqlInt64(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64(x.Value);
		}

		public static implicit operator SqlInt64(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt64(x.Value);
		}
	}
}
