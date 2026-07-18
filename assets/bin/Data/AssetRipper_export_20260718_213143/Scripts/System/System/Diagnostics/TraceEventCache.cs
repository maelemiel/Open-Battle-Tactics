using System.Collections;
using System.Threading;

namespace System.Diagnostics
{
	public class TraceEventCache
	{
		private DateTime started;

		private CorrelationManager manager;

		private string callstack;

		private string thread;

		private int process;

		private long timestamp;

		public string Callstack
		{
			get
			{
				return callstack;
			}
		}

		public DateTime DateTime
		{
			get
			{
				return started;
			}
		}

		public Stack LogicalOperationStack
		{
			get
			{
				return manager.LogicalOperationStack;
			}
		}

		public int ProcessId
		{
			get
			{
				return process;
			}
		}

		public string ThreadId
		{
			get
			{
				return thread;
			}
		}

		public long Timestamp
		{
			get
			{
				return timestamp;
			}
		}

		public TraceEventCache()
		{
			started = DateTime.Now;
			manager = Trace.CorrelationManager;
			callstack = Environment.StackTrace;
			timestamp = Stopwatch.GetTimestamp();
			thread = Thread.CurrentThread.Name;
			process = Process.GetCurrentProcess().Id;
		}
	}
}
