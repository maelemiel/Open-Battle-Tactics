namespace MobageEditor
{
	public class EventDeviceMetaDataHandler : IJSONDictionaryHandler
	{
		private string carrier;

		public EventDeviceMetaDataHandler(string carrier)
		{
			this.carrier = carrier;
		}

		public void Process(JsonData json)
		{
			JsonData jsonData = new JsonData();
			jsonData["IFA"] = "F4A8DDE9-3FB1-4565-A666-09E01650D0BC";
			jsonData["UDID"] = "5559d0da23195bd1ba4e2ef7405fb6fd00000000";
			json["avalue"] = jsonData;
			json["carr"] = carrier;
			string appId = Mobage.sharedInstance.AppId;
			if (appId != null)
			{
				appId = appId.ToLower();
				if (appId.Contains("-android"))
				{
					json["pltfmsku"] = "ANDROID";
				}
				else
				{
					json["pltfmsku"] = "IOS";
				}
			}
		}
	}
}
