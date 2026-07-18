namespace System.Runtime.InteropServices
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[Obsolete]
	public struct TYPEDESC
	{
		public IntPtr lpValue;

		public short vt;
	}
}
