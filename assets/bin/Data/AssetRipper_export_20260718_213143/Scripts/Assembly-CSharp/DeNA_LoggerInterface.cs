public class DeNA_LoggerInterface
{
	public static void InitializeLog()
	{
		bool flag = true;
		flag = false;
		Log.InitializeLogger("ANDROID", flag, new DeviceFormatter());
		Log.DebugTag("Logger Initialized", null, "DeNA_Logger");
	}
}
