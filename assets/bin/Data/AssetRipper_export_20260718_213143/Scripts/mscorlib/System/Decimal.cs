using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Decimal : IFormattable, IConvertible, IComparable, IComparable<decimal>, IEquatable<decimal>
	{
		public const decimal MinValue = -79228162514264337593543950335m;

		public const decimal MaxValue = 79228162514264337593543950335m;

		public const decimal MinusOne = -1m;

		public const decimal One = 1m;

		public const decimal Zero = 0m;

		private const int DECIMAL_DIVIDE_BY_ZERO = 5;

		private const uint MAX_SCALE = 28u;

		private const int iMAX_SCALE = 28;

		private const uint SIGN_FLAG = 2147483648u;

		private const uint SCALE_MASK = 16711680u;

		private const int SCALE_SHIFT = 16;

		private const uint RESERVED_SS32_BITS = 2130771967u;

		private static readonly decimal MaxValueDiv10 = 7922816251426433759354395033.5m;

		private uint flags;

		private uint hi;

		private uint lo;

		private uint mid;

		public Decimal(int lo, int mid, int hi, bool isNegative, byte scale)
		{
			this.lo = (uint)lo;
			this.mid = (uint)mid;
			this.hi = (uint)hi;
			if ((uint)scale > 28u)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("scale must be between 0 and 28"));
			}
			flags = scale;
			flags <<= 16;
			if (isNegative)
			{
				flags |= 2147483648u;
			}
		}

		public Decimal(int value)
		{
			hi = (mid = 0u);
			if (value < 0)
			{
				flags = 2147483648u;
				lo = (uint)(~value + 1);
			}
			else
			{
				flags = 0u;
				lo = (uint)value;
			}
		}

		[CLSCompliant(false)]
		public Decimal(uint value)
		{
			lo = value;
			flags = (hi = (mid = 0u));
		}

		public Decimal(long value)
		{
			hi = 0u;
			if (value < 0)
			{
				flags = 2147483648u;
				ulong num = (ulong)(~value + 1);
				lo = (uint)num;
				mid = (uint)(num >> 32);
			}
			else
			{
				flags = 0u;
				lo = (uint)value;
				mid = (uint)((ulong)value >> 32);
			}
		}

		[CLSCompliant(false)]
		public Decimal(ulong value)
		{
			flags = (hi = 0u);
			lo = (uint)value;
			mid = (uint)(value >> 32);
		}

		public Decimal(float value)
		{
			if (value > 7.9228163E+28f || value < -7.9228163E+28f || float.IsNaN(value) || float.IsNegativeInfinity(value) || float.IsPositiveInfinity(value))
			{
				throw new OverflowException(Locale.GetText("Value {0} is greater than Decimal.MaxValue or less than Decimal.MinValue", value));
			}
			decimal num = Parse(value.ToString(CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
			flags = num.flags;
			hi = num.hi;
			lo = num.lo;
			mid = num.mid;
		}

		public Decimal(double value)
		{
			if (value > 7.922816251426434E+28 || value < -7.922816251426434E+28 || double.IsNaN(value) || double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value))
			{
				throw new OverflowException(Locale.GetText("Value {0} is greater than Decimal.MaxValue or less than Decimal.MinValue", value));
			}
			decimal num = Parse(value.ToString(CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture);
			flags = num.flags;
			hi = num.hi;
			lo = num.lo;
			mid = num.mid;
		}

		public Decimal(int[] bits)
		{
			if (bits == null)
			{
				throw new ArgumentNullException(Locale.GetText("Bits is a null reference"));
			}
			if (bits.GetLength(0) != 4)
			{
				throw new ArgumentException(Locale.GetText("bits does not contain four values"));
			}
			lo = (uint)bits[0];
			mid = (uint)bits[1];
			hi = (uint)bits[2];
			flags = (uint)bits[3];
			byte b = (byte)(flags >> 16);
			if ((uint)b > 28u || (flags & 0x7F00FFFF) != 0)
			{
				throw new ArgumentException(Locale.GetText("Invalid bits[3]"));
			}
		}

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
			return this;
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

		public static decimal FromOACurrency(long cy)
		{
			return (decimal)cy / 10000m;
		}

		public static int[] GetBits(decimal d)
		{
			return new int[4]
			{
				(int)d.lo,
				(int)d.mid,
				(int)d.hi,
				(int)d.flags
			};
		}

		public static decimal Negate(decimal d)
		{
			d.flags ^= 2147483648u;
			return d;
		}

		public static decimal Add(decimal d1, decimal d2)
		{
			if (decimalIncr(ref d1, ref d2) == 0)
			{
				return d1;
			}
			throw new OverflowException(Locale.GetText("Overflow on adding decimal number"));
		}

		public static decimal Subtract(decimal d1, decimal d2)
		{
			d2.flags ^= 2147483648u;
			int num = decimalIncr(ref d1, ref d2);
			if (num == 0)
			{
				return d1;
			}
			throw new OverflowException(Locale.GetText("Overflow on subtracting decimal numbers (" + num + ")"));
		}

		public override int GetHashCode()
		{
			return (int)(flags ^ hi ^ lo ^ mid);
		}

		private static ulong u64(decimal value)
		{
			decimalFloorAndTrunc(ref value, 0);
			ulong result;
			if (decimal2UInt64(ref value, out result) != 0)
			{
				throw new OverflowException();
			}
			return result;
		}

		private static long s64(decimal value)
		{
			decimalFloorAndTrunc(ref value, 0);
			long result;
			if (decimal2Int64(ref value, out result) != 0)
			{
				throw new OverflowException();
			}
			return result;
		}

		public static bool Equals(decimal d1, decimal d2)
		{
			return Compare(d1, d2) == 0;
		}

		public override bool Equals(object value)
		{
			if (!(value is decimal))
			{
				return false;
			}
			return Equals((decimal)value, this);
		}

		private bool IsZero()
		{
			return hi == 0 && lo == 0 && mid == 0;
		}

		private bool IsNegative()
		{
			return (flags & 0x80000000u) == 2147483648u;
		}

		public static decimal Floor(decimal d)
		{
			decimalFloorAndTrunc(ref d, 1);
			return d;
		}

		public static decimal Truncate(decimal d)
		{
			decimalFloorAndTrunc(ref d, 0);
			return d;
		}

		public static decimal Round(decimal d, int decimals)
		{
			return Round(d, decimals, MidpointRounding.ToEven);
		}

		public static decimal Round(decimal d, int decimals, MidpointRounding mode)
		{
			if (mode != MidpointRounding.ToEven && mode != MidpointRounding.AwayFromZero)
			{
				throw new ArgumentException(string.Concat("The value '", mode, "' is not valid for this usage of the type MidpointRounding."), "mode");
			}
			if (decimals < 0 || decimals > 28)
			{
				throw new ArgumentOutOfRangeException("decimals", "[0,28]");
			}
			bool flag = d.IsNegative();
			if (flag)
			{
				d.flags ^= 2147483648u;
			}
			decimal num = (decimal)Math.Pow(10.0, decimals);
			decimal num2 = Floor(d);
			decimal d2 = d - num2;
			d2 *= 10000000000000000000000000000m;
			d2 = Floor(d2);
			d2 /= 10000000000000000000000000000m / num;
			d2 = Math.Round(d2, mode);
			d2 /= num;
			decimal num3 = num2 + d2;
			long num4 = decimals - ((num3.flags & 0x7FFF0000) >> 16);
			if (num4 > 0)
			{
				while (num4 > 0 && !(num3 > MaxValueDiv10))
				{
					num3 *= 10m;
					num4--;
				}
			}
			else if (num4 < 0)
			{
				for (; num4 < 0; num4++)
				{
					num3 /= 10m;
				}
			}
			num3.flags = (uint)(decimals - num4 << 16);
			if (flag)
			{
				num3.flags ^= 2147483648u;
			}
			return num3;
		}

		public static decimal Round(decimal d)
		{
			return Math.Round(d);
		}

		public static decimal Round(decimal d, MidpointRounding mode)
		{
			return Math.Round(d, mode);
		}

		public static decimal Multiply(decimal d1, decimal d2)
		{
			if (d1.IsZero() || d2.IsZero())
			{
				return 0m;
			}
			if (decimalMult(ref d1, ref d2) != 0)
			{
				throw new OverflowException();
			}
			return d1;
		}

		public static decimal Divide(decimal d1, decimal d2)
		{
			if (d2.IsZero())
			{
				throw new DivideByZeroException();
			}
			if (d1.IsZero())
			{
				return 0m;
			}
			d1.flags ^= 2147483648u;
			d1.flags ^= 2147483648u;
			decimal pc;
			if (decimalDiv(out pc, ref d1, ref d2) != 0)
			{
				throw new OverflowException();
			}
			return pc;
		}

		public static decimal Remainder(decimal d1, decimal d2)
		{
			if (d2.IsZero())
			{
				throw new DivideByZeroException();
			}
			if (d1.IsZero())
			{
				return 0m;
			}
			bool flag = d1.IsNegative();
			if (flag)
			{
				d1.flags ^= 2147483648u;
			}
			if (d2.IsNegative())
			{
				d2.flags ^= 2147483648u;
			}
			if (d1 == d2)
			{
				return 0m;
			}
			decimal result;
			if (d2 > d1)
			{
				result = d1;
			}
			else
			{
				if (decimalDiv(out result, ref d1, ref d2) != 0)
				{
					throw new OverflowException();
				}
				result = Truncate(result);
				result = d1 - result * d2;
			}
			if (flag)
			{
				result.flags ^= 2147483648u;
			}
			return result;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static int Compare(decimal d1, decimal d2)
		{
			return decimalCompare(ref d1, ref d2);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is decimal))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Decimal"));
			}
			return Compare(this, (decimal)value);
		}

		public int CompareTo(decimal value)
		{
			return Compare(this, value);
		}

		public bool Equals(decimal value)
		{
			return Equals(value, this);
		}

		public static decimal Ceiling(decimal d)
		{
			return Math.Ceiling(d);
		}

		public static decimal Parse(string s)
		{
			return Parse(s, NumberStyles.Number, null);
		}

		public static decimal Parse(string s, NumberStyles style)
		{
			return Parse(s, style, null);
		}

		public static decimal Parse(string s, IFormatProvider provider)
		{
			return Parse(s, NumberStyles.Number, provider);
		}

		private static void ThrowAtPos(int pos)
		{
			throw new FormatException(string.Format(Locale.GetText("Invalid character at position {0}"), pos));
		}

		private static void ThrowInvalidExp()
		{
			throw new FormatException(Locale.GetText("Invalid exponent"));
		}

		private static string stripStyles(string s, NumberStyles style, NumberFormatInfo nfi, out int decPos, out bool isNegative, out bool expFlag, out int exp, bool throwex)
		{
			isNegative = false;
			expFlag = false;
			exp = 0;
			decPos = -1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = (style & NumberStyles.AllowLeadingWhite) != 0;
			bool flag5 = (style & NumberStyles.AllowTrailingWhite) != 0;
			bool flag6 = (style & NumberStyles.AllowLeadingSign) != 0;
			bool flag7 = (style & NumberStyles.AllowTrailingSign) != 0;
			bool flag8 = (style & NumberStyles.AllowParentheses) != 0;
			bool flag9 = (style & NumberStyles.AllowThousands) != 0;
			bool flag10 = (style & NumberStyles.AllowDecimalPoint) != 0;
			bool flag11 = (style & NumberStyles.AllowExponent) != 0;
			bool flag12 = false;
			if ((style & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
			{
				int num = s.IndexOf(nfi.CurrencySymbol);
				if (num >= 0)
				{
					s = s.Remove(num, nfi.CurrencySymbol.Length);
					flag12 = true;
				}
			}
			string text = ((!flag12) ? nfi.NumberDecimalSeparator : nfi.CurrencyDecimalSeparator);
			string text2 = ((!flag12) ? nfi.NumberGroupSeparator : nfi.CurrencyGroupSeparator);
			int i = 0;
			int length = s.Length;
			StringBuilder stringBuilder = new StringBuilder(length);
			while (i < length)
			{
				char c = s[i];
				if (char.IsDigit(c))
				{
					break;
				}
				if (flag4 && char.IsWhiteSpace(c))
				{
					i++;
					continue;
				}
				if (flag8 && c == '(' && !flag && !flag2)
				{
					flag2 = true;
					flag = true;
					isNegative = true;
					i++;
					continue;
				}
				if (flag6 && c == nfi.NegativeSign[0] && !flag)
				{
					int length2 = nfi.NegativeSign.Length;
					if (length2 == 1 || s.IndexOf(nfi.NegativeSign, i, length2) == i)
					{
						flag = true;
						isNegative = true;
						i += length2;
					}
					continue;
				}
				if (flag6 && c == nfi.PositiveSign[0] && !flag)
				{
					int length3 = nfi.PositiveSign.Length;
					if (length3 == 1 || s.IndexOf(nfi.PositiveSign, i, length3) == i)
					{
						flag = true;
						i += length3;
					}
					continue;
				}
				if (flag10 && c == text[0])
				{
					int length4 = text.Length;
					if (length4 != 1 && s.IndexOf(text, i, length4) != i)
					{
						if (throwex)
						{
							ThrowAtPos(i);
							break;
						}
						return null;
					}
					break;
				}
				if (throwex)
				{
					ThrowAtPos(i);
					continue;
				}
				return null;
			}
			if (i == length)
			{
				if (throwex)
				{
					throw new FormatException(Locale.GetText("No digits found"));
				}
				return null;
			}
			while (i < length)
			{
				char c2 = s[i];
				if (char.IsDigit(c2))
				{
					stringBuilder.Append(c2);
					i++;
					continue;
				}
				if (flag9 && c2 == text2[0])
				{
					int length5 = text2.Length;
					if (length5 != 1 && s.IndexOf(text2, i, length5) != i)
					{
						if (!throwex)
						{
							return null;
						}
						ThrowAtPos(i);
					}
					i += length5;
					continue;
				}
				if (flag10 && c2 == text[0] && !flag3)
				{
					int length6 = text.Length;
					if (length6 == 1 || s.IndexOf(text, i, length6) == i)
					{
						decPos = stringBuilder.Length;
						flag3 = true;
						i += length6;
					}
					continue;
				}
				break;
			}
			if (i < length)
			{
				char c3 = s[i];
				if (flag11 && char.ToUpperInvariant(c3) == 'E')
				{
					expFlag = true;
					i++;
					if (i >= length)
					{
						if (!throwex)
						{
							return null;
						}
						ThrowInvalidExp();
					}
					c3 = s[i];
					bool flag13 = false;
					if (c3 == nfi.PositiveSign[0])
					{
						int length7 = nfi.PositiveSign.Length;
						if (length7 == 1 || s.IndexOf(nfi.PositiveSign, i, length7) == i)
						{
							i += length7;
							if (i >= length)
							{
								if (!throwex)
								{
									return null;
								}
								ThrowInvalidExp();
							}
						}
					}
					else if (c3 == nfi.NegativeSign[0])
					{
						int length8 = nfi.NegativeSign.Length;
						if (length8 == 1 || s.IndexOf(nfi.NegativeSign, i, length8) == i)
						{
							i += length8;
							if (i >= length)
							{
								if (!throwex)
								{
									return null;
								}
								ThrowInvalidExp();
							}
							flag13 = true;
						}
					}
					c3 = s[i];
					if (!char.IsDigit(c3))
					{
						if (!throwex)
						{
							return null;
						}
						ThrowInvalidExp();
					}
					exp = c3 - 48;
					for (i++; i < length && char.IsDigit(s[i]); i++)
					{
						exp *= 10;
						exp += s[i] - 48;
					}
					if (flag13)
					{
						exp *= -1;
					}
				}
			}
			while (i < length)
			{
				char c4 = s[i];
				if (flag5 && char.IsWhiteSpace(c4))
				{
					i++;
				}
				else if (flag8 && c4 == ')' && flag2)
				{
					flag2 = false;
					i++;
				}
				else if (flag7 && c4 == nfi.NegativeSign[0] && !flag)
				{
					int length9 = nfi.NegativeSign.Length;
					if (length9 == 1 || s.IndexOf(nfi.NegativeSign, i, length9) == i)
					{
						flag = true;
						isNegative = true;
						i += length9;
					}
				}
				else if (flag7 && c4 == nfi.PositiveSign[0] && !flag)
				{
					int length10 = nfi.PositiveSign.Length;
					if (length10 == 1 || s.IndexOf(nfi.PositiveSign, i, length10) == i)
					{
						flag = true;
						i += length10;
					}
				}
				else
				{
					if (!throwex)
					{
						return null;
					}
					ThrowAtPos(i);
				}
			}
			if (flag2)
			{
				if (throwex)
				{
					throw new FormatException(Locale.GetText("Closing Parentheses not found"));
				}
				return null;
			}
			if (!flag3)
			{
				decPos = stringBuilder.Length;
			}
			return stringBuilder.ToString();
		}

		public static decimal Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				throw new ArgumentException("Decimal.TryParse does not accept AllowHexSpecifier", "style");
			}
			decimal res;
			PerformParse(s, style, provider, out res, true);
			return res;
		}

		public static bool TryParse(string s, out decimal result)
		{
			if (s == null)
			{
				result = 0m;
				return false;
			}
			return PerformParse(s, NumberStyles.Number, null, out result, false);
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out decimal result)
		{
			if (s == null || (style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				result = 0m;
				return false;
			}
			return PerformParse(s, style, provider, out result, false);
		}

		private static bool PerformParse(string s, NumberStyles style, IFormatProvider provider, out decimal res, bool throwex)
		{
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			int decPos;
			bool isNegative;
			bool expFlag;
			int exp;
			s = stripStyles(s, style, instance, out decPos, out isNegative, out expFlag, out exp, throwex);
			if (s == null)
			{
				res = 0m;
				return false;
			}
			if (decPos < 0)
			{
				if (throwex)
				{
					throw new Exception(Locale.GetText("Error in System.Decimal.Parse"));
				}
				res = 0m;
				return false;
			}
			int length = s.Length;
			int i;
			for (i = 0; i < decPos && s[i] == '0'; i++)
			{
			}
			if (i > 1 && length > 1)
			{
				s = s.Substring(i, length - i);
				decPos -= i;
			}
			int num = ((decPos != 0) ? 28 : 27);
			length = s.Length;
			if (length >= num + 1 && string.Compare(s, 0, "79228162514264337593543950335", 0, num + 1, false, CultureInfo.InvariantCulture) <= 0)
			{
				num++;
			}
			if (length > num && decPos < length)
			{
				int num2 = s[num] - 48;
				s = s.Substring(0, num);
				bool flag = false;
				if (num2 > 5)
				{
					flag = true;
				}
				else if (num2 == 5)
				{
					if (isNegative)
					{
						flag = true;
					}
					else
					{
						int num3 = s[num - 1] - 48;
						flag = (num3 & 1) == 1;
					}
				}
				if (flag)
				{
					char[] array = s.ToCharArray();
					int num4 = num - 1;
					while (num4 >= 0)
					{
						int num5 = array[num4] - 48;
						if (array[num4] != '9')
						{
							array[num4] = (char)(num5 + 49);
							break;
						}
						array[num4--] = '0';
					}
					if (num4 == -1 && array[0] == '0')
					{
						decPos++;
						s = "1".PadRight(decPos, '0');
					}
					else
					{
						s = new string(array);
					}
				}
			}
			decimal val;
			if (string2decimal(out val, s, (uint)decPos, 0) != 0)
			{
				if (throwex)
				{
					throw new OverflowException();
				}
				res = 0m;
				return false;
			}
			if (expFlag && decimalSetExponent(ref val, exp) != 0)
			{
				if (throwex)
				{
					throw new OverflowException();
				}
				res = 0m;
				return false;
			}
			if (isNegative)
			{
				val.flags ^= 2147483648u;
			}
			res = val;
			return true;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Decimal;
		}

		public static byte ToByte(decimal value)
		{
			if (value > 255m || value < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue or less than Byte.MinValue"));
			}
			return (byte)Truncate(value);
		}

		public static double ToDouble(decimal d)
		{
			return Convert.ToDouble(d);
		}

		public static short ToInt16(decimal value)
		{
			if (value > 32767m || value < -32768m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue or less than Int16.MinValue"));
			}
			return (short)Truncate(value);
		}

		public static int ToInt32(decimal d)
		{
			if (d > 2147483647m || d < -2147483648m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int32.MaxValue or less than Int32.MinValue"));
			}
			return (int)Truncate(d);
		}

		public static long ToInt64(decimal d)
		{
			if (d > 9223372036854775807m || d < -9223372036854775808m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int64.MaxValue or less than Int64.MinValue"));
			}
			return (long)Truncate(d);
		}

		public static long ToOACurrency(decimal value)
		{
			return (long)(value * 10000m);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(decimal value)
		{
			if (value > 127m || value < -128m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue or less than SByte.MinValue"));
			}
			return (sbyte)Truncate(value);
		}

		public static float ToSingle(decimal d)
		{
			return Convert.ToSingle(d);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(decimal value)
		{
			if (value > 65535m || value < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue or less than UInt16.MinValue"));
			}
			return (ushort)Truncate(value);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(decimal d)
		{
			if (d > 4294967295m || d < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt32.MaxValue or less than UInt32.MinValue"));
			}
			return (uint)Truncate(d);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(decimal d)
		{
			if (d > 18446744073709551615m || d < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt64.MaxValue or less than UInt64.MinValue"));
			}
			return (ulong)Truncate(d);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(format, this, provider);
		}

		public override string ToString()
		{
			return ToString("G", null);
		}

		public string ToString(string format)
		{
			return ToString(format, null);
		}

		public string ToString(IFormatProvider provider)
		{
			return ToString("G", provider);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimal2UInt64(ref decimal val, out ulong result);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimal2Int64(ref decimal val, out long result);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalIncr(ref decimal d1, ref decimal d2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int decimal2string(ref decimal val, int digits, int decimals, char[] bufDigits, int bufSize, out int decPos, out int sign);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int string2decimal(out decimal val, string sDigits, uint decPos, int sign);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int decimalSetExponent(ref decimal val, int exp);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double decimal2double(ref decimal val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void decimalFloorAndTrunc(ref decimal val, int floorFlag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalMult(ref decimal pd1, ref decimal pd2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalDiv(out decimal pc, ref decimal pa, ref decimal pb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalIntDiv(out decimal pc, ref decimal pa, ref decimal pb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int decimalCompare(ref decimal d1, ref decimal d2);

		public static decimal operator +(decimal d1, decimal d2)
		{
			return Add(d1, d2);
		}

		public static decimal operator --(decimal d)
		{
			return Add(d, -1m);
		}

		public static decimal operator ++(decimal d)
		{
			return Add(d, 1m);
		}

		public static decimal operator -(decimal d1, decimal d2)
		{
			return Subtract(d1, d2);
		}

		public static decimal operator -(decimal d)
		{
			return Negate(d);
		}

		public static decimal operator +(decimal d)
		{
			return d;
		}

		public static decimal operator *(decimal d1, decimal d2)
		{
			return Multiply(d1, d2);
		}

		public static decimal operator /(decimal d1, decimal d2)
		{
			return Divide(d1, d2);
		}

		public static decimal operator %(decimal d1, decimal d2)
		{
			return Remainder(d1, d2);
		}

		public static explicit operator byte(decimal value)
		{
			ulong num = u64(value);
			return checked((byte)num);
		}

		[CLSCompliant(false)]
		public static explicit operator sbyte(decimal value)
		{
			long num = s64(value);
			return checked((sbyte)num);
		}

		public static explicit operator char(decimal value)
		{
			ulong num = u64(value);
			return (char)checked((ushort)num);
		}

		public static explicit operator short(decimal value)
		{
			long num = s64(value);
			return checked((short)num);
		}

		[CLSCompliant(false)]
		public static explicit operator ushort(decimal value)
		{
			ulong num = u64(value);
			return checked((ushort)num);
		}

		public static explicit operator int(decimal value)
		{
			long num = s64(value);
			return checked((int)num);
		}

		[CLSCompliant(false)]
		public static explicit operator uint(decimal value)
		{
			ulong num = u64(value);
			return checked((uint)num);
		}

		public static explicit operator long(decimal value)
		{
			return s64(value);
		}

		[CLSCompliant(false)]
		public static explicit operator ulong(decimal value)
		{
			return u64(value);
		}

		public static implicit operator decimal(byte value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(sbyte value)
		{
			return new decimal(value);
		}

		public static implicit operator decimal(short value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(ushort value)
		{
			return new decimal(value);
		}

		public static implicit operator decimal(char value)
		{
			return new decimal(value);
		}

		public static implicit operator decimal(int value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(uint value)
		{
			return new decimal(value);
		}

		public static implicit operator decimal(long value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(ulong value)
		{
			return new decimal(value);
		}

		public static explicit operator decimal(float value)
		{
			return new decimal(value);
		}

		public static explicit operator decimal(double value)
		{
			return new decimal(value);
		}

		public static explicit operator float(decimal value)
		{
			return (float)(double)value;
		}

		public static explicit operator double(decimal value)
		{
			return decimal2double(ref value);
		}

		public static bool operator !=(decimal d1, decimal d2)
		{
			return !Equals(d1, d2);
		}

		public static bool operator ==(decimal d1, decimal d2)
		{
			return Equals(d1, d2);
		}

		public static bool operator >(decimal d1, decimal d2)
		{
			return Compare(d1, d2) > 0;
		}

		public static bool operator >=(decimal d1, decimal d2)
		{
			return Compare(d1, d2) >= 0;
		}

		public static bool operator <(decimal d1, decimal d2)
		{
			return Compare(d1, d2) < 0;
		}

		public static bool operator <=(decimal d1, decimal d2)
		{
			return Compare(d1, d2) <= 0;
		}
	}
}
