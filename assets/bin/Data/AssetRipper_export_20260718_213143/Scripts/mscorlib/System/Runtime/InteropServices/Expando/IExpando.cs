using System.Reflection;

namespace System.Runtime.InteropServices.Expando
{
	[Guid("afbf15e6-c37c-11d2-b88e-00a0c9b471b8")]
	[ComVisible(true)]
	public interface IExpando : IReflect
	{
		FieldInfo AddField(string name);

		MethodInfo AddMethod(string name, Delegate method);

		PropertyInfo AddProperty(string name);

		void RemoveMember(MemberInfo m);
	}
}
