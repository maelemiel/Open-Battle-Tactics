using System;

namespace Mono.Security.Protocol.Tls.Handshake
{
	[Serializable]
	internal enum ClientCertificateType
	{
		RSA = 1,
		DSS = 2,
		RSAFixed = 3,
		DSSFixed = 4,
		Unknown = 255
	}
}
