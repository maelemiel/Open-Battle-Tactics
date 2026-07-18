using System.Collections;

namespace System.Diagnostics
{
	internal class TraceImpl
	{
		private static object initLock = new object();

		private static bool autoFlush;

		[ThreadStatic]
		private static int indentLevel = 0;

		[ThreadStatic]
		private static int indentSize;

		private static TraceListenerCollection listeners;

		private static bool use_global_lock;

		private static CorrelationManager correlation_manager = new CorrelationManager();

		public static bool AutoFlush
		{
			get
			{
				InitOnce();
				return autoFlush;
			}
			set
			{
				InitOnce();
				autoFlush = value;
			}
		}

		public static int IndentLevel
		{
			get
			{
				InitOnce();
				return indentLevel;
			}
			set
			{
				lock (ListenersSyncRoot)
				{
					indentLevel = value;
					foreach (TraceListener listener in Listeners)
					{
						listener.IndentLevel = indentLevel;
					}
				}
			}
		}

		public static int IndentSize
		{
			get
			{
				InitOnce();
				return indentSize;
			}
			set
			{
				lock (ListenersSyncRoot)
				{
					indentSize = value;
					foreach (TraceListener listener in Listeners)
					{
						listener.IndentSize = indentSize;
					}
				}
			}
		}

		public static TraceListenerCollection Listeners
		{
			get
			{
				InitOnce();
				return listeners;
			}
		}

		private static object ListenersSyncRoot
		{
			get
			{
				return ((ICollection)Listeners).SyncRoot;
			}
		}

		public static CorrelationManager CorrelationManager
		{
			get
			{
				InitOnce();
				return correlation_manager;
			}
		}

		[System.MonoLimitation("the property exists but it does nothing.")]
		public static bool UseGlobalLock
		{
			get
			{
				InitOnce();
				return use_global_lock;
			}
			set
			{
				InitOnce();
				use_global_lock = value;
			}
		}

		private TraceImpl()
		{
		}

		private static void InitOnce()
		{
			if (initLock == null)
			{
				return;
			}
			lock (initLock)
			{
				if (listeners == null)
				{
					System.Diagnostics.TraceImplSettings traceImplSettings = new System.Diagnostics.TraceImplSettings();
					autoFlush = traceImplSettings.AutoFlush;
					indentLevel = traceImplSettings.IndentLevel;
					indentSize = traceImplSettings.IndentSize;
					listeners = traceImplSettings.Listeners;
				}
			}
			initLock = null;
		}

		[System.MonoTODO]
		public static void Assert(bool condition)
		{
			if (!condition)
			{
				Fail(new StackTrace(true).ToString());
			}
		}

		[System.MonoTODO]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				Fail(message);
			}
		}

		[System.MonoTODO]
		public static void Assert(bool condition, string message, string detailMessage)
		{
			if (!condition)
			{
				Fail(message, detailMessage);
			}
		}

		public static void Close()
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.Close();
				}
			}
		}

		[System.MonoTODO]
		public static void Fail(string message)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.Fail(message);
				}
			}
		}

		[System.MonoTODO]
		public static void Fail(string message, string detailMessage)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.Fail(message, detailMessage);
				}
			}
		}

		public static void Flush()
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.Flush();
				}
			}
		}

		public static void Indent()
		{
			IndentLevel++;
		}

		public static void Unindent()
		{
			IndentLevel--;
		}

		public static void Write(object value)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.Write(value);
					if (AutoFlush)
					{
						listener.Flush();
					}
				}
			}
		}

		public static void Write(string message)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.Write(message);
					if (AutoFlush)
					{
						listener.Flush();
					}
				}
			}
		}

		public static void Write(object value, string category)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.Write(value, category);
					if (AutoFlush)
					{
						listener.Flush();
					}
				}
			}
		}

		public static void Write(string message, string category)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.Write(message, category);
					if (AutoFlush)
					{
						listener.Flush();
					}
				}
			}
		}

		public static void WriteIf(bool condition, object value)
		{
			if (condition)
			{
				Write(value);
			}
		}

		public static void WriteIf(bool condition, string message)
		{
			if (condition)
			{
				Write(message);
			}
		}

		public static void WriteIf(bool condition, object value, string category)
		{
			if (condition)
			{
				Write(value, category);
			}
		}

		public static void WriteIf(bool condition, string message, string category)
		{
			if (condition)
			{
				Write(message, category);
			}
		}

		public static void WriteLine(object value)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.WriteLine(value);
					if (AutoFlush)
					{
						listener.Flush();
					}
				}
			}
		}

		public static void WriteLine(string message)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.WriteLine(message);
					if (AutoFlush)
					{
						listener.Flush();
					}
				}
			}
		}

		public static void WriteLine(object value, string category)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.WriteLine(value, category);
					if (AutoFlush)
					{
						listener.Flush();
					}
				}
			}
		}

		public static void WriteLine(string message, string category)
		{
			lock (ListenersSyncRoot)
			{
				foreach (TraceListener listener in Listeners)
				{
					listener.WriteLine(message, category);
					if (AutoFlush)
					{
						listener.Flush();
					}
				}
			}
		}

		public static void WriteLineIf(bool condition, object value)
		{
			if (condition)
			{
				WriteLine(value);
			}
		}

		public static void WriteLineIf(bool condition, string message)
		{
			if (condition)
			{
				WriteLine(message);
			}
		}

		public static void WriteLineIf(bool condition, object value, string category)
		{
			if (condition)
			{
				WriteLine(value, category);
			}
		}

		public static void WriteLineIf(bool condition, string message, string category)
		{
			if (condition)
			{
				WriteLine(message, category);
			}
		}
	}
}
