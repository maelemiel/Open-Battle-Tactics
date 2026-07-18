namespace System.Runtime.InteropServices
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[Obsolete]
	public struct DISPPARAMS
	{
		public IntPtr rgvarg;

		public IntPtr rgdispidNamedArgs;

		public int cArgs;

		public int cNamedArgs;
	}
}
