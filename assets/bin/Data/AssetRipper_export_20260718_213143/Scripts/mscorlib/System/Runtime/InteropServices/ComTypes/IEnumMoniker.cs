namespace System.Runtime.InteropServices.ComTypes
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000102-0000-0000-c000-000000000046")]
	public interface IEnumMoniker
	{
		[PreserveSig]
		int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] IMoniker[] rgelt, IntPtr pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		void Reset();

		void Clone(out IEnumMoniker ppenum);
	}
}
