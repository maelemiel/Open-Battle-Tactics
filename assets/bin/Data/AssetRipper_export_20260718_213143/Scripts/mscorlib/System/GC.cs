using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace System
{
	public static class GC
	{
		public static extern int MaxGeneration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalCollect(int generation);

		public static void Collect()
		{
			InternalCollect(MaxGeneration);
		}

		public static void Collect(int generation)
		{
			if (generation < 0)
			{
				throw new ArgumentOutOfRangeException("generation");
			}
			InternalCollect(generation);
		}

		[MonoDocumentationNote("mode parameter ignored")]
		public static void Collect(int generation, GCCollectionMode mode)
		{
			Collect(generation);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetGeneration(object obj);

		public static int GetGeneration(WeakReference wo)
		{
			object target = wo.Target;
			if (target == null)
			{
				throw new ArgumentException();
			}
			return GetGeneration(target);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetTotalMemory(bool forceFullCollection);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern void KeepAlive(object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReRegisterForFinalize(object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern void SuppressFinalize(object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WaitForPendingFinalizers();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int CollectionCount(int generation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RecordPressure(long bytesAllocated);

		public static void AddMemoryPressure(long bytesAllocated)
		{
			RecordPressure(bytesAllocated);
		}

		public static void RemoveMemoryPressure(long bytesAllocated)
		{
			RecordPressure(-bytesAllocated);
		}
	}
}
