using System.Runtime.InteropServices;

namespace System.Configuration.Assemblies
{
	[Serializable]
	[ComVisible(true)]
	public enum AssemblyHashAlgorithm
	{
		None = 0,
		MD5 = 32771,
		SHA1 = 32772
	}
}
