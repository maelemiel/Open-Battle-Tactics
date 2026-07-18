namespace System.Diagnostics
{
	public class SourceFilter : TraceFilter
	{
		private string source;

		public string Source
		{
			get
			{
				return source;
			}
			set
			{
				if (source == null)
				{
					throw new ArgumentNullException("value");
				}
				source = value;
			}
		}

		public SourceFilter(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.source = source;
		}

		public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
		{
			return source == this.source;
		}
	}
}
