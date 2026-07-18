namespace System.Runtime.InteropServices.ComTypes
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("b196b287-bab4-101a-b69c-00aa00341d07")]
	public interface IEnumConnections
	{
		[PreserveSig]
		int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] CONNECTDATA[] rgelt, IntPtr pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		void Reset();

		void Clone(out IEnumConnections ppenum);
	}
}
