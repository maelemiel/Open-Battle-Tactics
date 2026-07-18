namespace System.Runtime.InteropServices.ComTypes
{
	[Serializable]
	[Flags]
	public enum LIBFLAGS
	{
		LIBFLAG_FRESTRICTED = 1,
		LIBFLAG_FCONTROL = 2,
		LIBFLAG_FHIDDEN = 4,
		LIBFLAG_FHASDISKIMAGE = 8
	}
}
