namespace System.Runtime.InteropServices
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[Obsolete]
	public struct PARAMDESC
	{
		public IntPtr lpVarValue;

		public PARAMFLAG wParamFlags;
	}
}
