using System.Runtime.InteropServices;

namespace System.Security
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	public sealed class SuppressUnmanagedCodeSecurityAttribute : Attribute
	{
	}
}
