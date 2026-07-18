using System.Runtime.InteropServices;

namespace System.Security
{
	[Serializable]
	[ComVisible(true)]
	public enum SecurityZone
	{
		MyComputer = 0,
		Intranet = 1,
		Trusted = 2,
		Internet = 3,
		Untrusted = 4,
		NoZone = -1
	}
}
