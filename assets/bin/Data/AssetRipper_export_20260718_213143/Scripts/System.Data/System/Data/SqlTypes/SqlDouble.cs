using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlDouble : IComparable, INullable, IXmlSerializable
	{
		private double value;

		private bool notNull;

		public static readonly SqlDouble MaxValue = new SqlDouble(double.MaxValue);

		public static readonly SqlDouble MinValue = new SqlDouble(double.MinValue);

		public static readonly SqlDouble Null;

		public static readonly SqlDouble Zero = new SqlDouble(0.0);

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public double Value
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

		public SqlDouble(double value)
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
				value = double.Parse(reader.Value);
				notNull = true;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString());
		}

		public static SqlDouble Add(SqlDouble x, SqlDouble y)
		{
			return x + y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlDouble))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlDouble"));
			}
			return CompareTo((SqlDouble)value);
		}

		public int CompareTo(SqlDouble value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public static SqlDouble Divide(SqlDouble x, SqlDouble y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlDouble))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlDouble)value).IsNull;
			}
			if (((SqlDouble)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlDouble)value);
		}

		public static SqlBoolean Equals(SqlDouble x, SqlDouble y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			long num = (long)value;
			return (int)(num ^ (num >> 32));
		}

		public static SqlBoolean GreaterThan(SqlDouble x, SqlDouble y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlDouble x, SqlDouble y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlDouble x, SqlDouble y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlDouble x, SqlDouble y)
		{
			return x <= y;
		}

		public static SqlDouble Multiply(SqlDouble x, SqlDouble y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlDouble x, SqlDouble y)
		{
			return x != y;
		}

		public static SqlDouble Parse(string s)
		{
			return new SqlDouble(double.Parse(s));
		}

		public static SqlDouble Subtract(SqlDouble x, SqlDouble y)
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
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "double";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("double", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlDouble operator +(SqlDouble x, SqlDouble y)
		{
			double num = 0.0;
			num = x.Value + y.Value;
			if (double.IsInfinity(num))
			{
				throw new OverflowException();
			}
			return new SqlDouble(num);
		}

		public static SqlDouble operator /(SqlDouble x, SqlDouble y)
		{
			double d = x.Value / y.Value;
			if (double.IsInfinity(d) && y.Value == 0.0)
			{
				throw new DivideByZeroException();
			}
			return new SqlDouble(d);
		}

		public static SqlBoolean operator ==(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator >(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value != y.Value);
		}

		public static SqlBoolean operator <(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlDouble operator *(SqlDouble x, SqlDouble y)
		{
			double d = x.Value * y.Value;
			if (double.IsInfinity(d))
			{
				throw new OverflowException();
			}
			return new SqlDouble(d);
		}

		public static SqlDouble operator -(SqlDouble x, SqlDouble y)
		{
			double d = x.Value - y.Value;
			if (double.IsInfinity(d))
			{
				throw new OverflowException();
			}
			return new SqlDouble(d);
		}

		public static SqlDouble operator -(SqlDouble x)
		{
			return new SqlDouble(0.0 - x.Value);
		}

		public static explicit operator SqlDouble(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDouble((int)x.ByteValue);
		}

		public static explicit operator double(SqlDouble x)
		{
			return x.Value;
		}

		public static explicit operator SqlDouble(SqlString x)
		{
			return Parse(x.Value);
		}

		public static implicit operator SqlDouble(double x)
		{
			return new SqlDouble(x);
		}

		public static implicit operator SqlDouble(SqlByte x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDouble((int)x.Value);
		}

		public static implicit operator SqlDouble(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDouble(x.ToDouble());
		}

		public static implicit operator SqlDouble(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDouble(x.Value);
		}

		public static implicit operator SqlDouble(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDouble(x.Value);
		}

		public static implicit operator SqlDouble(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDouble(x.Value);
		}

		public static implicit operator SqlDouble(SqlMoney x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDouble((double)x.Value);
		}

		public static implicit operator SqlDouble(SqlSingle x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDouble(x.Value);
		}
	}
}
