namespace Spine
{
	public class Event
	{
		public EventData Data { get; private set; }

		public int Int { get; set; }

		public float Float { get; set; }

		public string String { get; set; }

		public Event(EventData data)
		{
			Data = data;
		}

		public override string ToString()
		{
			return Data.Name;
		}
	}
}
