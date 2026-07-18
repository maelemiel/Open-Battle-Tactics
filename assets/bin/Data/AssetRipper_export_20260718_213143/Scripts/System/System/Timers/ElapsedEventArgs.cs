namespace System.Timers
{
	public class ElapsedEventArgs : EventArgs
	{
		private DateTime time;

		public DateTime SignalTime
		{
			get
			{
				return time;
			}
		}

		internal ElapsedEventArgs(DateTime time)
		{
			this.time = time;
		}
	}
}
