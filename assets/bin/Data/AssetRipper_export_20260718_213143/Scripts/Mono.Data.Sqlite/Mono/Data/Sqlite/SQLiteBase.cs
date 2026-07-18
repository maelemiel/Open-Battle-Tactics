using System;

namespace Mono.Data.Sqlite
{
	internal abstract class SQLiteBase : SqliteConvert, IDisposable
	{
		internal static object _lock = new object();

		internal abstract string Version { get; }

		internal abstract int Changes { get; }

		internal SQLiteBase(SQLiteDateFormats fmt)
			: base(fmt)
		{
		}

		internal abstract void Open(string strFilename, SQLiteOpenFlagsEnum flags, int maxPoolSize, bool usePool);

		internal abstract void Close();

		internal abstract void SetTimeout(int nTimeoutMS);

		internal abstract string SQLiteLastError();

		internal abstract void ClearPool();

		internal abstract SqliteStatement Prepare(SqliteConnection cnn, string strSql, SqliteStatement previous, uint timeoutMS, out string strRemain);

		internal abstract bool Step(SqliteStatement stmt);

		internal abstract int Reset(SqliteStatement stmt);

		internal abstract void Cancel();

		internal abstract void Bind_Double(SqliteStatement stmt, int index, double value);

		internal abstract void Bind_Int32(SqliteStatement stmt, int index, int value);

		internal abstract void Bind_Int64(SqliteStatement stmt, int index, long value);

		internal abstract void Bind_Text(SqliteStatement stmt, int index, string value);

		internal abstract void Bind_Blob(SqliteStatement stmt, int index, byte[] blobData);

		internal abstract void Bind_DateTime(SqliteStatement stmt, int index, DateTime dt);

		internal abstract void Bind_Null(SqliteStatement stmt, int index);

		internal abstract int Bind_ParamCount(SqliteStatement stmt);

		internal abstract string Bind_ParamName(SqliteStatement stmt, int index);

		internal abstract int Bind_ParamIndex(SqliteStatement stmt, string paramName);

		internal abstract int ColumnCount(SqliteStatement stmt);

		internal abstract string ColumnName(SqliteStatement stmt, int index);

		internal abstract TypeAffinity ColumnAffinity(SqliteStatement stmt, int index);

		internal abstract string ColumnType(SqliteStatement stmt, int index, out TypeAffinity nAffinity);

		internal abstract int ColumnIndex(SqliteStatement stmt, string columnName);

		internal abstract string ColumnOriginalName(SqliteStatement stmt, int index);

		internal abstract string ColumnDatabaseName(SqliteStatement stmt, int index);

		internal abstract string ColumnTableName(SqliteStatement stmt, int index);

		internal abstract void ColumnMetaData(string dataBase, string table, string column, out string dataType, out string collateSequence, out bool notNull, out bool primaryKey, out bool autoIncrement);

		internal abstract void GetIndexColumnExtendedInfo(string database, string index, string column, out int sortMode, out int onError, out string collationSequence);

		internal abstract double GetDouble(SqliteStatement stmt, int index);

		internal abstract int GetInt32(SqliteStatement stmt, int index);

		internal abstract long GetInt64(SqliteStatement stmt, int index);

		internal abstract string GetText(SqliteStatement stmt, int index);

		internal abstract long GetBytes(SqliteStatement stmt, int index, int nDataoffset, byte[] bDest, int nStart, int nLength);

		internal abstract long GetChars(SqliteStatement stmt, int index, int nDataoffset, char[] bDest, int nStart, int nLength);

		internal abstract DateTime GetDateTime(SqliteStatement stmt, int index);

		internal abstract bool IsNull(SqliteStatement stmt, int index);

		internal abstract void CreateCollation(string strCollation, SQLiteCollation func, SQLiteCollation func16);

		internal abstract void CreateFunction(string strFunction, int nArgs, bool needCollSeq, SQLiteCallback func, SQLiteCallback funcstep, SQLiteFinalCallback funcfinal);

		internal abstract CollationSequence GetCollationSequence(SqliteFunction func, IntPtr context);

		internal abstract int ContextCollateCompare(CollationEncodingEnum enc, IntPtr context, string s1, string s2);

		internal abstract int ContextCollateCompare(CollationEncodingEnum enc, IntPtr context, char[] c1, char[] c2);

		internal abstract int AggregateCount(IntPtr context);

		internal abstract IntPtr AggregateContext(IntPtr context);

		internal abstract long GetParamValueBytes(IntPtr ptr, int nDataOffset, byte[] bDest, int nStart, int nLength);

		internal abstract double GetParamValueDouble(IntPtr ptr);

		internal abstract int GetParamValueInt32(IntPtr ptr);

		internal abstract long GetParamValueInt64(IntPtr ptr);

		internal abstract string GetParamValueText(IntPtr ptr);

		internal abstract TypeAffinity GetParamValueType(IntPtr ptr);

		internal abstract void ReturnBlob(IntPtr context, byte[] value);

		internal abstract void ReturnDouble(IntPtr context, double value);

		internal abstract void ReturnError(IntPtr context, string value);

		internal abstract void ReturnInt32(IntPtr context, int value);

		internal abstract void ReturnInt64(IntPtr context, long value);

		internal abstract void ReturnNull(IntPtr context);

		internal abstract void ReturnText(IntPtr context, string value);

		internal abstract void SetPassword(byte[] passwordBytes);

		internal abstract void ChangePassword(byte[] newPasswordBytes);

		internal abstract void SetUpdateHook(SQLiteUpdateCallback func);

		internal abstract void SetCommitHook(SQLiteCommitCallback func);

		internal abstract void SetRollbackHook(SQLiteRollbackCallback func);

		internal abstract int GetCursorForTable(SqliteStatement stmt, int database, int rootPage);

		internal abstract long GetRowIdForCursor(SqliteStatement stmt, int cursor);

		internal abstract object GetValue(SqliteStatement stmt, int index, SQLiteType typ);

		protected virtual void Dispose(bool bDisposing)
		{
		}

		public void Dispose()
		{
			Dispose(true);
		}

		internal static string SQLiteLastError(SqliteConnectionHandle db)
		{
			return SqliteConvert.UTF8ToString(UnsafeNativeMethods.sqlite3_errmsg(db), -1);
		}

		internal static void FinalizeStatement(SqliteStatementHandle stmt)
		{
			lock (_lock)
			{
				int num = UnsafeNativeMethods.sqlite3_finalize(stmt);
				if (num > 0)
				{
					throw new SqliteException(num, null);
				}
			}
		}

		internal static void CloseConnection(SqliteConnectionHandle db)
		{
			lock (_lock)
			{
				ResetConnection(db);
				int num = UnsafeNativeMethods.sqlite3_close(db);
				if (num > 0)
				{
					throw new SqliteException(num, SQLiteLastError(db));
				}
			}
		}

		internal static void ResetConnection(SqliteConnectionHandle db)
		{
			lock (_lock)
			{
				IntPtr errMsg = IntPtr.Zero;
				do
				{
					errMsg = UnsafeNativeMethods.sqlite3_next_stmt(db, errMsg);
					if (errMsg != IntPtr.Zero)
					{
						UnsafeNativeMethods.sqlite3_reset(errMsg);
					}
				}
				while (errMsg != IntPtr.Zero);
				UnsafeNativeMethods.sqlite3_exec(db, SqliteConvert.ToUTF8("ROLLBACK"), IntPtr.Zero, IntPtr.Zero, out errMsg);
			}
		}
	}
}
