namespace System.IO
{
	internal class NullFileWatcher : System.IO.IFileWatcher
	{
		private static System.IO.IFileWatcher instance;

		public void StartDispatching(FileSystemWatcher fsw)
		{
		}

		public void StopDispatching(FileSystemWatcher fsw)
		{
		}

		public static bool GetInstance(out System.IO.IFileWatcher watcher)
		{
			if (instance != null)
			{
				watcher = instance;
				return true;
			}
			instance = (watcher = new System.IO.NullFileWatcher());
			return true;
		}
	}
}
