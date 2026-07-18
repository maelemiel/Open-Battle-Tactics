namespace MobageEditor
{
	public class Characteristics
	{
		public string PrefixedDeviceId = "IFV:F054121D-3CE4-4826-847D-39F1E2AFC3EA";

		public string Carrier = "KDDI";

		public string PlatformOS = "iPhone OS";

		public string Locale = "en_US";

		public string Timezone = "America/Los_Angeles (PDT) offset -2520";

		public string DeviceName = "x86_64";

		public string IdForPlatformServer = "ifv:76602712-4438-432A-A256-09AEC92EB533";

		public static Characteristics DefaultCharacteristics = new Characteristics();

		private JsonData _idsForAnalytics;

		public string PlatformOSVersion
		{
			get
			{
				UIDevice instance = UIDevice.Instance;
				return instance.SystemVersion;
			}
		}

		public string DeviceModel
		{
			get
			{
				return UIDevice.Instance.Model;
			}
		}

		public JsonData IDsForAnalytics
		{
			get
			{
				if (_idsForAnalytics == null)
				{
					_idsForAnalytics = new JsonData();
					_idsForAnalytics["UUID"] = "1823F075-8EFA-414E-823B-63DBA5669D97";
					_idsForAnalytics["IFA"] = "A409E571-6694-4307-B190-0C08DE43B39B";
					_idsForAnalytics["MVID"] = "DF8C4E4F-4471-412D-A753-1A9E0665B419";
					_idsForAnalytics["IFV"] = "2FCBCA80-AA74-449E-8216-57C0C357141E";
				}
				return _idsForAnalytics;
			}
		}
	}
}
