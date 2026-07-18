using System.Text;

namespace System
{
	public static class BitConverter
	{
		private static readonly bool SwappedWordsInDouble = DoubleWordsAreSwapped();

		public static readonly bool IsLittleEndian = AmILittleEndian();

		private unsafe static bool AmILittleEndian()
		{
			double num = 1.0;
			byte* ptr = (byte*)(&num);
			return *ptr == 0;
		}

		private unsafe static bool DoubleWordsAreSwapped()
		{
			double num = 1.0;
			byte* ptr = (byte*)(&num);
			return ptr[2] == 240;
		}

		public static long DoubleToInt64Bits(double value)
		{
			return ToInt64(GetBytes(value), 0);
		}

		public static double Int64BitsToDouble(long value)
		{
			return ToDouble(GetBytes(value), 0);
		}

		internal static double InternalInt64BitsToDouble(long value)
		{
			return SwappableToDouble(GetBytes(value), 0);
		}

		private unsafe static byte[] GetBytes(byte* ptr, int count)
		{
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = ptr[i];
			}
			return array;
		}

		public unsafe static byte[] GetBytes(bool value)
		{
			return GetBytes((byte*)(&value), 1);
		}

		public unsafe static byte[] GetBytes(char value)
		{
			return GetBytes((byte*)(&value), 2);
		}

		public unsafe static byte[] GetBytes(short value)
		{
			return GetBytes((byte*)(&value), 2);
		}

		public unsafe static byte[] GetBytes(int value)
		{
			return GetBytes((byte*)(&value), 4);
		}

		public unsafe static byte[] GetBytes(long value)
		{
			return GetBytes((byte*)(&value), 8);
		}

		[CLSCompliant(false)]
		public unsafe static byte[] GetBytes(ushort value)
		{
			return GetBytes((byte*)(&value), 2);
		}

		[CLSCompliant(false)]
		public unsafe static byte[] GetBytes(uint value)
		{
			return GetBytes((byte*)(&value), 4);
		}

		[CLSCompliant(false)]
		public unsafe static byte[] GetBytes(ulong value)
		{
			return GetBytes((byte*)(&value), 8);
		}

		public unsafe static byte[] GetBytes(float value)
		{
			return GetBytes((byte*)(&value), 4);
		}

		public unsafe static byte[] GetBytes(double value)
		{
			if (SwappedWordsInDouble)
			{
				byte[] array = new byte[8];
				byte* ptr = (byte*)(&value);
				array[0] = ptr[4];
				array[1] = ptr[5];
				array[2] = ptr[6];
				array[3] = ptr[7];
				array[4] = *ptr;
				array[5] = ptr[1];
				array[6] = ptr[2];
				array[7] = ptr[3];
				return array;
			}
			return GetBytes((byte*)(&value), 8);
		}

		private unsafe static void PutBytes(byte* dst, byte[] src, int start_index, int count)
		{
			if (src == null)
			{
				throw new ArgumentNullException("value");
			}
			if (start_index < 0 || start_index > src.Length - 1)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (src.Length - count < start_index)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			for (int i = 0; i < count; i++)
			{
				dst[i] = src[i + start_index];
			}
		}

