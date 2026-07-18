using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public interface ICustomAttributeProvider
	{
		object[] GetCustomAttributes(bool inherit);

		object[] GetCustomAttributes(Type attributeType, bool inherit);

		bool IsDefined(Type attributeType, bool inherit);
	}
}
