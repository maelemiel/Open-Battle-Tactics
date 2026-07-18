using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	[ComVisible(true)]
	public class ThreadStaticAttribute : Attribute
	{
	}
}
