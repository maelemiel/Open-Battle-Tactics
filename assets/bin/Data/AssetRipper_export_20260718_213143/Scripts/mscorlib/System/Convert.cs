using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace System
{
	public static class Convert
	{
		private const int MaxBytesPerLine = 57;

		public static readonly object DBNull = System.DBNull.Value;

		private static readonly Type[] conversionTable = new Type[19]
		{
			null,
			typeof(object),
			typeof(DBNull),
			typeof(bool),
			typeof(char),
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			null,
			typeof(string)
		};

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] InternalFromBase64String(string str, bool allowWhitespaceOnly);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] InternalFromBase64CharArray(char[] arr, int offset, int length);

		public static byte[] FromBase64CharArray(char[] inArray, int offset, int length)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset < 0");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length < 0");
			}
			if (offset > inArray.Length - length)
			{
				throw new ArgumentOutOfRangeException("offset + length > array.Length");
			}
			return InternalFromBase64CharArray(inArray, offset, length);
		}

		public static byte[] FromBase64String(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return new byte[0];
			}
			return InternalFromBase64String(s, true);
		}

		public static TypeCode GetTypeCode(object value)
		{
			if (value == null)
			{
				return TypeCode.Empty;
			}
			return Type.GetTypeCode(value.GetType());
		}

		public static bool IsDBNull(object value)
		{
			if (value is DBNull)
			{
				return true;
			}
			return false;
		}

		public static int ToBase64CharArray(byte[] inArray, int offsetIn, int length, char[] outArray, int offsetOut)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			if (outArray == null)
			{
				throw new ArgumentNullException("outArray");
			}
			if (offsetIn < 0 || length < 0 || offsetOut < 0)
			{
				throw new ArgumentOutOfRangeException("offsetIn, length, offsetOut < 0");
			}
			if (offsetIn > inArray.Length - length)
			{
				throw new ArgumentOutOfRangeException("offsetIn + length > array.Length");
			}
			byte[] bytes = ToBase64Transform.InternalTransformFinalBlock(inArray, offsetIn, length);
			char[] chars = new ASCIIEncoding().GetChars(bytes);
			if (offsetOut > outArray.Length - chars.Length)
			{
				throw new ArgumentOutOfRangeException("offsetOut + cOutArr.Length > outArray.Length");
			}
			Array.Copy(chars, 0, outArray, offsetOut, chars.Length);
			return chars.Length;
		}

		public static string ToBase64String(byte[] inArray)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			return ToBase64String(inArray, 0, inArray.Length);
		}

		public static string ToBase64String(byte[] inArray, int offset, int length)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			if (offset < 0 || length < 0)
			{
				throw new ArgumentOutOfRangeException("offset < 0 || length < 0");
			}
			if (offset > inArray.Length - length)
			{
				throw new ArgumentOutOfRangeException("offset + length > array.Length");
			}
			byte[] bytes = ToBase64Transform.InternalTransformFinalBlock(inArray, offset, length);
			return new ASCIIEncoding().GetString(bytes);
		}

		[ComVisible(false)]
		public static string ToBase64String(byte[] inArray, Base64FormattingOptions options)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			return ToBase64String(inArray, 0, inArray.Length, options);
		}

		[ComVisible(false)]
		public static string ToBase64String(byte[] inArray, int offset, int length, Base64FormattingOptions options)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			if (offset < 0 || length < 0)
			{
				throw new ArgumentOutOfRangeException("offset < 0 || length < 0");
			}
			if (offset > inArray.Length - length)
			{
				throw new ArgumentOutOfRangeException("offset + length > array.Length");
			}
			if (length == 0)
			{
				return string.Empty;
			}
			if (options == Base64FormattingOptions.InsertLineBreaks)
			{
				return ToBase64StringBuilderWithLine(inArray, offset, length).ToString();
			}
			return Encoding.ASCII.GetString(ToBase64Transform.InternalTransformFinalBlock(inArray, offset, length));
		}

		[ComVisible(false)]
		public static int ToBase64CharArray(byte[] inArray, int offsetIn, int length, char[] outArray, int offsetOut, Base64FormattingOptions options)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			if (outArray == null)
			{
				throw new ArgumentNullException("outArray");
			}
			if (offsetIn < 0 || length < 0 || offsetOut < 0)
			{
				throw new ArgumentOutOfRangeException("offsetIn, length, offsetOut < 0");
			}
			if (offsetIn > inArray.Length - length)
			{
				throw new ArgumentOutOfRangeException("offsetIn + length > array.Length");
			}
			if (length == 0)
			{
				return 0;
			}
			if (options == Base64FormattingOptions.InsertLineBreaks)
			{
				StringBuilder stringBuilder = ToBase64StringBuilderWithLine(inArray, offsetIn, length);
				stringBuilder.CopyTo(0, outArray, offsetOut, stringBuilder.Length);
				return stringBuilder.Length;
			}
			byte[] bytes = ToBase64Transform.InternalTransformFinalBlock(inArray, offsetIn, length);
			char[] chars = Encoding.ASCII.GetChars(bytes);
			if (offsetOut > outArray.Length - chars.Length)
			{
				throw new ArgumentOutOfRangeException("offsetOut + cOutArr.Length > outArray.Length");
			}
			Array.Copy(chars, 0, outArray, offsetOut, chars.Length);
			return chars.Length;
		}

		private static StringBuilder ToBase64StringBuilderWithLine(byte[] inArray, int offset, int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int result;
			int num = Math.DivRem(length, 57, out result);
			for (int i = 0; i < num; i++)
			{
				byte[] bytes = ToBase64Transform.InternalTransformFinalBlock(inArray, offset, 57);
				stringBuilder.AppendLine(Encoding.ASCII.GetString(bytes));
				offset += 57;
			}
			if (result == 0)
			{
				int length2 = Environment.NewLine.Length;
				stringBuilder.Remove(stringBuilder.Length - length2, length2);
			}
			else
			{
				byte[] bytes2 = ToBase64Transform.InternalTransformFinalBlock(inArray, offset, result);
				stringBuilder.Append(Encoding.ASCII.GetString(bytes2));
			}
			return stringBuilder;
		}

		public static bool ToBoolean(bool value)
		{
			return value;
		}

		public static bool ToBoolean(byte value)
		{
			return value != 0;
		}

		public static bool ToBoolean(char value)
		{
			throw new InvalidCastException(Locale.GetText("Can't convert char to bool"));
		}

		public static bool ToBoolean(DateTime value)
		{
			throw new InvalidCastException(Locale.GetText("Can't convert date to bool"));
		}

		public static bool ToBoolean(decimal value)
		{
			return value != 0m;
		}

		public static bool ToBoolean(double value)
		{
			return value != 0.0;
		}

		public static bool ToBoolean(float value)
		{
			return value != 0f;
		}

		public static bool ToBoolean(int value)
		{
			return value != 0;
		}

		public static bool ToBoolean(long value)
		{
			return value != 0;
		}

		[CLSCompliant(false)]
		public static bool ToBoolean(sbyte value)
		{
			return value != 0;
		}

		public static bool ToBoolean(short value)
		{
			return value != 0;
		}

		public static bool ToBoolean(string value)
		{
			if (value == null)
			{
				return false;
			}
			return bool.Parse(value);
		}

		public static bool ToBoolean(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return false;
			}
			return bool.Parse(value);
		}

		[CLSCompliant(false)]
		public static bool ToBoolean(uint value)
		{
			return value != 0;
		}

		[CLSCompliant(false)]
		public static bool ToBoolean(ulong value)
		{
			return value != 0;
		}

		[CLSCompliant(false)]
		public static bool ToBoolean(ushort value)
		{
			return value != 0;
		}

		public static bool ToBoolean(object value)
		{
			if (value == null)
			{
				return false;
			}
			return ToBoolean(value, null);
		}

		public static bool ToBoolean(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return false;
			}
			return ((IConvertible)value).ToBoolean(provider);
		}

		public static byte ToByte(bool value)
		{
			return (byte)(value ? 1u : 0u);
		}

		public static byte ToByte(byte value)
		{
			return value;
		}

		public static byte ToByte(char value)
		{
			if (value > 'ÿ')
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue"));
			}
			return (byte)value;
		}

		public static byte ToByte(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static byte ToByte(decimal value)
		{
			if (value > 255m || value < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue or less than Byte.MinValue"));
			}
			return (byte)Math.Round(value);
		}

		public static byte ToByte(double value)
		{
			if (value > 255.0 || value < 0.0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue or less than Byte.MinValue"));
			}
			if (double.IsNaN(value) || double.IsInfinity(value))
			{
				throw new OverflowException(Locale.GetText("Value is equal to Double.NaN, Double.PositiveInfinity, or Double.NegativeInfinity"));
			}
			return (byte)Math.Round(value);
		}

		public static byte ToByte(float value)
		{
			if (value > 255f || value < 0f)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue or less than Byte.Minalue"));
			}
			if (float.IsNaN(value) || float.IsInfinity(value))
			{
				throw new OverflowException(Locale.GetText("Value is equal to Single.NaN, Single.PositiveInfinity, or Single.NegativeInfinity"));
			}
			return (byte)Math.Round(value);
		}

		public static byte ToByte(int value)
		{
			if (value > 255 || value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue or less than Byte.MinValue"));
			}
			return (byte)value;
		}

		public static byte ToByte(long value)
		{
			if (value > 255 || value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue or less than Byte.MinValue"));
			}
			return (byte)value;
		}

		[CLSCompliant(false)]
		public static byte ToByte(sbyte value)
		{
			if (value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is less than Byte.MinValue"));
			}
			return (byte)value;
		}

		public static byte ToByte(short value)
		{
			if (value > 255 || value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue or less than Byte.MinValue"));
			}
			return (byte)value;
		}

		public static byte ToByte(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return byte.Parse(value);
		}

		public static byte ToByte(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return byte.Parse(value, provider);
		}

		public static byte ToByte(string value, int fromBase)
		{
			int num = ConvertFromBase(value, fromBase, true);
			if (num < 0 || num > 255)
			{
				throw new OverflowException();
			}
			return (byte)num;
		}

		[CLSCompliant(false)]
		public static byte ToByte(uint value)
		{
			if (value > 255)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue"));
			}
			return (byte)value;
		}

		[CLSCompliant(false)]
		public static byte ToByte(ulong value)
		{
			if (value > 255)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue"));
			}
			return (byte)value;
		}

		[CLSCompliant(false)]
		public static byte ToByte(ushort value)
		{
			if (value > 255)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Byte.MaxValue"));
			}
			return (byte)value;
		}

		public static byte ToByte(object value)
		{
			if (value == null)
			{
				return 0;
			}
			return ToByte(value, null);
		}

		public static byte ToByte(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return ((IConvertible)value).ToByte(provider);
		}

		public static char ToChar(bool value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static char ToChar(byte value)
		{
			return (char)value;
		}

		public static char ToChar(char value)
		{
			return value;
		}

		public static char ToChar(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static char ToChar(decimal value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static char ToChar(double value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static char ToChar(int value)
		{
			if (value > 65535 || value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Char.MaxValue or less than Char.MinValue"));
			}
			return (char)value;
		}

		public static char ToChar(long value)
		{
			if (value > 65535 || value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Char.MaxValue or less than Char.MinValue"));
			}
			return (char)value;
		}

		public static char ToChar(float value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		[CLSCompliant(false)]
		public static char ToChar(sbyte value)
		{
			if (value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is less than Char.MinValue"));
			}
			return (char)value;
		}

		public static char ToChar(short value)
		{
			if (value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is less than Char.MinValue"));
			}
			return (char)value;
		}

		public static char ToChar(string value)
		{
			return char.Parse(value);
		}

		public static char ToChar(string value, IFormatProvider provider)
		{
			return char.Parse(value);
		}

		[CLSCompliant(false)]
		public static char ToChar(uint value)
		{
			if (value > 65535)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Char.MaxValue"));
			}
			return (char)value;
		}

		[CLSCompliant(false)]
		public static char ToChar(ulong value)
		{
			if (value > 65535)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Char.MaxValue"));
			}
			return (char)value;
		}

		[CLSCompliant(false)]
		public static char ToChar(ushort value)
		{
			return (char)value;
		}

		public static char ToChar(object value)
		{
			if (value == null)
			{
				return '\0';
			}
			return ToChar(value, null);
		}

		public static char ToChar(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return '\0';
			}
			return ((IConvertible)value).ToChar(provider);
		}

		public static DateTime ToDateTime(string value)
		{
			if (value == null)
			{
				return DateTime.MinValue;
			}
			return DateTime.Parse(value);
		}

		public static DateTime ToDateTime(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return DateTime.MinValue;
			}
			return DateTime.Parse(value, provider);
		}

		public static DateTime ToDateTime(bool value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(byte value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(char value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(DateTime value)
		{
			return value;
		}

		public static DateTime ToDateTime(decimal value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(double value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(short value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(int value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(long value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(float value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static DateTime ToDateTime(object value)
		{
			if (value == null)
			{
				return DateTime.MinValue;
			}
			return ToDateTime(value, null);
		}

		public static DateTime ToDateTime(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return DateTime.MinValue;
			}
			return ((IConvertible)value).ToDateTime(provider);
		}

		[CLSCompliant(false)]
		public static DateTime ToDateTime(sbyte value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		[CLSCompliant(false)]
		public static DateTime ToDateTime(ushort value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		[CLSCompliant(false)]
		public static DateTime ToDateTime(uint value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		[CLSCompliant(false)]
		public static DateTime ToDateTime(ulong value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static decimal ToDecimal(bool value)
		{
			return value ? 1 : 0;
		}

		public static decimal ToDecimal(byte value)
		{
			return value;
		}

		public static decimal ToDecimal(char value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static decimal ToDecimal(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static decimal ToDecimal(decimal value)
		{
			return value;
		}

		public static decimal ToDecimal(double value)
		{
			return (decimal)value;
		}

		public static decimal ToDecimal(float value)
		{
			return (decimal)value;
		}

		public static decimal ToDecimal(int value)
		{
			return value;
		}

		public static decimal ToDecimal(long value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static decimal ToDecimal(sbyte value)
		{
			return value;
		}

		public static decimal ToDecimal(short value)
		{
			return value;
		}

		public static decimal ToDecimal(string value)
		{
			if (value == null)
			{
				return 0m;
			}
			return decimal.Parse(value);
		}

		public static decimal ToDecimal(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0m;
			}
			return decimal.Parse(value, provider);
		}

		[CLSCompliant(false)]
		public static decimal ToDecimal(uint value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static decimal ToDecimal(ulong value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static decimal ToDecimal(ushort value)
		{
			return value;
		}

		public static decimal ToDecimal(object value)
		{
			if (value == null)
			{
				return 0m;
			}
			return ToDecimal(value, null);
		}

		public static decimal ToDecimal(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0m;
			}
			return ((IConvertible)value).ToDecimal(provider);
		}

		public static double ToDouble(bool value)
		{
			return value ? 1 : 0;
		}

		public static double ToDouble(byte value)
		{
			return (int)value;
		}

		public static double ToDouble(char value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static double ToDouble(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static double ToDouble(decimal value)
		{
			return (double)value;
		}

		public static double ToDouble(double value)
		{
			return value;
		}

		public static double ToDouble(float value)
		{
			return value;
		}

		public static double ToDouble(int value)
		{
			return value;
		}

		public static double ToDouble(long value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static double ToDouble(sbyte value)
		{
			return value;
		}

		public static double ToDouble(short value)
		{
			return value;
		}

		public static double ToDouble(string value)
		{
			if (value == null)
			{
				return 0.0;
			}
			return double.Parse(value);
		}

		public static double ToDouble(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0.0;
			}
			return double.Parse(value, provider);
		}

		[CLSCompliant(false)]
		public static double ToDouble(uint value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static double ToDouble(ulong value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static double ToDouble(ushort value)
		{
			return (int)value;
		}

		public static double ToDouble(object value)
		{
			if (value == null)
			{
				return 0.0;
			}
			return ToDouble(value, null);
		}

		public static double ToDouble(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0.0;
			}
			return ((IConvertible)value).ToDouble(provider);
		}

		public static short ToInt16(bool value)
		{
			return (short)(value ? 1 : 0);
		}

		public static short ToInt16(byte value)
		{
			return value;
		}

		public static short ToInt16(char value)
		{
			if (value > '翿')
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue"));
			}
			return (short)value;
		}

		public static short ToInt16(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static short ToInt16(decimal value)
		{
			if (value > 32767m || value < -32768m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue or less than Int16.MinValue"));
			}
			return (short)Math.Round(value);
		}

		public static short ToInt16(double value)
		{
			if (value > 32767.0 || value < -32768.0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue or less than Int16.MinValue"));
			}
			return (short)Math.Round(value);
		}

		public static short ToInt16(float value)
		{
			if (value > 32767f || value < -32768f)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue or less than Int16.MinValue"));
			}
			return (short)Math.Round(value);
		}

		public static short ToInt16(int value)
		{
			if (value > 32767 || value < -32768)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue or less than Int16.MinValue"));
			}
			return (short)value;
		}

		public static short ToInt16(long value)
		{
			if (value > 32767 || value < -32768)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue or less than Int16.MinValue"));
			}
			return (short)value;
		}

		[CLSCompliant(false)]
		public static short ToInt16(sbyte value)
		{
			return value;
		}

		public static short ToInt16(short value)
		{
			return value;
		}

		public static short ToInt16(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return short.Parse(value);
		}

		public static short ToInt16(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return short.Parse(value, provider);
		}

		public static short ToInt16(string value, int fromBase)
		{
			int num = ConvertFromBase(value, fromBase, false);
			if (fromBase != 10)
			{
				if (num > 65535)
				{
					throw new OverflowException("Value was either too large or too small for an Int16.");
				}
				if (num > 32767)
				{
					return ToInt16(-(65536 - num));
				}
			}
			return ToInt16(num);
		}

		[CLSCompliant(false)]
		public static short ToInt16(uint value)
		{
			if ((long)value > 32767L)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue"));
			}
			return (short)value;
		}

		[CLSCompliant(false)]
		public static short ToInt16(ulong value)
		{
			if (value > 32767)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue"));
			}
			return (short)value;
		}

		[CLSCompliant(false)]
		public static short ToInt16(ushort value)
		{
			if (value > 32767)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int16.MaxValue"));
			}
			return (short)value;
		}

		public static short ToInt16(object value)
		{
			if (value == null)
			{
				return 0;
			}
			return ToInt16(value, null);
		}

		public static short ToInt16(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return ((IConvertible)value).ToInt16(provider);
		}

		public static int ToInt32(bool value)
		{
			return value ? 1 : 0;
		}

		public static int ToInt32(byte value)
		{
			return value;
		}

		public static int ToInt32(char value)
		{
			return value;
		}

		public static int ToInt32(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static int ToInt32(decimal value)
		{
			if (value > 2147483647m || value < -2147483648m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int32.MaxValue or less than Int32.MinValue"));
			}
			return (int)Math.Round(value);
		}

		public static int ToInt32(double value)
		{
			if (value > 2147483647.0 || value < -2147483648.0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int32.MaxValue or less than Int32.MinValue"));
			}
			return checked((int)Math.Round(value));
		}

		public static int ToInt32(float value)
		{
			if (value > 2.1474836E+09f || value < -2.1474836E+09f)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int32.MaxValue or less than Int32.MinValue"));
			}
			return checked((int)Math.Round(value));
		}

		public static int ToInt32(int value)
		{
			return value;
		}

		public static int ToInt32(long value)
		{
			if (value > int.MaxValue || value < int.MinValue)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int32.MaxValue or less than Int32.MinValue"));
			}
			return (int)value;
		}

		[CLSCompliant(false)]
		public static int ToInt32(sbyte value)
		{
			return value;
		}

		public static int ToInt32(short value)
		{
			return value;
		}

		public static int ToInt32(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return int.Parse(value);
		}

		public static int ToInt32(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return int.Parse(value, provider);
		}

		public static int ToInt32(string value, int fromBase)
		{
			return ConvertFromBase(value, fromBase, false);
		}

		[CLSCompliant(false)]
		public static int ToInt32(uint value)
		{
			if (value > int.MaxValue)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int32.MaxValue"));
			}
			return (int)value;
		}

		[CLSCompliant(false)]
		public static int ToInt32(ulong value)
		{
			if (value > int.MaxValue)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int32.MaxValue"));
			}
			return (int)value;
		}

		[CLSCompliant(false)]
		public static int ToInt32(ushort value)
		{
			return value;
		}

		public static int ToInt32(object value)
		{
			if (value == null)
			{
				return 0;
			}
			return ToInt32(value, null);
		}

		public static int ToInt32(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return ((IConvertible)value).ToInt32(provider);
		}

		public static long ToInt64(bool value)
		{
			return value ? 1 : 0;
		}

		public static long ToInt64(byte value)
		{
			return value;
		}

		public static long ToInt64(char value)
		{
			return (int)value;
		}

		public static long ToInt64(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static long ToInt64(decimal value)
		{
			if (value > 9223372036854775807m || value < -9223372036854775808m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int64.MaxValue or less than Int64.MinValue"));
			}
			return (long)Math.Round(value);
		}

		public static long ToInt64(double value)
		{
			if (value > 9.223372036854776E+18 || value < -9.223372036854776E+18)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int64.MaxValue or less than Int64.MinValue"));
			}
			return (long)Math.Round(value);
		}

		public static long ToInt64(float value)
		{
			if (value > 9.223372E+18f || value < -9.223372E+18f)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int64.MaxValue or less than Int64.MinValue"));
			}
			return (long)Math.Round(value);
		}

		public static long ToInt64(int value)
		{
			return value;
		}

		public static long ToInt64(long value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static long ToInt64(sbyte value)
		{
			return value;
		}

		public static long ToInt64(short value)
		{
			return value;
		}

		public static long ToInt64(string value)
		{
			if (value == null)
			{
				return 0L;
			}
			return long.Parse(value);
		}

		public static long ToInt64(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0L;
			}
			return long.Parse(value, provider);
		}

		public static long ToInt64(string value, int fromBase)
		{
			return ConvertFromBase64(value, fromBase, false);
		}

		[CLSCompliant(false)]
		public static long ToInt64(uint value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static long ToInt64(ulong value)
		{
			if (value > long.MaxValue)
			{
				throw new OverflowException(Locale.GetText("Value is greater than Int64.MaxValue"));
			}
			return (long)value;
		}

		[CLSCompliant(false)]
		public static long ToInt64(ushort value)
		{
			return value;
		}

		public static long ToInt64(object value)
		{
			if (value == null)
			{
				return 0L;
			}
			return ToInt64(value, null);
		}

		public static long ToInt64(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0L;
			}
			return ((IConvertible)value).ToInt64(provider);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(bool value)
		{
			return (sbyte)(value ? 1 : 0);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(byte value)
		{
			if (value > 127)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue"));
			}
			return (sbyte)value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(char value)
		{
			if (value > '\u007f')
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue"));
			}
			return (sbyte)value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(decimal value)
		{
			if (value > 127m || value < -128m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue or less than SByte.MinValue"));
			}
			return (sbyte)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(double value)
		{
			if (value > 127.0 || value < -128.0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue or less than SByte.MinValue"));
			}
			return (sbyte)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(float value)
		{
			if (value > 127f || value < -128f)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue or less than SByte.Minalue"));
			}
			return (sbyte)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(int value)
		{
			if (value > 127 || value < -128)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue or less than SByte.MinValue"));
			}
			return (sbyte)value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(long value)
		{
			if (value > 127 || value < -128)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue or less than SByte.MinValue"));
			}
			return (sbyte)value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(sbyte value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(short value)
		{
			if (value > 127 || value < -128)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue or less than SByte.MinValue"));
			}
			return (sbyte)value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return sbyte.Parse(value);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return sbyte.Parse(value, provider);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(string value, int fromBase)
		{
			int num = ConvertFromBase(value, fromBase, false);
			if (fromBase != 10 && num > 127)
			{
				return ToSByte(-(256 - num));
			}
			return ToSByte(num);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(uint value)
		{
			if ((long)value > 127L)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue"));
			}
			return (sbyte)value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(ulong value)
		{
			if (value > 127)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue"));
			}
			return (sbyte)value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(ushort value)
		{
			if (value > 127)
			{
				throw new OverflowException(Locale.GetText("Value is greater than SByte.MaxValue"));
			}
			return (sbyte)value;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(object value)
		{
			if (value == null)
			{
				return 0;
			}
			return ToSByte(value, null);
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return ((IConvertible)value).ToSByte(provider);
		}

		public static float ToSingle(bool value)
		{
			return value ? 1 : 0;
		}

		public static float ToSingle(byte value)
		{
			return (int)value;
		}

		public static float ToSingle(char value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static float ToSingle(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		public static float ToSingle(decimal value)
		{
			return (float)value;
		}

		public static float ToSingle(double value)
		{
			return (float)value;
		}

		public static float ToSingle(float value)
		{
			return value;
		}

		public static float ToSingle(int value)
		{
			return value;
		}

		public static float ToSingle(long value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static float ToSingle(sbyte value)
		{
			return value;
		}

		public static float ToSingle(short value)
		{
			return value;
		}

		public static float ToSingle(string value)
		{
			if (value == null)
			{
				return 0f;
			}
			return float.Parse(value);
		}

		public static float ToSingle(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0f;
			}
			return float.Parse(value, provider);
		}

		[CLSCompliant(false)]
		public static float ToSingle(uint value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static float ToSingle(ulong value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static float ToSingle(ushort value)
		{
			return (int)value;
		}

		public static float ToSingle(object value)
		{
			if (value == null)
			{
				return 0f;
			}
			return ToSingle(value, null);
		}

		public static float ToSingle(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0f;
			}
			return ((IConvertible)value).ToSingle(provider);
		}

		public static string ToString(bool value)
		{
			return value.ToString();
		}

		public static string ToString(bool value, IFormatProvider provider)
		{
			return value.ToString();
		}

		public static string ToString(byte value)
		{
			return value.ToString();
		}

		public static string ToString(byte value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(byte value, int toBase)
		{
			if (value == 0)
			{
				return "0";
			}
			if (toBase == 10)
			{
				return value.ToString();
			}
			byte[] bytes = BitConverter.GetBytes(value);
			switch (toBase)
			{
			case 2:
				return ConvertToBase2(bytes);
			case 8:
				return ConvertToBase8(bytes);
			case 16:
				return ConvertToBase16(bytes);
			default:
				throw new ArgumentException(Locale.GetText("toBase is not valid."));
			}
		}

		public static string ToString(char value)
		{
			return value.ToString();
		}

		public static string ToString(char value, IFormatProvider provider)
		{
			return value.ToString();
		}

		public static string ToString(DateTime value)
		{
			return value.ToString();
		}

		public static string ToString(DateTime value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(decimal value)
		{
			return value.ToString();
		}

		public static string ToString(decimal value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(double value)
		{
			return value.ToString();
		}

		public static string ToString(double value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(float value)
		{
			return value.ToString();
		}

		public static string ToString(float value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(int value)
		{
			return value.ToString();
		}

		public static string ToString(int value, int toBase)
		{
			if (value == 0)
			{
				return "0";
			}
			if (toBase == 10)
			{
				return value.ToString();
			}
			byte[] bytes = BitConverter.GetBytes(value);
			switch (toBase)
			{
			case 2:
				return ConvertToBase2(bytes);
			case 8:
				return ConvertToBase8(bytes);
			case 16:
				return ConvertToBase16(bytes);
			default:
				throw new ArgumentException(Locale.GetText("toBase is not valid."));
			}
		}

		public static string ToString(int value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(long value)
		{
			return value.ToString();
		}

		public static string ToString(long value, int toBase)
		{
			if (value == 0L)
			{
				return "0";
			}
			if (toBase == 10)
			{
				return value.ToString();
			}
			byte[] bytes = BitConverter.GetBytes(value);
			switch (toBase)
			{
			case 2:
				return ConvertToBase2(bytes);
			case 8:
				return ConvertToBase8(bytes);
			case 16:
				return ConvertToBase16(bytes);
			default:
				throw new ArgumentException(Locale.GetText("toBase is not valid."));
			}
		}

		public static string ToString(long value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(object value)
		{
			return ToString(value, null);
		}

		public static string ToString(object value, IFormatProvider provider)
		{
			if (value is IConvertible)
			{
				return ((IConvertible)value).ToString(provider);
			}
			if (value != null)
			{
				return value.ToString();
			}
			return string.Empty;
		}

		[CLSCompliant(false)]
		public static string ToString(sbyte value)
		{
			return value.ToString();
		}

		[CLSCompliant(false)]
		public static string ToString(sbyte value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(short value)
		{
			return value.ToString();
		}

		public static string ToString(short value, int toBase)
		{
			if (value == 0)
			{
				return "0";
			}
			if (toBase == 10)
			{
				return value.ToString();
			}
			byte[] bytes = BitConverter.GetBytes(value);
			switch (toBase)
			{
			case 2:
				return ConvertToBase2(bytes);
			case 8:
				return ConvertToBase8(bytes);
			case 16:
				return ConvertToBase16(bytes);
			default:
				throw new ArgumentException(Locale.GetText("toBase is not valid."));
			}
		}

		public static string ToString(short value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		public static string ToString(string value)
		{
			return value;
		}

		public static string ToString(string value, IFormatProvider provider)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static string ToString(uint value)
		{
			return value.ToString();
		}

		[CLSCompliant(false)]
		public static string ToString(uint value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		[CLSCompliant(false)]
		public static string ToString(ulong value)
		{
			return value.ToString();
		}

		[CLSCompliant(false)]
		public static string ToString(ulong value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		[CLSCompliant(false)]
		public static string ToString(ushort value)
		{
			return value.ToString();
		}

		[CLSCompliant(false)]
		public static string ToString(ushort value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(bool value)
		{
			return (ushort)(value ? 1u : 0u);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(byte value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(char value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(decimal value)
		{
			if (value > 65535m || value < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue or less than UInt16.MinValue"));
			}
			return (ushort)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(double value)
		{
			if (value > 65535.0 || value < 0.0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue or less than UInt16.MinValue"));
			}
			return (ushort)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(float value)
		{
			if (value > 65535f || value < 0f)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue or less than UInt16.MinValue"));
			}
			return (ushort)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(int value)
		{
			if (value > 65535 || value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue or less than UInt16.MinValue"));
			}
			return (ushort)value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(long value)
		{
			if (value > 65535 || value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue or less than UInt16.MinValue"));
			}
			return (ushort)value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(sbyte value)
		{
			if (value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is less than UInt16.MinValue"));
			}
			return (ushort)value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(short value)
		{
			if (value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is less than UInt16.MinValue"));
			}
			return (ushort)value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return ushort.Parse(value);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return ushort.Parse(value, provider);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(string value, int fromBase)
		{
			return ToUInt16(ConvertFromBase(value, fromBase, true));
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(uint value)
		{
			if (value > 65535)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue"));
			}
			return (ushort)value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(ulong value)
		{
			if (value > 65535)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt16.MaxValue"));
			}
			return (ushort)value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(ushort value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(object value)
		{
			if (value == null)
			{
				return 0;
			}
			return ToUInt16(value, null);
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return ((IConvertible)value).ToUInt16(provider);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(bool value)
		{
			return value ? 1u : 0u;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(byte value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(char value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(DateTime value)
		{
			throw new InvalidCastException("This conversion is not supported.");
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(decimal value)
		{
			if (value > 4294967295m || value < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt32.MaxValue or less than UInt32.MinValue"));
			}
			return (uint)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(double value)
		{
			if (value > 4294967295.0 || value < 0.0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt32.MaxValue or less than UInt32.MinValue"));
			}
			return (uint)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(float value)
		{
			if (value > 4.2949673E+09f || value < 0f)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt32.MaxValue or less than UInt32.MinValue"));
			}
			return (uint)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(int value)
		{
			if ((long)value < 0L)
			{
				throw new OverflowException(Locale.GetText("Value is less than UInt32.MinValue"));
			}
			return (uint)value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(long value)
		{
			if (value > uint.MaxValue || value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt32.MaxValue or less than UInt32.MinValue"));
			}
			return (uint)value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(sbyte value)
		{
			if ((long)value < 0L)
			{
				throw new OverflowException(Locale.GetText("Value is less than UInt32.MinValue"));
			}
			return (uint)value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(short value)
		{
			if ((long)value < 0L)
			{
				throw new OverflowException(Locale.GetText("Value is less than UInt32.MinValue"));
			}
			return (uint)value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(string value)
		{
			if (value == null)
			{
				return 0u;
			}
			return uint.Parse(value);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0u;
			}
			return uint.Parse(value, provider);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(string value, int fromBase)
		{
			return (uint)ConvertFromBase(value, fromBase, true);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(uint value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(ulong value)
		{
			if (value > uint.MaxValue)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt32.MaxValue"));
			}
			return (uint)value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(ushort value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(object value)
		{
			if (value == null)
			{
				return 0u;
			}
			return ToUInt32(value, null);
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0u;
			}
			return ((IConvertible)value).ToUInt32(provider);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(bool value)
		{
			return (ulong)(int)(value ? 1u : 0u);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(byte value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(char value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(DateTime value)
		{
			throw new InvalidCastException("The conversion is not supported.");
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(decimal value)
		{
			if (value > 18446744073709551615m || value < 0m)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt64.MaxValue or less than UInt64.MinValue"));
			}
			return (ulong)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(double value)
		{
			if (value > 1.8446744073709552E+19 || value < 0.0)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt64.MaxValue or less than UInt64.MinValue"));
			}
			return (ulong)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(float value)
		{
			if (value > 1.8446744E+19f || value < 0f)
			{
				throw new OverflowException(Locale.GetText("Value is greater than UInt64.MaxValue or less than UInt64.MinValue"));
			}
			return (ulong)Math.Round(value);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(int value)
		{
			if (value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is less than UInt64.MinValue"));
			}
			return (ulong)value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(long value)
		{
			if (value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is less than UInt64.MinValue"));
			}
			return (ulong)value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(sbyte value)
		{
			if (value < 0)
			{
				throw new OverflowException("Value is less than UInt64.MinValue");
			}
			return (ulong)value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(short value)
		{
			if (value < 0)
			{
				throw new OverflowException(Locale.GetText("Value is less than UInt64.MinValue"));
			}
			return (ulong)value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(string value)
		{
			if (value == null)
			{
				return 0uL;
			}
			return ulong.Parse(value);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0uL;
			}
			return ulong.Parse(value, provider);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(string value, int fromBase)
		{
			return (ulong)ConvertFromBase64(value, fromBase, true);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(uint value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(ulong value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(ushort value)
		{
			return value;
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(object value)
		{
			if (value == null)
			{
				return 0uL;
			}
			return ToUInt64(value, null);
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(object value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0uL;
			}
			return ((IConvertible)value).ToUInt64(provider);
		}

		public static object ChangeType(object value, Type conversionType)
		{
			if (value != null && conversionType == null)
			{
				throw new ArgumentNullException("conversionType");
			}
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			IFormatProvider provider = ((conversionType != typeof(DateTime)) ? ((IFormatProvider)currentCulture.NumberFormat) : ((IFormatProvider)currentCulture.DateTimeFormat));
			return ToType(value, conversionType, provider, true);
		}

		public static object ChangeType(object value, TypeCode typeCode)
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			Type type = conversionTable[(int)typeCode];
			IFormatProvider provider = ((type != typeof(DateTime)) ? ((IFormatProvider)currentCulture.NumberFormat) : ((IFormatProvider)currentCulture.DateTimeFormat));
			return ToType(value, type, provider, true);
		}

		public static object ChangeType(object value, Type conversionType, IFormatProvider provider)
		{
			if (value != null && conversionType == null)
			{
				throw new ArgumentNullException("conversionType");
			}
			return ToType(value, conversionType, provider, true);
		}

		public static object ChangeType(object value, TypeCode typeCode, IFormatProvider provider)
		{
			Type conversionType = conversionTable[(int)typeCode];
			return ToType(value, conversionType, provider, true);
		}

		private static bool NotValidBase(int value)
		{
			if (value == 2 || value == 8 || value == 10 || value == 16)
			{
				return false;
			}
			return true;
		}

		private static int ConvertFromBase(string value, int fromBase, bool unsigned)
		{
			if (NotValidBase(fromBase))
			{
				throw new ArgumentException("fromBase is not valid.");
			}
			if (value == null)
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int length = value.Length;
			bool flag = false;
			switch (fromBase)
			{
			case 10:
				if (value.Substring(num3, 1) == "-")
				{
					if (unsigned)
					{
						throw new OverflowException(Locale.GetText("The string was being parsed as an unsigned number and could not have a negative sign."));
					}
					flag = true;
					num3++;
				}
				break;
			case 16:
				if (value.Substring(num3, 1) == "-")
				{
					throw new ArgumentException("String cannot contain a minus sign if the base is not 10.");
				}
				if (length >= num3 + 2 && value[num3] == '0' && (value[num3 + 1] == 'x' || value[num3 + 1] == 'X'))
				{
					num3 += 2;
				}
				break;
			default:
				if (value.Substring(num3, 1) == "-")
				{
					throw new ArgumentException("String cannot contain a minus sign if the base is not 10.");
				}
				break;
			}
			if (length == num3)
			{
				throw new FormatException("Could not find any parsable digits.");
			}
			if (value[num3] == '+')
			{
				num3++;
			}
			while (num3 < length)
			{
				char c = value[num3++];
				int num4;
				if (char.IsNumber(c))
				{
					num4 = c - 48;
				}
				else
				{
					if (!char.IsLetter(c))
					{
						if (num > 0)
						{
							throw new FormatException("Additional unparsable characters are at the end of the string.");
						}
						throw new FormatException("Could not find any parsable digits.");
					}
					num4 = char.ToLowerInvariant(c) - 97 + 10;
				}
				if (num4 >= fromBase)
				{
					if (num > 0)
					{
						throw new FormatException("Additional unparsable characters are at the end of the string.");
					}
					throw new FormatException("Could not find any parsable digits.");
				}
				num2 = fromBase * num2 + num4;
				num++;
			}
			if (num == 0)
			{
				throw new FormatException("Could not find any parsable digits.");
			}
			if (flag)
			{
				return -num2;
			}
			return num2;
		}

		private static long ConvertFromBase64(string value, int fromBase, bool unsigned)
		{
			if (NotValidBase(fromBase))
			{
				throw new ArgumentException("fromBase is not valid.");
			}
			if (value == null)
			{
				return 0L;
			}
			int num = 0;
			int num2 = -1;
			long num3 = 0L;
			bool flag = false;
			int num4 = 0;
			int length = value.Length;
			switch (fromBase)
			{
			case 10:
				if (value.Substring(num4, 1) == "-")
				{
					if (unsigned)
					{
						throw new OverflowException(Locale.GetText("The string was being parsed as an unsigned number and could not have a negative sign."));
					}
					flag = true;
					num4++;
				}
				break;
			case 16:
				if (value.Substring(num4, 1) == "-")
				{
					throw new ArgumentException("String cannot contain a minus sign if the base is not 10.");
				}
				if (length >= num4 + 2 && value[num4] == '0' && (value[num4 + 1] == 'x' || value[num4 + 1] == 'X'))
				{
					num4 += 2;
				}
				break;
			default:
				if (value.Substring(num4, 1) == "-")
				{
					throw new ArgumentException("String cannot contain a minus sign if the base is not 10.");
				}
				break;
			}
			if (length == num4)
			{
				throw new FormatException("Could not find any parsable digits.");
			}
			if (value[num4] == '+')
			{
				num4++;
			}
			while (num4 < length)
			{
				char c = value[num4++];
				if (char.IsNumber(c))
				{
					num2 = c - 48;
				}
				else
				{
					if (!char.IsLetter(c))
					{
						if (num > 0)
						{
							throw new FormatException("Additional unparsable characters are at the end of the string.");
						}
						throw new FormatException("Could not find any parsable digits.");
					}
					num2 = char.ToLowerInvariant(c) - 97 + 10;
				}
				if (num2 >= fromBase)
				{
					if (num > 0)
					{
						throw new FormatException("Additional unparsable characters are at the end of the string.");
					}
					throw new FormatException("Could not find any parsable digits.");
				}
				num3 = fromBase * num3 + num2;
				num++;
			}
			if (num == 0)
			{
				throw new FormatException("Could not find any parsable digits.");
			}
			if (flag)
			{
				return -1 * num3;
			}
			return num3;
		}

		private static void EndianSwap(ref byte[] value)
		{
			byte[] array = new byte[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = value[value.Length - 1 - i];
			}
			value = array;
		}

		private static string ConvertToBase2(byte[] value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				EndianSwap(ref value);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int num = value.Length - 1; num >= 0; num--)
			{
				byte b = value[num];
				for (int i = 0; i < 8; i++)
				{
					if ((b & 0x80) == 128)
					{
						stringBuilder.Append('1');
					}
					else if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('0');
					}
					b <<= 1;
				}
			}
			return stringBuilder.ToString();
		}

		private static string ConvertToBase8(byte[] value)
		{
			ulong num = 0uL;
			switch (value.Length)
			{
			case 1:
				num = value[0];
				break;
			case 2:
				num = BitConverter.ToUInt16(value, 0);
				break;
			case 4:
				num = BitConverter.ToUInt32(value, 0);
				break;
			case 8:
				num = BitConverter.ToUInt64(value, 0);
				break;
			default:
				throw new ArgumentException("value");
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int num2 = 21; num2 >= 0; num2--)
			{
				char c = (char)((num >> num2 * 3) & 7);
				if (c != 0 || stringBuilder.Length > 0)
				{
					c = (char)(c + 48);
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private static string ConvertToBase16(byte[] value)
		{
			if (!BitConverter.IsLittleEndian)
			{
				EndianSwap(ref value);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int num = value.Length - 1; num >= 0; num--)
			{
				char c = (char)((value[num] >> 4) & 0xF);
				if (c != 0 || stringBuilder.Length > 0)
				{
					if (c < '\n')
					{
						c = (char)(c + 48);
					}
					else
					{
						c = (char)(c - 10);
						c = (char)(c + 97);
					}
					stringBuilder.Append(c);
				}
				char c2 = (char)(value[num] & 0xF);
				if (c2 != 0 || stringBuilder.Length > 0)
				{
					if (c2 < '\n')
					{
						c2 = (char)(c2 + 48);
					}
					else
					{
						c2 = (char)(c2 - 10);
						c2 = (char)(c2 + 97);
					}
					stringBuilder.Append(c2);
				}
			}
			return stringBuilder.ToString();
		}

		internal static object ToType(object value, Type conversionType, IFormatProvider provider, bool try_target_to_type)
		{
			if (value == null)
			{
				if (conversionType != null && conversionType.IsValueType)
				{
					throw new InvalidCastException("Null object can not be converted to a value type.");
				}
				return null;
			}
			if (conversionType == null)
			{
				throw new InvalidCastException("Cannot cast to destination type.");
			}
			if (value.GetType() == conversionType)
			{
				return value;
			}
			if (value is IConvertible)
			{
				IConvertible convertible = (IConvertible)value;
				if (conversionType == conversionTable[0])
				{
					throw new ArgumentNullException();
				}
				if (conversionType == conversionTable[1])
				{
					return value;
				}
				if (conversionType == conversionTable[2])
				{
					throw new InvalidCastException("Cannot cast to DBNull, it's not IConvertible");
				}
				if (conversionType == conversionTable[3])
				{
					return convertible.ToBoolean(provider);
				}
				if (conversionType == conversionTable[4])
				{
					return convertible.ToChar(provider);
				}
				if (conversionType == conversionTable[5])
				{
					return convertible.ToSByte(provider);
				}
				if (conversionType == conversionTable[6])
				{
					return convertible.ToByte(provider);
				}
				if (conversionType == conversionTable[7])
				{
					return convertible.ToInt16(provider);
				}
				if (conversionType == conversionTable[8])
				{
					return convertible.ToUInt16(provider);
				}
				if (conversionType == conversionTable[9])
				{
					return convertible.ToInt32(provider);
				}
				if (conversionType == conversionTable[10])
				{
					return convertible.ToUInt32(provider);
				}
				if (conversionType == conversionTable[11])
				{
					return convertible.ToInt64(provider);
				}
				if (conversionType == conversionTable[12])
				{
					return convertible.ToUInt64(provider);
				}
				if (conversionType == conversionTable[13])
				{
					return convertible.ToSingle(provider);
				}
				if (conversionType == conversionTable[14])
				{
					return convertible.ToDouble(provider);
				}
				if (conversionType == conversionTable[15])
				{
					return convertible.ToDecimal(provider);
				}
				if (conversionType == conversionTable[16])
				{
					return convertible.ToDateTime(provider);
				}
				if (conversionType == conversionTable[18])
				{
					return convertible.ToString(provider);
				}
				if (try_target_to_type)
				{
					return convertible.ToType(conversionType, provider);
				}
			}
			throw new InvalidCastException(Locale.GetText("Value is not a convertible object: " + value.GetType().ToString() + " to " + conversionType.FullName));
		}
	}
}
