using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Int64 : IFormattable, IConvertible, IComparable, IComparable<long>, IEquatable<long>
	{
		public const long MaxValue = 9223372036854775807L;

		public const long MinValue = -9223372036854775808L;

		internal long m_value;

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
			if (!(value is long))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Int64"));
			}
			long num = (long)value;
			if (this == num)
			{
				return 0;
			}
			return (this >= num) ? 1 : (-1);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is long))
			{
				return false;
			}
			return (long)obj == this;
		}

		public override int GetHashCode()
		{
			return (int)(this & 0xFFFFFFFFu) ^ (int)(this >> 32);
		}

		public int CompareTo(long value)
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

		public bool Equals(long obj)
		{
			return obj == this;
		}

		internal static bool Parse(string s, bool tryParse, out long result, out Exception exc)
		{
			long num = 0L;
			int num2 = 1;
			bool flag = false;
			result = 0L;
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
					exc = int.GetFormatException();
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
			while (true)
			{
				if (i < length)
				{
					char c = s[i];
					if (c >= '0' && c <= '9')
					{
						byte b = (byte)(c - 48);
						if (num > 922337203685477580L)
						{
							break;
						}
						if (num == 922337203685477580L)
						{
							if ((long)(int)b > 7L && (num2 == 1 || (long)(int)b > 8L))
							{
								break;
							}
							num = ((num2 != -1) ? (num * 10 + (int)b) : (num * num2 * 10 - (int)b));
							if (int.ProcessTrailingWhitespace(tryParse, s, i + 1, ref exc))
							{
								result = num;
								return true;
							}
							break;
						}
						num = num * 10 + (int)b;
						flag = true;
					}
					else if (!int.ProcessTrailingWhitespace(tryParse, s, i, ref exc))
					{
						return false;
					}
					i++;
					continue;
				}
				if (!flag)
				{
					if (!tryParse)
					{
						exc = int.GetFormatException();
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
			if (!tryParse)
			{
				exc = new OverflowException("Value is too large");
			}
			return false;
		}

		public static long Parse(string s, IFormatProvider provider)
		{
			return Parse(s, NumberStyles.Integer, provider);
		}

		public static long Parse(string s, NumberStyles style)
		{
			return Parse(s, style, null);
		}

		internal static bool Parse(string s, NumberStyles style, IFormatProvider fp, bool tryParse, out long result, out Exception exc)
		{
			result = 0L;
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
					exc = new FormatException("Input string was not in the correct format: s.Length==0.");
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
			if (!int.CheckStyle(style, tryParse, ref exc))
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
			int pos = 0;
			if (flag9 && !int.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
			{
				return false;
			}
			bool flag10 = false;
			bool negative = false;
			bool foundSign = false;
			bool foundCurrency = false;
			if (flag5 && s[pos] == '(')
			{
				flag10 = true;
				foundSign = true;
				negative = true;
				pos++;
				if (flag9 && !int.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
				{
					return false;
				}
				if (s.Substring(pos, numberFormatInfo.NegativeSign.Length) == numberFormatInfo.NegativeSign)
				{
					if (!tryParse)
					{
						exc = new FormatException("Input string was not in the correct format: Has Negative Sign.");
					}
					return false;
				}
				if (s.Substring(pos, numberFormatInfo.PositiveSign.Length) == numberFormatInfo.PositiveSign)
				{
					if (!tryParse)
					{
						exc = new FormatException("Input string was not in the correct format: Has Positive Sign.");
					}
					return false;
				}
			}
			if (flag7 && !foundSign)
			{
				int.FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
				if (foundSign)
				{
					if (flag9 && !int.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (flag)
					{
						int.FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
						if (foundCurrency && flag9 && !int.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
						{
							return false;
						}
					}
				}
			}
			if (flag && !foundCurrency)
			{
				int.FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
				if (foundCurrency)
				{
					if (flag9 && !int.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (foundCurrency && !foundSign && flag7)
					{
						int.FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
						if (foundSign && flag9 && !int.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
						{
							return false;
						}
					}
				}
			}
			long num = 0L;
			int num2 = 0;
			bool flag11 = false;
			do
			{
				if (!int.ValidDigit(s[pos], flag2))
				{
					if (!flag3 || (!int.FindOther(ref pos, s, numberFormatInfo.NumberGroupSeparator) && !int.FindOther(ref pos, s, numberFormatInfo.CurrencyGroupSeparator)))
					{
						if (flag11 || !flag4 || (!int.FindOther(ref pos, s, numberFormatInfo.NumberDecimalSeparator) && !int.FindOther(ref pos, s, numberFormatInfo.CurrencyDecimalSeparator)))
						{
							break;
						}
						flag11 = true;
					}
					continue;
				}
				if (flag2)
				{
					num2++;
					char c = s[pos++];
					int num3 = (char.IsDigit(c) ? (c - 48) : ((!char.IsLower(c)) ? (c - 65 + 10) : (c - 97 + 10)));
					ulong num4 = (ulong)num;
					try
					{
						num = (long)checked(num4 * 16 + (ulong)num3);
					}
					catch (OverflowException ex)
					{
						if (!tryParse)
						{
							exc = ex;
						}
						return false;
					}
					continue;
				}
				if (flag11)
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
					exc = new FormatException("Input string was not in the correct format: nDigits == 0.");
				}
				return false;
			}
			if (flag6 && !foundSign)
			{
				int.FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
				if (foundSign)
				{
					if (flag8 && !int.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (flag)
					{
						int.FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
					}
				}
			}
			if (flag && !foundCurrency)
			{
				if (numberFormatInfo.CurrencyPositivePattern == 3 && s[pos++] != ' ')
				{
					if (tryParse)
					{
						return false;
					}
					throw new FormatException("Input string was not in the correct format: no space between number and currency symbol.");
				}
				int.FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
				if (foundCurrency && pos < s.Length)
				{
					if (flag8 && !int.JumpOverWhite(ref pos, s, true, tryParse, ref exc))
					{
						return false;
					}
					if (!foundSign && flag6)
					{
						int.FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
					}
				}
			}
			if (flag8 && pos < s.Length && !int.JumpOverWhite(ref pos, s, false, tryParse, ref exc))
			{
				return false;
			}
			if (flag10)
			{
				if (pos >= s.Length || s[pos++] != ')')
				{
					if (!tryParse)
					{
						exc = new FormatException("Input string was not in the correct format: No room for close parens.");
					}
					return false;
				}
				if (flag8 && pos < s.Length && !int.JumpOverWhite(ref pos, s, false, tryParse, ref exc))
				{
					return false;
				}
			}
			if (pos < s.Length && s[pos] != 0)
			{
				if (!tryParse)
				{
					exc = new FormatException("Input string was not in the correct format: Did not parse entire string. pos = " + pos + " s.Length = " + s.Length);
				}
				return false;
			}
			if (!negative && !flag2)
			{
				try
				{
					num = checked(-num);
				}
				catch (OverflowException ex3)
				{
					if (!tryParse)
					{
						exc = ex3;
					}
					return false;
				}
			}
			result = num;
			return true;
		}

		public static long Parse(string s)
		{
			long result;
			Exception exc;
			if (!Parse(s, false, out result, out exc))
			{
				throw exc;
			}
			return result;
		}

		public static long Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			long result;
			Exception exc;
			if (!Parse(s, style, provider, false, out result, out exc))
			{
				throw exc;
			}
			return result;
		}

		public static bool TryParse(string s, out long result)
		{
			Exception exc;
			if (!Parse(s, true, out result, out exc))
			{
				result = 0L;
				return false;
			}
			return true;
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out long result)
		{
			Exception exc;
			if (!Parse(s, style, provider, true, out result, out exc))
			{
				result = 0L;
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
			return TypeCode.Int64;
		}
	}
}
