public static class QuitUtility
{
	public static void Restart()
	{
		Log.DebugTag("RESTARTING ", null, "QUITUTILITY");
		Singleton<InitializationManager>.instance.Restart();
	}
}
