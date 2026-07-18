using System.Security.Cryptography.X509Certificates;
using Mono.Security.Cryptography;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	internal sealed class TlsClientSettings
	{
		private string targetHost;

		private System.Security.Cryptography.X509Certificates.X509CertificateCollection certificates;

		private System.Security.Cryptography.X509Certificates.X509Certificate clientCertificate;

		private RSAManaged certificateRSA;

		public string TargetHost
		{
			get
			{
				return targetHost;
			}
			set
			{
				targetHost = value;
			}
		}

		public System.Security.Cryptography.X509Certificates.X509CertificateCollection Certificates
		{
			get
			{
				return certificates;
			}
			set
			{
				certificates = value;
			}
		}

		public System.Security.Cryptography.X509Certificates.X509Certificate ClientCertificate
		{
			get
			{
				return clientCertificate;
			}
			set
			{
				clientCertificate = value;
				UpdateCertificateRSA();
			}
		}

		public RSAManaged CertificateRSA
		{
			get
			{
				return certificateRSA;
			}
		}

		public TlsClientSettings()
		{
			certificates = new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
			targetHost = string.Empty;
		}

		public void UpdateCertificateRSA()
		{
			if (clientCertificate == null)
			{
				certificateRSA = null;
				return;
			}
			Mono.Security.X509.X509Certificate x509Certificate = new Mono.Security.X509.X509Certificate(clientCertificate.GetRawCertData());
			certificateRSA = new RSAManaged(x509Certificate.RSA.KeySize);
			certificateRSA.ImportParameters(x509Certificate.RSA.ExportParameters(false));
		}
	}
}
