namespace System.IO
{
	public class ErrorEventArgs : EventArgs
	{
		private Exception exception;

		public ErrorEventArgs(Exception exception)
		{
			this.exception = exception;
		}

		public virtual Exception GetException()
		{
			return exception;
		}
	}
}
