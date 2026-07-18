namespace System.Runtime.InteropServices.ComTypes
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000101-0000-0000-c000-000000000046")]
	public interface IEnumString
	{
		[PreserveSig]
		int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] string[] rgelt, IntPtr pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		void Reset();

		void Clone(out IEnumString ppenum);
	}
}
