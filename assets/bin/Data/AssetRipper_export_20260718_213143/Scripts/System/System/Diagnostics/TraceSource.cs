using System.Collections;
using System.Collections.Specialized;

namespace System.Diagnostics
{
	public class TraceSource
	{
		private SourceSwitch source_switch;

		private TraceListenerCollection listeners;

		public StringDictionary Attributes
		{
			get
			{
				return source_switch.Attributes;
			}
		}

		public TraceListenerCollection Listeners
		{
			get
			{
				return listeners;
			}
		}

		public string Name
		{
			get
			{
				return source_switch.DisplayName;
			}
		}

		public SourceSwitch Switch
		{
			get
			{
				return source_switch;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				source_switch = value;
			}
		}

		public TraceSource(string name)
			: this(name, SourceLevels.Off)
		{
		}

		public TraceSource(string name, SourceLevels sourceLevels)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Hashtable hashtable = null;
			System.Diagnostics.TraceSourceInfo traceSourceInfo = ((hashtable == null) ? null : (hashtable[name] as System.Diagnostics.TraceSourceInfo));
			source_switch = new SourceSwitch(name);
			if (traceSourceInfo == null)
			{
				listeners = new TraceListenerCollection();
				return;
			}
			source_switch.Level = traceSourceInfo.Levels;
			listeners = traceSourceInfo.Listeners;
		}

		public void Close()
		{
			lock (((ICollection)listeners).SyncRoot)
			{
				foreach (TraceListener listener in listeners)
				{
					listener.Close();
				}
			}
		}

		public void Flush()
		{
			lock (((ICollection)listeners).SyncRoot)
			{
				foreach (TraceListener listener in listeners)
				{
					listener.Flush();
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceData(TraceEventType eventType, int id, object data)
		{
			if (!source_switch.ShouldTrace(eventType))
			{
				return;
			}
			lock (((ICollection)listeners).SyncRoot)
			{
				foreach (TraceListener listener in listeners)
				{
					listener.TraceData(null, Name, eventType, id, data);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceData(TraceEventType eventType, int id, params object[] data)
		{
			if (!source_switch.ShouldTrace(eventType))
			{
				return;
			}
			lock (((ICollection)listeners).SyncRoot)
			{
				foreach (TraceListener listener in listeners)
				{
					listener.TraceData(null, Name, eventType, id, data);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id)
		{
			if (!source_switch.ShouldTrace(eventType))
			{
				return;
			}
			lock (((ICollection)listeners).SyncRoot)
			{
				foreach (TraceListener listener in listeners)
				{
					listener.TraceEvent(null, Name, eventType, id);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id, string message)
		{
			if (!source_switch.ShouldTrace(eventType))
			{
				return;
			}
			lock (((ICollection)listeners).SyncRoot)
			{
				foreach (TraceListener listener in listeners)
				{
					listener.TraceEvent(null, Name, eventType, id, message);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			if (!source_switch.ShouldTrace(eventType))
			{
				return;
			}
			lock (((ICollection)listeners).SyncRoot)
			{
				foreach (TraceListener listener in listeners)
				{
					listener.TraceEvent(null, Name, eventType, id, format, args);
				}
			}
		}

		[Conditional("TRACE")]
		public void TraceInformation(string format)
		{
		}

		[Conditional("TRACE")]
		public void TraceInformation(string format, params object[] args)
		{
		}

		[Conditional("TRACE")]
		public void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			if (!source_switch.ShouldTrace(TraceEventType.Transfer))
			{
				return;
			}
			lock (((ICollection)listeners).SyncRoot)
			{
				foreach (TraceListener listener in listeners)
				{
					listener.TraceTransfer(null, Name, id, message, relatedActivityId);
				}
			}
		}

		protected virtual string[] GetSupportedAttributes()
		{
			return null;
		}
	}
}
