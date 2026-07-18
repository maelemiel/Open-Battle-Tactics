namespace System.Runtime.InteropServices
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Obsolete]
	[Guid("00000102-0000-0000-c000-000000000046")]
	public interface UCOMIEnumMoniker
	{
		[PreserveSig]
		int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] UCOMIMoniker[] rgelt, out int pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		[PreserveSig]
		int Reset();

		void Clone(out UCOMIEnumMoniker ppenum);
	}
}
