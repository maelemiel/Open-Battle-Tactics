namespace System.Text.RegularExpressions
{
	internal struct Mark
	{
		public int Start;

		public int End;

		public int Previous;

		public bool IsDefined
		{
			get
			{
				return Start >= 0 && End >= 0;
			}
		}

		public int Index
		{
			get
			{
				return (Start >= End) ? End : Start;
			}
		}

		public int Length
		{
			get
			{
				return (Start >= End) ? (Start - End) : (End - Start);
			}
		}
	}
}
