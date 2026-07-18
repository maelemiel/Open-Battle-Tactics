using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[Serializable]
	[ComVisible(true)]
	public enum WindowsAccountType
	{
		Normal = 0,
		Guest = 1,
		System = 2,
		Anonymous = 3
	}
}
