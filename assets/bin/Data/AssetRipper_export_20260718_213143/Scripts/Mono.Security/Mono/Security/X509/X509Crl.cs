using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using Mono.Security.X509.Extensions;

namespace Mono.Security.X509
{
	public class X509Crl
	{
		public class X509CrlEntry
		{
			private byte[] sn;

			private DateTime revocationDate;

			private X509ExtensionCollection extensions;

			public byte[] SerialNumber
			{
				get
				{
					return (byte[])sn.Clone();
				}
			}

			public DateTime RevocationDate
			{
				get
				{
					return revocationDate;
				}
			}

			public X509ExtensionCollection Extensions
			{
				get
				{
					return extensions;
				}
			}

			internal X509CrlEntry(byte[] serialNumber, DateTime revocationDate, X509ExtensionCollection extensions)
			{
				sn = serialNumber;
				this.revocationDate = revocationDate;
				if (extensions == null)
				{
					this.extensions = new X509ExtensionCollection();
				}
				else
				{
					this.extensions = extensions;
				}
			}

			internal X509CrlEntry(ASN1 entry)
			{
				sn = entry[0].Value;
				Array.Reverse(sn);
				revocationDate = ASN1Convert.ToDateTime(entry[1]);
				extensions = new X509ExtensionCollection(entry[2]);
			}

			public byte[] GetBytes()
			{
				ASN1 aSN = new ASN1(48);
				aSN.Add(new ASN1(2, sn));
				aSN.Add(ASN1Convert.FromDateTime(revocationDate));
				if (extensions.Count > 0)
				{
					aSN.Add(new ASN1(extensions.GetBytes()));
				}
				return aSN.GetBytes();
			}
		}

		private string issuer;

		private byte version;

		private DateTime thisUpdate;

		private DateTime nextUpdate;

		private ArrayList entries;

		private string signatureOID;

		private byte[] signature;

		private X509ExtensionCollection extensions;

		private byte[] encoded;

		private byte[] hash_value;

		public ArrayList Entries
		{
			get
			{
				return ArrayList.ReadOnly(entries);
			}
		}

		public X509CrlEntry this[int index]
		{
			get
			{
				return (X509CrlEntry)entries[index];
			}
		}

		public X509CrlEntry this[byte[] serialNumber]
		{
			get
			{
				return GetCrlEntry(serialNumber);
			}
		}

		public X509ExtensionCollection Extensions
		{
			get
			{
				return extensions;
			}
		}

		public byte[] Hash
		{
			get
			{
				if (hash_value == null)
				{
					ASN1 aSN = new ASN1(encoded);
					byte[] bytes = aSN[0].GetBytes();
					HashAlgorithm hashAlgorithm = HashAlgorithm.Create(GetHashName());
					hash_value = hashAlgorithm.ComputeHash(bytes);
				}
				return hash_value;
			}
		}

		public string IssuerName
		{
			get
			{
				return issuer;
			}
		}

		public DateTime NextUpdate
		{
			get
			{
				return nextUpdate;
			}
		}

		public DateTime ThisUpdate
		{
			get
			{
				return thisUpdate;
			}
		}

