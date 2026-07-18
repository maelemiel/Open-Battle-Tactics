using System.Security.Cryptography.X509Certificates;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	internal class ServerContext : Context
	{
		private SslServerStream sslStream;

		private bool request_client_certificate;

		private bool clientCertificateRequired;

		public SslServerStream SslStream
		{
			get
			{
				return sslStream;
			}
		}

		public bool ClientCertificateRequired
		{
			get
			{
				return clientCertificateRequired;
			}
		}

		public bool RequestClientCertificate
		{
			get
			{
				return request_client_certificate;
			}
		}

		public ServerContext(SslServerStream stream, SecurityProtocolType securityProtocolType, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool requestClientCertificate)
			: base(securityProtocolType)
		{
			sslStream = stream;
			this.clientCertificateRequired = clientCertificateRequired;
			request_client_certificate = requestClientCertificate;
			Mono.Security.X509.X509Certificate value = new Mono.Security.X509.X509Certificate(serverCertificate.GetRawCertData());
			base.ServerSettings.Certificates = new Mono.Security.X509.X509CertificateCollection();
			base.ServerSettings.Certificates.Add(value);
			base.ServerSettings.UpdateCertificateRSA();
			base.ServerSettings.CertificateTypes = new ClientCertificateType[1];
			base.ServerSettings.CertificateTypes[0] = ClientCertificateType.RSA;
			Mono.Security.X509.X509CertificateCollection trustedRootCertificates = X509StoreManager.TrustedRootCertificates;
			string[] array = new string[trustedRootCertificates.Count];
			int num = 0;
			foreach (Mono.Security.X509.X509Certificate item in trustedRootCertificates)
			{
				array[num++] = item.IssuerName;
			}
			base.ServerSettings.DistinguisedNames = array;
		}
	}
}
