namespace MobageEditor
{
	public class DummySessionAnalytics : ISessionAnalytics
	{
		public string SessionId
		{
			get
			{
				return "dummysid";
			}
		}
	}
}
