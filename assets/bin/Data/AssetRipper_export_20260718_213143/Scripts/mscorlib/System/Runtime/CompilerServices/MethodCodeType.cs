using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	[ComVisible(true)]
	public enum MethodCodeType
	{
		IL = 0,
		Native = 1,
		OPTIL = 2,
		Runtime = 3
	}
}
