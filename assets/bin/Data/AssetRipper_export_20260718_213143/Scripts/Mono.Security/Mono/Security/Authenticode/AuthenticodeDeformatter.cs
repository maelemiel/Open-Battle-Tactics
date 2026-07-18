using System;
using System.Security;
using System.Security.Cryptography;
using Mono.Security.Cryptography;
using Mono.Security.X509;

namespace Mono.Security.Authenticode
{
	public class AuthenticodeDeformatter : AuthenticodeBase
	{
		private string filename;

		private byte[] hash;

		private X509CertificateCollection coll;

		private ASN1 signedHash;

		private DateTime timestamp;

		private X509Certificate signingCertificate;

		private int reason;

		private bool trustedRoot;

		private bool trustedTimestampRoot;

		private byte[] entry;

		private X509Chain signerChain;

		private X509Chain timestampChain;

		public string FileName
		{
			get
			{
				return filename;
			}
			set
			{
				Reset();
				try
				{
					CheckSignature(value);
				}
				catch (SecurityException)
				{
					throw;
				}
				catch (Exception)
				{
					reason = 1;
				}
			}
		}

		public byte[] Hash
		{
			get
			{
				if (signedHash == null)
				{
					return null;
				}
				return (byte[])signedHash.Value.Clone();
			}
		}

		public int Reason
		{
			get
			{
				if (reason == -1)
				{
					IsTrusted();
				}
				return reason;
			}
		}

		public byte[] Signature
		{
			get
			{
				if (entry == null)
				{
					return null;
				}
				return (byte[])entry.Clone();
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return timestamp;
			}
		}

		public X509CertificateCollection Certificates
		{
			get
			{
				return coll;
			}
		}

		public X509Certificate SigningCertificate
		{
			get
			{
				return signingCertificate;
			}
		}

		public AuthenticodeDeformatter()
		{
			reason = -1;
			signerChain = new X509Chain();
			timestampChain = new X509Chain();
		}

		public AuthenticodeDeformatter(string fileName)
			: this()
		{
			FileName = fileName;
		}

		public bool IsTrusted()
		{
			if (entry == null)
			{
				reason = 1;
				return false;
			}
			if (signingCertificate == null)
			{
				reason = 7;
				return false;
			}
			if (signerChain.Root == null || !trustedRoot)
			{
				reason = 6;
				return false;
			}
			if (timestamp != DateTime.MinValue)
			{
				if (timestampChain.Root == null || !trustedTimestampRoot)
				{
					reason = 6;
					return false;
				}
				if (!signingCertificate.WasCurrent(Timestamp))
				{
					reason = 4;
					return false;
				}
			}
			else if (!signingCertificate.IsCurrent)
			{
				reason = 8;
				return false;
			}
			if (reason == -1)
			{
				reason = 0;
			}
			return true;
		}

		private bool CheckSignature(string fileName)
		{
			filename = fileName;
			Open(filename);
			entry = GetSecurityEntry();
			if (entry == null)
			{
				reason = 1;
				Close();
				return false;
			}
			PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(entry);
			if (contentInfo.ContentType != "1.2.840.113549.1.7.2")
			{
				Close();
				return false;
			}
			PKCS7.SignedData signedData = new PKCS7.SignedData(contentInfo.Content);
			if (signedData.ContentInfo.ContentType != "1.3.6.1.4.1.311.2.1.4")
			{
				Close();
				return false;
			}
			coll = signedData.Certificates;
			ASN1 content = signedData.ContentInfo.Content;
			signedHash = content[0][1][1];
			HashAlgorithm hashAlgorithm = null;
			switch (signedHash.Length)
			{
			case 16:
				hashAlgorithm = HashAlgorithm.Create("MD5");
				hash = GetHash(hashAlgorithm);
				break;
			case 20:
				hashAlgorithm = HashAlgorithm.Create("SHA1");
				hash = GetHash(hashAlgorithm);
				break;
			default:
				reason = 5;
				Close();
				return false;
			}
			Close();
			if (!signedHash.CompareValue(hash))
			{
				reason = 2;
			}
			byte[] value = content[0].Value;
			hashAlgorithm.Initialize();
			byte[] calculatedMessageDigest = hashAlgorithm.ComputeHash(value);
			return VerifySignature(signedData, calculatedMessageDigest, hashAlgorithm) && reason == 0;
		}

		private bool CompareIssuerSerial(string issuer, byte[] serial, X509Certificate x509)
		{
			if (issuer != x509.IssuerName)
			{
				return false;
			}
			if (serial.Length != x509.SerialNumber.Length)
			{
				return false;
			}
			int num = serial.Length;
			for (int i = 0; i < serial.Length; i++)
			{
				if (serial[i] != x509.SerialNumber[--num])
				{
					return false;
				}
			}
			return true;
		}

