using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public class EventWaitHandle : WaitHandle
	{
		private EventWaitHandle(IntPtr handle)
		{
			Handle = handle;
		}

		public EventWaitHandle(bool initialState, EventResetMode mode)
		{
			bool manual = IsManualReset(mode);
			bool created;
			Handle = NativeEventCalls.CreateEvent_internal(manual, initialState, null, out created);
		}

		public EventWaitHandle(bool initialState, EventResetMode mode, string name)
		{
			bool manual = IsManualReset(mode);
			bool created;
			Handle = NativeEventCalls.CreateEvent_internal(manual, initialState, name, out created);
		}

		public EventWaitHandle(bool initialState, EventResetMode mode, string name, out bool createdNew)
		{
			bool manual = IsManualReset(mode);
			Handle = NativeEventCalls.CreateEvent_internal(manual, initialState, name, out createdNew);
		}

		private bool IsManualReset(EventResetMode mode)
		{
			if (mode < EventResetMode.AutoReset || mode > EventResetMode.ManualReset)
			{
				throw new ArgumentException("mode");
			}
			return mode == EventResetMode.ManualReset;
		}

		public bool Reset()
		{
			CheckDisposed();
			return NativeEventCalls.ResetEvent_internal(Handle);
		}

		public bool Set()
		{
			CheckDisposed();
			return NativeEventCalls.SetEvent_internal(Handle);
		}
	}
}
