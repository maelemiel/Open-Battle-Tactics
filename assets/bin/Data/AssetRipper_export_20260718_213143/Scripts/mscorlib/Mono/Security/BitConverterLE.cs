using System;

namespace Mono.Security
{
	internal sealed class BitConverterLE
	{
		private BitConverterLE()
		{
		}

		private unsafe static byte[] GetUShortBytes(byte* bytes)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return new byte[2]
				{
					bytes[1],
					*bytes
				};
			}
			return new byte[2]
			{
				*bytes,
				bytes[1]
			};
		}

		private unsafe static byte[] GetUIntBytes(byte* bytes)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return new byte[4]
				{
					bytes[3],
					bytes[2],
					bytes[1],
					*bytes
				};
			}
			return new byte[4]
			{
				*bytes,
				bytes[1],
				bytes[2],
				bytes[3]
			};
		}

		private unsafe static byte[] GetULongBytes(byte* bytes)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return new byte[8]
				{
					bytes[7],
					bytes[6],
					bytes[5],
					bytes[4],
					bytes[3],
					bytes[2],
					bytes[1],
					*bytes
				};
			}
			return new byte[8]
			{
				*bytes,
				bytes[1],
				bytes[2],
				bytes[3],
				bytes[4],
				bytes[5],
				bytes[6],
				bytes[7]
			};
		}

		internal static byte[] GetBytes(bool value)
		{
			return new byte[1] { (byte)(value ? 1 : 0) };
		}

		internal unsafe static byte[] GetBytes(char value)
		{
			return GetUShortBytes((byte*)(&value));
		}

		internal unsafe static byte[] GetBytes(short value)
		{
			return GetUShortBytes((byte*)(&value));
		}

		internal unsafe static byte[] GetBytes(int value)
		{
			return GetUIntBytes((byte*)(&value));
		}

		internal unsafe static byte[] GetBytes(long value)
		{
			return GetULongBytes((byte*)(&value));
		}

		internal unsafe static byte[] GetBytes(ushort value)
		{
			return GetUShortBytes((byte*)(&value));
		}

		internal unsafe static byte[] GetBytes(uint value)
		{
			return GetUIntBytes((byte*)(&value));
		}

		internal unsafe static byte[] GetBytes(ulong value)
		{
			return GetULongBytes((byte*)(&value));
		}

		internal unsafe static byte[] GetBytes(float value)
		{
			return GetUIntBytes((byte*)(&value));
		}

		internal unsafe static byte[] GetBytes(double value)
		{
			return GetULongBytes((byte*)(&value));
		}

		private unsafe static void UShortFromBytes(byte* dst, byte[] src, int startIndex)
		{
			if (BitConverter.IsLittleEndian)
			{
				*dst = src[startIndex];
				dst[1] = src[startIndex + 1];
			}
			else
			{
				*dst = src[startIndex + 1];
				dst[1] = src[startIndex];
			}
		}

		private unsafe static void UIntFromBytes(byte* dst, byte[] src, int startIndex)
		{
			if (BitConverter.IsLittleEndian)
			{
				*dst = src[startIndex];
				dst[1] = src[startIndex + 1];
				dst[2] = src[startIndex + 2];
				dst[3] = src[startIndex + 3];
			}
			else
			{
				*dst = src[startIndex + 3];
				dst[1] = src[startIndex + 2];
				dst[2] = src[startIndex + 1];
				dst[3] = src[startIndex];
			}
		}

		private unsafe static void ULongFromBytes(byte* dst, byte[] src, int startIndex)
		{
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < 8; i++)
				{
					dst[i] = src[startIndex + i];
				}
			}
			else
			{
				for (int j = 0; j < 8; j++)
				{
					dst[j] = src[startIndex + (7 - j)];
				}
			}
		}

		internal static bool ToBoolean(byte[] value, int startIndex)
		{
			return value[startIndex] != 0;
		}

		internal unsafe static char ToChar(byte[] value, int startIndex)
		{
			char result = default(char);
			UShortFromBytes((byte*)(&result), value, startIndex);
			return result;
		}

		internal unsafe static short ToInt16(byte[] value, int startIndex)
		{
			short result = default(short);
			UShortFromBytes((byte*)(&result), value, startIndex);
			return result;
		}

		internal unsafe static int ToInt32(byte[] value, int startIndex)
		{
			int result = default(int);
			UIntFromBytes((byte*)(&result), value, startIndex);
			return result;
		}

		internal unsafe static long ToInt64(byte[] value, int startIndex)
		{
			long result = default(long);
			ULongFromBytes((byte*)(&result), value, startIndex);
			return result;
		}

		internal unsafe static ushort ToUInt16(byte[] value, int startIndex)
		{
			ushort result = default(ushort);
			UShortFromBytes((byte*)(&result), value, startIndex);
			return result;
		}

		internal unsafe static uint ToUInt32(byte[] value, int startIndex)
		{
			uint result = default(uint);
			UIntFromBytes((byte*)(&result), value, startIndex);
			return result;
		}

		internal unsafe static ulong ToUInt64(byte[] value, int startIndex)
		{
			ulong result = default(ulong);
			ULongFromBytes((byte*)(&result), value, startIndex);
			return result;
		}

		internal unsafe static float ToSingle(byte[] value, int startIndex)
		{
			float result = default(float);
			UIntFromBytes((byte*)(&result), value, startIndex);
			return result;
		}

		internal unsafe static double ToDouble(byte[] value, int startIndex)
		{
			double result = default(double);
			ULongFromBytes((byte*)(&result), value, startIndex);
			return result;
		}
	}
}
