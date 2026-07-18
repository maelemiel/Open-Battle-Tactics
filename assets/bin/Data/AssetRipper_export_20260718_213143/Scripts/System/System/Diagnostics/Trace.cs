using System.Reflection;

namespace System.Diagnostics
{
	public sealed class Trace
	{
		public static bool AutoFlush
		{
			get
			{
				return System.Diagnostics.TraceImpl.AutoFlush;
			}
			set
			{
				System.Diagnostics.TraceImpl.AutoFlush = value;
			}
		}

		public static int IndentLevel
		{
			get
			{
				return System.Diagnostics.TraceImpl.IndentLevel;
			}
			set
			{
				System.Diagnostics.TraceImpl.IndentLevel = value;
			}
		}

		public static int IndentSize
		{
			get
			{
				return System.Diagnostics.TraceImpl.IndentSize;
			}
			set
			{
				System.Diagnostics.TraceImpl.IndentSize = value;
			}
		}

		public static TraceListenerCollection Listeners
		{
			get
			{
				return System.Diagnostics.TraceImpl.Listeners;
			}
		}

		public static CorrelationManager CorrelationManager
		{
			get
			{
				return System.Diagnostics.TraceImpl.CorrelationManager;
			}
		}

		public static bool UseGlobalLock
		{
			get
			{
				return System.Diagnostics.TraceImpl.UseGlobalLock;
			}
			set
			{
				System.Diagnostics.TraceImpl.UseGlobalLock = value;
			}
		}

		private Trace()
		{
		}

		[System.MonoNotSupported("")]
		public static void Refresh()
		{
			throw new NotImplementedException();
		}

		[Conditional("TRACE")]
		public static void Assert(bool condition)
		{
			System.Diagnostics.TraceImpl.Assert(condition);
		}

		[Conditional("TRACE")]
		public static void Assert(bool condition, string message)
		{
			System.Diagnostics.TraceImpl.Assert(condition, message);
		}

		[Conditional("TRACE")]
		public static void Assert(bool condition, string message, string detailMessage)
		{
			System.Diagnostics.TraceImpl.Assert(condition, message, detailMessage);
		}

		[Conditional("TRACE")]
		public static void Close()
		{
			System.Diagnostics.TraceImpl.Close();
		}

		[Conditional("TRACE")]
		public static void Fail(string message)
		{
			System.Diagnostics.TraceImpl.Fail(message);
		}

		[Conditional("TRACE")]
		public static void Fail(string message, string detailMessage)
		{
			System.Diagnostics.TraceImpl.Fail(message, detailMessage);
		}

		[Conditional("TRACE")]
		public static void Flush()
		{
			System.Diagnostics.TraceImpl.Flush();
		}

		[Conditional("TRACE")]
		public static void Indent()
		{
			System.Diagnostics.TraceImpl.Indent();
		}

		[Conditional("TRACE")]
		public static void Unindent()
		{
			System.Diagnostics.TraceImpl.Unindent();
		}

		[Conditional("TRACE")]
		public static void Write(object value)
		{
			System.Diagnostics.TraceImpl.Write(value);
		}

		[Conditional("TRACE")]
		public static void Write(string message)
		{
			System.Diagnostics.TraceImpl.Write(message);
		}

		[Conditional("TRACE")]
		public static void Write(object value, string category)
		{
			System.Diagnostics.TraceImpl.Write(value, category);
		}

		[Conditional("TRACE")]
		public static void Write(string message, string category)
		{
			System.Diagnostics.TraceImpl.Write(message, category);
		}

		[Conditional("TRACE")]
		public static void WriteIf(bool condition, object value)
		{
			System.Diagnostics.TraceImpl.WriteIf(condition, value);
		}

		[Conditional("TRACE")]
		public static void WriteIf(bool condition, string message)
		{
			System.Diagnostics.TraceImpl.WriteIf(condition, message);
		}

		[Conditional("TRACE")]
		public static void WriteIf(bool condition, object value, string category)
		{
			System.Diagnostics.TraceImpl.WriteIf(condition, value, category);
		}

		[Conditional("TRACE")]
		public static void WriteIf(bool condition, string message, string category)
		{
			System.Diagnostics.TraceImpl.WriteIf(condition, message, category);
		}

		[Conditional("TRACE")]
		public static void WriteLine(object value)
		{
			System.Diagnostics.TraceImpl.WriteLine(value);
		}

		[Conditional("TRACE")]
		public static void WriteLine(string message)
		{
			System.Diagnostics.TraceImpl.WriteLine(message);
		}

		[Conditional("TRACE")]
		public static void WriteLine(object value, string category)
		{
			System.Diagnostics.TraceImpl.WriteLine(value, category);
		}

		[Conditional("TRACE")]
		public static void WriteLine(string message, string category)
		{
			System.Diagnostics.TraceImpl.WriteLine(message, category);
		}

		[Conditional("TRACE")]
		public static void WriteLineIf(bool condition, object value)
		{
			System.Diagnostics.TraceImpl.WriteLineIf(condition, value);
		}

		[Conditional("TRACE")]
		public static void WriteLineIf(bool condition, string message)
		{
			System.Diagnostics.TraceImpl.WriteLineIf(condition, message);
		}

		[Conditional("TRACE")]
		public static void WriteLineIf(bool condition, object value, string category)
		{
			System.Diagnostics.TraceImpl.WriteLineIf(condition, value, category);
		}

		[Conditional("TRACE")]
		public static void WriteLineIf(bool condition, string message, string category)
		{
			System.Diagnostics.TraceImpl.WriteLineIf(condition, message, category);
		}

		private static void DoTrace(string kind, Assembly report, string message)
		{
			string arg = string.Empty;
			try
			{
				arg = report.Location;
			}
			catch (MethodAccessException)
			{
			}
			System.Diagnostics.TraceImpl.WriteLine(string.Format("{0} {1} : 0 : {2}", arg, kind, message));
		}

		[Conditional("TRACE")]
		public static void TraceError(string message)
		{
			DoTrace("Error", Assembly.GetCallingAssembly(), message);
		}

		[Conditional("TRACE")]
		public static void TraceError(string message, params object[] args)
		{
			DoTrace("Error", Assembly.GetCallingAssembly(), string.Format(message, args));
		}

		[Conditional("TRACE")]
		public static void TraceInformation(string message)
		{
			DoTrace("Information", Assembly.GetCallingAssembly(), message);
		}

		[Conditional("TRACE")]
		public static void TraceInformation(string message, params object[] args)
		{
			DoTrace("Information", Assembly.GetCallingAssembly(), string.Format(message, args));
		}

		[Conditional("TRACE")]
		public static void TraceWarning(string message)
		{
			DoTrace("Warning", Assembly.GetCallingAssembly(), message);
		}

		[Conditional("TRACE")]
		public static void TraceWarning(string message, params object[] args)
		{
			DoTrace("Warning", Assembly.GetCallingAssembly(), string.Format(message, args));
		}
	}
}
