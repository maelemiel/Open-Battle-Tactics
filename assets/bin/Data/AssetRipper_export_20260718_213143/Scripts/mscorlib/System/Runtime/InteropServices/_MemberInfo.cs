using System.Reflection;

namespace System.Runtime.InteropServices
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[TypeLibImportClass(typeof(MemberInfo))]
	[Guid("f7102fa9-cabb-3a74-a6da-b4567ef1b079")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface _MemberInfo
	{
		Type DeclaringType { get; }

		MemberTypes MemberType { get; }

		string Name { get; }

		Type ReflectedType { get; }

		new bool Equals(object other);

		object[] GetCustomAttributes(bool inherit);

		object[] GetCustomAttributes(Type attributeType, bool inherit);

		new int GetHashCode();

		new Type GetType();

		bool IsDefined(Type attributeType, bool inherit);

		new string ToString();

		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
