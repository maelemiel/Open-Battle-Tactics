using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlGuid : IComparable, INullable, IXmlSerializable
	{
		private Guid value;

		private bool notNull;

		public static readonly SqlGuid Null;

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public Guid Value
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

		public SqlGuid(byte[] value)
		{
			this.value = new Guid(value);
			notNull = true;
		}

		public SqlGuid(Guid g)
		{
			value = g;
			notNull = true;
		}

		public SqlGuid(string s)
		{
			value = new Guid(s);
			notNull = true;
		}

		public SqlGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			value = new Guid(a, b, c, d, e, f, g, h, i, j, k);
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

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlGuid))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlGuid"));
			}
			return CompareTo((SqlGuid)value);
		}

		public int CompareTo(SqlGuid value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlGuid))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlGuid)value).IsNull;
			}
			if (((SqlGuid)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlGuid)value);
		}

		public static SqlBoolean Equals(SqlGuid x, SqlGuid y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			byte[] array = ToByteArray();
			int num = 10;
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				num = 91 * num + b.GetHashCode();
			}
			return num;
		}

		public static SqlBoolean GreaterThan(SqlGuid x, SqlGuid y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlGuid x, SqlGuid y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlGuid x, SqlGuid y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlGuid x, SqlGuid y)
		{
			return x <= y;
		}

		public static SqlBoolean NotEquals(SqlGuid x, SqlGuid y)
		{
			return x != y;
		}

		public static SqlGuid Parse(string s)
		{
			return new SqlGuid(s);
		}

		public byte[] ToByteArray()
		{
			return value.ToByteArray();
		}

		public SqlBinary ToSqlBinary()
		{
			return (SqlBinary)this;
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
			return new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlBoolean operator ==(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator >(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.Value.CompareTo(y.Value) > 0)
			{
				return new SqlBoolean(true);
			}
			return new SqlBoolean(false);
		}

		public static SqlBoolean operator >=(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.Value.CompareTo(y.Value) >= 0)
			{
				return new SqlBoolean(true);
			}
			return new SqlBoolean(false);
		}

		public static SqlBoolean operator !=(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(!(x.Value == y.Value));
		}

		public static SqlBoolean operator <(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.Value.CompareTo(y.Value) < 0)
			{
				return new SqlBoolean(true);
			}
			return new SqlBoolean(false);
		}

		public static SqlBoolean operator <=(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.Value.CompareTo(y.Value) <= 0)
			{
				return new SqlBoolean(true);
			}
			return new SqlBoolean(false);
		}

		public static explicit operator SqlGuid(SqlBinary x)
		{
			return new SqlGuid(x.Value);
		}

		public static explicit operator Guid(SqlGuid x)
		{
			return x.Value;
		}

		public static explicit operator SqlGuid(SqlString x)
		{
			return new SqlGuid(x.Value);
		}

		public static implicit operator SqlGuid(Guid x)
		{
			return new SqlGuid(x);
		}
	}
}
