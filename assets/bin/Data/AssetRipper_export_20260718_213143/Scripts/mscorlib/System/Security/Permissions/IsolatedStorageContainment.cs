using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public enum IsolatedStorageContainment
	{
		None = 0,
		DomainIsolationByUser = 16,
		AssemblyIsolationByUser = 32,
		DomainIsolationByRoamingUser = 80,
		AssemblyIsolationByRoamingUser = 96,
		AdministerIsolatedStorageByUser = 112,
		UnrestrictedIsolatedStorage = 240,
		ApplicationIsolationByUser = 21,
		DomainIsolationByMachine = 48,
		AssemblyIsolationByMachine = 64,
		ApplicationIsolationByMachine = 69,
		ApplicationIsolationByRoamingUser = 101
	}
}
