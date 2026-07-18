using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text;
using System.Transactions;

namespace Mono.Data.Sqlite
{
	public sealed class SqliteConnection : DbConnection, ICloneable
	{
		private const string _dataDirectory = "|DataDirectory|";

		private const string _masterdb = "sqlite_master";

		private const string _tempmasterdb = "sqlite_temp_master";

		private ConnectionState _connectionState;

		private string _connectionString;

		internal int _transactionLevel;

		private System.Data.IsolationLevel _defaultIsolation;

		internal SQLiteEnlistment _enlistment;

		internal SQLiteBase _sql;

		private string _dataSource;

		private byte[] _password;

		private int _defaultTimeout = 30;

		internal bool _binaryGuid;

		internal long _version;

		private SQLiteUpdateCallback _updateCallback;

		private SQLiteCommitCallback _commitCallback;

		private SQLiteRollbackCallback _rollbackCallback;

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		[Editor("SQLite.Designer.SqliteConnectionStringEditor, SQLite.Designer, Version=1.0.36.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public override string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (_connectionState != ConnectionState.Closed)
				{
					throw new InvalidOperationException();
				}
				_connectionString = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DataSource
		{
			get
			{
				return _dataSource;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Database
		{
			get
			{
				return "main";
			}
		}

		public int DefaultTimeout
		{
			get
			{
				return _defaultTimeout;
			}
			set
			{
				_defaultTimeout = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ServerVersion
		{
			get
			{
				if (_connectionState != ConnectionState.Open)
				{
					throw new InvalidOperationException();
				}
				return _sql.Version;
			}
		}

		public static string SQLiteVersion
		{
			get
			{
				return SQLite3.SQLiteVersion;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override ConnectionState State
		{
			get
			{
				return _connectionState;
			}
		}

		protected override DbProviderFactory DbProviderFactory
		{
			get
			{
				return SqliteFactory.Instance;
			}
		}

		private event SQLiteUpdateEventHandler _updateHandler;

		private event SQLiteCommitHandler _commitHandler;

		private event EventHandler _rollbackHandler;

		public override event StateChangeEventHandler StateChange;

		public event SQLiteUpdateEventHandler Update
		{
			add
			{
				if (this._updateHandler == null)
				{
					_updateCallback = UpdateCallback;
					if (_sql != null)
					{
						_sql.SetUpdateHook(_updateCallback);
					}
				}
				this._updateHandler = (SQLiteUpdateEventHandler)Delegate.Combine(this._updateHandler, value);
			}
			remove
			{
				this._updateHandler = (SQLiteUpdateEventHandler)Delegate.Remove(this._updateHandler, value);
				if (this._updateHandler == null)
				{
					if (_sql != null)
					{
						_sql.SetUpdateHook(null);
					}
					_updateCallback = null;
				}
			}
		}

		public event SQLiteCommitHandler Commit
		{
			add
			{
				if (this._commitHandler == null)
				{
					_commitCallback = CommitCallback;
					if (_sql != null)
					{
						_sql.SetCommitHook(_commitCallback);
					}
				}
				this._commitHandler = (SQLiteCommitHandler)Delegate.Combine(this._commitHandler, value);
			}
			remove
			{
				this._commitHandler = (SQLiteCommitHandler)Delegate.Remove(this._commitHandler, value);
				if (this._commitHandler == null)
				{
					if (_sql != null)
					{
						_sql.SetCommitHook(null);
					}
					_commitCallback = null;
				}
			}
		}

		public event EventHandler RollBack
		{
			add
			{
				if (this._rollbackHandler == null)
				{
					_rollbackCallback = RollbackCallback;
					if (_sql != null)
					{
						_sql.SetRollbackHook(_rollbackCallback);
					}
				}
				this._rollbackHandler = (EventHandler)Delegate.Combine(this._rollbackHandler, value);
			}
			remove
			{
				this._rollbackHandler = (EventHandler)Delegate.Remove(this._rollbackHandler, value);
				if (this._rollbackHandler == null)
				{
					if (_sql != null)
					{
						_sql.SetRollbackHook(null);
					}
					_rollbackCallback = null;
				}
			}
		}

		public SqliteConnection()
			: this(string.Empty)
		{
		}

		public SqliteConnection(string connectionString)
		{
			_sql = null;
			_connectionState = ConnectionState.Closed;
			_connectionString = string.Empty;
			_transactionLevel = 0;
			_version = 0L;
			if (connectionString != null)
			{
				ConnectionString = connectionString;
			}
		}

		public SqliteConnection(SqliteConnection connection)
			: this(connection.ConnectionString)
		{
			if (connection.State != ConnectionState.Open)
			{
				return;
			}
			Open();
			using (DataTable dataTable = connection.GetSchema("Catalogs"))
			{
				foreach (DataRow row in dataTable.Rows)
				{
					string strA = row[0].ToString();
					if (string.Compare(strA, "main", true, CultureInfo.InvariantCulture) != 0 && string.Compare(strA, "temp", true, CultureInfo.InvariantCulture) != 0)
					{
						using (SqliteCommand sqliteCommand = CreateCommand())
						{
							sqliteCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "ATTACH DATABASE '{0}' AS [{1}]", row[1], row[0]);
							sqliteCommand.ExecuteNonQuery();
						}
					}
				}
			}
		}

		public object Clone()
		{
			return new SqliteConnection(this);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				Close();
			}
		}

		public static void CreateFile(string databaseFileName)
		{
			FileStream fileStream = File.Create(databaseFileName);
			fileStream.Close();
		}

		internal void OnStateChange(ConnectionState newState)
		{
			ConnectionState connectionState = _connectionState;
			_connectionState = newState;
			if (StateChange != null && connectionState != newState)
			{
				StateChangeEventArgs e = new StateChangeEventArgs(connectionState, newState);
				StateChange(this, e);
			}
		}

		[Obsolete("Use one of the standard BeginTransaction methods, this one will be removed soon")]
		public SqliteTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel, bool deferredLock)
		{
			return (SqliteTransaction)BeginDbTransaction(deferredLock ? System.Data.IsolationLevel.ReadCommitted : System.Data.IsolationLevel.Serializable);
		}

		[Obsolete("Use one of the standard BeginTransaction methods, this one will be removed soon")]
		public SqliteTransaction BeginTransaction(bool deferredLock)
		{
			return (SqliteTransaction)BeginDbTransaction(deferredLock ? System.Data.IsolationLevel.ReadCommitted : System.Data.IsolationLevel.Serializable);
		}

		public new SqliteTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
		{
			return (SqliteTransaction)BeginDbTransaction(isolationLevel);
		}

		public new SqliteTransaction BeginTransaction()
		{
			return (SqliteTransaction)BeginDbTransaction(_defaultIsolation);
		}

		protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
		{
			if (_connectionState != ConnectionState.Open)
			{
				throw new InvalidOperationException();
			}
			if (isolationLevel == System.Data.IsolationLevel.Unspecified)
			{
				isolationLevel = _defaultIsolation;
			}
			if (isolationLevel != System.Data.IsolationLevel.Serializable && isolationLevel != System.Data.IsolationLevel.ReadCommitted)
			{
				throw new ArgumentException("isolationLevel");
			}
			return new SqliteTransaction(this, isolationLevel != System.Data.IsolationLevel.Serializable);
		}

		public override void ChangeDatabase(string databaseName)
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			if (_sql != null)
			{
				if (_enlistment != null)
				{
					SqliteConnection sqliteConnection = new SqliteConnection();
					sqliteConnection._sql = _sql;
					sqliteConnection._transactionLevel = _transactionLevel;
					sqliteConnection._enlistment = _enlistment;
					sqliteConnection._connectionState = _connectionState;
					sqliteConnection._version = _version;
					sqliteConnection._enlistment._transaction._cnn = sqliteConnection;
					sqliteConnection._enlistment._disposeConnection = true;
					_sql = null;
					_enlistment = null;
				}
				if (_sql != null)
				{
					_sql.Close();
				}
				_sql = null;
				_transactionLevel = 0;
			}
			OnStateChange(ConnectionState.Closed);
		}

		public static void ClearPool(SqliteConnection connection)
		{
			if (connection._sql != null)
			{
				connection._sql.ClearPool();
			}
		}

		public static void ClearAllPools()
		{
			SqliteConnectionPool.ClearAllPools();
		}

		public new SqliteCommand CreateCommand()
		{
			return new SqliteCommand(this);
		}

		protected override DbCommand CreateDbCommand()
		{
			return CreateCommand();
		}

		internal static void MapMonoKeyword(string[] arPiece, SortedList<string, string> ls)
		{
			string key;
			string value;
			switch (arPiece[0].ToLower(CultureInfo.InvariantCulture))
			{
			case "uri":
				key = "Data Source";
				value = MapMonoUriPath(arPiece[1]);
				break;
			default:
				key = arPiece[0];
				value = arPiece[1];
				break;
			}
			ls.Add(key, value);
		}

		internal static string MapMonoUriPath(string path)
		{
			if (path.StartsWith("file://"))
			{
				return path.Substring(7);
			}
			if (path.StartsWith("file:"))
			{
				return path.Substring(5);
			}
			if (path.StartsWith("/"))
			{
				return path;
			}
			throw new InvalidOperationException("Invalid connection string: invalid URI");
		}

		internal static string MapUriPath(string path)
		{
			if (path.StartsWith("file://"))
			{
				return path.Substring(7);
			}
			if (path.StartsWith("file:"))
			{
				return path.Substring(5);
			}
			if (path.StartsWith("/"))
			{
				return path;
			}
			throw new InvalidOperationException("Invalid connection string: invalid URI");
		}

		internal static SortedList<string, string> ParseConnectionString(string connectionString)
		{
			string source = connectionString.Replace(',', ';');
			SortedList<string, string> sortedList = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
			string[] array = SqliteConvert.Split(source, ';');
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				string[] array2 = SqliteConvert.Split(array[i], '=');
				if (array2.Length == 2)
				{
					MapMonoKeyword(array2, sortedList);
					continue;
				}
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid ConnectionString format for parameter \"{0}\"", (array2.Length <= 0) ? "null" : array2[0]));
			}
			return sortedList;
		}

		public override void EnlistTransaction(Transaction transaction)
		{
			if (_transactionLevel > 0 && transaction != null)
			{
				throw new ArgumentException("Unable to enlist in transaction, a local transaction already exists");
			}
			if (_enlistment != null && transaction != _enlistment._scope)
			{
				throw new ArgumentException("Already enlisted in a transaction");
			}
			_enlistment = new SQLiteEnlistment(this, transaction);
		}

		internal static string FindKey(SortedList<string, string> items, string key, string defValue)
		{
			string value;
			if (items.TryGetValue(key, out value))
			{
				return value;
			}
			return defValue;
		}

		public override void Open()
		{
			if (_connectionState != ConnectionState.Closed)
			{
				throw new InvalidOperationException();
			}
			Close();
			SortedList<string, string> items = ParseConnectionString(_connectionString);
			if (Convert.ToInt32(FindKey(items, "Version", "3"), CultureInfo.InvariantCulture) != 3)
			{
				throw new NotSupportedException("Only SQLite Version 3 is supported at this time");
			}
			string text = FindKey(items, "Data Source", string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				text = FindKey(items, "Uri", string.Empty);
				if (string.IsNullOrEmpty(text))
				{
					throw new ArgumentException("Data Source cannot be empty.  Use :memory: to open an in-memory database");
				}
				text = MapUriPath(text);
			}
			text = ((string.Compare(text, ":MEMORY:", true, CultureInfo.InvariantCulture) != 0) ? ExpandFileName(text) : ":memory:");
			try
			{
				bool usePool = SqliteConvert.ToBoolean(FindKey(items, "Pooling", bool.FalseString));
				bool flag = SqliteConvert.ToBoolean(FindKey(items, "UseUTF16Encoding", bool.FalseString));
				int maxPoolSize = Convert.ToInt32(FindKey(items, "Max Pool Size", "100"));
				_defaultTimeout = Convert.ToInt32(FindKey(items, "Default Timeout", "30"), CultureInfo.CurrentCulture);
				_defaultIsolation = (System.Data.IsolationLevel)(int)Enum.Parse(typeof(System.Data.IsolationLevel), FindKey(items, "Default IsolationLevel", "Serializable"), true);
				if (_defaultIsolation != System.Data.IsolationLevel.Serializable && _defaultIsolation != System.Data.IsolationLevel.ReadCommitted)
				{
					throw new NotSupportedException("Invalid Default IsolationLevel specified");
				}
				SQLiteDateFormats fmt = (SQLiteDateFormats)(int)Enum.Parse(typeof(SQLiteDateFormats), FindKey(items, "DateTimeFormat", "ISO8601"), true);
				if (flag)
				{
					_sql = new SQLite3_UTF16(fmt);
				}
				else
				{
					_sql = new SQLite3(fmt);
				}
				SQLiteOpenFlagsEnum sQLiteOpenFlagsEnum = SQLiteOpenFlagsEnum.None;
				if (!SqliteConvert.ToBoolean(FindKey(items, "FailIfMissing", bool.FalseString)))
				{
					sQLiteOpenFlagsEnum |= SQLiteOpenFlagsEnum.Create;
				}
				sQLiteOpenFlagsEnum = ((!SqliteConvert.ToBoolean(FindKey(items, "Read Only", bool.FalseString))) ? (sQLiteOpenFlagsEnum | SQLiteOpenFlagsEnum.ReadWrite) : (sQLiteOpenFlagsEnum | SQLiteOpenFlagsEnum.ReadOnly));
				_sql.Open(text, sQLiteOpenFlagsEnum, maxPoolSize, usePool);
				_binaryGuid = SqliteConvert.ToBoolean(FindKey(items, "BinaryGUID", bool.TrueString));
				string text2 = FindKey(items, "Password", null);
				if (!string.IsNullOrEmpty(text2))
				{
					_sql.SetPassword(Encoding.UTF8.GetBytes(text2));
				}
				else if (_password != null)
				{
					_sql.SetPassword(_password);
				}
				_password = null;
				_dataSource = Path.GetFileNameWithoutExtension(text);
				OnStateChange(ConnectionState.Open);
				_version++;
				using (SqliteCommand sqliteCommand = CreateCommand())
				{
					string text3;
					if (text != ":memory:")
					{
						text3 = FindKey(items, "Page Size", "1024");
						if (Convert.ToInt32(text3, CultureInfo.InvariantCulture) != 1024)
						{
							sqliteCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "PRAGMA page_size={0}", text3);
							sqliteCommand.ExecuteNonQuery();
						}
					}
					text3 = FindKey(items, "Max Page Count", "0");
					if (Convert.ToInt32(text3, CultureInfo.InvariantCulture) != 0)
					{
						sqliteCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "PRAGMA max_page_count={0}", text3);
						sqliteCommand.ExecuteNonQuery();
					}
					text3 = FindKey(items, "Legacy Format", bool.FalseString);
					sqliteCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "PRAGMA legacy_file_format={0}", (!SqliteConvert.ToBoolean(text3)) ? "OFF" : "ON");
					sqliteCommand.ExecuteNonQuery();
					text3 = FindKey(items, "Synchronous", "Normal");
					if (string.Compare(text3, "Full", StringComparison.OrdinalIgnoreCase) != 0)
					{
						sqliteCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "PRAGMA synchronous={0}", text3);
						sqliteCommand.ExecuteNonQuery();
					}
					text3 = FindKey(items, "Cache Size", "2000");
					if (Convert.ToInt32(text3, CultureInfo.InvariantCulture) != 2000)
					{
						sqliteCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "PRAGMA cache_size={0}", text3);
						sqliteCommand.ExecuteNonQuery();
					}
					text3 = FindKey(items, "Journal Mode", "Delete");
					if (string.Compare(text3, "Default", StringComparison.OrdinalIgnoreCase) != 0)
					{
						sqliteCommand.CommandText = string.Format(CultureInfo.InvariantCulture, "PRAGMA journal_mode={0}", text3);
						sqliteCommand.ExecuteNonQuery();
					}
				}
				if (this._commitHandler != null)
				{
					_sql.SetCommitHook(_commitCallback);
				}
				if (this._updateHandler != null)
				{
					_sql.SetUpdateHook(_updateCallback);
				}
				if (this._rollbackHandler != null)
				{
					_sql.SetRollbackHook(_rollbackCallback);
				}
				if (Transaction.Current != null && SqliteConvert.ToBoolean(FindKey(items, "Enlist", bool.TrueString)))
				{
					EnlistTransaction(Transaction.Current);
				}
			}
			catch (SqliteException)
			{
				Close();
				throw;
			}
		}

		public void ChangePassword(string newPassword)
		{
			ChangePassword((!string.IsNullOrEmpty(newPassword)) ? Encoding.UTF8.GetBytes(newPassword) : null);
		}

		public void ChangePassword(byte[] newPassword)
		{
			if (_connectionState != ConnectionState.Open)
			{
				throw new InvalidOperationException("Database must be opened before changing the password.");
			}
			_sql.ChangePassword(newPassword);
		}

		public void SetPassword(string databasePassword)
		{
			SetPassword((!string.IsNullOrEmpty(databasePassword)) ? Encoding.UTF8.GetBytes(databasePassword) : null);
		}

		public void SetPassword(byte[] databasePassword)
		{
			if (_connectionState != ConnectionState.Closed)
			{
				throw new InvalidOperationException("Password can only be set before the database is opened.");
			}
			if (databasePassword != null && databasePassword.Length == 0)
			{
				databasePassword = null;
			}
			_password = databasePassword;
		}

		private string ExpandFileName(string sourceFile)
		{
			if (string.IsNullOrEmpty(sourceFile))
			{
				return sourceFile;
			}
			if (sourceFile.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
			{
				string text = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
				if (string.IsNullOrEmpty(text))
				{
					text = AppDomain.CurrentDomain.BaseDirectory;
				}
				if (sourceFile.Length > "|DataDirectory|".Length && (sourceFile["|DataDirectory|".Length] == Path.DirectorySeparatorChar || sourceFile["|DataDirectory|".Length] == Path.AltDirectorySeparatorChar))
				{
					sourceFile = sourceFile.Remove("|DataDirectory|".Length, 1);
				}
				sourceFile = Path.Combine(text, sourceFile.Substring("|DataDirectory|".Length));
			}
			sourceFile = Path.GetFullPath(sourceFile);
			return sourceFile;
		}

		public override DataTable GetSchema()
		{
			return GetSchema("MetaDataCollections", null);
		}

		public override DataTable GetSchema(string collectionName)
		{
			return GetSchema(collectionName, new string[0]);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			if (_connectionState != ConnectionState.Open)
			{
				throw new InvalidOperationException();
			}
			string[] array = new string[5];
			if (restrictionValues == null)
			{
				restrictionValues = new string[0];
			}
			restrictionValues.CopyTo(array, 0);
			switch (collectionName.ToUpper(CultureInfo.InvariantCulture))
			{
			case "METADATACOLLECTIONS":
				return Schema_MetaDataCollections();
			case "DATASOURCEINFORMATION":
				return Schema_DataSourceInformation();
			case "DATATYPES":
				return Schema_DataTypes();
			case "COLUMNS":
			case "TABLECOLUMNS":
				return Schema_Columns(array[0], array[2], array[3]);
			case "INDEXES":
				return Schema_Indexes(array[0], array[2], array[3]);
			case "TRIGGERS":
				return Schema_Triggers(array[0], array[2], array[3]);
			case "INDEXCOLUMNS":
				return Schema_IndexColumns(array[0], array[2], array[3], array[4]);
			case "TABLES":
				return Schema_Tables(array[0], array[2], array[3]);
			case "VIEWS":
				return Schema_Views(array[0], array[2]);
			case "VIEWCOLUMNS":
				return Schema_ViewColumns(array[0], array[2], array[3]);
			case "FOREIGNKEYS":
				return Schema_ForeignKeys(array[0], array[2], array[3]);
			case "CATALOGS":
				return Schema_Catalogs(array[0]);
			case "RESERVEDWORDS":
				return Schema_ReservedWords();
			default:
				throw new NotSupportedException();
			}
		}

		private static DataTable Schema_ReservedWords()
		{
			DataTable dataTable = new DataTable("MetaDataCollections");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("ReservedWord", typeof(string));
			dataTable.Columns.Add("MaximumVersion", typeof(string));
			dataTable.Columns.Add("MinimumVersion", typeof(string));
			dataTable.BeginLoadData();
			string[] array = SR.Keywords.Split(',');
			foreach (string value in array)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = value;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private static DataTable Schema_MetaDataCollections()
		{
			DataTable dataTable = new DataTable("MetaDataCollections");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("CollectionName", typeof(string));
			dataTable.Columns.Add("NumberOfRestrictions", typeof(int));
			dataTable.Columns.Add("NumberOfIdentifierParts", typeof(int));
			dataTable.BeginLoadData();
			StringReader stringReader = new StringReader(SR.MetaDataCollections);
			dataTable.ReadXml((TextReader)stringReader);
			stringReader.Close();
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_DataSourceInformation()
		{
			DataTable dataTable = new DataTable("DataSourceInformation");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add(DbMetaDataColumnNames.CompositeIdentifierSeparatorPattern, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.DataSourceProductName, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.DataSourceProductVersion, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.DataSourceProductVersionNormalized, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.GroupByBehavior, typeof(int));
			dataTable.Columns.Add(DbMetaDataColumnNames.IdentifierPattern, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.IdentifierCase, typeof(int));
			dataTable.Columns.Add(DbMetaDataColumnNames.OrderByColumnsInSelect, typeof(bool));
			dataTable.Columns.Add(DbMetaDataColumnNames.ParameterMarkerFormat, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.ParameterMarkerPattern, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.ParameterNameMaxLength, typeof(int));
			dataTable.Columns.Add(DbMetaDataColumnNames.ParameterNamePattern, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.QuotedIdentifierPattern, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.QuotedIdentifierCase, typeof(int));
			dataTable.Columns.Add(DbMetaDataColumnNames.StatementSeparatorPattern, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.StringLiteralPattern, typeof(string));
			dataTable.Columns.Add(DbMetaDataColumnNames.SupportedJoinOperators, typeof(int));
			dataTable.BeginLoadData();
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = new object[17]
			{
				null, "SQLite", _sql.Version, _sql.Version, 3, "(^\\[\\p{Lo}\\p{Lu}\\p{Ll}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Nd}@$#_]*$)|(^\\[[^\\]\\0]|\\]\\]+\\]$)|(^\\\"[^\\\"\\0]|\\\"\\\"+\\\"$)", 1, false, "{0}", "@[\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}\\p{Nd}\\uff3f_@#\\$]*(?=\\s+|$)",
				255, "^[\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}_@#][\\p{Lo}\\p{Lu}\\p{Ll}\\p{Lm}\\p{Nd}\\uff3f_@#\\$]*(?=\\s+|$)", "(([^\\[]|\\]\\])*)", 1, ";", "'(([^']|'')*)'", 15
			};
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_Columns(string strCatalog, string strTable, string strColumn)
		{
			DataTable dataTable = new DataTable("Columns");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("TABLE_CATALOG", typeof(string));
			dataTable.Columns.Add("TABLE_SCHEMA", typeof(string));
			dataTable.Columns.Add("TABLE_NAME", typeof(string));
			dataTable.Columns.Add("COLUMN_NAME", typeof(string));
			dataTable.Columns.Add("COLUMN_GUID", typeof(Guid));
			dataTable.Columns.Add("COLUMN_PROPID", typeof(long));
			dataTable.Columns.Add("ORDINAL_POSITION", typeof(int));
			dataTable.Columns.Add("COLUMN_HASDEFAULT", typeof(bool));
			dataTable.Columns.Add("COLUMN_DEFAULT", typeof(string));
			dataTable.Columns.Add("COLUMN_FLAGS", typeof(long));
			dataTable.Columns.Add("IS_NULLABLE", typeof(bool));
			dataTable.Columns.Add("DATA_TYPE", typeof(string));
			dataTable.Columns.Add("TYPE_GUID", typeof(Guid));
			dataTable.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
			dataTable.Columns.Add("CHARACTER_OCTET_LENGTH", typeof(int));
			dataTable.Columns.Add("NUMERIC_PRECISION", typeof(int));
			dataTable.Columns.Add("NUMERIC_SCALE", typeof(int));
			dataTable.Columns.Add("DATETIME_PRECISION", typeof(long));
			dataTable.Columns.Add("CHARACTER_SET_CATALOG", typeof(string));
			dataTable.Columns.Add("CHARACTER_SET_SCHEMA", typeof(string));
			dataTable.Columns.Add("CHARACTER_SET_NAME", typeof(string));
			dataTable.Columns.Add("COLLATION_CATALOG", typeof(string));
			dataTable.Columns.Add("COLLATION_SCHEMA", typeof(string));
			dataTable.Columns.Add("COLLATION_NAME", typeof(string));
			dataTable.Columns.Add("DOMAIN_CATALOG", typeof(string));
			dataTable.Columns.Add("DOMAIN_NAME", typeof(string));
			dataTable.Columns.Add("DESCRIPTION", typeof(string));
			dataTable.Columns.Add("PRIMARY_KEY", typeof(bool));
			dataTable.Columns.Add("EDM_TYPE", typeof(string));
			dataTable.Columns.Add("AUTOINCREMENT", typeof(bool));
			dataTable.Columns.Add("UNIQUE", typeof(bool));
			dataTable.BeginLoadData();
			if (string.IsNullOrEmpty(strCatalog))
			{
				strCatalog = "main";
			}
			string text = ((string.Compare(strCatalog, "temp", true, CultureInfo.InvariantCulture) != 0) ? "sqlite_master" : "sqlite_temp_master");
			using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{1}] WHERE [type] LIKE 'table' OR [type] LIKE 'view'", strCatalog, text), this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						if (!string.IsNullOrEmpty(strTable) && string.Compare(strTable, sqliteDataReader.GetString(2), true, CultureInfo.InvariantCulture) != 0)
						{
							continue;
						}
						try
						{
							using (SqliteCommand sqliteCommand2 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{1}]", strCatalog, sqliteDataReader.GetString(2)), this))
							{
								using (SqliteDataReader sqliteDataReader2 = sqliteCommand2.ExecuteReader(CommandBehavior.SchemaOnly))
								{
									using (DataTable dataTable2 = sqliteDataReader2.GetSchemaTable(true, true))
									{
										foreach (DataRow row in dataTable2.Rows)
										{
											if (string.Compare(row[SchemaTableColumn.ColumnName].ToString(), strColumn, true, CultureInfo.InvariantCulture) == 0 || strColumn == null)
											{
												DataRow dataRow2 = dataTable.NewRow();
												dataRow2["NUMERIC_PRECISION"] = row[SchemaTableColumn.NumericPrecision];
												dataRow2["NUMERIC_SCALE"] = row[SchemaTableColumn.NumericScale];
												dataRow2["TABLE_NAME"] = sqliteDataReader.GetString(2);
												dataRow2["COLUMN_NAME"] = row[SchemaTableColumn.ColumnName];
												dataRow2["TABLE_CATALOG"] = strCatalog;
												dataRow2["ORDINAL_POSITION"] = row[SchemaTableColumn.ColumnOrdinal];
												dataRow2["COLUMN_HASDEFAULT"] = row[SchemaTableOptionalColumn.DefaultValue] != DBNull.Value;
												dataRow2["COLUMN_DEFAULT"] = row[SchemaTableOptionalColumn.DefaultValue];
												dataRow2["IS_NULLABLE"] = row[SchemaTableColumn.AllowDBNull];
												dataRow2["DATA_TYPE"] = row["DataTypeName"].ToString().ToLower(CultureInfo.InvariantCulture);
												dataRow2["EDM_TYPE"] = SqliteConvert.DbTypeToTypeName((DbType)(int)row[SchemaTableColumn.ProviderType]).ToString().ToLower(CultureInfo.InvariantCulture);
												dataRow2["CHARACTER_MAXIMUM_LENGTH"] = row[SchemaTableColumn.ColumnSize];
												dataRow2["TABLE_SCHEMA"] = row[SchemaTableColumn.BaseSchemaName];
												dataRow2["PRIMARY_KEY"] = row[SchemaTableColumn.IsKey];
												dataRow2["AUTOINCREMENT"] = row[SchemaTableOptionalColumn.IsAutoIncrement];
												dataRow2["COLLATION_NAME"] = row["CollationType"];
												dataRow2["UNIQUE"] = row[SchemaTableColumn.IsUnique];
												dataTable.Rows.Add(dataRow2);
											}
										}
									}
								}
							}
						}
						catch (SqliteException)
						{
						}
					}
				}
			}
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_Indexes(string strCatalog, string strTable, string strIndex)
		{
			DataTable dataTable = new DataTable("Indexes");
			List<int> list = new List<int>();
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("TABLE_CATALOG", typeof(string));
			dataTable.Columns.Add("TABLE_SCHEMA", typeof(string));
			dataTable.Columns.Add("TABLE_NAME", typeof(string));
			dataTable.Columns.Add("INDEX_CATALOG", typeof(string));
			dataTable.Columns.Add("INDEX_SCHEMA", typeof(string));
			dataTable.Columns.Add("INDEX_NAME", typeof(string));
			dataTable.Columns.Add("PRIMARY_KEY", typeof(bool));
			dataTable.Columns.Add("UNIQUE", typeof(bool));
			dataTable.Columns.Add("CLUSTERED", typeof(bool));
			dataTable.Columns.Add("TYPE", typeof(int));
			dataTable.Columns.Add("FILL_FACTOR", typeof(int));
			dataTable.Columns.Add("INITIAL_SIZE", typeof(int));
			dataTable.Columns.Add("NULLS", typeof(int));
			dataTable.Columns.Add("SORT_BOOKMARKS", typeof(bool));
			dataTable.Columns.Add("AUTO_UPDATE", typeof(bool));
			dataTable.Columns.Add("NULL_COLLATION", typeof(int));
			dataTable.Columns.Add("ORDINAL_POSITION", typeof(int));
			dataTable.Columns.Add("COLUMN_NAME", typeof(string));
			dataTable.Columns.Add("COLUMN_GUID", typeof(Guid));
			dataTable.Columns.Add("COLUMN_PROPID", typeof(long));
			dataTable.Columns.Add("COLLATION", typeof(short));
			dataTable.Columns.Add("CARDINALITY", typeof(decimal));
			dataTable.Columns.Add("PAGES", typeof(int));
			dataTable.Columns.Add("FILTER_CONDITION", typeof(string));
			dataTable.Columns.Add("INTEGRATED", typeof(bool));
			dataTable.Columns.Add("INDEX_DEFINITION", typeof(string));
			dataTable.BeginLoadData();
			if (string.IsNullOrEmpty(strCatalog))
			{
				strCatalog = "main";
			}
			string text = ((string.Compare(strCatalog, "temp", true, CultureInfo.InvariantCulture) != 0) ? "sqlite_master" : "sqlite_temp_master");
			using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{1}] WHERE [type] LIKE 'table'", strCatalog, text), this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						bool flag = false;
						list.Clear();
						if (!string.IsNullOrEmpty(strTable) && string.Compare(sqliteDataReader.GetString(2), strTable, true, CultureInfo.InvariantCulture) != 0)
						{
							continue;
						}
						try
						{
							using (SqliteCommand sqliteCommand2 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "PRAGMA [{0}].table_info([{1}])", strCatalog, sqliteDataReader.GetString(2)), this))
							{
								using (SqliteDataReader sqliteDataReader2 = sqliteCommand2.ExecuteReader())
								{
									while (sqliteDataReader2.Read())
									{
										if (sqliteDataReader2.GetInt32(5) == 1)
										{
											list.Add(sqliteDataReader2.GetInt32(0));
											if (string.Compare(sqliteDataReader2.GetString(2), "INTEGER", true, CultureInfo.InvariantCulture) == 0)
											{
												flag = true;
											}
										}
									}
								}
							}
						}
						catch (SqliteException)
						{
						}
						if (list.Count == 1 && flag)
						{
							DataRow dataRow = dataTable.NewRow();
							dataRow["TABLE_CATALOG"] = strCatalog;
							dataRow["TABLE_NAME"] = sqliteDataReader.GetString(2);
							dataRow["INDEX_CATALOG"] = strCatalog;
							dataRow["PRIMARY_KEY"] = true;
							dataRow["INDEX_NAME"] = string.Format(CultureInfo.InvariantCulture, "{1}_PK_{0}", sqliteDataReader.GetString(2), text);
							dataRow["UNIQUE"] = true;
							if (string.Compare((string)dataRow["INDEX_NAME"], strIndex, true, CultureInfo.InvariantCulture) == 0 || strIndex == null)
							{
								dataTable.Rows.Add(dataRow);
							}
							list.Clear();
						}
						try
						{
							using (SqliteCommand sqliteCommand3 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "PRAGMA [{0}].index_list([{1}])", strCatalog, sqliteDataReader.GetString(2)), this))
							{
								using (SqliteDataReader sqliteDataReader3 = sqliteCommand3.ExecuteReader())
								{
									while (sqliteDataReader3.Read())
									{
										if (string.Compare(sqliteDataReader3.GetString(1), strIndex, true, CultureInfo.InvariantCulture) != 0 && strIndex != null)
										{
											continue;
										}
										DataRow dataRow = dataTable.NewRow();
										dataRow["TABLE_CATALOG"] = strCatalog;
										dataRow["TABLE_NAME"] = sqliteDataReader.GetString(2);
										dataRow["INDEX_CATALOG"] = strCatalog;
										dataRow["INDEX_NAME"] = sqliteDataReader3.GetString(1);
										dataRow["UNIQUE"] = sqliteDataReader3.GetBoolean(2);
										dataRow["PRIMARY_KEY"] = false;
										using (SqliteCommand sqliteCommand4 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{2}] WHERE [type] LIKE 'index' AND [name] LIKE '{1}'", strCatalog, sqliteDataReader3.GetString(1).Replace("'", "''"), text), this))
										{
											using (SqliteDataReader sqliteDataReader4 = sqliteCommand4.ExecuteReader())
											{
												if (sqliteDataReader4.Read() && !sqliteDataReader4.IsDBNull(4))
												{
													dataRow["INDEX_DEFINITION"] = sqliteDataReader4.GetString(4);
												}
											}
										}
										if (list.Count > 0 && sqliteDataReader3.GetString(1).StartsWith("sqlite_autoindex_" + sqliteDataReader.GetString(2), StringComparison.InvariantCultureIgnoreCase))
										{
											using (SqliteCommand sqliteCommand5 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "PRAGMA [{0}].index_info([{1}])", strCatalog, sqliteDataReader3.GetString(1)), this))
											{
												using (SqliteDataReader sqliteDataReader5 = sqliteCommand5.ExecuteReader())
												{
													int num = 0;
													while (sqliteDataReader5.Read())
													{
														if (!list.Contains(sqliteDataReader5.GetInt32(1)))
														{
															num = 0;
															break;
														}
														num++;
													}
													if (num == list.Count)
													{
														dataRow["PRIMARY_KEY"] = true;
														list.Clear();
													}
												}
											}
										}
										dataTable.Rows.Add(dataRow);
									}
								}
							}
						}
						catch (SqliteException)
						{
						}
					}
				}
			}
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_Triggers(string catalog, string table, string triggerName)
		{
			DataTable dataTable = new DataTable("Triggers");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("TABLE_CATALOG", typeof(string));
			dataTable.Columns.Add("TABLE_SCHEMA", typeof(string));
			dataTable.Columns.Add("TABLE_NAME", typeof(string));
			dataTable.Columns.Add("TRIGGER_NAME", typeof(string));
			dataTable.Columns.Add("TRIGGER_DEFINITION", typeof(string));
			dataTable.BeginLoadData();
			if (string.IsNullOrEmpty(table))
			{
				table = null;
			}
			if (string.IsNullOrEmpty(catalog))
			{
				catalog = "main";
			}
			string text = ((string.Compare(catalog, "temp", true, CultureInfo.InvariantCulture) != 0) ? "sqlite_master" : "sqlite_temp_master");
			using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT [type], [name], [tbl_name], [rootpage], [sql], [rowid] FROM [{0}].[{1}] WHERE [type] LIKE 'trigger'", catalog, text), this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						if ((string.Compare(sqliteDataReader.GetString(1), triggerName, true, CultureInfo.InvariantCulture) == 0 || triggerName == null) && (table == null || string.Compare(table, sqliteDataReader.GetString(2), true, CultureInfo.InvariantCulture) == 0))
						{
							DataRow dataRow = dataTable.NewRow();
							dataRow["TABLE_CATALOG"] = catalog;
							dataRow["TABLE_NAME"] = sqliteDataReader.GetString(2);
							dataRow["TRIGGER_NAME"] = sqliteDataReader.GetString(1);
							dataRow["TRIGGER_DEFINITION"] = sqliteDataReader.GetString(4);
							dataTable.Rows.Add(dataRow);
						}
					}
				}
			}
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_Tables(string strCatalog, string strTable, string strType)
		{
			DataTable dataTable = new DataTable("Tables");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("TABLE_CATALOG", typeof(string));
			dataTable.Columns.Add("TABLE_SCHEMA", typeof(string));
			dataTable.Columns.Add("TABLE_NAME", typeof(string));
			dataTable.Columns.Add("TABLE_TYPE", typeof(string));
			dataTable.Columns.Add("TABLE_ID", typeof(long));
			dataTable.Columns.Add("TABLE_ROOTPAGE", typeof(int));
			dataTable.Columns.Add("TABLE_DEFINITION", typeof(string));
			dataTable.BeginLoadData();
			if (string.IsNullOrEmpty(strCatalog))
			{
				strCatalog = "main";
			}
			string text = ((string.Compare(strCatalog, "temp", true, CultureInfo.InvariantCulture) != 0) ? "sqlite_master" : "sqlite_temp_master");
			using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT [type], [name], [tbl_name], [rootpage], [sql], [rowid] FROM [{0}].[{1}] WHERE [type] LIKE 'table'", strCatalog, text), this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						string text2 = sqliteDataReader.GetString(0);
						if (string.Compare(sqliteDataReader.GetString(2), 0, "SQLITE_", 0, 7, true, CultureInfo.InvariantCulture) == 0)
						{
							text2 = "SYSTEM_TABLE";
						}
						if ((string.Compare(strType, text2, true, CultureInfo.InvariantCulture) == 0 || strType == null) && (string.Compare(sqliteDataReader.GetString(2), strTable, true, CultureInfo.InvariantCulture) == 0 || strTable == null))
						{
							DataRow dataRow = dataTable.NewRow();
							dataRow["TABLE_CATALOG"] = strCatalog;
							dataRow["TABLE_NAME"] = sqliteDataReader.GetString(2);
							dataRow["TABLE_TYPE"] = text2;
							dataRow["TABLE_ID"] = sqliteDataReader.GetInt64(5);
							dataRow["TABLE_ROOTPAGE"] = sqliteDataReader.GetInt32(3);
							dataRow["TABLE_DEFINITION"] = sqliteDataReader.GetString(4);
							dataTable.Rows.Add(dataRow);
						}
					}
				}
			}
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_Views(string strCatalog, string strView)
		{
			DataTable dataTable = new DataTable("Views");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("TABLE_CATALOG", typeof(string));
			dataTable.Columns.Add("TABLE_SCHEMA", typeof(string));
			dataTable.Columns.Add("TABLE_NAME", typeof(string));
			dataTable.Columns.Add("VIEW_DEFINITION", typeof(string));
			dataTable.Columns.Add("CHECK_OPTION", typeof(bool));
			dataTable.Columns.Add("IS_UPDATABLE", typeof(bool));
			dataTable.Columns.Add("DESCRIPTION", typeof(string));
			dataTable.Columns.Add("DATE_CREATED", typeof(DateTime));
			dataTable.Columns.Add("DATE_MODIFIED", typeof(DateTime));
			dataTable.BeginLoadData();
			if (string.IsNullOrEmpty(strCatalog))
			{
				strCatalog = "main";
			}
			string text = ((string.Compare(strCatalog, "temp", true, CultureInfo.InvariantCulture) != 0) ? "sqlite_master" : "sqlite_temp_master");
			using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{1}] WHERE [type] LIKE 'view'", strCatalog, text), this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						if (string.Compare(sqliteDataReader.GetString(1), strView, true, CultureInfo.InvariantCulture) == 0 || string.IsNullOrEmpty(strView))
						{
							string text2 = sqliteDataReader.GetString(4).Replace('\r', ' ').Replace('\n', ' ')
								.Replace('\t', ' ');
							int num = CultureInfo.InvariantCulture.CompareInfo.IndexOf(text2, " AS ", CompareOptions.IgnoreCase);
							if (num > -1)
							{
								text2 = text2.Substring(num + 4).Trim();
								DataRow dataRow = dataTable.NewRow();
								dataRow["TABLE_CATALOG"] = strCatalog;
								dataRow["TABLE_NAME"] = sqliteDataReader.GetString(2);
								dataRow["IS_UPDATABLE"] = false;
								dataRow["VIEW_DEFINITION"] = text2;
								dataTable.Rows.Add(dataRow);
							}
						}
					}
				}
			}
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_Catalogs(string strCatalog)
		{
			DataTable dataTable = new DataTable("Catalogs");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("CATALOG_NAME", typeof(string));
			dataTable.Columns.Add("DESCRIPTION", typeof(string));
			dataTable.Columns.Add("ID", typeof(long));
			dataTable.BeginLoadData();
			using (SqliteCommand sqliteCommand = new SqliteCommand("PRAGMA database_list", this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						if (string.Compare(sqliteDataReader.GetString(1), strCatalog, true, CultureInfo.InvariantCulture) == 0 || strCatalog == null)
						{
							DataRow dataRow = dataTable.NewRow();
							dataRow["CATALOG_NAME"] = sqliteDataReader.GetString(1);
							dataRow["DESCRIPTION"] = sqliteDataReader.GetString(2);
							dataRow["ID"] = sqliteDataReader.GetInt64(0);
							dataTable.Rows.Add(dataRow);
						}
					}
				}
			}
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_DataTypes()
		{
			DataTable dataTable = new DataTable("DataTypes");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("TypeName", typeof(string));
			dataTable.Columns.Add("ProviderDbType", typeof(int));
			dataTable.Columns.Add("ColumnSize", typeof(long));
			dataTable.Columns.Add("CreateFormat", typeof(string));
			dataTable.Columns.Add("CreateParameters", typeof(string));
			dataTable.Columns.Add("DataType", typeof(string));
			dataTable.Columns.Add("IsAutoIncrementable", typeof(bool));
			dataTable.Columns.Add("IsBestMatch", typeof(bool));
			dataTable.Columns.Add("IsCaseSensitive", typeof(bool));
			dataTable.Columns.Add("IsFixedLength", typeof(bool));
			dataTable.Columns.Add("IsFixedPrecisionScale", typeof(bool));
			dataTable.Columns.Add("IsLong", typeof(bool));
			dataTable.Columns.Add("IsNullable", typeof(bool));
			dataTable.Columns.Add("IsSearchable", typeof(bool));
			dataTable.Columns.Add("IsSearchableWithLike", typeof(bool));
			dataTable.Columns.Add("IsLiteralSupported", typeof(bool));
			dataTable.Columns.Add("LiteralPrefix", typeof(string));
			dataTable.Columns.Add("LiteralSuffix", typeof(string));
			dataTable.Columns.Add("IsUnsigned", typeof(bool));
			dataTable.Columns.Add("MaximumScale", typeof(short));
			dataTable.Columns.Add("MinimumScale", typeof(short));
			dataTable.Columns.Add("IsConcurrencyType", typeof(bool));
			dataTable.BeginLoadData();
			StringReader stringReader = new StringReader(SR.DataTypes);
			dataTable.ReadXml((TextReader)stringReader);
			stringReader.Close();
			dataTable.AcceptChanges();
			dataTable.EndLoadData();
			return dataTable;
		}

		private DataTable Schema_IndexColumns(string strCatalog, string strTable, string strIndex, string strColumn)
		{
			DataTable dataTable = new DataTable("IndexColumns");
			List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("CONSTRAINT_CATALOG", typeof(string));
			dataTable.Columns.Add("CONSTRAINT_SCHEMA", typeof(string));
			dataTable.Columns.Add("CONSTRAINT_NAME", typeof(string));
			dataTable.Columns.Add("TABLE_CATALOG", typeof(string));
			dataTable.Columns.Add("TABLE_SCHEMA", typeof(string));
			dataTable.Columns.Add("TABLE_NAME", typeof(string));
			dataTable.Columns.Add("COLUMN_NAME", typeof(string));
			dataTable.Columns.Add("ORDINAL_POSITION", typeof(int));
			dataTable.Columns.Add("INDEX_NAME", typeof(string));
			dataTable.Columns.Add("COLLATION_NAME", typeof(string));
			dataTable.Columns.Add("SORT_MODE", typeof(string));
			dataTable.Columns.Add("CONFLICT_OPTION", typeof(int));
			if (string.IsNullOrEmpty(strCatalog))
			{
				strCatalog = "main";
			}
			string text = ((string.Compare(strCatalog, "temp", true, CultureInfo.InvariantCulture) != 0) ? "sqlite_master" : "sqlite_temp_master");
			dataTable.BeginLoadData();
			using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{1}] WHERE [type] LIKE 'table'", strCatalog, text), this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						bool flag = false;
						list.Clear();
						if (!string.IsNullOrEmpty(strTable) && string.Compare(sqliteDataReader.GetString(2), strTable, true, CultureInfo.InvariantCulture) != 0)
						{
							continue;
						}
						try
						{
							using (SqliteCommand sqliteCommand2 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "PRAGMA [{0}].table_info([{1}])", strCatalog, sqliteDataReader.GetString(2)), this))
							{
								using (SqliteDataReader sqliteDataReader2 = sqliteCommand2.ExecuteReader())
								{
									while (sqliteDataReader2.Read())
									{
										if (sqliteDataReader2.GetInt32(5) == 1)
										{
											list.Add(new KeyValuePair<int, string>(sqliteDataReader2.GetInt32(0), sqliteDataReader2.GetString(1)));
											if (string.Compare(sqliteDataReader2.GetString(2), "INTEGER", true, CultureInfo.InvariantCulture) == 0)
											{
												flag = true;
											}
										}
									}
								}
							}
						}
						catch (SqliteException)
						{
						}
						if (list.Count == 1 && flag)
						{
							DataRow dataRow = dataTable.NewRow();
							dataRow["CONSTRAINT_CATALOG"] = strCatalog;
							dataRow["CONSTRAINT_NAME"] = string.Format(CultureInfo.InvariantCulture, "{1}_PK_{0}", sqliteDataReader.GetString(2), text);
							dataRow["TABLE_CATALOG"] = strCatalog;
							dataRow["TABLE_NAME"] = sqliteDataReader.GetString(2);
							dataRow["COLUMN_NAME"] = list[0].Value;
							dataRow["INDEX_NAME"] = dataRow["CONSTRAINT_NAME"];
							dataRow["ORDINAL_POSITION"] = 0;
							dataRow["COLLATION_NAME"] = "BINARY";
							dataRow["SORT_MODE"] = "ASC";
							dataRow["CONFLICT_OPTION"] = 2;
							if (string.IsNullOrEmpty(strIndex) || string.Compare(strIndex, (string)dataRow["INDEX_NAME"], true, CultureInfo.InvariantCulture) == 0)
							{
								dataTable.Rows.Add(dataRow);
							}
						}
						using (SqliteCommand sqliteCommand3 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{2}] WHERE [type] LIKE 'index' AND [tbl_name] LIKE '{1}'", strCatalog, sqliteDataReader.GetString(2).Replace("'", "''"), text), this))
						{
							using (SqliteDataReader sqliteDataReader3 = sqliteCommand3.ExecuteReader())
							{
								while (sqliteDataReader3.Read())
								{
									int num = 0;
									if (!string.IsNullOrEmpty(strIndex) && string.Compare(strIndex, sqliteDataReader3.GetString(1), true, CultureInfo.InvariantCulture) != 0)
									{
										continue;
									}
									try
									{
										using (SqliteCommand sqliteCommand4 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "PRAGMA [{0}].index_info([{1}])", strCatalog, sqliteDataReader3.GetString(1)), this))
										{
											using (SqliteDataReader sqliteDataReader4 = sqliteCommand4.ExecuteReader())
											{
												while (sqliteDataReader4.Read())
												{
													DataRow dataRow = dataTable.NewRow();
													dataRow["CONSTRAINT_CATALOG"] = strCatalog;
													dataRow["CONSTRAINT_NAME"] = sqliteDataReader3.GetString(1);
													dataRow["TABLE_CATALOG"] = strCatalog;
													dataRow["TABLE_NAME"] = sqliteDataReader3.GetString(2);
													dataRow["COLUMN_NAME"] = sqliteDataReader4.GetString(2);
													dataRow["INDEX_NAME"] = sqliteDataReader3.GetString(1);
													dataRow["ORDINAL_POSITION"] = num;
													int sortMode;
													int onError;
													string collationSequence;
													_sql.GetIndexColumnExtendedInfo(strCatalog, sqliteDataReader3.GetString(1), sqliteDataReader4.GetString(2), out sortMode, out onError, out collationSequence);
													if (!string.IsNullOrEmpty(collationSequence))
													{
														dataRow["COLLATION_NAME"] = collationSequence;
													}
													dataRow["SORT_MODE"] = ((sortMode != 0) ? "DESC" : "ASC");
													dataRow["CONFLICT_OPTION"] = onError;
													num++;
													if (string.IsNullOrEmpty(strColumn) || string.Compare(strColumn, dataRow["COLUMN_NAME"].ToString(), true, CultureInfo.InvariantCulture) == 0)
													{
														dataTable.Rows.Add(dataRow);
													}
												}
											}
										}
									}
									catch (SqliteException)
									{
									}
								}
							}
						}
					}
				}
			}
			dataTable.EndLoadData();
			dataTable.AcceptChanges();
			return dataTable;
		}

		private DataTable Schema_ViewColumns(string strCatalog, string strView, string strColumn)
		{
			DataTable dataTable = new DataTable("ViewColumns");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("VIEW_CATALOG", typeof(string));
			dataTable.Columns.Add("VIEW_SCHEMA", typeof(string));
			dataTable.Columns.Add("VIEW_NAME", typeof(string));
			dataTable.Columns.Add("VIEW_COLUMN_NAME", typeof(string));
			dataTable.Columns.Add("TABLE_CATALOG", typeof(string));
			dataTable.Columns.Add("TABLE_SCHEMA", typeof(string));
			dataTable.Columns.Add("TABLE_NAME", typeof(string));
			dataTable.Columns.Add("COLUMN_NAME", typeof(string));
			dataTable.Columns.Add("ORDINAL_POSITION", typeof(int));
			dataTable.Columns.Add("COLUMN_HASDEFAULT", typeof(bool));
			dataTable.Columns.Add("COLUMN_DEFAULT", typeof(string));
			dataTable.Columns.Add("COLUMN_FLAGS", typeof(long));
			dataTable.Columns.Add("IS_NULLABLE", typeof(bool));
			dataTable.Columns.Add("DATA_TYPE", typeof(string));
			dataTable.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
			dataTable.Columns.Add("NUMERIC_PRECISION", typeof(int));
			dataTable.Columns.Add("NUMERIC_SCALE", typeof(int));
			dataTable.Columns.Add("DATETIME_PRECISION", typeof(long));
			dataTable.Columns.Add("CHARACTER_SET_CATALOG", typeof(string));
			dataTable.Columns.Add("CHARACTER_SET_SCHEMA", typeof(string));
			dataTable.Columns.Add("CHARACTER_SET_NAME", typeof(string));
			dataTable.Columns.Add("COLLATION_CATALOG", typeof(string));
			dataTable.Columns.Add("COLLATION_SCHEMA", typeof(string));
			dataTable.Columns.Add("COLLATION_NAME", typeof(string));
			dataTable.Columns.Add("PRIMARY_KEY", typeof(bool));
			dataTable.Columns.Add("EDM_TYPE", typeof(string));
			dataTable.Columns.Add("AUTOINCREMENT", typeof(bool));
			dataTable.Columns.Add("UNIQUE", typeof(bool));
			if (string.IsNullOrEmpty(strCatalog))
			{
				strCatalog = "main";
			}
			string text = ((string.Compare(strCatalog, "temp", true, CultureInfo.InvariantCulture) != 0) ? "sqlite_master" : "sqlite_temp_master");
			dataTable.BeginLoadData();
			using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{1}] WHERE [type] LIKE 'view'", strCatalog, text), this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						if (!string.IsNullOrEmpty(strView) && string.Compare(strView, sqliteDataReader.GetString(2), true, CultureInfo.InvariantCulture) != 0)
						{
							continue;
						}
						using (SqliteCommand sqliteCommand2 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{1}]", strCatalog, sqliteDataReader.GetString(2)), this))
						{
							string text2 = sqliteDataReader.GetString(4).Replace('\r', ' ').Replace('\n', ' ')
								.Replace('\t', ' ');
							int num = CultureInfo.InvariantCulture.CompareInfo.IndexOf(text2, " AS ", CompareOptions.IgnoreCase);
							if (num < 0)
							{
								continue;
							}
							text2 = text2.Substring(num + 4);
							using (SqliteCommand sqliteCommand3 = new SqliteCommand(text2, this))
							{
								using (SqliteDataReader sqliteDataReader2 = sqliteCommand2.ExecuteReader(CommandBehavior.SchemaOnly))
								{
									using (SqliteDataReader sqliteDataReader3 = sqliteCommand3.ExecuteReader(CommandBehavior.SchemaOnly))
									{
										using (DataTable dataTable2 = sqliteDataReader2.GetSchemaTable(false, false))
										{
											using (DataTable dataTable3 = sqliteDataReader3.GetSchemaTable(false, false))
											{
												for (num = 0; num < dataTable3.Rows.Count; num++)
												{
													DataRow dataRow = dataTable2.Rows[num];
													DataRow dataRow2 = dataTable3.Rows[num];
													if (string.Compare(dataRow[SchemaTableColumn.ColumnName].ToString(), strColumn, true, CultureInfo.InvariantCulture) == 0 || strColumn == null)
													{
														DataRow dataRow3 = dataTable.NewRow();
														dataRow3["VIEW_CATALOG"] = strCatalog;
														dataRow3["VIEW_NAME"] = sqliteDataReader.GetString(2);
														dataRow3["TABLE_CATALOG"] = strCatalog;
														dataRow3["TABLE_SCHEMA"] = dataRow2[SchemaTableColumn.BaseSchemaName];
														dataRow3["TABLE_NAME"] = dataRow2[SchemaTableColumn.BaseTableName];
														dataRow3["COLUMN_NAME"] = dataRow2[SchemaTableColumn.BaseColumnName];
														dataRow3["VIEW_COLUMN_NAME"] = dataRow[SchemaTableColumn.ColumnName];
														dataRow3["COLUMN_HASDEFAULT"] = dataRow[SchemaTableOptionalColumn.DefaultValue] != DBNull.Value;
														dataRow3["COLUMN_DEFAULT"] = dataRow[SchemaTableOptionalColumn.DefaultValue];
														dataRow3["ORDINAL_POSITION"] = dataRow[SchemaTableColumn.ColumnOrdinal];
														dataRow3["IS_NULLABLE"] = dataRow[SchemaTableColumn.AllowDBNull];
														dataRow3["DATA_TYPE"] = dataRow["DataTypeName"];
														dataRow3["EDM_TYPE"] = SqliteConvert.DbTypeToTypeName((DbType)(int)dataRow[SchemaTableColumn.ProviderType]).ToString().ToLower(CultureInfo.InvariantCulture);
														dataRow3["CHARACTER_MAXIMUM_LENGTH"] = dataRow[SchemaTableColumn.ColumnSize];
														dataRow3["TABLE_SCHEMA"] = dataRow[SchemaTableColumn.BaseSchemaName];
														dataRow3["PRIMARY_KEY"] = dataRow[SchemaTableColumn.IsKey];
														dataRow3["AUTOINCREMENT"] = dataRow[SchemaTableOptionalColumn.IsAutoIncrement];
														dataRow3["COLLATION_NAME"] = dataRow["CollationType"];
														dataRow3["UNIQUE"] = dataRow[SchemaTableColumn.IsUnique];
														dataTable.Rows.Add(dataRow3);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			dataTable.EndLoadData();
			dataTable.AcceptChanges();
			return dataTable;
		}

		private DataTable Schema_ForeignKeys(string strCatalog, string strTable, string strKeyName)
		{
			DataTable dataTable = new DataTable("ForeignKeys");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("CONSTRAINT_CATALOG", typeof(string));
			dataTable.Columns.Add("CONSTRAINT_SCHEMA", typeof(string));
			dataTable.Columns.Add("CONSTRAINT_NAME", typeof(string));
			dataTable.Columns.Add("TABLE_CATALOG", typeof(string));
			dataTable.Columns.Add("TABLE_SCHEMA", typeof(string));
			dataTable.Columns.Add("TABLE_NAME", typeof(string));
			dataTable.Columns.Add("CONSTRAINT_TYPE", typeof(string));
			dataTable.Columns.Add("IS_DEFERRABLE", typeof(bool));
			dataTable.Columns.Add("INITIALLY_DEFERRED", typeof(bool));
			dataTable.Columns.Add("FKEY_FROM_COLUMN", typeof(string));
			dataTable.Columns.Add("FKEY_FROM_ORDINAL_POSITION", typeof(int));
			dataTable.Columns.Add("FKEY_TO_CATALOG", typeof(string));
			dataTable.Columns.Add("FKEY_TO_SCHEMA", typeof(string));
			dataTable.Columns.Add("FKEY_TO_TABLE", typeof(string));
			dataTable.Columns.Add("FKEY_TO_COLUMN", typeof(string));
			if (string.IsNullOrEmpty(strCatalog))
			{
				strCatalog = "main";
			}
			string text = ((string.Compare(strCatalog, "temp", true, CultureInfo.InvariantCulture) != 0) ? "sqlite_master" : "sqlite_temp_master");
			dataTable.BeginLoadData();
			using (SqliteCommand sqliteCommand = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}].[{1}] WHERE [type] LIKE 'table'", strCatalog, text), this))
			{
				using (SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
				{
					while (sqliteDataReader.Read())
					{
						if (!string.IsNullOrEmpty(strTable) && string.Compare(strTable, sqliteDataReader.GetString(2), true, CultureInfo.InvariantCulture) != 0)
						{
							continue;
						}
						try
						{
							using (SqliteCommandBuilder sqliteCommandBuilder = new SqliteCommandBuilder())
							{
								using (SqliteCommand sqliteCommand2 = new SqliteCommand(string.Format(CultureInfo.InvariantCulture, "PRAGMA [{0}].foreign_key_list([{1}])", strCatalog, sqliteDataReader.GetString(2)), this))
								{
									using (SqliteDataReader sqliteDataReader2 = sqliteCommand2.ExecuteReader())
									{
										while (sqliteDataReader2.Read())
										{
											DataRow dataRow = dataTable.NewRow();
											dataRow["CONSTRAINT_CATALOG"] = strCatalog;
											dataRow["CONSTRAINT_NAME"] = string.Format(CultureInfo.InvariantCulture, "FK_{0}_{1}", sqliteDataReader[2], sqliteDataReader2.GetInt32(0));
											dataRow["TABLE_CATALOG"] = strCatalog;
											dataRow["TABLE_NAME"] = sqliteCommandBuilder.UnquoteIdentifier(sqliteDataReader.GetString(2));
											dataRow["CONSTRAINT_TYPE"] = "FOREIGN KEY";
											dataRow["IS_DEFERRABLE"] = false;
											dataRow["INITIALLY_DEFERRED"] = false;
											dataRow["FKEY_FROM_COLUMN"] = sqliteCommandBuilder.UnquoteIdentifier(sqliteDataReader2[3].ToString());
											dataRow["FKEY_TO_CATALOG"] = strCatalog;
											dataRow["FKEY_TO_TABLE"] = sqliteCommandBuilder.UnquoteIdentifier(sqliteDataReader2[2].ToString());
											dataRow["FKEY_TO_COLUMN"] = sqliteCommandBuilder.UnquoteIdentifier(sqliteDataReader2[4].ToString());
											dataRow["FKEY_FROM_ORDINAL_POSITION"] = sqliteDataReader2[1];
											if (string.IsNullOrEmpty(strKeyName) || string.Compare(strKeyName, dataRow["CONSTRAINT_NAME"].ToString(), true, CultureInfo.InvariantCulture) == 0)
											{
												dataTable.Rows.Add(dataRow);
											}
										}
									}
								}
							}
						}
						catch (SqliteException)
						{
						}
					}
				}
			}
			dataTable.EndLoadData();
			dataTable.AcceptChanges();
			return dataTable;
		}

		private void UpdateCallback(IntPtr puser, int type, IntPtr database, IntPtr table, long rowid)
		{
			this._updateHandler(this, new UpdateEventArgs(SqliteConvert.UTF8ToString(database, -1), SqliteConvert.UTF8ToString(table, -1), (UpdateEventType)type, rowid));
		}

		private int CommitCallback(IntPtr parg)
		{
			CommitEventArgs e = new CommitEventArgs();
			this._commitHandler(this, e);
			return e.AbortTransaction ? 1 : 0;
		}

		private void RollbackCallback(IntPtr parg)
		{
			this._rollbackHandler(this, EventArgs.Empty);
		}
	}
}
