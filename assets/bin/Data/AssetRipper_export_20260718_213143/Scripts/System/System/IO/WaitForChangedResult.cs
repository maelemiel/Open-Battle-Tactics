namespace System.IO
{
	public struct WaitForChangedResult
	{
		private WatcherChangeTypes changeType;

		private string name;

		private string oldName;

		private bool timedOut;

		public WatcherChangeTypes ChangeType
		{
			get
			{
				return changeType;
			}
			set
			{
				changeType = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public string OldName
		{
			get
			{
				return oldName;
			}
			set
			{
				oldName = value;
			}
		}

		public bool TimedOut
		{
			get
			{
				return timedOut;
			}
			set
			{
				timedOut = value;
			}
		}
	}
}
