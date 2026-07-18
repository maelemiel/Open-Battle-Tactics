using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlByte : IComparable, INullable, IXmlSerializable
	{
		private byte value;

		private bool notNull;

		public static readonly SqlByte MaxValue = new SqlByte(byte.MaxValue);

		public static readonly SqlByte MinValue = new SqlByte(0);

		public static readonly SqlByte Null;

		public static readonly SqlByte Zero = new SqlByte(0);

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public byte Value
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

		public SqlByte(byte value)
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

		public static SqlByte Add(SqlByte x, SqlByte y)
		{
			return x + y;
		}

		public static SqlByte BitwiseAnd(SqlByte x, SqlByte y)
		{
			return x & y;
		}

		public static SqlByte BitwiseOr(SqlByte x, SqlByte y)
		{
			return x | y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlByte))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlByte"));
			}
			return CompareTo((SqlByte)value);
		}

		public int CompareTo(SqlByte value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public static SqlByte Divide(SqlByte x, SqlByte y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlByte))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlByte)value).IsNull;
			}
			if (((SqlByte)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlByte)value);
		}

		public static SqlBoolean Equals(SqlByte x, SqlByte y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			return value;
		}

		public static SqlBoolean GreaterThan(SqlByte x, SqlByte y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlByte x, SqlByte y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlByte x, SqlByte y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlByte x, SqlByte y)
		{
			return x <= y;
		}

		public static SqlByte Mod(SqlByte x, SqlByte y)
		{
			return x % y;
		}

		public static SqlByte Modulus(SqlByte x, SqlByte y)
		{
			return x % y;
		}

		public static SqlByte Multiply(SqlByte x, SqlByte y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlByte x, SqlByte y)
		{
			return x != y;
		}

		public static SqlByte OnesComplement(SqlByte x)
		{
			return ~x;
		}

		public static SqlByte Parse(string s)
		{
			return new SqlByte(byte.Parse(s));
		}

		public static SqlByte Subtract(SqlByte x, SqlByte y)
		{
			return x - y;
		}

		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
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

		public static SqlByte Xor(SqlByte x, SqlByte y)
		{
			return x ^ y;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("unsignedByte", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlByte operator +(SqlByte x, SqlByte y)
		{
			return new SqlByte(checked((byte)(x.Value + y.Value)));
		}

		public static SqlByte operator &(SqlByte x, SqlByte y)
		{
			return new SqlByte((byte)(x.Value & y.Value));
		}

		public static SqlByte operator |(SqlByte x, SqlByte y)
		{
			return new SqlByte((byte)(x.Value | y.Value));
		}

		public static SqlByte operator /(SqlByte x, SqlByte y)
		{
			checked
			{
				return new SqlByte((byte)unchecked(x.Value / y.Value));
			}
		}

		public static SqlBoolean operator ==(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlByte operator ^(SqlByte x, SqlByte y)
		{
			return new SqlByte((byte)(x.Value ^ y.Value));
		}

		public static SqlBoolean operator >(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value != y.Value);
		}

		public static SqlBoolean operator <(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlByte operator %(SqlByte x, SqlByte y)
		{
			return new SqlByte((byte)(x.Value % y.Value));
		}

		public static SqlByte operator *(SqlByte x, SqlByte y)
		{
			return new SqlByte(checked((byte)(x.Value * y.Value)));
		}

		public static SqlByte operator ~(SqlByte x)
		{
			return new SqlByte((byte)(~x.Value));
		}

		public static SqlByte operator -(SqlByte x, SqlByte y)
		{
			return new SqlByte(checked((byte)(x.Value - y.Value)));
		}

		public static explicit operator SqlByte(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlByte(x.ByteValue);
		}

		public static explicit operator byte(SqlByte x)
		{
			return x.Value;
		}

		public static explicit operator SqlByte(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlByte((byte)x.Value);
		}

		public static explicit operator SqlByte(SqlDouble x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlByte(checked((byte)x.Value));
		}

		public static explicit operator SqlByte(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlByte(checked((byte)x.Value));
		}

		public static explicit operator SqlByte(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlByte(checked((byte)x.Value));
		}

		public static explicit operator SqlByte(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlByte(checked((byte)x.Value));
		}

		public static explicit operator SqlByte(SqlMoney x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlByte((byte)x.Value);
		}

		public static explicit operator SqlByte(SqlSingle x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlByte(checked((byte)x.Value));
		}

		public static explicit operator SqlByte(SqlString x)
		{
			return Parse(x.Value);
		}

		public static implicit operator SqlByte(byte x)
		{
			return new SqlByte(x);
		}
	}
}
