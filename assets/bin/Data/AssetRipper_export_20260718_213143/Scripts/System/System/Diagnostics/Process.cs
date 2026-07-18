using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace System.Diagnostics
{
	[Designer("System.Diagnostics.Design.ProcessDesigner, System.Design, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultEvent("Exited")]
	[DefaultProperty("StartInfo")]
	[MonitoringDescription("Represents a system process")]
	public class Process : Component
	{
		private struct ProcInfo
		{
			public IntPtr process_handle;

			public IntPtr thread_handle;

			public int pid;

			public int tid;

			public string[] envKeys;

			public string[] envValues;

			public string UserName;

			public string Domain;

			public IntPtr Password;

			public bool LoadUserProfile;
		}

		[Flags]
		private enum AsyncModes
		{
			NoneYet = 0,
			SyncOutput = 1,
			SyncError = 2,
			AsyncOutput = 4,
			AsyncError = 8
		}

		[StructLayout(LayoutKind.Sequential)]
		private sealed class ProcessAsyncReader
		{
			public object Sock;

			public IntPtr handle;

			public object state;

			public AsyncCallback callback;

			public ManualResetEvent wait_handle;

			public Exception delayedException;

			public object EndPoint;

			private byte[] buffer = new byte[4196];

			public int Offset;

			public int Size;

			public int SockFlags;

			public object AcceptSocket;

			public object[] Addresses;

			public int port;

			public object Buffers;

			public bool ReuseSocket;

			public object acc_socket;

			public int total;

			public bool completed_sync;

			private bool completed;

			private bool err_out;

			internal int error;

			public int operation = 8;

			public object ares;

			public int EndCalled;

			private Process process;

			private Stream stream;

			private StringBuilder sb = new StringBuilder();

			public AsyncReadHandler ReadHandler;

			public bool IsCompleted
			{
				get
				{
					return completed;
				}
			}

			public WaitHandle WaitHandle
			{
				get
				{
					lock (this)
					{
						if (wait_handle == null)
						{
							wait_handle = new ManualResetEvent(completed);
						}
						return wait_handle;
					}
				}
			}

			public ProcessAsyncReader(Process process, IntPtr handle, bool err_out)
			{
				this.process = process;
				this.handle = handle;
				stream = new FileStream(handle, FileAccess.Read, false);
				ReadHandler = AddInput;
				this.err_out = err_out;
			}

			public void AddInput()
			{
				lock (this)
				{
					int num = stream.Read(buffer, 0, buffer.Length);
					if (num == 0)
					{
						completed = true;
						if (wait_handle != null)
						{
							wait_handle.Set();
						}
						FlushLast();
						return;
					}
					try
					{
						sb.Append(Encoding.Default.GetString(buffer, 0, num));
					}
					catch
					{
						for (int i = 0; i < num; i++)
						{
							sb.Append((char)buffer[i]);
						}
					}
					Flush(false);
					ReadHandler.BeginInvoke(null, this);
				}
			}

			private void FlushLast()
			{
				Flush(true);
				if (err_out)
				{
					process.OnOutputDataReceived(null);
				}
				else
				{
					process.OnErrorDataReceived(null);
				}
			}

			private void Flush(bool last)
			{
				if (sb.Length == 0 || (err_out && process.output_canceled) || (!err_out && process.error_canceled))
				{
					return;
				}
				string text = sb.ToString();
				sb.Length = 0;
				string[] array = text.Split('\n');
				int num = array.Length;
				if (num == 0)
				{
					return;
				}
				for (int i = 0; i < num - 1; i++)
				{
					if (err_out)
					{
						process.OnOutputDataReceived(array[i]);
					}
					else
					{
						process.OnErrorDataReceived(array[i]);
					}
				}
				string text2 = array[num - 1];
				if (last || (num == 1 && text2 == string.Empty))
				{
					if (err_out)
					{
						process.OnOutputDataReceived(text2);
					}
					else
					{
						process.OnErrorDataReceived(text2);
					}
				}
				else
				{
					sb.Append(text2);
				}
			}

			public void Close()
			{
				stream.Close();
			}
		}

		private class ProcessWaitHandle : WaitHandle
		{
			public ProcessWaitHandle(IntPtr handle)
			{
				Handle = ProcessHandle_duplicate(handle);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr ProcessHandle_duplicate(IntPtr handle);
		}

		private delegate void AsyncReadHandler();

		private IntPtr process_handle;

		private int pid;

		private bool enableRaisingEvents;

		private bool already_waiting;

		private ISynchronizeInvoke synchronizingObject;

		private EventHandler exited_event;

		private IntPtr stdout_rd;

		private IntPtr stderr_rd;

		private ProcessModuleCollection module_collection;

		private string process_name;

		private StreamReader error_stream;

		private StreamWriter input_stream;

		private StreamReader output_stream;

		private ProcessStartInfo start_info;

		private AsyncModes async_mode;

		private bool output_canceled;

		private bool error_canceled;

		private ProcessAsyncReader async_output;

		private ProcessAsyncReader async_error;

		private bool disposed;

		[MonitoringDescription("Base process priority.")]
		[System.MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int BasePriority
		{
			get
			{
				return 0;
			}
		}

		[MonitoringDescription("Check for exiting of the process to raise the apropriate event.")]
		[Browsable(false)]
		[DefaultValue(false)]
		public bool EnableRaisingEvents
		{
			get
			{
				return enableRaisingEvents;
			}
			set
			{
				bool flag = enableRaisingEvents;
				enableRaisingEvents = value;
				if (enableRaisingEvents && !flag)
				{
					StartExitCallbackIfNeeded();
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The exit code of the process.")]
		public int ExitCode
		{
			get
			{
				if (process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				int num = ExitCode_internal(process_handle);
				if (num == 259)
				{
					throw new InvalidOperationException("The process must exit before getting the requested information.");
				}
				return num;
			}
		}

		[MonitoringDescription("The exit time of the process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DateTime ExitTime
		{
			get
			{
				if (process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				if (!HasExited)
				{
					throw new InvalidOperationException("The process must exit before getting the requested information.");
				}
				return DateTime.FromFileTime(ExitTime_internal(process_handle));
			}
		}

		[MonitoringDescription("Handle for this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public IntPtr Handle
		{
			get
			{
				return process_handle;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.MonoTODO]
		[MonitoringDescription("Handles for this process.")]
		public int HandleCount
		{
			get
			{
				return 0;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Determines if the process is still running.")]
		public bool HasExited
		{
			get
			{
				if (process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				int num = ExitCode_internal(process_handle);
				if (num == 259)
				{
					return false;
				}
				return true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Process identifier.")]
		public int Id
		{
			get
			{
				if (pid == 0)
				{
					throw new InvalidOperationException("Process ID has not been set.");
				}
				return pid;
			}
		}

		[Browsable(false)]
		[MonitoringDescription("The name of the computer running the process.")]
		[System.MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string MachineName
		{
			get
			{
				return "localhost";
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The main module of the process.")]
		[Browsable(false)]
		public ProcessModule MainModule
		{
			get
			{
				return Modules[0];
			}
		}

		[System.MonoTODO]
		[MonitoringDescription("The handle of the main window of the process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr MainWindowHandle
		{
			get
			{
				return (IntPtr)0;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The title of the main window of the process.")]
		[System.MonoTODO]
		public string MainWindowTitle
		{
			get
			{
				return "null";
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum working set for this process.")]
		public IntPtr MaxWorkingSet
		{
			get
			{
				if (HasExited)
				{
					throw new InvalidOperationException("The process " + ProcessName + " (ID " + Id + ") has exited");
				}
				int min;
				int max;
				if (!GetWorkingSet_internal(process_handle, out min, out max))
				{
					throw new Win32Exception();
				}
				return (IntPtr)max;
			}
			set
			{
				if (HasExited)
				{
					throw new InvalidOperationException("The process " + ProcessName + " (ID " + Id + ") has exited");
				}
				if (!SetWorkingSet_internal(process_handle, 0, value.ToInt32(), false))
				{
					throw new Win32Exception();
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The minimum working set for this process.")]
		public IntPtr MinWorkingSet
		{
			get
			{
				if (HasExited)
				{
					throw new InvalidOperationException("The process " + ProcessName + " (ID " + Id + ") has exited");
				}
				int min;
				int max;
				if (!GetWorkingSet_internal(process_handle, out min, out max))
				{
					throw new Win32Exception();
				}
				return (IntPtr)min;
			}
			set
			{
				if (HasExited)
				{
					throw new InvalidOperationException("The process " + ProcessName + " (ID " + Id + ") has exited");
				}
				if (!SetWorkingSet_internal(process_handle, value.ToInt32(), 0, true))
				{
					throw new Win32Exception();
				}
			}
		}

		[Browsable(false)]
		[MonitoringDescription("The modules that are loaded as part of this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ProcessModuleCollection Modules
		{
			get
			{
				if (module_collection == null)
				{
					module_collection = new ProcessModuleCollection(GetModules_internal(process_handle));
				}
				return module_collection;
			}
		}

		[MonitoringDescription("The number of bytes that are not pageable.")]
		[System.MonoTODO]
		[Obsolete("Use NonpagedSystemMemorySize64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int NonpagedSystemMemorySize
		{
			get
			{
				return 0;
			}
		}

		[MonitoringDescription("The number of bytes that are paged.")]
		[System.MonoTODO]
		[Obsolete("Use PagedMemorySize64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PagedMemorySize
		{
			get
			{
				return 0;
			}
		}

		[Obsolete("Use PagedSystemMemorySize64")]
		[System.MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of paged system memory in bytes.")]
		public int PagedSystemMemorySize
		{
			get
			{
				return 0;
			}
		}

		[Obsolete("Use PeakPagedMemorySize64")]
		[System.MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum amount of paged memory used by this process.")]
		public int PeakPagedMemorySize
		{
			get
			{
				return 0;
			}
		}

		[MonitoringDescription("The maximum amount of virtual memory used by this process.")]
		[Obsolete("Use PeakVirtualMemorySize64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PeakVirtualMemorySize
		{
			get
			{
				int error;
				return (int)GetProcessData(pid, 8, out error);
			}
		}

		[MonitoringDescription("The maximum amount of system memory used by this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use PeakWorkingSet64")]
		public int PeakWorkingSet
		{
			get
			{
				int error;
				return (int)GetProcessData(pid, 5, out error);
			}
		}

		[System.MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The number of bytes that are not pageable.")]
		[ComVisible(false)]
		public long NonpagedSystemMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		[ComVisible(false)]
		[System.MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The number of bytes that are paged.")]
		public long PagedMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.MonoTODO]
		[MonitoringDescription("The amount of paged system memory in bytes.")]
		[ComVisible(false)]
		public long PagedSystemMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		[System.MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum amount of paged memory used by this process.")]
		[ComVisible(false)]
		public long PeakPagedMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum amount of virtual memory used by this process.")]
		[ComVisible(false)]
		public long PeakVirtualMemorySize64
		{
			get
			{
				int error;
				return GetProcessData(pid, 8, out error);
			}
		}

		[ComVisible(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum amount of system memory used by this process.")]
		public long PeakWorkingSet64
		{
			get
			{
				int error;
				return GetProcessData(pid, 5, out error);
			}
		}

		[MonitoringDescription("Process will be of higher priority while it is actively used.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.MonoTODO]
		public bool PriorityBoostEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.MonoLimitation("Under Unix, only root is allowed to raise the priority.")]
		[MonitoringDescription("The relative process priority.")]
		public ProcessPriorityClass PriorityClass
		{
			get
			{
				if (process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				int error;
				int priorityClass = GetPriorityClass(process_handle, out error);
				if (priorityClass == 0)
				{
					throw new Win32Exception(error);
				}
				return (ProcessPriorityClass)priorityClass;
			}
			set
			{
				if (!Enum.IsDefined(typeof(ProcessPriorityClass), value))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ProcessPriorityClass));
				}
				if (process_handle == IntPtr.Zero)
				{
					throw new InvalidOperationException("Process has not been started.");
				}
				int error;
				if (!SetPriorityClass(process_handle, (int)value, out error))
				{
					throw new Win32Exception(error);
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of memory exclusively used by this process.")]
		[Obsolete("Use PrivateMemorySize64")]
		public int PrivateMemorySize
		{
			get
			{
				int error;
				return (int)GetProcessData(pid, 6, out error);
			}
		}

		[MonitoringDescription("The session ID for this process.")]
		[System.MonoNotSupported("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SessionId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonitoringDescription("The amount of processing time spent in the OS core for this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan PrivilegedProcessorTime
		{
			get
			{
				return new TimeSpan(Times(process_handle, 1));
			}
		}

		[MonitoringDescription("The name of this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ProcessName
		{
			get
			{
				if (process_name == null)
				{
					if (process_handle == IntPtr.Zero)
					{
						throw new InvalidOperationException("No process is associated with this object.");
					}
					process_name = ProcessName_internal(process_handle);
					if (process_name == null)
					{
						throw new InvalidOperationException("Process has exited, so the requested information is not available.");
					}
					if (process_name.EndsWith(".exe") || process_name.EndsWith(".bat") || process_name.EndsWith(".com"))
					{
						process_name = process_name.Substring(0, process_name.Length - 4);
					}
				}
				return process_name;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Allowed processor that can be used by this process.")]
		[System.MonoTODO]
		public IntPtr ProcessorAffinity
		{
			get
			{
				return (IntPtr)0;
			}
			set
			{
			}
		}

		[System.MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Is this process responsive.")]
		public bool Responding
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The standard error stream of this process.")]
		public StreamReader StandardError
		{
			get
			{
				if (error_stream == null)
				{
					throw new InvalidOperationException("Standard error has not been redirected");
				}
				if ((async_mode & AsyncModes.AsyncError) != AsyncModes.NoneYet)
				{
					throw new InvalidOperationException("Cannot mix asynchronous and synchonous reads.");
				}
				async_mode |= AsyncModes.SyncError;
				return error_stream;
			}
		}

		[MonitoringDescription("The standard input stream of this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public StreamWriter StandardInput
		{
			get
			{
				if (input_stream == null)
				{
					throw new InvalidOperationException("Standard input has not been redirected");
				}
				return input_stream;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The standard output stream of this process.")]
		public StreamReader StandardOutput
		{
			get
			{
				if (output_stream == null)
				{
					throw new InvalidOperationException("Standard output has not been redirected");
				}
				if ((async_mode & AsyncModes.AsyncOutput) != AsyncModes.NoneYet)
				{
					throw new InvalidOperationException("Cannot mix asynchronous and synchonous reads.");
				}
				async_mode |= AsyncModes.SyncOutput;
				return output_stream;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		[MonitoringDescription("Information for the start of this process.")]
		public ProcessStartInfo StartInfo
		{
			get
			{
				if (start_info == null)
				{
					start_info = new ProcessStartInfo();
				}
				return start_info;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				start_info = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The time this process started.")]
		public DateTime StartTime
		{
			get
			{
				return DateTime.FromFileTime(StartTime_internal(process_handle));
			}
		}

		[MonitoringDescription("The object that is used to synchronize event handler calls for this process.")]
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The number of threads of this process.")]
		[System.MonoTODO]
		public ProcessThreadCollection Threads
		{
			get
			{
				return ProcessThreadCollection.GetEmpty();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The total CPU time spent for this process.")]
		public TimeSpan TotalProcessorTime
		{
			get
			{
				return new TimeSpan(Times(process_handle, 2));
			}
		}

		[MonitoringDescription("The CPU time spent for this process in user mode.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan UserProcessorTime
		{
			get
			{
				return new TimeSpan(Times(process_handle, 0));
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of virtual memory currently used for this process.")]
		[Obsolete("Use VirtualMemorySize64")]
		public int VirtualMemorySize
		{
			get
			{
				int error;
				return (int)GetProcessData(pid, 7, out error);
			}
		}

		[MonitoringDescription("The amount of physical memory currently used for this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use WorkingSet64")]
		public int WorkingSet
		{
			get
			{
				int error;
				return (int)GetProcessData(pid, 4, out error);
			}
		}

		[ComVisible(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of memory exclusively used by this process.")]
		public long PrivateMemorySize64
		{
			get
			{
				int error;
				return GetProcessData(pid, 6, out error);
			}
		}

		[ComVisible(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of virtual memory currently used for this process.")]
		public long VirtualMemorySize64
		{
			get
			{
				int error;
				return GetProcessData(pid, 7, out error);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of physical memory currently used for this process.")]
		[ComVisible(false)]
		public long WorkingSet64
		{
			get
			{
				int error;
				return GetProcessData(pid, 4, out error);
			}
		}

		private static bool IsWindows
		{
			get
			{
				PlatformID platform = Environment.OSVersion.Platform;
				if (platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE)
				{
					return true;
				}
				return false;
			}
		}

		[Browsable(true)]
		[MonitoringDescription("Raised when it receives output data")]
		public event DataReceivedEventHandler OutputDataReceived;

		[Browsable(true)]
		[MonitoringDescription("Raised when it receives error data")]
		public event DataReceivedEventHandler ErrorDataReceived;

		[Category("Behavior")]
		[MonitoringDescription("Raised when this process exits.")]
		public event EventHandler Exited
		{
			add
			{
				if (process_handle != IntPtr.Zero && HasExited)
				{
					value.BeginInvoke(null, null, null, null);
					return;
				}
				exited_event = (EventHandler)Delegate.Combine(exited_event, value);
				if (exited_event != null)
				{
					StartExitCallbackIfNeeded();
				}
			}
			remove
			{
				exited_event = (EventHandler)Delegate.Remove(exited_event, value);
			}
		}

		private Process(IntPtr handle, int id)
		{
			process_handle = handle;
			pid = id;
		}

		public Process()
		{
		}

		private void StartExitCallbackIfNeeded()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ExitCode_internal(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long ExitTime_internal(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetWorkingSet_internal(IntPtr handle, out int min, out int max);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetWorkingSet_internal(IntPtr handle, int min, int max, bool use_min);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ProcessModule[] GetModules_internal(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetProcessData(int pid, int data_type, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPriorityClass(IntPtr handle, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPriorityClass(IntPtr handle, int priority, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long Times(IntPtr handle, int type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string ProcessName_internal(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long StartTime_internal(IntPtr handle);

		public void Close()
		{
			Dispose(true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Kill_internal(IntPtr handle, int signo);

		private bool Close(int signo)
		{
			if (process_handle == IntPtr.Zero)
			{
				throw new SystemException("No process to kill.");
			}
			int num = ExitCode_internal(process_handle);
			if (num != 259)
			{
				throw new InvalidOperationException("The process already finished.");
			}
			return Kill_internal(process_handle, signo);
		}

		public bool CloseMainWindow()
		{
			return Close(2);
		}

		[System.MonoTODO]
		public static void EnterDebugMode()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetProcess_internal(int pid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPid_internal();

		public static Process GetCurrentProcess()
		{
			int pid_internal = GetPid_internal();
			IntPtr process_internal = GetProcess_internal(pid_internal);
			if (process_internal == IntPtr.Zero)
			{
				throw new SystemException("Can't find current process");
			}
			return new Process(process_internal, pid_internal);
		}

		public static Process GetProcessById(int processId)
		{
			IntPtr process_internal = GetProcess_internal(processId);
			if (process_internal == IntPtr.Zero)
			{
				throw new ArgumentException("Can't find process with ID " + processId);
			}
			return new Process(process_internal, processId);
		}

		[System.MonoTODO("There is no support for retrieving process information from a remote machine")]
		public static Process GetProcessById(int processId, string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if (!IsLocalMachine(machineName))
			{
				throw new NotImplementedException();
			}
			return GetProcessById(processId);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int[] GetProcesses_internal();

		public static Process[] GetProcesses()
		{
			int[] processes_internal = GetProcesses_internal();
			ArrayList arrayList = new ArrayList();
			if (processes_internal == null)
			{
				return new Process[0];
			}
			for (int i = 0; i < processes_internal.Length; i++)
			{
				try
				{
					arrayList.Add(GetProcessById(processes_internal[i]));
				}
				catch (SystemException)
				{
				}
			}
			return (Process[])arrayList.ToArray(typeof(Process));
		}

		[System.MonoTODO("There is no support for retrieving process information from a remote machine")]
		public static Process[] GetProcesses(string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if (!IsLocalMachine(machineName))
			{
				throw new NotImplementedException();
			}
			return GetProcesses();
		}

		public static Process[] GetProcessesByName(string processName)
		{
			Process[] processes = GetProcesses();
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < processes.Length; i++)
			{
				if (string.Compare(processName, processes[i].ProcessName, true) == 0)
				{
					arrayList.Add(processes[i]);
				}
			}
			return (Process[])arrayList.ToArray(typeof(Process));
		}

		[System.MonoTODO]
		public static Process[] GetProcessesByName(string processName, string machineName)
		{
			throw new NotImplementedException();
		}

		public void Kill()
		{
			Close(1);
		}

		[System.MonoTODO]
		public static void LeaveDebugMode()
		{
		}

		public void Refresh()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ShellExecuteEx_internal(ProcessStartInfo startInfo, ref ProcInfo proc_info);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateProcess_internal(ProcessStartInfo startInfo, IntPtr stdin, IntPtr stdout, IntPtr stderr, ref ProcInfo proc_info);

		private static bool Start_shell(ProcessStartInfo startInfo, Process process)
		{
			ProcInfo proc_info = default(ProcInfo);
			if (startInfo.RedirectStandardInput || startInfo.RedirectStandardOutput || startInfo.RedirectStandardError)
			{
				throw new InvalidOperationException("UseShellExecute must be false when redirecting I/O.");
			}
			if (startInfo.HaveEnvVars)
			{
				throw new InvalidOperationException("UseShellExecute must be false in order to use environment variables.");
			}
			FillUserInfo(startInfo, ref proc_info);
			bool flag;
			try
			{
				flag = ShellExecuteEx_internal(startInfo, ref proc_info);
			}
			finally
			{
				if (proc_info.Password != IntPtr.Zero)
				{
					Marshal.FreeBSTR(proc_info.Password);
				}
				proc_info.Password = IntPtr.Zero;
			}
			if (!flag)
			{
				throw new Win32Exception(-proc_info.pid);
			}
			process.process_handle = proc_info.process_handle;
			process.pid = proc_info.pid;
			process.StartExitCallbackIfNeeded();
			return flag;
		}

		private static bool Start_noshell(ProcessStartInfo startInfo, Process process)
		{
			ProcInfo proc_info = default(ProcInfo);
			IntPtr read_handle = IntPtr.Zero;
			IntPtr write_handle = IntPtr.Zero;
			if (startInfo.HaveEnvVars)
			{
				string[] array = new string[startInfo.EnvironmentVariables.Count];
				startInfo.EnvironmentVariables.Keys.CopyTo(array, 0);
				proc_info.envKeys = array;
				array = new string[startInfo.EnvironmentVariables.Count];
				startInfo.EnvironmentVariables.Values.CopyTo(array, 0);
				proc_info.envValues = array;
			}
			bool flag;
			System.IO.MonoIOError error;
			if (startInfo.RedirectStandardInput)
			{
				if (IsWindows)
				{
					int options = 2;
					IntPtr write_handle2;
					flag = System.IO.MonoIO.CreatePipe(out read_handle, out write_handle2);
					if (flag)
					{
						flag = System.IO.MonoIO.DuplicateHandle(GetCurrentProcess().Handle, write_handle2, GetCurrentProcess().Handle, out write_handle, 0, 0, options);
						System.IO.MonoIO.Close(write_handle2, out error);
					}
				}
				else
				{
					flag = System.IO.MonoIO.CreatePipe(out read_handle, out write_handle);
				}
				if (!flag)
				{
					throw new IOException("Error creating standard input pipe");
				}
			}
			else
			{
				read_handle = System.IO.MonoIO.ConsoleInput;
				write_handle = (IntPtr)0;
			}
			IntPtr write_handle3;
			if (startInfo.RedirectStandardOutput)
			{
				IntPtr read_handle2 = IntPtr.Zero;
				if (IsWindows)
				{
					int options2 = 2;
					IntPtr read_handle3;
					flag = System.IO.MonoIO.CreatePipe(out read_handle3, out write_handle3);
					if (flag)
					{
						System.IO.MonoIO.DuplicateHandle(GetCurrentProcess().Handle, read_handle3, GetCurrentProcess().Handle, out read_handle2, 0, 0, options2);
						System.IO.MonoIO.Close(read_handle3, out error);
					}
				}
				else
				{
					flag = System.IO.MonoIO.CreatePipe(out read_handle2, out write_handle3);
				}
				process.stdout_rd = read_handle2;
				if (!flag)
				{
					if (startInfo.RedirectStandardInput)
					{
						System.IO.MonoIO.Close(read_handle, out error);
						System.IO.MonoIO.Close(write_handle, out error);
					}
					throw new IOException("Error creating standard output pipe");
				}
			}
			else
			{
				process.stdout_rd = (IntPtr)0;
				write_handle3 = System.IO.MonoIO.ConsoleOutput;
			}
			IntPtr write_handle4;
			if (startInfo.RedirectStandardError)
			{
				IntPtr read_handle4 = IntPtr.Zero;
				if (IsWindows)
				{
					int options3 = 2;
					IntPtr read_handle5;
					flag = System.IO.MonoIO.CreatePipe(out read_handle5, out write_handle4);
					if (flag)
					{
						System.IO.MonoIO.DuplicateHandle(GetCurrentProcess().Handle, read_handle5, GetCurrentProcess().Handle, out read_handle4, 0, 0, options3);
						System.IO.MonoIO.Close(read_handle5, out error);
					}
				}
				else
				{
					flag = System.IO.MonoIO.CreatePipe(out read_handle4, out write_handle4);
				}
				process.stderr_rd = read_handle4;
				if (!flag)
				{
					if (startInfo.RedirectStandardInput)
					{
						System.IO.MonoIO.Close(read_handle, out error);
						System.IO.MonoIO.Close(write_handle, out error);
					}
					if (startInfo.RedirectStandardOutput)
					{
						System.IO.MonoIO.Close(process.stdout_rd, out error);
						System.IO.MonoIO.Close(write_handle3, out error);
					}
					throw new IOException("Error creating standard error pipe");
				}
			}
			else
			{
				process.stderr_rd = (IntPtr)0;
				write_handle4 = System.IO.MonoIO.ConsoleError;
			}
			FillUserInfo(startInfo, ref proc_info);
			try
			{
				flag = CreateProcess_internal(startInfo, read_handle, write_handle3, write_handle4, ref proc_info);
			}
			finally
			{
				if (proc_info.Password != IntPtr.Zero)
				{
					Marshal.FreeBSTR(proc_info.Password);
				}
				proc_info.Password = IntPtr.Zero;
			}
			if (!flag)
			{
				if (startInfo.RedirectStandardInput)
				{
					System.IO.MonoIO.Close(read_handle, out error);
					System.IO.MonoIO.Close(write_handle, out error);
				}
				if (startInfo.RedirectStandardOutput)
				{
					System.IO.MonoIO.Close(process.stdout_rd, out error);
					System.IO.MonoIO.Close(write_handle3, out error);
				}
				if (startInfo.RedirectStandardError)
				{
					System.IO.MonoIO.Close(process.stderr_rd, out error);
					System.IO.MonoIO.Close(write_handle4, out error);
				}
				throw new Win32Exception(-proc_info.pid, "ApplicationName='" + startInfo.FileName + "', CommandLine='" + startInfo.Arguments + "', CurrentDirectory='" + startInfo.WorkingDirectory + "'");
			}
			process.process_handle = proc_info.process_handle;
			process.pid = proc_info.pid;
			if (startInfo.RedirectStandardInput)
			{
				System.IO.MonoIO.Close(read_handle, out error);
				process.input_stream = new StreamWriter(new System.IO.MonoSyncFileStream(write_handle, FileAccess.Write, true, 8192), Console.Out.Encoding);
				process.input_stream.AutoFlush = true;
			}
			Encoding encoding = startInfo.StandardOutputEncoding ?? Console.Out.Encoding;
			Encoding encoding2 = startInfo.StandardErrorEncoding ?? Console.Out.Encoding;
			if (startInfo.RedirectStandardOutput)
			{
				System.IO.MonoIO.Close(write_handle3, out error);
				process.output_stream = new StreamReader(new System.IO.MonoSyncFileStream(process.stdout_rd, FileAccess.Read, true, 8192), encoding, true, 8192);
			}
			if (startInfo.RedirectStandardError)
			{
				System.IO.MonoIO.Close(write_handle4, out error);
				process.error_stream = new StreamReader(new System.IO.MonoSyncFileStream(process.stderr_rd, FileAccess.Read, true, 8192), encoding2, true, 8192);
			}
			process.StartExitCallbackIfNeeded();
			return flag;
		}

		private static void FillUserInfo(ProcessStartInfo startInfo, ref ProcInfo proc_info)
		{
			if (startInfo.UserName != null)
			{
				proc_info.UserName = startInfo.UserName;
				proc_info.Domain = startInfo.Domain;
				if (startInfo.Password != null)
				{
					proc_info.Password = Marshal.SecureStringToBSTR(startInfo.Password);
				}
				else
				{
					proc_info.Password = IntPtr.Zero;
				}
				proc_info.LoadUserProfile = startInfo.LoadUserProfile;
			}
		}

		private static bool Start_common(ProcessStartInfo startInfo, Process process)
		{
			if (startInfo.FileName == null || startInfo.FileName.Length == 0)
			{
				throw new InvalidOperationException("File name has not been set");
			}
			if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
			{
				throw new InvalidOperationException("StandardErrorEncoding is only supported when standard error is redirected");
			}
			if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException("StandardOutputEncoding is only supported when standard output is redirected");
			}
			if (startInfo.UseShellExecute)
			{
				if (!string.IsNullOrEmpty(startInfo.UserName))
				{
					throw new InvalidOperationException("UserShellExecute must be false if an explicit UserName is specified when starting a process");
				}
				return Start_shell(startInfo, process);
			}
			return Start_noshell(startInfo, process);
		}

		public bool Start()
		{
			if (process_handle != IntPtr.Zero)
			{
				Process_free_internal(process_handle);
				process_handle = IntPtr.Zero;
			}
			return Start_common(start_info, this);
		}

		public static Process Start(ProcessStartInfo startInfo)
		{
			if (startInfo == null)
			{
				throw new ArgumentNullException("startInfo");
			}
			Process process = new Process();
			process.StartInfo = startInfo;
			if (Start_common(startInfo, process))
			{
				return process;
			}
			return null;
		}

		public static Process Start(string fileName)
		{
			return Start(new ProcessStartInfo(fileName));
		}

		public static Process Start(string fileName, string arguments)
		{
			return Start(new ProcessStartInfo(fileName, arguments));
		}

		public static Process Start(string fileName, string username, SecureString password, string domain)
		{
			return Start(fileName, null, username, password, domain);
		}

		public static Process Start(string fileName, string arguments, string username, SecureString password, string domain)
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);
			processStartInfo.UserName = username;
			processStartInfo.Password = password;
			processStartInfo.Domain = domain;
			processStartInfo.UseShellExecute = false;
			return Start(processStartInfo);
		}

		public override string ToString()
		{
			return base.ToString() + " (" + ProcessName + ")";
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool WaitForExit_internal(IntPtr handle, int ms);

		public void WaitForExit()
		{
			WaitForExit(-1);
		}

		public bool WaitForExit(int milliseconds)
		{
			int num = milliseconds;
			if (num == int.MaxValue)
			{
				num = -1;
			}
			DateTime dateTime = DateTime.UtcNow;
			if (async_output != null && !async_output.IsCompleted)
			{
				if (!async_output.WaitHandle.WaitOne(num, false))
				{
					return false;
				}
				if (num >= 0)
				{
					DateTime utcNow = DateTime.UtcNow;
					num -= (int)(utcNow - dateTime).TotalMilliseconds;
					if (num <= 0)
					{
						return false;
					}
					dateTime = utcNow;
				}
			}
			if (async_error != null && !async_error.IsCompleted)
			{
				if (!async_error.WaitHandle.WaitOne(num, false))
				{
					return false;
				}
				if (num >= 0)
				{
					num -= (int)(DateTime.UtcNow - dateTime).TotalMilliseconds;
					if (num <= 0)
					{
						return false;
					}
				}
			}
			return WaitForExit_internal(process_handle, num);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool WaitForInputIdle_internal(IntPtr handle, int ms);

		[System.MonoTODO]
		public bool WaitForInputIdle()
		{
			return WaitForInputIdle(-1);
		}

		[System.MonoTODO]
		public bool WaitForInputIdle(int milliseconds)
		{
			return WaitForInputIdle_internal(process_handle, milliseconds);
		}

		private static bool IsLocalMachine(string machineName)
		{
			if (machineName == "." || machineName.Length == 0)
			{
				return true;
			}
			return string.Compare(machineName, Environment.MachineName, true) == 0;
		}

		private void OnOutputDataReceived(string str)
		{
			if (this.OutputDataReceived != null)
			{
				this.OutputDataReceived(this, new DataReceivedEventArgs(str));
			}
		}

		private void OnErrorDataReceived(string str)
		{
			if (this.ErrorDataReceived != null)
			{
				this.ErrorDataReceived(this, new DataReceivedEventArgs(str));
			}
		}

		[ComVisible(false)]
		public void BeginOutputReadLine()
		{
			if (process_handle == IntPtr.Zero || output_stream == null || !StartInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException("Standard output has not been redirected or process has not been started.");
			}
			if ((async_mode & AsyncModes.SyncOutput) != AsyncModes.NoneYet)
			{
				throw new InvalidOperationException("Cannot mix asynchronous and synchonous reads.");
			}
			async_mode |= AsyncModes.AsyncOutput;
			output_canceled = false;
			if (async_output == null)
			{
				async_output = new ProcessAsyncReader(this, stdout_rd, true);
				async_output.ReadHandler.BeginInvoke(null, async_output);
			}
		}

		[ComVisible(false)]
		public void CancelOutputRead()
		{
			if (process_handle == IntPtr.Zero || output_stream == null || !StartInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException("Standard output has not been redirected or process has not been started.");
			}
			if ((async_mode & AsyncModes.SyncOutput) != AsyncModes.NoneYet)
			{
				throw new InvalidOperationException("OutputStream is not enabled for asynchronous read operations.");
			}
			if (async_output == null)
			{
				throw new InvalidOperationException("No async operation in progress.");
			}
			output_canceled = true;
		}

		[ComVisible(false)]
		public void BeginErrorReadLine()
		{
			if (process_handle == IntPtr.Zero || error_stream == null || !StartInfo.RedirectStandardError)
			{
				throw new InvalidOperationException("Standard error has not been redirected or process has not been started.");
			}
			if ((async_mode & AsyncModes.SyncError) != AsyncModes.NoneYet)
			{
				throw new InvalidOperationException("Cannot mix asynchronous and synchonous reads.");
			}
			async_mode |= AsyncModes.AsyncError;
			error_canceled = false;
			if (async_error == null)
			{
				async_error = new ProcessAsyncReader(this, stderr_rd, false);
				async_error.ReadHandler.BeginInvoke(null, async_error);
			}
		}

		[ComVisible(false)]
		public void CancelErrorRead()
		{
			if (process_handle == IntPtr.Zero || output_stream == null || !StartInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException("Standard output has not been redirected or process has not been started.");
			}
			if ((async_mode & AsyncModes.SyncOutput) != AsyncModes.NoneYet)
			{
				throw new InvalidOperationException("OutputStream is not enabled for asynchronous read operations.");
			}
			if (async_error == null)
			{
				throw new InvalidOperationException("No async operation in progress.");
			}
			error_canceled = true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Process_free_internal(IntPtr handle);

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				if (disposing)
				{
					lock (this)
					{
						if (async_output != null)
						{
							async_output.Close();
						}
						if (async_error != null)
						{
							async_error.Close();
						}
					}
				}
				lock (this)
				{
					if (process_handle != IntPtr.Zero)
					{
						Process_free_internal(process_handle);
						process_handle = IntPtr.Zero;
					}
					if (input_stream != null)
					{
						input_stream.Close();
						input_stream = null;
					}
					if (output_stream != null)
					{
						output_stream.Close();
						output_stream = null;
					}
					if (error_stream != null)
					{
						error_stream.Close();
						error_stream = null;
					}
				}
			}
			base.Dispose(disposing);
		}

		~Process()
		{
			Dispose(false);
		}

		private static void CBOnExit(object state, bool unused)
		{
			Process process = (Process)state;
			process.OnExited();
		}

		protected void OnExited()
		{
			if (exited_event == null)
			{
				return;
			}
			if (synchronizingObject == null)
			{
				Delegate[] invocationList = exited_event.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					EventHandler eventHandler = (EventHandler)invocationList[i];
					try
					{
						eventHandler(this, EventArgs.Empty);
					}
					catch
					{
					}
				}
			}
			else
			{
				object[] args = new object[2]
				{
					this,
					EventArgs.Empty
				};
				synchronizingObject.BeginInvoke(exited_event, args);
			}
		}
	}
}
