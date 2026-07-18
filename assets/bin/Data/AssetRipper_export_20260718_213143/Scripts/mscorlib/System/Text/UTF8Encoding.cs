using System.Runtime.InteropServices;

namespace System.Text
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	[MonoTODO("EncoderFallback is not handled")]
	public class UTF8Encoding : Encoding
	{
		[Serializable]
		private class UTF8Decoder : Decoder
		{
			private uint leftOverBits;

			private uint leftOverCount;

			public UTF8Decoder(DecoderFallback fallback)
			{
				base.Fallback = fallback;
				leftOverBits = 0u;
				leftOverCount = 0u;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				DecoderFallbackBuffer fallbackBuffer = null;
				byte[] bufferArg = null;
				return InternalGetCharCount(bytes, index, count, leftOverBits, leftOverCount, this, ref fallbackBuffer, ref bufferArg, false);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				DecoderFallbackBuffer fallbackBuffer = null;
				byte[] bufferArg = null;
				return InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref leftOverBits, ref leftOverCount, this, ref fallbackBuffer, ref bufferArg, false);
			}
		}

		[Serializable]
		private class UTF8Encoder : Encoder
		{
			private char leftOverForCount;

			private char leftOverForConv;

			public UTF8Encoder(bool emitIdentifier)
			{
				leftOverForCount = '\0';
				leftOverForConv = '\0';
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return InternalGetByteCount(chars, index, count, ref leftOverForCount, flush);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, bool flush)
			{
				return InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, ref leftOverForConv, flush);
			}

			public unsafe override int GetByteCount(char* chars, int count, bool flush)
			{
				return InternalGetByteCount(chars, count, ref leftOverForCount, flush);
			}

			public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, bool flush)
			{
				return InternalGetBytes(chars, charCount, bytes, byteCount, ref leftOverForConv, flush);
			}
		}

		internal const int UTF8_CODE_PAGE = 65001;

		private bool emitIdentifier;

		public UTF8Encoding()
			: this(false, false)
		{
		}

		public UTF8Encoding(bool encoderShouldEmitUTF8Identifier)
			: this(encoderShouldEmitUTF8Identifier, false)
		{
		}

		public UTF8Encoding(bool encoderShouldEmitUTF8Identifier, bool throwOnInvalidBytes)
			: base(65001)
		{
			emitIdentifier = encoderShouldEmitUTF8Identifier;
			if (throwOnInvalidBytes)
			{
				SetFallbackInternal(null, DecoderFallback.ExceptionFallback);
			}
			else
			{
				SetFallbackInternal(null, DecoderFallback.StandardSafeFallback);
			}
			web_name = (body_name = (header_name = "utf-8"));
			encoding_name = "Unicode (UTF-8)";
			is_browser_save = true;
			is_browser_display = true;
			is_mail_news_display = true;
			is_mail_news_save = true;
			windows_code_page = 1200;
		}

		private unsafe static int InternalGetByteCount(char[] chars, int index, int count, ref char leftOver, bool flush)
		{
			//IL_0090->IL0097: Incompatible stack types: I vs Ref
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
			if (index == chars.Length)
			{
				if (flush && leftOver != 0)
				{
					leftOver = '\0';
					return 3;
				}
				return 0;
			}
			fixed (char* ptr = &(chars != null && chars.Length != 0 ? ref chars[0] : ref *(char*)null))
			{
				return InternalGetByteCount((char*)((byte*)ptr + index * 2), count, ref leftOver, flush);
			}
		}

		private unsafe static int InternalGetByteCount(char* chars, int count, ref char leftOver, bool flush)
		{
			int num = 0;
			char* ptr = (char*)((byte*)chars + count * 2);
			while (chars < ptr)
			{
				if (leftOver == '\0')
				{
					while (chars < ptr)
					{
						if (*chars < '\u0080')
						{
							num++;
						}
						else if (*chars < 'ࠀ')
						{
							num += 2;
						}
						else if (*chars < '\ud800' || *chars > '\udfff')
						{
							num += 3;
						}
						else if (*chars <= '\udbff')
						{
							if (chars + 1 >= ptr || chars[1] < '\udc00' || chars[1] > '\udfff')
							{
								leftOver = *chars;
								chars++;
								break;
							}
							num += 4;
							chars++;
						}
						else
						{
							num += 3;
							leftOver = '\0';
						}
						chars++;
					}
				}
				else
				{
					if (*chars >= '\udc00' && *chars <= '\udfff')
					{
						num += 4;
						chars++;
					}
					else
					{
						num += 3;
					}
					leftOver = '\0';
				}
			}
			if (flush && leftOver != 0)
			{
				num += 3;
				leftOver = '\0';
			}
			return num;
		}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			char leftOver = '\0';
			return InternalGetByteCount(chars, index, count, ref leftOver, true);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetByteCount(char* chars, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (count == 0)
			{
				return 0;
			}
			char leftOver = '\0';
			return InternalGetByteCount(chars, count, ref leftOver, true);
		}

		private unsafe static int InternalGetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, ref char leftOver, bool flush)
		{
			//IL_00c8->IL00cf: Incompatible stack types: I vs Ref
			//IL_0102->IL0109: Incompatible stack types: I vs Ref
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
			if (charIndex == chars.Length)
			{
				if (flush && leftOver != 0)
				{
					leftOver = '\0';
				}
				return 0;
			}
			fixed (char* ptr = &(chars != null && chars.Length != 0 ? ref chars[0] : ref *(char*)null))
			{
				if (bytes.Length == byteIndex)
				{
					return InternalGetBytes((char*)((byte*)ptr + charIndex * 2), charCount, null, 0, ref leftOver, flush);
				}
				fixed (byte* ptr2 = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
				{
					return InternalGetBytes((char*)((byte*)ptr + charIndex * 2), charCount, ptr2 + byteIndex, bytes.Length - byteIndex, ref leftOver, flush);
				}
			}
		}

		private unsafe static int InternalGetBytes(char* chars, int count, byte* bytes, int bcount, ref char leftOver, bool flush)
		{
			char* ptr = (char*)((byte*)chars + count * 2);
			byte* ptr2 = bytes + bcount;
			while (true)
			{
				if (chars < ptr)
				{
					if (leftOver == '\0')
					{
						while (chars < ptr)
						{
							int num = *chars;
							if (num < 128)
							{
								if (bytes >= ptr2)
								{
									goto end_IL_022c;
								}
								*(bytes++) = (byte)num;
							}
							else if (num < 2048)
							{
								if (bytes + 1 >= ptr2)
								{
									goto end_IL_022c;
								}
								*bytes = (byte)(0xC0 | (num >> 6));
								bytes[1] = (byte)(0x80 | (num & 0x3F));
								bytes += 2;
							}
							else if (num < 55296 || num > 57343)
							{
								if (bytes + 2 >= ptr2)
								{
									goto end_IL_022c;
								}
								*bytes = (byte)(0xE0 | (num >> 12));
								bytes[1] = (byte)(0x80 | ((num >> 6) & 0x3F));
								bytes[2] = (byte)(0x80 | (num & 0x3F));
								bytes += 3;
							}
							else
							{
								if (num <= 56319)
								{
									leftOver = *chars;
									chars++;
									break;
								}
								if (bytes + 2 >= ptr2)
								{
									goto end_IL_022c;
								}
								*bytes = (byte)(0xE0 | (num >> 12));
								bytes[1] = (byte)(0x80 | ((num >> 6) & 0x3F));
								bytes[2] = (byte)(0x80 | (num & 0x3F));
								bytes += 3;
								leftOver = '\0';
							}
							chars++;
						}
						continue;
					}
					if (*chars >= '\udc00' && *chars <= '\udfff')
					{
						int num2 = 65536 + *chars - 56320 + (leftOver - 55296 << 10);
						if (bytes + 3 >= ptr2)
						{
							break;
						}
						*bytes = (byte)(0xF0 | (num2 >> 18));
						bytes[1] = (byte)(0x80 | ((num2 >> 12) & 0x3F));
						bytes[2] = (byte)(0x80 | ((num2 >> 6) & 0x3F));
						bytes[3] = (byte)(0x80 | (num2 & 0x3F));
						bytes += 4;
						chars++;
					}
					else
					{
						int num3 = leftOver;
						if (bytes + 2 >= ptr2)
						{
							break;
						}
						*bytes = (byte)(0xE0 | (num3 >> 12));
						bytes[1] = (byte)(0x80 | ((num3 >> 6) & 0x3F));
						bytes[2] = (byte)(0x80 | (num3 & 0x3F));
						bytes += 3;
					}
					leftOver = '\0';
					continue;
				}
				if (flush && leftOver != 0)
				{
					int num4 = leftOver;
					if (bytes + 2 >= ptr2)
					{
						break;
					}
					*bytes = (byte)(0xE0 | (num4 >> 12));
					bytes[1] = (byte)(0x80 | ((num4 >> 6) & 0x3F));
					bytes[2] = (byte)(0x80 | (num4 & 0x3F));
					bytes += 3;
					leftOver = '\0';
				}
				return (int)(long)(IntPtr)(bytes - (ulong)(ptr2 - bcount));
				continue;
				end_IL_022c:
				break;
			}
			throw new ArgumentException("Insufficient Space", "bytes");
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			char leftOver = '\0';
			return InternalGetBytes(chars, charIndex, charCount, bytes, byteIndex, ref leftOver, true);
		}

		public unsafe override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			//IL_00ec->IL00f4: Incompatible stack types: I vs Ref
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charIndex < 0 || charIndex > s.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_StringIndex"));
			}
			if (charCount < 0 || charCount > s.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_StringRange"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			if (charIndex == s.Length)
			{
				return 0;
			}
			fixed (char* ptr = s)
			{
				char leftOver = '\0';
				if (bytes.Length == byteIndex)
				{
					return InternalGetBytes((char*)((byte*)ptr + charIndex * 2), charCount, null, 0, ref leftOver, true);
				}
				fixed (byte* ptr2 = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
				{
					return InternalGetBytes((char*)((byte*)ptr + charIndex * 2), charCount, ptr2 + byteIndex, bytes.Length - byteIndex, ref leftOver, true);
				}
			}
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charCount < 0)
			{
				throw new IndexOutOfRangeException("charCount");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (byteCount < 0)
			{
				throw new IndexOutOfRangeException("charCount");
			}
			if (charCount == 0)
			{
				return 0;
			}
			char leftOver = '\0';
			if (byteCount == 0)
			{
				return InternalGetBytes(chars, charCount, null, 0, ref leftOver, true);
			}
			return InternalGetBytes(chars, charCount, bytes, byteCount, ref leftOver, true);
		}

		private unsafe static int InternalGetCharCount(byte[] bytes, int index, int count, uint leftOverBits, uint leftOverCount, object provider, ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
		{
			//IL_007a->IL0081: Incompatible stack types: I vs Ref
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
			if (count == 0)
			{
				return 0;
			}
			fixed (byte* ptr = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
			{
				return InternalGetCharCount(ptr + index, count, leftOverBits, leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
			}
		}

		private unsafe static int InternalGetCharCount(byte* bytes, int count, uint leftOverBits, uint leftOverCount, object provider, ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
		{
			int num = 0;
			int num2 = 0;
			if (leftOverCount == 0)
			{
				int num3 = num + count;
				while (num < num3 && bytes[num] < 128)
				{
					num2++;
					num++;
					count--;
				}
			}
			uint num4 = leftOverBits;
			uint num5 = leftOverCount & 0xF;
			uint num6 = (leftOverCount >> 4) & 0xF;
			while (count > 0)
			{
				uint num7 = bytes[num++];
				count--;
				if (num6 == 0)
				{
					if (num7 < 128)
					{
						num2++;
					}
					else if ((num7 & 0xE0) == 192)
					{
						num4 = num7 & 0x1F;
						num5 = 1u;
						num6 = 2u;
					}
					else if ((num7 & 0xF0) == 224)
					{
						num4 = num7 & 0xF;
						num5 = 1u;
						num6 = 3u;
					}
					else if ((num7 & 0xF8) == 240)
					{
						num4 = num7 & 7;
						num5 = 1u;
						num6 = 4u;
					}
					else if ((num7 & 0xFC) == 248)
					{
						num4 = num7 & 3;
						num5 = 1u;
						num6 = 5u;
					}
					else if ((num7 & 0xFE) == 252)
					{
						num4 = num7 & 3;
						num5 = 1u;
						num6 = 6u;
					}
					else
					{
						num2 += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, num - 1, 1u);
					}
				}
				else if ((num7 & 0xC0) == 128)
				{
					num4 = (num4 << 6) | (num7 & 0x3F);
					if (++num5 < num6)
					{
						continue;
					}
					if (num4 >= 65536)
					{
						num2 = ((num4 >= 1114112) ? (num2 + Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, num - num5, num5)) : (num2 + 2));
					}
					else
					{
						bool flag = false;
						switch (num6)
						{
						case 2u:
							flag = num4 <= 127;
							break;
						case 3u:
							flag = num4 <= 2047;
							break;
						case 4u:
							flag = num4 <= 65535;
							break;
						case 5u:
							flag = num4 <= 2097151;
							break;
						case 6u:
							flag = num4 <= 67108863;
							break;
						}
						num2 = (flag ? (num2 + Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, num - num5, num5)) : (((num4 & 0xF800) != 55296) ? (num2 + 1) : (num2 + Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, num - num5, num5))));
					}
					num6 = 0u;
				}
				else
				{
					num2 += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, num - num5, num5);
					num6 = 0u;
					num--;
					count++;
				}
			}
			if (flush && num6 != 0)
			{
				num2 += Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, num - num5, num5);
			}
			return num2;
		}

		private unsafe static int Fallback(object provider, ref DecoderFallbackBuffer buffer, ref byte[] bufferArg, byte* bytes, long index, uint size)
		{
			if (buffer == null)
			{
				DecoderFallback decoderFallback = provider as DecoderFallback;
				if (decoderFallback != null)
				{
					buffer = decoderFallback.CreateFallbackBuffer();
				}
				else
				{
					buffer = ((Decoder)provider).FallbackBuffer;
				}
			}
			if (bufferArg == null)
			{
				bufferArg = new byte[1];
			}
			int num = 0;
			for (int i = 0; i < size; i++)
			{
				bufferArg[0] = bytes[(int)index + i];
				buffer.Fallback(bufferArg, 0);
				num += buffer.Remaining;
				buffer.Reset();
			}
			return num;
		}

		private unsafe static void Fallback(object provider, ref DecoderFallbackBuffer buffer, ref byte[] bufferArg, byte* bytes, long byteIndex, uint size, char* chars, ref int charIndex)
		{
			if (buffer == null)
			{
				DecoderFallback decoderFallback = provider as DecoderFallback;
				if (decoderFallback != null)
				{
					buffer = decoderFallback.CreateFallbackBuffer();
				}
				else
				{
					buffer = ((Decoder)provider).FallbackBuffer;
				}
			}
			if (bufferArg == null)
			{
				bufferArg = new byte[1];
			}
			for (int i = 0; i < size; i++)
			{
				bufferArg[0] = bytes[byteIndex + i];
				buffer.Fallback(bufferArg, 0);
				while (buffer.Remaining > 0)
				{
					*(char*)((byte*)chars + charIndex++ * 2) = buffer.GetNextChar();
				}
				buffer.Reset();
			}
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			DecoderFallbackBuffer fallbackBuffer = null;
			byte[] bufferArg = null;
			return InternalGetCharCount(bytes, index, count, 0u, 0u, base.DecoderFallback, ref fallbackBuffer, ref bufferArg, true);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			DecoderFallbackBuffer fallbackBuffer = null;
			byte[] bufferArg = null;
			return InternalGetCharCount(bytes, count, 0u, 0u, base.DecoderFallback, ref fallbackBuffer, ref bufferArg, true);
		}

		private unsafe static int InternalGetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, ref uint leftOverBits, ref uint leftOverCount, object provider, ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
		{
			//IL_00b6->IL00bd: Incompatible stack types: I vs Ref
			//IL_0103->IL010a: Incompatible stack types: I vs Ref
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
			if (charIndex == chars.Length)
			{
				return 0;
			}
			fixed (char* ptr = &(chars != null && chars.Length != 0 ? ref chars[0] : ref *(char*)null))
			{
				if (byteCount == 0 || byteIndex == bytes.Length)
				{
					return InternalGetChars(null, 0, (char*)((byte*)ptr + charIndex * 2), chars.Length - charIndex, ref leftOverBits, ref leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
				}
				fixed (byte* ptr2 = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
				{
					return InternalGetChars(ptr2 + byteIndex, byteCount, (char*)((byte*)ptr + charIndex * 2), chars.Length - charIndex, ref leftOverBits, ref leftOverCount, provider, ref fallbackBuffer, ref bufferArg, flush);
				}
			}
		}

		private unsafe static int InternalGetChars(byte* bytes, int byteCount, char* chars, int charCount, ref uint leftOverBits, ref uint leftOverCount, object provider, ref DecoderFallbackBuffer fallbackBuffer, ref byte[] bufferArg, bool flush)
		{
			int num = 0;
			int i = 0;
			int charIndex = num;
			if (leftOverCount == 0)
			{
				int num2 = i + byteCount;
				while (i < num2 && bytes[i] < 128)
				{
					*(short*)((byte*)chars + charIndex * 2) = bytes[i];
					charIndex++;
					i++;
					byteCount--;
				}
			}
			uint num3 = leftOverBits;
			uint num4 = leftOverCount & 0xF;
			uint num5 = (leftOverCount >> 4) & 0xF;
			for (int num6 = i + byteCount; i < num6; i++)
			{
				uint num7 = bytes[i];
				if (num5 == 0)
				{
					if (num7 < 128)
					{
						if (charIndex >= charCount)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
						}
						*(ushort*)((byte*)chars + charIndex++ * 2) = (ushort)num7;
					}
					else if ((num7 & 0xE0) == 192)
					{
						num3 = num7 & 0x1F;
						num4 = 1u;
						num5 = 2u;
					}
					else if ((num7 & 0xF0) == 224)
					{
						num3 = num7 & 0xF;
						num4 = 1u;
						num5 = 3u;
					}
					else if ((num7 & 0xF8) == 240)
					{
						num3 = num7 & 7;
						num4 = 1u;
						num5 = 4u;
					}
					else if ((num7 & 0xFC) == 248)
					{
						num3 = num7 & 3;
						num4 = 1u;
						num5 = 5u;
					}
					else if ((num7 & 0xFE) == 252)
					{
						num3 = num7 & 3;
						num4 = 1u;
						num5 = 6u;
					}
					else
					{
						Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, i, 1u, chars, ref charIndex);
					}
				}
				else if ((num7 & 0xC0) == 128)
				{
					num3 = (num3 << 6) | (num7 & 0x3F);
					if (++num4 < num5)
					{
						continue;
					}
					if (num3 < 65536)
					{
						bool flag = false;
						switch (num5)
						{
						case 2u:
							flag = num3 <= 127;
							break;
						case 3u:
							flag = num3 <= 2047;
							break;
						case 4u:
							flag = num3 <= 65535;
							break;
						case 5u:
							flag = num3 <= 2097151;
							break;
						case 6u:
							flag = num3 <= 67108863;
							break;
						}
						if (flag)
						{
							Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, i - num4, num4, chars, ref charIndex);
						}
						else if ((num3 & 0xF800) == 55296)
						{
							Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, i - num4, num4, chars, ref charIndex);
						}
						else
						{
							if (charIndex >= charCount)
							{
								throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
							}
							*(ushort*)((byte*)chars + charIndex++ * 2) = (ushort)num3;
						}
					}
					else if (num3 < 1114112)
					{
						if (charIndex + 2 > charCount)
						{
							throw new ArgumentException(Encoding._("Arg_InsufficientSpace"), "chars");
						}
						num3 -= 65536;
						*(ushort*)((byte*)chars + charIndex++ * 2) = (ushort)((num3 >> 10) + 55296);
						*(ushort*)((byte*)chars + charIndex++ * 2) = (ushort)((num3 & 0x3FF) + 56320);
					}
					else
					{
						Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, i - num4, num4, chars, ref charIndex);
					}
					num5 = 0u;
				}
				else
				{
					Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, i - num4, num4, chars, ref charIndex);
					num5 = 0u;
					i--;
				}
			}
			if (flush && num5 != 0)
			{
				Fallback(provider, ref fallbackBuffer, ref bufferArg, bytes, i - num4, num4, chars, ref charIndex);
			}
			leftOverBits = num3;
			leftOverCount = num4 | (num5 << 4);
			return charIndex - num;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			uint leftOverBits = 0u;
			uint leftOverCount = 0u;
			DecoderFallbackBuffer fallbackBuffer = null;
			byte[] bufferArg = null;
			return InternalGetChars(bytes, byteIndex, byteCount, chars, charIndex, ref leftOverBits, ref leftOverCount, base.DecoderFallback, ref fallbackBuffer, ref bufferArg, true);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			DecoderFallbackBuffer fallbackBuffer = null;
			byte[] bufferArg = null;
			uint leftOverBits = 0u;
			uint leftOverCount = 0u;
			return InternalGetChars(bytes, byteCount, chars, charCount, ref leftOverBits, ref leftOverCount, base.DecoderFallback, ref fallbackBuffer, ref bufferArg, true);
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_NonNegative"));
			}
			return charCount * 4;
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
			return new UTF8Decoder(base.DecoderFallback);
		}

		public override Encoder GetEncoder()
		{
			return new UTF8Encoder(emitIdentifier);
		}

		public override byte[] GetPreamble()
		{
			if (emitIdentifier)
			{
				return new byte[3] { 239, 187, 191 };
			}
			return new byte[0];
		}

		public override bool Equals(object value)
		{
			UTF8Encoding uTF8Encoding = value as UTF8Encoding;
			if (uTF8Encoding != null)
			{
				return codePage == uTF8Encoding.codePage && emitIdentifier == uTF8Encoding.emitIdentifier && base.DecoderFallback.Equals(uTF8Encoding.DecoderFallback) && base.EncoderFallback.Equals(uTF8Encoding.EncoderFallback);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override int GetByteCount(string chars)
		{
			return base.GetByteCount(chars);
		}

		[ComVisible(false)]
		public override string GetString(byte[] bytes, int index, int count)
		{
			return base.GetString(bytes, index, count);
		}
	}
}
