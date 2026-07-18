using System.Reflection.Emit;

namespace System.Runtime.InteropServices
{
	[Guid("4E6350D1-A08B-3DEC-9A3E-C465F9AEEC0C")]
	[TypeLibImportClass(typeof(LocalBuilder))]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComVisible(true)]
	[CLSCompliant(false)]
	public interface _LocalBuilder
	{
		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
