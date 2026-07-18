using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public sealed class ReaderWriterLock : CriticalFinalizerObject
	{
		private int seq_num = 1;

		private int state;

		private int readers;

		private LockQueue writer_queue;

		private Hashtable reader_locks;

		private int writer_lock_owner;

		public bool IsReaderLockHeld
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				lock (this)
				{
					return reader_locks.ContainsKey(Thread.CurrentThreadId);
				}
			}
		}

		public bool IsWriterLockHeld
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				lock (this)
				{
					return state < 0 && Thread.CurrentThreadId == writer_lock_owner;
				}
			}
		}

		public int WriterSeqNum
		{
			get
			{
				lock (this)
				{
					return seq_num;
				}
			}
		}

		public ReaderWriterLock()
		{
			writer_queue = new LockQueue(this);
			reader_locks = new Hashtable();
			GC.SuppressFinalize(this);
		}

		[MonoTODO]
		~ReaderWriterLock()
		{
		}

		public void AcquireReaderLock(int millisecondsTimeout)
		{
			AcquireReaderLock(millisecondsTimeout, 1);
		}

		private void AcquireReaderLock(int millisecondsTimeout, int initialLockCount)
		{
			lock (this)
			{
				if (HasWriterLock())
				{
					AcquireWriterLock(millisecondsTimeout, initialLockCount);
					return;
				}
				object obj = reader_locks[Thread.CurrentThreadId];
				if (obj == null)
				{
					readers++;
					try
					{
						if (state < 0 || !writer_queue.IsEmpty)
						{
							do
							{
								if (!Monitor.Wait(this, millisecondsTimeout))
								{
									throw new ApplicationException("Timeout expired");
								}
							}
							while (state < 0);
						}
					}
					finally
					{
						readers--;
					}
					reader_locks[Thread.CurrentThreadId] = initialLockCount;
					state += initialLockCount;
				}
				else
				{
					reader_locks[Thread.CurrentThreadId] = (int)obj + 1;
					state++;
				}
			}
		}

		public void AcquireReaderLock(TimeSpan timeout)
		{
			int millisecondsTimeout = CheckTimeout(timeout);
			AcquireReaderLock(millisecondsTimeout, 1);
		}

		public void AcquireWriterLock(int millisecondsTimeout)
		{
			AcquireWriterLock(millisecondsTimeout, 1);
		}

		private void AcquireWriterLock(int millisecondsTimeout, int initialLockCount)
		{
			lock (this)
			{
				if (HasWriterLock())
				{
					state--;
					return;
				}
				if (state != 0 || !writer_queue.IsEmpty)
				{
					do
					{
						if (!writer_queue.Wait(millisecondsTimeout))
						{
							throw new ApplicationException("Timeout expired");
						}
					}
					while (state != 0);
				}
				state = -initialLockCount;
				writer_lock_owner = Thread.CurrentThreadId;
				seq_num++;
			}
		}

		public void AcquireWriterLock(TimeSpan timeout)
		{
			int millisecondsTimeout = CheckTimeout(timeout);
			AcquireWriterLock(millisecondsTimeout, 1);
		}

		public bool AnyWritersSince(int seqNum)
		{
			lock (this)
			{
				return seq_num > seqNum;
			}
		}

		public void DowngradeFromWriterLock(ref LockCookie lockCookie)
		{
			lock (this)
			{
				if (!HasWriterLock())
				{
					throw new ApplicationException("The thread does not have the writer lock.");
				}
				state = lockCookie.ReaderLocks;
				reader_locks[Thread.CurrentThreadId] = state;
				if (readers > 0)
				{
					Monitor.PulseAll(this);
				}
			}
		}

		public LockCookie ReleaseLock()
		{
			LockCookie lockCookie;
			lock (this)
			{
				lockCookie = GetLockCookie();
				if (lockCookie.WriterLocks != 0)
				{
					ReleaseWriterLock(lockCookie.WriterLocks);
				}
				else if (lockCookie.ReaderLocks != 0)
				{
					ReleaseReaderLock(lockCookie.ReaderLocks, lockCookie.ReaderLocks);
				}
			}
			return lockCookie;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void ReleaseReaderLock()
		{
			lock (this)
			{
				if (HasWriterLock())
				{
					ReleaseWriterLock();
					return;
				}
				if (state > 0)
				{
					object obj = reader_locks[Thread.CurrentThreadId];
					if (obj != null)
					{
						ReleaseReaderLock((int)obj, 1);
						return;
					}
				}
				throw new ApplicationException("The thread does not have any reader or writer locks.");
			}
		}

		private void ReleaseReaderLock(int currentCount, int releaseCount)
		{
			int num = currentCount - releaseCount;
			if (num == 0)
			{
				reader_locks.Remove(Thread.CurrentThreadId);
			}
			else
			{
				reader_locks[Thread.CurrentThreadId] = num;
			}
			state -= releaseCount;
			if (state == 0 && !writer_queue.IsEmpty)
			{
				writer_queue.Pulse();
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void ReleaseWriterLock()
		{
			lock (this)
			{
				if (!HasWriterLock())
				{
					throw new ApplicationException("The thread does not have the writer lock.");
				}
				ReleaseWriterLock(1);
			}
		}

		private void ReleaseWriterLock(int releaseCount)
		{
			state += releaseCount;
			if (state == 0)
			{
				if (readers > 0)
				{
					Monitor.PulseAll(this);
				}
				else if (!writer_queue.IsEmpty)
				{
					writer_queue.Pulse();
				}
			}
		}

		public void RestoreLock(ref LockCookie lockCookie)
		{
			lock (this)
			{
				if (lockCookie.WriterLocks != 0)
				{
					AcquireWriterLock(-1, lockCookie.WriterLocks);
				}
				else if (lockCookie.ReaderLocks != 0)
				{
					AcquireReaderLock(-1, lockCookie.ReaderLocks);
				}
			}
		}

		public LockCookie UpgradeToWriterLock(int millisecondsTimeout)
		{
			LockCookie lockCookie;
			lock (this)
			{
				lockCookie = GetLockCookie();
				if (lockCookie.WriterLocks != 0)
				{
					state--;
					return lockCookie;
				}
				if (lockCookie.ReaderLocks != 0)
				{
					ReleaseReaderLock(lockCookie.ReaderLocks, lockCookie.ReaderLocks);
				}
			}
			AcquireWriterLock(millisecondsTimeout);
			return lockCookie;
		}

		public LockCookie UpgradeToWriterLock(TimeSpan timeout)
		{
			int millisecondsTimeout = CheckTimeout(timeout);
			return UpgradeToWriterLock(millisecondsTimeout);
		}

		private LockCookie GetLockCookie()
		{
			LockCookie result = new LockCookie(Thread.CurrentThreadId);
			if (HasWriterLock())
			{
				result.WriterLocks = -state;
			}
			else
			{
				object obj = reader_locks[Thread.CurrentThreadId];
				if (obj != null)
				{
					result.ReaderLocks = (int)obj;
				}
			}
			return result;
		}

		private bool HasWriterLock()
		{
			return state < 0 && Thread.CurrentThreadId == writer_lock_owner;
		}

		private int CheckTimeout(TimeSpan timeout)
		{
			int num = (int)timeout.TotalMilliseconds;
			if (num < -1)
			{
				throw new ArgumentOutOfRangeException("timeout", "Number must be either non-negative or -1");
			}
			return num;
		}
	}
}
