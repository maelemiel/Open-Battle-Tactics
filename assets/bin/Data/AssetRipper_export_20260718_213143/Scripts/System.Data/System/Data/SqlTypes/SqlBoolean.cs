using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlBoolean : IComparable, INullable, IXmlSerializable
	{
		private byte value;

		private bool notNull;

		public static readonly SqlBoolean False = new SqlBoolean(false);

		public static readonly SqlBoolean Null;

		public static readonly SqlBoolean One = new SqlBoolean(1);

		public static readonly SqlBoolean True = new SqlBoolean(true);

		public static readonly SqlBoolean Zero = new SqlBoolean(0);

		public byte ByteValue
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException(global::Locale.GetText("The property is set to null."));
				}
				return value;
			}
		}

		public bool IsFalse
		{
			get
			{
				if (IsNull)
				{
					return false;
				}
				return value == 0;
			}
		}

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public bool IsTrue
		{
			get
			{
				if (IsNull)
				{
					return false;
				}
				return value != 0;
			}
		}

		public bool Value
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException(global::Locale.GetText("The property is set to null."));
				}
				return IsTrue;
			}
		}

		public SqlBoolean(bool value)
		{
			this.value = (byte)(value ? 1u : 0u);
			notNull = true;
		}

		public SqlBoolean(int value)
		{
			this.value = (byte)((value != 0) ? 1u : 0u);
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

		public static SqlBoolean And(SqlBoolean x, SqlBoolean y)
		{
			return x & y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlBoolean))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlBoolean"));
			}
			return CompareTo((SqlBoolean)value);
		}

		public int CompareTo(SqlBoolean value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.ByteValue);
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlBoolean))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlBoolean)value).IsNull;
			}
			if (((SqlBoolean)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlBoolean)value);
		}

		public static SqlBoolean Equals(SqlBoolean x, SqlBoolean y)
		{
			return x == y;
		}

		public static SqlBoolean GreaterThan(SqlBoolean x, SqlBoolean y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEquals(SqlBoolean x, SqlBoolean y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlBoolean x, SqlBoolean y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEquals(SqlBoolean x, SqlBoolean y)
		{
			return x <= y;
		}

		public override int GetHashCode()
		{
			if (IsTrue)
			{
				return 1;
			}
			return 0;
		}

		public static SqlBoolean NotEquals(SqlBoolean x, SqlBoolean y)
		{
			return x != y;
		}

		public static SqlBoolean OnesComplement(SqlBoolean x)
		{
			return ~x;
		}

		public static SqlBoolean Or(SqlBoolean x, SqlBoolean y)
		{
			return x | y;
		}

		public static SqlBoolean Parse(string s)
		{
			switch (s)
			{
			case "0":
				return new SqlBoolean(false);
			case "1":
				return new SqlBoolean(true);
			default:
				return new SqlBoolean(bool.Parse(s));
			}
		}

		public SqlByte ToSqlByte()
		{
			return new SqlByte(value);
		}

		public SqlDecimal ToSqlDecimal()
		{
			return (SqlDecimal)this;
		}

		public SqlDouble ToSqlDouble()
		{
			return (SqlDouble)this;
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

		public SqlSingle ToSqlSingle()
		{
			return (SqlSingle)this;
		}

		public SqlString ToSqlString()
		{
			if (IsNull)
			{
				return new SqlString("Null");
			}
			if (IsTrue)
			{
				return new SqlString("True");
			}
			return new SqlString("False");
		}

		public override string ToString()
		{
			if (IsNull)
			{
				return "Null";
			}
			if (IsTrue)
			{
				return "True";
			}
			return "False";
		}

		public static SqlBoolean Xor(SqlBoolean x, SqlBoolean y)
		{
			return x ^ y;
		}

		private static int Compare(SqlBoolean x, SqlBoolean y)
		{
			if (x == y)
			{
				return 0;
			}
			if (x.IsTrue && y.IsFalse)
			{
				return 1;
			}
			if (x.IsFalse && y.IsTrue)
			{
				return -1;
			}
			return 0;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlBoolean operator &(SqlBoolean x, SqlBoolean y)
		{
			return new SqlBoolean(x.Value & y.Value);
		}

		public static SqlBoolean operator |(SqlBoolean x, SqlBoolean y)
		{
			return new SqlBoolean(x.Value | y.Value);
		}

		public static SqlBoolean operator ==(SqlBoolean x, SqlBoolean y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator ^(SqlBoolean x, SqlBoolean y)
		{
			return new SqlBoolean(x.Value ^ y.Value);
		}

		public static bool operator false(SqlBoolean x)
		{
			return x.IsFalse;
		}

		public static SqlBoolean operator !=(SqlBoolean x, SqlBoolean y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(x.Value != y.Value);
		}

		public static SqlBoolean operator !(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(!x.Value);
		}

		public static SqlBoolean operator ~(SqlBoolean x)
		{
			return (!x.IsTrue) ? new SqlBoolean(true) : new SqlBoolean(false);
		}

		public static SqlBoolean operator >(SqlBoolean x, SqlBoolean y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(Compare(x, y) > 0);
		}

		public static SqlBoolean operator >=(SqlBoolean x, SqlBoolean y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(Compare(x, y) >= 0);
		}

		public static SqlBoolean operator <(SqlBoolean x, SqlBoolean y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(Compare(x, y) < 0);
		}

		public static SqlBoolean operator <=(SqlBoolean x, SqlBoolean y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(Compare(x, y) <= 0);
		}

		public static bool operator true(SqlBoolean x)
		{
			return x.IsTrue;
		}

		public static explicit operator bool(SqlBoolean x)
		{
			return x.Value;
		}

		public static explicit operator SqlBoolean(SqlByte x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(x.Value);
		}

		public static explicit operator SqlBoolean(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean((int)x.Value);
		}

		public static explicit operator SqlBoolean(SqlDouble x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean((int)x.Value);
		}

		public static explicit operator SqlBoolean(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(x.Value);
		}

		public static explicit operator SqlBoolean(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(x.Value);
		}

		public static explicit operator SqlBoolean(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean(checked((int)x.Value));
		}

		public static explicit operator SqlBoolean(SqlMoney x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean((int)x.Value);
		}

		public static explicit operator SqlBoolean(SqlSingle x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlBoolean((int)x.Value);
		}

		public static explicit operator SqlBoolean(SqlString x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return Parse(x.Value);
		}

		public static implicit operator SqlBoolean(bool x)
		{
			return new SqlBoolean(x);
		}
	}
}
