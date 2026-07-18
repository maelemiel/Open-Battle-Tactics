using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[Serializable]
	[ComVisible(true)]
	public enum WindowsBuiltInRole
	{
		Administrator = 544,
		User = 545,
		Guest = 546,
		PowerUser = 547,
		AccountOperator = 548,
		SystemOperator = 549,
		PrintOperator = 550,
		BackupOperator = 551,
		Replicator = 552
	}
}
