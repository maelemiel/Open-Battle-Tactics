namespace MobageEditor
{
	public class SessionStartEvent : SessionBaseEvent
	{
		private static JsonData sstPayload
		{
			get
			{
				return new JsonData();
			}
		}

		public SessionStartEvent()
			: base("SST", sstPayload)
		{
			AppendSessionBaseValuesToEnvelope();
		}
	}
}
