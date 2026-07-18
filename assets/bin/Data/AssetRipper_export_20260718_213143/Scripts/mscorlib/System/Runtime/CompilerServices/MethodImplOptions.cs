using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	[ComVisible(true)]
	[Flags]
	public enum MethodImplOptions
	{
		Unmanaged = 4,
		ForwardRef = 0x10,
		InternalCall = 0x1000,
		Synchronized = 0x20,
		NoInlining = 8,
		PreserveSig = 0x80,
		NoOptimization = 0x40
	}
}
