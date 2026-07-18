using System;

namespace Mono.Security.Protocol.Tls
{
	[Serializable]
	internal enum HandshakeState
	{
		None = 0,
		Started = 1,
		Finished = 2
	}
}
