namespace System.Runtime.InteropServices
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Obsolete]
	[Guid("b196b287-bab4-101a-b69c-00aa00341d07")]
	public interface UCOMIEnumConnections
	{
		[PreserveSig]
		int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] CONNECTDATA[] rgelt, out int pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		[PreserveSig]
		void Reset();

		void Clone(out UCOMIEnumConnections ppenum);
	}
}
