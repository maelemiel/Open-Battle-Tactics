using System.Runtime.InteropServices;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public enum DriveType
	{
		CDRom = 5,
		Fixed = 3,
		Network = 4,
		NoRootDirectory = 1,
		Ram = 6,
		Removable = 2,
		Unknown = 0
	}
}
