using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	[DefaultEvent("InfoMessage")]
	public sealed class SqlConnection : DbConnection, IDisposable, ICloneable, IDbConnection
	{
		private sealed class SqlMonitorSocket : UdpClient
		{
			private static readonly int SqlMonitorUdpPort = 1434;

			private string server;

			private string instance;

			internal SqlMonitorSocket(string ServerName, string InstanceName)
				: base(ServerName, SqlMonitorUdpPort)
			{
				server = ServerName;
				instance = InstanceName;
			}

			internal int DiscoverTcpPort(int timeoutSeconds)
			{
				base.Client.Blocking = false;
				ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
				byte[] array = new byte[instance.Length + 1];
				array[0] = 4;
				aSCIIEncoding.GetBytes(instance, 0, instance.Length, array, 1);
				Send(array, array.Length);
				if (!base.Active)
				{
					return -1;
				}
				long num = timeoutSeconds * 1000000;
				if (!base.Client.Poll((int)num, SelectMode.SelectRead))
				{
					return -1;
				}
				if (base.Client.Available <= 0)
				{
					return -1;
				}
				IPEndPoint remoteEP = new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[0], 0);
				byte[] bytes = Receive(ref remoteEP);
				string text = Encoding.ASCII.GetString(bytes);
				string[] array2 = text.Split(';');
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < array2.Length / 2 && i < 256; i++)
				{
					hashtable[array2[i * 2]] = array2[i * 2 + 1];
				}
				if (!hashtable.ContainsKey("tcp"))
				{
					string message = "Mono does not support names pipes or shared memory for connecting to SQL Server. Please enable the TCP/IP protocol.";
					throw new NotImplementedException(message);
				}
				int result = int.Parse((string)hashtable["tcp"]);
				Close();
				return result;
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

		private new static class MetaDataCollections
		{
			private static readonly ColumnInfo[] columns = new ColumnInfo[3]
			{
				new ColumnInfo("CollectionName", typeof(string)),
				new ColumnInfo("NumberOfRestrictions", typeof(int)),
				new ColumnInfo("NumberOfIdentifierParts", typeof(int))
			};

			private static readonly object[][] rows = new object[18][]
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
				new object[3] { "StructuredTypeMembers", 4, 4 },
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
						instance = new DataTable("MetaDataCollections");
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

		private static class DataSourceInformation
		{
			private static readonly ColumnInfo[] columns = new ColumnInfo[17]
			{
				new ColumnInfo("CompositeIdentifierSeparatorPattern", typeof(string)),
				new ColumnInfo("DataSourceProductName", typeof(string)),
				new ColumnInfo("DataSourceProductVersion", typeof(string)),
				new ColumnInfo("DataSourceProductVersionNormalized", typeof(string)),
				new ColumnInfo("GroupByBehavior", typeof(GroupByBehavior)),
				new ColumnInfo("IdentifierPattern", typeof(string)),
				new ColumnInfo("IdentifierCase", typeof(IdentifierCase)),
				new ColumnInfo("OrderByColumnsInSelect", typeof(bool)),
				new ColumnInfo("ParameterMarkerFormat", typeof(string)),
				new ColumnInfo("ParameterMarkerPattern", typeof(string)),
				new ColumnInfo("ParameterNameMaxLength", typeof(int)),
				new ColumnInfo("ParameterNamePattern", typeof(string)),
				new ColumnInfo("QuotedIdentifierPattern", typeof(string)),
				new ColumnInfo("QuotedIdentifierCase", typeof(IdentifierCase)),
				new ColumnInfo("StatementSeparatorPattern", typeof(string)),
				new ColumnInfo("StringLiteralPattern", typeof(string)),
				new ColumnInfo("SupportedJoinOperators", typeof(SupportedJoinOperators))
			};

			public static DataTable GetInstance(SqlConnection conn)
			{
				DataTable dataTable = new DataTable("DataSourceInformation");
				ColumnInfo[] array = columns;
				for (int i = 0; i < array.Length; i++)
				{
					ColumnInfo columnInfo = array[i];
					dataTable.Columns.Add(columnInfo.name, columnInfo.type);
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = "\\.";
				dataRow[1] = "Microsoft SQL Server";
				dataRow[2] = conn.ServerVersion;
				dataRow[3] = conn.ServerVersion;
				dataRow[4] = GroupByBehavior.Unrelated;
				dataRow[5] = "(^\\[\\p{Lo}\\p{Lu}\\p{Ll}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Nd}@$#_]*$)|(^\\[[^\\]\\0]|\\]\\]+\\]$)|(^\\\"[^\\\"\\0]|\\\"\\\"+\\\"$)";
				dataRow[6] = IdentifierCase.Insensitive;
				dataRow[7] = false;
				dataRow[8] = "{0}";
				dataRow[9] = "@[\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}\\p{Nd}\\uff3f_@#\\$]*(?=\\s+|$)";
				dataRow[10] = 128;
				dataRow[11] = "^[\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}\\p{Nd}\\uff3f_@#\\$]*(?=\\s+|$)";
				dataRow[12] = "(([^\\[]|\\]\\])*)";
				dataRow[13] = IdentifierCase.Insensitive;
				dataRow[14] = ";";
				dataRow[15] = "'(([^']|'')*)'";
				dataRow[16] = SupportedJoinOperators.Inner | SupportedJoinOperators.LeftOuter | SupportedJoinOperators.RightOuter | SupportedJoinOperators.FullOuter;
				dataTable.Rows.Add(dataRow);
				return dataTable;
			}
		}

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

			private static readonly object[][] rows = new object[30][]
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
				},
				new object[22]
				{
					"date",
					31,
					3L,
					"date",
					DBNull.Value,
					"System.DateTime",
					false,
					false,
					false,
					true,
					true,
					false,
					true,
					true,
					true,
					DBNull.Value,
					DBNull.Value,
					DBNull.Value,
					false,
					DBNull.Value,
					"{ts '",
					"'}"
				},
				new object[22]
				{
					"time",
					32,
					5L,
					"time({0})",
					"scale",
					"System.TimeSpan",
					false,
					false,
					false,
					false,
					false,
					false,
					true,
					true,
					true,
					DBNull.Value,
					(short)7,
					(short)0,
					false,
					DBNull.Value,
					"{ts '",
					"'}"
				},
				new object[22]
				{
					"datetime2",
					33,
					8L,
					"datetime2({0})",
					"scale",
					"System.DateTime",
					false,
					true,
					false,
					false,
					false,
					false,
					true,
					true,
					true,
					DBNull.Value,
					(short)7,
					(short)0,
					false,
					DBNull.Value,
					"{ts '",
					"'}"
				},
				new object[22]
				{
					"datetimeoffset",
					34,
					10L,
					"datetimeoffset({0})",
					"scale",
					"System.DateTimeOffset",
					false,
					true,
					false,
					false,
					false,
					false,
					true,
					true,
					true,
					DBNull.Value,
					(short)7,
					(short)0,
					false,
					DBNull.Value,
					"{ts '",
					"'}"
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

			private static readonly object[][] rows = new object[44][]
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
				new object[5] { "StructuredTypeMembers", "Catalog", "@Catalog", "TYPE_CATALOG", 1 },
				new object[5] { "StructuredTypeMembers", "Owner", "@Owner", "TYPE_SCHEMA", 2 },
				new object[5] { "StructuredTypeMembers", "Type", "@Type", "TYPE_NAME", 3 },
				new object[5] { "StructuredTypeMembers", "Member", "@Member", "MEMBER_NAME", 4 },
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
				new object[5] { "IndexColumns", "Catalog", "@Catalog", "db_name()", 1 },
				new object[5] { "IndexColumns", "Owner", "@Owner", "user_name()", 2 },
				new object[5] { "IndexColumns", "Table", "@Table", "o.name", 3 },
				new object[5] { "IndexColumns", "ConstraintName", "@ConstraintName", "x.name", 4 },
				new object[5] { "IndexColumns", "Column", "@Column", "c.name", 5 },
				new object[5] { "Indexes", "Catalog", "@Catalog", "db_name()", 1 },
				new object[5] { "Indexes", "Owner", "@Owner", "user_name()", 2 },
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

		private const int DEFAULT_PACKETSIZE = 8000;

		private const int MAX_PACKETSIZE = 32768;

		private const int MIN_PACKETSIZE = 512;

		private const int DEFAULT_CONNECTIONTIMEOUT = 15;

		private const int DEFAULT_MAXPOOLSIZE = 100;

		private const int MIN_MAXPOOLSIZE = 1;

		private const int DEFAULT_MINPOOLSIZE = 0;

		private const int DEFAULT_PORT = 1433;

		private bool disposed;

		private static TdsConnectionPoolManager sqlConnectionPools = new TdsConnectionPoolManager(TdsVersion.tds80);

		private TdsConnectionPool pool;

		private string connectionString;

		private SqlTransaction transaction;

		private TdsConnectionParameters parms;

		private bool connectionReset;

		private bool pooling;

		private string dataSource;

		private int connectionTimeout;

		private int minPoolSize;

		private int maxPoolSize;

		private int packetSize;

		private int port;

		private bool fireInfoMessageEventOnUserErrors;

		private bool statisticsEnabled;

		private ConnectionState state;

		private SqlDataReader dataReader;

		private XmlReader xmlReader;

		private Tds tds;

		private bool async;

		private bool userInstance;

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.SQL.Design.SqlConnectionStringEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RecommendedAsConfigurable(true)]
		public override string ConnectionString
		{
			get
			{
				if (connectionString == null)
				{
					return string.Empty;
				}
				return connectionString;
			}
			[System.MonoTODO("persist security info, encrypt, enlist keyword not implemented")]
			set
			{
				if (state == ConnectionState.Open)
				{
					throw new InvalidOperationException("Not Allowed to change ConnectionString property while Connection state is OPEN");
				}
				SetConnectionString(value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ConnectionTimeout
		{
			get
			{
				return connectionTimeout;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Database
		{
			get
			{
				if (State == ConnectionState.Open)
				{
					return tds.Database;
				}
				return parms.Database;
			}
		}

		internal SqlDataReader DataReader
		{
			get
			{
				return dataReader;
			}
			set
			{
				dataReader = value;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DataSource
		{
			get
			{
				return dataSource;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PacketSize
		{
			get
			{
				if (State == ConnectionState.Open)
				{
					return tds.PacketSize;
				}
				return packetSize;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override string ServerVersion
		{
			get
			{
				if (state == ConnectionState.Closed)
				{
					throw ExceptionHelper.ConnectionClosed();
				}
				return tds.ServerVersion;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override ConnectionState State
		{
			get
			{
				return state;
			}
		}

		internal Tds Tds
		{
			get
			{
				return tds;
			}
		}

		internal SqlTransaction Transaction
		{
			get
			{
				return transaction;
			}
			set
			{
				transaction = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string WorkstationId
		{
			get
			{
				return parms.Hostname;
			}
		}

		internal XmlReader XmlReader
		{
			get
			{
				return xmlReader;
			}
			set
			{
				xmlReader = value;
			}
		}

		public bool FireInfoMessageEventOnUserErrors
		{
			get
			{
				return fireInfoMessageEventOnUserErrors;
			}
			set
			{
				fireInfoMessageEventOnUserErrors = value;
			}
		}

		[DefaultValue(false)]
		public bool StatisticsEnabled
		{
			get
			{
				return statisticsEnabled;
			}
			set
			{
				statisticsEnabled = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool AsyncProcessing
		{
			get
			{
				return async;
			}
		}

		public event SqlInfoMessageEventHandler InfoMessage;

		public SqlConnection()
			: this(null)
		{
		}

		public SqlConnection(string connectionString)
		{
			ConnectionString = connectionString;
		}

		object ICloneable.Clone()
		{
			return new SqlConnection(ConnectionString);
		}

		private void ErrorHandler(object sender, TdsInternalErrorMessageEventArgs e)
		{
			try
			{
				if (!tds.IsConnected)
				{
					Close();
				}
			}
			catch
			{
				try
				{
					Close();
				}
				catch
				{
				}
			}
			throw new SqlException(e.Class, e.LineNumber, e.Message, e.Number, e.Procedure, e.Server, "Mono SqlClient Data Provider", e.State);
		}

		private void MessageHandler(object sender, TdsInternalInfoMessageEventArgs e)
		{
			OnSqlInfoMessage(CreateSqlInfoMessageEvent(e.Errors));
		}

		public new SqlTransaction BeginTransaction()
		{
			return BeginTransaction(IsolationLevel.ReadCommitted, string.Empty);
		}

		public new SqlTransaction BeginTransaction(IsolationLevel iso)
		{
			return BeginTransaction(iso, string.Empty);
		}

		public SqlTransaction BeginTransaction(string transactionName)
		{
			return BeginTransaction(IsolationLevel.ReadCommitted, transactionName);
		}

		public SqlTransaction BeginTransaction(IsolationLevel iso, string transactionName)
		{
			if (state == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			if (transaction != null)
			{
				throw new InvalidOperationException("SqlConnection does not support parallel transactions.");
			}
			string empty = string.Empty;
			switch (iso)
			{
			case IsolationLevel.ReadUncommitted:
				empty = "READ UNCOMMITTED";
				break;
			case IsolationLevel.RepeatableRead:
				empty = "REPEATABLE READ";
				break;
			case IsolationLevel.Serializable:
				empty = "SERIALIZABLE";
				break;
			case IsolationLevel.ReadCommitted:
				empty = "READ COMMITTED";
				break;
			case IsolationLevel.Snapshot:
				empty = "SNAPSHOT";
				break;
			case IsolationLevel.Unspecified:
				iso = IsolationLevel.ReadCommitted;
				empty = "READ COMMITTED";
				break;
			case IsolationLevel.Chaos:
				throw new ArgumentOutOfRangeException("IsolationLevel", string.Format(CultureInfo.CurrentCulture, "The IsolationLevel enumeration value, {0}, is not supported by the .Net Framework SqlClient Data Provider.", (int)iso));
			default:
				throw new ArgumentOutOfRangeException("IsolationLevel", string.Format(CultureInfo.CurrentCulture, "The IsolationLevel enumeration value, {0}, is invalid.", (int)iso));
			}
			tds.Execute(string.Format("SET TRANSACTION ISOLATION LEVEL {0};BEGIN TRANSACTION {1}", empty, transactionName));
			transaction = new SqlTransaction(this, iso);
			return transaction;
		}

		public override void ChangeDatabase(string database)
		{
			if (!IsValidDatabaseName(database))
			{
				throw new ArgumentException(string.Format("The database name {0} is not valid.", database));
			}
			if (state != ConnectionState.Open)
			{
				throw new InvalidOperationException("The connection is not open.");
			}
			tds.Execute(string.Format("use [{0}]", database));
		}

		private void ChangeState(ConnectionState currentState)
		{
			if (currentState != state)
			{
				ConnectionState originalState = state;
				state = currentState;
				OnStateChange(CreateStateChangeEvent(originalState, currentState));
			}
		}

		public override void Close()
		{
			if (transaction != null && transaction.IsOpen)
			{
				transaction.Rollback();
			}
			if (dataReader != null || xmlReader != null)
			{
				if (tds != null)
				{
					tds.SkipToEnd();
				}
				dataReader = null;
				xmlReader = null;
			}
			if (tds != null && tds.IsConnected)
			{
				if (pooling && tds.Pooling)
				{
					if (pool != null)
					{
						pool.ReleaseConnection(tds);
						pool = null;
					}
				}
				else
				{
					tds.Disconnect();
				}
			}
			if (tds != null)
			{
				tds.TdsErrorMessage -= ErrorHandler;
				tds.TdsInfoMessage -= MessageHandler;
			}
			ChangeState(ConnectionState.Closed);
		}

		public new SqlCommand CreateCommand()
		{
			SqlCommand sqlCommand = new SqlCommand();
			sqlCommand.Connection = this;
			return sqlCommand;
		}

		private SqlInfoMessageEventArgs CreateSqlInfoMessageEvent(TdsInternalErrorCollection errors)
		{
			return new SqlInfoMessageEventArgs(errors);
		}

		private StateChangeEventArgs CreateStateChangeEvent(ConnectionState originalState, ConnectionState currentState)
		{
			return new StateChangeEventArgs(originalState, currentState);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && !disposed)
				{
					if (State == ConnectionState.Open)
					{
						Close();
					}
					ConnectionString = null;
				}
			}
			finally
			{
				disposed = true;
				base.Dispose(disposing);
			}
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return BeginTransaction(isolationLevel);
		}

		protected override DbCommand CreateDbCommand()
		{
			return CreateCommand();
		}

		public override void Open()
		{
			string theServerName = string.Empty;
			if (state == ConnectionState.Open)
			{
				throw new InvalidOperationException("The Connection is already Open (State=Open)");
			}
			if (connectionString == null || connectionString.Trim().Length == 0)
			{
				throw new InvalidOperationException("Connection string has not been initialized.");
			}
			try
			{
				if (!pooling)
				{
					if (!ParseDataSource(dataSource, out port, out theServerName))
					{
						throw new SqlException(20, 0, "SQL Server does not exist or access denied.", 17, "ConnectionOpen (Connect()).", dataSource, parms.ApplicationName, 0);
					}
					tds = new Tds80(theServerName, port, PacketSize, ConnectionTimeout);
					tds.Pooling = false;
				}
				else
				{
					if (!ParseDataSource(dataSource, out port, out theServerName))
					{
						throw new SqlException(20, 0, "SQL Server does not exist or access denied.", 17, "ConnectionOpen (Connect()).", dataSource, parms.ApplicationName, 0);
					}
					TdsConnectionInfo info = new TdsConnectionInfo(theServerName, port, packetSize, ConnectionTimeout, minPoolSize, maxPoolSize);
					pool = sqlConnectionPools.GetConnectionPool(connectionString, info);
					tds = pool.GetConnection();
				}
			}
			catch (TdsTimeoutException e)
			{
				throw SqlException.FromTdsInternalException(e);
			}
			catch (TdsInternalException e2)
			{
				throw SqlException.FromTdsInternalException(e2);
			}
			tds.TdsErrorMessage += ErrorHandler;
			tds.TdsInfoMessage += MessageHandler;
			if (!tds.IsConnected)
			{
				try
				{
					tds.Connect(parms);
				}
				catch
				{
					if (pooling)
					{
						pool.ReleaseConnection(tds);
					}
					throw;
				}
			}
			disposed = false;
			ChangeState(ConnectionState.Open);
		}

		private bool ParseDataSource(string theDataSource, out int thePort, out string theServerName)
		{
			theServerName = string.Empty;
			string empty = string.Empty;
			if (theDataSource == null)
			{
				throw new ArgumentException("Format of initialization string does not conform to specifications");
			}
			thePort = 1433;
			bool result = true;
			int num = 0;
			if ((num = theDataSource.IndexOf(',')) > -1)
			{
				theServerName = theDataSource.Substring(0, num);
				string s = theDataSource.Substring(num + 1);
				thePort = int.Parse(s);
			}
			else if ((num = theDataSource.IndexOf('\\')) > -1)
			{
				theServerName = theDataSource.Substring(0, num);
				empty = theDataSource.Substring(num + 1);
				port = DiscoverTcpPortViaSqlMonitor(theServerName, empty);
				if (port == -1)
				{
					result = false;
				}
			}
			else
			{
				theServerName = theDataSource;
			}
			if (theServerName.Length == 0 || theServerName == "(local)" || theServerName == ".")
			{
				theServerName = "localhost";
			}
			if ((num = theServerName.IndexOf("tcp:")) > -1)
			{
				theServerName = theServerName.Substring(num + 4);
			}
			return result;
		}

		private bool ConvertIntegratedSecurity(string value)
		{
			if (value.ToUpper() == "SSPI")
			{
				return true;
			}
			return ConvertToBoolean("integrated security", value, false);
		}

		private bool ConvertToBoolean(string key, string value, bool defaultValue)
		{
			if (value.Length == 0)
			{
				return defaultValue;
			}
			switch (value.ToUpper())
			{
			case "TRUE":
			case "YES":
				return true;
			case "FALSE":
			case "NO":
				return false;
			default:
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value \"{0}\" for key '{1}'.", value, key));
			}
		}

		private int ConvertToInt32(string key, string value, int defaultValue)
		{
			if (value.Length == 0)
			{
				return defaultValue;
			}
			try
			{
				return int.Parse(value);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value \"{0}\" for key '{1}'.", value, key), innerException);
			}
		}

		private int DiscoverTcpPortViaSqlMonitor(string ServerName, string InstanceName)
		{
			SqlMonitorSocket sqlMonitorSocket = new SqlMonitorSocket(ServerName, InstanceName);
			int result = sqlMonitorSocket.DiscoverTcpPort(ConnectionTimeout);
			sqlMonitorSocket = null;
			return result;
		}

		private void SetConnectionString(string connectionString)
		{
			SetDefaultConnectionParameters();
			if (connectionString == null || connectionString.Trim().Length == 0)
			{
				this.connectionString = connectionString;
				return;
			}
			connectionString += ";";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			string text = string.Empty;
			string empty = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < connectionString.Length; i++)
			{
				char c = connectionString[i];
				char c2 = ((i != connectionString.Length - 1) ? connectionString[i + 1] : '\0');
				switch (c)
				{
				case '\'':
					if (flag2)
					{
						stringBuilder.Append(c);
					}
					else if (c2.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						flag = !flag;
					}
					break;
				case '"':
					if (flag)
					{
						stringBuilder.Append(c);
					}
					else if (c2.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						flag2 = !flag2;
					}
					break;
				case ';':
					if (flag2 || flag)
					{
						stringBuilder.Append(c);
						break;
					}
					if (text != string.Empty && text != null)
					{
						empty = stringBuilder.ToString();
						SetProperties(text.ToLower().Trim(), empty);
					}
					else if (stringBuilder.Length != 0)
					{
						throw new ArgumentException("Format of initialization string does not conform to specifications");
					}
					flag3 = true;
					text = string.Empty;
					empty = string.Empty;
					stringBuilder = new StringBuilder();
					break;
				case '=':
					if (flag2 || flag || !flag3)
					{
						stringBuilder.Append(c);
					}
					else if (c2.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						text = stringBuilder.ToString();
						stringBuilder = new StringBuilder();
						flag3 = false;
					}
					break;
				case ' ':
					if (flag || flag2)
					{
						stringBuilder.Append(c);
					}
					else if (stringBuilder.Length > 0 && !c2.Equals(';'))
					{
						stringBuilder.Append(c);
					}
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
			if (minPoolSize > maxPoolSize)
			{
				throw new ArgumentException("Invalid value for 'min pool size' or 'max pool size'; 'min pool size' must not be greater than 'max pool size'.");
			}
			connectionString = connectionString.Substring(0, connectionString.Length - 1);
			this.connectionString = connectionString;
		}

		private void SetDefaultConnectionParameters()
		{
			if (parms == null)
			{
				parms = new TdsConnectionParameters();
			}
			else
			{
				parms.Reset();
			}
			dataSource = string.Empty;
			connectionTimeout = 15;
			connectionReset = true;
			pooling = true;
			maxPoolSize = 100;
			minPoolSize = 0;
			packetSize = 8000;
			port = 1433;
			async = false;
		}

		private void SetProperties(string name, string value)
		{
			switch (name)
			{
			case "app":
			case "application name":
				parms.ApplicationName = value;
				break;
			case "attachdbfilename":
			case "extended properties":
			case "initial file name":
				parms.AttachDBFileName = value;
				break;
			case "timeout":
			case "connect timeout":
			case "connection timeout":
			{
				int num4 = ConvertToInt32("connect timeout", value, 15);
				if (num4 < 0)
				{
					throw new ArgumentException("Invalid 'connect timeout'. Must be an integer >=0 ");
				}
				connectionTimeout = num4;
				break;
			}
			case "connection lifetime":
				break;
			case "connection reset":
				connectionReset = ConvertToBoolean("connection reset", value, true);
				break;
			case "language":
			case "current language":
				parms.Language = value;
				break;
			case "data source":
			case "server":
			case "address":
			case "addr":
			case "network address":
				dataSource = value;
				break;
			case "encrypt":
				if (ConvertToBoolean(name, value, false))
				{
					throw new NotImplementedException("SSL encryption for data sent between client and server is not implemented.");
				}
				break;
			case "enlist":
				if (!ConvertToBoolean(name, value, true))
				{
					throw new NotImplementedException("Disabling the automatic enlistment of connections in the thread's current transaction context is not implemented.");
				}
				break;
			case "initial catalog":
			case "database":
				parms.Database = value;
				break;
			case "integrated security":
			case "trusted_connection":
				parms.DomainLogin = ConvertIntegratedSecurity(value);
				break;
			case "max pool size":
			{
				int num3 = ConvertToInt32(name, value, 100);
				if (num3 < 1)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid '{0}'. The value must be greater than {1}.", name, 1));
				}
				maxPoolSize = num3;
				break;
			}
			case "min pool size":
			{
				int num2 = ConvertToInt32(name, value, 0);
				if (num2 < 0)
				{
					throw new ArgumentException("Invalid 'min pool size'. Must be a integer >= 0");
				}
				minPoolSize = num2;
				break;
			}
			case "multipleactiveresultsets":
				ConvertToBoolean(name, value, false);
				break;
			case "asynchronous processing":
			case "async":
				async = ConvertToBoolean(name, value, false);
				break;
			case "net":
			case "network":
			case "network library":
				if (!value.ToUpper().Equals("DBMSSOCN"))
				{
					throw new ArgumentException("Unsupported network library.");
				}
				break;
			case "packet size":
			{
				int num = ConvertToInt32(name, value, 8000);
				if (num < 512 || num > 32768)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid 'Packet Size'. The value must be between {0} and {1}.", 512, 32768));
				}
				packetSize = num;
				break;
			}
			case "password":
			case "pwd":
				parms.Password = value;
				break;
			case "persistsecurityinfo":
			case "persist security info":
				break;
			case "pooling":
				pooling = ConvertToBoolean(name, value, true);
				break;
			case "uid":
			case "user":
			case "user id":
				parms.User = value;
				break;
			case "wsid":
			case "workstation id":
				parms.Hostname = value;
				break;
			case "user instance":
				userInstance = ConvertToBoolean(name, value, false);
				break;
			default:
				throw new ArgumentException("Keyword not supported : '" + name + "'.");
			}
		}

		private static bool IsValidDatabaseName(string database)
		{
			if (database == null || database.Trim().Length == 0 || database.Length > 128)
			{
				return false;
			}
			if (database[0] == '"' && database[database.Length] == '"')
			{
				database = database.Substring(1, database.Length - 2);
			}
			else if (char.IsDigit(database[0]))
			{
				return false;
			}
			if (database[0] == '_')
			{
				return false;
			}
			string text = database.Substring(1, database.Length - 1);
			foreach (char c in text)
			{
				if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
				{
					return false;
				}
			}
			return true;
		}

		private void OnSqlInfoMessage(SqlInfoMessageEventArgs value)
		{
			if (this.InfoMessage != null)
			{
				this.InfoMessage(this, value);
			}
		}

		public override DataTable GetSchema()
		{
			if (state == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			return MetaDataCollections.Instance;
		}

		public override DataTable GetSchema(string collectionName)
		{
			return GetSchema(collectionName, null);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			if (state == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
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
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The requested collection ({0}) is not defined.", collectionName));
			}
			SqlCommand sqlCommand = null;
			DataTable dataTable = new DataTable();
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
			switch (text)
			{
			case "Databases":
				sqlCommand = new SqlCommand("select name as database_name, dbid, crdate as create_date from master.sys.sysdatabases where (name = @Name or (@Name is null))", this);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				break;
			case "ForeignKeys":
				sqlCommand = new SqlCommand("select CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CONSTRAINT_TYPE, IS_DEFERRABLE, INITIALLY_DEFERRED from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where (CONSTRAINT_CATALOG = @Catalog or (@Catalog is null)) and (CONSTRAINT_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @Table or (@Table is null)) and (CONSTRAINT_NAME = @Name or (@Name is null)) and CONSTRAINT_TYPE = 'FOREIGN KEY' order by CONSTRAINT_CATALOG, CONSTRAINT_SCHEMA, CONSTRAINT_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				break;
			case "Indexes":
				sqlCommand = new SqlCommand("select distinct db_name() as constraint_catalog, constraint_schema = user_name (o.uid), constraint_name = x.name, table_catalog = db_name (), table_schema = user_name (o.uid), table_name = o.name, index_name  = x.name from sysobjects o, sysindexes x, sysindexkeys xk where o.type in ('U') and x.id = o.id and o.id = xk.id and x.indid = xk.indid and xk.keyno = x.keycnt and (db_name() = @Catalog or (@Catalog is null)) and (user_name() = @Owner or (@Owner is null)) and (o.name = @Table or (@Table is null)) and (x.name = @Name or (@Name is null))order by table_name, index_name", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				break;
			case "IndexColumns":
				sqlCommand = new SqlCommand("select distinct db_name() as constraint_catalog, constraint_schema = user_name (o.uid), constraint_name = x.name, table_catalog = db_name (), table_schema = user_name (o.uid), table_name = o.name, column_name = c.name, ordinal_position = convert (int, xk.keyno), keyType = c.xtype, index_name = x.name from sysobjects o, sysindexes x, syscolumns c, sysindexkeys xk where o.type in ('U') and x.id = o.id and o.id = c.id and o.id = xk.id and x.indid = xk.indid and c.colid = xk.colid and xk.keyno <= x.keycnt and permissions (o.id, c.name) <> 0 and (db_name() = @Catalog or (@Catalog is null)) and (user_name() = @Owner or (@Owner is null)) and (o.name = @Table or (@Table is null)) and (x.name = @ConstraintName or (@ConstraintName is null)) and (c.name = @Column or (@Column is null)) order by table_name, index_name", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 8);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 13);
				sqlCommand.Parameters.Add("@ConstraintName", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Column", SqlDbType.NVarChar, 4000);
				break;
			case "Procedures":
				sqlCommand = new SqlCommand("select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ROUTINE_CATALOG, ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_TYPE, CREATED, LAST_ALTERED from INFORMATION_SCHEMA.ROUTINES where (SPECIFIC_CATALOG = @Catalog or (@Catalog is null)) and (SPECIFIC_SCHEMA = @Owner or (@Owner is null)) and (SPECIFIC_NAME = @Name or (@Name is null)) and (ROUTINE_TYPE = @Type or (@Type is null)) order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Type", SqlDbType.NVarChar, 4000);
				break;
			case "ProcedureParameters":
				sqlCommand = new SqlCommand("select SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION, PARAMETER_MODE, IS_RESULT, AS_LOCATOR, PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, COLLATION_CATALOG, COLLATION_SCHEMA, COLLATION_NAME, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, INTERVAL_TYPE, INTERVAL_PRECISION from INFORMATION_SCHEMA.PARAMETERS where (SPECIFIC_CATALOG = @Catalog or (@Catalog is null)) and (SPECIFIC_SCHEMA = @Owner or (@Owner is null)) and (SPECIFIC_NAME = @Name or (@Name is null)) and (PARAMETER_NAME = @Parameter or (@Parameter is null)) order by SPECIFIC_CATALOG, SPECIFIC_SCHEMA, SPECIFIC_NAME, PARAMETER_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Parameter", SqlDbType.NVarChar, 4000);
				break;
			case "Tables":
				sqlCommand = new SqlCommand("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE from INFORMATION_SCHEMA.TABLES where (TABLE_CATALOG = @catalog or (@catalog is null)) and (TABLE_SCHEMA = @owner or (@owner is null))and (TABLE_NAME = @name or (@name is null)) and (TABLE_TYPE = @table_type or (@table_type is null))", this);
				sqlCommand.Parameters.Add("@catalog", SqlDbType.NVarChar, 8);
				sqlCommand.Parameters.Add("@owner", SqlDbType.NVarChar, 3);
				sqlCommand.Parameters.Add("@name", SqlDbType.NVarChar, 11);
				sqlCommand.Parameters.Add("@table_type", SqlDbType.NVarChar, 10);
				break;
			case "Columns":
				sqlCommand = new SqlCommand("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION, NUMERIC_PRECISION_RADIX, NUMERIC_SCALE, DATETIME_PRECISION, CHARACTER_SET_CATALOG, CHARACTER_SET_SCHEMA, CHARACTER_SET_NAME, COLLATION_CATALOG from INFORMATION_SCHEMA.COLUMNS where (TABLE_CATALOG = @Catalog or (@Catalog is null)) and (TABLE_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @table or (@Table is null)) and (COLUMN_NAME = @column or (@Column is null)) order by TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Column", SqlDbType.NVarChar, 4000);
				break;
			case "Users":
				sqlCommand = new SqlCommand("select uid, name as user_name, createdate, updatedate from sysusers where (name = @Name or (@Name is null))", this);
				sqlCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 4000);
				break;
			case "StructuredTypeMembers":
				throw new NotImplementedException();
			case "Views":
				sqlCommand = new SqlCommand("select TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, CHECK_OPTION, IS_UPDATABLE from INFORMATION_SCHEMA.VIEWS where (TABLE_CATALOG = @Catalog or (@Catalog is null)) TABLE_SCHEMA = @Owner or (@Owner is null)) and (TABLE_NAME = @table or (@Table is null)) order by TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				break;
			case "ViewColumns":
				sqlCommand = new SqlCommand("select VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME, TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME from INFORMATION_SCHEMA.VIEW_COLUMN_USAGE where (VIEW_CATALOG = @Catalog (@Catalog is null)) and (VIEW_SCHEMA = @Owner (@Owner is null)) and (VIEW_NAME = @Table or (@Table is null)) and (COLUMN_NAME = @Column or (@Column is null)) order by VIEW_CATALOG, VIEW_SCHEMA, VIEW_NAME", this);
				sqlCommand.Parameters.Add("@Catalog", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Owner", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Table", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@Column", SqlDbType.NVarChar, 4000);
				break;
			case "UserDefinedTypes":
				sqlCommand = new SqlCommand("select assemblies.name as assembly_name, types.assembly_class as udt_name, ASSEMBLYPROPERTY(assemblies.name, 'VersionMajor') as version_major, ASSEMBLYPROPERTY(assemblies.name, 'VersionMinor') as version_minor, ASSEMBLYPROPERTY(assemblies.name, 'VersionBuild') as version_build, ASSEMBLYPROPERTY(assemblies.name, 'VersionRevision') as version_revision, ASSEMBLYPROPERTY(assemblies.name, 'CultureInfo') as culture_info, ASSEMBLYPROPERTY(assemblies.name, 'PublicKey') as public_key, is_fixed_length, max_length, Create_Date, Permission_set_desc from sys.assemblies as assemblies join sys.assembly_types as types on assemblies.assembly_id = types.assembly_id where (assportemblies.name = @AssemblyName or (@AssemblyName is null)) and (types.assembly_class = @UDTName or (@UDTName is null))", this);
				sqlCommand.Parameters.Add("@AssemblyName", SqlDbType.NVarChar, 4000);
				sqlCommand.Parameters.Add("@UDTName", SqlDbType.NVarChar, 4000);
				break;
			case "MetaDataCollections":
				return MetaDataCollections.Instance;
			case "DataSourceInformation":
				return DataSourceInformation.GetInstance(this);
			case "DataTypes":
				return DataTypes.Instance;
			case "ReservedWords":
				return ReservedWords.Instance;
			case "Restrictions":
				return Restrictions.Instance;
			}
			for (int i = 0; i < num; i++)
			{
				sqlCommand.Parameters[i].Value = restrictionValues[i];
			}
			sqlDataAdapter.SelectCommand = sqlCommand;
			sqlDataAdapter.Fill(dataTable);
			return dataTable;
		}

		public static void ChangePassword(string connectionString, string newPassword)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("The 'connectionString' cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(newPassword))
			{
				throw new ArgumentNullException("The 'newPassword' cannot be null or empty.");
			}
			if (newPassword.Length > 128)
			{
				throw new ArgumentException("The length of 'newPassword' cannot exceed 128 characters.");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				sqlConnection.tds.Execute(string.Format("sp_password '{0}', '{1}', '{2}'", sqlConnection.parms.Password, newPassword, sqlConnection.parms.User));
			}
		}

		public static void ClearAllPools()
		{
			IDictionary connectionPool = sqlConnectionPools.GetConnectionPool();
			foreach (TdsConnectionPool value in connectionPool.Values)
			{
				if (value != null)
				{
					value.ResetConnectionPool();
				}
			}
			connectionPool.Clear();
		}

		public static void ClearPool(SqlConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (connection.pooling)
			{
				TdsConnectionPool connectionPool = sqlConnectionPools.GetConnectionPool(connection.ConnectionString);
				if (connectionPool != null)
				{
					connectionPool.ResetConnectionPool();
				}
			}
		}
	}
}
