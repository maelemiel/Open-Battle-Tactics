using System;
using System.Security.Cryptography;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls
{
	internal abstract class CipherSuite
	{
		public static byte[] EmptyArray = new byte[0];

		private short code;

		private string name;

		private CipherAlgorithmType cipherAlgorithmType;

		private HashAlgorithmType hashAlgorithmType;

		private ExchangeAlgorithmType exchangeAlgorithmType;

		private bool isExportable;

		private CipherMode cipherMode;

		private byte keyMaterialSize;

		private int keyBlockSize;

		private byte expandedKeyMaterialSize;

		private short effectiveKeyBits;

		private byte ivSize;

		private byte blockSize;

		private Context context;

		private SymmetricAlgorithm encryptionAlgorithm;

		private ICryptoTransform encryptionCipher;

		private SymmetricAlgorithm decryptionAlgorithm;

		private ICryptoTransform decryptionCipher;

		private KeyedHashAlgorithm clientHMAC;

		private KeyedHashAlgorithm serverHMAC;

		protected ICryptoTransform EncryptionCipher
		{
			get
			{
				return encryptionCipher;
			}
		}

		protected ICryptoTransform DecryptionCipher
		{
			get
			{
				return decryptionCipher;
			}
		}

		protected KeyedHashAlgorithm ClientHMAC
		{
			get
			{
				return clientHMAC;
			}
		}

		protected KeyedHashAlgorithm ServerHMAC
		{
			get
			{
				return serverHMAC;
			}
		}

		public CipherAlgorithmType CipherAlgorithmType
		{
			get
			{
				return cipherAlgorithmType;
			}
		}

		public string HashAlgorithmName
		{
			get
			{
				switch (hashAlgorithmType)
				{
				case HashAlgorithmType.Md5:
					return "MD5";
				case HashAlgorithmType.Sha1:
					return "SHA1";
				default:
					return "None";
				}
			}
		}

		public HashAlgorithmType HashAlgorithmType
		{
			get
			{
				return hashAlgorithmType;
			}
		}

		public int HashSize
		{
			get
			{
				switch (hashAlgorithmType)
				{
				case HashAlgorithmType.Md5:
					return 16;
				case HashAlgorithmType.Sha1:
					return 20;
				default:
					return 0;
				}
			}
		}

		public ExchangeAlgorithmType ExchangeAlgorithmType
		{
			get
			{
				return exchangeAlgorithmType;
			}
		}

		public CipherMode CipherMode
		{
			get
			{
				return cipherMode;
			}
		}

		public short Code
		{
			get
			{
				return code;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public bool IsExportable
		{
			get
			{
				return isExportable;
			}
		}

		public byte KeyMaterialSize
		{
			get
			{
				return keyMaterialSize;
			}
		}

		public int KeyBlockSize
		{
			get
			{
				return keyBlockSize;
			}
		}

		public byte ExpandedKeyMaterialSize
		{
			get
			{
				return expandedKeyMaterialSize;
			}
		}

		public short EffectiveKeyBits
		{
			get
			{
				return effectiveKeyBits;
			}
		}

		public byte IvSize
		{
			get
			{
				return ivSize;
			}
		}

		public Context Context
		{
			get
			{
				return context;
			}
			set
			{
				context = value;
			}
		}

		public CipherSuite(short code, string name, CipherAlgorithmType cipherAlgorithmType, HashAlgorithmType hashAlgorithmType, ExchangeAlgorithmType exchangeAlgorithmType, bool exportable, bool blockMode, byte keyMaterialSize, byte expandedKeyMaterialSize, short effectiveKeyBits, byte ivSize, byte blockSize)
		{
			this.code = code;
			this.name = name;
			this.cipherAlgorithmType = cipherAlgorithmType;
			this.hashAlgorithmType = hashAlgorithmType;
			this.exchangeAlgorithmType = exchangeAlgorithmType;
			isExportable = exportable;
			if (blockMode)
			{
				cipherMode = CipherMode.CBC;
			}
			this.keyMaterialSize = keyMaterialSize;
			this.expandedKeyMaterialSize = expandedKeyMaterialSize;
			this.effectiveKeyBits = effectiveKeyBits;
			this.ivSize = ivSize;
			this.blockSize = blockSize;
			keyBlockSize = this.keyMaterialSize + HashSize + this.ivSize << 1;
		}

		internal void Write(byte[] array, int offset, short value)
		{
			if (offset > array.Length - 2)
			{
				throw new ArgumentException("offset");
			}
			array[offset] = (byte)(value >> 8);
			array[offset + 1] = (byte)value;
		}

		internal void Write(byte[] array, int offset, ulong value)
		{
			if (offset > array.Length - 8)
			{
				throw new ArgumentException("offset");
			}
			array[offset] = (byte)(value >> 56);
			array[offset + 1] = (byte)(value >> 48);
			array[offset + 2] = (byte)(value >> 40);
			array[offset + 3] = (byte)(value >> 32);
			array[offset + 4] = (byte)(value >> 24);
			array[offset + 5] = (byte)(value >> 16);
			array[offset + 6] = (byte)(value >> 8);
			array[offset + 7] = (byte)value;
		}

		public void InitializeCipher()
		{
			createEncryptionCipher();
			createDecryptionCipher();
		}

		public byte[] EncryptRecord(byte[] fragment, byte[] mac)
		{
			int num = fragment.Length + mac.Length;
			int num2 = 0;
			if (CipherMode == CipherMode.CBC)
			{
				num++;
				num2 = blockSize - num % blockSize;
				if (num2 == blockSize)
				{
					num2 = 0;
				}
				num += num2;
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(fragment, 0, array, 0, fragment.Length);
			Buffer.BlockCopy(mac, 0, array, fragment.Length, mac.Length);
			if (num2 > 0)
			{
				int num3 = fragment.Length + mac.Length;
				for (int i = num3; i < num3 + num2 + 1; i++)
				{
					array[i] = (byte)num2;
				}
			}
			EncryptionCipher.TransformBlock(array, 0, array.Length, array, 0);
			return array;
		}

		public void DecryptRecord(byte[] fragment, out byte[] dcrFragment, out byte[] dcrMAC)
		{
			int num = 0;
			int num2 = 0;
			DecryptionCipher.TransformBlock(fragment, 0, fragment.Length, fragment, 0);
			if (CipherMode == CipherMode.CBC)
			{
				num2 = fragment[fragment.Length - 1];
				num = fragment.Length - (num2 + 1) - HashSize;
			}
			else
			{
				num = fragment.Length - HashSize;
			}
			dcrFragment = new byte[num];
			dcrMAC = new byte[HashSize];
			Buffer.BlockCopy(fragment, 0, dcrFragment, 0, dcrFragment.Length);
			Buffer.BlockCopy(fragment, dcrFragment.Length, dcrMAC, 0, dcrMAC.Length);
		}

		public abstract byte[] ComputeClientRecordMAC(ContentType contentType, byte[] fragment);

		public abstract byte[] ComputeServerRecordMAC(ContentType contentType, byte[] fragment);

		public abstract void ComputeMasterSecret(byte[] preMasterSecret);

		public abstract void ComputeKeys();

		public byte[] CreatePremasterSecret()
		{
			ClientContext clientContext = (ClientContext)context;
			byte[] secureRandomBytes = context.GetSecureRandomBytes(48);
			secureRandomBytes[0] = (byte)(clientContext.ClientHelloProtocol >> 8);
			secureRandomBytes[1] = (byte)clientContext.ClientHelloProtocol;
			return secureRandomBytes;
		}

		public byte[] PRF(byte[] secret, string label, byte[] data, int length)
		{
			int num = secret.Length >> 1;
			if ((secret.Length & 1) == 1)
			{
				num++;
			}
			TlsStream tlsStream = new TlsStream();
			tlsStream.Write(Encoding.ASCII.GetBytes(label));
			tlsStream.Write(data);
			byte[] seed = tlsStream.ToArray();
			tlsStream.Reset();
			byte[] array = new byte[num];
			Buffer.BlockCopy(secret, 0, array, 0, num);
			byte[] array2 = new byte[num];
			Buffer.BlockCopy(secret, secret.Length - num, array2, 0, num);
			byte[] array3 = Expand("MD5", array, seed, length);
			byte[] array4 = Expand("SHA1", array2, seed, length);
			byte[] array5 = new byte[length];
			for (int i = 0; i < array5.Length; i++)
			{
				array5[i] = (byte)(array3[i] ^ array4[i]);
			}
			return array5;
		}

		public byte[] Expand(string hashName, byte[] secret, byte[] seed, int length)
		{
			int num = ((!(hashName == "MD5")) ? 20 : 16);
			int num2 = length / num;
			if (length % num > 0)
			{
				num2++;
			}
			Mono.Security.Cryptography.HMAC hMAC = new Mono.Security.Cryptography.HMAC(hashName, secret);
			TlsStream tlsStream = new TlsStream();
			byte[][] array = new byte[num2 + 1][];
			array[0] = seed;
			for (int i = 1; i <= num2; i++)
			{
				TlsStream tlsStream2 = new TlsStream();
				hMAC.TransformFinalBlock(array[i - 1], 0, array[i - 1].Length);
				array[i] = hMAC.Hash;
				tlsStream2.Write(array[i]);
				tlsStream2.Write(seed);
				hMAC.TransformFinalBlock(tlsStream2.ToArray(), 0, (int)tlsStream2.Length);
				tlsStream.Write(hMAC.Hash);
				tlsStream2.Reset();
			}
			byte[] array2 = new byte[length];
			Buffer.BlockCopy(tlsStream.ToArray(), 0, array2, 0, array2.Length);
			tlsStream.Reset();
			return array2;
		}

		private void createEncryptionCipher()
		{
			switch (cipherAlgorithmType)
			{
			case CipherAlgorithmType.Des:
				encryptionAlgorithm = DES.Create();
				break;
			case CipherAlgorithmType.Rc2:
				encryptionAlgorithm = RC2.Create();
				break;
			case CipherAlgorithmType.Rc4:
				encryptionAlgorithm = new ARC4Managed();
				break;
			case CipherAlgorithmType.TripleDes:
				encryptionAlgorithm = TripleDES.Create();
				break;
			case CipherAlgorithmType.Rijndael:
				encryptionAlgorithm = Rijndael.Create();
				break;
			}
			if (cipherMode == CipherMode.CBC)
			{
				encryptionAlgorithm.Mode = cipherMode;
				encryptionAlgorithm.Padding = PaddingMode.None;
				encryptionAlgorithm.KeySize = expandedKeyMaterialSize * 8;
				encryptionAlgorithm.BlockSize = blockSize * 8;
			}
			if (context is ClientContext)
			{
				encryptionAlgorithm.Key = context.ClientWriteKey;
				encryptionAlgorithm.IV = context.ClientWriteIV;
			}
			else
			{
				encryptionAlgorithm.Key = context.ServerWriteKey;
				encryptionAlgorithm.IV = context.ServerWriteIV;
			}
			encryptionCipher = encryptionAlgorithm.CreateEncryptor();
			if (context is ClientContext)
			{
				clientHMAC = new Mono.Security.Cryptography.HMAC(HashAlgorithmName, context.Negotiating.ClientWriteMAC);
			}
			else
			{
				serverHMAC = new Mono.Security.Cryptography.HMAC(HashAlgorithmName, context.Negotiating.ServerWriteMAC);
			}
		}

		private void createDecryptionCipher()
		{
			switch (cipherAlgorithmType)
			{
			case CipherAlgorithmType.Des:
				decryptionAlgorithm = DES.Create();
				break;
			case CipherAlgorithmType.Rc2:
				decryptionAlgorithm = RC2.Create();
				break;
			case CipherAlgorithmType.Rc4:
				decryptionAlgorithm = new ARC4Managed();
				break;
			case CipherAlgorithmType.TripleDes:
				decryptionAlgorithm = TripleDES.Create();
				break;
			case CipherAlgorithmType.Rijndael:
				decryptionAlgorithm = Rijndael.Create();
				break;
			}
			if (cipherMode == CipherMode.CBC)
			{
				decryptionAlgorithm.Mode = cipherMode;
				decryptionAlgorithm.Padding = PaddingMode.None;
				decryptionAlgorithm.KeySize = expandedKeyMaterialSize * 8;
				decryptionAlgorithm.BlockSize = blockSize * 8;
			}
			if (context is ClientContext)
			{
				decryptionAlgorithm.Key = context.ServerWriteKey;
				decryptionAlgorithm.IV = context.ServerWriteIV;
			}
			else
			{
				decryptionAlgorithm.Key = context.ClientWriteKey;
				decryptionAlgorithm.IV = context.ClientWriteIV;
			}
			decryptionCipher = decryptionAlgorithm.CreateDecryptor();
			if (context is ClientContext)
			{
				serverHMAC = new Mono.Security.Cryptography.HMAC(HashAlgorithmName, context.Negotiating.ServerWriteMAC);
			}
			else
			{
				clientHMAC = new Mono.Security.Cryptography.HMAC(HashAlgorithmName, context.Negotiating.ClientWriteMAC);
			}
		}
	}
}
