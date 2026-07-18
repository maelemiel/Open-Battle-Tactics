using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public enum LoaderOptimization
	{
		NotSpecified = 0,
		SingleDomain = 1,
		MultiDomain = 2,
		MultiDomainHost = 3,
		[Obsolete]
		DomainMask = 3,
		[Obsolete]
		DisallowBindings = 4
	}
}