		public string SignatureAlgorithm
		{
			get
			{
				return signatureOID;
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
		}

		public byte[] RawData
		{
			get
			{
				return (byte[])encoded.Clone();
			}
		}

		public byte Version
		{
			get
			{
				return version;
			}
		}

		public bool IsCurrent
		{
			get
			{
				return WasCurrent(DateTime.Now);
			}
		}

		public X509Crl(byte[] crl)
		{
			if (crl == null)
			{
				throw new ArgumentNullException("crl");
			}
			encoded = (byte[])crl.Clone();
			Parse(encoded);
		}

		private void Parse(byte[] crl)
		{
			string text = "Input data cannot be coded as a valid CRL.";
			try
			{
				ASN1 aSN = new ASN1(encoded);
				if (aSN.Tag != 48 || aSN.Count != 3)
				{
					throw new CryptographicException(text);
				}
				ASN1 aSN2 = aSN[0];
				if (aSN2.Tag != 48 || aSN2.Count < 3)
				{
					throw new CryptographicException(text);
				}
				int num = 0;
				if (aSN2[num].Tag == 2)
				{
					version = (byte)(aSN2[num++].Value[0] + 1);
				}
				else
				{
					version = 1;
				}
				signatureOID = ASN1Convert.ToOid(aSN2[num++][0]);
				issuer = X501.ToString(aSN2[num++]);
				thisUpdate = ASN1Convert.ToDateTime(aSN2[num++]);
				ASN1 aSN3 = aSN2[num++];
				if (aSN3.Tag == 23 || aSN3.Tag == 24)
				{
					nextUpdate = ASN1Convert.ToDateTime(aSN3);
					aSN3 = aSN2[num++];
				}
				entries = new ArrayList();
				if (aSN3 != null && aSN3.Tag == 48)
				{
					ASN1 aSN4 = aSN3;
					for (int i = 0; i < aSN4.Count; i++)
					{
						entries.Add(new X509CrlEntry(aSN4[i]));
					}
				}
				else
				{
					num--;
				}
				ASN1 aSN5 = aSN2[num];
				if (aSN5 != null && aSN5.Tag == 160 && aSN5.Count == 1)
				{
					extensions = new X509ExtensionCollection(aSN5[0]);
				}
				else
				{
					extensions = new X509ExtensionCollection(null);
				}
				string text2 = ASN1Convert.ToOid(aSN[1][0]);
				if (signatureOID != text2)
				{
					throw new CryptographicException(text + " [Non-matching signature algorithms in CRL]");
				}
				byte[] value = aSN[2].Value;
				signature = new byte[value.Length - 1];
				Buffer.BlockCopy(value, 1, signature, 0, signature.Length);
			}
			catch
			{
				throw new CryptographicException(text);
			}
		}

		public bool WasCurrent(DateTime instant)
		{
			if (nextUpdate == DateTime.MinValue)
			{
				return instant >= thisUpdate;
			}
			return instant >= thisUpdate && instant <= nextUpdate;
		}

		public byte[] GetBytes()
		{
			return (byte[])encoded.Clone();
		}

		private bool Compare(byte[] array1, byte[] array2)
		{
			if (array1 == null && array2 == null)
			{
				return true;
			}
			if (array1 == null || array2 == null)
			{
				return false;
			}
			if (array1.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		public X509CrlEntry GetCrlEntry(X509Certificate x509)
		{
			if (x509 == null)
			{
				throw new ArgumentNullException("x509");
			}
			return GetCrlEntry(x509.SerialNumber);
		}

		public X509CrlEntry GetCrlEntry(byte[] serialNumber)
		{
			if (serialNumber == null)
			{
				throw new ArgumentNullException("serialNumber");
			}
			for (int i = 0; i < entries.Count; i++)
			{
				X509CrlEntry x509CrlEntry = (X509CrlEntry)entries[i];
				if (Compare(serialNumber, x509CrlEntry.SerialNumber))
				{
					return x509CrlEntry;
				}
			}
			return null;
		}

		public bool VerifySignature(X509Certificate x509)
		{
			if (x509 == null)
			{
				throw new ArgumentNullException("x509");
			}
			if (x509.Version >= 3)
			{
				X509Extension x509Extension = x509.Extensions["2.5.29.15"];
				if (x509Extension != null)
				{
					KeyUsageExtension keyUsageExtension = new KeyUsageExtension(x509Extension);
					if (!keyUsageExtension.Support(KeyUsages.cRLSign))
					{
						return false;
					}
				}
				x509Extension = x509.Extensions["2.5.29.19"];
				if (x509Extension != null)
				{
					BasicConstraintsExtension basicConstraintsExtension = new BasicConstraintsExtension(x509Extension);
					if (!basicConstraintsExtension.CertificateAuthority)
					{
						return false;
					}
				}
			}
			if (issuer != x509.SubjectName)
			{
				return false;
			}
			switch (signatureOID)
			{
			case "1.2.840.10040.4.3":
				return VerifySignature(x509.DSA);
			default:
				return VerifySignature(x509.RSA);
			}
		}

		private string GetHashName()
		{
			switch (signatureOID)
			{
			case "1.2.840.113549.1.1.2":
				return "MD2";
			case "1.2.840.113549.1.1.4":
				return "MD5";
			case "1.2.840.10040.4.3":
			case "1.2.840.113549.1.1.5":
				return "SHA1";
			default:
				throw new CryptographicException("Unsupported hash algorithm: " + signatureOID);
			}
		}

		internal bool VerifySignature(DSA dsa)
		{
			if (signatureOID != "1.2.840.10040.4.3")
			{
				throw new CryptographicException("Unsupported hash algorithm: " + signatureOID);
			}
			DSASignatureDeformatter dSASignatureDeformatter = new DSASignatureDeformatter(dsa);
			dSASignatureDeformatter.SetHashAlgorithm("SHA1");
			ASN1 aSN = new ASN1(signature);
			if (aSN == null || aSN.Count != 2)
			{
				return false;
			}
			byte[] value = aSN[0].Value;
			byte[] value2 = aSN[1].Value;
			byte[] array = new byte[40];
			int num = System.Math.Max(0, value.Length - 20);
			int dstOffset = System.Math.Max(0, 20 - value.Length);
			Buffer.BlockCopy(value, num, array, dstOffset, value.Length - num);
			int num2 = System.Math.Max(0, value2.Length - 20);
			int dstOffset2 = System.Math.Max(20, 40 - value2.Length);
			Buffer.BlockCopy(value2, num2, array, dstOffset2, value2.Length - num2);
			return dSASignatureDeformatter.VerifySignature(Hash, array);
		}

		internal bool VerifySignature(RSA rsa)
		{
			RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
			rSAPKCS1SignatureDeformatter.SetHashAlgorithm(GetHashName());
			return rSAPKCS1SignatureDeformatter.VerifySignature(Hash, signature);
		}

		public bool VerifySignature(AsymmetricAlgorithm aa)
		{
			if (aa == null)
			{
				throw new ArgumentNullException("aa");
			}
			if (aa is RSA)
			{
				return VerifySignature(aa as RSA);
			}
			if (aa is DSA)
			{
				return VerifySignature(aa as DSA);
			}
			throw new NotSupportedException("Unknown Asymmetric Algorithm " + aa.ToString());
		}

		public static X509Crl CreateFromFile(string filename)
		{
			byte[] array = null;
			using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
			}
			return new X509Crl(array);
		}
	}
}
