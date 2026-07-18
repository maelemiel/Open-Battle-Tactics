namespace System.Runtime.InteropServices
{
	[Serializable]
	[Obsolete]
	[Flags]
	public enum PARAMFLAG
	{
		PARAMFLAG_NONE = 0,
		PARAMFLAG_FIN = 1,
		PARAMFLAG_FOUT = 2,
		PARAMFLAG_FLCID = 4,
		PARAMFLAG_FRETVAL = 8,
		PARAMFLAG_FOPT = 0x10,
		PARAMFLAG_FHASDEFAULT = 0x20,
		PARAMFLAG_FHASCUSTDATA = 0x40
	}
}
