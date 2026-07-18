using System.Reflection.Emit;

namespace System.Runtime.InteropServices
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("007D8A14-FDF3-363E-9A0B-FEC0618260A2")]
	[TypeLibImportClass(typeof(MethodBuilder))]
	public interface _MethodBuilder
	{
		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
