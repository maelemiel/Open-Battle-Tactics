using System;

namespace Mono.Security.Protocol.Tls
{
	[Serializable]
	public enum ExchangeAlgorithmType
	{
		DiffieHellman = 0,
		Fortezza = 1,
		None = 2,
		RsaKeyX = 3,
		RsaSign = 4
	}
}
