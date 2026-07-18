using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.IO
{
	[IODescription("")]
	[DefaultEvent("Changed")]
	public class FileSystemWatcher : Component, ISupportInitialize
	{
		private enum EventType
		{
			FileSystemEvent = 0,
			ErrorEvent = 1,
			RenameEvent = 2
		}

		private bool enableRaisingEvents;

		private string filter;

		private bool includeSubdirectories;

		private int internalBufferSize;

		private NotifyFilters notifyFilter;

		private string path;

		private string fullpath;

		private ISynchronizeInvoke synchronizingObject;

		private WaitForChangedResult lastData;

		private bool waiting;

		private System.IO.SearchPattern2 pattern;

		private bool disposed;

		private string mangledFilter;

		private static System.IO.IFileWatcher watcher;

		private static object lockobj = new object();

		internal bool Waiting
		{
			get
			{
				return waiting;
			}
			set
			{
				waiting = value;
			}
		}

		internal string MangledFilter
		{
			get
			{
				if (filter != "*.*")
				{
					return filter;
				}
				if (mangledFilter != null)
				{
					return mangledFilter;
				}
				string result = "*.*";
				if (watcher.GetType() != typeof(System.IO.WindowsWatcher))
				{
					result = "*";
				}
				return result;
			}
		}

		internal System.IO.SearchPattern2 Pattern
		{
			get
			{
				if (pattern == null)
				{
					pattern = new System.IO.SearchPattern2(MangledFilter);
				}
				return pattern;
			}
		}

		internal string FullPath
		{
			get
			{
				if (fullpath == null)
				{
					if (path == null || path == string.Empty)
					{
						fullpath = Environment.CurrentDirectory;
					}
					else
					{
						fullpath = System.IO.Path.GetFullPath(path);
					}
				}
				return fullpath;
			}
		}

		[DefaultValue(false)]
		[IODescription("Flag to indicate if this instance is active")]
		public bool EnableRaisingEvents
		{
			get
			{
				return enableRaisingEvents;
			}
			set
			{
				if (value != enableRaisingEvents)
				{
					enableRaisingEvents = value;
					if (value)
					{
						Start();
					}
					else
					{
						Stop();
					}
				}
			}
		}

		[RecommendedAsConfigurable(true)]
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue("*.*")]
		[IODescription("File name filter pattern")]
		public string Filter
		{
			get
			{
				return filter;
			}
			set
			{
				if (value == null || value == string.Empty)
				{
					value = "*.*";
				}
				if (filter != value)
				{
					filter = value;
					pattern = null;
					mangledFilter = null;
				}
			}
		}

		[IODescription("Flag to indicate we want to watch subdirectories")]
		[DefaultValue(false)]
		public bool IncludeSubdirectories
		{
			get
			{
				return includeSubdirectories;
			}
			set
			{
				if (includeSubdirectories != value)
				{
					includeSubdirectories = value;
					if (value && enableRaisingEvents)
					{
						Stop();
						Start();
					}
				}
			}
		}

		[DefaultValue(8192)]
		[Browsable(false)]
		public int InternalBufferSize
		{
			get
			{
				return internalBufferSize;
			}
			set
			{
				if (internalBufferSize != value)
				{
					if (value < 4196)
					{
						value = 4196;
					}
					internalBufferSize = value;
					if (enableRaisingEvents)
					{
						Stop();
						Start();
					}
				}
			}
		}

		[DefaultValue(NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite)]
		[IODescription("Flag to indicate which change event we want to monitor")]
		public NotifyFilters NotifyFilter
		{
			get
			{
				return notifyFilter;
			}
			set
			{
				if (notifyFilter != value)
				{
					notifyFilter = value;
					if (enableRaisingEvents)
					{
						Stop();
						Start();
					}
				}
			}
		}

		[IODescription("The directory to monitor")]
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue("")]
		[Editor("System.Diagnostics.Design.FSWPathEditor, System.Design, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RecommendedAsConfigurable(true)]
		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				if (!(path == value))
				{
					bool flag = false;
					Exception ex = null;
					try
					{
						flag = Directory.Exists(value);
					}
					catch (Exception ex2)
					{
						ex = ex2;
					}
					if (ex != null)
					{
						throw new ArgumentException("Invalid directory name", "value", ex);
					}
					if (!flag)
					{
						throw new ArgumentException("Directory does not exists", "value");
					}
					path = value;
					fullpath = null;
					if (enableRaisingEvents)
					{
						Stop();
						Start();
					}
				}
			}
		}

		[Browsable(false)]
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
			}
		}

		[IODescription("The object used to marshal the event handler calls resulting from a directory change")]
		[DefaultValue(null)]
		[Browsable(false)]
		public ISynchronizeInvoke SynchronizingObject
		{
			get
			{
				return synchronizingObject;
			}
			set
			{
				synchronizingObject = value;
			}
		}

		[IODescription("Occurs when a file/directory change matches the filter")]
		public event FileSystemEventHandler Changed;

		[IODescription("Occurs when a file/directory creation matches the filter")]
		public event FileSystemEventHandler Created;

		[IODescription("Occurs when a file/directory deletion matches the filter")]
		public event FileSystemEventHandler Deleted;

		[Browsable(false)]
		public event ErrorEventHandler Error;

		[IODescription("Occurs when a file/directory rename matches the filter")]
		public event RenamedEventHandler Renamed;

		public FileSystemWatcher()
		{
			notifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
			enableRaisingEvents = false;
			filter = "*.*";
			includeSubdirectories = false;
			internalBufferSize = 8192;
			path = string.Empty;
			InitWatcher();
		}

		public FileSystemWatcher(string path)
			: this(path, "*.*")
		{
		}

		public FileSystemWatcher(string path, string filter)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (path == string.Empty)
			{
				throw new ArgumentException("Empty path", "path");
			}
			if (!Directory.Exists(path))
			{
				throw new ArgumentException("Directory does not exists", "path");
			}
			enableRaisingEvents = false;
			this.filter = filter;
			includeSubdirectories = false;
			internalBufferSize = 8192;
			notifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
			this.path = path;
			synchronizingObject = null;
			InitWatcher();
		}

		private void InitWatcher()
		{
			lock (lockobj)
			{
				if (watcher != null)
				{
					return;
				}
				string environmentVariable = Environment.GetEnvironmentVariable("MONO_MANAGED_WATCHER");
				int num = 0;
				if (environmentVariable == null)
				{
					num = InternalSupportsFSW();
				}
				bool flag = false;
				switch (num)
				{
				case 1:
					flag = System.IO.DefaultWatcher.GetInstance(out watcher);
					break;
				case 2:
					flag = System.IO.FAMWatcher.GetInstance(out watcher, false);
					break;
				case 3:
					flag = System.IO.KeventWatcher.GetInstance(out watcher);
					break;
				case 4:
					flag = System.IO.FAMWatcher.GetInstance(out watcher, true);
					break;
				case 5:
					flag = System.IO.InotifyWatcher.GetInstance(out watcher, true);
					break;
				}
				if (num == 0 || !flag)
				{
					if (string.Compare(environmentVariable, "disabled", true) == 0)
					{
						System.IO.NullFileWatcher.GetInstance(out watcher);
					}
					else
					{
						System.IO.DefaultWatcher.GetInstance(out watcher);
					}
				}
			}
		}

		[Conditional("DEBUG")]
		[Conditional("TRACE")]
		private void ShowWatcherInfo()
		{
			Console.WriteLine("Watcher implementation: {0}", (watcher == null) ? "<none>" : watcher.GetType().ToString());
		}

		public void BeginInit()
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				Stop();
			}
			base.Dispose(disposing);
		}

		~FileSystemWatcher()
		{
			disposed = true;
			Stop();
		}

		public void EndInit()
		{
		}

		private void RaiseEvent(Delegate ev, EventArgs arg, EventType evtype)
		{
			if ((object)ev == null)
			{
				return;
			}
			if (synchronizingObject == null)
			{
				switch (evtype)
				{
				case EventType.RenameEvent:
					((RenamedEventHandler)ev).BeginInvoke(this, (RenamedEventArgs)arg, null, null);
					break;
				case EventType.ErrorEvent:
					((ErrorEventHandler)ev).BeginInvoke(this, (ErrorEventArgs)arg, null, null);
					break;
				case EventType.FileSystemEvent:
					((FileSystemEventHandler)ev).BeginInvoke(this, (FileSystemEventArgs)arg, null, null);
					break;
				}
			}
			else
			{
				synchronizingObject.BeginInvoke(ev, new object[2] { this, arg });
			}
		}

		protected void OnChanged(FileSystemEventArgs e)
		{
			RaiseEvent(this.Changed, e, EventType.FileSystemEvent);
		}

		protected void OnCreated(FileSystemEventArgs e)
		{
			RaiseEvent(this.Created, e, EventType.FileSystemEvent);
		}

		protected void OnDeleted(FileSystemEventArgs e)
		{
			RaiseEvent(this.Deleted, e, EventType.FileSystemEvent);
		}

		protected void OnError(ErrorEventArgs e)
		{
			RaiseEvent(this.Error, e, EventType.ErrorEvent);
		}

		protected void OnRenamed(RenamedEventArgs e)
		{
			RaiseEvent(this.Renamed, e, EventType.RenameEvent);
		}

		public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
		{
			return WaitForChanged(changeType, -1);
		}

		public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
		{
			WaitForChangedResult result = default(WaitForChangedResult);
			bool flag = EnableRaisingEvents;
			if (!flag)
			{
				EnableRaisingEvents = true;
			}
			bool flag2;
			lock (this)
			{
				waiting = true;
				flag2 = Monitor.Wait(this, timeout);
				if (flag2)
				{
					result = lastData;
				}
			}
			EnableRaisingEvents = flag;
			if (!flag2)
			{
				result.TimedOut = true;
			}
			return result;
		}

		internal void DispatchEvents(System.IO.FileAction act, string filename, ref RenamedEventArgs renamed)
		{
			if (waiting)
			{
				lastData = default(WaitForChangedResult);
			}
			switch (act)
			{
			case System.IO.FileAction.Added:
				lastData.Name = filename;
				lastData.ChangeType = WatcherChangeTypes.Created;
				OnCreated(new FileSystemEventArgs(WatcherChangeTypes.Created, path, filename));
				break;
			case System.IO.FileAction.Removed:
				lastData.Name = filename;
				lastData.ChangeType = WatcherChangeTypes.Deleted;
				OnDeleted(new FileSystemEventArgs(WatcherChangeTypes.Deleted, path, filename));
				break;
			case System.IO.FileAction.Modified:
				lastData.Name = filename;
				lastData.ChangeType = WatcherChangeTypes.Changed;
				OnChanged(new FileSystemEventArgs(WatcherChangeTypes.Changed, path, filename));
				break;
			case System.IO.FileAction.RenamedOldName:
				if (renamed != null)
				{
					OnRenamed(renamed);
				}
				lastData.OldName = filename;
				lastData.ChangeType = WatcherChangeTypes.Renamed;
				renamed = new RenamedEventArgs(WatcherChangeTypes.Renamed, path, filename, string.Empty);
				break;
			case System.IO.FileAction.RenamedNewName:
				lastData.Name = filename;
				lastData.ChangeType = WatcherChangeTypes.Renamed;
				if (renamed == null)
				{
					renamed = new RenamedEventArgs(WatcherChangeTypes.Renamed, path, string.Empty, filename);
				}
				OnRenamed(renamed);
				renamed = null;
				break;
			}
		}

		private void Start()
		{
			watcher.StartDispatching(this);
		}

		private void Stop()
		{
			watcher.StopDispatching(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int InternalSupportsFSW();
	}
}
