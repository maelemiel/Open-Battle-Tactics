using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	[Flags]
	public enum EventAttributes
	{
		None = 0,
		SpecialName = 0x200,
		ReservedMask = 0x400,
		RTSpecialName = 0x400
	}
}
