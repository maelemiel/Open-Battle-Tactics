using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System.Threading
{
	public static class Interlocked
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int CompareExchange(ref int location1, int value, int comparand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern object CompareExchange(ref object location1, object value, object comparand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float CompareExchange(ref float location1, float value, float comparand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int Decrement(ref int location);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long Decrement(ref long location);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int Increment(ref int location);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long Increment(ref long location);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int Exchange(ref int location1, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern object Exchange(ref object location1, object value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float Exchange(ref float location1, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long CompareExchange(ref long location1, long value, long comparand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern IntPtr CompareExchange(ref IntPtr location1, IntPtr value, IntPtr comparand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double CompareExchange(ref double location1, double value, double comparand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[ComVisible(false)]
		public static extern T CompareExchange<T>(ref T location1, T value, T comparand) where T : class;

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long Exchange(ref long location1, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern IntPtr Exchange(ref IntPtr location1, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Exchange(ref double location1, double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ComVisible(false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern T Exchange<T>(ref T location1, T value) where T : class;

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long Read(ref long location);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int Add(ref int location1, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern long Add(ref long location1, long value);
	}
}
