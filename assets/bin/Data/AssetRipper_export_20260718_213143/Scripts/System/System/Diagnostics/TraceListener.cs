using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	public abstract class TraceListener : MarshalByRefObject, IDisposable
	{
		[ThreadStatic]
		private int indentLevel;

		[ThreadStatic]
		private int indentSize = 4;

		[ThreadStatic]
		private StringDictionary attributes = new StringDictionary();

		[ThreadStatic]
		private TraceFilter filter;

		[ThreadStatic]
		private TraceOptions options;

		private string name;

		private bool needIndent = true;

		public int IndentLevel
		{
			get
			{
				return indentLevel;
			}
			set
			{
				indentLevel = value;
			}
		}

		public int IndentSize
		{
			get
			{
				return indentSize;
			}
			set
			{
				indentSize = value;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		protected bool NeedIndent
		{
			get
			{
				return needIndent;
			}
			set
			{
				needIndent = value;
			}
		}

		[System.MonoLimitation("This property exists but is never considered.")]
		public virtual bool IsThreadSafe
		{
			get
			{
				return false;
			}
		}

		public StringDictionary Attributes
		{
			get
			{
				return attributes;
			}
		}

		[ComVisible(false)]
		public TraceFilter Filter
		{
			get
			{
				return filter;
			}
			set
			{
				filter = value;
			}
		}

		[ComVisible(false)]
		public TraceOptions TraceOutputOptions
		{
			get
			{
				return options;
			}
			set
			{
				options = value;
			}
		}

		protected TraceListener()
			: this(string.Empty)
		{
		}

		protected TraceListener(string name)
		{
			Name = name;
		}

		public virtual void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual void Fail(string message)
		{
			Fail(message, string.Empty);
		}

		public virtual void Fail(string message, string detailMessage)
		{
			WriteLine("---- DEBUG ASSERTION FAILED ----");
			WriteLine("---- Assert Short Message ----");
			WriteLine(message);
			WriteLine("---- Assert Long Message ----");
			WriteLine(detailMessage);
			WriteLine(string.Empty);
		}

		public virtual void Flush()
		{
		}

		public virtual void Write(object o)
		{
			Write(o.ToString());
		}

		public abstract void Write(string message);

		public virtual void Write(object o, string category)
		{
			Write(o.ToString(), category);
		}

		public virtual void Write(string message, string category)
		{
			Write(category + ": " + message);
		}

		protected virtual void WriteIndent()
		{
			NeedIndent = false;
			string message = new string(' ', IndentLevel * IndentSize);
			Write(message);
		}

		public virtual void WriteLine(object o)
		{
			WriteLine(o.ToString());
		}

		public abstract void WriteLine(string message);

		public virtual void WriteLine(object o, string category)
		{
			WriteLine(o.ToString(), category);
		}

		public virtual void WriteLine(string message, string category)
		{
			WriteLine(category + ": " + message);
		}

		internal static string FormatArray(ICollection list, string joiner)
		{
			string[] array = new string[list.Count];
			int num = 0;
			foreach (object item in list)
			{
				array[num++] = ((item == null) ? string.Empty : item.ToString());
			}
			return string.Join(joiner, array);
		}

		[ComVisible(false)]
		public virtual void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
			{
				return;
			}
			WriteLine(string.Format("{0} {1}: {2} : {3}", source, eventType, id, data));
			if (eventCache != null)
			{
				if ((TraceOutputOptions & TraceOptions.ProcessId) != TraceOptions.None)
				{
					WriteLine("    ProcessId=" + eventCache.ProcessId);
				}
				if ((TraceOutputOptions & TraceOptions.LogicalOperationStack) != TraceOptions.None)
				{
					WriteLine("    LogicalOperationStack=" + FormatArray(eventCache.LogicalOperationStack, ", "));
				}
				if ((TraceOutputOptions & TraceOptions.ThreadId) != TraceOptions.None)
				{
					WriteLine("    ThreadId=" + eventCache.ThreadId);
				}
				if ((TraceOutputOptions & TraceOptions.DateTime) != TraceOptions.None)
				{
					WriteLine("    DateTime=" + eventCache.DateTime.ToString("o"));
				}
				if ((TraceOutputOptions & TraceOptions.Timestamp) != TraceOptions.None)
				{
					WriteLine("    Timestamp=" + eventCache.Timestamp);
				}
				if ((TraceOutputOptions & TraceOptions.Callstack) != TraceOptions.None)
				{
					WriteLine("    Callstack=" + eventCache.Callstack);
				}
			}
		}

		[ComVisible(false)]
		public virtual void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			if (Filter == null || Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
			{
				TraceData(eventCache, source, eventType, id, FormatArray(data, " "));
			}
		}

		[ComVisible(false)]
		public virtual void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
		{
			TraceEvent(eventCache, source, eventType, id, null);
		}

		[ComVisible(false)]
		public virtual void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			TraceData(eventCache, source, eventType, id, message);
		}

		[ComVisible(false)]
		public virtual void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			TraceEvent(eventCache, source, eventType, id, string.Format(format, args));
		}

		[ComVisible(false)]
		public virtual void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
		{
			TraceEvent(eventCache, source, TraceEventType.Transfer, id, string.Format("{0}, relatedActivityId={1}", message, relatedActivityId));
		}

		protected internal virtual string[] GetSupportedAttributes()
		{
			return null;
		}
	}
}
