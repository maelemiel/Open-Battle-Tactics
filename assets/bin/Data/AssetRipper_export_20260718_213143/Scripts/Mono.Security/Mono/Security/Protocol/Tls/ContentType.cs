using System;

namespace Mono.Security.Protocol.Tls
{
	[Serializable]
	internal enum ContentType : byte
	{
		ChangeCipherSpec = 20,
		Alert = 21,
		Handshake = 22,
		ApplicationData = 23
	}
}
