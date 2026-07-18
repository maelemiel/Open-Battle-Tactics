using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Byte : IFormattable, IConvertible, IComparable, IComparable<byte>, IEquatable<byte>
	{
		public const byte MinValue = 0;

		public const byte MaxValue = 255;

		internal byte m_value;

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
			return this;
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this);
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
			if (!(value is byte))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Byte."));
			}
			byte b = (byte)value;
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
			if (!(obj is byte))
			{
				return false;
			}
			return (byte)obj == this;
		}

		public override int GetHashCode()
		{
			return this;
		}

		public int CompareTo(byte value)
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

		public bool Equals(byte obj)
		{
			return this == obj;
		}

		public static byte Parse(string s, IFormatProvider provider)
		{
			return Parse(s, NumberStyles.Integer, provider);
		}

		public static byte Parse(string s, NumberStyles style)
		{
			return Parse(s, style, null);
		}

		public static byte Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			uint num = uint.Parse(s, style, provider);
			if (num > 255)
			{
				throw new OverflowException(Locale.GetText("Value too large."));
			}
			return (byte)num;
		}

		public static byte Parse(string s)
		{
			return Parse(s, NumberStyles.Integer, null);
		}

		public static bool TryParse(string s, out byte result)
		{
			return TryParse(s, NumberStyles.Integer, null, out result);
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out byte result)
		{
			result = 0;
			uint result2;
			if (!uint.TryParse(s, style, provider, out result2))
			{
				return false;
			}
			if (result2 > 255)
			{
				return false;
			}
			result = (byte)result2;
			return true;
		}

		public override string ToString()
		{
			return NumberFormatter.NumberToString(this, null);
		}

		public string ToString(string format)
		{
			return ToString(format, null);
		}

		public string ToString(IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(this, provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(format, this, provider);
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Byte;
		}
	}
}
