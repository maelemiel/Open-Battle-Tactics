using System;

namespace Mono.Security.X509.Extensions
{
	[Flags]
	public enum KeyUsages
	{
		digitalSignature = 0x80,
		nonRepudiation = 0x40,
		keyEncipherment = 0x20,
		dataEncipherment = 0x10,
		keyAgreement = 8,
		keyCertSign = 4,
		cRLSign = 2,
		encipherOnly = 1,
		decipherOnly = 0x800,
		none = 0
	}
}
