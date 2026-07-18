using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.Xml;
using Mono.Data.Tds;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	[TypeConverter("System.Data.SqlClient.SqlParameter+SqlParameterConverter, System.Data, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class SqlParameter : DbParameter, ICloneable, IDataParameter, IDbDataParameter
	{
		private TdsMetaParameter metaParameter;

		private SqlParameterCollection container;

		private DbType dbType;

		private ParameterDirection direction = ParameterDirection.Input;

		private bool isTypeSet;

		private int offset;

		private SqlDbType sqlDbType;

		private string sourceColumn;

		private DataRowVersion sourceVersion;

		private SqlCompareOptions compareInfo;

		private int localeId;

		private Type sqlType;

		private bool typeChanged;

		private bool sourceColumnNullMapping;

		private string xmlSchemaCollectionDatabase = string.Empty;

		private string xmlSchemaCollectionOwningSchema = string.Empty;

		private string xmlSchemaCollectionName = string.Empty;

		private static Hashtable type_mapping;

		internal SqlParameterCollection Container
		{
			get
			{
				return container;
			}
			set
			{
				container = value;
			}
		}

		public override DbType DbType
		{
			get
			{
				return dbType;
			}
			set
			{
				SetDbType(value);
				typeChanged = true;
				isTypeSet = true;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public override ParameterDirection Direction
		{
			get
			{
				return direction;
			}
			set
			{
				direction = value;
				switch (direction)
				{
				case ParameterDirection.Output:
					MetaParameter.Direction = TdsParameterDirection.Output;
					break;
				case ParameterDirection.InputOutput:
					MetaParameter.Direction = TdsParameterDirection.InputOutput;
					break;
				case ParameterDirection.ReturnValue:
					MetaParameter.Direction = TdsParameterDirection.ReturnValue;
					break;
				case (ParameterDirection)4:
				case (ParameterDirection)5:
					break;
				}
			}
		}

		internal TdsMetaParameter MetaParameter
		{
			get
			{
				return metaParameter;
			}
		}

		public override bool IsNullable
		{
			get
			{
				return metaParameter.IsNullable;
			}
			set
			{
				metaParameter.IsNullable = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public int Offset
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
			}
		}

		public override string ParameterName
		{
			get
			{
				return metaParameter.ParameterName;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				metaParameter.ParameterName = value;
			}
		}

		[DefaultValue(0)]
		public byte Precision
		{
			get
			{
				return metaParameter.Precision;
			}
			set
			{
				metaParameter.Precision = value;
			}
		}

		[DefaultValue(0)]
		public byte Scale
		{
			get
			{
				return metaParameter.Scale;
			}
			set
			{
				metaParameter.Scale = value;
			}
		}

		public override int Size
		{
			get
			{
				return metaParameter.Size;
			}
			set
			{
				metaParameter.Size = value;
			}
		}

		public override string SourceColumn
		{
			get
			{
				if (sourceColumn == null)
				{
					return string.Empty;
				}
				return sourceColumn;
			}
			set
			{
				sourceColumn = value;
			}
		}

		public override DataRowVersion SourceVersion
		{
			get
			{
				return sourceVersion;
			}
			set
			{
				sourceVersion = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DbProviderSpecificTypeProperty(true)]
		public SqlDbType SqlDbType
		{
			get
			{
				return sqlDbType;
			}
			set
			{
				SetSqlDbType(value);
				typeChanged = true;
				isTypeSet = true;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(StringConverter))]
		public override object Value
		{
			get
			{
				if (sqlType != null)
				{
					return GetSqlValue(metaParameter.RawValue);
				}
				return metaParameter.RawValue;
			}
			set
			{
				if (!isTypeSet)
				{
					InferSqlType(value);
				}
				if (value is INullable)
				{
					sqlType = value.GetType();
					value = SqlTypeToFrameworkType(value);
				}
				metaParameter.RawValue = value;
			}
		}

		[Browsable(false)]
		public SqlCompareOptions CompareInfo
		{
			get
			{
				return compareInfo;
			}
			set
			{
				compareInfo = value;
			}
		}

		[Browsable(false)]
		public int LocaleId
		{
			get
			{
				return localeId;
			}
			set
			{
				localeId = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public object SqlValue
		{
			get
			{
				return GetSqlValue(metaParameter.RawValue);
			}
			set
			{
				Value = value;
			}
		}

		public override bool SourceColumnNullMapping
		{
			get
			{
				return sourceColumnNullMapping;
			}
			set
			{
				sourceColumnNullMapping = value;
			}
		}

		public string XmlSchemaCollectionDatabase
		{
			get
			{
				return xmlSchemaCollectionDatabase;
			}
			set
			{
				xmlSchemaCollectionDatabase = ((value != null) ? value : string.Empty);
			}
		}

		public string XmlSchemaCollectionName
		{
			get
			{
				return xmlSchemaCollectionName;
			}
			set
			{
				xmlSchemaCollectionName = ((value != null) ? value : string.Empty);
			}
		}

		public string XmlSchemaCollectionOwningSchema
		{
			get
			{
				return xmlSchemaCollectionOwningSchema;
			}
			set
			{
				xmlSchemaCollectionOwningSchema = ((value != null) ? value : string.Empty);
			}
		}

		internal override Type SystemType
		{
			get
			{
				return (Type)DbParameter.DbTypeMapping[sqlDbType];
			}
		}

		internal override object FrameworkDbType
		{
			get
			{
				return sqlDbType;
			}
			set
			{
				try
				{
					object obj = DbTypeFromName((string)value);
					SetDbType((DbType)(int)obj);
				}
				catch (ArgumentException)
				{
					object obj = FrameworkDbTypeFromName((string)value);
					SetSqlDbType((SqlDbType)(int)obj);
				}
			}
		}

		public SqlParameter()
			: this(string.Empty, SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Current, null)
		{
			isTypeSet = false;
		}

		public SqlParameter(string parameterName, object value)
		{
			if (parameterName == null)
			{
				parameterName = string.Empty;
			}
			metaParameter = new TdsMetaParameter(parameterName, GetFrameworkValue);
			metaParameter.RawValue = value;
			InferSqlType(value);
			sourceVersion = DataRowVersion.Current;
		}

		public SqlParameter(string parameterName, SqlDbType dbType)
			: this(parameterName, dbType, 0, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, null)
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size)
			: this(parameterName, dbType, size, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, null)
		{
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, string sourceColumn)
			: this(parameterName, dbType, size, ParameterDirection.Input, false, 0, 0, sourceColumn, DataRowVersion.Current, null)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SqlParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
		{
			if (parameterName == null)
			{
				parameterName = string.Empty;
			}
			metaParameter = new TdsMetaParameter(parameterName, size, isNullable, precision, scale, GetFrameworkValue);
			metaParameter.RawValue = value;
			if (dbType != SqlDbType.Variant)
			{
				SqlDbType = dbType;
			}
			Direction = direction;
			SourceColumn = sourceColumn;
			SourceVersion = sourceVersion;
		}

		public SqlParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value, string xmlSchemaCollectionDatabase, string xmlSchemaCollectionOwningSchema, string xmlSchemaCollectionName)
			: this(parameterName, dbType, size, direction, false, precision, scale, sourceColumn, sourceVersion, value)
		{
			XmlSchemaCollectionDatabase = xmlSchemaCollectionDatabase;
			XmlSchemaCollectionOwningSchema = xmlSchemaCollectionOwningSchema;
			XmlSchemaCollectionName = xmlSchemaCollectionName;
			SourceColumnNullMapping = sourceColumnNullMapping;
		}

		internal SqlParameter(object[] dbValues)
			: this(dbValues[3].ToString(), null)
		{
			ParameterName = (string)dbValues[3];
			switch ((short)dbValues[5])
			{
			case 1:
				Direction = ParameterDirection.Input;
				break;
			case 2:
				Direction = ParameterDirection.InputOutput;
				break;
			case 3:
				Direction = ParameterDirection.Output;
				break;
			case 4:
				Direction = ParameterDirection.ReturnValue;
				break;
			default:
				Direction = ParameterDirection.Input;
				break;
			}
			SqlDbType = FrameworkDbTypeFromName((string)dbValues[16]);
			if (MetaParameter.IsVariableSizeType && dbValues[10] != DBNull.Value)
			{
				Size = (int)dbValues[10];
			}
			if (SqlDbType == SqlDbType.Decimal)
			{
				if (dbValues[12] != null && dbValues[12] != DBNull.Value)
				{
					Precision = (byte)(short)dbValues[12];
				}
				if (dbValues[13] != null && dbValues[13] != DBNull.Value)
				{
					Scale = (byte)(short)dbValues[13];
				}
			}
		}

		static SqlParameter()
		{
			if (DbParameter.DbTypeMapping == null)
			{
				DbParameter.DbTypeMapping = new Hashtable();
			}
			DbParameter.DbTypeMapping.Add(SqlDbType.BigInt, typeof(long));
			DbParameter.DbTypeMapping.Add(SqlDbType.Bit, typeof(bool));
			DbParameter.DbTypeMapping.Add(SqlDbType.Char, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.NChar, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.Text, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.NText, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.VarChar, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.NVarChar, typeof(string));
			DbParameter.DbTypeMapping.Add(SqlDbType.SmallDateTime, typeof(DateTime));
			DbParameter.DbTypeMapping.Add(SqlDbType.DateTime, typeof(DateTime));
			DbParameter.DbTypeMapping.Add(SqlDbType.Decimal, typeof(decimal));
			DbParameter.DbTypeMapping.Add(SqlDbType.Float, typeof(double));
			DbParameter.DbTypeMapping.Add(SqlDbType.Binary, typeof(byte[]));
			DbParameter.DbTypeMapping.Add(SqlDbType.Image, typeof(byte[]));
			DbParameter.DbTypeMapping.Add(SqlDbType.Money, typeof(decimal));
			DbParameter.DbTypeMapping.Add(SqlDbType.SmallMoney, typeof(decimal));
			DbParameter.DbTypeMapping.Add(SqlDbType.VarBinary, typeof(byte[]));
			DbParameter.DbTypeMapping.Add(SqlDbType.TinyInt, typeof(byte));
			DbParameter.DbTypeMapping.Add(SqlDbType.Int, typeof(int));
			DbParameter.DbTypeMapping.Add(SqlDbType.Real, typeof(float));
			DbParameter.DbTypeMapping.Add(SqlDbType.SmallInt, typeof(short));
			DbParameter.DbTypeMapping.Add(SqlDbType.UniqueIdentifier, typeof(Guid));
			DbParameter.DbTypeMapping.Add(SqlDbType.Variant, typeof(object));
			DbParameter.DbTypeMapping.Add(SqlDbType.Xml, typeof(string));
			type_mapping = new Hashtable();
			type_mapping.Add(typeof(long), SqlDbType.BigInt);
			type_mapping.Add(typeof(SqlInt64), SqlDbType.BigInt);
			type_mapping.Add(typeof(bool), SqlDbType.Bit);
			type_mapping.Add(typeof(SqlBoolean), SqlDbType.Bit);
			type_mapping.Add(typeof(char), SqlDbType.NVarChar);
			type_mapping.Add(typeof(char[]), SqlDbType.NVarChar);
			type_mapping.Add(typeof(SqlChars), SqlDbType.NVarChar);
			type_mapping.Add(typeof(string), SqlDbType.NVarChar);
			type_mapping.Add(typeof(SqlString), SqlDbType.NVarChar);
			type_mapping.Add(typeof(DateTime), SqlDbType.DateTime);
			type_mapping.Add(typeof(SqlDateTime), SqlDbType.DateTime);
			type_mapping.Add(typeof(decimal), SqlDbType.Decimal);
			type_mapping.Add(typeof(SqlDecimal), SqlDbType.Decimal);
			type_mapping.Add(typeof(double), SqlDbType.Float);
			type_mapping.Add(typeof(SqlDouble), SqlDbType.Float);
			type_mapping.Add(typeof(byte[]), SqlDbType.VarBinary);
			type_mapping.Add(typeof(SqlBinary), SqlDbType.VarBinary);
			type_mapping.Add(typeof(SqlBytes), SqlDbType.VarBinary);
			type_mapping.Add(typeof(byte), SqlDbType.TinyInt);
			type_mapping.Add(typeof(SqlByte), SqlDbType.TinyInt);
			type_mapping.Add(typeof(int), SqlDbType.Int);
			type_mapping.Add(typeof(SqlInt32), SqlDbType.Int);
			type_mapping.Add(typeof(float), SqlDbType.Real);
			type_mapping.Add(typeof(SqlSingle), SqlDbType.Real);
			type_mapping.Add(typeof(short), SqlDbType.SmallInt);
			type_mapping.Add(typeof(SqlInt16), SqlDbType.SmallInt);
			type_mapping.Add(typeof(Guid), SqlDbType.UniqueIdentifier);
			type_mapping.Add(typeof(SqlGuid), SqlDbType.UniqueIdentifier);
			type_mapping.Add(typeof(SqlMoney), SqlDbType.Money);
			type_mapping.Add(typeof(XmlReader), SqlDbType.Xml);
			type_mapping.Add(typeof(SqlXml), SqlDbType.Xml);
			type_mapping.Add(typeof(object), SqlDbType.Variant);
		}

		object ICloneable.Clone()
		{
			return new SqlParameter(ParameterName, SqlDbType, Size, Direction, IsNullable, Precision, Scale, SourceColumn, SourceVersion, Value);
		}

		internal void CheckIfInitialized()
		{
			if (!isTypeSet)
			{
				throw new Exception("all parameters to have an explicity set type");
			}
			if (MetaParameter.IsVariableSizeType)
			{
				if (SqlDbType == SqlDbType.Decimal && Precision == 0)
				{
					throw new Exception("Parameter of type 'Decimal' have an explicitly set Precision and Scale");
				}
				if (Size == 0)
				{
					throw new Exception("all variable length parameters to have an explicitly set non-zero Size");
				}
			}
		}

		private void InferSqlType(object value)
		{
			if (value == null || value == DBNull.Value)
			{
				SetSqlDbType(SqlDbType.NVarChar);
				return;
			}
			Type type = value.GetType();
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			object obj = type_mapping[type];
			if (obj == null)
			{
				throw new ArgumentException(string.Format("The parameter data type of {0} is invalid.", type.FullName));
			}
			SetSqlDbType((SqlDbType)(int)obj);
		}

		private DbType DbTypeFromName(string name)
		{
			switch (name.ToLower())
			{
			case "ansistring":
				return DbType.AnsiString;
			case "ansistringfixedlength":
				return DbType.AnsiStringFixedLength;
			case "binary":
				return DbType.Binary;
			case "boolean":
				return DbType.Boolean;
			case "byte":
				return DbType.Byte;
			case "currency":
				return DbType.Currency;
			case "date":
				return DbType.Date;
			case "datetime":
				return DbType.DateTime;
			case "decimal":
				return DbType.Decimal;
			case "double":
				return DbType.Double;
			case "guid":
				return DbType.Guid;
			case "int16":
				return DbType.Int16;
			case "int32":
				return DbType.Int32;
			case "int64":
				return DbType.Int64;
			case "object":
				return DbType.Object;
			case "single":
				return DbType.Single;
			case "string":
				return DbType.String;
			case "stringfixedlength":
				return DbType.StringFixedLength;
			case "time":
				return DbType.Time;
			case "xml":
				return DbType.Xml;
			default:
			{
				string message = string.Format("No mapping exists from {0} to a known DbType.", name);
				throw new ArgumentException(message);
			}
			}
		}

		private void SetDbType(DbType type)
		{
			switch (type)
			{
			case DbType.AnsiString:
				MetaParameter.TypeName = "varchar";
				sqlDbType = SqlDbType.VarChar;
				MetaParameter.IsVariableSizeType = true;
				break;
			case DbType.AnsiStringFixedLength:
				MetaParameter.TypeName = "char";
				sqlDbType = SqlDbType.Char;
				MetaParameter.IsVariableSizeType = true;
				break;
			case DbType.Binary:
				MetaParameter.TypeName = "varbinary";
				sqlDbType = SqlDbType.VarBinary;
				MetaParameter.IsVariableSizeType = true;
				break;
			case DbType.Boolean:
				MetaParameter.TypeName = "bit";
				sqlDbType = SqlDbType.Bit;
				break;
			case DbType.Byte:
				MetaParameter.TypeName = "tinyint";
				sqlDbType = SqlDbType.TinyInt;
				break;
			case DbType.Currency:
				sqlDbType = SqlDbType.Money;
				MetaParameter.TypeName = "money";
				break;
			case DbType.Date:
			case DbType.DateTime:
				MetaParameter.TypeName = "datetime";
				sqlDbType = SqlDbType.DateTime;
				break;
			case DbType.Decimal:
				MetaParameter.TypeName = "decimal";
				sqlDbType = SqlDbType.Decimal;
				break;
			case DbType.Double:
				MetaParameter.TypeName = "float";
				sqlDbType = SqlDbType.Float;
				break;
			case DbType.Guid:
				MetaParameter.TypeName = "uniqueidentifier";
				sqlDbType = SqlDbType.UniqueIdentifier;
				break;
			case DbType.Int16:
				MetaParameter.TypeName = "smallint";
				sqlDbType = SqlDbType.SmallInt;
				break;
			case DbType.Int32:
				MetaParameter.TypeName = "int";
				sqlDbType = SqlDbType.Int;
				break;
			case DbType.Int64:
				MetaParameter.TypeName = "bigint";
				sqlDbType = SqlDbType.BigInt;
				break;
			case DbType.Object:
				MetaParameter.TypeName = "sql_variant";
				sqlDbType = SqlDbType.Variant;
				break;
			case DbType.Single:
				MetaParameter.TypeName = "real";
				sqlDbType = SqlDbType.Real;
				break;
			case DbType.String:
				MetaParameter.TypeName = "nvarchar";
				sqlDbType = SqlDbType.NVarChar;
				MetaParameter.IsVariableSizeType = true;
				break;
			case DbType.StringFixedLength:
				MetaParameter.TypeName = "nchar";
				sqlDbType = SqlDbType.NChar;
				MetaParameter.IsVariableSizeType = true;
				break;
			case DbType.Time:
				MetaParameter.TypeName = "datetime";
				sqlDbType = SqlDbType.DateTime;
				break;
			case DbType.Xml:
				MetaParameter.TypeName = "xml";
				sqlDbType = SqlDbType.Xml;
				MetaParameter.IsVariableSizeType = true;
				break;
			default:
			{
				string message = string.Format("No mapping exists from DbType {0} to a known SqlDbType.", type);
				throw new ArgumentException(message);
			}
			}
			dbType = type;
		}

		private SqlDbType FrameworkDbTypeFromName(string dbTypeName)
		{
			switch (dbTypeName.ToLower())
			{
			case "bigint":
				return SqlDbType.BigInt;
			case "binary":
				return SqlDbType.Binary;
			case "bit":
				return SqlDbType.Bit;
			case "char":
				return SqlDbType.Char;
			case "datetime":
				return SqlDbType.DateTime;
			case "decimal":
				return SqlDbType.Decimal;
			case "float":
				return SqlDbType.Float;
			case "image":
				return SqlDbType.Image;
			case "int":
				return SqlDbType.Int;
			case "money":
				return SqlDbType.Money;
			case "nchar":
				return SqlDbType.NChar;
			case "ntext":
				return SqlDbType.NText;
			case "nvarchar":
				return SqlDbType.NVarChar;
			case "real":
				return SqlDbType.Real;
			case "smalldatetime":
				return SqlDbType.SmallDateTime;
			case "smallint":
				return SqlDbType.SmallInt;
			case "smallmoney":
				return SqlDbType.SmallMoney;
			case "text":
				return SqlDbType.Text;
			case "timestamp":
				return SqlDbType.Timestamp;
			case "tinyint":
				return SqlDbType.TinyInt;
			case "uniqueidentifier":
				return SqlDbType.UniqueIdentifier;
			case "varbinary":
				return SqlDbType.VarBinary;
			case "varchar":
				return SqlDbType.VarChar;
			case "sql_variant":
				return SqlDbType.Variant;
			case "xml":
				return SqlDbType.Xml;
			default:
				return SqlDbType.Variant;
			}
		}

		internal void SetSqlDbType(SqlDbType type)
		{
			switch (type)
			{
			case SqlDbType.BigInt:
				MetaParameter.TypeName = "bigint";
				dbType = DbType.Int64;
				break;
			case SqlDbType.Binary:
				MetaParameter.TypeName = "binary";
				dbType = DbType.Binary;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.Timestamp:
				MetaParameter.TypeName = "timestamp";
				dbType = DbType.Binary;
				break;
			case SqlDbType.VarBinary:
				MetaParameter.TypeName = "varbinary";
				dbType = DbType.Binary;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.Bit:
				MetaParameter.TypeName = "bit";
				dbType = DbType.Boolean;
				break;
			case SqlDbType.Char:
				MetaParameter.TypeName = "char";
				dbType = DbType.AnsiStringFixedLength;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.DateTime:
				MetaParameter.TypeName = "datetime";
				dbType = DbType.DateTime;
				break;
			case SqlDbType.SmallDateTime:
				MetaParameter.TypeName = "smalldatetime";
				dbType = DbType.DateTime;
				break;
			case SqlDbType.Decimal:
				MetaParameter.TypeName = "decimal";
				dbType = DbType.Decimal;
				break;
			case SqlDbType.Float:
				MetaParameter.TypeName = "float";
				dbType = DbType.Double;
				break;
			case SqlDbType.Image:
				MetaParameter.TypeName = "image";
				dbType = DbType.Binary;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.Int:
				MetaParameter.TypeName = "int";
				dbType = DbType.Int32;
				break;
			case SqlDbType.Money:
				MetaParameter.TypeName = "money";
				dbType = DbType.Currency;
				break;
			case SqlDbType.SmallMoney:
				MetaParameter.TypeName = "smallmoney";
				dbType = DbType.Currency;
				break;
			case SqlDbType.NChar:
				MetaParameter.TypeName = "nchar";
				dbType = DbType.StringFixedLength;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.NText:
				MetaParameter.TypeName = "ntext";
				dbType = DbType.String;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.NVarChar:
				MetaParameter.TypeName = "nvarchar";
				dbType = DbType.String;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.Real:
				MetaParameter.TypeName = "real";
				dbType = DbType.Single;
				break;
			case SqlDbType.SmallInt:
				MetaParameter.TypeName = "smallint";
				dbType = DbType.Int16;
				break;
			case SqlDbType.Text:
				MetaParameter.TypeName = "text";
				dbType = DbType.AnsiString;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.VarChar:
				MetaParameter.TypeName = "varchar";
				dbType = DbType.AnsiString;
				MetaParameter.IsVariableSizeType = true;
				break;
			case SqlDbType.TinyInt:
				MetaParameter.TypeName = "tinyint";
				dbType = DbType.Byte;
				break;
			case SqlDbType.UniqueIdentifier:
				MetaParameter.TypeName = "uniqueidentifier";
				dbType = DbType.Guid;
				break;
			case SqlDbType.Variant:
				MetaParameter.TypeName = "sql_variant";
				dbType = DbType.Object;
				break;
			case SqlDbType.Xml:
				MetaParameter.TypeName = "xml";
				dbType = DbType.Xml;
				MetaParameter.IsVariableSizeType = true;
				break;
			default:
			{
				string message = string.Format("No mapping exists from SqlDbType {0} to a known DbType.", type);
				throw new ArgumentOutOfRangeException("SqlDbType", message);
			}
			}
			sqlDbType = type;
		}

		public override string ToString()
		{
			return ParameterName;
		}

		private object GetFrameworkValue(object rawValue, ref bool updated)
		{
			updated = typeChanged || updated;
			object result;
			if (updated)
			{
				result = SqlTypeToFrameworkType(rawValue);
				typeChanged = false;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private object GetSqlValue(object value)
		{
			if (value == null)
			{
				return value;
			}
			switch (sqlDbType)
			{
			case SqlDbType.BigInt:
				if (value == DBNull.Value)
				{
					return SqlInt64.Null;
				}
				return (SqlInt64)(long)value;
			case SqlDbType.Binary:
			case SqlDbType.Image:
			case SqlDbType.Timestamp:
			case SqlDbType.VarBinary:
				if (value == DBNull.Value)
				{
					return SqlBinary.Null;
				}
				return (SqlBinary)(byte[])value;
			case SqlDbType.Bit:
				if (value == DBNull.Value)
				{
					return SqlBoolean.Null;
				}
				return (SqlBoolean)(bool)value;
			case SqlDbType.Char:
			case SqlDbType.NChar:
			case SqlDbType.NText:
			case SqlDbType.NVarChar:
			case SqlDbType.Text:
			case SqlDbType.VarChar:
			{
				if (value == DBNull.Value)
				{
					return SqlString.Null;
				}
				Type type = value.GetType();
				string text = ((type == typeof(char)) ? value.ToString() : ((type != typeof(char[])) ? ((string)value) : new string((char[])value)));
				return (SqlString)text;
			}
			case SqlDbType.DateTime:
			case SqlDbType.SmallDateTime:
				if (value == DBNull.Value)
				{
					return SqlDateTime.Null;
				}
				return (SqlDateTime)(DateTime)value;
			case SqlDbType.Decimal:
				if (value == DBNull.Value)
				{
					return SqlDecimal.Null;
				}
				if (value is TdsBigDecimal)
				{
					return SqlDecimal.FromTdsBigDecimal((TdsBigDecimal)value);
				}
				return (SqlDecimal)(decimal)value;
			case SqlDbType.Float:
				if (value == DBNull.Value)
				{
					return SqlDouble.Null;
				}
				return (SqlDouble)(double)value;
			case SqlDbType.Int:
				if (value == DBNull.Value)
				{
					return SqlInt32.Null;
				}
				return (SqlInt32)(int)value;
			case SqlDbType.Money:
			case SqlDbType.SmallMoney:
				if (value == DBNull.Value)
				{
					return SqlMoney.Null;
				}
				return (SqlMoney)(decimal)value;
			case SqlDbType.Real:
				if (value == DBNull.Value)
				{
					return SqlSingle.Null;
				}
				return (SqlSingle)(float)value;
			case SqlDbType.UniqueIdentifier:
				if (value == DBNull.Value)
				{
					return SqlGuid.Null;
				}
				return (SqlGuid)(Guid)value;
			case SqlDbType.SmallInt:
				if (value == DBNull.Value)
				{
					return SqlInt16.Null;
				}
				return (SqlInt16)(short)value;
			case SqlDbType.TinyInt:
				if (value == DBNull.Value)
				{
					return SqlByte.Null;
				}
				return (SqlByte)(byte)value;
			case SqlDbType.Xml:
				if (value == DBNull.Value)
				{
					return SqlXml.Null;
				}
				return (SqlXml)value;
			default:
				throw new NotImplementedException(string.Concat("Type '", sqlDbType, "' not implemented."));
			}
		}

		private object SqlTypeToFrameworkType(object value)
		{
			INullable nullable = value as INullable;
			if (nullable == null)
			{
				return ConvertToFrameworkType(value);
			}
			if (nullable.IsNull)
			{
				return DBNull.Value;
			}
			Type type = value.GetType();
			if (typeof(SqlString) == type)
			{
				return ((SqlString)value).Value;
			}
			if (typeof(SqlInt16) == type)
			{
				return ((SqlInt16)value).Value;
			}
			if (typeof(SqlInt32) == type)
			{
				return ((SqlInt32)value).Value;
			}
			if (typeof(SqlDateTime) == type)
			{
				return ((SqlDateTime)value).Value;
			}
			if (typeof(SqlInt64) == type)
			{
				return ((SqlInt64)value).Value;
			}
			if (typeof(SqlBinary) == type)
			{
				return ((SqlBinary)value).Value;
			}
			if (typeof(SqlBytes) == type)
			{
				return ((SqlBytes)value).Value;
			}
			if (typeof(SqlChars) == type)
			{
				return ((SqlChars)value).Value;
			}
			if (typeof(SqlBoolean) == type)
			{
				return ((SqlBoolean)value).Value;
			}
			if (typeof(SqlByte) == type)
			{
				return ((SqlByte)value).Value;
			}
			if (typeof(SqlDecimal) == type)
			{
				return ((SqlDecimal)value).Value;
			}
			if (typeof(SqlDouble) == type)
			{
				return ((SqlDouble)value).Value;
			}
			if (typeof(SqlGuid) == type)
			{
				return ((SqlGuid)value).Value;
			}
			if (typeof(SqlMoney) == type)
			{
				return ((SqlMoney)value).Value;
			}
			if (typeof(SqlSingle) == type)
			{
				return ((SqlSingle)value).Value;
			}
			return value;
		}

		internal object ConvertToFrameworkType(object value)
		{
			if (value == null || value == DBNull.Value)
			{
				return value;
			}
			if (sqlDbType == SqlDbType.Variant)
			{
				return metaParameter.Value;
			}
			Type systemType = SystemType;
			if (systemType == null)
			{
				throw new NotImplementedException("Type Not Supported : " + sqlDbType);
			}
			Type type = value.GetType();
			if (type == systemType)
			{
				return value;
			}
			object obj = null;
			try
			{
				return ConvertToFrameworkType(value, systemType);
			}
			catch (FormatException innerException)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Parameter value could not be converted from {0} to {1}.", type.Name, systemType.Name), innerException);
			}
		}

		private object ConvertToFrameworkType(object value, Type frameworkType)
		{
			object obj = Convert.ChangeType(value, frameworkType);
			SqlDbType sqlDbType = this.sqlDbType;
			if (sqlDbType == SqlDbType.Money || sqlDbType == SqlDbType.SmallMoney)
			{
				obj = decimal.Round((decimal)obj, 4);
			}
			return obj;
		}

		public override void ResetDbType()
		{
			InferSqlType(Value);
		}

		public void ResetSqlDbType()
		{
			InferSqlType(Value);
		}
	}
}
