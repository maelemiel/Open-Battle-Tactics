namespace MobageEditor
{
	public abstract class AnalyticsEvent : IAnalyticsEvent
	{
		private JsonData payload;

		public JsonData Envelope { get; private set; }

		public AnalyticsEvent(string eventId, string eventClass, string sourceType, JsonData payload)
		{
			this.payload = ((payload == null) ? JsonMapper.ToObject("{}") : payload);
			Envelope = new JsonData();
			Envelope["evid"] = eventId;
			Envelope["srcty"] = sourceType;
			Envelope["evpl"] = this.payload;
			Envelope["evcl"] = eventClass;
		}
	}
}
