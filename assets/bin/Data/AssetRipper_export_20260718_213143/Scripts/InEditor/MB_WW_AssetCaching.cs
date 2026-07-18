public class MB_WW_AssetCaching
{
	private static object lockObject = new object();

	private static bool wasEnabled;

	public static void StartCaching()
	{
		lock (lockObject)
		{
			if (!wasEnabled)
			{
				wasEnabled = true;
			}
		}
	}
}
