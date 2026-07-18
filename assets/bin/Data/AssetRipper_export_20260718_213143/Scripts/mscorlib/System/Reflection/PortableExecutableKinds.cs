using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	[Flags]
	public enum PortableExecutableKinds
	{
		NotAPortableExecutableImage = 0,
		ILOnly = 1,
		Required32Bit = 2,
		PE32Plus = 4,
		Unmanaged32Bit = 8
	}
}
