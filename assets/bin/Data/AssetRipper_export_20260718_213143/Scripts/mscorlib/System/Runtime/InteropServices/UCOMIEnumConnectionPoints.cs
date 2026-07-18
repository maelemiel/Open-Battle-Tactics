namespace System.Runtime.InteropServices
{
	[ComImport]
	[Guid("b196b285-bab4-101a-b69c-00aa00341d07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Obsolete]
	public interface UCOMIEnumConnectionPoints
	{
		[PreserveSig]
		int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] UCOMIConnectionPoint[] rgelt, out int pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		[PreserveSig]
		int Reset();

		void Clone(out UCOMIEnumConnectionPoints ppenum);
	}
}
