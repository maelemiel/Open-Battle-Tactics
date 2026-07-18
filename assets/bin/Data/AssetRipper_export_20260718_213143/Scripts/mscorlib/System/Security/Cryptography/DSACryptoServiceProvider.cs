using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class DSACryptoServiceProvider : DSA, ICspAsymmetricAlgorithm
	{
		private const int PROV_DSS_DH = 13;

		private KeyPairPersistence store;

		private bool persistKey;

		private bool persisted;

		private bool privateKeyExportable = true;

		private bool m_disposed;

		private DSAManaged dsa;

		private static bool useMachineKeyStore;

		public override string KeyExchangeAlgorithm
		{
			get
			{
				return null;
			}
		}

		public override int KeySize
		{
			get
			{
				return dsa.KeySize;
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
			}
		}

		[ComVisible(false)]
		public bool PublicOnly
		{
			get
			{
				return dsa.PublicOnly;
			}
		}

		public override string SignatureAlgorithm
		{
			get
			{
				return "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
			}
		}

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

		[ComVisible(false)]
		[MonoTODO("call into KeyPairPersistence to get details")]
		public CspKeyContainerInfo CspKeyContainerInfo
		{
			get
			{
				return null;
			}
		}

		public DSACryptoServiceProvider()
			: this(1024, null)
		{
		}

		public DSACryptoServiceProvider(CspParameters parameters)
			: this(1024, parameters)
		{
		}

		public DSACryptoServiceProvider(int dwKeySize)
			: this(dwKeySize, null)
		{
		}

		public DSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
		{
			LegalKeySizesValue = new KeySizes[1];
			LegalKeySizesValue[0] = new KeySizes(512, 1024, 64);
			KeySize = dwKeySize;
			dsa = new DSAManaged(dwKeySize);
			dsa.KeyGenerated += OnKeyGenerated;
			persistKey = parameters != null;
			if (parameters == null)
			{
				parameters = new CspParameters(13);
				if (useMachineKeyStore)
				{
					parameters.Flags |= CspProviderFlags.UseMachineKeyStore;
				}
				store = new KeyPairPersistence(parameters);
				return;
			}
			store = new KeyPairPersistence(parameters);
			store.Load();
			if (store.KeyValue != null)
			{
				persisted = true;
				FromXmlString(store.KeyValue);
			}
		}

		~DSACryptoServiceProvider()
		{
			Dispose(false);
		}

		public override DSAParameters ExportParameters(bool includePrivateParameters)
		{
			if (includePrivateParameters && !privateKeyExportable)
			{
				throw new CryptographicException(Locale.GetText("Cannot export private key"));
			}
			return dsa.ExportParameters(includePrivateParameters);
		}

		public override void ImportParameters(DSAParameters parameters)
		{
			dsa.ImportParameters(parameters);
		}

		public override byte[] CreateSignature(byte[] rgbHash)
		{
			return dsa.CreateSignature(rgbHash);
		}

		public byte[] SignData(byte[] buffer)
		{
			HashAlgorithm hashAlgorithm = SHA1.Create();
			byte[] rgbHash = hashAlgorithm.ComputeHash(buffer);
			return dsa.CreateSignature(rgbHash);
		}

		public byte[] SignData(byte[] buffer, int offset, int count)
		{
			HashAlgorithm hashAlgorithm = SHA1.Create();
			byte[] rgbHash = hashAlgorithm.ComputeHash(buffer, offset, count);
			return dsa.CreateSignature(rgbHash);
		}

		public byte[] SignData(Stream inputStream)
		{
			HashAlgorithm hashAlgorithm = SHA1.Create();
			byte[] rgbHash = hashAlgorithm.ComputeHash(inputStream);
			return dsa.CreateSignature(rgbHash);
		}

		public byte[] SignHash(byte[] rgbHash, string str)
		{
			if (string.Compare(str, "SHA1", true, CultureInfo.InvariantCulture) != 0)
			{
				throw new CryptographicException(Locale.GetText("Only SHA1 is supported."));
			}
			return dsa.CreateSignature(rgbHash);
		}

		public bool VerifyData(byte[] rgbData, byte[] rgbSignature)
		{
			HashAlgorithm hashAlgorithm = SHA1.Create();
			byte[] rgbHash = hashAlgorithm.ComputeHash(rgbData);
			return dsa.VerifySignature(rgbHash, rgbSignature);
		}

		public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
		{
			if (str == null)
			{
				str = "SHA1";
			}
			if (string.Compare(str, "SHA1", true, CultureInfo.InvariantCulture) != 0)
			{
				throw new CryptographicException(Locale.GetText("Only SHA1 is supported."));
			}
			return dsa.VerifySignature(rgbHash, rgbSignature);
		}

		public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
		{
			return dsa.VerifySignature(rgbHash, rgbSignature);
		}

		protected override void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (persisted && !persistKey)
				{
					store.Remove();
				}
				if (dsa != null)
				{
					dsa.Clear();
				}
				m_disposed = true;
			}
		}

		private void OnKeyGenerated(object sender, EventArgs e)
		{
			if (persistKey && !persisted)
			{
				store.KeyValue = ToXmlString(!dsa.PublicOnly);
				store.Save();
				persisted = true;
			}
		}

		[ComVisible(false)]
		public byte[] ExportCspBlob(bool includePrivateParameters)
		{
			byte[] array = null;
			if (includePrivateParameters)
			{
				return CryptoConvert.ToCapiPrivateKeyBlob(this);
			}
			return CryptoConvert.ToCapiPublicKeyBlob(this);
		}

		[ComVisible(false)]
		public void ImportCspBlob(byte[] keyBlob)
		{
			if (keyBlob == null)
			{
				throw new ArgumentNullException("keyBlob");
			}
			DSA dSA = CryptoConvert.FromCapiKeyBlobDSA(keyBlob);
			if (dSA is DSACryptoServiceProvider)
			{
				DSAParameters parameters = dSA.ExportParameters(!(dSA as DSACryptoServiceProvider).PublicOnly);
				ImportParameters(parameters);
				return;
			}
			try
			{
				DSAParameters parameters2 = dSA.ExportParameters(true);
				ImportParameters(parameters2);
			}
			catch
			{
				DSAParameters parameters3 = dSA.ExportParameters(false);
				ImportParameters(parameters3);
			}
		}
	}
}
