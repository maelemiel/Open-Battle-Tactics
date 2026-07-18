using System.Reflection.Emit;

namespace System.Runtime.InteropServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[TypeLibImportClass(typeof(ParameterBuilder))]
	[Guid("36329EBA-F97A-3565-BC07-0ED5C6EF19FC")]
	[CLSCompliant(false)]
	[ComVisible(true)]
	public interface _ParameterBuilder
	{
		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
