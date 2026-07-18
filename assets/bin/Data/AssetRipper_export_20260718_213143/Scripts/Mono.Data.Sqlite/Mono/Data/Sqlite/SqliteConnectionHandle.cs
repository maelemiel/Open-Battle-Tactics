using System;
using System.Runtime.InteropServices;

namespace Mono.Data.Sqlite
{
	internal class SqliteConnectionHandle : CriticalHandle
	{
		public override bool IsInvalid
		{
			get
			{
				return handle == IntPtr.Zero;
			}
		}

		private SqliteConnectionHandle(IntPtr db)
			: this()
		{
			SetHandle(db);
		}

		internal SqliteConnectionHandle()
			: base(IntPtr.Zero)
		{
		}

		protected override bool ReleaseHandle()
		{
			try
			{
				SQLiteBase.CloseConnection(this);
			}
			catch (SqliteException)
			{
			}
			return true;
		}

		public static implicit operator IntPtr(SqliteConnectionHandle db)
		{
			return db.handle;
		}

		public static implicit operator SqliteConnectionHandle(IntPtr db)
		{
			return new SqliteConnectionHandle(db);
		}
	}
}
