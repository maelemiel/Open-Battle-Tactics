using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Int32 : IFormattable, IConvertible, IComparable, IComparable<int>, IEquatable<int>
	{
		public const int MaxValue = 2147483647;

		public const int MinValue = -2147483648;

		internal int m_value;

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
			return this;
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
			if (!(value is int))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Int32"));
			}
			int num = (int)value;
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
			if (!(obj is int))
			{
				return false;
			}
			return (int)obj == this;
		}

		public override int GetHashCode()
		{
			return this;
		}

		public int CompareTo(int value)
		{
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

		public bool Equals(int obj)
		{
			return obj == this;
		}

		internal static bool ProcessTrailingWhitespace(bool tryParse, string s, int position, ref Exception exc)
		{
			int length = s.Length;
			for (int i = position; i < length; i++)
			{
				char c = s[i];
				if (c != 0 && !char.IsWhiteSpace(c))
				{
					if (!tryParse)
					{
						exc = GetFormatException();
					}
					return false;
				}
			}
			return true;
		}

		internal static bool Parse(string s, bool tryParse, out int result, out Exception exc)
		{
			int num = 0;
			int num2 = 1;
			bool flag = false;
			result = 0;
			exc = null;
			if (s == null)
			{
				if (!tryParse)
				{
					exc = new ArgumentNullException("s");
				}
				return false;
			}
			int length = s.Length;
			int i;
			for (i = 0; i < length; i++)
			{
				char c = s[i];
				if (!char.IsWhiteSpace(c))
				{
					break;
				}
			}
			if (i == length)
			{
				if (!tryParse)
				{
					exc = GetFormatException();
				}
				return false;
			}
			switch (s[i])
			{
			case '+':
				i++;
				break;
			case '-':
				num2 = -1;
				i++;
				break;
			}
			for (; i < length; i++)
			{
				char c = s[i];
				switch (c)
				{
				case '\0':
					i = length;
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				{
					byte b = (byte)(c - 48);
					if (num <= 214748364)
					{
						if (num != 214748364)
						{
							num = num * 10 + b;
							flag = true;
							break;
						}
						if (b <= 7 || (num2 != 1 && b <= 8))
						{
							num = ((num2 != -1) ? (num * 10 + b) : (num * num2 * 10 - b));
							if (ProcessTrailingWhitespace(tryParse, s, i + 1, ref exc))
							{
								result = num;
								return true;
							}
						}
					}
					if (!tryParse)
					{
						exc = new OverflowException("Value is too large");
					}
					return false;
				}
				default:
					if (!ProcessTrailingWhitespace(tryParse, s, i, ref exc))
					{
						return false;
					}
					break;
				}
			}
			if (!flag)
			{
				if (!tryParse)
				{
					exc = GetFormatException();
				}
				return false;
			}
			if (num2 == -1)
			{
				result = num * num2;
			}
			else
			{
				result = num;
			}
			return true;
		}

		public static int Parse(string s, IFormatProvider provider)
		{
			return Parse(s, NumberStyles.Integer, provider);
		}

		public static int Parse(string s, NumberStyles style)
		{
			return Parse(s, style, null);
		}

		internal static bool CheckStyle(NumberStyles style, bool tryParse, ref Exception exc)
		{
			if ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				NumberStyles numberStyles = style ^ NumberStyles.AllowHexSpecifier;
				if ((numberStyles & NumberStyles.AllowLeadingWhite) != NumberStyles.None)
				{
					numberStyles ^= NumberStyles.AllowLeadingWhite;
				}
				if ((numberStyles & NumberStyles.AllowTrailingWhite) != NumberStyles.None)
				{
					numberStyles ^= NumberStyles.AllowTrailingWhite;
				}
				if (numberStyles != NumberStyles.None)
				{
					if (!tryParse)
					{
						exc = new ArgumentException("With AllowHexSpecifier only AllowLeadingWhite and AllowTrailingWhite are permitted.");
					}
					return false;
				}
			}
			else if ((uint)style > 511u)
			{
				if (!tryParse)
				{
					exc = new ArgumentException("Not a valid number style");
				}
				return false;
			}
			return true;
		}

		internal static bool JumpOverWhite(ref int pos, string s, bool reportError, bool tryParse, ref Exception exc)
		{
			while (pos < s.Length && char.IsWhiteSpace(s[pos]))
			{
				pos++;
			}
			if (reportError && pos >= s.Length)
			{
				if (!tryParse)
				{
					exc = GetFormatException();
				}
				return false;
			}
			return true;
		}

		internal static void FindSign(ref int pos, string s, NumberFormatInfo nfi, ref bool foundSign, ref bool negative)
		{
			if (pos + nfi.NegativeSign.Length <= s.Length && s.IndexOf(nfi.NegativeSign, pos, nfi.NegativeSign.Length) == pos)
			{
				negative = true;
				foundSign = true;
				pos += nfi.NegativeSign.Length;
			}
			else if (pos + nfi.PositiveSign.Length < s.Length && s.IndexOf(nfi.PositiveSign, pos, nfi.PositiveSign.Length) == pos)
			{
				negative = false;
				pos += nfi.PositiveSign.Length;
				foundSign = true;
			}
		}

		internal static void FindCurrency(ref int pos, string s, NumberFormatInfo nfi, ref bool foundCurrency)
		{
			if (pos + nfi.CurrencySymbol.Length <= s.Length && s.Substring(pos, nfi.CurrencySymbol.Length) == nfi.CurrencySymbol)
			{
				foundCurrency = true;
				pos += nfi.CurrencySymbol.Length;
			}
		}

		internal static bool FindExponent(ref int pos, string s, ref int exponent, bool tryParse, ref Exception exc)
		{
			exponent = 0;
			long num = 0L;
			int num2 = s.IndexOfAny(new char[2] { 'e', 'E' }, pos);
			if (num2 < 0)
			{
				exc = null;
				return false;
			}
			if (++num2 == s.Length)
			{
				exc = ((!tryParse) ? GetFormatException() : null);
				return true;
			}
			if (s[num2] == '-')
			{
				exc = ((!tryParse) ? new OverflowException("Value too large or too small.") : null);
				return true;
			}
			if (s[num2] == '+' && ++num2 == s.Length)
			{
				exc = ((!tryParse) ? GetFormatException() : null);
				return true;
			}
			for (; num2 < s.Length; num2++)
			{
				if (!char.IsDigit(s[num2]))
				{
					exc = ((!tryParse) ? GetFormatException() : null);
					return true;
				}
				num = checked(num * 10 - (s[num2] - 48));
				if (num < int.MinValue || num > int.MaxValue)
				{
					exc = ((!tryParse) ? new OverflowException("Value too large or too small.") : null);
					return true;
				}
			}
			num = -num;
			exc = null;
			exponent = (int)num;
			pos = num2;
			return true;
		}

		internal static bool FindOther(ref int pos, string s, string other)
		{
			if (pos + other.Length <= s.Length && s.Substring(pos, other.Length) == other)
			{
				pos += other.Length;
				return true;
			}
			return false;
		}

		internal static bool ValidDigit(char e, bool allowHex)
		{
			if (allowHex)
			{
				return char.IsDigit(e) || (e >= 'A' && e <= 'F') || (e >= 'a' && e <= 'f');
			}
			return char.IsDigit(e);
		}

		internal static Exception GetFormatException()
		{
			return new FormatException("Input string was not in the correct format");
		}

		internal static bool Parse(string s, NumberStyles style, IFormatProvider fp, bool tryParse, out int result, out Exception exc)
		{
			result = 0;
			exc = null;
			if (s == null)
			{
				if (!tryParse)
				{
					exc = new ArgumentNullException();
				}
				return false;
			}
			if (s.Length == 0)
			{
				if (!tryParse)
				{
					exc = GetFormatException();
				}
				return false;
			}
			NumberFormatInfo numberFormatInfo = null;
			if (fp != null)
			{
				Type typeFromHandle = typeof(NumberFormatInfo);
				numberFormatInfo = (NumberFormatInfo)fp.GetFormat(typeFromHandle);
			}
			if (numberFormatInfo == null)
			{
				numberFormatInfo = Thread.CurrentThread.CurrentCulture.NumberFormat;
			}
			if (!CheckStyle(style, tryParse, ref exc))
			{
				return false;
			}
			bool flag = (style & NumberStyles.AllowCurrencySymbol) != 0;
			bool flag2 = (style & NumberStyles.AllowHexSpecifier) != 0;
			bool flag3 = (style & NumberStyles.AllowThousands) != 0;
			bool flag4 = (style & NumberStyles.AllowDecimalPoint) != 0;
			bool flag5 = (style & NumberStyles.AllowParentheses) != 0;
			bool flag6 = (style & NumberStyles.AllowTrailingSign) != 0;
			bool flag7 = (style & NumberStyles.AllowLeadingSign) != 0;
			bool flag8 = (style & NumberStyles.AllowTrailingWhite) != 0;
			bool flag9 = (style & NumberStyles.AllowLeadingWhite) != 0;
			bool flag10 = (style & NumberStyles.AllowExponent) != 0;
			int pos = 0;
			if (flag9 && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
			{
				return false;
			}
			bool flag11 = false;
			bool negative = false;
			bool foundSign = false;
			bool foundCurrency = false;
			if (flag5 && s[pos] == '(')
			{
				flag11 = true;
				foundSign = true;
				negative = true;
				pos++;
				if (flag9 && JumpOverWhite(ref pos, s, true, tryParse, ref exc))
				{
					return false;
				}
				if (s.Substring(pos, numberFormatInfo.NegativeSign.Length) == numberFormatInfo.NegativeSign)
				{
					if (!tryParse)
					{
						exc = GetFormatException();
					}
					return false;
				}
				if (s.Substring(pos, numberFormatInfo.PositiveSign.Length) == numberFormatInfo.PositiveSign)
				{
					if (!tryParse)
					{
						exc = GetFormatException();
					}
					return false;
				}
			}
			if (flag7 && !foundSign)
			{
				FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
				if (foundSign)
				{
					if (flag9 && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (flag)
					{
						FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
						if (foundCurrency && flag9 && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
						{
							return false;
						}
					}
				}
			}
			if (flag && !foundCurrency)
			{
				FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
				if (foundCurrency)
				{
					if (flag9 && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (foundCurrency && !foundSign && flag7)
					{
						FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
						if (foundSign && flag9 && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
						{
							return false;
						}
					}
				}
			}
			int num = 0;
			int num2 = 0;
			bool flag12 = false;
			int exponent = 0;
			do
			{
				if (!ValidDigit(s[pos], flag2))
				{
					if (!flag3 || !FindOther(ref pos, s, numberFormatInfo.NumberGroupSeparator))
					{
						if (flag12 || !flag4 || !FindOther(ref pos, s, numberFormatInfo.NumberDecimalSeparator))
						{
							break;
						}
						flag12 = true;
					}
					continue;
				}
				if (flag2)
				{
					num2++;
					char c = s[pos++];
					int num3 = (char.IsDigit(c) ? (c - 48) : ((!char.IsLower(c)) ? (c - 65 + 10) : (c - 97 + 10)));
					uint num4 = (uint)num;
					if (tryParse)
					{
						if ((num4 & 0xF0000000u) != 0)
						{
							return false;
						}
						num = (int)(num4 * 16) + num3;
					}
					else
					{
						num = (int)checked(num4 * 16 + (uint)num3);
					}
					continue;
				}
				if (flag12)
				{
					num2++;
					if (s[pos++] != '0')
					{
						if (!tryParse)
						{
							exc = new OverflowException("Value too large or too small.");
						}
						return false;
					}
					continue;
				}
				num2++;
				try
				{
					num = checked(num * 10 - (s[pos++] - 48));
				}
				catch (OverflowException)
				{
					if (!tryParse)
					{
						exc = new OverflowException("Value too large or too small.");
					}
					return false;
				}
			}
			while (pos < s.Length);
			if (num2 == 0)
			{
				if (!tryParse)
				{
					exc = GetFormatException();
				}
				return false;
			}
			if (flag10 && FindExponent(ref pos, s, ref exponent, tryParse, ref exc) && exc != null)
			{
				return false;
			}
			if (flag6 && !foundSign)
			{
				FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
				if (foundSign)
				{
					if (flag8 && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (flag)
					{
						FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
					}
				}
			}
			if (flag && !foundCurrency)
			{
				FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
				if (foundCurrency)
				{
					if (flag8 && !JumpOverWhite(ref pos, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (!foundSign && flag6)
					{
						FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
					}
				}
			}
			if (flag8 && pos < s.Length && !JumpOverWhite(ref pos, s, false, tryParse, ref exc))
			{
				return false;
			}
			if (flag11)
			{
				if (pos >= s.Length || s[pos++] != ')')
				{
					if (!tryParse)
					{
						exc = GetFormatException();
					}
					return false;
				}
				if (flag8 && pos < s.Length && !JumpOverWhite(ref pos, s, false, tryParse, ref exc))
				{
					return false;
				}
			}
			if (pos < s.Length && s[pos] != 0)
			{
				if (!tryParse)
				{
					exc = GetFormatException();
				}
				return false;
			}
			if (!negative && !flag2)
			{
				if (tryParse)
				{
					long num5 = -num;
					if (num5 < int.MinValue || num5 > int.MaxValue)
					{
						return false;
					}
					num = (int)num5;
				}
				else
				{
					num = checked(-num);
				}
			}
			if (exponent > 0)
			{
				double num6 = Math.Pow(10.0, exponent) * (double)num;
				if (num6 < -2147483648.0 || num6 > 2147483647.0)
				{
					if (!tryParse)
					{
						exc = new OverflowException("Value too large or too small.");
					}
					return false;
				}
				num = (int)num6;
			}
			result = num;
			return true;
		}

		public static int Parse(string s)
		{
			int result;
			Exception exc;
			if (!Parse(s, false, out result, out exc))
			{
				throw exc;
			}
			return result;
		}

		public static int Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			int result;
			Exception exc;
			if (!Parse(s, style, provider, false, out result, out exc))
			{
				throw exc;
			}
			return result;
		}

		public static bool TryParse(string s, out int result)
		{
			Exception exc;
			if (!Parse(s, true, out result, out exc))
			{
				result = 0;
				return false;
			}
			return true;
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out int result)
		{
			Exception exc;
			if (!Parse(s, style, provider, true, out result, out exc))
			{
				result = 0;
				return false;
			}
			return true;
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
			return TypeCode.Int32;
		}
	}
}