		private bool VerifySignature(PKCS7.SignedData sd, byte[] calculatedMessageDigest, HashAlgorithm ha)
		{
			string text = null;
			ASN1 aSN = null;
			for (int i = 0; i < sd.SignerInfo.AuthenticatedAttributes.Count; i++)
			{
				ASN1 aSN2 = (ASN1)sd.SignerInfo.AuthenticatedAttributes[i];
				switch (ASN1Convert.ToOid(aSN2[0]))
				{
				case "1.2.840.113549.1.9.3":
					text = ASN1Convert.ToOid(aSN2[1][0]);
					break;
				case "1.2.840.113549.1.9.4":
					aSN = aSN2[1][0];
					break;
				}
			}
			if (text != "1.3.6.1.4.1.311.2.1.4")
			{
				return false;
			}
			if (aSN == null)
			{
				return false;
			}
			if (!aSN.CompareValue(calculatedMessageDigest))
			{
				return false;
			}
			string str = CryptoConfig.MapNameToOID(ha.ToString());
			ASN1 aSN3 = new ASN1(49);
			foreach (ASN1 authenticatedAttribute in sd.SignerInfo.AuthenticatedAttributes)
			{
				aSN3.Add(authenticatedAttribute);
			}
			ha.Initialize();
			byte[] rgbHash = ha.ComputeHash(aSN3.GetBytes());
			byte[] signature = sd.SignerInfo.Signature;
			string issuerName = sd.SignerInfo.IssuerName;
			byte[] serialNumber = sd.SignerInfo.SerialNumber;
			foreach (X509Certificate item in coll)
			{
				if (CompareIssuerSerial(issuerName, serialNumber, item) && item.PublicKey.Length > signature.Length >> 3)
				{
					signingCertificate = item;
					RSACryptoServiceProvider rSACryptoServiceProvider = (RSACryptoServiceProvider)item.RSA;
					if (rSACryptoServiceProvider.VerifyHash(rgbHash, str, signature))
					{
						signerChain.LoadCertificates(coll);
						trustedRoot = signerChain.Build(item);
						break;
					}
				}
			}
			if (sd.SignerInfo.UnauthenticatedAttributes.Count == 0)
			{
				trustedTimestampRoot = true;
			}
			else
			{
				for (int j = 0; j < sd.SignerInfo.UnauthenticatedAttributes.Count; j++)
				{
					ASN1 aSN4 = (ASN1)sd.SignerInfo.UnauthenticatedAttributes[j];
					switch (ASN1Convert.ToOid(aSN4[0]))
					{
					case "1.2.840.113549.1.9.6":
					{
						PKCS7.SignerInfo cs = new PKCS7.SignerInfo(aSN4[1]);
						trustedTimestampRoot = VerifyCounterSignature(cs, signature);
						break;
					}
					}
				}
			}
			return trustedRoot && trustedTimestampRoot;
		}

		private bool VerifyCounterSignature(PKCS7.SignerInfo cs, byte[] signature)
		{
			if (cs.Version != 1)
			{
				return false;
			}
			string text = null;
			ASN1 aSN = null;
			for (int i = 0; i < cs.AuthenticatedAttributes.Count; i++)
			{
				ASN1 aSN2 = (ASN1)cs.AuthenticatedAttributes[i];
				switch (ASN1Convert.ToOid(aSN2[0]))
				{
				case "1.2.840.113549.1.9.3":
					text = ASN1Convert.ToOid(aSN2[1][0]);
					break;
				case "1.2.840.113549.1.9.4":
					aSN = aSN2[1][0];
					break;
				case "1.2.840.113549.1.9.5":
					timestamp = ASN1Convert.ToDateTime(aSN2[1][0]);
					break;
				}
			}
			if (text != "1.2.840.113549.1.7.1")
			{
				return false;
			}
			if (aSN == null)
			{
				return false;
			}
			string hashName = null;
			switch (aSN.Length)
			{
			case 16:
				hashName = "MD5";
				break;
			case 20:
				hashName = "SHA1";
				break;
			}
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(hashName);
			if (!aSN.CompareValue(hashAlgorithm.ComputeHash(signature)))
			{
				return false;
			}
			byte[] signature2 = cs.Signature;
			ASN1 aSN3 = new ASN1(49);
			foreach (ASN1 authenticatedAttribute in cs.AuthenticatedAttributes)
			{
				aSN3.Add(authenticatedAttribute);
			}
			byte[] hashValue = hashAlgorithm.ComputeHash(aSN3.GetBytes());
			string issuerName = cs.IssuerName;
			byte[] serialNumber = cs.SerialNumber;
			foreach (X509Certificate item in coll)
			{
				if (CompareIssuerSerial(issuerName, serialNumber, item) && item.PublicKey.Length > signature2.Length)
				{
					RSACryptoServiceProvider rSACryptoServiceProvider = (RSACryptoServiceProvider)item.RSA;
					RSAManaged rSAManaged = new RSAManaged();
					rSAManaged.ImportParameters(rSACryptoServiceProvider.ExportParameters(false));
					if (PKCS1.Verify_v15(rSAManaged, hashAlgorithm, hashValue, signature2, true))
					{
						timestampChain.LoadCertificates(coll);
						return timestampChain.Build(item);
					}
				}
			}
			return false;
		}

		private void Reset()
		{
			filename = null;
			entry = null;
			hash = null;
			signedHash = null;
			signingCertificate = null;
			reason = -1;
			trustedRoot = false;
			trustedTimestampRoot = false;
			signerChain.Reset();
			timestampChain.Reset();
			timestamp = DateTime.MinValue;
		}
	}
}
