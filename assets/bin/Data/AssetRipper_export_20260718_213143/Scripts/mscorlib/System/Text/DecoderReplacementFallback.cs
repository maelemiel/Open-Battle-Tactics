namespace System.Text
{
	[Serializable]
	public sealed class DecoderReplacementFallback : DecoderFallback
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

		public DecoderReplacementFallback()
			: this("?")
		{
		}

		[MonoTODO]
		public DecoderReplacementFallback(string replacement)
		{
			if (replacement == null)
			{
				throw new ArgumentNullException();
			}
			this.replacement = replacement;
		}

		public override DecoderFallbackBuffer CreateFallbackBuffer()
		{
			return new DecoderReplacementFallbackBuffer(this);
		}

		public override bool Equals(object value)
		{
			DecoderReplacementFallback decoderReplacementFallback = value as DecoderReplacementFallback;
			return decoderReplacementFallback != null && replacement == decoderReplacementFallback.replacement;
		}

		public override int GetHashCode()
		{
			return replacement.GetHashCode();
		}
	}
}
