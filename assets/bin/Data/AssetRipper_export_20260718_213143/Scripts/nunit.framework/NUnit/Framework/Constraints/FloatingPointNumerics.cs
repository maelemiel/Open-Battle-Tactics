using System;
using System.Runtime.InteropServices;

namespace NUnit.Framework.Constraints
{
	public class FloatingPointNumerics
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntUnion
		{
			[FieldOffset(0)]
			public float Float;

			[FieldOffset(0)]
			public int Int;

			[FieldOffset(0)]
			public uint UInt;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongUnion
		{
			[FieldOffset(0)]
			public double Double;

			[FieldOffset(0)]
			public long Long;

			[FieldOffset(0)]
			public ulong ULong;
		}

		public static bool AreAlmostEqualUlps(float left, float right, int maxUlps)
		{
			FloatIntUnion floatIntUnion = default(FloatIntUnion);
			FloatIntUnion floatIntUnion2 = default(FloatIntUnion);
			floatIntUnion.Float = left;
			floatIntUnion2.Float = right;
			uint num = floatIntUnion.UInt >> 31;
			uint num2 = floatIntUnion2.UInt >> 31;
			uint num3 = (uint)(int.MinValue - (int)floatIntUnion.UInt) & num;
			floatIntUnion.UInt = num3 | (floatIntUnion.UInt & ~num);
			uint num4 = (uint)(int.MinValue - (int)floatIntUnion2.UInt) & num2;
			floatIntUnion2.UInt = num4 | (floatIntUnion2.UInt & ~num2);
			return Math.Abs(floatIntUnion.Int - floatIntUnion2.Int) <= maxUlps;
		}

		public static bool AreAlmostEqualUlps(double left, double right, long maxUlps)
		{
			DoubleLongUnion doubleLongUnion = default(DoubleLongUnion);
			DoubleLongUnion doubleLongUnion2 = default(DoubleLongUnion);
			doubleLongUnion.Double = left;
			doubleLongUnion2.Double = right;
			ulong num = doubleLongUnion.ULong >> 63;
			ulong num2 = doubleLongUnion2.ULong >> 63;
			ulong num3 = (ulong)(long.MinValue - (long)doubleLongUnion.ULong) & num;
			doubleLongUnion.ULong = num3 | (doubleLongUnion.ULong & ~num);
			ulong num4 = (ulong)(long.MinValue - (long)doubleLongUnion2.ULong) & num2;
			doubleLongUnion2.ULong = num4 | (doubleLongUnion2.ULong & ~num2);
			return Math.Abs(doubleLongUnion.Long - doubleLongUnion2.Long) <= maxUlps;
		}

		public static int ReinterpretAsInt(float value)
		{
			FloatIntUnion floatIntUnion = new FloatIntUnion
			{
				Float = value
			};
			return floatIntUnion.Int;
		}

		public static long ReinterpretAsLong(double value)
		{
			DoubleLongUnion doubleLongUnion = new DoubleLongUnion
			{
				Double = value
			};
			return doubleLongUnion.Long;
		}

		public static float ReinterpretAsFloat(int value)
		{
			FloatIntUnion floatIntUnion = new FloatIntUnion
			{
				Int = value
			};
			return floatIntUnion.Float;
		}

		public static double ReinterpretAsDouble(long value)
		{
			DoubleLongUnion doubleLongUnion = new DoubleLongUnion
			{
				Long = value
			};
			return doubleLongUnion.Double;
		}

		private FloatingPointNumerics()
		{
		}
	}
}
