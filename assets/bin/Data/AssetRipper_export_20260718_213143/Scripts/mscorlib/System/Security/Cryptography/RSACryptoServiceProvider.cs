using System.IO;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RSACryptoServiceProvider : RSA, ICspAsymmetricAlgorithm
	{
		private const int PROV_RSA_FULL = 1;

		private KeyPairPersistence store;

		private bool persistKey;

		private bool persisted;

		private bool privateKeyExportable = true;

		private bool m_disposed;

		private RSAManaged rsa;

		private static bool useMachineKeyStore;

		public static bool UseMachineKeyStore
		{
			get
			{
				return useMachineKeyStore;
			}
			set
			{
				useMachineKeyStore = value;
			}
		}

		public override string KeyExchangeAlgorithm
		{
			get
			{
				return "RSA-PKCS1-KeyEx";
			}
		}

		public override int KeySize
		{
			get
			{
				if (rsa == null)
				{
					return KeySizeValue;
				}
				return rsa.KeySize;
			}
		}

		public bool PersistKeyInCsp
		{
			get
			{
				return persistKey;
			}
			set
			{
				persistKey = value;
				if (persistKey)
				{
					OnKeyGenerated(rsa, null);
				}
			}
		}

		[ComVisible(false)]
		public bool PublicOnly
		{
			get
			{
				return rsa.PublicOnly;
			}
		}

		public override string SignatureAlgorithm
		{
			get
			{
				return "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
			}
		}

		[ComVisible(false)]
		[MonoTODO("Always return null")]
		public CspKeyContainerInfo CspKeyContainerInfo
		{
			get
			{
				return null;
			}
		}

		public RSACryptoServiceProvider()
		{
			Common(1024, null);
		}

		public RSACryptoServiceProvider(CspParameters parameters)
		{
			Common(1024, parameters);
		}

		public RSACryptoServiceProvider(int dwKeySize)
		{
			Common(dwKeySize, null);
		}

		public RSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
		{
			Common(dwKeySize, parameters);
		}

		private void Common(int dwKeySize, CspParameters p)
		{
			LegalKeySizesValue = new KeySizes[1];
			LegalKeySizesValue[0] = new KeySizes(384, 16384, 8);
			base.KeySize = dwKeySize;
			rsa = new RSAManaged(KeySize);
			rsa.KeyGenerated += OnKeyGenerated;
			persistKey = p != null;
			if (p == null)
			{
				p = new CspParameters(1);
				if (useMachineKeyStore)
				{
					p.Flags |= CspProviderFlags.UseMachineKeyStore;
				}
				store = new KeyPairPersistence(p);
				return;
			}
			store = new KeyPairPersistence(p);
			store.Load();
			if (store.KeyValue != null)
			{
				persisted = true;
				FromXmlString(store.KeyValue);
			}
		}

		~RSACryptoServiceProvider()
		{
			Dispose(false);
		}

		public byte[] Decrypt(byte[] rgb, bool fOAEP)
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("rsa");
			}
			AsymmetricKeyExchangeDeformatter asymmetricKeyExchangeDeformatter = null;
			asymmetricKeyExchangeDeformatter = ((!fOAEP) ? ((AsymmetricKeyExchangeDeformatter)new RSAPKCS1KeyExchangeDeformatter(rsa)) : ((AsymmetricKeyExchangeDeformatter)new RSAOAEPKeyExchangeDeformatter(rsa)));
			return asymmetricKeyExchangeDeformatter.DecryptKeyExchange(rgb);
		}

		public override byte[] DecryptValue(byte[] rgb)
		{
			if (!rsa.IsCrtPossible)
			{
				throw new CryptographicException("Incomplete private key - missing CRT.");
			}
			return rsa.DecryptValue(rgb);
		}

		public byte[] Encrypt(byte[] rgb, bool fOAEP)
		{
			AsymmetricKeyExchangeFormatter asymmetricKeyExchangeFormatter = null;
			asymmetricKeyExchangeFormatter = ((!fOAEP) ? ((AsymmetricKeyExchangeFormatter)new RSAPKCS1KeyExchangeFormatter(rsa)) : ((AsymmetricKeyExchangeFormatter)new RSAOAEPKeyExchangeFormatter(rsa)));
			return asymmetricKeyExchangeFormatter.CreateKeyExchange(rgb);
		}

		public override byte[] EncryptValue(byte[] rgb)
		{
			return rsa.EncryptValue(rgb);
		}

		public override RSAParameters ExportParameters(bool includePrivateParameters)
		{
			if (includePrivateParameters && !privateKeyExportable)
			{
				throw new CryptographicException("cannot export private key");
			}
			return rsa.ExportParameters(includePrivateParameters);
		}

		public override void ImportParameters(RSAParameters parameters)
		{
			rsa.ImportParameters(parameters);
		}

		private HashAlgorithm GetHash(object halg)
		{
			if (halg == null)
			{
				throw new ArgumentNullException("halg");
			}
			HashAlgorithm hashAlgorithm = null;
			if (halg is string)
			{
				return HashAlgorithm.Create((string)halg);
			}
			if (halg is HashAlgorithm)
			{
				return (HashAlgorithm)halg;
			}
			if (halg is Type)
			{
				return (HashAlgorithm)Activator.CreateInstance((Type)halg);
			}
			throw new ArgumentException("halg");
		}

		public byte[] SignData(byte[] buffer, object halg)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return SignData(buffer, 0, buffer.Length, halg);
		}

		public byte[] SignData(Stream inputStream, object halg)
		{
			HashAlgorithm hash = GetHash(halg);
			byte[] hashValue = hash.ComputeHash(inputStream);
			return PKCS1.Sign_v15(this, hash, hashValue);
		}

		public byte[] SignData(byte[] buffer, int offset, int count, object halg)
		{
			HashAlgorithm hash = GetHash(halg);
			byte[] hashValue = hash.ComputeHash(buffer, offset, count);
			return PKCS1.Sign_v15(this, hash, hashValue);
		}

		private string GetHashNameFromOID(string oid)
		{
			switch (oid)
			{
			case "1.3.14.3.2.26":
				return "SHA1";
			case "1.2.840.113549.2.5":
				return "MD5";
			default:
				throw new NotSupportedException(oid + " is an unsupported hash algorithm for RSA signing");
			}
		}

		public byte[] SignHash(byte[] rgbHash, string str)
		{
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			string hashName = ((str != null) ? GetHashNameFromOID(str) : "SHA1");
			HashAlgorithm hash = HashAlgorithm.Create(hashName);
			return PKCS1.Sign_v15(this, hash, rgbHash);
		}

		public bool VerifyData(byte[] buffer, object halg, byte[] signature)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			HashAlgorithm hash = GetHash(halg);
			byte[] hashValue = hash.ComputeHash(buffer);
			return PKCS1.Verify_v15(this, hash, hashValue, signature);
		}

		public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
		{
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			if (rgbSignature == null)
			{
				throw new ArgumentNullException("rgbSignature");
			}
			string hashName = ((str != null) ? GetHashNameFromOID(str) : "SHA1");
			HashAlgorithm hash = HashAlgorithm.Create(hashName);
			return PKCS1.Verify_v15(this, hash, rgbHash, rgbSignature);
		}

		protected override void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (persisted && !persistKey)
				{
					store.Remove();
				}
				if (rsa != null)
				{
					rsa.Clear();
				}
				m_disposed = true;
			}
		}

		private void OnKeyGenerated(object sender, EventArgs e)
		{
			if (persistKey && !persisted)
			{
				store.KeyValue = ToXmlString(!rsa.PublicOnly);
				store.Save();
				persisted = true;
			}
		}

		[ComVisible(false)]
		public byte[] ExportCspBlob(bool includePrivateParameters)
		{
			byte[] array = null;
			array = ((!includePrivateParameters) ? CryptoConvert.ToCapiPublicKeyBlob(this) : CryptoConvert.ToCapiPrivateKeyBlob(this));
			array[5] = 164;
			return array;
		}

		[ComVisible(false)]
		public void ImportCspBlob(byte[] keyBlob)
		{
			if (keyBlob == null)
			{
				throw new ArgumentNullException("keyBlob");
			}
			RSA rSA = CryptoConvert.FromCapiKeyBlob(keyBlob);
			if (rSA is RSACryptoServiceProvider)
			{
				RSAParameters parameters = rSA.ExportParameters(!(rSA as RSACryptoServiceProvider).PublicOnly);
				ImportParameters(parameters);
				return;
			}
			try
			{
				RSAParameters parameters2 = rSA.ExportParameters(true);
				ImportParameters(parameters2);
			}
			catch
			{
				RSAParameters parameters3 = rSA.ExportParameters(false);
				ImportParameters(parameters3);
			}
		}
	}
}
