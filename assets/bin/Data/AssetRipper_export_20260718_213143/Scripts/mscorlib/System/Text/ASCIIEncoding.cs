using System.Runtime.InteropServices;

namespace System.Text
{
	[Serializable]
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with .NET")]
	public class ASCIIEncoding : Encoding
	{
		internal const int ASCII_CODE_PAGE = 20127;

		[ComVisible(false)]
		public override bool IsSingleByte
		{
			get
			{
				return true;
			}
		}

		public ASCIIEncoding()
			: base(20127)
		{
			body_name = (header_name = (web_name = "us-ascii"));
			encoding_name = "US-ASCII";
			is_mail_news_display = true;
			is_mail_news_save = true;
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
			return count;
		}

		public override int GetByteCount(string chars)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			return chars.Length;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			EncoderFallbackBuffer buffer = null;
			char[] fallback_chars = null;
			return GetBytes(chars, charIndex, charCount, bytes, byteIndex, ref buffer, ref fallback_chars);
		}

		private int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex, ref EncoderFallbackBuffer buffer, ref char[] fallback_chars)
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
			if (bytes.Length - byteIndex < charCount)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			int num = charCount;
			while (num-- > 0)
			{
				char c = chars[charIndex++];
				if (c < '\u0080')
				{
					bytes[byteIndex++] = (byte)c;
					continue;
				}
				if (buffer == null)
				{
					buffer = base.EncoderFallback.CreateFallbackBuffer();
				}
				if (char.IsSurrogate(c) && num > 1 && char.IsSurrogate(chars[charIndex]))
				{
					buffer.Fallback(c, chars[charIndex], charIndex++ - 1);
				}
				else
				{
					buffer.Fallback(c, charIndex - 1);
				}
				if (fallback_chars == null || fallback_chars.Length < buffer.Remaining)
				{
					fallback_chars = new char[buffer.Remaining];
				}
				for (int i = 0; i < fallback_chars.Length; i++)
				{
					fallback_chars[i] = buffer.GetNextChar();
				}
				byteIndex += GetBytes(fallback_chars, 0, fallback_chars.Length, bytes, byteIndex, ref buffer, ref fallback_chars);
			}
			return charCount;
		}

		public override int GetBytes(string chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			EncoderFallbackBuffer buffer = null;
			char[] fallback_chars = null;
			return GetBytes(chars, charIndex, charCount, bytes, byteIndex, ref buffer, ref fallback_chars);
		}

		private int GetBytes(string chars, int charIndex, int charCount, byte[] bytes, int byteIndex, ref EncoderFallbackBuffer buffer, ref char[] fallback_chars)
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
				throw new ArgumentOutOfRangeException("charIndex", Encoding._("ArgRange_StringIndex"));
			}
			if (charCount < 0 || charCount > chars.Length - charIndex)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_StringRange"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			if (bytes.Length - byteIndex < charCount)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			int num = charCount;
			while (num-- > 0)
			{
				char c = chars[charIndex++];
				if (c < '\u0080')
				{
					bytes[byteIndex++] = (byte)c;
					continue;
				}
				if (buffer == null)
				{
					buffer = base.EncoderFallback.CreateFallbackBuffer();
				}
				if (char.IsSurrogate(c) && num > 1 && char.IsSurrogate(chars[charIndex]))
				{
					buffer.Fallback(c, chars[charIndex], charIndex++ - 1);
				}
				else
				{
					buffer.Fallback(c, charIndex - 1);
				}
				if (fallback_chars == null || fallback_chars.Length < buffer.Remaining)
				{
					fallback_chars = new char[buffer.Remaining];
				}
				for (int i = 0; i < fallback_chars.Length; i++)
				{
					fallback_chars[i] = buffer.GetNextChar();
				}
				byteIndex += GetBytes(fallback_chars, 0, fallback_chars.Length, bytes, byteIndex, ref buffer, ref fallback_chars);
			}
			return charCount;
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
			return count;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			DecoderFallbackBuffer buffer = null;
			return GetChars(bytes, byteIndex, byteCount, chars, charIndex, ref buffer);
		}

		private int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, ref DecoderFallbackBuffer buffer)
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
			if (chars.Length - charIndex < byteCount)
			{
				throw new ArgumentException(Encoding._("Arg_InsufficientSpace"));
			}
			int num = byteCount;
			while (num-- > 0)
			{
				char c = (char)bytes[byteIndex++];
				if (c < '\u0080')
				{
					chars[charIndex++] = c;
					continue;
				}
				if (buffer == null)
				{
					buffer = base.DecoderFallback.CreateFallbackBuffer();
				}
				buffer.Fallback(bytes, byteIndex);
				while (buffer.Remaining > 0)
				{
					chars[charIndex++] = buffer.GetNextChar();
				}
			}
			return byteCount;
		}

		public override int GetMaxByteCount(int charCount)
		{
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount", Encoding._("ArgRange_NonNegative"));
			}
			return charCount;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_NonNegative"));
			}
			return byteCount;
		}

		public unsafe override string GetString(byte[] bytes, int byteIndex, int byteCount)
		{
			//IL_007e->IL0085: Incompatible stack types: I vs Ref
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", Encoding._("ArgRange_Array"));
			}
			if (byteCount < 0 || byteCount > bytes.Length - byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteCount", Encoding._("ArgRange_Array"));
			}
			if (byteCount == 0)
			{
				return string.Empty;
			}
			fixed (byte* ptr = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
			{
				string text = string.InternalAllocateStr(byteCount);
				fixed (char* ptr2 = text)
				{
					byte* ptr3 = ptr + byteIndex;
					byte* ptr4 = ptr3 + byteCount;
					char* ptr5 = ptr2;
					while (ptr3 < ptr4)
					{
						byte b = *(ptr3++);
						*(ptr5++) = (char)((b > 127) ? 63 : b);
					}
				}
				return text;
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
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			if (byteCount < charCount)
			{
				throw new ArgumentException("bytecount is less than the number of bytes required", "byteCount");
			}
			for (int i = 0; i < charCount; i++)
			{
				char c = *(char*)((byte*)chars + i * 2);
				bytes[i] = (byte)((c >= '\u0080') ? '?' : c);
			}
			return charCount;
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
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
			if (charCount < byteCount)
			{
				throw new ArgumentException("charcount is less than the number of bytes required", "charCount");
			}
			for (int i = 0; i < byteCount; i++)
			{
				byte b = bytes[i];
				*(short*)((byte*)chars + i * 2) = (short)((b <= 127) ? b : 63);
			}
			return byteCount;
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetCharCount(byte* bytes, int count)
		{
			return count;
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe override int GetByteCount(char* chars, int count)
		{
			return count;
		}

		[ComVisible(false)]
		[MonoTODO("we have simple override to match method signature.")]
		public override Decoder GetDecoder()
		{
			return base.GetDecoder();
		}

		[ComVisible(false)]
		[MonoTODO("we have simple override to match method signature.")]
		public override Encoder GetEncoder()
		{
			return base.GetEncoder();
		}
	}
}
