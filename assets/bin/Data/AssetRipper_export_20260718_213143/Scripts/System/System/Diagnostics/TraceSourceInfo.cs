namespace System.Diagnostics
{
	internal class TraceSourceInfo
	{
		private string name;

		private SourceLevels levels;

		private TraceListenerCollection listeners;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public SourceLevels Levels
		{
			get
			{
				return levels;
			}
		}

		public TraceListenerCollection Listeners
		{
			get
			{
				return listeners;
			}
		}

		public TraceSourceInfo(string name, SourceLevels levels)
		{
			this.name = name;
			this.levels = levels;
			listeners = new TraceListenerCollection();
		}

		internal TraceSourceInfo(string name, SourceLevels levels, System.Diagnostics.TraceImplSettings settings)
		{
			this.name = name;
			this.levels = levels;
			listeners = new TraceListenerCollection(false);
			listeners.Add(new DefaultTraceListener(), settings);
		}
	}
}
