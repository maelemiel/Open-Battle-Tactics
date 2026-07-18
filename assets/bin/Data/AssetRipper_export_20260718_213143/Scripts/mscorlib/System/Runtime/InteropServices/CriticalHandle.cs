using System.Runtime.ConstrainedExecution;

namespace System.Runtime.InteropServices
{
	public abstract class CriticalHandle : CriticalFinalizerObject, IDisposable
	{
		protected IntPtr handle;

		private bool _disposed;

		public bool IsClosed
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return _disposed;
			}
		}

		public abstract bool IsInvalid
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected CriticalHandle(IntPtr invalidHandleValue)
		{
			handle = invalidHandleValue;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		~CriticalHandle()
		{
			Dispose(false);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Close()
		{
			Dispose(true);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Dispose()
		{
			Dispose(true);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;
				if (!IsInvalid && disposing && !IsInvalid && !ReleaseHandle())
				{
					GC.SuppressFinalize(this);
				}
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected abstract bool ReleaseHandle();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected void SetHandle(IntPtr handle)
		{
			this.handle = handle;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void SetHandleAsInvalid()
		{
			_disposed = true;
		}
	}
}
