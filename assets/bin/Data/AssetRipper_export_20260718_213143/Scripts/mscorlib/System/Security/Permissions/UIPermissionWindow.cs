using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public enum UIPermissionWindow
	{
		NoWindows = 0,
		SafeSubWindows = 1,
		SafeTopLevelWindows = 2,
		AllWindows = 3
	}
}
