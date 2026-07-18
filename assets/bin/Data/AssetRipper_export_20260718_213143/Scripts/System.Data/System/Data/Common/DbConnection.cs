using System.ComponentModel;
using System.Transactions;

namespace System.Data.Common
{
	public abstract class DbConnection : Component, IDisposable, IDbConnection
	{
		private static class DataTypes
		{
			private static readonly ColumnInfo[] columns = new ColumnInfo[22]
			{
				new ColumnInfo("TypeName", typeof(string)),
				new ColumnInfo("ProviderDbType", typeof(int)),
				new ColumnInfo("ColumnSize", typeof(long)),
				new ColumnInfo("CreateFormat", typeof(string)),
				new ColumnInfo("CreateParameters", typeof(string)),
				new ColumnInfo("DataType", typeof(string)),
				new ColumnInfo("IsAutoIncrementable", typeof(bool)),
				new ColumnInfo("IsBestMatch", typeof(bool)),
				new ColumnInfo("IsCaseSensitive", typeof(bool)),
				new ColumnInfo("IsFixedLength", typeof(bool)),
				new ColumnInfo("IsFixedPrecisionScale", typeof(bool)),
				new ColumnInfo("IsLong", typeof(bool)),
				new ColumnInfo("IsNullable", typeof(bool)),
				new ColumnInfo("IsSearchable", typeof(bool)),
				new ColumnInfo("IsSearchableWithLike", typeof(bool)),
				new ColumnInfo("IsUnsigned", typeof(bool)),
				new ColumnInfo("MaximumScale", typeof(short)),
				new ColumnInfo("MinimumScale", typeof(short)),
				new ColumnInfo("IsConcurrencyType", typeof(bool)),
				new ColumnInfo("IsLiteralSupported", typeof(bool)),
				new ColumnInfo("LiteralPrefix", typeof(string)),
				new ColumnInfo("LiteralSuffix", typeof(string))
			};

