namespace System.Runtime.InteropServices
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum TypeLibExporterFlags
	{
		OnlyReferenceRegistered = 1,
		None = 0,
		CallerResolvedReferences = 2,
		OldNames = 4,
		ExportAs32Bit = 0x10,
		ExportAs64Bit = 0x20
	}
}
