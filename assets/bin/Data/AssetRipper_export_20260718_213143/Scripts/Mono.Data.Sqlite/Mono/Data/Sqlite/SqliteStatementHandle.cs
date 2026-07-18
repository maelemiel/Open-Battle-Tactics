using System;
using System.Runtime.InteropServices;

namespace Mono.Data.Sqlite
{
	internal class SqliteStatementHandle : CriticalHandle
	{
		public override bool IsInvalid
		{
			get
			{
				return handle == IntPtr.Zero;
			}
		}

		private SqliteStatementHandle(IntPtr stmt)
			: this()
		{
			SetHandle(stmt);
		}

		internal SqliteStatementHandle()
			: base(IntPtr.Zero)
		{
		}

		protected override bool ReleaseHandle()
		{
			try
			{
				SQLiteBase.FinalizeStatement(this);
			}
			catch (SqliteException)
			{
			}
			return true;
		}

		public static implicit operator IntPtr(SqliteStatementHandle stmt)
		{
			return stmt.handle;
		}

		public static implicit operator SqliteStatementHandle(IntPtr stmt)
		{
			return new SqliteStatementHandle(stmt);
		}
	}
}
