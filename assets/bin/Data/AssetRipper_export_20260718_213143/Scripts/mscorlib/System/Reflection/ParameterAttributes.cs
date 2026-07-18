using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	[Flags]
	public enum ParameterAttributes
	{
		None = 0,
		In = 1,
		Out = 2,
		Lcid = 4,
		Retval = 8,
		Optional = 0x10,
		ReservedMask = 0xF000,
		HasDefault = 0x1000,
		HasFieldMarshal = 0x2000,
		Reserved3 = 0x4000,
		Reserved4 = 0x8000
	}
}
