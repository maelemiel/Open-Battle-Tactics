using System;
using System.IO;
using System.Text;
using UnityEngine.Internal;

namespace UnityEngine
{
	internal sealed class WWWTranscoder
	{
		private static byte[] ucHexChars = WWW.DefaultEncoding.GetBytes("0123456789ABCDEF");

		private static byte[] lcHexChars = WWW.DefaultEncoding.GetBytes("0123456789abcdef");

		private static byte urlEscapeChar = 37;

		private static byte urlSpace = 43;

		private static byte[] urlForbidden = WWW.DefaultEncoding.GetBytes("@&;:<>=?\"'/\\!#%+$,{}|^[]`");

		private static byte qpEscapeChar = 61;

		private static byte qpSpace = 95;

		private static byte[] qpForbidden = WWW.DefaultEncoding.GetBytes("&;=?\"'%+_");

		private static byte Hex2Byte(byte[] b, int offset)
		{
			byte b2 = 0;
			for (int i = offset; i < offset + 2; i++)
			{
				b2 *= 16;
				int num = b[i];
				if (num >= 48 && num <= 57)
				{
					num -= 48;
				}
				else if (num >= 65 && num <= 75)
				{
					num -= 55;
				}
				else if (num >= 97 && num <= 102)
				{
					num -= 87;
				}
				if (num > 15)
				{
					return 63;
				}
				b2 += (byte)num;
			}
			return b2;
		}

		private static byte[] Byte2Hex(byte b, byte[] hexChars)
		{
			return new byte[2]
			{
				hexChars[b >> 4],
				hexChars[b & 0xF]
			};
		}

		[ExcludeFromDocs]
		public static string URLEncode(string toEncode)
		{
			Encoding uTF = Encoding.UTF8;
			return URLEncode(toEncode, uTF);
		}

		public static string URLEncode(string toEncode, [DefaultValue("Encoding.UTF8")] Encoding e)
		{
			byte[] array = Encode(e.GetBytes(toEncode), urlEscapeChar, urlSpace, urlForbidden, false);
			return WWW.DefaultEncoding.GetString(array, 0, array.Length);
		}

		public static byte[] URLEncode(byte[] toEncode)
		{
			return Encode(toEncode, urlEscapeChar, urlSpace, urlForbidden, false);
		}

		[ExcludeFromDocs]
		public static string QPEncode(string toEncode)
		{
			Encoding uTF = Encoding.UTF8;
			return QPEncode(toEncode, uTF);
		}

		public static string QPEncode(string toEncode, [DefaultValue("Encoding.UTF8")] Encoding e)
		{
			byte[] array = Encode(e.GetBytes(toEncode), qpEscapeChar, qpSpace, qpForbidden, true);
			return WWW.DefaultEncoding.GetString(array, 0, array.Length);
		}

		public static byte[] QPEncode(byte[] toEncode)
		{
			return Encode(toEncode, qpEscapeChar, qpSpace, qpForbidden, true);
		}

		public static byte[] Encode(byte[] input, byte escapeChar, byte space, byte[] forbidden, bool uppercase)
		{
			using (MemoryStream memoryStream = new MemoryStream(input.Length * 2))
			{
				for (int i = 0; i < input.Length; i++)
				{
					if (input[i] == 32)
					{
						memoryStream.WriteByte(space);
					}
					else if (input[i] < 32 || input[i] > 126 || ByteArrayContains(forbidden, input[i]))
					{
						memoryStream.WriteByte(escapeChar);
						memoryStream.Write(Byte2Hex(input[i], (!uppercase) ? lcHexChars : ucHexChars), 0, 2);
					}
					else
					{
						memoryStream.WriteByte(input[i]);
					}
				}
				return memoryStream.ToArray();
			}
		}

		private static bool ByteArrayContains(byte[] array, byte b)
		{
			return Array.IndexOf(array, b) != -1;
		}

		[ExcludeFromDocs]
		public static string URLDecode(string toEncode)
		{
			Encoding uTF = Encoding.UTF8;
			return URLDecode(toEncode, uTF);
		}

		public static string URLDecode(string toEncode, [DefaultValue("Encoding.UTF8")] Encoding e)
		{
			byte[] array = Decode(WWW.DefaultEncoding.GetBytes(toEncode), urlEscapeChar, urlSpace);
			return e.GetString(array, 0, array.Length);
		}

		public static byte[] URLDecode(byte[] toEncode)
		{
			return Decode(toEncode, urlEscapeChar, urlSpace);
		}

		[ExcludeFromDocs]
		public static string QPDecode(string toEncode)
		{
			Encoding uTF = Encoding.UTF8;
			return QPDecode(toEncode, uTF);
		}

		public static string QPDecode(string toEncode, [DefaultValue("Encoding.UTF8")] Encoding e)
		{
			byte[] array = Decode(WWW.DefaultEncoding.GetBytes(toEncode), qpEscapeChar, qpSpace);
			return e.GetString(array, 0, array.Length);
		}

		public static byte[] QPDecode(byte[] toEncode)
		{
			return Decode(toEncode, qpEscapeChar, qpSpace);
		}

		public static byte[] Decode(byte[] input, byte escapeChar, byte space)
		{
			using (MemoryStream memoryStream = new MemoryStream(input.Length))
			{
				for (int i = 0; i < input.Length; i++)
				{
					if (input[i] == space)
					{
						memoryStream.WriteByte(32);
					}
					else if (input[i] == escapeChar && i + 2 < input.Length)
					{
						i++;
						memoryStream.WriteByte(Hex2Byte(input, i++));
					}
					else
					{
						memoryStream.WriteByte(input[i]);
					}
				}
				return memoryStream.ToArray();
			}
		}

		[ExcludeFromDocs]
		public static bool SevenBitClean(string s)
		{
			Encoding uTF = Encoding.UTF8;
			return SevenBitClean(s, uTF);
		}

		public static bool SevenBitClean(string s, [DefaultValue("Encoding.UTF8")] Encoding e)
		{
			return SevenBitClean(e.GetBytes(s));
		}

		public static bool SevenBitClean(byte[] input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] < 32 || input[i] > 126)
				{
					return false;
				}
			}
			return true;
		}
	}
}
