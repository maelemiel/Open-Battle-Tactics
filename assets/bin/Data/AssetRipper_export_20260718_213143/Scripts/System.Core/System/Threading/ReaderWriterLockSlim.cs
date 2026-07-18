namespace System.Threading
{
	public class ReaderWriterLockSlim : IDisposable
	{
		private sealed class LockDetails
		{
			public int ThreadId;

			public int ReadLocks;
		}

		private static readonly bool smp;

		private int myLock;

		private int owners;

		private Thread upgradable_thread;

		private Thread write_thread;

		private uint numWriteWaiters;

		private uint numReadWaiters;

		private uint numUpgradeWaiters;

		private EventWaitHandle writeEvent;

		private EventWaitHandle readEvent;

		private EventWaitHandle upgradeEvent;

		private readonly LockRecursionPolicy recursionPolicy;

		private LockDetails[] read_locks = new LockDetails[8];

		public bool IsReadLockHeld
		{
			get
			{
				return RecursiveReadCount != 0;
			}
		}

		public bool IsWriteLockHeld
		{
			get
			{
				return RecursiveWriteCount != 0;
			}
		}

		public bool IsUpgradeableReadLockHeld
		{
			get
			{
				return RecursiveUpgradeCount != 0;
			}
		}

		public int CurrentReadCount
		{
			get
			{
				return owners & 0xFFFFFFF;
			}
		}

		public int RecursiveReadCount
		{
			get
			{
				EnterMyLock();
				LockDetails readLockDetails = GetReadLockDetails(Thread.CurrentThread.ManagedThreadId, false);
				int result = ((readLockDetails != null) ? readLockDetails.ReadLocks : 0);
				ExitMyLock();
				return result;
			}
		}

		public int RecursiveUpgradeCount
		{
			get
			{
				return (upgradable_thread == Thread.CurrentThread) ? 1 : 0;
			}
		}

		public int RecursiveWriteCount
		{
			get
			{
				return (write_thread == Thread.CurrentThread) ? 1 : 0;
			}
		}

		public int WaitingReadCount
		{
			get
			{
				return (int)numReadWaiters;
			}
		}

		public int WaitingUpgradeCount
		{
			get
			{
				return (int)numUpgradeWaiters;
			}
		}

		public int WaitingWriteCount
		{
			get
			{
				return (int)numWriteWaiters;
			}
		}

		public LockRecursionPolicy RecursionPolicy
		{
			get
			{
				return recursionPolicy;
			}
		}

		private bool MyLockHeld
		{
			get
			{
				return myLock != 0;
			}
		}

		public ReaderWriterLockSlim()
		{
		}

		public ReaderWriterLockSlim(LockRecursionPolicy recursionPolicy)
		{
			this.recursionPolicy = recursionPolicy;
			if (recursionPolicy != LockRecursionPolicy.NoRecursion)
			{
				throw new NotImplementedException("recursionPolicy != NoRecursion not currently implemented");
			}
		}

		static ReaderWriterLockSlim()
		{
			smp = Environment.ProcessorCount > 1;
		}

		public void EnterReadLock()
		{
			TryEnterReadLock(-1);
		}

		public bool TryEnterReadLock(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			if (read_locks == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (Thread.CurrentThread == write_thread)
			{
				throw new LockRecursionException("Read lock cannot be acquired while write lock is held");
			}
			EnterMyLock();
			LockDetails readLockDetails = GetReadLockDetails(Thread.CurrentThread.ManagedThreadId, true);
			if (readLockDetails.ReadLocks != 0)
			{
				ExitMyLock();
				throw new LockRecursionException("Recursive read lock can only be aquired in SupportsRecursion mode");
			}
			readLockDetails.ReadLocks++;
			while (true)
			{
				if (owners >= 0 && numWriteWaiters == 0)
				{
					owners++;
					ExitMyLock();
					return true;
				}
				if (millisecondsTimeout == 0)
				{
					ExitMyLock();
					return false;
				}
				if (readEvent == null)
				{
					LazyCreateEvent(ref readEvent, false);
				}
				else if (!WaitOnEvent(readEvent, ref numReadWaiters, millisecondsTimeout))
				{
					break;
				}
			}
			return false;
		}

		public bool TryEnterReadLock(TimeSpan timeout)
		{
			return TryEnterReadLock(CheckTimeout(timeout));
		}

		public void ExitReadLock()
		{
			EnterMyLock();
			if (owners < 1)
			{
				ExitMyLock();
				throw new SynchronizationLockException("Releasing lock and no read lock taken");
			}
			owners--;
			GetReadLockDetails(Thread.CurrentThread.ManagedThreadId, false).ReadLocks--;
			ExitAndWakeUpAppropriateWaiters();
		}

		public void EnterWriteLock()
		{
			TryEnterWriteLock(-1);
		}

		public bool TryEnterWriteLock(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			if (read_locks == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (IsWriteLockHeld)
			{
				throw new LockRecursionException();
			}
			EnterMyLock();
			LockDetails readLockDetails = GetReadLockDetails(Thread.CurrentThread.ManagedThreadId, false);
			if (readLockDetails != null && readLockDetails.ReadLocks > 0)
			{
				ExitMyLock();
				throw new LockRecursionException("Write lock cannot be acquired while read lock is held");
			}
			while (true)
			{
				if (owners == 0)
				{
					owners = -1;
					write_thread = Thread.CurrentThread;
					break;
				}
				if (owners == 1 && upgradable_thread == Thread.CurrentThread)
				{
					owners = -1;
					write_thread = Thread.CurrentThread;
					break;
				}
				if (millisecondsTimeout == 0)
				{
					ExitMyLock();
					return false;
				}
				if (upgradable_thread == Thread.CurrentThread)
				{
					if (upgradeEvent == null)
					{
						LazyCreateEvent(ref upgradeEvent, false);
						continue;
					}
					if (numUpgradeWaiters != 0)
					{
						ExitMyLock();
						throw new ApplicationException("Upgrading lock to writer lock already in process, deadlock");
					}
					if (!WaitOnEvent(upgradeEvent, ref numUpgradeWaiters, millisecondsTimeout))
					{
						return false;
					}
				}
				else if (writeEvent == null)
				{
					LazyCreateEvent(ref writeEvent, true);
				}
				else if (!WaitOnEvent(writeEvent, ref numWriteWaiters, millisecondsTimeout))
				{
					return false;
				}
			}
			ExitMyLock();
			return true;
		}

		public bool TryEnterWriteLock(TimeSpan timeout)
		{
			return TryEnterWriteLock(CheckTimeout(timeout));
		}

		public void ExitWriteLock()
		{
			EnterMyLock();
			if (owners != -1)
			{
				ExitMyLock();
				throw new SynchronizationLockException("Calling ExitWriterLock when no write lock is held");
			}
			if (upgradable_thread == Thread.CurrentThread)
			{
				owners = 1;
			}
			else
			{
				owners = 0;
			}
			write_thread = null;
			ExitAndWakeUpAppropriateWaiters();
		}

		public void EnterUpgradeableReadLock()
		{
			TryEnterUpgradeableReadLock(-1);
		}

		public bool TryEnterUpgradeableReadLock(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			if (read_locks == null)
			{
				throw new ObjectDisposedException(null);
			}
			if (IsUpgradeableReadLockHeld)
			{
				throw new LockRecursionException();
			}
			if (IsWriteLockHeld)
			{
				throw new LockRecursionException();
			}
			EnterMyLock();
			while (true)
			{
				if (owners == 0 && numWriteWaiters == 0 && upgradable_thread == null)
				{
					owners++;
					upgradable_thread = Thread.CurrentThread;
					ExitMyLock();
					return true;
				}
				if (millisecondsTimeout == 0)
				{
					ExitMyLock();
					return false;
				}
				if (readEvent == null)
				{
					LazyCreateEvent(ref readEvent, false);
				}
				else if (!WaitOnEvent(readEvent, ref numReadWaiters, millisecondsTimeout))
				{
					break;
				}
			}
			return false;
		}

		public bool TryEnterUpgradeableReadLock(TimeSpan timeout)
		{
			return TryEnterUpgradeableReadLock(CheckTimeout(timeout));
		}

		public void ExitUpgradeableReadLock()
		{
			EnterMyLock();
			owners--;
			upgradable_thread = null;
			ExitAndWakeUpAppropriateWaiters();
		}

		public void Dispose()
		{
			read_locks = null;
		}

		private void EnterMyLock()
		{
			if (Interlocked.CompareExchange(ref myLock, 1, 0) != 0)
			{
				EnterMyLockSpin();
			}
		}

		private void EnterMyLockSpin()
		{
			int num = 0;
			while (true)
			{
				if (num < 3 && smp)
				{
					Thread.SpinWait(20);
				}
				else
				{
					Thread.Sleep(0);
				}
				if (Interlocked.CompareExchange(ref myLock, 1, 0) == 0)
				{
					break;
				}
				num++;
			}
		}

		private void ExitMyLock()
		{
			myLock = 0;
		}

		private void ExitAndWakeUpAppropriateWaiters()
		{
			if (owners == 1 && numUpgradeWaiters != 0)
			{
				ExitMyLock();
				upgradeEvent.Set();
			}
			else if (owners == 0 && numWriteWaiters != 0)
			{
				ExitMyLock();
				writeEvent.Set();
			}
			else if (owners >= 0 && numReadWaiters != 0)
			{
				ExitMyLock();
				readEvent.Set();
			}
			else
			{
				ExitMyLock();
			}
		}

		private void LazyCreateEvent(ref EventWaitHandle waitEvent, bool makeAutoResetEvent)
		{
			ExitMyLock();
			EventWaitHandle eventWaitHandle = ((!makeAutoResetEvent) ? ((EventWaitHandle)new ManualResetEvent(false)) : ((EventWaitHandle)new AutoResetEvent(false)));
			EnterMyLock();
			if (waitEvent == null)
			{
				waitEvent = eventWaitHandle;
			}
		}

		private bool WaitOnEvent(EventWaitHandle waitEvent, ref uint numWaiters, int millisecondsTimeout)
		{
			waitEvent.Reset();
			numWaiters++;
			bool flag = false;
			ExitMyLock();
			try
			{
				flag = waitEvent.WaitOne(millisecondsTimeout, false);
			}
			finally
			{
				EnterMyLock();
				numWaiters--;
				if (!flag)
				{
					ExitMyLock();
				}
			}
			return flag;
		}

		private static int CheckTimeout(TimeSpan timeout)
		{
			try
			{
				return checked((int)timeout.TotalMilliseconds);
			}
			catch (OverflowException)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
		}

		private LockDetails GetReadLockDetails(int threadId, bool create)
		{
			int i;
			LockDetails lockDetails;
			for (i = 0; i < read_locks.Length; i++)
			{
				lockDetails = read_locks[i];
				if (lockDetails == null)
				{
					break;
				}
				if (lockDetails.ThreadId == threadId)
				{
					return lockDetails;
				}
			}
			if (!create)
			{
				return null;
			}
			if (i == read_locks.Length)
			{
				Array.Resize(ref read_locks, read_locks.Length * 2);
			}
			lockDetails = (read_locks[i] = new LockDetails());
			lockDetails.ThreadId = threadId;
			return lockDetails;
		}
	}
}
