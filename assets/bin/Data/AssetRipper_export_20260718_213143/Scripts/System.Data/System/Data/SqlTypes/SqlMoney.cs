using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlMoney : IComparable, INullable, IXmlSerializable
	{
		private decimal value;

		private bool notNull;

		public static readonly SqlMoney MaxValue;

		public static readonly SqlMoney MinValue;

		public static readonly SqlMoney Null;

		public static readonly SqlMoney Zero;

		private static readonly NumberFormatInfo MoneyFormat;

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public decimal Value
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

		public SqlMoney(decimal value)
		{
			if (value > 922337203685477.5807m || value < -922337203685477.5808m)
			{
				throw new OverflowException();
			}
			this.value = decimal.Round(value, 4);
			notNull = true;
		}

		public SqlMoney(double value)
			: this((decimal)value)
		{
		}

		public SqlMoney(int value)
			: this((decimal)value)
		{
		}

		public SqlMoney(long value)
			: this((decimal)value)
		{
		}

		static SqlMoney()
		{
			MaxValue = new SqlMoney(922337203685477.5807m);
			MinValue = new SqlMoney(-922337203685477.5808m);
			Zero = new SqlMoney(0);
			MoneyFormat = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();
			MoneyFormat.NumberDecimalDigits = 4;
			MoneyFormat.NumberGroupSeparator = string.Empty;
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

		public static SqlMoney Add(SqlMoney x, SqlMoney y)
		{
			return x + y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlMoney))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlMoney"));
			}
			return CompareSqlMoney((SqlMoney)value);
		}

		private int CompareSqlMoney(SqlMoney value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public int CompareTo(SqlMoney value)
		{
			return CompareSqlMoney(value);
		}

		public static SqlMoney Divide(SqlMoney x, SqlMoney y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlMoney))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlMoney)value).IsNull;
			}
			if (((SqlMoney)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlMoney)value);
		}

		public static SqlBoolean Equals(SqlMoney x, SqlMoney y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			return (int)value;
		}

		public static SqlBoolean GreaterThan(SqlMoney x, SqlMoney y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlMoney x, SqlMoney y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlMoney x, SqlMoney y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlMoney x, SqlMoney y)
		{
			return x <= y;
		}

		public static SqlMoney Multiply(SqlMoney x, SqlMoney y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlMoney x, SqlMoney y)
		{
			return x != y;
		}

		public static SqlMoney Parse(string s)
		{
			decimal num = decimal.Parse(s);
			if (num > MaxValue.Value || num < MinValue.Value)
			{
				throw new OverflowException();
			}
			return new SqlMoney(num);
		}

		public static SqlMoney Subtract(SqlMoney x, SqlMoney y)
		{
			return x - y;
		}

		public decimal ToDecimal()
		{
			return value;
		}

		public double ToDouble()
		{
			return (double)value;
		}

		public int ToInt32()
		{
			return (int)Math.Round(value);
		}

		public long ToInt64()
		{
			return (long)Math.Round(value);
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

		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
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
			if (!notNull)
			{
				return "Null";
			}
			return value.ToString("N", MoneyFormat);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlMoney operator +(SqlMoney x, SqlMoney y)
		{
			return new SqlMoney(x.Value + y.Value);
		}

		public static SqlMoney operator /(SqlMoney x, SqlMoney y)
		{
			return new SqlMoney(x.Value / y.Value);
		}

		public static SqlBoolean operator ==(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator >(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(!(x.Value == y.Value));
		}

		public static SqlBoolean operator <(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlMoney operator *(SqlMoney x, SqlMoney y)
		{
			return new SqlMoney(x.Value * y.Value);
		}

		public static SqlMoney operator -(SqlMoney x, SqlMoney y)
		{
			return new SqlMoney(x.Value - y.Value);
		}

		public static SqlMoney operator -(SqlMoney x)
		{
			return new SqlMoney(-x.Value);
		}

		public static explicit operator SqlMoney(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlMoney((decimal)x.ByteValue);
		}

		public static explicit operator SqlMoney(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlMoney(x.Value);
		}

		public static explicit operator SqlMoney(SqlDouble x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlMoney((decimal)x.Value);
		}

		public static explicit operator decimal(SqlMoney x)
		{
			return x.Value;
		}

		public static explicit operator SqlMoney(SqlSingle x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlMoney((decimal)x.Value);
		}

		public static explicit operator SqlMoney(SqlString x)
		{
			return Parse(x.Value);
		}

		public static explicit operator SqlMoney(double x)
		{
			return new SqlMoney(x);
		}

		public static implicit operator SqlMoney(long x)
		{
			return new SqlMoney(x);
		}

		public static implicit operator SqlMoney(decimal x)
		{
			return new SqlMoney(x);
		}

		public static implicit operator SqlMoney(SqlByte x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlMoney((decimal)x.Value);
		}

		public static implicit operator SqlMoney(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlMoney((decimal)x.Value);
		}

		public static implicit operator SqlMoney(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlMoney(x.Value);
		}

		public static implicit operator SqlMoney(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlMoney(x.Value);
		}
	}
}
