namespace System.Runtime.InteropServices
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[Obsolete]
	public struct CONNECTDATA
	{
		[MarshalAs(UnmanagedType.Interface)]
		public object pUnk;

		public int dwCookie;
	}
}
