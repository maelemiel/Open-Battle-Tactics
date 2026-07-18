using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public abstract class Enum : ValueType, IFormattable, IConvertible, IComparable
	{
		private static char[] split_char = new char[1] { ',' };

		private object Value
		{
			get
			{
				return get_value();
			}
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(Value, provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(Value, provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(Value, provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(Value, provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(Value, provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(Value, provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(Value, provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(Value, provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(Value, provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(Value, provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(Value, provider);
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (targetType == typeof(string))
			{
				return ToString(provider);
			}
			return Convert.ToType(Value, targetType, provider, false);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(Value, provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(Value, provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(Value, provider);
		}

		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(GetUnderlyingType(GetType()));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object get_value();

		[ComVisible(true)]
		public static Array GetValues(Type enumType)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType is not an Enum type.", "enumType");
			}
			MonoEnumInfo info;
			MonoEnumInfo.GetInfo(enumType, out info);
			return (Array)info.values.Clone();
		}

		[ComVisible(true)]
		public static string[] GetNames(Type enumType)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType is not an Enum type.");
			}
			MonoEnumInfo info;
			MonoEnumInfo.GetInfo(enumType, out info);
			return (string[])info.names.Clone();
		}

		private static int FindPosition(object value, Array values)
		{
			int[] array = values as int[];
			if (array != null)
			{
				return Array.BinarySearch((Array)array, (object)(int)value, (IComparer)MonoEnumInfo.int_comparer);
			}
			uint[] array2 = values as uint[];
			if (array2 != null)
			{
				return Array.BinarySearch(array2, (uint)value);
			}
			short[] array3 = values as short[];
			if (array3 != null)
			{
				return Array.BinarySearch((Array)array3, (object)(short)value, (IComparer)MonoEnumInfo.short_comparer);
			}
			ushort[] array4 = values as ushort[];
			if (array4 != null)
			{
				return Array.BinarySearch(array4, (ushort)value);
			}
			sbyte[] array5 = values as sbyte[];
			if (array5 != null)
			{
				return Array.BinarySearch((Array)array5, (object)(sbyte)value, (IComparer)MonoEnumInfo.sbyte_comparer);
			}
			byte[] array6 = values as byte[];
			if (array6 != null)
			{
				return Array.BinarySearch(array6, (byte)value);
			}
			long[] array7 = values as long[];
			if (array7 != null)
			{
				return Array.BinarySearch((Array)array7, (object)(long)value, (IComparer)MonoEnumInfo.long_comparer);
			}
			ulong[] array8 = values as ulong[];
			if (array8 != null)
			{
				return Array.BinarySearch(array8, (ulong)value);
			}
			return Array.BinarySearch(values, value);
		}

		[ComVisible(true)]
		public static string GetName(Type enumType, object value)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType is not an Enum type.", "enumType");
			}
			value = ToObject(enumType, value);
			MonoEnumInfo info;
			MonoEnumInfo.GetInfo(enumType, out info);
			int num = FindPosition(value, info.values);
			return (num < 0) ? null : info.names[num];
		}

		[ComVisible(true)]
		public static bool IsDefined(Type enumType, object value)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType is not an Enum type.", "enumType");
			}
			MonoEnumInfo info;
			MonoEnumInfo.GetInfo(enumType, out info);
			Type type = value.GetType();
			if (type == typeof(string))
			{
				return ((IList)info.names).Contains(value);
			}
			if (type == info.utype || type == enumType)
			{
				value = ToObject(enumType, value);
				MonoEnumInfo.GetInfo(enumType, out info);
				return FindPosition(value, info.values) >= 0;
			}
			throw new ArgumentException("The value parameter is not the correct type.It must be type String or the same type as the underlying typeof the Enum.");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type get_underlying_type(Type enumType);

		[ComVisible(true)]
		public static Type GetUnderlyingType(Type enumType)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType is not an Enum type.", "enumType");
			}
			return get_underlying_type(enumType);
		}

		[ComVisible(true)]
		public static object Parse(Type enumType, string value)
		{
			return Parse(enumType, value, false);
		}

		private static int FindName(Hashtable name_hash, string[] names, string name, bool ignoreCase)
		{
			if (!ignoreCase)
			{
				if (name_hash != null)
				{
					object obj = name_hash[name];
					if (obj != null)
					{
						return (int)obj;
					}
				}
				else
				{
					for (int i = 0; i < names.Length; i++)
					{
						if (name == names[i])
						{
							return i;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < names.Length; j++)
				{
					if (string.Compare(name, names[j], ignoreCase, CultureInfo.InvariantCulture) == 0)
					{
						return j;
					}
				}
			}
			return -1;
		}

		private static ulong GetValue(object value, TypeCode typeCode)
		{
			switch (typeCode)
			{
			case TypeCode.Byte:
				return (byte)value;
			case TypeCode.SByte:
				return (byte)(sbyte)value;
			case TypeCode.Int16:
				return (ushort)(short)value;
			case TypeCode.Int32:
				return (uint)(int)value;
			case TypeCode.Int64:
				return (ulong)(long)value;
			case TypeCode.UInt16:
				return (ushort)value;
			case TypeCode.UInt32:
				return (uint)value;
			case TypeCode.UInt64:
				return (ulong)value;
			default:
				throw new ArgumentException("typeCode is not a valid type code for an Enum");
			}
		}

		[ComVisible(true)]
		public static object Parse(Type enumType, string value, bool ignoreCase)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType is not an Enum type.", "enumType");
			}
			value = value.Trim();
			if (value.Length == 0)
			{
				throw new ArgumentException("An empty string is not considered a valid value.");
			}
			MonoEnumInfo info;
			MonoEnumInfo.GetInfo(enumType, out info);
			int num = FindName(info.name_hash, info.names, value, ignoreCase);
			if (num >= 0)
			{
				return info.values.GetValue(num);
			}
			TypeCode typeCode = ((Enum)info.values.GetValue(0)).GetTypeCode();
			if (value.IndexOf(',') != -1)
			{
				string[] array = value.Split(split_char);
				ulong num2 = 0uL;
				for (int i = 0; i < array.Length; i++)
				{
					num = FindName(info.name_hash, info.names, array[i].Trim(), ignoreCase);
					if (num < 0)
					{
						throw new ArgumentException("The requested value was not found.");
					}
					num2 |= GetValue(info.values.GetValue(num), typeCode);
				}
				return ToObject(enumType, num2);
			}
			switch (typeCode)
			{
			case TypeCode.SByte:
			{
				sbyte result8;
				if (sbyte.TryParse(value, out result8))
				{
					return ToObject(enumType, result8);
				}
				break;
			}
			case TypeCode.Byte:
			{
				byte result4;
				if (byte.TryParse(value, out result4))
				{
					return ToObject(enumType, result4);
				}
				break;
			}
			case TypeCode.Int16:
			{
				short result6;
				if (short.TryParse(value, out result6))
				{
					return ToObject(enumType, result6);
				}
				break;
			}
			case TypeCode.UInt16:
			{
				ushort result2;
				if (ushort.TryParse(value, out result2))
				{
					return ToObject(enumType, result2);
				}
				break;
			}
			case TypeCode.Int32:
			{
				int result7;
				if (int.TryParse(value, out result7))
				{
					return ToObject(enumType, result7);
				}
				break;
			}
			case TypeCode.UInt32:
			{
				uint result5;
				if (uint.TryParse(value, out result5))
				{
					return ToObject(enumType, result5);
				}
				break;
			}
			case TypeCode.Int64:
			{
				long result3;
				if (long.TryParse(value, out result3))
				{
					return ToObject(enumType, result3);
				}
				break;
			}
			case TypeCode.UInt64:
			{
				ulong result;
				if (ulong.TryParse(value, out result))
				{
					return ToObject(enumType, result);
				}
				break;
			}
			}
			throw new ArgumentException(string.Format("The requested value '{0}' was not found.", value));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int compare_value_to(object other);

		public int CompareTo(object target)
		{
			if (target == null)
			{
				return 1;
			}
			Type type = GetType();
			if (target.GetType() != type)
			{
				throw new ArgumentException(string.Format("Object must be the same type as the enum. The type passed in was {0}; the enum type was {1}.", target.GetType(), type));
			}
			return compare_value_to(target);
		}

		public override string ToString()
		{
			return ToString("G");
		}

		[Obsolete("Provider is ignored, just use ToString")]
		public string ToString(IFormatProvider provider)
		{
			return ToString("G", provider);
		}

		public string ToString(string format)
		{
			if (format == string.Empty || format == null)
			{
				format = "G";
			}
			return Format(GetType(), Value, format);
		}

		[Obsolete("Provider is ignored, just use ToString")]
		public string ToString(string format, IFormatProvider provider)
		{
			if (format == string.Empty || format == null)
			{
				format = "G";
			}
			return Format(GetType(), Value, format);
		}

		[ComVisible(true)]
		public static object ToObject(Type enumType, byte value)
		{
			return ToObject(enumType, (object)value);
		}

		[ComVisible(true)]
		public static object ToObject(Type enumType, short value)
		{
			return ToObject(enumType, (object)value);
		}

		[ComVisible(true)]
		public static object ToObject(Type enumType, int value)
		{
			return ToObject(enumType, (object)value);
		}

		[ComVisible(true)]
		public static object ToObject(Type enumType, long value)
		{
			return ToObject(enumType, (object)value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ComVisible(true)]
		public static extern object ToObject(Type enumType, object value);

		[ComVisible(true)]
		[CLSCompliant(false)]
		public static object ToObject(Type enumType, sbyte value)
		{
			return ToObject(enumType, (object)value);
		}

		[ComVisible(true)]
		[CLSCompliant(false)]
		public static object ToObject(Type enumType, ushort value)
		{
			return ToObject(enumType, (object)value);
		}

		[ComVisible(true)]
		[CLSCompliant(false)]
		public static object ToObject(Type enumType, uint value)
		{
			return ToObject(enumType, (object)value);
		}

		[ComVisible(true)]
		[CLSCompliant(false)]
		public static object ToObject(Type enumType, ulong value)
		{
			return ToObject(enumType, (object)value);
		}

		public override bool Equals(object obj)
		{
			return ValueType.DefaultEquals(this, obj);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int get_hashcode();

		public override int GetHashCode()
		{
			return get_hashcode();
		}

		private static string FormatSpecifier_X(Type enumType, object value, bool upper)
		{
			switch (Type.GetTypeCode(enumType))
			{
			case TypeCode.SByte:
				return ((sbyte)value).ToString((!upper) ? "x2" : "X2");
			case TypeCode.Byte:
				return ((byte)value).ToString((!upper) ? "x2" : "X2");
			case TypeCode.Int16:
				return ((short)value).ToString((!upper) ? "x4" : "X4");
			case TypeCode.UInt16:
				return ((ushort)value).ToString((!upper) ? "x4" : "X4");
			case TypeCode.Int32:
				return ((int)value).ToString((!upper) ? "x8" : "X8");
			case TypeCode.UInt32:
				return ((uint)value).ToString((!upper) ? "x8" : "X8");
			case TypeCode.Int64:
				return ((long)value).ToString((!upper) ? "x16" : "X16");
			case TypeCode.UInt64:
				return ((ulong)value).ToString((!upper) ? "x16" : "X16");
			default:
				throw new Exception("Invalid type code for enumeration.");
			}
		}

		private static string FormatFlags(Type enumType, object value)
		{
			string text = string.Empty;
			MonoEnumInfo info;
			MonoEnumInfo.GetInfo(enumType, out info);
			string text2 = value.ToString();
			if (text2 == "0")
			{
				text = GetName(enumType, value);
				if (text == null)
				{
					text = text2;
				}
				return text;
			}
			switch (((Enum)info.values.GetValue(0)).GetTypeCode())
			{
			case TypeCode.SByte:
			{
				sbyte b = (sbyte)value;
				for (int num7 = info.values.Length - 1; num7 >= 0; num7--)
				{
					sbyte b2 = (sbyte)info.values.GetValue(num7);
					if (b2 != 0 && (b & b2) == b2)
					{
						text = info.names[num7] + ((!(text == string.Empty)) ? ", " : string.Empty) + text;
						b -= b2;
					}
				}
				if (b != 0)
				{
					return text2;
				}
				break;
			}
			case TypeCode.Byte:
			{
				byte b3 = (byte)value;
				for (int num8 = info.values.Length - 1; num8 >= 0; num8--)
				{
					byte b4 = (byte)info.values.GetValue(num8);
					if (b4 != 0 && (b3 & b4) == b4)
					{
						text = info.names[num8] + ((!(text == string.Empty)) ? ", " : string.Empty) + text;
						b3 -= b4;
					}
				}
				if (b3 != 0)
				{
					return text2;
				}
				break;
			}
			case TypeCode.Int16:
			{
				short num18 = (short)value;
				for (int num19 = info.values.Length - 1; num19 >= 0; num19--)
				{
					short num20 = (short)info.values.GetValue(num19);
					if (num20 != 0 && (num18 & num20) == num20)
					{
						text = info.names[num19] + ((!(text == string.Empty)) ? ", " : string.Empty) + text;
						num18 -= num20;
					}
				}
				if (num18 != 0)
				{
					return text2;
				}
				break;
			}
			case TypeCode.Int32:
			{
				int num12 = (int)value;
				for (int num13 = info.values.Length - 1; num13 >= 0; num13--)
				{
					int num14 = (int)info.values.GetValue(num13);
					if (num14 != 0 && (num12 & num14) == num14)
					{
						text = info.names[num13] + ((!(text == string.Empty)) ? ", " : string.Empty) + text;
						num12 -= num14;
					}
				}
				if (num12 != 0)
				{
					return text2;
				}
				break;
			}
			case TypeCode.UInt16:
			{
				ushort num15 = (ushort)value;
				for (int num16 = info.values.Length - 1; num16 >= 0; num16--)
				{
					ushort num17 = (ushort)info.values.GetValue(num16);
					if (num17 != 0 && (num15 & num17) == num17)
					{
						text = info.names[num16] + ((!(text == string.Empty)) ? ", " : string.Empty) + text;
						num15 -= num17;
					}
				}
				if (num15 != 0)
				{
					return text2;
				}
				break;
			}
			case TypeCode.UInt32:
			{
				uint num4 = (uint)value;
				for (int num5 = info.values.Length - 1; num5 >= 0; num5--)
				{
					uint num6 = (uint)info.values.GetValue(num5);
					if (num6 != 0 && (num4 & num6) == num6)
					{
						text = info.names[num5] + ((!(text == string.Empty)) ? ", " : string.Empty) + text;
						num4 -= num6;
					}
				}
				if (num4 != 0)
				{
					return text2;
				}
				break;
			}
			case TypeCode.Int64:
			{
				long num9 = (long)value;
				for (int num10 = info.values.Length - 1; num10 >= 0; num10--)
				{
					long num11 = (long)info.values.GetValue(num10);
					if (num11 != 0L && (num9 & num11) == num11)
					{
						text = info.names[num10] + ((!(text == string.Empty)) ? ", " : string.Empty) + text;
						num9 -= num11;
					}
				}
				if (num9 != 0L)
				{
					return text2;
				}
				break;
			}
			case TypeCode.UInt64:
			{
				ulong num = (ulong)value;
				for (int num2 = info.values.Length - 1; num2 >= 0; num2--)
				{
					ulong num3 = (ulong)info.values.GetValue(num2);
					if (num3 != 0L && (num & num3) == num3)
					{
						text = info.names[num2] + ((!(text == string.Empty)) ? ", " : string.Empty) + text;
						num -= num3;
					}
				}
				if (num != 0L)
				{
					return text2;
				}
				break;
			}
			}
			if (text == string.Empty)
			{
				return text2;
			}
			return text;
		}

		[ComVisible(true)]
		public static string Format(Type enumType, object value, string format)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("enumType is not an Enum type.", "enumType");
			}
			Type type = value.GetType();
			Type underlyingType = GetUnderlyingType(enumType);
			if (type.IsEnum)
			{
				if (type != enumType)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Object must be the same type as the enum. The type passed in was {0}; the enum type was {1}.", type.FullName, enumType.FullName));
				}
			}
			else if (type != underlyingType)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Enum underlying type and the object must be the same type or object. Type passed in was {0}; the enum underlying type was {1}.", type.FullName, underlyingType.FullName));
			}
			if (format.Length != 1)
			{
				throw new FormatException("Format String can be only \"G\",\"g\",\"X\",\"x\",\"F\",\"f\",\"D\" or \"d\".");
			}
			char c = format[0];
			string text;
			if (c == 'G' || c == 'g')
			{
				if (!enumType.IsDefined(typeof(FlagsAttribute), false))
				{
					text = GetName(enumType, value);
					if (text == null)
					{
						text = value.ToString();
					}
					return text;
				}
				c = 'f';
			}
			if (c == 'f' || c == 'F')
			{
				return FormatFlags(enumType, value);
			}
			text = string.Empty;
			switch (c)
			{
			case 'X':
				return FormatSpecifier_X(enumType, value, true);
			case 'x':
				return FormatSpecifier_X(enumType, value, false);
			case 'D':
			case 'd':
				if (underlyingType == typeof(ulong))
				{
					return Convert.ToUInt64(value).ToString();
				}
				return Convert.ToInt64(value).ToString();
			default:
				throw new FormatException("Format String can be only \"G\",\"g\",\"X\",\"x\",\"F\",\"f\",\"D\" or \"d\".");
			}
		}
	}
}