			private static readonly object[][] rows = new object[26][]
			{
				new object[22]
				{
					"smallint", 16, 5, "smallint", null, "System.Int16", true, true, false, true,
					true, false, true, true, false, false, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"int", 8, 10, "int", null, "System.Int32", true, true, false, true,
					true, false, true, true, false, false, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"real", 13, 7, "real", null, "System.Single", false, true, false, true,
					false, false, true, true, false, false, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"float", 6, 53, "float({0})", "number of bits used to store the mantissa", "System.Double", false, true, false, true,
					false, false, true, true, false, false, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"money", 9, 19, "money", null, "System.Decimal", false, false, false, true,
					true, false, true, true, false, false, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"smallmoney", 17, 10, "smallmoney", null, "System.Decimal", false, false, false, true,
					true, false, true, true, false, false, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"bit", 2, 1, "bit", null, "System.Boolean", false, false, false, true,
					false, false, true, true, false, null, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"tinyint", 20, 3, "tinyint", null, "System.SByte", true, true, false, true,
					true, false, true, true, false, true, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"bigint", 0, 19, "bigint", null, "System.Int64", true, true, false, true,
					true, false, true, true, false, false, null, null, false, null,
					null, null
				},
				new object[22]
				{
					"timestamp", 19, 8, "timestamp", null, "System.Byte[]", false, false, false, true,
					false, false, false, true, false, null, null, null, true, null,
					"0x", null
				},
				new object[22]
				{
					"binary", 1, 8000, "binary({0})", "length", "System.Byte[]", false, true, false, true,
					false, false, true, true, false, null, null, null, false, null,
					"0x", null
				},
				new object[22]
				{
					"image",
					7,
					int.MaxValue,
					"image",
					null,
					"System.Byte[]",
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					false,
					false,
					null,
					null,
					null,
					false,
					null,
					"0x",
					null
				},
				new object[22]
				{
					"text",
					18,
					int.MaxValue,
					"text",
					null,
					"System.String",
					false,
					true,
					false,
					false,
					false,
					true,
					true,
					false,
					true,
					null,
					null,
					null,
					false,
					null,
					"'",
					"'"
				},
				new object[22]
				{
					"ntext", 11, 1073741823, "ntext", null, "System.String", false, true, false, false,
					false, true, true, false, true, null, null, null, false, null,
					"N'", "'"
				},
				new object[22]
				{
					"decimal", 5, 38, "decimal({0}, {1})", "precision,scale", "System.Decimal", true, true, false, true,
					false, false, true, true, false, false, 38, 0, false, null,
					null, null
				},
				new object[22]
				{
					"numeric", 5, 38, "numeric({0}, {1})", "precision,scale", "System.Decimal", true, true, false, true,
					false, false, true, true, false, false, 38, 0, false, null,
					null, null
				},
				new object[22]
				{
					"datetime", 4, 23, "datetime", null, "System.DateTime", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"{ts '", "'}"
				},
				new object[22]
				{
					"smalldatetime", 15, 16, "smalldatetime", null, "System.DateTime", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"{ts '", "'}"
				},
				new object[22]
				{
					"sql_variant", 23, null, "sql_variant", null, "System.Object", false, true, false, false,
					false, false, true, true, false, null, null, null, false, false,
					null, null
				},
				new object[22]
				{
					"xml",
					25,
					int.MaxValue,
					"xml",
					null,
					"System.String",
					false,
					false,
					false,
					false,
					false,
					true,
					true,
					false,
					false,
					null,
					null,
					null,
					false,
					false,
					null,
					null
				},
				new object[22]
				{
					"varchar",
					22,
					int.MaxValue,
					"varchar({0})",
					"max length",
					"System.String",
					false,
					true,
					false,
					false,
					false,
					false,
					true,
					true,
					true,
					null,
					null,
					null,
					false,
					null,
					"'",
					"'"
				},
				new object[22]
				{
					"char",
					3,
					int.MaxValue,
					"char({0})",
					"length",
					"System.String",
					false,
					true,
					false,
					true,
					false,
					false,
					true,
					true,
					true,
					null,
					null,
					null,
					false,
					null,
					"'",
					"'"
				},
				new object[22]
				{
					"nchar", 10, 1073741823, "nchar({0})", "length", "System.String", false, true, false, true,
					false, false, true, true, true, null, null, null, false, null,
					"N'", "'"
				},
				new object[22]
				{
					"nvarchar", 12, 1073741823, "nvarchar({0})", "max length", "System.String", false, true, false, false,
					false, false, true, true, true, null, null, null, false, null,
					"N'", "'"
				},
				new object[22]
				{
					"varbinary", 21, 1073741823, "varbinary({0})", "max length", "System.Byte[]", false, true, false, false,
					false, false, true, true, false, null, null, null, false, null,
					"0x", null
				},
				new object[22]
				{
					"uniqueidentifier", 14, 16, "uniqueidentifier", null, "System.Guid", false, true, false, true,
					false, false, true, true, false, null, null, null, false, null,
					"'", "'"
				}
			};

			private static DataTable instance;

			public static DataTable Instance
			{
				get
				{
					if (instance == null)
					{
						instance = new DataTable("DataTypes");
						ColumnInfo[] array = columns;
						for (int i = 0; i < array.Length; i++)
						{
							ColumnInfo columnInfo = array[i];
							instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						object[][] array2 = rows;
						foreach (object[] values in array2)
						{
							instance.LoadDataRow(values, true);
						}
					}
					return instance;
				}
			}
		}

		private struct ColumnInfo
		{
			public string name;

			public Type type;

			public ColumnInfo(string name, Type type)
			{
				this.name = name;
				this.type = type;
			}
		}

		internal static class MetaDataCollections
		{
			private static readonly ColumnInfo[] columns = new ColumnInfo[3]
			{
				new ColumnInfo("CollectionName", typeof(string)),
				new ColumnInfo("NumberOfRestrictions", typeof(int)),
				new ColumnInfo("NumberOfIdentifierParts", typeof(int))
			};

