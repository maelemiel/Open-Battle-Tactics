using System;
using System.Runtime.InteropServices;

namespace Mono.Data.Sqlite
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void SQLiteFinalCallback(IntPtr context);
}
