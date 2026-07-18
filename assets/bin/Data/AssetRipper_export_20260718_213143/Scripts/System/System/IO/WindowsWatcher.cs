namespace System.IO
{
	internal class WindowsWatcher : System.IO.IFileWatcher
	{
		private WindowsWatcher()
		{
		}

		public static bool GetInstance(out System.IO.IFileWatcher watcher)
		{
			throw new NotSupportedException();
		}

		public void StartDispatching(FileSystemWatcher fsw)
		{
		}

		public void StopDispatching(FileSystemWatcher fsw)
		{
		}
	}
}