			private static readonly object[][] rows = new object[17][]
			{
				new object[3] { "MetaDataCollections", 0, 0 },
				new object[3] { "DataSourceInformation", 0, 0 },
				new object[3] { "DataTypes", 0, 0 },
				new object[3] { "Restrictions", 0, 0 },
				new object[3] { "ReservedWords", 0, 0 },
				new object[3] { "Users", 1, 1 },
				new object[3] { "Databases", 1, 1 },
				new object[3] { "Tables", 4, 3 },
				new object[3] { "Columns", 4, 4 },
				new object[3] { "Views", 3, 3 },
				new object[3] { "ViewColumns", 4, 4 },
				new object[3] { "ProcedureParameters", 4, 1 },
				new object[3] { "Procedures", 4, 3 },
				new object[3] { "ForeignKeys", 4, 3 },
				new object[3] { "IndexColumns", 5, 4 },
				new object[3] { "Indexes", 4, 3 },
				new object[3] { "UserDefinedTypes", 2, 1 }
			};

			private static DataTable instance;

			public static DataTable Instance
			{
				get
				{
					if (instance == null)
					{
						instance = new DataTable("GetSchema");
						ColumnInfo[] array = columns;
						for (int i = 0; i < array.Length; i++)
						{
							ColumnInfo columnInfo = array[i];
							instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						object[][] array2 = rows;
						foreach (object[] values in array2)
						{
							instance.LoadDataRow(values, true);
						}
					}
					return instance;
				}
			}
		}

		private static class Restrictions
		{
			private static readonly ColumnInfo[] columns = new ColumnInfo[5]
			{
				new ColumnInfo("CollectionName", typeof(string)),
				new ColumnInfo("RestrictionName", typeof(string)),
				new ColumnInfo("ParameterName", typeof(string)),
				new ColumnInfo("RestrictionDefault", typeof(string)),
				new ColumnInfo("RestrictionNumber", typeof(int))
			};

