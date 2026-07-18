using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	public interface IIdentity
	{
		string AuthenticationType { get; }

		bool IsAuthenticated { get; }

		string Name { get; }
	}
}
