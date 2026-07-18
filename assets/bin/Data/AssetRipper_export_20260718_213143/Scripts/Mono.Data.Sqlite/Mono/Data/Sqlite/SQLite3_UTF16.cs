using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mono.Data.Sqlite
{
	internal class SQLite3_UTF16 : SQLite3
	{
		internal SQLite3_UTF16(SQLiteDateFormats fmt)
			: base(fmt)
		{
		}

		public override string ToString(IntPtr b, int nbytelen)
		{
			return UTF16ToString(b, nbytelen);
		}

		public static string UTF16ToString(IntPtr b, int nbytelen)
		{
			if (nbytelen == 0 || b == IntPtr.Zero)
			{
				return string.Empty;
			}
			if (nbytelen == -1)
			{
				return Marshal.PtrToStringUni(b);
			}
			return Marshal.PtrToStringUni(b, nbytelen / 2);
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
				if ((flags & SQLiteOpenFlagsEnum.Create) == 0 && !File.Exists(strFilename))
				{
					throw new SqliteException(14, strFilename);
				}
				IntPtr db;
				int num = UnsafeNativeMethods.sqlite3_open16(strFilename, out db);
				if (num > 0)
				{
					throw new SqliteException(num, null);
				}
				_sql = db;
			}
			_functionsArray = SqliteFunction.BindFunctions(this);
		}

		internal override void Bind_DateTime(SqliteStatement stmt, int index, DateTime dt)
		{
			Bind_Text(stmt, index, ToString(dt));
		}

		internal override void Bind_Text(SqliteStatement stmt, int index, string value)
		{
			int num = UnsafeNativeMethods.sqlite3_bind_text16(stmt._sqlite_stmt, index, value, value.Length * 2, (IntPtr)(-1));
			if (num > 0)
			{
				throw new SqliteException(num, SQLiteLastError());
			}
		}

		internal override DateTime GetDateTime(SqliteStatement stmt, int index)
		{
			return ToDateTime(GetText(stmt, index));
		}

		internal override string ColumnName(SqliteStatement stmt, int index)
		{
			return UTF16ToString(UnsafeNativeMethods.sqlite3_column_name16(stmt._sqlite_stmt, index), -1);
		}

		internal override string GetText(SqliteStatement stmt, int index)
		{
			return UTF16ToString(UnsafeNativeMethods.sqlite3_column_text16(stmt._sqlite_stmt, index), -1);
		}

		internal override string ColumnOriginalName(SqliteStatement stmt, int index)
		{
			return UTF16ToString(UnsafeNativeMethods.sqlite3_column_origin_name16(stmt._sqlite_stmt, index), -1);
		}

		internal override string ColumnDatabaseName(SqliteStatement stmt, int index)
		{
			return UTF16ToString(UnsafeNativeMethods.sqlite3_column_database_name16(stmt._sqlite_stmt, index), -1);
		}

		internal override string ColumnTableName(SqliteStatement stmt, int index)
		{
			return UTF16ToString(UnsafeNativeMethods.sqlite3_column_table_name16(stmt._sqlite_stmt, index), -1);
		}

		internal override string GetParamValueText(IntPtr ptr)
		{
			return UTF16ToString(UnsafeNativeMethods.sqlite3_value_text16(ptr), -1);
		}

		internal override void ReturnError(IntPtr context, string value)
		{
			UnsafeNativeMethods.sqlite3_result_error16(context, value, value.Length * 2);
		}

		internal override void ReturnText(IntPtr context, string value)
		{
			UnsafeNativeMethods.sqlite3_result_text16(context, value, value.Length * 2, (IntPtr)(-1));
		}
	}
}
