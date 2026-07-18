using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public class Overlapped
	{
		private IAsyncResult ares;

		private int offsetL;

		private int offsetH;

		private int evt;

		private IntPtr evt_ptr;

		public IAsyncResult AsyncResult
		{
			get
			{
				return ares;
			}
			set
			{
				ares = value;
			}
		}

		[Obsolete("Not 64bit compatible.  Use EventHandleIntPtr instead.", false)]
		public int EventHandle
		{
			get
			{
				return evt;
			}
			set
			{
				evt = value;
			}
		}

		[ComVisible(false)]
		public IntPtr EventHandleIntPtr
		{
			get
			{
				return evt_ptr;
			}
			set
			{
				evt_ptr = value;
			}
		}

		public int OffsetHigh
		{
			get
			{
				return offsetH;
			}
			set
			{
				offsetH = value;
			}
		}

		public int OffsetLow
		{
			get
			{
				return offsetL;
			}
			set
			{
				offsetL = value;
			}
		}

		public Overlapped()
		{
		}

		[Obsolete("Not 64bit compatible.  Please use the constructor that takes IntPtr for the event handle", false)]
		public Overlapped(int offsetLo, int offsetHi, int hEvent, IAsyncResult ar)
		{
			offsetL = offsetLo;
			offsetH = offsetHi;
			evt = hEvent;
			ares = ar;
		}

		public Overlapped(int offsetLo, int offsetHi, IntPtr hEvent, IAsyncResult ar)
		{
			offsetL = offsetLo;
			offsetH = offsetHi;
			evt_ptr = hEvent;
			ares = ar;
		}

		[CLSCompliant(false)]
		public unsafe static void Free(NativeOverlapped* nativeOverlappedPtr)
		{
			if ((IntPtr)nativeOverlappedPtr == IntPtr.Zero)
			{
				throw new ArgumentNullException("nativeOverlappedPtr");
			}
			Marshal.FreeHGlobal((IntPtr)nativeOverlappedPtr);
		}

		[CLSCompliant(false)]
		public unsafe static Overlapped Unpack(NativeOverlapped* nativeOverlappedPtr)
		{
			if ((IntPtr)nativeOverlappedPtr == IntPtr.Zero)
			{
				throw new ArgumentNullException("nativeOverlappedPtr");
			}
			Overlapped overlapped = new Overlapped();
			overlapped.offsetL = nativeOverlappedPtr->OffsetLow;
			overlapped.offsetH = nativeOverlappedPtr->OffsetHigh;
			overlapped.evt = (int)nativeOverlappedPtr->EventHandle;
			return overlapped;
		}

		[MonoTODO("Security - we need to propagate the call stack")]
		[Obsolete("Use Pack(iocb, userData) instead", false)]
		[CLSCompliant(false)]
		public unsafe NativeOverlapped* Pack(IOCompletionCallback iocb)
		{
			NativeOverlapped* ptr = (NativeOverlapped*)(void*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
			ptr->OffsetLow = offsetL;
			ptr->OffsetHigh = offsetH;
			ptr->EventHandle = (IntPtr)evt;
			return ptr;
		}

		[MonoTODO("handle userData")]
		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe NativeOverlapped* Pack(IOCompletionCallback iocb, object userData)
		{
			NativeOverlapped* ptr = (NativeOverlapped*)(void*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeOverlapped)));
			ptr->OffsetLow = offsetL;
			ptr->OffsetHigh = offsetH;
			ptr->EventHandle = evt_ptr;
			return ptr;
		}

		[Obsolete("Use UnsafePack(iocb, userData) instead", false)]
		[CLSCompliant(false)]
		public unsafe NativeOverlapped* UnsafePack(IOCompletionCallback iocb)
		{
			return Pack(iocb);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe NativeOverlapped* UnsafePack(IOCompletionCallback iocb, object userData)
		{
			return Pack(iocb, userData);
		}
	}
}
