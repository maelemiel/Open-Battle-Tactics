using System;
using System.Collections;
using System.Security.Cryptography;
using Mono.Security.X509;

namespace Mono.Security
{
	internal sealed class PKCS7
	{
		public class Oid
		{
			public const string rsaEncryption = "1.2.840.113549.1.1.1";

			public const string data = "1.2.840.113549.1.7.1";

			public const string signedData = "1.2.840.113549.1.7.2";

			public const string envelopedData = "1.2.840.113549.1.7.3";

			public const string signedAndEnvelopedData = "1.2.840.113549.1.7.4";

			public const string digestedData = "1.2.840.113549.1.7.5";

			public const string encryptedData = "1.2.840.113549.1.7.6";

			public const string contentType = "1.2.840.113549.1.9.3";

			public const string messageDigest = "1.2.840.113549.1.9.4";

			public const string signingTime = "1.2.840.113549.1.9.5";

			public const string countersignature = "1.2.840.113549.1.9.6";
		}

		public class ContentInfo
		{
			private string contentType;

			private ASN1 content;

			public ASN1 ASN1
			{
				get
				{
					return GetASN1();
				}
			}

			public ASN1 Content
			{
				get
				{
					return content;
				}
				set
				{
					content = value;
				}
			}

			public string ContentType
			{
				get
				{
					return contentType;
				}
				set
				{
					contentType = value;
				}
			}

			public ContentInfo()
			{
				content = new ASN1(160);
			}

			public ContentInfo(string oid)
				: this()
			{
				contentType = oid;
			}

			public ContentInfo(byte[] data)
				: this(new ASN1(data))
			{
			}

			public ContentInfo(ASN1 asn1)
			{
				if (asn1.Tag != 48 || (asn1.Count < 1 && asn1.Count > 2))
				{
					throw new ArgumentException("Invalid ASN1");
				}
				if (asn1[0].Tag != 6)
				{
					throw new ArgumentException("Invalid contentType");
				}
				contentType = ASN1Convert.ToOid(asn1[0]);
				if (asn1.Count > 1)
				{
					if (asn1[1].Tag != 160)
					{
						throw new ArgumentException("Invalid content");
					}
					content = asn1[1];
				}
			}

			internal ASN1 GetASN1()
			{
				ASN1 aSN = new ASN1(48);
				aSN.Add(ASN1Convert.FromOid(contentType));
				if (content != null && content.Count > 0)
				{
					aSN.Add(content);
				}
				return aSN;
			}

			public byte[] GetBytes()
			{
				return GetASN1().GetBytes();
			}
		}

		public class EncryptedData
		{
			private byte _version;

			private ContentInfo _content;

			private ContentInfo _encryptionAlgorithm;

			private byte[] _encrypted;

			public ASN1 ASN1
			{
				get
				{
					return GetASN1();
				}
			}

			public ContentInfo ContentInfo
			{
				get
				{
					return _content;
				}
			}

			public ContentInfo EncryptionAlgorithm
			{
				get
				{
					return _encryptionAlgorithm;
				}
			}

			public byte[] EncryptedContent
			{
				get
				{
					if (_encrypted == null)
					{
						return null;
					}
					return (byte[])_encrypted.Clone();
				}
			}

			public byte Version
			{
				get
				{
					return _version;
				}
				set
				{
					_version = value;
				}
			}

			public EncryptedData()
			{
				_version = 0;
			}

			public EncryptedData(byte[] data)
				: this(new ASN1(data))
			{
			}

