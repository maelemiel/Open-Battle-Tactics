using System.Runtime.InteropServices;

namespace System.Text
{
	[Serializable]
	[ComVisible(true)]
	public abstract class Decoder
	{
		private DecoderFallback fallback = new DecoderReplacementFallback();

		private DecoderFallbackBuffer fallback_buffer;

		[ComVisible(false)]
		public DecoderFallback Fallback
		{
			get
			{
				return fallback;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				fallback = value;
				fallback_buffer = null;
			}
		}

		[ComVisible(false)]
		public DecoderFallbackBuffer FallbackBuffer
		{
			get
			{
				if (fallback_buffer == null)
				{
					fallback_buffer = fallback.CreateFallbackBuffer();
				}
				return fallback_buffer;
			}
		}

		public abstract int GetCharCount(byte[] bytes, int index, int count);

		public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);

		[ComVisible(false)]
		public virtual int GetCharCount(byte[] bytes, int index, int count, bool flush)
		{
			if (flush)
			{
				Reset();
			}
			return GetCharCount(bytes, index, count);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe virtual int GetCharCount(byte* bytes, int count, bool flush)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			byte[] array = new byte[count];
			Marshal.Copy((IntPtr)bytes, array, 0, count);
			return GetCharCount(array, 0, count, flush);
		}

		public virtual int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, bool flush)
		{
			CheckArguments(bytes, byteIndex, byteCount);
			CheckArguments(chars, charIndex);
			if (flush)
			{
				Reset();
			}
			return GetChars(bytes, byteIndex, byteCount, chars, charIndex);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe virtual int GetChars(byte* bytes, int byteCount, char* chars, int charCount, bool flush)
		{
			CheckArguments(chars, charCount, bytes, byteCount);
			char[] array = new char[charCount];
			Marshal.Copy((IntPtr)chars, array, 0, charCount);
			byte[] array2 = new byte[byteCount];
			Marshal.Copy((IntPtr)bytes, array2, 0, byteCount);
			return GetChars(array2, 0, byteCount, array, 0, flush);
		}

		[ComVisible(false)]
		public virtual void Reset()
		{
			if (fallback_buffer != null)
			{
				fallback_buffer.Reset();
			}
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe virtual void Convert(byte* bytes, int byteCount, char* chars, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
		{
			CheckArguments(chars, charCount, bytes, byteCount);
			bytesUsed = byteCount;
			while (true)
			{
				charsUsed = GetCharCount(bytes, bytesUsed, flush);
				if (charsUsed <= charCount)
				{
					break;
				}
				flush = false;
				bytesUsed >>= 1;
			}
			completed = bytesUsed == byteCount;
			charsUsed = GetChars(bytes, bytesUsed, chars, charCount, flush);
		}

		[ComVisible(false)]
		public virtual void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
		{
			CheckArguments(bytes, byteIndex, byteCount);
			CheckArguments(chars, charIndex);
			if (charCount < 0 || chars.Length < charIndex + charCount)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			bytesUsed = byteCount;
			while (true)
			{
				charsUsed = GetCharCount(bytes, byteIndex, bytesUsed, flush);
				if (charsUsed <= charCount)
				{
					break;
				}
				flush = false;
				bytesUsed >>= 1;
			}
			completed = bytesUsed == byteCount;
			charsUsed = GetChars(bytes, byteIndex, bytesUsed, chars, charIndex, flush);
		}

		private void CheckArguments(char[] chars, int charIndex)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charIndex < 0 || chars.Length <= charIndex)
			{
				throw new ArgumentOutOfRangeException("charIndex");
			}
		}

		private void CheckArguments(byte[] bytes, int byteIndex, int byteCount)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (byteIndex < 0 || bytes.Length <= byteIndex)
			{
				throw new ArgumentOutOfRangeException("byteIndex");
			}
			if (byteCount < 0 || bytes.Length < byteIndex + byteCount)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
		}

		private unsafe void CheckArguments(char* chars, int charCount, byte* bytes, int byteCount)
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
		}
	}
}
