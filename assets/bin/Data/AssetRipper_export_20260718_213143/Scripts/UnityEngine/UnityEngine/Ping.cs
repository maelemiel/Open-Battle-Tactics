using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Ping
	{
		private IntPtr pingWrapper;

		public extern bool isDone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern string ip
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Ping(string address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void DestroyPing();

		~Ping()
		{
			DestroyPing();
		}
	}
}