			public EncryptedData(ASN1 asn1)
				: this()
			{
				if (asn1.Tag != 48 || asn1.Count < 2)
				{
					throw new ArgumentException("Invalid EncryptedData");
				}
				if (asn1[0].Tag != 2)
				{
					throw new ArgumentException("Invalid version");
				}
				_version = asn1[0].Value[0];
				ASN1 aSN = asn1[1];
				if (aSN.Tag != 48)
				{
					throw new ArgumentException("missing EncryptedContentInfo");
				}
				ASN1 aSN2 = aSN[0];
				if (aSN2.Tag != 6)
				{
					throw new ArgumentException("missing EncryptedContentInfo.ContentType");
				}
				_content = new ContentInfo(ASN1Convert.ToOid(aSN2));
				ASN1 aSN3 = aSN[1];
				if (aSN3.Tag != 48)
				{
					throw new ArgumentException("missing EncryptedContentInfo.ContentEncryptionAlgorithmIdentifier");
				}
				_encryptionAlgorithm = new ContentInfo(ASN1Convert.ToOid(aSN3[0]));
				_encryptionAlgorithm.Content = aSN3[1];
				ASN1 aSN4 = aSN[2];
				if (aSN4.Tag != 128)
				{
					throw new ArgumentException("missing EncryptedContentInfo.EncryptedContent");
				}
				_encrypted = aSN4.Value;
			}

			internal ASN1 GetASN1()
			{
				return null;
			}

			public byte[] GetBytes()
			{
				return GetASN1().GetBytes();
			}
		}

		public class EnvelopedData
		{
			private byte _version;

			private ContentInfo _content;

			private ContentInfo _encryptionAlgorithm;

			private ArrayList _recipientInfos;

			private byte[] _encrypted;

			public ArrayList RecipientInfos
			{
				get
				{
					return _recipientInfos;
				}
			}

			public ASN1 ASN1
			{
				get
				{
					return GetASN1();
				}
			}

			public ContentInfo ContentInfo
			{
				get
				{
					return _content;
				}
			}

			public ContentInfo EncryptionAlgorithm
			{
				get
				{
					return _encryptionAlgorithm;
				}
			}

			public byte[] EncryptedContent
			{
				get
				{
					if (_encrypted == null)
					{
						return null;
					}
					return (byte[])_encrypted.Clone();
				}
			}

			public byte Version
			{
				get
				{
					return _version;
				}
				set
				{
					_version = value;
				}
			}

			public EnvelopedData()
			{
				_version = 0;
				_content = new ContentInfo();
				_encryptionAlgorithm = new ContentInfo();
				_recipientInfos = new ArrayList();
			}

			public EnvelopedData(byte[] data)
				: this(new ASN1(data))
			{
			}

			public EnvelopedData(ASN1 asn1)
				: this()
			{
				if (asn1[0].Tag != 48 || asn1[0].Count < 3)
				{
					throw new ArgumentException("Invalid EnvelopedData");
				}
				if (asn1[0][0].Tag != 2)
				{
					throw new ArgumentException("Invalid version");
				}
				_version = asn1[0][0].Value[0];
				ASN1 aSN = asn1[0][1];
				if (aSN.Tag != 49)
				{
					throw new ArgumentException("missing RecipientInfos");
				}
				for (int i = 0; i < aSN.Count; i++)
				{
					ASN1 data = aSN[i];
					_recipientInfos.Add(new RecipientInfo(data));
				}
				ASN1 aSN2 = asn1[0][2];
				if (aSN2.Tag != 48)
				{
					throw new ArgumentException("missing EncryptedContentInfo");
				}
				ASN1 aSN3 = aSN2[0];
				if (aSN3.Tag != 6)
				{
					throw new ArgumentException("missing EncryptedContentInfo.ContentType");
				}
				_content = new ContentInfo(ASN1Convert.ToOid(aSN3));
				ASN1 aSN4 = aSN2[1];
				if (aSN4.Tag != 48)
				{
					throw new ArgumentException("missing EncryptedContentInfo.ContentEncryptionAlgorithmIdentifier");
				}
				_encryptionAlgorithm = new ContentInfo(ASN1Convert.ToOid(aSN4[0]));
				_encryptionAlgorithm.Content = aSN4[1];
				ASN1 aSN5 = aSN2[2];
				if (aSN5.Tag != 128)
				{
					throw new ArgumentException("missing EncryptedContentInfo.EncryptedContent");
				}
				_encrypted = aSN5.Value;
			}

