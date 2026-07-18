namespace System.Runtime.InteropServices
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[Obsolete]
	public struct IDLDESC
	{
		public int dwReserved;

		public IDLFLAG wIDLFlags;
	}
}
