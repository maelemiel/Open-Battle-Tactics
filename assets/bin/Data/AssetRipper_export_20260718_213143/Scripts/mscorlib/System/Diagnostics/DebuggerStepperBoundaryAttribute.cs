using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
	public sealed class DebuggerStepperBoundaryAttribute : Attribute
	{
	}
}
