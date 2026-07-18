using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Double : IFormattable, IConvertible, IComparable, IComparable<double>, IEquatable<double>
	{
		public const double Epsilon = 5E-324;

		public const double MaxValue = 1.7976931348623157E+308;

		public const double MinValue = -1.7976931348623157E+308;

		public const double NaN = 0.0 / 0.0;

		public const double NegativeInfinity = -1.0 / 0.0;

		public const double PositiveInfinity = 1.0 / 0.0;

		private const int State_AllowSign = 1;

		private const int State_Digits = 2;

		private const int State_Decimal = 3;

		private const int State_ExponentSign = 4;

		private const int State_Exponent = 5;

		private const int State_ConsumeWhiteSpace = 6;

		private const int State_Exit = 7;

		internal double m_value;

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			return Convert.ToType(this, targetType, provider, false);
		}

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
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException();
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
			if (!(value is double))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Double"));
			}
			double num = (double)value;
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
			if (this > num)
			{
				return 1;
			}
			if (this < num)
			{
				return -1;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is double))
			{
				return false;
			}
			double num = (double)obj;
			if (IsNaN(num))
			{
				return IsNaN(this);
			}
			return num == this;
		}

		public int CompareTo(double value)
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
			if (this > value)
			{
				return 1;
			}
			if (this < value)
			{
				return -1;
			}
			return 0;
		}

		public bool Equals(double obj)
		{
			if (IsNaN(obj))
			{
				if (IsNaN(this))
				{
					return true;
				}
				return false;
			}
			return obj == this;
		}

		public unsafe override int GetHashCode()
		{
			double num = this;
			return ((long*)(&num))->GetHashCode();
		}

		public static bool IsInfinity(double d)
		{
			return d == double.PositiveInfinity || d == double.NegativeInfinity;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static bool IsNaN(double d)
		{
			return d != d;
		}

		public static bool IsNegativeInfinity(double d)
		{
			return d < 0.0 && (d == double.NegativeInfinity || d == double.PositiveInfinity);
		}

		public static bool IsPositiveInfinity(double d)
		{
			return d > 0.0 && (d == double.NegativeInfinity || d == double.PositiveInfinity);
		}

		public static double Parse(string s)
		{
			return Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);
		}

		public static double Parse(string s, IFormatProvider provider)
		{
			return Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);
		}

		public static double Parse(string s, NumberStyles style)
		{
			return Parse(s, style, null);
		}

		public static double Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			double result;
			Exception exc;
			if (!Parse(s, style, provider, false, out result, out exc))
			{
				throw exc;
			}
			return result;
		}

		internal unsafe static bool Parse(string s, NumberStyles style, IFormatProvider provider, bool tryParse, out double result, out Exception exc)
		{
			result = 0.0;
			exc = null;
			if (s == null)
			{
				if (!tryParse)
				{
					exc = new ArgumentNullException("s");
				}
				return false;
			}
			if (s.Length == 0)
			{
				if (!tryParse)
				{
					exc = new FormatException();
				}
				return false;
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				string text = Locale.GetText("Double doesn't support parsing with '{0}'.", "AllowHexSpecifier");
				throw new ArgumentException(text);
			}
			if (style > NumberStyles.Any)
			{
				if (!tryParse)
				{
					exc = new ArgumentException();
				}
				return false;
			}
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			if (instance == null)
			{
				throw new Exception("How did this happen?");
			}
			int length = s.Length;
			int num = 0;
			int i = 0;
			bool flag = (style & NumberStyles.AllowLeadingWhite) != 0;
			bool flag2 = (style & NumberStyles.AllowTrailingWhite) != 0;
			if (flag)
			{
				for (; i < length && char.IsWhiteSpace(s[i]); i++)
				{
				}
				if (i == length)
				{
					if (!tryParse)
					{
						exc = int.GetFormatException();
					}
					return false;
				}
			}
			int num2 = s.Length - 1;
			if (flag2)
			{
				while (char.IsWhiteSpace(s[num2]))
				{
					num2--;
				}
			}
			if (TryParseStringConstant(instance.NaNSymbol, s, i, num2))
			{
				result = double.NaN;
				return true;
			}
			if (TryParseStringConstant(instance.PositiveInfinitySymbol, s, i, num2))
			{
				result = double.PositiveInfinity;
				return true;
			}
			if (TryParseStringConstant(instance.NegativeInfinitySymbol, s, i, num2))
			{
				result = double.NegativeInfinity;
				return true;
			}
			byte[] array = new byte[length + 1];
			int num3 = 1;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			if ((style & NumberStyles.AllowDecimalPoint) != NumberStyles.None)
			{
				text2 = instance.NumberDecimalSeparator;
				num4 = text2.Length;
			}
			if ((style & NumberStyles.AllowThousands) != NumberStyles.None)
			{
				text3 = instance.NumberGroupSeparator;
				num5 = text3.Length;
			}
			if ((style & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
			{
				text4 = instance.CurrencySymbol;
				num6 = text4.Length;
			}
			string positiveSign = instance.PositiveSign;
			string negativeSign = instance.NegativeSign;
			for (; i < length; i++)
			{
				char c = s[i];
				if (c == '\0')
				{
					i = length;
					continue;
				}
				switch (num3)
				{
				case 1:
					if ((style & NumberStyles.AllowLeadingSign) != NumberStyles.None)
					{
						if (c == positiveSign[0] && s.Substring(i, positiveSign.Length) == positiveSign)
						{
							num3 = 2;
							i += positiveSign.Length - 1;
							continue;
						}
						if (c == negativeSign[0] && s.Substring(i, negativeSign.Length) == negativeSign)
						{
							num3 = 2;
							array[num++] = 45;
							i += negativeSign.Length - 1;
							continue;
						}
					}
					num3 = 2;
					goto case 2;
				case 2:
					if (char.IsDigit(c))
					{
						array[num++] = (byte)c;
						break;
					}
					if (c == 'e' || c == 'E')
					{
						goto case 3;
					}
					if (num4 > 0 && text2[0] == c && string.CompareOrdinal(s, i, text2, 0, num4) == 0)
					{
						array[num++] = 46;
						i += num4 - 1;
						num3 = 3;
						break;
					}
					if (num5 > 0 && text3[0] == c && s.Substring(i, num5) == text3)
					{
						i += num5 - 1;
						num3 = 2;
						break;
					}
					if (num6 > 0 && text4[0] == c && s.Substring(i, num6) == text4)
					{
						i += num6 - 1;
						num3 = 2;
						break;
					}
					if (char.IsWhiteSpace(c))
					{
						goto case 6;
					}
					if (!tryParse)
					{
						exc = new FormatException("Unknown char: " + c);
					}
					return false;
				case 3:
					if (char.IsDigit(c))
					{
						array[num++] = (byte)c;
						break;
					}
					if (c == 'e' || c == 'E')
					{
						if ((style & NumberStyles.AllowExponent) == 0)
						{
							if (!tryParse)
							{
								exc = new FormatException("Unknown char: " + c);
							}
							return false;
						}
						array[num++] = (byte)c;
						num3 = 4;
						break;
					}
					if (char.IsWhiteSpace(c))
					{
						goto case 6;
					}
					if (!tryParse)
					{
						exc = new FormatException("Unknown char: " + c);
					}
					return false;
				case 4:
					if (char.IsDigit(c))
					{
						num3 = 5;
						goto case 5;
					}
					if (c == positiveSign[0] && s.Substring(i, positiveSign.Length) == positiveSign)
					{
						num3 = 2;
						i += positiveSign.Length - 1;
						continue;
					}
					if (c == negativeSign[0] && s.Substring(i, negativeSign.Length) == negativeSign)
					{
						num3 = 2;
						array[num++] = 45;
						i += negativeSign.Length - 1;
						continue;
					}
					if (char.IsWhiteSpace(c))
					{
						goto case 6;
					}
					if (!tryParse)
					{
						exc = new FormatException("Unknown char: " + c);
					}
					return false;
				case 5:
					if (char.IsDigit(c))
					{
						array[num++] = (byte)c;
						break;
					}
					if (char.IsWhiteSpace(c))
					{
						goto case 6;
					}
					if (!tryParse)
					{
						exc = new FormatException("Unknown char: " + c);
					}
					return false;
				case 6:
					if (flag2 && char.IsWhiteSpace(c))
					{
						num3 = 6;
						break;
					}
					if (!tryParse)
					{
						exc = new FormatException("Unknown char");
					}
					return false;
				}
				if (num3 != 7)
				{
					continue;
				}
				break;
			}
			array[num] = 0;
			fixed (byte* byte_ptr = &array[0])
			{
				double value;
				if (!ParseImpl(byte_ptr, out value))
				{
					if (!tryParse)
					{
						exc = int.GetFormatException();
					}
					return false;
				}
				if (IsPositiveInfinity(value) || IsNegativeInfinity(value))
				{
					if (!tryParse)
					{
						exc = new OverflowException();
					}
					return false;
				}
				result = value;
				return true;
			}
		}

		private static bool TryParseStringConstant(string format, string s, int start, int end)
		{
			return end - start + 1 == format.Length && string.CompareOrdinal(format, 0, s, start, format.Length) == 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool ParseImpl(byte* byte_ptr, out double value);

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out double result)
		{
			Exception exc;
			if (!Parse(s, style, provider, true, out result, out exc))
			{
				result = 0.0;
				return false;
			}
			return true;
		}

		public static bool TryParse(string s, out double result)
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
			return TypeCode.Double;
		}
	}
}
