namespace System.Runtime.InteropServices.ComTypes
{
	[ComImport]
	[Guid("b196b285-bab4-101a-b69c-00aa00341d07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumConnectionPoints
	{
		[PreserveSig]
		int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] IConnectionPoint[] rgelt, IntPtr pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		void Reset();

		void Clone(out IEnumConnectionPoints ppenum);
	}
}
