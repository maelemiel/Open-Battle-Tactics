using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public enum PackingSize
	{
		Unspecified = 0,
		Size1 = 1,
		Size2 = 2,
		Size4 = 4,
		Size8 = 8,
		Size16 = 0x10,
		Size32 = 0x20,
		Size64 = 0x40,
		Size128 = 0x80
	}
}
