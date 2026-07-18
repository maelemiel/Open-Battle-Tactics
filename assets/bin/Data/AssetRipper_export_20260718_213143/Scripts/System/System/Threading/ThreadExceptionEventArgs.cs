namespace System.Threading
{
	public class ThreadExceptionEventArgs : EventArgs
	{
		private Exception exception;

		public Exception Exception
		{
			get
			{
				return exception;
			}
		}

		public ThreadExceptionEventArgs(Exception t)
		{
			exception = t;
		}
	}
}
