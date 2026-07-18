using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Mono.Data.Sqlite
{
	[SuppressUnmanagedCodeSecurity]
	internal static class UnsafeNativeMethods
	{
		private const string SQLITE_DLL = "sqlite3";

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_close(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_create_function(IntPtr db, byte[] strName, int nArgs, int nType, IntPtr pvUser, SQLiteCallback func, SQLiteCallback fstep, SQLiteFinalCallback ffinal);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_finalize(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_open_v2(byte[] utf8Filename, out IntPtr db, int flags, IntPtr vfs);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_open(byte[] utf8Filename, out IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int sqlite3_open16(string fileName, out IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_reset(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_bind_parameter_name(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_database_name(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_database_name16(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_decltype(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_decltype16(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_name(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_name16(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_origin_name(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_origin_name16(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_table_name(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_table_name16(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_text(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_text16(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_errmsg(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_prepare(IntPtr db, IntPtr pSql, int nBytes, out IntPtr stmt, out IntPtr ptrRemain);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_table_column_metadata(IntPtr db, byte[] dbName, byte[] tblName, byte[] colName, out IntPtr ptrDataType, out IntPtr ptrCollSeq, out int notNull, out int primaryKey, out int autoInc);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_value_text(IntPtr p);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_value_text16(IntPtr p);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_libversion();

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void sqlite3_interrupt(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_changes(IntPtr db);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_busy_timeout(IntPtr db, int ms);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_bind_blob(IntPtr stmt, int index, byte[] value, int nSize, IntPtr nTransient);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_bind_double(IntPtr stmt, int index, double value);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_bind_int(IntPtr stmt, int index, int value);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_bind_int64(IntPtr stmt, int index, long value);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_bind_null(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_bind_text(IntPtr stmt, int index, byte[] value, int nlen, IntPtr pvReserved);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_bind_parameter_count(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_bind_parameter_index(IntPtr stmt, byte[] strName);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_column_count(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_step(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double sqlite3_column_double(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_column_int(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern long sqlite3_column_int64(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_column_blob(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_column_bytes(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern TypeAffinity sqlite3_column_type(IntPtr stmt, int index);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_create_collation(IntPtr db, byte[] strName, int nType, IntPtr pvUser, SQLiteCollation func);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_aggregate_count(IntPtr context);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_value_blob(IntPtr p);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_value_bytes(IntPtr p);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern double sqlite3_value_double(IntPtr p);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_value_int(IntPtr p);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern long sqlite3_value_int64(IntPtr p);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern TypeAffinity sqlite3_value_type(IntPtr p);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void sqlite3_result_blob(IntPtr context, byte[] value, int nSize, IntPtr pvReserved);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void sqlite3_result_double(IntPtr context, double value);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void sqlite3_result_error(IntPtr context, byte[] strErr, int nLen);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void sqlite3_result_int(IntPtr context, int value);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void sqlite3_result_int64(IntPtr context, long value);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void sqlite3_result_null(IntPtr context);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void sqlite3_result_text(IntPtr context, byte[] value, int nLen, IntPtr pvReserved);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_aggregate_context(IntPtr context, int nBytes);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern int sqlite3_bind_text16(IntPtr stmt, int index, string value, int nlen, IntPtr pvReserved);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern void sqlite3_result_error16(IntPtr context, string strName, int nLen);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern void sqlite3_result_text16(IntPtr context, string strName, int nLen, IntPtr pvReserved);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_key(IntPtr db, byte[] key, int keylen);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_rekey(IntPtr db, byte[] key, int keylen);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_update_hook(IntPtr db, SQLiteUpdateCallback func, IntPtr pvUser);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_commit_hook(IntPtr db, SQLiteCommitCallback func, IntPtr pvUser);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_rollback_hook(IntPtr db, SQLiteRollbackCallback func, IntPtr pvUser);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_db_handle(IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr sqlite3_next_stmt(IntPtr db, IntPtr stmt);

		[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sqlite3_exec(IntPtr db, byte[] strSql, IntPtr pvCallback, IntPtr pvParam, out IntPtr errMsg);
	}
}
