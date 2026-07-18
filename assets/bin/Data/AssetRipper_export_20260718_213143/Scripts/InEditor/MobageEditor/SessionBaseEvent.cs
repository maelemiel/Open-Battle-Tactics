namespace MobageEditor
{
	public abstract class SessionBaseEvent : PlusEvent
	{
		public SessionBaseEvent(string eventId, JsonData payload)
			: base(eventId, payload)
		{
		}

		protected void AppendSessionBaseValuesToEnvelope()
		{
			Characteristics defaultCharacteristics = Characteristics.DefaultCharacteristics;
			if (defaultCharacteristics != null)
			{
				Envelope["hwty"] = defaultCharacteristics.DeviceName;
				Envelope["hwrev"] = defaultCharacteristics.DeviceModel;
				Envelope["osrev"] = defaultCharacteristics.PlatformOS + " " + defaultCharacteristics.PlatformOSVersion;
			}
			Envelope["pver"] = "NDK";
		}
	}
}
