namespace MobageEditor
{
	public class PlusEvent : AnalyticsEvent
	{
		public PlusEvent(string eventId, JsonData payload)
			: base(eventId, "PLUS", "PC", payload)
		{
		}

		public PlusEvent(string eventId)
			: this(eventId, null)
		{
		}
	}
}
