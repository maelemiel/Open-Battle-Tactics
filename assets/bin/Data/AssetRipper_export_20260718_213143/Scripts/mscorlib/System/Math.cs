using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace System
{
	public static class Math
	{
		public const double E = 2.718281828459045;

		public const double PI = 3.141592653589793;

		public static decimal Abs(decimal value)
		{
			return (!(value < 0m)) ? value : (-value);
		}

		public static double Abs(double value)
		{
			return (!(value < 0.0)) ? value : (0.0 - value);
		}

		public static float Abs(float value)
		{
			return (!(value < 0f)) ? value : (0f - value);
		}

		public static int Abs(int value)
		{
			if (value == int.MinValue)
			{
				throw new OverflowException(Locale.GetText("Value is too small."));
			}
			return (value >= 0) ? value : (-value);
		}

		public static long Abs(long value)
		{
			if (value == long.MinValue)
			{
				throw new OverflowException(Locale.GetText("Value is too small."));
			}
			return (value >= 0) ? value : (-value);
		}

		[CLSCompliant(false)]
		public static sbyte Abs(sbyte value)
		{
			if (value == sbyte.MinValue)
			{
				throw new OverflowException(Locale.GetText("Value is too small."));
			}
			return (sbyte)((value >= 0) ? value : (-value));
		}

		public static short Abs(short value)
		{
			if (value == short.MinValue)
			{
				throw new OverflowException(Locale.GetText("Value is too small."));
			}
			return (short)((value >= 0) ? value : (-value));
		}

		public static decimal Ceiling(decimal d)
		{
			decimal num = Floor(d);
			if (num != d)
			{
				++num;
			}
			return num;
		}

		public static double Ceiling(double a)
		{
			double num = Floor(a);
			if (num != a)
			{
				num += 1.0;
			}
			return num;
		}

		public static long BigMul(int a, int b)
		{
			return (long)a * (long)b;
		}

		public static int DivRem(int a, int b, out int result)
		{
			result = a % b;
			return a / b;
		}

		public static long DivRem(long a, long b, out long result)
		{
			result = a % b;
			return a / b;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Floor(double d);

		public static double IEEERemainder(double x, double y)
		{
			if (y == 0.0)
			{
				return double.NaN;
			}
			double num = x - y * Round(x / y);
			if (num != 0.0)
			{
				return num;
			}
			return (!(x > 0.0)) ? BitConverter.Int64BitsToDouble(long.MinValue) : 0.0;
		}

		public static double Log(double a, double newBase)
		{
			double num = Log(a) / Log(newBase);
			return (num != 0.0) ? num : 0.0;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static byte Max(byte val1, byte val2)
		{
			return (val1 <= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static decimal Max(decimal val1, decimal val2)
		{
			return (!(val1 > val2)) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static double Max(double val1, double val2)
		{
			if (double.IsNaN(val1) || double.IsNaN(val2))
			{
				return double.NaN;
			}
			return (!(val1 > val2)) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static float Max(float val1, float val2)
		{
			if (float.IsNaN(val1) || float.IsNaN(val2))
			{
				return float.NaN;
			}
			return (!(val1 > val2)) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static int Max(int val1, int val2)
		{
			return (val1 <= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static long Max(long val1, long val2)
		{
			return (val1 <= val2) ? val2 : val1;
		}

		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static sbyte Max(sbyte val1, sbyte val2)
		{
			return (val1 <= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static short Max(short val1, short val2)
		{
			return (val1 <= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[CLSCompliant(false)]
		public static uint Max(uint val1, uint val2)
		{
			return (val1 <= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[CLSCompliant(false)]
		public static ulong Max(ulong val1, ulong val2)
		{
			return (val1 <= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[CLSCompliant(false)]
		public static ushort Max(ushort val1, ushort val2)
		{
			return (val1 <= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static byte Min(byte val1, byte val2)
		{
			return (val1 >= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static decimal Min(decimal val1, decimal val2)
		{
			return (!(val1 < val2)) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static double Min(double val1, double val2)
		{
			if (double.IsNaN(val1) || double.IsNaN(val2))
			{
				return double.NaN;
			}
			return (!(val1 < val2)) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static float Min(float val1, float val2)
		{
			if (float.IsNaN(val1) || float.IsNaN(val2))
			{
				return float.NaN;
			}
			return (!(val1 < val2)) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static int Min(int val1, int val2)
		{
			return (val1 >= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static long Min(long val1, long val2)
		{
			return (val1 >= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[CLSCompliant(false)]
		public static sbyte Min(sbyte val1, sbyte val2)
		{
			return (val1 >= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static short Min(short val1, short val2)
		{
			return (val1 >= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[CLSCompliant(false)]
		public static uint Min(uint val1, uint val2)
		{
			return (val1 >= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[CLSCompliant(false)]
		public static ulong Min(ulong val1, ulong val2)
		{
			return (val1 >= val2) ? val2 : val1;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[CLSCompliant(false)]
		public static ushort Min(ushort val1, ushort val2)
		{
			return (val1 >= val2) ? val2 : val1;
		}

		public static decimal Round(decimal d)
		{
			decimal num = decimal.Floor(d);
			decimal num2 = d - num;
			if ((num2 == 0.5m && 2.0m * (num / 2.0m - decimal.Floor(num / 2.0m)) != 0m) || num2 > 0.5m)
			{
				++num;
			}
			return num;
		}

		public static decimal Round(decimal d, int decimals)
		{
			return decimal.Round(d, decimals);
		}

		public static decimal Round(decimal d, MidpointRounding mode)
		{
			if (mode != MidpointRounding.ToEven && mode != MidpointRounding.AwayFromZero)
			{
				throw new ArgumentException(string.Concat("The value '", mode, "' is not valid for this usage of the type MidpointRounding."), "mode");
			}
			if (mode == MidpointRounding.ToEven)
			{
				return Round(d);
			}
			return RoundAwayFromZero(d);
		}

		private static decimal RoundAwayFromZero(decimal d)
		{
			decimal num = decimal.Floor(d);
			decimal num2 = d - num;
			if (num >= 0m && num2 >= 0.5m)
			{
				++num;
			}
			else if (num < 0m && num2 > 0.5m)
			{
				++num;
			}
			return num;
		}

		public static decimal Round(decimal d, int decimals, MidpointRounding mode)
		{
			return decimal.Round(d, decimals, mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Round(double a);

		public static double Round(double value, int digits)
		{
			if (digits < 0 || digits > 15)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("Value is too small or too big."));
			}
			return Round2(value, digits, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double Round2(double value, int digits, bool away_from_zero);

		public static double Round(double value, MidpointRounding mode)
		{
			if (mode != MidpointRounding.ToEven && mode != MidpointRounding.AwayFromZero)
			{
				throw new ArgumentException(string.Concat("The value '", mode, "' is not valid for this usage of the type MidpointRounding."), "mode");
			}
			if (mode == MidpointRounding.ToEven)
			{
				return Round(value);
			}
			if (value > 0.0)
			{
				return Floor(value + 0.5);
			}
			return Ceiling(value - 0.5);
		}

		public static double Round(double value, int digits, MidpointRounding mode)
		{
			if (mode != MidpointRounding.ToEven && mode != MidpointRounding.AwayFromZero)
			{
				throw new ArgumentException(string.Concat("The value '", mode, "' is not valid for this usage of the type MidpointRounding."), "mode");
			}
			if (mode == MidpointRounding.ToEven)
			{
				return Round(value, digits);
			}
			return Round2(value, digits, true);
		}

		public static double Truncate(double d)
		{
			if (d > 0.0)
			{
				return Floor(d);
			}
			if (d < 0.0)
			{
				return Ceiling(d);
			}
			return d;
		}

		public static decimal Truncate(decimal d)
		{
			return decimal.Truncate(d);
		}

		public static decimal Floor(decimal d)
		{
			return decimal.Floor(d);
		}

		public static int Sign(decimal value)
		{
			if (value > 0m)
			{
				return 1;
			}
			return (!(value == 0m)) ? (-1) : 0;
		}

		public static int Sign(double value)
		{
			if (double.IsNaN(value))
			{
				throw new ArithmeticException("NAN");
			}
			if (value > 0.0)
			{
				return 1;
			}
			return (value != 0.0) ? (-1) : 0;
		}

		public static int Sign(float value)
		{
			if (float.IsNaN(value))
			{
				throw new ArithmeticException("NAN");
			}
			if (value > 0f)
			{
				return 1;
			}
			return (value != 0f) ? (-1) : 0;
		}

		public static int Sign(int value)
		{
			if (value > 0)
			{
				return 1;
			}
			return (value != 0) ? (-1) : 0;
		}

		public static int Sign(long value)
		{
			if (value > 0)
			{
				return 1;
			}
			return (value != 0L) ? (-1) : 0;
		}

		[CLSCompliant(false)]
		public static int Sign(sbyte value)
		{
			if (value > 0)
			{
				return 1;
			}
			return (value != 0) ? (-1) : 0;
		}

		public static int Sign(short value)
		{
			if (value > 0)
			{
				return 1;
			}
			return (value != 0) ? (-1) : 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Sin(double a);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Cos(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Tan(double a);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Sinh(double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Cosh(double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Tanh(double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Acos(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Asin(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Atan(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Atan2(double y, double x);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Exp(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Log(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Log10(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Pow(double x, double y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern double Sqrt(double d);
	}
}
