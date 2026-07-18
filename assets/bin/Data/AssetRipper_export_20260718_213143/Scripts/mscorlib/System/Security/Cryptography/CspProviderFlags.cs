using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum CspProviderFlags
	{
		UseMachineKeyStore = 1,
		UseDefaultKeyContainer = 2,
		UseExistingKey = 8,
		NoFlags = 0,
		NoPrompt = 0x40,
		UseArchivableKey = 0x10,
		UseNonExportableKey = 4,
		UseUserProtectedKey = 0x20
	}
}
