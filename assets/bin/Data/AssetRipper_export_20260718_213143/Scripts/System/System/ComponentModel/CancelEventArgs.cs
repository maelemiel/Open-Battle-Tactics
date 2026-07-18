namespace System.ComponentModel
{
	public class CancelEventArgs : EventArgs
	{
		private bool cancel;

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

		public CancelEventArgs()
		{
			cancel = false;
		}

		public CancelEventArgs(bool cancel)
		{
			this.cancel = cancel;
		}
	}
}
