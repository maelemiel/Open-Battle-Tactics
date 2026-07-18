namespace System.Text
{
	[Serializable]
	public sealed class DecoderExceptionFallback : DecoderFallback
	{
		public override int MaxCharCount
		{
			get
			{
				return 0;
			}
		}

		public override DecoderFallbackBuffer CreateFallbackBuffer()
		{
			return new DecoderExceptionFallbackBuffer();
		}

		public override bool Equals(object value)
		{
			return value is DecoderExceptionFallback;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
