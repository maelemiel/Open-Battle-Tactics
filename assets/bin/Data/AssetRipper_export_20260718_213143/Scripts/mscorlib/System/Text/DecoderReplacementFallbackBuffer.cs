namespace System.Text
{
	public sealed class DecoderReplacementFallbackBuffer : DecoderFallbackBuffer
	{
		private bool fallback_assigned;

		private int current;

		private string replacement;

		public override int Remaining
		{
			get
			{
				return fallback_assigned ? (replacement.Length - current) : 0;
			}
		}

		public DecoderReplacementFallbackBuffer(DecoderReplacementFallback fallback)
		{
			if (fallback == null)
			{
				throw new ArgumentNullException("fallback");
			}
			replacement = fallback.DefaultString;
			current = 0;
		}

		public override bool Fallback(byte[] bytesUnknown, int index)
		{
			if (bytesUnknown == null)
			{
				throw new ArgumentNullException("bytesUnknown");
			}
			if (fallback_assigned && Remaining != 0)
			{
				throw new ArgumentException("Reentrant Fallback method invocation occured. It might be because either this FallbackBuffer is incorrectly shared by multiple threads, invoked inside Encoding recursively, or Reset invocation is forgotten.");
			}
			if (index < 0 || bytesUnknown.Length < index)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			fallback_assigned = true;
			current = 0;
			return replacement.Length > 0;
		}

		public override char GetNextChar()
		{
			if (!fallback_assigned)
			{
				return '\0';
			}
			if (current >= replacement.Length)
			{
				return '\0';
			}
			return replacement[current++];
		}

		public override bool MovePrevious()
		{
			if (current == 0)
			{
				return false;
			}
			current--;
			return true;
		}

		public override void Reset()
		{
			fallback_assigned = false;
			current = 0;
		}
	}
}
