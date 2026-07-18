using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;

namespace System.Threading
{
	[ComVisible(true)]
	public static class Monitor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Monitor_try_enter(object obj, int ms);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Enter(object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern void Exit(object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Monitor_pulse(object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Monitor_test_synchronised(object obj);

		public static void Pulse(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (!Monitor_test_synchronised(obj))
			{
				throw new SynchronizationLockException("Object is not synchronized");
			}
			Monitor_pulse(obj);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Monitor_pulse_all(object obj);

		public static void PulseAll(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (!Monitor_test_synchronised(obj))
			{
				throw new SynchronizationLockException("Object is not synchronized");
			}
			Monitor_pulse_all(obj);
		}

		public static bool TryEnter(object obj)
		{
			return TryEnter(obj, 0);
		}

		public static bool TryEnter(object obj, int millisecondsTimeout)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (millisecondsTimeout == -1)
			{
				Enter(obj);
				return true;
			}
			if (millisecondsTimeout < 0)
			{
				throw new ArgumentException("negative value for millisecondsTimeout", "millisecondsTimeout");
			}
			return Monitor_try_enter(obj, millisecondsTimeout);
		}

		public static bool TryEnter(object obj, TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1 || num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout out of range");
			}
			return TryEnter(obj, (int)num);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Monitor_wait(object obj, int ms);

		public static bool Wait(object obj)
		{
			return Wait(obj, -1);
		}

		public static bool Wait(object obj, int millisecondsTimeout)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", "timeout out of range");
			}
			if (!Monitor_test_synchronised(obj))
			{
				throw new SynchronizationLockException("Object is not synchronized");
			}
			return Monitor_wait(obj, millisecondsTimeout);
		}

		public static bool Wait(object obj, TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1 || num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout out of range");
			}
			return Wait(obj, (int)num);
		}

		public static bool Wait(object obj, int millisecondsTimeout, bool exitContext)
		{
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				return Wait(obj, millisecondsTimeout);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
		}

		public static bool Wait(object obj, TimeSpan timeout, bool exitContext)
		{
			try
			{
				if (exitContext)
				{
					SynchronizationAttribute.ExitContext();
				}
				return Wait(obj, timeout);
			}
			finally
			{
				if (exitContext)
				{
					SynchronizationAttribute.EnterContext();
				}
			}
		}
	}
}