		public static bool ToBoolean(byte[] value, int startIndex)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > value.Length - 1)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (value[startIndex] != 0)
			{
				return true;
			}
			return false;
		}

		public unsafe static char ToChar(byte[] value, int startIndex)
		{
			char result = default(char);
			PutBytes((byte*)(&result), value, startIndex, 2);
			return result;
		}

		public unsafe static short ToInt16(byte[] value, int startIndex)
		{
			short result = default(short);
			PutBytes((byte*)(&result), value, startIndex, 2);
			return result;
		}

		public unsafe static int ToInt32(byte[] value, int startIndex)
		{
			int result = default(int);
			PutBytes((byte*)(&result), value, startIndex, 4);
			return result;
		}

		public unsafe static long ToInt64(byte[] value, int startIndex)
		{
			long result = default(long);
			PutBytes((byte*)(&result), value, startIndex, 8);
			return result;
		}

		[CLSCompliant(false)]
		public unsafe static ushort ToUInt16(byte[] value, int startIndex)
		{
			ushort result = default(ushort);
			PutBytes((byte*)(&result), value, startIndex, 2);
			return result;
		}

		[CLSCompliant(false)]
		public unsafe static uint ToUInt32(byte[] value, int startIndex)
		{
			uint result = default(uint);
			PutBytes((byte*)(&result), value, startIndex, 4);
			return result;
		}

		[CLSCompliant(false)]
		public unsafe static ulong ToUInt64(byte[] value, int startIndex)
		{
			ulong result = default(ulong);
			PutBytes((byte*)(&result), value, startIndex, 8);
			return result;
		}

		public unsafe static float ToSingle(byte[] value, int startIndex)
		{
			float result = default(float);
			PutBytes((byte*)(&result), value, startIndex, 4);
			return result;
		}

		public unsafe static double ToDouble(byte[] value, int startIndex)
		{
			double result = default(double);
			if (SwappedWordsInDouble)
			{
				byte* ptr = (byte*)(&result);
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (startIndex < 0 || startIndex > value.Length - 1)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (value.Length - 8 < startIndex)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				*ptr = value[startIndex + 4];
				ptr[1] = value[startIndex + 5];
				ptr[2] = value[startIndex + 6];
				ptr[3] = value[startIndex + 7];
				ptr[4] = value[startIndex];
				ptr[5] = value[startIndex + 1];
				ptr[6] = value[startIndex + 2];
				ptr[7] = value[startIndex + 3];
				return result;
			}
			PutBytes((byte*)(&result), value, startIndex, 8);
			return result;
		}

		internal unsafe static double SwappableToDouble(byte[] value, int startIndex)
		{
			double result = default(double);
			if (SwappedWordsInDouble)
			{
				byte* ptr = (byte*)(&result);
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (startIndex < 0 || startIndex > value.Length - 1)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (value.Length - 8 < startIndex)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				*ptr = value[startIndex + 4];
				ptr[1] = value[startIndex + 5];
				ptr[2] = value[startIndex + 6];
				ptr[3] = value[startIndex + 7];
				ptr[4] = value[startIndex];
				ptr[5] = value[startIndex + 1];
				ptr[6] = value[startIndex + 2];
				ptr[7] = value[startIndex + 3];
				return result;
			}
			if (!IsLittleEndian)
			{
				byte* ptr2 = (byte*)(&result);
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (startIndex < 0 || startIndex > value.Length - 1)
				{
					throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				if (value.Length - 8 < startIndex)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				*ptr2 = value[startIndex + 7];
				ptr2[1] = value[startIndex + 6];
				ptr2[2] = value[startIndex + 5];
				ptr2[3] = value[startIndex + 4];
				ptr2[4] = value[startIndex + 3];
				ptr2[5] = value[startIndex + 2];
				ptr2[6] = value[startIndex + 1];
				ptr2[7] = value[startIndex];
				return result;
			}
			PutBytes((byte*)(&result), value, startIndex, 8);
			return result;
		}

		public static string ToString(byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return ToString(value, 0, value.Length);
		}

		public static string ToString(byte[] value, int startIndex)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return ToString(value, startIndex, value.Length - startIndex);
		}

		public static string ToString(byte[] value, int startIndex, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("byteArray");
			}
			if (startIndex < 0 || startIndex >= value.Length)
			{
				if (startIndex == 0 && value.Length == 0)
				{
					return string.Empty;
				}
				throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Value must be positive.");
			}
			if (startIndex > value.Length - length)
			{
				throw new ArgumentException("startIndex + length > value.Length");
			}
			if (length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(length * 3 - 1);
			int num = startIndex + length;
			for (int i = startIndex; i < num; i++)
			{
				if (i > startIndex)
				{
					stringBuilder.Append('-');
				}
				char c = (char)((value[i] >> 4) & 0xF);
				char c2 = (char)(value[i] & 0xF);
				if (c < '\n')
				{
					c = (char)(c + 48);
				}
				else
				{
					c = (char)(c - 10);
					c = (char)(c + 65);
				}
				if (c2 < '\n')
				{
					c2 = (char)(c2 + 48);
				}
				else
				{
					c2 = (char)(c2 - 10);
					c2 = (char)(c2 + 65);
				}
				stringBuilder.Append(c);
				stringBuilder.Append(c2);
			}
			return stringBuilder.ToString();
		}
	}
}
