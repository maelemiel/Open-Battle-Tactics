namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public enum GCHandleType
	{
		Weak = 0,
		WeakTrackResurrection = 1,
		Normal = 2,
		Pinned = 3
	}
}
