namespace MobageEditor
{
	public class EventSequenceNumberHandler : IJSONDictionaryHandler
	{
		private long next = 1L;

		public void Process(JsonData data)
		{
			data["seq"] = next++;
		}
	}
}
