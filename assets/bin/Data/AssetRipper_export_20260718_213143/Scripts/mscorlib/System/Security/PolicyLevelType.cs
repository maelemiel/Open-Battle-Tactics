using System.Runtime.InteropServices;

namespace System.Security
{
	[Serializable]
	[ComVisible(true)]
	public enum PolicyLevelType
	{
		User = 0,
		Machine = 1,
		Enterprise = 2,
		AppDomain = 3
	}
}
