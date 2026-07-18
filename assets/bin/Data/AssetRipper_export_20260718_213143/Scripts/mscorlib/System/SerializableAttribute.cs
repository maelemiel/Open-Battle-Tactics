using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false, AllowMultiple = false)]
	[ComVisible(true)]
	public sealed class SerializableAttribute : Attribute
	{
	}
}
