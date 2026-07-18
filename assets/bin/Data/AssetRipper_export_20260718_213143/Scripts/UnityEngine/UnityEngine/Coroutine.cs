using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Coroutine : YieldInstruction
	{
		internal IntPtr m_Ptr;

		private Coroutine()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void ReleaseCoroutine();

		~Coroutine()
		{
			ReleaseCoroutine();
		}
	}
}
