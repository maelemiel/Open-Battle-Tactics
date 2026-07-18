using System;

namespace Mono.Security.Protocol.Tls
{
	[Serializable]
	public enum CipherAlgorithmType
	{
		Des = 0,
		None = 1,
		Rc2 = 2,
		Rc4 = 3,
		Rijndael = 4,
		SkipJack = 5,
		TripleDes = 6
	}
}
