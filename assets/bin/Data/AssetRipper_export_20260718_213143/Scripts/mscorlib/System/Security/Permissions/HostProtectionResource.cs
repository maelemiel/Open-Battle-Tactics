using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum HostProtectionResource
	{
		None = 0,
		Synchronization = 1,
		SharedState = 2,
		ExternalProcessMgmt = 4,
		SelfAffectingProcessMgmt = 8,
		ExternalThreading = 0x10,
		SelfAffectingThreading = 0x20,
		SecurityInfrastructure = 0x40,
		UI = 0x80,
		MayLeakOnAbort = 0x100,
		All = 0x1FF
	}
}
