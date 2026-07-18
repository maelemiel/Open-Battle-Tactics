using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Single : IFormattable, IConvertible, IComparable, IComparable<float>, IEquatable<float>
	{
		public const float Epsilon = 1E-45f;

		public const float MaxValue = 3.4028235E+38f;

		public const float MinValue = -3.4028235E+38f;

		public const float NaN = 0f / 0f;

		public const float PositiveInfinity = 1f / 0f;

		public const float NegativeInfinity = -1f / 0f;

		private const double MaxValueEpsilon = 3.6147112457961776E+29;

		internal float m_value;

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(this);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this);
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			return Convert.ToType(this, targetType, provider, false);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is float))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Single."));
			}
			float num = (float)value;
			if (IsPositiveInfinity(this) && IsPositiveInfinity(num))
			{
				return 0;
			}
			if (IsNegativeInfinity(this) && IsNegativeInfinity(num))
			{
				return 0;
			}
			if (IsNaN(num))
			{
				if (IsNaN(this))
				{
					return 0;
				}
				return 1;
			}
			if (IsNaN(this))
			{
				if (IsNaN(num))
				{
					return 0;
				}
				return -1;
			}
			if (this == num)
			{
				return 0;
			}
			if (this > num)
			{
				return 1;
			}
			return -1;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is float))
			{
				return false;
			}
			float num = (float)obj;
			if (IsNaN(num))
			{
				return IsNaN(this);
			}
			return num == this;
		}

		public int CompareTo(float value)
		{
			if (IsPositiveInfinity(this) && IsPositiveInfinity(value))
			{
				return 0;
			}
			if (IsNegativeInfinity(this) && IsNegativeInfinity(value))
			{
				return 0;
			}
			if (IsNaN(value))
			{
				if (IsNaN(this))
				{
					return 0;
				}
				return 1;
			}
			if (IsNaN(this))
			{
				if (IsNaN(value))
				{
					return 0;
				}
				return -1;
			}
			if (this == value)
			{
				return 0;
			}
			if (this > value)
			{
				return 1;
			}
			return -1;
		}

		public bool Equals(float obj)
		{
			if (IsNaN(obj))
			{
				return IsNaN(this);
			}
			return obj == this;
		}

		public unsafe override int GetHashCode()
		{
			float num = this;
			return *(int*)(&num);
		}

		public static bool IsInfinity(float f)
		{
			return f == float.PositiveInfinity || f == float.NegativeInfinity;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static bool IsNaN(float f)
		{
			return f != f;
		}

		public static bool IsNegativeInfinity(float f)
		{
			return f < 0f && (f == float.NegativeInfinity || f == float.PositiveInfinity);
		}

		public static bool IsPositiveInfinity(float f)
		{
			return f > 0f && (f == float.NegativeInfinity || f == float.PositiveInfinity);
		}

		public static float Parse(string s)
		{
			double num = double.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}

		public static float Parse(string s, IFormatProvider provider)
		{
			double num = double.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}

		public static float Parse(string s, NumberStyles style)
		{
			double num = double.Parse(s, style, null);
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}

		public static float Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			double num = double.Parse(s, style, provider);
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out float result)
		{
			double result2;
			Exception exc;
			if (!double.Parse(s, style, provider, true, out result2, out exc))
			{
				result = 0f;
				return false;
			}
			if (result2 - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(result2))
			{
				result = 0f;
				return false;
			}
			result = (float)result2;
			return true;
		}

		public static bool TryParse(string s, out float result)
		{
			return TryParse(s, NumberStyles.Any, null, out result);
		}

		public override string ToString()
		{
			return NumberFormatter.NumberToString(this, null);
		}

		public string ToString(IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(this, provider);
		}

		public string ToString(string format)
		{
			return ToString(format, null);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(format, this, provider);
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Single;
		}
	}
}
