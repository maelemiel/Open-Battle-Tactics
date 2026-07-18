using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Boolean : IConvertible, IComparable, IComparable<bool>, IEquatable<bool>
	{
		public static readonly string FalseString = "False";

		public static readonly string TrueString = "True";

		internal bool m_value;

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
			return this;
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

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is bool))
			{
				throw new ArgumentException(Locale.GetText("Object is not a Boolean."));
			}
			bool flag = (bool)obj;
			if (this && !flag)
			{
				return 1;
			}
			return (this != flag) ? (-1) : 0;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is bool))
			{
				return false;
			}
			bool flag = (bool)obj;
			return (!this) ? (!flag) : flag;
		}

		public int CompareTo(bool value)
		{
			if (this == value)
			{
				return 0;
			}
			return this ? 1 : (-1);
		}

		public bool Equals(bool obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return this ? 1 : 0;
		}

		public static bool Parse(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			value = value.Trim();
			if (string.Compare(value, TrueString, true, CultureInfo.InvariantCulture) == 0)
			{
				return true;
			}
			if (string.Compare(value, FalseString, true, CultureInfo.InvariantCulture) == 0)
			{
				return false;
			}
			throw new FormatException(Locale.GetText("Value is not equivalent to either TrueString or FalseString."));
		}

		public static bool TryParse(string value, out bool result)
		{
			result = false;
			if (value == null)
			{
				return false;
			}
			value = value.Trim();
			if (string.Compare(value, TrueString, true, CultureInfo.InvariantCulture) == 0)
			{
				result = true;
				return true;
			}
			if (string.Compare(value, FalseString, true, CultureInfo.InvariantCulture) == 0)
			{
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return (!this) ? FalseString : TrueString;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Boolean;
		}

		public string ToString(IFormatProvider provider)
		{
			return ToString();
		}
	}
}
