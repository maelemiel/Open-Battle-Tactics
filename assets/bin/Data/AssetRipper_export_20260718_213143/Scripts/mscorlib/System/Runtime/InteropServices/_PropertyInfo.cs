using System.Globalization;
using System.Reflection;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[TypeLibImportClass(typeof(PropertyInfo))]
	[Guid("F59ED4E4-E68F-3218-BD77-061AA82824BF")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[CLSCompliant(false)]
	public interface _PropertyInfo
	{
		PropertyAttributes Attributes { get; }

		bool CanRead { get; }

		bool CanWrite { get; }

		Type DeclaringType { get; }

		bool IsSpecialName { get; }

		MemberTypes MemberType { get; }

		string Name { get; }

		Type PropertyType { get; }

		Type ReflectedType { get; }

		new bool Equals(object other);

		MethodInfo[] GetAccessors();

		MethodInfo[] GetAccessors(bool nonPublic);

		object[] GetCustomAttributes(bool inherit);

		object[] GetCustomAttributes(Type attributeType, bool inherit);

		MethodInfo GetGetMethod();

		MethodInfo GetGetMethod(bool nonPublic);

		new int GetHashCode();

		ParameterInfo[] GetIndexParameters();

		MethodInfo GetSetMethod();

		MethodInfo GetSetMethod(bool nonPublic);

		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);

		new Type GetType();

		object GetValue(object obj, object[] index);

		object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

		bool IsDefined(Type attributeType, bool inherit);

		void SetValue(object obj, object value, object[] index);

		void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

		new string ToString();
	}
}
