using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Threading
{
	[ComVisible(false)]
	public sealed class Semaphore : WaitHandle
	{
		private Semaphore(IntPtr handle)
		{
			Handle = handle;
		}

		public Semaphore(int initialCount, int maximumCount)
			: this(initialCount, maximumCount, null)
		{
		}

		public Semaphore(int initialCount, int maximumCount, string name)
		{
			if (initialCount < 0)
			{
				throw new ArgumentOutOfRangeException("initialCount", "< 0");
			}
			if (maximumCount < 1)
			{
				throw new ArgumentOutOfRangeException("maximumCount", "< 1");
			}
			if (initialCount > maximumCount)
			{
				throw new ArgumentException("initialCount > maximumCount");
			}
			bool createdNew;
			Handle = CreateSemaphore_internal(initialCount, maximumCount, name, out createdNew);
		}

		public Semaphore(int initialCount, int maximumCount, string name, out bool createdNew)
			: this(initialCount, maximumCount, name, out createdNew, null)
		{
		}

		[System.MonoTODO("Does not support access control, semaphoreSecurity is ignored")]
		public Semaphore(int initialCount, int maximumCount, string name, out bool createdNew, SemaphoreSecurity semaphoreSecurity)
		{
			if (initialCount < 0)
			{
				throw new ArgumentOutOfRangeException("initialCount", "< 0");
			}
			if (maximumCount < 1)
			{
				throw new ArgumentOutOfRangeException("maximumCount", "< 1");
			}
			if (initialCount > maximumCount)
			{
				throw new ArgumentException("initialCount > maximumCount");
			}
			Handle = CreateSemaphore_internal(initialCount, maximumCount, name, out createdNew);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateSemaphore_internal(int initialCount, int maximumCount, string name, out bool createdNew);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ReleaseSemaphore_internal(IntPtr handle, int releaseCount, out bool fail);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr OpenSemaphore_internal(string name, SemaphoreRights rights, out System.IO.MonoIOError error);

		[System.MonoTODO]
		public SemaphoreSecurity GetAccessControl()
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[PrePrepareMethod]
		public int Release()
		{
			return Release(1);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public int Release(int releaseCount)
		{
			if (releaseCount < 1)
			{
				throw new ArgumentOutOfRangeException("releaseCount");
			}
			bool fail;
			int result = ReleaseSemaphore_internal(Handle, releaseCount, out fail);
			if (fail)
			{
				throw new SemaphoreFullException();
			}
			return result;
		}

		[System.MonoTODO]
		public void SetAccessControl(SemaphoreSecurity semaphoreSecurity)
		{
			if (semaphoreSecurity == null)
			{
				throw new ArgumentNullException("semaphoreSecurity");
			}
			throw new NotImplementedException();
		}

		public static Semaphore OpenExisting(string name)
		{
			return OpenExisting(name, SemaphoreRights.Modify | SemaphoreRights.Synchronize);
		}

		public unsafe static Semaphore OpenExisting(string name, SemaphoreRights rights)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0 || name.Length > 260)
			{
				throw new ArgumentException("name", Locale.GetText("Invalid length [1-260]."));
			}
			System.IO.MonoIOError error;
			IntPtr intPtr = OpenSemaphore_internal(name, rights, out error);
			if (intPtr == (IntPtr)(void*)null)
			{
				switch (error)
				{
				case System.IO.MonoIOError.ERROR_FILE_NOT_FOUND:
					throw new WaitHandleCannotBeOpenedException(Locale.GetText("Named Semaphore handle does not exist: ") + name);
				case System.IO.MonoIOError.ERROR_ACCESS_DENIED:
					throw new UnauthorizedAccessException();
				default:
					throw new IOException(Locale.GetText("Win32 IO error: ") + error);
				}
			}
			return new Semaphore(intPtr);
		}
	}
}
