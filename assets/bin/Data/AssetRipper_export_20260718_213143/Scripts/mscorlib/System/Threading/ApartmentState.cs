using System.Runtime.InteropServices;

namespace System.Threading
{
	[Serializable]
	[ComVisible(true)]
	public enum ApartmentState
	{
		STA = 0,
		MTA = 1,
		Unknown = 2
	}
}
