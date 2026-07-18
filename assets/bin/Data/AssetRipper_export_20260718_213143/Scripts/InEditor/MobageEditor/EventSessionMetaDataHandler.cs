using System;

namespace MobageEditor
{
	public class EventSessionMetaDataHandler : IJSONDictionaryHandler
	{
		private static Random random = new Random();

		public string SID = string.Format("{0:x8}{1:x8}{2:x8}{3:x8}", random.Next(), random.Next(), random.Next(), random.Next());

		public void Process(JsonData data)
		{
			data["sid"] = SID;
		}
	}
}
