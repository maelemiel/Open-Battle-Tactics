using System;

namespace Mono.Data.Sqlite
{
	[Flags]
	internal enum SQLiteOpenFlagsEnum
	{
		None = 0,
		ReadOnly = 1,
		ReadWrite = 2,
		Create = 4,
		SharedCache = 0x1000000,
		Default = 6
	}
}
