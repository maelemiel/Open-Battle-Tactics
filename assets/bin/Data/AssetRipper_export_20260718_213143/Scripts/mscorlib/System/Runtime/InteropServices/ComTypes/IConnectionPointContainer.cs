namespace System.Runtime.InteropServices.ComTypes
{
	[ComImport]
	[Guid("b196b284-bab4-101a-b69c-00aa00341d07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IConnectionPointContainer
	{
		void EnumConnectionPoints(out IEnumConnectionPoints ppEnum);

		void FindConnectionPoint([In] ref Guid riid, out IConnectionPoint ppCP);
	}
}
