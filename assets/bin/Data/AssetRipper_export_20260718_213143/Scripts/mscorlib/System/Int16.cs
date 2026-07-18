using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Int16 : IFormattable, IConvertible, IComparable, IComparable<short>, IEquatable<short>
	{
		public const short MaxValue = 32767;

		public const short MinValue = -32768;

		internal short m_value;

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
			if (!(value is short))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Int16"));
			}
			short num = (short)value;
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
			if (!(obj is short))
			{
				return false;
			}
			return (short)obj == this;
		}

		public override int GetHashCode()
		{
			return this;
		}

		public int CompareTo(short value)
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

		public bool Equals(short obj)
		{
			return obj == this;
		}

		internal static bool Parse(string s, bool tryParse, out short result, out Exception exc)
		{
			short num = 0;
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
						if (num > 3276)
						{
							break;
						}
						if (num == 3276)
						{
							if (b > 7 && (num2 == 1 || b > 8))
							{
								break;
							}
							num = ((num2 != -1) ? ((short)(num * 10 + b)) : ((short)(num * num2 * 10 - b)));
							if (int.ProcessTrailingWhitespace(tryParse, s, i + 1, ref exc))
							{
								result = num;
								return true;
							}
							break;
						}
						num = (short)(num * 10 + b);
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
					result = (short)(num * num2);
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

		public static short Parse(string s, IFormatProvider provider)
		{
			return Parse(s, NumberStyles.Integer, provider);
		}

		public static short Parse(string s, NumberStyles style)
		{
			return Parse(s, style, null);
		}

		public static short Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			int num = int.Parse(s, style, provider);
			if (num > 32767 || num < -32768)
			{
				throw new OverflowException("Value too large or too small.");
			}
			return (short)num;
		}

		public static short Parse(string s)
		{
			short result;
			Exception exc;
			if (!Parse(s, false, out result, out exc))
			{
				throw exc;
			}
			return result;
		}

		public static bool TryParse(string s, out short result)
		{
			Exception exc;
			if (!Parse(s, true, out result, out exc))
			{
				result = 0;
				return false;
			}
			return true;
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out short result)
		{
			result = 0;
			int result2;
			if (!int.TryParse(s, style, provider, out result2))
			{
				return false;
			}
			if (result2 > 32767 || result2 < -32768)
			{
				return false;
			}
			result = (short)result2;
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
			return TypeCode.Int16;
		}
	}
}
