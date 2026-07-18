using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public struct LockCookie
	{
		internal int ThreadId;

		internal int ReaderLocks;

		internal int WriterLocks;

		internal LockCookie(int thread_id)
		{
			ThreadId = thread_id;
			ReaderLocks = 0;
			WriterLocks = 0;
		}

		internal LockCookie(int thread_id, int reader_locks, int writer_locks)
		{
			ThreadId = thread_id;
			ReaderLocks = reader_locks;
			WriterLocks = writer_locks;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(LockCookie obj)
		{
			if (ThreadId == obj.ThreadId && ReaderLocks == obj.ReaderLocks && WriterLocks == obj.WriterLocks)
			{
				return true;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is LockCookie))
			{
				return false;
			}
			return obj.Equals(this);
		}

		public static bool operator ==(LockCookie a, LockCookie b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(LockCookie a, LockCookie b)
		{
			return !a.Equals(b);
		}
	}
}
