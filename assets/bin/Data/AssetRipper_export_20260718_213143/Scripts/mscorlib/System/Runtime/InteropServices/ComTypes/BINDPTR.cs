namespace System.Runtime.InteropServices.ComTypes
{
	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	public struct BINDPTR
	{
		[FieldOffset(0)]
		public IntPtr lpfuncdesc;

		[FieldOffset(0)]
		public IntPtr lptcomp;

		[FieldOffset(0)]
		public IntPtr lpvardesc;
	}
}
