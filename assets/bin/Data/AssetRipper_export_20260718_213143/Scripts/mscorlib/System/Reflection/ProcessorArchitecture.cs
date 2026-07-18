using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public enum ProcessorArchitecture
	{
		None = 0,
		MSIL = 1,
		X86 = 2,
		IA64 = 3,
		Amd64 = 4
	}
}
