namespace System.IO
{
	public class RenamedEventArgs : FileSystemEventArgs
	{
		private string oldName;

		private string oldFullPath;

		public string OldFullPath
		{
			get
			{
				return oldFullPath;
			}
		}

		public string OldName
		{
			get
			{
				return oldName;
			}
		}

		public RenamedEventArgs(WatcherChangeTypes changeType, string directory, string name, string oldName)
			: base(changeType, directory, name)
		{
			this.oldName = oldName;
			oldFullPath = Path.Combine(directory, oldName);
		}
	}
}
