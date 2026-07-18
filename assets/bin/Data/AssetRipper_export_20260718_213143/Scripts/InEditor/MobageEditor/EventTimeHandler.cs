using System;

namespace MobageEditor
{
	public class EventTimeHandler : IJSONDictionaryHandler
	{
		private int startUptime = DateTime.Now.Millisecond;

		private long unixtimestamp
		{
			get
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				DateTime now = DateTime.Now;
				return (long)(now - dateTime).TotalMilliseconds;
			}
		}

		public void Process(JsonData pJSON)
		{
			int millisecond = DateTime.Now.Millisecond;
			pJSON["seqdt"] = millisecond - startUptime;
			pJSON["evts"] = unixtimestamp;
		}
	}
}
