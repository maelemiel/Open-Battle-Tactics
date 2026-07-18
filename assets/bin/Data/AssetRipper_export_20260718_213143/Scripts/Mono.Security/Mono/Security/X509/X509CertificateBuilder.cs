using System;
using System.Security.Cryptography;

namespace Mono.Security.X509
{
	public class X509CertificateBuilder : X509Builder
	{
		private byte version;

		private byte[] sn;

		private string issuer;

		private DateTime notBefore;

		private DateTime notAfter;

		private string subject;

		private AsymmetricAlgorithm aa;

		private byte[] issuerUniqueID;

		private byte[] subjectUniqueID;

		private X509ExtensionCollection extensions;

		public byte Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public byte[] SerialNumber
		{
			get
			{
				return sn;
			}
			set
			{
				sn = value;
			}
		}

		public string IssuerName
		{
			get
			{
				return issuer;
			}
			set
			{
				issuer = value;
			}
		}

		public DateTime NotBefore
		{
			get
			{
				return notBefore;
			}
			set
			{
				notBefore = value;
			}
		}

		public DateTime NotAfter
		{
			get
			{
				return notAfter;
			}
			set
			{
				notAfter = value;
			}
		}

		public string SubjectName
		{
			get
			{
				return subject;
			}
			set
			{
				subject = value;
			}
		}

		public AsymmetricAlgorithm SubjectPublicKey
		{
			get
			{
				return aa;
			}
			set
			{
				aa = value;
			}
		}

		public byte[] IssuerUniqueId
		{
			get
			{
				return issuerUniqueID;
			}
			set
			{
				issuerUniqueID = value;
			}
		}

		public byte[] SubjectUniqueId
		{
			get
			{
				return subjectUniqueID;
			}
			set
			{
				subjectUniqueID = value;
			}
		}

		public X509ExtensionCollection Extensions
		{
			get
			{
				return extensions;
			}
		}

		public X509CertificateBuilder()
			: this(3)
		{
		}

		public X509CertificateBuilder(byte version)
		{
			if (version > 3)
			{
				throw new ArgumentException("Invalid certificate version");
			}
			this.version = version;
			extensions = new X509ExtensionCollection();
		}

		private ASN1 SubjectPublicKeyInfo()
		{
			ASN1 aSN = new ASN1(48);
			if (aa is RSA)
			{
				aSN.Add(PKCS7.AlgorithmIdentifier("1.2.840.113549.1.1.1"));
				RSAParameters rSAParameters = (aa as RSA).ExportParameters(false);
				ASN1 aSN2 = new ASN1(48);
				aSN2.Add(ASN1Convert.FromUnsignedBigInteger(rSAParameters.Modulus));
				aSN2.Add(ASN1Convert.FromUnsignedBigInteger(rSAParameters.Exponent));
				aSN.Add(new ASN1(UniqueIdentifier(aSN2.GetBytes())));
			}
			else
			{
				if (!(aa is DSA))
				{
					throw new NotSupportedException("Unknown Asymmetric Algorithm " + aa.ToString());
				}
				DSAParameters dSAParameters = (aa as DSA).ExportParameters(false);
				ASN1 aSN3 = new ASN1(48);
				aSN3.Add(ASN1Convert.FromUnsignedBigInteger(dSAParameters.P));
				aSN3.Add(ASN1Convert.FromUnsignedBigInteger(dSAParameters.Q));
				aSN3.Add(ASN1Convert.FromUnsignedBigInteger(dSAParameters.G));
				aSN.Add(PKCS7.AlgorithmIdentifier("1.2.840.10040.4.1", aSN3));
				ASN1 aSN4 = aSN.Add(new ASN1(3));
				aSN4.Add(ASN1Convert.FromUnsignedBigInteger(dSAParameters.Y));
			}
			return aSN;
		}

		private byte[] UniqueIdentifier(byte[] id)
		{
			ASN1 aSN = new ASN1(3);
			byte[] array = new byte[id.Length + 1];
			Buffer.BlockCopy(id, 0, array, 1, id.Length);
			aSN.Value = array;
			return aSN.GetBytes();
		}

		protected override ASN1 ToBeSigned(string oid)
		{
			ASN1 aSN = new ASN1(48);
			if (version > 1)
			{
				byte[] data = new byte[1] { (byte)(version - 1) };
				ASN1 aSN2 = aSN.Add(new ASN1(160));
				aSN2.Add(new ASN1(2, data));
			}
			aSN.Add(new ASN1(2, sn));
			aSN.Add(PKCS7.AlgorithmIdentifier(oid));
			aSN.Add(X501.FromString(issuer));
			ASN1 aSN3 = aSN.Add(new ASN1(48));
			aSN3.Add(ASN1Convert.FromDateTime(notBefore));
			aSN3.Add(ASN1Convert.FromDateTime(notAfter));
			aSN.Add(X501.FromString(subject));
			aSN.Add(SubjectPublicKeyInfo());
			if (version > 1)
			{
				if (issuerUniqueID != null)
				{
					aSN.Add(new ASN1(161, UniqueIdentifier(issuerUniqueID)));
				}
				if (subjectUniqueID != null)
				{
					aSN.Add(new ASN1(161, UniqueIdentifier(subjectUniqueID)));
				}
				if (version > 2 && extensions.Count > 0)
				{
					aSN.Add(new ASN1(163, extensions.GetBytes()));
				}
			}
			return aSN;
		}
	}
}
