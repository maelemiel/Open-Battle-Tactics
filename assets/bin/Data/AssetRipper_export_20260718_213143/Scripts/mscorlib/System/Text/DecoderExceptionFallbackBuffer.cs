namespace System.Text
{
	public sealed class DecoderExceptionFallbackBuffer : DecoderFallbackBuffer
	{
		public override int Remaining
		{
			get
			{
				return 0;
			}
		}

		public override bool Fallback(byte[] bytesUnknown, int index)
		{
			throw new DecoderFallbackException(null, bytesUnknown, index);
		}

		public override char GetNextChar()
		{
			return '\0';
		}

		public override bool MovePrevious()
		{
			return false;
		}
	}
}
