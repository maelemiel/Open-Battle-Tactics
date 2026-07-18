namespace System.Runtime.InteropServices
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[Obsolete]
	public struct VARDESC
	{
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		[ComVisible(false)]
		public struct DESCUNION
		{
			[FieldOffset(0)]
			public IntPtr lpvarValue;

			[FieldOffset(0)]
			public int oInst;
		}

		public int memid;

		public string lpstrSchema;

		public ELEMDESC elemdescVar;

		public short wVarFlags;

		public VarEnum varkind;
	}
}
