using System;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace Mono.Data.Sqlite
{
	internal class SQLite3 : SQLiteBase
	{
		protected SqliteConnectionHandle _sql;

		protected string _fileName;

		protected bool _usePool;

		protected int _poolVersion;

		private bool _buildingSchema;

		protected SqliteFunction[] _functionsArray;

		internal override string Version
		{
			get
			{
				return SQLiteVersion;
			}
		}

		internal static string SQLiteVersion
		{
			get
			{
				return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_libversion(), -1);
			}
		}

		internal override int Changes
		{
			get
			{
				return UnsafeNativeMethods.sqlite3_changes(_sql);
			}
		}

		internal SQLite3(SQLiteDateFormats fmt)
			: base(fmt)
		{
		}

		protected override void Dispose(bool bDisposing)
		{
			if (bDisposing)
			{
				Close();
			}
		}

		internal override void Close()
		{
			if (_sql != null)
			{
				if (_usePool)
				{
					SQLiteBase.ResetConnection(_sql);
					SqliteConnectionPool.Add(_fileName, _sql, _poolVersion);
				}
				else
				{
					_sql.Dispose();
				}
			}
			_sql = null;
		}

		internal override void Cancel()
		{
			UnsafeNativeMethods.sqlite3_interrupt(_sql);
		}

		internal override void Open(string strFilename, SQLiteOpenFlagsEnum flags, int maxPoolSize, bool usePool)
		{
			if (_sql != null)
			{
				return;
			}
			_usePool = usePool;
			if (usePool)
			{
				_fileName = strFilename;
				_sql = SqliteConnectionPool.Remove(strFilename, maxPoolSize, out _poolVersion);
			}
			if (_sql == null)
			{
				int num;
				IntPtr db;
				try
				{
					num = UnsafeNativeMethods.sqlite3_open_v2(SqliteConvert.ToUTF8(strFilename), out db, (int)flags, IntPtr.Zero);
				}
				catch (EntryPointNotFoundException)
				{
					Console.WriteLine("Your sqlite3 version is old - please upgrade to at least v3.5.0!");
					num = UnsafeNativeMethods.sqlite3_open(SqliteConvert.ToUTF8(strFilename), out db);
				}
				if (num > 0)
				{
					throw new SqliteException(num, null);
				}
				_sql = db;
			}
			_functionsArray = SqliteFunction.BindFunctions(this);
			SetTimeout(0);
		}

		internal override void ClearPool()
		{
			SqliteConnectionPool.ClearPool(_fileName);
		}

		internal override void SetTimeout(int nTimeoutMS)
		{
			int num = UnsafeNativeMethods.sqlite3_busy_timeout(_sql, nTimeoutMS);
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override bool Step(SqliteStatement stmt)
		{
			Random random = null;
			uint tickCount = (uint)Environment.TickCount;
			uint num = (uint)(stmt._command._commandTimeout * 1000);
			while (true)
			{
				int num2 = UnsafeNativeMethods.sqlite3_step(stmt._sqlite_stmt);
				if (num2 == 100)
				{
					return true;
				}
				if (num2 == 101)
				{
					break;
				}
				if (num2 <= 0)
				{
					continue;
				}
				int num3 = Reset(stmt);
				switch (num3)
				{
				case 0:
					throw new SqliteException(num2, SQLiteLastError());
				case 5:
				case 6:
					if (stmt._command != null)
					{
						if (random == null)
						{
							random = new Random();
						}
						if ((uint)(Environment.TickCount - (int)tickCount) > num)
						{
							throw new SqliteException(num3, SQLiteLastError());
						}
						Thread.CurrentThread.Join(random.Next(1, 150));
					}
					break;
				}
			}
			return false;
		}

		internal override int Reset(SqliteStatement stmt)
		{
			int num = UnsafeNativeMethods.sqlite3_reset(stmt._sqlite_stmt);
			if (num == 17)
			{
				string strRemain;
				using (SqliteStatement sqliteStatement = Prepare(null, stmt._sqlStatement, null, (uint)(stmt._command._commandTimeout * 1000), out strRemain))
				{
					stmt._sqlite_stmt.Dispose();
					stmt._sqlite_stmt = sqliteStatement._sqlite_stmt;
					sqliteStatement._sqlite_stmt = null;
					stmt.BindParameters();
				}
				return -1;
			}
			if (num == 6 || num == 5)
			{
				return num;
			}
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
			return 0;
		}

		internal override string SQLiteLastError()
		{
			return SQLiteBase.SQLiteLastError(_sql);
		}

		internal override SqliteStatement Prepare(SqliteConnection cnn, string strSql, SqliteStatement previous, uint timeoutMS, out string strRemain)
		{
			IntPtr stmt = IntPtr.Zero;
			IntPtr ptrRemain = IntPtr.Zero;
			int nativestringlen = 0;
			int num = 17;
			int num2 = 0;
			byte[] array = SqliteConvert.ToUTF8(strSql);
			string text = null;
			SqliteStatement sqliteStatement = null;
			Random random = null;
			uint tickCount = (uint)Environment.TickCount;
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			IntPtr pSql = gCHandle.AddrOfPinnedObject();
			try
			{
				while ((num == 17 || num == 6 || num == 5) && num2 < 3)
				{
					num = UnsafeNativeMethods.sqlite3_prepare(_sql, pSql, array.Length - 1, out stmt, out ptrRemain);
					nativestringlen = -1;
					switch (num)
					{
					case 17:
						num2++;
						break;
					case 1:
						if (string.Compare(SQLiteLastError(), "near \"TYPES\": syntax error", StringComparison.OrdinalIgnoreCase) == 0)
						{
							int num3 = strSql.IndexOf(';');
							if (num3 == -1)
							{
								num3 = strSql.Length - 1;
							}
							text = strSql.Substring(0, num3 + 1);
							strSql = strSql.Substring(num3 + 1);
							strRemain = string.Empty;
							while (sqliteStatement == null && strSql.Length > 0)
							{
								sqliteStatement = Prepare(cnn, strSql, previous, timeoutMS, out strRemain);
								strSql = strRemain;
							}
							if (sqliteStatement != null)
							{
								sqliteStatement.SetTypes(text);
							}
							return sqliteStatement;
						}
						if (_buildingSchema || string.Compare(SQLiteLastError(), 0, "no such table: TEMP.SCHEMA", 0, 26, StringComparison.OrdinalIgnoreCase) != 0)
						{
							break;
						}
						strRemain = string.Empty;
						_buildingSchema = true;
						try
						{
							ISQLiteSchemaExtensions iSQLiteSchemaExtensions = ((IServiceProvider)SqliteFactory.Instance).GetService(typeof(ISQLiteSchemaExtensions)) as ISQLiteSchemaExtensions;
							if (iSQLiteSchemaExtensions != null)
							{
								iSQLiteSchemaExtensions.BuildTempSchema(cnn);
							}
							while (sqliteStatement == null && strSql.Length > 0)
							{
								sqliteStatement = Prepare(cnn, strSql, previous, timeoutMS, out strRemain);
								strSql = strRemain;
							}
							return sqliteStatement;
						}
						finally
						{
							_buildingSchema = false;
						}
					case 5:
					case 6:
						if (random == null)
						{
							random = new Random();
						}
						if ((uint)(Environment.TickCount - (int)tickCount) > timeoutMS)
						{
							throw new SqliteException(num, SQLiteLastError());
						}
						Thread.CurrentThread.Join(random.Next(1, 150));
						break;
					}
				}
				if (num > 0)
				{
					throw new SqliteException(num, SQLiteLastError());
				}
				strRemain = SqliteConvert.UTF8ToString(ptrRemain, nativestringlen);
				if (stmt != IntPtr.Zero)
				{
					sqliteStatement = new SqliteStatement(this, stmt, strSql.Substring(0, strSql.Length - strRemain.Length), previous);
				}
				return sqliteStatement;
			}
			finally
			{
				gCHandle.Free();
			}
		}

		internal override void Bind_Double(SqliteStatement stmt, int index, double value)
		{
			int num = UnsafeNativeMethods.sqlite3_bind_double(stmt._sqlite_stmt, index, value);
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void Bind_Int32(SqliteStatement stmt, int index, int value)
		{
			int num = UnsafeNativeMethods.sqlite3_bind_int(stmt._sqlite_stmt, index, value);
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void Bind_Int64(SqliteStatement stmt, int index, long value)
		{
			int num = UnsafeNativeMethods.sqlite3_bind_int64(stmt._sqlite_stmt, index, value);
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void Bind_Text(SqliteStatement stmt, int index, string value)
		{
			byte[] array = SqliteConvert.ToUTF8(value);
			int num = UnsafeNativeMethods.sqlite3_bind_text(stmt._sqlite_stmt, index, array, array.Length - 1, (IntPtr)(-1));
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void Bind_DateTime(SqliteStatement stmt, int index, DateTime dt)
		{
			byte[] array = ToUTF8(dt);
			int num = UnsafeNativeMethods.sqlite3_bind_text(stmt._sqlite_stmt, index, array, array.Length - 1, (IntPtr)(-1));
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void Bind_Blob(SqliteStatement stmt, int index, byte[] blobData)
		{
			int num = UnsafeNativeMethods.sqlite3_bind_blob(stmt._sqlite_stmt, index, blobData, blobData.Length, (IntPtr)(-1));
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void Bind_Null(SqliteStatement stmt, int index)
		{
			int num = UnsafeNativeMethods.sqlite3_bind_null(stmt._sqlite_stmt, index);
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override int Bind_ParamCount(SqliteStatement stmt)
		{
			return UnsafeNativeMethods.sqlite3_bind_parameter_count(stmt._sqlite_stmt);
		}

		internal override string Bind_ParamName(SqliteStatement stmt, int index)
		{
			return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_bind_parameter_name(stmt._sqlite_stmt, index), -1);
		}

		internal override int Bind_ParamIndex(SqliteStatement stmt, string paramName)
		{
			return UnsafeNativeMethods.sqlite3_bind_parameter_index(stmt._sqlite_stmt, SqliteConvert.ToUTF8(paramName));
		}

		internal override int ColumnCount(SqliteStatement stmt)
		{
			return UnsafeNativeMethods.sqlite3_column_count(stmt._sqlite_stmt);
		}

		internal override string ColumnName(SqliteStatement stmt, int index)
		{
			return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_column_name(stmt._sqlite_stmt, index), -1);
		}

		internal override TypeAffinity ColumnAffinity(SqliteStatement stmt, int index)
		{
			return UnsafeNativeMethods.sqlite3_column_type(stmt._sqlite_stmt, index);
		}

		internal override string ColumnType(SqliteStatement stmt, int index, out TypeAffinity nAffinity)
		{
			int nativestringlen = -1;
			IntPtr intPtr = UnsafeNativeMethods.sqlite3_column_decltype(stmt._sqlite_stmt, index);
			nAffinity = ColumnAffinity(stmt, index);
			if (intPtr != IntPtr.Zero)
			{
				return SqliteConvert.UTF8ToString(intPtr, nativestringlen);
			}
			string[] typeDefinitions = stmt.TypeDefinitions;
			if (typeDefinitions != null && index < typeDefinitions.Length && typeDefinitions[index] != null)
			{
				return typeDefinitions[index];
			}
			return string.Empty;
		}

		internal override int ColumnIndex(SqliteStatement stmt, string columnName)
		{
			int num = ColumnCount(stmt);
			for (int i = 0; i < num; i++)
			{
				if (string.Compare(columnName, ColumnName(stmt, i), true, CultureInfo.InvariantCulture) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		internal override string ColumnOriginalName(SqliteStatement stmt, int index)
		{
			return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_column_origin_name(stmt._sqlite_stmt, index), -1);
		}

		internal override string ColumnDatabaseName(SqliteStatement stmt, int index)
		{
			return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_column_database_name(stmt._sqlite_stmt, index), -1);
		}

		internal override string ColumnTableName(SqliteStatement stmt, int index)
		{
			return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_column_table_name(stmt._sqlite_stmt, index), -1);
		}

		internal override void ColumnMetaData(string dataBase, string table, string column, out string dataType, out string collateSequence, out bool notNull, out bool primaryKey, out bool autoIncrement)
		{
			int nativestringlen = -1;
			int nativestringlen2 = -1;
			IntPtr ptrDataType;
			IntPtr ptrCollSeq;
			int notNull2;
			int primaryKey2;
			int autoInc;
			int num = UnsafeNativeMethods.sqlite3_table_column_metadata(_sql, SqliteConvert.ToUTF8(dataBase), SqliteConvert.ToUTF8(table), SqliteConvert.ToUTF8(column), out ptrDataType, out ptrCollSeq, out notNull2, out primaryKey2, out autoInc);
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
			dataType = SqliteConvert.UTF8ToString(ptrDataType, nativestringlen);
			collateSequence = SqliteConvert.UTF8ToString(ptrCollSeq, nativestringlen2);
			notNull = notNull2 == 1;
			primaryKey = primaryKey2 == 1;
			autoIncrement = autoInc == 1;
		}

		internal override double GetDouble(SqliteStatement stmt, int index)
		{
			return UnsafeNativeMethods.sqlite3_column_double(stmt._sqlite_stmt, index);
		}

		internal override int GetInt32(SqliteStatement stmt, int index)
		{
			return UnsafeNativeMethods.sqlite3_column_int(stmt._sqlite_stmt, index);
		}

		internal override long GetInt64(SqliteStatement stmt, int index)
		{
			return UnsafeNativeMethods.sqlite3_column_int64(stmt._sqlite_stmt, index);
		}

		internal override string GetText(SqliteStatement stmt, int index)
		{
			return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_column_text(stmt._sqlite_stmt, index), -1);
		}

		internal override DateTime GetDateTime(SqliteStatement stmt, int index)
		{
			return ToDateTime(UnsafeNativeMethods.sqlite3_column_text(stmt._sqlite_stmt, index), -1);
		}

		internal unsafe override long GetBytes(SqliteStatement stmt, int index, int nDataOffset, byte[] bDest, int nStart, int nLength)
		{
			int num = nLength;
			int num2 = UnsafeNativeMethods.sqlite3_column_bytes(stmt._sqlite_stmt, index);
			IntPtr intPtr = UnsafeNativeMethods.sqlite3_column_blob(stmt._sqlite_stmt, index);
			if (bDest == null)
			{
				return num2;
			}
			if (num + nStart > bDest.Length)
			{
				num = bDest.Length - nStart;
			}
			if (num + nDataOffset > num2)
			{
				num = num2 - nDataOffset;
			}
			if (num > 0)
			{
				Marshal.Copy((IntPtr)((byte*)(void*)intPtr + nDataOffset), bDest, nStart, num);
			}
			else
			{
				num = 0;
			}
			return num;
		}

		internal override long GetChars(SqliteStatement stmt, int index, int nDataOffset, char[] bDest, int nStart, int nLength)
		{
			int num = nLength;
			string text = GetText(stmt, index);
			int length = text.Length;
			if (bDest == null)
			{
				return length;
			}
			if (num + nStart > bDest.Length)
			{
				num = bDest.Length - nStart;
			}
			if (num + nDataOffset > length)
			{
				num = length - nDataOffset;
			}
			if (num > 0)
			{
				text.CopyTo(nDataOffset, bDest, nStart, num);
			}
			else
			{
				num = 0;
			}
			return num;
		}

		internal override bool IsNull(SqliteStatement stmt, int index)
		{
			return ColumnAffinity(stmt, index) == TypeAffinity.Null;
		}

		internal override int AggregateCount(IntPtr context)
		{
			return UnsafeNativeMethods.sqlite3_aggregate_count(context);
		}

		internal override void CreateFunction(string strFunction, int nArgs, bool needCollSeq, SQLiteCallback func, SQLiteCallback funcstep, SQLiteFinalCallback funcfinal)
		{
			int num = UnsafeNativeMethods.sqlite3_create_function(_sql, SqliteConvert.ToUTF8(strFunction), nArgs, 4, IntPtr.Zero, func, funcstep, funcfinal);
			if (num == 0)
			{
				num = UnsafeNativeMethods.sqlite3_create_function(_sql, SqliteConvert.ToUTF8(strFunction), nArgs, 1, IntPtr.Zero, func, funcstep, funcfinal);
			}
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void CreateCollation(string strCollation, SQLiteCollation func, SQLiteCollation func16)
		{
			int num = UnsafeNativeMethods.sqlite3_create_collation(_sql, SqliteConvert.ToUTF8(strCollation), 2, IntPtr.Zero, func16);
			if (num == 0)
			{
				UnsafeNativeMethods.sqlite3_create_collation(_sql, SqliteConvert.ToUTF8(strCollation), 1, IntPtr.Zero, func);
			}
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override int ContextCollateCompare(CollationEncodingEnum enc, IntPtr context, string s1, string s2)
		{
			throw new NotImplementedException();
		}

		internal override int ContextCollateCompare(CollationEncodingEnum enc, IntPtr context, char[] c1, char[] c2)
		{
			throw new NotImplementedException();
		}

		internal override CollationSequence GetCollationSequence(SqliteFunction func, IntPtr context)
		{
			throw new NotImplementedException();
		}

		internal unsafe override long GetParamValueBytes(IntPtr p, int nDataOffset, byte[] bDest, int nStart, int nLength)
		{
			int num = nLength;
			int num2 = UnsafeNativeMethods.sqlite3_value_bytes(p);
			IntPtr intPtr = UnsafeNativeMethods.sqlite3_value_blob(p);
			if (bDest == null)
			{
				return num2;
			}
			if (num + nStart > bDest.Length)
			{
				num = bDest.Length - nStart;
			}
			if (num + nDataOffset > num2)
			{
				num = num2 - nDataOffset;
			}
			if (num > 0)
			{
				Marshal.Copy((IntPtr)((byte*)(void*)intPtr + nDataOffset), bDest, nStart, num);
			}
			else
			{
				num = 0;
			}
			return num;
		}

		internal override double GetParamValueDouble(IntPtr ptr)
		{
			return UnsafeNativeMethods.sqlite3_value_double(ptr);
		}

		internal override int GetParamValueInt32(IntPtr ptr)
		{
			return UnsafeNativeMethods.sqlite3_value_int(ptr);
		}

		internal override long GetParamValueInt64(IntPtr ptr)
		{
			return UnsafeNativeMethods.sqlite3_value_int64(ptr);
		}

		internal override string GetParamValueText(IntPtr ptr)
		{
			return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_value_text(ptr), -1);
		}

		internal override TypeAffinity GetParamValueType(IntPtr ptr)
		{
			return UnsafeNativeMethods.sqlite3_value_type(ptr);
		}

		internal override void ReturnBlob(IntPtr context, byte[] value)
		{
			UnsafeNativeMethods.sqlite3_result_blob(context, value, value.Length, (IntPtr)(-1));
		}

		internal override void ReturnDouble(IntPtr context, double value)
		{
			UnsafeNativeMethods.sqlite3_result_double(context, value);
		}

		internal override void ReturnError(IntPtr context, string value)
		{
			UnsafeNativeMethods.sqlite3_result_error(context, SqliteConvert.ToUTF8(value), value.Length);
		}

		internal override void ReturnInt32(IntPtr context, int value)
		{
			UnsafeNativeMethods.sqlite3_result_int(context, value);
		}

		internal override void ReturnInt64(IntPtr context, long value)
		{
			UnsafeNativeMethods.sqlite3_result_int64(context, value);
		}

		internal override void ReturnNull(IntPtr context)
		{
			UnsafeNativeMethods.sqlite3_result_null(context);
		}

		internal override void ReturnText(IntPtr context, string value)
		{
			byte[] array = SqliteConvert.ToUTF8(value);
			UnsafeNativeMethods.sqlite3_result_text(context, SqliteConvert.ToUTF8(value), array.Length - 1, (IntPtr)(-1));
		}

		internal override IntPtr AggregateContext(IntPtr context)
		{
			return UnsafeNativeMethods.sqlite3_aggregate_context(context, 1);
		}

		internal override void SetPassword(byte[] passwordBytes)
		{
			int num = UnsafeNativeMethods.sqlite3_key(_sql, passwordBytes, passwordBytes.Length);
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void ChangePassword(byte[] newPasswordBytes)
		{
			int num = UnsafeNativeMethods.sqlite3_rekey(_sql, newPasswordBytes, (newPasswordBytes != null) ? newPasswordBytes.Length : 0);
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override void SetUpdateHook(SQLiteUpdateCallback func)
		{
			UnsafeNativeMethods.sqlite3_update_hook(_sql, func, IntPtr.Zero);
		}

		internal override void SetCommitHook(SQLiteCommitCallback func)
		{
			UnsafeNativeMethods.sqlite3_commit_hook(_sql, func, IntPtr.Zero);
		}

		internal override void SetRollbackHook(SQLiteRollbackCallback func)
		{
			UnsafeNativeMethods.sqlite3_rollback_hook(_sql, func, IntPtr.Zero);
		}

		internal override object GetValue(SqliteStatement stmt, int index, SQLiteType typ)
		{
			if (IsNull(stmt, index))
			{
				return DBNull.Value;
			}
			TypeAffinity typeAffinity = typ.Affinity;
			Type type = null;
			if (typ.Type != DbType.Object)
			{
				type = SqliteConvert.SQLiteTypeToType(typ);
				typeAffinity = SqliteConvert.TypeToAffinity(type);
			}
			switch (typeAffinity)
			{
			case TypeAffinity.Blob:
			{
				if (typ.Type == DbType.Guid && typ.Affinity == TypeAffinity.Text)
				{
					return new Guid(GetText(stmt, index));
				}
				int num = (int)GetBytes(stmt, index, 0, null, 0, 0);
				byte[] array = new byte[num];
				GetBytes(stmt, index, 0, array, 0, num);
				if (typ.Type == DbType.Guid && num == 16)
				{
					return new Guid(array);
				}
				return array;
			}
			case TypeAffinity.DateTime:
				return GetDateTime(stmt, index);
			case TypeAffinity.Double:
				if (type == null)
				{
					return GetDouble(stmt, index);
				}
				return Convert.ChangeType(GetDouble(stmt, index), type, null);
			case TypeAffinity.Int64:
				if (type == null)
				{
					return GetInt64(stmt, index);
				}
				return Convert.ChangeType(GetInt64(stmt, index), type, null);
			default:
				return GetText(stmt, index);
			}
		}

		internal override int GetCursorForTable(SqliteStatement stmt, int db, int rootPage)
		{
			return -1;
		}

		internal override long GetRowIdForCursor(SqliteStatement stmt, int cursor)
		{
			return 0L;
		}

		internal override void GetIndexColumnExtendedInfo(string database, string index, string column, out int sortMode, out int onError, out string collationSequence)
		{
			sortMode = 0;
			onError = 2;
			collationSequence = "BINARY";
		}
	}
}
