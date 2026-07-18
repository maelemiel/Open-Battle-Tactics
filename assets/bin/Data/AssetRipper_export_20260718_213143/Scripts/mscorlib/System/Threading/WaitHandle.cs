using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
	[ComVisible(true)]
	public abstract class WaitHandle : MarshalByRefObject, IDisposable
	{
		public const int WaitTimeout = 258;

		private SafeWaitHandle safe_wait_handle;

		protected static readonly IntPtr InvalidHandle = (IntPtr)(-1);

		private bool disposed;

		[Obsolete("In the profiles > 2.x, use SafeHandle instead of Handle")]
		public virtual IntPtr Handle
		{
			get
			{
				return safe_wait_handle.DangerousGetHandle();
			}
			set
			{
				if (value == InvalidHandle)
				{
					safe_wait_handle = new SafeWaitHandle(InvalidHandle, false);
				}
				else
				{
					safe_wait_handle = new SafeWaitHandle(value, true);
				}
			}
		}

		public SafeWaitHandle SafeWaitHandle
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				return safe_wait_handle;
			}
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			set
			{
				if (value == null)
				{
					safe_wait_handle = new SafeWaitHandle(InvalidHandle, false);
				}
				else
				{
					safe_wait_handle = value;
				}
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool WaitAll_internal(WaitHandle[] handles, int ms, bool exitContext);

		private static void CheckArray(WaitHandle[] handles, bool waitAll)
		{
			if (handles == null)
			{
				throw new ArgumentNullException("waitHandles");
			}
			int num = handles.Length;
			if (num > 64)
			{
				throw new NotSupportedException("Too many handles");
			}
			foreach (WaitHandle waitHandle in handles)
			{
				if (waitHandle == null)
				{
					throw new ArgumentNullException("waitHandles", "null handle");
				}
				if (waitHandle.safe_wait_handle == null)
				{
					throw new ArgumentException("null element found", "waitHandle");
				}
			}
		}

		public static bool WaitAll(WaitHandle[] waitHandles)
		{
			CheckArray(waitHandles, true);
			return WaitAll_internal(waitHandles, -1, false);
		}

		public static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
		{
			CheckArray(waitHandles, true);
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				return WaitAll_internal(waitHandles, millisecondsTimeout, false);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
		}

		public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
		{
			CheckArray(waitHandles, true);
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1 || num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				return WaitAll_internal(waitHandles, (int)num, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int WaitAny_internal(WaitHandle[] handles, int ms, bool exitContext);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles)
		{
			CheckArray(waitHandles, false);
			return WaitAny_internal(waitHandles, -1, false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext)
		{
			CheckArray(waitHandles, false);
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				return WaitAny_internal(waitHandles, millisecondsTimeout, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout)
		{
			return WaitAny(waitHandles, timeout, false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout)
		{
			return WaitAny(waitHandles, millisecondsTimeout, false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static int WaitAny(WaitHandle[] waitHandles, TimeSpan timeout, bool exitContext)
		{
			CheckArray(waitHandles, false);
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1 || num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				return WaitAny_internal(waitHandles, (int)num, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
		}

		public virtual void Close()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool WaitOne_internal(IntPtr handle, int ms, bool exitContext);

		protected virtual void Dispose(bool explicitDisposing)
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (safe_wait_handle == null)
			{
				return;
			}
			lock (this)
			{
				if (safe_wait_handle != null)
				{
					safe_wait_handle.Dispose();
				}
			}
		}

		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn)
		{
			return SignalAndWait(toSignal, toWaitOn, -1, false);
		}

		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, int millisecondsTimeout, bool exitContext)
		{
			if (toSignal == null)
			{
				throw new ArgumentNullException("toSignal");
			}
			if (toWaitOn == null)
			{
				throw new ArgumentNullException("toWaitOn");
			}
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			return SignalAndWait_Internal(toSignal.Handle, toWaitOn.Handle, millisecondsTimeout, exitContext);
		}

		public static bool SignalAndWait(WaitHandle toSignal, WaitHandle toWaitOn, TimeSpan timeout, bool exitContext)
		{
			double totalMilliseconds = timeout.TotalMilliseconds;
			if (totalMilliseconds > 2147483647.0)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return SignalAndWait(toSignal, toWaitOn, Convert.ToInt32(totalMilliseconds), false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SignalAndWait_Internal(IntPtr toSignal, IntPtr toWaitOn, int ms, bool exitContext);

		public virtual bool WaitOne()
		{
			CheckDisposed();
			bool success = false;
			try
			{
				safe_wait_handle.DangerousAddRef(ref success);
				return WaitOne_internal(safe_wait_handle.DangerousGetHandle(), -1, false);
			}
			finally
			{
				if (success)
				{
					safe_wait_handle.DangerousRelease();
				}
			}
		}

		public virtual bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			CheckDisposed();
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			bool success = false;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				safe_wait_handle.DangerousAddRef(ref success);
				return WaitOne_internal(safe_wait_handle.DangerousGetHandle(), millisecondsTimeout, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
				if (success)
				{
					safe_wait_handle.DangerousRelease();
				}
			}
		}

		public virtual bool WaitOne(int millisecondsTimeout)
		{
			return WaitOne(millisecondsTimeout, false);
		}

		public virtual bool WaitOne(TimeSpan timeout)
		{
			return WaitOne(timeout, false);
		}

		public virtual bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			CheckDisposed();
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1 || num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			bool success = false;
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				safe_wait_handle.DangerousAddRef(ref success);
				return WaitOne_internal(safe_wait_handle.DangerousGetHandle(), (int)num, exitContext);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
				if (success)
				{
					safe_wait_handle.DangerousRelease();
				}
			}
		}

		internal void CheckDisposed()
		{
			if (disposed || safe_wait_handle == null)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}

		public static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout)
		{
			return WaitAll(waitHandles, millisecondsTimeout, false);
		}

		public static bool WaitAll(WaitHandle[] waitHandles, TimeSpan timeout)
		{
			return WaitAll(waitHandles, timeout, false);
		}

		~WaitHandle()
		{
			Dispose(false);
		}
	}
}
