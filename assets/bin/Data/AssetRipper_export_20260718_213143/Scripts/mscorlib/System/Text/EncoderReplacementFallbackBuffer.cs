namespace System.Text
{
	public sealed class EncoderReplacementFallbackBuffer : EncoderFallbackBuffer
	{
		private string replacement;

		private int current;

		private bool fallback_assigned;

		public override int Remaining
		{
			get
			{
				return replacement.Length - current;
			}
		}

		public EncoderReplacementFallbackBuffer(EncoderReplacementFallback fallback)
		{
			if (fallback == null)
			{
				throw new ArgumentNullException("fallback");
			}
			replacement = fallback.DefaultString;
			current = 0;
		}

		public override bool Fallback(char charUnknown, int index)
		{
			return Fallback(index);
		}

		public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
		{
			return Fallback(index);
		}

		private bool Fallback(int index)
		{
			if (fallback_assigned && Remaining != 0)
			{
				throw new ArgumentException("Reentrant Fallback method invocation occured. It might be because either this FallbackBuffer is incorrectly shared by multiple threads, invoked inside Encoding recursively, or Reset invocation is forgotten.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			fallback_assigned = true;
			current = 0;
			return replacement.Length > 0;
		}

		public override char GetNextChar()
		{
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
			current = 0;
		}
	}
}
