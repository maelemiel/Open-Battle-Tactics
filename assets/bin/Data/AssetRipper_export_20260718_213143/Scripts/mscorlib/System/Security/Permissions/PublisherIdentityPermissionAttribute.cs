using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Cryptography;

namespace System.Security.Permissions
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	public sealed class PublisherIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		private string certFile;

		private string signedFile;

		private string x509data;

		public string CertFile
		{
			get
			{
				return certFile;
			}
			set
			{
				certFile = value;
			}
		}

		public string SignedFile
		{
			get
			{
				return signedFile;
			}
			set
			{
				signedFile = value;
			}
		}

		public string X509Certificate
		{
			get
			{
				return x509data;
			}
			set
			{
				x509data = value;
			}
		}

		public PublisherIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new PublisherIdentityPermission(PermissionState.Unrestricted);
			}
			X509Certificate x509Certificate = null;
			if (x509data != null)
			{
				byte[] data = CryptoConvert.FromHex(x509data);
				x509Certificate = new X509Certificate(data);
				return new PublisherIdentityPermission(x509Certificate);
			}
			if (certFile != null)
			{
				x509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile(certFile);
				return new PublisherIdentityPermission(x509Certificate);
			}
			if (signedFile != null)
			{
				x509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromSignedFile(signedFile);
				return new PublisherIdentityPermission(x509Certificate);
			}
			return new PublisherIdentityPermission(PermissionState.None);
		}
	}
}
