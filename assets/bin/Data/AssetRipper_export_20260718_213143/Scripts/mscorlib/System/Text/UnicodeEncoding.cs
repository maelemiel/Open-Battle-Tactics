using System.Runtime.InteropServices;

namespace System.Text
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	public class UnicodeEncoding : Encoding
	{
		private sealed class UnicodeDecoder : Decoder
		{
			private bool bigEndian;

			private int leftOverByte;

			public UnicodeDecoder(bool bigEndian)
			{
				this.bigEndian = bigEndian;
				leftOverByte = -1;
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
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
				if (leftOverByte != -1)
				{
					return (count + 1) / 2;
				}
				return count / 2;
			}

			public unsafe override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				//IL_0138->IL013f: Incompatible stack types: I vs Ref
				//IL_0157->IL015f: Incompatible stack types: I vs Ref
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
				if (byteCount == 0)
				{
					return 0;
				}
				int num = leftOverByte;
				int num2 = ((num == -1) ? (byteCount / 2) : ((byteCount + 1) / 2));
				if (chars.Length - charIndex < num2)
				{
					throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
				}
				if (num != -1)
				{
					if (bigEndian)
					{
						chars[charIndex] = (char)((num << 8) | bytes[byteIndex]);
					}
					else
					{
						chars[charIndex] = (char)((bytes[byteIndex] << 8) | num);
					}
					charIndex++;
					byteIndex++;
					byteCount--;
				}
				if ((byteCount & -2) != 0)
				{
					fixed (byte* ptr = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
					{
						fixed (char* ptr2 = &(chars != null && chars.Length != 0 ? ref chars[0] : ref *(char*)null))
						{
							CopyChars(ptr + byteIndex, (byte*)ptr2 + charIndex * 2, byteCount, bigEndian);
						}
					}
				}
				if ((byteCount & 1) == 0)
				{
					leftOverByte = -1;
				}
				else
				{
					leftOverByte = bytes[byteCount + byteIndex - 1];
				}
				return num2;
			}
		}

		internal const int UNICODE_CODE_PAGE = 1200;

		internal const int BIG_UNICODE_CODE_PAGE = 1201;

		public const int CharSize = 2;

		private bool bigEndian;

		private bool byteOrderMark;

		public UnicodeEncoding()
			: this(false, true)
		{
			bigEndian = false;
			byteOrderMark = true;
		}

		public UnicodeEncoding(bool bigEndian, bool byteOrderMark)
			: this(bigEndian, byteOrderMark, false)
		{
		}

		public UnicodeEncoding(bool bigEndian, bool byteOrderMark, bool throwOnInvalidBytes)
			: base((!bigEndian) ? 1200 : 1201)
		{
			if (throwOnInvalidBytes)
			{
				SetFallbackInternal(null, new DecoderExceptionFallback());
			}
			else
			{
				SetFallbackInternal(null, new DecoderReplacementFallback("\ufffd"));
			}
			this.bigEndian = bigEndian;
			this.byteOrderMark = byteOrderMark;
			if (bigEndian)
			{
				body_name = "unicodeFFFE";
				encoding_name = "Unicode (Big-Endian)";
				header_name = "unicodeFFFE";
				is_browser_save = false;
				web_name = "unicodeFFFE";
			}
			else
			{
				body_name = "utf-16";
				encoding_name = "Unicode";
				header_name = "utf-16";
				is_browser_save = true;
				web_name = "utf-16";
			}
			windows_code_page = 1200;
		}

		public override int GetByteCount(char[] chars, int index, int count)
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
			return count * 2;
		}

		public override int GetByteCount(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return s.Length * 2;
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetByteCount(char* chars, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return count * 2;
		}

		public unsafe override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			//IL_00cd->IL00d4: Incompatible stack types: I vs Ref
			//IL_00ec->IL00f4: Incompatible stack types: I vs Ref
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
			if (charCount == 0)
			{
				return 0;
			}
			int byteCount = bytes.Length - byteIndex;
			if (bytes.Length == 0)
			{
				bytes = new byte[1];
			}
			fixed (char* ptr = &(chars != null && chars.Length != 0 ? ref chars[0] : ref *(char*)null))
			{
				fixed (byte* ptr2 = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
				{
					return GetBytesInternal((char*)((byte*)ptr + charIndex * 2), charCount, ptr2 + byteIndex, byteCount);
				}
			}
		}

		public unsafe override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			//IL_00e0->IL00e8: Incompatible stack types: I vs Ref
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
			if (charCount == 0)
			{
				return 0;
			}
			int byteCount = bytes.Length - byteIndex;
			if (bytes.Length == 0)
			{
				bytes = new byte[1];
			}
			fixed (char* ptr = s)
			{
				fixed (byte* ptr2 = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
				{
					return GetBytesInternal((char*)((byte*)ptr + charIndex * 2), charCount, ptr2 + byteIndex, byteCount);
				}
			}
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			return GetBytesInternal(chars, charCount, bytes, byteCount);
		}

		private unsafe int GetBytesInternal(char* chars, int charCount, byte* bytes, int byteCount)
		{
			int num = charCount * 2;
			if (byteCount < num)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			CopyChars((byte*)chars, bytes, num, bigEndian);
			return num;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
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
			return count / 2;
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return count / 2;
		}

		public unsafe override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			//IL_00cd->IL00d4: Incompatible stack types: I vs Ref
			//IL_00ec->IL00f4: Incompatible stack types: I vs Ref
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
			if (byteCount == 0)
			{
				return 0;
			}
			int charCount = chars.Length - charIndex;
			if (chars.Length == 0)
			{
				chars = new char[1];
			}
			fixed (byte* ptr = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
			{
				fixed (char* ptr2 = &(chars != null && chars.Length != 0 ? ref chars[0] : ref *(char*)null))
				{
					return GetCharsInternal(ptr + byteIndex, byteCount, (char*)((byte*)ptr2 + charIndex * 2), charCount);
				}
			}
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			return GetCharsInternal(bytes, byteCount, chars, charCount);
		}

		[ComVisible(false)]
		public unsafe override string GetString(byte[] bytes, int index, int count)
		{
			//IL_0089->IL0090: Incompatible stack types: I vs Ref
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
				return string.Empty;
			}
			int num = count / 2;
			string text = string.InternalAllocateStr(num);
			fixed (byte* ptr = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
			{
				fixed (char* chars = text)
				{
					GetCharsInternal(ptr + index, count, chars, num);
				}
			}
			return text;
		}

		private unsafe int GetCharsInternal(byte* bytes, int byteCount, char* chars, int charCount)
		{
			int num = byteCount / 2;
			if (charCount < num)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			CopyChars(bytes, (byte*)chars, byteCount, bigEndian);
			return num;
		}

		[ComVisible(false)]
		public override Encoder GetEncoder()
		{
			return base.GetEncoder();
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_NonNegative"));
			}
			return charCount * 2;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_NonNegative"));
			}
			return byteCount / 2;
		}

		public override Decoder GetDecoder()
		{
			return new UnicodeDecoder(bigEndian);
		}

		public override byte[] GetPreamble()
		{
			if (byteOrderMark)
			{
				byte[] array = new byte[2];
				if (bigEndian)
				{
					array[0] = 254;
					array[1] = byte.MaxValue;
				}
				else
				{
					array[0] = byte.MaxValue;
					array[1] = 254;
				}
				return array;
			}
			return new byte[0];
		}

		public override bool Equals(object value)
		{
			UnicodeEncoding unicodeEncoding = value as UnicodeEncoding;
			if (unicodeEncoding != null)
			{
				return codePage == unicodeEncoding.codePage && bigEndian == unicodeEncoding.bigEndian && byteOrderMark == unicodeEncoding.byteOrderMark;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private unsafe static void CopyChars(byte* src, byte* dest, int count, bool bigEndian)
		{
			if (BitConverter.IsLittleEndian != bigEndian)
			{
				string.memcpy(dest, src, count & -2);
				return;
			}
			switch (count)
			{
			case 0:
				return;
			case 1:
				return;
			default:
				do
				{
					*dest = src[1];
					dest[1] = *src;
					dest[2] = src[3];
					dest[3] = src[2];
					dest[4] = src[5];
					dest[5] = src[4];
					dest[6] = src[7];
					dest[7] = src[6];
					dest[8] = src[9];
					dest[9] = src[8];
					dest[10] = src[11];
					dest[11] = src[10];
					dest[12] = src[13];
					dest[13] = src[12];
					dest[14] = src[15];
					dest[15] = src[14];
					dest += 16;
					src += 16;
					count -= 16;
				}
				while ((count & -16) != 0);
				switch (count)
				{
				case 0:
					return;
				case 1:
					return;
				case 4:
				case 5:
				case 6:
				case 7:
					goto IL_01f1;
				case 2:
				case 3:
					goto end_IL_001a;
				}
				goto case 8;
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
				*dest = src[1];
				dest[1] = *src;
				dest[2] = src[3];
				dest[3] = src[2];
				dest[4] = src[5];
				dest[5] = src[4];
				dest[6] = src[7];
				dest[7] = src[6];
				dest += 8;
				src += 8;
				if ((count & 4) != 0)
				{
					goto IL_01f1;
				}
				goto IL_0217;
			case 4:
			case 5:
			case 6:
			case 7:
				goto IL_01f1;
			case 2:
			case 3:
				break;
				IL_0217:
				if ((count & 2) == 0)
				{
					return;
				}
				break;
				IL_01f1:
				*dest = src[1];
				dest[1] = *src;
				dest[2] = src[3];
				dest[3] = src[2];
				dest += 4;
				src += 4;
				goto IL_0217;
				end_IL_001a:
				break;
			}
			*dest = src[1];
			dest[1] = *src;
		}
	}
}
