using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace System.Runtime.InteropServices
{
	public abstract class SafeHandle : CriticalFinalizerObject, IDisposable
	{
		protected IntPtr handle;

		private IntPtr invalid_handle_value;

		private int refcount;

		private bool owns_handle;

		public bool IsClosed
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return refcount <= 0;
			}
		}

		public abstract bool IsInvalid
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get;
		}

		protected SafeHandle()
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected SafeHandle(IntPtr invalidHandleValue, bool ownsHandle)
		{
			invalid_handle_value = invalidHandleValue;
			owns_handle = ownsHandle;
			refcount = 1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Close()
		{
			if (refcount == 0)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			int num;
			int num2;
			do
			{
				num = refcount;
				num2 = num - 1;
			}
			while (Interlocked.CompareExchange(ref refcount, num2, num) != num);
			if (num2 == 0 && owns_handle && !IsInvalid)
			{
				ReleaseHandle();
				handle = invalid_handle_value;
				refcount = -1;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void DangerousAddRef(ref bool success)
		{
			if (refcount <= 0)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			int num;
			int value;
			do
			{
				num = refcount;
				value = num + 1;
				if (num <= 0)
				{
					throw new ObjectDisposedException(GetType().FullName);
				}
			}
			while (Interlocked.CompareExchange(ref refcount, value, num) != num);
			success = true;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public IntPtr DangerousGetHandle()
		{
			if (refcount <= 0)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			return handle;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void DangerousRelease()
		{
			if (refcount <= 0)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
			int num;
			int num2;
			do
			{
				num = refcount;
				num2 = num - 1;
			}
			while (Interlocked.CompareExchange(ref refcount, num2, num) != num);
			if (num2 == 0 && owns_handle && !IsInvalid)
			{
				ReleaseHandle();
				handle = invalid_handle_value;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public void SetHandleAsInvalid()
		{
			handle = invalid_handle_value;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected abstract bool ReleaseHandle();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected void SetHandle(IntPtr handle)
		{
			this.handle = handle;
		}

		~SafeHandle()
		{
			if (owns_handle && !IsInvalid)
			{
				ReleaseHandle();
				handle = invalid_handle_value;
			}
		}
	}
}
