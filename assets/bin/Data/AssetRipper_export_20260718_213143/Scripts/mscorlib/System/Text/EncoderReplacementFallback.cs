namespace System.Text
{
	[Serializable]
	public sealed class EncoderReplacementFallback : EncoderFallback
	{
		private string replacement;

		public string DefaultString
		{
			get
			{
				return replacement;
			}
		}

		public override int MaxCharCount
		{
			get
			{
				return replacement.Length;
			}
		}

		public EncoderReplacementFallback()
			: this("?")
		{
		}

		[MonoTODO]
		public EncoderReplacementFallback(string replacement)
		{
			if (replacement == null)
			{
				throw new ArgumentNullException();
			}
			this.replacement = replacement;
		}

		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			return new EncoderReplacementFallbackBuffer(this);
		}

		public override bool Equals(object value)
		{
			EncoderReplacementFallback encoderReplacementFallback = value as EncoderReplacementFallback;
			return encoderReplacementFallback != null && replacement == encoderReplacementFallback.replacement;
		}

		public override int GetHashCode()
		{
			return replacement.GetHashCode();
		}
	}
}
