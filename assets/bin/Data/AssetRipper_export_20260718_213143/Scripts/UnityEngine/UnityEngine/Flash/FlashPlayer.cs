namespace UnityEngine.Flash
{
	public sealed class FlashPlayer
	{
		public static string TargetVersion
		{
			get
			{
				return GetUnityAppConstants("TargetFlashPlayerVersion");
			}
		}

		public static string TargetSwfVersion
		{
			get
			{
				return GetUnityAppConstants("TargetSwfVersion");
			}
		}

		internal static string GetUnityAppConstants(string name)
		{
			return ActionScript.Expression<string>("UnityNative.getUnityAppConstants()[{0}]", new object[1] { name });
		}
	}
}