			private static readonly object[][] rows = new object[40][]
			{
				new object[5] { "Users", "User_Name", "@Name", "name", 1 },
				new object[5] { "Databases", "Name", "@Name", "Name", 1 },
				new object[5] { "Tables", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[5] { "Tables", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[5] { "Tables", "Table", "@Name", "TABLE_NAME", 3 },
				new object[5] { "Tables", "TableType", "@TableType", "TABLE_TYPE", 4 },
				new object[5] { "Columns", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[5] { "Columns", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[5] { "Columns", "Table", "@Table", "TABLE_NAME", 3 },
				new object[5] { "Columns", "Column", "@Column", "COLUMN_NAME", 4 },
				new object[5] { "Views", "Catalog", "@Catalog", "TABLE_CATALOG", 1 },
				new object[5] { "Views", "Owner", "@Owner", "TABLE_SCHEMA", 2 },
				new object[5] { "Views", "Table", "@Table", "TABLE_NAME", 3 },
				new object[5] { "ViewColumns", "Catalog", "@Catalog", "VIEW_CATALOG", 1 },
				new object[5] { "ViewColumns", "Owner", "@Owner", "VIEW_SCHEMA", 2 },
				new object[5] { "ViewColumns", "Table", "@Table", "VIEW_NAME", 3 },
				new object[5] { "ViewColumns", "Column", "@Column", "COLUMN_NAME", 4 },
				new object[5] { "ProcedureParameters", "Catalog", "@Catalog", "SPECIFIC_CATALOG", 1 },
				new object[5] { "ProcedureParameters", "Owner", "@Owner", "SPECIFIC_SCHEMA", 2 },
				new object[5] { "ProcedureParameters", "Name", "@Name", "SPECIFIC_NAME", 3 },
				new object[5] { "ProcedureParameters", "Parameter", "@Parameter", "PARAMETER_NAME", 4 },
				new object[5] { "Procedures", "Catalog", "@Catalog", "SPECIFIC_CATALOG", 1 },
				new object[5] { "Procedures", "Owner", "@Owner", "SPECIFIC_SCHEMA", 2 },
				new object[5] { "Procedures", "Name", "@Name", "SPECIFIC_NAME", 3 },
				new object[5] { "Procedures", "Type", "@Type", "ROUTINE_TYPE", 4 },
				new object[5] { "IndexColumns", "Catalog", "@Catalog", "db_name(}", 1 },
				new object[5] { "IndexColumns", "Owner", "@Owner", "user_name(}", 2 },
				new object[5] { "IndexColumns", "Table", "@Table", "o.name", 3 },
				new object[5] { "IndexColumns", "ConstraintName", "@ConstraintName", "x.name", 4 },
				new object[5] { "IndexColumns", "Column", "@Column", "c.name", 5 },
				new object[5] { "Indexes", "Catalog", "@Catalog", "db_name(}", 1 },
				new object[5] { "Indexes", "Owner", "@Owner", "user_name(}", 2 },
				new object[5] { "Indexes", "Table", "@Table", "o.name", 3 },
				new object[5] { "Indexes", "Name", "@Name", "x.name", 4 },
				new object[5] { "UserDefinedTypes", "assembly_name", "@AssemblyName", "assemblies.name", 1 },
				new object[5] { "UserDefinedTypes", "udt_name", "@UDTName", "types.assembly_class", 2 },
				new object[5] { "ForeignKeys", "Catalog", "@Catalog", "CONSTRAINT_CATALOG", 1 },
				new object[5] { "ForeignKeys", "Owner", "@Owner", "CONSTRAINT_SCHEMA", 2 },
				new object[5] { "ForeignKeys", "Table", "@Table", "TABLE_NAME", 3 },
				new object[5] { "ForeignKeys", "Name", "@Name", "CONSTRAINT_NAME", 4 }
			};

			private static DataTable instance;

			public static DataTable Instance
			{
				get
				{
					if (instance == null)
					{
						instance = new DataTable("Restrictions");
						ColumnInfo[] array = columns;
						for (int i = 0; i < array.Length; i++)
						{
							ColumnInfo columnInfo = array[i];
							instance.Columns.Add(columnInfo.name, columnInfo.type);
						}
						object[][] array2 = rows;
						foreach (object[] values in array2)
						{
							instance.LoadDataRow(values, true);
						}
					}
					return instance;
				}
			}
		}

		private static class ReservedWords
		{
			private static readonly string[] reservedWords = new string[393]
			{
				"ADD", "EXCEPT", "PERCENT", "ALL", "EXEC", "PLAN", "ALTER", "EXECUTE", "PRECISION", "AND",
				"EXISTS", "PRIMARY", "ANY", "EXIT", "PRINT", "AS", "FETCH", "PROC", "ASC", "FILE",
				"PROCEDURE", "AUTHORIZATION", "FILLFACTOR", "PUBLIC", "BACKUP", "FOR", "RAISERROR", "BEGIN", "FOREIGN", "READ",
				"BETWEEN", "FREETEXT", "READTEXT", "BREAK", "FREETEXTTABLE", "RECONFIGURE", "BROWSE", "FROM", "REFERENCES", "BULK",
				"FULL", "REPLICATION", "BY", "FUNCTION", "RESTORE", "CASCADE", "GOTO", "RESTRICT", "CASE", "GRANT",
				"RETURN", "CHECK", "GROUP", "REVOKE", "CHECKPOINT", "HAVING", "RIGHT", "CLOSE", "HOLDLOCK", "ROLLBACK",
				"CLUSTERED", "IDENTITY", "ROWCOUNT", "COALESCE", "IDENTITY_INSERT", "ROWGUIDCOL", "COLLATE", "IDENTITYCOL", "RULE", "COLUMN",
				"IF", "SAVE", "COMMIT", "IN", "SCHEMA", "COMPUTE", "INDEX", "SELECT", "CONSTRAINT", "INNER",
				"SESSION_USER", "CONTAINS", "INSERT", "SET", "CONTAINSTABLE", "INTERSECT", "SETUSER", "CONTINUE", "INTO", "SHUTDOWN",
				"CONVERT", "IS", "SOME", "CREATE", "JOIN", "STATISTICS", "CROSS", "KEY", "SYSTEM_USER", "CURRENT",
				"KILL", "TABLE", "CURRENT_DATE", "LEFT", "TEXTSIZE", "CURRENT_TIME", "LIKE", "THEN", "CURRENT_TIMESTAMP", "LINENO",
				"TO", "CURRENT_USER", "LOAD", "TOP", "CURSOR", "NATIONAL", "TRAN", "DATABASE", "NOCHECK", "TRANSACTION",
				"DBCC", "NONCLUSTERED", "TRIGGER", "DEALLOCATE", "NOT", "TRUNCATE", "DECLARE", "NULL", "TSEQUAL", "DEFAULT",
				"NULLIF", "UNION", "DELETE", "OF", "UNIQUE", "DENY", "OFF", "UPDATE", "DESC", "OFFSETS",
				"UPDATETEXT", "DISK", "ON", "USE", "DISTINCT", "OPEN", "USER", "DISTRIBUTED", "OPENDATASOURCE", "VALUES",
				"DOUBLE", "OPENQUERY", "VARYING", "DROP", "OPENROWSET", "VIEW", "DUMMY", "OPENXML", "WAITFOR", "DUMP",
				"OPTION", "WHEN", "ELSE", "OR", "WHERE", "END", "ORDER", "WHILE", "ERRLVL", "OUTER",
				"WITH", "ESCAPE", "OVER", "WRITETEXT", "ABSOLUTE", "FOUND", "PRESERVE", "ACTION", "FREE", "PRIOR",
				"ADMIN", "GENERAL", "PRIVILEGES", "AFTER", "GET", "READS", "AGGREGATE", "GLOBAL", "REAL", "ALIAS",
				"GO", "RECURSIVE", "ALLOCATE", "GROUPING", "REF", "ARE", "HOST", "REFERENCING", "ARRAY", "HOUR",
				"RELATIVE", "ASSERTION", "IGNORE", "RESULT", "AT", "IMMEDIATE", "RETURNS", "BEFORE", "INDICATOR", "ROLE",
				"BINARY", "INITIALIZE", "ROLLUP", "BIT", "INITIALLY", "ROUTINE", "BLOB", "INOUT", "ROW", "BOOLEAN",
				"INPUT", "ROWS", "BOTH", "INT", "SAVEPOINT", "BREADTH", "INTEGER", "SCROLL", "CALL", "INTERVAL",
				"SCOPE", "CASCADED", "ISOLATION", "SEARCH", "CAST", "ITERATE", "SECOND", "CATALOG", "LANGUAGE", "SECTION",
				"CHAR", "LARGE", "SEQUENCE", "CHARACTER", "LAST", "SESSION", "CLASS", "LATERAL", "SETS", "CLOB",
				"LEADING", "SIZE", "COLLATION", "LESS", "SMALLINT", "COMPLETION", "LEVEL", "SPACE", "CONNECT", "LIMIT",
				"SPECIFIC", "CONNECTION", "LOCAL", "SPECIFICTYPE", "CONSTRAINTS", "LOCALTIME", "SQL", "CONSTRUCTOR", "LOCALTIMESTAMP", "SQLEXCEPTION",
				"CORRESPONDING", "LOCATOR", "SQLSTATE", "CUBE", "MAP", "SQLWARNING", "CURRENT_PATH", "MATCH", "START", "CURRENT_ROLE",
				"MINUTE", "STATE", "CYCLE", "MODIFIES", "STATEMENT", "DATA", "MODIFY", "STATIC", "DATE", "MODULE",
				"STRUCTURE", "DAY", "MONTH", "TEMPORARY", "DEC", "NAMES", "TERMINATE", "DECIMAL", "NATURAL", "THAN",
				"DEFERRABLE", "NCHAR", "TIME", "DEFERRED", "NCLOB", "TIMESTAMP", "DEPTH", "NEW", "TIMEZONE_HOUR", "DEREF",
				"NEXT", "TIMEZONE_MINUTE", "DESCRIBE", "NO", "TRAILING", "DESCRIPTOR", "NONE", "TRANSLATION", "DESTROY", "NUMERIC",
				"TREAT", "DESTRUCTOR", "OBJECT", "TRUE", "DETERMINISTIC", "OLD", "UNDER", "DICTIONARY", "ONLY", "UNKNOWN",
				"DIAGNOSTICS", "OPERATION", "UNNEST", "DISCONNECT", "ORDINALITY", "USAGE", "DOMAIN", "OUT", "USING", "DYNAMIC",
				"OUTPUT", "VALUE", "EACH", "PAD", "VARCHAR", "END-EXEC", "PARAMETER", "VARIABLE", "EQUALS", "PARAMETERS",
				"WHENEVER", "EVERY", "PARTIAL", "WITHOUT", "EXCEPTION", "PATH", "WORK", "EXTERNAL", "POSTFIX", "WRITE",
				"FALSE", "PREFIX", "YEAR", "FIRST", "PREORDER", "ZONE", "FLOAT", "PREPARE", "ADA", "AVG",
				"BIT_LENGTH", "CHAR_LENGTH", "CHARACTER_LENGTH", "COUNT", "EXTRACT", "FORTRAN", "INCLUDE", "INSENSITIVE", "LOWER", "MAX",
				"MIN", "OCTET_LENGTH", "OVERLAPS", "PASCAL", "POSITION", "SQLCA", "SQLCODE", "SQLERROR", "SUBSTRING", "SUM",
				"TRANSLATE", "TRIM", "UPPER"
			};

			private static DataTable instance;

			public static DataTable Instance
			{
				get
				{
					if (instance == null)
					{
						DataRow dataRow = null;
						instance = new DataTable("ReservedWords");
						instance.Columns.Add("ReservedWord", typeof(string));
						string[] array = reservedWords;
						foreach (string value in array)
						{
							dataRow = instance.NewRow();
							dataRow["ReservedWord"] = value;
							instance.Rows.Add(dataRow);
						}
					}
					return instance;
				}
			}
		}

		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		[RecommendedAsConfigurable(true)]
		public abstract string ConnectionString { get; set; }

		public abstract string Database { get; }

		public abstract string DataSource { get; }

		[Browsable(false)]
		public abstract string ServerVersion { get; }

		[Browsable(false)]
		public abstract ConnectionState State { get; }

		public virtual int ConnectionTimeout
		{
			get
			{
				return 15;
			}
		}

		protected virtual DbProviderFactory DbProviderFactory
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual event StateChangeEventHandler StateChange;

		IDbTransaction IDbConnection.BeginTransaction()
		{
			return BeginTransaction();
		}

		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
		{
			return BeginTransaction(il);
		}

		IDbCommand IDbConnection.CreateCommand()
		{
			return CreateCommand();
		}

		protected abstract DbTransaction BeginDbTransaction(IsolationLevel isolationLevel);

		public DbTransaction BeginTransaction()
		{
			return BeginDbTransaction(IsolationLevel.Unspecified);
		}

		public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			return BeginDbTransaction(isolationLevel);
		}

		public abstract void ChangeDatabase(string databaseName);

		public abstract void Close();

		public DbCommand CreateCommand()
		{
			return CreateDbCommand();
		}

		protected abstract DbCommand CreateDbCommand();

		public virtual void EnlistTransaction(Transaction transaction)
		{
			throw new NotSupportedException();
		}

		public virtual DataTable GetSchema()
		{
			return MetaDataCollections.Instance;
		}

		public virtual DataTable GetSchema(string collectionName)
		{
			return GetSchema(collectionName, null);
		}

		private void AddParameter(DbCommand command, string parameterName, DbType parameterType, int parameterSize)
		{
			DbParameter dbParameter = command.CreateParameter();
			dbParameter.ParameterName = parameterName;
			dbParameter.DbType = parameterType;
			dbParameter.Size = parameterSize;
			command.Parameters.Add(dbParameter);
		}

		public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			if (collectionName == null)
			{
				throw new ArgumentException();
			}
			string text = null;
			DataTable instance = MetaDataCollections.Instance;
			int num = ((restrictionValues != null) ? restrictionValues.Length : 0);
			foreach (DataRow row in instance.Rows)
			{
				if (string.Compare((string)row["CollectionName"], collectionName, true) == 0)
				{
					if (num > (int)row["NumberOfRestrictions"])
					{
						throw new ArgumentException("More restrictions were provided than the requested schema ('" + row["CollectionName"].ToString() + "') supports");
					}
					text = row["CollectionName"].ToString();
				}
			}
			if (text == null)
			{
				throw new ArgumentException("The requested collection ('" + collectionName + "') is not defined.");
			}
			DbCommand dbCommand = null;
			DataTable dataTable = new DataTable();
			switch (text)
			{
			case "Databases":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select name as database_name, dbid, crdate as create_date from master.sys.sysdatabases where (name = @Name or (@Name is null))";
				AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				break;
			case "ForeignKeys":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_TYPE, IS_DEFERRABLE, INITIALLY_DEFERRED from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where (CONSTRAINT_CATALOG = @Catalog or (@Catalog is null)) and (CONSTRAINT_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @Table or (@Table is null)) and (CONSTRAINT_NAME = @Name or (@Name is null)) and CONSTRAINT_TYPE = 'FOREIGN KEY' order by CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME";
				AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				break;
			case "Indexes":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select distinct db_name() as constraint_catalog, constraint_schema = user_name (o.uid), constraint_name = x.name, table_catalog = db_name (), table_schema = user_name (o.uid), table_name = o.name, index_name  = x.name from sysobjects o, sysindexes x, sysindexkeys xk where o.type in ('U') and x.id = o.id and o.id = xk.id and x.indid = xk.indid and xk.keyno = x.keycnt and (db_name() = @Catalog or (@Catalog is null)) and (user_name() = @Owner or (@Owner is null)) and (o.name = @Table or (@Table is null)) and (x.name = @Name or (@Name is null))order by table_name, index_name";
				AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				break;
			case "IndexColumns":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select distinct db_name() as constraint_catalog, constraint_schema = user_name (o.uid), constraint_name = x.name, table_catalog = db_name (), table_schema = user_name (o.uid), table_name = o.name, column_name = c.name, ordinal_position = convert (int, xk.keyno), keyType = c.xtype, index_name = x.name from sysobjects o, sysindexes x, syscolumns c, sysindexkeys xk where o.type in ('U') and x.id = o.id and o.id = c.id and o.id = xk.id and x.indid = xk.indid and c.colid = xk.colid and xk.keyno <= x.keycnt and permissions (o.id, c.name) <> 0 and (db_name() = @Catalog or (@Catalog is null)) and (user_name() = @Owner or (@Owner is null)) and (o.name = @Table or (@Table is null)) and (x.name = @ConstraintName or (@ConstraintName is null)) and (c.name = @Column or (@Column is null)) order by table_name, index_name";
				AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 8);
				AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 13);
				AddParameter(dbCommand, "@ConstraintName", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Column", DbType.StringFixedLength, 4000);
				break;
			case "Procedures":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_TYPE, CREATED, LAST_ALTERED from INFORMATION_SCHEMA.ROUTINES where (SPECIFIC_CATALOG = @Catalog or (@Catalog is null)) and (SPECIFIC_SCHEMA = @Owner or (@Owner is null)) and (SPECIFIC_NAME = @Name or (@Name is null)) and (ROUTINE_TYPE = @Type or (@Type is null)) order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME";
				AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Type", DbType.StringFixedLength, 4000);
				break;
			case "ProcedureParameters":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, IS_RESULT, AS_LOCATOR, PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, COLLATION_CATALOG, COLLATION_SCHEMA, COLLATION_NAME, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, INTERVAL_TYPE, INTERVAL_PRECISION from INFORMATION_SCHEMA.PARAMETERS where (SPECIFIC_CATALOG = @Catalog or (@Catalog is null)) and (SPECIFIC_SCHEMA = @Owner or (@Owner is null)) and (SPECIFIC_NAME = @Name or (@Name is null)) and (PARAMETER_NAME = @Parameter or (@Parameter is null)) order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, PARAMETER_NAME";
				AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Parameter", DbType.StringFixedLength, 4000);
				break;
			case "Tables":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE from INFORMATION_SCHEMA.TABLES where (TABLE_CATALOG = @catalog or (@catalog is null)) and (TABLE_SCHEMA = @owner or (@owner is null))and (TABLE_NAME = @name or (@name is null)) and (TABLE_TYPE = @table_type or (@table_type is null))";
				AddParameter(dbCommand, "@catalog", DbType.StringFixedLength, 8);
				AddParameter(dbCommand, "@owner", DbType.StringFixedLength, 3);
				AddParameter(dbCommand, "@name", DbType.StringFixedLength, 11);
				AddParameter(dbCommand, "@table_type", DbType.StringFixedLength, 10);
				break;
			case "Columns":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, COLLATION_CATALOG from INFORMATION_SCHEMA.COLUMNS where (TABLE_CATALOG = @Catalog or (@Catalog is null)) and (TABLE_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @table or (@Table is null)) and (COLUMN_NAME = @column or (@Column is null)) order by TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME";
				AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Column", DbType.StringFixedLength, 4000);
				break;
			case "Users":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select uid, name as user_name, createdate, updatedate from sysusers where (name = @Name or (@Name is null))";
				AddParameter(dbCommand, "@Name", DbType.StringFixedLength, 4000);
				break;
			case "Views":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CHECK_OPTION, IS_UPDATABLE from INFORMATION_SCHEMA.VIEWS where (TABLE_CATALOG = @Catalog or (@Catalog is null)) TABLE_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @table or (@Table is null)) order by TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME";
				AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				break;
			case "ViewColumns":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME from INFORMATION_SCHEMA.VIEW_COLUMN_USAGE where (VIEW_CATALOG = @Catalog (@Catalog is null)) and (VIEW_SCHEMA = @Owner (@Owner is null)) and (VIEW_NAME = @Table or (@Table is null)) and (COLUMN_NAME = @Column or (@Column is null)) order by VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME";
				AddParameter(dbCommand, "@Catalog", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Owner", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Table", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@Column", DbType.StringFixedLength, 4000);
				break;
			case "UserDefinedTypes":
				dbCommand = CreateCommand();
				dbCommand.Connection = this;
				dbCommand.CommandText = "select assemblies.name as assembly_name, types.assembly_class as udt_name, ASSEMBLYPROPERTY(assemblies.name, 'VersionMajor') as version_major, ASSEMBLYPROPERTY(assemblies.name, 'VersionMinor') as version_minor, ASSEMBLYPROPERTY(assemblies.name, 'VersionBuild') as version_build, ASSEMBLYPROPERTY(assemblies.name, 'VersionRevision') as version_revision, ASSEMBLYPROPERTY(assemblies.name, 'CultureInfo') as culture_info, ASSEMBLYPROPERTY(assemblies.name, 'PublicKey') as public_key, is_fixed_length, max_length, Create_Date, Permission_set_desc from sys.assemblies as assemblies join sys.assembly_types as types on assemblies.assembly_id = types.assembly_id where (assemblies.name = @AssemblyName or (@AssemblyName is null)) and (types.assembly_class = @UDTName or (@UDTName is null))";
				AddParameter(dbCommand, "@AssemblyName", DbType.StringFixedLength, 4000);
				AddParameter(dbCommand, "@UDTName", DbType.StringFixedLength, 4000);
				break;
			case "MetaDataCollections":
				return MetaDataCollections.Instance;
			case "DataSourceInformation":
				throw new NotImplementedException();
			case "DataTypes":
				return DataTypes.Instance;
			case "ReservedWords":
				return ReservedWords.Instance;
			case "Restrictions":
				return Restrictions.Instance;
			}
			for (int i = 0; i < num; i++)
			{
				dbCommand.Parameters[i].Value = restrictionValues[i];
			}
			DbDataAdapter dbDataAdapter = DbProviderFactory.CreateDataAdapter();
			dbDataAdapter.SelectCommand = dbCommand;
			dbDataAdapter.Fill(dataTable);
			return dataTable;
		}

		public abstract void Open();

		protected virtual void OnStateChange(StateChangeEventArgs stateChanged)
		{
			if (this.StateChange != null)
			{
				this.StateChange(this, stateChanged);
			}
		}
	}
}
