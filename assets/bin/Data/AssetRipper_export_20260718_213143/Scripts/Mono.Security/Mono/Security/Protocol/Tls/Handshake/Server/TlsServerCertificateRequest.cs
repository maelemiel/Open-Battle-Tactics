using System;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsServerCertificateRequest : HandshakeMessage
	{
		public TlsServerCertificateRequest(Context context)
			: base(context, HandshakeType.CertificateRequest)
		{
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			ServerContext serverContext = (ServerContext)base.Context;
			int num = serverContext.ServerSettings.CertificateTypes.Length;
			WriteByte(Convert.ToByte(num));
			for (int i = 0; i < num; i++)
			{
				WriteByte((byte)serverContext.ServerSettings.CertificateTypes[i]);
			}
			if (serverContext.ServerSettings.DistinguisedNames.Length > 0)
			{
				TlsStream tlsStream = new TlsStream();
				string[] distinguisedNames = serverContext.ServerSettings.DistinguisedNames;
				foreach (string rdn in distinguisedNames)
				{
					byte[] bytes = X501.FromString(rdn).GetBytes();
					tlsStream.Write((short)bytes.Length);
					tlsStream.Write(bytes);
				}
				Write((short)tlsStream.Length);
				Write(tlsStream.ToArray());
			}
			else
			{
				Write((short)0);
			}
		}
	}
}
