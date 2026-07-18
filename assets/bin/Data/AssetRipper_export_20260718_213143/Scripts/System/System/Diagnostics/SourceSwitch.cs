namespace System.Diagnostics
{
	public class SourceSwitch : Switch
	{
		private const string description = "Source switch.";

		public SourceLevels Level
		{
			get
			{
				return (SourceLevels)base.SwitchSetting;
			}
			set
			{
				base.SwitchSetting = (int)value;
			}
		}

		public SourceSwitch(string displayName)
			: this(displayName, null)
		{
		}

		public SourceSwitch(string displayName, string defaultSwitchValue)
			: base(displayName, "Source switch.", defaultSwitchValue)
		{
		}

		public bool ShouldTrace(TraceEventType eventType)
		{
			switch (eventType)
			{
			case TraceEventType.Critical:
				return (Level & SourceLevels.Critical) != 0;
			case TraceEventType.Error:
				return (Level & SourceLevels.Error) != 0;
			case TraceEventType.Warning:
				return (Level & SourceLevels.Warning) != 0;
			case TraceEventType.Information:
				return (Level & SourceLevels.Information) != 0;
			case TraceEventType.Verbose:
				return (Level & SourceLevels.Verbose) != 0;
			default:
				return (Level & SourceLevels.ActivityTracing) != 0;
			}
		}

		protected override void OnValueChanged()
		{
			base.SwitchSetting = (int)Enum.Parse(typeof(SourceLevels), base.Value, true);
		}
	}
}
