namespace System.ComponentModel
{
	public class RunWorkerCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object result;

		public object Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return result;
			}
		}

		public new object UserState
		{
			get
			{
				return null;
			}
		}

		public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled)
			: base(error, cancelled, null)
		{
			this.result = result;
		}
	}
}
