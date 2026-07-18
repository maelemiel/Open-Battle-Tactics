using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
	[Serializable]
	[ComVisible(true)]
	public enum RegistryHive
	{
		ClassesRoot = int.MinValue,
		CurrentConfig = -2147483643,
		CurrentUser = -2147483647,
		DynData = -2147483642,
		LocalMachine = -2147483646,
		PerformanceData = -2147483644,
		Users = -2147483645
	}
}
