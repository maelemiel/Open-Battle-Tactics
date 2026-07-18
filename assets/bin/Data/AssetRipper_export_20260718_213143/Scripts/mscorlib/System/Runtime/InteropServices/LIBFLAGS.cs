namespace System.Runtime.InteropServices
{
	[Serializable]
	[Obsolete]
	[Flags]
	public enum LIBFLAGS
	{
		LIBFLAG_FRESTRICTED = 1,
		LIBFLAG_FCONTROL = 2,
		LIBFLAG_FHIDDEN = 4,
		LIBFLAG_FHASDISKIMAGE = 8
	}
}
