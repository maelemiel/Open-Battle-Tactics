using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public enum PEFileKinds
	{
		Dll = 1,
		ConsoleApplication = 2,
		WindowApplication = 3
	}
}
