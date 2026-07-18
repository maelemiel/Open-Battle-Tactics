using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[CLSCompliant(false)]
	[ComVisible(true)]
	public struct UInt16 : IFormattable, IConvertible, IComparable, IComparable<ushort>, IEquatable<ushort>
	{
		public const ushort MaxValue = 65535;

		public const ushort MinValue = 0;

		internal ushort m_value;

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
			return this;
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
			if (!(value is ushort))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.UInt16."));
			}
			return this - (ushort)value;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ushort))
			{
				return false;
			}
			return (ushort)obj == this;
		}

		public override int GetHashCode()
		{
			return this;
		}

		public int CompareTo(ushort value)
		{
			return this - value;
		}

		public bool Equals(ushort obj)
		{
			return obj == this;
		}

		[CLSCompliant(false)]
		public static ushort Parse(string s, IFormatProvider provider)
		{
			return Parse(s, NumberStyles.Integer, provider);
		}

		[CLSCompliant(false)]
		public static ushort Parse(string s, NumberStyles style)
		{
			return Parse(s, style, null);
		}

		[CLSCompliant(false)]
		public static ushort Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			uint num = uint.Parse(s, style, provider);
			if (num > 65535)
			{
				throw new OverflowException(Locale.GetText("Value too large."));
			}
			return (ushort)num;
		}

		[CLSCompliant(false)]
		public static ushort Parse(string s)
		{
			return Parse(s, NumberStyles.Number, null);
		}

		[CLSCompliant(false)]
		public static bool TryParse(string s, out ushort result)
		{
			return TryParse(s, NumberStyles.Integer, null, out result);
		}

		[CLSCompliant(false)]
		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out ushort result)
		{
			result = 0;
			uint result2;
			if (!uint.TryParse(s, style, provider, out result2))
			{
				return false;
			}
			if (result2 > 65535)
			{
				return false;
			}
			result = (ushort)result2;
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
			return TypeCode.UInt16;
		}
	}
}
