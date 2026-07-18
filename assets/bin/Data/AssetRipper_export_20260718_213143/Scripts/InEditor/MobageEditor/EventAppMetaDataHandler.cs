namespace MobageEditor
{
	public class EventAppMetaDataHandler : IJSONDictionaryHandler
	{
		private string regionId;

		private string appId;

		private string appVersion;

		private string ndkVersion;

		public EventAppMetaDataHandler(string regionId, string appId, string appVersion, string ndkVersion)
		{
			this.regionId = regionId;
			this.appId = appId;
			this.appVersion = appVersion;
			this.ndkVersion = ndkVersion;
		}

		public void Process(JsonData json)
		{
			json["apiver"] = 5;
			json["pver"] = "NDK-" + ndkVersion;
			json["asku"] = appId;
			json["arel"] = appVersion;
			json["srvc"] = regionId;
		}
	}
}
