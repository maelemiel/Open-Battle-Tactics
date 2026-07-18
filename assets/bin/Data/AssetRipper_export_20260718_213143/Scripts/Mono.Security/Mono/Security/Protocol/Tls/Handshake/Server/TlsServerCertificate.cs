using System;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsServerCertificate : HandshakeMessage
	{
		public TlsServerCertificate(Context context)
			: base(context, HandshakeType.Certificate)
		{
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			TlsStream tlsStream = new TlsStream();
			foreach (X509Certificate certificate in base.Context.ServerSettings.Certificates)
			{
				tlsStream.WriteInt24(certificate.RawData.Length);
				tlsStream.Write(certificate.RawData);
			}
			WriteInt24(Convert.ToInt32(tlsStream.Length));
			Write(tlsStream.ToArray());
			tlsStream.Close();
		}
	}
}
