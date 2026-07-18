using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlSingle : IComparable, INullable, IXmlSerializable
	{
		private float value;

		private bool notNull;

		public static readonly SqlSingle MaxValue = new SqlSingle(float.MaxValue);

		public static readonly SqlSingle MinValue = new SqlSingle(float.MinValue);

		public static readonly SqlSingle Null;

		public static readonly SqlSingle Zero = new SqlSingle(0f);

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public float Value
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

		public SqlSingle(double value)
		{
			this.value = (float)value;
			notNull = true;
		}

		public SqlSingle(float value)
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

		public static SqlSingle Add(SqlSingle x, SqlSingle y)
		{
			return x + y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlSingle))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlSingle"));
			}
			return CompareSqlSingle((SqlSingle)value);
		}

		public int CompareTo(SqlSingle value)
		{
			return CompareSqlSingle(value);
		}

		private int CompareSqlSingle(SqlSingle value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public static SqlSingle Divide(SqlSingle x, SqlSingle y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlSingle))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlSingle)value).IsNull;
			}
			if (((SqlSingle)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlSingle)value);
		}

		public static SqlBoolean Equals(SqlSingle x, SqlSingle y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			long num = (long)value;
			return (int)(num ^ (num >> 32));
		}

		public static SqlBoolean GreaterThan(SqlSingle x, SqlSingle y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlSingle x, SqlSingle y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlSingle x, SqlSingle y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlSingle x, SqlSingle y)
		{
			return x <= y;
		}

		public static SqlSingle Multiply(SqlSingle x, SqlSingle y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlSingle x, SqlSingle y)
		{
			return x != y;
		}

		public static SqlSingle Parse(string s)
		{
			return new SqlSingle(float.Parse(s));
		}

		public static SqlSingle Subtract(SqlSingle x, SqlSingle y)
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
			return (SqlDecimal)this;
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

		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
		}

		public SqlMoney ToSqlMoney()
		{
			return (SqlMoney)this;
		}

		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		public override string ToString()
		{
			if (!notNull)
			{
				return "Null";
			}
			return value.ToString();
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("float", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlSingle operator +(SqlSingle x, SqlSingle y)
		{
			float f = x.Value + y.Value;
			if (float.IsInfinity(f))
			{
				throw new OverflowException();
			}
			return new SqlSingle(f);
		}

		public static SqlSingle operator /(SqlSingle x, SqlSingle y)
		{
			float f = x.Value / y.Value;
			if (float.IsInfinity(f) && (double)y.Value == 0.0)
			{
				throw new DivideByZeroException();
			}
			return new SqlSingle(x.Value / y.Value);
		}

		public static SqlBoolean operator ==(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator >(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value != y.Value);
		}

		public static SqlBoolean operator <(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlSingle operator *(SqlSingle x, SqlSingle y)
		{
			float f = x.Value * y.Value;
			if (float.IsInfinity(f))
			{
				throw new OverflowException();
			}
			return new SqlSingle(f);
		}

		public static SqlSingle operator -(SqlSingle x, SqlSingle y)
		{
			float f = x.Value - y.Value;
			if (float.IsInfinity(f))
			{
				throw new OverflowException();
			}
			return new SqlSingle(f);
		}

		public static SqlSingle operator -(SqlSingle x)
		{
			return new SqlSingle(0f - x.Value);
		}

		public static explicit operator SqlSingle(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlSingle((int)x.ByteValue);
		}

		public static explicit operator SqlSingle(SqlDouble x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			float f = (float)x.Value;
			if (float.IsInfinity(f))
			{
				throw new OverflowException();
			}
			return new SqlSingle(f);
		}

		public static explicit operator float(SqlSingle x)
		{
			return x.Value;
		}

		public static explicit operator SqlSingle(SqlString x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return Parse(x.Value);
		}

		public static implicit operator SqlSingle(float x)
		{
			return new SqlSingle(x);
		}

		public static implicit operator SqlSingle(SqlByte x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlSingle((int)x.Value);
		}

		public static implicit operator SqlSingle(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlSingle((float)x.Value);
		}

		public static implicit operator SqlSingle(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlSingle(x.Value);
		}

		public static implicit operator SqlSingle(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlSingle(x.Value);
		}

		public static implicit operator SqlSingle(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlSingle(x.Value);
		}

		public static implicit operator SqlSingle(SqlMoney x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlSingle((float)x.Value);
		}
	}
}
