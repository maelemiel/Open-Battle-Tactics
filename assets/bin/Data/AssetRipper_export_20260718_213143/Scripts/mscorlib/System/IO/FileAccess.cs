using System.Runtime.InteropServices;

namespace System.IO
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum FileAccess
	{
		Read = 1,
		Write = 2,
		ReadWrite = 3
	}
}
