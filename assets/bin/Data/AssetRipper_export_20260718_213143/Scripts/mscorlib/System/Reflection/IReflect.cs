using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Guid("AFBF15E5-C37C-11d2-B88E-00A0C9B471B8")]
	[ComVisible(true)]
	public interface IReflect
	{
		Type UnderlyingSystemType { get; }

		FieldInfo GetField(string name, BindingFlags bindingAttr);

		FieldInfo[] GetFields(BindingFlags bindingAttr);

		MemberInfo[] GetMember(string name, BindingFlags bindingAttr);

		MemberInfo[] GetMembers(BindingFlags bindingAttr);

		MethodInfo GetMethod(string name, BindingFlags bindingAttr);

		MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers);

		MethodInfo[] GetMethods(BindingFlags bindingAttr);

		PropertyInfo[] GetProperties(BindingFlags bindingAttr);

		PropertyInfo GetProperty(string name, BindingFlags bindingAttr);

		PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers);

		object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);
	}
}
