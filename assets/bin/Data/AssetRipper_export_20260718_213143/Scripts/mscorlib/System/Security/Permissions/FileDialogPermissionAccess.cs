using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum FileDialogPermissionAccess
	{
		None = 0,
		Open = 1,
		Save = 2,
		OpenSave = 3
	}
}
