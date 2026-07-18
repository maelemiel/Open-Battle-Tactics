namespace System.Text
{
	[Serializable]
	public sealed class EncoderFallbackException : ArgumentException
	{
		private const string defaultMessage = "Failed to decode the input byte sequence to Unicode characters.";

		private char char_unknown;

		private char char_unknown_high;

		private char char_unknown_low;

		private int index = -1;

		public char CharUnknown
		{
			get
			{
				return char_unknown;
			}
		}

		public char CharUnknownHigh
		{
			get
			{
				return char_unknown_high;
			}
		}

		public char CharUnknownLow
		{
			get
			{
				return char_unknown_low;
			}
		}

		[MonoTODO]
		public int Index
		{
			get
			{
				return index;
			}
		}

		public EncoderFallbackException()
			: this(null)
		{
		}

		public EncoderFallbackException(string message)
			: base(message)
		{
		}

		public EncoderFallbackException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal EncoderFallbackException(char charUnknown, int index)
			: base(null)
		{
			char_unknown = charUnknown;
			this.index = index;
		}

		internal EncoderFallbackException(char charUnknownHigh, char charUnknownLow, int index)
			: base(null)
		{
			char_unknown_high = charUnknownHigh;
			char_unknown_low = charUnknownLow;
			this.index = index;
		}

		[MonoTODO]
		public bool IsUnknownSurrogate()
		{
			throw new NotImplementedException();
		}
	}
}