			internal ASN1 GetASN1()
			{
				return new ASN1(48);
			}

			public byte[] GetBytes()
			{
				return GetASN1().GetBytes();
			}
		}

		public class RecipientInfo
		{
			private int _version;

			private string _oid;

			private byte[] _key;

			private byte[] _ski;

			private string _issuer;

			private byte[] _serial;

			public string Oid
			{
				get
				{
					return _oid;
				}
			}

			public byte[] Key
			{
				get
				{
					if (_key == null)
					{
						return null;
					}
					return (byte[])_key.Clone();
				}
			}

			public byte[] SubjectKeyIdentifier
			{
				get
				{
					if (_ski == null)
					{
						return null;
					}
					return (byte[])_ski.Clone();
				}
			}

			public string Issuer
			{
				get
				{
					return _issuer;
				}
			}

			public byte[] Serial
			{
				get
				{
					if (_serial == null)
					{
						return null;
					}
					return (byte[])_serial.Clone();
				}
			}

			public int Version
			{
				get
				{
					return _version;
				}
			}

			public RecipientInfo()
			{
			}

			public RecipientInfo(ASN1 data)
			{
				if (data.Tag != 48)
				{
					throw new ArgumentException("Invalid RecipientInfo");
				}
				ASN1 aSN = data[0];
				if (aSN.Tag != 2)
				{
					throw new ArgumentException("missing Version");
				}
				_version = aSN.Value[0];
				ASN1 aSN2 = data[1];
				if (aSN2.Tag == 128 && _version == 3)
				{
					_ski = aSN2.Value;
				}
				else
				{
					_issuer = X501.ToString(aSN2[0]);
					_serial = aSN2[1].Value;
				}
				ASN1 aSN3 = data[2];
				_oid = ASN1Convert.ToOid(aSN3[0]);
				ASN1 aSN4 = data[3];
				_key = aSN4.Value;
			}
		}

		public class SignedData
		{
			private byte version;

			private string hashAlgorithm;

			private ContentInfo contentInfo;

			private X509CertificateCollection certs;

			private ArrayList crls;

			private SignerInfo signerInfo;

			private bool mda;

			private bool signed;

			public ASN1 ASN1
			{
				get
				{
					return GetASN1();
				}
			}

			public X509CertificateCollection Certificates
			{
				get
				{
					return certs;
				}
			}

			public ContentInfo ContentInfo
			{
				get
				{
					return contentInfo;
				}
			}

			public ArrayList Crls
			{
				get
				{
					return crls;
				}
			}

			public string HashName
			{
				get
				{
					return hashAlgorithm;
				}
				set
				{
					hashAlgorithm = value;
					signerInfo.HashName = value;
				}
			}

			public SignerInfo SignerInfo
			{
				get
				{
					return signerInfo;
				}
			}

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

			public bool UseAuthenticatedAttributes
			{
				get
				{
					return mda;
				}
				set
				{
					mda = value;
				}
			}

			public SignedData()
			{
				version = 1;
				contentInfo = new ContentInfo();
				certs = new X509CertificateCollection();
				crls = new ArrayList();
				signerInfo = new SignerInfo();
				mda = true;
				signed = false;
			}

			public SignedData(byte[] data)
				: this(new ASN1(data))
			{
			}

