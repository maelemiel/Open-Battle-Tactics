namespace System.Net.Sockets
{
	public class LingerOption
	{
		private bool enabled;

		private int seconds;

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public int LingerTime
		{
			get
			{
				return seconds;
			}
			set
			{
				seconds = value;
			}
		}

		public LingerOption(bool enable, int secs)
		{
			enabled = enable;
			seconds = secs;
		}
	}
}
