namespace System.Runtime.InteropServices
{
	[ComImport]
	[Obsolete]
	[Guid("00020404-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface UCOMIEnumVARIANT
	{
		[PreserveSig]
		int Next(int celt, int rgvar, int pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		[PreserveSig]
		int Reset();

		void Clone(int ppenum);
	}
}
