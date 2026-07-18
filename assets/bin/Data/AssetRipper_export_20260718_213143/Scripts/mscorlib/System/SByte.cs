using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	[CLSCompliant(false)]
	public struct SByte : IFormattable, IConvertible, IComparable, IComparable<sbyte>, IEquatable<sbyte>
	{
		public const sbyte MinValue = -128;

		public const sbyte MaxValue = 127;

		internal sbyte m_value;

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
			return this;
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

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is sbyte))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.SByte."));
			}
			sbyte b = (sbyte)obj;
			if (this == b)
			{
				return 0;
			}
			if (this > b)
			{
				return 1;
			}
			return -1;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is sbyte))
			{
				return false;
			}
			return (sbyte)obj == this;
		}

		public override int GetHashCode()
		{
			return this;
		}

		public int CompareTo(sbyte value)
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

		public bool Equals(sbyte obj)
		{
			return obj == this;
		}

		internal static bool Parse(string s, bool tryParse, out sbyte result, out Exception exc)
		{
			int num = 0;
			bool flag = false;
			bool flag2 = false;
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
				flag = true;
				i++;
				break;
			}
			for (; i < length; i++)
			{
				char c = s[i];
				if (c >= '0' && c <= '9')
				{
					if (tryParse)
					{
						int num2 = num * 10 - (c - 48);
						if (num2 < -128)
						{
							return false;
						}
						num = (sbyte)num2;
					}
					else
					{
						num = checked(num * 10 - (c - 48));
					}
					flag2 = true;
					continue;
				}
				if (char.IsWhiteSpace(c))
				{
					for (i++; i < length; i++)
					{
						if (!char.IsWhiteSpace(s[i]))
						{
							if (!tryParse)
							{
								exc = int.GetFormatException();
							}
							return false;
						}
					}
					break;
				}
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			if (!flag2)
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			num = ((!flag) ? (-num) : num);
			if (num < -128 || num > 127)
			{
				if (!tryParse)
				{
					exc = new OverflowException();
				}
				return false;
			}
			result = (sbyte)num;
			return true;
		}

		[CLSCompliant(false)]
		public static sbyte Parse(string s, IFormatProvider provider)
		{
			return Parse(s, NumberStyles.Integer, provider);
		}

		[CLSCompliant(false)]
		public static sbyte Parse(string s, NumberStyles style)
		{
			return Parse(s, style, null);
		}

		[CLSCompliant(false)]
		public static sbyte Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			int num = int.Parse(s, style, provider);
			if (num > 127 || num < -128)
			{
				throw new OverflowException(Locale.GetText("Value too large or too small."));
			}
			return (sbyte)num;
		}

		[CLSCompliant(false)]
		public static sbyte Parse(string s)
		{
			sbyte result;
			Exception exc;
			if (!Parse(s, false, out result, out exc))
			{
				throw exc;
			}
			return result;
		}

		[CLSCompliant(false)]
		public static bool TryParse(string s, out sbyte result)
		{
			Exception exc;
			if (!Parse(s, true, out result, out exc))
			{
				result = 0;
				return false;
			}
			return true;
		}

		[CLSCompliant(false)]
		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out sbyte result)
		{
			result = 0;
			int result2;
			if (!int.TryParse(s, style, provider, out result2))
			{
				return false;
			}
			if (result2 > 127 || result2 < -128)
			{
				return false;
			}
			result = (sbyte)result2;
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
			return TypeCode.SByte;
		}
	}
}
