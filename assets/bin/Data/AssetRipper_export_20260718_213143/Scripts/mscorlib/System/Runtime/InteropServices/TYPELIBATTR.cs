namespace System.Runtime.InteropServices
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[Obsolete]
	public struct TYPELIBATTR
	{
		public Guid guid;

		public int lcid;

		public SYSKIND syskind;

		public short wMajorVerNum;

		public short wMinorVerNum;

		public LIBFLAGS wLibFlags;
	}
}
