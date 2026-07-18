using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.IO
{
	internal class KeventWatcher : System.IO.IFileWatcher
	{
		private static bool failed;

		private static System.IO.KeventWatcher instance;

		private static Hashtable watches;

		private static Hashtable requests;

		private static Thread thread;

		private static int conn;

		private static bool stop;

		private KeventWatcher()
		{
		}

		public static bool GetInstance(out System.IO.IFileWatcher watcher)
		{
			if (failed)
			{
				watcher = null;
				return false;
			}
			if (instance != null)
			{
				watcher = instance;
				return true;
			}
			watches = Hashtable.Synchronized(new Hashtable());
			requests = Hashtable.Synchronized(new Hashtable());
			conn = kqueue();
			if (conn == -1)
			{
				failed = true;
				watcher = null;
				return false;
			}
			instance = new System.IO.KeventWatcher();
			watcher = instance;
			return true;
		}

		public void StartDispatching(FileSystemWatcher fsw)
		{
			System.IO.KeventData keventData;
			lock (this)
			{
				if (thread == null)
				{
					thread = new Thread(Monitor);
					thread.IsBackground = true;
					thread.Start();
				}
				keventData = (System.IO.KeventData)watches[fsw];
			}
			if (keventData == null)
			{
				keventData = new System.IO.KeventData();
				keventData.FSW = fsw;
				keventData.Directory = fsw.FullPath;
				keventData.FileMask = fsw.MangledFilter;
				keventData.IncludeSubdirs = fsw.IncludeSubdirectories;
				keventData.Enabled = true;
				lock (this)
				{
					StartMonitoringDirectory(keventData);
					watches[fsw] = keventData;
					stop = false;
				}
			}
		}

		private static void StartMonitoringDirectory(System.IO.KeventData data)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(data.Directory);
			if (data.DirEntries == null)
			{
				data.DirEntries = new Hashtable();
				FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
				foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
				{
					data.DirEntries.Add(fileSystemInfo.FullName, new System.IO.KeventFileData(fileSystemInfo, fileSystemInfo.LastAccessTime, fileSystemInfo.LastWriteTime));
				}
			}
			int num = open(data.Directory, 0, 0);
			System.IO.kevent ev = new System.IO.kevent
			{
				udata = IntPtr.Zero
			};
			System.IO.timespec ts = new System.IO.timespec
			{
				tv_sec = 0,
				tv_usec = 0
			};
			if (num > 0)
			{
				ev.ident = num;
				ev.filter = -4;
				ev.flags = 21;
				ev.fflags = 31u;
				ev.data = 0;
				ev.udata = Marshal.StringToHGlobalAuto(data.Directory);
				System.IO.kevent evtlist = new System.IO.kevent
				{
					udata = IntPtr.Zero
				};
				kevent(conn, ref ev, 1, ref evtlist, 0, ref ts);
				data.ev = ev;
				requests[num] = data;
			}
			if (data.IncludeSubdirs)
			{
			}
		}

		public void StopDispatching(FileSystemWatcher fsw)
		{
			lock (this)
			{
				System.IO.KeventData keventData = (System.IO.KeventData)watches[fsw];
				if (keventData != null)
				{
					StopMonitoringDirectory(keventData);
					watches.Remove(fsw);
					if (watches.Count == 0)
					{
						stop = true;
					}
					if (keventData.IncludeSubdirs)
					{
					}
				}
			}
		}

		private static void StopMonitoringDirectory(System.IO.KeventData data)
		{
			close(data.ev.ident);
		}

		private void Monitor()
		{
			while (!stop)
			{
				System.IO.kevent evtlist = new System.IO.kevent
				{
					udata = IntPtr.Zero
				};
				System.IO.kevent ev = new System.IO.kevent
				{
					udata = IntPtr.Zero
				};
				System.IO.timespec ts = new System.IO.timespec
				{
					tv_sec = 0,
					tv_usec = 0
				};
				int num;
				lock (this)
				{
					num = kevent(conn, ref ev, 0, ref evtlist, 1, ref ts);
				}
				if (num > 0)
				{
					System.IO.KeventData data = (System.IO.KeventData)requests[evtlist.ident];
					StopMonitoringDirectory(data);
					StartMonitoringDirectory(data);
					ProcessEvent(evtlist);
				}
				else
				{
					Thread.Sleep(500);
				}
			}
			lock (this)
			{
				thread = null;
				stop = false;
			}
		}

		private void ProcessEvent(System.IO.kevent ev)
		{
			lock (this)
			{
				System.IO.KeventData keventData = (System.IO.KeventData)requests[ev.ident];
				if (!keventData.Enabled)
				{
					return;
				}
				string empty = string.Empty;
				FileSystemWatcher fSW = keventData.FSW;
				System.IO.FileAction fileAction = (System.IO.FileAction)0;
				DirectoryInfo directoryInfo = new DirectoryInfo(keventData.Directory);
				FileSystemInfo changedFsi = null;
				try
				{
					FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
					foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
					{
						if (!keventData.DirEntries.ContainsKey(fileSystemInfo.FullName) || !(fileSystemInfo is FileInfo))
						{
							continue;
						}
						System.IO.KeventFileData keventFileData = (System.IO.KeventFileData)keventData.DirEntries[fileSystemInfo.FullName];
						if (keventFileData.LastWriteTime != fileSystemInfo.LastWriteTime)
						{
							empty = fileSystemInfo.Name;
							fileAction = System.IO.FileAction.Modified;
							keventData.DirEntries[fileSystemInfo.FullName] = new System.IO.KeventFileData(fileSystemInfo, fileSystemInfo.LastAccessTime, fileSystemInfo.LastWriteTime);
							if (fSW.IncludeSubdirectories && fileSystemInfo is DirectoryInfo)
							{
								keventData.Directory = empty;
								requests[ev.ident] = keventData;
								ProcessEvent(ev);
							}
							PostEvent(empty, fSW, fileAction, changedFsi);
						}
					}
				}
				catch (Exception)
				{
				}
				try
				{
					bool flag = true;
					while (flag)
					{
						foreach (System.IO.KeventFileData value in keventData.DirEntries.Values)
						{
							if (!File.Exists(value.fsi.FullName) && !Directory.Exists(value.fsi.FullName))
							{
								empty = value.fsi.Name;
								fileAction = System.IO.FileAction.Removed;
								keventData.DirEntries.Remove(value.fsi.FullName);
								PostEvent(empty, fSW, fileAction, changedFsi);
								break;
							}
						}
						flag = false;
					}
				}
				catch (Exception)
				{
				}
				try
				{
					FileSystemInfo[] fileSystemInfos2 = directoryInfo.GetFileSystemInfos();
					foreach (FileSystemInfo fileSystemInfo2 in fileSystemInfos2)
					{
						if (!keventData.DirEntries.ContainsKey(fileSystemInfo2.FullName))
						{
							changedFsi = fileSystemInfo2;
							empty = fileSystemInfo2.Name;
							fileAction = System.IO.FileAction.Added;
							keventData.DirEntries[fileSystemInfo2.FullName] = new System.IO.KeventFileData(fileSystemInfo2, fileSystemInfo2.LastAccessTime, fileSystemInfo2.LastWriteTime);
							PostEvent(empty, fSW, fileAction, changedFsi);
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}

		private void PostEvent(string filename, FileSystemWatcher fsw, System.IO.FileAction fa, FileSystemInfo changedFsi)
		{
			RenamedEventArgs renamed = null;
			if (fa == (System.IO.FileAction)0)
			{
				return;
			}
			if (fsw.IncludeSubdirectories && fa == System.IO.FileAction.Added && changedFsi is DirectoryInfo)
			{
				System.IO.KeventData keventData = new System.IO.KeventData();
				keventData.FSW = fsw;
				keventData.Directory = changedFsi.FullName;
				keventData.FileMask = fsw.MangledFilter;
				keventData.IncludeSubdirs = fsw.IncludeSubdirectories;
				keventData.Enabled = true;
				lock (this)
				{
					StartMonitoringDirectory(keventData);
				}
			}
			if (!fsw.Pattern.IsMatch(filename, true))
			{
				return;
			}
			lock (fsw)
			{
				fsw.DispatchEvents(fa, filename, ref renamed);
				if (fsw.Waiting)
				{
					fsw.Waiting = false;
					System.Threading.Monitor.PulseAll(fsw);
				}
			}
		}

		[DllImport("libc")]
		private static extern int open(string path, int flags, int mode_t);

		[DllImport("libc")]
		private static extern int close(int fd);

		[DllImport("libc")]
		private static extern int kqueue();

		[DllImport("libc")]
		private static extern int kevent(int kqueue, ref System.IO.kevent ev, int nchanges, ref System.IO.kevent evtlist, int nevents, ref System.IO.timespec ts);
	}
}
