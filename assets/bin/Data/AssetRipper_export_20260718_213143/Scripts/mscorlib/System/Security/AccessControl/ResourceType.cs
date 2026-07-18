namespace System.Security.AccessControl
{
	public enum ResourceType
	{
		Unknown = 0,
		FileObject = 1,
		Service = 2,
		Printer = 3,
		RegistryKey = 4,
		LMShare = 5,
		KernelObject = 6,
		WindowObject = 7,
		DSObject = 8,
		DSObjectAll = 9,
		ProviderDefined = 10,
		WmiGuidObject = 11,
		RegistryWow6432Key = 12
	}
}
