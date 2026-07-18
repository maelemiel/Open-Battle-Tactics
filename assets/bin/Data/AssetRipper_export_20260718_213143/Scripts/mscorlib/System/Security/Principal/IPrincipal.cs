using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	public interface IPrincipal
	{
		IIdentity Identity { get; }

		bool IsInRole(string role);
	}
}
