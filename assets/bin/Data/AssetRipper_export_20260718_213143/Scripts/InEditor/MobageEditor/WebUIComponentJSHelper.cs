namespace MobageEditor
{
	public class WebUIComponentJSHelper
	{
		public const string JSCallExperienceLaunched = "JSUIWindow.onExperienceLaunched()";

		public const string JSCallOnHidden = "JSUIWindow.onHidden()";

		public const string JSCallOnShown = "JSUIWindow.onShown()";

		public static string JSInsertExperienceName(string name)
		{
			return string.Format("JSUIWindow.experienceName = {0};", name);
		}

		public static string JSInsertExperienceOptions(JsonData optionsDict)
		{
			return string.Format("JSUIWindow.experienceOptions = {0};", (optionsDict == null) ? "{}" : optionsDict.ToJson());
		}

		public static string JSInsertTabName(string name)
		{
			return string.Format("JSUIWindow.tabName = {0};", name);
		}

		public static string JSInsertTabOptions(JsonData optionsDict)
		{
			return string.Format("JSUIWindow.tabOptions = {0};", (optionsDict == null) ? "{}" : optionsDict.ToJson());
		}
	}
}
