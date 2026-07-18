using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Enum, Inherited = false)]
	[ComVisible(true)]
	public class FlagsAttribute : Attribute
	{
	}
}
