using System;
using System.Data;
using System.Data.SqlTypes;
using System.Threading;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlMetaData
	{
		private SqlCompareOptions compareOptions;

		private string databaseName;

		private long localeId;

		private long maxLength;

		private string name;

		private byte precision = 10;

		private byte scale;

		private string owningSchema;

		private string objectName;

		private SqlDbType sqlDbType = SqlDbType.NVarChar;

		private DbType dbType = DbType.String;

		private Type type = typeof(string);

		public SqlCompareOptions CompareOptions
		{
			get
			{
				return compareOptions;
			}
		}

		public DbType DbType
		{
			get
			{
				return dbType;
			}
		}

		public long LocaleId
		{
			get
			{
				return localeId;
			}
		}

		public static long Max
		{
			get
			{
				return -1L;
			}
		}

		public long MaxLength
		{
			get
			{
				return maxLength;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public byte Precision
		{
			get
			{
				return precision;
			}
		}

		public byte Scale
		{
			get
			{
				return scale;
			}
		}

		public SqlDbType SqlDbType
		{
			get
			{
				return sqlDbType;
			}
		}

		public string XmlSchemaCollectionDatabase
		{
			get
			{
				return databaseName;
			}
		}

		public string XmlSchemaCollectionName
		{
			get
			{
				return objectName;
			}
		}

		public string XmlSchemaCollectionOwningSchema
		{
			get
			{
				return owningSchema;
			}
		}

		[System.MonoTODO]
		public string TypeName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public SqlMetaData(string name, SqlDbType sqlDbType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			switch (sqlDbType)
			{
			case SqlDbType.Bit:
				maxLength = 1L;
				precision = 1;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Boolean;
				type = typeof(bool);
				break;
			case SqlDbType.BigInt:
				maxLength = 8L;
				precision = 19;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Int64;
				type = typeof(long);
				break;
			case SqlDbType.DateTime:
				maxLength = 8L;
				precision = 23;
				scale = 3;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.DateTime;
				type = typeof(DateTime);
				break;
			case SqlDbType.Decimal:
				maxLength = 9L;
				precision = 18;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Decimal;
				type = typeof(decimal);
				break;
			case SqlDbType.Float:
				maxLength = 8L;
				precision = 53;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Double;
				type = typeof(float);
				break;
			case SqlDbType.Int:
				maxLength = 4L;
				precision = 10;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Int32;
				type = typeof(int);
				break;
			case SqlDbType.Money:
				maxLength = 8L;
				precision = 19;
				scale = 4;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Currency;
				type = typeof(double);
				break;
			case SqlDbType.SmallDateTime:
				maxLength = 4L;
				precision = 16;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.DateTime;
				type = typeof(DateTime);
				break;
			case SqlDbType.SmallInt:
				maxLength = 2L;
				precision = 5;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Int16;
				type = typeof(short);
				break;
			case SqlDbType.SmallMoney:
				maxLength = 4L;
				precision = 10;
				scale = 4;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Currency;
				type = typeof(double);
				break;
			case SqlDbType.Timestamp:
				maxLength = 8L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.DateTime;
				type = typeof(DateTime);
				break;
			case SqlDbType.TinyInt:
				maxLength = 1L;
				precision = 3;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Int16;
				type = typeof(short);
				break;
			case SqlDbType.UniqueIdentifier:
				maxLength = 16L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Guid;
				type = typeof(Guid);
				break;
			case SqlDbType.Xml:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.Xml;
				type = typeof(string);
				break;
			default:
				throw new ArgumentException("SqlDbType not supported");
			}
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, long maxLength)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			switch (sqlDbType)
			{
			case SqlDbType.Binary:
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.Binary;
				type = typeof(byte[]);
				break;
			case SqlDbType.Char:
				localeId = Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.AnsiStringFixedLength;
				type = typeof(string);
				break;
			case SqlDbType.Image:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Binary;
				type = typeof(byte[]);
				break;
			case SqlDbType.NChar:
				localeId = Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.String;
				type = typeof(string);
				break;
			case SqlDbType.NText:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.String;
				type = typeof(string);
				break;
			case SqlDbType.NVarChar:
				maxLength = -1L;
				localeId = Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.String;
				type = typeof(string);
				break;
			case SqlDbType.Text:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.String;
				type = typeof(char[]);
				break;
			case SqlDbType.VarBinary:
				maxLength = -1L;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.Binary;
				type = typeof(byte[]);
				break;
			case SqlDbType.VarChar:
				maxLength = -1L;
				localeId = Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.String;
				type = typeof(char[]);
				break;
			default:
				throw new ArgumentException("SqlDbType not supported");
			}
			this.maxLength = maxLength;
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		[System.MonoTODO]
		public SqlMetaData(string name, SqlDbType sqlDbType, Type userDefinedType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			if (sqlDbType == SqlDbType.Udt)
			{
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Guid;
				type = typeof(Guid);
				this.name = name;
				throw new NotImplementedException();
			}
			throw new ArgumentException("SqlDbType not supported");
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, byte precision, byte scale)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			if (sqlDbType == SqlDbType.Decimal)
			{
				maxLength = 9L;
				this.precision = precision;
				this.scale = scale;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Decimal;
				type = typeof(decimal);
				this.name = name;
				this.sqlDbType = sqlDbType;
				return;
			}
			throw new ArgumentException("SqlDbType not supported");
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, long maxLength, long locale, SqlCompareOptions compareOptions)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			switch (sqlDbType)
			{
			case SqlDbType.Char:
				dbType = DbType.AnsiStringFixedLength;
				type = typeof(char[]);
				break;
			case SqlDbType.NChar:
				dbType = DbType.StringFixedLength;
				type = typeof(char[]);
				break;
			case SqlDbType.NText:
			case SqlDbType.NVarChar:
				dbType = DbType.String;
				type = typeof(string);
				break;
			case SqlDbType.Text:
			case SqlDbType.VarChar:
				dbType = DbType.AnsiString;
				type = typeof(char[]);
				break;
			default:
				throw new ArgumentException("SqlDbType not supported");
			}
			this.compareOptions = compareOptions;
			localeId = locale;
			this.maxLength = maxLength;
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, string database, string owningSchema, string objectName)
		{
			if ((name == null || objectName == null) && database != null && owningSchema != null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			if (sqlDbType == SqlDbType.Xml)
			{
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.String;
				type = typeof(string);
				this.name = name;
				this.sqlDbType = sqlDbType;
				databaseName = database;
				this.owningSchema = owningSchema;
				this.objectName = objectName;
				return;
			}
			throw new ArgumentException("SqlDbType not supported");
		}

		public SqlMetaData(string name, SqlDbType sqlDbType, long maxLength, byte precision, byte scale, long localeId, SqlCompareOptions compareOptions, Type userDefinedType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			this.compareOptions = compareOptions;
			this.localeId = localeId;
			this.maxLength = maxLength;
			this.precision = precision;
			this.scale = scale;
			switch (sqlDbType)
			{
			case SqlDbType.Bit:
				maxLength = 1L;
				precision = 1;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Boolean;
				type = typeof(bool);
				break;
			case SqlDbType.BigInt:
				maxLength = 8L;
				precision = 19;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Int64;
				type = typeof(long);
				break;
			case SqlDbType.DateTime:
				maxLength = 8L;
				precision = 23;
				scale = 3;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.DateTime;
				type = typeof(DateTime);
				break;
			case SqlDbType.Decimal:
				maxLength = 9L;
				precision = 18;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Decimal;
				type = typeof(decimal);
				break;
			case SqlDbType.Float:
				maxLength = 8L;
				precision = 53;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Decimal;
				type = typeof(float);
				break;
			case SqlDbType.Image:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Binary;
				type = typeof(byte[]);
				break;
			case SqlDbType.Int:
				maxLength = 4L;
				precision = 10;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Int32;
				type = typeof(int);
				break;
			case SqlDbType.Money:
				maxLength = 8L;
				precision = 19;
				scale = 4;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Currency;
				type = typeof(decimal);
				break;
			case SqlDbType.NText:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.String;
				type = typeof(string);
				break;
			case SqlDbType.Real:
				maxLength = 4L;
				precision = 24;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Single;
				type = typeof(float);
				break;
			case SqlDbType.SmallDateTime:
				maxLength = 4L;
				precision = 16;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.DateTime;
				type = typeof(DateTime);
				break;
			case SqlDbType.SmallInt:
				maxLength = 2L;
				precision = 5;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Int16;
				type = typeof(short);
				break;
			case SqlDbType.SmallMoney:
				maxLength = 4L;
				precision = 10;
				scale = 4;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Currency;
				type = typeof(decimal);
				break;
			case SqlDbType.Text:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = Thread.CurrentThread.CurrentCulture.LCID;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.AnsiString;
				type = typeof(char[]);
				break;
			case SqlDbType.Timestamp:
				maxLength = 8L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Byte;
				type = typeof(byte[]);
				break;
			case SqlDbType.TinyInt:
				maxLength = 1L;
				precision = 3;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Int16;
				type = typeof(short);
				break;
			case SqlDbType.UniqueIdentifier:
				maxLength = 16L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Guid;
				type = typeof(Guid);
				break;
			case SqlDbType.Udt:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Object;
				type = typeof(object);
				break;
			case SqlDbType.Variant:
				maxLength = 8016L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.None;
				dbType = DbType.Object;
				type = typeof(object);
				break;
			case SqlDbType.Xml:
				maxLength = -1L;
				precision = 0;
				scale = 0;
				localeId = 0L;
				compareOptions = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;
				dbType = DbType.Xml;
				type = typeof(string);
				break;
			default:
				throw new ArgumentException("SqlDbType not supported");
			}
			this.name = name;
			this.sqlDbType = sqlDbType;
		}

		public bool Adjust(bool value)
		{
			if (type != typeof(bool))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public byte Adjust(byte value)
		{
			if (type != typeof(byte))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public byte[] Adjust(byte[] value)
		{
			if (type != typeof(byte[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public char Adjust(char value)
		{
			if (type != typeof(char))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public char[] Adjust(char[] value)
		{
			if (type != typeof(char[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public DateTime Adjust(DateTime value)
		{
			if (type != typeof(DateTime))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public decimal Adjust(decimal value)
		{
			if (type != typeof(decimal))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public double Adjust(double value)
		{
			if (type != typeof(double))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public Guid Adjust(Guid value)
		{
			if (type != typeof(Guid))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public short Adjust(short value)
		{
			if (type != typeof(short))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public int Adjust(int value)
		{
			if (type != typeof(int))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public long Adjust(long value)
		{
			if (type != typeof(long))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public object Adjust(object value)
		{
			if (type != typeof(object))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public float Adjust(float value)
		{
			if (type != typeof(float))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlBinary Adjust(SqlBinary value)
		{
			if (type != typeof(byte[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlBoolean Adjust(SqlBoolean value)
		{
			if (type != typeof(bool))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlByte Adjust(SqlByte value)
		{
			if (type != typeof(byte))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlBytes Adjust(SqlBytes value)
		{
			if (type != typeof(byte[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlChars Adjust(SqlChars value)
		{
			if (type != typeof(char[]))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlDateTime Adjust(SqlDateTime value)
		{
			if (type != typeof(DateTime))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlDecimal Adjust(SqlDecimal value)
		{
			if (type != typeof(decimal))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlDouble Adjust(SqlDouble value)
		{
			if (type != typeof(double))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlGuid Adjust(SqlGuid value)
		{
			if (type != typeof(Guid))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlInt16 Adjust(SqlInt16 value)
		{
			if (type != typeof(short))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlInt32 Adjust(SqlInt32 value)
		{
			if (type != typeof(int))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlInt64 Adjust(SqlInt64 value)
		{
			if (type != typeof(long))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlMoney Adjust(SqlMoney value)
		{
			if (type != typeof(decimal))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlSingle Adjust(SqlSingle value)
		{
			if (type != typeof(float))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public SqlString Adjust(SqlString value)
		{
			if (type != typeof(string))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public string Adjust(string value)
		{
			if (type != typeof(string))
			{
				throw new ArgumentException("Value does not match the SqlMetaData type");
			}
			return value;
		}

		public static SqlMetaData InferFromValue(object value, string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name can not be null");
			}
			if (value == null)
			{
				throw new ArgumentException("value can not be null");
			}
			SqlMetaData sqlMetaData = null;
			switch (value.GetType().ToString())
			{
			case "System.Boolean":
				return new SqlMetaData(name, SqlDbType.Bit);
			case "System.Byte":
				return new SqlMetaData(name, SqlDbType.Binary);
			case "System.Byte[]":
				return new SqlMetaData(name, SqlDbType.VarBinary);
			case "System.Char":
				return new SqlMetaData(name, SqlDbType.Char);
			case "System.Char[]":
				return new SqlMetaData(name, SqlDbType.VarChar);
			case "System.DateTime":
				return new SqlMetaData(name, SqlDbType.DateTime);
			case "System.Decimal":
				return new SqlMetaData(name, SqlDbType.Decimal);
			case "System.Double":
				return new SqlMetaData(name, SqlDbType.Float);
			case "System.Guid":
				return new SqlMetaData(name, SqlDbType.UniqueIdentifier);
			case "System.Int16":
				return new SqlMetaData(name, SqlDbType.SmallInt);
			case "System.Int32":
				return new SqlMetaData(name, SqlDbType.Int);
			case "System.Int64":
				return new SqlMetaData(name, SqlDbType.BigInt);
			case "System.Single":
				return new SqlMetaData(name, SqlDbType.Real);
			case "System.String":
				return new SqlMetaData(name, SqlDbType.NVarChar);
			default:
				return new SqlMetaData(name, SqlDbType.Variant);
			}
		}
	}
}
