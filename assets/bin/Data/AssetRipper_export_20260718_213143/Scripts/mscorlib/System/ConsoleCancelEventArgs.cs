namespace System
{
	[Serializable]
	public sealed class ConsoleCancelEventArgs : EventArgs
	{
		private bool cancel;

		private ConsoleSpecialKey specialKey;

		public bool Cancel
		{
			get
			{
				return cancel;
			}
			set
			{
				cancel = value;
			}
		}

		public ConsoleSpecialKey SpecialKey
		{
			get
			{
				return specialKey;
			}
		}

		internal ConsoleCancelEventArgs(ConsoleSpecialKey key)
		{
			specialKey = key;
		}
	}
}
