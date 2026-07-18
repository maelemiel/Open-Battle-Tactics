namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public enum ImporterEventKind
	{
		NOTIF_TYPECONVERTED = 0,
		NOTIF_CONVERTWARNING = 1,
		ERROR_REFTOINVALIDTYPELIB = 2
	}
}
