namespace System.ComponentModel
{
	public class DoWorkEventArgs : EventArgs
	{
		private object arg;

		private object result;

		private bool cancel;

		public object Argument
		{
			get
			{
				return arg;
			}
		}

		public object Result
		{
			get
			{
				return result;
			}
			set
			{
				result = value;
			}
		}

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

		public DoWorkEventArgs(object argument)
		{
			arg = argument;
		}
	}
}
