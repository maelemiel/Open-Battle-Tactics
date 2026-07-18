using System;

namespace Mono.Security.X509
{
	[Serializable]
	[Flags]
	public enum X509ChainStatusFlags
	{
		InvalidBasicConstraints = 0x400,
		NoError = 0,
		NotSignatureValid = 8,
		NotTimeNested = 2,
		NotTimeValid = 1,
		PartialChain = 0x10000,
		UntrustedRoot = 0x20
	}
}
