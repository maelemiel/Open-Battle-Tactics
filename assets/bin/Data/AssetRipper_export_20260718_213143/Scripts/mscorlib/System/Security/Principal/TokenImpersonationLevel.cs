using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[Serializable]
	[ComVisible(true)]
	public enum TokenImpersonationLevel
	{
		Anonymous = 1,
		Delegation = 4,
		Identification = 2,
		Impersonation = 3,
		None = 0
	}
}
