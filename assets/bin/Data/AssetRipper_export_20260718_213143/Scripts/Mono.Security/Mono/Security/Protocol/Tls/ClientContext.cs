using System.Security.Cryptography.X509Certificates;

namespace Mono.Security.Protocol.Tls
{
	internal class ClientContext : Context
	{
		private SslClientStream sslStream;

		private short clientHelloProtocol;

		public SslClientStream SslStream
		{
			get
			{
				return sslStream;
			}
		}

		public short ClientHelloProtocol
		{
			get
			{
				return clientHelloProtocol;
			}
			set
			{
				clientHelloProtocol = value;
			}
		}

		public ClientContext(SslClientStream stream, SecurityProtocolType securityProtocolType, string targetHost, X509CertificateCollection clientCertificates)
			: base(securityProtocolType)
		{
			sslStream = stream;
			base.ClientSettings.Certificates = clientCertificates;
			base.ClientSettings.TargetHost = targetHost;
		}

		public override void Clear()
		{
			clientHelloProtocol = 0;
			base.Clear();
		}
	}
}
