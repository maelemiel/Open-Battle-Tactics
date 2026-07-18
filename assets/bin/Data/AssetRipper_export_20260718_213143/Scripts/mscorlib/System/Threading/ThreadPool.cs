using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Threading
{
	public static class ThreadPool
	{
		[Obsolete("This method is obsolete, use BindHandle(SafeHandle) instead")]
		public static bool BindHandle(IntPtr osHandle)
		{
			return true;
		}

		public static bool BindHandle(SafeHandle osHandle)
		{
			if (osHandle == null)
			{
				throw new ArgumentNullException("osHandle");
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetMaxThreads(out int workerThreads, out int completionPortThreads);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetMinThreads(out int workerThreads, out int completionPortThreads);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[MonoTODO("The min number of completion port threads is not evaluated.")]
		public static extern bool SetMinThreads(int workerThreads, int completionPortThreads);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[MonoTODO("The max number of threads cannot be decremented.")]
		public static extern bool SetMaxThreads(int workerThreads, int completionPortThreads);

		public static bool QueueUserWorkItem(WaitCallback callBack)
		{
			return QueueUserWorkItem(callBack, null);
		}

		public static bool QueueUserWorkItem(WaitCallback callBack, object state)
		{
			if (callBack == null)
			{
				throw new ArgumentNullException("callBack");
			}
			IAsyncResult asyncResult = callBack.BeginInvoke(state, null, null);
			if (asyncResult == null)
			{
				return false;
			}
			return true;
		}

		public static RegisteredWaitHandle RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, int millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			return RegisterWaitForSingleObject(waitObject, callBack, state, (long)millisecondsTimeOutInterval, executeOnlyOnce);
		}

		public static RegisteredWaitHandle RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, long millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			if (millisecondsTimeOutInterval < -1)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout < -1");
			}
			if (millisecondsTimeOutInterval > int.MaxValue)
			{
				throw new NotSupportedException("Timeout is too big. Maximum is Int32.MaxValue");
			}
			TimeSpan timeout = new TimeSpan(0, 0, 0, 0, (int)millisecondsTimeOutInterval);
			RegisteredWaitHandle registeredWaitHandle = new RegisteredWaitHandle(waitObject, callBack, state, timeout, executeOnlyOnce);
			QueueUserWorkItem(registeredWaitHandle.Wait, null);
			return registeredWaitHandle;
		}

		public static RegisteredWaitHandle RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, TimeSpan timeout, bool executeOnlyOnce)
		{
			return RegisterWaitForSingleObject(waitObject, callBack, state, (long)timeout.TotalMilliseconds, executeOnlyOnce);
		}

		[CLSCompliant(false)]
		public static RegisteredWaitHandle RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callBack, object state, uint millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			return RegisterWaitForSingleObject(waitObject, callBack, state, (long)millisecondsTimeOutInterval, executeOnlyOnce);
		}
	}
}
