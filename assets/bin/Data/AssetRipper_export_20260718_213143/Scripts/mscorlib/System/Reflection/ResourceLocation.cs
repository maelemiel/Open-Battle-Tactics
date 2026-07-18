using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum ResourceLocation
	{
		Embedded = 1,
		ContainedInAnotherAssembly = 2,
		ContainedInManifestFile = 4
	}
}
