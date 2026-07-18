namespace System.Runtime.InteropServices
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("b196b286-bab4-101a-b69c-00aa00341d07")]
	[Obsolete]
	public interface UCOMIConnectionPoint
	{
		void GetConnectionInterface(out Guid pIID);

		void GetConnectionPointContainer(out UCOMIConnectionPointContainer ppCPC);

		void Advise([MarshalAs(UnmanagedType.Interface)] object pUnkSink, out int pdwCookie);

		void Unadvise(int dwCookie);

		void EnumConnections(out UCOMIEnumConnections ppEnum);
	}
}