			public SignedData(ASN1 asn1)
			{
				if (asn1[0].Tag != 48 || asn1[0].Count < 4)
				{
					throw new ArgumentException("Invalid SignedData");
				}
				if (asn1[0][0].Tag != 2)
				{
					throw new ArgumentException("Invalid version");
				}
				version = asn1[0][0].Value[0];
				contentInfo = new ContentInfo(asn1[0][2]);
				int num = 3;
				certs = new X509CertificateCollection();
				if (asn1[0][num].Tag == 160)
				{
					for (int i = 0; i < asn1[0][num].Count; i++)
					{
						certs.Add(new X509Certificate(asn1[0][num][i].GetBytes()));
					}
					num++;
				}
				crls = new ArrayList();
				if (asn1[0][num].Tag == 161)
				{
					for (int j = 0; j < asn1[0][num].Count; j++)
					{
						crls.Add(asn1[0][num][j].GetBytes());
					}
					num++;
				}
				if (asn1[0][num].Count > 0)
				{
					signerInfo = new SignerInfo(asn1[0][num]);
				}
				else
				{
					signerInfo = new SignerInfo();
				}
				if (signerInfo.HashName != null)
				{
					HashName = OidToName(signerInfo.HashName);
				}
				mda = signerInfo.AuthenticatedAttributes.Count > 0;
			}

			public bool VerifySignature(AsymmetricAlgorithm aa)
			{
				if (aa == null)
				{
					return false;
				}
				RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(aa);
				rSAPKCS1SignatureDeformatter.SetHashAlgorithm(this.hashAlgorithm);
				HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.hashAlgorithm);
				byte[] signature = signerInfo.Signature;
				byte[] array = null;
				if (mda)
				{
					ASN1 aSN = new ASN1(49);
					foreach (ASN1 authenticatedAttribute in signerInfo.AuthenticatedAttributes)
					{
						aSN.Add(authenticatedAttribute);
					}
					array = hashAlgorithm.ComputeHash(aSN.GetBytes());
				}
				else
				{
					array = hashAlgorithm.ComputeHash(contentInfo.Content[0].Value);
				}
				if (array != null && signature != null)
				{
					return rSAPKCS1SignatureDeformatter.VerifySignature(array, signature);
				}
				return false;
			}

			internal string OidToName(string oid)
			{
				switch (oid)
				{
				case "1.3.14.3.2.26":
					return "SHA1";
				case "1.2.840.113549.2.2":
					return "MD2";
				case "1.2.840.113549.2.5":
					return "MD5";
				case "2.16.840.1.101.3.4.1":
					return "SHA256";
				case "2.16.840.1.101.3.4.2":
					return "SHA384";
				case "2.16.840.1.101.3.4.3":
					return "SHA512";
				default:
					return oid;
				}
			}

			internal ASN1 GetASN1()
			{
				ASN1 aSN = new ASN1(48);
				byte[] data = new byte[1] { version };
				aSN.Add(new ASN1(2, data));
				ASN1 aSN2 = aSN.Add(new ASN1(49));
				if (this.hashAlgorithm != null)
				{
					string oid = CryptoConfig.MapNameToOID(this.hashAlgorithm);
					aSN2.Add(AlgorithmIdentifier(oid));
				}
				ASN1 aSN3 = contentInfo.ASN1;
				aSN.Add(aSN3);
				if (!signed && this.hashAlgorithm != null)
				{
					if (mda)
					{
						ASN1 value = Attribute("1.2.840.113549.1.9.3", aSN3[0]);
						signerInfo.AuthenticatedAttributes.Add(value);
						HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.hashAlgorithm);
						byte[] data2 = hashAlgorithm.ComputeHash(aSN3[1][0].Value);
						ASN1 aSN4 = new ASN1(48);
						ASN1 value2 = Attribute("1.2.840.113549.1.9.4", aSN4.Add(new ASN1(4, data2)));
						signerInfo.AuthenticatedAttributes.Add(value2);
					}
					else
					{
						RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(signerInfo.Key);
						rSAPKCS1SignatureFormatter.SetHashAlgorithm(this.hashAlgorithm);
						HashAlgorithm hashAlgorithm2 = HashAlgorithm.Create(this.hashAlgorithm);
						byte[] rgbHash = hashAlgorithm2.ComputeHash(aSN3[1][0].Value);
						signerInfo.Signature = rSAPKCS1SignatureFormatter.CreateSignature(rgbHash);
					}
					signed = true;
				}
				if (certs.Count > 0)
				{
					ASN1 aSN5 = aSN.Add(new ASN1(160));
					foreach (X509Certificate cert in certs)
					{
						aSN5.Add(new ASN1(cert.RawData));
					}
				}
				if (crls.Count > 0)
				{
					ASN1 aSN6 = aSN.Add(new ASN1(161));
					foreach (byte[] crl in crls)
					{
						aSN6.Add(new ASN1(crl));
					}
				}
				ASN1 aSN7 = aSN.Add(new ASN1(49));
				if (signerInfo.Key != null)
				{
					aSN7.Add(signerInfo.ASN1);
				}
				return aSN;
			}

