namespace System.Text
{
	[Serializable]
	public abstract class DecoderFallback
	{
		private static DecoderFallback exception_fallback = new DecoderExceptionFallback();

		private static DecoderFallback replacement_fallback = new DecoderReplacementFallback();

		private static DecoderFallback standard_safe_fallback = new DecoderReplacementFallback("\ufffd");

		public static DecoderFallback ExceptionFallback
		{
			get
			{
				return exception_fallback;
			}
		}

		public abstract int MaxCharCount { get; }

		public static DecoderFallback ReplacementFallback
		{
			get
			{
				return replacement_fallback;
			}
		}

		internal static DecoderFallback StandardSafeFallback
		{
			get
			{
				return standard_safe_fallback;
			}
		}

		public abstract DecoderFallbackBuffer CreateFallbackBuffer();
	}
}
