namespace System.Diagnostics
{
	[SwitchLevel(typeof(TraceLevel))]
	public class TraceSwitch : Switch
	{
		public TraceLevel Level
		{
			get
			{
				return (TraceLevel)base.SwitchSetting;
			}
			set
			{
				if (!Enum.IsDefined(typeof(TraceLevel), value))
				{
					throw new ArgumentException("value");
				}
				base.SwitchSetting = (int)value;
			}
		}

		public bool TraceError
		{
			get
			{
				return base.SwitchSetting >= 1;
			}
		}

		public bool TraceWarning
		{
			get
			{
				return base.SwitchSetting >= 2;
			}
		}

		public bool TraceInfo
		{
			get
			{
				return base.SwitchSetting >= 3;
			}
		}

		public bool TraceVerbose
		{
			get
			{
				return base.SwitchSetting >= 4;
			}
		}

		public TraceSwitch(string displayName, string description)
			: base(displayName, description)
		{
		}

		public TraceSwitch(string displayName, string description, string defaultSwitchValue)
			: base(displayName, description)
		{
			base.Value = defaultSwitchValue;
		}

		protected override void OnSwitchSettingChanged()
		{
			if (base.SwitchSetting < 0)
			{
				base.SwitchSetting = 0;
			}
			else if (base.SwitchSetting > 4)
			{
				base.SwitchSetting = 4;
			}
		}

		protected override void OnValueChanged()
		{
			base.SwitchSetting = (int)Enum.Parse(typeof(TraceLevel), base.Value, true);
		}
	}
}
