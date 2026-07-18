using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class SymmetricAlgorithm : IDisposable
	{
		protected int BlockSizeValue;

		protected byte[] IVValue;

		protected int KeySizeValue;

		protected byte[] KeyValue;

		protected KeySizes[] LegalBlockSizesValue;

		protected KeySizes[] LegalKeySizesValue;

		protected int FeedbackSizeValue;

		protected CipherMode ModeValue;

		protected PaddingMode PaddingValue;

		private bool m_disposed;

		public virtual int BlockSize
		{
			get
			{
				return BlockSizeValue;
			}
			set
			{
				if (!KeySizes.IsLegalKeySize(LegalBlockSizesValue, value))
				{
					throw new CryptographicException(Locale.GetText("block size not supported by algorithm"));
				}
				if (BlockSizeValue != value)
				{
					BlockSizeValue = value;
					IVValue = null;
				}
			}
		}

		public virtual int FeedbackSize
		{
			get
			{
				return FeedbackSizeValue;
			}
			set
			{
				if (value <= 0 || value > BlockSizeValue)
				{
					throw new CryptographicException(Locale.GetText("feedback size larger than block size"));
				}
				FeedbackSizeValue = value;
			}
		}

		public virtual byte[] IV
		{
			get
			{
				if (IVValue == null)
				{
					GenerateIV();
				}
				return (byte[])IVValue.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("IV");
				}
				if (value.Length << 3 != BlockSizeValue)
				{
					throw new CryptographicException(Locale.GetText("IV length is different than block size"));
				}
				IVValue = (byte[])value.Clone();
			}
		}

		public virtual byte[] Key
		{
			get
			{
				if (KeyValue == null)
				{
					GenerateKey();
				}
				return (byte[])KeyValue.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Key");
				}
				int num = value.Length << 3;
				if (!KeySizes.IsLegalKeySize(LegalKeySizesValue, num))
				{
					throw new CryptographicException(Locale.GetText("Key size not supported by algorithm"));
				}
				KeySizeValue = num;
				KeyValue = (byte[])value.Clone();
			}
		}

		public virtual int KeySize
		{
			get
			{
				return KeySizeValue;
			}
			set
			{
				if (!KeySizes.IsLegalKeySize(LegalKeySizesValue, value))
				{
					throw new CryptographicException(Locale.GetText("Key size not supported by algorithm"));
				}
				KeySizeValue = value;
				KeyValue = null;
			}
		}

		public virtual KeySizes[] LegalBlockSizes
		{
			get
			{
				return LegalBlockSizesValue;
			}
		}

		public virtual KeySizes[] LegalKeySizes
		{
			get
			{
				return LegalKeySizesValue;
			}
		}

		public virtual CipherMode Mode
		{
			get
			{
				return ModeValue;
			}
			set
			{
				if (!Enum.IsDefined(ModeValue.GetType(), value))
				{
					throw new CryptographicException(Locale.GetText("Cipher mode not available"));
				}
				ModeValue = value;
			}
		}

		public virtual PaddingMode Padding
		{
			get
			{
				return PaddingValue;
			}
			set
			{
				if (!Enum.IsDefined(PaddingValue.GetType(), value))
				{
					throw new CryptographicException(Locale.GetText("Padding mode not available"));
				}
				PaddingValue = value;
			}
		}

		protected SymmetricAlgorithm()
		{
			ModeValue = CipherMode.CBC;
			PaddingValue = PaddingMode.PKCS7;
			m_disposed = false;
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SymmetricAlgorithm()
		{
			Dispose(false);
		}

		public void Clear()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (KeyValue != null)
				{
					Array.Clear(KeyValue, 0, KeyValue.Length);
					KeyValue = null;
				}
				if (disposing)
				{
				}
				m_disposed = true;
			}
		}

		public virtual ICryptoTransform CreateDecryptor()
		{
			return CreateDecryptor(Key, IV);
		}

		public abstract ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV);

		public virtual ICryptoTransform CreateEncryptor()
		{
			return CreateEncryptor(Key, IV);
		}

		public abstract ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV);

		public abstract void GenerateIV();

		public abstract void GenerateKey();

		public bool ValidKeySize(int bitLength)
		{
			return KeySizes.IsLegalKeySize(LegalKeySizesValue, bitLength);
		}

		public static SymmetricAlgorithm Create()
		{
			return Create("System.Security.Cryptography.SymmetricAlgorithm");
		}

		public static SymmetricAlgorithm Create(string algName)
		{
			return (SymmetricAlgorithm)CryptoConfig.CreateFromName(algName);
		}
	}
}
