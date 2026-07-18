namespace System.Runtime.InteropServices
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum TypeLibImporterFlags
	{
		PrimaryInteropAssembly = 1,
		UnsafeInterfaces = 2,
		SafeArrayAsSystemArray = 4,
		TransformDispRetVals = 8,
		None = 0,
		PreventClassMembers = 0x10,
		ImportAsAgnostic = 0x800,
		ImportAsItanium = 0x400,
		ImportAsX64 = 0x200,
		ImportAsX86 = 0x100,
		ReflectionOnlyLoading = 0x1000,
		SerializableValueClasses = 0x20
	}
}
