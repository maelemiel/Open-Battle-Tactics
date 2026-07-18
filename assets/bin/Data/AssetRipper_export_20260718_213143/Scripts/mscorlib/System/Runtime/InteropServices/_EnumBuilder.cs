using System.Reflection.Emit;

namespace System.Runtime.InteropServices
{
	[CLSCompliant(false)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("C7BD73DE-9F85-3290-88EE-090B8BDFE2DF")]
	[TypeLibImportClass(typeof(EnumBuilder))]
	[ComVisible(true)]
	public interface _EnumBuilder
	{
		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
