using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Lifetime
{
	[Serializable]
	[ComVisible(true)]
	public enum LeaseState
	{
		Null = 0,
		Initial = 1,
		Active = 2,
		Renewing = 3,
		Expired = 4
	}
}
