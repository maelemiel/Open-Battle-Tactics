using System.Security.Cryptography;
using Mono.Security.Cryptography;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	internal class TlsServerSettings
	{
		private X509CertificateCollection certificates;

		private RSA certificateRSA;

		private RSAParameters rsaParameters;

		private byte[] signedParams;

		private string[] distinguisedNames;

		private bool serverKeyExchange;

		private bool certificateRequest;

		private ClientCertificateType[] certificateTypes;

		public bool ServerKeyExchange
		{
			get
			{
				return serverKeyExchange;
			}
			set
			{
				serverKeyExchange = value;
			}
		}

		public X509CertificateCollection Certificates
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

		public RSA CertificateRSA
		{
			get
			{
				return certificateRSA;
			}
		}

		public RSAParameters RsaParameters
		{
			get
			{
				return rsaParameters;
			}
			set
			{
				rsaParameters = value;
			}
		}

		public byte[] SignedParams
		{
			get
			{
				return signedParams;
			}
			set
			{
				signedParams = value;
			}
		}

		public bool CertificateRequest
		{
			get
			{
				return certificateRequest;
			}
			set
			{
				certificateRequest = value;
			}
		}

		public ClientCertificateType[] CertificateTypes
		{
			get
			{
				return certificateTypes;
			}
			set
			{
				certificateTypes = value;
			}
		}

		public string[] DistinguisedNames
		{
			get
			{
				return distinguisedNames;
			}
			set
			{
				distinguisedNames = value;
			}
		}

		public void UpdateCertificateRSA()
		{
			if (certificates == null || certificates.Count == 0)
			{
				certificateRSA = null;
				return;
			}
			certificateRSA = new RSAManaged(certificates[0].RSA.KeySize);
			certificateRSA.ImportParameters(certificates[0].RSA.ExportParameters(false));
		}
	}
}
