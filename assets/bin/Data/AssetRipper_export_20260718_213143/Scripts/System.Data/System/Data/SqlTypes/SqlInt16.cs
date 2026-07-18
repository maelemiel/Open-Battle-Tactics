using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlInt16 : IComparable, INullable, IXmlSerializable
	{
		private short value;

		private bool notNull;

		public static readonly SqlInt16 MaxValue = new SqlInt16(short.MaxValue);

		public static readonly SqlInt16 MinValue = new SqlInt16(short.MinValue);

		public static readonly SqlInt16 Null;

		public static readonly SqlInt16 Zero = new SqlInt16(0);

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public short Value
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

		public SqlInt16(short value)
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
				value = short.Parse(reader.Value);
				notNull = true;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString());
		}

		public static SqlInt16 Add(SqlInt16 x, SqlInt16 y)
		{
			return x + y;
		}

		public static SqlInt16 BitwiseAnd(SqlInt16 x, SqlInt16 y)
		{
			return x & y;
		}

		public static SqlInt16 BitwiseOr(SqlInt16 x, SqlInt16 y)
		{
			return x | y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlInt16))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlInt16"));
			}
			return CompareSqlInt16((SqlInt16)value);
		}

		public int CompareTo(SqlInt16 value)
		{
			return CompareSqlInt16(value);
		}

		private int CompareSqlInt16(SqlInt16 value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public static SqlInt16 Divide(SqlInt16 x, SqlInt16 y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlInt16))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlInt16)value).IsNull;
			}
			if (((SqlInt16)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlInt16)value);
		}

		public static SqlBoolean Equals(SqlInt16 x, SqlInt16 y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			return value;
		}

		public static SqlBoolean GreaterThan(SqlInt16 x, SqlInt16 y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlInt16 x, SqlInt16 y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlInt16 x, SqlInt16 y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlInt16 x, SqlInt16 y)
		{
			return x <= y;
		}

		public static SqlInt16 Mod(SqlInt16 x, SqlInt16 y)
		{
			return x % y;
		}

		public static SqlInt16 Modulus(SqlInt16 x, SqlInt16 y)
		{
			return x % y;
		}

		public static SqlInt16 Multiply(SqlInt16 x, SqlInt16 y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlInt16 x, SqlInt16 y)
		{
			return x != y;
		}

		public static SqlInt16 OnesComplement(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return ~x;
		}

		public static SqlInt16 Parse(string s)
		{
			return new SqlInt16(short.Parse(s));
		}

		public static SqlInt16 Subtract(SqlInt16 x, SqlInt16 y)
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

		public SqlInt32 ToSqlInt32()
		{
			return this;
		}

		public SqlInt64 ToSqlInt64()
		{
			return this;
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

		public static SqlInt16 Xor(SqlInt16 x, SqlInt16 y)
		{
			return x ^ y;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "short";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("short", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlInt16 operator +(SqlInt16 x, SqlInt16 y)
		{
			return new SqlInt16(checked((short)(x.Value + y.Value)));
		}

		public static SqlInt16 operator &(SqlInt16 x, SqlInt16 y)
		{
			return new SqlInt16((short)(x.value & y.Value));
		}

		public static SqlInt16 operator |(SqlInt16 x, SqlInt16 y)
		{
			return new SqlInt16((short)(x.Value | y.Value));
		}

		public static SqlInt16 operator /(SqlInt16 x, SqlInt16 y)
		{
			checked
			{
				return new SqlInt16((short)unchecked(x.Value / y.Value));
			}
		}

		public static SqlBoolean operator ==(SqlInt16 x, SqlInt16 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlInt16 operator ^(SqlInt16 x, SqlInt16 y)
		{
			return new SqlInt16((short)(x.Value ^ y.Value));
		}

		public static SqlBoolean operator >(SqlInt16 x, SqlInt16 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlInt16 x, SqlInt16 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlInt16 x, SqlInt16 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value != y.Value);
		}

		public static SqlBoolean operator <(SqlInt16 x, SqlInt16 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlInt16 x, SqlInt16 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlInt16 operator %(SqlInt16 x, SqlInt16 y)
		{
			return new SqlInt16((short)(x.Value % y.Value));
		}

		public static SqlInt16 operator *(SqlInt16 x, SqlInt16 y)
		{
			return new SqlInt16(checked((short)(x.Value * y.Value)));
		}

		public static SqlInt16 operator ~(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt16((short)(~x.Value));
		}

		public static SqlInt16 operator -(SqlInt16 x, SqlInt16 y)
		{
			return new SqlInt16(checked((short)(x.Value - y.Value)));
		}

		public static SqlInt16 operator -(SqlInt16 x)
		{
			return new SqlInt16(checked((short)(-x.Value)));
		}

		public static explicit operator SqlInt16(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt16(x.ByteValue);
		}

		public static explicit operator SqlInt16(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt16((short)x.Value);
		}

		public static explicit operator SqlInt16(SqlDouble x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt16(checked((short)x.Value));
		}

		public static explicit operator short(SqlInt16 x)
		{
			return x.Value;
		}

		public static explicit operator SqlInt16(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt16(checked((short)x.Value));
		}

		public static explicit operator SqlInt16(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt16(checked((short)x.Value));
		}

		public static explicit operator SqlInt16(SqlMoney x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt16((short)Math.Round(x.Value));
		}

		public static explicit operator SqlInt16(SqlSingle x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlInt16(checked((short)x.Value));
		}

		public static explicit operator SqlInt16(SqlString x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return Parse(x.Value);
		}

		public static implicit operator SqlInt16(short x)
		{
			return new SqlInt16(x);
		}

		public static implicit operator SqlInt16(SqlByte x)
		{
			return new SqlInt16(x.Value);
		}
	}
}
