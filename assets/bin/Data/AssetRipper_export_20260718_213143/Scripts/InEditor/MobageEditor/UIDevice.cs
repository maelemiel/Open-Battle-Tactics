namespace MobageEditor
{
	public class UIDevice
	{
		private static readonly UIDevice instance = new UIDevice();

		public static UIDevice Instance
		{
			get
			{
				return instance;
			}
		}

		public string SystemVersion
		{
			get
			{
				return "6.1.4";
			}
		}

		public string Model
		{
			get
			{
				return "iPhone";
			}
		}

		private UIDevice()
		{
		}
	}
}
