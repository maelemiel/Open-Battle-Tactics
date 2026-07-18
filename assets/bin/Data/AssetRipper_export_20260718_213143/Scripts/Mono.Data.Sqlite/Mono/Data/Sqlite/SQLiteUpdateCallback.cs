using System;
using System.Runtime.InteropServices;

namespace Mono.Data.Sqlite
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void SQLiteUpdateCallback(IntPtr puser, int type, IntPtr database, IntPtr table, long rowid);
}
