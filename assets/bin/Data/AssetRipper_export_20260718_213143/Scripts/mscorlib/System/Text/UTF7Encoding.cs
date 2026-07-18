using System.Runtime.InteropServices;

namespace System.Text
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	public class UTF7Encoding : Encoding
	{
		private sealed class UTF7Decoder : Decoder
		{
			private int leftOver;

			public UTF7Decoder()
			{
				leftOver = 0;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return InternalGetCharCount(bytes, index, count, leftOver);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				return InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref leftOver);
			}
		}

		private sealed class UTF7Encoder : Encoder
		{
			private bool allowOptionals;

			private int leftOver;

			private bool isInShifted;

			public UTF7Encoder(bool allowOptionals)
			{
				this.allowOptionals = allowOptionals;
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return InternalGetByteCount(chars, index, count, flush, leftOver, isInShifted, allowOptionals);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
			{
				return InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, flush, ref leftOver, ref isInShifted, allowOptionals);
			}
		}

		internal const int UTF7_CODE_PAGE = 65000;

		private const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

		private bool allowOptionals;

		private static readonly byte[] encodingRules = new byte[128]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
			1, 0, 0, 1, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 1, 2, 2, 2, 2, 2, 2, 1,
			1, 1, 2, 3, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 2,
			2, 2, 2, 1, 2, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 2, 0, 2, 2, 2, 2, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 2, 2, 2, 0, 0
		};

		private static readonly sbyte[] base64Values = new sbyte[256]
		{
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, 62, -1, -1, -1, 63, 52, 53,
			54, 55, 56, 57, 58, 59, 60, 61, -1, -1,
			-1, -1, -1, -1, -1, 0, 1, 2, 3, 4,
			5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
			25, -1, -1, -1, -1, -1, -1, 26, 27, 28,
			29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
			39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
			49, 50, 51, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1
		};

		public UTF7Encoding()
			: this(false)
		{
		}

		public UTF7Encoding(bool allowOptionals)
			: base(65000)
		{
			this.allowOptionals = allowOptionals;
			body_name = "utf-7";
			encoding_name = "Unicode (UTF-7)";
			header_name = "utf-7";
			is_mail_news_display = true;
			is_mail_news_save = true;
			web_name = "utf-7";
			windows_code_page = 1200;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			return (!allowOptionals) ? hashCode : (-hashCode);
		}

		[ComVisible(false)]
		public override bool Equals(object value)
		{
			UTF7Encoding uTF7Encoding = value as UTF7Encoding;
			if (uTF7Encoding == null)
			{
				return false;
			}
			return allowOptionals == uTF7Encoding.allowOptionals && base.EncoderFallback.Equals(uTF7Encoding.EncoderFallback) && base.DecoderFallback.Equals(uTF7Encoding.DecoderFallback);
		}

		private static int InternalGetByteCount(char[] chars, int index, int count, bool flush, int leftOver, bool isInShifted, bool allowOptionals)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (index < 0 || index > chars.Length)
			{
				throw new ArgumentOutOfRangeException("index", Encoding._("ArgRange_Array"));
			}
			if (count < 0 || count > chars.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", Encoding._("ArgRange_Array"));
			}
			int num = 0;
			int num2 = leftOver >> 8;
			byte[] array = encodingRules;
			while (count > 0)
			{
				int num3 = chars[index++];
				count--;
				switch ((num3 < 128) ? array[num3] : 0)
				{
				case 0:
					if (!isInShifted)
					{
						num++;
						num2 = 0;
						isInShifted = true;
					}
					for (num2 += 16; num2 >= 6; num2 -= 6)
					{
						num++;
					}
					break;
				case 1:
					if (isInShifted)
					{
						if (num2 != 0)
						{
							num++;
							num2 = 0;
						}
						num++;
						isInShifted = false;
					}
					num++;
					break;
				case 2:
					if (allowOptionals)
					{
						goto case 1;
					}
					goto case 0;
				case 3:
					if (isInShifted)
					{
						if (num2 != 0)
						{
							num++;
							num2 = 0;
						}
						num++;
						isInShifted = false;
					}
					num += 2;
					break;
				}
			}
			if (isInShifted && flush)
			{
				if (num2 != 0)
				{
					num++;
				}
				num++;
			}
			return num;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return InternalGetByteCount(chars, index, count, true, 0, false, allowOptionals);
		}

		private static int InternalGetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush, ref int leftOver, ref bool isInShifted, bool allowOptionals)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_Array"));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_Array"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			int num = byteIndex;
			int num2 = bytes.Length;
			int num3 = leftOver >> 8;
			int num4 = leftOver & 0xFF;
			byte[] array = encodingRules;
			string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
			while (charCount > 0)
			{
				int num5 = chars[charIndex++];
				charCount--;
				switch ((num5 < 128) ? array[num5] : 0)
				{
				case 0:
					if (!isInShifted)
					{
						if (num >= num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
						}
						bytes[num++] = 43;
						isInShifted = true;
						num3 = 0;
					}
					num4 = (num4 << 16) | num5;
					num3 += 16;
					while (num3 >= 6)
					{
						if (num >= num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
						}
						num3 -= 6;
						bytes[num++] = (byte)text[num4 >> num3];
						num4 &= (1 << num3) - 1;
					}
					break;
				case 1:
					if (isInShifted)
					{
						if (num3 != 0)
						{
							if (num + 1 > num2)
							{
								throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
							}
							bytes[num++] = (byte)text[num4 << 6 - num3];
						}
						if (num + 1 > num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
						}
						bytes[num++] = 45;
						isInShifted = false;
						num3 = 0;
						num4 = 0;
					}
					if (num >= num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)num5;
					break;
				case 2:
					if (allowOptionals)
					{
						goto case 1;
					}
					goto case 0;
				case 3:
					if (isInShifted)
					{
						if (num3 != 0)
						{
							if (num + 1 > num2)
							{
								throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
							}
							bytes[num++] = (byte)text[num4 << 6 - num3];
						}
						if (num + 1 > num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
						}
						bytes[num++] = 45;
						isInShifted = false;
						num3 = 0;
						num4 = 0;
					}
					if (num + 2 > num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = 43;
					bytes[num++] = 45;
					break;
				}
			}
			if (isInShifted && flush)
			{
				if (num3 != 0)
				{
					if (num + 1 > num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "bytes");
					}
					bytes[num++] = (byte)text[num4 << 6 - num3];
				}
				bytes[num++] = 45;
				num3 = 0;
				num4 = 0;
				isInShifted = false;
			}
			leftOver = (num3 << 8) | num4;
			return num - byteIndex;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			int leftOver = 0;
			bool isInShifted = false;
			return InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, true, ref leftOver, ref isInShifted, allowOptionals);
		}

		private static int InternalGetCharCount(byte[] bytes, int index, int count, int leftOver)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", Encoding._("ArgRange_Array"));
			}
			if (count < 0 || count > bytes.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", Encoding._("ArgRange_Array"));
			}
			int num = 0;
			bool flag = (leftOver & 0x1000000) == 0;
			bool flag2 = (leftOver & 0x2000000) != 0;
			int num2 = (leftOver >> 16) & 0xFF;
			sbyte[] array = base64Values;
			while (count > 0)
			{
				int num3 = bytes[index++];
				count--;
				if (flag)
				{
					if (num3 != 43)
					{
						num++;
						continue;
					}
					flag = false;
					flag2 = true;
					continue;
				}
				if (num3 == 45)
				{
					if (flag2)
					{
						num++;
					}
					num2 = 0;
					flag = true;
				}
				else if (array[num3] != -1)
				{
					num2 += 6;
					if (num2 >= 16)
					{
						num++;
						num2 -= 16;
					}
				}
				else
				{
					num++;
					flag = true;
					num2 = 0;
				}
				flag2 = false;
			}
			return num;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return InternalGetCharCount(bytes, index, count, 0);
		}

		private static int InternalGetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, ref int leftOver)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_Array"));
			}
			if (charIndex < 0 || charIndex > chars.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_Array"));
			}
			int num = charIndex;
			int num2 = chars.Length;
			bool flag = (leftOver & 0x1000000) == 0;
			bool flag2 = (leftOver & 0x2000000) != 0;
			bool flag3 = (leftOver & 0x4000000) != 0;
			int num3 = (leftOver >> 16) & 0xFF;
			int num4 = leftOver & 0xFFFF;
			sbyte[] array = base64Values;
			while (byteCount > 0)
			{
				int num5 = bytes[byteIndex++];
				byteCount--;
				if (flag)
				{
					if (num5 != 43)
					{
						if (num >= num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
						}
						if (flag3)
						{
							throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
						}
						chars[num++] = (char)num5;
					}
					else
					{
						flag = false;
						flag2 = true;
					}
					continue;
				}
				int num6;
				if (num5 == 45)
				{
					if (flag2)
					{
						if (num >= num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
						}
						if (flag3)
						{
							throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
						}
						chars[num++] = '+';
					}
					flag = true;
					num3 = 0;
					num4 = 0;
				}
				else if ((num6 = array[num5]) != -1)
				{
					num4 = (num4 << 6) | num6;
					num3 += 6;
					if (num3 >= 16)
					{
						if (num >= num2)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
						}
						num3 -= 16;
						char c = (char)(num4 >> num3);
						if ((c & 0xFC00) == 55296)
						{
							flag3 = true;
						}
						else if ((c & 0xFC00) == 56320)
						{
							if (!flag3)
							{
								throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
							}
							flag3 = false;
						}
						chars[num++] = c;
						num4 &= (1 << num3) - 1;
					}
				}
				else
				{
					if (num >= num2)
					{
						throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
					}
					if (flag3)
					{
						throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
					}
					chars[num++] = (char)num5;
					flag = true;
					num3 = 0;
					num4 = 0;
				}
				flag2 = false;
			}
			leftOver = num4 | (num3 << 16) | ((!flag) ? 16777216 : 0) | (flag2 ? 33554432 : 0) | (flag3 ? 67108864 : 0);
			return num - charIndex;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			int leftOver = 0;
			int result = InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref leftOver);
			if ((leftOver & 0x4000000) != 0)
			{
				throw new ArgumentException(Encoding._("Arg_InvalidUTF7"), "chars");
			}
			return result;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_NonNegative"));
			}
			if (charCount == 0)
			{
				return 0;
			}
			return 8 * (charCount / 3) + charCount % 3 * 3 + 2;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_NonNegative"));
			}
			return byteCount;
		}

		public override Decoder GetDecoder()
		{
			return new UTF7Decoder();
		}

		public override Encoder GetEncoder()
		{
			return new UTF7Encoder(allowOptionals);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetByteCount(char* chars, int count)
		{
			return base.GetByteCount(chars, count);
		}

		[ComVisible(false)]
		public override int GetByteCount(string s)
		{
			return base.GetByteCount(s);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			return base.GetBytes(chars, charCount, bytes, byteCount);
		}

		[ComVisible(false)]
		public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return base.GetBytes(s, charIndex, charCount, bytes, byteIndex);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			return base.GetCharCount(bytes, count);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			return base.GetChars(bytes, byteCount, chars, charCount);
		}

		[ComVisible(false)]
		public override string GetString(byte[] bytes, int index, int count)
		{
			return base.GetString(bytes, index, count);
		}
	}
}
