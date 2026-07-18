using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[Serializable]
	[ComVisible(true)]
	public enum PrincipalPolicy
	{
		UnauthenticatedPrincipal = 0,
		NoPrincipal = 1,
		WindowsPrincipal = 2
	}
}