			public byte[] GetBytes()
			{
				return GetASN1().GetBytes();
			}
		}

		public class SignerInfo
		{
			private byte version;

			private X509Certificate x509;

			private string hashAlgorithm;

			private AsymmetricAlgorithm key;

			private ArrayList authenticatedAttributes;

			private ArrayList unauthenticatedAttributes;

			private byte[] signature;

			private string issuer;

			private byte[] serial;

			private byte[] ski;

			public string IssuerName
			{
				get
				{
					return issuer;
				}
			}

			public byte[] SerialNumber
			{
				get
				{
					if (serial == null)
					{
						return null;
					}
					return (byte[])serial.Clone();
				}
			}

			public byte[] SubjectKeyIdentifier
			{
				get
				{
					if (ski == null)
					{
						return null;
					}
					return (byte[])ski.Clone();
				}
			}

			public ASN1 ASN1
			{
				get
				{
					return GetASN1();
				}
			}

			public ArrayList AuthenticatedAttributes
			{
				get
				{
					return authenticatedAttributes;
				}
			}

			public X509Certificate Certificate
			{
				get
				{
					return x509;
				}
				set
				{
					x509 = value;
				}
			}

			public string HashName
			{
				get
				{
					return hashAlgorithm;
				}
				set
				{
					hashAlgorithm = value;
				}
			}

			public AsymmetricAlgorithm Key
			{
				get
				{
					return key;
				}
				set
				{
					key = value;
				}
			}

			public byte[] Signature
			{
				get
				{
					if (signature == null)
					{
						return null;
					}
					return (byte[])signature.Clone();
				}
				set
				{
					if (value != null)
					{
						signature = (byte[])value.Clone();
					}
				}
			}

			public ArrayList UnauthenticatedAttributes
			{
				get
				{
					return unauthenticatedAttributes;
				}
			}

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

			public SignerInfo()
			{
				version = 1;
				authenticatedAttributes = new ArrayList();
				unauthenticatedAttributes = new ArrayList();
			}

			public SignerInfo(byte[] data)
				: this(new ASN1(data))
			{
			}

			public SignerInfo(ASN1 asn1)
				: this()
			{
				if (asn1[0].Tag != 48 || asn1[0].Count < 5)
				{
					throw new ArgumentException("Invalid SignedData");
				}
				if (asn1[0][0].Tag != 2)
				{
					throw new ArgumentException("Invalid version");
				}
				version = asn1[0][0].Value[0];
				ASN1 aSN = asn1[0][1];
				if (aSN.Tag == 128 && version == 3)
				{
					ski = aSN.Value;
				}
				else
				{
					issuer = X501.ToString(aSN[0]);
					serial = aSN[1].Value;
				}
				ASN1 aSN2 = asn1[0][2];
				hashAlgorithm = ASN1Convert.ToOid(aSN2[0]);
				int num = 3;
				ASN1 aSN3 = asn1[0][num];
				if (aSN3.Tag == 160)
				{
					num++;
					for (int i = 0; i < aSN3.Count; i++)
					{
						authenticatedAttributes.Add(aSN3[i]);
					}
				}
				num++;
				ASN1 aSN4 = asn1[0][num++];
				if (aSN4.Tag == 4)
				{
					signature = aSN4.Value;
				}
				ASN1 aSN5 = asn1[0][num];
				if (aSN5 != null && aSN5.Tag == 161)
				{
					for (int j = 0; j < aSN5.Count; j++)
					{
						unauthenticatedAttributes.Add(aSN5[j]);
					}
				}
			}

			internal ASN1 GetASN1()
			{
				if (key == null || this.hashAlgorithm == null)
				{
					return null;
				}
				byte[] data = new byte[1] { version };
				ASN1 aSN = new ASN1(48);
				aSN.Add(new ASN1(2, data));
				aSN.Add(IssuerAndSerialNumber(x509));
				string oid = CryptoConfig.MapNameToOID(this.hashAlgorithm);
				aSN.Add(AlgorithmIdentifier(oid));
				ASN1 aSN2 = null;
				if (authenticatedAttributes.Count > 0)
				{
					aSN2 = aSN.Add(new ASN1(160));
					foreach (ASN1 authenticatedAttribute in authenticatedAttributes)
					{
						aSN2.Add(authenticatedAttribute);
					}
				}
				if (key is RSA)
				{
					aSN.Add(AlgorithmIdentifier("1.2.840.113549.1.1.1"));
					if (aSN2 != null)
					{
						RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(key);
						rSAPKCS1SignatureFormatter.SetHashAlgorithm(this.hashAlgorithm);
						byte[] bytes = aSN2.GetBytes();
						bytes[0] = 49;
						HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.hashAlgorithm);
						byte[] rgbHash = hashAlgorithm.ComputeHash(bytes);
						signature = rSAPKCS1SignatureFormatter.CreateSignature(rgbHash);
					}
					aSN.Add(new ASN1(4, signature));
					if (unauthenticatedAttributes.Count > 0)
					{
						ASN1 aSN3 = aSN.Add(new ASN1(161));
						foreach (ASN1 unauthenticatedAttribute in unauthenticatedAttributes)
						{
							aSN3.Add(unauthenticatedAttribute);
						}
					}
					return aSN;
				}
				if (key is DSA)
				{
					throw new NotImplementedException("not yet");
				}
				throw new CryptographicException("Unknown assymetric algorithm");
			}

			public byte[] GetBytes()
			{
				return GetASN1().GetBytes();
			}
		}

		private PKCS7()
		{
		}

		public static ASN1 Attribute(string oid, ASN1 value)
		{
			ASN1 aSN = new ASN1(48);
			aSN.Add(ASN1Convert.FromOid(oid));
			ASN1 aSN2 = aSN.Add(new ASN1(49));
			aSN2.Add(value);
			return aSN;
		}

		public static ASN1 AlgorithmIdentifier(string oid)
		{
			ASN1 aSN = new ASN1(48);
			aSN.Add(ASN1Convert.FromOid(oid));
			aSN.Add(new ASN1(5));
			return aSN;
		}

		public static ASN1 AlgorithmIdentifier(string oid, ASN1 parameters)
		{
			ASN1 aSN = new ASN1(48);
			aSN.Add(ASN1Convert.FromOid(oid));
			aSN.Add(parameters);
			return aSN;
		}

		public static ASN1 IssuerAndSerialNumber(X509Certificate x509)
		{
			ASN1 asn = null;
			ASN1 asn2 = null;
			ASN1 aSN = new ASN1(x509.RawData);
			int num = 0;
			bool flag = false;
			while (num < aSN[0].Count)
			{
				ASN1 aSN2 = aSN[0][num++];
				if (aSN2.Tag == 2)
				{
					asn2 = aSN2;
				}
				else if (aSN2.Tag == 48)
				{
					if (flag)
					{
						asn = aSN2;
						break;
					}
					flag = true;
				}
			}
			ASN1 aSN3 = new ASN1(48);
			aSN3.Add(asn);
			aSN3.Add(asn2);
			return aSN3;
		}
	}
}
