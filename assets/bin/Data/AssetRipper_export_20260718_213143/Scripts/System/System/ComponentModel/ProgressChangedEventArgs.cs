namespace System.ComponentModel
{
	public class ProgressChangedEventArgs : EventArgs
	{
		private int progress;

		private object state;

		public int ProgressPercentage
		{
			get
			{
				return progress;
			}
		}

		public object UserState
		{
			get
			{
				return state;
			}
		}

		public ProgressChangedEventArgs(int progressPercentage, object userState)
		{
			progress = progressPercentage;
			state = userState;
		}
	}
}
