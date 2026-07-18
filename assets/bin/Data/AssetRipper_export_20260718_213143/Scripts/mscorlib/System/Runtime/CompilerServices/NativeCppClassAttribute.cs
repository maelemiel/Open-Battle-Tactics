using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Struct, Inherited = true)]
	public sealed class NativeCppClassAttribute : Attribute
	{
	}
}
