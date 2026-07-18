using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum EnvironmentPermissionAccess
	{
		NoAccess = 0,
		Read = 1,
		Write = 2,
		AllAccess = 3
	}
}
