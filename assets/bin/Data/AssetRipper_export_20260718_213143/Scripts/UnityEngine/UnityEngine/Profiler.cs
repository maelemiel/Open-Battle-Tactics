using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Profiler
	{
		public static extern bool supported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern string logFile
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool enableBinaryLog
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern uint usedHeapSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Conditional("ENABLE_PROFILER")]
		[WrapperlessIcall]
		public static extern void AddFramesFromFile(string file);

		[Conditional("ENABLE_PROFILER")]
		public static void BeginSample(string name)
		{
			BeginSampleOnly(name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Conditional("ENABLE_PROFILER")]
		[WrapperlessIcall]
		public static extern void BeginSample(string name, Object targetObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void BeginSampleOnly(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Conditional("ENABLE_PROFILER")]
		[WrapperlessIcall]
		public static extern void EndSample();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int GetRuntimeMemorySize(Object o);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern uint GetMonoHeapSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern uint GetMonoUsedSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern uint GetTotalAllocatedMemory();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern uint GetTotalUnusedReservedMemory();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern uint GetTotalReservedMemory();
	}
}
