namespace System.Runtime.ConstrainedExecution
{
	[Serializable]
	public enum Consistency
	{
		MayCorruptAppDomain = 1,
		MayCorruptInstance = 2,
		MayCorruptProcess = 0,
		WillNotCorruptState = 3
	}
}
