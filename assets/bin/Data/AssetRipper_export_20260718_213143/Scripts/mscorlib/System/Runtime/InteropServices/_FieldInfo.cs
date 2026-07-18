using System.Globalization;
using System.Reflection;

namespace System.Runtime.InteropServices
{
	[Guid("8A7C1442-A9FB-366B-80D8-4939FFA6DBE0")]
	[ComVisible(true)]
	[CLSCompliant(false)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[TypeLibImportClass(typeof(FieldInfo))]
	public interface _FieldInfo
	{
		FieldAttributes Attributes { get; }

		Type DeclaringType { get; }

		RuntimeFieldHandle FieldHandle { get; }

		Type FieldType { get; }

		bool IsAssembly { get; }

		bool IsFamily { get; }

		bool IsFamilyAndAssembly { get; }

		bool IsFamilyOrAssembly { get; }

		bool IsInitOnly { get; }

		bool IsLiteral { get; }

		bool IsNotSerialized { get; }

		bool IsPinvokeImpl { get; }

		bool IsPrivate { get; }

		bool IsPublic { get; }

		bool IsSpecialName { get; }

		bool IsStatic { get; }

		MemberTypes MemberType { get; }

		string Name { get; }

		Type ReflectedType { get; }

		new bool Equals(object other);

		object[] GetCustomAttributes(bool inherit);

		object[] GetCustomAttributes(Type attributeType, bool inherit);

		new int GetHashCode();

		new Type GetType();

		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);

		object GetValue(object obj);

		object GetValueDirect(TypedReference obj);

		bool IsDefined(Type attributeType, bool inherit);

		void SetValue(object obj, object value);

		void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture);

		void SetValueDirect(TypedReference obj, object value);

		new string ToString();
	}
}
