using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace UnityEngine
{
	public sealed class ComputeBuffer : IDisposable
	{
		internal IntPtr m_Ptr;

		public extern int count
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int stride
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public ComputeBuffer(int count, int stride)
			: this(count, stride, ComputeBufferType.Default)
		{
		}

		public ComputeBuffer(int count, int stride, ComputeBufferType type)
		{
			m_Ptr = IntPtr.Zero;
			InitBuffer(this, count, stride, type);
		}

		~ComputeBuffer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			DestroyBuffer(this);
			m_Ptr = IntPtr.Zero;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void InitBuffer(ComputeBuffer buf, int count, int stride, ComputeBufferType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void DestroyBuffer(ComputeBuffer buf);

		public void Release()
		{
			Dispose();
		}

		[SecuritySafeCritical]
		public void SetData(Array data)
		{
			InternalSetData(data, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[SecurityCritical]
		[WrapperlessIcall]
		private extern void InternalSetData(Array data, int elemSize);

		[SecuritySafeCritical]
		public void GetData(Array data)
		{
			InternalGetData(data, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[SecurityCritical]
		[WrapperlessIcall]
		private extern void InternalGetData(Array data, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffset);
	}
}
