using System.Collections;
using System.Threading;

namespace System.IO
{
	internal class DefaultWatcher : System.IO.IFileWatcher
	{
		private static System.IO.DefaultWatcher instance;

		private static Thread thread;

		private static Hashtable watches;

		private static string[] NoStringsArray = new string[0];

		private DefaultWatcher()
		{
		}

		public static bool GetInstance(out System.IO.IFileWatcher watcher)
		{
			if (instance != null)
			{
				watcher = instance;
				return true;
			}
			instance = new System.IO.DefaultWatcher();
			watcher = instance;
			return true;
		}

		public void StartDispatching(FileSystemWatcher fsw)
		{
			lock (this)
			{
				if (watches == null)
				{
					watches = new Hashtable();
				}
				if (thread == null)
				{
					thread = new Thread(Monitor);
					thread.IsBackground = true;
					thread.Start();
				}
			}
			lock (watches)
			{
				System.IO.DefaultWatcherData defaultWatcherData = (System.IO.DefaultWatcherData)watches[fsw];
				if (defaultWatcherData == null)
				{
					defaultWatcherData = new System.IO.DefaultWatcherData();
					defaultWatcherData.Files = new Hashtable();
					watches[fsw] = defaultWatcherData;
				}
				defaultWatcherData.FSW = fsw;
				defaultWatcherData.Directory = fsw.FullPath;
				defaultWatcherData.NoWildcards = !fsw.Pattern.HasWildcard;
				if (defaultWatcherData.NoWildcards)
				{
					defaultWatcherData.FileMask = Path.Combine(defaultWatcherData.Directory, fsw.MangledFilter);
				}
				else
				{
					defaultWatcherData.FileMask = fsw.MangledFilter;
				}
				defaultWatcherData.IncludeSubdirs = fsw.IncludeSubdirectories;
				defaultWatcherData.Enabled = true;
				defaultWatcherData.DisabledTime = DateTime.MaxValue;
				UpdateDataAndDispatch(defaultWatcherData, false);
			}
		}

		public void StopDispatching(FileSystemWatcher fsw)
		{
			lock (this)
			{
				if (watches == null)
				{
					return;
				}
			}
			lock (watches)
			{
				System.IO.DefaultWatcherData defaultWatcherData = (System.IO.DefaultWatcherData)watches[fsw];
				if (defaultWatcherData != null)
				{
					defaultWatcherData.Enabled = false;
					defaultWatcherData.DisabledTime = DateTime.Now;
				}
			}
		}

		private void Monitor()
		{
			int num = 0;
			while (true)
			{
				Thread.Sleep(750);
				Hashtable hashtable;
				lock (watches)
				{
					if (watches.Count == 0)
					{
						if (++num == 20)
						{
							break;
						}
						continue;
					}
					hashtable = (Hashtable)watches.Clone();
				}
				if (hashtable.Count == 0)
				{
					continue;
				}
				num = 0;
				foreach (System.IO.DefaultWatcherData value in hashtable.Values)
				{
					if (UpdateDataAndDispatch(value, true))
					{
						lock (watches)
						{
							watches.Remove(value.FSW);
						}
					}
				}
			}
			lock (this)
			{
				thread = null;
			}
		}

		private bool UpdateDataAndDispatch(System.IO.DefaultWatcherData data, bool dispatch)
		{
			if (!data.Enabled)
			{
				return data.DisabledTime != DateTime.MaxValue && (DateTime.Now - data.DisabledTime).TotalSeconds > 5.0;
			}
			DoFiles(data, data.Directory, dispatch);
			return false;
		}

		private static void DispatchEvents(FileSystemWatcher fsw, System.IO.FileAction action, string filename)
		{
			RenamedEventArgs renamed = null;
			lock (fsw)
			{
				fsw.DispatchEvents(action, filename, ref renamed);
				if (fsw.Waiting)
				{
					fsw.Waiting = false;
					System.Threading.Monitor.PulseAll(fsw);
				}
			}
		}

		private void DoFiles(System.IO.DefaultWatcherData data, string directory, bool dispatch)
		{
			bool flag = Directory.Exists(directory);
			if (flag && data.IncludeSubdirs)
			{
				string[] directories = Directory.GetDirectories(directory);
				foreach (string directory2 in directories)
				{
					DoFiles(data, directory2, dispatch);
				}
			}
			string[] array = null;
			array = ((!flag) ? NoStringsArray : ((!data.NoWildcards) ? Directory.GetFileSystemEntries(directory, data.FileMask) : ((!File.Exists(data.FileMask) && !Directory.Exists(data.FileMask)) ? NoStringsArray : new string[1] { data.FileMask })));
			foreach (string key4 in data.Files.Keys)
			{
				System.IO.FileData fileData = (System.IO.FileData)data.Files[key4];
				if (fileData.Directory == directory)
				{
					fileData.NotExists = true;
				}
			}
			string[] array2 = array;
			foreach (string text in array2)
			{
				System.IO.FileData fileData2 = (System.IO.FileData)data.Files[text];
				if (fileData2 == null)
				{
					try
					{
						data.Files.Add(text, CreateFileData(directory, text));
					}
					catch
					{
						data.Files.Remove(text);
						continue;
					}
					if (dispatch)
					{
						DispatchEvents(data.FSW, System.IO.FileAction.Added, text);
					}
				}
				else if (fileData2.Directory == directory)
				{
					fileData2.NotExists = false;
				}
			}
			if (!dispatch)
			{
				return;
			}
			ArrayList arrayList = null;
			foreach (string key5 in data.Files.Keys)
			{
				System.IO.FileData fileData3 = (System.IO.FileData)data.Files[key5];
				if (fileData3.NotExists)
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(key5);
					DispatchEvents(data.FSW, System.IO.FileAction.Removed, key5);
				}
			}
			if (arrayList != null)
			{
				foreach (string item in arrayList)
				{
					data.Files.Remove(item);
				}
				arrayList = null;
			}
			foreach (string key6 in data.Files.Keys)
			{
				System.IO.FileData fileData4 = (System.IO.FileData)data.Files[key6];
				DateTime creationTime;
				DateTime lastWriteTime;
				try
				{
					creationTime = File.GetCreationTime(key6);
					lastWriteTime = File.GetLastWriteTime(key6);
				}
				catch
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(key6);
					DispatchEvents(data.FSW, System.IO.FileAction.Removed, key6);
					continue;
				}
				if (creationTime != fileData4.CreationTime || lastWriteTime != fileData4.LastWriteTime)
				{
					fileData4.CreationTime = creationTime;
					fileData4.LastWriteTime = lastWriteTime;
					DispatchEvents(data.FSW, System.IO.FileAction.Modified, key6);
				}
			}
			if (arrayList == null)
			{
				return;
			}
			foreach (string item2 in arrayList)
			{
				data.Files.Remove(item2);
			}
		}

		private static System.IO.FileData CreateFileData(string directory, string filename)
		{
			System.IO.FileData fileData = new System.IO.FileData();
			string path = Path.Combine(directory, filename);
			fileData.Directory = directory;
			fileData.Attributes = File.GetAttributes(path);
			fileData.CreationTime = File.GetCreationTime(path);
			fileData.LastWriteTime = File.GetLastWriteTime(path);
			return fileData;
		}
	}
}
