using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public enum UIPermissionClipboard
	{
		NoClipboard = 0,
		OwnClipboard = 1,
		AllClipboard = 2
	}
}
