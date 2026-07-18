namespace MobageEditor
{
	public class PlusUIEvent : AnalyticsEvent
	{
		public PlusUIEvent(string eventId, JsonData payload)
			: base(eventId, "PLUSUI", "PC", payload)
		{
		}
	}
}
