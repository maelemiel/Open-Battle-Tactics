namespace System.Runtime.InteropServices
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	[Obsolete]
	public struct STATSTG
	{
		public string pwcsName;

		public int type;

		public long cbSize;

		public FILETIME mtime;

		public FILETIME ctime;

		public FILETIME atime;

		public int grfMode;

		public int grfLocksSupported;

		public Guid clsid;

		public int grfStateBits;

		public int reserved;
	}
}
