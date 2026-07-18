namespace System.Runtime.InteropServices
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00020403-0000-0000-c000-000000000046")]
	[Obsolete]
	public interface UCOMITypeComp
	{
		void Bind([MarshalAs(UnmanagedType.LPWStr)] string szName, int lHashVal, short wFlags, out UCOMITypeInfo ppTInfo, out DESCKIND pDescKind, out BINDPTR pBindPtr);

		void BindType([MarshalAs(UnmanagedType.LPWStr)] string szName, int lHashVal, out UCOMITypeInfo ppTInfo, out UCOMITypeComp ppTComp);
	}
}
