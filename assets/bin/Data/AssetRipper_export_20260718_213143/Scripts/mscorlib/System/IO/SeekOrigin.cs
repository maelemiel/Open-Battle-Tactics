using System.Runtime.InteropServices;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public enum SeekOrigin
	{
		Begin = 0,
		Current = 1,
		End = 2
	}
}
