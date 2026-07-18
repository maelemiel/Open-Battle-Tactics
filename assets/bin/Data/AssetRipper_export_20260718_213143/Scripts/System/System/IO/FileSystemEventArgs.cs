namespace System.IO
{
	public class FileSystemEventArgs : EventArgs
	{
		private WatcherChangeTypes changeType;

		private string directory;

		private string name;

		public WatcherChangeTypes ChangeType
		{
			get
			{
				return changeType;
			}
		}

		public string FullPath
		{
			get
			{
				return Path.Combine(directory, name);
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public FileSystemEventArgs(WatcherChangeTypes changeType, string directory, string name)
		{
			this.changeType = changeType;
			this.directory = directory;
			this.name = name;
		}

		internal void SetName(string name)
		{
			this.name = name;
		}
	}
}
